# US-14 — Crear y mover marcadores geográficos con identidad estable

**Proyecto:** geovial-api
**Documento:** US-14-crear-mover-marcadores_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-03 Marcadores, observaciones y carga manual
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como agente de campo o jefe de área, quiero crear marcadores y mover o etiquetar uno sin perder su identidad, para registrar puntos del mapa estables que agrupan observaciones.

## 2. Contexto

CU-07 describe el alta y la edición de marcadores sobre el mapa del relevamiento. NB-03 enmarca la captura de marcadores, observaciones y evidencia. RC-01 garantiza la identidad estable del marcador ante el movimiento o el etiquetado, y RN-03 fija que los marcadores conviven con conflictos sin bloquear la operación. Resuelve la necesidad de tener puntos del mapa estables a los que anclar la evidencia.

## 3. Criterios de aceptación

- Given un relevamiento activo, When un agente de campo o jefe de área crea un marcador y luego lo mueve o lo etiqueta, Then el sistema conserva el mismo identificador del marcador y devuelve su representación actualizada.
- Given un marcador existente, When se crea otro marcador dentro de su radio, Then el sistema registra el conflicto sin bloquear el alta, conforme a RN-03.
- Given un alta de marcador con un identificador de origen ya usado, When se repite el alta, Then el sistema no duplica el marcador y devuelve la representación del alta ya realizada.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-03 |
| CU cubiertos | CU-07 |
| BT derivadas | BT-02, BT-05, BT-14 |
| Tests previstos | acceptance/AT-14-marcadores; contract test de POST y PATCH de marcadores |

## 5. Prioridad y estimación

Must porque el marcador es el punto del mapa al que se anclan observaciones y evidencia; sin él no hay captura georreferenciada (NB-03). 5 SP por Planning Poker (Fibonacci): suma el alta, el movimiento y el etiquetado preservando la identidad estable (RC-01), la convivencia con conflictos por radio (RN-03) y la deduplicación por identificador de origen.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-07)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-05)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La baja de marcador se trata en US-15 para no mezclar alta y edición con baja en una sola historia. El conflicto por radio convive sin bloquear y se resuelve al cierre (US-28). Se asume que el identificador de origen lo provee el cliente como marca opaca de deduplicación.
