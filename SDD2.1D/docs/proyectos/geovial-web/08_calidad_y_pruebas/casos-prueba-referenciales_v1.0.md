# Casos de prueba referenciales — geovial-web

Proyecto: geovial-web
Documento: casos-prueba-referenciales_v1.0.md
Versión: 1.0
Estado: Propuesto
Fecha: 2026-06-15
Autor: Ingeniero QA / SDET (web-monolith)

## 1. Propósito y convenciones

Catálogo de casos de prueba referenciales (TC-XX) de `geovial-web`. Cada TC referencia al menos un CU, una RN o un NFR de origen, declara su tipo (unitario, integración, componente de UI, snapshot, e2e, rendimiento), su setup, sus pasos en forma Given/When/Then, su expected output, su actual output y su status. Hay al menos un TC por cada CU crítico y por cada RN; se incluyen TC de componente de UI y de snapshot de vistas clave y un TC del NFR de interacción. Los identificadores son contiguos de dos dígitos (TC-01 a TC-22).

Status inicial: todos los TC están en estado Pendiente porque el front aún no se implementó (mini-plan de 07 con todos los tramos pendientes de inicio). El campo Actual output registra "no ejecutado" hasta la primera corrida; la matriz de cobertura se actualiza al cierre de cada tramo.

## 2. Catálogo de casos de prueba

