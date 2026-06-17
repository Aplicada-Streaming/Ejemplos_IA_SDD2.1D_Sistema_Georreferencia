# Calidad y pruebas — geovial-web (índice de sección)

Proyecto: geovial-web
Documento: README.md
Versión: 1.0
Estado: Propuesto
Fecha: 2026-06-15
Autor: Ingeniero QA / SDET (web-monolith)

Punto de entrada navegable de la categoría 08 (calidad y pruebas) de `geovial-web`, el front web de render server-side de los roles administradores de la solución GeoVial. Tipo de proyecto: web-monolith. Variante de especialidad: QA + SDET con pirámide clásica (regla 08 §1.2).

## 1. Artefactos vigentes

| Artefacto | Estado | Descripción |
| --- | --- | --- |
| [estrategia-calidad_v1.0.md](estrategia-calidad_v1.0.md) | Propuesto | Atributos ISO/IEC 25010 priorizados con métricas de origen NFR, quality gates mecánicos, RACI para equipo de un dev y cadencia de revisión |
| [estrategia-testing_v1.0.md](estrategia-testing_v1.0.md) | Propuesto | Pirámide 70/20/10 justificada, cobertura por capa (incl. presentación 60 %), tooling por rol abstracto, fixtures, datos de prueba y ambiente |
| [plan-pruebas_v1.0.md](plan-pruebas_v1.0.md) | Propuesto | Criterios de entrada y salida, riesgos de calidad y plan por los cinco tramos del mini-plan de 07 |
| [matriz-cobertura-pruebas_v1.0.md](matriz-cobertura-pruebas_v1.0.md) | Propuesto | Tres tablas obligatorias (CU↔Tests, NFR↔Tests, RN↔Tests) más cobertura por capa y gaps |
| [casos-prueba-referenciales_v1.0.md](casos-prueba-referenciales_v1.0.md) | Propuesto | Catálogo de 22 TC con setup, pasos Given/When/Then, expected y status; incluye UI/snapshot y NFR de interacción |
| [criterios-validacion_v1.0.md](criterios-validacion_v1.0.md) | Propuesto | Criterios numéricos que habilitan declarar el sistema validado para release |
| [definition-of-done_v1.0.md](definition-of-done_v1.0.md) | Propuesto | DoD canónica por capa (US, BT, sprint/tramo, release); referenciada por el mini-plan de 07 |
| README.md | Propuesto | Este índice de la sección |

## 2. Artefacto omitido

- `guia-testing-extensibilidad_v1.0.md`: NO se genera. El proyecto tiene `tiene_extensibilidad=false` (no expone plugins, extensiones ni handlers externos); la regla 08 §2.2 indica para `web-monolith` "Guía de extensibilidad: No (salvo motor de extensión interno)", y `geovial-web` no tiene motor de extensión interno. La omisión queda registrada aquí por la convención de la sección.

## 3. Resumen de la estrategia

- Pirámide objetivo: 70 % unitario, 20 % integración, 10 % e2e más snapshot (web-monolith, regla 08 §2.2).
- Cobertura por capa: Aplicación de UI 80/70, infraestructura (Cliente de API y adaptador de mapa) 70/60, presentación 60/50; sin capa de Dominio propia (el dominio es de geovial-api).
- Gate global de CI: líneas ≥ 80 %, branches ≥ 70 % (intake §17 geovial-web P.6), reconciliado con los pisos por capa; el global y el por capa se cumplen simultáneamente.
- NFR validados: latencia de interacción p95 ≤ 200 ms, ≥ 50 circuitos concurrentes, custodia del token (0 exposiciones), disponibilidad ≥ 99,5 % (intake §17 geovial-web P.10).

## 4. Quality gates configurados en CI

Compilación sin warnings tratados como error; pruebas unitarias, de integración (a través de la API contra base efímera), de componente de UI (motor headless) y snapshot en verde; gate de cobertura global y por capa; análisis estático sin issues críticos; custodia del token; NFR de interacción y de concurrencia para el release (estrategia-calidad §3). Estos gates se materializan como stages del pipeline en la categoría 09.

## 5. Trazabilidad y vinculación cross-doc

- Upstream: 02 (CU-01 a CU-11 con Given/When/Then, RN-01 a RN-05), 05 (ADR-01 a ADR-05, NFR de quality attributes §8), 06 (Definition of Ready, backlog) y 07 (mini-plan con cinco tramos). Insumo de cabecera: SOLUTION-INTAKE-geovial_v1.0.md §17 geovial-web P.6 y P.10.
- Downstream: 09 ejecuta los quality gates declarados aquí; 10 detalla cómo correr los tests; 11 incluye al menos un test ejecutable por ejemplo.
- DoD canónica: `definition-of-done_v1.0.md` es la fuente única; el mini-plan de 07 la referencia y no la redefine.

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice inicial de la sección 08 de geovial-web: siete artefactos obligatorios del tipo web-monolith, registro de la omisión de guia-testing-extensibilidad por tiene_extensibilidad=false, resumen de pirámide y cobertura, quality gates de CI y vinculación cross-doc upstream (02/05/06/07) y downstream (09/10/11). |
