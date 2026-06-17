# CU-03 — Eliminar un archivo

**Proyecto:** geovial-storage
**Documento:** CU-03-eliminar-archivo_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que el consumidor quite del proveedor activo un archivo previamente guardado, identificado por su identificador lógico, dejando ese identificador libre y sin contenido recuperable.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| geovial-api (consumidor) | Primario | Solicita la eliminación de un archivo por su identificador lógico |
| Proveedor de almacenamiento activo | Sistema | Quita físicamente el contenido alojado |

## 3. Precondiciones

- Existe un proveedor activo configurado y validado (ver CU-06).
- El consumidor dispone del identificador lógico del archivo que desea eliminar.

## 4. Flujo principal

1. El consumidor invoca la operación de eliminación entregando el identificador lógico.
2. La librería valida el formato del identificador.
3. La librería delega la eliminación en el proveedor activo de forma transparente.
4. El proveedor activo quita el contenido y confirma la operación.
5. La librería informa al consumidor que la eliminación se completó.

## 5. Flujos alternativos

- FA-01 Eliminación idempotente de un identificador inexistente. Disparador: el consumidor solicita eliminar un identificador que no corresponde a ningún archivo. La librería trata la operación como exitosa por idempotencia (el estado deseado —ausencia del archivo— ya se cumple) y lo informa sin error. Punto de retorno: paso 5 del flujo principal.
- FA-02 Eliminación múltiple bajo un prefijo. Disparador: el consumidor solicita eliminar todos los archivos bajo un prefijo. La librería elimina el conjunto y devuelve la cantidad eliminada. Punto de retorno: paso 5 del flujo principal.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| DESTINO_INVALIDO | El identificador o prefijo no cumple el formato admitido | La librería rechaza la operación antes de delegar en el proveedor |
| PROVEEDOR_NO_DISPONIBLE | El proveedor activo no responde o rechaza la eliminación | La librería propaga el error de forma uniforme; el archivo permanece en el estado previo a la invocación |
| ELIMINACION_PARCIAL | En una eliminación múltiple, parte del conjunto no pudo eliminarse | La librería informa cuáles identificadores quedaron sin eliminar para que el consumidor reintente |

## 7. Postcondiciones

- En caso de éxito: el identificador queda libre y deja de ser recuperable por CU-02; CU-04 lo reporta como inexistente.
- En caso de fallo total: el o los archivos permanecen tal como estaban antes de la invocación.
- En caso de eliminación parcial: los identificadores informados como no eliminados siguen presentes y son recuperables.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un archivo guardado con identificador `relevamientos/2026/r-001/foto-01.jpg` | El consumidor invoca eliminar con ese identificador | La librería confirma la eliminación y CU-04 reporta el identificador como inexistente |
| CA-02 | Un identificador `relevamientos/2026/r-001/inexistente.jpg` que no existe | El consumidor invoca eliminar con ese identificador | La librería informa éxito por idempotencia, sin error |
| CA-03 | Tres archivos bajo el prefijo `relevamientos/2026/r-001/` | El consumidor invoca eliminar bajo ese prefijo | La librería devuelve una cantidad eliminada de 3 |
| CA-04 | Un proveedor activo que no responde | El consumidor invoca eliminar un identificador válido | La librería devuelve PROVEEDOR_NO_DISPONIBLE y el archivo permanece intacto |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-07 (principal) |
| Reglas de negocio aplicables | RN-01 |
| Historias de usuario a generar | US-05 (en 06) |
| Componentes esperados | Operación de eliminación de la abstracción de almacenamiento y su adaptador por proveedor (en 05) |
| Tests previstos | Prueba de eliminación efectiva con verificación posterior de inexistencia; prueba de idempotencia sobre inexistente; prueba de eliminación parcial (en 08) |

## 10. Notas y supuestos

- La idempotencia de la eliminación de un inexistente es una decisión funcional explícita para simplificar al consumidor; se verifica como criterio de aceptación.
- La librería no implementa papelera ni recuperación posterior; eliminar es definitivo desde la perspectiva del contrato.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de eliminación del contrato de almacenamiento. |
