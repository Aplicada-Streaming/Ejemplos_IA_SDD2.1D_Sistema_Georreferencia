# Arquitectura de solución — geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** arquitectura-solucion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto Móvil

## 1. Objetivo

Este documento describe la arquitectura técnica de `geovial-mobile`, la app de campo offline-first del agente de relevamiento. Está dirigido a quien implementa el proyecto (06), planifica los sprints (07), prueba la integración (08) y opera el despliegue (09). Define el cómo estructural de la captura georreferenciada sin conexión, su almacén local y la sincronización delegada en la librería de sincronización, sin entrar en el detalle de implementación de cada historia de usuario. El detalle de cada decisión vive en los ADR individuales bajo `adrs/`; el almacén local en `modelo-datos-logico_v1.0.md`; el pipeline de captura y sincronización en `flujo-ejecucion_v1.0.md`.

## 2. Estilo arquitectónico

El estilo elegido es una aplicación móvil híbrida de plataforma única con patrón de presentación (MVVM) y diseño offline-first. La capa de presentación se construye con vistas embebidas (componentes de interfaz hospedados en un contenedor nativo de la plataforma móvil) y un componente de mapa para la georreferenciación; la lógica de presentación se separa de las vistas mediante modelos de vista que median entre la interfaz y los servicios de aplicación. El almacén local del dispositivo es la fuente de trabajo durante la captura; el dominio autoritativo es el de la API de relevamientos, que prevalece al sincronizar. La sincronización (detección de conectividad, orden subir-luego-bajar, idempotencia y reanudación) se delega íntegramente en la librería de sincronización que el proyecto consume, no se reimplementa.

La descomposición interna sigue una separación en capas locales: presentación (vistas y modelos de vista), aplicación (servicios de captura, sesión y orquestación de sincronización), dominio local (entidades replicadas y reglas de cola) e infraestructura (acceso al almacén local, adaptadores de plataforma para ubicación, cámara, almacenamiento de archivos, almacenamiento seguro de credenciales y cliente del contrato REST). Las dependencias apuntan hacia adentro: la presentación depende de la aplicación, la aplicación del dominio, y la infraestructura implementa puertos que la aplicación define.

Justificación contra alternativas descartadas:

| Criterio | App híbrida + MVVM offline-first (elegida) | App nativa sin patrón de presentación | Cliente delgado online dependiente de red |
| --- | --- | --- | --- |
| Trabajo sin conectividad | Captura 100 % offline con almacén y cola local (RN-05) | Posible, pero sin separación de capas la lógica de captura se acopla a la vista | Imposible: requiere red permanente, contradice NB-04 y F-07 |
| Separación lógica-presentación | MVVM aísla la lógica y la hace verificable sin interfaz | Lógica embebida en la vista; difícil de probar (intake §17.P.6) | No aplica el problema, pero no resuelve el offline |
| Reúso del motor de sincronización | Consume la librería de sincronización por contrato (intake §14) | Reimplementaría la sincronización en la app, contra el reúso pedido | Sincronización trivial pero sin capacidad offline |
| Convivencia con conflictos | El almacén local admite marcadores en conflicto como estado válido (RN-03) | Posible pero sin modelo de cola explícito | No aplica: no hay captura local |

Se descartan: (a) app nativa sin patrón de presentación, porque acopla la lógica de captura a las vistas y dificulta probar el modo offline y la sincronización por separado (intake §17.P.6); (b) cliente delgado dependiente de conectividad, porque contradice el requisito offline-first de NB-04 y la capacidad F-07. La decisión de estilo se formaliza en ADR-01.

## 3. Vista lógica

Componentes con responsabilidad, entradas, salidas y dependencias unidireccionales. Cada componente declara los CU que cubre.

