# US-43 — Rechazar una clave de idempotencia reutilizada de forma inconsistente

**Proyecto:** geovial-api
**Documento:** US-43-rechazar-clave-reutilizada-inconsistente_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-08 Capacidades transversales del contrato
**Prioridad MoSCoW:** Should
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como cliente consumidor de la API, quiero que el backend rechace una clave de idempotencia reutilizada con contenido distinto, para detectar errores del cliente en vez de aplicar un efecto equivocado.

## 2. Contexto

CU-21 describe la idempotencia de las operaciones no seguras reintentables. NB-04, NB-01, NB-02 y NB-06 enmarcan los flujos donde la clave de idempotencia protege los efectos. ADR-08 resuelve la cabecera dedicada para la clave, y RN-07 fija que un reenvío con la misma clave no duplica el efecto, mientras que una reutilización inconsistente se rechaza.

## 3. Criterios de aceptación

- Given una operación registrada bajo una clave con su huella de contenido, When el cliente reenvía la misma clave con la misma huella, Then el sistema devuelve el resultado registrado.
- Given una clave ya registrada con una huella, When el cliente la reenvía con una huella distinta, Then el sistema responde con el código CLAVE_REUTILIZADA_INCONSISTENTE y no aplica el efecto.
- Given una operación en curso bajo una clave, When llega un reintento durante su ejecución, Then el sistema no inicia una segunda ejecución.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-04, NB-01, NB-02, NB-06 |
| CU cubiertos | CU-21 |
| BT derivadas | BT-11 |
| Tests previstos | acceptance/AT-43-clave-inconsistente; contract test de clave reutilizada con huella distinta |

## 5. Prioridad y estimación

Should porque distinguir un reenvío legítimo de una reutilización inconsistente endurece la idempotencia de NB-04, NB-01, NB-02 y NB-06 y detecta errores del cliente, pero la protección básica contra duplicados ya la aporta US-42. 3 SP por Planning Poker (Fibonacci): se apoya en el registro por clave existente y agrega la comparación de la huella del contenido y un código de rechazo estable.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-21)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-11)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La huella del contenido se compara para distinguir un reenvío legítimo de una reutilización inconsistente de la misma clave. Esta historia complementa la aceptación de la clave de idempotencia de US-42, sobre el mismo servicio transversal.
