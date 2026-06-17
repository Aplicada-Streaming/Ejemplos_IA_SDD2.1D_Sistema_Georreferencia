# 06 Backlog técnico — geovial-api

**Proyecto:** geovial-api
**Tipo:** rest-api
**Versión vigente:** 1.0
**Fecha:** 2026-06-15
**Titular:** Scrum Master + API Product Owner

Punto de entrada navegable del backlog de geovial-api. Recibe upstream de 01 (NB-01 a NB-07), 02 (CU-01 a CU-22, RN-01 a RN-07, RC-01 a RC-06) y 05 (ADR-01 a ADR-10, contrato REST, modelo lógico). Alimenta 07 (sprint plan) y 08 (acceptance y contract tests).

## Documentos de la sección

| Documento | Contenido |
| --- | --- |
| [product-backlog_v1.0.md](product-backlog_v1.0.md) | Índice maestro priorizado: objetivos, 8 épicas, 44 US por épica, métricas MoSCoW y refinement. |
| [backlog-tecnico_v1.0.md](backlog-tecnico_v1.0.md) | Vista técnica: 8 épicas técnicas, 21 BT inline y matriz BT↔US↔CU. |
| [definition-of-ready_v1.0.md](definition-of-ready_v1.0.md) | Criterios DoR para US (7) y BT (5), excepciones y aprobador. |
| `historias-usuario/` | 44 historias individuales `US-XX-<kebab>_v1.0.md`. |

No existe carpeta `tareas-tecnicas/` en v1.0: las 21 BT viven inline en `backlog-tecnico_v1.0.md` por estar por debajo del umbral de 30 (regla 06 §3.3). Se creará si una versión futura supera ese umbral.

## Modo de archivos aplicado (umbrales regla 06 §3.3)

| Ítem | Cantidad | Umbral | Modo aplicado |
| --- | --- | --- | --- |
| Historias de usuario (US) | 44 | más de 20 → archivos individuales obligatorios | Archivos individuales bajo `historias-usuario/US-XX-<kebab>_v1.0.md`. |
| Tareas técnicas (BT) | 21 | más de 30 → archivos individuales | Inline en `backlog-tecnico_v1.0.md` (21 < 30; el rango 15-30 admite inline conservando la estructura de secciones). |

## Resumen de épicas

| Épica | Nombre | US | Épica técnica afín |
| --- | --- | --- | --- |
| EP-01 | Usuarios, sesión y autorización | US-01 a US-06, US-37, US-38 | EP-T1, EP-T3 |
| EP-02 | Relevamientos y ciclo de vida | US-07 a US-13 | EP-T2 |
| EP-03 | Marcadores, observaciones y carga manual | US-14 a US-20 | EP-T2, EP-T6 |
| EP-04 | Sincronización sin conexión | US-21 a US-24 | EP-T5 |
| EP-05 | Revisión, conflictos y cierre | US-25 a US-30 | EP-T5 |
| EP-06 | Portabilidad del relevamiento | US-31 a US-34 | EP-T6 |
| EP-07 | Configuración de almacenamiento | US-35, US-36 | EP-T6 |
| EP-08 | Capacidades transversales del contrato | US-39 a US-44 | EP-T4, EP-T7 |

Épicas técnicas: EP-T1 fundaciones de capas, EP-T2 persistencia y migraciones, EP-T3 autenticación y autorización, EP-T4 comunicación transversal, EP-T5 sincronización/idempotencia/conflictos, EP-T6 almacenamiento de archivos, EP-T7 versionado/contrato/contract tests, EP-T8 calidad y observabilidad.

## US Must Have del MVP

El MVP es el camino end-to-end de NB-01 a NB-05 más las transversales Must. US Must (27): US-01, US-02, US-03, US-04, US-05, US-07, US-08, US-10, US-12, US-14, US-16, US-17, US-19, US-21, US-22, US-23, US-24, US-25, US-27, US-28, US-29, US-37, US-38, US-39, US-40, US-42, US-44.

Las US Could (US-30, US-31 a US-36) componen la deuda planificable de las épicas EP-06 y EP-07, a incorporar si la cadencia lo permite.

## BT prioritarias

Fundaciones bloqueantes del MVP: BT-01 (capas), BT-02 (dominio), BT-04 (esquema y migración M0001), BT-05 (repositorios), BT-07 (token), BT-08 (autorización), BT-09 (errores), BT-11 (idempotencia), BT-12 (subida de sync), BT-19 (contract tests), BT-20 (gate de cobertura).

## DoR vigente

Siete criterios para US y cinco para BT (`definition-of-ready_v1.0.md`). Aprobador: API Product Owner, con revisiones acotadas de AG-02 (trazabilidad CU), AG-05 (justificación técnica) y AG-08 (verificabilidad). La DoR habla de cuándo empezar; la Definition of Done de 08 habla de cuándo terminar.

## Distribución MoSCoW

| Prioridad | US | % |
| --- | --- | --- |
| Must | 27 | 61,4 % |
| Should | 10 | 22,7 % |
| Could | 7 | 15,9 % |
| Won't (v1.0) | 0 | 0 % |

## Convenciones

Identificadores de dos dígitos uniformes (US-XX, BT-XX, EP-XX, EP-TX); estimación Fibonacci; archivos `_v1.0.md` con guion bajo; slugs en kebab-lowercase; UTF-8 LF; fechas YYYY-MM-DD; sin stacks concretos ni productos comerciales.