| Componente | Responsabilidad | Entradas | Salidas | Dependencias | CU cubiertos |
| --- | --- | --- | --- | --- | --- |
| Presentación de sesión | Vistas y modelos de vista de inicio de sesión, deslogueo completo y relogueo por seguridad del dispositivo | Gestos del agente; estado de sesión | Solicitudes de sesión; navegación | Servicio de sesión | CU-01 |
| Presentación de captura | Vistas y modelos de vista del mapa, marcadores, fotos, comentarios y etiquetas | Gestos del agente; eventos del componente de mapa | Comandos de captura | Servicio de captura; componente de mapa | CU-02, CU-03, CU-04, CU-05, CU-07 |
| Presentación de sincronización | Vista de estado de sincronización, cola pendiente y disparo manual | Gestos del agente; eventos de progreso | Solicitudes de ciclo | Servicio de sincronización | CU-06 |
| Servicio de sesión | Orquesta inicio online, relogueo por seguridad del dispositivo y deslogueo; custodia del token | Credenciales; verificación del dispositivo | Token bearer custodiado; estado de sesión | Adaptador de almacenamiento seguro; cliente del contrato REST | CU-01 |
| Servicio de captura | Aplica las reglas de captura offline: crear/mover marcador, resolver coordenadas, anclar foto, comentar, etiquetar, carga manual con radio | Comandos de captura; ubicación; imágenes | Entidades locales; cambios encolados | Repositorio del almacén local; adaptadores de ubicación, cámara y archivos | CU-02, CU-03, CU-04, CU-05, CU-07 |
| Servicio de sincronización | Orquesta el ciclo subir-luego-bajar invocando la librería de sincronización; refleja estado, conflictos y reanudación | Solicitud de ciclo; señal de conectividad | Resumen del ciclo; cola drenada; marca avanzada | Adaptador de la librería de sincronización; repositorio del almacén local | CU-06, CU-02 |
| Repositorio del almacén local | Persiste y consulta las entidades locales y la cola de cambios; aplica migraciones versionadas | Entidades y consultas | Filas del almacén local | Almacén local del dispositivo | CU-02..CU-07 |
| Adaptador de la librería de sincronización | Implementa los puertos que la librería de sincronización requiere (almacén local, backend remoto, proveedor de credencial) y consume sus operaciones | Configuración de sesión; cambios locales | Operaciones del motor; notificaciones | Cliente del contrato REST; repositorio del almacén local; servicio de sesión | CU-06 |
| Cliente del contrato REST | Consume el contrato REST del dominio autoritativo presentando el token bearer | Solicitudes; token | Respuestas del backend | Adaptador de red | CU-01, CU-06 |
| Adaptadores de plataforma | Median con ubicación/GPS, cámara, acceso a archivos del dispositivo y almacenamiento seguro; gestionan permisos y degradación | Solicitudes de la aplicación | Datos de plataforma o estado de permiso | Plataforma móvil única | CU-01, CU-03, CU-04, CU-07 |

Trazabilidad de cobertura: los 7 CU quedan cubiertos. CU-01 por presentación y servicio de sesión y los adaptadores; CU-02 a CU-05 y CU-07 por la presentación de captura y el servicio de captura sobre el almacén local; CU-06 por la presentación y el servicio de sincronización a través del adaptador de la librería. No hay CU sin componente.

## 4. Vista de procesos

La app es de un solo proceso de usuario sobre la plataforma móvil única, con concurrencia acotada para no bloquear la interfaz:

