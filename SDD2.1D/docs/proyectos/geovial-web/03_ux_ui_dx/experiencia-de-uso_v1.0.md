# Experiencia de uso — geovial-web

**Proyecto:** geovial-web
**Documento:** experiencia-de-uso_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** UX/UI Designer + Frontend Lead
**Variante:** UX/UI

## 0. Propósito y posición en la cadena

Este documento es el marco de experiencia del front web `geovial-web`, la herramienta de los roles administradores de la solución GeoVial. Recibe insumos de 00 (visión y personas) y de 02 (los once casos de uso con interacción humana y las cinco reglas de negocio de presentación) y produce el ancla de experiencia que los wireframes de esta sección materializan, que 06 convierte en criterios de aceptación visuales y que 08 traduce en tests de interfaz y de accesibilidad.

El marco define cómo se siente el front: audiencia, principios de diseño, flujos, estados, accesibilidad, internacionalización, performance percibida, errores y recuperación. No define el qué funcional (vive en 02) ni el cómo técnico de la capa de presentación (vive en 05). No incluye colores, tipografías ni decisiones de interfaz fina; tampoco menciona stacks concretos: cuando se habla de "componente de mapa", "tabla de datos", "carrusel", "formulario" o "modal" se describe el rol del componente, no su implementación.

## 1. Audiencia y contexto de uso

La audiencia del front web es la jerarquía administradora de tres niveles que opera en navegador. El agente de campo no es audiencia del front salvo por la excepción acotada de la carga manual (CU-09), y su herramienta principal es la aplicación de campo, fuera del alcance de esta sección.

| Persona | Origen en 00 | Contexto físico | Contexto emocional | Frecuencia y duración | Competencia digital esperada |
| --- | --- | --- | --- | --- | --- |
| Jefe de área | Visión §2 (rol del sistema, nivel operativo) | Escritorio de oficina, monitor amplio, conexión de oficina estable | Foco en avanzar el trabajo de campo y cerrar informes a tiempo; carga cognitiva alta al revisar evidencia | Uso diario, sesiones de media a larga duración durante la revisión y el cierre | Media; usa herramientas administrativas de gestión, no necesariamente técnicas |
| Jefe general | Visión §2 (administra jefes de área) | Escritorio de oficina | Supervisión; menos operación directa, más administración de personas | Uso esporádico, sesiones cortas y dirigidas | Media |
| Usuario raíz | Visión §2 (acceso pleno, configura el sistema) | Escritorio de oficina | Configuración y puesta a punto; cautela ante cambios de alcance global | Uso ocasional, sesiones puntuales de configuración | Alta |
| Agente de campo (excepción) | Visión §2 (operación en terreno) | Puede entrar al front desde un equipo de oficina al regresar del campo | Apuro por dejar la evidencia cargada; cansancio tras la jornada | Excepcional, solo carga manual de su relevamiento asignado (CU-09) | Media a baja |

Notas de contexto:

- La persona primaria del front es el jefe de área: concentra nueve de los once CU y los flujos de mayor carga cognitiva (revisión sobre mapa, resolución de conflictos, cierre). El diseño prioriza su jornada.
- El contexto del front es de oficina con conexión razonablemente estable. La tolerancia a la pérdida de conexión existe pero no es el modo de operación normal: el offline-first es de la aplicación de campo, no del front. El front debe degradar con dignidad ante una caída de red, no operar offline.
- El jefe general y el usuario raíz usan superficies acotadas (administración de usuarios y configuración) con sesiones breves; su prioridad es claridad y prevención de errores de alto impacto.

## 2. Principios de diseño

Se seleccionan las heurísticas de Nielsen y las leyes UX pertinentes a este front, con justificación de aplicación al producto. Cada principio se verifica por inspección heurística de dos revisores antes de pasar a 08.

