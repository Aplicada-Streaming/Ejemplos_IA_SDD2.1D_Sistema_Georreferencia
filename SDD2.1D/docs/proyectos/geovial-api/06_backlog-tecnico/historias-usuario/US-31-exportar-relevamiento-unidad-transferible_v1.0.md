# US-31 — Exportar un relevamiento completo en una unidad transferible

**Proyecto:** geovial-api
**Documento:** US-31-exportar-relevamiento-unidad-transferible_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-06 Portabilidad del relevamiento
**Prioridad MoSCoW:** Could
**Estimación:** 8 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero exportar un relevamiento completo en una unidad transferible única, para mover o respaldar el relevamiento fuera del sistema.

## 2. Contexto

CU-15 describe la exportación de un relevamiento como unidad transferible. NB-06 enmarca la portabilidad del relevamiento como capacidad Could Have del alcance. RN-01 limita la exportación a los relevamientos dentro del alcance del solicitante, de modo que el jefe de área solo puede exportar lo que le corresponde.

## 3. Criterios de aceptación

- Given un jefe de área con un relevamiento dentro de su alcance, When solicita la exportación, Then el sistema produce una unidad transferible única del relevamiento.
- Given un relevamiento fuera del alcance del solicitante, When intenta exportarlo, Then el sistema responde con el código FUERA_DE_ALCANCE y no produce la unidad.
- Given un identificador de relevamiento que no existe, When se intenta exportar, Then el sistema responde con el código RECURSO_NO_ENCONTRADO.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-06 |
| CU cubiertos | CU-15 |
| BT derivadas | BT-05, BT-15 |
| Tests previstos | acceptance/AT-31-exportar; contract test de POST de exportación |

## 5. Prioridad y estimación

Could porque la portabilidad del relevamiento es capacidad Could Have de NB-06: aporta valor de respaldo y movilidad, pero no condiciona el flujo principal. 8 SP por Planning Poker (Fibonacci): consolidar un relevamiento completo en una unidad transferible única implica recorrer y agregar sus componentes, aplicar el control de alcance (RN-01) y manejar los rechazos por alcance e inexistencia.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-15)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-15)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

El formato físico de empaquetado de la unidad transferible pertenece al stack y no se fija en esta historia. El contenido detallado de la exportación (comentarios, etiquetas, fotos) se trata en US-32.
