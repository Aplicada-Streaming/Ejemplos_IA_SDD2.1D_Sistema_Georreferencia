# Calidad y pruebas — geovial-mobile (índice de sección)

Proyecto: geovial-mobile
Documento: README.md
Versión: 1.0
Estado: Propuesto
Fecha: 2026-06-15
Autor: Ingeniero QA / SDET (mobile)

Punto de entrada navegable de la categoría 08 (calidad y pruebas) de `geovial-mobile`, la app de campo offline-first de captura de observaciones georreferenciadas con sincronización de la solución GeoVial. Tipo de proyecto: mobile-app-maui. Variante de especialidad: QA + Mobile Testing Specialist (regla 08 §1.2): pruebas de interfaz móvil, snapshot de pantallas críticas, pruebas de ciclo de vida y pruebas del modo offline/sincronización.

## 1. Artefactos vigentes

| Artefacto | Estado | Descripción |
| --- | --- | --- |
| [estrategia-calidad_v1.0.md](estrategia-calidad_v1.0.md) | Propuesto | Atributos ISO/IEC 25010 priorizados con métricas de origen NFR, quality gates mecánicos, RACI para equipo de un dev y cadencia de revisión |
| [estrategia-testing_v1.0.md](estrategia-testing_v1.0.md) | Propuesto | Pirámide 70/15/15 justificada (interfaz móvil en e2e), cobertura por capa (lógica 75 / presentación 60), tooling por rol abstracto, fixtures, datos de prueba y ambiente con modo offline/sincronización |
| [plan-pruebas_v1.0.md](plan-pruebas_v1.0.md) | Propuesto | Criterios de entrada y salida, riesgos de calidad y plan por los tres tramos del mini-plan de 07 |
| [matriz-cobertura-pruebas_v1.0.md](matriz-cobertura-pruebas_v1.0.md) | Propuesto | Tres tablas obligatorias (CU↔Tests, NFR↔Tests, RN↔Tests) más cobertura por capa y gaps |
| [casos-prueba-referenciales_v1.0.md](casos-prueba-referenciales_v1.0.md) | Propuesto | Catálogo de 28 TC con setup, pasos Given/When/Then, expected y status; incluye modo offline, captura georreferenciada y NFR numéricos |
| [criterios-validacion_v1.0.md](criterios-validacion_v1.0.md) | Propuesto | Criterios numéricos que habilitan declarar el sistema validado para release |
| [definition-of-done_v1.0.md](definition-of-done_v1.0.md) | Propuesto | DoD canónica por capa (US, BT, tramo, release); referenciada por el mini-plan de 07 |
| README.md | Propuesto | Este índice de la sección |

## 2. Artefacto omitido

- `guia-testing-extensibilidad_v1.0.md`: NO se genera. El proyecto tiene `tiene_extensibilidad=false` (no expone plugins, extensiones ni handlers externos); la regla 08 §2.2 indica para `mobile-app-maui` "Guía de extensibilidad: No (salvo plugins)", y `geovial-mobile` no admite plugins. La omisión queda registrada aquí por la convención de la sección.

## 3. Resumen de la estrategia

- Pirámide objetivo: 70 % unitario, 15 % integración, 15 % extremo a extremo (interfaz móvil) más snapshot y ciclo de vida en el tramo superior (mobile-app-maui, regla 08 §2.2).
- Cobertura por capa: lógica (Aplicación + Dominio local) 75/70, infraestructura (almacén local, adaptadores de plataforma, cliente REST) 70/60, presentación (vistas, modelos de vista y componente de mapa) 60/50.
- Gate global de CI: líneas ≥ 80 %, branches ≥ 70 % (intake §17 geovial-mobile P.6), reconciliado con los pisos por capa; el global y el por capa se cumplen simultáneamente y el global no compensa una capa por debajo de su piso.
- NFR validados: captura 100 % offline, cola ≥ 1000 cambios, ciclo de 100 cambios ≤ 30 s, reanudación sin pérdida, arranque en frío ≤ 3 s (intake §17 geovial-mobile P.10). Todos con TC ejecutable.

## 4. Quality gates configurados en CI

Compilación sin warnings tratados como error; pruebas unitarias, de integración (almacén local efímero), de interfaz móvil, de modo offline/sincronización y snapshot de pantallas críticas en verde; gate de cobertura global y por capa; análisis estático sin issues críticos; firma del paquete Android con keystore resguardado; NFR de captura offline, cola, ciclo de sincronización, reanudación y arranque para el release (estrategia-calidad §3). Estos gates se materializan como stages del pipeline en la categoría 09.

## 5. Trazabilidad y vinculación cross-doc

- Upstream: 02 (CU-01 a CU-07 con Given/When/Then, RN-01 a RN-05), 05 (ADR-01 a ADR-05, modelo de datos lógico local, flujo de sincronización, NFR de quality attributes §8), 06 (Definition of Ready, product backlog de 15 US, backlog técnico de 13 BT) y 07 (mini-plan con tres tramos). Insumo de cabecera: SOLUTION-INTAKE-geovial_v1.0.md §17 geovial-mobile P.6 y P.10.
- Downstream: 09 ejecuta los quality gates declarados aquí; 10 detalla cómo correr los tests; 11 incluye al menos un test ejecutable por ejemplo.
- DoD canónica: `definition-of-done_v1.0.md` es la fuente única; el mini-plan de 07 la referencia y no la redefine.

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice inicial de la sección 08 de geovial-mobile: siete artefactos obligatorios del tipo mobile-app-maui, registro de la omisión de guia-testing-extensibilidad por tiene_extensibilidad=false (sin plugins), resumen de pirámide 70/15/15 y cobertura por capa (lógica 75 / presentación 60), quality gates de CI con modo offline/sincronización y snapshot de pantallas críticas, y vinculación cross-doc upstream (02/05/06/07) y downstream (09/10/11). |
