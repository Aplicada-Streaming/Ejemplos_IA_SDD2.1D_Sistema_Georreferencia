# Casos de prueba referenciales — geovial-api

**Proyecto:** geovial-api
**Documento:** casos-prueba-referenciales_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (variante API Testing Specialist)

## 1. Propósito y convenciones

Catálogo de casos de prueba referenciales TC-XX de `geovial-api`. Cada TC referencia al menos un CU, RN o NFR (08 §6) y declara tipo, setup, pasos en Given/When/Then, expected output, actual output y status. Incluye al menos un TC por CU crítico, un TC por RN, los TC de cada NFR numérico y el catálogo de contract tests por recurso/endpoint (TC-34, 100 % de los 35 endpoints).

Convenciones:

- Identificador `TC-XX` de dos dígitos, slug kebab descriptivo.
- Tipo: unit, integration, e2e, contract, performance, property-based.
- Status: Verde, Rojo, Pendiente, Deshabilitado (con motivo). A la fecha del documento la construcción no ha iniciado (07 §9), por lo que el actual output es "Sin ejecutar" y el status es Pendiente en todos los TC; ambos se actualizan al cierre de cada tramo.
- Los códigos de error son los estables del catálogo problem+json (`contratos-rest_v1.0.md` §5), en mayúsculas sin tildes, opacos al idioma.
- Cada TC parte de una base de datos efímera sembrada con los fixtures que su setup declara (estrategia-testing §5 y §7).

## 2. Casos de prueba

### TC-01 — alta-agente-respeta-jerarquia

- Tipo: Integration
- Cubre: CU-01, RN-01
- Setup: jefe de área autenticado con token válido; identificador de acceso del agente libre.
- Pasos: Given un jefe de área autenticado, When solicita el alta de un agente de campo de su área y, en una segunda solicitud, el alta de un usuario de dos niveles superiores, Then la primera crea el agente vinculado al jefe y la segunda se rechaza.
- Expected: alta de agente devuelve recurso creado con ubicación y rol AGENTE vinculado al jefe; el alta de salto de nivel devuelve problem+json con código JERARQUIA_NO_PERMITIDA (403).
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-02 — baja-usuario-conserva-autoria

- Tipo: Integration
- Cubre: CU-01, CU-02, RN-02
- Setup: agente habilitado de un área con observaciones cargadas y dos relevamientos en recolección.
- Pasos: Given un agente con autoría registrada, When el jefe de área lo da de baja, Then el acceso queda inhabilitado y la autoría y los registros se conservan; el agente no puede autenticarse después.
- Expected: el usuario pasa a INHABILITADO sin borrar; sus observaciones conservan autoría e identidad; un intento de autenticación posterior devuelve USUARIO_INHABILITADO (401); los relevamientos quedan disponibles para reasignación. Baja de agente de otra área → AGENTE_FUERA_DE_AREA (403).
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-03 — iniciar-sesion-emite-token

- Tipo: Integration
- Cubre: CU-03, RN-01
- Setup: usuario habilitado con credenciales conocidas.
- Pasos: Given credenciales válidas, When solicita iniciar sesión y, en otra solicitud, lo hace con credenciales incorrectas, Then la primera emite un token y la segunda se rechaza.
- Expected: inicio de sesión válido devuelve token bearer con rol y vigencia; credenciales inválidas → CREDENCIALES_INVALIDAS (401); usuario inhabilitado → USUARIO_INHABILITADO (401). El secreto de credencial nunca aparece en la respuesta.
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-04 — crear-relevamiento-tramo-no-vacio

- Tipo: Integration
- Cubre: CU-04, RN-05, RN-01
- Setup: jefe de área autenticado.
- Pasos: Given un jefe de área, When crea un relevamiento con un tramo de dos puentes y un camino y, en otra solicitud, uno sin puentes ni caminos, Then el primero se crea en estado recolección y el segundo se rechaza.
- Expected: relevamiento creado con estado RECOLECCION y ubicación del recurso; composición de tramo vacía → TRAMO_INCOMPLETO (400).
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-05 — asignar-agente-area-relevamiento

