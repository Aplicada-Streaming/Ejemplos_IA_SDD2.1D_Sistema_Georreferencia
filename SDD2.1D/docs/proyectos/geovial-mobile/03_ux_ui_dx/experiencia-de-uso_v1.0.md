# Experiencia de uso — geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** experiencia-de-uso_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Mobile UX Designer + Accessibility Specialist
**Variante:** UX/UI

## 0. Propósito y posición en la cadena

Este documento es el marco de experiencia de la app móvil `geovial-mobile`, la herramienta del agente de campo de la solución GeoVial. Recibe insumos de 00 (visión y persona objetivo: el agente de campo) y de 02 (los siete casos de uso de flujos de campo y las cinco reglas de negocio del lado móvil) y produce el ancla de experiencia que los wireframes de esta sección materializan, que 06 convierte en criterios de aceptación visuales y de ergonomía táctil, y que 08 traduce en tests de interfaz, de captura offline y de accesibilidad.

El marco define cómo se siente la app: audiencia y contexto de uso en terreno, principios de diseño, flujos clave, estados y feedback, accesibilidad reforzada, internacionalización, performance percibida, errores y recuperación. No define el qué funcional (vive en 02) ni el cómo técnico de la capa de presentación, el almacén local o el motor de sincronización (vive en 05). No incluye colores, tipografías ni decisiones de interfaz fina; tampoco menciona stacks concretos: cuando se habla de "app móvil", "componente de mapa", "cámara del dispositivo", "almacén local" o "seguridad del dispositivo (patrón, huella)" se describe el rol de la pieza, no su implementación.

El piso de accesibilidad es WCAG 2.2 nivel AA, reforzado por el contexto de uso en exteriores, táctil, con guantes y bajo luz solar directa.

## 1. Audiencia y contexto de uso

La audiencia única de la app es el agente de campo: la persona que toma los relevamientos asignados e ingresa y administra sus observaciones, fotos, comentarios y etiquetas en terreno (visión §2). No hay roles administradores en la app: la gestión y el cierre ocurren en el front web, fuera de esta sección. El actor primario es el mismo en los siete CU.

| Persona | Origen en 00 | Contexto físico | Contexto emocional | Frecuencia y duración | Competencia digital esperada |
| --- | --- | --- | --- | --- | --- |
| Agente de campo | Visión §2 (rol del sistema, operación en terreno) | Intemperie sobre puentes y caminos; de pie o en movimiento; sol directo o sombra cambiante; polvo, humedad y frío; manos ocupadas o con guantes; conectividad nula o intermitente | Concentración en la inspección del tramo, no en la app; apuro por no demorar la recorrida; necesidad de confiar en que nada se pierde sin conexión | Uso intensivo durante jornadas de campo; ráfagas de captura intercaladas con desplazamiento; sincronización al recuperar señal | Media a baja; usa el teléfono a diario pero no necesariamente apps técnicas; prioriza simpleza y robustez sobre opciones |

Notas de contexto que condicionan todo el diseño:

- Uso táctil con una sola mano y, a menudo, con guantes. Los objetivos táctiles son grandes y separados; los gestos se reducen a los imprescindibles (tocar, arrastrar el marcador, deslizar fotos). No se exige precisión fina ni gestos de varios dedos para acciones críticas.
- Luz solar directa. La legibilidad no puede depender del color ni de contrastes sutiles: el contraste es alto, la jerarquía es de tamaño y posición, y cada estado se comunica además por texto e ícono, nunca solo por color.
- Movilidad y fatiga. El agente está de pie, posiblemente cansado y con la atención en el tramo. Las acciones primarias están al alcance del pulgar, las pantallas muestran lo justo y el sistema confirma cada captura para que el agente no tenga que recordar qué ya registró.
- Conectividad variable. El modo normal es sin conexión. La app nunca bloquea la captura por falta de red; la sincronización es un proceso de segundo plano que el agente puede ignorar hasta que vuelve la señal.
- Dispositivo posiblemente compartido. Varios agentes pueden usar el mismo equipo; la sesión se protege con la seguridad del dispositivo y se libera con un deslogueo completo (CU-01, RN-04).
- Permisos del sistema operativo. La app pide ubicación (GPS), cámara y acceso al almacenamiento o galería solo cuando la función los necesita por primera vez, explicando para qué; ante denegación, degrada con dignidad en lugar de bloquearse (CU-03, CU-04, CU-07).

## 2. Principios de diseño

Se seleccionan las heurísticas de Nielsen y las leyes UX pertinentes a una app de captura en terreno, con justificación de aplicación al producto. Cada principio se verifica antes de pasar a 08.

