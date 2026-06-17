# Product Backlog — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** product-backlog_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + Backlog Curator (AG-06)
**Estimación:** Story points con escala Fibonacci (1, 2, 3, 5, 8, 13)

## 1. Objetivos del producto

`aplicada-sync` es una librería redistribuible y agnóstica del dominio cuya superficie pública (capa Abstractions) permite a una aplicación host propagar sus cambios locales hacia un backend remoto bajo la política subir-luego-bajar, de forma idempotente, tolerante a cortes y conviviente con estados en conflicto. El propósito del backlog es entregar esa superficie pública consumible y sus garantías de motor (orden, idempotencia, reanudación, convivencia con conflicto) trazables a NB-04 (trabajo sin conexión con sincronización confiable).

El MVP queda definido por las historias Must Have: una sesión inicializable e inyectable con sus estrategias obligatorias (EP-01), una cola persistente y ordenada que no duplica por identificador (EP-02), el ciclo subir-luego-bajar con orden garantizado y convivencia con conflicto (EP-03) y la reanudación sin pérdida ni duplicación tras un corte en la subida (EP-06). Sin estas cuatro capacidades no se cumple el propósito del motor ni los criterios de éxito de NB-04 (integridad 0 perdidos/0 duplicados, convivencia con conflicto sin bloqueo). El disparo automático por conectividad (EP-04) y la observabilidad de estado y cola (EP-05) completan el valor pero no bloquean la garantía central.

Las historias se redactan en vocabulario neutral de librería (sesión de sincronización, cambio local, almacén local del host, backend remoto, estrategia de extensión); no se nombran stacks, productos ni protocolos concretos, que viven en el intake §17 y en la categoría 11.

## 2. Épicas

Las épicas se organizan por superficie de la capa Abstractions y por capacidad del motor interno, según la variante `library` (§1.2 de las reglas).

| EP | Nombre | Descripción breve | Superficie / capacidad | Sprints estimados |
| --- | --- | --- | --- | --- |
| EP-01 | Sesión de sincronización | Configurar, inicializar y recuperar la sesión inyectando las estrategias del host | Operación Inicializar sesión + contrato de configuración | 1 |
| EP-02 | Encolado de cambios locales | Registrar y persistir cambios en una cola ordenada y única por identificador estable | Operación Encolar cambio + contrato de cola | 1 |
| EP-03 | Motor subir-luego-bajar | Ejecutar el ciclo de dos fases con orden estricto, idempotencia y convivencia con conflicto | Operación Ejecutar sincronización + orquestador del ciclo | 2 |
| EP-04 | Disparo automático por conectividad | Disparar a lo sumo un ciclo ante recuperación de red, sin reentrada | Operación Habilitar disparo automático + observador de conectividad | 1 |
| EP-05 | Observabilidad de estado y cola | Exponer estado, progreso parcial, cola de pendientes y elementos en conflicto | Operación Consultar estado y cola + registro de estado | 1 |
| EP-06 | Reanudación tras interrupción | Continuar una subida parcial desde el punto de corte sin pérdida ni duplicación | Operación Reanudar sincronización + marca de progreso | 1 |

## 3. Historias por épica

Las historias viven inline (13 US, por debajo del umbral de 20 US de §3.3). Cada US Must o Should incluye sus criterios de aceptación Given/When/Then (mínimo dos escenarios) y su check INVEST a continuación de la tabla de su épica.

| US | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-01 | Inicializar una sesión con estrategias obligatorias | Must | 5 | Borrador | CU-01 | EP-01 |
| US-02 | Recuperar una sesión persistida con su cola | Should | 3 | Borrador | CU-01 | EP-01 |
| US-03 | Encolar un cambio local con identificador estable | Must | 3 | Borrador | CU-02 | EP-02 |
| US-04 | Reencolar sin duplicar por identificador | Must | 3 | Borrador | CU-02 | EP-02 |
| US-05 | Ejecutar el ciclo subir-luego-bajar en orden | Must | 8 | Borrador | CU-03 | EP-03 |
| US-06 | Convivir con estados en conflicto sin abortar | Must | 5 | Borrador | CU-03 | EP-03 |
| US-07 | Evitar ciclos concurrentes en una misma sesión | Should | 3 | Borrador | CU-03 | EP-03 |
| US-08 | Disparar el ciclo al recuperar conectividad | Should | 5 | Borrador | CU-04 | EP-04 |
| US-09 | No reentrar por rebote de conectividad | Could | 3 | Borrador | CU-04 | EP-04 |
| US-10 | Consultar estado y tamaño de la cola | Should | 3 | Borrador | CU-05 | EP-05 |
| US-11 | Listar elementos en conflicto conocidos | Could | 2 | Borrador | CU-05 | EP-05 |
| US-12 | Reanudar una subida interrumpida sin duplicar | Must | 8 | Borrador | CU-06 | EP-06 |
| US-13 | Tolerar un nuevo corte durante la reanudación | Should | 3 | Borrador | CU-06 | EP-06 |

