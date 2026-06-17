# US-13 — Retornar el relevamiento de revisión a recolección

**Proyecto:** geovial-api
**Documento:** US-13-retornar-revision-a-recoleccion_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-02 Relevamientos y ciclo de vida
**Prioridad MoSCoW:** Should
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero retornar un relevamiento de revisión a recolección, para reabrir la captura si la revisión detecta faltantes.

## 2. Contexto

CU-06 describe el manejo del ciclo de vida del relevamiento por parte del jefe de área, incluido el retorno de etapa cuando la revisión encuentra faltantes. RN-05 fija que la transición se hace sin saltos de estado y RC-04 garantiza que solo se admiten transiciones hacia un estado válido. Resuelve la necesidad de reabrir la captura sin tener que crear un relevamiento nuevo.

## 3. Criterios de aceptación

- Given un relevamiento en estado REVISION operado por un jefe de área habilitado, When solicita el retorno a RECOLECCION, Then el sistema deja el relevamiento en RECOLECCION y devuelve su representación con el nuevo estado.
- Given un relevamiento que no está en REVISION, When se solicita el retorno a RECOLECCION, Then el sistema lo rechaza con el código RELEVAMIENTO_NO_EN_REVISION o TRANSICION_NO_PERMITIDA y no cambia el estado.
- Given un relevamiento ya en RECOLECCION, When se repite el retorno a RECOLECCION, Then el sistema responde de forma idempotente sin alterar el estado ni romper.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-02 |
| CU cubiertos | CU-06 |
| BT derivadas | BT-06, BT-14 |
| Tests previstos | acceptance/AT-13-retorno-recoleccion; contract test de POST de transiciones |

## 5. Prioridad y estimación

Should porque el retorno de etapa mejora el ciclo de trabajo cuando la revisión detecta faltantes, pero no bloquea el flujo principal de avance del relevamiento. 3 SP por Planning Poker (Fibonacci): reusa la maquinaria de transiciones de la transición a revisión, agregando la validación del estado de origen (RC-04, RN-05) y el manejo idempotente.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-06)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-06)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La reapertura desde cierre se trata en US-30 para no mezclar el retorno entre etapas activas con la reapertura de un relevamiento ya cerrado. Se asume que la autorización del jefe de área sobre su alcance ya está resuelta aguas arriba.
