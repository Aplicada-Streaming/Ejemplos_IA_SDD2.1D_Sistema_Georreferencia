# Product Backlog — geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** product-backlog_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Scrum Master + Mobile Lead
**Modo de redacción:** US inline (15 US < 20; ver §3.3 de 06_rules_backlog_tecnico.md)
**Técnica de estimación:** Fibonacci (1, 2, 3, 5, 8, 13)

## 1. Objetivos del producto

Construir la app de campo offline-first del agente de relevamiento, capaz de capturar observaciones georreferenciadas y sincronizarlas sin pérdida cuando vuelve la conexión. El MVP buscado habilita: iniciar sesión y reanudar el trabajo por la seguridad del dispositivo, seleccionar un relevamiento asignado desde la copia local, crear y mover marcadores sobre el mapa, capturar fotos con resolución de coordenadas, registrar comentarios y etiquetas, trabajar 100 % sin conexión y sincronizar subiendo antes de bajar, y cargar fotos manualmente priorizando la ubicación incrustada con radio de agrupación. El backlog se ordena por capacidad móvil (sesión, selección, captura, observación, sincronización, carga manual) y delega la mecánica de sincronización a la librería consumida (ADR-03).

## 2. Épicas

| EP-XX | Nombre | Descripción | Sprints estimados |
| --- | --- | --- | --- |
| EP-01 | Sesión y relogueo por seguridad del dispositivo | Inicio en línea con credenciales, relogueo por la seguridad del dispositivo en sesión activa y deslogueo completo para dispositivo compartido, con custodia segura del token. | 1 |
| EP-02 | Selección de relevamiento asignado | Listado de relevamientos asignados desde la copia local y fijación del contexto activo de captura, con refresco cuando hay conexión. | 1 |
| EP-03 | Mapa y captura georreferenciada | Centrado por ubicación, creación y movimiento de marcadores, y captura de foto con resolución de coordenada en el momento, ejecutables sin conexión. | 2 |
| EP-04 | Observación con comentarios y etiquetas | Enriquecimiento de la observación con nota, comentario por foto y etiquetas reutilizables sobre fotos y marcadores, en el almacén local. | 1 |
| EP-05 | Trabajo offline y sincronización | Acumulación de cambios en la cola local, detección de conexión y ciclo subir-luego-bajar con reanudación y convivencia con conflictos, por consumo de la librería. | 2 |
| EP-06 | Carga manual de fotos | Carga de un conjunto de fotos del dispositivo con priorización de la ubicación incrustada y agrupación por radio en marcadores locales. | 1 |

## 3. Historias por épica

| US-XX | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-01 | Iniciar sesión en línea con credenciales | Must | 5 | Borrador | CU-01 | EP-01 |
| US-02 | Reloguear por seguridad del dispositivo y deslogueo completo | Must | 5 | Borrador | CU-01 | EP-01 |
| US-03 | Ver y seleccionar un relevamiento asignado | Must | 3 | Borrador | CU-02 | EP-02 |
| US-04 | Refrescar la lista de relevamientos con conexión | Should | 3 | Borrador | CU-02 | EP-02 |
| US-05 | Centrar por ubicación y crear un marcador | Must | 5 | Borrador | CU-03 | EP-03 |
| US-06 | Mover un marcador conservando su identidad | Should | 3 | Borrador | CU-03 | EP-03 |
| US-07 | Capturar una foto con resolución de coordenada | Must | 5 | Borrador | CU-04 | EP-03 |
| US-08 | Conservar la foto sin señal de ubicación como pendiente | Should | 3 | Borrador | CU-04 | EP-03 |
| US-09 | Agregar nota, comentario y etiquetas a la observación | Must | 3 | Borrador | CU-05 | EP-04 |
| US-10 | Reutilizar y editar etiquetas del relevamiento | Could | 2 | Borrador | CU-05 | EP-04 |
| US-11 | Sincronizar subiendo antes de bajar | Must | 8 | Borrador | CU-06 | EP-05 |
| US-12 | Reanudar una sincronización interrumpida sin duplicar | Must | 5 | Borrador | CU-06 | EP-05 |
| US-13 | Convivir con conflictos durante la sincronización | Should | 3 | Borrador | CU-06 | EP-05 |
| US-14 | Cargar fotos manualmente con radio de agrupación | Must | 5 | Borrador | CU-07 | EP-06 |
| US-15 | Resolver fotos sin ubicación incrustada como pendientes | Could | 3 | Borrador | CU-07 | EP-06 |