### TC-01-ingreso-credenciales-validas-habilita-rol
- Tipo: integración (a través de la API contra base efímera)
- Cubre: CU-01 (CA-01), RN-03
- Setup: un jefe de área habilitado con identificador "jarea.norte" y credencial válida sembrado en la base efímera de geovial-api.
- Pasos: Given un jefe de área habilitado; When ingresa sus credenciales en el front web; Then el front abre una sesión con rol jefe de área, muestra las pantallas de relevamientos y agentes y conserva el token del lado servidor.
- Expected output: sesión activa con rol jefe de área; pantallas del alcance del rol habilitadas.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-02-credenciales-invalidas-no-abre-sesion
- Tipo: unitario (mapeo de error) + integración
- Cubre: CU-01 (CA-02, CA-04), RN-03
- Setup: doble del Cliente de API que devuelve problem+json con CREDENCIALES_INVALIDAS y, en una variante, USUARIO_INHABILITADO.
- Pasos: Given un usuario con credencial incorrecta o dado de baja; When ingresa al front web; Then el front informa el código correspondiente, permanece en la pantalla de ingreso y no abre sesión.
- Expected output: feedback de credenciales inválidas o acceso revocado; sin sesión.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-03-administracion-usuarios-acotada-por-rol
- Tipo: unitario (control de visibilidad por rol) + integración
- Cubre: CU-02 (CA-01, CA-02, CA-03), RN-01
- Setup: un jefe general y un jefe de área con sus universos de usuarios sembrados.
- Pasos: Given un jefe de área que ve solo sus propios agentes; When abre la administración de usuarios y luego intenta crear un rol que no es su nivel inmediato inferior; Then el front lista solo su ámbito, no muestra agentes de otros jefes ni jefes generales, y no ofrece el rol no inmediato (JERARQUIA_NO_PERMITIDA).
- Expected output: listado y formulario acotados al alcance del rol; rol no inmediato no ofrecido.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-04-baja-conserva-autoria-visible
- Tipo: integración (a través de la API)
- Cubre: CU-02 (CA-04), RN-02
- Setup: un agente "agente.lopez" con observaciones ya cargadas, asignado a un relevamiento del jefe.
- Pasos: Given un jefe de área que da de baja a "agente.lopez" con observaciones cargadas; When confirma la baja; Then el front lo muestra como dado de baja, inhabilitado para acceder, y las observaciones cargadas conservan su autoría visible.
- Expected output: usuario dado de baja; evidencia con autoría conservada, no huérfana.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-05-crear-relevamiento-recoleccion-y-tramo-vacio
- Tipo: integración + unitario (validación de formulario)
- Cubre: CU-03 (CA-01, CA-02), RN-04
- Setup: un jefe de área con sesión activa.
- Pasos: Given un jefe de área; When crea un relevamiento "Tramo Norte" con dos puentes y un camino, y en otra corrida intenta crear uno sin ningún puente ni camino; Then el primero queda en estado de recolección y visible en el listado, y el segundo se bloquea con TRAMO_VACIO manteniendo el formulario.
- Expected output: relevamiento creado en recolección; envío bloqueado ante tramo vacío.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-06-edicion-solo-en-recoleccion
- Tipo: unitario (control de habilitación por estado) + integración
- Cubre: CU-03 (CA-03, CA-04), RN-04
- Setup: un relevamiento "Tramo Norte" en revisión y un conjunto de relevamientos en distintos estados.
- Pasos: Given un relevamiento ya en revisión; When el jefe lo abre para editar la composición del tramo y luego filtra el listado por estado "recolección"; Then el front presenta la edición en solo lectura (RELEVAMIENTO_NO_EN_RECOLECCION) y el filtro muestra solo los relevamientos en recolección de ese jefe.
- Expected output: edición deshabilitada fuera de recolección; filtro por estado correcto.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-07-asignar-y-rechazar-duplicado
- Tipo: integración + unitario
- Cubre: CU-04 (CA-01, CA-02, CA-04), RN-01, RN-04
- Setup: un relevamiento "Tramo Norte" en recolección con dos agentes disponibles del jefe; en variante, un relevamiento cerrado.
- Pasos: Given un relevamiento en recolección y dos agentes disponibles; When el jefe asigna a ambos, luego intenta asignar dos veces al mismo agente y, en otra corrida, intenta asignar sobre un relevamiento cerrado; Then el front muestra a ambos asignados, mantiene una sola asignación ante el duplicado (ASIGNACION_DUPLICADA) y presenta la sección en solo lectura sobre el relevamiento cerrado (RELEVAMIENTO_CERRADO).
- Expected output: asignaciones correctas; sin duplicado; solo lectura al cierre.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-08-reasignacion-conserva-recolectado
- Tipo: integración (a través de la API)
- Cubre: CU-04 (CA-03), RN-04
- Setup: un relevamiento con "agente.lopez" asignado que ya cargó observaciones y un agente "agente.gomez" disponible.
- Pasos: Given un relevamiento con "agente.lopez" y observaciones cargadas; When el jefe lo reemplaza por "agente.gomez"; Then el front muestra a gomez asignado, a lopez sin asignación y las observaciones de lopez conservadas.
- Expected output: reasignación aplicada sin pérdida de lo recolectado.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-09-marcador-mapa-crear-mover-y-conflicto
- Tipo: componente de UI (adaptador de mapa) + integración
- Cubre: CU-05 (CA-01, CA-02, CA-03, CA-04), RN-01, RN-04
- Setup: un relevamiento "Tramo Norte" en recolección abierto sobre el componente de mapa; en variante, un relevamiento en revisión.
- Pasos: Given un relevamiento en recolección en el mapa; When el jefe crea un marcador con etiqueta "acceso", luego arrastra un marcador existente, luego crea uno dentro del radio de otro y, en otra corrida, intenta crear sobre un relevamiento en revisión; Then el front fija el marcador con su etiqueta e identidad estable, actualiza la coordenada conservando identidad al mover, muestra el marcador en conflicto con la información accesible, y presenta el mapa en solo lectura fuera de recolección (RELEVAMIENTO_NO_EN_RECOLECCION).
- Expected output: creación, movimiento e identidad estable; conflicto convive; solo lectura fuera de recolección.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-10-carrusel-encadenado-y-filtro-etiqueta
- Tipo: componente de UI (carrusel) + integración
- Cubre: CU-06 (CA-01, CA-02, CA-04), RN-04
- Setup: un relevamiento en revisión con tres marcadores con fotos, fotos etiquetadas "fisura" y un marcador sin fotos.
- Pasos: Given un relevamiento en revisión con tres marcadores con fotos; When el jefe avanza en el carrusel hasta el final de un marcador, filtra por la etiqueta "fisura" y selecciona un marcador sin fotos; Then el front encadena con las fotos del marcador contiguo sin cerrar el carrusel, muestra solo lo etiquetado "fisura" y, ante el marcador sin fotos, informa y ofrece pasar al contiguo.
- Expected output: encadenado correcto; filtro por etiqueta; manejo de marcador sin fotos.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-11-evidencia-accesible-con-conflictos
- Tipo: integración
- Cubre: CU-06 (CA-03), RN-02, RN-05
- Setup: un relevamiento en revisión con dos marcadores en conflicto dentro de un radio, con evidencia cargada por un autor que luego fue dado de baja.
- Pasos: Given un relevamiento con dos marcadores en conflicto y evidencia de un autor dado de baja; When el jefe abre la revisión sobre el mapa; Then el front muestra toda la evidencia accesible, señala que hay conflictos pendientes de resolver al cierre y conserva visible la autoría pese a la baja.
- Expected output: información accesible con conflictos presentes; autoría conservada.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-12-resolver-conflicto-unificar-separar
- Tipo: integración (a través de la API)
- Cubre: CU-07 (CA-01, CA-02, CA-04), RN-05
- Setup: un relevamiento en revisión con dos marcadores en conflicto, con etiquetas "fisura" y "junta".
- Pasos: Given un relevamiento en revisión con un conflicto; When el jefe los unifica, luego unifica marcadores con etiquetas distintas y luego reabre una resolución previa de "separados" y la unifica; Then el front muestra un único marcador resultante con la evidencia de ambos y un conflicto menos, el marcador resultante conserva ambas etiquetas, y la reapertura vuelve a dejar el conflicto pendiente aplicando la nueva decisión.
- Expected output: unificación reasigna evidencia; unión de etiquetas; reapertura aplica nueva decisión.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-13-resolucion-fuera-de-revision-solo-lectura
- Tipo: unitario (control de habilitación por estado)
- Cubre: CU-07 (CA-03), RN-04, RN-05
- Setup: un relevamiento todavía en recolección con un conflicto presente.
- Pasos: Given un relevamiento en recolección con un conflicto; When el jefe intenta resolver el conflicto; Then el front presenta la pantalla en solo lectura (RELEVAMIENTO_NO_EN_REVISION).
- Expected output: resolución no habilitada fuera de revisión.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-14-transicion-recoleccion-a-revision-y-cierre
- Tipo: integración
- Cubre: CU-08 (CA-01, CA-03, CA-04), RN-04, RN-05
- Setup: un relevamiento "Tramo Norte" en recolección sin conflictos y, en variante, un relevamiento cerrado.
- Pasos: Given un relevamiento en recolección; When el jefe lo pasa a revisión, luego lo cierra sin conflictos pendientes y, en otra corrida, intenta pasar un relevamiento ya cerrado de nuevo a recolección; Then el front lo muestra en revisión, luego cerrado y habilitado para el informe, y no ofrece la transición no permitida desde cierre (TRANSICION_NO_PERMITIDA).
- Expected output: transición y cierre válidos; transición inválida no ofrecida.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-15-cierre-bloqueado-con-conflictos-pendientes
- Tipo: integración + unitario
- Cubre: CU-08 (CA-02), RN-05
- Setup: un relevamiento en revisión con un conflicto de marcadores sin resolver.
- Pasos: Given un relevamiento en revisión con un conflicto sin resolver; When el jefe intenta cerrarlo; Then el front bloquea el cierre con CONFLICTOS_PENDIENTES y deriva a la pantalla de resolución (CU-07).
- Expected output: cierre bloqueado; derivación a resolución.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-16-carga-manual-radio-y-foto-sin-ubicacion
- Tipo: integración (a través de la API)
- Cubre: CU-09 (CA-01, CA-02, CA-03, CA-04), RN-04, RN-01
- Setup: un agente asignado a un relevamiento en recolección; fotos con ubicación incrustada dentro y fuera de un radio de 15 metros y una foto sin ubicación incrustada.
- Pasos: Given un agente asignado con radio de 15 metros definido; When sube tres fotos con ubicación dentro del radio, una foto lejana, una foto sin ubicación y, en otra corrida, intenta subir sin definir radio; Then el front agrupa las tres en un único marcador, crea un marcador nuevo para la lejana, deja la foto sin ubicación pendiente de ubicación manual sin coordenada inventada y bloquea la carga sin radio (RADIO_NO_DEFINIDO).
- Expected output: agrupación por radio; marcador nuevo para foto lejana; foto sin ubicación pendiente; carga sin radio bloqueada.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-17-exportar-importar-y-unidad-invalida
- Tipo: integración
- Cubre: CU-10 (CA-01, CA-02, CA-03, CA-04), RN-01
- Setup: un relevamiento cerrado con fotos, comentarios y etiquetas; una unidad transferible válida y una dañada; un relevamiento de otro jefe.
- Pasos: Given un relevamiento cerrado dentro del alcance; When el jefe lo exporta, luego importa una unidad válida, luego intenta importar una unidad dañada y, en otra corrida, intenta exportar un relevamiento ajeno; Then el front entrega una única unidad transferible, reconstruye el relevamiento con toda su evidencia en su lugar, rechaza la unidad dañada con UNIDAD_INVALIDA sin crear un relevamiento parcial, y no lista el relevamiento ajeno (FUERA_DE_ALCANCE).
- Expected output: export produce unidad única; import reconstruye; unidad inválida rechazada; alcance respetado.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-18-configuracion-almacenamiento-solo-raiz
- Tipo: unitario (control de visibilidad por rol) + integración
- Cubre: CU-11 (CA-01, CA-02, CA-03, CA-04), RN-01
- Setup: un usuario raíz con destino vigente "infraestructura propia" y al menos dos destinos disponibles; un jefe de área con sesión activa.
- Pasos: Given un usuario raíz; When cambia el destino a un servicio externo con sus datos completos, luego intenta aplicar un destino externo sin completar datos y, en variante, un jefe de área intenta acceder a la pantalla; Then el front muestra el nuevo destino vigente al raíz, no envía el cambio con datos incompletos (CONFIGURACION_INVALIDA) y no ofrece la pantalla al jefe de área (ROL_NO_AUTORIZADO), siendo el cambio transparente para los demás roles.
- Expected output: cambio solo por el raíz; datos incompletos no aplican; pantalla no disponible para otros roles.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-19-custodia-token-no-expuesto-al-navegador
- Tipo: componente de UI (no exposición)
- Cubre: CU-01, NFR custodia del token (arquitectura §8, ADR-03)
- Setup: una sesión activa de cualquier rol administrador sobre el front.
- Pasos: Given una sesión activa con token retenido del lado servidor del circuito; When se inspecciona la superficie de presentación serializada al navegador en cada vista clave; Then el token bearer no aparece serializado en ninguna vista.
- Expected output: 0 exposiciones del token al navegador.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-20-latencia-interaccion-p95-vistas-clave
- Tipo: rendimiento (interacción del circuito)
- Cubre: NFR latencia de interacción p95 (P.10); CU-03, CU-06, CU-08
- Setup: ambiente de referencia equivalente al productivo con geovial-api alcanzable y red estable; vistas clave de listado de relevamientos (CU-03), revisión sobre mapa con carrusel (CU-06) y ciclo del relevamiento (CU-08).
- Pasos: Given el front en el ambiente de referencia; When se ejecuta una carga de interacciones representativas sobre las vistas clave midiendo el percentil 95 del tiempo entre la acción del usuario y la actualización de la vista, excluyendo la latencia atribuible al backend; Then el p95 de interacción es ≤ 200 ms.
- Expected output: latencia de interacción p95 ≤ 200 ms.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-21-concurrencia-50-circuitos
- Tipo: rendimiento (carga de circuitos)
- Cubre: NFR ≥ 50 circuitos concurrentes (P.10)
- Setup: ambiente de referencia; cliente de carga capaz de abrir y sostener circuitos interactivos concurrentes.
- Pasos: Given el front en el ambiente de referencia; When se abren y mantienen al menos 50 circuitos interactivos concurrentes ejecutando interacciones; Then la latencia de interacción p95 se sostiene y ningún circuito pierde su estado de sesión.
- Expected output: ≥ 50 circuitos concurrentes con p95 sostenido y sin pérdida de estado de sesión.
- Actual output: no ejecutado.
- Status: Pendiente.

