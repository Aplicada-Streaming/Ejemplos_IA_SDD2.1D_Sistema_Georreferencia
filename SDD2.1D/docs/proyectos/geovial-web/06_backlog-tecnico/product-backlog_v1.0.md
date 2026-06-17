# Product backlog — geovial-web

**Proyecto:** geovial-web
**Documento:** product-backlog_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Scrum Master

## 1. Objetivos del producto

El propósito de este backlog es entregar el front web de los roles administradores de la solución: una herramienta que permita ingresar con sesión, administrar usuarios por jerarquía, gestionar y asignar relevamientos, crear marcadores iniciales sobre un mapa, revisar la evidencia sobre mapa con un carrusel de fotos, resolver conflictos al cierre y cerrar el relevamiento, con carga manual de evidencia y capacidades de portabilidad y configuración como extras. El MVP buscado cubre el camino principal del relevamiento de extremo a extremo (ingreso, administración, gestión, asignación, marcadores, revisión, resolución y cierre); la portabilidad y la configuración de almacenamiento quedan como capacidades posteriores que agregan valor sin integrar ese camino principal. El front no es dueño del dominio: toda lectura y escritura de datos se hace por contrato contra el servicio de dominio, que es la fuente de verdad.

## 2. Épicas

Las épicas se organizan por capacidad funcional del producto. Cada épica agrupa las historias que materializan una capacidad observable por el usuario administrador y traza a uno o más casos de uso de la especificación funcional.

| Épica | Nombre | Descripción breve | CU cubiertos | Sprints estimados |
| --- | --- | --- | --- | --- |
| EP-01 | Acceso y administración de usuarios | Ingreso y cierre de sesión y administración de usuarios por jerarquía, con visibilidad acotada por rol | CU-01, CU-02 | 1 a 2 |
| EP-02 | Gestión de relevamientos y marcadores | Crear, editar, listar y dar de baja relevamientos sobre un tramo, y crear y ubicar marcadores iniciales sobre el mapa | CU-03, CU-05 | 1 a 2 |
| EP-03 | Asignación de agentes | Asignar, reasignar y quitar agentes de campo de un relevamiento, conservando lo recolectado | CU-04 | 1 |
| EP-04 | Revisión sobre mapa con carrusel | Recorrer marcadores sobre el mapa, navegar el carrusel encadenado de fotos y filtrar la evidencia por etiqueta | CU-06 | 1 a 2 |
| EP-05 | Resolución de conflictos y cierre | Resolver los conflictos de marcadores pendientes y transicionar el estado del relevamiento hasta el cierre | CU-07, CU-08 | 1 a 2 |
| EP-06 | Carga manual de evidencia vía web | Carga manual completa de un relevamiento por el agente desde el front, con radio de agrupación y edición de evidencia | CU-09 | 1 |
| EP-07 | Portabilidad y configuración | Exportar e importar un relevamiento completo y configurar el destino de almacenamiento de archivos | CU-10, CU-11 | 1 |

## 3. Historias por épica

Estimación en story points con técnica Fibonacci (1, 2, 3, 5, 8, 13). Estado inicial Borrador para todas las historias de la versión 1.0 del backlog; el paso a Ready depende de la Definition of Ready (`definition-of-ready_v1.0.md`).

### EP-01 — Acceso y administración de usuarios

| US | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-01 | Ingresar al front con credenciales y obtener una sesión con rol | Must | 5 | Borrador | CU-01 | EP-01 |
| US-02 | Cerrar la sesión y dejar el acceso liberado | Must | 3 | Borrador | CU-01 | EP-01 |
| US-03 | Listar y dar de alta usuarios del nivel inmediato inferior | Must | 5 | Borrador | CU-02 | EP-01 |
| US-04 | Dar de baja un usuario conservando su autoría visible | Should | 3 | Borrador | CU-02 | EP-01 |

### EP-02 — Gestión de relevamientos y marcadores

