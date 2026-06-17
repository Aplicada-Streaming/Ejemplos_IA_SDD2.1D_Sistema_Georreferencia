# Backlog técnico — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** backlog-tecnico_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + Backlog Curator (AG-06)
**Estimación:** Story points con escala Fibonacci (1, 2, 3, 5, 8, 13); los spikes llevan caja temporal explícita

Vista del backlog desde la lente técnica del motor de sincronización `aplicada-sync` (tipo `library`). Cada BT se justifica en una fuente upstream de 02 o 05 (ADR, componente de la vista lógica o contrato de la capa Abstractions) y declara al menos una US consumidora, o se justifica como infraestructura compartida con ADR explícita. Identificadores de dos dígitos uniformes: `BT-01` a `BT-14`, `ET-01` a `ET-06`. Vocabulario neutral de librería; los stacks concretos viven en el intake §17.

## 1. Épicas técnicas

Las épicas técnicas agrupan las BT por capa arquitectónica y concern transversal del motor. Cada una declara objetivo, alcance y fuente upstream.

### ET-01 — Fundaciones de la capa Abstractions

- Objetivo: establecer la frontera estable que aísla la superficie pública del host y habilita la inversión de dependencias.
- Alcance: definir los contratos de extensión (almacén local, transporte, credencial, conectividad), la operación de inicialización de sesión y el armado de la configuración con validación de coherencia.
- Fuente upstream: ADR-01 (Clean Architecture con capa Abstractions), ADR-02 (inversión de dependencias hacia adaptadores del host), `contratos-abstractions_v1.0.md` §3-§4, `extensibilidad_v1.0.md` §3-§5; componente Coordinador de sesión.
- BT contenidas: BT-01, BT-02, BT-03.

### ET-02 — Cola local persistente y ordenada

- Objetivo: materializar una cola de pendientes única por identificador estable que conserve el orden de creación y tolere el volumen objetivo.
- Alcance: estructura de metadatos de la cola en el almacén local, clave de unicidad por identificador, conservación de orden y consulta del tamaño.
- Fuente upstream: ADR-04 (cola local persistente y ordenada), ADR-07 (idempotencia por identificador estable), componente Cola de cambios locales pendientes; NFR de capacidad de cola (>= 1000).
- BT contenidas: BT-04, BT-05.

### ET-03 — Pipeline de orquestación subir-luego-bajar

- Objetivo: implementar el ciclo de dos fases con orden estricto como invariante dura, no configurable.
- Alcance: orquestador del ciclo, ejecutor de fase de subida, ejecutor de fase de bajada, exclusión mutua de ciclo y armado del resumen del ciclo.
- Fuente upstream: ADR-05 (pipeline de orden estricto subir-antes-de-bajar), RN-01, componentes Orquestador del ciclo y Ejecutores de fase; `flujo-ejecucion_v1.0.md`.
- BT contenidas: BT-06, BT-07, BT-08.

### ET-04 — Reanudación e idempotencia

- Objetivo: garantizar 0 perdidos y 0 duplicados ante cortes, reanudando desde el punto de corte.
- Alcance: marca de progreso persistida, reenvío de solo los no confirmados, reconocimiento por identificador en el backend y resolución de progreso inconsistente con la cola como fuente de verdad.
- Fuente upstream: ADR-06 (reanudación por marca de progreso), ADR-07 (idempotencia por identificador estable), RN-02, componentes Orquestador y Ejecutores de fase.
- BT contenidas: BT-09, BT-10.

### ET-05 — Conectividad, estado y convivencia con conflicto

- Objetivo: disparar a lo sumo un ciclo ante recuperación de red sin reentrada y exponer el estado, el progreso y los elementos en conflicto sin bloquear.
- Alcance: observador de conectividad con descarte de eventos redundantes, registro de estado y progreso, listado de elementos en conflicto convivientes.
- Fuente upstream: ADR-08 (convivencia con estados en conflicto), RN-03, componentes Observador de conectividad y Registro de estado y progreso; operación Consultar estado y cola del contrato.
- BT contenidas: BT-11, BT-12.

