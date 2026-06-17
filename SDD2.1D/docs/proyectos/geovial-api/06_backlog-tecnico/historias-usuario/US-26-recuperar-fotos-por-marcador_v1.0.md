# US-26 — Recuperar las fotos por marcador para el carrusel de revisión

**Proyecto:** geovial-api
**Documento:** US-26-recuperar-fotos-por-marcador_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-05 Revisión, conflictos y cierre
**Prioridad MoSCoW:** Should
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero recuperar las fotos agrupadas por marcador de un relevamiento, para alimentar el carrusel de fotos de la revisión.

## 2. Contexto

CU-12 describe la consulta del relevamiento para la revisión sobre mapa. NB-05, en su funcionalidad F-12, contempla el carrusel de fotos por marcador como apoyo a esa revisión. ADR-09 resuelve que cada foto se entrega como referencia lógica a través de la abstracción de almacenamiento, sin acoplar el contrato al binario.

## 3. Criterios de aceptación

- Given un jefe de área con un relevamiento dentro de su alcance, When recupera las fotos por marcador, Then el sistema devuelve las fotos agrupadas por marcador con sus referencias y comentarios.
- Given un marcador sin fotos asociadas, When se recuperan sus fotos, Then el sistema devuelve una colección vacía y no un error.
- Given un marcador con muchas fotos, When se recuperan, Then el sistema entrega los resultados paginados con un tamaño de página acotado.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-05 |
| CU cubiertos | CU-12 |
| BT derivadas | BT-05, BT-10, BT-15 |
| Tests previstos | acceptance/AT-26-fotos-por-marcador; contract test de GET de fotos por marcador |

## 5. Prioridad y estimación

Should porque el carrusel de fotos enriquece la revisión, pero la revisión sobre mapa puede operar con marcadores y conflictos aun sin él. 3 SP por Planning Poker (Fibonacci): la agrupación por marcador y la paginación acotada son acotadas, y la entrega como referencia lógica (ADR-09) reutiliza la abstracción de almacenamiento sin manipular binarios.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-12)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-10)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

El backend provee el contrato de datos con las referencias de fotos y sus comentarios; el carrusel visual lo render el cliente. La foto se referencia vía la abstracción de almacenamiento, sin exponer el binario en este contrato.
