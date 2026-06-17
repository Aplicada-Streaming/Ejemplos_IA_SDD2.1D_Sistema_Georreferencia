# Arquitectura de solución — geovial-web

**Proyecto:** geovial-web
**Documento:** arquitectura-solucion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto Senior

## 1. Objetivo

Este documento describe la arquitectura técnica interna de `geovial-web`, el front web de render server-side de la solución GeoVial, herramienta de los roles administradores (usuario raíz, jefe general y jefe de área, más la excepción de carga manual del agente). El front recolecta, crea y revisa relevamientos sobre un mapa interactivo y consume por contrato la API REST de `geovial-api`; no posee persistencia de dominio propia (intake §17 geovial-web P.4, `tiene_persistencia=false`) ni expone una API externa. Está dirigido al equipo de desarrollo que construye y mantiene el front, a los revisores funcionales que validan la cobertura de los once casos de uso (CU-01 a CU-11) y a las categorías downstream 06 (backlog técnico), 08 (testing e integración) y 09 (despliegue del contenedor de front). Define el cómo estructural —estilo, vistas, cross-cutting, atributos de calidad y decisiones gobernantes— sin entrar en el detalle de interfaz fina, que vive en la sección 03 (UX/UI). El modelo de dominio autoritativo y su modelo lógico viven en `geovial-api`; aquí solo se referencian. La vista de solución y los contratos inter-proyecto de la solución viven en `_solucion/` y aquí solo se referencian.

## 2. Estilo arquitectónico

Se adopta un front de render server-side con circuito interactivo persistente: la interfaz se renderiza en el servidor y mantiene, por sesión de usuario, un circuito interactivo de larga vida que sincroniza el estado de la interfaz con el navegador a través de una conexión persistente. Sobre ese estilo se aplica una separación de capas en el cliente, de dependencia unidireccional hacia el núcleo de la aplicación de UI: Presentación (vistas y componentes de interfaz, incluido el componente de mapa), Aplicación de UI (orquestación de la interacción, gestión del estado de la sesión y de la pantalla, mapeo de errores a feedback) y Cliente de API (adaptador que traduce las intenciones del usuario en llamadas al contrato REST de `geovial-api` y normaliza sus respuestas). La Presentación depende de la Aplicación de UI; la Aplicación de UI depende de un puerto de acceso al dominio que el Cliente de API implementa; ninguna capa de presentación habla directamente con la red. Es una única unidad desplegable, fijada por requisito (intake §17.P.2).

El front no es dueño del dominio: todo dato autoritativo (usuarios, relevamientos, marcadores, observaciones, fotos, conflictos, configuración de almacenamiento) vive en `geovial-api`. El front mantiene únicamente estado de UI y de sesión efímero en el circuito interactivo. La tolerancia a conflictos de marcadores es una propiedad del dominio que el front presenta, no resuelve: convive con los conflictos durante la recolección y la revisión y solo ofrece la resolución (unificar o separar) al cierre del relevamiento (RN-05, RN-04 del front).

Justificación contra alternativas descartadas:

| Criterio | Render server-side con circuito persistente (elegido) | Cliente enriquecido que ejecuta en el navegador | Front de páginas sin estado interactivo persistente |
| --- | --- | --- | --- |
| Requisito del intake (§17.P.2) | Cumple: estilo fijado por requisito | Descartado por el requisito de render server-side interactivo | Descartado: no sostiene la interacción continua de mapa y carrusel |
| Custodia del token (§17.P.5) | El token se mantiene del lado servidor del circuito, no se expone al navegador | El token viajaría al navegador, ampliando la superficie de exposición | El token tendería a vivir en el cliente entre páginas |
| Interactividad de mapa y carrusel (CU-05, CU-06) | Directa: el circuito empuja actualizaciones de estado al navegador | Posible, pero duplica lógica de dominio en el cliente | Requiere recargas o lógica de cliente adicional |
| Complejidad operativa (equipo de 1 dev) | Baja: un modelo de desarrollo unificado y un solo artefacto | Media: dos modelos de ejecución y empaquetado de cliente | Baja, pero degrada la experiencia interactiva |
| Tolerancia a red del cliente | Exige conexión persistente con el servidor (trade-off aceptado, §17.P.12) | Tolera cortes mejor, a costa de lógica de dominio en el cliente | Tolera cortes, sin interacción rica |

