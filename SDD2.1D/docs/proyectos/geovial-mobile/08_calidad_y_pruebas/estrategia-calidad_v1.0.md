# Estrategia de calidad — geovial-mobile

Proyecto: geovial-mobile
Documento: estrategia-calidad_v1.0.md
Versión: 1.0
Estado: Propuesto
Fecha: 2026-06-15
Autor: Ingeniero QA / SDET (mobile)

## 1. Definición de calidad para el proyecto

`geovial-mobile` es la app de campo offline-first de la solución GeoVial: el relevador captura observaciones georreferenciadas en terreno sin conectividad y la app las sincroniza después. Que el sistema "tenga calidad" significa, en este contexto, que toda captura realizada en campo sin conexión se persiste localmente sin pérdida y se sincroniza con el backend de forma íntegra (sin pérdida ni duplicación) en cuanto se recupera la conexión, conviviendo con los conflictos de marcadores sin bloquear el trabajo; que la sesión se resuelve de forma segura en un dispositivo que se comparte entre relevadores; y que la georreferenciación de cada observación es correcta o se degrada de forma explícita y honesta (nunca inventa una coordenada). El perfil de riesgo dominante es la pérdida de datos de campo: una observación que no llega a la oficina del jefe de área no se puede recapturar.

## 2. Atributos de calidad priorizados (ISO/IEC 25010)

Cada atributo lleva su prioridad y, cuando corresponde, la métrica numérica con su NFR de origen (intake §17 geovial-mobile P.10 y arquitectura 05 §8).

| Atributo ISO/IEC 25010 | Prioridad | Qué significa acá | Métrica / NFR de origen |
| --- | --- | --- | --- |
| Fiabilidad (tolerancia a fallos, recuperabilidad) | Crítica | La cola local persiste y la sincronización reanuda sin pérdida tras un corte; ninguna captura offline se pierde | Captura 100 % offline; cola ≥ 1000 cambios; reanudación sin pérdida ni duplicación (P.10) |
| Funcionalidad (completitud, corrección) | Crítica | Los 7 CU cumplen sus criterios Given/When/Then, incluida la georreferenciación correcta y la degradación honesta | 7 CU cubiertos; RN-01 a RN-05 verificadas |
| Eficiencia de desempeño (comportamiento temporal) | Alta | El ciclo de sincronización y el arranque cumplen sus tiempos en el dispositivo de referencia | Ciclo de 100 cambios ≤ 30 s; arranque en frío ≤ 3 s (P.10) |
| Seguridad (confidencialidad, autenticidad) | Alta | El token bearer vive solo en el almacén seguro del dispositivo; el deslogueo libera el equipo; el relogueo exige verificación del dispositivo | Token nunca en texto plano; relogueo por seguridad del dispositivo (RN-04, ADR-05) |
| Usabilidad (operabilidad, protección frente a errores) | Media | Las degradaciones por permiso, sin señal o sin espacio son explícitas y no inventan datos | RN-01 (no inventa coordenada); ADR-04 |
| Compatibilidad (coexistencia) | Media | El esquema local versionado migra en el arranque sin perder datos; un único target (Android) | Migración inicial en arranque (ADR-02); target único Android (P.9) |
| Mantenibilidad (modularidad, testabilidad) | Media | La lógica de captura y sincronización se prueba sin interfaz; capas con dependencia hacia adentro | Cobertura por capa lógica 75 / presentación 60 (P.6) |
| Portabilidad | Baja | Sin objetivo multiplataforma en v1: el único target es Android | Sin iOS ni Windows en v1 (P.9) |

La observabilidad no es crítica en este proyecto (`tiene_observabilidad_critica = false`, P.10): no hay SLO de disponibilidad ≥ 99,9 % ni objetivo de latencia p99 numérico que validar.

## 3. Quality gates

Conjunto de criterios mecánicos que el pipeline aplica antes de declarar un build o un release como aceptable. Cada gate especifica condición, herramienta (por rol abstracto) y consecuencia. Estos gates se materializan como stages del pipeline en la categoría 09.

