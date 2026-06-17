# Backlog técnico — geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** backlog-tecnico_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Scrum Master + Mobile Lead
**Estimación:** Story points con escala Fibonacci (1, 2, 3, 5, 8, 13); los spikes llevan caja temporal explícita

Vista del backlog desde la lente técnica de la app de campo offline-first `geovial-mobile` (tipo `mobile-app-maui`). Cada BT se justifica en una fuente upstream de 01, 02 o 05 (NB, CU, RN, ADR, componente de la vista lógica, modelo lógico del almacén local o contrato consumido) y declara al menos una US consumidora, o se justifica como infraestructura compartida con ADR explícita. Identificadores de dos dígitos uniformes: `BT-01` a `BT-13`, `ET-01` a `ET-06`. Vocabulario abstracto de plataforma móvil; los stacks concretos viven en el intake §17. Modo inline (13 BT < 30; §3.3 de las reglas).

## 1. Épicas técnicas

Las épicas técnicas agrupan las BT por capa arquitectónica y capacidad móvil transversal (offline, sincronización, permisos, ciclo de vida). Cada una declara objetivo, alcance, fuente upstream y BT contenidas.

### ET-01 — Almacén local, esquema y migraciones

- Objetivo: materializar el almacén local persistente que sostiene la captura offline, con un esquema replicado del dominio autoritativo y migraciones versionadas auditables entre versiones de la app.
- Alcance: estructura de las ocho tablas locales y sus dos asociaciones, índices y restricciones de réplica, transacción local de captura (entidad y cambio encolado juntos o ninguno) y migración inicial aplicada en el arranque.
- Fuente upstream: ADR-02 (persistencia en almacén local con migraciones versionadas), modelo lógico del almacén local (05), RN-05; componente repositorio del almacén local de la capa de infraestructura.
- BT contenidas: BT-01, BT-02.

### ET-02 — Cola de cambios y trabajo offline

- Objetivo: acumular cada captura como un cambio en la cola local persistente con orden de creación e identificador de origen estable, tolerando el volumen objetivo sin pérdida.
- Alcance: cola única por identificador de origen, drenado en orden de creación de solo los pendientes, persistencia transaccional con su entidad y verificación de la capacidad objetivo.
- Fuente upstream: ADR-02 (cola con orden e identificador de origen), RN-05, RN-02; modelo lógico (cambio_encolado, índices de cola); NFR de capacidad de la cola local (>= 1000 cambios).
- BT contenidas: BT-03, BT-04.

### ET-03 — Mapa, ubicación y captura georreferenciada

- Objetivo: integrar el componente de mapa local, centrar por la ubicación del dispositivo, crear y mover marcadores conservando su identidad y capturar fotos resolviendo la coordenada en el momento, todo sin conexión.
- Alcance: componente de mapa de la presentación de captura con eventos de posición y de gesto sobre el marcador; adaptador de ubicación; adaptador de cámara y resolución de coordenada en el momento; alta y movimiento de marcadores y anclaje de fotos a observaciones.
- Fuente upstream: ADR-01 (estilo offline-first y componente de mapa en la presentación), RN-05, RN-01; CU-03, CU-04; `flujo-ejecucion_v1.0.md` (captura offline).
- BT contenidas: BT-05, BT-06.

### ET-04 — Permisos del sistema operativo y degradación

- Objetivo: gobernar de forma centralizada los permisos del dispositivo (ubicación, cámara, acceso a archivos) con solicitud en el primer uso y degradación explícita sin caer ni inventar datos.
- Alcance: adaptadores de plataforma que encapsulan la solicitud y el chequeo de cada permiso; enrutado a la degradación por permiso negado o revocado, por falta de señal de ubicación y por falta de espacio de almacenamiento.
- Fuente upstream: ADR-04 (gestión de permisos con degradación), RN-01 (no inventar coordenada), RN-05; CU-03, CU-04, CU-07; regla 05 §2.2 (decisión de permisos obligatoria para el tipo).
- BT contenidas: BT-07, BT-08.

### ET-05 — Sesión, almacenamiento seguro del token y ciclo de vida