- Tipo: Integration
- Cubre: CU-05, RN-01, RN-05; RC-05
- Setup: jefe de área con un relevamiento abierto y dos agentes de su área.
- Pasos: Given un relevamiento en recolección, When asigna dos agentes del área y, en otra solicitud, intenta asignar sobre un relevamiento cerrado, Then las asignaciones se registran y la asignación sobre cerrado se rechaza.
- Expected: dos asignaciones vigentes únicas por par agente-relevamiento (RC-05); asignar a relevamiento cerrado → RELEVAMIENTO_CERRADO (409); asignar agente de otra área → AGENTE_FUERA_DE_AREA (403).
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-06 — transicionar-recoleccion-a-revision

- Tipo: Integration
- Cubre: CU-06, RN-05
- Setup: relevamiento en estado recolección del jefe.
- Pasos: Given un relevamiento en recolección, When solicita pasarlo a revisión y, en otra solicitud, intenta una transición no contemplada, Then la primera cambia el estado y la segunda se rechaza.
- Expected: estado pasa a REVISION con autor y momento registrados; transición no contemplada → TRANSICION_NO_PERMITIDA (409).
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-07 — crear-marcador-convive-conflicto

- Tipo: Integration
- Cubre: CU-07, RN-03, RN-04; RC-01, RC-02
- Setup: agente asignado a un relevamiento en recolección con un marcador existente con radio aplicable.
- Pasos: Given un marcador existente, When el agente crea otro marcador dentro del radio del primero y, en otra solicitud, intenta dar de baja un marcador con observaciones ancladas, Then el segundo marcador se crea y registra un conflicto sin bloquear, y la baja con observaciones se rechaza.
- Expected: segundo marcador creado con identidad propia y estable (RC-01); se registra ConflictoMarcadores en estado pendiente sin bloquear la operación (RN-03); baja de marcador con observaciones → MARCADOR_CON_OBSERVACIONES (RC-02).
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-08 — retornar-revision-a-recoleccion

- Tipo: Integration
- Cubre: CU-06, RN-05
- Setup: relevamiento en estado revisión del jefe.
- Pasos: Given un relevamiento en revisión que necesita más recolección, When el jefe lo devuelve a recolección, Then la transición inversa se aplica y la captura se reabre.
- Expected: estado vuelve a RECOLECCION registrando la transición; la captura de marcadores y observaciones queda habilitada nuevamente.
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-09 — crear-observacion-anclada-con-fotos

- Tipo: Integration
- Cubre: CU-08, RN-03, RN-04
- Setup: agente con acceso a un marcador existente; destino de almacenamiento local efímero disponible.
- Pasos: Given un marcador existente, When el agente crea una observación con nota y dos fotos con comentario y etiqueta y, en otra solicitud, ancla una observación a un marcador inexistente, Then la primera se ancla y aloja las fotos y la segunda se rechaza.
- Expected: observación anclada al marcador con autoría; fotos alojadas con referencia lógica al almacén (no el binario); marcador inexistente → MARCADOR_INEXISTENTE; foto no almacenable → la observación se conserva y se devuelve FOTO_NO_ALMACENABLE.
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-10 — carga-manual-agrupa-por-radio

- Tipo: Integration
- Cubre: CU-09, RN-04, RN-03
- Setup: agente en relevamiento abierto con radio de agrupación definido; tres fotos con ubicación incrustada dentro del radio.
- Pasos: Given un radio definido y tres fotos en el radio, When el agente realiza la carga manual y, en otra solicitud, intenta una carga sin radio aplicable, Then la primera agrupa las tres en un único marcador y la segunda se rechaza.
- Expected: las tres fotos se agrupan en un único marcador priorizando su ubicación incrustada; se reportan cero marcadores nuevos adicionales; carga sin radio → RADIO_NO_DEFINIDO (400).
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-11 — subir-lote-cambios-locales

