# US-21 — Subir el lote de cambios locales del agente

**Proyecto:** geovial-api
**Documento:** US-21-subir-lote-cambios-locales_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-04 Sincronización sin conexión
**Prioridad MoSCoW:** Must
**Estimación:** 8 SP (Fibonacci)

## 1. Historia

Como cliente de campo del agente, quiero subir el lote de cambios locales del relevamiento asignado, para incorporar al backend el trabajo capturado sin conexión.

## 2. Contexto

CU-10 describe la subida del lote de cambios locales en el flujo de sincronización sin conexión. NB-04 enmarca la sincronización sin conexión del trabajo de campo. RN-06 fija subir antes de bajar, RN-07 deduplica por identificador de origen y RN-03 registra los conflictos sin bloquear. La capacidad del lote contempla un volumen mayor o igual a 1000 cambios. Resuelve la incorporación al backend del trabajo capturado fuera de línea.

## 3. Criterios de aceptación

- Given un cliente de campo asignado a un relevamiento abierto y un lote bien formado, When sube el lote de cambios locales, Then el sistema aplica el lote una sola vez, registra los conflictos sin bloquear y devuelve el resultado de la subida.
- Given un lote que no respeta el formato esperado, When se intenta subir, Then el sistema responde con el código LOTE_MALFORMADO y no aplica cambios parciales.
- Given un relevamiento cerrado, When se intenta subir un lote a ese relevamiento, Then el sistema responde con el código RELEVAMIENTO_CERRADO y no aplica el lote.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-04 |
| CU cubiertos | CU-10 |
| BT derivadas | BT-11, BT-12, BT-21 |
| Tests previstos | acceptance/AT-21-subida-lote; contract test de POST de subida de sincronización |

## 5. Prioridad y estimación

Must porque la subida del lote es el punto de entrada del trabajo capturado sin conexión al backend, eje de NB-04. 8 SP por Planning Poker (Fibonacci): suma la aplicación exactamente una vez con deduplicación por identificador de origen (RN-07), el orden subir antes de bajar (RN-06), el registro de conflictos sin bloquear (RN-03), la capacidad de lote mayor o igual a 1000 cambios y el manejo de lote malformado y relevamiento cerrado.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-10)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-11, BT-12)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La reanudación de una subida interrumpida se trata en US-22. El agente debe estar asignado al relevamiento; en caso contrario el sistema responde con el código RELEVAMIENTO_NO_ASIGNADO. Se asume que el identificador de origen de cada cambio actúa como marca opaca de deduplicación.
