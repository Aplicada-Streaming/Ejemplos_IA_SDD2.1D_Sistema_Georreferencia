# CU-04 — Verificar la existencia de un archivo

**Proyecto:** geovial-storage
**Documento:** CU-04-verificar-existencia-archivo_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que el consumidor consulte si un identificador lógico corresponde a un archivo presente en el proveedor activo, sin transferir su contenido, para decidir flujos de guardado, recuperación o eliminación.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| geovial-api (consumidor) | Primario | Consulta la presencia de un archivo por su identificador lógico |
| Proveedor de almacenamiento activo | Sistema | Informa si el identificador está presente |

## 3. Precondiciones

- Existe un proveedor activo configurado y validado (ver CU-06).
- El consumidor dispone del identificador lógico a verificar.

## 4. Flujo principal

1. El consumidor invoca la operación de verificación entregando el identificador lógico.
2. La librería valida el formato del identificador.
3. La librería delega la consulta de presencia en el proveedor activo de forma transparente.
4. El proveedor activo responde si el identificador está presente o no.
5. La librería devuelve al consumidor una respuesta booleana de presencia y, si está presente, el tamaño del archivo.

## 5. Flujos alternativos

- FA-01 Verificación con devolución de metadatos. Disparador: el consumidor pide, además de la presencia, los metadatos del archivo (tipo y tamaño). La librería los devuelve cuando el archivo existe. Punto de retorno: paso 5 del flujo principal.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| DESTINO_INVALIDO | El identificador no cumple el formato admitido | La librería rechaza la operación antes de delegar en el proveedor |
| PROVEEDOR_NO_DISPONIBLE | El proveedor activo no responde a la consulta de presencia | La librería propaga el error de forma uniforme; no devuelve un resultado de presencia ambiguo |

## 7. Postcondiciones

- En caso de éxito: el consumidor recibe una respuesta de presencia coherente con el estado real del proveedor (RN-02: un identificador recuperable se reporta presente; uno eliminado se reporta ausente). El estado del proveedor no se modifica.
- En caso de fallo: no se devuelve un valor de presencia; el consumidor recibe el error y el estado del proveedor queda intacto.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un archivo guardado con identificador `relevamientos/2026/r-001/foto-01.jpg` de 245 KB | El consumidor invoca verificar con ese identificador | La librería devuelve presencia verdadera y un tamaño de 245 KB |
| CA-02 | Un identificador `relevamientos/2026/r-001/inexistente.jpg` que no existe | El consumidor invoca verificar con ese identificador | La librería devuelve presencia falsa |
| CA-03 | Un identificador `relevamientos/2026/r-001/foto-01.jpg` que fue eliminado por CU-03 | El consumidor invoca verificar con ese identificador | La librería devuelve presencia falsa |
| CA-04 | Un proveedor activo que no responde | El consumidor invoca verificar un identificador válido | La librería devuelve PROVEEDOR_NO_DISPONIBLE y no un valor de presencia ambiguo |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-07 (principal), NB-03 (soporte: el consumidor verifica antes de asociar una foto a una observación) |
| Reglas de negocio aplicables | RN-01, RN-02 |
| Historias de usuario a generar | US-06 (en 06) |
| Componentes esperados | Operación de verificación de presencia de la abstracción de almacenamiento y su adaptador por proveedor (en 05) |
| Tests previstos | Prueba de presencia verdadera tras guardar; prueba de presencia falsa sobre inexistente; prueba de presencia falsa tras eliminar (en 08) |

## 10. Notas y supuestos

- La verificación no transfiere el contenido; su costo y latencia son menores que los de la recuperación.
- La coherencia entre verificación, guardado y eliminación es la propiedad central verificable y se apoya en RN-02.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de verificación de existencia del contrato de almacenamiento. |
