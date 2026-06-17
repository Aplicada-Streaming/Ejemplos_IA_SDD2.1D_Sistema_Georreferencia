# US-44 — Versionar el contrato público y preservar la compatibilidad

**Proyecto:** geovial-api
**Documento:** US-44-versionar-contrato-preservar-compatibilidad_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-08 Capacidades transversales del contrato
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como cliente consumidor de la API, quiero consumir el contrato bajo un prefijo de versión mayor con compatibilidad preservada, para no romperme cuando el contrato evoluciona.

## 2. Contexto

CU-22 describe el versionado del contrato público y la convivencia entre versiones. NB-01 a NB-05 enmarcan las capacidades de negocio expuestas que deben evolucionar sin romper a los clientes. ADR-10 resuelve el versionado por URI con un prefijo de versión mayor en la ruta, y el contrato REST en su sección 6 fija las reglas de compatibilidad y convivencia.

## 3. Criterios de aceptación

- Given un cambio compatible dentro de la misma versión mayor, When un cliente existente consume el contrato, Then el cambio no rompe a los clientes y sigue respondiendo bajo el mismo prefijo de versión mayor.
- Given una versión mayor retirada o inexistente en la ruta, When el cliente la solicita, Then el sistema responde con el código VERSION_NO_SOPORTADA.
- Given un recurso ausente en la versión indicada, When el cliente lo solicita bajo ese prefijo, Then el sistema responde con el código RECURSO_NO_EN_VERSION.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-01 a NB-05 |
| CU cubiertos | CU-22 |
| BT derivadas | BT-17, BT-18, BT-19 |
| Tests previstos | acceptance/AT-44-versionado; contract test de cambio compatible e incompatible |

## 5. Prioridad y estimación

Must porque el versionado por prefijo de versión mayor protege la evolución de todo el contrato de NB-01 a NB-05: sin él, cualquier cambio incompatible rompería a los clientes en producción. 5 SP por Planning Poker (Fibonacci): introduce el enrutamiento por prefijo de versión mayor, la convivencia de versiones y los códigos de rechazo para versión y recurso no disponibles en la versión indicada.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-22)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-17, BT-19)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

Una versión mayor nueva conserva la anterior durante el período de convivencia, y las deprecaciones se anuncian antes del retiro. El versionado protege a geovial-web y geovial-mobile, que consumen el contrato bajo el prefijo de versión mayor en la ruta.
