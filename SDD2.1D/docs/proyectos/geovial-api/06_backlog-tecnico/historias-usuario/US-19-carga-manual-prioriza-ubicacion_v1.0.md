# US-19 — Cargar fotos manualmente priorizando la ubicación incrustada

**Proyecto:** geovial-api
**Documento:** US-19-carga-manual-prioriza-ubicacion_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-03 Marcadores, observaciones y carga manual
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como agente de campo, quiero cargar fotos manualmente priorizando la ubicación incrustada de cada foto, para ubicar la evidencia en el mapa sin re-tomarla en campo.

## 2. Contexto

CU-09 describe la carga manual de fotos al relevamiento. NB-03 enmarca la incorporación de evidencia al mapa. RN-04 fija que la carga manual prioriza la ubicación incrustada de cada foto y no inventa coordenadas cuando esa señal está ausente. Resuelve la incorporación de evidencia ya tomada sin necesidad de volver al terreno.

## 3. Criterios de aceptación

- Given una foto con ubicación incrustada y un relevamiento activo, When un agente de campo la carga manualmente, Then el sistema la ubica en el mapa usando la ubicación incrustada y devuelve la representación de la carga.
- Given una foto sin ubicación incrustada, When se la carga manualmente, Then el sistema la deja pendiente de ubicación manual sin inventar coordenada, conforme a RN-04.
- Given una carga manual sin radio definido cuando este es requerido, When se procesa la carga, Then el sistema responde con el código RADIO_NO_DEFINIDO.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-03 |
| CU cubiertos | CU-09 |
| BT derivadas | BT-15, BT-16 |
| Tests previstos | acceptance/AT-19-carga-manual; contract test de POST de carga manual |

## 5. Prioridad y estimación

Must porque la carga manual con ubicación incrustada permite incorporar evidencia sin volver al terreno, sostén operativo de NB-03. 5 SP por Planning Poker (Fibonacci): suma la lectura y priorización de la ubicación incrustada (RN-04), el manejo de la foto sin señal de ubicación y la validación del radio requerido.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-09)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-16)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La agrupación por radio de las fotos cargadas se trata en US-20. El tratamiento de la foto sin señal de ubicación se apoya en el supuesto de la §9 de la especificación funcional, a confirmar con el negocio; mientras tanto no se inventan coordenadas.