### 3.1 Detalle de las US

Cada US trae historia, prioridad y, para Must y Should, criterios de aceptación Given/When/Then con al menos dos escenarios (un happy path y un edge case). Las US Could traen al menos un escenario. La trazabilidad detallada vive en la matriz BT↔US↔CU del `backlog-tecnico_v1.0.md`.

---

#### US-01 — Iniciar sesión en línea con credenciales

**Épica:** EP-01 | **MoSCoW:** Must | **SP:** 5 (Fibonacci) | **CU:** CU-01 | **NB:** NB-01

Como agente de campo, quiero iniciar sesión la primera vez con mis credenciales y conexión, para que la app obtenga y custodie mi acceso y habilite el trabajo de campo en el dispositivo.

Justificación MoSCoW: sin inicio de sesión no hay identidad del relevador ni acceso al trabajo de campo; es base de toda la operación (NB-01).

Criterios de aceptación:
- Given una app recién instalada con conexión y un agente con usuario habilitado, When el agente ingresa credenciales válidas y confirma, Then la app obtiene el token, lo guarda en el almacenamiento seguro del dispositivo y habilita el trabajo de campo.
- Given una app recién instalada sin conexión, When el agente intenta el inicio inicial, Then la app responde que el primer inicio requiere conexión y no crea sesión.
- Given un inicio en línea con credenciales rechazadas por el backend, When el agente confirma, Then la app no crea sesión, no guarda token y solicita reingresar las credenciales.

---

#### US-02 — Reloguear por seguridad del dispositivo y deslogueo completo

**Épica:** EP-01 | **MoSCoW:** Must | **SP:** 5 (Fibonacci) | **CU:** CU-01 | **NB:** NB-01

Como agente de campo, quiero reanudar una sesión activa verificándome por la seguridad del dispositivo y cerrar la sesión por completo cuando termino, para no reingresar credenciales en cada reanudación y liberar un dispositivo compartido sin filtrar datos.

Justificación MoSCoW: el dispositivo es compartido; sin relogueo seguro y deslogueo completo no se protege la sesión ni se libera el equipo (RN-04, NB-01).

Criterios de aceptación:
- Given una sesión activa guardada y la app reiniciada, When el agente se verifica por la seguridad del dispositivo, Then la app rehabilita el acceso reutilizando el token sin pedir credenciales.
- Given un dispositivo con la sesión activa del agente A, When el agente A ejecuta el deslogueo completo, Then la app borra el token y los datos de sesión y muestra el inicio de sesión sin datos del agente A.
- Given una reanudación cuyo token guardado ya venció, When el agente se verifica por la seguridad del dispositivo, Then la app exige un nuevo inicio en línea con credenciales.
- Given un dispositivo con la sesión activa del agente A, When el agente B intenta reloguearse por la seguridad del dispositivo, Then la app no le da acceso sobre la sesión ajena y exige deslogueo completo antes de un nuevo inicio en línea.

---

#### US-03 — Ver y seleccionar un relevamiento asignado

**Épica:** EP-02 | **MoSCoW:** Must | **SP:** 3 (Fibonacci) | **CU:** CU-02 | **NB:** NB-04, NB-03

Como agente de campo con sesión activa, quiero ver mis relevamientos asignados desde la copia local y elegir uno, para fijarlo como contexto activo y empezar a capturar observaciones, con o sin conexión.

Justificación MoSCoW: es el punto de entrada de toda la recolección; sin contexto activo no hay captura (CU-02).

