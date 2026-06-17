# CU-11 — Entregar las actualizaciones del relevamiento asignado (bajada de sincronización)

**Proyecto:** geovial-api
**Documento:** CU-11-entregar-actualizaciones-relevamiento_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que el backend entregue al cliente de campo las actualizaciones del relevamiento asignado posteriores a la última sincronización conocida, de modo que el agente disponga en su dispositivo de las novedades del jefe y del resto de la cuadrilla. Es la fase de bajada de la sincronización, que ocurre solo después de la subida (RN-06).

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Cliente de campo del agente | Primario | Solicita las actualizaciones posteriores a su última sincronización |
| Backend de sincronización | Sistema | Calcula y entrega las novedades del relevamiento desde la marca recibida |
| Almacén relacional y de archivos | Sistema | Aporta los cambios del relevamiento y las referencias de fotos |

## 3. Precondiciones

- El cliente presenta un token de autenticación vigente del agente (CU-03).
- El relevamiento está asignado al agente.
- La fase de subida del ciclo de sincronización concluyó para este cliente (RN-06).
- El cliente aporta una marca de su última sincronización conocida del relevamiento.

## 4. Flujo principal

1. El cliente solicita las actualizaciones del relevamiento posteriores a su marca de última sincronización.
2. El backend valida el token, el alcance del agente y que la subida previa del ciclo haya concluido (RN-06).
3. El backend calcula el conjunto de cambios del relevamiento ocurridos después de la marca recibida: marcadores, observaciones, fotos, comentarios, etiquetas, asignaciones y estado.
4. El backend entrega ese conjunto junto con una nueva marca de sincronización que el cliente guardará para el próximo ciclo.
5. Las entidades en conflicto se entregan como estado válido en conflicto, sin bloquear la bajada (RN-03).

## 5. Flujos alternativos

- 5.A Sin novedades. Disparador: no hubo cambios en el relevamiento desde la marca recibida. El backend entrega un conjunto vacío y la misma marca o una equivalente, sin obligar a aplicar nada en el cliente. Retorna al paso 4.
- 5.B Reasignación detectada en la bajada. Disparador: el relevamiento dejó de estar asignado al agente entre la subida y la bajada. El backend informa la pérdida de la asignación para que el cliente deje de mostrar el relevamiento, sin entregar más actualizaciones de su contenido. Termina con la novedad de desasignación.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| SUBIDA_NO_CONCLUIDA | El cliente solicita la bajada sin haber concluido la subida del ciclo | Rechaza con estado de conflicto e indica que debe completar la subida primero (RN-06) |
| MARCA_INVALIDA | La marca de última sincronización aportada no es reconocible | Rechaza con estado de solicitud inválida y solicita una sincronización completa del relevamiento |
| RELEVAMIENTO_NO_ASIGNADO | El relevamiento no está asignado al agente | Rechaza con estado de prohibido y no entrega actualizaciones |

## 7. Postcondiciones

- Éxito: el cliente recibe las novedades posteriores a su marca y una marca nueva para el próximo ciclo; los conflictos viajan como estado válido.
- Fallo: no se entregan actualizaciones y se devuelve un problema con el código correspondiente.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un agente que completó la subida y cuyo relevamiento tuvo 4 cambios del jefe desde su última marca | El cliente solicita la bajada | El backend entrega los 4 cambios y una marca nueva de sincronización |
| CA-02 | Un agente que solicita la bajada sin haber completado la subida del ciclo | El cliente solicita la bajada | El backend rechaza con el código SUBIDA_NO_CONCLUIDA |
| CA-03 | Un relevamiento sin cambios desde la marca del agente | El cliente solicita la bajada | El backend entrega un conjunto vacío y una marca equivalente |
| CA-04 | Un relevamiento con un marcador marcado en conflicto | El cliente solicita la bajada | El backend entrega el marcador en conflicto como estado válido sin bloquear la bajada |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-04 |
| Reglas de negocio aplicables | RN-03, RN-06 |
| Historias de usuario a generar | US-23, US-24 (en 06) |
| Componentes esperados | Recurso de bajada de sincronización; servicio de cálculo de novedades por marca; control de orden subir-antes-de-bajar (referencia tentativa a 05) |
| Tests previstos | Entrega de novedades posteriores a la marca; bajada sin subida rechazada; sin novedades entrega vacío; conflicto entregado como estado válido (en 08) |

## 10. Notas y supuestos

- El orden subir-antes-de-bajar es la invariante RN-06; este CU exige que la subida (CU-10) haya concluido antes de entregar la bajada.
- La marca de última sincronización es opaca para el cliente; su semántica interna pertenece a la categoría 05.
- La disponibilidad de datos para revisión a tiempo (criterio de NB-04) depende de que este ciclo se complete tras recuperar conexión.

## 12. Performance esperado del CU

- La bajada debe entregar el conjunto de novedades dentro del objetivo de lectura del proyecto en condiciones normales y escalar con el volumen de cambios del relevamiento.

## 15. Idempotencia y reintento

- La bajada es una operación segura y repetible: solicitarla dos veces con la misma marca entrega el mismo conjunto de novedades sin efectos secundarios.
- La marca nueva solo se adopta cuando el cliente confirma haber aplicado las novedades, evitando perder cambios ante un corte durante la bajada.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de bajada de actualizaciones en la sincronización, derivado de NB-04 (F-07). |
