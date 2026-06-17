# US-42 — Aceptar una clave de idempotencia en las operaciones no seguras

**Proyecto:** geovial-api
**Documento:** US-42-clave-idempotencia-operaciones-no-seguras_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-08 Capacidades transversales del contrato
**Prioridad MoSCoW:** Must
**Estimación:** 8 SP (Fibonacci)

## 1. Historia

Como cliente consumidor de la API, quiero enviar una clave de idempotencia en las operaciones no seguras reintentables, para reintentar sin miedo a duplicar efectos.

## 2. Contexto

CU-21 describe la idempotencia de las operaciones no seguras reintentables. NB-04, NB-01, NB-02 y NB-06 enmarcan los flujos donde un reintento podría duplicar efectos, como la sincronización y la importación. ADR-08 resuelve recibir la clave de idempotencia en una cabecera dedicada, y RN-07 fija que un reenvío con la misma clave no debe duplicar el efecto.

## 3. Criterios de aceptación

- Given una operación previa registrada bajo una clave de idempotencia, When el cliente la reintenta con la misma clave, Then el sistema devuelve el resultado registrado sin duplicar el efecto.
- Given una operación en curso con una clave de idempotencia, When llega un reintento concurrente con la misma clave, Then el sistema no inicia una segunda ejecución.
- Given una política que exige clave de idempotencia para la operación, When la solicitud llega sin la clave, Then el sistema responde con el código CLAVE_REQUERIDA_AUSENTE.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-04, NB-01, NB-02, NB-06 |
| CU cubiertos | CU-21 |
| BT derivadas | BT-11 |
| Tests previstos | acceptance/AT-42-idempotencia-clave; contract test de operación reintentada con clave |

## 5. Prioridad y estimación

Must porque la idempotencia protege los efectos de los flujos no seguros de NB-04, NB-01, NB-02 y NB-06: sin ella, un reintento de red o un cliente nervioso podría duplicar efectos. 8 SP por Planning Poker (Fibonacci): es un servicio transversal que registra resultados por clave, resuelve la concurrencia para no iniciar segundas ejecuciones y exige la clave cuando la política lo requiere.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-21)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-11)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

El rechazo de una clave reutilizada de forma inconsistente se detalla en US-43. La unicidad de la clave se garantiza con una restricción del almacén relacional, que evita carreras al registrar el primer resultado por clave.