- Tipo: Integration
- Cubre: CU-10, RN-06, RN-07, RN-03
- Setup: agente asignado a un relevamiento abierto; lote de cinco cambios locales nuevos con identificadores de origen.
- Pasos: Given un agente asignado y un lote de cinco cambios, When sube el lote y, en otra solicitud, sube cambios de un relevamiento al que no está asignado, Then el primero aplica los cinco y el segundo se rechaza sin aplicar nada.
- Expected: respuesta de subida con cinco aplicados y cero reenvíos; lote con marcador en el radio de otro incorpora el marcador y registra conflicto sin bloquear (RN-03); relevamiento no asignado → RELEVAMIENTO_NO_ASIGNADO (403); relevamiento cerrado → RELEVAMIENTO_CERRADO (409); lote malformado → LOTE_MALFORMADO (400).
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-12 — bajar-actualizaciones-tras-subida

- Tipo: Integration
- Cubre: CU-11, RN-06, RN-03
- Setup: agente que concluyó su subida; relevamiento con cuatro cambios posteriores a su última marca.
- Pasos: Given una subida concluida y cambios posteriores a la marca, When solicita la bajada y, en otra solicitud, la solicita sin haber concluido la subida del ciclo, Then la primera entrega las novedades y la segunda se rechaza.
- Expected: bajada entrega los cuatro cambios y una marca nueva opaca (RC-06); sin subida previa concluida → SUBIDA_NO_CONCLUIDA (409); relevamiento sin cambios desde la marca → conjunto vacío y marca equivalente; marca inválida → MARCA_INVALIDA (400).
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-13 — consultar-relevamiento-para-revision

- Tipo: Contract
- Cubre: CU-12, RN-01, RN-03
- Setup: jefe de área con un relevamiento de diez marcadores, alguno con par en conflicto.
- Pasos: Given un relevamiento del alcance del jefe, When consulta su detalle para revisión y, en otra solicitud, consulta un relevamiento de otro jefe, Then la primera devuelve los marcadores con sus fotos y conflictos señalados y la segunda se rechaza.
- Expected: respuesta con los diez marcadores, sus fotos en orden encadenado a los contiguos y los conflictos señalados sin ocultarlos (RN-03); fuera de ámbito → RELEVAMIENTO_FUERA_DE_AMBITO (403); el payload valida contra el esquema OpenAPI del recurso.
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-14 — resolver-conflicto-unificar-marcadores

- Tipo: Integration
- Cubre: CU-13, RN-03, RN-05
- Setup: relevamiento en revisión con dos marcadores en conflicto, con etiquetas distintas y observaciones cada uno.
- Pasos: Given un relevamiento en revisión con un conflicto, When el jefe unifica los dos marcadores y, en otra solicitud, intenta resolver un conflicto con el relevamiento en recolección, Then la primera reasigna las observaciones y la segunda se rechaza.
- Expected: las observaciones se reasignan al marcador resultante conservando fotos y la unión de etiquetas de ambos; el conflicto pasa a resuelto; resolver fuera de revisión → RELEVAMIENTO_NO_EN_REVISION (409).
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-15 — cerrar-relevamiento-exige-conflictos-resueltos

- Tipo: Integration
- Cubre: CU-14, RN-05, RN-03
- Setup: dos relevamientos en revisión, uno sin conflictos pendientes y otro con un conflicto pendiente.
- Pasos: Given un relevamiento en revisión sin conflictos, When el jefe lo cierra y, en otra solicitud, intenta cerrar uno con conflicto pendiente, Then el primero transiciona a cierre y el segundo se rechaza.
- Expected: cierre exitoso con momento y autor registrados; cierre con conflictos pendientes → CONFLICTOS_PENDIENTES (409); cerrar un relevamiento que no está en revisión → RELEVAMIENTO_NO_EN_REVISION (409).
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-16 — exportar-relevamiento-unidad-completa

- Tipo: Contract
- Cubre: CU-15, RN-01
- Setup: jefe con un relevamiento cerrado de varios marcadores y fotos, alguna foto en proveedor remoto. (Could Have; sujeto a cadencia del Tramo 4.)
- Pasos: Given un relevamiento cerrado del alcance, When el jefe lo exporta y, en otra solicitud, exporta uno con una foto no recuperable del almacén, Then el primero produce la unidad transferible completa y el segundo se detiene.
- Expected: unidad transferible con el 100 % de comentarios, etiquetas y fotos, recuperando transparentemente del proveedor remoto; foto no recuperable → FOTO_NO_RECUPERABLE sin entregar una unidad incompleta; fuera de ámbito → RELEVAMIENTO_FUERA_DE_AMBITO (403).
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-17 — sincronizacion-reanuda-sin-duplicar

