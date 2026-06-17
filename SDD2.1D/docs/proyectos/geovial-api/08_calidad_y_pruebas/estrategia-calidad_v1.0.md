# Estrategia de calidad — geovial-api

**Proyecto:** geovial-api
**Documento:** estrategia-calidad_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (variante API Testing Specialist)

## 1. Definición de calidad para el proyecto

`geovial-api` tiene calidad cuando su contrato público REST cumple, de forma verificable y sin regresiones, lo que la especificación funcional declara: los 22 casos de uso (CU-01 a CU-22) se comportan según sus criterios Given/When/Then, las siete reglas de negocio (RN-01 a RN-07) se sostienen como invariantes bajo concurrencia, y los objetivos numéricos de los NFR (latencia, disponibilidad, capacidad de lote, idempotencia) se miden en un ambiente equivalente al productivo y se cumplen. Como backend monolítico que concentra la lógica, la persistencia y la seguridad de toda la solución, su perfil de riesgo prioriza la corrección funcional, la integridad de los datos en la sincronización sin conexión y el control de acceso jerárquico por encima de la eficiencia bruta; un defecto de autorización o de idempotencia es más grave que un milisegundo de latencia.

El proyecto es `tiene_persistencia=true`, `tiene_auth=true`, `tiene_extensibilidad=false`, `tiene_observabilidad_critica=false`, equipo de un desarrollador (equipo_n=1). La estrategia se calibra a esa realidad: automatización exhaustiva como red de seguridad de un único desarrollador, sin testing de extensibilidad (no hay handlers ni middlewares externos publicados), sin SLO de disponibilidad ≥ 99,9 % ni objetivo de latencia p99.

## 2. Atributos de calidad priorizados (ISO/IEC 25010)

Cada atributo declara su prioridad para este proyecto, la justificación y, cuando corresponde, la métrica numérica con su NFR de origen (05 §8, intake §17.P.10).

| Atributo ISO/IEC 25010 | Prioridad | Justificación | Métrica / NFR de origen |
| --- | --- | --- | --- |
| Adecuación funcional (functional suitability) | Crítica | El contrato REST es el producto; cada CU debe cumplir sus criterios Given/When/Then y cada RN debe sostenerse. | 22 CU cubiertos por TC; 7 RN con TC; 100 % de endpoints con contract test (NFR cobertura) |
| Fiabilidad (reliability) | Crítica | La sincronización sin conexión no puede perder ni duplicar evidencia de campo; el ciclo del relevamiento debe ser consistente. | Idempotencia 100 % (idempotencia de operaciones no seguras); lote ≥ 1000 sin pérdida ni duplicación (capacidad de lote); disponibilidad ≥ 99,5 % mensual |
| Seguridad (security) | Crítica | Control de acceso jerárquico de cuatro niveles; una escalada de privilegios operaría fuera de alcance. La autoría histórica se conserva ante la baja. | 0 violaciones de jerarquía bajo concurrencia (integridad de jerarquía); acceso fuera de alcance rechazado (RN-01); autoría conservada (RN-02) |
| Eficiencia de desempeño (performance efficiency) | Alta | La revisión sobre mapa y los listados deben responder en tiempo; la sincronización debe absorber lotes grandes. | Latencia p95 lecturas ≤ 300 ms; p95 escrituras ≤ 500 ms; lote de sincronización ≥ 1000 cambios |
| Mantenibilidad (maintainability) | Alta | Un único desarrollador depende de una suite de regresión confiable y de cobertura por capa para evolucionar sin romper. | Cobertura: aplicación ≥ 80 %, infraestructura ≥ 70 %; líneas ≥ 80 %, branches ≥ 70 % (gate de CI) |
| Compatibilidad (compatibility) | Media | El contrato lo consumen `geovial-web` y `geovial-mobile`; un cambio incompatible los rompería en silencio. | Versionado por URI con convivencia ≥ 1 MINOR; contract tests de compatibilidad (CU-22) |
| Usabilidad (usability) | Media | Para una API, la usabilidad es la del contrato: errores problem+json uniformes con código estable, opaco al idioma. | Todo error en problem+json RFC 7807 con código estable (CU-19) |
| Portabilidad (portability) | Baja | El backend corre en un contenedor único; la portabilidad del relevamiento como dato (export/import) es funcional, no de plataforma. | Export/import de unidad transferible (CU-15, CU-16) cubierto por TC; runtime fijado en 09 |

Atributos críticos: adecuación funcional, fiabilidad y seguridad. Toda decisión de gate y de priorización de esfuerzo de testing se resuelve a favor de estos tres.

## 3. Quality gates

Conjunto de criterios mecánicos que el pipeline (09, BT-20) aplica antes de declarar un build, una rama o un release aceptable. Cada gate especifica condición, herramienta abstracta y consecuencia. Son bloqueantes para mergear salvo donde se indique.

