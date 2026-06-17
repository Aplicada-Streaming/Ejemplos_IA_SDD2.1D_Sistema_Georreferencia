# Matriz de cobertura de pruebas — geovial-web

Proyecto: geovial-web
Documento: matriz-cobertura-pruebas_v1.0.md
Versión: 1.0
Estado: Propuesto
Fecha: 2026-06-15
Autor: Ingeniero QA / SDET (web-monolith)

## 1. Propósito y alcance

Documento bisagra de la categoría 08: relaciona los casos de uso, las reglas de negocio y los requerimientos no funcionales de `geovial-web` con los casos de prueba referenciales (TC-XX del `casos-prueba-referenciales_v1.0.md`). Contiene las tres tablas obligatorias (CU↔Tests, NFR↔Tests, RN↔Tests) más la tabla de cobertura por capa. Cubre los 11 CU (CU-01 a CU-11), las 5 RN (RN-01 a RN-05) y los NFR numéricos de P.10. El estado de los tests es Pendiente porque el front aún no se implementó; la matriz se actualiza al cierre de cada tramo del mini-plan de 07.

## 2. Trazabilidad CU↔Tests

Cada CU se lista con un criterio Given/When/Then representativo de 02, el TC que lo cubre, su tipo y su estado. Los TC cubren todos los escenarios de aceptación de cada CU (detalle por CA en el catálogo).

| CU | Criterio Given-When-Then representativo | Test ID | Tipo | Estado |
| --- | --- | --- | --- | --- |
| CU-01 | Given un jefe de área habilitado, When ingresa credenciales válidas, Then el front abre sesión con su rol | TC-01, TC-02, TC-19, TC-22 | Integración / Unitario / Componente UI / Snapshot | Pendiente |
| CU-02 | Given un jefe de área que ve solo sus agentes, When abre la administración de usuarios, Then no muestra usuarios fuera de su ámbito | TC-03, TC-04 | Unitario / Integración | Pendiente |
| CU-03 | Given un jefe de área, When crea un relevamiento con tramo no vacío, Then queda en recolección y visible | TC-05, TC-06, TC-20, TC-22 | Integración / Unitario / Rendimiento / Snapshot | Pendiente |
| CU-04 | Given un relevamiento en recolección y agentes disponibles, When el jefe asigna, Then los muestra asignados sin duplicar | TC-07, TC-08 | Integración / Unitario | Pendiente |
| CU-05 | Given un relevamiento en recolección en el mapa, When el jefe crea y mueve un marcador, Then se fija con identidad estable | TC-09 | Componente UI / Integración | Pendiente |
| CU-06 | Given un relevamiento en revisión con marcadores con fotos, When avanza el carrusel al final, Then encadena con el marcador contiguo | TC-10, TC-11, TC-20, TC-22 | Componente UI / Integración / Rendimiento / Snapshot | Pendiente |
| CU-07 | Given un conflicto en revisión, When el jefe unifica, Then resulta un único marcador con la evidencia de ambos | TC-12, TC-13, TC-22 | Integración / Unitario / Snapshot | Pendiente |
| CU-08 | Given un relevamiento en revisión con conflicto sin resolver, When intenta cerrar, Then se bloquea y deriva a resolución | TC-14, TC-15, TC-20 | Integración / Unitario / Rendimiento | Pendiente |
| CU-09 | Given un agente con radio definido, When sube fotos dentro del radio, Then se agrupan en un único marcador | TC-16 | Integración | Pendiente |
| CU-10 | Given un relevamiento cerrado, When el jefe lo exporta, Then obtiene una única unidad transferible | TC-17 | Integración | Pendiente |
| CU-11 | Given un usuario raíz, When cambia el destino con datos completos, Then se actualiza transparente para los demás roles | TC-18 | Unitario / Integración | Pendiente |

Todos los CU tienen al menos un TC. No hay CU huérfano ni TC huérfano de CU/RN/NFR.

## 3. Trazabilidad NFR↔Tests

Cada NFR numérico de P.10 (intake §17 geovial-web) y los NFR de arquitectura 05 §8 tienen un test asociado y un tooling de medición.

| NFR | SLA / objetivo numérico | Test | Tooling (rol abstracto) |
| --- | --- | --- | --- |
| NFR latencia de interacción p95 | ≤ 200 ms sobre el circuito en red estable, vistas clave (CU-03, CU-06, CU-08) | TC-20 | Cliente de pruebas de rendimiento de interacción |
| NFR circuitos concurrentes | ≥ 50 circuitos interactivos sosteniendo p95 y sin pérdida de estado de sesión | TC-21 | Cliente de pruebas de carga de circuitos |
| NFR custodia del token | 0 exposiciones del token bearer al navegador | TC-19 | Prueba de componente de no exposición (motor headless de UI) |
| NFR disponibilidad mensual | ≥ 99,5 % del contenedor de front | Medición de disponibilidad (SLO observado en 09), no TC unitario | Métrica de disponibilidad del ambiente de referencia |
| NFR cobertura (gate de CI) | Líneas ≥ 80 %, branches ≥ 70 %, presentación ≥ 60 % | Gate de cobertura sobre la suite completa | Reporte de cobertura por capa del runtime |