| Heurística de Nielsen | Aplicación en geovial-web | Verificación |
| --- | --- | --- |
| Visibilidad del estado del sistema | El estado del ciclo del relevamiento (recolección, revisión, cierre) se muestra siempre y de forma consistente en el listado y en cada pantalla del relevamiento; la sincronización de evidencia y la carga de fotos muestran progreso (RN-04). | Inspección heurística; test de snapshot del indicador de estado en cada vista (08). |
| Correspondencia entre el sistema y el mundo real | El vocabulario de pantalla usa los términos del dominio que el usuario reconoce (relevamiento, marcador, tramo, conflicto, etiqueta) sin jerga técnica de implementación. | Revisión de microcopy contra el glosario de 02 y el glosario-ux de esta sección. |
| Control y libertad del usuario | Toda acción destructiva o de avance de ciclo (dar de baja, transicionar de estado, unificar marcadores, cerrar) es confirmable y, cuando el backend lo permite, reversible: la resolución de un conflicto se puede reabrir antes del cierre (CU-07 5.A); el cierre admite reapertura si el backend la habilita (CU-08). | Test de flujo de reapertura y de cancelación de confirmaciones (08). |
| Consistencia y estándares | El mismo patrón de listado con filtros, el mismo patrón de formulario y el mismo patrón de modal de confirmación se reutilizan entre superficies; el carrusel encadenado es un concepto único reutilizado (ver `representacion-carrusel-fotos_v1.0.md`). | Auditoría de consistencia de patrones entre wireframes. |
| Prevención de errores | El front oculta o deshabilita las acciones inválidas para el estado vigente y el rol (RN-01, RN-04): no se ofrece editar la composición de un tramo fuera de recolección, ni cerrar con conflictos pendientes (RN-05), ni se muestran pantallas fuera del alcance del rol. | Test de habilitación por estado y por rol (08, sobre CU-03, CU-07, CU-08, CU-11). |
| Reconocer antes que recordar | El listado de relevamientos, la lista de conflictos pendientes y el carrusel exponen la información en contexto; el usuario no recuerda identificadores ni navega a ciegas. | Inspección heurística sobre las tres superficies clave. |
| Flexibilidad y eficiencia de uso | Filtros por estado y por etiqueta, búsqueda por nombre de tramo y encadenamiento de marcadores en el carrusel aceleran al usuario frecuente sin estorbar al ocasional (CU-03 5.B, CU-06). | Pruebas de usabilidad con jefe de área. |
| Diseño estético y minimalista | Cada pantalla muestra solo lo que el rol y el estado requieren; la densidad de la revisión sobre mapa se controla con paneles colapsables y filtros. | Inspección heurística de densidad de información. |
| Ayudar a reconocer, diagnosticar y recuperarse de errores | Cada error visible dice qué pasó, por qué y qué hacer (ver §8); ningún mensaje es un código técnico crudo. | Auditoría de mensajes contra la taxonomía de §8. |
| Ayuda y documentación | Estados vacíos con texto orientativo y acción siguiente; microcopy de ayuda contextual en formularios y en la confirmación de cierre. | Revisión de estados vacíos de cada wireframe. |

Leyes UX aplicadas:

| Ley UX | Aplicación en geovial-web | Justificación |
| --- | --- | --- |
| Ley de Hick | El menú principal expone solo las superficies del rol vigente; las transiciones de estado se reducen a las válidas desde el estado actual (CU-08). | Menos opciones simultáneas reducen el tiempo de decisión del jefe en pantallas de alta frecuencia. |
| Ley de Fitts | Las acciones primarias (asignar, crear, resolver, cerrar) son destinos grandes y consistentes; los controles del carrusel (anterior, siguiente, ampliar) son amplios y de ubicación fija. | El uso es de escritorio con cursor; objetivos grandes y estables aceleran la operación repetitiva del carrusel. |
| Ley de Miller | El carrusel y la lista de conflictos paginan o agrupan para no exigir retener muchos elementos a la vez; los filtros reducen el conjunto visible. | La revisión maneja muchas fotos y marcadores; agrupar evita sobrecarga de memoria de trabajo. |
| Ley de Jakob | El front reutiliza patrones conocidos de aplicaciones administrativas web (listado con filtros, formulario, mapa con marcadores y panel lateral, modal de confirmación). | La audiencia conoce herramientas administrativas; respetar convenciones reduce la curva de aprendizaje (mitiga R-01 de 00). |

## 3. Flujos clave

Cada flujo es un user journey con disparador, pasos, puntos de fricción anticipados y salida. Los flujos se anclan a los CU de 02.

### 3.1 Ingreso y cierre de sesión (CU-01)