- Cliente enriquecido que ejecuta en el navegador: descartado por el requisito explícito de render server-side (intake §17.P.2, §17.P.11) y porque obligaría a custodiar el token bearer en el navegador, contrario a §17.P.5 (el token se mantiene del lado servidor del circuito).
- Front de páginas sin estado interactivo persistente: descartado porque no sostiene la interacción continua del mapa con marcadores ni el carrusel encadenado de fotos (CU-05, CU-06), que requieren empuje de estado del servidor al navegador sin recargas.
- Front sin separación de capas (lógica de interacción y acceso a la API mezclada en las vistas): descartado porque impediría probar la lógica de presentación y el mapeo de errores sin levantar la red, y dispersaría el manejo del token y el contrato por todas las vistas.

Vistas C4 incluidas (cuatro vistas mínimas): vista lógica (§3, nivel componentes C4), vista de procesos (§4), vista de despliegue (§5, nivel contenedores C4) y vista de datos (§6, que referencia que el dominio es de `geovial-api` y que el front solo maneja estado de UI/sesión efímero).

## 3. Vista lógica

El front se descompone en componentes cohesivos agrupados por capa. La dependencia es unidireccional hacia el núcleo de la aplicación de UI: la Presentación depende de la Aplicación de UI; la Aplicación de UI depende de un puerto de acceso al dominio que el Cliente de API implementa. Los componentes transversales (sesión y token, mapeo de errores, control de visibilidad por rol, estado del circuito) se materializan como servicios de la Aplicación de UI invocados por todas las vistas.