- Tipo: Integration
- Cubre: CU-10, RN-07, RN-06
- Setup: agente con un lote en curso; se simula un corte tras aplicar parte del lote.
- Pasos: Given una subida interrumpida con parte del lote aplicada, When el agente reenvía el lote completo, Then los cambios ya aplicados se reconocen y no se duplican y los pendientes se aplican.
- Expected: el reenvío reconoce por identificador de origen los cambios ya aplicados (reenvíos reconocidos) y aplica solo los restantes; el estado final es equivalente a una única aplicación del lote, sin pérdida ni duplicación.
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-18 — sincronizacion-reenvio-lote-idempotente

- Tipo: Integration
- Cubre: CU-10, CU-21, RN-07
- Setup: agente con un lote ya aplicado completo.
- Pasos: Given un lote ya aplicado, When el agente lo reenvía íntegro, Then ningún cambio se aplica una segunda vez.
- Expected: respuesta con cero aplicados y todos reconocidos como reenvíos; el estado del relevamiento no cambia.
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-19 — bajada-rechazada-sin-subida-concluida

- Tipo: Integration
- Cubre: CU-11, RN-06
- Setup: agente con cambios locales pendientes de subir en el ciclo.
- Pasos: Given un ciclo con subida no concluida, When el agente solicita la bajada, Then se rechaza.
- Expected: bajada → SUBIDA_NO_CONCLUIDA (409); ninguna actualización se entrega antes de concluir la subida (orden subir-antes-de-bajar).
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-20 — foto-sin-ubicacion-incrustada-pendiente

- Tipo: Integration
- Cubre: CU-09, RN-04
- Setup: lote de carga manual con una foto sin datos de ubicación incrustados.
- Pasos: Given una foto sin ubicación incrustada, When el agente la carga manualmente, Then queda pendiente de ubicación manual sin inventar coordenada.
- Expected: la foto se registra como pendiente de ubicación manual; no se le asigna una coordenada inventada; se reporta como sin ubicación resuelta.
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-21 — latencia-p95-lecturas

- Tipo: Performance
- Cubre: NFR latencia p95 lecturas (≤ 300 ms); CU-04, CU-12, CU-20
- Setup: ambiente equivalente al productivo; base sembrada con volumen representativo; cliente de carga.
- Pasos: Given una carga sostenida de solicitudes de consulta y listado, When se ejecutan los endpoints de lectura (consultar relevamiento, listar relevamientos, listar marcadores), Then el percentil 95 de latencia se mantiene en el objetivo.
- Expected: p95 ≤ 300 ms medido de extremo a extremo desde la recepción hasta la respuesta, con paginación y alcance aplicados.
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-22 — latencia-p95-escrituras

- Tipo: Performance
- Cubre: NFR latencia p95 escrituras (≤ 500 ms); CU-01, CU-04, CU-08
- Setup: ambiente equivalente al productivo; cliente de carga.
- Pasos: Given una carga sostenida de altas y mutaciones, When se ejecutan los endpoints de escritura (crear usuario/agente, crear relevamiento, crear observación con foto), Then el percentil 95 de latencia se mantiene en el objetivo.
- Expected: p95 ≤ 500 ms de extremo a extremo, incluida la transacción del almacén y la verificación de idempotencia.
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-23 — paginar-y-filtrar-listados

- Tipo: Integration
- Cubre: CU-20, RN-01
- Setup: jefe con 30 relevamientos en distintos estados.
- Pasos: Given 30 relevamientos, When solicita la primera página de tamaño 10 filtrando por estado y, en otra solicitud, indica un filtro inexistente, Then la primera devuelve 10 con navegación y la segunda se rechaza.
- Expected: página con 10 elementos y referencia a página siguiente; filtro inexistente → FILTRO_NO_SOPORTADO (400) informando los válidos; orden no soportado → ORDEN_NO_SOPORTADO; tamaño sobre el máximo → se acota al máximo y se informa.
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-24 — alcance-antes-de-paginar

