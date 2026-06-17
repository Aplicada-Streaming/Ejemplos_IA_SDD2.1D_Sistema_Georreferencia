# US-40 — Paginar los listados de recursos con referencias de navegación

**Proyecto:** geovial-api
**Documento:** US-40-paginar-listados-navegacion_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-08 Capacidades transversales del contrato
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como cliente consumidor de la API, quiero paginar los listados con tamaño y posición y recibir referencias de navegación, para recorrer grandes conjuntos sin traerlos completos.

## 2. Contexto

CU-20 describe los listados paginados de recursos. NB-02, NB-03 y NB-05 enmarcan los recursos cuyas colecciones pueden crecer y necesitan recorrerse por partes. ADR-04 resuelve la paginación por tamaño y posición de página con referencias de navegación, y RN-01 fija que el alcance se aplica antes de paginar.

## 3. Criterios de aceptación

- Given un listado con más recursos que el tamaño de página, When el cliente pide una página por tamaño y posición, Then el sistema devuelve una página con el tamaño efectivo y referencias a la página siguiente y a la anterior.
- Given una posición de página inválida, When el cliente la solicita, Then el sistema responde con el código POSICION_INVALIDA.
- Given un tamaño de página por encima del tope permitido, When el cliente lo solicita, Then el sistema acota el tamaño al máximo y devuelve la página con ese tamaño.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-02, NB-03, NB-05 |
| CU cubiertos | CU-20 |
| BT derivadas | BT-10 |
| Tests previstos | acceptance/AT-40-paginacion; contract test de listado paginado |

## 5. Prioridad y estimación

Must porque la paginación es transversal a los listados de NB-02, NB-03 y NB-05: sin ella, recorrer colecciones grandes obligaría a traerlas completas, lo que no escala. 5 SP por Planning Poker (Fibonacci): provee un servicio común de paginación con tamaño y posición, acotamiento del tamaño al tope, referencias de navegación y un código de rechazo para la posición inválida.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-20)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-10)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

El filtrado y el orden de los listados se detallan en US-41 y se combinan con esta paginación. El alcance jerárquico se aplica antes de paginar (US-38), de modo que la paginación opera siempre sobre el conjunto ya acotado.
