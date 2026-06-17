# Ejemplo 02 — Configurar el proveedor activo y gestionar archivos con transparencia

**Proyecto:** geovial-storage
**Documento:** ejemplo-02-intermedio_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-15
**Autor:** Developer Advocate / Sample Engineer Senior
**Nivel:** Intermedio
**Ubicación del código:** `/samples/geovial-storage/02-intermedio-con-extensiones/`

## 1. Objetivo del sample

Demostrar el ciclo de gestión completo de un relevamiento sobre la abstracción de almacenamiento y la transparencia entre proveedores: seleccionar y validar el proveedor activo, eliminar, verificar existencia y listar bajo un prefijo, y luego reejecutar el mismo flujo cambiando el proveedor activo sin tocar el código de consumo. Al finalizar, el desarrollador sabe configurar el proveedor activo con una validación en seco, opera las cuatro operaciones de gestión y comprueba que el contrato es idéntico cualquiera sea el proveedor.

## 2. Nivel

Intermedio. Asume completado el sample 01 (guardar y recuperar con el proveedor local). Agrega la configuración y selección del proveedor activo (CU-06), las operaciones de gestión que el básico no mostraba (eliminar, verificar, listar) y, sobre todo, la propiedad central de la librería: la transparencia al cambiar de proveedor (RN-01). No implementa un proveedor nuevo; eso queda para el sample 03.

## 3. Prerequisites

| Prerequisito | Versión mínima / cómo obtenerlo |
| --- | --- |
| Runtime del consumidor | El declarado para la librería en intake §17.P.9. |
| Gestor de paquetes del ecosistema | El del runtime anterior, para restaurar las dependencias del sample. |
| Una ubicación local accesible y escribible | Cualquier carpeta con permisos de lectura y escritura; la usa el proveedor local. |
| Parámetros y credenciales de un proveedor remoto (sintéticos) | Solo para el tramo de cambio de proveedor; pueden ser los de un doble de conformidad o un destino remoto efímero. Nunca credenciales productivas (RN-03). |

El sample arranca usando solo el proveedor local; el cambio a un proveedor remoto aparece en el último tramo y, si no se dispone de credenciales remotas, ese tramo se demuestra contra un segundo destino local o un doble que cumple el puerto.

## 4. Cómo correrlo

1. Clonar el repositorio y entrar a la carpeta del sample: `cd samples/geovial-storage/02-intermedio-con-extensiones`.
2. Restaurar las dependencias con el gestor de paquetes del ecosistema.
3. Ejecutar el comando de arranque con el proveedor local, que guarda tres archivos de un relevamiento, los lista, verifica uno y elimina otro.
4. Ejecutar el comando de cambio de proveedor con la validación en seco y luego la activación del proveedor remoto (o del segundo destino).
5. Reejecutar el mismo flujo del paso 3 contra el nuevo proveedor activo y comparar ambas salidas con el output esperado de la sección 6.

## 5. Estructura del código

```
02-intermedio-con-extensiones/
├── README.md                       # Resumen del sample y enlace a este markdown
├── src/
│   ├── Program.<ext>               # Orquesta los tramos: gestion y cambio de proveedor
│   ├── FlujoRelevamiento.<ext>     # Guardar, listar, verificar y eliminar bajo prefijo
│   ├── config/local.<ext>          # Parametros del proveedor local
│   └── config/remoto.<ext>         # Parametros y credenciales sinteticos del remoto
└── tests/
    ├── gestion-test.<ext>          # Verifica eliminar, verificar y listar
    └── transparencia-test.<ext>    # Verifica resultados equivalentes entre proveedores
```

## 6. Qué esperar

Salida esperada al ejecutar el flujo con el proveedor local y luego con el remoto:

```
== Proveedor activo: local ==
Guardados 3 archivos bajo prefijo: relevamientos/2026/r-001/
Listado    -> 3 identificadores  testigo de continuacion: no
Verificar  -> relevamientos/2026/r-001/foto-01.jpg  presente: si  tamanio: 245 KB
Eliminar   -> relevamientos/2026/r-001/foto-02.jpg  eliminado: si
Verificar  -> relevamientos/2026/r-001/foto-02.jpg  presente: no

== Validacion en seco del proveedor remoto ==
Validacion -> soporte: si  formato: si  conectividad: si  proveedor activo: NO cambia

== Proveedor activo: remoto ==
Guardados 3 archivos bajo prefijo: relevamientos/2026/r-001/
Listado    -> 3 identificadores  testigo de continuacion: no
Verificar  -> relevamientos/2026/r-001/foto-01.jpg  presente: si  tamanio: 245 KB
Eliminar   -> relevamientos/2026/r-001/foto-02.jpg  eliminado: si
Verificar  -> relevamientos/2026/r-001/foto-02.jpg  presente: no

Resultado: mismo comportamiento observable con local y con remoto; 0 ramas por proveedor
```

El criterio de éxito es la última línea: las dos corridas producen resultados equivalentes y el código de consumo no cambia al cambiar de proveedor (RN-01). La validación en seco confirma la configuración sin alterar el proveedor activo (CU-06, FA-02).

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado |
| --- | --- | --- |
| Credenciales remotas con formato inválido | Entregar parámetros mal formados en la validación en seco | La librería devuelve `CREDENCIALES_INVALIDAS` sin intentar conectividad y conserva el proveedor activo anterior (CU-06, CA-02) |
| Eliminar un identificador inexistente | Pedir eliminar un identificador que no existe | La librería informa éxito por idempotencia, sin error (CU-03, CA-02) |
| Listado con paginación | Guardar diez archivos y listar con tamaño de página 4 | La librería devuelve 4 identificadores y un testigo de continuación no vacío (CU-05, CA-02) |
| Cambiar el proveedor sin alcance de raíz | Invocar el cambio de proveedor con un actor sin alcance de usuario raíz | La librería rechaza con `AUTORIZACION_INSUFICIENTE` y no modifica el proveedor activo (CU-06, CA-04); puente hacia el sample 03 |

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| CU-03 | Caso de uso | Elimina un archivo bajo un prefijo y demuestra la idempotencia sobre un inexistente |
| CU-04 | Caso de uso | Verifica la presencia de un identificador antes y después de eliminarlo, con coherencia de estado |
| CU-05 | Caso de uso | Lista los identificadores bajo el prefijo del relevamiento, con testigo de continuación cuando aplica |
| CU-06 | Caso de uso | Valida en seco y activa el proveedor remoto; las operaciones siguientes lo usan sin cambiar su invocación |
| RN-01 | Regla de negocio | Verifica que el contrato es idéntico cualquiera sea el proveedor activo, sin ramas por proveedor |
| RN-03 | Regla de negocio | Usa credenciales sintéticas; la confirmación de activación no revela las credenciales recibidas |
| ADR-01 | Decisión arquitectónica | Materializa la selección del proveedor activo por estrategia, resuelta desde la configuración |
| ADR-04 | Decisión arquitectónica | Materializa la normalización a un comportamiento observable único entre proveedores |
| NFR-03 | Requisito no funcional | El test de transparencia comprueba 0 diferencias de comportamiento observable y 0 ramas por proveedor |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Versión inicial del sample intermedio: configuración y selección del proveedor activo con validación en seco, eliminación, verificación y listado, y transparencia verificada entre proveedores (CU-03, CU-04, CU-05, CU-06, RN-01, NFR-03). |