| US | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-05 | Crear y listar relevamientos sobre un tramo vial | Must | 5 | Borrador | CU-03 | EP-02 |
| US-06 | Editar y dar de baja un relevamiento según su estado | Should | 3 | Borrador | CU-03 | EP-02 |
| US-07 | Crear y ubicar marcadores iniciales sobre el mapa | Must | 8 | Borrador | CU-05 | EP-02 |

### EP-03 — Asignación de agentes

| US | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-08 | Asignar agentes de campo a un relevamiento | Must | 5 | Borrador | CU-04 | EP-03 |
| US-09 | Reasignar y quitar agentes conservando lo recolectado | Should | 3 | Borrador | CU-04 | EP-03 |

### EP-04 — Revisión sobre mapa con carrusel

| US | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-10 | Recorrer marcadores y navegar el carrusel encadenado de fotos | Must | 8 | Borrador | CU-06 | EP-04 |
| US-11 | Filtrar la evidencia por etiqueta durante la revisión | Should | 3 | Borrador | CU-06 | EP-04 |

### EP-05 — Resolución de conflictos y cierre

| US | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-12 | Resolver un conflicto de marcadores unificando o separando | Must | 8 | Borrador | CU-07 | EP-05 |
| US-13 | Transicionar el estado del relevamiento por su ciclo | Must | 5 | Borrador | CU-08 | EP-05 |
| US-14 | Cerrar el relevamiento solo sin conflictos pendientes | Must | 5 | Borrador | CU-08, CU-07 | EP-05 |

### EP-06 — Carga manual de evidencia vía web

| US | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-15 | Cargar fotos con agrupación por radio desde el front | Must | 8 | Borrador | CU-09 | EP-06 |
| US-16 | Completar comentarios, etiquetas y ubicación manual de la evidencia | Should | 5 | Borrador | CU-09 | EP-06 |

### EP-07 — Portabilidad y configuración

| US | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-17 | Exportar e importar un relevamiento completo | Could | 5 | Borrador | CU-10 | EP-07 |
| US-18 | Configurar el destino de almacenamiento de archivos | Could | 3 | Borrador | CU-11 | EP-07 |

### Detalle de historias Must y Should con criterios de aceptación

Cada historia Must y Should declara su bloque de historia, su trazabilidad y al menos dos escenarios Given/When/Then (un happy path y un edge case), de acuerdo con la regla 06 §4.8. Las historias Could se documentan con su intención y un escenario de referencia; se refinan al promoverse.

#### US-01 — Ingresar al front con credenciales y obtener una sesión con rol

Como usuario administrador, quiero ingresar al front con mis credenciales y obtener una sesión asociada a mi rol, para operar solo las pantallas y acciones que mi nivel jerárquico habilita.

Prioridad MoSCoW: Must. Sin ingreso no hay acceso a ninguna otra capacidad; es la puerta de entrada del front.
Estimación: 5 SP (Fibonacci).
CU relacionados: CU-01. NB upstream: NB-01.

Criterios de aceptación:

- Given un usuario administrador habilitado con credenciales válidas, When ingresa sus credenciales en el front, Then el front abre una sesión asociada a su rol y habilita solo las pantallas y acciones de su alcance.
- Given un usuario con credencial incorrecta, When intenta ingresar, Then el front informa credenciales inválidas y permanece en el ingreso sin abrir sesión.
- Given un usuario dado de baja que conserva su identificador, When intenta ingresar, Then el front informa que el acceso está revocado y no abre sesión.

#### US-02 — Cerrar la sesión y dejar el acceso liberado

Como usuario administrador, quiero cerrar mi sesión, para abandonar el front sin dejar acceso disponible en el dispositivo.

Prioridad MoSCoW: Must. El cierre completa el ciclo de sesión y es condición de uso compartido del dispositivo.
Estimación: 3 SP (Fibonacci).
CU relacionados: CU-01. NB upstream: NB-01.

Criterios de aceptación:

- Given un usuario con sesión activa, When solicita cerrar su sesión, Then el front descarta el estado de sesión y vuelve a la pantalla de ingreso sin rastros de la identidad anterior.
- Given una sesión recién cerrada, When el usuario intenta volver con el botón de retroceso del navegador, Then el front no recupera la sesión y exige ingresar de nuevo.
- Given una sesión cuyo acceso vence durante el uso, When el usuario realiza una acción, Then el front informa que la sesión expiró y lleva al ingreso para reautenticarse.

#### US-03 — Listar y dar de alta usuarios del nivel inmediato inferior

Como usuario administrador, quiero listar y dar de alta a los usuarios de mi nivel inmediato inferior, para delegar la administración del personal dentro de mi alcance.

Prioridad MoSCoW: Must. La administración jerárquica de usuarios es una capacidad central de NB-01.
Estimación: 5 SP (Fibonacci).
CU relacionados: CU-02. NB upstream: NB-01.

Criterios de aceptación:

- Given un administrador con sesión activa, When da de alta a un usuario de su nivel inmediato inferior, Then el front lo crea y lo muestra en el listado acotado a su alcance.
- Given un administrador que solo alcanza su propio ámbito, When abre la administración de usuarios, Then el front no muestra usuarios de otros administradores ni de niveles fuera de su alcance.
- Given un administrador que abre el alta de un rol que no es su nivel inmediato inferior, When despliega el formulario, Then el front no ofrece ese rol y, si se forzara, el alta es rechazada e informada.
- Given un alta con un identificador de acceso ya existente, When envía el alta, Then el front informa el duplicado y mantiene el formulario para corregirlo.

#### US-04 — Dar de baja un usuario conservando su autoría visible

Como usuario administrador, quiero dar de baja a un usuario de mi alcance conservando su autoría histórica, para retirarle el acceso sin perder la traza de lo que produjo.

Prioridad MoSCoW: Should. El alta cubre el camino mínimo; la baja completa la administración pero el MVP opera sin ella en la primera iteración.
Estimación: 3 SP (Fibonacci).
CU relacionados: CU-02. NB upstream: NB-01.

Criterios de aceptación:

- Given un usuario con evidencia ya cargada, When el administrador confirma su baja, Then el front lo muestra como dado de baja y la evidencia conserva su autoría visible.
- Given un usuario con trabajo en curso, When el administrador inicia la baja, Then el front advierte que la baja inhabilita el acceso pero conserva lo recolectado y pide confirmación explícita antes de enviarla.

#### US-05 — Crear y listar relevamientos sobre un tramo vial

Como jefe de área, quiero crear relevamientos sobre un tramo vial y listar los míos con su estado, para planificar y seguir el trabajo de campo.

Prioridad MoSCoW: Must. Es el punto de partida del camino principal del relevamiento.
Estimación: 5 SP (Fibonacci).
CU relacionados: CU-03. NB upstream: NB-02.

Criterios de aceptación:

- Given un jefe de área con sesión activa, When crea un relevamiento con un tramo de al menos un puente o camino, Then el front lo crea en estado de recolección y lo muestra en su listado.
- Given un jefe en el formulario de creación, When intenta crear un relevamiento con el tramo vacío, Then el front bloquea el envío e informa que el tramo debe abarcar al menos un puente o camino.
- Given un jefe con varios relevamientos en distintos estados, When filtra el listado por un estado del ciclo, Then el front muestra solo los relevamientos en ese estado dentro de su alcance.

#### US-06 — Editar y dar de baja un relevamiento según su estado

Como jefe de área, quiero editar o dar de baja un relevamiento según su estado, para corregir su composición mientras está en recolección y retirar los que ya no corresponden.

Prioridad MoSCoW: Should. Refina la gestión; el MVP crea y lista, y la edición y baja completan la capacidad.
Estimación: 3 SP (Fibonacci).
CU relacionados: CU-03. NB upstream: NB-02.

Criterios de aceptación:

- Given un relevamiento en recolección, When el jefe edita su nombre o la composición del tramo, Then el front envía los cambios y refleja el resultado.
- Given un relevamiento que ya avanzó de recolección, When el jefe intenta editar la composición del tramo, Then el front presenta la vista en solo lectura y no envía la edición.

