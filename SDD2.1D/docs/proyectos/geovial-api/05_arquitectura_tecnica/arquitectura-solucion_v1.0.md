# Arquitectura de solución — geovial-api

**Proyecto:** geovial-api
**Documento:** arquitectura-solucion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer

## 1. Objetivo

Este documento describe la arquitectura técnica interna de `geovial-api`, el backend monolítico y proyecto principal de la solución GeoVial, que expone la API REST consumida por el front web y la app móvil, concentra la lógica de negocio, la persistencia en un almacén relacional y la seguridad por token bearer. Está dirigido al equipo de desarrollo que construye y mantiene el backend, a los revisores funcionales que validan la cobertura de los 22 casos de uso (CU-01 a CU-22) y a las categorías downstream 06 (backlog técnico), 08 (testing e integración) y 09 (despliegue). Define el cómo estructural —estilo, vistas, cross-cutting, atributos de calidad y decisiones gobernantes— sin entrar en el detalle de cada operación, que vive en `contratos-rest_v1.0.md`, `modelo-datos-logico_v1.0.md`, `flujo-ejecucion_v1.0.md` y los ADRs bajo `adrs/`. La vista de solución y los contratos inter-proyecto de la solución viven en `_solucion/` (Fase H) y aquí solo se referencian.

## 2. Estilo arquitectónico

Se adopta una arquitectura en capas tipo Clean Architecture, con cuatro capas concéntricas de dependencia unidireccional hacia el dominio: Dominio (entidades e invariantes), Aplicación (casos de uso, orquestación de comandos y consultas, puertos hacia el exterior), Infraestructura (adaptadores de persistencia sobre el almacén relacional, integración con la abstracción de almacenamiento, emisión y validación de tokens) y API (la superficie REST que traduce solicitudes HTTP a invocaciones de la capa de Aplicación y respuestas a representaciones del contrato). La dependencia apunta siempre hacia adentro: la API depende de Aplicación; Aplicación depende de Dominio y declara puertos que Infraestructura implementa; Dominio no depende de nadie. Es un monolito de un único proceso desplegable, fijado por requisito (intake §17.P.2).

El modelo de dominio admite el conflicto de marcadores como estado válido durante la recolección y la revisión: el sistema convive con él, mantiene la información accesible y difiere la resolución a la unificación o separación al cierre del relevamiento (RN-03, ADR-06). Esta tolerancia es una propiedad de primera clase del dominio, no un caso de error.

Justificación contra alternativas descartadas:

| Criterio | Capas / Clean (elegido) | Microservicios | Servir la UI desde el mismo proceso |
| --- | --- | --- | --- |
| Requisito del intake (§17.P.2) | Cumple: monolito de backend con separación de capas | Descartado por el requisito de monolito | Descartado: el front es un proyecto cliente separado |
| Complejidad operativa (equipo de 1 dev) | Baja: un único artefacto desplegable | Alta: orquestación, mallas, transacciones distribuidas | Baja, pero acopla dos ciclos de vida y dos tipos D8 |
| Testeo del dominio sin infraestructura | Directo: dominio puro sin dependencias salientes | Posible pero con sobrecarga de contratos inter-servicio | Mezcla la lógica de presentación con la de negocio |
| Time to market (MVP) | Rápido | Lento | Medio, pero rompe la frontera cliente-servidor del manifiesto |
| Asimetría lectura/escritura | Capas con servicios de consulta separados de los de comando, sin CQRS pleno | Sobredimensionado | No resuelve el problema |

