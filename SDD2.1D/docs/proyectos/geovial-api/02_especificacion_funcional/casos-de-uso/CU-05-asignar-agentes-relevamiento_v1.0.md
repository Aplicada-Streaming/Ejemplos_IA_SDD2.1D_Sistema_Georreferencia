# CU-05 — Asignar y reasignar agentes de campo a un relevamiento

**Proyecto:** geovial-api
**Documento:** CU-05-asignar-agentes-relevamiento_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que el jefe de área asigne agentes de campo a un relevamiento y los reasigne cuando cambian las condiciones de campo, dejando explícito quién releva cada tramo sin perder lo ya recolectado. Evita tramos sin cubrir y esfuerzo duplicado.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área | Primario | Asigna y reasigna agentes a sus relevamientos |
| Backend de relevamientos | Sistema | Valida el alcance, registra y revoca asignaciones |
| Almacén relacional | Sistema | Persiste la relación de asignación entre agente y relevamiento |

## 3. Precondiciones

- El solicitante está autenticado y su rol es jefe de área (CU-03).
- El relevamiento existe, pertenece al jefe solicitante y no está cerrado.
- El agente a asignar existe, pertenece al área del jefe y está habilitado.

## 4. Flujo principal

1. El jefe de área solicita asignar uno o más agentes de su área a un relevamiento.
2. El backend valida que el relevamiento es suyo, que está abierto y que cada agente pertenece a su área.
3. El backend registra una asignación por cada agente, evitando duplicar una asignación existente (RN-01 sobre alcance; unicidad de asignación por RC).
4. El backend responde con el conjunto de agentes asignados al relevamiento.
5. Para reasignar, el jefe solicita quitar un agente y, opcionalmente, agregar otro; el backend revoca la asignación del primero y registra la del segundo, conservando lo recolectado por el agente saliente.

## 5. Flujos alternativos

- 5.A Reasignación con evidencia ya cargada. Disparador: el agente saliente ya capturó observaciones en el relevamiento. El backend revoca su asignación pero conserva la autoría de sus observaciones; el agente entrante puede continuar la recolección. Retorna al paso 5.
- 5.B Asignación duplicada. Disparador: el jefe asigna un agente ya asignado al mismo relevamiento. El backend no crea una asignación adicional y responde con el conjunto actual sin cambios. Retorna al paso 4.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| AGENTE_FUERA_DE_AREA | Un agente a asignar no pertenece al área del jefe | Rechaza con estado de prohibido y no registra ninguna asignación del lote |
| RELEVAMIENTO_CERRADO | El relevamiento está en estado de cierre | Rechaza con estado de conflicto y no modifica las asignaciones |
| AGENTE_INHABILITADO | El agente a asignar fue dado de baja | Rechaza con estado de solicitud inválida y no registra la asignación |

## 7. Postcondiciones

- Éxito en asignación: cada agente válido queda asignado al relevamiento, sin duplicados.
- Éxito en reasignación: el agente saliente deja de estar asignado, su evidencia se conserva y el agente entrante queda asignado.
- Fallo: las asignaciones no cambian y se devuelve un problema con el código correspondiente.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un jefe de área con un relevamiento en recolección y dos agentes de su área | Asigna ambos agentes al relevamiento | El backend registra dos asignaciones y responde con los dos agentes asignados en menos de 5 minutos de gestión |
| CA-02 | Un relevamiento con un agente que ya cargó observaciones | El jefe reemplaza ese agente por otro de su área | El backend revoca la asignación del saliente, conserva sus observaciones y asigna al entrante |
| CA-03 | Un relevamiento ya cerrado | El jefe intenta asignar un agente | El backend rechaza con el código RELEVAMIENTO_CERRADO |
| CA-04 | Un relevamiento y un agente de otra área | El jefe intenta asignar ese agente | El backend rechaza con el código AGENTE_FUERA_DE_AREA y no registra ninguna asignación |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-02 |
| Reglas de negocio aplicables | RN-01, RN-05 |
| Historias de usuario a generar | US-10, US-11 (en 06) |
| Componentes esperados | Recurso de asignaciones; servicio de validación de área y de unicidad; repositorio de asignaciones (referencia tentativa a 05) |
| Tests previstos | Asignación de agentes del área; reasignación conserva evidencia; relevamiento cerrado no admite asignación; agente fuera de área rechazado (en 08) |

## 10. Notas y supuestos

- La unicidad de la asignación agente-relevamiento es una restricción de integridad del modelo (ver RC del modelo conceptual).
- La reasignación desde la app de campo (F-14) la consume el cliente móvil sobre este mismo recurso; el backend no distingue el origen del cliente.

## 12. Performance esperado del CU

- La asignación y la reasignación deben resolverse dentro del objetivo de escritura (p95 menor o igual a 500 ms), de modo que una reasignación se complete en pocos minutos.

## 15. Idempotencia y reintento

- La asignación es idempotente respecto del par agente-relevamiento: reintentarla no crea asignaciones duplicadas (RC de unicidad de asignación).
- La revocación repetida de una asignación ya inexistente deja el estado sin cambios y responde con éxito.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de asignación y reasignación de agentes a un relevamiento, derivado de NB-02 (F-04, F-14). |