- Disparador: un administrador abre el front para trabajar.
- Pasos: entrega identificador y credencial; el front abre la sesión y habilita solo las superficies de su rol; al terminar, cierra la sesión y vuelve al ingreso sin rastros de la identidad anterior.
- Puntos de fricción anticipados: credenciales inválidas, usuario inhabilitado que conserva traza, rol sin acceso web (agente fuera de la carga manual), backend no disponible, sesión expirada durante el uso. Cada uno se resuelve con un mensaje accionable (§8) y nunca deja una sesión a medias.
- Salida: sesión activa con superficies del rol, o retorno al ingreso con motivo claro.

### 3.2 Administración de usuarios por jerarquía (CU-02)

- Disparador: un administrador necesita dar de alta, editar o dar de baja a un usuario del nivel inmediato inferior.
- Pasos: lista los usuarios de su alcance; crea o edita uno; al dar de baja confirma la acción y entiende que la baja inhabilita el acceso pero conserva la autoría visible (RN-02).
- Puntos de fricción anticipados: confusión entre baja y borrado de evidencia; el front comunica explícitamente que la autoría se conserva. El rol del nivel no inmediato no se ofrece (RN-01).
- Salida: el conjunto de usuarios del alcance queda actualizado y la traza de autoría intacta.

### 3.3 Planificación del trabajo: crear, editar y listar relevamientos (CU-03)

- Disparador: el jefe de área planifica un nuevo relevamiento o consulta los existentes.
- Pasos: abre el listado con el estado del ciclo de cada relevamiento; crea uno indicando nombre y composición del tramo (puentes y caminos); edita la composición solo si está en recolección; da de baja con confirmación; filtra por estado y busca por nombre.
- Puntos de fricción anticipados: intentar crear con tramo vacío (bloqueado con mensaje claro); intentar editar fuera de recolección (vista en solo lectura, RN-04); listas largas (filtro y búsqueda delegados al backend).
- Salida: el relevamiento queda en el estado correcto y visible en el listado.

### 3.4 Asignación de agentes (CU-04)

- Disparador: el jefe de área asigna o reasigna agentes a un relevamiento en recolección.
- Pasos: abre la sección de agentes del relevamiento; asigna o reasigna; el front refleja el resultado.
- Puntos de fricción anticipados: asignación fuera de estado válido o fuera de alcance del rol, resuelta por habilitación previa y mensaje ante rechazo del backend.
- Salida: la lista de asignados queda actualizada.

### 3.5 Marcadores iniciales sobre el mapa (CU-05)

- Disparador: el jefe de área deja marcadores de referencia antes del trabajo de campo, en recolección.
- Pasos: sobre el componente de mapa, fija un marcador, lo mueve y lo etiqueta.
- Puntos de fricción anticipados: ubicación imprecisa (se permite mover el punto); acción deshabilitada fuera de recolección.
- Salida: los marcadores iniciales quedan sobre el mapa del relevamiento.

### 3.6 Revisión sobre mapa con carrusel de fotos (CU-06) — flujo principal

- Disparador: el jefe de área revisa la evidencia sincronizada de un relevamiento en revisión para confeccionar el informe.
- Pasos: abre el relevamiento sobre el mapa; el front presenta los marcadores con su evidencia; selecciona un marcador y abre el carrusel de sus fotos con comentarios; avanza y retrocede, y al llegar al extremo encadena con el marcador contiguo; amplía una foto y lee comentario y etiqueta; filtra por etiqueta para acotar el conjunto.
- Puntos de fricción anticipados: marcador sin fotos (ofrecer pasar al contiguo); foto no recuperable del almacén (marcador de foto no disponible y continuar el carrusel); filtro sin coincidencias (avisar y permitir limpiar); presencia de conflictos (mostrar toda la evidencia sin bloquear y señalar que hay conflictos pendientes, RN-05); evidencia no disponible momentáneamente (ofrecer reintentar). La autoría de cada registro permanece visible aun si su autor fue dado de baja (RN-02).
- Salida: el jefe tiene el panorama de la evidencia ordenado por marcador y ubicación; el estado del relevamiento no cambia por revisar.

### 3.7 Resolución de conflictos al cierre (CU-07)