Criterios de aceptación:
- Given un agente con sesión activa y tres relevamientos asignados en la copia local, When el agente abre la lista y selecciona uno, Then la app lo fija como contexto activo y abre su mapa con los marcadores locales.
- Given un agente sin relevamientos sincronizados y sin conexión, When el agente abre la lista, Then la app responde que no hay relevamientos disponibles sin conexión y no fija contexto activo.
- Given un relevamiento cerrado por el jefe presente en la copia local, When el agente lo selecciona, Then la app lo abre en modo lectura y no habilita nuevas capturas.

---

#### US-04 — Refrescar la lista de relevamientos con conexión

**Épica:** EP-02 | **MoSCoW:** Should | **SP:** 3 (Fibonacci) | **CU:** CU-02 | **NB:** NB-04

Como agente de campo con conexión, quiero actualizar mi lista de relevamientos, para incorporar nuevas asignaciones y cambios sin esperar a una próxima jornada.

Justificación MoSCoW: mejora la operación pero el MVP funciona con la copia local sincronizada por otros caminos; no bloquea la captura (CU-02 5.B).

Criterios de aceptación:
- Given un agente con conexión y una asignación nueva en el backend, When el agente solicita refrescar la lista, Then la app sincroniza, agrega el relevamiento nuevo a la copia local y lo muestra en la lista.
- Given un agente sin conexión, When el agente solicita refrescar la lista, Then la app conserva la lista local vigente y no falla por la ausencia de red.

---

#### US-05 — Centrar por ubicación y crear un marcador

**Épica:** EP-03 | **MoSCoW:** Must | **SP:** 5 (Fibonacci) | **CU:** CU-03 | **NB:** NB-03

Como agente de campo, quiero centrar el mapa sobre mi posición y crear un marcador en ese punto, para anclar mis observaciones al lugar del tramo donde estoy, sin conexión.

Justificación MoSCoW: el marcador es el gesto base de georreferenciación; sin él no hay dónde anclar la evidencia (CU-03, NB-03).

Criterios de aceptación:
- Given un agente con un relevamiento en recolección y permiso de ubicación concedido, When el agente centra por la ubicación del dispositivo y crea un marcador, Then la app crea el marcador en el almacén local con identidad propia y lo encola como cambio pendiente.
- Given un dispositivo sin señal de ubicación, When el agente toca centrar, Then la app informa que no hay señal y ofrece fijar el marcador manualmente en el mapa, sin inventar coordenada.
- Given un agente que niega el permiso de ubicación, When el agente intenta centrar, Then la app degrada a fijación manual del marcador en el mapa y explica que el permiso es necesario para centrar.

---

#### US-06 — Mover un marcador conservando su identidad

**Épica:** EP-03 | **MoSCoW:** Should | **SP:** 3 (Fibonacci) | **CU:** CU-03 | **NB:** NB-03

Como agente de campo, quiero mover un marcador existente a una nueva coordenada, para corregir su posición sin perder las observaciones ya ancladas a él.

Justificación MoSCoW: la corrección de posición es importante pero el MVP captura con el marcador creado; el movimiento puede afinarse luego (CU-03 5.B).

Criterios de aceptación:
- Given un marcador existente en el mapa con observaciones ancladas, When el agente lo mueve a una nueva coordenada, Then la app actualiza la coordenada conservando la identidad del marcador y encola el cambio.
- Given un marcador nuevo creado dentro del radio de otro existente, When el agente confirma la posición, Then la app crea el marcador y lo deja convivir como posible conflicto sin bloquear la recolección.

---

#### US-07 — Capturar una foto con resolución de coordenada

**Épica:** EP-03 | **MoSCoW:** Must | **SP:** 5 (Fibonacci) | **CU:** CU-04 | **NB:** NB-03

Como agente de campo, quiero tomar una foto en terreno y que la app resuelva su coordenada en el momento, para que la evidencia quede anclada al punto del tramo donde fue tomada, sin conexión.

Justificación MoSCoW: la captura georreferenciada es el núcleo de la propuesta de valor; sin ella no hay MVP defendible (NB-03, CU-04).