- Objetivo: orquestar los tres modos de sesión (inicio en línea, relogueo por seguridad del dispositivo y deslogueo completo) custodiando el token en el almacenamiento seguro del dispositivo y reaccionando al ciclo de vida del sistema operativo.
- Alcance: servicio de sesión y adaptador de almacenamiento seguro; relogueo por seguridad del dispositivo ante reinicio o desbloqueo; deslogueo completo que borra token y datos de sesión; arranque en frío y reanudación dentro del objetivo de arranque.
- Fuente upstream: ADR-05 (token seguro y relogueo por seguridad del dispositivo), RN-04; CU-01; NB-01; `flujo-ejecucion_v1.0.md` (arranque y sesión); NFR de arranque en frío (<= 3 s).
- BT contenidas: BT-09, BT-10.

### ET-06 — Sincronización por consumo del motor y convivencia con conflictos

- Objetivo: orquestar el ciclo subir-luego-bajar consumiendo el motor de la librería de sincronización por contrato, con disparo por detección de conectividad, reanudación idempotente y convivencia no bloqueante con los marcadores en conflicto en el cliente.
- Alcance: adaptador de la librería de sincronización que implementa los puertos requeridos (referencia al almacén local, al backend remoto y proveedor de credencial); servicio de sincronización que ejecuta el ciclo, habilita el disparo automático, consulta estado y reanuda; aplicación y reporte de conflictos en la copia local sin abortar.
- Fuente upstream: ADR-03 (sincronización por consumo del motor subir-luego-bajar), RN-02, RN-03, RN-05; CU-06, CU-02; NB-04; contrato consumido de la librería de sincronización y endpoints de subida/bajada del contrato REST; `flujo-ejecucion_v1.0.md` (sincronización y reanudación); NFR de tiempo de ciclo y de reanudación.
- BT contenidas: BT-11, BT-12, BT-13.

## 2. BT por épica

Tipos: spike, feature, refactor, devops, docs. Prioridad declarada por su aporte al MVP. Estimación en SP Fibonacci; los spikes llevan caja temporal.

### ET-01 — Almacén local, esquema y migraciones

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-01 | Crear el esquema del almacén local con índices y restricciones de réplica | feature | Alta | 5 SP | ADR-02; modelo lógico §1-§3 (8 tablas + 2 asociaciones); RN-05 | — | Existen las ocho tablas y las dos asociaciones con sus claves, índices y restricciones del modelo lógico; la referencia obligatoria de observación a marcador se respeta; el identificador de origen del cambio encolado es único; a lo sumo un texto por foto; el cliente no transiciona el estado del relevamiento (solo lectura) |
| BT-02 | Aplicar migraciones versionadas en el arranque con migración inicial auditable | feature | Alta | 3 SP | ADR-02; modelo lógico §4 (migración inicial); intake §17.P.4 | BT-01 | La migración inicial identificada construye el esquema desde cero y fija la versión de esquema local; el esquema se reconstruye de forma reproducible desde la migración inicial; una versión de esquema nueva se aplica en el arranque sin pérdida de datos previos; sin nombrar producto de almacén concreto |

### ET-02 — Cola de cambios y trabajo offline

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-03 | Implementar la cola de cambios con orden de creación e identificador de origen y persistencia transaccional | feature | Alta | 5 SP | ADR-02; modelo lógico (cambio_encolado, índices de cola); RN-05, RN-02; CU-03, CU-04, CU-05 | BT-01 | Cada captura persiste su entidad y su cambio encolado en una sola transacción local (ambos o ninguno); la cola conserva el orden de creación; un identificador de origen repetido no crea una segunda entrada; el drenado entrega solo los pendientes en orden y deja los confirmados fuera |
| BT-04 | Verificar el volumen objetivo de la cola local sin pérdida | feature | Media | 3 SP | NFR de capacidad de la cola (>= 1000); ADR-02 | BT-03 | La cola encola, consulta y drena correctamente con al menos 1000 cambios pendientes sin pérdida ni alteración del orden; el tamaño reportado coincide con las entradas únicas; la consulta de estado no altera la cola |

