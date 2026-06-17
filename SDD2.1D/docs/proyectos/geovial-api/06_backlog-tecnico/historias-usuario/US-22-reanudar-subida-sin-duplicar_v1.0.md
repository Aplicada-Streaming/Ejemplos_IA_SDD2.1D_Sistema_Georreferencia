# US-22 — Reanudar una subida interrumpida sin duplicar cambios

**Proyecto:** geovial-api
**Documento:** US-22-reanudar-subida-sin-duplicar_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-04 Sincronización sin conexión
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como cliente de campo del agente, quiero reanudar una subida interrumpida reenviando el lote, para completar la sincronización tras un corte sin perder ni duplicar trabajo.

## 2. Contexto

CU-10, en su flujo alternativo FA-01 de reanudación, describe el reenvío de un lote cuya subida quedó parcial tras un corte de conexión. NB-04 enmarca la sincronización sin conexión del trabajo de campo. RN-07 garantiza la idempotencia por identificador de origen, de modo que el reenvío reconoce lo ya aplicado y aplica solo lo nuevo. Esta capacidad mitiga el riesgo R-03 del negocio de pérdida o duplicación de cambios.

## 3. Criterios de aceptación

- Given un lote cuya subida quedó parcial tras un corte, When el cliente reenvía el lote, Then el sistema reconoce los cambios ya aplicados por su identificador de origen y aplica solo los nuevos.
- Given un cambio que reutiliza un identificador de origen ya aplicado pero con contenido distinto, When se reenvía el lote, Then el sistema responde con el código CLAVE_REUTILIZADA_INCONSISTENTE y no aplica ese cambio.
- Given un reenvío completo del lote sin novedades respecto de lo ya aplicado, When se procesa, Then el sistema no produce ningún efecto duplicado y confirma el estado consistente.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-04 |
| CU cubiertos | CU-10 |
| BT derivadas | BT-11, BT-12 |
| Tests previstos | acceptance/AT-22-reanudacion-subida; contract test de POST de subida reintentada |

## 5. Prioridad y estimación

Must porque la reanudación idempotente es condición para que un corte de conexión no obligue a rehacer el trabajo ni a duplicarlo, eje de la confiabilidad de NB-04. 5 SP por Planning Poker (Fibonacci): la lógica de reenvío reutiliza la deduplicación por identificador de origen (RN-07) ya prevista para la subida, pero suma la detección de reutilización inconsistente de clave y la garantía de no duplicación ante reenvíos completos.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-10)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-11, BT-12)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

Se asume el escenario de la §9 de la especificación funcional sobre pérdida de conexión con subida parcial, a confirmar con el negocio. La reanudación es idempotente: no pierde ni duplica cambios, y se apoya en el identificador de origen como marca opaca de deduplicación.