- Hilo de interfaz: atiende los gestos del agente y renderiza vistas y el componente de mapa. Las operaciones de captura se resuelven contra el almacén local de forma asincrónica para preservar la fluidez.
- Trabajo de captura: la creación de marcadores, la resolución de coordenadas, la captura de foto y el encolado del cambio se ejecutan fuera del hilo de interfaz y confirman su escritura en el almacén local antes de notificar a la vista (RN-05). Cada captura es una transacción local: o se persiste la entidad y su cambio encolado, o no se persiste ninguno.
- Orquestación de sincronización: el ciclo subir-luego-bajar corre en una tarea en segundo plano disparada por la señal de conectividad o por acción manual. Mientras corre, la captura sigue habilitada; los cambios nuevos se encolan y se incorporan al siguiente ciclo. El orden subir-antes-de-bajar es una garantía del motor consumido (RN-02); la app no atiende la bajada hasta que la subida del ciclo concluye.
- Manejo de estado en memoria: el estado de sesión (situación y token custodiado) y el estado de sincronización (situación del motor, tamaño de cola, marca, conflictos conocidos, progreso parcial) se mantienen en memoria durante la sesión y se rehidratan del almacén local y del almacenamiento seguro al arrancar.
- Ciclo de vida del sistema operativo: al pasar a segundo plano o ante reinicio, la app conserva la cola y la marca en el almacén local; al reanudar con sesión activa, exige relogueo por seguridad del dispositivo antes de exponer datos (RN-04). Un corte durante la subida deja la cola consistente y reanudable, sin pérdida ni duplicación (RN-02).

No hay consumidores concurrentes ni transacciones distribuidas en el cliente: la consistencia transaccional fina vive en el backend; el cliente solo garantiza atomicidad local de cada captura y la integridad de la cola.

## 5. Vista de despliegue

- Unidad de despliegue: un único paquete de aplicación instalable, distribuido por un canal interno (no tienda pública en v1, intake §17.P.7), firmado en el pipeline.
- Runtime objetivo: la plataforma móvil única declarada en el intake (un solo sistema operativo móvil; no se soportan otras plataformas en v1, intake §17.P.9). El paquete embebe su runtime y el contenedor de vistas embebidas.
- Dependencias de infraestructura del dispositivo: servicio de ubicación/GPS, cámara, acceso a archivos y galería, almacén local persistente y almacenamiento seguro provisto por la plataforma. Todas se acceden por adaptadores con gestión de permisos y degradación (ADR-04).
- Dependencias externas de red: el contrato REST del dominio autoritativo (consumido, no definido aquí; ver `proyectos/geovial-api/05_arquitectura_tecnica/contratos-rest_v1.0.md`) y, para la sincronización, la librería de sincronización que la app integra como paquete redistribuible (ver `proyectos/aplicada-sync/05_arquitectura_tecnica/contratos-abstractions_v1.0.md`). La app es consumidora en ambas aristas del manifiesto (`geovial-mobile → geovial-api`, `geovial-mobile → aplicada-sync`).
- Configuración: el identificador del host remoto y los parámetros de sesión se proveen a la librería de sincronización en la inicialización; los secretos y el token nunca viajan en texto plano y se custodian en el almacenamiento seguro del dispositivo (ADR-05).

## 6. Vista de datos

El almacén local del dispositivo es una réplica parcial del dominio autoritativo de la API de relevamientos, mantenida para trabajar offline-first; no es la fuente de verdad. Al sincronizar, la app sube los cambios locales y luego baja las actualizaciones del backend, que prevalecen.

- Persistencia: almacén local persistente del dispositivo con 8 entidades (RelevamientoLocal, MarcadorLocal, ObservacionLocal, FotoLocal, ComentarioLocal, EtiquetaLocal, CambioEncolado y MarcaSincronizacionLocal), con migraciones versionadas aplicadas en el arranque. El detalle de tipos físicos, índices, restricciones y migración inicial vive en `modelo-datos-logico_v1.0.md`.
- Cola de cambios: `CambioEncolado` es la cola local persistente que conserva el orden de creación y un identificador de origen estable por cambio para la idempotencia. Tolera al menos 1000 cambios pendientes (RN-05, NFR de §8). Un cambio se retira de la cola solo tras su confirmación en la subida (RN-05).
- Metadatos de sincronización: `MarcaSincronizacionLocal` registra el punto de sincronización por relevamiento; su valor es opaco y monótono. Los metadatos de sincronización los gestiona la librería de sincronización (intake §17.P.4); la app los persiste en el almacén local.
- Binarios de fotos: el binario de cada foto se aloja en el dispositivo hasta sincronizar, referenciado lógicamente desde `FotoLocal`; no se guarda el binario en la fila de datos.
- Relación con el dominio autoritativo: cada entidad local es réplica de una entidad del backend (Relevamiento, MarcadorGeografico, Observacion, Foto, Comentario, Etiqueta) o un artefacto de sincronización del cliente (cola y marca). La identidad estable del marcador, la referencia obligatoria de observación a marcador y la monotonía de la marca son invariantes que el backend gobierna y el cliente respeta como réplica.
- Caches y particionamiento: no aplica particionamiento ni sharding; el almacén local es de un único agente y único dispositivo. El estado en memoria de la sesión actúa como cache efímera de lo persistido.

