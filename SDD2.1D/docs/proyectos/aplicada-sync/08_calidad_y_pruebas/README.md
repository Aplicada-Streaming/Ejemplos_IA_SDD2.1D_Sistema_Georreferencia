# 08 Calidad y pruebas — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** README.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (AG-08), variante QA + SDET Library

Índice navegable de los artefactos de calidad y pruebas del motor de sincronización `aplicada-sync` (tipo `library`, redistribuible). La librería es transformación entrada-salida sobre una superficie pública: se valida por su contrato y por las garantías de su motor (orden subir-luego-bajar, idempotencia, reanudación sin pérdida, convivencia con conflicto), sin UI ni ambiente desplegable propio.

## Artefactos vigentes

| Documento | Propósito | Estado |
| --- | --- | --- |
| [estrategia-calidad_v1.0.md](estrategia-calidad_v1.0.md) | Definición de calidad, atributos ISO 25010 priorizados, quality gates G1-G9, RACI y cadencia | Propuesto |
| [estrategia-testing_v1.0.md](estrategia-testing_v1.0.md) | Pirámide 80/15/5, cobertura por capa, tooling, property-based y mutation, fixtures, datos y ambiente | Propuesto |
| [plan-pruebas_v1.0.md](plan-pruebas_v1.0.md) | Alcance, criterios de entrada/salida, riesgos de calidad y plan por los tramos R1/R2/R3 | Propuesto |
| [matriz-cobertura-pruebas_v1.0.md](matriz-cobertura-pruebas_v1.0.md) | Tres tablas obligatorias (CU↔Tests, NFR↔Tests, RN↔Tests) más cobertura por capa y gaps | Propuesto |
| [casos-prueba-referenciales_v1.0.md](casos-prueba-referenciales_v1.0.md) | Catálogo TC-01 a TC-21 con setup, pasos Given/When/Then, expected, actual y status | Propuesto |
| [criterios-validacion_v1.0.md](criterios-validacion_v1.0.md) | Criterios numéricos para declarar el paquete validado/publicable | Propuesto |
| [definition-of-done_v1.0.md](definition-of-done_v1.0.md) | DoD canónica por capa (US, BT, tramo, release); referenciada por el mini-plan de 07 | Propuesto |
| [guia-testing-extensibilidad_v1.0.md](guia-testing-extensibilidad_v1.0.md) | Cómo testear los puntos de extensión (almacén local, transporte, credencial, conectividad) sin tocar el núcleo | Propuesto |

## Quality gates configurados (materializados en 09)

| Gate | Condición | Consecuencia |
| --- | --- | --- |
| G1 | Compila sin advertencias tratadas como error | Bloquea merge y publicación |
| G2 | Suite unitaria verde, ningún test sin assert | Bloquea merge |
| G3 | Contract tests por interfaz de extensión verdes | Bloquea merge |
| G4 | Cobertura por capa (dominio 85/80, infra 70/60) y global 80/70 (intake §17 P.6) | Bloquea merge |
| G5 | Mutation score del dominio >= 60 % | Bloquea release |
| G6 | Property-based de orden, idempotencia y no duplicación verdes | Bloquea merge |
| G7 | NFR numéricos dentro de SLA (lote 100 <= 30 s, cola >= 1000, 0/0 en reanudación) | Bloquea release |
| G8 | Compatibilidad de superficie pública y verificación post-publicación | Bloquea publicación |
| G9 | Análisis estático sin issues críticos | Bloquea merge |

## DoD canónica del proyecto

La Definition of Done canónica vive en [definition-of-done_v1.0.md](definition-of-done_v1.0.md). El mini-plan de release de la categoría 07 la referencia; no la redefine.

## Reconciliación de coberturas

El gate global del intake §17 P.6 (>= 80 % líneas / >= 70 % branches) y las coberturas por capa de 08 (dominio 85/80, infraestructura 70/60) son compatibles: el dominio se exige por encima del piso global y la infraestructura al piso de su capa, de modo que cumplir las coberturas por capa implica cumplir el gate global. La cobertura se reporta por capa, nunca como número global único.

## Trazabilidad upstream/downstream

- Upstream: 02 (CU-01 a CU-06 con Given/When/Then, RN-01 a RN-03), 05 (arquitectura, ADR-01 a ADR-08, contratos-abstractions, extensibilidad, quality attributes §8), 06 (DoR, US-01 a US-13, BT-01 a BT-14), 07 (mini-plan de release), intake §17 P.6/P.10.
- Downstream: 09 (los gates G1-G9 se ejecutan como stages del pipeline), 10 (developer guide de cómo correr la suite), 11 (al menos un test ejecutable por sample, incluido el demo MAUI de extensibilidad).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 08 de aplicada-sync con el índice de los ocho artefactos de calidad (siete obligatorios más la guía de extensibilidad), el resumen de los quality gates G1-G9, el enlace a la DoD canónica y la reconciliación de coberturas con el gate global del intake §17 P.6. |