Criterios de aceptación:
- Given un agente sobre un marcador activo con permisos de cámara y ubicación concedidos y señal disponible, When el agente toma una foto, Then la app resuelve la coordenada en el momento, ancla la foto a una observación del marcador y la encola.
- Given un agente que negó el permiso de cámara, When el agente toca capturar foto, Then la app no abre la cámara y explica que el permiso es necesario.
- Given un marcador con una observación previa, When el agente captura otra foto sobre ese marcador, Then la app agrega la foto al mismo marcador, que queda compartido por varias observaciones.

---

#### US-08 — Conservar la foto sin señal de ubicación como pendiente

**Épica:** EP-03 | **MoSCoW:** Should | **SP:** 3 (Fibonacci) | **CU:** CU-04 | **NB:** NB-03

Como agente de campo, quiero que una foto tomada sin señal de ubicación se conserve anclada al marcador y marcada como pendiente de ubicación, para no perder la evidencia ni que la app invente una coordenada.

Justificación MoSCoW: protege la calidad de la georreferenciación; el MVP captura con señal y este es el camino degradado a no inventar datos (CU-04 5.A, RN-01).

Criterios de aceptación:
- Given un agente que toma una foto sin señal de ubicación, When el agente confirma la captura, Then la app conserva la foto anclada al marcador y la marca como pendiente de ubicación, sin inventar coordenada.
- Given el almacén local del dispositivo sin espacio para alojar la imagen, When el agente intenta guardar la foto, Then la app no guarda el binario, avisa al agente que libere espacio y conserva lo ya encolado sin pérdida.

---

#### US-09 — Agregar nota, comentario y etiquetas a la observación

**Épica:** EP-04 | **MoSCoW:** Must | **SP:** 3 (Fibonacci) | **CU:** CU-05 | **NB:** NB-03

Como agente de campo, quiero escribir una nota, comentar cada foto y aplicar etiquetas, para que la evidencia quede clasificada y descrita en el lugar y el jefe pueda filtrarla luego, sin conexión.

Justificación MoSCoW: completa la observación para que sea revisable; una observación sin descripción ni clasificación pierde valor para el cierre (CU-05, NB-03).

Criterios de aceptación:
- Given una foto de una observación en un relevamiento en recolección, When el agente le escribe un comentario y le aplica una etiqueta, Then la app registra el comentario y la etiqueta en el almacén local y encola los cambios.
- Given un agente que intenta crear una etiqueta sin nombre, When el agente confirma la etiqueta vacía, Then la app no crea la etiqueta y solicita un nombre.
- Given una foto que quedó pendiente de ubicación precisa, When el agente le agrega comentario y etiqueta, Then la app los registra sin requerir coordenada de la foto.

---

#### US-10 — Reutilizar y editar etiquetas del relevamiento

**Épica:** EP-04 | **MoSCoW:** Could | **SP:** 2 (Fibonacci) | **CU:** CU-05 | **NB:** NB-03

Como agente de campo, quiero reutilizar etiquetas ya usadas en el relevamiento y editar comentarios o quitar etiquetas, para mantener una clasificación consistente y corregir lo que cargué mal.

Justificación MoSCoW: agrega valor de consistencia pero el MVP ya permite crear etiquetas; la reutilización y edición pueden esperar a una iteración posterior (CU-05 5.A, 5.C).

Criterios de aceptación:
- Given una etiqueta ya usada en el relevamiento, When el agente la aplica a otra foto, Then la app aplica la misma etiqueta sin duplicarla y la deja compartida entre las fotos.

---

#### US-11 — Sincronizar subiendo antes de bajar

**Épica:** EP-05 | **MoSCoW:** Must | **SP:** 8 (Fibonacci) | **CU:** CU-06 | **NB:** NB-04

Como agente de campo, quiero que la app sincronice al recuperar conexión subiendo primero mis cambios locales y bajando después las actualizaciones del relevamiento, para que mi trabajo de campo llegue completo y sin que lo pisen novedades del backend.