| Heurística de Nielsen | Aplicación en geovial-mobile | Verificación |
| --- | --- | --- |
| Visibilidad del estado del sistema | Un indicador de conectividad y de estado de sincronización está siempre presente (sin conexión, cambios en cola, sincronizando, al día); cada captura confirma que quedó guardada localmente y encolada (RN-05, RN-02). | Inspección heurística; snapshot del indicador de estado en cada superficie (08). |
| Correspondencia con el mundo real | El vocabulario de pantalla usa términos que el agente reconoce (relevamiento, tramo, marcador, foto, etiqueta, comentario) sin jerga técnica de implementación ni del almacén local. | Revisión de microcopy contra el glosario de 02 y el glosario-ux de esta sección. |
| Control y libertad del usuario | El agente puede mover un marcador, reubicar una foto pendiente, corregir un comentario o quitar una etiqueta mientras el relevamiento esté en recolección (CU-03, CU-05); las capturas son revisables y editables, no irreversibles. | Test de edición y de movimiento de marcador conservando identidad (08, sobre CU-03, CU-05). |
| Consistencia y estándares | El mismo patrón de captura (acción primaria grande, confirmación de guardado, indicador de cola) se repite entre crear marcador, capturar foto y comentar; la app respeta las convenciones del sistema operativo móvil para permisos, cámara y seguridad del dispositivo. | Auditoría de consistencia de patrones entre wireframes. |
| Prevención de errores | La app no inventa coordenadas: ante falta de señal o de metadatos, la foto o el marcador quedan pendientes de ubicación en lugar de ubicarse mal (CU-03 SIN_SENAL_GPS, CU-04 5.A, CU-07 5.A); el deslogueo completo se confirma para no borrar la sesión por accidente. | Test de captura sin GPS deja pendiente sin coordenada inventada (08, sobre CU-03, CU-04, CU-07). |
| Reconocer antes que recordar | El mapa muestra la posición del agente, los marcadores existentes y su evidencia en contexto; la cola de sincronización lista qué falta subir; el agente no memoriza identificadores ni cuántas capturas hizo. | Inspección heurística sobre mapa de captura y estado de sincronización. |
| Flexibilidad y eficiencia de uso | El agente experto centra por GPS y captura en pocos toques; reutiliza etiquetas ya creadas del relevamiento sin re-tipearlas (CU-05 5.A) y carga lotes de fotos por radio en una operación (CU-07). | Pruebas de usabilidad en terreno con agentes. |
| Diseño estético y minimalista | Cada pantalla muestra solo lo necesario para la tarea de campo; los controles secundarios se repliegan; la densidad se mantiene baja para legibilidad bajo sol y uso con guantes. | Inspección heurística de densidad y legibilidad en exteriores. |
| Reconocer, diagnosticar y recuperarse de errores | Cada error dice qué pasó, por qué y qué hacer (ver §8); ningún código técnico crudo llega a la pantalla; ante permiso denegado se ofrece la vía alternativa (fijación manual, etc.). | Auditoría de mensajes contra la taxonomía de §8. |
| Ayuda y documentación | Estados vacíos con texto orientativo y acción siguiente; microcopy contextual al pedir cada permiso, explicando para qué lo usa la app. | Revisión de estados vacíos y de los diálogos de permiso de cada wireframe. |

Leyes UX aplicadas:

| Ley UX | Aplicación en geovial-mobile | Justificación |
| --- | --- | --- |
| Ley de Fitts | Las acciones primarias (centrar por GPS, crear marcador, capturar foto, sincronizar) son objetivos grandes, separados y ubicados en la zona alcanzable con el pulgar. | El uso es táctil, de pie, con guantes y a menudo con una sola mano; objetivos grandes y bien ubicados reducen el error de toque. |
| Ley de Hick | Cada pantalla expone pocas acciones primarias por vez; las opciones avanzadas se repliegan; el flujo de captura ofrece la decisión mínima en cada paso. | Menos opciones simultáneas reducen el tiempo de decisión de quien tiene la atención en el tramo, no en la app. |
| Ley de Miller | La lista de relevamientos, el detalle de observación y la cola de sincronización agrupan y resumen para no exigir retener muchos elementos; los contadores reemplazan a la enumeración mental. | El agente maneja muchas fotos y cambios pendientes; agrupar evita sobrecarga de memoria de trabajo en campo. |
| Ley de Jakob | La app reutiliza patrones móviles conocidos (lista, mapa con pines, cámara del dispositivo, selector de fotos, indicador de sincronización tipo bandeja de salida). | La audiencia conoce su teléfono; respetar convenciones del sistema operativo reduce la curva de aprendizaje y mitiga R-01 de 00 (baja adopción en campo). |
| Ley de Tesler (de la conservación de la complejidad) | La complejidad de la sincronización (orden subir-luego-bajar, idempotencia, reanudación, conflictos) la absorbe el sistema; al agente se le presenta como un estado simple: hay cambios por subir, se están subiendo, está al día, hay elementos en conflicto. | El agente no debe gestionar la mecánica de sincronización; el sistema asume esa complejidad para no trasladarla a quien trabaja en terreno (RN-02, RN-03). |

## 3. Flujos clave

Cada flujo es un user journey con disparador, pasos, puntos de fricción anticipados y salida, anclado a los CU de 02. Todos los flujos de captura funcionan sin conexión.

### 3.1 Sesión y seguridad del dispositivo (CU-01)

- Disparador: el agente abre la app para trabajar, la reanuda tras un bloqueo, o libera el equipo al terminar la jornada.
- Pasos: primer inicio con credenciales y conexión; la app guarda el token en el almacenamiento seguro del dispositivo y habilita el trabajo; en cada reanudación con sesión activa, la app pide verificarse por la seguridad del dispositivo (patrón, huella) sin reingresar credenciales; al terminar en un equipo compartido, el agente ejecuta el deslogueo completo, que borra el token y los datos de sesión.
- Puntos de fricción anticipados: primer inicio sin red (la app explica que requiere conexión por única vez, SIN_CONEXION_INICIO); token vencido en el relogueo (exige nuevo inicio online, 5.A); cambio de usuario sobre sesión ajena (exige deslogueo previo, 5.C); dispositivo sin seguridad configurada (advierte y exige inicio online en cada reanudación, DISPOSITIVO_SIN_SEGURIDAD); verificación del dispositivo fallida (permite reintentar o cerrar sesión).
- Salida: sesión activa con el trabajo de campo habilitado, acceso rehabilitado sin credenciales, o equipo liberado sin rastros del agente anterior.

