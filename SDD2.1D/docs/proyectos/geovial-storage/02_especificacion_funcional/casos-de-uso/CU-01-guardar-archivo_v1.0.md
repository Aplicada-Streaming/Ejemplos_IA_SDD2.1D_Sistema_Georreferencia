# CU-01 — Guardar un archivo

**Proyecto:** geovial-storage
**Documento:** CU-01-guardar-archivo_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que el consumidor de la librería (el backend) persista un archivo —típicamente una fotografía de un relevamiento— en el proveedor de almacenamiento activo y obtenga a cambio un identificador lógico estable con el que recuperarlo después. El consumidor no necesita saber dónde queda físicamente el archivo.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| geovial-api (consumidor) | Primario | Invoca la operación de guardado entregando el contenido y los metadatos del archivo |
| Proveedor de almacenamiento activo | Sistema | Aloja físicamente el contenido y confirma la persistencia |

## 3. Precondiciones

- Existe un proveedor activo configurado y validado (ver CU-06).
- El consumidor dispone del contenido del archivo y de los metadatos mínimos (nombre lógico propuesto o prefijo de destino, y tipo de contenido).
- El contenido a guardar tiene un tamaño mayor que cero.

## 4. Flujo principal

1. El consumidor invoca la operación de guardado entregando el contenido del archivo, un prefijo o ruta lógica de destino y el tipo de contenido.
2. La librería valida que el contenido no esté vacío y que el destino lógico cumpla el formato admitido.
3. La librería delega la persistencia en el proveedor activo de forma transparente.
4. El proveedor activo persiste el contenido y confirma la operación.
5. La librería genera o confirma el identificador lógico del archivo y lo devuelve al consumidor junto con el tamaño persistido.

## 5. Flujos alternativos

- FA-01 Identificador lógico provisto por el consumidor. Disparador: el consumidor entrega un identificador lógico explícito en lugar de un prefijo. La librería verifica que no colisione con un archivo existente; si no colisiona, persiste y devuelve ese identificador. Punto de retorno: paso 5 del flujo principal.
- FA-02 Sobrescritura explícita. Disparador: el consumidor solicita guardar con la marca de sobrescritura activada sobre un identificador ya existente. La librería reemplaza el contenido conservando el identificador. Punto de retorno: paso 5 del flujo principal.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| CONTENIDO_VACIO | El contenido entregado tiene tamaño cero | La librería rechaza la operación sin contactar al proveedor y devuelve el error; no se crea ningún archivo |
| DESTINO_INVALIDO | El prefijo o identificador lógico no cumple el formato admitido | La librería rechaza la operación antes de delegar en el proveedor |
| IDENTIFICADOR_DUPLICADO | El identificador provisto ya existe y no se solicitó sobrescritura | La librería rechaza la operación y conserva intacto el archivo preexistente |
| PROVEEDOR_NO_DISPONIBLE | El proveedor activo no responde o rechaza la persistencia | La librería propaga el error de forma uniforme; no queda un archivo a medias asociado al identificador |

## 7. Postcondiciones

- En caso de éxito: el archivo queda persistido en el proveedor activo y su identificador lógico es recuperable por CU-02 y verificable por CU-04. El contenido recuperado será idénticamente igual al guardado (RN-02).
- En caso de fallo: no queda ningún archivo parcial asociado al identificador; el estado del proveedor es el previo a la invocación.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un proveedor activo configurado y un contenido de 245 KB con destino `relevamientos/2026/r-001/` y tipo `image/jpeg` | El consumidor invoca guardar | La librería devuelve un identificador lógico no vacío y un tamaño persistido de 245 KB |
| CA-02 | Un contenido de 0 bytes con destino válido | El consumidor invoca guardar | La librería rechaza con el código CONTENIDO_VACIO y no crea ningún archivo |
| CA-03 | Un identificador lógico `relevamientos/2026/r-001/foto-01.jpg` que ya existe, sin marca de sobrescritura | El consumidor invoca guardar con ese identificador | La librería rechaza con el código IDENTIFICADOR_DUPLICADO y conserva el archivo preexistente |
| CA-04 | Un identificador lógico `relevamientos/2026/r-001/foto-01.jpg` que ya existe, con marca de sobrescritura activada | El consumidor invoca guardar con ese identificador | La librería reemplaza el contenido y devuelve el mismo identificador |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-07 (principal), NB-03 (soporte: origina los archivos a guardar) |
| Reglas de negocio aplicables | RN-01, RN-02 |
| Historias de usuario a generar | US-01, US-02 (en 06) |
| Componentes esperados | Operación de guardado de la abstracción de almacenamiento y su adaptador por proveedor (en 05) |
| Tests previstos | Prueba de guardado contra proveedor en memoria; prueba de rechazo por contenido vacío; prueba de duplicado y de sobrescritura (en 08) |

## 10. Notas y supuestos

- El identificador lógico es opaco para el consumidor en cuanto a la ubicación física; su forma exacta la fija la categoría 05.
- La librería no decide políticas de retención ni de ciclo de vida del archivo; solo persiste cuando se la invoca.
- El tipo de contenido se registra como metadato pero la librería no transforma ni recodifica el binario (RN-02).

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de guardado del contrato de almacenamiento. |