- Disparador: el jefe de área, con el relevamiento en revisión, debe resolver los conflictos de marcadores antes de cerrar.
- Pasos: abre la lista de conflictos pendientes; selecciona uno y compara sobre el mapa los marcadores involucrados con su evidencia; decide unificar o mantener separados; al unificar marcadores con etiquetas distintas, el front advierte que el resultante conservará la unión de etiquetas; puede reabrir una decisión antes del cierre.
- Puntos de fricción anticipados: relevamiento fuera de revisión (solo lectura, RN-04); conflicto ya resuelto o inexistente (se retira de la lista y se refresca); decisión equivocada (reversible antes del cierre).
- Salida: la lista de conflictos pendientes se vacía y queda habilitado el cierre (RN-05).

### 3.8 Transición de estado y cierre (CU-08)

- Disparador: el jefe de área hace avanzar el ciclo del relevamiento o lo cierra.
- Pasos: ve el estado vigente y solo las transiciones válidas; pasa de recolección a revisión, o devuelve de revisión a recolección si falta evidencia; al cerrar, el front verifica que no haya conflictos pendientes y, si los hay, bloquea el cierre y deriva a la resolución (CU-07).
- Puntos de fricción anticipados: intento de cierre con conflictos (bloqueado con derivación clara, RN-05); transición no válida desde el estado actual (no se ofrece); cambio de estado en otra sesión (el front recarga el estado vigente).
- Salida: el relevamiento queda en el nuevo estado o cerrado y habilitado para el informe.

### 3.9 Carga manual de un relevamiento vía web (CU-09)

- Disparador: el agente asignado, de vuelta en oficina, carga manualmente la evidencia de su relevamiento en recolección.
- Pasos: ingresa al front solo a esta superficie (RN-03); sube fotos que el backend agrupa por ubicación y radio; las fotos sin datos de ubicación se presentan como pendientes de ubicación manual sin inventar coordenada; completa notas, comentarios y etiquetas.
- Puntos de fricción anticipados: foto sin ubicación incrustada (pendiente de ubicación manual); acción deshabilitada fuera de recolección; agente sin acceso a otras superficies.
- Salida: la evidencia queda cargada y disponible para la revisión del jefe.

### 3.10 Portabilidad y configuración (CU-10, CU-11)

- Disparador: el jefe de área o el raíz exporta o importa un relevamiento completo (CU-10, Could Have); el raíz configura el destino de almacenamiento (CU-11).
- Pasos: dispara la exportación o la importación de un archivo único; o selecciona el destino de almacenamiento de archivos.
- Puntos de fricción anticipados: operaciones largas (progreso y estado claro); pantalla de configuración no disponible para roles distintos del raíz (RN-01).
- Salida: el relevamiento queda exportado o importado; o el destino vigente queda configurado.

## 4. Estados y feedback

Mapa de estados por superficie clave. El feedback visual se describe por su rol (banner, inline, skeleton, confirmación sutil), sin colores ni tipografías. Estos estados son piso para los wireframes de la sección y para los snapshot tests de 08.

### 4.1 Tabla general de estados

| Estado | Condición que lo produce | Feedback visual | Feedback textual |
| --- | --- | --- | --- |
| Vacío | No hay datos para mostrar todavía (sin relevamientos, sin conflictos, marcador sin fotos) | Ilustración o ícono neutro y CTA de acción siguiente | Texto orientativo que explica el vacío y propone el próximo paso |
| Cargando | Operación asíncrona en curso contra el backend | Skeleton de la estructura esperada; spinner solo si la espera supera el umbral percibido | Indicación de qué se está cargando (listado, evidencia, fotos) |
| Con datos | El backend respondió con contenido | Render normal de la superficie | — |
| Error recuperable | Falla transitoria o de validación con vía de recuperación | Banner o inline según el alcance del error | Causa probable y acción de recuperación (reintentar, corregir) |
| Sin conexión al circuito | El front no alcanza el backend o la sesión expiró | Banner persistente no intrusivo | Aviso de servicio no disponible o sesión expirada, con acción de reintento o reingreso |
| Éxito | Acción completada (crear, asignar, resolver, transicionar, cerrar) | Confirmación visual sutil | Confirmación breve con la próxima acción posible |

### 4.2 Estados por superficie clave