### 3.2 Elegir el relevamiento de trabajo (CU-02)

- Disparador: el agente, con sesión activa, elige sobre qué relevamiento asignado va a trabajar.
- Pasos: abre la lista de relevamientos asignados servida del almacén local, con tramo y estado; selecciona uno; la app lo fija como contexto activo y abre el mapa con sus marcadores y observaciones locales; queda habilitado para capturar.
- Puntos de fricción anticipados: primer uso sin nada sincronizado y sin red (informa que no hay relevamientos disponibles sin conexión, SIN_RELEVAMIENTOS_LOCALES); refresco con red para traer asignaciones nuevas (dispara un ciclo de sincronización, CU-06); relevamiento cerrado por el jefe (se abre en solo lectura, sin habilitar capturas, RELEVAMIENTO_CERRADO).
- Salida: un relevamiento queda como contexto activo de captura con su mapa abierto, o el agente queda informado de que no hay datos disponibles sin conexión.

### 3.3 Georreferenciar: centrar por GPS y crear o mover marcador (CU-03)

- Disparador: el agente quiere anclar evidencia al punto del tramo donde está parado.
- Pasos: toca centrar por GPS; la app pide el permiso de ubicación si es la primera vez y centra el mapa sobre su posición; el agente crea un marcador en esa posición o mueve uno existente; la app lo guarda en el almacén local con identidad propia y estable y lo encola para sincronizar.
- Puntos de fricción anticipados: permiso de ubicación denegado (la app explica para qué lo necesita y ofrece fijar el marcador manualmente en el mapa, PERMISO_UBICACION_DENEGADO); sin señal de GPS (ofrece fijación manual sin inventar coordenada, SIN_SENAL_GPS); precisión baja (centra igual y señala la baja precisión para que el agente ajuste, 5.C); marcador dentro del radio de otro (lo crea y lo deja convivir como posible conflicto, sin bloquear, RN-03); relevamiento cerrado (modo lectura).
- Salida: existe un marcador en el almacén local, encolado como cambio pendiente, listo para anclar fotos y observaciones.

### 3.4 Capturar foto con resolución de coordenadas en el momento (CU-04)

- Disparador: el agente fotografía la evidencia en el lugar de la observación.
- Pasos: toca capturar foto sobre el marcador activo; la app abre la cámara del dispositivo (pidiendo el permiso si es la primera vez) y el agente toma la foto; la app resuelve la coordenada del momento, ancla la foto a una observación del marcador y aloja la imagen en el almacén local, encolando los cambios.
- Puntos de fricción anticipados: permiso de cámara denegado (no abre la cámara y explica que es necesario, PERMISO_CAMARA_DENEGADO); sin señal de GPS al disparar (conserva la foto anclada al marcador y la marca como pendiente de ubicación precisa, sin inventar coordenada, 5.A); sin marcador del entorno (crea uno en la coordenada del momento, 5.C); almacén local sin espacio (no guarda y avisa que libere espacio, ALMACEN_LOCAL_SIN_ESPACIO); relevamiento cerrado (modo lectura).
- Salida: una observación con su foto georreferenciada queda en el almacén local y encolada, o la foto queda pendiente de ubicación sin coordenada inventada.

### 3.5 Enriquecer la evidencia: comentarios y etiquetas (CU-05)

- Disparador: el agente describe y clasifica lo que capturó.
- Pasos: abre una observación o una foto del marcador activo; escribe la nota de la observación, un comentario para la foto y aplica una o más etiquetas a la foto o al marcador; reutiliza etiquetas ya existentes del relevamiento o crea nuevas; la app registra todo en el almacén local y lo encola.
- Puntos de fricción anticipados: etiqueta sin nombre (rechazada con pedido de nombre, ETIQUETA_VACIA); comentar una foto pendiente de ubicación (se permite igual; la falta de coordenada no impide describir, 5.B); observación o foto que ya no existe (avisa que no está disponible, OBSERVACION_INEXISTENTE); relevamiento cerrado (modo lectura).
- Salida: la observación queda descrita y clasificada en el almacén local, con los cambios encolados.

### 3.6 Trabajar sin conexión y sincronizar (CU-06) — flujo eje

- Disparador: el agente recolecta sin red y, al recuperar señal, la app sincroniza; o el agente fuerza la sincronización.
- Pasos: cada captura se acumula en la cola local; la app detecta conexión y orquesta el ciclo subir-luego-bajar a través de la librería de sincronización; sube primero los cambios locales en orden de creación, retirándolos de la cola al confirmarse; solo al concluir la subida baja las actualizaciones del relevamiento; muestra un resumen con cambios subidos, actualizaciones bajadas y elementos en conflicto.
- Puntos de fricción anticipados: corte durante la subida (deja confirmado lo subido, conserva el resto en la cola, no baja, queda reanudable sin duplicar, 5.A); actualizaciones en conflicto en la bajada (las aplica como estado válido en conflicto, no aborta, las reporta en el resumen, 5.B, RN-03); sincronización ya en curso (no inicia un segundo ciclo, muestra el vigente, 5.C); backend inalcanzable (detiene el ciclo, conserva la cola, BACKEND_INALCANZABLE); token rechazado (detiene sin alterar la cola y pide reloguear, TOKEN_INVALIDO); relevamiento cerrado por el jefe (no sube, conserva la cola y avisa que ya no admite cambios, RELEVAMIENTO_CERRADO).
- Salida: la cola quedó vacía de lo confirmado y la copia local actualizada; o quedó reanudable sin pérdida ni duplicación, con el agente informado del estado.

