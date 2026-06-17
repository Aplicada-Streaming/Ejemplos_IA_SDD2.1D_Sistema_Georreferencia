# US-28 — Resolver un conflicto unificando o separando marcadores

**Proyecto:** geovial-api
**Documento:** US-28-resolver-conflicto-unificar-separar_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-05 Revisión, conflictos y cierre
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero resolver un conflicto unificando o separando los marcadores involucrados, para dejar el relevamiento consistente para poder cerrarlo.

## 2. Contexto

CU-13 describe la resolución de conflictos de marcadores durante la revisión. NB-05 enmarca la revisión, conflictos y cierre, y prevé que la resolución ocurra al cierre del trabajo de campo. RN-03 reconoce la convivencia con el conflicto como estado válido del dominio hasta su resolución. RC-04 fija las transiciones de estado válidas y ADR-06 resuelve la idempotencia de la operación por clave.

## 3. Criterios de aceptación

- Given un jefe de área y un conflicto pendiente dentro de su alcance, When lo resuelve unificando o separando los marcadores, Then el sistema aplica la resolución y el conflicto pasa a estado resuelto.
- Given un identificador de conflicto que no existe, When se intenta resolver, Then el sistema responde con el código RECURSO_NO_ENCONTRADO.
- Given una resolución ya aplicada con la misma clave, When se reenvía, Then el sistema no la duplica y mantiene el conflicto en estado resuelto.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-05 |
| CU cubiertos | CU-13 |
| BT derivadas | BT-06, BT-14 |
| Tests previstos | acceptance/AT-28-resolver-conflicto; contract test de POST de resolución de conflicto |

## 5. Prioridad y estimación

Must porque sin resolución de conflictos el relevamiento no alcanza la consistencia exigida por RN-05 para el cierre. 5 SP por Planning Poker (Fibonacci): la operación abarca dos modos de resolución (unificar o separar), una transición de estado válida (RC-04), la idempotencia por clave (ADR-06) y el rechazo por conflicto inexistente.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-13)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-06, BT-14)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La convivencia con el conflicto es un estado válido del dominio hasta su resolución. Se asume el escenario de la §9 de la especificación funcional sobre conflictos entre dos agentes, a confirmar con el negocio.