### ET-03 — Mapa, ubicación y captura georreferenciada

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-05 | Integrar el componente de mapa local y la creación y movimiento de marcadores | feature | Alta | 5 SP | ADR-01 (componente de mapa en la presentación de captura); RN-03, RN-05; CU-03; `flujo-ejecucion_v1.0.md` §5 | BT-03 | El componente de mapa local centra por la ubicación del dispositivo cuando hay permiso y señal; crear un marcador lo persiste con identidad propia y lo encola; mover un marcador actualiza la coordenada conservando su identidad y las observaciones ancladas; un marcador dentro del radio de otro convive como posible conflicto sin bloquear |
| BT-06 | Capturar foto y resolver la coordenada en el momento anclándola a una observación | feature | Alta | 5 SP | RN-01, RN-05; CU-04; modelo lógico (foto_local, observacion_local, origen de ubicación); `flujo-ejecucion_v1.0.md` §5 | BT-03, BT-05 | Con señal disponible, la foto resuelve su coordenada en el momento, se ancla a una observación del marcador y se encola; sin señal, la foto se conserva pendiente de ubicación sin coordenada inventada; varias fotos pueden compartir un mismo marcador; el binario se aloja en el dispositivo y se referencia lógicamente |

### ET-04 — Permisos del sistema operativo y degradación

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-07 | Centralizar la solicitud y el chequeo de permisos en los adaptadores de plataforma | feature | Alta | 5 SP | ADR-04; regla 05 §2.2; CU-03, CU-04, CU-07 | BT-05 | Cada permiso (ubicación, cámara, acceso a archivos) se solicita en el primer uso y se chequea antes de cada operación que lo requiere; ninguna vista solicita permisos directamente; el estado del permiso se expone a los servicios de captura; una revocación posterior se detecta antes de la siguiente operación |
| BT-08 | Implementar las degradaciones explícitas por permiso, falta de señal y falta de espacio | feature | Media | 5 SP | ADR-04; RN-01, RN-05; CU-03 (5.C), CU-04 (5.A) | BT-06, BT-07 | Ubicación denegada degrada a fijación manual del marcador sin centrar; cámara denegada no abre la cámara y explica el permiso; acceso a archivos denegado no accede a las fotos y explica el permiso; sin señal la foto queda pendiente de ubicación sin coordenada inventada; sin espacio no se persiste el binario, se avisa y se conserva lo ya encolado |

### ET-05 — Sesión, almacenamiento seguro del token y ciclo de vida

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-09 | Custodiar el token en el almacenamiento seguro y consumir el contrato de sesión | feature | Alta | 5 SP | ADR-05; NB-01; CU-01; endpoints de sesión del contrato REST | BT-01 | El inicio en línea con credenciales obtiene el token del backend y lo guarda en el almacenamiento seguro del dispositivo; el token nunca se guarda ni se registra en texto plano; el cliente del contrato lo presenta como credencial portadora en cada solicitud salvo el inicio; las credenciales no se persisten |
| BT-10 | Orquestar los tres modos de sesión y el relogueo por seguridad del dispositivo en el ciclo de vida | feature | Alta | 5 SP | ADR-05; RN-04; CU-01; `flujo-ejecucion_v1.0.md` §4; NFR de arranque (<= 3 s) | BT-09 | Con sesión activa, el reinicio o el desbloqueo exigen verificación por la seguridad del dispositivo sin reingresar credenciales; sin seguridad configurada en el dispositivo, la app advierte y exige inicio en línea; el deslogueo completo borra token y datos de sesión y libera el dispositivo; un token vencido en la reanudación exige nuevo inicio en línea; arranque en frío hasta la pantalla de sesión o verificación dentro del objetivo |

