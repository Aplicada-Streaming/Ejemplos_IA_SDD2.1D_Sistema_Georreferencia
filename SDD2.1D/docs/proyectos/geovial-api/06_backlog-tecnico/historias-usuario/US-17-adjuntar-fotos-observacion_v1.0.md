# US-17 — Adjuntar fotos a una observación delegando el binario al almacén

**Proyecto:** geovial-api
**Documento:** US-17-adjuntar-fotos-observacion_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-03 Marcadores, observaciones y carga manual
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como agente de campo, quiero adjuntar fotos a una observación, para aportar evidencia visual del punto observado.

## 2. Contexto

CU-08 describe la incorporación de fotos a una observación del relevamiento. NB-03 enmarca la captura de evidencia visual anclada al mapa. ADR-09 fija que el binario de la foto se delega a la abstracción de almacenamiento y que el almacén relacional persiste solo la referencia lógica. Resuelve el aporte de evidencia visual sin cargar el binario en el almacén relacional.

## 3. Criterios de aceptación

- Given una observación existente y el proveedor de almacenamiento activo disponible, When un agente de campo adjunta una foto, Then el sistema persiste la referencia lógica, deja el binario en el proveedor activo y devuelve la representación de la foto adjunta.
- Given el proveedor de almacenamiento no disponible, When se intenta adjuntar una foto, Then el sistema responde con el código PROVEEDOR_NO_DISPONIBLE y no persiste referencia lógica huérfana.
- Given un adjunto de foto con un identificador de origen ya usado, When se repite el adjunto, Then el sistema no duplica la foto y devuelve la representación del adjunto ya realizado.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-03 |
| CU cubiertos | CU-08 |
| BT derivadas | BT-02, BT-05, BT-15 |
| Tests previstos | acceptance/AT-17-adjuntar-foto; contract test de POST de fotos |

## 5. Prioridad y estimación

Must porque la foto es la evidencia visual central de la observación (NB-03); sin ella la revisión pierde respaldo. 5 SP por Planning Poker (Fibonacci): suma la delegación del binario a la abstracción de almacenamiento (ADR-09), el manejo del proveedor no disponible y la deduplicación por identificador de origen.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-08)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-15)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

El binario nunca vive en el almacén relacional: solo se persiste la referencia lógica a la abstracción de almacenamiento. Los comentarios y etiquetas de la foto se tratan en US-18 para no mezclar el adjunto con su clasificación.