- Microservicios: descartado por el requisito explícito de backend monolítico (intake §14, §17.P.2) y por la complejidad operativa que no justifica un único dominio de negocio operado por un equipo de un desarrollador; introduciría transacciones distribuidas y consistencia eventual donde el dominio pide consistencia inmediata (jerarquía de usuarios, transición de estados, unicidad de asignación).
- Servir la UI desde el mismo proceso: descartado porque el front es un proyecto separado (`geovial-web`, tipo `web-monolith`) que consume el contrato REST; fusionarlo rompería la frontera cliente-servidor del manifiesto (intake §13, §14) y el versionado independiente del contrato (CU-22).
- Monolito sin separación de capas (todo en la capa de API): descartado porque impediría probar el dominio y los casos de uso sin levantar la infraestructura, y dispersaría las invariantes (RN-01 a RN-07) por la capa de transporte.

Vistas C4 incluidas (cuatro vistas mínimas): vista lógica (§3, nivel componentes C4), vista de procesos (§4), vista de despliegue (§5, nivel contenedores C4) y vista de datos (§6, con referencia al modelo lógico).

## 3. Vista lógica

El backend se descompone en componentes cohesivos agrupados por capa. La dependencia es unidireccional hacia el dominio: la API depende de Aplicación; Aplicación depende de Dominio y de puertos; Infraestructura implementa los puertos; Dominio no tiene dependencias salientes. Los componentes transversales (autorización, manejo de errores, paginación, idempotencia, versionado) se materializan como middleware de la capa de API o como servicios de la capa de Aplicación invocados por todos los casos de uso.

| Componente | Capa | Responsabilidad | Entradas | Salidas | Dependencias | CU cubiertos |
| --- | --- | --- | --- | --- | --- | --- |
| Superficie REST de usuarios y sesión | API | Expone los endpoints de usuarios, agentes y autenticación; traduce HTTP a comandos/consultas | Solicitudes HTTP con token bearer | Representaciones de usuario y token; problem+json | Aplicación; middleware transversal | CU-01, CU-02, CU-03 |
| Superficie REST de relevamientos y ciclo | API | Expone relevamientos, tramo, transiciones de estado y cierre | Solicitudes HTTP | Representaciones de relevamiento; problem+json | Aplicación; middleware transversal | CU-04, CU-06, CU-12, CU-14 |
| Superficie REST de asignaciones | API | Expone asignación y reasignación de agentes a relevamientos | Solicitudes HTTP | Representaciones de asignación; problem+json | Aplicación; middleware transversal | CU-05 |
| Superficie REST de marcadores y observaciones | API | Expone marcadores, observaciones, fotos, comentarios, etiquetas y carga manual | Solicitudes HTTP; binarios de fotos | Representaciones de marcador/observación; referencias de foto; problem+json | Aplicación; middleware transversal | CU-07, CU-08, CU-09 |
| Superficie REST de sincronización | API | Expone los endpoints de subida y bajada del ciclo de sincronización | Lote de cambios; marca de sincronización | Resultado de subida; conjunto de novedades y marca nueva; problem+json | Aplicación; middleware transversal | CU-10, CU-11 |
| Superficie REST de conflictos | API | Expone la consulta y la resolución de conflictos de marcadores al cierre | Solicitudes HTTP | Representaciones de conflicto y su resolución; problem+json | Aplicación; middleware transversal | CU-13 |
| Superficie REST de portabilidad | API | Expone la exportación e importación de un relevamiento completo en una unidad transferible única | Solicitud de exportación; unidad transferible importada | Unidad transferible; resultado de importación; problem+json | Aplicación; middleware transversal | CU-15, CU-16 |
| Superficie REST de configuración de almacenamiento | API | Expone la configuración del destino de almacenamiento por el usuario raíz | Selección de proveedor y parámetros | Confirmación de destino activo; problem+json | Aplicación; puerto de almacenamiento | CU-17 |
| Servicio de autorización por rol y alcance | API (middleware) / Aplicación | Resuelve el rol del solicitante y acota toda operación y listado al alcance jerárquico antes de ejecutar | Token validado; recurso solicitado | Permitir o rechazar (prohibido/no autorizado) | Dominio (jerarquía); puerto de identidad | CU-18 (transversal a todos) |
| Manejador de errores transversal | API (middleware) | Traduce todo fallo a una representación problem+json con código estable y estado acorde | Excepción o resultado de fallo | Respuesta problem+json RFC 7807 | Catálogo de códigos | CU-19 (transversal) |
| Servicio de paginación y filtros | Aplicación | Aplica alcance, filtros, orden y paginación a todo listado de recursos | Parámetros de listado | Página acotada con referencias de navegación | Dominio; repositorios | CU-20 (transversal) |
| Servicio de idempotencia | Aplicación | Reconoce claves de idempotencia e identificadores de origen y evita duplicar efectos de operaciones no seguras | Operación con clave/identificador | Resultado nuevo o resultado registrado | Repositorio de claves de idempotencia | CU-21 (transversal) |
| Servicio de versionado del contrato | API | Resuelve la versión mayor solicitada y aplica la política de compatibilidad y retiro | Versión indicada en la ruta | Recurso de la versión resuelta | Política de versionado | CU-22 (transversal) |
| Casos de uso de Aplicación | Aplicación | Orquesta comandos y consultas de cada CU; valida invariantes vía Dominio; coordina puertos | Comandos/consultas validados | Resultados de dominio; eventos internos | Dominio; puertos (repositorios, almacenamiento, identidad, idempotencia) | CU-01 a CU-17 |
| Modelo de dominio | Dominio | Entidades, agregados e invariantes de negocio (jerarquía, ciclo de relevamiento, identidad de marcador, conflictos, autoría) | Operaciones de dominio | Entidades y reglas aplicadas | Ninguna saliente | RN-01 a RN-07; RC-01 a RC-06 |
| Adaptador de persistencia relacional | Infraestructura | Implementa los repositorios sobre el almacén relacional; aplica restricciones e índices | Operaciones de repositorio | Filas persistidas y leídas | Puerto de repositorio; almacén relacional | sostiene CU-01 a CU-17 |
| Adaptador de almacenamiento de archivos | Infraestructura | Implementa el puerto de almacenamiento delegando en la abstracción de `geovial-storage` | Binarios y referencias de foto | Identificadores lógicos del almacén | Puerto de almacenamiento; abstracción de almacenamiento | CU-08, CU-09, CU-15, CU-16, CU-17 |
| Adaptador de identidad y token | Infraestructura | Emite y valida tokens bearer por credenciales; resuelve rol y alcance del portador | Credenciales; token | Token firmado; identidad validada | Puerto de identidad; resguardo de secretos | CU-03, CU-18 |