### ET-06 — Sincronización por consumo del motor y convivencia con conflictos

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-11 | Integrar el adaptador de la librería de sincronización implementando sus puertos | feature | Alta | 5 SP | ADR-03; contrato consumido de la librería (inicializar sesión, encolar, ejecutar, reanudar, consultar estado); CU-06 | BT-03, BT-09 | El adaptador provee los puertos requeridos por el motor: referencia al almacén local, referencia al backend remoto sobre el cliente del contrato y proveedor de credencial sobre el almacenamiento seguro; el arranque de la sesión inicializa el motor con la configuración de sesión; un puerto faltante se manifiesta como error de configuración y no como caída; la app fija la versión mayor de los contratos consumidos |
| BT-12 | Orquestar el ciclo subir-luego-bajar con detección de conectividad y reanudación idempotente | feature | Alta | 8 SP | ADR-03; RN-02, RN-05; CU-06; endpoints de subida y bajada del contrato REST; NFR de tiempo de ciclo y de reanudación | BT-11 | La detección de conectividad habilita el disparo automático y el agente también puede forzarlo; el ciclo sube primero todos los cambios locales y solo después baja las actualizaciones; la cola vacía omite la subida y baja directamente; un token rechazado detiene el ciclo, conserva la cola intacta y solicita reloguear; un corte durante la subida reanuda desde el punto de corte reconociendo los reenvíos por identificador de origen, sin pérdida ni duplicación |
| BT-13 | Aplicar y reportar los marcadores en conflicto y exponer el estado de sincronización | feature | Media | 3 SP | ADR-03; RN-03; CU-06 (5.B); modelo lógico (marcador_local.en_conflicto, índice de conflicto) | BT-12 | Una bajada con un marcador en conflicto por radio se aplica a la copia local sin abortar el ciclo y se reporta como elemento en conflicto en el resumen; un disparo forzado mientras hay un ciclo activo para ese relevamiento no inicia un segundo ciclo y muestra el estado del vigente; la consulta de estado expone situación, tamaño de cola, marca, conflictos conocidos y progreso parcial sin alterar la cola |

## 3. Trazabilidad BT↔US↔CU

Para cada BT: las US que la consumen y los CU upstream. Todas las BT tienen al menos una US consumidora; las marcadas como infraestructura compartida se justifican además con ADR explícita.

| BT | Título | US consumidoras | CU upstream | Fuente upstream principal |
| --- | --- | --- | --- | --- |
| BT-01 | Esquema del almacén local con índices y restricciones de réplica | US-03, US-05, US-07, US-09, US-14 | CU-02, CU-03, CU-04, CU-05, CU-07 | ADR-02; modelo lógico (infraestructura compartida) |
| BT-02 | Migraciones versionadas en el arranque con migración inicial | US-03, US-05, US-07, US-09 | CU-02, CU-03, CU-04, CU-05 | ADR-02; modelo lógico §4 (infraestructura compartida) |
| BT-03 | Cola de cambios con orden e identificador de origen y transacción local | US-05, US-07, US-09, US-11, US-14 | CU-03, CU-04, CU-05, CU-06, CU-07 | ADR-02; RN-05, RN-02 |
| BT-04 | Verificación del volumen objetivo de la cola local | US-11, US-12 | CU-06 | NFR de capacidad de la cola; ADR-02 |
| BT-05 | Componente de mapa local y creación y movimiento de marcadores | US-05, US-06 | CU-03 | ADR-01; RN-03, RN-05 |
| BT-06 | Captura de foto y resolución de coordenada anclada a la observación | US-07, US-08 | CU-04 | RN-01, RN-05; modelo lógico (foto_local) |
| BT-07 | Solicitud y chequeo de permisos centralizado en adaptadores | US-05, US-07, US-14 | CU-03, CU-04, CU-07 | ADR-04; regla 05 §2.2 |
| BT-08 | Degradaciones por permiso, falta de señal y falta de espacio | US-05, US-08, US-15 | CU-03, CU-04, CU-07 | ADR-04; RN-01, RN-05 |
| BT-09 | Token en almacenamiento seguro y consumo del contrato de sesión | US-01 | CU-01 | ADR-05; NB-01 |
| BT-10 | Tres modos de sesión y relogueo por seguridad del dispositivo | US-01, US-02, US-11 | CU-01, CU-06 | ADR-05; RN-04 |
| BT-11 | Adaptador de la librería de sincronización con sus puertos | US-11, US-12 | CU-06 | ADR-03; contrato consumido (infraestructura compartida) |
| BT-12 | Ciclo subir-luego-bajar con conectividad y reanudación idempotente | US-04, US-11, US-12 | CU-02, CU-06 | ADR-03; RN-02, RN-05 |
| BT-13 | Convivencia con conflictos y estado de sincronización | US-13 | CU-06 | ADR-03; RN-03 |

