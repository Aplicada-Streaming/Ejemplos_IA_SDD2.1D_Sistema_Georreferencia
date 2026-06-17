# US-16 — Crear una observación anclada a un marcador existente

**Proyecto:** geovial-api
**Documento:** US-16-crear-observacion-anclada_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-03 Marcadores, observaciones y carga manual
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como agente de campo, quiero crear una observación con nota anclada a un marcador existente, para registrar lo observado en un punto concreto del relevamiento.

## 2. Contexto

CU-08 describe el alta de observaciones sobre un marcador del relevamiento. NB-03 enmarca la captura de evidencia anclada al mapa. RC-02 fija el ancla obligatoria de toda observación a un marcador existente, y RN-02 garantiza que la autoría de la observación se conserva. Resuelve el registro de lo observado en un punto concreto sin perder la trazabilidad del autor.

## 3. Criterios de aceptación

- Given un marcador existente en un relevamiento activo, When un agente de campo crea una observación con nota anclada a ese marcador, Then el sistema crea la observación con su autor y devuelve su representación.
- Given un identificador de marcador inexistente, When se intenta anclar una observación, Then el sistema responde con el código RECURSO_NO_ENCONTRADO y no crea la observación.
- Given un alta de observación con un identificador de origen ya usado, When se repite el alta, Then el sistema no duplica la observación y devuelve la representación del alta ya realizada.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-03 |
| CU cubiertos | CU-08 |
| BT derivadas | BT-02, BT-05 |
| Tests previstos | acceptance/AT-16-crear-observacion; contract test de POST de observaciones |

## 5. Prioridad y estimación

Must porque la observación anclada es el registro nuclear de lo capturado en campo (NB-03); sin ella no hay evidencia que revisar. 5 SP por Planning Poker (Fibonacci): suma la verificación del ancla obligatoria a marcador existente (RC-02), la conservación de la autoría (RN-02) y la deduplicación por identificador de origen.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-08)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-05)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

Las fotos de la observación se tratan en US-17 y los comentarios y etiquetas en US-18, para no mezclar el alta de la observación con su evidencia visual y su clasificación. Se asume que el identificador de origen lo provee el cliente como marca opaca de deduplicación.
