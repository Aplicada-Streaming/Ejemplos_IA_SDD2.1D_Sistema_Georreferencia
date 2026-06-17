# Estrategia de calidad — geovial-storage

**Proyecto:** geovial-storage
**Documento:** estrategia-calidad_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (variante QA + SDET Library)

## 1. Definición de calidad para el proyecto

`geovial-storage` es una librería de tipo `library` que expone al backend `geovial-api` una abstracción de alojamiento de archivos transparente, con proveedores intercambiables (local / remoto / otro) seleccionables por el usuario raíz. No tiene interfaz de usuario ni ambiente desplegado propio: su valor se observa enteramente en su superficie pública (entradas, salidas y errores). El sistema tiene calidad cuando, para una misma entrada, su contrato público se comporta de forma idéntica cualquiera sea el proveedor activo (RN-01), cuando lo recuperado es idénticamente igual byte a byte a lo guardado (RN-02) y cuando las credenciales de proveedor nunca se exponen por la superficie pública ni por los errores (RN-03). La calidad se mide sobre esas tres invariantes y sobre los seis casos de uso del contrato (CU-01 a CU-06), con cobertura diferenciada por capa y verificación de las métricas no funcionales declaradas en 05.

El perfil de riesgo es de librería integrada internamente (no redistribuible como paquete, consumidor único `geovial-api`): un cambio que rompa el contrato propaga el defecto al backend completo. Por eso la disciplina de calidad prioriza la estabilidad del contrato, la transparencia entre proveedores y la no regresión sobre la superficie pública.

## 2. Atributos de calidad priorizados (ISO/IEC 25010)

| Atributo ISO/IEC 25010 | Prioridad | Subcaracterística dominante | Métrica numérica y NFR de origen |
| --- | --- | --- | --- |
| Fiabilidad | Crítica | Madurez, recuperabilidad, tolerancia a fallos | Integridad 100 % byte a byte (NFR-04); idempotencia de eliminación; propagación uniforme de error transitorio (NFR-03) |
| Seguridad | Crítica | Confidencialidad | 0 ocurrencias de credenciales en resultados, errores y registros (NFR-05, RN-03, ADR-05) |
| Compatibilidad | Crítica | Interoperabilidad, coexistencia | 0 diferencias de comportamiento observable y 0 ramas por proveedor en el consumidor (NFR-03, RN-01, ADR-04); estabilidad de versión menor (ADR-03) |
| Funcionalidad | Alta | Completitud, corrección, pertinencia | 6/6 CU cubiertos; cada criterio Given-When-Then con al menos un TC verde |
| Eficiencia de desempeño | Alta | Comportamiento temporal, capacidad | Latencia p95 ≤ 1 s para archivos ≤ 5 MB con proveedor local (NFR-01); tamaño máximo 25 MB configurable (NFR-02) |
| Mantenibilidad | Alta | Modularidad, capacidad de prueba, modificabilidad | Cobertura por capa (85 % dominio, 70 % infraestructura); mutation score ≥ 60 % en dominio; cobertura ≥ 80 % líneas / ≥ 70 % branches del gate global |
| Portabilidad | Media | Adaptabilidad, reemplazabilidad | Un proveedor nuevo se incorpora como adaptador sin tocar el núcleo; suite de conformidad de proveedor reutilizable |
| Usabilidad | No aplica | — | La librería no tiene interfaz de usuario; la experiencia del desarrollador se gobierna en la categoría 03 (DX) |

La prioridad declarada ordena el esfuerzo de testing: fiabilidad, seguridad y compatibilidad concentran la batería crítica (RN-01, RN-02, RN-03), porque un defecto allí compromete el contrato y se propaga al consumidor.

## 3. Quality gates

Cada gate es un criterio mecánico que el pipeline aplica antes de declarar aceptable un build, una rama o un release. Las herramientas se nombran por rol abstracto (la materialización concreta vive en la categoría 09).