| Superficie | Vacío | Cargando | Con datos | Error / sin conexión |
| --- | --- | --- | --- | --- |
| Pantalla de ingreso (CU-01) | Formulario limpio con campos de identificador y credencial | Indicador de validación de credenciales en curso, controles deshabilitados | Sesión abierta: redirige a la superficie inicial del rol | Credenciales inválidas o usuario inhabilitado (inline en el formulario); servicio no disponible (banner, conserva el identificador para reintentar); rol sin acceso web (mensaje y retorno al ingreso) |
| Panel de relevamientos (CU-03) | Sin relevamientos: ilustración y CTA "crear el primer relevamiento" | Skeleton de filas de la tabla de datos | Tabla con estado del ciclo, filtros por estado y búsqueda por tramo | Falla al listar: banner con reintento; sesión expirada: aviso y reingreso |
| Revisión sobre mapa con carrusel (CU-06) | Relevamiento sin evidencia sincronizada: aviso de que aún no hay evidencia para revisar | Skeleton del mapa y del panel de marcadores; spinner en el carrusel al traer fotos | Mapa con marcadores, panel de evidencia y carrusel encadenado | Evidencia no disponible (banner con reintento); foto no recuperable (marcador de foto no disponible inline, continúa el carrusel); conflictos presentes (aviso no bloqueante) |
| Resolución de conflictos al cierre (CU-07) | Sin conflictos pendientes: estado resuelto y cierre habilitado | Skeleton de la lista de conflictos | Lista de conflictos con comparador de marcadores sobre el mapa | Conflicto inexistente o ya resuelto (se retira y refresca); fuera de revisión (solo lectura); fuera de alcance (mensaje) |

## 5. Accesibilidad

Compromiso explícito: WCAG 2.2 nivel AA como piso mínimo en todo el front. No se aceptan menciones genéricas; se enumeran los criterios prioritarios y dónde se aplican. Las menciones a versiones anteriores de WCAG, si las hubiera, son solo histórico evolutivo y no rebajan el piso.

Criterios prioritarios:

| Criterio WCAG 2.2 AA | Aplicación en geovial-web | Dónde se verifica |
| --- | --- | --- |
| Contraste de texto (1.4.3, 4.5:1) y de componentes no textuales (1.4.11, 3:1) | Texto, controles del carrusel, indicadores de estado y marcadores del mapa con contraste suficiente; el estado del ciclo no se comunica solo por color (1.4.1 uso del color), también por etiqueta de texto e ícono | Test de contraste automatizado y manual (08) |
| Foco visible (2.4.7) y foco no oscurecido (2.4.11, novedad 2.2) | Indicador de foco visible en todos los controles; en el modal de carrusel y en los modales de confirmación el elemento enfocado nunca queda tapado por overlays | Navegación por teclado y snapshot de foco (08) |
| Navegación completa por teclado (2.1.1) y sin trampa de teclado (2.1.2) | Toda la operación (listar, filtrar, abrir carrusel, avanzar y retroceder, ampliar, resolver conflictos, confirmar y cerrar) es operable por teclado; el carrusel y los modales devuelven el foco al cerrarse | Recorrido solo-teclado de los cuatro flujos clave (08) |
| Etiquetas semánticas y nombres de campo (1.3.1, 4.1.2, 3.3.2) | Formularios de ingreso, alta de usuario y creación de relevamiento con etiquetas asociadas a cada campo; tablas de datos con encabezados semánticos | Test de roles y nombres accesibles (08) |
| Alternativas textuales (1.1.1) | Cada foto del carrusel expone su comentario y etiqueta como texto alternativo accesible; el marcador de foto no disponible se anuncia como tal; los íconos de estado tienen nombre accesible | Auditoría de alternativas textuales del carrusel (08) |
| Anuncio de cambios dinámicos (4.1.3 mensajes de estado) | El avance del carrusel, el resultado de una resolución de conflicto, la confirmación de cierre y los banners de error se anuncian por región de estado para lectores de pantalla | Test con lector de pantalla sobre CU-06 y CU-07 (08) |
| Tamaño de objetivo (2.5.8, novedad 2.2) | Los controles del carrusel y las acciones primarias cumplen el tamaño mínimo de objetivo; relevante por el uso repetitivo del carrusel | Inspección de tamaño de objetivo |
| Ayuda consistente (3.2.6, novedad 2.2) y entrada redundante (3.3.7, novedad 2.2) | La vía de ayuda y reintento se ubica de forma consistente; no se pide reingresar datos ya provistos en el mismo flujo (por ejemplo, el identificador se conserva tras un fallo de servicio en el ingreso) | Inspección heurística y test de flujo de reintento |

