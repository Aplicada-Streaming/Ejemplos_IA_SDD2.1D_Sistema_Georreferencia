# US-29 — Cerrar el relevamiento exigiendo los conflictos resueltos

**Proyecto:** geovial-api
**Documento:** US-29-cerrar-relevamiento-conflictos-resueltos_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-05 Revisión, conflictos y cierre
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero cerrar un relevamiento solo cuando sus conflictos están resueltos, para habilitar el informe con un relevamiento consistente.

## 2. Contexto

CU-14 describe el cierre del relevamiento como hito de la revisión. NB-05 enmarca la revisión, conflictos y cierre del trabajo de campo. RN-05 exige que el cierre proceda solo sin conflictos pendientes. RC-04 fija las transiciones de estado válidas y ADR-06 resuelve la idempotencia de la operación por clave.

## 3. Criterios de aceptación

- Given un relevamiento en revisión con todos sus conflictos resueltos, When el jefe de área lo cierra, Then el sistema lo cierra y habilita el informe.
- Given un relevamiento con conflictos pendientes, When se intenta cerrarlo, Then el sistema responde con el código CONFLICTOS_PENDIENTES y no lo cierra.
- Given un cierre ya aplicado con la misma clave, When se reenvía, Then el sistema no lo duplica y mantiene el relevamiento cerrado.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-05 |
| CU cubiertos | CU-14 |
| BT derivadas | BT-06, BT-14 |
| Tests previstos | acceptance/AT-29-cierre-relevamiento; contract test de POST de cierre |

## 5. Prioridad y estimación

Must porque el cierre es el hito que habilita el informe y materializa la precondición de consistencia de RN-05. 5 SP por Planning Poker (Fibonacci): combina la verificación de que no haya conflictos pendientes, una transición de estado válida (RC-04), la idempotencia por clave (ADR-06) y el efecto sobre el ciclo de vida del relevamiento.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-14)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-06, BT-14)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

El cierre es el hito que habilita el informe y bloquea nuevas subidas; un intento de subir a un relevamiento cerrado responde con el código RELEVAMIENTO_CERRADO en CU-10. La reapertura de un relevamiento cerrado se trata en US-30.
