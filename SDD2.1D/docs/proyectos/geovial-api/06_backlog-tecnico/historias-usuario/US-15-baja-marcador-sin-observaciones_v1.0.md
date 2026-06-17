# US-15 — Dar de baja un marcador solo si no tiene observaciones ancladas

**Proyecto:** geovial-api
**Documento:** US-15-baja-marcador-sin-observaciones_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-03 Marcadores, observaciones y carga manual
**Prioridad MoSCoW:** Should
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como agente de campo o jefe de área, quiero dar de baja un marcador que no tiene observaciones ancladas, para limpiar puntos erróneos sin perder evidencia anclada.

## 2. Contexto

CU-07 describe la edición de marcadores, incluida su baja cuando se trata de un punto erróneo. RC-02 fija la referencia obligatoria de toda observación a un marcador, por lo que la baja debe preservar la integridad referencial. RN-03 enmarca que el marcador puede convivir con conflictos sin bloquear. Resuelve la limpieza de puntos sin perder la evidencia que pudiera estar anclada.

## 3. Criterios de aceptación

- Given un marcador sin observaciones ancladas, When un agente de campo o jefe de área solicita su baja, Then el sistema da de baja el marcador y devuelve la confirmación de la operación.
- Given un marcador con observaciones ancladas, When se solicita su baja, Then el sistema la rechaza preservando la referencia obligatoria de RC-02 y no da de baja nada.
- Given un identificador de marcador inexistente, When se solicita su baja, Then el sistema responde con el código RECURSO_NO_ENCONTRADO.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-03 |
| CU cubiertos | CU-07 |
| BT derivadas | BT-05 |
| Tests previstos | acceptance/AT-15-baja-marcador; contract test de DELETE de marcador |

## 5. Prioridad y estimación

Should porque limpiar puntos erróneos mejora la calidad del mapa, pero no bloquea el flujo principal de captura. 3 SP por Planning Poker (Fibonacci): la baja es acotada, con foco en la verificación previa de observaciones ancladas (RC-02) y el manejo del recurso inexistente.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-07)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-05)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La baja segura preserva la integridad referencial: nunca se elimina un marcador que tiene evidencia anclada. Se asume que el recuento de observaciones ancladas está disponible al momento de evaluar la baja.
