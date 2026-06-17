# US-18 — Comentar y etiquetar fotos y marcadores

**Proyecto:** geovial-api
**Documento:** US-18-comentar-etiquetar-fotos-marcadores_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-03 Marcadores, observaciones y carga manual
**Prioridad MoSCoW:** Should
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como agente de campo, quiero agregar a lo sumo un comentario y etiquetas a una foto y etiquetas a un marcador, para clasificar y describir la evidencia para la revisión.

## 2. Contexto

CU-08 describe la clasificación y descripción de la evidencia capturada. NB-03 fija que la etiqueta es única por relevamiento y reutilizable entre fotos y marcadores, y que se admite a lo sumo un comentario por foto. Resuelve la necesidad de describir y clasificar fotos y marcadores para facilitar la revisión sobre mapa.

## 3. Criterios de aceptación

- Given una foto sin comentario y un conjunto de etiquetas del relevamiento, When un agente de campo agrega un comentario y etiquetas a la foto y etiquetas a un marcador, Then el sistema asocia el comentario único y las etiquetas y devuelve la representación actualizada.
- Given una foto que ya tiene un comentario, When se intenta agregar un segundo comentario a la misma foto, Then el sistema lo rechaza por unicidad y no agrega el segundo comentario.
- Given una etiqueta que pertenece a otro relevamiento, When se intenta aplicar a una foto o marcador, Then el sistema responde con el código FUERA_DE_ALCANCE o RECURSO_NO_ENCONTRADO y no la aplica.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-03 |
| CU cubiertos | CU-08 |
| BT derivadas | BT-05 |
| Tests previstos | acceptance/AT-18-comentar-etiquetar; contract test de PATCH de fotos y marcadores |

## 5. Prioridad y estimación

Should porque comentar y etiquetar mejora la calidad de la revisión, pero no bloquea la captura ni el avance del ciclo de vida. 3 SP por Planning Poker (Fibonacci): la asociación es acotada, con foco en la unicidad del comentario por foto y en el alcance de la etiqueta por relevamiento (NB-03).

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-08)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-05)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La cardinalidad 1—0..1 del comentario por foto es invariante del modelo. La etiqueta es única por relevamiento y reutilizable entre fotos y marcadores; aplicar una etiqueta de otro relevamiento queda fuera de alcance.