| Componente | Capa | Responsabilidad | Entradas | Salidas | Dependencias | CU cubiertos |
| --- | --- | --- | --- | --- | --- | --- |
| Vista de ingreso y sesión | Presentación | Pantalla de inicio y cierre de sesión; captura de credenciales | Interacción del usuario | Intención de inicio/cierre de sesión | Aplicación de UI; servicio de sesión y token | CU-01 |
| Vista de administración de usuarios | Presentación | Listado, alta y baja de usuarios del alcance del rol | Interacción del usuario | Comandos de usuario y agente | Aplicación de UI; control de visibilidad por rol | CU-02 |
| Vista de relevamientos | Presentación | Crear, editar y listar relevamientos con su tramo | Interacción del usuario | Comandos y consultas de relevamiento | Aplicación de UI | CU-03 |
| Vista de asignaciones | Presentación | Asignar y reasignar agentes a un relevamiento | Interacción del usuario | Comandos de asignación | Aplicación de UI | CU-04 |
| Vista de mapa y marcadores | Presentación | Componente de mapa con marcadores; crear y mover marcadores iniciales | Interacción del usuario; coordenadas del mapa | Comandos de marcador; estado del mapa | Aplicación de UI; componente de mapa | CU-05, CU-06 |
| Componente de carrusel de fotos | Presentación | Carrusel encadenado de fotos por marcador (ampliar, comentar, etiquetar, filtrar) | Selección de marcador y foto | Estado del carrusel; comandos de comentario y etiqueta | Aplicación de UI | CU-06 |
| Vista de resolución de conflictos | Presentación | Presenta los conflictos pendientes y ofrece unificar o separar al cierre | Interacción del usuario | Comandos de resolución de conflicto | Aplicación de UI; control de habilitación por estado | CU-07 |
| Vista de ciclo del relevamiento | Presentación | Transición de estado y cierre, condicionado a conflictos resueltos | Interacción del usuario | Comandos de transición y cierre | Aplicación de UI; control de habilitación por estado | CU-08 |
| Vista de carga manual | Presentación | Carga manual completa de un relevamiento por el agente vía web | Interacción del usuario; archivos de foto | Comandos de carga manual | Aplicación de UI | CU-09 |
| Vista de portabilidad | Presentación | Exportar e importar un relevamiento completo en una unidad transferible única | Interacción del usuario; unidad transferible | Comandos de exportación e importación | Aplicación de UI | CU-10 |
| Vista de configuración de almacenamiento | Presentación | Configuración del destino de almacenamiento por el usuario raíz | Interacción del usuario | Comandos de configuración | Aplicación de UI; control de visibilidad por rol | CU-11 |
| Servicio de sesión y token | Aplicación de UI | Mantiene el token bearer del lado servidor del circuito, asociado a la sesión; inicia y cierra sesión | Credenciales; intención de cierre | Estado de sesión; token disponible para el Cliente de API | Cliente de API (autenticación); estado del circuito | CU-01 (transversal a todos) |
| Control de visibilidad y acciones por rol | Aplicación de UI | Presenta solo pantallas y acciones del alcance del rol (RN-01, RN-03) | Rol del portador; pantalla solicitada | Habilitar u ocultar pantallas y acciones | Estado de sesión | RN-01, RN-03 (transversal) |
| Control de habilitación por estado del relevamiento | Aplicación de UI | Habilita solo las acciones válidas para el estado vigente del relevamiento (RN-04, RN-05) | Estado del relevamiento; conflictos pendientes | Acciones habilitadas o bloqueadas | Cliente de API (consulta de estado) | RN-04, RN-05 (CU-03 a CU-09) |
| Mapeador de errores a feedback de UI | Aplicación de UI | Traduce el error problem+json de la API a un mensaje y un estado de pantalla comprensibles | Representación de error de la API | Feedback de UI (mensaje, estado de pantalla) | Cliente de API; catálogo de mensajes (03) | transversal a CU-01 a CU-11 |
| Orquestadores de interacción de cada CU | Aplicación de UI | Coordinan la interacción de cada CU; validan la entrada de pantalla y delegan en el Cliente de API | Comandos y consultas de pantalla | Resultados para la vista; feedback de error | Puerto de acceso al dominio; mapeador de errores | CU-01 a CU-11 |
| Cliente del contrato REST | Cliente de API | Traduce las intenciones en llamadas al contrato de `geovial-api` y normaliza respuestas y errores | Comandos y consultas validados; token | Representaciones de dominio; errores normalizados | Puerto de acceso al dominio; contrato REST de `geovial-api` | sostiene CU-01 a CU-11 |
| Adaptador del componente de mapa | Cliente de API / Presentación | Integra el componente de mapa de terceros (pines, mover pin, centrar) con el estado de la aplicación de UI | Coordenadas y eventos del mapa | Estado del mapa para la vista | Componente de mapa; Aplicación de UI | CU-05, CU-06 |

Cobertura de CU: los once casos de uso quedan cubiertos. CU-01 a CU-11 atraviesan una vista de Presentación, su orquestador de interacción en la Aplicación de UI y el Cliente de API que consume el recurso correspondiente de `geovial-api` (correspondencia de consumo en 02 §7). El control de visibilidad por rol, el control de habilitación por estado y el mapeador de errores son servicios transversales invocados por toda la superficie. No hay CU huérfano y ningún componente excede el alcance funcional de la especificación 02 ni redefine el dominio.

## 4. Vista de procesos

El front se ejecuta como un proceso de servidor que aloja, por cada usuario activo, un circuito interactivo persistente de larga vida; cada circuito porta el estado de UI y de sesión de un usuario. No hay proceso ni hilo de fondo planificado: toda actividad es dirigida por la interacción del usuario o por la respuesta del backend.