### 3.7 Carga manual de fotos con radio de agrupación (CU-07)

- Disparador: el agente carga fotos ya tomadas (por ejemplo, con otro equipo) al relevamiento activo.
- Pasos: selecciona un conjunto de fotos del almacenamiento del dispositivo (pidiendo el permiso de acceso si es la primera vez); por cada foto, la app extrae la ubicación incrustada y la prioriza; busca un marcador local dentro del radio de agrupación; agrupa la foto en él o crea un marcador nuevo en su ubicación; aloja los binarios y encola; muestra el resultado: fotos agrupadas, marcadores nuevos y fotos sin ubicación resuelta.
- Puntos de fricción anticipados: sin radio aplicable (no procesa y pide un radio, RADIO_NO_DEFINIDO); permiso de almacenamiento denegado (no accede y explica que es necesario, PERMISO_ALMACENAMIENTO_DENEGADO); foto sin ubicación incrustada (pendiente de ubicación manual sin inventar coordenada, 5.A); varias fotos en el mismo radio (todas al mismo marcador, 5.B); formato no soportado (omite esa foto, continúa con el resto y la señala, FORMATO_FOTO_NO_SOPORTADO); relevamiento cerrado (modo lectura).
- Salida: las fotos con ubicación quedan agrupadas en marcadores y las sin ubicación quedan pendientes de ubicación manual, con todo encolado.

## 4. Estados y feedback

Mapa de estados por superficie clave. El feedback visual se describe por su rol (banner, inline, skeleton, confirmación sutil, indicador persistente), sin colores ni tipografías. Por el contexto offline-first, los estados sin conexión y sincronizando son de primera clase, no excepcionales. Estos estados son piso para los wireframes de la sección y para los snapshot tests de 08.

### 4.1 Tabla general de estados

| Estado | Condición que lo produce | Feedback visual | Feedback textual |
| --- | --- | --- | --- |
| Vacío | No hay datos para mostrar todavía (sin relevamientos, marcador sin fotos, cola vacía) | Ilustración o ícono neutro y acción siguiente | Texto orientativo que explica el vacío y propone el próximo paso |
| Cargando | Operación local o de red en curso (abrir copia local, procesar lote de fotos) | Skeleton de la estructura esperada; spinner solo si supera el umbral percibido | Indicación de qué se está preparando |
| Con datos | La superficie tiene contenido para operar | Render normal de la superficie | — |
| Sin conexión | El dispositivo no tiene red disponible | Indicador persistente y no intrusivo de modo sin conexión, siempre visible | "Sin conexión. Tu trabajo se guarda en el dispositivo y se sincroniza solo cuando vuelva la señal." |
| Sincronizando | Hay un ciclo de sincronización en curso | Indicador de progreso con avance de la cola; no bloquea la captura | "Subiendo tus cambios… N de M" y, tras la subida, "Trayendo actualizaciones del relevamiento" |
| Error recuperable | Falla transitoria o de validación con vía de recuperación | Banner o inline según el alcance del error | Causa probable y acción de recuperación (reintentar, corregir, liberar espacio) |
| Éxito | Captura guardada localmente y encolada, o ciclo de sincronización completado | Confirmación visual sutil y actualización del contador de cola | "Guardado y en cola para sincronizar" o "Relevamiento al día" con el resumen del ciclo |
| En conflicto | Hay marcadores en conflicto por radio en el relevamiento | Marca no bloqueante en el marcador y nota en el resumen de sincronización | "Hay marcadores en conflicto. Tu trabajo sigue disponible; el jefe los resuelve al cierre." (RN-03) |
| Pendiente de ubicación | Una foto quedó sin coordenada (sin GPS o sin metadatos) | Marca de pendiente de ubicación sobre la foto y acceso a ubicarla en el mapa | "Esta foto quedó sin ubicación. Ubicala en el mapa cuando puedas." (sin coordenada inventada) |

### 4.2 Estados por superficie clave