| Gate | Condición | Herramienta (rol abstracto) | Consecuencia si falla |
| --- | --- | --- | --- |
| G-01 Compilación limpia | Compilación sin warnings tratados como error | Compilador del runtime objetivo | Bloquea el merge |
| G-02 Unit y contract verdes | Toda la suite unitaria y de contrato en verde | Framework de tests unitarios; corredor de contract tests | Bloquea el merge |
| G-03 Cobertura global | Líneas ≥ 80 % y branches ≥ 70 % (gate de intake §17 P.6) | Medidor de cobertura del runtime | Bloquea el merge |
| G-04 Cobertura por capa | Dominio ≥ 85 % líneas / 80 % branches; infraestructura ≥ 70 % líneas / 60 % branches | Medidor de cobertura con segmentación por capa | Bloquea el merge |
| G-05 Mutation score dominio | Mutation score ≥ 60 % en la capa de dominio | Framework de mutation testing | Bloquea el release (informativo en feature branch) |
| G-06 Transparencia entre proveedores | La batería de contrato única pasa contra cada proveedor soportado con resultados equivalentes | Corredor de contract tests parametrizado por proveedor | Bloquea el merge |
| G-07 No filtración de credenciales | 0 ocurrencias de credenciales o parámetros de conexión en resultados, errores y registros | Property-based test y analizador estático | Bloquea el merge |
| G-08 NFR de latencia | Latencia p95 ≤ 1 s para archivos ≤ 5 MB con proveedor local | Banco de medición de desempeño | Bloquea el release |
| G-09 Análisis estático | Sin issues críticos del análisis estático | Analizador estático del runtime | Bloquea el merge |

La reconciliación entre el gate global de intake (G-03: ≥ 80 % líneas / ≥ 70 % branches) y la cobertura por capa de las reglas 08 (G-04) se detalla en `estrategia-testing_v1.0.md` §2 y en `matriz-cobertura-pruebas_v1.0.md` §5: los umbrales por capa son más exigentes en dominio y, ponderados sobre la base de código, satisfacen el piso global; son compatibles, no contradictorios.

## 4. Roles QA dentro del equipo

El proyecto tiene `equipo_n=1` (un único desarrollador, según intake §2). El RACI se simplifica pero las responsabilidades siguen explícitas.

| Actividad | Responsable | Aprobador | Consultado | Informado |
| --- | --- | --- | --- | --- |
| Diseñar casos de prueba y la batería de contrato | Desarrollador en rol SDET | Desarrollador en rol QA | AG-02 (trazabilidad CU), AG-05 (NFR) | AG-06 |
| Implementar fixtures, dobles de proveedor y automatización | Desarrollador en rol SDET | Desarrollador en rol QA | AG-09 (gates en CI) | — |
| Ejecutar la suite y reportar cobertura | Pipeline CI (automático) | Desarrollador en rol QA | — | AG-06 |
| Aprobar el release contra criterios de validación | Desarrollador en rol QA | AG-06 (Scrum Master) | AG-05 | Stakeholder propietario |

Aun con un solo desarrollador, las facetas QA (qué probar, gates, criterios de aceptación) y SDET (frameworks, fixtures, automatización, cobertura) se ejercen de forma diferenciada para preservar la objetividad de la verificación.

## 5. Cadencia de revisión

- La estrategia de calidad, sus métricas y umbrales se revisan al cierre de cada tramo del mini-plan de 07 (cinco tramos) y de forma obligatoria antes de cada release.
- Cualquier cambio de umbral de cobertura o de mutation score a la baja requiere una ADR explícita (regla 08 §2.2): el equipo no puede bajar el piso sin justificación arquitectónica.
- La Definition of Done (`definition-of-done_v1.0.md`) es la fuente canónica; cualquier cambio de sus criterios versionables se registra en el control de cambios de ese documento y se comunica en el sprint review siguiente.
- La matriz de cobertura se actualiza al cierre de cada tramo, reflejando el estado real de los tests (sin dejar "Pendiente" donde ya hay tests implementados).

## 6. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU cubiertos | CU-01 a CU-06 (02) |
| RN | RN-01 transparencia, RN-02 integridad, RN-03 credenciales (02) |
| NFR | NFR-01 a NFR-06 (05, intake §17 P.10) |
| ADR | ADR-01 estilo, ADR-02 superficie pública, ADR-03 versionado, ADR-04 transparencia/límites, ADR-05 credenciales (05) |
| Gate global | intake §17 P.6 (≥ 80 % líneas / ≥ 70 % branches) |
| Downstream | 09 (materializa los gates en el pipeline), 10 (developer guide de testing), 11 (examples ejecutables) |

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Estrategia de calidad inicial de geovial-storage: definición de calidad para una librería de almacenamiento transparente, atributos ISO/IEC 25010 priorizados con métrica y NFR de origen, nueve quality gates, RACI para equipo_n=1 y cadencia de revisión. |
