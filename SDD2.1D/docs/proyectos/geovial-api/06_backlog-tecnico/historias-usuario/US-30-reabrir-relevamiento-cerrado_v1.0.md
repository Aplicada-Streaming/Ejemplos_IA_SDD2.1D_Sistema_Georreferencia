# US-30 — Reabrir un relevamiento cerrado a revisión

**Proyecto:** geovial-api
**Documento:** US-30-reabrir-relevamiento-cerrado_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-05 Revisión, conflictos y cierre
**Prioridad MoSCoW:** Could
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero reabrir un relevamiento cerrado devolviéndolo a revisión, para corregir un cierre prematuro sin perder la información.

## 2. Contexto

CU-14 describe el cierre del relevamiento y contempla su reapertura como vuelta atrás controlada. NB-05 enmarca la revisión, conflictos y cierre del trabajo de campo. RN-05 admite la transición de cierre a revisión para corregir un cierre prematuro. RC-04 fija las transiciones de estado válidas, entre ellas CIERRE a REVISION.

## 3. Criterios de aceptación

- Given un relevamiento en estado CIERRE dentro del alcance del jefe de área, When lo reabre, Then el sistema lo transiciona a REVISION conservando la información registrada.
- Given un relevamiento que no está cerrado, When se intenta reabrirlo, Then el sistema responde con el código TRANSICION_NO_PERMITIDA y no cambia su estado.
- Given una reapertura ya aplicada con la misma clave, When se reenvía, Then el sistema no la duplica y mantiene el relevamiento en revisión.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-05 |
| CU cubiertos | CU-14 |
| BT derivadas | BT-06, BT-14 |
| Tests previstos | acceptance/AT-30-reapertura; contract test de POST de transición de reapertura |

## 5. Prioridad y estimación

Could porque la reapertura corrige un cierre prematuro, pero el flujo principal de revisión y cierre opera sin ella. 3 SP por Planning Poker (Fibonacci): es una transición acotada CIERRE a REVISION sobre la máquina de estados ya prevista (RC-04), con un rechazo estable cuando el relevamiento no está cerrado y comportamiento idempotente.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-14)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-06)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

Could Have: se incorpora si la cadencia del sprint lo permite. La reapertura preserva la evidencia ya registrada y devuelve el relevamiento a revisión sin pérdida de información.