Cobertura de CU: los 22 casos de uso quedan cubiertos. CU-01 a CU-17 atraviesan una superficie REST y su caso de uso de Aplicación correspondiente; CU-18 a CU-22 son servicios transversales materializados como middleware o servicios de Aplicación invocados por toda la superficie. No hay CU huérfano y ningún componente excede el alcance funcional de la especificación 02.

## 4. Vista de procesos

El backend es un proceso único, multihilo, que atiende solicitudes HTTP concurrentes; no tiene proceso ni hilo de fondo de larga vida en esta versión (la sincronización es dirigida por la solicitud del cliente, no por un planificador).

- Concurrencia. Cada solicitud HTTP se atiende como una unidad de trabajo aislada (request-scoped). Los casos de uso de Aplicación son sin estado compartido mutable entre solicitudes; el estado vive en el almacén relacional. Las solicitudes concurrentes se ejecutan en paralelo sobre el pool de hilos del runtime.
- Transacciones y atomicidad. Cada comando que muta estado se ejecuta dentro de una transacción local del almacén relacional: o se aplica completo o no deja efectos parciales. Las invariantes que exigen consistencia inmediata (unicidad de asignación RC-05, integridad de la jerarquía RC-03, transición de estado RC-04/RN-05, referencia observación-marcador RC-02) se materializan con restricciones declarativas a nivel del almacén dentro de la misma transacción, de modo que la atomicidad no depende de la lógica de aplicación. No hay transacciones distribuidas.
- Ciclo de sincronización (orquestación de varios pasos). La subida (CU-10) y la bajada (CU-11) forman un pipeline de dos fases ordenadas: el backend incorpora primero el lote de cambios locales y solo después atiende la bajada (RN-06, ADR-07). La subida procesa el lote cambio por cambio, deduplicando por identificador de origen (RN-07) y registrando los conflictos de marcadores sin bloquear (RN-03). El detalle paso a paso de este pipeline vive en `flujo-ejecucion_v1.0.md`.
- Idempotencia bajo concurrencia. El servicio de idempotencia serializa el efecto de una operación no segura por clave: un reintento concurrente con la misma clave no inicia una segunda ejecución y devuelve el resultado en curso o el ya registrado (CU-21, ADR-08). La unicidad de la clave se garantiza con una restricción del almacén, no con bloqueos en memoria.
- Alojamiento de binarios. Las fotos se delegan a la abstracción de almacenamiento (ADR-09), modelada como operación asincrónica no bloqueante, de modo que la entrada/salida del proveedor remoto no consuma hilos de atención de solicitudes.
- Manejo de estado en memoria. El backend no cachea entidades de dominio entre solicitudes en esta versión; la configuración del proveedor de almacenamiento activo y la política de versiones vigentes son el único estado de proceso de tamaño acotado.