| Superficie | Vacío | Cargando | Con datos | Sin conexión | Sincronizando | Error |
| --- | --- | --- | --- | --- | --- | --- |
| Inicio de sesión y relogueo (CU-01) | Formulario de credenciales limpio (primer inicio) o solicitud de verificación del dispositivo (reanudación) | Validación de credenciales o de verificación en curso, controles deshabilitados | Sesión activa: abre la lista de relevamientos | Primer inicio bloqueado: "El primer inicio necesita conexión por única vez." (SIN_CONEXION_INICIO); el relogueo por seguridad del dispositivo funciona sin red | No aplica (la sesión no sincroniza datos del relevamiento) | Credenciales inválidas (inline); verificación del dispositivo fallida (reintentar o cerrar sesión); dispositivo sin seguridad (advertencia) |
| Lista de relevamientos asignados (CU-02) | "Todavía no tenés relevamientos en el dispositivo" con acción de refrescar si hay red | Skeleton de filas de la lista | Lista con tramo y estado de cada relevamiento | Lista servida del almacén local; el refresco se difiere; sin copia local: SIN_RELEVAMIENTOS_LOCALES | Refresco trayendo asignaciones nuevas con progreso | Relevamiento ya no asignado tras refresco (se retira); relevamiento cerrado (abre en solo lectura) |
| Mapa de captura (CU-03, CU-04) | Relevamiento sin marcadores: mapa centrado y acción de crear el primer marcador | Skeleton del mapa y obtención de posición del GPS | Mapa con posición del agente, marcadores y acciones de captura | Captura plenamente operativa sin red; las capturas se encolan | Indicador de cola visible mientras un ciclo corre en segundo plano, sin interrumpir la captura | Permiso de ubicación o cámara denegado (degradación a fijación manual / captura no procede); sin señal de GPS (pendiente de ubicación); sin espacio (ALMACEN_LOCAL_SIN_ESPACIO) |
| Detalle de observación (CU-05) | Observación sin fotos ni comentarios: acción de capturar o de cargar fotos | Carga de las fotos del marcador desde el almacén local | Fotos del marcador con sus comentarios y etiquetas | Edición de notas, comentarios y etiquetas operativa sin red, se encola | Indicador de cola visible; la edición no se interrumpe | Etiqueta vacía (inline); observación o foto inexistente (aviso); relevamiento cerrado (solo lectura) |
| Estado de sincronización (CU-06) | Cola vacía: "No tenés cambios pendientes. Relevamiento al día." | Recuento de la cola y verificación de conectividad | Lista de cambios en cola y resumen del último ciclo | "Sin conexión. Hay N cambios esperando para subir." con la cola visible | Progreso del ciclo: fase de subida (N de M) y luego fase de bajada; elementos en conflicto al final | Backend inalcanzable o token inválido (conserva la cola, ofrece reintento o reloguear) |
| Carga manual de fotos (CU-07) | Sin fotos seleccionadas: acción de elegir fotos del dispositivo | Procesamiento del lote (lectura de ubicación incrustada y agrupación por radio) | Resultado: agrupadas, marcadores nuevos, sin ubicación resuelta | Procesamiento y encolado operativos sin red | Indicador de cola tras encolar el lote | Sin radio (RADIO_NO_DEFINIDO); permiso de almacenamiento denegado; formato no soportado (omite y señala) |

## 5. Accesibilidad

Compromiso explícito: WCAG 2.2 nivel AA como piso mínimo en toda la app, reforzado por el contexto de uso en terreno. No se aceptan menciones genéricas; se enumeran los criterios prioritarios y dónde se aplican. Las menciones a versiones anteriores de WCAG, si las hubiera, son solo histórico evolutivo y no rebajan el piso.

El refuerzo de accesibilidad responde a tres condiciones del contexto: legibilidad bajo sol directo, uso táctil con guantes y operación en movimiento. Los criterios novedad 2.2 (foco no oscurecido, tamaño de objetivo, ayuda consistente, entrada redundante, autenticación accesible) se listan explícitamente para dejar claro que el piso es 2.2 AA y no una versión anterior.

| Criterio WCAG 2.2 AA | Aplicación reforzada en geovial-mobile | Dónde se verifica |
| --- | --- | --- |
| Contraste de texto (1.4.3, 4.5:1) y de componentes no textuales (1.4.11, 3:1) | Contraste alto en texto, controles, pines del mapa e indicadores de estado y conectividad para legibilidad bajo sol directo; se apunta a un margen por encima del mínimo donde el contexto exterior lo exige | Test de contraste automatizado y manual, incluida prueba de legibilidad en exteriores (08) |
| Uso del color (1.4.1) | Ningún estado se comunica solo por color: conectividad, cola, conflicto y pendiente de ubicación se indican también por texto e ícono | Auditoría de redundancia de señal de estado (08) |
| Tamaño de objetivo (2.5.8, novedad 2.2) | Las acciones primarias (centrar por GPS, crear marcador, capturar foto, sincronizar) superan con holgura el tamaño mínimo de objetivo, dimensionadas para uso con guantes; los objetivos están separados para evitar toques accidentales | Inspección de tamaño y separación de objetivos táctiles (08) |
| Foco visible (2.4.7) y foco no oscurecido (2.4.11, novedad 2.2) | Indicador de foco visible para navegación con lector de pantalla o teclado externo; en diálogos de permiso, cámara y confirmación el elemento enfocado no queda tapado | Recorrido con lector de pantalla y snapshot de foco (08) |
| Etiquetas y nombres accesibles (1.3.1, 4.1.2, 3.3.2) | Cada acción de captura, cada pin, cada foto y cada control de sincronización tiene nombre accesible; los formularios de credenciales y de comentario tienen etiqueta asociada | Test de roles y nombres accesibles (08) |
| Alternativas textuales (1.1.1) | Cada foto expone su comentario y etiqueta como texto alternativo accesible; los íconos de conectividad, cola y conflicto tienen nombre accesible; el estado de la foto (georreferenciada o pendiente de ubicación) se anuncia | Auditoría de alternativas textuales (08) |
| Anuncio de cambios dinámicos (4.1.3 mensajes de estado) | El guardado de cada captura, el cambio de conectividad, el progreso de sincronización y el resumen del ciclo se anuncian por región de estado para lectores de pantalla | Test con lector de pantalla sobre CU-04 y CU-06 (08) |
| Movimiento por gestos y operación por puntero (2.5.1, 2.5.2) | Toda acción que use un gesto (arrastrar el marcador, deslizar fotos) tiene una alternativa de un solo toque; las acciones se confirman al levantar el dedo, permitiendo abortar el gesto | Test de alternativas a gestos (08, sobre CU-03) |
| Orientación (1.3.4) | La app opera en portrait como orientación primaria de campo y no exige rotar el dispositivo para completar ninguna tarea; ver nota responsive de cada wireframe | Inspección de operación en portrait (08) |
| Autenticación accesible (3.3.8, novedad 2.2) | El relogueo se apoya en la seguridad del dispositivo (huella, patrón), sin exigir resolver pruebas cognitivas ni transcribir; el primer inicio admite pegar credenciales y mostrar la credencial | Inspección del flujo de inicio y relogueo (08, sobre CU-01) |
| Ayuda consistente (3.2.6, novedad 2.2) y entrada redundante (3.3.7, novedad 2.2) | La vía de reintento de sincronización y de ubicar una foto pendiente se ubica de forma consistente; no se pide reingresar datos ya provistos en el mismo flujo | Inspección heurística y test de flujo de reintento (08) |