- Concurrencia. El proceso atiende múltiples circuitos concurrentes (objetivo de al menos cincuenta circuitos interactivos en el ambiente de referencia, §8). Cada circuito es la unidad de aislamiento del estado de un usuario: el estado de UI y el token no se comparten entre circuitos. Las llamadas al contrato REST son operaciones de entrada/salida asincrónicas y no bloqueantes, de modo que la espera de la respuesta del backend no bloquee el hilo que atiende otros circuitos.
- Manejo de estado en memoria. El estado vive en el circuito: la pantalla actual, el estado del mapa y del carrusel, los filtros, los resultados de consulta cacheados para la sesión y el token bearer. Es estado efímero: si el circuito se pierde (corte de conexión, reciclado del proceso), el front lo reconstruye consultando de nuevo a la API, que es la fuente de verdad. No hay transacciones de dominio en el front: la atomicidad de cualquier cambio la garantiza `geovial-api` dentro de su transacción.
- Sesión y token. El servicio de sesión retiene el token bearer del lado servidor del circuito y nunca lo entrega al navegador (ADR-03 de este proyecto). El cierre de sesión descarta el token y el estado del circuito asociado. La pérdida del circuito no implica pérdida de datos de dominio, solo de estado de UI reconstruible.
- Interacción de larga duración. Las acciones que producen una unidad transferible (exportación, CU-10) o que suben varias fotos (carga manual, CU-09) se ejecutan como operaciones asincrónicas que reportan progreso y resultado a la vista sin bloquear el circuito; el front delega el trabajo pesado en la API y solo refleja el avance.
- Tolerancia a conflictos. El front no resuelve conflictos de marcadores durante la recolección ni la revisión: los presenta como estado válido (RN-05) y solo habilita la resolución al cierre, consumiendo la operación de resolución de la API.

La orquestación del front no es compleja: cada interacción se traduce en una o pocas llamadas al contrato REST, sin coordinación de varios pasos ni transacciones distribuidas. Por eso no se produce `flujo-ejecucion_v1.0.md` (05 §2.2: el flujo solo aplica con orquestación compleja).

## 5. Vista de despliegue

`geovial-web` se despliega como una única unidad: un contenedor de front que aloja el proceso de servidor del front, sirve la interfaz por render server-side y mantiene los circuitos interactivos. Consume por red el contrato REST de `geovial-api` y no comparte proceso con el backend. La descripción se mantiene sin nombrar productos concretos; el runtime y la base del contenedor se fijan en la categoría 09 a partir del intake §17.P.9.

| Unidad | Naturaleza | Runtime objetivo | Dependencias de infraestructura |
| --- | --- | --- | --- |
| Contenedor de front (`geovial-web`) | Proceso de servidor que renderiza la UI y aloja los circuitos interactivos | Runtime del front dentro de un contenedor | Red entrante de los navegadores (conexión persistente del circuito); red saliente hacia el contrato REST de `geovial-api`; red saliente hacia el proveedor del componente de mapa |
| Navegador del usuario | Cliente de presentación; sostiene el extremo del circuito interactivo | Navegadores evergreen de escritorio y móvil (últimas dos versiones mayores de uso corriente, §17.P.9) | Conexión persistente con el contenedor de front |
| Contrato REST de `geovial-api` | Dependencia externa de dominio (no se despliega con el front) | Contenedor de backend separado (intake §16) | Red entre el contenedor de front y el de backend; el front no accede al almacén ni al almacenamiento de archivos directamente |
| Proveedor del componente de mapa | Servicio de teselas/render de mapa de terceros | Servicio externo de mapas | Red saliente desde el navegador o el front hacia el proveedor de mapas |

Notas de despliegue, sin detalle de proveedor concreto:

- El contenedor de front, el contenedor de backend y el contenedor de base son unidades separadas (intake §16); el front consume el backend por la red y no comparte proceso ni accede a la persistencia del dominio.
- El front no requiere almacenamiento persistente propio: su estado es efímero y reconstruible (§6); el reciclado del contenedor no pierde datos de dominio, solo circuitos activos, que se reconstruyen al reconectar.
- El render server-side con circuito persistente exige afinidad de sesión cuando hay más de una réplica del contenedor de front: las solicitudes de un mismo circuito deben volver a la misma réplica. La estrategia concreta de afinidad y de escalado de réplicas pertenece a la categoría 09.
- Los secretos (credenciales del front hacia el backend, parámetros del proveedor de mapas) se inyectan desde un gestor de secretos del entorno y nunca se incluyen en la imagen ni en el control de versiones (intake §17.P.5). El token bearer del usuario no es un secreto de despliegue: vive solo en memoria del circuito durante la sesión.

## 6. Vista de datos