### EP-01 — Sesión de sincronización

US-01 — Como aplicación host, quiero inicializar una sesión de sincronización inyectando mis estrategias de almacén local y de transporte, para dejar el motor listo y validado antes de encolar o sincronizar cambios.

- Given una configuración de sesión completa y coherente (identificador de host, almacén local y backend remoto presentes), When el host solicita inicializar la sesión, Then el motor valida la configuración, prepara las estructuras de metadatos en el almacén local y devuelve un identificador de sesión con estado inicial listo.
- Given una configuración a la que le falta una estrategia obligatoria, When el host solicita inicializar la sesión, Then el motor rechaza con CONFIGURACION_INCOMPLETA y no deja ninguna sesión a medias ni estructuras parciales en el almacén local.
- Given una configuración sin proveedor de credencial, When el host inicializa la sesión, Then el motor deja la sesión en estado no autenticada, admite encolar cambios pero no ejecutar la sincronización.

INVEST: Independent (no requiere otra US del mismo sprint para planificarse). Negotiable (el alcance de validaciones admite ajuste en refinamiento). Valuable (el host obtiene un motor listo y validado, valor explícito). Estimable (CU-01 y contrato de configuración cierran la información). Small (cabe en un sprint). Testable (escenarios verificables contra los códigos de error del contrato).

US-02 — Como aplicación host, quiero reinicializar sobre una sesión previa persistida, para conservar los cambios pendientes acumulados sin conexión y no perder trabajo de campo entre arranques de la aplicación.

- Given un almacén local que contiene una sesión previa del mismo host con cola y marca de progreso, When el host vuelve a inicializar la sesión, Then el motor reutiliza el estado persistido, conserva los cambios pendientes y devuelve el estado recuperado.
- Given un almacén local sin sesión previa para el host, When el host inicializa la sesión, Then el motor crea las estructuras desde cero y devuelve estado inicial listo con cola vacía.

INVEST: Independent (planificable tras US-01 pero no dentro del mismo sprint obligatorio). Negotiable (la política de recuperación admite ajuste). Valuable (preserva el trabajo offline entre arranques). Estimable (flujo 5.A de CU-01 cierra el alcance). Small. Testable (recuperación verificable comparando cola antes y después del reinicio).

### EP-02 — Encolado de cambios locales

US-03 — Como aplicación host, quiero encolar un cambio local con un identificador estable y su carga útil opaca, para acumular el trabajo de campo en una cola ordenada que el motor subirá cuando haya conectividad.

- Given una sesión inicializada y un cambio local con identificador estable presente, When el host solicita encolarlo, Then el motor lo persiste en la cola conservando el orden de creación y devuelve confirmación con el tamaño actualizado de la cola.
- Given un cambio local que llega sin identificador estable, When el host solicita encolarlo, Then el motor rechaza con IDENTIFICADOR_CAMBIO_AUSENTE y deja la cola inalterada.
- Given que no hay sesión inicializada, When el host intenta encolar un cambio, Then el motor rechaza con SESION_NO_INICIALIZADA sin persistir nada.

INVEST: Independent. Negotiable (la forma del cambio admite campos opcionales). Valuable (habilita la captura offline acumulable). Estimable (CU-02 y contrato del cambio local cierran el alcance). Small. Testable (el tamaño de cola y los códigos de error son observables).

US-04 — Como aplicación host, quiero que reencolar un cambio con un identificador ya presente no genere duplicados, para garantizar que el backend reciba una sola aplicación efectiva de cada cambio (RN-02).

