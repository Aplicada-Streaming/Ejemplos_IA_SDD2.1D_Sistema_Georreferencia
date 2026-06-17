# Estrategia de calidad — geovial-web

Proyecto: geovial-web
Documento: estrategia-calidad_v1.0.md
Versión: 1.0
Estado: Propuesto
Fecha: 2026-06-15
Autor: Ingeniero QA / SDET (web-monolith)

## 1. Definición de calidad para el proyecto

`geovial-web` tiene calidad cuando el front de render server-side permite a los roles administradores (usuario raíz, jefe general y jefe de área, más la excepción de carga manual del agente) ingresar, administrar usuarios por jerarquía, gestionar y asignar relevamientos, crear y revisar marcadores sobre el mapa, navegar el carrusel encadenado de fotos y cerrar el relevamiento tras resolver sus conflictos, todo dentro de la latencia de interacción comprometida y sin exponer el token de acceso al navegador. Como el front no posee persistencia de dominio propia (`tiene_persistencia=false`) ni invariantes de integridad propias, la calidad se concentra en tres ejes: fidelidad de la presentación al alcance del rol y al estado del relevamiento, robustez del consumo del contrato REST de `geovial-api` (incluido el mapeo de errores a feedback) y respuesta interactiva del circuito. El perfil de riesgo es moderado: hay autenticación (`tiene_auth=true`) pero no observabilidad crítica (`tiene_observabilidad_critica=false`) y no se manejan datos regulados.

## 2. Atributos de calidad priorizados (ISO/IEC 25010)

La priorización ordena el esfuerzo de aseguramiento. Cada atributo declara prioridad, justificación y, cuando corresponde, la métrica numérica con su NFR de origen (intake §17 geovial-web P.10 y arquitectura 05 §8).

| Atributo ISO/IEC 25010 | Prioridad | Justificación en el contexto del front | Métrica numérica y NFR de origen |
| --- | --- | --- | --- |
| Adecuación funcional | Crítica | El front debe cubrir los 11 CU con sus criterios Given/When/Then y reflejar fielmente las 5 RN de presentación y flujo; una acción ofrecida fuera de alcance o de estado es un defecto | Cobertura funcional: 100 % de CU críticos con al menos un TC verde (criterios-validacion §2) |
| Seguridad | Alta | Hay autenticación por token bearer; el token se custodia del lado servidor del circuito y nunca se serializa al navegador (ADR-03); la autorización autoritativa la resuelve el backend y el front solo refleja el alcance del rol (RN-01, RN-03) | 0 exposiciones del token al navegador (NFR custodia del token, arquitectura §8) |
| Eficiencia de desempeño | Alta | La experiencia de mapa y carrusel exige respuesta interactiva sostenida del circuito, también bajo concurrencia | Latencia de interacción p95 ≤ 200 ms; ≥ 50 circuitos concurrentes (NFR P.10) |
| Fiabilidad | Alta | El estado del circuito es efímero y reconstruible desde la API; la pérdida del circuito no debe perder datos de dominio; disponibilidad objetivo del contenedor de front | Disponibilidad mensual ≥ 99,5 % (NFR P.10); reconstrucción del estado de UI sin pérdida de dominio |
| Usabilidad | Media | El front debe presentar estados visibles del relevamiento, habilitar solo acciones válidas y traducir cada error del backend a feedback comprensible (RN-04, ADR-05); el detalle fino de interfaz vive en 03 | Snapshot estable de vistas clave; cero errores sin contemplar presentados como detalle del backend |
| Mantenibilidad | Media | La separación de capas (Presentación / Aplicación de UI / Cliente de API) y la centralización del consumo del contrato y del mapeo de errores deben sostener la cobertura por capa | Cobertura por capa (estrategia-testing §2); gate de cobertura líneas ≥ 80 %, branches ≥ 70 %, presentación ≥ 60 % |
| Compatibilidad | Media | El front debe operar sobre navegadores evergreen de escritorio y móvil (últimas dos versiones mayores de uso corriente, intake §17.P.9) y convivir con el contrato versionado de la API | Vistas clave verificadas con un motor headless de UI; versión mayor del contrato fijada |
| Portabilidad | Baja | El front se despliega como un único contenedor; no hay requisito de portar a otros runtimes más allá del contenedor objetivo | Sin métrica numérica propia; cubierto por la imagen de contenedor (09) |

Atributos de prioridad crítica y alta concentran el diseño de casos de prueba y los quality gates bloqueantes. La portabilidad no origina pruebas dedicadas en esta versión.

## 3. Quality gates

Conjunto de criterios mecánicos que el pipeline aplica antes de declarar un build o un release aceptable. Cada gate especifica condición, herramienta por rol abstracto y consecuencia. Estos gates se materializan como stages del pipeline en la categoría 09.

