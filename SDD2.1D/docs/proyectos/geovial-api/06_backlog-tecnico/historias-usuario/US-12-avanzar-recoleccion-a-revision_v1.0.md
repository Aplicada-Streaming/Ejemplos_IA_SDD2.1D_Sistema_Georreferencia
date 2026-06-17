# US-12 — Avanzar el relevamiento de recolección a revisión

**Proyecto:** geovial-api
**Documento:** US-12-avanzar-recoleccion-a-revision_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-02 Relevamientos y ciclo de vida
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero transicionar un relevamiento de recolección a revisión, para pasar al control sobre mapa cuando la recolección terminó.

## 2. Contexto

CU-06 describe el avance del ciclo de vida del relevamiento operado por el jefe de área. NB-02 enmarca el ciclo de vida del relevamiento y sus etapas. RN-05 fija que la transición se hace sin saltos de estado, y RC-04 garantiza que solo se admiten transiciones hacia un estado válido. Resuelve el cierre operativo de la etapa de captura para habilitar el control sobre mapa.

## 3. Criterios de aceptación

- Given un relevamiento en estado RECOLECCION operado por un jefe de área habilitado, When solicita la transición a REVISION, Then el sistema deja el relevamiento en REVISION y devuelve su representación con el nuevo estado.
- Given un relevamiento en un estado desde el cual la transición a REVISION no está permitida, When se solicita la transición, Then el sistema la rechaza con el código TRANSICION_NO_PERMITIDA y no cambia el estado.
- Given un relevamiento ya en REVISION, When se repite la transición a REVISION, Then el sistema responde de forma idempotente sin alterar el estado ni romper.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-02 |
| CU cubiertos | CU-06 |
| BT derivadas | BT-02, BT-06, BT-14 |
| Tests previstos | acceptance/AT-12-transicion-revision; contract test de POST de transiciones |

## 5. Prioridad y estimación

Must porque sin la transición a revisión no se puede cerrar la etapa de captura ni habilitar el control sobre mapa, eje del ciclo de vida de NB-02. 5 SP por Planning Poker (Fibonacci): la transición en sí es acotada, pero suma la validación de estado válido (RC-04), el respeto de la transición sin saltos (RN-05) y el manejo idempotente de la repetición.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-06)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-06)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

El retorno de revisión a recolección se trata en US-13 para no mezclar avance y retroceso en una sola historia. El cierre del relevamiento se trata en US-29. Se asume que la autorización del jefe de área sobre su alcance ya está resuelta aguas arriba.