- Tipo: Integration
- Cubre: CU-20, CU-18, RN-01
- Setup: dos jefes de área, cada uno con sus propios relevamientos.
- Pasos: Given un jefe de área que lista relevamientos, When solicita el listado, Then solo recibe los de su alcance jerárquico, acotados antes de paginar.
- Expected: el listado contiene únicamente los relevamientos del ámbito del solicitante; ningún relevamiento de otro jefe aparece en ninguna página (el alcance se aplica antes de la paginación, no después).
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-25 — autorizar-acceso-por-rol-y-token

- Tipo: Integration
- Cubre: CU-18, RN-01, RN-02
- Setup: un agente de campo con token válido; una solicitud sin token.
- Pasos: Given una solicitud sin token a un recurso protegido y un agente que intenta una acción de nivel superior, When se envían, Then la primera se rechaza por no autenticada y la segunda por acción no permitida.
- Expected: solicitud sin token → NO_AUTENTICADO (401) sin ejecutar efecto; agente que intenta dar de alta a otro usuario → ACCION_NO_PERMITIDA (403); la autorización ocurre antes de cualquier efecto.
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-26 — acceso-fuera-de-alcance-entre-pares

- Tipo: Integration
- Cubre: CU-18, RN-01
- Setup: dos jefes de área del mismo nivel, cada uno con sus agentes y relevamientos.
- Pasos: Given un jefe de área, When intenta operar sobre un recurso de otro jefe del mismo nivel, Then se rechaza por fuera de alcance.
- Expected: operación sobre recurso fuera del ámbito → FUERA_DE_ALCANCE (403); no hay fuga de información del recurso ajeno.
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-27 — errores-formato-problem-json-uniforme

- Tipo: Integration
- Cubre: CU-19
- Setup: solicitud de creación con varios campos inválidos; condición que dispara un fallo interno simulado.
- Pasos: Given una solicitud con varios campos inválidos y una condición de fallo interno, When se envían, Then la primera devuelve un único problema que enumera los campos y la segunda un problema genérico sin detalles.
- Expected: un único problem+json RFC 7807 con código estable que enumera cada campo y su motivo (FORMATO_SOLICITUD_INVALIDO); fallo interno → ERROR_INTERNO (500) sin exponer detalles internos; todo error porta código en mayúsculas sin tildes, opaco al idioma.
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-28 — configurar-destino-almacenamiento-por-raiz

- Tipo: Integration
- Cubre: CU-17, RN-01
- Setup: usuario raíz autenticado; un proveedor de almacenamiento de prueba válido. (Could Have; sujeto a cadencia.)
- Pasos: Given un usuario raíz, When establece un proveedor como destino activo y, en otra solicitud, un jefe de área intenta cambiar el destino, Then el primero activa el destino y el segundo se rechaza.
- Expected: destino activado sin exponer credenciales en la respuesta; consulta del destino activo no revela el secreto; rol jefe de área → ROL_NO_AUTORIZADO (403); proveedor no disponible → PROVEEDOR_NO_DISPONIBLE (409); credenciales inválidas → CREDENCIALES_PROVEEDOR_INVALIDAS (400).
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-29 — idempotencia-por-clave-en-alta

- Tipo: Integration
- Cubre: CU-21, RN-07; NFR idempotencia
- Setup: jefe de área; clave de idempotencia nueva.
- Pasos: Given un alta de agente con una clave de idempotencia, When el cliente la envía y la reintenta con la misma clave y, en otra solicitud, reutiliza la clave con contenido distinto, Then el reintento no duplica y la reutilización inconsistente se rechaza.
- Expected: el agente se crea una sola vez; el reintento con la misma clave devuelve el mismo agente sin duplicar; clave reutilizada con contenido distinto → CLAVE_REUTILIZADA_INCONSISTENTE (409); operación no segura sin clave cuando se exige → CLAVE_REQUERIDA_AUSENTE (400).
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-30 — idempotencia-por-identificador-de-origen

