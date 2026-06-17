# US-08 — Visualizar y consultar los relevamientos del alcance

**Proyecto:** geovial-api
**Documento:** US-08-visualizar-relevamientos-alcance_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-02 Relevamientos y ciclo de vida
**Prioridad MoSCoW:** Must
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero listar y consultar los relevamientos de mi alcance, para tener visibilidad del estado de mi trabajo de campo.

## 2. Contexto

CU-04 incluye la consulta de relevamientos como parte de la gestión exigida por NB-02. RN-01 obliga a acotar al alcance del solicitante antes de paginar, de modo que nunca aparezcan relevamientos de otro ámbito. CU-20 define la paginación y los filtros soportados sobre el listado. Resuelve el problema de que el jefe vea de un vistazo en qué estado está su trabajo de campo sin ver lo ajeno.

## 3. Criterios de aceptación

- Given un jefe de área habilitado, When lista los relevamientos filtrando por estado, Then el sistema devuelve una lista paginada acotada a su alcance y solo con los relevamientos que cumplen el filtro.
- Given un jefe de área que envía un filtro no contemplado, When solicita el listado, Then el sistema rechaza la operación con el código FILTRO_NO_SOPORTADO y no devuelve datos parciales.
- Given un relevamiento que está fuera del alcance del jefe, When intenta consultarlo de forma directa, Then el sistema rechaza la operación con el código FUERA_DE_ALCANCE.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-02 |
| CU cubiertos | CU-04 |
| BT derivadas | BT-05, BT-08, BT-10 |
| Tests previstos | acceptance/AT-08-listar-relevamientos; contract test de GET de relevamientos |

## 5. Prioridad y estimación

Must porque sin visibilidad del estado de los relevamientos el jefe no puede conducir su trabajo de campo; es parte central de NB-02. 3 SP por Planning Poker (Fibonacci): reutiliza la paginación y los filtros de CU-20, y la mayor parte del esfuerzo es aplicar el recorte por alcance (RN-01) antes de paginar.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-04)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-10, BT-08)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La consulta para revisión sobre mapa, con marcadores y conflictos, se trata aparte en US-25 y queda fuera del scope de esta historia. El recorte por alcance (RN-01) se aplica siempre antes de la paginación para evitar fugas de relevamientos ajenos en la cuenta de resultados.