- Given un cambio ya pendiente en la cola con cierto identificador, When el host vuelve a encolar un cambio con ese mismo identificador antes de subirlo, Then el motor conserva una sola entrada por identificador, actualiza la carga útil y confirma sin incrementar el tamaño de la cola.
- Given dos cambios con identificadores distintos donde el segundo anula al primero, When el host los encola en orden, Then el motor conserva ambas entradas respetando el orden de creación, sin colapsarlas.

INVEST: Independent. Negotiable (la política de actualización de carga útil admite ajuste). Valuable (garantiza no duplicación, criterio de éxito de NB-04). Estimable (flujos 5.A y 5.B de CU-02). Small. Testable (cardinalidad de la cola verificable).

### EP-03 — Motor subir-luego-bajar

US-05 — Como aplicación host, quiero ejecutar un ciclo de sincronización que suba primero todos los cambios pendientes y solo después baje las actualizaciones, para que el backend nunca reciba una bajada mientras quedan cambios confirmables por subir (RN-01).

- Given una sesión lista con cola de pendientes no vacía, When el host ejecuta la sincronización, Then el motor sube los pendientes en orden de creación, verifica que no quedan confirmables, recién entonces baja las actualizaciones posteriores a la última marca y devuelve un resumen con cambios subidos y actualizaciones bajadas.
- Given una sesión lista con cola de pendientes vacía, When el host ejecuta la sincronización, Then el motor omite la fase de subida, registra cero cambios subidos y procede directamente a la bajada.
- Given un corte de conectividad durante la fase de subida, When la subida queda incompleta, Then el motor no inicia la bajada, deja la sesión en estado reanudable y reporta SUBIDA_INCOMPLETA sin pérdida ni duplicación.

INVEST: Independent (consume la cola de EP-02 pero se planifica como ciclo propio). Negotiable (el detalle de reporte admite ajuste). Valuable (entrega la garantía central de orden de NB-04). Estimable (CU-03 y ADR-05 cierran el alcance). Small (un ciclo de dos fases cabe en un sprint). Testable (orden y resumen verificables).

US-06 — Como aplicación host, quiero que un estado en conflicto reportado por el backend no aborte el ciclo, para que la operación de campo continúe y el conflicto se difiera sin bloquear la sincronización (RN-03).

- Given una bajada en la que el backend marca una o más entidades en conflicto, When el motor las aplica, Then las incorpora como estado válido en conflicto, no aborta el ciclo y las incluye en el resumen como elementos en conflicto.
- Given un ciclo que baja entidades sin conflicto, When el motor las aplica, Then el resumen reporta cero elementos en conflicto y el ciclo concluye en estado listo.

INVEST: Independent. Negotiable (el formato del reporte de conflicto admite ajuste). Valuable (cumple el criterio de NB-04 de 0 ciclos bloqueados por conflicto). Estimable (flujo 5.B de CU-03 y ADR-08). Small. Testable (continuidad del ciclo y conteo de conflictos verificables).

US-07 — Como aplicación host, quiero que una solicitud de ejecución mientras hay un ciclo en curso no inicie un segundo ciclo, para evitar ciclos paralelos sobre la misma sesión y preservar la integridad de la cola.

- Given un ciclo de sincronización ya en curso para una sesión, When el host solicita ejecutar otra vez, Then el motor no inicia un segundo ciclo y devuelve el estado de la ejecución vigente.
- Given que no hay ningún ciclo en curso, When el host solicita ejecutar, Then el motor inicia un único ciclo normalmente.

INVEST: Independent. Negotiable (el dato devuelto del ciclo vigente admite ajuste). Valuable (evita corrupción por concurrencia). Estimable (flujo 5.C de CU-03 y vista de procesos). Small. Testable (exclusión mutua observable).

### EP-04 — Disparo automático por conectividad

US-08 — Como aplicación host, quiero habilitar que el motor dispare la sincronización al recuperar conectividad, para que el trabajo de campo se sincronice por sí solo sin gestión manual al volver a zona con cobertura.

- Given el disparo automático habilitado y una sesión autenticada, When la fuente de conectividad emite una transición a red disponible, Then el motor dispara un ciclo subir-luego-bajar y notifica al host el resultado.
- Given el disparo automático no habilitado, When llega un evento de red disponible, Then el motor no dispara ningún ciclo y reporta DISPARO_AUTOMATICO_DESHABILITADO.
- Given una sesión sin credencial vigente, When llega un evento de red disponible con disparo habilitado, Then el motor no dispara y reporta SESION_NO_AUTENTICADA sin alterar la cola.