Nota sobre disponibilidad: el objetivo de disponibilidad ≥ 99,5 % es un SLO observado en operación (categoría 09), no un caso de prueba unitario ejecutable; se declara aquí para completar la trazabilidad de los NFR numéricos y se valida por medición en el ambiente de referencia, no en CI. El resto de los NFR numéricos tiene TC ejecutable asociado.

## 4. Trazabilidad RN↔Tests

Cada RN se verifica por al menos un TC.

| RN | Invariante (resumen) | TC | Tipo |
| --- | --- | --- | --- |
| RN-01 | El front presenta solo pantallas y acciones del alcance del rol | TC-03, TC-07, TC-09, TC-16, TC-17, TC-18 | Unitario / Integración / Componente UI |
| RN-02 | La baja inhabilita el acceso y conserva la autoría visible | TC-04, TC-11 | Integración |
| RN-03 | El front es de administradores; el agente solo entra a la carga manual | TC-01, TC-02 | Integración / Unitario |
| RN-04 | El front habilita solo las acciones válidas para el estado vigente | TC-05, TC-06, TC-09, TC-10, TC-13, TC-14, TC-16, TC-22 | Unitario / Integración / Componente UI / Snapshot |
| RN-05 | No se ofrece el cierre con conflictos pendientes; conviven sin bloquear | TC-11, TC-12, TC-13, TC-14, TC-15 | Integración / Unitario |

## 5. Cobertura por capa

Umbrales objetivo por capa (estrategia-testing §2). Los valores observados se completan al ejecutar la suite; hoy figuran como pendientes por front no implementado. El front no tiene capa de Dominio propia (el dominio es de `geovial-api`).

| Capa | Líneas (%) | Branches (%) | Mutation score (%) | Umbral mínimo |
| --- | --- | --- | --- | --- |
| Dominio | no aplica | no aplica | no aplica | dominio en geovial-api |
| Aplicación de UI | pendiente | pendiente | — | 80 / 70 / — |
| Infraestructura (Cliente de API y adaptador de mapa) | pendiente | pendiente | — | 70 / 60 / — |
| Presentación (vistas y componentes) | pendiente | pendiente | — | 60 / 50 / — |
| Gate global del proyecto | pendiente | pendiente | — | 80 / 70 / — |

Reconciliación del gate global con la cobertura por capa: el gate global de §17 P.6 (líneas ≥ 80 %, branches ≥ 70 %) se mide sobre la unión de las tres capas; los pisos por capa se miden por separado. Ambos gates deben pasar simultáneamente: el global no compensa una capa por debajo de su piso. La presentación tiene piso propio de 60 % de líneas por mandato del intake; por debajo de ese valor el gate por capa falla aunque el agregado de 80 % se cumpla por compensación de las otras capas.

## 6. Gaps identificados

- Cobertura observada por capa pendiente de medir: el front no se implementó (todos los tramos del mini-plan de 07 pendientes). Plan de remediación: completar los valores observados al cierre de cada tramo y bloquear el merge con el gate de cobertura.
- TC de mapa y carrusel (TC-09, TC-10) dependen del cierre del spike BT-06 (caja temporal de 2 días, Tramo 3); hasta entonces el adaptador de mapa se prueba en aislamiento con un doble del componente. Plan de remediación: habilitar el e2e sobre el mapa real tras el informe del spike.
- Disponibilidad ≥ 99,5 % se valida por medición operativa en 09, no por TC en CI. Plan de remediación: el SLO se monitorea en producción y se reporta contra el NFR; el front depende además de la disponibilidad de `geovial-api`, medida contra el NFR homónimo de ese proyecto.
- Las ADR-04 y ADR-05 en estado Propuesto condicionan los TC de separación de capas y de mapeo de errores; su ratificación en Sprint 0 es precondición del Tramo 1.

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Matriz de cobertura inicial de geovial-web con las tres tablas obligatorias (CU↔Tests sobre los 11 CU, NFR↔Tests sobre los NFR de P.10, RN↔Tests sobre las 5 RN) más la tabla de cobertura por capa con umbrales y la reconciliación del gate global con los pisos por capa (presentación ≥ 60 %). Gaps con plan de remediación; estados Pendiente por front no implementado. |
