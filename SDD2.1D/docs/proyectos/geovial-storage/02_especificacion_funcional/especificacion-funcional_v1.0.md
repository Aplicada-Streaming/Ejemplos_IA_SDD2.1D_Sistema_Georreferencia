# Especificación funcional — geovial-storage

**Proyecto:** geovial-storage
**Documento:** especificacion-funcional_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Este documento es el índice maestro de la especificación funcional de `geovial-storage`, la librería que provee al backend de la solución GeoVial una abstracción de alojamiento de archivos transparente, con proveedores intercambiables (proveedor local, proveedor de almacenamiento de objetos remoto u otros proveedores) seleccionables por el usuario raíz. El proyecto es del tipo `library`: cada caso de uso (CU) describe un contrato de uso de la superficie pública de la librería, no una pantalla ni un flujo de interfaz. La especificación define el qué del contrato; el cómo (tipos, interfaces concretas, productos de proveedor) vive en la categoría 05.

La librería se integra al consumidor `geovial-api` y no se redistribuye como paquete. Su valor es desacoplar al backend del destino físico donde se guardan las fotografías de los relevamientos, de modo que el destino pueda cambiar sin que el consumidor cambie su forma de invocar la librería.

## 2. Alcance funcional cubierto

El alcance de esta especificación es el contrato público de almacenamiento: las operaciones que el consumidor invoca para guardar, recuperar, eliminar, verificar y listar archivos, y la operación de selección del proveedor activo reservada al usuario raíz. Queda fuera de esta especificación el detalle de cada proveedor concreto, el formato de las credenciales, la organización física de los binarios y las firmas de tipo, que pertenecen a la categoría 05.

Por tratarse de un proyecto `library` (regla 02 §2.2), no se produce modelo conceptual de datos ni reglas conceptuales de modelo. Se incluyen reglas de negocio (RN) porque la abstracción impone invariantes de dominio sobre su contrato (transparencia, integridad y manejo seguro de credenciales). Se adopta la sección opcional de compatibilidad de versión pública (02 §4.3, §17) como nota de estabilidad del contrato, descrita en cada CU y consolidada en la §6 de este índice.

## 3. Catálogo de casos de uso

| CU | Nombre | Operación del contrato | Actor primario | Estado |
| --- | --- | --- | --- | --- |
| CU-01 | Guardar un archivo | Persiste un archivo en el proveedor activo y devuelve su identificador lógico | geovial-api (consumidor) | Propuesto |
| CU-02 | Recuperar un archivo | Devuelve el contenido de un archivo previamente guardado a partir de su identificador lógico | geovial-api (consumidor) | Propuesto |
| CU-03 | Eliminar un archivo | Quita del proveedor activo un archivo identificado por su identificador lógico | geovial-api (consumidor) | Propuesto |
| CU-04 | Verificar la existencia de un archivo | Informa si un identificador lógico corresponde a un archivo presente en el proveedor activo | geovial-api (consumidor) | Propuesto |
| CU-05 | Listar archivos bajo un prefijo | Enumera los identificadores lógicos presentes bajo un prefijo dado en el proveedor activo | geovial-api (consumidor) | Propuesto |
| CU-06 | Configurar el proveedor de almacenamiento activo | Selecciona y valida el proveedor activo (local / remoto / otro) y sus credenciales | Usuario raíz | Propuesto |

## 4. Catálogo de reglas de negocio

| RN | Nombre | Invariante | CU afectados |
| --- | --- | --- | --- |
| RN-01 | Transparencia del proveedor hacia el consumidor | El contrato público es idéntico cualquiera sea el proveedor activo | CU-01, CU-02, CU-03, CU-04, CU-05, CU-06 |
| RN-02 | Integridad del archivo almacenado | Lo recuperado es idénticamente igual a lo guardado bajo el mismo identificador lógico | CU-01, CU-02, CU-04 |
| RN-03 | Manejo seguro de las credenciales del proveedor | Las credenciales del proveedor nunca se exponen por la superficie pública de la librería | CU-02, CU-05, CU-06 |

## 5. Matriz de trazabilidad NB → CU → RN → US

La trazabilidad upstream principal es NB-07 (almacenamiento de archivos configurable). NB-03 (captura georreferenciada) es soporte porque origina los archivos que la librería aloja, y NB-06 (portabilidad del relevamiento) es soporte porque la exportación e importación de un relevamiento completo se apoya en las operaciones de almacenamiento. Las US se generan en 06; aquí se enumeran las que cada CU originará.

| NB upstream | CU | RN aplicables | US a generar (en 06) |
| --- | --- | --- | --- |
| NB-07 (principal), NB-03 (soporte) | CU-01 Guardar un archivo | RN-01, RN-02 | US-01, US-02 |
| NB-07 (principal), NB-06 (soporte) | CU-02 Recuperar un archivo | RN-01, RN-02, RN-03 | US-03, US-04 |
| NB-07 (principal) | CU-03 Eliminar un archivo | RN-01 | US-05 |
| NB-07 (principal), NB-03 (soporte) | CU-04 Verificar la existencia de un archivo | RN-01, RN-02 | US-06 |
| NB-07 (principal), NB-06 (soporte) | CU-05 Listar archivos bajo un prefijo | RN-01, RN-03 | US-07 |
| NB-07 (principal) | CU-06 Configurar el proveedor de almacenamiento activo | RN-01, RN-03 | US-08, US-09 |

Cobertura bidireccional. Cada CU declara al menos una NB y NB-07 está cubierta por los seis CU. NB-03 y NB-06 aparecen como soporte donde corresponde. No hay CU huérfano. NB-07 está mapeada a CU-17 en el catálogo de la solución `geovial-api`; en el proyecto `geovial-storage` esa necesidad se materializa en el contrato de la librería, renumerado localmente como CU-01 a CU-06 según la regla 3.1 (numeración propia del proyecto), conservando la trazabilidad a la NB de origen.

## 6. Compatibilidad de versión pública (nota de estabilidad del contrato)

Por ser una `library` (02 §4.3, §17), el conjunto de los seis CU define la superficie pública estable de `geovial-storage`. Las reglas de estabilidad del contrato son:

- Agregar un proveedor nuevo o un parámetro opcional con valor por defecto es un cambio compatible (versión menor): no rompe a `geovial-api`.
- Cambiar la semántica de una operación existente, quitar una operación o volver obligatorio un parámetro antes opcional es un cambio incompatible (versión mayor) y obliga a coordinar con el consumidor.
- El identificador lógico de un archivo, una vez emitido por CU-01, mantiene su significado a través de versiones menores de la librería.

El detalle de versionado de artefactos y de empaquetado vive en las categorías técnicas; aquí solo se fija la estabilidad funcional del contrato.

## 7. Decisiones de recorte (02 §5.2)

- Se generaron seis CU, por encima del mínimo de cinco para `library`, separando "verificar existencia" (CU-04) de "listar" (CU-05) porque son operaciones distintas de la superficie pública con criterios de error propios.
- No se creó un CU transversal de manejo de errores: los errores de proveedor no disponible y de identificador inexistente se repiten en varios CU, pero su tratamiento es suficientemente acotado para resolverse en cada CU sin un documento aparte.
- No se modeló la subida por fragmentos ni el versionado de blobs como CU propios: son detalles de implementación de proveedor (categoría 05), no operaciones del contrato funcional.

## 8. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice maestro inicial de la especificación funcional de geovial-storage: catálogo de seis CU, tres RN, matriz NB→CU→RN→US y nota de compatibilidad de versión pública. |