## 5. Vista de despliegue

`geovial-api` se despliega como una única unidad: un contenedor de backend que aloja el proceso del runtime con la API REST, la lógica y la librería de almacenamiento embebida. Es el contenedor sobre el que se apoyan los clientes web y móvil. La descripción se mantiene sin nombrar productos concretos; el runtime y la base del contenedor se fijan en la categoría 09 a partir del intake §17.P.9.

| Unidad | Naturaleza | Runtime objetivo | Dependencias de infraestructura |
| --- | --- | --- | --- |
| Contenedor de backend (`geovial-api`) | Proceso único que expone la API REST | Runtime del backend dentro de un contenedor | Red entrante de los clientes; red saliente hacia el almacén relacional y el proveedor de almacenamiento remoto |
| Librería de almacenamiento embebida (`geovial-storage`) | Componente en proceso dentro del contenedor de backend | El mismo runtime del backend | Destino del proveedor activo (almacenamiento local del contenedor o servicio de objetos remoto) |
| Almacén relacional | Servicio de persistencia | Contenedor o servicio de base dedicado | Almacenamiento persistente; conectividad con el contenedor de backend |
| Destino de almacenamiento local de fotos | Ubicación de archivos asociada al contenedor de backend | Sistema de archivos del contenedor o volumen montado | Almacenamiento persistente (volumen) para sobrevivir al reciclado |

Notas de despliegue, sin detalle de proveedor concreto:

- El contenedor de backend, el contenedor de base y el contenedor del front son unidades separadas (intake §16); el front consume el backend por la red, no comparte proceso.
- El almacenamiento local de fotos se monta sobre el contenedor de backend y debe ser persistente (volumen) para no perder la evidencia al reciclar el contenedor; la decisión y su verificación corresponden a la categoría 09 (heredada de la vista de despliegue de `geovial-storage`).
- Las migraciones del esquema relacional se aplican en un arranque controlado del despliegue mediante la herramienta de migraciones del runtime (ADR-02), antes de habilitar el tráfico.
- Los secretos (clave de firma de tokens, credenciales del proveedor de almacenamiento, cadena de conexión al almacén) se inyectan desde un gestor de secretos del entorno y nunca se incluyen en la imagen ni en el control de versiones (intake §17.P.5).

## 6. Vista de datos

El backend persiste el estado de dominio en un almacén relacional y delega los binarios de las fotos a la abstracción de almacenamiento; no mantiene caché de entidades en esta versión.

