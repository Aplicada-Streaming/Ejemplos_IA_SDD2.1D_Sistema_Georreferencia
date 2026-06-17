# US-07 — Crear un relevamiento con su tramo vial no vacío

**Proyecto:** geovial-api
**Documento:** US-07-crear-relevamiento-tramo_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-02 Relevamientos y ciclo de vida
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero crear un relevamiento con su tramo vial compuesto por puentes y caminos, para iniciar la unidad de trabajo de campo sobre un tramo concreto.

## 2. Contexto

CU-04 describe la creación de relevamientos como unidad de trabajo de campo, según exige NB-02. RN-05 fija que el relevamiento nace en estado RECOLECCION, y CU-04 impone que el tramo vial sea una composición no vacía de puentes y caminos: no se admite un relevamiento sin tramo. Resuelve el problema de delimitar el alcance físico del trabajo antes de habilitar la recolección.

## 3. Criterios de aceptación

- Given un jefe de área habilitado, When crea un relevamiento con un tramo vial no vacío de puentes y caminos, Then el sistema lo da de alta en estado RECOLECCION y devuelve su representación con el tramo asociado.
- Given un jefe de área habilitado, When intenta crear un relevamiento con un tramo vial vacío, Then el sistema rechaza la operación con el código TRAMO_INCOMPLETO y no crea nada.
- Given un jefe de área que reintenta la creación con la misma clave de idempotencia, When repite la solicitud, Then el sistema no duplica el relevamiento y devuelve la representación del alta ya realizada.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-02 |
| CU cubiertos | CU-04 |
| BT derivadas | BT-02, BT-04, BT-05, BT-06 |
| Tests previstos | acceptance/AT-07-crear-relevamiento; contract test de POST de relevamientos |

## 5. Prioridad y estimación

Must porque el relevamiento es la unidad central de NB-02: sin crearlo no hay recolección, revisión ni cierre posibles. 5 SP por Planning Poker (Fibonacci): suma la composición no vacía del tramo, el estado inicial reglado (RN-05) y la idempotencia del alta sobre un agregado con entidades hijas.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-04)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-05, BT-06)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La visualización y consulta de los relevamientos del alcance se trata en US-08, y la baja de un relevamiento en US-09, para no mezclar alta, consulta y baja en una sola historia. La asignación de agentes al relevamiento se trata en US-10.