`geovial-web` no tiene persistencia de dominio propia (intake §17.P.4, `tiene_persistencia=false`): el estado de dominio es de `geovial-api`, que es la fuente de verdad. El front solo maneja estado de UI y de sesión efímero, en memoria del circuito. Por esta razón no se produce `modelo-datos-logico_v1.0.md`: el modelo lógico autoritativo de las entidades (Usuario, Rol, Relevamiento, TramoVial, Asignacion, MarcadorGeografico, ConflictoMarcadores, Observacion, Foto, Comentario, Etiqueta) y de la configuración de almacenamiento vive en `geovial-api` (`modelo-datos-logico_v1.0.md` de ese proyecto). La omisión queda registrada como decisión en ADR-02 de este proyecto.

- Estado de dominio (autoritativo, no en el front). Toda lectura y escritura de entidades de dominio se realiza contra el contrato REST de `geovial-api`. El front presenta una vista de consumo del modelo conceptual (02 §5) y no posee invariantes de integridad propias: la integridad la garantizan las reglas conceptuales del modelo de `geovial-api`.
- Estado de UI y de sesión (efímero, en el front). El circuito retiene la pantalla actual, el estado del componente de mapa (pines, centro, zoom), el estado del carrusel (marcador y foto activos, filtros), los resultados de consulta cacheados para la sesión y el token bearer del lado servidor. Es estado volátil: se pierde al cerrar la sesión o al reciclar el circuito y se reconstruye consultando a la API.
- Caché. La única caché es la de sesión en memoria del circuito, de alcance acotado y vida igual a la del circuito; no hay caché persistente ni compartida entre circuitos. La caché se invalida al cambiar de pantalla o al recibir un error de versión o de estado del backend.
- Sin particionamiento ni multi-tenant en el front. La solución es single-tenant (intake §17.P.4 de geovial-api, `multi_tenant=false`); la jerarquía de cuatro roles es control de acceso resuelto por el backend, que el front solo refleja en la visibilidad de pantallas y acciones (RN-01, RN-03).
- Binarios de fotos. El front no almacena binarios: presenta las fotos por referencia desde la API y delega cualquier carga de archivos (carga manual, CU-09; importación, CU-10) al contrato REST, que a su vez delega en la librería de almacenamiento del backend.

## 7. Cross-cutting concerns

Decisiones transversales centralizadas para todo el front:

- Sesión y token. El servicio de sesión obtiene el token bearer del backend presentando credenciales (CU-01, ADR-03) y lo retiene del lado servidor del circuito, asociado a la sesión; nunca lo expone al navegador. El Cliente de API adjunta el token a cada llamada al contrato. El cierre de sesión descarta el token y el estado del circuito.
- Autorización en presentación. La autorización autoritativa la resuelve `geovial-api`; el front aplica un control de presentación que muestra solo pantallas y acciones del alcance del rol (RN-01) y restringe el acceso del front a los roles administradores, con la excepción de la carga manual del agente (RN-03). El front nunca asume que ocultar una acción equivale a autorizarla: toda operación se valida en el backend, y el front mapea un eventual rechazo a feedback.
- Manejo de errores. Todo error proviene del contrato REST de `geovial-api` como problem+json RFC 7807 con un código estable (CU-19 de la API). El mapeador de errores a feedback de UI (ADR-05 de este proyecto) traduce cada código a un mensaje y un estado de pantalla, alineado con el catálogo de mensajes de la sección 03; un error no contemplado se presenta como un fallo genérico sin filtrar detalles del backend.
- Habilitación por estado. El control de habilitación por estado del relevamiento presenta los estados visibles y habilita solo las acciones válidas para el estado vigente (RN-04), y no ofrece el cierre con conflictos pendientes (RN-05). La verdad del estado es del backend; el front la consulta y la refleja.
- Configuración y secretos. La configuración (dirección del contrato REST, parámetros del componente de mapa) se inyecta desde el entorno; los secretos del front hacia el backend viven en un gestor de secretos del entorno, nunca en el control de versiones (intake §17.P.5).
- Logging, tracing y métricas. El front emite registros estructurados con un identificador de correlación por interacción, propagable en las llamadas al contrato REST para enlazar la traza con el backend; ningún registro contiene credenciales ni el token. Se exponen puntos de medición de latencia de interacción del circuito, conteo de errores por código y número de circuitos concurrentes, suficientes para medir los NFR de §8. La observabilidad no es crítica en esta versión (intake §17.P.10, `tiene_observabilidad_critica=false`): no se fija un SLO de disponibilidad ≥ 99,9 % ni un objetivo de latencia p99 numérico.

