# Plan de pruebas — geovial-web

Proyecto: geovial-web
Documento: plan-pruebas_v1.0.md
Versión: 1.0
Estado: Propuesto
Fecha: 2026-06-15
Autor: Ingeniero QA / SDET (web-monolith)

## 1. Alcance del plan

Cubre la ejecución de pruebas de `geovial-web` a lo largo de los cinco tramos del `mini-plan_v1.0.md` de 07 (modo mini-plan, `equipo_n=1`), que comprometen las 18 historias de usuario (US-01 a US-18) y las 14 tareas técnicas (BT-01 a BT-14) del backlog de 06, y que avanzan los 11 casos de uso (CU-01 a CU-11) y las 5 reglas de negocio (RN-01 a RN-05) de la especificación funcional de 02.

Módulos incluidos: ingreso y sesión, administración de usuarios por jerarquía, gestión de relevamientos, asignación de agentes, marcadores sobre el mapa, revisión con carrusel, resolución de conflictos, transición de estado y cierre, carga manual vía web, portabilidad y configuración de almacenamiento.

Módulos excluidos: la captura en terreno offline-first, el relogueo por seguridad del dispositivo, el análisis automático de imágenes y el ruteo, que pertenecen a otros proyectos de la solución y no originan CU en el front (02 §2). El dominio autoritativo y sus invariantes se prueban en `geovial-api`; aquí solo se prueba el consumo del contrato y la presentación.

## 2. Criterios de entrada

El plan, o el tramo del plan, se ejecuta cuando:

- El build del front compila sin warnings tratados como error.
- Los ítems del tramo cumplen la Definition of Ready de 06 (`definition-of-ready_v1.0.md`).
- Existe una instancia de `geovial-api` alcanzable respaldada por una base de datos efímera sembrable, para las pruebas de integración a través de la API.
- Los fixtures y el dataset sintético del tramo están disponibles o su provisión está acordada (relevamientos en cada estado, fotos con y sin ubicación incrustada, usuarios por rol).
- Para los tramos que tocan los cimientos (Tramo 1), las ADR-04 y ADR-05 en estado Propuesto están ratificadas (excepción de Sprint 0 de la DoR), porque condicionan la separación de capas y el mapeo de errores que la suite verifica.

## 3. Criterios de salida

El plan, o el tramo del plan, se declara ejecutado con éxito cuando:

- Todos los CU críticos del tramo tienen al menos un TC verde (criterios-validacion §2).
- La suite de unidad, integración, componente de UI y snapshot del tramo está en verde.
- El gate de cobertura global (líneas ≥ 80 %, branches ≥ 70 %) y los pisos por capa (Aplicación de UI 80/70, infraestructura 70/60, presentación 60/50) se cumplen sobre el alcance del tramo.
- Los defectos blocker y críticos del tramo están cerrados; cada bug cerrado generó al menos un TC de regresión (regla 08 §4.10).
- Para el release: los NFR numéricos de P.10 (interacción p95 ≤ 200 ms, ≥ 50 circuitos concurrentes, custodia del token, disponibilidad ≥ 99,5 %) están validados en el ambiente de referencia.

## 4. Riesgos de calidad

Cada riesgo declara impacto, probabilidad y mitigación, alineado con los riesgos arquitectónicos de 05 §9 y los del mini-plan de 07 §6.

| Riesgo | Impacto | Probabilidad | Mitigación |
| --- | --- | --- | --- |
| El spike de integración del componente de mapa (BT-06) no encuentra un camino claro de sincronización del estado dentro de su caja temporal de 2 días, retrasando los TC de mapa y carrusel (CU-05, CU-06) | Alto | Media | TC de mapa y carrusel se preparan como pendientes hasta cerrar el spike; el adaptador de mapa se prueba primero en aislamiento (doble del componente) antes del e2e sobre el mapa real |
| Fragilidad de los snapshots de vistas de render server-side ante cambios de layout de 03, generando rojos espurios | Medio | Media | Snapshot acotado a la estructura estable de las vistas clave; política de regeneración con justificación y revisión; el detalle fino de interfaz no se fija en snapshot |
| Acoplamiento de la suite de integración a la disponibilidad y a la versión del contrato de `geovial-api`; un cambio del contrato rompe los tests | Alto | Media | El Cliente de API centraliza el consumo; los dobles de unidad se construyen con factoría única; la integración fija la versión mayor del contrato; los fixtures de error problem+json se versionan |
| Camino crítico de cimientos (BT-01 a BT-04, BT-11) concentra dependencias; un atraso temprano propaga el retraso de la cobertura a todos los tramos | Alto | Alta | Los TC de los cimientos (sesión, token, visibilidad por rol, mapeo de errores) se priorizan en el Tramo 1; los tramos se cierran por criterios verificables, no por fecha |
| Inestabilidad de las pruebas de carga de circuitos por la naturaleza del circuito interactivo persistente, dando falsos positivos en el NFR de concurrencia | Medio | Media | El ambiente de referencia se aísla; la prueba de carga verifica latencia p95 sostenida y no pérdida de estado de sesión por circuito; se repite la medición antes de bloquear el release |
| Inconsistencia entre la habilitación de acciones del front y la autorización real del backend, no detectada por pruebas que solo miran la presentación | Medio | Baja | Los TC de RN-01 y RN-04 verifican tanto el ocultamiento en presentación como el mapeo del rechazo del backend a feedback (no se autoriza por ocultamiento) |