### ET-06 — Versionado, distribución y verificación de superficie pública

- Objetivo: asegurar que el paquete distribuible respete la política de compatibilidad y que el contrato sea reproducible tras la publicación.
- Alcance: catálogo estable de códigos de error, instrumentación de diagnóstico y métricas consultables, y verificación post-publicación con un quick-start.
- Fuente upstream: ADR-03 (versionado de la superficie pública), `contratos-abstractions_v1.0.md` §5-§6, cross-cutting de logging/métricas de `arquitectura-solucion_v1.0.md` §7; verificación post-publicación (intake §17 P.8).
- BT contenidas: BT-13, BT-14.

## 2. BT por épica

Tipos: spike, feature, refactor, devops, docs. Prioridad declarada por su aporte al MVP. Estimación en SP Fibonacci; los spikes llevan caja temporal.

### ET-01 — Fundaciones de la capa Abstractions

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-01 | Definir los contratos de extensión de la capa Abstractions | feature | Alta | 5 SP | ADR-01, ADR-02; `extensibilidad_v1.0.md` §3; `contratos-abstractions_v1.0.md` §4 | — | Existen las firmas de las estrategias de almacén local, transporte, credencial y conectividad; el núcleo programa solo contra las abstracciones; ningún componente del núcleo instancia un adaptador concreto; los contratos respetan las formas de datos del §4 del contrato |
| BT-02 | Implementar la operación de inicialización y el armado de la configuración | feature | Alta | 5 SP | Operación Inicializar sesión (contrato §3); componente Coordinador de sesión; CU-01 | BT-01 | La configuración completa inicializa la sesión en estado listo; falta de estrategia obligatoria devuelve CONFIGURACION_INCOMPLETA; ausencia de credencial deja estado no autenticada; no quedan estructuras parciales ante fallo |
| BT-03 | Spike de registro y resolución explícita de estrategias | spike | Media | 3 SP (caja temporal 2 días) | ADR-02 (alternativa de descubrimiento automático descartada); `extensibilidad_v1.0.md` §5 | BT-01 | Informe que confirma el registro explícito por el host sin descubrimiento automático; recomendación sobre obligatorias vs. opcionales; eleva hallazgos a la implementación de BT-02; sin recomendación clara al cierre, documenta el bloqueo |

### ET-02 — Cola local persistente y ordenada

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-04 | Implementar la cola persistente única por identificador estable | feature | Alta | 5 SP | ADR-04, ADR-07; componente Cola de cambios locales pendientes; CU-02 | BT-01, BT-02 | Una sola entrada por identificador estable; reencolar el mismo identificador no incrementa el tamaño y actualiza la carga útil; entradas con identificadores distintos conservan el orden de creación; el encolado sin identificador devuelve IDENTIFICADOR_CAMBIO_AUSENTE |
| BT-05 | Verificar el volumen objetivo de la cola local | feature | Media | 3 SP | NFR capacidad de cola (>= 1000); ADR-04 | BT-04 | La cola encola, consulta y ejecuta correctamente con >= 1000 pendientes sin degradación funcional; el tamaño reportado coincide con las entradas únicas; la consulta de estado no altera la cola |

### ET-03 — Pipeline de orquestación subir-luego-bajar

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-06 | Orquestar el ciclo de dos fases con orden estricto | feature | Alta | 8 SP | ADR-05, RN-01; componente Orquestador del ciclo; CU-03 | BT-02, BT-04 | La bajada no inicia hasta que la subida confirma cero pendientes confirmables; cola vacía omite la subida y baja igual; el resumen reporta subidos y bajados; 0 bajadas mientras quedan pendientes confirmables |
| BT-07 | Implementar los ejecutores de fase de subida y de bajada | feature | Alta | 5 SP | ADR-05, ADR-07; componentes Ejecutor de subida y Ejecutor de bajada; CU-03 | BT-06 | La subida envía en orden y retira de la cola lo confirmado por identificador; la bajada solicita actualizaciones posteriores a la marca y las aplica una sola vez por identidad; BACKEND_INALCANZABLE y CREDENCIAL_INVALIDA se reportan con código estable |
| BT-08 | Garantizar la exclusión mutua de un único ciclo por sesión | feature | Media | 3 SP | Vista de procesos (exclusión mutua); CU-03 flujo 5.C | BT-06 | Una segunda solicitud durante un ciclo en curso devuelve el estado de la ejecución vigente sin iniciar un segundo ciclo; sin ciclo en curso inicia uno solo; el encolado concurrente conserva orden e idempotencia |

