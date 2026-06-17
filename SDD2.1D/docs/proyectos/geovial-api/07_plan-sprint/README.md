# 07 Plan de sprint — geovial-api

**Proyecto:** geovial-api
**Documento:** README.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API PM (equipo_n=1)

## 1. Modo de esta sección: mini-plan (equipo_n=1)

El proyecto geovial-api se construye y mantiene por un único desarrollador (equipo_n=1, según SOLUTION-INTAKE §2: Implementador, Departamento de desarrollo de software, 1 dev). Por eso la categoría 07 se genera en MODO MINI-PLAN, conforme a la regla constructiva 07 (`07_rules_plan_sprint.md` §2.1 y §2.2, escenario "equipo de 1 dev").

La categoría 07 no se omite para proyectos de un solo desarrollador: se reduce a un mini-plan documentado (regla 07 §0). El artefacto único de la sección es:

- `mini-plan_v1.0.md` — plan único condensado que combina objetivo de valor, ítems comprometidos por tramos/release, alcance técnico y dependencias, Definition of Done por referencia, trazabilidad a CU y NB por tramo, riesgos con mitigación y bitácora de avance.

## 2. Por qué se omiten los artefactos de sprint completos

La tabla de inclusión por tamaño de equipo (regla 07 §2.2) establece que, para un equipo de un solo desarrollador, el `mini-plan_v1.0.md` sustituye a los cuatro artefactos de sprint completos. En consecuencia, esta sección NO contiene:

- `plan-iteracion-sprint-XX_v1.0.md` — los planes de iteración por sprint se reemplazan por la organización en tramos del mini-plan; no hay ceremonias formales de planning por sprint con un único dev.
- `template-sprint-review_v1.0.md` — sin equipo que demuestre ante un Product Owner separado en una review formal recurrente; el avance se registra en la bitácora del mini-plan.
- `template-sprint-retrospectiva_v1.0.md` — sin retrospectiva de equipo; la mejora continua se gestiona en la calibración de capacidad al cierre de cada tramo.
- `velocidad-equipo_v1.0.md` — sin tracking de velocity por sprint con promedio móvil; la capacidad se calibra con el avance real del único desarrollador, registrado en la bitácora del mini-plan.

Si en el futuro el equipo crece a dos o más desarrolladores, esta sección debe migrar al modo completo: generar los planes de iteración por sprint, las plantillas reusables de review y retrospectiva, y el tracking de velocidad, retirando el mini-plan.

## 3. Índice de la sección

| Documento | Estado | Descripción |
| --- | --- | --- |
| `mini-plan_v1.0.md` | Propuesto | Plan único de construcción en cuatro tramos (F0 a F3) alineados al roadmap de 00 e incrementos de release |

## 4. Trazabilidad

- Upstream: 00 (`roadmap-producto_v1.0.md`, fases F0 a F3 e incrementos), 06 (product backlog US-01 a US-44, backlog técnico BT-01 a BT-21, Definition of Ready), 05 (ADR-01 a ADR-10 y contrato REST), 02 (CU-01 a CU-22, RN, RC), 01 (NB-01 a NB-07).
- Downstream: 08 (`08_calidad_y_pruebas`, Definition of Done canónica y acceptance/contract tests de cada US comprometida) — pendiente de generación; el mini-plan referenciará su DoD por enlace cuando exista.

## 5. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 07 de geovial-api en modo mini-plan (equipo_n=1). Declara el modo aplicado, justifica la omisión de los cuatro artefactos de sprint completos según regla 07 §2.2 e indexa el `mini-plan_v1.0.md`. |