Notas de justificación de infraestructura compartida:

- BT-01 (esquema del almacén local) es la base sobre la que se apoya toda la captura offline y la cola; su justificación de infraestructura compartida es ADR-02 y el modelo lógico. Aun así se declaran las US consumidoras principales por trazabilidad.
- BT-02 (migraciones versionadas) habilita la evolución auditable del esquema entre versiones de la app; su justificación de infraestructura compartida es ADR-02 y §4 del modelo lógico. Se declaran las US que dependen de un esquema local disponible.
- BT-11 (adaptador de la librería de sincronización) es la frontera de integración del motor consumido; su justificación de infraestructura compartida es ADR-03 y el contrato consumido. Se declaran las US de sincronización que dependen del motor inicializado.

Cobertura: las 15 US tienen al menos una BT que las soporta (US-06 por BT-05; US-09 y US-10 por BT-01 y BT-03 vía la persistencia de comentarios y etiquetas; US-15 por BT-08); los 7 CU quedan cubiertos por al menos una BT; las 13 BT tienen fuente upstream y US consumidora o justificación de infraestructura compartida. No hay BT huérfana ni US sin soporte técnico.

Cobertura por CU:

| CU | Capacidad | BT que lo cubren |
| --- | --- | --- |
| CU-01 | Sesión y seguridad | BT-09, BT-10 |
| CU-02 | Selección de relevamiento | BT-01, BT-02, BT-12 |
| CU-03 | Mapa y marcadores | BT-01, BT-03, BT-05, BT-07, BT-08 |
| CU-04 | Captura de foto georreferenciada | BT-01, BT-03, BT-06, BT-07, BT-08 |
| CU-05 | Comentarios y etiquetas | BT-01, BT-03 |
| CU-06 | Offline y sincronización | BT-03, BT-04, BT-10, BT-11, BT-12, BT-13 |
| CU-07 | Carga manual | BT-01, BT-03, BT-07, BT-08 |

## 4. Referencias cruzadas

- Vista de producto: `product-backlog_v1.0.md` (épicas EP-XX, US-XX, MoSCoW, story points y métricas).
- Filtro de entrada: `definition-of-ready_v1.0.md` (DoR para BT).
- Upstream: NB-01, NB-03, NB-04 (01); especificación funcional, CU-01 a CU-07 y RN-01 a RN-05 (02); arquitectura de solución, ADR-01 a ADR-05, modelo lógico del almacén local y flujo de ejecución (05); contratos consumidos de la librería de sincronización y del backend (`aplicada-sync`, `geovial-api`).
- Downstream: 07 (sprint plan, asignación a sprint y velocity), 08 (acceptance tests desde los escenarios Given/When/Then y pruebas de migración, capacidad de cola, reanudación y convivencia con conflictos).

## 5. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Backlog técnico inicial de geovial-mobile: 6 épicas técnicas (ET-01 a ET-06) por capa arquitectónica y capacidad móvil, 13 BT inline (BT-01 a BT-13) con tipo, prioridad, estimación Fibonacci, fuente upstream, dependencias y criterios, matriz cruzada BT↔US↔CU y cobertura por CU. Cubre las diez capacidades técnicas del tipo mobile-app-maui (almacén local y migraciones, cola, motor de sincronización por consumo, conectividad, captura de foto y resolución de coordenadas, permisos, mapa local, token seguro y relogueo, conflictos en cliente y ciclo de vida). Derivado de los ADR-01 a ADR-05, el modelo lógico del almacén local y el flujo de ejecución de 05. Modo inline por debajo del umbral de 30 BT. |
