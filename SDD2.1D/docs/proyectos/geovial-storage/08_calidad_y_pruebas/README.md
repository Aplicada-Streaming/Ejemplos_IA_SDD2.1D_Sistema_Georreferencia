# 08 Calidad y pruebas — geovial-storage

**Proyecto:** geovial-storage
**Tipo (D8):** library
**Variante:** Ingeniero QA / SDET Senior (QA + SDET Library)
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior

Punto de entrada navegable de la sección 08 de `geovial-storage`, la librería que expone al backend de GeoVial una abstracción de alojamiento de archivos transparente con proveedores intercambiables (local / remoto / otro) seleccionables por el usuario raíz. La sección define la estrategia de calidad y de testing, el plan de pruebas, la matriz de cobertura, el catálogo de casos de prueba, los criterios de validación, la Definition of Done canónica y la guía de testing de extensibilidad. La pirámide objetivo es 80 unit / 15 integration / 5 e2e+snapshot (regla 08 §2.2 para `library`).

## Documentos de la sección

| Documento | Estado | Descripción |
| --- | --- | --- |
| [estrategia-calidad_v1.0.md](estrategia-calidad_v1.0.md) | Propuesto | Atributos ISO/IEC 25010 priorizados, nueve quality gates, RACI para equipo_n=1 y cadencia de revisión. |
| [estrategia-testing_v1.0.md](estrategia-testing_v1.0.md) | Propuesto | Pirámide 80/15/5, cobertura por capa, tooling por rol abstracto, contract tests por proveedor, property-based, snapshot del catálogo de errores y política de dobles/fixtures. |
| [plan-pruebas_v1.0.md](plan-pruebas_v1.0.md) | Propuesto | Alcance, criterios de entrada/salida, seis riesgos de calidad y plan de testing por los cinco tramos del mini-plan de 07. |
| [matriz-cobertura-pruebas_v1.0.md](matriz-cobertura-pruebas_v1.0.md) | Propuesto | Tres tablas obligatorias (CU↔Tests, NFR↔Tests, RN↔Tests) más cobertura por capa con reconciliación del gate global. |
| [casos-prueba-referenciales_v1.0.md](casos-prueba-referenciales_v1.0.md) | Propuesto | Catálogo de 28 TC con setup, pasos Given-When-Then, expected y status. |
| [criterios-validacion_v1.0.md](criterios-validacion_v1.0.md) | Propuesto | Criterios numéricos funcionales, no funcionales, de regresión y de calidad de código para declarar el sistema validado. |
| [definition-of-done_v1.0.md](definition-of-done_v1.0.md) | Propuesto | DoD canónica por capa (US, BT, sprint, release); fuente única referenciada por el mini-plan de 07. |
| [guia-testing-extensibilidad_v1.0.md](guia-testing-extensibilidad_v1.0.md) | Propuesto | Suite de conformidad de proveedor para testear un proveedor nuevo contra el puerto sin tocar el núcleo. |

## Quality gates configurados

| Gate | Condición | Consecuencia |
| --- | --- | --- |
| G-01 | Compilación sin warnings-as-errors | Bloquea merge |
| G-02 | Unit y contract verdes | Bloquea merge |
| G-03 | Cobertura global ≥ 80 % líneas / ≥ 70 % branches (intake §17 P.6) | Bloquea merge |
| G-04 | Cobertura por capa (dominio 85/80, infraestructura 70/60) | Bloquea merge |
| G-05 | Mutation score dominio ≥ 60 % | Bloquea release |
| G-06 | Transparencia: batería única equivalente por proveedor | Bloquea merge |
| G-07 | 0 filtración de credenciales | Bloquea merge |
| G-08 | Latencia p95 ≤ 1 s para ≤ 5 MB local | Bloquea release |
| G-09 | Análisis estático sin issues críticos | Bloquea merge |

El detalle de cada gate vive en `estrategia-calidad_v1.0.md` §3; su materialización como stages del pipeline pertenece a 09.

## DoD canónica

La Definition of Done canónica del proyecto es [definition-of-done_v1.0.md](definition-of-done_v1.0.md). El mini-plan de 07 la referencia y no la redefine; su nota de pendencia queda satisfecha por este artefacto.

## Cobertura por capa (objetivo)

| Capa | Líneas | Branches | Mutation |
| --- | --- | --- | --- |
| Dominio (Abstracciones, Núcleo, Registro, Resguardo) | ≥ 85 % | ≥ 80 % | ≥ 60 % |
| Infraestructura (adaptadores de proveedor) | ≥ 70 % | ≥ 60 % | — |
| Global agregado (gate de intake) | ≥ 80 % | ≥ 70 % | — |

Reconciliación: los umbrales por capa de la regla 08 y el gate global de intake §17 P.6 son compatibles; ver `matriz-cobertura-pruebas_v1.0.md` §5.

## Trazabilidad

- Upstream: CU-01 a CU-06 y RN-01 a RN-03 (02); NFR-01 a NFR-06, ADR-01 a ADR-05, `contratos-abstractions_v1.0.md` y `extensibilidad_v1.0.md` (05); US-01 a US-09, BT-01 a BT-13, DoR (06); mini-plan y sus cinco tramos (07); intake §17 P.6 y P.10.
- Downstream: 09 (gates en el pipeline CI/CD), 10 (developer guide de testing), 11 (un test ejecutable por ejemplo).
- Cada TC referencia al menos un CU, RN o NFR; la matriz tiene las tres tablas obligatorias más la cobertura por capa.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 08 de geovial-storage: índice de los siete artefactos obligatorios más la guía de extensibilidad, quality gates configurados, DoD canónica, cobertura por capa objetivo y trazabilidad upstream/downstream. |
