# US-23 — Bajar las actualizaciones del relevamiento posteriores a la marca

**Proyecto:** geovial-api
**Documento:** US-23-bajar-actualizaciones-posteriores-marca_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-04 Sincronización sin conexión
**Prioridad MoSCoW:** Must
**Estimación:** 8 SP (Fibonacci)

## 1. Historia

Como cliente de campo del agente, quiero bajar las novedades del relevamiento posteriores a mi marca de sincronización, para traer al dispositivo solo lo que cambió desde la última sincronización.

## 2. Contexto

CU-11 describe la bajada incremental de novedades del relevamiento en el flujo de sincronización sin conexión. NB-04 enmarca la sincronización sin conexión del trabajo de campo. RN-06 exige que la bajada se atienda solo tras concluir la subida del ciclo. RC-06 define la marca de sincronización como opaca y monótona, de modo que el dispositivo solo trae lo posterior a su marca y recibe una marca nueva.

## 3. Criterios de aceptación

- Given un cliente con una marca de sincronización válida y la subida del ciclo concluida, When solicita la bajada, Then el sistema entrega las novedades posteriores a la marca y devuelve una marca nueva opaca.
- Given una marca de sincronización que no es interpretable por el backend, When se solicita la bajada, Then el sistema responde con el código MARCA_INVALIDA y no entrega novedades.
- Given un cliente cuya subida del ciclo no concluyó, When solicita la bajada, Then el sistema responde con el código SUBIDA_NO_CONCLUIDA y no entrega novedades.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-04 |
| CU cubiertos | CU-11 |
| BT derivadas | BT-12, BT-13 |
| Tests previstos | acceptance/AT-23-bajada-incremental; contract test de POST de bajada de sincronización |

## 5. Prioridad y estimación

Must porque la bajada incremental cierra el ciclo de sincronización sin conexión de NB-04 y evita arrastrar todo el relevamiento en cada intercambio. 8 SP por Planning Poker (Fibonacci): el cálculo incremental sobre los índices de novedades, la emisión de una marca opaca y monótona (RC-06), la validación de la marca de entrada y la compuerta de subida concluida (RN-06) concentran complejidad de dominio y de contrato.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-11)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-13)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

El cálculo incremental usa los índices de novedades del modelo lógico. La marca de sincronización es opaca y monótona; el cliente no debe interpretar su contenido. El rechazo por orden subir antes de bajar se detalla en US-24.