- Almacén relacional. Es la fuente de verdad de las 12 entidades del modelo conceptual (Usuario, Rol, Relevamiento, TramoVial, Asignacion, MarcadorGeografico, ConflictoMarcadores, Observacion, Foto, Comentario, Etiqueta, MarcaSincronizacion) y de las tablas técnicas de soporte (claves de idempotencia). El mapeo a tablas con tipos físicos, índices, restricciones y migración inicial vive en `modelo-datos-logico_v1.0.md`.
- Binarios de fotos. No se almacenan en el almacén relacional: la tabla de fotos guarda únicamente la referencia lógica (identificador opaco) que devuelve la abstracción de almacenamiento; el binario reside en el proveedor activo (local o de objetos remoto) gobernado por `geovial-storage` (ADR-09).
- Consistencia. Single-tenant para una única organización (intake §17.P.4, multi_tenant=false): la jerarquía de cuatro roles es control de acceso, no aislamiento por tenant. No hay columna discriminadora de tenant ni partición por organización.
- Marca de sincronización. La entidad MarcaSincronizacion sostiene el cálculo de novedades por relevamiento y cliente; su valor es opaco para el cliente y monótono (RC-06). Su persistencia habilita la bajada incremental (CU-11) sin recalcular todo el relevamiento.
- Caché y particionamiento. No hay caché de contenidos ni sharding en esta versión; la única organización lógica de los binarios es por prefijo de relevamiento delegado a la librería de almacenamiento.

## 7. Cross-cutting concerns

Decisiones transversales centralizadas para todo el backend:

- Autorización. Toda solicitud autenticada pasa por el servicio de autorización por rol y alcance (CU-18, ADR-03), que acota la operación al nivel jerárquico inmediato inferior y al ámbito del solicitante (RN-01, RC-03) antes de ejecutar cualquier efecto y antes de paginar cualquier listado (RN-01 sobre CU-20). La autoría histórica se conserva ante la baja (RN-02): inhabilitar un usuario no borra ni desatribuye sus registros.
- Manejo de errores. Todo fallo se devuelve como problem+json RFC 7807 (CU-19, ADR-05) con un código estable en mayúsculas sin tildes, un mensaje legible, el estado HTTP acorde y, cuando aporta, el campo o recurso implicado. Los códigos son opacos al idioma; el catálogo completo y su forma exacta viven en `contratos-rest_v1.0.md` y se alinean con `dx-error-messages_v1.0.md` (03). Un error interno no previsto devuelve un código genérico sin filtrar detalles.
- Idempotencia. Las operaciones no seguras reintentables aceptan una clave de idempotencia y la subida de sincronización porta identificadores de origen (CU-21, RN-07, ADR-08); el backend reconoce el reenvío y devuelve el resultado registrado sin duplicar efectos.
- Versionado del contrato. La API se expone bajo una versión mayor explícita en la ruta (CU-22, ADR-10); los cambios compatibles se incorporan dentro de la misma versión mayor y los incompatibles publican una versión nueva conservando la anterior durante un período de convivencia.
- Configuración y secretos. La configuración (cadena de conexión, proveedor de almacenamiento, parámetros de token) se inyecta desde el entorno; los secretos viven en un gestor de secretos del entorno, nunca en el control de versiones (intake §17.P.5). El backend es su propio emisor y validador de tokens; no hay proveedor de identidad externo.
- Logging, tracing y métricas. El backend emite registros estructurados con un identificador de correlación por solicitud, propagable a la librería de almacenamiento y a las trazas que cruzan a los clientes; ningún registro contiene credenciales ni binarios. Se exponen puntos de medición de latencia por operación, conteo de errores por código y volumen de cambios por ciclo de sincronización, suficientes para medir los NFR de §8. La observabilidad no es crítica en esta versión (intake §17.P.10, tiene_observabilidad_critica=false): no se fija un SLO de disponibilidad ≥ 99,9 % ni un objetivo de latencia p99 numérico.

## 8. Quality attributes (NFR)

