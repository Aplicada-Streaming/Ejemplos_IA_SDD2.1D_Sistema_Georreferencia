# Plan de pruebas — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** plan-pruebas_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (AG-08), variante QA + SDET Library

## 1. Alcance del plan

Este plan cubre la validación de la librería `aplicada-sync` a lo largo de los tres tramos de release del mini-plan de 07 (R1 MVP, R2 disparo automático y observabilidad, R3 errores y compatibilidad publicable). La unidad de trabajo es la versión publicable, no el sprint timeboxed, en coherencia con el modo release-driven y `equipo_n=1`.

Módulos incluidos:

- Superficie pública (capa Abstractions): las seis operaciones del ciclo de vida (CU-01 a CU-06) y los cuatro contratos de extensión.
- Núcleo del motor: cola única por identificador, orquestador del ciclo subir-luego-bajar, ejecutores de fase, marca de progreso y reanudación, observador de conectividad, registro de estado, catálogo de errores.
- Invariantes RN-01 (orden), RN-02 (idempotencia) y RN-03 (convivencia con conflicto).
- NFR numéricos del intake §17 P.10.

Módulos excluidos (fuera del contrato de la librería, por pertenecer al host o al backend):

- La autenticación y la emisión de credenciales (el motor reutiliza el token del host).
- La semántica de dominio de cada cambio local (carga útil opaca).
- La resolución de conflictos del lado del backend (el motor convive y reporta, no resuelve, RN-03).
- La interacción visual con la persona usuaria (categoría 03).
- El stack concreto del runtime y la implementación física del almacén local del host (los TC usan dobles).

## 2. Criterios de entrada

El plan se ejecuta para un tramo cuando:

- El build compila sin advertencias tratadas como error (gate G1).
- Las BT de soporte del tramo, según el mini-plan de 07 §3, tienen su superficie implementada y los dobles de las estrategias de extensión disponibles (requisito de la DoR de 06 §1 criterio 7: datos o dobles de prueba disponibles).
- Los criterios de aceptación Given/When/Then de las US del tramo están redactados y son testables (DoR de 06 §1 criterio 3; revisión AG-08).
- El módulo de soporte de tests (dobles centralizados, fixtures) está disponible y versionado.

## 3. Criterios de salida

El plan se declara ejecutado con éxito para un tramo cuando:

- Cobertura por capa alcanzada: dominio >= 85 % líneas / >= 80 % branches; infraestructura >= 70 % / >= 60 %; global >= 80 % / >= 70 % (gate G4; intake §17 P.6).
- Mutation score del dominio >= 60 % (gate G5).
- 100 % de los CU del tramo con al menos un TC verde por cada criterio Given/When/Then.
- Cada RN aplicable al tramo verificada por al menos un TC verde.
- Cada NFR con objetivo numérico cuyo módulo entra en el tramo tiene su TC de medición verde (gate G7).
- Cero defectos blockers abiertos (definidos en §4 como defectos de integridad de datos o de violación de invariante).
- Suite de regresión verde: ningún TC verde del tramo anterior pasó a rojo sin justificación documentada.
- Para R3 / release: verificación post-publicación reproduce el contrato (gate G8, BT-14).

## 4. Riesgos de calidad

Alineados con los riesgos arquitectónicos de 05 §9 y los riesgos del mini-plan de 07 §6.

| Riesgo de calidad | Impacto | Probabilidad | Mitigación |
| --- | --- | --- | --- |
| Defecto de idempotencia o de orden que produzca pérdida o duplicación de datos sin ser detectado (blocker) | Alto | Media | Property-based testing de invariantes (TC-12, TC-13) más mutation testing en el dominio; ningún release pasa con G5 o G6 en rojo; cada defecto de integridad genera un TC de regresión |
| Reanudación que reenvía o aplica dos veces tras un corte (US-12/US-13, las más caras del MVP) | Alto | Alta | TC de reanudación con cortes en posición arbitraria (TC-08, TC-09) y comparación del conjunto aplicado contra el esperado; almacén persistente efímero real para la marca de progreso |
| Cambio incompatible silencioso de la superficie pública que rompa consumidores externos | Alto | Media | Snapshot del contrato (resumen, estado, conjunto de errores) y matriz de compatibilidad; gate G8 bloquea la publicación; verificación post-publicación en proyecto limpio (BT-14) |
| Disparos concurrentes por rebote de conectividad que generen ciclos paralelos | Medio | Media | TC de exclusión mutua y de descarte de rebote (TC-10, TC-11) con fuente de conectividad de prueba que emite eventos en ráfaga |
| Cola que degrada por encima del volumen objetivo (>= 1000) | Medio | Baja | TC de capacidad con generador determinista de 1000 cambios (TC-14); se mide tamaño reportado y correcto encolado/consulta/ejecución |
| Suite no determinista por dependencia de orden o de reloj que enmascare un defecto | Medio | Media | Aislamiento por test, dobles reinstanciados, semillas fijas; revisión de que ningún TC dependa del orden de ejecución (08_rules §5.4) |
| Con `equipo_n=1` no hay revisión por pares que detecte un test sin assert o un catch silencioso | Medio | Media | Gate G2 rechaza tests sin assert; análisis estático (G9); mutation testing penaliza tests que ejecutan sin verificar |

