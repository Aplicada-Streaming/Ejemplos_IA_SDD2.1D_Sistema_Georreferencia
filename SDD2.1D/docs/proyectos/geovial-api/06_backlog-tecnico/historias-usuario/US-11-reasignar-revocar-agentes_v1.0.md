# US-11 — Reasignar y revocar agentes de un relevamiento

**Proyecto:** geovial-api
**Documento:** US-11-reasignar-revocar-agentes_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-02 Relevamientos y ciclo de vida
**Prioridad MoSCoW:** Should
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero revocar la asignación de un agente y reasignar otro, para ajustar el equipo de un relevamiento durante la recolección.

## 2. Contexto

CU-05 contempla la reasignación de agentes (F-14) para acomodar cambios de equipo mientras el relevamiento está en recolección. RC-05 fija que la revocación deja el par agente-relevamiento sin asignación vigente sin borrar el histórico, y RN-01 limita la operación al alcance del jefe. Resuelve el problema de mover personal entre relevamientos sin perder el rastro de quién estuvo asignado.

## 3. Criterios de aceptación

- Given un jefe de área habilitado y un agente con asignación vigente en un relevamiento de su alcance, When revoca la asignación, Then el par queda sin asignación vigente y el registro histórico se conserva.
- Given un relevamiento de su alcance al que se le revocó un agente, When el jefe asigna otro agente de su área, Then el sistema crea la nueva asignación vigente para ese par.
- Given una asignación que no existe, When el jefe intenta revocarla, Then el sistema responde con el código RECURSO_NO_ENCONTRADO.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-02 |
| CU cubiertos | CU-05 |
| BT derivadas | BT-05, BT-06 |
| Tests previstos | acceptance/AT-11-reasignar-agente; contract test de DELETE de asignación |

## 5. Prioridad y estimación

Should porque permite ajustar el equipo durante la recolección, pero un relevamiento puede avanzar con la asignación inicial de US-10 si no hay cambios. 3 SP por Planning Poker (Fibonacci): es una operación acotada que reutiliza el modelo de asignaciones de US-10 y solo agrega la revocación que conserva histórico (RC-05).

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-05)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-05)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La revocación deja el par sin vigencia conforme a RC-05 y no borra el histórico de asignaciones, que se conserva para la trazabilidad. La asignación inicial de un agente a un relevamiento se trata en US-10.