## 6. Internacionalización

- Idioma de partida: español rioplatense. La app se diseña con el texto externalizado para admitir otros idiomas sin rediseño, aunque hoy no hay un segundo idioma comprometido por el negocio (00 no declara multi-idioma).
- Expansión de texto: se prevé hasta un 35 por ciento de expansión respecto del español para idiomas más extensos; los rótulos de acción, los estados (sin conexión, sincronizando, en conflicto, pendiente de ubicación) y los mensajes no se truncan ni rompen el layout en pantallas estrechas. Las etiquetas del dominio (creadas por el agente) son de longitud variable y se truncan con indicación visual y texto completo accesible.
- Dirección de lectura: izquierda a derecha. La lógica del carrusel de fotos del marcador y del avance de la cola no se acopla a la dirección de lectura, de modo que un futuro idioma de derecha a izquierda solo requiera reflejar la disposición.
- Formatos de fecha, hora y número: fechas en formato YYYY-MM-DD en los artefactos de la sección; en pantalla, formato localizable según la configuración regional del dispositivo, sin asumir un formato fijo. Las coordenadas geográficas se presentan con separador decimal coherente con la configuración regional y la app no fuerza un formato de coordenada que el agente deba interpretar.
- Contenido del usuario: las fotos, los comentarios y las etiquetas son contenido del agente y se muestran tal cual los cargó, sin traducir.
- Permisos del sistema operativo: los diálogos de permiso de ubicación, cámara y almacenamiento los presenta el sistema operativo en el idioma del dispositivo; el microcopy propio de la app que justifica cada permiso se externaliza junto con el resto del texto.

## 7. Performance percibida

Tiempos máximos tolerables por acción y técnicas de percepción. Los tiempos son objetivos de experiencia, no presupuesto técnico (que vive en 05). La premisa es que la captura nunca espera por la red.

| Acción | Tolerancia percibida | Técnica de performance percibida |
| --- | --- | --- |
| Arranque en frío hasta el inicio de sesión o la verificación del dispositivo | Pocos segundos (objetivo de arranque del proyecto, ≤ 3 s en el dispositivo de referencia) | Pantalla de arranque mínima; verificación del dispositivo disponible apenas carga |
| Abrir la lista de relevamientos | Sensación de respuesta casi inmediata | Servida del almacén local; skeleton de filas si hace falta; el refresco con red es secundario |
| Centrar por GPS | Respuesta inmediata al tocar; la posición se afina en segundo plano | Centrar primero sobre la última posición conocida y refinar al llegar la fija; indicar precisión sin bloquear |
| Crear o mover un marcador | Inmediato; el marcador aparece al instante | Guardado local optimista; el encolado para sincronizar ocurre detrás, sin demorar el gesto |
| Capturar una foto | La cámara abre rápido; al confirmar, la foto aparece anclada de inmediato | Resolución de coordenada en paralelo a la escritura local; confirmación de guardado inmediata; la sincronización es posterior |
| Registrar comentario o etiqueta | Inmediato | Guardado local optimista; reutilización de etiquetas sin recargar el catálogo |
| Cargar un lote de fotos (CU-07) | Operación reconocidamente progresiva | Progreso por foto procesada; resultado parcial visible a medida que agrupa; no simular instantaneidad |
| Sincronizar | En segundo plano; nunca bloquea la captura | Indicador de progreso de la cola; el agente sigue trabajando mientras sube y baja; resumen al final |

Criterios de animación: animaciones breves y funcionales (confirmación de guardado, transición entre fotos del marcador, progreso de la cola). Se respeta la preferencia de movimiento reducido del sistema operativo: si el agente la activa, las transiciones se simplifican o se suprimen. Ninguna animación bloquea la captura ni oculta el estado de conectividad o de sincronización.

## 8. Errores y recuperación

Taxonomía de los errores que el agente verá, con el tono de los mensajes y la vía de recuperación. Principio de redacción: cada mensaje dice qué pasó, por qué pasó (cuando aporta) y qué hacer; lenguaje plano, en segunda persona del singular rioplatense, sin culpar al agente ni mostrar códigos técnicos crudos. Los códigos de la tabla provienen de los CU de 02 y son referencia interna y de trazabilidad con 08, no texto literal de pantalla.

