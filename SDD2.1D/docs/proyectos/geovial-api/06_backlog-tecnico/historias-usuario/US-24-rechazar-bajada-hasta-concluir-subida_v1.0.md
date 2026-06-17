# US-24 — Rechazar la bajada hasta concluir la subida del ciclo

**Proyecto:** geovial-api
**Documento:** US-24-rechazar-bajada-hasta-concluir-subida_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-04 Sincronización sin conexión
**Prioridad MoSCoW:** Must
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como cliente de campo del agente, quiero que el backend rechace la bajada hasta que la subida del ciclo concluyó, para garantizar que el dispositivo no reciba datos antes de aportar los propios.

## 2. Contexto

CU-11 describe la bajada de novedades, que solo procede una vez aportados los cambios locales. NB-04 enmarca la sincronización sin conexión del trabajo de campo. RN-06 fija el orden subir antes de bajar dentro de cada ciclo. ADR-07 resuelve materializar esa compuerta como una condición verificable por relevamiento y cliente antes de atender la bajada.

## 3. Criterios de aceptación

- Given un cliente cuya subida del ciclo concluyó, When solicita la bajada del relevamiento, Then el sistema atiende la bajada y entrega las novedades correspondientes.
- Given un cliente cuya subida del ciclo no concluyó, When solicita la bajada, Then el sistema responde con el código SUBIDA_NO_CONCLUIDA y no entrega novedades.
- Given un relevamiento con varios clientes, When uno de ellos no concluyó su subida, Then la compuerta de subida concluida se evalúa por relevamiento y cliente, sin afectar a los demás.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-04 |
| CU cubiertos | CU-11 |
| BT derivadas | BT-13 |
| Tests previstos | acceptance/AT-24-orden-subir-antes-bajar; contract test de POST de bajada sin subida concluida |

## 5. Prioridad y estimación

Must porque el orden subir antes de bajar (RN-06) es invariante de consistencia de la sincronización de NB-04: sin él, el dispositivo podría recibir datos antes de aportar los propios. 3 SP por Planning Poker (Fibonacci): es una compuerta acotada sobre la marca de sincronización, con un único código de rechazo estable, aunque debe evaluarse por relevamiento y cliente.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-11)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-13)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La compuerta de orden se materializa en la marca de sincronización mediante la condición subida_concluida, evaluada por relevamiento y cliente. La bajada incremental propiamente dicha se detalla en US-23.