| Gate | Condición | Herramienta (rol abstracto) | Consecuencia si falla |
| --- | --- | --- | --- |
| Compilación | El paquete de aplicación compila sin warnings tratados como error | Compilador del runtime objetivo | Bloquea el merge |
| Pruebas en verde | La suite unitaria, de interfaz móvil y de modo offline/sincronización está en verde | Framework de pruebas del runtime; framework de pruebas de interfaz móvil; framework de pruebas de sincronización | Bloquea el merge |
| Cobertura global | Líneas ≥ 80 %, branches ≥ 70 % sobre el proyecto (intake §17 P.6) | Reporte de cobertura por capa del runtime | Bloquea el merge |
| Cobertura por capa | Lógica ≥ 75 %, presentación ≥ 60 % (intake §17 P.6) | Reporte de cobertura por capa del runtime | Bloquea el merge |
| Análisis estático | Sin issues críticos nuevos | Analizador estático del runtime | Bloquea el merge |
| Snapshot de pantallas críticas | El render de las pantallas críticas coincide con su baseline aprobado | Framework de snapshot de vistas | Bloquea el merge; regeneración solo con cambio justificado y revisado |
| Firma del paquete | El paquete de aplicación Android se firma con el keystore resguardado | Herramienta de firma del paquete | Bloquea la publicación al canal interno |
| NFR de release | Captura 100 % offline; cola ≥ 1000; ciclo de 100 cambios ≤ 30 s; reanudación sin pérdida; arranque ≤ 3 s | Pruebas de sincronización y de tiempo de arranque en dispositivo de referencia | Bloquea el release |

Reconciliación del gate global con los pisos por capa: el gate de cobertura global (líneas ≥ 80 %, branches ≥ 70 %) se mide sobre la unión de las capas; los pisos por capa (lógica 75, presentación 60) se miden por separado. Ambos deben pasar a la vez: el global no compensa una capa por debajo de su piso. El detalle de la reconciliación está en `estrategia-testing_v1.0.md` §2 y en `matriz-cobertura-pruebas_v1.0.md` §5.

## 4. Roles QA dentro del equipo

El equipo es de un solo desarrollador (`equipo_n=1`), por lo que el mismo profesional concentra el diseño, la implementación y la ejecución de los tests, y un revisor externo aprueba el release. El RACI explícito es liviano y se apoya en las revisiones sectoriales de las demás especialidades.

| Actividad | Responsable (R) | Aprueba (A) | Consultado (C) | Informado (I) |
| --- | --- | --- | --- | --- |
| Diseño de la estrategia y de los casos de prueba | QA / SDET (mobile) | QA / SDET (mobile) | AG-02 (funcional), AG-05 (arquitectura) | Equipo |
| Implementación de tests y fixtures | Dev / QA-SDET (mobile) | QA / SDET (mobile) | — | Equipo |
| Ejecución de la suite y de los NFR en dispositivo | Dev / QA-SDET (mobile) | QA / SDET (mobile) | AG-09 (DevOps, pipeline) | Equipo |
| Verificabilidad de los Given/When/Then (entrada a sprint) | QA / SDET (mobile) | Scrum Master + Mobile Lead | AG-02 | Equipo |
| Aprobación del release | QA / SDET (mobile) | Revisor de release | AG-05 | Stakeholders |
| Aceptación de excepciones a la DoD | QA / SDET (mobile) | QA / SDET (mobile) | AG-05 (si toca ADR) | Equipo |

Colaboración multi-especialidad (regla 08 §1.3): AG-02 valida que cada CU tenga al menos un TC por criterio Given/When/Then; AG-05 valida que cada NFR numérico tenga un test; AG-06 mantiene la DoD referenciada desde el mini-plan; AG-09 materializa los quality gates como stages del pipeline; AG-10 cita esta estrategia en la guía de testing del repositorio.

## 5. Cadencia de revisión

- La estrategia de calidad y la de testing se revisan al cierre de cada tramo del mini-plan de 07 (tres tramos) y cuando se ratifica una ADR en estado Propuesto que toque persistencia, sincronización, permisos o autenticación (ADR-01 a ADR-05).
- Los umbrales de cobertura y los objetivos de NFR son ratificables (intake §17 P.6 y P.10 los marcan como propuestos): cualquier cambio a la baja exige una ADR que lo justifique (regla 08 §2.2). Un cambio al alza no requiere ADR.
- La matriz de cobertura se actualiza al cierre de cada tramo con los valores observados de cobertura por capa y el estado real de cada TC.
- Cualquier cambio en los criterios versionables de la `definition-of-done_v1.0.md` se registra en el control de cambios de ese documento y se comunica en la revisión del tramo siguiente (regla 08 §3.4).

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Estrategia de calidad inicial de geovial-mobile: definición de calidad centrada en captura offline sin pérdida y sincronización íntegra; atributos ISO/IEC 25010 priorizados con métricas de origen NFR (P.10) y ADR; quality gates mecánicos con reconciliación del gate global de cobertura con los pisos por capa (lógica 75 / presentación 60); RACI para equipo de un dev con revisiones sectoriales; cadencia de revisión por tramo y por ratificación de ADR. |