Los objetivos numéricos provienen del intake §17.P.10 (propuestos y ratificables). El mecanismo de medición se apoya en los puntos de medición de §7 y se materializa como pruebas de rendimiento e integración en 08.

| NFR | Objetivo numérico | Mecanismo de medición | ADR relacionada |
| --- | --- | --- | --- |
| Latencia p95 de lecturas | ≤ 300 ms en ambiente equivalente al productivo | Prueba de carga sobre los endpoints de consulta y listado (CU-04, CU-12, CU-20) midiendo el percentil 95 desde la recepción de la solicitud hasta la respuesta, instrumentado por los puntos de medición de §7 (referencia a pruebas de rendimiento en 08) | ADR-01, ADR-02, ADR-04 |
| Latencia p95 de escrituras | ≤ 500 ms en ambiente equivalente al productivo | Prueba de carga sobre los endpoints de alta y mutación (CU-01, CU-04, CU-08) midiendo el percentil 95 de extremo a extremo, incluida la transacción del almacén y la verificación de idempotencia (referencia a 08) | ADR-02, ADR-08 |
| Disponibilidad mensual | ≥ 99,5 % | Cálculo del tiempo de servicio disponible sobre el total del mes a partir de las sondas de salud del contenedor de backend; sin SLO de 99,9 % (referencia a la categoría 09) | ADR-01 |
| Capacidad del lote de sincronización | ≥ 1000 cambios por relevamiento en una subida sin pérdida ni duplicación | Prueba de carga del endpoint de subida (CU-10) con un lote de al menos 1000 cambios, verificando que se aplican una sola vez y que los reenvíos se reconocen (RN-07, referencia a 08) | ADR-07, ADR-08 |
| Idempotencia de operaciones no seguras | 100 % de operaciones repetidas con la misma clave sin efecto duplicado | Pruebas de reintento por clave de idempotencia y por identificador de origen sobre altas, asignaciones, subidas e importaciones (CU-21, referencia a 08) | ADR-08 |
| Integridad de la jerarquía y el ciclo | 0 violaciones de jerarquía, transición de estado o unicidad de asignación bajo concurrencia | Pruebas de concurrencia que verifican que las restricciones del almacén impiden estados inválidos (RC-03, RC-04, RC-05, referencia a 08) | ADR-02, ADR-03 |
| Cobertura de pruebas (gate de CI) | Líneas ≥ 80 %; branches ≥ 70 %; aplicación ≥ 80 %; infraestructura ≥ 70 %; 100 % de endpoints públicos con contract test | Medición de cobertura en el pipeline como gate bloqueante (intake §17.P.6, §17.P.8) | ADR-01, ADR-05 |

## 9. Riesgos arquitectónicos

| Riesgo | Impacto | Probabilidad | Mitigación |
| --- | --- | --- | --- |
| Pérdida o duplicación de datos en la sincronización sin conexión (cortes, reenvíos) | Alto: comprometería la evidencia de campo, riesgo R-03 del negocio | Media | Orden subir-antes-de-bajar (ADR-07), idempotencia por identificador de origen y clave (ADR-08), reanudación sin reaplicar lo confirmado (CU-10 FA-01); pruebas de corte y reenvío en 08 |
| Estado de relevamiento inconsistente por transición inválida o cierre con conflictos pendientes | Alto: rompería la consistencia del informe de cierre | Baja | Restricción de estado y derivación a nivel del almacén (RC-04), rechazo con CONFLICTOS_PENDIENTES (RN-05); transacción atómica por transición (ADR-02) |
| Escalada de privilegios por fallo de autorización jerárquica | Alto: un rol operaría fuera de su alcance | Media | Autorización transversal previa a todo efecto y a la paginación (CU-18, ADR-03), integridad de la cadena de administración a nivel del almacén (RC-03); pruebas de acceso fuera de alcance en 08 |
| Filtración de credenciales del proveedor de almacenamiento o de la clave de firma de tokens | Alto: comprometería la evidencia y la sesión | Baja | Secretos en gestor del entorno fuera del control de versiones (intake §17.P.5), no filtración heredada del contrato de `geovial-storage` (RN-03 de storage); errores sin parámetros sensibles (ADR-05) |
| Incumplimiento del objetivo de latencia p95 por listados sin acotar o consultas sin índice | Medio: degradaría la experiencia de revisión | Media | Paginación obligatoria y filtros declarados por recurso (ADR-04), índices del modelo lógico para las consultas de revisión y sincronización; pruebas de rendimiento en 08 |
| Pérdida de evidencia del proveedor local al reciclar el contenedor de backend | Alto: se perderían las fotografías | Media | Requerir almacenamiento persistente (volumen) para el destino local; decisión y verificación en la categoría 09 (heredado de `geovial-storage` §5) |
| Rotura silenciosa de los clientes por un cambio incompatible del contrato | Medio: rompería `geovial-web` y `geovial-mobile` | Baja | Versionado por URI con convivencia de la versión previa (ADR-10), contract tests del 100 % de endpoints en CI (intake §17.P.6); política de compatibilidad declarada (CU-22) |