- Tipo: Integration
- Cubre: CU-21, CU-10, RN-07; NFR idempotencia
- Setup: lote de sincronización cuyos cambios portan identificadores de origen.
- Pasos: Given un lote con identificadores de origen, When se sube y se reenvía el mismo lote, Then los cambios se aplican una sola vez por identificador.
- Expected: 100 % de los cambios repetidos con el mismo identificador de origen sin efecto duplicado; el reenvío se reconoce sin reaplicar.
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-31 — capacidad-lote-sincronizacion-1000

- Tipo: Performance
- Cubre: NFR capacidad de lote (≥ 1000); CU-10, RN-07
- Setup: ambiente equivalente al productivo; dataset de un lote de al menos 1000 cambios generado por semilla fija.
- Pasos: Given un lote de al menos 1000 cambios, When el agente lo sube, Then se aplica completo sin pérdida ni duplicación.
- Expected: los ≥ 1000 cambios se aplican una sola vez; un reenvío del lote completo se reconoce sin duplicar; ningún cambio se pierde; el estado final es consistente.
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-32 — importar-relevamiento-idempotente

- Tipo: Integration
- Cubre: CU-16, RN-07, RN-01
- Setup: unidad transferible válida exportada por el sistema. (Could Have; sujeto a cadencia.)
- Pasos: Given una unidad transferible válida, When un jefe la importa y, en otra solicitud, la reimporta y, en una tercera, importa una unidad corrupta, Then la primera reconstruye la estructura, la segunda no duplica y la tercera se rechaza.
- Expected: importación reconstruye el 100 % de la estructura con ubicación del recurso; reimport de la misma unidad → idempotente, sin duplicar el relevamiento; unidad corrupta o ajena → UNIDAD_INVALIDA; unidad con fotos parcialmente alojadas → reconstruye lo alojable y reporta lo no alojado.
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-33 — concurrencia-jerarquia-y-unicidad-asignacion

- Tipo: Integration
- Cubre: NFR integridad de jerarquía y ciclo; RC-03, RC-04, RC-05
- Setup: base efímera con las restricciones del almacén aplicadas; ejecución concurrente de operaciones que compiten.
- Pasos: Given operaciones concurrentes que intentan violar la jerarquía, la transición de estado o la unicidad de asignación, When se ejecutan en paralelo, Then las restricciones del almacén impiden todo estado inválido.
- Expected: cero violaciones de jerarquía (RC-03), de transición de estado (RC-04) o de unicidad de asignación agente-relevamiento (RC-05) bajo concurrencia; las operaciones perdedoras se rechazan de forma atómica sin dejar efectos parciales.
- Actual: Sin ejecutar.
- Status: Pendiente.

### TC-34 — contract-tests-cobertura-total-endpoints

- Tipo: Contract
- Cubre: NFR cobertura (100 % de endpoints con contract test); CU-01 a CU-22
- Setup: especificación OpenAPI versionada materializada (BT-18); cliente HTTP de pruebas; base efímera; framework de validación de contrato.
- Pasos: Given el contrato OpenAPI de la versión `/v1`, When se ejercita cada uno de los 35 endpoints públicos con solicitudes válidas e inválidas, Then cada respuesta valida contra el esquema y los códigos de error declarados.
- Expected: los 35 endpoints (ver subcasos) responden con el esquema y los códigos problem+json del contrato; el fuzz de contrato sobre el esquema no produce respuestas fuera de contrato; ningún endpoint queda sin contract test (100 % de cobertura de endpoints).
- Actual: Sin ejecutar.
- Status: Pendiente.

Subcasos por recurso/endpoint (un contract test por recurso, cubriendo sus operaciones):