## 8. Quality attributes (NFR)

Los objetivos numéricos provienen del intake §17 geovial-web P.10 (propuestos y ratificables). El mecanismo de medición se apoya en los puntos de medición de §7 y se materializa como pruebas de rendimiento, de componente y de integración en 08.

| NFR | Objetivo numérico | Mecanismo de medición | ADR relacionada |
| --- | --- | --- | --- |
| Latencia de interacción p95 | ≤ 200 ms sobre el circuito en red estable | Prueba de interacción sobre las pantallas clave (CU-03, CU-06, CU-08) midiendo el percentil 95 del tiempo entre la acción del usuario y la actualización de la vista, instrumentado por los puntos de medición de §7 (referencia a pruebas de rendimiento en 08); excluye la latencia atribuible al backend, medida aparte contra el NFR de `geovial-api` | ADR-01, ADR-05 |
| Circuitos concurrentes | ≥ 50 circuitos interactivos en el ambiente de referencia | Prueba de carga que abre y mantiene circuitos concurrentes verificando que la latencia de interacción p95 se sostiene y que ningún circuito pierde su estado de sesión (referencia a 08) | ADR-01, ADR-04 |
| Disponibilidad mensual | ≥ 99,5 % | Medición de disponibilidad del contenedor de front en el ambiente de referencia; el front depende además de la disponibilidad de `geovial-api`, medida contra el NFR homónimo de ese proyecto | ADR-01 |
| Custodia del token | 0 exposiciones del token bearer al navegador | Verificación de que el token no se serializa al cliente en ninguna pantalla, por inspección de la superficie de presentación y prueba de componente (referencia a 08) | ADR-03 |
| Cobertura de pruebas (gate de CI) | Líneas ≥ 80 %, branches ≥ 70 %, presentación ≥ 60 % | Gate de cobertura en CI sobre las capas Aplicación de UI y Cliente de API y la capa de Presentación (intake §17.P.6) | ADR-04, ADR-05 |

La trazabilidad NFR↔arquitectura↔ADR queda explícita en esta tabla (columna ADR relacionada) y se refuerza en la §10.

## 9. Riesgos arquitectónicos

| Riesgo | Impacto | Probabilidad | Mitigación |
| --- | --- | --- | --- |
| Pérdida del circuito interactivo por corte de red o reciclado del contenedor, con pérdida del estado de UI en curso | Medio | Media | El estado del circuito es efímero y reconstruible desde la API (fuente de verdad, §6); el front reconecta y reconstruye la pantalla; ninguna acción de dominio depende de retener estado de UI no confirmado en el backend |
| Acoplamiento al contrato REST de `geovial-api`: un cambio incompatible del contrato rompe el front | Alto | Media | El front fija la versión mayor del contrato que consume y se apoya en la política de versionado por URI de la API (CU-22, ADR-10 de geovial-api), que conserva la versión previa durante un período de convivencia; el Cliente de API centraliza el consumo del contrato en un único componente |
| Saturación de circuitos concurrentes por encima del objetivo de 50, degradando la latencia de interacción | Alto | Media | Prueba de carga del objetivo de §8; afinidad de sesión y escalado horizontal del contenedor de front en 09; el estado por circuito es de tamaño acotado y reconstruible |
| Fuga del token bearer al navegador por un descuido de implementación | Alto | Baja | El token se retiene del lado servidor del circuito por diseño (ADR-03); verificación explícita de no exposición como NFR (§8) y prueba de componente en 08 |
| Latencia de interacción degradada por la dependencia de la latencia del backend o del proveedor de mapas | Medio | Media | La medición de la latencia de interacción del front se separa de la del backend (§8); las llamadas al contrato son asincrónicas y no bloquean el circuito; el componente de mapa carga teselas de forma diferida |
| Inconsistencia entre la habilitación de acciones del front y la autorización real del backend | Medio | Baja | El front nunca autoriza por ocultamiento: toda operación se valida en el backend y un rechazo se mapea a feedback (RN-01, RN-03, §7); la verdad del estado y del alcance es del backend |