## 7. Cross-cutting concerns

- Registro de eventos (logging): registro estructurado local de eventos de captura, permisos y ciclos de sincronización, sin volcar credenciales ni el token; útil para diagnóstico en el dispositivo de desarrollo. El proyecto declara `tiene_observabilidad_critica = false` (intake §17.P.10): no hay objetivo de disponibilidad ni de latencia p99 numérico, ni trazas distribuidas exigidas.
- Trazado (tracing) y métricas: se acota a contadores locales de tamaño de cola, duración del ciclo de sincronización y tiempo de arranque, que alimentan los NFR de §8; no hay correlación distribuida obligatoria.
- Manejo de errores: el cliente distingue defecto de integración (entrada inválida, recurso ausente) de condición transitoria (conectividad), conforme a la taxonomía del contrato de la librería de sincronización y a problem+json del contrato REST. Las condiciones transitorias no pierden datos y se resuelven reintentando o reanudando; los elementos en conflicto se reportan sin abortar el ciclo (RN-03). Los errores de permiso degradan la funcionalidad de forma explícita (ADR-04). La app nunca expone detalles internos al agente: informa con mensaje accionable.
- Configuración y secretos: el token y cualquier secreto se custodian en el almacenamiento seguro del dispositivo, nunca en texto plano ni en el registro (ADR-05). La configuración de sesión hacia la librería de sincronización se inyecta en la inicialización; el deslogueo completo borra el token y los datos de sesión del dispositivo.
- Permisos del sistema operativo: la solicitud, el chequeo y la degradación ante denegación de ubicación/GPS, cámara y acceso a archivos se centralizan en los adaptadores de plataforma y se gobiernan por una única política (ADR-04).

## 8. Quality attributes (NFR)

NFR derivados de intake §17.P.10 del proyecto, con objetivo numérico y mecanismo de medición.

| NFR | Objetivo numérico | Mecanismo de medición | ADR relacionada |
| --- | --- | --- | --- |
| Captura offline | 100 % de la captura de una observación con foto funciona sin conexión | Prueba de modo avión: crear marcador, capturar foto, comentar y etiquetar sin red; verificar persistencia y encolado en el almacén local (08) | ADR-01, ADR-02 |
| Capacidad de la cola local | ≥ 1000 cambios pendientes sin pérdida | Prueba de carga del almacén local: encolar 1000 cambios y verificar integridad y orden de creación (08) | ADR-02, ADR-03 |
| Tiempo de un ciclo de sincronización | Un lote de 100 cambios completa ≤ 30 s en red móvil típica | Cronometrado del ciclo subir-luego-bajar con 100 cambios en red de referencia (08) | ADR-03 |
| Reanudación sin pérdida | El ciclo reanuda tras un corte sin pérdida ni duplicación | Prueba de corte durante la subida y reanudación: verificar cola consistente, sin duplicados por identificador de origen (08) | ADR-03 |
| Arranque en frío | ≤ 3 s hasta la pantalla de sesión/verificación en el dispositivo de referencia | Cronometrado del arranque en frío en el dispositivo de referencia (08) | ADR-01, ADR-05 |

## 9. Riesgos arquitectónicos