#### US-07 — Crear y ubicar marcadores iniciales sobre el mapa

Como jefe de área, quiero crear y ubicar marcadores iniciales sobre el mapa de un relevamiento en recolección, para orientar a la cuadrilla sobre los puntos de referencia del tramo.

Prioridad MoSCoW: Must. La interacción de mapa con marcadores es una capacidad distintiva del front y precondición de la revisión posterior.
Estimación: 8 SP (Fibonacci).
CU relacionados: CU-05. NB upstream: NB-02.

Criterios de aceptación:

- Given un relevamiento en recolección abierto en el mapa, When el jefe crea un marcador en un punto y le agrega una etiqueta de referencia, Then el front lo fija en el mapa con esa etiqueta y queda con identidad estable.
- Given un marcador ya creado, When el jefe lo arrastra a otra posición, Then el front actualiza su coordenada conservando su identidad.
- Given un marcador ubicado dentro del radio de otro existente, When el jefe lo crea igualmente, Then el front lo muestra y la información queda accesible aunque exista conflicto, difiriendo la resolución al cierre.
- Given un relevamiento que ya no está en recolección, When el jefe intenta crear un marcador inicial, Then el front presenta el mapa en solo lectura.

#### US-08 — Asignar agentes de campo a un relevamiento

Como jefe de área, quiero asignar agentes de campo de mi ámbito a un relevamiento, para repartir el trabajo de campo de forma trazable.

Prioridad MoSCoW: Must. Sin asignación no hay agentes habilitados para recolectar; integra el camino principal.
Estimación: 5 SP (Fibonacci).
CU relacionados: CU-04. NB upstream: NB-02.

Criterios de aceptación:

- Given un relevamiento en recolección y agentes disponibles del jefe, When el jefe asigna uno o varios agentes, Then el front los muestra como asignados al relevamiento.
- Given un agente ya asignado a ese relevamiento, When el jefe intenta asignarlo de nuevo, Then el front mantiene una sola asignación y no duplica el vínculo.
- Given un relevamiento ya cerrado, When el jefe intenta asignar un agente, Then el front presenta la sección en solo lectura y no envía cambios.

#### US-09 — Reasignar y quitar agentes conservando lo recolectado

Como jefe de área, quiero reasignar o quitar agentes de un relevamiento, para reacomodar el trabajo cuando cambian las condiciones de campo sin perder lo ya recolectado.

Prioridad MoSCoW: Should. Complementa la asignación; el MVP asigna y la reasignación afina la operación.
Estimación: 3 SP (Fibonacci).
CU relacionados: CU-04. NB upstream: NB-02.

Criterios de aceptación:

- Given un relevamiento con un agente asignado que ya cargó evidencia, When el jefe lo reemplaza por otro agente, Then el front muestra al entrante asignado, al saliente sin asignación y la evidencia del saliente conservada.
- Given un relevamiento en recolección con un único agente asignado, When el jefe quita a ese agente, Then el front advierte que el relevamiento quedará sin agentes y pide confirmación antes de enviar la revocación.

#### US-10 — Recorrer marcadores y navegar el carrusel encadenado de fotos

Como jefe de área, quiero recorrer los marcadores sobre el mapa y navegar el carrusel encadenado de fotos, para revisar la evidencia en su contexto geográfico y preparar el informe de cierre.

Prioridad MoSCoW: Must. La revisión sobre mapa con carrusel es el corazón de NB-05 y precondición del cierre.
Estimación: 8 SP (Fibonacci).
CU relacionados: CU-06. NB upstream: NB-05.

Criterios de aceptación:

- Given un relevamiento en revisión con varios marcadores con fotos, When el jefe avanza en el carrusel hasta el final de un marcador, Then el front encadena con las fotos del marcador contiguo sin cerrar el carrusel.
- Given un relevamiento con marcadores en conflicto, When el jefe abre la revisión sobre el mapa, Then el front muestra toda la evidencia accesible y señala que hay conflictos pendientes de resolver al cierre.
- Given un marcador sin fotos cargadas, When el jefe lo selecciona, Then el front informa que no tiene fotos y ofrece pasar al marcador contiguo.

