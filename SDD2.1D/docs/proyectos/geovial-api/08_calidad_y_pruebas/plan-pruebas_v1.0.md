# Plan de pruebas — geovial-api

**Proyecto:** geovial-api
**Documento:** plan-pruebas_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (variante API Testing Specialist)

## 1. Alcance del plan

Este plan operativo cubre la construcción completa de `geovial-api` organizada en los cuatro tramos del mini-plan (07, `mini-plan_v1.0.md`), alineados a las fases F0 a F3 del roadmap. Cubre los 22 casos de uso (CU-01 a CU-22), las siete reglas de negocio (RN-01 a RN-07), el contrato REST completo (35 endpoints públicos en 9 áreas) y los NFR numéricos del intake §17.P.10.

Incluido en el plan:

- Pruebas unitarias de dominio (invariantes RN/RC) y de casos de uso de aplicación.
- Pruebas de integración contra base de datos efímera (persistencia, transacciones, restricciones).
- Contract tests del 100 % de endpoints públicos por versión mayor.
- Pruebas e2e de los journeys críticos del ciclo del relevamiento.
- Pruebas de rendimiento de los NFR numéricos (latencia p95, lote de sincronización ≥ 1000).
- Pruebas de seguridad de autorización jerárquica (acceso fuera de alcance, escalada de privilegios) y de conservación de autoría.

Excluido del plan (alcance de otras categorías o proyectos):

- El render del mapa, el carrusel de fotos y la interacción visual: pertenecen a `geovial-web` y `geovial-mobile`; este plan prueba el contrato de datos que el backend provee (CU-12).
- La librería de sincronización del cliente (`aplicada-sync`) y la de almacenamiento (`geovial-storage`): tienen su propio 08; este plan prueba la integración del backend con ellas a través de sus contratos.
- El testing de extensibilidad: omitido por `tiene_extensibilidad=false` (no hay handlers ni middlewares externos publicados); no se produce `guia-testing-extensibilidad`.
- La observabilidad crítica (SLO ≥ 99,9 %, latencia p99): fuera de alcance por `tiene_observabilidad_critica=false` (intake §17.P.10).

## 2. Criterios de entrada

El plan, o el tramo de plan correspondiente, se ejecuta cuando:

- El build de la rama está verde (G1, G2 de `estrategia-calidad_v1.0.md` §3).
- Las US y BT del tramo cumplen la Definition of Ready (06, `definition-of-ready_v1.0.md`): criterios de aceptación en Given/When/Then, trazabilidad a CU declarada, fixtures e insumos de contrato identificados.
- La base de datos efímera y los fixtures del tramo están disponibles y siembran sin error.
- El contrato REST (o el subconjunto del tramo) está materializado lo suficiente para escribir los contract tests; a partir del Tramo 4 existe la especificación OpenAPI versionada (BT-18).
- Los secretos no productivos del ambiente de pruebas están inyectados desde el entorno.

## 3. Criterios de salida

El plan, o el tramo, se declara ejecutado con éxito cuando:

- Toda la suite del tramo pasa en verde (G2) y ningún test verde de la revisión anterior pasó a rojo sin justificación (G7).
- La cobertura por capa del código del tramo cumple los pisos (dominio 85/80, aplicación 80/70, infraestructura 70/60) y el agregado global cumple líneas ≥ 80 % / branches ≥ 70 % (G3).
- El 100 % de los endpoints públicos introducidos en el tramo tiene al menos un contract test en verde (G4); al cierre del Tramo 4, el 100 % de los 35 endpoints del contrato está cubierto.
- Ningún defecto de severidad blocker queda abierto; los defectos no-blocker tienen TC de regresión y plan registrado.
- Los NFR numéricos cuyo soporte se completa en el tramo se midieron y cumplen en el ambiente equivalente al productivo (G8); el conjunto completo se valida antes del release (ver `criterios-validacion_v1.0.md`).
- Los criterios de transición de fase del roadmap (00 §5) del incremento correspondiente se satisfacen (07 §8).

## 4. Riesgos de calidad

Alineados con los riesgos arquitectónicos de 05 §9 y los del mini-plan 07 §7.

| Riesgo de calidad | Impacto | Probabilidad | Mitigación de testing |
| --- | --- | --- | --- |
| Pérdida o duplicación de datos en la sincronización (cortes, reenvíos, lote ≥ 1000) | Alto | Media | Contract y e2e de subida/bajada con reenvío y corte simulado (TC-17, TC-18, TC-19); prueba de carga del lote ≥ 1000 (TC-31); idempotencia por identificador de origen y clave (TC-29, TC-30). Spike de BT-11/BT-12 al inicio del Tramo 3 |
| Escalada de privilegios por fallo de autorización jerárquica | Alto | Media | Tests de acceso fuera de alcance y de salto de nivel para cada rol (TC-25, TC-26); pruebas de concurrencia sobre la integridad de jerarquía (TC-33) |
| Estado de relevamiento inconsistente (transición inválida, cierre con conflictos) | Alto | Baja | Tests de transición no permitida (TC-08), de cierre rechazado con conflictos pendientes (TC-14) y de restricción de estado a nivel del almacén |
| Incumplimiento del objetivo de latencia p95 por listados sin acotar o sin índice | Medio | Media | Prueba de carga de lecturas y escrituras (TC-21, TC-22); verificación de que la paginación y el alcance se aplican antes de servir (TC-23, TC-24) |
| Rotura silenciosa de `geovial-web`/`geovial-mobile` por cambio incompatible del contrato | Medio | Baja | Contract tests del 100 % de endpoints por versión (TC-34) y test de cambio compatible que no rompe (TC-35); validación de OpenAPI contra implementación (G5) |
| Cobertura global que esconde una capa débil | Medio | Media | Reporte por capa con umbrales diferenciados (G3); revisión de la matriz al cierre de cada tramo |
| Falta de regresión al cerrar un bug | Medio | Media | Todo bug cerrado genera o extiende un TC antes de declararse cerrado (DoD US y BT) |
| Tests no deterministas (flaky) por orden o estado compartido | Medio | Baja | Base efímera por suite, fixtures aislados, prohibición de dependencia de orden (estrategia-testing §5 y §7) |

