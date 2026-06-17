# Referencia de API — geovial-storage

**Proyecto:** geovial-storage
**Documento:** referencia-api_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Technical Writer + SDK Documentation Lead
**Tipo Diátaxis:** Reference
**Audiencia:** Developer integrador del backend que consume la abstracción de almacenamiento
**Nivel:** Medio
**Tiempo estimado de lectura:** 14 min

## 0. Alcance y paridad

Esta referencia describe la superficie pública estable que `geovial-storage` expone al consumidor: dos interfaces (almacenamiento y configuración del proveedor) con seis operaciones, sus esquemas de datos lógicos y su catálogo de errores. Mantiene paridad uno a uno con `contratos-abstractions_v1.0.md` (05); cuando esos documentos cambian, esta referencia se actualiza al mismo ritmo. Las firmas de tipo físicas y los nombres de proveedor pertenecen al stack y viven en intake/05; los ejemplos ejecutables viven en 11. Aquí las operaciones se describen de forma neutral de stack: operación, entradas, salidas y errores.

Las operaciones son asincrónicas (no bloqueantes), coherente con la vista de procesos de la arquitectura (05 §4). El vocabulario de cada parámetro está definido en `glosario-tecnico_v1.0.md`.

## 1. Tipos públicos

| Tipo lógico | Propósito | Forma | Invariante |
| --- | --- | --- | --- |
| identificador-logico | Referenciar un archivo sin conocer su ubicación física. | Cadena opaca. | Estable a través de versiones menores; lo emitido por guardar conserva su significado (ADR-03). |
| destino-logico | Agrupar los archivos de un relevamiento al guardar o como raíz del listado. | Prefijo o ruta lógica con formato admitido (lo fija 05/stack). | Debe cumplir el formato admitido; en caso contrario `DESTINO_INVALIDO`. |
| contenido | Transportar los bytes del archivo. | Secuencia de bytes. | No se transforma ni recodifica en ningún punto (RN-02). |
| metadatos | Describir un archivo sin transferir su binario. | Tipo de contenido y tamaño persistido. | El tamaño persistido coincide con el del contenido guardado. |
| resultado-de-listado | Devolver una página de identificadores bajo un prefijo. | Colección de identificadores más testigo de continuación opcional. | Garantiza cardinalidad y pertenencia bajo el prefijo; no garantiza el orden (CU-05). |
| configuracion-de-proveedor | Seleccionar y parametrizar el proveedor activo. | Proveedor seleccionado más parámetros y credenciales. | Las credenciales entran pero no salen por ninguna vía pública (RN-03; ADR-05). |

## 2. Interfaz de almacenamiento (operaciones de datos)

Dirigida al consumidor. Cubre CU-01 a CU-05. Todas las operaciones pueden devolver `PROVEEDOR_NO_DISPONIBLE` (fallo transitorio del proveedor activo) y `PROVEEDOR_NO_CONFIGURADO` (no hay proveedor activo) además de los errores propios de cada operación.

### 2.1 Guardar (CU-01)

| Aspecto | Detalle |
| --- | --- |
| Propósito | Persistir un contenido y devolver un identificador lógico estable. |
| Parámetros | `contenido` (secuencia de bytes, obligatorio, no vacío); `destino` (prefijo o identificador lógico explícito, obligatorio); `tipo-de-contenido` (cadena, obligatorio); `sobrescritura` (booleano, opcional, default `falso`). |
| Retorno | `identificador-logico` y `tamaño-persistido`. |
| Excepciones | `CONTENIDO_VACIO`, `DESTINO_INVALIDO`, `IDENTIFICADOR_DUPLICADO`, `TAMANIO_EXCEDIDO`, `PROVEEDOR_NO_DISPONIBLE`, `PROVEEDOR_NO_CONFIGURADO`. |
| Notas | Si `destino` es un identificador explícito que ya existe y `sobrescritura` es `falso`, se rechaza con `IDENTIFICADOR_DUPLICADO` (CU-01, FA-01). Con `sobrescritura` en `verdadero`, reemplaza el contenido conservando el identificador (FA-02). El tamaño se valida contra el máximo configurado (default 25 MB) antes de delegar. |