INVEST: Independent (depende del ciclo de EP-03 pero se planifica como capacidad propia). Negotiable (la política de notificación admite ajuste). Valuable (elimina la gestión manual, valor de NB-04). Estimable (CU-04 y ADR-05). Small. Testable (disparo y códigos verificables con una fuente de conectividad de prueba).

US-09 — Como aplicación host, quiero que múltiples eventos de recuperación en una ventana breve no generen ciclos paralelos, para que el rebote de la red no dispare sincronizaciones redundantes.

- Given un ciclo ya en curso, When llegan varios eventos de recuperación de conectividad en una ventana breve, Then el motor ignora los eventos redundantes y no inicia ciclos paralelos.
- Given un evento de pérdida de conectividad, When el motor lo recibe, Then no dispara ningún ciclo y deja que un ciclo en curso, si lo hay, finalice o se detenga por sus propias excepciones.

INVEST: Independent. Negotiable (la ventana de descarte admite ajuste). Valuable (protege contra disparos redundantes). Estimable (flujos 5.A y 5.C de CU-04). Small. Testable (no reentrada verificable). Nota: Could; se redactan criterios BDD por buena práctica aunque la regla solo los exige para Must y Should.

### EP-05 — Observabilidad de estado y cola

US-10 — Como aplicación host, quiero consultar el estado de la sesión y el tamaño de la cola sin alterarla, para presentar al usuario cuántos cambios quedan por sincronizar y la situación del motor.

- Given una sesión inicializada con cambios pendientes, When el host consulta el estado, Then el motor devuelve la situación de la sesión, la cantidad de pendientes y la marca de última sincronización sin modificar la cola.
- Given una sincronización en curso, When el host consulta el estado, Then el motor devuelve situación sincronizando con el progreso parcial (subidos y restantes).
- Given que no hay sesión inicializada, When el host consulta el estado, Then el motor responde SESION_NO_INICIALIZADA sin alterar nada.

INVEST: Independent. Negotiable (el nivel de detalle admite ajuste). Valuable (habilita la presentación del avance al usuario). Estimable (CU-05 y registro de estado). Small. Testable (lectura sin efectos colaterales verificable).

US-11 — Como aplicación host, quiero listar los elementos marcados en conflicto, para mostrarlos como pendientes de resolución sin que el motor los resuelva por su cuenta (RN-03).

- Given una sesión con elementos en conflicto conocidos, When el host solicita el listado de conflictos, Then el motor devuelve los identificadores en conflicto dejando claro que convive con ellos y no los resuelve.
- Given una sesión sin conflictos conocidos, When el host solicita el listado de conflictos, Then el motor devuelve una lista vacía.

INVEST: Independent. Negotiable. Valuable (visibiliza los conflictos diferidos). Estimable (flujo 5.B de CU-05). Small. Testable (contenido del listado verificable). Nota: Could; criterios BDD incluidos por buena práctica.

### EP-06 — Reanudación tras interrupción

US-12 — Como aplicación host, quiero reanudar una subida interrumpida reenviando solo los cambios no confirmados, para no perder ni duplicar trabajo de campo tras un corte de conectividad (RN-02, ADR-06, ADR-07).

- Given una sesión en estado reanudable con marca de progreso y pendientes no confirmados, When el host reanuda la sincronización, Then el motor reenvía solo los cambios faltantes en orden, el backend reconoce por identificador los ya recibidos sin reaplicarlos, y recién al concluir la subida el motor baja las actualizaciones.
- Given un corte ocurrido después de que el backend recibió todos los cambios pero antes de registrar la confirmación, When el host reanuda, Then el backend reconoce todos por identificador, no reaplica ninguno y el motor procede a la bajada.
- Given una marca de progreso que no concuerda con la cola, When el host intenta reanudar, Then el motor reporta PROGRESO_INCONSISTENTE y reconstruye desde la cola como fuente de verdad sin duplicar.

INVEST: Independent (consume el ciclo de EP-03 pero entrega su propia garantía de resiliencia). Negotiable (el detalle del resumen de reanudación admite ajuste). Valuable (cumple el criterio de NB-04 de 0 perdidos y 0 duplicados). Estimable (CU-06, ADR-06, ADR-07). Small (cabe en un sprint con holgura). Testable (conjunto aplicado en el backend comparable contra el esperado).