#### US-11 — Filtrar la evidencia por etiqueta durante la revisión

Como jefe de área, quiero filtrar la evidencia por etiqueta durante la revisión, para concentrarme en los marcadores y fotos de un tipo de hallazgo.

Prioridad MoSCoW: Should. Mejora la revisión; el recorrido base de US-10 funciona sin el filtro.
Estimación: 3 SP (Fibonacci).
CU relacionados: CU-06. NB upstream: NB-05.

Criterios de aceptación:

- Given un relevamiento con fotos etiquetadas y otras sin esa etiqueta, When el jefe filtra por una etiqueta, Then el front muestra solo los marcadores y fotos que llevan esa etiqueta.
- Given un filtro por una etiqueta que ningún marcador lleva, When el jefe lo aplica, Then el front informa que no hay coincidencias y permite limpiar el filtro.

#### US-12 — Resolver un conflicto de marcadores unificando o separando

Como jefe de área, quiero resolver cada conflicto de marcadores decidiendo unificarlos o mantenerlos separados, para dejar la evidencia catalogada antes del cierre.

Prioridad MoSCoW: Must. La resolución de conflictos es precondición del cierre (RN-05) y capacidad central de NB-05.
Estimación: 8 SP (Fibonacci).
CU relacionados: CU-07. NB upstream: NB-05.

Criterios de aceptación:

- Given un relevamiento en revisión con dos marcadores en conflicto dentro de un radio, When el jefe decide unificarlos, Then el front muestra un único marcador resultante con la evidencia de ambos y un conflicto menos pendiente.
- Given un conflicto entre marcadores con etiquetas distintas, When el jefe los unifica, Then el front advierte y el marcador resultante conserva la unión de las etiquetas.
- Given un relevamiento que aún no está en revisión, When el jefe intenta resolver un conflicto, Then el front presenta la pantalla en solo lectura.
- Given un conflicto ya resuelto como separados antes del cierre, When el jefe lo reabre y lo unifica, Then el front vuelve a dejarlo pendiente y aplica la nueva decisión.

#### US-13 — Transicionar el estado del relevamiento por su ciclo

Como jefe de área, quiero transicionar el estado de un relevamiento por su ciclo, para formalizar el avance del trabajo entre recolección y revisión.

Prioridad MoSCoW: Must. El ciclo de estados ordena el trabajo y habilita la revisión y el cierre.
Estimación: 5 SP (Fibonacci).
CU relacionados: CU-08. NB upstream: NB-05.

Criterios de aceptación:

- Given un relevamiento en recolección, When el jefe solicita pasarlo a revisión, Then el front lo muestra en estado de revisión.
- Given un relevamiento en revisión al que le falta evidencia, When el jefe solicita devolverlo a recolección, Then el front aplica la transición de retorno controlado y lo muestra en recolección.
- Given un relevamiento ya cerrado, When el jefe intenta una transición no válida desde el cierre, Then el front no ofrece esa transición.

#### US-14 — Cerrar el relevamiento solo sin conflictos pendientes

Como jefe de área, quiero cerrar un relevamiento solo cuando no quedan conflictos pendientes, para garantizar que el informe se apoya en evidencia catalogada.

Prioridad MoSCoW: Must. El cierre es el hito que habilita el informe; su condición de ausencia de conflictos es RN-05.
Estimación: 5 SP (Fibonacci).
CU relacionados: CU-08, CU-07. NB upstream: NB-05.

Criterios de aceptación:

- Given un relevamiento en revisión sin conflictos pendientes, When el jefe solicita cerrarlo, Then el front lo muestra cerrado y habilitado para el informe.
- Given un relevamiento en revisión con un conflicto sin resolver, When el jefe intenta cerrarlo, Then el front bloquea el cierre, informa los conflictos pendientes y deriva a la pantalla de resolución.

#### US-15 — Cargar fotos con agrupación por radio desde el front