### 2.2 Recuperar (CU-02)

| Aspecto | Detalle |
| --- | --- |
| Propósito | Devolver el contenido idénticamente igual al guardado, por identificador. |
| Parámetros | `identificador` (obligatorio); `rango-de-bytes` (par inicio-fin, opcional); `solo-metadatos` (booleano, opcional, default `falso`). |
| Retorno | `contenido` y `metadatos` (tipo, tamaño); o solo `metadatos` si `solo-metadatos` es `verdadero`; o el segmento solicitado si se pasó `rango-de-bytes`. |
| Excepciones | `IDENTIFICADOR_INEXISTENTE`, `DESTINO_INVALIDO`, `RANGO_INVALIDO`, `PROVEEDOR_NO_DISPONIBLE`, `PROVEEDOR_NO_CONFIGURADO`. |
| Notas | El contenido devuelto es idéntico byte a byte al guardado (RN-02). La recuperación por rango conserva la integridad del segmento (CU-02, FA-02). |

### 2.3 Eliminar (CU-03)

| Aspecto | Detalle |
| --- | --- |
| Propósito | Quitar el contenido asociado a un identificador, o a todos los identificadores bajo un prefijo. |
| Parámetros | `identificador` (obligatorio) o `prefijo` para eliminación múltiple (alternativo). |
| Retorno | Confirmación; en eliminación múltiple, `cantidad-eliminada`. |
| Excepciones | `DESTINO_INVALIDO`, `ELIMINACION_PARCIAL`, `PROVEEDOR_NO_DISPONIBLE`, `PROVEEDOR_NO_CONFIGURADO`. |
| Notas | Eliminar un identificador inexistente se trata como éxito por idempotencia, sin error (CU-03, FA-01). En eliminación múltiple, `ELIMINACION_PARCIAL` informa los identificadores no eliminados para reintentar (FA-02). |

### 2.4 Verificar existencia (CU-04)

| Aspecto | Detalle |
| --- | --- |
| Propósito | Informar si un identificador corresponde a un archivo presente, sin transferir el binario. |
| Parámetros | `identificador` (obligatorio); `devolver-metadatos` (booleano, opcional, default `falso`). |
| Retorno | `presencia` (booleano); si está presente, `tamaño` y, con `devolver-metadatos`, los `metadatos`. |
| Excepciones | `DESTINO_INVALIDO`, `PROVEEDOR_NO_DISPONIBLE`, `PROVEEDOR_NO_CONFIGURADO`. |
| Notas | La presencia es coherente con el estado real: un identificador recuperable se reporta presente; uno eliminado se reporta ausente (RN-02). Un fallo transitorio devuelve error, nunca un valor de presencia ambiguo. |

### 2.5 Listar bajo prefijo (CU-05)

| Aspecto | Detalle |
| --- | --- |
| Propósito | Enumerar los identificadores presentes bajo un prefijo, con paginación. |
| Parámetros | `prefijo` (obligatorio); `tamaño-de-pagina` (entero, opcional); `testigo-de-continuacion` (opcional). |
| Retorno | `resultado-de-listado`: lista de identificadores y, si la enumeración no se completó, un `testigo-de-continuacion`. |
| Excepciones | `DESTINO_INVALIDO`, `TESTIGO_INVALIDO`, `PROVEEDOR_NO_DISPONIBLE`, `PROVEEDOR_NO_CONFIGURADO`. |
| Notas | Un prefijo sin coincidencias devuelve lista vacía sin error (CU-05, FA-02). El orden no está garantizado; se garantiza cardinalidad y pertenencia. Un testigo vencido o alterado devuelve `TESTIGO_INVALIDO` y obliga a reiniciar el listado. |

## 3. Interfaz de configuración del proveedor activo (CU-06)

Dirigida al usuario raíz a través del consumidor. Cambiar el proveedor activo requiere alcance de usuario raíz; cualquier otro actor recibe `AUTORIZACION_INSUFICIENTE`.