## 10. Trazabilidad

| Componente / decisión | CU upstream | RN / RC upstream | NFR | ADRs que lo gobiernan | Tests previstos (en 08) |
| --- | --- | --- | --- | --- | --- |
| Modelo de dominio y capas | CU-01 a CU-22 | RN-01 a RN-07; RC-01 a RC-06 | Cobertura | ADR-01 | Pruebas unitarias de dominio sin infraestructura; invariantes de jerarquía, estado y marcador |
| Persistencia relacional | CU-01 a CU-17 | RC-02, RC-03, RC-04, RC-05 | Latencia escrituras, integridad | ADR-02 | Integración contra base efímera; restricciones e índices; concurrencia |
| Autenticación y autorización | CU-03, CU-18 | RN-01, RN-02; RC-03 | Integridad jerarquía | ADR-03 | Token por credenciales; acceso fuera de alcance rechazado; autoría conservada en baja |
| Paginación y filtros | CU-20 | RN-01 | Latencia lecturas | ADR-04 | Página con navegación; filtro combinado; tamaño acotado; alcance antes de paginar |
| Manejo de errores | CU-19 | (uniformiza RN existentes) | Cobertura | ADR-05 | Error uniforme problem+json; múltiples campos; error interno sin filtración |
| Tolerancia a conflictos de marcadores | CU-07, CU-08, CU-10, CU-11, CU-12, CU-13, CU-14 | RN-03, RN-05; RC-01, RC-04 | — | ADR-06 | Conflicto convive sin bloquear; cierre rechazado con conflictos pendientes |
| Orden de sincronización | CU-10, CU-11 | RN-06; RC-06 | Capacidad de lote | ADR-07 | Subida antes de bajada; bajada sin subida rechazada con SUBIDA_NO_CONCLUIDA |
| Idempotencia | CU-10, CU-21 | RN-07; RC-05, RC-06 | Idempotencia, capacidad de lote | ADR-08 | Reenvío sin duplicar; clave reutilizada inconsistente rechazada |
| Integración con almacenamiento | CU-08, CU-09, CU-15, CU-16, CU-17 | RN-04 | — | ADR-09 | Referencia de foto persistida; transparencia del proveedor; configuración por raíz |
| Versionado del contrato | CU-22 | (política de compatibilidad) | — | ADR-10 | Cambio compatible no rompe; versión retirada rechazada; recurso ausente en versión |

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Arquitectura de solución inicial de geovial-api: estilo Clean Architecture en capas; cuatro vistas mínimas (lógica, procesos, despliegue, datos); cross-cutting de autorización, errores, idempotencia, versionado, secretos y observabilidad; tabla de NFR con los objetivos numéricos del intake §17.P.10 y su mecanismo de medición; riesgos arquitectónicos; y trazabilidad CU/RN/RC/NFR/ADR. |
