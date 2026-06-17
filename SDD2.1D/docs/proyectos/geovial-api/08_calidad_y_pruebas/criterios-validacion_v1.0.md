# Criterios de validación — geovial-api

**Proyecto:** geovial-api
**Documento:** criterios-validacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (variante API Testing Specialist)

## 1. Propósito

Este documento define cuándo `geovial-api` se considera validado para release. Reúne los criterios funcionales, no funcionales, de regresión y de calidad de código que, cumplidos en conjunto y medidos en un ambiente equivalente al productivo, permiten declarar al sistema apto para liberar. Es el complemento de release de la Definition of Done por ítem (`definition-of-done_v1.0.md`): la DoD gobierna el cierre de cada US, BT y tramo; estos criterios gobiernan la liberación del incremento. El alcance del MVP son las necesidades NB-01 a NB-05 (Must); las capacidades Could de portabilidad y almacenamiento (NB-06, NB-07) se validan si la cadencia del Tramo 4 las incorpora y, en caso contrario, se difieren sin bloquear el release del MVP.

## 2. Criterios funcionales

- Cada CU crítico (CU-01 a CU-14, CU-18 a CU-22; el MVP) está cubierto por su TC en `casos-prueba-referenciales_v1.0.md` y su TC está en verde.
- Cada criterio Given/When/Then de cada CU del alcance del release pasa: el happy path y el escenario de error/edge declarado en la matriz CU↔Tests (`matriz-cobertura-pruebas_v1.0.md` §2).
- Cada regla de negocio (RN-01 a RN-07) tiene su TC en verde (matriz §4): jerarquía y alcance, conservación de autoría, convivencia con conflictos, priorización de ubicación y radio, transición de estados, orden subir-antes-de-bajar e idempotencia.
- El ciclo completo del relevamiento (crear → asignar → capturar → sincronizar → revisar → resolver conflictos → cerrar) pasa de extremo a extremo (e2e) sin defectos blocker.
- Los CU Could (CU-15, CU-16, CU-17) se validan solo si entran en el alcance del release; si se difieren, queda registrado como deuda planificable sin afectar el MVP.

## 3. Criterios no funcionales

Cada NFR cumple su SLA medido en el ambiente de pruebas equivalente al productivo (intake §17.P.10, 05 §8).

| NFR | SLA | Test / mecanismo | Condición de validación |
| --- | --- | --- | --- |
| Latencia p95 lecturas | ≤ 300 ms | TC-21 (cliente de carga) | p95 medido ≤ 300 ms en ambiente equivalente al productivo |
| Latencia p95 escrituras | ≤ 500 ms | TC-22 (cliente de carga) | p95 medido ≤ 500 ms de extremo a extremo |
| Capacidad de lote de sincronización | ≥ 1000 cambios sin pérdida ni duplicación | TC-31 (carga con lote ≥ 1000) | el lote se aplica una sola vez, sin pérdida; reenvío reconocido |
| Idempotencia de operaciones no seguras | 100 % sin efecto duplicado | TC-29, TC-30 | reintentos por clave y por identificador de origen sin duplicar |
| Integridad de jerarquía y ciclo | 0 violaciones bajo concurrencia | TC-33 | cero estados inválidos de jerarquía, transición o unicidad de asignación |
| Disponibilidad mensual | ≥ 99,5 % | Monitoreo de sondas de salud (09) | métrica observada; sin SLO de 99,9 % (tiene_observabilidad_critica=false) |
| Cobertura (gate de CI) | líneas ≥ 80 %; branches ≥ 70 %; aplicación ≥ 80 %; infra ≥ 70 %; 100 % endpoints con contract test | G3 + G4 + TC-34 | gate del pipeline en verde con cobertura por capa cumplida |

Todo NFR con objetivo numérico se mide; ninguno se declara cumplido por inspección cualitativa.

## 4. Criterios de regresión

- La suite de regresión completa (unit, integración, contrato, e2e) se ejecuta y queda en verde antes del release.
- Ningún test que estaba verde en la versión anterior pasa a rojo sin una justificación registrada (ADR o nota de cambio); esto es el gate G7 de `estrategia-calidad_v1.0.md` §3.
- Todo bug cerrado durante el ciclo generó al menos un TC de regresión que lo previene (08 §5.4); ese TC está en la suite y en verde.
- Los contract tests del 100 % de endpoints (TC-34) pasan, garantizando que ningún cambio rompió el contrato que consumen `geovial-web` y `geovial-mobile`.

## 5. Criterios de calidad de código

- La cobertura por capa cumple los pisos: dominio 85/80, aplicación 80/70, infraestructura 70/60; y el agregado global cumple líneas ≥ 80 % / branches ≥ 70 % (gate G3). Se reporta por capa, no como número global único.
- El 100 % de los endpoints públicos tiene contract test (gate G4) y la especificación OpenAPI valida contra la implementación sin deriva (gate G5).
- El análisis estático no introduce issues críticos nuevos respecto de la línea de base (gate G6).
- La compilación es limpia, sin warnings tratados como error (gate G1).
- Mutation score: no es gate en v1.0; se reporta como métrica observada en dominio cuando la herramienta esté en CI (objetivo informativo ≥ 60 %), registrado como gap planificable en la matriz §6.

## 6. Excepciones documentadas

Cualquier criterio no cumplido al momento del release se acepta solo con una ADR explícita y un plan de remediación con BT asociado (08 §4.7). Casos admitidos:

| Excepción | Condición de aceptación | Aprobador |
| --- | --- | --- |
| CU Could (CU-15, CU-16, CU-17) diferidos | Registrados como deuda planificable; no afectan el MVP (NB-01 a NB-05) | API Product Owner |
| Mutation testing ausente del gate | Reportado como métrica observada; no degrada los gates obligatorios | QA/SDET |
| Umbral de cobertura por capa bajado | Solo con ADR que justifique la baja y BT de remediación (08 §2.2) | QA/SDET + Arquitecto |
| NFR medido fuera de ambiente equivalente al productivo | No admitido para release; el NFR debe remedirse en el ambiente correcto antes de liberar | Arquitecto |

Ninguna excepción exime de los criterios funcionales del MVP ni de los contract tests del 100 % de endpoints: un release del MVP sin estos no se aprueba.

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Criterios de validación iniciales de geovial-api: criterios funcionales (22 CU y 7 RN con TC en verde, e2e del ciclo del relevamiento), no funcionales (NFR numéricos medidos en ambiente equivalente al productivo), de regresión (suite verde, sin regresión injustificada, TC por bug cerrado), de calidad de código (cobertura por capa, contract total, análisis estático) y excepciones documentadas con aprobador. |
