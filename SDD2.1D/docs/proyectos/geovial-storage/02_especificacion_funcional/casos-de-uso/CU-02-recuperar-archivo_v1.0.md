# CU-02 — Recuperar un archivo

**Proyecto:** geovial-storage
**Documento:** CU-02-recuperar-archivo_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que el consumidor obtenga el contenido de un archivo previamente guardado, a partir de su identificador lógico, recibiéndolo idénticamente igual a como fue persistido, sin que importe en qué proveedor esté alojado.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| geovial-api (consumidor) | Primario | Solicita el contenido de un archivo por su identificador lógico |
| Proveedor de almacenamiento activo | Sistema | Entrega el contenido alojado |

## 3. Precondiciones

- Existe un proveedor activo configurado y validado (ver CU-06).
- El consumidor dispone del identificador lógico del archivo que desea recuperar.

## 4. Flujo principal

1. El consumidor invoca la operación de recuperación entregando el identificador lógico.
2. La librería valida el formato del identificador.
3. La librería delega la lectura en el proveedor activo de forma transparente.
4. El proveedor activo localiza el archivo y entrega su contenido.
5. La librería devuelve al consumidor el contenido y los metadatos asociados (tipo de contenido y tamaño), sin alterar el binario.

## 5. Flujos alternativos

- FA-01 Recuperación de metadatos sin contenido. Disparador: el consumidor solicita solamente los metadatos del archivo (tipo y tamaño) sin transferir el binario. La librería devuelve los metadatos. Punto de retorno: fin del CU.
- FA-02 Recuperación parcial por rango. Disparador: el consumidor solicita un rango de bytes del archivo. La librería devuelve únicamente el segmento solicitado conservando su integridad. Punto de retorno: paso 5 del flujo principal.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| IDENTIFICADOR_INEXISTENTE | El identificador lógico no corresponde a ningún archivo del proveedor activo | La librería devuelve el error sin contenido; no crea ni modifica nada |
| DESTINO_INVALIDO | El identificador no cumple el formato admitido | La librería rechaza la operación antes de delegar en el proveedor |
| RANGO_INVALIDO | El rango de bytes solicitado excede el tamaño del archivo o está mal formado | La librería rechaza la operación con este código |
| PROVEEDOR_NO_DISPONIBLE | El proveedor activo no responde o rechaza la lectura | La librería propaga el error de forma uniforme, sin filtrar datos de configuración ni de credenciales del proveedor (RN-03) |

## 7. Postcondiciones

- En caso de éxito: el consumidor recibe un contenido idénticamente igual al guardado bajo ese identificador (RN-02). El estado del proveedor no se modifica.
- En caso de fallo: no se entrega contenido y el estado del proveedor queda intacto.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un archivo previamente guardado con identificador `relevamientos/2026/r-001/foto-01.jpg` de 245 KB y tipo `image/jpeg` | El consumidor invoca recuperar con ese identificador | La librería devuelve un contenido de 245 KB idénticamente igual al guardado y el tipo `image/jpeg` |
| CA-02 | Un identificador `relevamientos/2026/r-001/inexistente.jpg` que nunca fue guardado | El consumidor invoca recuperar con ese identificador | La librería devuelve el código IDENTIFICADOR_INEXISTENTE sin contenido |
| CA-03 | Un archivo guardado de 245 KB y una solicitud del rango de bytes 0 a 1023 | El consumidor invoca recuperar por rango | La librería devuelve exactamente 1024 bytes correspondientes al inicio del archivo |
| CA-04 | Un proveedor activo que no responde | El consumidor invoca recuperar un identificador válido | La librería devuelve PROVEEDOR_NO_DISPONIBLE sin exponer las credenciales del proveedor |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-07 (principal), NB-06 (soporte: la exportación lee los archivos del relevamiento) |
| Reglas de negocio aplicables | RN-01, RN-02, RN-03 |
| Historias de usuario a generar | US-03, US-04 (en 06) |
| Componentes esperados | Operación de lectura de la abstracción de almacenamiento y su adaptador por proveedor (en 05) |
| Tests previstos | Prueba de ida y vuelta guardar-recuperar con verificación de igualdad binaria; prueba de identificador inexistente; prueba de recuperación por rango (en 08) |

## 10. Notas y supuestos

- La igualdad binaria entre lo guardado y lo recuperado es la condición central verificable y se apoya en RN-02.
- La librería no cachea contenidos; cada recuperación consulta al proveedor activo, salvo decisión de la categoría 05.
- El detalle de transferencia (flujo de bytes, descarga completa o por tramos) es de implementación; aquí solo se fija que la integridad se conserva.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de recuperación del contrato de almacenamiento. |