| Subcaso | Recurso / endpoints | CU | Códigos de error ejercitados |
| --- | --- | --- | --- |
| TC-34.1 | Sesión: POST /v1/sesiones; DELETE /v1/sesiones/actual; POST /v1/sesiones/revalidacion | CU-03 | CREDENCIALES_INVALIDAS, USUARIO_INHABILITADO |
| TC-34.2 | Usuarios y agentes: GET/POST/GET{id}/DELETE{id} usuarios; POST/DELETE agentes | CU-01, CU-02 | JERARQUIA_NO_PERMITIDA, ROL_NO_AUTORIZADO, FUERA_DE_ALCANCE |
| TC-34.3 | Relevamientos y ciclo: GET/POST/GET{id}/DELETE{id}; transiciones; cierre | CU-04, CU-06, CU-12, CU-14 | TRAMO_INCOMPLETO, TRANSICION_NO_PERMITIDA, CONFLICTOS_PENDIENTES, RELEVAMIENTO_NO_EN_REVISION |
| TC-34.4 | Asignaciones: GET/POST; DELETE {agenteId} | CU-05 | RELEVAMIENTO_CERRADO, AGENTE_FUERA_DE_AREA |
| TC-34.5 | Marcadores/observaciones/carga: GET/POST/PATCH/DELETE marcadores; POST observaciones; POST fotos; PATCH fotos; POST carga-manual | CU-07, CU-08, CU-09 | RADIO_NO_DEFINIDO, MARCADOR_INEXISTENTE, RELEVAMIENTO_CERRADO |
| TC-34.6 | Sincronización: POST subida; POST bajada | CU-10, CU-11 | RELEVAMIENTO_NO_ASIGNADO, SUBIDA_NO_CONCLUIDA, LOTE_MALFORMADO, MARCA_INVALIDA |
| TC-34.7 | Conflictos: GET; POST resolución | CU-13 | RELEVAMIENTO_NO_EN_REVISION, ROL_NO_AUTORIZADO |
| TC-34.8 | Portabilidad: POST exportación; POST importación | CU-15, CU-16 | FOTO_NO_RECUPERABLE, UNIDAD_INVALIDA |
| TC-34.9 | Configuración almacenamiento: GET/PUT; POST validación | CU-17 | ROL_NO_AUTORIZADO, PROVEEDOR_NO_DISPONIBLE, CREDENCIALES_PROVEEDOR_INVALIDAS |

### TC-35 — versionado-contrato-compatible-e-incompatible

- Tipo: Contract
- Cubre: CU-22
- Setup: dos revisiones del contrato bajo el prefijo `/v1`; un cliente que consume la versión vigente.
- Pasos: Given un cliente que consume la versión mayor vigente, When el backend agrega un campo opcional y, en otra solicitud, el cliente pide una versión retirada, Then el cliente sigue funcionando y la versión retirada se rechaza.
- Expected: agregar un campo opcional dentro de `/v1` no rompe al cliente (cambio compatible); versión retirada o inexistente → VERSION_NO_SOPORTADA; versión requerida ausente → VERSION_REQUERIDA_AUSENTE; recurso ausente en la versión indicada → RECURSO_NO_EN_VERSION; un cambio incompatible publica una versión mayor nueva conservando la anterior durante la convivencia.
- Actual: Sin ejecutar.
- Status: Pendiente.

## 3. Resumen de cobertura del catálogo

- CU críticos cubiertos: los 22 CU tienen al menos un TC (ver `matriz-cobertura-pruebas_v1.0.md` §2).
- RN cubiertas: las 7 RN tienen al menos un TC (matriz §4).
- NFR numéricos cubiertos: latencia p95 lecturas (TC-21), escrituras (TC-22), capacidad de lote (TC-31), idempotencia (TC-29, TC-30), integridad bajo concurrencia (TC-33), cobertura/contract total (TC-34); disponibilidad por monitoreo en 09.
- Contract tests: TC-34 cubre el 100 % de los 35 endpoints públicos por versión.

## 4. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Catálogo inicial de 35 casos de prueba referenciales de geovial-api (TC-01 a TC-35), con al menos un TC por CU crítico, un TC por RN, un TC por NFR numérico y el contract test total (TC-34) que cubre el 100 % de los 35 endpoints públicos por recurso. Cada TC declara tipo, setup, pasos en Given/When/Then, expected con códigos problem+json estables, actual y status. |