## 5. Plan por tramo de release

El plan se organiza por los tres tramos del mini-plan de 07, no por sprint timeboxed. El recurso es el desarrollador único en su rol AG-08.

| Tramo | Alcance de testing | Recursos | Entregables de calidad |
| --- | --- | --- | --- |
| R1 (MVP) | Inicialización (CU-01), encolado y no duplicación (CU-02, RN-02), ciclo subir-luego-bajar y orden (CU-03, RN-01), convivencia con conflicto (CU-03, RN-03), reanudación sin pérdida (CU-06). TC-01 a TC-09, TC-12, TC-13, TC-16. NFR de orden, idempotencia, reanudación | Dev único (AG-08); dobles de almacén/transporte/credencial; almacén persistente efímero | Suite unitaria y de contrato verde; property-based de orden e idempotencia; cobertura por capa de R1; matriz CU/RN -> TC actualizada |
| R2 (disparo, observabilidad, endurecimiento) | Exclusión mutua (CU-03 5.C), disparo por conectividad y descarte de rebote (CU-04), consulta de estado y conflictos (CU-05), recuperación de sesión persistida (CU-01 5.A), nuevo corte en reanudación (CU-06 5.B). TC-10, TC-11, TC-15, TC-17, TC-18, TC-09 | Dev único; doble de fuente de conectividad con eventos en ráfaga | TC de conectividad y de estado verdes; regresión de R1 verde; cobertura por capa de R2 |
| R3 (errores, capacidad, compatibilidad) | Catálogo estable de errores (BT-13), volumen objetivo de cola >= 1000 (NFR Capacidad, BT-05), tiempo de lote de 100 (NFR Tiempo), compatibilidad de superficie pública con quick-start (BT-14). TC-14, TC-19, TC-20, TC-21 | Dev único; generador de 1000 cambios; backend de prueba con latencia móvil simulada; proyecto limpio para post-publicación | NFR numéricos verdes; snapshot del contrato y matriz de compatibilidad; verificación post-publicación; mutation score de release |

## 6. Recursos

- Personas: un desarrollador en su rol AG-08 (diseño, implementación y aprobación de tests; `equipo_n=1`).
- Ambientes: ejecución local y en el pipeline de CI (categoría 09); sin ambiente desplegado propio (la librería corre embebida en un host). Almacén persistente efímero temporal para los TC de reanudación; backend de prueba en proceso que simula latencia móvil y corte para los TC de ciclo y de NFR.
- Datasets: sintéticos y deterministas (§6 de `estrategia-testing_v1.0.md`); generador de cola de >= 1000 cambios con semilla fija; conjuntos property-based regenerados por semilla.
- Herramientas: frameworks por rol abstracto (tests unitarios, property-based, snapshot, mutation, cobertura por capa, benchmark/carga) según `estrategia-testing_v1.0.md` §3.

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Plan de pruebas inicial de aplicada-sync: alcance por superficie pública y núcleo del motor con exclusiones del host/backend, criterios de entrada y salida ligados a los gates G1/G4/G5/G7/G8 y al intake §17 P.6, siete riesgos de calidad alineados con 05 §9 y 07 §6, plan por los tres tramos de release del mini-plan de 07 (R1/R2/R3) con TC asignados, y recursos para equipo_n=1. Derivado de 02, 05, 06, 07 y de las reglas 08 §4.4. |