## 10. Trazabilidad

| CU (geovial-web) | RN aplicables | Recurso consumido de geovial-api (02 §7) | ADRs que lo gobiernan | Tests previstos (en 08) |
| --- | --- | --- | --- | --- |
| CU-01 Iniciar y cerrar sesión | RN-01, RN-03 | Autenticación y sesión (CU-03 API) | ADR-01, ADR-03, ADR-05 | Componente de login; no exposición del token; mapeo de credenciales inválidas a feedback |
| CU-02 Administrar usuarios por jerarquía | RN-01, RN-02 | Usuarios y agentes (CU-01, CU-02 API) | ADR-01, ADR-03, ADR-04 | Visibilidad por rol; baja conserva autoría visible; integración contra la API |
| CU-03 Crear, editar y listar relevamientos | RN-01, RN-04 | Relevamientos (CU-04 API) | ADR-01, ADR-04, ADR-05 | Habilitación por estado; latencia de interacción p95; snapshot de listado |
| CU-04 Asignar y reasignar agentes | RN-01, RN-04 | Asignaciones (CU-05 API) | ADR-01, ADR-04, ADR-05 | Habilitación por estado; mapeo de error de asignación duplicada |
| CU-05 Crear marcadores iniciales sobre el mapa | RN-01, RN-04 | Marcadores (CU-07 API) | ADR-01, ADR-04 | Componente de mapa: crear y mover pin; integración del marcador con la API |
| CU-06 Revisar sobre mapa con carrusel | RN-01, RN-02, RN-04 | Consulta de revisión (CU-12 API) | ADR-01, ADR-04 | Carrusel encadenado; estado de mapa; latencia de interacción p95 |
| CU-07 Resolver conflictos al cierre | RN-05, RN-01 | Resolución de conflictos (CU-13 API) | ADR-01, ADR-04, ADR-05 | Convivencia con conflictos; resolución unificar/separar; mapeo de conflictos pendientes |
| CU-08 Transicionar estado y cerrar | RN-05, RN-01 | Transición y cierre (CU-06, CU-14 API) | ADR-01, ADR-04, ADR-05 | Cierre bloqueado con conflictos pendientes; habilitación por estado |
| CU-09 Carga manual completa vía web | RN-04, RN-01 | Carga manual (CU-09 API) | ADR-01, ADR-03, ADR-05 | Acceso del agente al front (RN-03); carga asincrónica; mapeo de foto sin ubicación |
| CU-10 Exportar e importar relevamiento | RN-01 | Portabilidad (CU-15, CU-16 API) | ADR-01, ADR-04, ADR-05 | Exportación asincrónica; importación; mapeo de unidad malformada |
| CU-11 Configurar destino de almacenamiento | RN-01 | Configuración de almacenamiento (CU-17 API) | ADR-01, ADR-03 | Visibilidad solo para el usuario raíz; mapeo de proveedor no disponible |

NB upstream cubiertas (02 §1): NB-01, NB-02, NB-05, NB-06, NB-07. Las RN del front (RN-01 a RN-05) derivan de las RN del backend autoritativo y se vinculan a los componentes de §3 y a los cross-cutting de §7. Downstream: esta arquitectura ancla 06 (US US-01 a US-25 según 02 §6 y 03), 08 (tests de componente, snapshot, accesibilidad e integración a través de la API) y 09 (despliegue del contenedor de front, afinidad de sesión y escalado).

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Arquitectura de solución inicial de geovial-web: estilo de render server-side con circuito interactivo persistente y separación de capas en el cliente (Presentación / Aplicación de UI / Cliente de API), cuatro vistas mínimas (lógica, procesos, despliegue, datos con el dominio en geovial-api y estado de UI/sesión efímero), cross-cutting de sesión/token/errores, NFR con métricas numéricas (interacción p95 ≤ 200 ms, ≥ 50 circuitos concurrentes, disponibilidad 99,5 %), riesgos y trazabilidad CU/RN/recurso de API/ADR. |