| Riesgo | Impacto | Probabilidad | Mitigación |
| --- | --- | --- | --- |
| Pérdida o duplicación de datos en una sincronización interrumpida | Alto | Media | Cola local persistente, identificador de origen estable por cambio para idempotencia, orden subir-antes-de-bajar y reanudación delegados en la librería de sincronización (ADR-03, RN-02, RN-05) |
| Georreferenciación imprecisa por GPS pobre o foto sin ubicación incrustada | Medio | Media | No inventar coordenada: marcar la foto como pendiente de ubicación y permitir fijación manual del pin; radio de agrupación en carga manual (ADR-04, RN-01) |
| Permiso del sistema operativo denegado o revocado (ubicación, cámara, archivos) | Medio | Media | Degradación explícita por permiso: fijación manual sin GPS, bloqueo claro de cámara o galería con mensaje accionable, sin caída de la app (ADR-04) |
| Token comprometido en un dispositivo compartido | Alto | Baja | Almacenamiento seguro del dispositivo, relogueo por seguridad del dispositivo en reanudación y deslogueo completo que borra token y datos de sesión (ADR-05, RN-04) |
| Almacén local sin espacio durante la captura | Medio | Baja | Detección de espacio insuficiente, aviso al agente y no pérdida de lo ya encolado; el binario de foto no se persiste si no hay espacio (ADR-02, ADR-04) |
| Incompatibilidad con un cambio mayor del contrato REST o del contrato de sincronización | Medio | Baja | Ambos contratos declaran versionado con compatibilidad hacia atrás y período de convivencia; la app fija la versión mayor que consume (ADR-03) |

## 10. Trazabilidad

| CU | RN aplicables | ADRs que lo gobiernan | Componentes que lo cubren | Tests previstos (08) |
| --- | --- | --- | --- | --- |
| CU-01 Iniciar/cerrar sesión y relogueo | RN-04 | ADR-01, ADR-05 | Presentación y servicio de sesión; adaptadores de plataforma | Relogueo por seguridad del dispositivo; deslogueo borra token; arranque ≤ 3 s |
| CU-02 Seleccionar relevamiento asignado | RN-05, RN-02 | ADR-01, ADR-02, ADR-03 | Presentación de captura; servicio de captura; repositorio local | Selección desde almacén local sin red |
| CU-03 Centrar GPS y crear/mover marcador | RN-03, RN-05 | ADR-01, ADR-02, ADR-04 | Presentación y servicio de captura; adaptador de ubicación | Degradación a fijación manual sin permiso; encolado del marcador |
| CU-04 Capturar foto y resolver coordenadas | RN-01, RN-05 | ADR-02, ADR-04 | Servicio de captura; adaptadores de cámara, ubicación y archivos | Captura 100 % offline; foto pendiente de ubicación sin coordenada inventada; sin espacio avisa |
| CU-05 Agregar comentarios y etiquetas | RN-05, RN-03 | ADR-02 | Servicio de captura; repositorio local | Comentario y etiquetas offline; encolado |
| CU-06 Trabajar sin conexión y sincronizar | RN-02, RN-03, RN-05 | ADR-03 | Presentación y servicio de sincronización; adaptador de la librería | Cola ≥ 1000; 100 cambios ≤ 30 s; reanudación sin pérdida; conflictos conviven |
| CU-07 Carga manual con radio de agrupación | RN-01, RN-03 | ADR-02, ADR-04 | Servicio de captura; adaptador de archivos | Prioridad de ubicación incrustada; agrupación por radio; permiso de galería |

Trazabilidad NFR↔arquitectura↔ADR consolidada en §8. Downstream: las historias de usuario y el backlog técnico (06), el sprint plan (07), el testing técnico y de integración (08) y el despliegue del paquete (09) toman este documento como ancla.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Arquitectura inicial de geovial-mobile: estilo app híbrida con MVVM offline-first, cuatro vistas mínimas (lógica, procesos, despliegue, datos), almacén local de 8 entidades como réplica del dominio autoritativo, cross-cutting, NFR numéricos de captura offline, cola ≥ 1000, sincronización ≤ 30 s y arranque ≤ 3 s, riesgos y trazabilidad CU/RN/ADR. |
