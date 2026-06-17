# Plan de sprint — geovial-web (índice de sección)

**Proyecto:** geovial-web
**Documento:** README.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Scrum Master
**Modo:** mini-plan (equipo_n=1)

Punto de entrada navegable de la planificación de ejecución de `geovial-web`, el front web de los roles administradores de la solución. Tipo de proyecto: web-monolith.

## 1. Modo de la sección

Proyecto de un único desarrollador (equipo_n=1). Conforme a la regla 07 §2.1 y §2.2, la sección opera en modo mini-plan: un único documento condensado (`mini-plan_v1.0.md`) sustituye a los planes de iteración por sprint, las plantillas de sprint review y de retrospectiva y el tracking de velocidad. Esos cuatro artefactos no se generan mientras el equipo sea de un solo dev; si el equipo crece a dos o más desarrolladores, la sección migra al juego completo de artefactos por sprint.

## 2. Artefactos de la sección

| Artefacto | Descripción |
| --- | --- |
| `mini-plan_v1.0.md` | Plan único condensado: objetivo orientado a valor, ítems comprometidos en cinco tramos (US-01 a US-18 y BT-01 a BT-14), trazabilidad por tramo a CU y NB, riesgos con mitigación y bitácora de avance |
| `README.md` | Este índice de la sección |

## 3. Resumen del mini-plan

| Tramo | Foco | Fase roadmap | SP del tramo |
| --- | --- | --- | --- |
| Tramo 1 | Cimientos del front y walking skeleton de acceso y administración de usuarios | F0 | 44 |
| Tramo 2 | Administración completa de usuarios y gestión de relevamientos | F0 / F1 | 21 |
| Tramo 3 | Marcadores sobre el mapa y asignación de agentes | F1 | 27 |
| Tramo 4 | Revisión con carrusel, resolución de conflictos y cierre | F3 | 45 |
| Tramo 5 | Carga manual de evidencia y cierre de alcance | F2 (web) / F3 | 34 |

El primer tramo materializa el walking skeleton de la fase F0 del roadmap (autenticación y administración de usuarios de punta a punta) sobre los cimientos técnicos del front. Los tramos avanzan respetando las dependencias entre BT del backlog técnico y el orden topológico de fases del roadmap.

## 4. Trazabilidad y dependencias

- Upstream: backlog de 06 (`product-backlog_v1.0.md`, `backlog-tecnico_v1.0.md`, `definition-of-ready_v1.0.md`), casos de uso y reglas de negocio de 02 (CU-01 a CU-11, RN-01 a RN-05), arquitectura y decisiones de 05 (ADR-01 a ADR-05) y roadmap de producto de 00_contexto (fases F0 a F3).
- Necesidades de negocio que avanzan: NB-01, NB-02, NB-05, NB-06, NB-07.
- Downstream: la Definition of Done canónica vive en la categoría 08, pendiente de generación; el mini-plan la referencia por adelantado y se vinculará explícitamente al producirse 08. Cada US comprometida dispara su caso de aceptación en 08.

## 5. Definition of Ready y Definition of Done

La entrada de cada ítem al trabajo se gobierna por la `definition-of-ready_v1.0.md` de 06 (cuándo empezar). La terminación se gobierna por la Definition of Done canónica de 08 (cuándo terminar), referenciada y no redefinida en el mini-plan.

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice inicial de la sección 07 de geovial-web en modo mini-plan (equipo_n=1): artefactos de la sección, resumen de los cinco tramos del mini-plan, trazabilidad upstream y downstream, y relación DoR (06) / DoD (08). |
