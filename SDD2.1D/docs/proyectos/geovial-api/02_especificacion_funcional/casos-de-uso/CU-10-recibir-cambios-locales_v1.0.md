# CU-10 — Recibir el lote de cambios locales del agente (subida de sincronización)

**Proyecto:** geovial-api
**Documento:** CU-10-recibir-cambios-locales_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que el backend reciba un lote de cambios que un agente capturó sin conexión en terreno —marcadores, observaciones, fotos, comentarios y etiquetas— y los incorpore al relevamiento asignado de forma confiable, sin pérdidas ni duplicaciones. Es la fase de subida de la sincronización, que ocurre antes de cualquier bajada (RN-06).

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Cliente de campo del agente | Primario | Envía el lote de cambios locales acumulados sin conexión |
| Backend de sincronización | Sistema | Recibe, valida y aplica el lote de cambios al relevamiento |
| Almacén relacional y de archivos | Sistema | Persiste los cambios aplicados y aloja los binarios de las fotos |

## 3. Precondiciones

- El cliente presenta un token de autenticación vigente del agente (CU-03).
- El relevamiento destino existe y está asignado al agente.
- El relevamiento no está cerrado.
- Cada cambio del lote porta un identificador de origen que permite reconocer reenvíos (RN-07, idempotencia).

## 4. Flujo principal

1. El cliente de campo envía el lote de cambios locales pendientes en el orden en que se generaron.
2. El backend valida el token, el alcance del agente sobre el relevamiento y que el relevamiento esté abierto.
3. El backend procesa cada cambio del lote en orden; por cada identificador de origen ya aplicado, reconoce el reenvío y no lo duplica (RN-07).
4. El backend aplica los cambios nuevos: crea o actualiza marcadores, observaciones, fotos, comentarios y etiquetas, delegando el alojamiento de los binarios al almacén de archivos.
5. Los marcadores que caen dentro de un mismo radio se aceptan como conflicto que convive con la operación, sin bloquear la subida (RN-03).
6. El backend responde con el resultado de la subida: cambios aplicados, reenvíos reconocidos y elementos en conflicto registrados.

## 5. Flujos alternativos

- 5.A Subida parcial por corte de conexión. Disparador: la conexión se corta tras aplicar parte del lote. El backend deja aplicados y confirmados los cambios ya procesados y permite que el cliente reenvíe el resto en una subida posterior sin reaplicar lo ya confirmado (RN-07). Retorna al paso 3 en el siguiente intento.
- 5.B Lote con marcadores en conflicto. Disparador: el lote trae marcadores dentro del radio de marcadores existentes. El backend los registra como conflicto y los incorpora sin unificar; la resolución se difiere al cierre (RN-03, CU-13). Retorna al paso 6.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| RELEVAMIENTO_NO_ASIGNADO | El agente envía cambios de un relevamiento que no tiene asignado | Rechaza con estado de prohibido y no aplica ningún cambio del lote |
| RELEVAMIENTO_CERRADO | El relevamiento destino fue cerrado por el jefe | Rechaza la subida e indica al cliente que el relevamiento ya no admite cambios (ver Notas) |
| LOTE_MALFORMADO | Un cambio del lote no porta identificador de origen o viola la estructura esperada | Rechaza el lote sin aplicar cambios y señala el cambio inválido |

## 7. Postcondiciones

- Éxito: los cambios nuevos quedan aplicados al relevamiento, los reenvíos quedaron reconocidos sin duplicar y los conflictos quedaron registrados sin bloquear.
- Éxito parcial: los cambios confirmados quedan aplicados y el resto puede reenviarse sin pérdida ni duplicación.
- Fallo: ningún cambio del lote se aplica y se devuelve un problema con el código correspondiente.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un agente con un relevamiento asignado y un lote de 5 cambios locales nuevos | El cliente envía la subida | El backend aplica los 5 cambios y responde con 5 aplicados y 0 reenvíos |
| CA-02 | Un lote en el que 3 cambios ya fueron aplicados en una subida anterior | El cliente reenvía el lote completo tras un corte | El backend aplica solo los cambios nuevos y reconoce los 3 reenvíos sin duplicarlos |
| CA-03 | Un lote que trae un marcador dentro del radio de un marcador existente | El cliente envía la subida | El backend incorpora el marcador y lo registra como conflicto sin bloquear la subida |
| CA-04 | Un agente que envía cambios de un relevamiento que no tiene asignado | El cliente envía la subida | El backend rechaza con el código RELEVAMIENTO_NO_ASIGNADO y no aplica nada |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-04 |
| Reglas de negocio aplicables | RN-03, RN-06, RN-07 |
| Historias de usuario a generar | US-21, US-22 (en 06) |
| Componentes esperados | Recurso de subida de sincronización; servicio de deduplicación por identificador de origen; aplicador de cambios sobre marcadores y observaciones (referencia tentativa a 05) |
| Tests previstos | Aplicación de lote nuevo; reenvío reconocido sin duplicar; conflicto incorporado sin bloquear; relevamiento no asignado rechazado (en 08) |

## 10. Notas y supuestos

- La subida ocurre siempre antes de la bajada en un ciclo de sincronización (RN-06); este CU describe la fase de subida y CU-11 la de bajada.
- El cierre del relevamiento mientras el agente tiene cambios locales sin sincronizar es un caso límite del intake con respuesta pendiente del cliente; este CU asume que el cierre bloquea nuevas subidas y devuelve RELEVAMIENTO_CERRADO, a confirmar (ver §9 del índice).
- La idempotencia por identificador de origen es la invariante RN-07.

## 12. Performance esperado del CU

- La subida tolera lotes de al menos 1000 cambios por relevamiento sin pérdida ni duplicación, según el NFR del proyecto.

## 15. Idempotencia y reintento

- Toda la subida es idempotente por identificador de origen: reenviar un lote tras un corte no reaplica cambios ya confirmados (RN-07).
- Una subida parcial deja un punto de continuación que el cliente reanuda en el siguiente intento sin reaplicar lo confirmado.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de subida de cambios locales en la sincronización, derivado de NB-04 (F-07). |