Como agente de campo, quiero subir fotos desde el front y que se agrupen en marcadores según un radio, para cargar manualmente la evidencia de un relevamiento asignado sin capturar en terreno.

Prioridad MoSCoW: Must. Es la materialización web de la carga manual; capacidad declarada del front sobre NB-02.
Estimación: 8 SP (Fibonacci).
CU relacionados: CU-09. NB upstream: NB-02.

Criterios de aceptación:

- Given un agente asignado a un relevamiento en recolección con un radio de agrupación definido, When sube varias fotos con ubicación incrustada dentro de ese radio, Then el front las muestra agrupadas en un único marcador.
- Given una foto cuya ubicación está lejos de todo marcador, When el agente la sube, Then el front la muestra en un marcador nuevo creado en la ubicación de la foto.
- Given un agente que no definió el radio de agrupación, When intenta subir fotos, Then el front bloquea la carga e informa que falta definir el radio.

#### US-16 — Completar comentarios, etiquetas y ubicación manual de la evidencia

Como agente de campo, quiero completar comentarios y etiquetas de cada foto y ubicar manualmente las fotos sin ubicación, para dejar la evidencia descrita y posicionada.

Prioridad MoSCoW: Should. La carga base de US-15 sube y agrupa; la descripción y la ubicación manual completan la evidencia.
Estimación: 5 SP (Fibonacci).
CU relacionados: CU-09. NB upstream: NB-02.

Criterios de aceptación:

- Given fotos ya agrupadas en marcadores, When el agente agrega comentario y etiqueta a cada foto y la nota de la observación, Then el front envía los datos y refleja la evidencia descrita.
- Given una foto sin datos de ubicación incrustados, When el agente la sube, Then el front la deja pendiente de ubicación manual sin asignarle coordenada y permite ubicarla sobre el mapa.

#### US-17 — Exportar e importar un relevamiento completo

Como jefe de área o usuario raíz, quiero exportar un relevamiento completo como una unidad transferible e importar esa unidad, para compartir, archivar y mover relevamientos entre entornos.

Prioridad MoSCoW: Could. Es portabilidad (NB-06): agrega valor pero no integra el camino principal del relevamiento; puede esperar a una iteración posterior.
Estimación: 5 SP (Fibonacci).
CU relacionados: CU-10. NB upstream: NB-06.

Escenario de referencia (se refina al promover la historia):

- Given un relevamiento dentro del alcance del solicitante, When solicita exportarlo, Then el front entrega una única unidad transferible para descargar con todo el relevamiento; e inversamente, dada una unidad válida, al importarla el front muestra el relevamiento reconstruido, y dada una unidad dañada el front rechaza la importación sin crear un relevamiento parcial.

#### US-18 — Configurar el destino de almacenamiento de archivos

Como usuario raíz, quiero consultar y cambiar el destino donde se alojan las fotografías, para controlar dónde se guarda la evidencia según costo, capacidad y contexto.

Prioridad MoSCoW: Could. Es configuración de almacenamiento (NB-07): transparente para los demás roles y fuera del camino principal; puede esperar.
Estimación: 3 SP (Fibonacci).
CU relacionados: CU-11. NB upstream: NB-07.

Escenario de referencia (se refina al promover la historia):

- Given un usuario raíz con un destino vigente, When cambia el destino y completa sus datos de configuración, Then el front muestra el nuevo destino vigente y el cambio es transparente para los demás roles; y dado un rol que no es el raíz, el front no le ofrece esta pantalla.

## 4. Métricas de avance

Distribución por prioridad MoSCoW sobre el backlog inicial (18 US, 90 SP):

| Prioridad | Cantidad de US | Suma de SP | Participación en SP |
| --- | --- | --- | --- |
| Must | 11 | 65 | 72 % |
| Should | 5 | 17 | 19 % |
| Could | 2 | 8 | 9 % |
| Won't (v1.0) | 0 | 0 | 0 % |