| Categoría | Código de origen (02) | Qué ve el agente | Vía de recuperación |
| --- | --- | --- | --- |
| Credenciales o acceso | CREDENCIALES_INVALIDAS (CU-01) | "Las credenciales no coinciden. Revisá tu usuario y tu credencial e intentá de nuevo." | Reintento inline, conservando el usuario |
| Conexión inicial | SIN_CONEXION_INICIO (CU-01) | "El primer inicio necesita conexión por única vez. Buscá señal e intentá de nuevo." | Reintento cuando haya red; no se crea sesión |
| Seguridad del dispositivo | VERIFICACION_DISPOSITIVO_FALLIDA (CU-01) | "No pudimos verificarte con la seguridad del dispositivo. Probá de nuevo o cerrá sesión." | Reintento de la verificación o deslogueo completo |
| Seguridad del dispositivo | DISPOSITIVO_SIN_SEGURIDAD (CU-01) | "Este dispositivo no tiene patrón ni huella configurados. Vas a tener que iniciar sesión cada vez." | Configurar la seguridad del dispositivo o aceptar el inicio online en cada reanudación |
| Datos sin conexión | SIN_RELEVAMIENTOS_LOCALES (CU-02) | "Todavía no tenés relevamientos en el dispositivo y no hay conexión para traerlos." | Buscar señal y refrescar; el contexto no se fija |
| Estado del recurso | RELEVAMIENTO_NO_ASIGNADO (CU-02) | "Este relevamiento ya no está asignado a vos." | Se retira de la lista; el contexto no se fija |
| Estado del recurso | RELEVAMIENTO_CERRADO (CU-02, CU-03, CU-04, CU-05, CU-07) | "Este relevamiento está cerrado. Lo abrimos en solo lectura." | Vista en solo lectura; no se habilitan capturas |
| Permisos del sistema operativo | PERMISO_UBICACION_DENEGADO (CU-03) | "Necesitamos el permiso de ubicación para centrar por GPS. Podés fijar el marcador a mano en el mapa." | Conceder el permiso desde el sistema operativo o fijación manual en el mapa |
| Señal de GPS | SIN_SENAL_GPS (CU-03, CU-04) | "No hay señal de GPS ahora. Fijá el marcador en el mapa; no inventamos la ubicación." (CU-03) / "Guardamos la foto sin ubicación; ubicala en el mapa cuando puedas." (CU-04) | Fijación manual del marcador / foto pendiente de ubicación, sin coordenada inventada |
| Permisos del sistema operativo | PERMISO_CAMARA_DENEGADO (CU-04) | "Necesitamos el permiso de cámara para tomar la foto." | Conceder el permiso desde el sistema operativo |
| Almacenamiento | ALMACEN_LOCAL_SIN_ESPACIO (CU-04) | "No hay espacio en el dispositivo para guardar la foto. Liberá espacio e intentá de nuevo." | Liberar espacio y reintentar la captura |
| Validación de entrada | ETIQUETA_VACIA (CU-05) | "La etiqueta necesita un nombre." | Corrección inline; la etiqueta no se crea |
| Recurso ausente | OBSERVACION_INEXISTENTE (CU-05) | "Esa observación ya no está disponible." | El cambio no se aplica; vuelve al marcador |
| Carga manual | RADIO_NO_DEFINIDO (CU-07) | "Falta un radio de agrupación para esta carga." | Definir o esperar el radio aplicable; el conjunto no se procesa |
| Permisos del sistema operativo | PERMISO_ALMACENAMIENTO_DENEGADO (CU-07) | "Necesitamos acceso a tus fotos para cargarlas." | Conceder el permiso desde el sistema operativo |
| Carga manual | FORMATO_FOTO_NO_SOPORTADO (CU-07) | "No pudimos procesar una de las fotos por su formato; seguimos con el resto." | La foto se omite y se señala en el resultado; el resto continúa |
| Sincronización | BACKEND_INALCANZABLE (CU-06) | "No pudimos conectar con el servidor. Tu trabajo está guardado y lo reintentamos solos." | Reintento automático al volver la red; la cola se conserva |
| Sincronización | TOKEN_INVALIDO (CU-06) | "Tu sesión caducó. Reingresá para seguir sincronizando; tu trabajo está a salvo." | Reloguear (CU-01); la cola se conserva intacta |
| Sincronización | RELEVAMIENTO_CERRADO en subida (CU-06) | "El relevamiento se cerró. Tus cambios quedan guardados, pero ya no se pueden subir." | La cola se conserva; el agente queda informado; resolución del lado web |

Tono y voz: cercano, directo, en segunda persona del singular rioplatense, sin tecnicismos ni reproches. Se prioriza decirle al agente que su trabajo está a salvo y qué puede hacer ahora. El handoff aplica a accesos caducados (reloguear) y a la resolución de conflictos y de relevamientos cerrados, que se difiere al jefe en la web; la app informa y conserva el trabajo, no lo descarta.

## 9. Trazabilidad

Trazabilidad upstream (persona de 00, CU de 02, RN de 02) y downstream (wireframes de esta sección, US a generar en 06, tests previstos en 08).