Justificación MoSCoW: el orden subir-antes-de-bajar es la garantía dura contra la pérdida y duplicación de datos; es la capacidad que hace viable el trabajo de campo sin red (RN-02, NB-04).

Criterios de aceptación:
- Given un agente con cinco cambios locales encolados y conexión recuperada, When la app detecta conexión y sincroniza, Then la app sube primero los cinco cambios y solo después baja las actualizaciones, mostrando los cinco subidos antes de cualquier bajada.
- Given un agente con la cola vacía y conexión, When la app sincroniza, Then la app omite la subida y procede directamente a la bajada de actualizaciones.
- Given un agente con cambios encolados cuyo token fue rechazado por el backend, When la app sincroniza, Then la app detiene el ciclo, conserva la cola intacta y solicita reloguear.

---

#### US-12 — Reanudar una sincronización interrumpida sin duplicar

**Épica:** EP-05 | **MoSCoW:** Must | **SP:** 5 (Fibonacci) | **CU:** CU-06 | **NB:** NB-04

Como agente de campo, quiero que una sincronización cortada se reanude desde el punto de corte sin reaplicar lo ya confirmado, para no perder ni duplicar mi trabajo cuando la conexión es intermitente.

Justificación MoSCoW: sin reanudación idempotente, un corte de red rompe la integridad de la sincronización, que es un riesgo de máximo impacto del negocio (RN-02, RN-05, NB-04).

Criterios de aceptación:
- Given un ciclo con tres pendientes donde la conexión se corta tras confirmar el primero, When la app sincroniza, Then la app deja uno confirmado, conserva dos en la cola, no baja actualizaciones y deja el relevamiento reanudable sin duplicar.
- Given un relevamiento reanudable con dos pendientes y conexión recuperada, When la app retoma la sincronización, Then la app reconoce los reenvíos por su identificador de origen y no aplica un cambio dos veces.

---

#### US-13 — Convivir con conflictos durante la sincronización

**Épica:** EP-05 | **MoSCoW:** Should | **SP:** 3 (Fibonacci) | **CU:** CU-06 | **NB:** NB-04

Como agente de campo, quiero que la sincronización aplique y reporte los marcadores en conflicto sin abortar el ciclo, para que mi trabajo continúe y la resolución quede para el cierre desde la web.

Justificación MoSCoW: importante para la continuidad, pero el MVP de sincronización ya sube y baja; el reporte de conflictos refina la convivencia ya garantizada por la librería (RN-03, CU-06 5.B).

Criterios de aceptación:
- Given una bajada que incluye un marcador en conflicto por radio, When la app sincroniza, Then la app aplica la actualización en conflicto a la copia local sin abortar y la reporta como elemento en conflicto en el resumen.
- Given un agente que fuerza una sincronización mientras hay un ciclo activo para ese relevamiento, When el agente la dispara, Then la app no inicia un segundo ciclo y muestra el estado del ciclo vigente.

---

#### US-14 — Cargar fotos manualmente con radio de agrupación

**Épica:** EP-06 | **MoSCoW:** Must | **SP:** 5 (Fibonacci) | **CU:** CU-07 | **NB:** NB-03

Como agente de campo, quiero cargar un conjunto de fotos del dispositivo y que la app las ubique por su ubicación incrustada y las agrupe por radio en marcadores, para aprovechar la georreferenciación que la foto trae y evitar reubicar cada una a mano, sin conexión.

Justificación MoSCoW: capacidad declarada Must del núcleo de captura estructurada; evita retrabajo de oficina (NB-03, RN-01, CU-07).

Criterios de aceptación:
- Given un radio de agrupación definido y tres fotos con coordenada incrustada dentro de ese radio, When el agente carga las tres fotos, Then la app las agrupa en un único marcador y reporta cero marcadores nuevos adicionales para esas fotos.
- Given un radio definido y una foto con coordenada incrustada lejana a todo marcador existente, When el agente carga la foto, Then la app crea un marcador local nuevo en la coordenada de la foto y agrupa la foto en él.
- Given una carga sin radio de agrupación aplicable, When el agente intenta cargar fotos, Then la app no procesa el conjunto y solicita un radio aplicable.