### 3.1 Configurar proveedor activo (CU-06)

| Aspecto | Detalle |
| --- | --- |
| Propósito | Seleccionar y validar el proveedor activo y sus credenciales. |
| Parámetros | `proveedor` (identificador del proveedor a activar, obligatorio); `parametros-y-credenciales` (obligatorio según proveedor; el proveedor local no requiere credenciales remotas); `validacion-en-seco` (booleano, opcional, default `falso`). |
| Retorno | Confirmación de activación sin revelar las credenciales; o, con `validacion-en-seco`, el resultado de validación sin cambiar el proveedor activo. |
| Excepciones | `PROVEEDOR_NO_SOPORTADO`, `CREDENCIALES_INVALIDAS`, `PROVEEDOR_INACCESIBLE`, `AUTORIZACION_INSUFICIENTE`. |
| Notas | El proveedor local omite la validación de credenciales remotas y comprueba que la ubicación local sea accesible y escribible (CU-06, FA-01). La validación en seco ejecuta soporte, formato, conectividad y permisos sin fijar el proveedor (FA-02). Ante cualquier fallo, el proveedor activo anterior se conserva. La confirmación nunca repite ni refleja las credenciales recibidas (RN-03). |

## 4. Excepciones

El contrato expone un conjunto único de códigos, idéntico cualquiera sea el proveedor activo (RN-01). Los códigos son estables, en mayúsculas y sin tildes, independientes del idioma y del proveedor; el texto descriptivo es traducible sin cambiar el código. Ningún error incluye credenciales ni parámetros de conexión (RN-03). El catálogo completo con mensaje, causa y acción sugerida vive en `dx-error-messages_v1.0.md` (03) y se reusa en `troubleshooting_v1.0.md`.

| Código | Categoría | Cuándo se lanza | Operaciones |
| --- | --- | --- | --- |
| `CONTENIDO_VACIO` | Entrada inválida | El contenido a guardar tiene tamaño cero; no se crea archivo. | Guardar |
| `DESTINO_INVALIDO` | Entrada inválida | El identificador o prefijo no cumple el formato admitido. | Guardar, recuperar, eliminar, verificar, listar |
| `RANGO_INVALIDO` | Entrada inválida | El rango de bytes excede el tamaño del archivo o está mal formado. | Recuperar |
| `TESTIGO_INVALIDO` | Entrada inválida | El testigo de continuación está vencido o mal formado. | Listar |
| `TAMANIO_EXCEDIDO` | Entrada inválida | El contenido supera el tamaño máximo configurado (default 25 MB); se rechaza sin contactar al proveedor. | Guardar |
| `CREDENCIALES_INVALIDAS` | Entrada inválida | Las credenciales o parámetros no tienen el formato requerido. | Configurar proveedor |
| `PROVEEDOR_NO_SOPORTADO` | Entrada inválida | El proveedor indicado no pertenece al conjunto soportado. | Configurar proveedor |
| `IDENTIFICADOR_INEXISTENTE` | Recurso ausente | El identificador no corresponde a ningún archivo del proveedor activo. | Recuperar |
| `PROVEEDOR_NO_CONFIGURADO` | Recurso ausente | Se invocó una operación de datos sin un proveedor activo configurado. | Guardar, recuperar, eliminar, verificar, listar |
| `IDENTIFICADOR_DUPLICADO` | Conflicto de estado | El identificador explícito ya existe y no se pidió sobrescritura. | Guardar |
| `ELIMINACION_PARCIAL` | Conflicto de estado | En eliminación múltiple, parte del conjunto no pudo eliminarse. | Eliminar |
| `PROVEEDOR_NO_DISPONIBLE` | Error transitorio | El proveedor activo no responde o rechaza la operación; sin filtrar configuración. | Guardar, recuperar, eliminar, verificar, listar |
| `PROVEEDOR_INACCESIBLE` | Error transitorio | La comprobación de conectividad o permisos al configurar falló; sin filtrar configuración. | Configurar proveedor |
| `AUTORIZACION_INSUFICIENTE` | Permiso insuficiente | Quien invoca no tiene alcance de usuario raíz para cambiar el proveedor activo. | Configurar proveedor |

