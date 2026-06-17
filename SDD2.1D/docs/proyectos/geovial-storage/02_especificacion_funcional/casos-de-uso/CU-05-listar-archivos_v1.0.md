# CU-05 — Listar archivos bajo un prefijo

**Proyecto:** geovial-storage
**Documento:** CU-05-listar-archivos_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que el consumidor enumere los identificadores lógicos presentes bajo un prefijo dado en el proveedor activo, para recorrer los archivos de un relevamiento (por ejemplo, todas las fotos de un tramo) sin conocer de antemano sus identificadores exactos.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| geovial-api (consumidor) | Primario | Solicita el listado de archivos bajo un prefijo |
| Proveedor de almacenamiento activo | Sistema | Devuelve los identificadores presentes bajo el prefijo |

## 3. Precondiciones

- Existe un proveedor activo configurado y validado (ver CU-06).
- El consumidor dispone del prefijo lógico sobre el cual listar.

## 4. Flujo principal

1. El consumidor invoca la operación de listado entregando el prefijo lógico y, opcionalmente, un tamaño máximo de página.
2. La librería valida el formato del prefijo.
3. La librería delega la enumeración en el proveedor activo de forma transparente.
4. El proveedor activo devuelve los identificadores presentes bajo el prefijo.
5. La librería entrega al consumidor la lista de identificadores y, si la enumeración no se completó, un testigo de continuación para pedir la página siguiente.

## 5. Flujos alternativos

- FA-01 Paginación por continuación. Disparador: la cantidad de archivos bajo el prefijo excede el tamaño de página solicitado. La librería devuelve la primera página con un testigo de continuación; el consumidor reinvoca con ese testigo para obtener las páginas siguientes. Punto de retorno: paso 5 del flujo principal.
- FA-02 Prefijo sin coincidencias. Disparador: ningún identificador comienza con el prefijo. La librería devuelve una lista vacía sin error. Punto de retorno: fin del CU.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| DESTINO_INVALIDO | El prefijo no cumple el formato admitido | La librería rechaza la operación antes de delegar en el proveedor |
| TESTIGO_INVALIDO | El testigo de continuación está vencido o mal formado | La librería rechaza la operación con este código y el consumidor debe reiniciar el listado |
| PROVEEDOR_NO_DISPONIBLE | El proveedor activo no responde a la enumeración | La librería propaga el error de forma uniforme, sin exponer datos de configuración ni credenciales del proveedor (RN-03) |

## 7. Postcondiciones

- En caso de éxito: el consumidor recibe los identificadores presentes bajo el prefijo; el conjunto refleja el estado real del proveedor en el momento de la consulta. El estado del proveedor no se modifica.
- En caso de fallo: no se entrega listado y el estado del proveedor queda intacto.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Tres archivos guardados bajo el prefijo `relevamientos/2026/r-001/` | El consumidor invoca listar con ese prefijo | La librería devuelve los tres identificadores y ningún testigo de continuación |
| CA-02 | Diez archivos bajo el prefijo `relevamientos/2026/r-002/` y un tamaño de página de 4 | El consumidor invoca listar con ese prefijo y ese tamaño de página | La librería devuelve 4 identificadores y un testigo de continuación no vacío |
| CA-03 | Ningún archivo bajo el prefijo `relevamientos/2026/r-999/` | El consumidor invoca listar con ese prefijo | La librería devuelve una lista vacía sin error |
| CA-04 | Un proveedor activo que no responde | El consumidor invoca listar bajo un prefijo válido | La librería devuelve PROVEEDOR_NO_DISPONIBLE sin exponer las credenciales del proveedor |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-07 (principal), NB-06 (soporte: la exportación recorre los archivos del relevamiento) |
| Reglas de negocio aplicables | RN-01, RN-03 |
| Historias de usuario a generar | US-07 (en 06) |
| Componentes esperados | Operación de enumeración de la abstracción de almacenamiento y su adaptador por proveedor (en 05) |
| Tests previstos | Prueba de listado completo bajo prefijo; prueba de paginación con testigo de continuación; prueba de prefijo sin coincidencias (en 08) |

## 10. Notas y supuestos

- La librería ofrece listado por prefijo, no consultas arbitrarias por contenido o metadatos; alcanza para recorrer los archivos de un relevamiento.
- El orden de los identificadores en el listado no se garantiza salvo que la categoría 05 lo fije; los criterios de aceptación verifican la cardinalidad y la pertenencia, no el orden.
- La paginación existe para que el contrato sea uniforme aun cuando el proveedor remoto limite el tamaño de respuesta.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de listado del contrato de almacenamiento. |