---

#### US-15 — Resolver fotos sin ubicación incrustada como pendientes

**Épica:** EP-06 | **MoSCoW:** Could | **SP:** 3 (Fibonacci) | **CU:** CU-07 | **NB:** NB-03

Como agente de campo, quiero que las fotos cargadas sin ubicación incrustada queden registradas como pendientes de ubicación manual, para no perderlas ni que la app les invente una coordenada, y ubicarlas luego en el mapa.

Justificación MoSCoW: camino degradado deseable; el MVP de carga manual procesa las fotos con ubicación y este caso afina la cobertura del conjunto (CU-07 5.A, RN-01).

Criterios de aceptación:
- Given un conjunto con una foto sin datos de ubicación incrustados, When el agente carga el conjunto, Then la app registra esa foto como pendiente de ubicación manual, sin agruparla por radio ni inventar coordenada, y la incluye en el resultado como sin ubicación resuelta.

## 4. Métricas de avance

Resumen por prioridad MoSCoW sobre las 15 US del backlog inicial.

| Prioridad | Cantidad US | Story points | Porcentaje de US |
| --- | --- | --- | --- |
| Must | 9 | 44 | 60 % |
| Should | 4 | 12 | 27 % |
| Could | 2 | 5 (2 + 3) | 13 % |
| Total | 15 | 61 | 100 % |

Nota de distribución: la proporción Must 60 % / Should 27 % / Could 13 % respeta la distribución sugerida (50-60 % Must, 20-30 % Should, 10-20 % Could) y evita el anti-patrón de backlog 100 % Must. Las 9 US Must (US-01, US-02, US-03, US-05, US-07, US-09, US-11, US-12 y US-14) cubren las 7 capacidades funcionales con al menos una US Must por CU.

| Métrica | Valor inicial |
| --- | --- |
| US cerradas (Done) | 0 de 15 |
| Story points cerrados | 0 de 61 |
| Porcentaje cerrado | 0 % |
| Deuda en backlog (US en Borrador sin Ready) | 15 |

No hay capacidad Won't (v1.0) declarada: las 7 capacidades funcionales (CU-01 a CU-07) están representadas dentro del alcance v1.0. La planificación temporal por sprint pertenece a 07 y no se fija aquí.

## 5. Refinamiento

| Aspecto | Definición |
| --- | --- |
| Cadencia | Una sesión de refinement por sprint (piso del tipo mobile-app-maui, §2.2). |
| Responsable | Scrum Master + Mobile Lead, con revisión acotada de AG-02 (trazabilidad a CU), AG-05 (justificación de BT en ADR) y AG-08 (verificabilidad de criterios). |
| Formato de estimación | Planning Poker con escala Fibonacci (1, 2, 3, 5, 8, 13), declarada y mantenida en todo el backlog. |
| Entrada | El refinement toma US en Borrador, las lleva a Ready aplicando la Definition of Ready (`definition-of-ready_v1.0.md`) y desglosa BT cuando aplica. |
| Salida | US Ready con criterios Given/When/Then, estimación y trazabilidad a CU; quedan disponibles para el Sprint Planning de 07. |
| Tamaño del equipo | equipo_n = 1; el refinement es liviano y la curaduría del backlog la sostiene el rol único con las revisiones acotadas. |

Referencia cruzada: este product-backlog se complementa con `backlog-tecnico_v1.0.md` (BT y matriz BT↔US↔CU) y con `definition-of-ready_v1.0.md` (filtro de entrada al sprint).

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Product backlog inicial de geovial-mobile: 6 épicas por capacidad móvil, 15 US inline con MoSCoW, story points Fibonacci y trazabilidad a los 7 CU. Distribución MoSCoW 9 Must / 4 Should / 2 Could (61 SP) reconciliada con la tabla de historias. Modo inline por debajo del umbral de 20 US. |