### TC-22-snapshot-vistas-clave
- Tipo: snapshot de vistas
- Cubre: CU-01, CU-03, CU-06, CU-07 (estabilidad de render); RN-04
- Setup: dataset sintético sembrado para las vistas de ingreso, listado de relevamientos, revisión sobre mapa con carrusel y resolución de conflictos.
- Pasos: Given las vistas clave con el dataset sintético; When se renderizan del lado servidor; Then la estructura del render coincide con el snapshot baseline aprobado; cualquier diferencia exige regeneración con justificación y revisión.
- Expected output: render estable contra el baseline; sin diferencias no aprobadas.
- Actual output: no ejecutado.
- Status: Pendiente.

## 3. Resumen de cobertura por CU, RN y NFR

| Origen | TC que lo cubren |
| --- | --- |
| CU-01 | TC-01, TC-02, TC-19, TC-22 |
| CU-02 | TC-03, TC-04 |
| CU-03 | TC-05, TC-06, TC-20, TC-22 |
| CU-04 | TC-07, TC-08 |
| CU-05 | TC-09 |
| CU-06 | TC-10, TC-11, TC-20, TC-22 |
| CU-07 | TC-12, TC-13, TC-22 |
| CU-08 | TC-14, TC-15, TC-20 |
| CU-09 | TC-16 |
| CU-10 | TC-17 |
| CU-11 | TC-18 |
| RN-01 | TC-03, TC-07, TC-09, TC-16, TC-17, TC-18 |
| RN-02 | TC-04, TC-11 |
| RN-03 | TC-01, TC-02 |
| RN-04 | TC-05, TC-06, TC-09, TC-10, TC-13, TC-14, TC-16, TC-22 |
| RN-05 | TC-11, TC-12, TC-13, TC-14, TC-15 |
| NFR custodia del token | TC-19 |
| NFR latencia interacción p95 | TC-20 |
| NFR circuitos concurrentes | TC-21 |

## 4. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Catálogo inicial de 22 casos de prueba referenciales de geovial-web: al menos un TC por cada uno de los 11 CU y por cada una de las 5 RN, TC de componente de UI (mapa, carrusel), TC de snapshot de vistas clave (TC-22) y TC del NFR de interacción (TC-20), más concurrencia (TC-21) y custodia del token (TC-19). Pasos en Given/When/Then derivados de los criterios de aceptación de 02; status inicial Pendiente por front no implementado. |