Conteo de historias: 18 en total (11 Must, 5 Should, 2 Could). La suma de SP por prioridad es 65 + 17 + 8 = 90, que coincide con el total general; cada historia se cuenta una sola vez aunque US-14 trace a dos CU (CU-08 y CU-07), porque es una única historia Must de 5 SP. La participación en SP se calcula sobre 90 SP totales y se redondea, por lo que puede no sumar exactamente 100 %.

| Métrica | Valor inicial |
| --- | --- |
| Total de historias | 18 |
| Total de story points | 90 |
| Porcentaje cerrado (Done) | 0 % |
| Deuda en backlog (historias en Borrador sin DoR cumplida) | 18 |
| MVP (historias Must) | 11 US, 65 SP |

El MVP definido por las historias Must (EP-01 a EP-06) cubre el camino principal del relevamiento de extremo a extremo: ingreso, administración de usuarios, gestión de relevamientos, marcadores, asignación, revisión sobre mapa con carrusel, resolución de conflictos, transición de estado, cierre y carga manual de fotos. La portabilidad y la configuración (EP-07) quedan como Could fuera del MVP.

## 5. Refinamiento

| Aspecto | Definición |
| --- | --- |
| Cadencia | Una sesión de refinement por sprint, mínimo, conforme al piso de la regla 06 §2.2 para web-monolith |
| Responsable de la facilitación | Scrum Master |
| Participantes | Equipo de desarrollo (un desarrollador), con revisión acotada del Analista Funcional (trazabilidad a CU), el Arquitecto (justificación de BT) y QA (verificabilidad de criterios) |
| Técnica de estimación | Planning Poker con escala Fibonacci (1, 2, 3, 5, 8, 13); se mantiene la misma técnica en todo el backlog |
| Entradas del refinement | Casos de uso y reglas de negocio de la especificación funcional; ADRs y componentes de la arquitectura; feedback de sprints previos |
| Salidas del refinement | Historias que pasan de Borrador a Ready al cumplir la Definition of Ready; ajuste de prioridad MoSCoW y de estimación |
| Promoción de historias Could | US-17 y US-18 se refinan y se les agregan escenarios Given/When/Then completos al promoverlas a una iteración planificada |

## 6. Vinculación cross-doc

- Este backlog se complementa con el `backlog-tecnico_v1.0.md`, que organiza las tareas técnicas por épica técnica y mantiene la matriz BT↔US↔CU.
- La `definition-of-ready_v1.0.md` actúa como filtro de entrada: ninguna historia entra a Sprint Planning sin cumplir la DoR.
- Upstream: cada historia traza a uno o más CU de la especificación funcional (02) y a una NB de las necesidades de negocio (01: NB-01, NB-02, NB-05, NB-06, NB-07).
- Downstream: cada historia alimenta el sprint plan (07) y los acceptance tests (08) a partir de sus escenarios Given/When/Then.

## 7. Modo de organización del backlog

Con 18 historias de usuario (menos de 20) y 14 tareas técnicas (menos de 30), ambos artefactos operan en modo inline conforme a la regla 06 §3.3: las US viven inline en este `product-backlog_v1.0.md` y las BT inline en el `backlog-tecnico_v1.0.md`, sin carpetas `historias-usuario/` ni `tareas-tecnicas/`. Cada US conserva su bloque de historia, su trazabilidad y sus criterios de aceptación; cada BT conserva su justificación, dependencias y trazabilidad. Si el backlog superara los umbrales, se migraría a archivos individuales.

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Backlog de producto inicial de geovial-web: 7 épicas por capacidad funcional (EP-01 a EP-07), 18 historias de usuario (US-01 a US-18) con MoSCoW, story points Fibonacci y trazabilidad a CU-01 a CU-11 y a NB-01, NB-02, NB-05, NB-06, NB-07; historias Must y Should con criterios Given/When/Then; métricas de avance y política de refinement. Modo inline (18 US < 20, 14 BT < 30). |
| 1.0 | 2026-06-15 | Corrección de consistencia: reconciliación de la tabla de métricas SP con la suma ítem-a-ítem de las historias. |
