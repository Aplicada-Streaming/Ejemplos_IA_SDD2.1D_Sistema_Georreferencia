# Puertos y adaptadores (arquitectura hexagonal)

> Nota técnica del proyecto **GeoVial**. Usa como casos reales la abstracción de almacenamiento
> ([IObjectStore](../../src/GeoVial.Storage/Abstractions/IObjectStore.cs)) y el motor de
> sincronización offline ([Aplicada.Sync](../../src/Aplicada.Sync/)).

## 1. Resumen

La **arquitectura hexagonal** (o **puertos y adaptadores**, Alistair Cockburn, 2005) organiza el
software en un **núcleo** que contiene la lógica de negocio y un conjunto de **adaptadores** que lo
conectan con el mundo exterior (bases de datos, APIs, servicios de archivos, UI). El núcleo define
**puertos** (interfaces) y nunca depende de tecnologías concretas: son los adaptadores los que
dependen del núcleo. Resultado: la lógica es **testeable, intercambiable y estable** ante cambios de
infraestructura.

## 2. Fundamentos

### 2.1. El problema que resuelve

Si la lógica de negocio llama directamente al SDK de un proveedor, a un ORM o a un cliente HTTP,
queda **acoplada** a esa tecnología: cambiarla obliga a tocar el negocio, y probarlo exige levantar
la infraestructura. La hexagonal invierte esa dependencia.

### 2.2. Puerto

Un **puerto** es una **interfaz** que expresa una capacidad en términos del dominio, sin detalles de
implementación. Hay dos tipos:

- **Puertos de entrada (driving):** cómo el exterior usa al núcleo (p. ej. los servicios de
  aplicación que invocan los controladores).
- **Puertos de salida (driven):** lo que el núcleo necesita del exterior (p. ej. "guardar un
  archivo", "persistir", "enviar al backend").

### 2.3. Adaptador

Un **adaptador** es una implementación concreta de un puerto que habla una tecnología específica.
Para un mismo puerto puede haber varios adaptadores intercambiables.

### 2.4. Regla de dependencia

Las dependencias **apuntan hacia adentro**: infraestructura → aplicación → dominio. El dominio no
conoce a la infraestructura; ambos se encuentran en el puerto (una interfaz que pertenece al lado de
adentro). Esto es la **Inversión de Dependencias** (la "D" de SOLID) llevada a la arquitectura.

## 3. Cómo lo aplica GeoVial

### 3.1. Almacenamiento de archivos (caso canónico)

- **Puerto de salida:** [IObjectStore](../../src/GeoVial.Storage/Abstractions/IObjectStore.cs)
  (`SaveAsync`, `GetAsync`, `DeleteAsync`, `ExistsAsync`, `ListAsync`).
- **Adaptadores:**
  - [LocalObjectStore](../../src/GeoVial.Storage/Providers/Local/LocalObjectStore.cs) — disco.
  - [MemoryObjectStore](../../src/GeoVial.Storage/Providers/Memory/MemoryObjectStore.cs) — RAM.
  - [S3ObjectStore](../../src/GeoVial.Storage/Providers/S3/S3ObjectStore.cs) — S3/MinIO/R2.
  - [RouterObjectStore](../../src/GeoVial.Storage/Providers/RouterObjectStore.cs) — un adaptador que
    **enruta** a otro según el destino activo (patrón compuesto + estrategia).

El resto del backend (subir fotos, exportar relevamientos) depende solo de `IObjectStore`. Por eso se
pudo **agregar S3 y conmutarlo en caliente** (CU-17) sin tocar ninguna lógica de negocio, y probar la
captura de fotos con un almacén en memoria.

### 3.2. Sincronización offline (puertos en una librería)

El motor [MotorSincronizacion](../../src/Aplicada.Sync/MotorSincronizacion.cs) define **dos puertos
de salida** que la app móvil implementa:

- [IAlmacenLocal](../../src/Aplicada.Sync/Puertos.cs) — la cola local y la marca (lo materializará
  SQLite en el dispositivo).
- [IBackendSincronizacion](../../src/Aplicada.Sync/Puertos.cs) — el backend remoto (lo materializará
  un cliente HTTP contra geovial-api).

El motor no sabe de HTTP ni de SQLite: contiene **solo la política** (subir-luego-bajar, idempotencia,
reanudación), por eso se prueba con dobles en memoria. Ver
[sincronizacion-subir-luego-bajar.md](sincronizacion-subir-luego-bajar.md).

### 3.3. Otros puertos del backend

- `IServicioRelevamientos`, `IServicioFotos`, `IServicioSincronizacion`, `IServicioConflictos`,
  `IServicioPortabilidad`, `IServicioAlmacenamiento` (puertos de entrada del backend, usados por los
  controladores).
- `IRegistroAlmacenamiento` (puerto de salida para configurar el destino activo, CU-17).

### 3.4. Clean Architecture dentro del backend

`GeoVial.WebApi` ordena el código por capas con la misma regla de dependencia:

```
Controllers  ->  Application  ->  Domain
        \           |              ^
         \          v              |
          ------> Infrastructure ---
                (EF Core, JWT, IObjectStore concreto)
```

- **Domain:** entidades y reglas (Relevamiento, Marcador, transiciones RN-05...).
- **Application:** servicios + contratos (DTOs, puertos).
- **Infrastructure:** EF Core, hash de contraseñas, emisor de tokens, persistencia.
- **Controllers/Api:** adaptadores de entrada HTTP (REST).

## 4. Beneficios concretos observados

| Beneficio | Dónde se ve en GeoVial |
|---|---|
| **Intercambiar tecnología sin tocar el negocio** | Agregar S3 y conmutar destino (CU-17) |
| **Testeabilidad** | Mock de `IAmazonS3`; fake en memoria de `IObjectStore`; dobles de los puertos del motor de sync |
| **Aislar política de infraestructura** | `MotorSincronizacion` no depende de HTTP/SQLite |
| **Reemplazo en runtime** | `RouterObjectStore` conmuta el adaptador activo |

## 5. Buenas prácticas

- **El puerto pertenece al núcleo** y se expresa en su lenguaje (no "PutObject", sino "guardar un
  objeto"). El nombre y la forma los decide el lado de adentro.
- **Adaptadores delgados:** traducen y delegan; no meten lógica de negocio. (Ver
  [pruebas-de-adaptadores-con-mocks.md](pruebas-de-adaptadores-con-mocks.md).)
- **Errores del proveedor → errores del dominio:** el adaptador traduce (p. ej. un 404 de S3 a
  `ObjectNotFoundException`), para que el núcleo no vea excepciones específicas del proveedor.
- **Inyección de dependencias por constructor**, con la interfaz; el contenedor elige el adaptador
  concreto.
- **No filtrar tipos de la tecnología** a través del puerto (p. ej. el puerto usa `Stream`, no tipos
  del SDK de AWS).

### 5.1. Olores (anti-patrones)

- Puerto que es un **reflejo 1:1 del SDK** (entonces no abstrae nada).
- Lógica de negocio dentro del adaptador.
- El dominio referenciando paquetes de infraestructura.

## 6. Referencias

- Alistair Cockburn — *Hexagonal Architecture (Ports and Adapters)*.
- Robert C. Martin — *Clean Architecture* (regla de dependencia).
- Principios **SOLID**, en particular Inversión de Dependencias.

## 7. Control de cambios

| Versión | Fecha | Cambios |
|---|---|---|
| 1.0 | 2026-06-17 | Nota inicial sobre puertos y adaptadores aplicada a GeoVial (almacenamiento, sincronización, capas del backend). |
