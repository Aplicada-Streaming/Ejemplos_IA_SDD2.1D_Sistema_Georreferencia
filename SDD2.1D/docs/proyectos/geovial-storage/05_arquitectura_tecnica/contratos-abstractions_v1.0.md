# Contrato de Abstracciones — geovial-storage

**Proyecto:** geovial-storage
**Documento:** contratos-abstractions_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer

## 1. Alcance del contrato

Este documento define la superficie pública estable que `geovial-storage` expone a su consumidor `geovial-api`: la abstracción de almacenamiento transparente. Es el contrato de Abstractions que la regla 05 §2.2 obliga a documentar para un proyecto `library`. Materializa los seis casos de uso de la especificación funcional:

| CU | Operación del contrato | Audiencia |
| --- | --- | --- |
| CU-01 | Guardar un archivo | Consumidor (`geovial-api`) |
| CU-02 | Recuperar un archivo | Consumidor |
| CU-03 | Eliminar un archivo | Consumidor |
| CU-04 | Verificar la existencia de un archivo | Consumidor |
| CU-05 | Listar archivos bajo un prefijo | Consumidor |
| CU-06 | Configurar el proveedor de almacenamiento activo | Usuario raíz (a través del consumidor) |

El contrato es idéntico cualquiera sea el proveedor activo (RN-01) y se describe en términos de mecanismo abstracto: las firmas de tipo concretas y los nombres de proveedor pertenecen al stack y viven en intake §17; los ejemplos ejecutables viven en 11.

## 2. Formato

El contrato se expresa como dos interfaces de la capa de Abstracciones, descritas aquí de forma neutral de stack (operaciones, entradas, salidas y errores), no como un esquema de red. La librería no tiene contrato de red propio: se invoca en proceso desde el backend (intake §17.P.3). Las interfaces son:

- Interfaz de almacenamiento: agrupa las operaciones de datos (CU-01 a CU-05), dirigida al consumidor.
- Interfaz de configuración del proveedor activo: agrupa la selección y validación del proveedor (CU-06), dirigida al usuario raíz a través del consumidor.

La separación de ambas interfaces es la decisión de ADR-02 (cada interfaz alineada con su audiencia y su política de autorización). Las operaciones se modelan como asincrónicas (no bloqueantes), coherente con la vista de procesos (§4 de la arquitectura).

## 3. Operaciones

| Operación | CU | Entrada | Salida | Errores posibles |
| --- | --- | --- | --- | --- |
| Guardar | CU-01 | Contenido (no vacío), destino lógico (prefijo o identificador explícito), tipo de contenido, marca de sobrescritura opcional | Identificador lógico, tamaño persistido | CONTENIDO_VACIO, DESTINO_INVALIDO, IDENTIFICADOR_DUPLICADO, TAMANIO_EXCEDIDO, PROVEEDOR_NO_DISPONIBLE, PROVEEDOR_NO_CONFIGURADO |
| Recuperar | CU-02 | Identificador lógico, rango de bytes opcional, modo solo-metadatos opcional | Contenido (idéntico al guardado) y metadatos (tipo, tamaño); o solo metadatos | IDENTIFICADOR_INEXISTENTE, DESTINO_INVALIDO, RANGO_INVALIDO, PROVEEDOR_NO_DISPONIBLE, PROVEEDOR_NO_CONFIGURADO |
| Eliminar | CU-03 | Identificador lógico, o prefijo para eliminación múltiple | Confirmación; en eliminación múltiple, cantidad eliminada | DESTINO_INVALIDO, ELIMINACION_PARCIAL, PROVEEDOR_NO_DISPONIBLE, PROVEEDOR_NO_CONFIGURADO |
| Verificar existencia | CU-04 | Identificador lógico, devolución de metadatos opcional | Presencia (booleano) y, si está presente, tamaño y metadatos | DESTINO_INVALIDO, PROVEEDOR_NO_DISPONIBLE, PROVEEDOR_NO_CONFIGURADO |
| Listar bajo prefijo | CU-05 | Prefijo lógico, tamaño máximo de página opcional, testigo de continuación opcional | Lista de identificadores y, si la enumeración no se completó, testigo de continuación | DESTINO_INVALIDO, TESTIGO_INVALIDO, PROVEEDOR_NO_DISPONIBLE, PROVEEDOR_NO_CONFIGURADO |
| Configurar proveedor activo | CU-06 | Proveedor a activar, parámetros y credenciales, modo validación-en-seco opcional | Confirmación de activación (sin revelar credenciales); o resultado de validación en seco | PROVEEDOR_NO_SOPORTADO, CREDENCIALES_INVALIDAS, PROVEEDOR_INACCESIBLE, AUTORIZACION_INSUFICIENTE |

Garantías de comportamiento (ADR-04):

