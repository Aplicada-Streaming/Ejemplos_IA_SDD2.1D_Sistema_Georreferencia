# Matriz de cobertura de pruebas — geovial-mobile

Proyecto: geovial-mobile
Documento: matriz-cobertura-pruebas_v1.0.md
Versión: 1.0
Estado: Propuesto
Fecha: 2026-06-15
Autor: Ingeniero QA / SDET (mobile)

## 1. Propósito y alcance

Documento bisagra de la categoría 08: relaciona los casos de uso, las reglas de negocio y los requerimientos no funcionales de `geovial-mobile` con los casos de prueba referenciales (TC-XX del `casos-prueba-referenciales_v1.0.md`). Contiene las tres tablas obligatorias (CU↔Tests, NFR↔Tests, RN↔Tests) más la tabla de cobertura por capa. Cubre los 7 CU (CU-01 a CU-07), las 5 RN (RN-01 a RN-05) y los NFR numéricos de P.10 (intake §17 geovial-mobile). El estado de los tests es Pendiente porque la app aún no se implementó; la matriz se actualiza al cierre de cada tramo del mini-plan de 07.

## 2. Trazabilidad CU↔Tests

Cada CU se lista con un criterio Given/When/Then representativo de 02, los TC que lo cubren, su tipo y su estado. Los TC cubren todos los escenarios de aceptación de cada CU (detalle por CA en el catálogo).

| CU | Criterio Given-When-Then representativo | Test ID | Tipo | Estado |
| --- | --- | --- | --- | --- |
| CU-01 | Given un agente habilitado con conexión, When ingresa credenciales válidas, Then obtiene el token, lo guarda en el almacén seguro y habilita el trabajo de campo | TC-01, TC-02, TC-03, TC-04, TC-26, TC-27 | Integración / Ciclo de vida / Snapshot | Pendiente |
| CU-02 | Given un agente con relevamientos en copia local, When selecciona uno, Then lo fija como contexto activo y abre su mapa con los marcadores locales | TC-05, TC-06, TC-07, TC-27, TC-28 | Interfaz móvil / Integración / Snapshot | Pendiente |
| CU-03 | Given relevamiento en recolección con permiso de ubicación, When centra por GPS y crea un marcador, Then lo crea local con identidad propia y lo encola | TC-08, TC-09, TC-10, TC-11, TC-27 | Sincronización / Unitario / Snapshot | Pendiente |
| CU-04 | Given un marcador activo con permisos y señal, When toma una foto, Then resuelve la coordenada del momento, la ancla a una observación y la encola | TC-12, TC-13, TC-14, TC-27 | Sincronización / Unitario / Integración / Snapshot | Pendiente |
| CU-05 | Given una foto de una observación en recolección, When escribe comentario y aplica etiqueta, Then los registra local y encola los cambios | TC-15, TC-16, TC-17, TC-27 | Integración / Unitario / Snapshot | Pendiente |
| CU-06 | Given cambios locales encolados y conexión recuperada, When sincroniza, Then sube primero y solo después baja, sin pérdida ni duplicación y conviviendo con conflictos | TC-18, TC-19, TC-20, TC-21, TC-24, TC-25, TC-27, TC-28 | Sincronización / Snapshot / Integración | Pendiente |
| CU-07 | Given un radio definido y fotos con ubicación incrustada, When las carga, Then las agrupa por radio priorizando la ubicación de la imagen | TC-22, TC-23 | Unitario | Pendiente |

Todos los CU tienen al menos un TC. No hay CU huérfano ni TC huérfano de CU/RN/NFR.

## 3. Trazabilidad NFR↔Tests

Cada NFR numérico de P.10 (intake §17 geovial-mobile) y los NFR de arquitectura 05 §8 tienen un test asociado y un tooling de medición. Todos los NFR numéricos del proyecto tienen TC ejecutable.

| NFR | SLA / objetivo numérico | Test | Tooling (rol abstracto) |
| --- | --- | --- | --- |
| NFR captura offline | 100 % de la captura de una observación con foto funciona sin conexión | TC-08, TC-12 | Doble del adaptador de conectividad en estado sin conexión |
| NFR capacidad de la cola local | La cola tolera ≥ 1000 cambios pendientes sin pérdida | TC-24 | Factoría determinista de cola y reporte del almacén local |
| NFR tiempo del ciclo de sincronización | Un lote de 100 cambios completa el ciclo ≤ 30 s en red móvil típica | TC-25 | Medidor de tiempo de ciclo sobre el dispositivo de referencia |
| NFR reanudación sin pérdida | El ciclo reanuda tras un corte sin pérdida ni duplicación | TC-19 | Doble de conectividad con corte y backend que deduplica por identificador de origen |
| NFR arranque en frío | Arranque en frío ≤ 3 s hasta la pantalla de sesión/verificación | TC-26 | Medidor de tiempo de arranque sobre el dispositivo de referencia |
| NFR cobertura (gate de CI) | Líneas ≥ 80 %, branches ≥ 70 %; lógica ≥ 75 %, presentación ≥ 60 % | Gate de cobertura sobre la suite completa | Reporte de cobertura por capa del runtime |

Nota: este proyecto no tiene SLO de disponibilidad ≥ 99,9 % ni objetivo de latencia p99 numérico (`tiene_observabilidad_critica = false`, P.10), por lo que no hay un NFR de disponibilidad que medir en operación; todos los NFR numéricos declarados son verificables por TC ejecutable en el ambiente de referencia.