### ET-04 — Reanudación e idempotencia

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-09 | Persistir la marca de progreso y reanudar desde el punto de corte | feature | Alta | 8 SP | ADR-06, RN-01; componentes Orquestador y Ejecutores; CU-06 | BT-06, BT-07 | Un corte en la subida deja la sesión reanudable con la marca persistida; la reanudación reenvía solo los no confirmados en orden; la bajada solo ocurre tras concluir la subida reanudada; 0 perdidos y 0 duplicados tras el corte |
| BT-10 | Resolver progreso inconsistente con la cola como fuente de verdad | refactor | Media | 3 SP | ADR-06 (cola como fuente de verdad); ADR-07; CU-06 flujo de progreso | BT-09 | Una marca de progreso que no concuerda con la cola devuelve PROGRESO_INCONSISTENTE y reconstruye desde la cola sin duplicar; el reconocimiento por identificador confirma los ya recibidos sin reaplicarlos; un nuevo corte conserva el avance |

### ET-05 — Conectividad, estado y convivencia con conflicto

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-11 | Implementar el observador de conectividad con descarte de rebote | feature | Media | 5 SP | Componente Observador de conectividad; CU-04; vista de procesos | BT-06 | Un evento de red disponible con disparo habilitado y sesión autenticada dispara a lo sumo un ciclo; el modo deshabilitado devuelve DISPARO_AUTOMATICO_DESHABILITADO; la sesión no autenticada devuelve SESION_NO_AUTENTICADA; los eventos redundantes durante un ciclo no generan reentrada |
| BT-12 | Componer el registro de estado, progreso y elementos en conflicto | feature | Media | 5 SP | ADR-08, RN-03; componente Registro de estado y progreso; operación Consultar estado y cola (contrato §3); CU-05 | BT-04, BT-07 | El estado expone situación, pendientes, última marca y progreso parcial sin alterar la cola; un conflicto reportado por el backend se incorpora y se lista sin abortar el ciclo; el listado de conflictos deja claro que el motor no los resuelve; sin sesión devuelve SESION_NO_INICIALIZADA |

### ET-06 — Versionado, distribución y verificación de superficie pública

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-13 | Establecer el catálogo estable de errores y el diagnóstico estructurado | feature | Media | 3 SP | `contratos-abstractions_v1.0.md` §5; cross-cutting de logging y métricas (`arquitectura-solucion_v1.0.md` §7) | BT-01 | Cada condición usa un código estable y único del catálogo; el código no se traduce ni cambia entre versiones menores; el diagnóstico incluye identificador de sesión, fase y código sin la carga útil de dominio; existen contadores consultables de subidos, bajados, conflictos, reintentos y reanudaciones |
| BT-14 | Verificar la compatibilidad de la superficie pública con un quick-start | devops | Media | 3 SP | ADR-03; `contratos-abstractions_v1.0.md` §6; verificación post-publicación (intake §17 P.8) | BT-02, BT-06, BT-09 | El paquete publicado se restaura en un proyecto limpio y el quick-start reproduce el contrato; un cambio incompatible queda detectado contra la matriz de compatibilidad; un quick-start que no reproduzca el comportamiento bloquea la publicación |

## 3. Trazabilidad BT↔US↔CU

Para cada BT: las US que la consumen y los CU upstream. Todas las BT tienen al menos una US consumidora salvo las de infraestructura compartida, que se justifican con ADR explícita.