- Igualdad binaria: lo recuperado es idénticamente igual a lo guardado mientras no se sobrescriba ni elimine (RN-02).
- Idempotencia: eliminar un identificador inexistente se trata como éxito (CU-03, FA-01).
- Listado: se garantiza cardinalidad y pertenencia bajo el prefijo; no se garantiza el orden (CU-05).
- Coherencia: un identificador recuperable se reporta presente y uno eliminado se reporta ausente (CU-04, RN-02).

## 4. Esquemas de datos

Tipos lógicos del contrato (forma abstracta; los tipos físicos viven en el stack de §17):

- Identificador lógico: cadena opaca para el consumidor en cuanto a la ubicación física; estable a través de versiones menores de la librería (CU-01 nota; ADR-03).
- Destino lógico: prefijo o ruta lógica con un formato admitido que la categoría 05/stack fija; agrupa los archivos de un relevamiento.
- Contenido: secuencia de bytes; la librería no la transforma ni recodifica (RN-02).
- Metadatos del archivo: tipo de contenido y tamaño persistido.
- Resultado de listado: colección de identificadores más testigo de continuación opcional.
- Configuración de proveedor: proveedor seleccionado más parámetros y credenciales; las credenciales entran pero no salen (RN-03; ADR-05).

Las credenciales y parámetros sensibles no forman parte de ningún esquema de salida: no se devuelven en resultados ni se incluyen en errores (RN-03).

## 5. Manejo de errores

El contrato expone un conjunto único de códigos de error, idéntico cualquiera sea el proveedor activo (RN-01). El catálogo completo, con mensaje, causa y acción sugerida, vive en `dx-error-messages_v1.0.md` (03); aquí se consolida la taxonomía y los códigos reservados.

| Categoría | Códigos | Tratamiento |
| --- | --- | --- |
| Entrada inválida | CONTENIDO_VACIO, DESTINO_INVALIDO, RANGO_INVALIDO, TESTIGO_INVALIDO, CREDENCIALES_INVALIDAS, PROVEEDOR_NO_SOPORTADO, TAMANIO_EXCEDIDO | Rechazo antes de delegar en el proveedor |
| Recurso ausente | IDENTIFICADOR_INEXISTENTE, PROVEEDOR_NO_CONFIGURADO | No se modifica ni crea nada |
| Conflicto de estado | IDENTIFICADOR_DUPLICADO, ELIMINACION_PARCIAL | El estado preexistente se conserva o se informa el conjunto no procesado |
| Error transitorio | PROVEEDOR_NO_DISPONIBLE, PROVEEDOR_INACCESIBLE | Propagación uniforme y reintentable; sin filtrar configuración ni credenciales (RN-03) |
| Permiso insuficiente | AUTORIZACION_INSUFICIENTE | Solo el usuario raíz cambia el proveedor activo |

Reglas de error: los códigos son estables, en mayúsculas y sin tildes, independientes del idioma y del proveedor (RN-01); el texto descriptivo es traducible sin cambiar el código; ningún error incluye credenciales ni parámetros de conexión (RN-03; ADR-05).

## 6. Versionado del contrato

Gobernado por ADR-03 (SemVer 2.0.0, derivado del tag y alineado al ciclo del backend; Conventional Commits) y por la nota de compatibilidad de la especificación funcional (02 §6):

- Cambio compatible (versión menor): agregar un proveedor, una operación, un parámetro opcional con valor por defecto, o una traducción de mensajes de error. No rompe a `geovial-api`.
- Cambio incompatible (versión mayor): cambiar la semántica de una operación, quitar una operación, quitar o renombrar un código de error, o volver obligatorio un parámetro antes opcional. Obliga a coordinar con el consumidor.
- Deprecación: una operación o código se marca como obsoleto en una versión menor antes de removerse en la siguiente mayor.
- El identificador lógico emitido por CU-01 conserva su significado a través de versiones menores.

La librería no se publica como paquete redistribuible (intake §13); el contrato se versiona para el consumidor único interno.

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU cubiertos | CU-01, CU-02, CU-03, CU-04, CU-05, CU-06 |
| RN aplicables | RN-01 (transparencia), RN-02 (integridad), RN-03 (manejo seguro de credenciales) |
| NB upstream | NB-07 (principal); NB-03 y NB-06 (soporte) |
| ADRs que lo gobiernan | ADR-01 (estilo), ADR-02 (superficie pública estable), ADR-03 (versionado), ADR-04 (transparencia e integridad), ADR-05 (credenciales) |
| Contrato inter-proyecto | `geovial-storage → geovial-api` (productor expone a consumidor); arista del manifiesto §13/§14; se indexa en la vista de solución |
| Tests previstos (en 08) | Batería de contrato única por proveedor (RN-01); igualdad binaria guardar-recuperar (RN-02); no filtración de credenciales (RN-03); compatibilidad de versión menor sin cambios en el consumidor |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Contrato de Abstractions inicial de geovial-storage: dos interfaces (almacenamiento y configuración del proveedor), seis operaciones derivadas de CU-01 a CU-06, esquemas de datos lógicos, taxonomía de errores uniforme, política de versionado (ADR-03) y trazabilidad CU/RN/NB/ADR. |
