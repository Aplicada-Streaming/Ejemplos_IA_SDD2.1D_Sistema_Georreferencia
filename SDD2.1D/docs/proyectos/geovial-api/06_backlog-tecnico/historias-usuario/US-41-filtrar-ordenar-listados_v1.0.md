# US-41 — Filtrar y ordenar los listados por criterios soportados

**Proyecto:** geovial-api
**Documento:** US-41-filtrar-ordenar-listados_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-08 Capacidades transversales del contrato
**Prioridad MoSCoW:** Should
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como cliente consumidor de la API, quiero filtrar y ordenar los listados por los criterios soportados, para encontrar recursos por estado o etiqueta sin recorrer todo.

## 2. Contexto

CU-20 describe los listados de recursos, sobre los que se aplican filtros y orden. NB-02, NB-03 y NB-05 enmarcan los recursos cuyas colecciones se consultan. ADR-04 resuelve la paginación y los criterios declarados de filtro y orden, y RN-01 fija que el alcance se aplica antes de cualquier recorte de la colección.

## 3. Criterios de aceptación

- Given un listado con un criterio de filtro y un criterio de orden soportados, When el cliente filtra por estado o etiqueta y ordena por un criterio soportado, Then el sistema devuelve los recursos filtrados y ordenados según lo pedido.
- Given un criterio de filtro no declarado para el recurso, When el cliente lo solicita, Then el sistema responde con el código FILTRO_NO_SOPORTADO.
- Given un criterio de orden no declarado para el recurso, When el cliente lo solicita, Then el sistema responde con el código ORDEN_NO_SOPORTADO.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-02, NB-03, NB-05 |
| CU cubiertos | CU-20 |
| BT derivadas | BT-10 |
| Tests previstos | acceptance/AT-41-filtros-orden; contract test de listado filtrado y ordenado |

## 5. Prioridad y estimación

Should porque filtrar y ordenar mejora notablemente la experiencia de consumo de los listados de NB-02, NB-03 y NB-05, pero la paginación por sí sola ya permite recorrer las colecciones; puede entregarse después del núcleo. 3 SP por Planning Poker (Fibonacci): se apoya en el servicio de listados existente, agrega validación contra los criterios declarados y dos códigos de rechazo estables.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-20)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-10)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

Los criterios de filtro y orden soportados por cada recurso se declaran en el contrato, de modo que cualquier criterio fuera de esa lista se rechaza. El filtrado y el orden son combinables con la paginación de US-40 sobre el mismo conjunto ya acotado por alcance.