## 5. Plan por tramo

Cada tramo del mini-plan de 07 mapea su alcance de testing, recursos y entregables. La columna "CU/RN/NFR en foco" referencia los identificadores que el tramo lleva a verde.

| Tramo | Alcance de testing | CU / RN / NFR en foco | Recursos | Entregables |
| --- | --- | --- | --- | --- |
| Tramo 1 — Cimientos y walking skeleton de acceso y administración | Unitario de sesión y token, visibilidad por rol, mapeo de errores; integración de ingreso y alta de usuario a través de la API; snapshot de la vista de ingreso; TC de custodia del token | CU-01, CU-02 (parcial); RN-01, RN-03; NFR custodia del token | Base efímera + instancia de geovial-api; fixtures de usuarios por rol y errores de credenciales | Suite de cimientos verde; TC-01, TC-02, TC-03, TC-12, TC-18 |
| Tramo 2 — Administración completa de usuarios y gestión de relevamientos | Unitario de habilitación por estado y validación de formulario; integración de baja con conservación de autoría y de creación/listado de relevamiento; snapshot del listado de relevamientos | CU-02, CU-03; RN-02, RN-04 | Fixtures de relevamientos en cada estado; agentes con observaciones cargadas | TC-03, TC-04, TC-05, TC-06; snapshot de listado |
| Tramo 3 — Marcadores sobre el mapa y asignación de agentes | Componente de UI del adaptador de mapa (crear/mover pin); integración de creación de marcador y de asignación; TC de conflicto por radio que convive | CU-04, CU-05; RN-01, RN-04 | Componente de mapa integrado (post BT-06); fixtures de marcadores con/sin conflicto | TC-07, TC-08, TC-09; prueba de componente de mapa |
| Tramo 4 — Revisión con carrusel, resolución de conflictos y cierre | Componente de UI del carrusel encadenado y filtro por etiqueta; integración de resolución unificar/separar; TC de cierre bloqueado con conflictos; TC del NFR de interacción sobre revisión | CU-06, CU-07, CU-08; RN-02, RN-04, RN-05; NFR interacción p95, concurrencia | Ambiente de referencia para NFR; relevamiento con conflictos pendientes | TC-10, TC-11, TC-13, TC-14, TC-15, TC-19, TC-20; snapshot de revisión y de resolución |
| Tramo 5 — Carga manual de evidencia y cierre de alcance | Unitario e integración de carga con radio de agrupación y foto sin ubicación; TC de portabilidad (export/import) y de configuración de almacenamiento solo para el raíz | CU-09, CU-10, CU-11; RN-01, RN-04 | Fixtures de fotos con/sin EXIF de ubicación; unidad transferible válida e inválida | TC-16, TC-17, TC-21, TC-22 |

Regresión: al cierre de cada tramo se ejecuta la suite completa acumulada; ningún TC verde del tramo anterior puede pasar a rojo sin justificación (criterios-validacion §4).

## 6. Recursos

- Personas. Un desarrollador full-stack implementa y corre los tests; el QA / SDET diseña la estrategia y los casos y aprueba el release; el Analista Funcional firma la trazabilidad a CU; el Arquitecto valida los NFR (RACI en estrategia-calidad §4).
- Ambientes. Ambiente de pruebas con base de datos efímera en contenedor descartable y una instancia de `geovial-api`; ambiente de referencia equivalente al productivo para los NFR de interacción, concurrencia y disponibilidad.
- Datasets. Dataset sintético versionado: usuarios por rol, relevamientos en cada estado del ciclo, marcadores con y sin conflicto por radio, fotos con y sin ubicación incrustada, unidades transferibles válida e inválida, y representaciones de error problem+json por código estable.
- Herramientas. Framework de pruebas unitarias, framework de pruebas de integración, motor headless de UI, framework de snapshot, cliente de pruebas de rendimiento y de carga, reporte de cobertura por capa y analizador estático (todos por rol abstracto, estrategia-testing §3).

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Plan de pruebas inicial de geovial-web: alcance sobre 11 CU, 5 RN, 18 US y 14 BT; criterios de entrada y salida; seis riesgos de calidad alineados a 05 §9 y 07 §6; plan por los cinco tramos del mini-plan con CU/RN/NFR en foco, recursos y entregables (TC); regresión acumulada por tramo; recursos de personas, ambientes, datasets y herramientas. |