Los criterios novedad 2.2 (foco no oscurecido, tamaño de objetivo, ayuda consistente, entrada redundante) se listan explícitamente para dejar claro que el piso es 2.2 AA y no una versión anterior.

## 6. Internacionalización

- Idioma de partida: español rioplatense. El front se diseña con el texto externalizado para admitir la incorporación de otros idiomas sin rediseño, aunque hoy no hay un segundo idioma comprometido por el negocio (00 no declara multi-idioma).
- Expansión de texto: se prevé hasta un 35 por ciento de expansión respecto del español para idiomas más extensos; los rótulos de botones, las etiquetas y los encabezados de tabla no se truncan y los layouts no se rompen ante texto más largo. Las etiquetas del dominio (creadas por el usuario) pueden ser de longitud variable y deben truncarse con indicación visual y texto completo accesible.
- Dirección de lectura: izquierda a derecha. Se evita acoplar la lógica del carrusel a la dirección de lectura, de modo que un futuro idioma de derecha a izquierda solo requiera reflejar la disposición.
- Formatos de fecha, hora y número: fechas en formato YYYY-MM-DD en los artefactos de la sección; en pantalla, formato localizable según la configuración regional, sin asumir un formato fijo en el código. Las coordenadas geográficas se presentan con separador decimal coherente con la configuración regional.
- Las fotos y su evidencia no se traducen; los comentarios y etiquetas son contenido del usuario y se muestran tal cual los cargó.

## 7. Performance percibida

Tiempos máximos tolerables por acción y técnicas de percepción. Los tiempos son objetivos de experiencia, no presupuesto técnico (que vive en 05).

| Acción | Tolerancia percibida | Técnica de performance percibida |
| --- | --- | --- |
| Ingreso y apertura de la superficie del rol | Respuesta inmediata al enviar; superficie utilizable en pocos segundos | Indicador de validación; deshabilitar el envío para evitar reenvíos |
| Listar relevamientos | Sensación de respuesta casi inmediata | Skeleton de filas mientras llega el listado; paginación delegada al backend |
| Abrir la revisión sobre mapa | El mapa y los marcadores aparecen progresivamente | Skeleton del mapa y del panel; render incremental de marcadores; spinner solo si supera el umbral |
| Navegar el carrusel (avanzar, retroceder, ampliar) | Respuesta inmediata entre fotos ya cargadas | Precarga de las fotos contiguas; placeholder mientras llega un binario; nunca bloquear el carrusel por una foto faltante |
| Aplicar filtro por etiqueta | Sensación inmediata | Optimistic UI en el filtrado local de lo ya cargado cuando es seguro; refresco del conjunto si el backend acota |
| Resolver un conflicto (unificar o separar) | Confirmación pronta | Estado de envío en el control; actualización de la lista de pendientes al confirmar |
| Transicionar de estado o cerrar | Confirmación pronta del nuevo estado | Estado de envío; bloqueo de doble disparo del cierre |
| Exportar o importar (CU-10) | Operación reconocidamente larga | Progreso explícito y mensaje de que la operación puede demorar; no simular instantaneidad |

Criterios de animación: animaciones breves y funcionales (transición entre fotos del carrusel, aparición de banners). Se respeta la preferencia de movimiento reducido del sistema operativo: si el usuario la activa, las transiciones se simplifican o se suprimen. Ninguna animación bloquea la interacción ni oculta el estado del sistema.

## 8. Errores y recuperación

Taxonomía de los errores que el usuario verá en el front, con el tono de los mensajes y la vía de recuperación. Principio de redacción: cada mensaje dice qué pasó, por qué pasó (cuando aporta) y qué hacer; lenguaje plano, sin culpar al usuario, sin códigos técnicos crudos en la superficie. Los códigos de la tabla provienen de los CU de 02 y son referencia interna y de trazabilidad con 08, no texto literal de pantalla.