## 4. Trazabilidad RN↔Tests

Cada RN se verifica por al menos un TC.

| RN | Invariante (resumen) | TC | Tipo |
| --- | --- | --- | --- |
| RN-01 | En la carga manual prioriza la ubicación incrustada de la imagen y agrupa por radio; nunca inventa coordenada (degradación a pendiente de ubicación) | TC-10, TC-13, TC-17, TC-22, TC-23 | Unitario |
| RN-02 | En cada ciclo de sincronización sube primero los cambios locales y solo después baja actualizaciones; el corte conserva los confirmados y deja el resto en cola | TC-18, TC-19, TC-25 | Sincronización |
| RN-03 | Trata los marcadores en conflicto por radio como estado válido: crea, conserva accesibles y sincroniza sin bloquear; difiere la resolución al cierre desde la web | TC-07, TC-09, TC-20 | Integración / Unitario / Sincronización |
| RN-04 | En sesión activa rehabilita por seguridad del dispositivo sin credenciales; inicio y cambio de usuario exigen inicio online; el token vive solo en almacén seguro | TC-01, TC-02, TC-03, TC-21 | Integración / Ciclo de vida / Sincronización |
| RN-05 | Toda captura de campo funciona sin conexión, se persiste local y se encola como cambio pendiente retenido hasta sync confirmada, sin pérdida | TC-04, TC-05, TC-06, TC-08, TC-11, TC-12, TC-14, TC-15, TC-16, TC-19, TC-24, TC-28 | Integración / Interfaz móvil / Sincronización / Unitario |

## 5. Cobertura por capa

Umbrales objetivo por capa (estrategia-testing §2). Los valores observados se completan al ejecutar la suite; hoy figuran como pendientes por app no implementada. Las capas internas son las de la arquitectura 05 §2 (ADR-01); el intake (§17 P.6) fija pisos para la capa de lógica (Aplicación + Dominio local) y para la de presentación.

| Capa | Líneas (%) | Branches (%) | Mutation score (%) | Umbral mínimo |
| --- | --- | --- | --- | --- |
| Lógica (Aplicación + Dominio local) | pendiente | pendiente | — | 75 / 70 / — |
| Infraestructura (almacén local, adaptadores de plataforma, cliente REST) | pendiente | pendiente | — | 70 / 60 / — |
| Presentación (vistas, modelos de vista y componente de mapa) | pendiente | pendiente | — | 60 / 50 / — |
| Gate global del proyecto | pendiente | pendiente | — | 80 / 70 / — |

Reconciliación del gate global con la cobertura por capa: el gate global de §17 P.6 (líneas ≥ 80 %, branches ≥ 70 %) se mide sobre la unión de las capas; los pisos por capa (lógica 75 / presentación 60) se miden por separado. Ambos gates deben pasar simultáneamente: el global no compensa una capa por debajo de su piso. La presentación tiene piso propio de 60 % de líneas por mandato del intake; por debajo de ese valor el gate por capa falla aunque el agregado de 80 % se cumpla por compensación de las otras capas. Como la mayor parte del código verificable del proyecto es lógica de captura y sincronización, para alcanzar el agregado de 80 / 70 la capa de lógica y la de infraestructura deben superar holgadamente sus pisos de 75 / 70 y 70 / 60 respectivamente; el piso de presentación de 60 / 50 es el más bajo porque la presentación es delgada (las vistas solo enlazan a sus modelos de vista, sin lógica de captura ni de sincronización, ADR-01).

## 6. Gaps identificados

- Cobertura observada por capa pendiente de medir: la app no se implementó (todos los tramos del mini-plan de 07 pendientes). Plan de remediación: completar los valores observados al cierre de cada tramo y bloquear el merge con el gate de cobertura global y por capa.
- Los TC de NFR en dispositivo (TC-24 cola ≥ 1000, TC-25 ciclo de 100 cambios ≤ 30 s, TC-26 arranque ≤ 3 s) dependen de la disponibilidad del dispositivo Android de referencia y del canal de distribución del paquete (riesgo del mini-plan 07 §6). Plan de remediación: hasta tener dispositivo, la cola ≥ 1000 (TC-24) se verifica con almacén local efímero sin interfaz, y el ciclo y el arranque se estiman con el doble del backend; el cierre del Tramo 3 exige la medición real en dispositivo.
- El TC-25 (tiempo de ciclo) y el TC-18/TC-19 (contrato subir-luego-bajar y reanudación idempotente) dependen del contrato de la librería de sincronización `aplicada-sync`; si el contrato no está publicado se usa un doble del motor (Ready condicional, mini-plan 07 §6). Plan de remediación: ejercitar el contrato real al integrar `aplicada-sync` en el Tramo 3.
- Las ADR-01 a ADR-05 en estado Propuesto condicionan los TC de capas, de sincronización, de permisos y de autenticación; su ratificación en Sprint 0 es precondición del Tramo 1. Plan de remediación: ratificar las ADR antes de cerrar el Tramo 1.

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Matriz de cobertura inicial de geovial-mobile con las tres tablas obligatorias (CU↔Tests sobre los 7 CU, NFR↔Tests sobre los NFR numéricos de P.10, RN↔Tests sobre las 5 RN) más la tabla de cobertura por capa con umbrales y la reconciliación del gate global (≥ 80 % / ≥ 70 %) con los pisos por capa (lógica 75 / presentación 60). Todos los NFR numéricos con TC ejecutable. Gaps con plan de remediación; estados Pendiente por app no implementada. |