US-13 — Como aplicación host, quiero que un nuevo corte durante la reanudación conserve el avance y deje la sesión nuevamente reanudable, para poder reintentar las veces que haga falta sin perder ni duplicar datos.

- Given una reanudación en curso, When la conectividad vuelve a caer mientras se reenvían los pendientes, Then el motor conserva el avance, deja la sesión nuevamente reanudable y no inicia la bajada.
- Given un backend que sigue sin responder, When el host intenta reanudar, Then el motor reporta BACKEND_INALCANZABLE y mantiene los pendientes intactos en estado reanudable.

INVEST: Independent. Negotiable. Valuable (resiliencia ante cortes encadenados). Estimable (flujo 5.B de CU-06). Small. Testable (idempotencia tras cortes sucesivos verificable).

## 4. Métricas de avance

Resumen por prioridad MoSCoW (técnica de estimación: Fibonacci). El reparto evita el anti-patrón de "todo Must".

| Prioridad | Cantidad de US | Story points | Porcentaje de US |
| --- | --- | --- | --- |
| Must | 6 | 32 | 46 % |
| Should | 5 | 17 | 38 % |
| Could | 2 | 5 | 15 % |
| Won't (v1.0) | 0 | 0 | 0 % |
| Total | 13 | 54 | 100 % |

Avance del backlog:

| Indicador | Valor v1.0 |
| --- | --- |
| US en estado Done | 0 |
| Porcentaje cerrado | 0 % |
| SP cerrados | 0 de 54 |
| US Ready (pasaron DoR) | 0 (todas en Borrador) |
| Deuda en backlog (US sin refinar a Ready) | 13 |

MVP por Must Have: US-01, US-03, US-04, US-05, US-06, US-12 (32 SP). Estas seis cubren sesión, cola única, ciclo en orden, convivencia con conflicto y reanudación sin pérdida, que son las garantías que NB-04 exige (integridad 0/0 y convivencia con conflicto sin bloqueo). Should y Could completan disparo automático, observabilidad y endurecimiento ante rebote y cortes encadenados.

Candidatos a v2.0 (fuera de este backlog, documentados para no perderse): estrategias de resolución de conflicto inyectables por el host (hoy el motor solo convive, RN-03/ADR-08), métricas exportables a un destino del host más allá de los contadores consultables, y compactación de la cola por colapso de cambios equivalentes.

## 5. Refinamiento

- Cadencia: una sesión de refinement por sprint (mínimo de la variante `library`, §2.2 de las reglas). El equipo es de un desarrollador (equipo_n=1); la sesión la conduce el AG-06 con participación del desarrollador como implementador.
- Formato de estimación: Planning Poker con escala Fibonacci, declarada y mantenida en todo el backlog (campo Estimación de la cabecera). No se mezcla con horas ni con T-shirt.
- Entrada y salida del refinement: una US entra al refinement en estado Borrador y solo pasa a Ready cuando cumple la Definition of Ready (`definition-of-ready_v1.0.md`). Una US Ready es candidata a entrar al Sprint Planning de la categoría 07.
- Revisiones acotadas: AG-02 firma la trazabilidad a CU (columna CU relacionados sin huérfanas); AG-05 valida que las BT derivadas se justifican en ADR, componente o contrato de 05; AG-08 valida que los criterios Given/When/Then alimentan los acceptance tests de 08.
- Curaduría continua: las US Could (US-09, US-11) y los candidatos a v2.0 se revisan cada sprint para confirmar prioridad o moverlos a Won't (v1.0) si el alcance lo exige.

## 6. Referencias cruzadas

- Vista técnica: `backlog-tecnico_v1.0.md` (épicas técnicas, BT y matriz BT↔US↔CU).
- Filtro de entrada: `definition-of-ready_v1.0.md`.
- Upstream: NB-04 (01); especificación funcional, CU-01 a CU-06 y RN-01 a RN-03 (02); arquitectura, ADR-01 a ADR-08, contratos-abstractions y extensibilidad (05).

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Product backlog inicial de aplicada-sync: 6 épicas (EP-01 a EP-06) por superficie de Abstractions y capacidad del motor, 13 US inline (US-01 a US-13) con MoSCoW, SP Fibonacci, criterios Given/When/Then e INVEST, métricas de avance y política de refinement. Derivado de NB-04, de la especificación funcional de 02 y de la arquitectura de 05. |
