# 07 Plan de sprint — aplicada-sync

**Proyecto:** aplicada-sync
**Solución:** GeoVial
**Documento:** README.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Scrum Master + Maintainer Lead (AG-07)

Punto de entrada navegable de la planificación de ejecución del motor de sincronización aplicada-sync (tipo library, redistribuible, release-driven). El trabajo se organiza por versiones publicables y se documenta en modo mini-plan por tratarse de un equipo de un solo desarrollador.

## Modo mini-plan (equipo_n=1)

El equipo de implementación de la solución GeoVial es el Departamento de desarrollo de software con un único desarrollador (intake §2, equipo_n=1). Según §2.1 y §2.2 de `07_rules_plan_sprint.md`, los proyectos de un solo dev reemplazan los cuatro artefactos de sprint por un mini-plan único. En consecuencia, esta sección contiene exclusivamente:

- [mini-plan_v1.0.md](mini-plan_v1.0.md) — plan único condensado release-driven: objetivo de release, ítems comprometidos por tramo (US y BT del backlog de 06), secuencia de construcción por dependencias, DoD por referencia a 08, riesgos con mitigación, criterios de hecho, trazabilidad y bitácora de avance semanal.

## Artefactos de sprint omitidos y por qué

Los siguientes artefactos de la categoría 07 corresponden a equipos de dos o más desarrolladores y se omiten deliberadamente para este proyecto (§2.2, escenario equipo de 1 dev):

| Artefacto omitido | Motivo de la omisión |
| --- | --- |
| `plan-iteracion-sprint-XX_v1.0.md` | No hay ceremonias de planning por sprint con un único dev; el compromiso por versión publicable vive en el mini-plan |
| `template-sprint-review_v1.0.md` | No hay review formal con stakeholders por iteración; el avance se registra en la bitácora del mini-plan |
| `template-sprint-retrospectiva_v1.0.md` | No hay retrospectiva de equipo; la recalibración la hace el propio dev en la bitácora |
| `velocidad-equipo_v1.0.md` | No hay velocity de equipo ni promedio móvil con un solo dev; la capacidad se ajusta tramo a tramo en el mini-plan |

## Trazabilidad upstream/downstream

- Upstream: backlog de 06 (product-backlog_v1.0.md con EP-01 a EP-06 y US-01 a US-13; backlog-tecnico_v1.0.md con BT-01 a BT-14; definition-of-ready_v1.0.md); especificación funcional de 02 (CU-01 a CU-06, RN-01 a RN-03); arquitectura de 05 (ADR-01 a ADR-08); necesidad de negocio NB-04 de 01.
- Downstream: 08 (acceptance tests y DoD canónica que cierra cada US comprometida; hoy pendiente de generar), 09 (DevOps si un tramo introduce cambios de pipeline o publicación), 10 (developer guide si una decisión del release impacta convenciones).

## Convenciones aplicadas

- Nomenclatura `mini-plan_v1.0.md` con un único separador antes de la versión (`_v`); sin doble separador.
- H1 directo seguido del bloque de metadatos; sin `--` previo al H1.
- Cada ítem referencia el identificador exacto del backlog de 06; no se inventan identificadores.
- Estimación en story points con escala Fibonacci, coherente con 06.
- Vocabulario neutral de librería; sin stacks concretos ni productos comerciales en el texto normativo.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 07 de aplicada-sync: declara el modo mini-plan (equipo_n=1), enumera el mini-plan vigente, justifica la omisión de los cuatro artefactos de sprint completos y consolida la trazabilidad upstream/downstream. |