| BT | Título | US consumidoras | CU upstream | Fuente upstream principal |
| --- | --- | --- | --- | --- |
| BT-01 | Contratos de extensión de la capa Abstractions | US-01, US-03, US-05, US-08 | CU-01, CU-02, CU-03, CU-04 | ADR-01, ADR-02 (infraestructura compartida) |
| BT-02 | Operación de inicialización y armado de configuración | US-01, US-02 | CU-01 | Contrato §3; componente Coordinador de sesión |
| BT-03 | Spike de registro y resolución de estrategias | US-01 | CU-01 | ADR-02; `extensibilidad_v1.0.md` §5 |
| BT-04 | Cola persistente única por identificador estable | US-03, US-04, US-10 | CU-02, CU-05 | ADR-04, ADR-07 |
| BT-05 | Verificación del volumen objetivo de la cola | US-03, US-10 | CU-02, CU-05 | NFR capacidad de cola; ADR-04 |
| BT-06 | Orquestación del ciclo de dos fases en orden estricto | US-05, US-08 | CU-03, CU-04 | ADR-05, RN-01 |
| BT-07 | Ejecutores de fase de subida y de bajada | US-05, US-06 | CU-03 | ADR-05, ADR-07 |
| BT-08 | Exclusión mutua de un único ciclo por sesión | US-07 | CU-03 | Vista de procesos; CU-03 flujo 5.C |
| BT-09 | Marca de progreso y reanudación desde el corte | US-12, US-13 | CU-06 | ADR-06, RN-01 |
| BT-10 | Resolución de progreso inconsistente | US-12, US-13 | CU-06 | ADR-06, ADR-07 |
| BT-11 | Observador de conectividad con descarte de rebote | US-08, US-09 | CU-04 | Componente Observador de conectividad |
| BT-12 | Registro de estado, progreso y conflictos | US-06, US-10, US-11 | CU-03, CU-05 | ADR-08, RN-03 |
| BT-13 | Catálogo de errores y diagnóstico estructurado | US-01, US-03, US-05, US-08, US-12 | CU-01, CU-02, CU-03, CU-04, CU-06 | Contrato §5 (infraestructura compartida) |
| BT-14 | Verificación de compatibilidad con quick-start | US-01, US-05, US-12 | CU-01, CU-03, CU-06 | ADR-03; contrato §6 |

Notas de justificación de infraestructura compartida:

- BT-01 (contratos de extensión) es la frontera estable que habilita toda la superficie pública; su justificación de infraestructura compartida es ADR-01 y ADR-02. Aun así se declaran las US consumidoras principales por trazabilidad.
- BT-13 (catálogo de errores y diagnóstico) es transversal a todas las operaciones; su justificación de infraestructura compartida es el §5 del contrato. Se declaran las US que dependen de códigos de error estables para sus criterios de aceptación.

Cobertura: las 13 US tienen al menos una BT que las soporta; los 6 CU quedan cubiertos por al menos una BT; las 14 BT tienen fuente upstream y US consumidora o justificación de infraestructura compartida.

## 4. Referencias cruzadas

- Vista de producto: `product-backlog_v1.0.md` (épicas EP-XX, US-XX, MoSCoW y métricas).
- Filtro de entrada: `definition-of-ready_v1.0.md` (DoR para BT).
- Upstream: NB-04 (01); CU-01 a CU-06 y RN-01 a RN-03 (02); ADR-01 a ADR-08, `contratos-abstractions_v1.0.md`, `extensibilidad_v1.0.md`, `arquitectura-solucion_v1.0.md` (05).

## 5. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Backlog técnico inicial de aplicada-sync: 6 épicas técnicas (ET-01 a ET-06) por capa arquitectónica y concern transversal, 14 BT inline (BT-01 a BT-14) con tipo, prioridad, estimación Fibonacci, fuente upstream, dependencias y criterios, y matriz cruzada BT↔US↔CU. Derivado de los ADR-01 a ADR-08, los componentes de la vista lógica y los contratos de la capa Abstractions de 05. |
