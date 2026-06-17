# US-10 — Asignar un agente de campo a un relevamiento

**Proyecto:** geovial-api
**Documento:** US-10-asignar-agente-relevamiento_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-02 Relevamientos y ciclo de vida
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero asignar un agente de campo a un relevamiento, para habilitar al agente a recolectar en ese relevamiento.

## 2. Contexto

CU-05 describe la asignación de agentes a relevamientos como paso previo a la recolección exigida por NB-02. RC-05 impone la unicidad de la asignación vigente por par agente-relevamiento, de modo que no existan dos asignaciones vivas del mismo par. RN-01 limita la operación al alcance del jefe. Resuelve el problema de autorizar a un agente concreto a recolectar sobre un relevamiento determinado.

## 3. Criterios de aceptación

- Given un jefe de área habilitado, un relevamiento de su alcance y un agente de su área, When asigna el agente al relevamiento, Then el sistema crea la asignación vigente y el par queda habilitado para recolectar.
- Given un par agente-relevamiento que ya tiene asignación vigente, When el jefe vuelve a asignar el mismo par, Then el sistema no duplica la asignación y mantiene el par vigente único conforme a RC-05.
- Given un usuario cuyo rol no es agente de campo, When el jefe intenta asignarlo al relevamiento, Then el sistema rechaza la operación con el código ROL_NO_AUTORIZADO.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-02 |
| CU cubiertos | CU-05 |
| BT derivadas | BT-02, BT-05, BT-06 |
| Tests previstos | acceptance/AT-10-asignar-agente; contract test de POST de asignaciones |

## 5. Prioridad y estimación

Must porque sin asignar agentes ningún relevamiento puede recolectarse: es el puente entre la creación del relevamiento y el trabajo de campo de NB-02. 5 SP por Planning Poker (Fibonacci): suma la verificación de rol y alcance, y la unicidad de la asignación vigente por par a nivel del almacén relacional (RC-05) con comportamiento idempotente.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-05)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-05, BT-06)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La reasignación de otro agente y la revocación de una asignación vigente se tratan en US-11, fuera del scope de esta historia. La unicidad de la asignación vigente por par (RC-05) se garantiza en el almacén relacional, no solo en la capa de aplicación.
