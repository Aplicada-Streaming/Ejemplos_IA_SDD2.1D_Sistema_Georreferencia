# Glosario técnico — geovial-storage

**Proyecto:** geovial-storage
**Documento:** glosario-tecnico_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Technical Writer + SDK Documentation Lead
**Tipo Diátaxis:** Reference
**Audiencia:** Developer integrador del backend que consume la abstracción de almacenamiento
**Nivel:** Básico
**Tiempo estimado de lectura:** 6 min

## 1. Cómo usar este glosario

Esta es la fuente canónica del vocabulario del consumidor de `geovial-storage`. El resto de los documentos de la categoría 10 enlaza acá en lugar de redefinir un término. Cada entrada declara el término en kebab-case, una definición operativa de una a tres oraciones y la referencia cruzada al documento donde el término se desarrolla.

Convención: los términos del contrato (operaciones, errores) usan su forma en el código (mayúsculas, sin tildes, para los códigos de error). Los conceptos llevan un identificador kebab que coincide con el usado en `conceptos-fundamentales_v1.0.md`.

## 2. Vocabulario canónico

| Término | Definición operativa | Referencia cross-doc |
| --- | --- | --- |
| abstraccion-de-almacenamiento | Contrato único de operaciones de archivo que el consumidor invoca sin saber dónde queda físicamente el contenido. Es la superficie pública de la librería. | `conceptos-fundamentales_v1.0.md` §2 (`concepto-abstraccion`) |
| proveedor-activo | Destino físico que aloja el contenido en un momento dado (local, remoto u otro). Lo selecciona el usuario raíz y es transparente para el consumidor. | `conceptos-fundamentales_v1.0.md` §2 (`concepto-proveedor-activo`) |
| proveedor | Adaptador que implementa el puerto de almacenamiento para un destino concreto. Se categoriza de forma neutral como local, remoto u otro. | `conceptos-fundamentales_v1.0.md` §4 |
| transparencia | Propiedad por la cual el contrato, los resultados y los códigos de error son idénticos cualquiera sea el proveedor activo. El consumidor no escribe ramas por proveedor (RN-01). | `conceptos-fundamentales_v1.0.md` §3 (`concepto-transparencia`) |
| integridad-binaria | Garantía de que el contenido recuperado es idénticamente igual, byte a byte, al guardado mientras no se sobrescriba ni elimine (RN-02). | `conceptos-fundamentales_v1.0.md` §3 (`concepto-integridad`) |
| identificador-logico | Cadena opaca para el consumidor en cuanto a la ubicación física, con la que recupera, verifica o elimina un archivo. Estable a través de versiones menores. | `referencia-api_v1.0.md` §1; `conceptos-fundamentales_v1.0.md` §4 |
| destino-logico | Prefijo o ruta lógica con un formato admitido que agrupa los archivos de un relevamiento. Se usa al guardar y como raíz del listado. | `referencia-api_v1.0.md` §1 |
| prefijo | Fragmento inicial de identificador que delimita un conjunto de archivos para listar o eliminar en bloque. | `referencia-api_v1.0.md` §2 (operación listar) |
| contenido | Secuencia de bytes del archivo. La librería no la transforma ni recodifica en ningún punto (RN-02). | `referencia-api_v1.0.md` §1 |
| metadatos | Tipo de contenido y tamaño persistido asociados a un archivo. Se obtienen sin transferir el binario completo. | `referencia-api_v1.0.md` §2 (recuperar, verificar) |
| testigo-de-continuacion | Marca opaca que devuelve el listado cuando la enumeración no se completó; se reenvía para pedir la página siguiente. | `referencia-api_v1.0.md` §2 (operación listar) |
| sobrescritura | Marca opcional al guardar que autoriza reemplazar el contenido de un identificador ya existente conservando el identificador. | `referencia-api_v1.0.md` §2 (operación guardar) |
| validacion-en-seco | Comprobación de una configuración de proveedor (soporte, formato, conectividad y permisos) sin fijarlo como activo (CU-06, FA-02). | `guia-integracion-servicio-backend_v1.0.md` §3; `referencia-api_v1.0.md` §3 |
| resguardo-de-credenciales | Componente interno que custodia las credenciales del proveedor: entran por la configuración y no salen por ninguna vía de la superficie pública (RN-03; ADR-05). | `conceptos-fundamentales_v1.0.md` §3 (`concepto-credenciales`) |
| puerto-de-proveedor | Contrato interno que un proveedor nuevo implementa para integrarse sin tocar el núcleo. Es el único punto de extensión de la librería. | `conceptos-fundamentales_v1.0.md` §5; ver 05 `extensibilidad_v1.0.md` |
| codigo-de-error | Clave estable en mayúsculas y sin tildes que identifica una condición de error, independiente del idioma y del proveedor. El developer programa contra el código, no contra el texto. | `referencia-api_v1.0.md` §4; `troubleshooting_v1.0.md` §1 |
| tamano-maximo | Límite de tamaño de archivo, configurable, con valor por defecto 25 MB, validado por el núcleo antes de delegar (ADR-04). | `referencia-api_v1.0.md` §2 (guardar); `troubleshooting_v1.0.md` ISSUE-05 |
| version-menor | Cambio compatible del contrato (agregar un proveedor, una operación o un parámetro opcional) que no rompe al consumidor (ADR-03). | `conceptos-fundamentales_v1.0.md` §3; ver 05 `contratos-abstractions_v1.0.md` §6 |
| version-mayor | Cambio incompatible del contrato (cambiar semántica, quitar una operación o un código, volver obligatorio un parámetro opcional) que obliga a coordinar con el consumidor (ADR-03). | `conceptos-fundamentales_v1.0.md` §3; ver 05 `contratos-abstractions_v1.0.md` §6 |

## 3. Referencias cruzadas

- 05 `contratos-abstractions_v1.0.md` §3 y §4: operaciones y esquemas de datos lógicos de los que se deriva este vocabulario.
- 05 `extensibilidad_v1.0.md` §2 y §3: definición del puerto de proveedor y sus obligaciones.
- 02 `casos-de-uso/`: CU-01 a CU-06, origen funcional de cada operación y término.
- 10 `conceptos-fundamentales_v1.0.md`: desarrollo de los conceptos referenciados en la columna cross-doc.
- 10 `referencia-api_v1.0.md`: firmas y parámetros de cada operación nombrada aquí.

## 4. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Glosario inicial del consumidor: vocabulario canónico de la abstracción de almacenamiento (operaciones, conceptos, errores y términos de configuración) con definición operativa y referencia cruzada por término. |