## 5. Garantías de comportamiento

Verificadas por la batería de contrato única (ADR-04), idénticas para todo proveedor (RN-01):

- Igualdad binaria: lo recuperado es idénticamente igual a lo guardado mientras no se sobrescriba ni elimine (RN-02).
- Idempotencia: eliminar un identificador inexistente se trata como éxito (CU-03, FA-01).
- Listado: se garantiza cardinalidad y pertenencia bajo el prefijo; no se garantiza el orden (CU-05).
- Coherencia: un identificador recuperable se reporta presente y uno eliminado se reporta ausente (CU-04, RN-02).

## 6. Ejemplos breves por bloque

Descritos como secuencia de invocaciones y efecto esperado (sin stack); el código ejecutable equivalente vive en 11.

Bloque de datos — ida y vuelta con verificación:

```
1. guardar(contenido=foto, destino="relevamientos/2026/r-001/", tipo="image/jpeg")
   -> identificador="relevamientos/2026/r-001/foto-01.jpg", tamaño-persistido=245 KB
2. verificar(identificador)                 -> presencia=verdadero, tamaño=245 KB
3. recuperar(identificador)                 -> contenido idéntico al guardado (RN-02)
4. recuperar(identificador, rango=[0,1023]) -> exactamente 1024 bytes del inicio
5. eliminar(identificador)                  -> confirmación; verificar -> presencia=falso
```

Bloque de listado — paginación:

```
1. listar(prefijo="relevamientos/2026/r-002/", tamaño-de-pagina=4)
   -> [4 identificadores], testigo-de-continuacion="t1"
2. listar(prefijo="relevamientos/2026/r-002/", testigo-de-continuacion="t1")
   -> [identificadores restantes], sin testigo  (enumeración completa)
```

Bloque de configuración — validar en seco y luego activar (usuario raíz):

```
1. configurar(proveedor="remoto", parametros-y-credenciales=cfg, validacion-en-seco=verdadero)
   -> resultado de validación; el proveedor activo NO cambia
2. configurar(proveedor="remoto", parametros-y-credenciales=cfg)
   -> confirmación de activación (sin revelar credenciales)
   las operaciones de datos ya escritas siguen igual (RN-01)
```

## 7. Versionado del contrato

Gobernado por ADR-03 (SemVer 2.0.0). Agregar un proveedor, una operación o un parámetro opcional con default es versión menor (compatible). Cambiar la semántica de una operación, quitar una operación, quitar o renombrar un código de error, o volver obligatorio un parámetro antes opcional es versión mayor (incompatible) y obliga a coordinar con el consumidor. El identificador lógico emitido por guardar conserva su significado a través de versiones menores. Detalle en `contratos-abstractions_v1.0.md` §6 (05).

## 8. Referencias cruzadas

- 05 `contratos-abstractions_v1.0.md` §3, §4, §5, §6: contrato fuente del que esta referencia mantiene paridad.
- 05 `adrs/ADR-02-superficie-publica-estable_v1.0.md`: frontera público/interno de las dos interfaces.
- 05 `adrs/ADR-04-transparencia-limites-proveedor_v1.0.md`: garantías de comportamiento y tamaño máximo.
- 02 `casos-de-uso/`: CU-01 a CU-06, origen funcional y criterios de aceptación de cada operación.
- 03 `dx-error-messages_v1.0.md`: catálogo de errores con mensaje, causa y acción.
- 10 `glosario-tecnico_v1.0.md`: definición de cada tipo y término usado en las firmas.
- 10 `troubleshooting_v1.0.md`: diagnóstico paso a paso de los errores listados en §4.

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Referencia inicial de la superficie pública: tipos lógicos, interfaz de almacenamiento (guardar, recuperar, eliminar, verificar, listar), interfaz de configuración del proveedor (CU-06), catálogo de catorce excepciones, garantías de comportamiento, ejemplos breves por bloque y política de versionado. Paridad con `contratos-abstractions_v1.0.md` (05). |
