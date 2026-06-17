# US-27 — Listar los conflictos de marcadores del relevamiento

**Proyecto:** geovial-api
**Documento:** US-27-listar-conflictos-relevamiento_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-05 Revisión, conflictos y cierre
**Prioridad MoSCoW:** Must
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero listar los conflictos de marcadores del relevamiento y su estado, para conocer qué debo resolver antes de cerrar.

## 2. Contexto

CU-13 describe la gestión de conflictos de marcadores durante la revisión. NB-05 enmarca la revisión, conflictos y cierre del trabajo de campo. RN-03 establece que los conflictos conviven con el relevamiento sin bloquearlo. RN-05 fija que su resolución es precondición del cierre, por lo que el listado guía al jefe de área sobre qué falta resolver.

## 3. Criterios de aceptación

- Given un jefe de área con un relevamiento dentro de su alcance, When lista sus conflictos, Then el sistema devuelve los conflictos con su estado pendiente o resuelto.
- Given un relevamiento sin conflictos registrados, When se listan, Then el sistema devuelve una colección vacía y no un error.
- Given un relevamiento fuera del alcance del solicitante, When intenta listar sus conflictos, Then el sistema responde con el código FUERA_DE_ALCANCE.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-05 |
| CU cubiertos | CU-13 |
| BT derivadas | BT-10, BT-14 |
| Tests previstos | acceptance/AT-27-listar-conflictos; contract test de GET de conflictos |

## 5. Prioridad y estimación

Must porque conocer los conflictos pendientes es condición para resolverlos y, con ello, para habilitar el cierre exigido por RN-05. 3 SP por Planning Poker (Fibonacci): es una consulta acotada que lista conflictos con su estado, aplica el control de alcance (RN-01) y devuelve colección vacía cuando no hay conflictos.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-13)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-14)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La resolución de un conflicto se trata en US-28. El cierre del relevamiento exige todos los conflictos resueltos y se detalla en US-29.
