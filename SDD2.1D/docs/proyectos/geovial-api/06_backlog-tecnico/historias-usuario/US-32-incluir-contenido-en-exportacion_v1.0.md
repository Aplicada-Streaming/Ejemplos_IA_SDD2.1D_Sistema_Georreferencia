# US-32 — Incluir comentarios, etiquetas y fotos en la exportación

**Proyecto:** geovial-api
**Documento:** US-32-incluir-contenido-en-exportacion_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-06 Portabilidad del relevamiento
**Prioridad MoSCoW:** Could
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero que la exportación incluya los comentarios, etiquetas y fotos del relevamiento, para exportar la evidencia completa sin pérdida.

## 2. Contexto

CU-15 describe la exportación de un relevamiento como unidad transferible. NB-06 enmarca la portabilidad del relevamiento, que solo es valiosa si arrastra la evidencia asociada. ADR-09 resuelve que las fotos se referencian a través de la abstracción de almacenamiento, de modo que el binario se resuelve vía la librería de almacenamiento al armar la unidad.

## 3. Criterios de aceptación

- Given un relevamiento con comentarios, etiquetas y fotos dentro del alcance, When se exporta, Then la unidad transferible incluye los comentarios, las etiquetas y las referencias de las fotos.
- Given una foto cuyo binario no resuelve en el proveedor de almacenamiento, When se exporta, Then el sistema responde con el código PROVEEDOR_NO_DISPONIBLE y no produce una unidad incompleta de forma silenciosa.
- Given un relevamiento sin fotos, When se exporta, Then el sistema produce una unidad transferible válida con sus comentarios y etiquetas.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-06 |
| CU cubiertos | CU-15 |
| BT derivadas | BT-15 |
| Tests previstos | acceptance/AT-32-exportar-contenido; contract test de POST de exportación con contenido |

## 5. Prioridad y estimación

Could porque completa la portabilidad Could Have de NB-06: sin la evidencia asociada la exportación pierde sentido, pero sigue fuera del flujo principal. 5 SP por Planning Poker (Fibonacci): suma la inclusión de comentarios, etiquetas y referencias de fotos, la resolución del binario vía la abstracción de almacenamiento (ADR-09) y el manejo del proveedor no disponible.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-15)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-15)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

El binario de cada foto se resuelve vía la librería de almacenamiento detrás de la abstracción de almacenamiento. La importación inversa de una unidad transferible se trata en US-33.