| Categoría | Código de origen (02) | Qué ve el usuario | Vía de recuperación |
| --- | --- | --- | --- |
| Credenciales o acceso | CREDENCIALES_INVALIDAS (CU-01) | "Las credenciales no coinciden. Revisá tu identificador y tu credencial e intentá de nuevo." | Reintento inline en el formulario, conservando el identificador |
| Credenciales o acceso | USUARIO_INHABILITADO (CU-01) | "Tu acceso está revocado. Si creés que es un error, contactá a quien administra tu cuenta." | Handoff al administrador; no se abre sesión |
| Credenciales o acceso | ROL_SIN_ACCESO_WEB (CU-01) | "Tu rol opera desde la aplicación de campo y no tiene acceso a esta herramienta." | Cierre de la sesión recién abierta y retorno al ingreso |
| Validación de entrada | TRAMO_VACIO (CU-03) | "El tramo debe abarcar al menos un puente o un camino para poder crear el relevamiento." | Corrección inline; el formulario se conserva |
| Estado del recurso | RELEVAMIENTO_NO_EN_RECOLECCION (CU-03), RELEVAMIENTO_NO_EN_REVISION (CU-07) | "Este relevamiento ya no está en la etapa que permite esta acción. Te lo mostramos en solo lectura." | Vista en solo lectura; recarga del estado vigente |
| Estado del recurso | TRANSICION_NO_PERMITIDA (CU-08) | La transición no se ofrece; si el backend la rechaza: "Esa transición no está disponible desde el estado actual." | El front recarga el estado vigente y ofrece solo las transiciones válidas |
| Conflicto de estado | CONFLICTOS_PENDIENTES (CU-08) | "No se puede cerrar todavía: quedan conflictos de marcadores por resolver." | Derivación directa a la pantalla de resolución de conflictos (CU-07) |
| Conflicto de estado | CONFLICTO_INEXISTENTE (CU-07) | "Este conflicto ya fue resuelto." | Se retira de la lista y se refresca el listado de pendientes |
| Alcance / permisos | FUERA_DE_ALCANCE (CU-03, CU-06, CU-07, CU-08) | "Este elemento está fuera de tu alcance." | El recurso no se lista; ante rechazo del backend, mensaje y retorno a la lista del alcance |
| Recurso ausente | FOTO_NO_DISPONIBLE (CU-06) | "Esta foto no está disponible en este momento." | Marcador de foto no disponible inline; el carrusel continúa con el resto |
| Error transitorio | EVIDENCIA_NO_DISPONIBLE (CU-06) | "No pudimos traer la evidencia ahora. Probá de nuevo en un momento." | Botón de reintento |
| Sin conexión / sesión | Sesión expirada (CU-01 5.A), front sin alcance al backend (CU-01 5.B) | "Tu sesión expiró, ingresá de nuevo." / "El servicio no está disponible. Reintentá en unos segundos." | Reingreso o reintento; se conserva lo recuperable del contexto |

Tono y voz: cercano, directo, en segunda persona del singular rioplatense, sin tecnicismos ni reproches. Se prioriza decirle al usuario qué puede hacer ahora. El handoff humano aplica solo a accesos revocados, donde el front no puede resolver y deriva a quien administra la cuenta.

## 9. Trazabilidad

Trazabilidad upstream (persona de 00, CU de 02, RN de 02) y downstream (wireframes de esta sección, US a generar en 06, tests previstos en 08).