## 5. Plan por tramo

El trabajo se organiza por tramos de construcción (07 §3), no por sprints de calendario, por ser un proyecto de un único desarrollador sin fecha objetivo. Cada tramo cierra por criterios de transición verificables.

| Tramo (fase) | Alcance de testing | Recursos | Entregables de testing |
| --- | --- | --- | --- |
| Tramo 1 — Esqueleto, autenticación y jerarquía (F0) | Unit de dominio de jerarquía e invariantes (RN-01, RN-02); unit de autorización por rol y alcance, errores problem+json y versionado; integración de persistencia de usuarios y restricciones (RC-03); contract tests de sesión, usuarios y agentes (11 endpoints de §3.1 y §3.2); e2e del walking skeleton login→alta→baja jerárquica | QA/SDET; base efímera; fixtures de roles y usuarios; secretos de prueba | TC-01..TC-07 (parcial), TC-25, TC-26, TC-27, TC-34 (parcial), TC-35; suite de regresión base |
| Tramo 2 — Relevamientos y marcadores (F1) | Unit de transición de estados (RN-05) y convivencia con conflictos (RN-03); unit de paginación/filtros y alcance antes de paginar (RN-01 sobre CU-20); integración de relevamientos, asignaciones (RC-05), marcadores (RC-01, RC-02) y observaciones; contract tests de relevamientos, asignaciones, marcadores/observaciones (20 endpoints de §3.3, §3.4, §3.5) | QA/SDET; base efímera; fixtures de relevamientos por estado y marcadores con/sin conflicto | TC-04, TC-05, TC-06, TC-07, TC-08, TC-12 (parcial), TC-23, TC-24, TC-28; ampliación de TC-34 |
| Tramo 3 — Captura y sincronización (F2) | Unit de idempotencia (RN-07) y orden subir-antes-de-bajar (RN-06); unit de carga manual por radio y priorización de ubicación (RN-04); integración del pipeline subida/bajada; contract tests de sincronización (2 endpoints de §3.6); e2e de corte y reenvío; carga del lote ≥ 1000 | QA/SDET; base efímera; dataset de lote ≥ 1000 por semilla fija; fotos sintéticas con/sin ubicación | TC-09, TC-10, TC-11, TC-17, TC-18, TC-19, TC-20, TC-29, TC-30, TC-31; ampliación de TC-34 |
| Tramo 4 — Revisión, conflictos, cierre y portabilidad (F3) | Unit y contract de consulta para revisión (CU-12), conflictos (CU-13) y cierre (CU-14, RN-05/RN-03); contract tests de conflictos, portabilidad y configuración de almacenamiento (7 endpoints de §3.7, §3.8, §3.9); materialización OpenAPI (BT-18) y contract tests del 100 % de endpoints (BT-19); gate de cobertura (BT-20); pruebas de rendimiento de todos los NFR | QA/SDET; ambiente equivalente al productivo para NFR; cliente de carga; base efímera | TC-12, TC-13, TC-14, TC-15, TC-16, TC-21, TC-22, TC-32, TC-33; cierre de TC-34 (100 % endpoints) y TC-35; suite de regresión completa |

Las US Could de portabilidad y configuración de almacenamiento (CU-15, CU-16, CU-17) se prueban en el Tramo 4 solo si la cadencia lo permite (07 §6); en caso de diferimiento, sus TC quedan como gap planificable registrado en la matriz §6, sin bloquear el MVP (NB-01 a NB-05).

## 6. Recursos

- Personas: un desarrollador que cumple el rol QA/SDET (equipo_n=1), con revisiones acotadas del Analista funcional (AG-02) sobre trazabilidad CU↔TC y del Arquitecto (AG-05) sobre NFR↔TC.
- Ambientes: ambiente de CI con base de datos efímera para unit/integración/contrato; ambiente equivalente al productivo para las pruebas de rendimiento (NFR); contenedor de backend con sus dependencias para e2e.
- Datasets: fixtures sintéticos versionados (roles, usuarios, relevamientos por estado, marcadores con/sin conflicto, fotos con/sin ubicación); dataset de carga de lote ≥ 1000 cambios generado por semilla fija.
- Herramientas (rol abstracto): framework de pruebas unitarias, framework de mocking, base de datos efímera, cliente HTTP de pruebas, framework de validación de contrato sobre OpenAPI, generador de fuzz de contrato, cliente de carga, reporte de cobertura por capa.

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Plan de pruebas inicial de geovial-api: alcance e inclusiones/exclusiones, criterios de entrada y salida, ocho riesgos de calidad alineados con 05 §9 y 07 §7 con su mitigación de testing, plan por los cuatro tramos del mini-plan con alcance, recursos y entregables de TC por tramo, y recursos del proyecto para equipo de un desarrollador. |
