# 08 Calidad y pruebas — geovial-api

**Proyecto:** geovial-api
**Documento:** README.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (variante API Testing Specialist)

## 1. Propósito

Índice navegable de los artefactos de calidad y pruebas de `geovial-api`, el backend monolítico (`rest-api`) y proyecto principal de la solución GeoVial. La categoría 08 ancla la disciplina de validación: estrategia de calidad, estrategia de testing, plan de pruebas, matriz de cobertura, catálogo de casos de prueba, criterios de validación y la Definition of Done canónica del proyecto.

## 2. Artefactos vigentes

| Artefacto | Estado | Descripción |
| --- | --- | --- |
| `estrategia-calidad_v1.0.md` | Propuesto | Atributos ISO/IEC 25010 priorizados con métricas, ocho quality gates (G1–G8) y reconciliación del gate global con la cobertura por capa, RACI y cadencia. |
| `estrategia-testing_v1.0.md` | Propuesto | Pirámide 70/20/10 justificada, cobertura mínima por capa, tooling por rol abstracto, mocks/fixtures, datos de prueba y ambiente con base de datos efímera. |
| `plan-pruebas_v1.0.md` | Propuesto | Alcance, criterios de entrada/salida, riesgos de calidad y plan por los cuatro tramos del mini-plan de 07. |
| `matriz-cobertura-pruebas_v1.0.md` | Propuesto | Tres tablas de trazabilidad (CU↔Tests con los 22 CU, NFR↔Tests, RN↔Tests con las 7 RN) más cobertura por capa e inventario del 100 % de endpoints con contract test. |
| `casos-prueba-referenciales_v1.0.md` | Propuesto | Catálogo TC-01 a TC-35 con setup, pasos Given/When/Then, expected, actual y status; incluye el contract test total (TC-34) de los 35 endpoints. |
| `criterios-validacion_v1.0.md` | Propuesto | Criterios funcionales, no funcionales, de regresión y de calidad de código para declarar el sistema validado para release. |
| `definition-of-done_v1.0.md` | Propuesto | DoD canónica por capa (US, BT, sprint→tramo, release); fuente única referenciada por el mini-plan de 07. |
| `README.md` | Propuesto | Este índice. |

## 3. Artefacto omitido

- `guia-testing-extensibilidad_v1.0.md`: omitido. El proyecto declara `tiene_extensibilidad=false` (intake §17.P.2 y composición de la solución): `geovial-api` no expone handlers ni middlewares externos publicados para consumo de terceros; sus componentes transversales (autorización, errores, paginación, idempotencia, versionado) son internos. La regla 08 §2.2 (`rest-api`) condiciona este artefacto a "si admite handlers o middlewares externos"; al no admitirlos, no aplica. Si en una versión futura el backend habilitara puntos de extensión externos, se generaría este artefacto en esa iteración.

## 4. Quality gates configurados en CI

Definidos en `estrategia-calidad_v1.0.md` §3 y materializados en el pipeline de 09 (BT-20):

| Gate | Condición resumida | Bloquea |
| --- | --- | --- |
| G1 | Compilación sin warnings tratados como error | Merge |
| G2 | Suite en verde, sin tests sin assert | Merge |
| G3 | Cobertura por capa (dominio 85/80, aplicación 80/70, infra 70/60) y global (líneas ≥ 80 % / branches ≥ 70 %) | Merge |
| G4 | 100 % de endpoints públicos con contract test | Merge |
| G5 | OpenAPI valida contra implementación, sin deriva | Merge |
| G6 | Análisis estático sin issues críticos nuevos | Merge |
| G7 | Sin regresión injustificada | Release |
| G8 | NFR numéricos medidos y cumplidos en ambiente equivalente al productivo | Release |

## 5. Enlace a la DoD canónica

La Definition of Done canónica del proyecto es `definition-of-done_v1.0.md`. Es la única fuente; el mini-plan de 07 (`07_plan-sprint/mini-plan_v1.0.md` §5) la referencia por enlace y no la redefine.

## 6. Trazabilidad upstream

- 02 (especificación funcional): 22 CU con Given/When/Then, 7 RN, 6 RC, modelo conceptual de 12 entidades.
- 05 (arquitectura técnica): NFR numéricos (§8), ADR-01 a ADR-10, contrato REST de 35 endpoints, modelo de datos lógico de 17 tablas.
- 06 (backlog técnico): DoR, 44 US, 21 BT.
- 07 (plan de sprint): mini-plan con cuatro tramos (F0–F3) en modo equipo_n=1.
- Intake: `SOLUTION-INTAKE-geovial_v1.0.md` §17 P.6 (cobertura) y P.10 (NFR).

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 08 de geovial-api: índice de los siete artefactos obligatorios más este README, registro de la omisión de la guía de testing de extensibilidad por tiene_extensibilidad=false, tabla de quality gates y enlace a la DoD canónica. |