| Flujo / superficie | Persona objetivo (00) | CU origen (02) | RN aplicables (02) | Wireframe asociado (03) | US a generar (06) | Tests previstos (08) |
| --- | --- | --- | --- | --- | --- | --- |
| Ingreso y cierre de sesión | Jefe de área, jefe general, raíz (agente solo carga manual) | CU-01 | RN-01, RN-03 | wireframes-pantalla-login_v1.0.md | US-01, US-02 | Ingreso válido habilita el rol; credenciales inválidas; cierre no recupera sesión; rol sin acceso web rechazado |
| Administración de usuarios | Raíz, jefe general, jefe de área | CU-02 | RN-01, RN-02 | (cubierto por el marco; superficie no priorizada como wireframe mínimo) | US-03, US-04, US-05 | Listado acotado al alcance; baja conserva autoría |
| Panel de relevamientos | Jefe de área | CU-03 | RN-01, RN-04 | wireframes-panel-relevamientos_v1.0.md | US-06, US-07, US-08 | Creación en recolección visible; tramo vacío bloqueado; edición fuera de recolección en solo lectura; filtro por estado |
| Asignación de agentes | Jefe de área | CU-04 | RN-01, RN-04 | (cubierto por el marco) | US-09, US-10 | Asignación y reasignación; habilitación por estado |
| Marcadores iniciales | Jefe de área | CU-05 | RN-01, RN-02, RN-04 | (cubierto por el marco; mapa descrito en revisión) | US-11, US-12 | Crear y mover marcador solo en recolección |
| Revisión sobre mapa con carrusel | Jefe de área | CU-06 | RN-01, RN-02, RN-04, RN-05 | wireframes-revision-mapa-carrusel_v1.0.md | US-13, US-14, US-15 | Carrusel encadena al contiguo; filtro por etiqueta; evidencia accesible con conflictos; marcador sin fotos |
| Resolución de conflictos al cierre | Jefe de área | CU-07 | RN-05, RN-01, RN-04 | wireframes-resolucion-conflictos-cierre_v1.0.md | US-16, US-17 | Unificación reasigna evidencia; unión de etiquetas; fuera de revisión en solo lectura; reapertura antes del cierre |
| Transición de estado y cierre | Jefe de área | CU-08 | RN-05, RN-01 | wireframes-resolucion-conflictos-cierre_v1.0.md (sección de cierre) | US-18, US-19 | Transición recolección a revisión; cierre bloqueado con conflictos; cierre sin conflictos |
| Carga manual vía web | Agente de campo (excepción) | CU-09 | RN-04, RN-01, RN-03 | (cubierto por el marco; superficie de excepción) | US-20, US-21 | Foto sin ubicación como pendiente; carga solo en recolección |
| Portabilidad | Jefe de área, raíz | CU-10 | RN-01 | (cubierto por el marco; Could Have) | US-22, US-23 | Exportar e importar relevamiento completo |
| Configuración de almacenamiento | Raíz | CU-11 | RN-01 | (cubierto por el marco; solo raíz) | US-24, US-25 | Pantalla no disponible fuera del raíz |

Concepto transversal: el carrusel de fotos encadenado se documenta como representación reutilizable en `representacion-carrusel-fotos_v1.0.md` y lo invocan el wireframe de revisión y el de resolución de conflictos.

## 10. Notas y supuestos

- El front no posee persistencia ni invariantes de integridad: el estado de dominio y su validación viven en `geovial-api`. La experiencia se diseña sobre la premisa de consumir un contrato y reflejar fielmente lo que el backend habilita o rechaza (02 §1, modelo conceptual §5).
- El offline-first no es del front: el contexto normal es de oficina con conexión. El estado "sin conexión al circuito" cubre la degradación digna ante una caída de red o expiración de sesión, no un modo de trabajo offline. El trabajo offline pertenece a la aplicación de campo, fuera de esta sección.
- El detalle visual fino (paleta, tipografía, espaciados, componentes concretos) no se decide aquí: vive en 05 o en el design system. Este marco fija comportamiento, jerarquía y estados.
- Supuestos heredados de 02 §9, sin redefinir: conflictos entre cambios de dos agentes (misma política de convivencia y resolución al cierre), cierre con cambios locales sin sincronizar (el backend bloquea nuevas subidas al cerrar) y foto sin ubicación en la carga manual (pendiente de ubicación manual sin inventar coordenada). A confirmar con el negocio; no bloquean el marco de experiencia.
- Las superficies no priorizadas como wireframe mínimo (administración de usuarios, asignación, marcadores iniciales, carga manual, portabilidad, configuración) quedan cubiertas por este marco y por sus CU; pueden sumarse wireframes propios en una versión posterior si la cobertura lo exige (el mínimo del tipo es piso, no techo).

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Marco de experiencia inicial del front web geovial-web (variante UX/UI): audiencia de los tres roles administradores más la excepción del agente, principios de diseño con heurísticas de Nielsen y leyes UX, diez flujos clave anclados a los once CU, estados y feedback por superficie, accesibilidad WCAG 2.2 AA con criterios prioritarios, internacionalización, performance percibida, taxonomía de errores y trazabilidad upstream y downstream. Ancla los cuatro wireframes mínimos del tipo web-monolith. |