| Gate | Condición | Herramienta (rol abstracto) | Consecuencia si falla |
| --- | --- | --- | --- |
| G1 Compilación limpia | Compila sin warnings tratados como error | Compilador del runtime | Bloquea el merge |
| G2 Pruebas en verde | Toda la suite (unit, integración, contrato) pasa; ningún test sin assert ni deshabilitado sin motivo registrado | Framework de pruebas; cliente HTTP de pruebas | Bloquea el merge |
| G3 Cobertura por capa | Líneas ≥ 80 %, branches ≥ 70 % global; aplicación ≥ 80 %, infraestructura ≥ 70 % (ver reconciliación §3.1) | Reporte de cobertura del runtime | Bloquea el merge |
| G4 Contract test total | 100 % de los endpoints públicos del contrato (35 operaciones, ver matriz §4) cubiertos por al menos un contract test por versión | Framework de validación de contrato sobre OpenAPI | Bloquea el merge |
| G5 Validación de contrato | La especificación OpenAPI materializada (BT-18) valida contra la implementación; sin deriva entre contrato e implementación | Framework de validación de contrato; fuzz de endpoints | Bloquea el merge |
| G6 Análisis estático | Sin issues críticos del análisis estático | Analizador estático | Bloquea el merge |
| G7 Regresión | Ningún test verde de la versión anterior pasa a rojo sin justificación registrada (ADR o nota) | Comparación de suite entre revisiones | Bloquea el release |
| G8 NFR numéricos | Cada NFR numérico de §8 de 05 se mide y cumple en el ambiente equivalente al productivo antes del release | Cliente de carga / pruebas de rendimiento | Bloquea el release (no el merge) |

### 3.1 Reconciliación del gate global con la cobertura por capa

El intake §17.P.6 y el NFR de cobertura de 05 §8 declaran un gate global de líneas ≥ 80 % y branches ≥ 70 %. La regla 08 §2.2 para `rest-api` exige cobertura por capa: aplicación ≥ 80 % y infraestructura ≥ 70 %, además del 100 % de endpoints con contract test. Ambos coexisten sin contradicción: el gate global de líneas/branches es el piso agregado del pipeline (G3), y la cobertura por capa lo descompone para evitar el anti-patrón de la cobertura global que esconde capas débiles (08 §4.10). En la práctica, el gate por capa es más estricto que el global en la capa de aplicación (donde vive la lógica de los casos de uso), por lo que cumplir la cobertura por capa implica cumplir el gate global. Esta estrategia adopta la cobertura por capa como criterio rector y reporta el global como consecuencia (detalle en `matriz-cobertura-pruebas_v1.0.md` §5).

## 4. Roles QA dentro del equipo

El proyecto es de un único desarrollador (equipo_n=1); los roles QA se cumplen por la misma persona con el apoyo de revisiones acotadas de las demás especialidades. Se declara el RACI para fijar titularidad de cada actividad.

| Actividad | Responsable (R) | Aprobador (A) | Consultado (C) | Informado (I) |
| --- | --- | --- | --- | --- |
| Diseñar los casos de prueba (TC) | QA/SDET (AG-08) | QA/SDET | Analista funcional (AG-02), Arquitecto (AG-05) | Equipo |
| Implementar y mantener la automatización | QA/SDET | QA/SDET | — | DevOps (AG-09) |
| Ejecutar la suite en CI | Pipeline (09) | QA/SDET | DevOps | Equipo |
| Validar trazabilidad CU↔TC | QA/SDET | Analista funcional (AG-02) | — | Equipo |
| Validar que cada NFR numérico tiene test | QA/SDET | Arquitecto (AG-05) | — | Equipo |
| Aprobar el release (criterios de validación) | QA/SDET | API Product Owner | Arquitecto, DevOps | Stakeholders |
| Mantener la DoD canónica | QA/SDET | QA/SDET | Scrum Master (AG-06) | Equipo |

## 5. Cadencia de revisión

- La estrategia de calidad, la pirámide y los umbrales de cobertura se revisan al cierre de cada tramo del mini-plan (07, Tramos 1 a 4) y obligatoriamente al cierre del Tramo 1, cuando se calibra la línea de base de avance del desarrollador.
- La matriz de cobertura (`matriz-cobertura-pruebas_v1.0.md`) se actualiza al cierre de cada tramo para reflejar el estado real de los TC, evitando el anti-patrón de la matriz desactualizada (08 §4.10).
- Cualquier cambio de un umbral de cobertura a la baja requiere una ADR que lo justifique (08 §2.2); subirlo no.
- Todo bug cerrado genera al menos un TC nuevo de regresión o extiende uno existente antes de declararse cerrado (08 §5.4); la matriz incorpora el TC en el cierre del tramo.
- La DoD canónica (`definition-of-done_v1.0.md`) es el documento más sensible: cualquier cambio en sus criterios se registra en su §9 y se comunica en la revisión del tramo siguiente.

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Estrategia de calidad inicial de geovial-api: definición de calidad por perfil de riesgo, ocho atributos ISO/IEC 25010 priorizados con métricas y NFR de origen, ocho quality gates con reconciliación del gate global frente a la cobertura por capa, RACI de roles QA para equipo de un desarrollador y cadencia de revisión por tramo. |