| Flujo / superficie | Persona objetivo (00) | CU origen (02) | RN aplicables (02) | Wireframe asociado (03) | US a generar (06) | Tests previstos (08) |
| --- | --- | --- | --- | --- | --- | --- |
| Sesión y seguridad del dispositivo | Agente de campo | CU-01 | RN-04 | wireframes-pantalla-login-relogueo_v1.0.md | US-01, US-02 | Inicio online guarda token en almacén seguro; relogueo sin credenciales; deslogueo borra sesión; inicio sin conexión rechazado; cambio de usuario exige deslogueo |
| Elegir relevamiento de trabajo | Agente de campo | CU-02 | RN-05, RN-02 | wireframes-lista-relevamientos-asignados_v1.0.md | US-03, US-04 | Selección fija contexto y abre mapa local; lista vacía sin conexión rechazada; refresco con conexión agrega asignación; relevamiento cerrado en solo lectura |
| Georreferenciar y crear o mover marcador | Agente de campo | CU-03 | RN-03, RN-05 | wireframes-mapa-captura_v1.0.md | US-05, US-06 | Centrar por GPS y crear marcador encolado; convivencia con conflicto por radio; sin señal de GPS ofrece fijación manual; permiso denegado ofrece fijación manual; mover marcador conserva identidad |
| Capturar foto con coordenada | Agente de campo | CU-04 | RN-01, RN-05 | wireframes-mapa-captura_v1.0.md | US-07, US-08 | Captura resuelve coordenada y ancla foto; captura sin GPS deja pendiente de ubicación; permiso de cámara denegado bloquea captura; marcador compartido por varias observaciones |
| Enriquecer evidencia | Agente de campo | CU-05 | RN-05, RN-03 | wireframes-detalle-observacion_v1.0.md | US-09, US-10 | Comentario y etiqueta registrados y encolados; etiqueta reutilizada sin duplicar; comentario sobre foto sin ubicación; etiqueta vacía rechazada |
| Trabajar sin conexión y sincronizar | Agente de campo | CU-06 | RN-02, RN-03, RN-05 | wireframes-estado-sincronizacion_v1.0.md | US-11, US-12, US-13 | Orden subir-antes-de-bajar verificado; corte en subida deja reanudable sin duplicar; convivencia con conflicto en la bajada; token rechazado conserva la cola |
| Carga manual de fotos | Agente de campo | CU-07 | RN-01, RN-03 | wireframes-mapa-captura_v1.0.md (acción de carga) y wireframes-detalle-observacion_v1.0.md (resultado) | US-14, US-15 | Agrupación dentro del radio; marcador nuevo para foto lejana; foto sin ubicación queda pendiente; carga sin radio rechazada |

Concepto transversal: el indicador de conectividad y de estado de sincronización (sin conexión, cambios en cola, sincronizando, al día, en conflicto) es un patrón persistente reutilizado por todos los wireframes; se describe en cada uno y se ancla en este marco. La marca de pendiente de ubicación de una foto se reutiliza entre el mapa de captura, el detalle de observación y la carga manual.

## 10. Notas y supuestos

- La app no es la fuente de verdad: el dominio autoritativo es el de geovial-api y el almacén local es una réplica parcial para trabajo offline (02 §1, modelo conceptual §0). La experiencia se diseña sobre la premisa de capturar localmente, encolar y dejar que la sincronización reconcilie con el backend.
- El offline-first es el modo normal de la app, no una degradación: la captura nunca espera por la red. El estado sin conexión es esperado y se comunica con calma, asegurando al agente que su trabajo está a salvo (RN-05).
- La mecánica de la sincronización (detección de conectividad, orden subir-luego-bajar, idempotencia, reanudación) vive en la librería de sincronización y en el backend; la app la presenta como estado simple y no la gobierna (RN-02, 02 §2).
- La app no resuelve conflictos de marcadores: convive con ellos, los mantiene accesibles y los reporta; la resolución se difiere al cierre en la web (RN-03). La experiencia de resolución no pertenece a esta sección.
- El detalle visual fino (paleta, tipografía, espaciados, componentes concretos, render del mapa, estilo de los pines y gestos táctiles exactos) no se decide aquí: vive en 05 o en el design system. Este marco fija comportamiento, jerarquía, estados y ergonomía táctil.
- Permisos del sistema operativo: la app solicita ubicación, cámara y acceso al almacenamiento o galería solo cuando la función los necesita por primera vez, con microcopy que justifica cada uno; ante denegación o revocación, degrada con la vía alternativa correspondiente (CU-03, CU-04, CU-07). El relogueo usa la seguridad del dispositivo (CU-01, RN-04).
- Plataforma: la app se distribuye para una única plataforma móvil en la primera versión (compatibilidad-plataformas §2); el diseño asume las convenciones de esa plataforma móvil, sin nombrarla como stack, y opera en portrait como orientación primaria de campo.
- Supuestos heredados de 02 §9, sin redefinir: foto cargada sin metadatos de ubicación (pendiente de ubicación manual sin inventar coordenada), captura sin señal de GPS (marcador manual y foto pendiente de ubicación), pérdida de conexión en subida parcial (reanudación idempotente sin pérdida ni duplicación), cierre del relevamiento con cambios sin sincronizar (el backend bloquea nuevas subidas, la app conserva la cola e informa) y conflictos entre dos agentes (misma política de convivencia y resolución al cierre). A confirmar con el negocio; no bloquean el marco de experiencia.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Marco de experiencia inicial de la app de campo geovial-mobile (variante UX/UI móvil): audiencia única del agente de campo con contexto en terreno (guantes, sol, movilidad, conectividad variable, dispositivo compartido), principios de diseño con heurísticas de Nielsen y leyes UX para uso táctil, siete flujos clave anclados a los siete CU con captura offline como eje, estados y feedback con sin conexión y sincronizando como estados de primera clase, accesibilidad WCAG 2.2 AA reforzada para exteriores y uso con guantes, internacionalización, performance percibida que nunca bloquea la captura, taxonomía de errores con vías de recuperación y reflejo de los permisos del sistema operativo, y trazabilidad upstream y downstream. Ancla los cinco wireframes mínimos del tipo mobile-app-maui. |
