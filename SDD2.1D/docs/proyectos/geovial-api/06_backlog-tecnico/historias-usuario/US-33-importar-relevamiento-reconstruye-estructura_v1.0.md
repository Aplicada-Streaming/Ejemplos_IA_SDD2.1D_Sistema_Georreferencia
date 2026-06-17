# US-33 — Importar un relevamiento reconstruyendo su estructura

**Proyecto:** geovial-api
**Documento:** US-33-importar-relevamiento-reconstruye-estructura_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-06 Portabilidad del relevamiento
**Prioridad MoSCoW:** Could
**Estimación:** 8 SP (Fibonacci)

## 1. Historia

Como jefe de área o usuario raíz, quiero importar un relevamiento desde una unidad transferible reconstruyendo su estructura, para incorporar un relevamiento producido fuera del sistema.

## 2. Contexto

CU-16 describe la importación de un relevamiento portado en una unidad transferible, reconstruyendo su jerarquía interna. NB-06 enmarca la portabilidad del relevamiento como una capacidad Could Have de la API. RN-01 acota la operación al ámbito jerárquico del solicitante, y RN-07 establece la idempotencia que protege a las importaciones reintentadas.

## 3. Criterios de aceptación

- Given una unidad transferible bien formada con un relevamiento completo, When un jefe de área o usuario raíz solicita la importación, Then el sistema reconstruye el relevamiento, sus marcadores, sus observaciones y sus fotos a partir de la unidad transferible.
- Given una unidad transferible malformada, When se solicita la importación, Then el sistema responde con el código LOTE_MALFORMADO o FORMATO_SOLICITUD_INVALIDO y no incorpora nada.
- Given un solicitante sin rol de jefe de área ni de usuario raíz, When intenta importar, Then el sistema responde con el código ROL_NO_AUTORIZADO y no inicia la importación.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-06 |
| CU cubiertos | CU-16 |
| BT derivadas | BT-05, BT-06, BT-15 |
| Tests previstos | acceptance/AT-33-importar; contract test de POST de importación |

## 5. Prioridad y estimación

Could porque la importación de relevamientos producidos fuera del sistema es una capacidad de portabilidad valiosa pero no crítica para el ciclo central de NB-06; puede planificarse después de cubrir alta, recolección y revisión. 8 SP por Planning Poker (Fibonacci): reconstruir una estructura completa de relevamiento, marcadores, observaciones y fotos desde una unidad transferible, con validación de forma y de rol, implica varias entidades y caminos de rechazo.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-16)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-15, BT-06)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La importación reconstruye la estructura a partir del contenido de la unidad transferible y la incorpora al ámbito del solicitante. La idempotencia de importación, que evita duplicar un relevamiento al reintentar, se detalla en US-34.