| Gate | Condición | Herramienta (rol abstracto) | Consecuencia si falla |
| --- | --- | --- | --- |
| Compilación limpia | El front compila sin warnings tratados como error | Compilador del runtime objetivo | Bloquea el merge |
| Pruebas unitarias y de integración en verde | Toda la suite de unidad y de integración (a través de la API contra base efímera) pasa | Framework de pruebas unitarias y framework de pruebas de integración | Bloquea el merge |
| Pruebas de componente de UI y snapshot en verde | Las pruebas de componente con motor headless de UI y los snapshots de vistas clave pasan sin diferencias no aprobadas | Motor headless de UI; framework de snapshot | Bloquea el merge |
| Gate de cobertura global | Líneas ≥ 80 % y branches ≥ 70 % sobre el conjunto del proyecto | Reporte de cobertura del runtime | Bloquea el merge |
| Gate de cobertura por capa | Aplicación de UI ≥ 80 % líneas / ≥ 70 % branches; Cliente de API (infraestructura) ≥ 70 % líneas / ≥ 60 % branches; Presentación ≥ 60 % líneas / ≥ 50 % branches | Reporte de cobertura por capa | Bloquea el merge |
| Análisis estático | Sin issues críticos nuevos | Analizador estático del runtime | Bloquea el merge |
| Custodia del token | El token bearer no se serializa al navegador en ninguna vista | Prueba de componente de no exposición (motor headless) | Bloquea el release |
| NFR de interacción | Latencia de interacción p95 ≤ 200 ms en el ambiente de referencia sobre las vistas clave | Prueba de rendimiento de interacción | Bloquea el release; admite excepción solo con ADR y plan de remediación |
| NFR de concurrencia | ≥ 50 circuitos concurrentes sosteniendo la latencia p95 y sin pérdida de estado de sesión | Prueba de carga de circuitos | Bloquea el release; admite excepción solo con ADR y plan de remediación |

Reconciliación del gate global con la cobertura por capa: el gate global de §17 P.6 (líneas ≥ 80 %, branches ≥ 70 %) es el umbral agregado del proyecto y se mide sobre todas las capas en conjunto; los umbrales por capa de la tabla anterior son pisos diferenciados que evitan que una capa muy cubierta (por ejemplo, Aplicación de UI) enmascare una capa débil (Presentación). Ambos gates conviven y los dos deben pasar: el global no sustituye al por capa ni viceversa. La presentación tiene un piso propio de 60 % de líneas (intake §17 geovial-web P.6) por debajo del cual el gate por capa falla aunque el global de 80 % se cumpla por compensación de otras capas.

## 4. Roles QA dentro del equipo

El proyecto es de un único desarrollador full-stack (`equipo_n=1`); el rol QA / SDET aporta el diseño de la estrategia y las revisiones acotadas previstas en la regla 06 §1.3. RACI condensado para un equipo de un dev:

| Actividad | Responsable (R) | Aprueba (A) | Consultado (C) | Informado (I) |
| --- | --- | --- | --- | --- |
| Diseño de la estrategia y de los casos de prueba | QA / SDET | QA / SDET | Arquitecto, Analista Funcional | Dev |
| Implementación de los tests y fixtures | Dev | QA / SDET | QA / SDET | — |
| Ejecución de la suite y de los gates en CI | Dev (vía pipeline) | QA / SDET | DevOps (09) | — |
| Validación de la cobertura de CU y de las RN | Analista Funcional (firma trazabilidad) | QA / SDET | Dev | Scrum Master |
| Validación de NFR numéricos | Arquitecto | QA / SDET | Dev | Scrum Master |
| Aprobación del release | QA / SDET | QA / SDET | Arquitecto, Scrum Master | Dev |

La titularidad de los artefactos de calidad es del QA / SDET; las demás especialidades aportan revisión sectorial y consumen los criterios.

## 5. Cadencia de revisión

- La estrategia de calidad y la estrategia de testing se revisan al cierre de cada tramo del mini-plan de 07 y cuando se ratifica una ADR en estado Propuesto (ADR-04, ADR-05) que afecte la separación de capas o el mapeo de errores.
- Los umbrales de cobertura por capa y los objetivos de NFR son ratificables (intake P.6, P.10); cualquier cambio a la baja requiere ADR explícita (regla 08 §2.2).
- La matriz de cobertura se actualiza al cierre de cada tramo (regla 08 §4.10, antipatrón de matriz desactualizada).
- La Definition of Done (`definition-of-done_v1.0.md`) es la fuente canónica; cualquier cambio en sus criterios versionables se registra en su §9 y se comunica al equipo en la revisión del tramo siguiente.

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Estrategia de calidad inicial de geovial-web: atributos ISO/IEC 25010 priorizados con métricas de origen NFR, quality gates mecánicos (compilación, pruebas, cobertura global y por capa, análisis estático, custodia del token, NFR de interacción y concurrencia), RACI para equipo de un dev y cadencia de revisión por tramo. Reconcilia el gate global de cobertura (≥ 80 % / ≥ 70 %) con los pisos por capa, incluida presentación ≥ 60 %. |
