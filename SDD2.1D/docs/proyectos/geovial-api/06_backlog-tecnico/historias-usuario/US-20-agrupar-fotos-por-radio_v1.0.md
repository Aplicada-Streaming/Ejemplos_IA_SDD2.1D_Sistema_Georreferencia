# US-20 — Agrupar por radio las fotos sin ubicación o cercanas

**Proyecto:** geovial-api
**Documento:** US-20-agrupar-fotos-por-radio_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-03 Marcadores, observaciones y carga manual
**Prioridad MoSCoW:** Should
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como agente de campo, quiero agrupar por radio las fotos cercanas o sin ubicación durante la carga manual, para consolidar evidencia de un mismo punto sin duplicar marcadores.

## 2. Contexto

CU-09 describe la carga manual de fotos al relevamiento y su consolidación en el mapa. NB-03 enmarca la incorporación de evidencia. RN-04 fija el radio de agrupación de fotos durante la carga, y RN-03 establece la convivencia con conflictos que surgen por radio sin bloquear la operación. Resuelve la consolidación de evidencia de un mismo punto evitando marcadores duplicados.

## 3. Criterios de aceptación

- Given un conjunto de fotos dentro del radio de agrupación y un relevamiento activo, When un agente de campo las carga manualmente, Then el sistema las agrupa al mismo marcador y devuelve la representación de la carga consolidada.
- Given una carga manual sin radio definido, When se procesa la agrupación, Then el sistema responde con el código RADIO_NO_DEFINIDO.
- Given dos grupos de fotos dentro del radio, When se procesa la carga, Then el sistema registra el conflicto que convive sin bloquear, conforme a RN-03.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-03 |
| CU cubiertos | CU-09 |
| BT derivadas | BT-16 |
| Tests previstos | acceptance/AT-20-agrupacion-radio; contract test de POST de carga manual con radio |

## 5. Prioridad y estimación

Should porque la agrupación por radio mejora la calidad del mapa y evita marcadores duplicados, pero no bloquea la carga manual básica. 5 SP por Planning Poker (Fibonacci): suma el cálculo de agrupación por radio (RN-04), la validación del radio definido y la convivencia con conflictos por radio sin bloquear (RN-03).

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-09)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-16)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La resolución del conflicto por radio se difiere al cierre del relevamiento (US-28); durante la carga el conflicto convive sin bloquear. Se asume que el radio de agrupación se provee como parámetro de la carga manual.
