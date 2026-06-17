# US-25 — Consultar el relevamiento completo para la revisión sobre mapa

**Proyecto:** geovial-api
**Documento:** US-25-consultar-relevamiento-revision-mapa_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-05 Revisión, conflictos y cierre
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero consultar un relevamiento con sus marcadores y conflictos para revisarlo sobre mapa, para revisar el trabajo de campo antes del cierre.

## 2. Contexto

CU-12 describe la consulta del relevamiento completo como insumo de la revisión sobre mapa. NB-05 enmarca la revisión, conflictos y cierre del trabajo de campo. RN-01 limita la consulta al alcance del solicitante. RN-03 establece que los conflictos son visibles durante la revisión sin bloquear el resto del relevamiento.

## 3. Criterios de aceptación

- Given un jefe de área con un relevamiento dentro de su alcance, When consulta el relevamiento, Then el sistema devuelve sus marcadores y sus conflictos para la revisión sobre mapa.
- Given un relevamiento fuera del alcance del solicitante, When intenta consultarlo, Then el sistema responde con el código FUERA_DE_ALCANCE y no devuelve datos.
- Given un identificador de relevamiento que no existe, When se intenta consultar, Then el sistema responde con el código RECURSO_NO_ENCONTRADO.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-05 |
| CU cubiertos | CU-12 |
| BT derivadas | BT-05, BT-08, BT-10 |
| Tests previstos | acceptance/AT-25-consulta-revision; contract test de GET de relevamiento con marcadores y conflictos |

## 5. Prioridad y estimación

Must porque la consulta del relevamiento completo es el punto de partida de la revisión sobre mapa de NB-05; sin ella el jefe de área no puede evaluar el trabajo antes del cierre. 5 SP por Planning Poker (Fibonacci): integra marcadores y conflictos en una vista coherente, aplica el control de alcance (RN-01) y distingue los rechazos por alcance y por inexistencia con códigos estables.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-12)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-10, BT-08)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

El carrusel de fotos por marcador se trata en US-26. El render visual sobre mapa pertenece al cliente; el backend solo provee el contrato de datos con marcadores y conflictos.
