# DX — Catálogo de errores del motor aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** dx-error-messages_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** DX Lead
**Variante:** DX

## 0. Superficie pública que documenta

Este catálogo recoge los mensajes de error que el motor de sincronización `aplicada-sync` devuelve al developer integrador a través de su superficie pública. Cada código proviene de las tablas de excepciones y errores de los casos de uso de la categoría 02 (CU-01 a CU-06), y cubre los escenarios de conectividad, sincronización interrumpida o parcial, conflicto tolerado y cola pendiente. Es el modo reference de los errores dentro del plan Diátaxis del paquete (`dx-developer-experience_v1.0.md` §4). Los códigos forman parte del contrato de la superficie pública: alterarlos constituye un cambio incompatible (02 §8).

## 1. Principios de redacción de errores

- Lenguaje plano y accionable. Cada error responde tres preguntas: qué pasó, por qué pasó y qué hacer al respecto.
- Sin culpar al integrador ni a la persona usuaria. El motor describe la condición detectada, no atribuye intención ni reprocha.
- Sin mensajes genéricos. No existe un "ocurrió un problema": cada condición de los CU tiene un código estable y una acción sugerida.
- Distinción explícita entre defecto de integración y condición transitoria. Como la librería trabaja sin conexión por diseño, separa los errores que requieren corregir la integración (entrada inválida, conflicto de estado) de las condiciones de red que solo requieren reintentar o reanudar (error transitorio) y que no implican pérdida de datos.
- Acción sugerida verificable. La acción siempre devuelve al integrador a un estado del que puede continuar (proveer el campo faltante, renovar la credencial, reanudar el ciclo, esperar conectividad).
- Código estable y único. Cada código es invariante a través de versiones menores y de la localización; el texto se traduce, el código no.

## 2. Taxonomía

| Categoría | Significado | Naturaleza | Acción típica del integrador |
| --- | --- | --- | --- |
| Entrada inválida | El integrador llamó la superficie pública con datos faltantes o incompletos. | Defecto de integración. | Corregir la llamada o la configuración y reintentar. |
| Recurso ausente | Falta un recurso del entorno que el host debe proveer (sesión, fuente de conectividad, almacén local). | Defecto de integración o de entorno. | Proveer o inicializar el recurso faltante. |
| Conflicto de estado | La operación no es válida en el estado actual del motor (sesión ya inicializada, ciclo ya en curso, sesión no reanudable). | Condición de estado. | Ajustar el momento de la llamada o usar el resultado del estado vigente. |
| Error transitorio | Condición de red o de progreso que se resuelve reintentando o reanudando, sin pérdida ni duplicación. | Transitorio, esperado en trabajo sin conexión. | Reintentar o reanudar cuando vuelva la conectividad. |
| Autenticación | La credencial falta o fue rechazada por el backend. | Condición de credencial del host. | Proveer o renovar la credencial y reintentar. |

El motor no expone una categoría de "error interno" propia en su contrato público: toda condición catalogada es diagnosticable y accionable por el integrador. Un fallo no contemplado por estos códigos se considera un defecto de la librería y se atiende por el feedback loop (`dx-developer-experience_v1.0.md` §7), no por este catálogo.

## 3. Catálogo

Catálogo completo de los errores de la superficie pública, agrupado por escenario. La columna CU indica el caso de uso de la categoría 02 del que proviene el código.

### 3.1 Inicialización y configuración

| Código | Categoría | Mensaje | Causa probable | Acción sugerida | CU |
| --- | --- | --- | --- | --- | --- |
| CONFIGURACION_INCOMPLETA | Entrada inválida | Falta un campo obligatorio de la configuración de sesión: {campo}. | La configuración llegó sin el almacén local, el backend remoto o el identificador de host. | Completar el campo indicado en el detalle y volver a inicializar; el motor no dejó sesión a medias. | CU-01 |
| ALMACEN_LOCAL_INACCESIBLE | Recurso ausente | No se puede abrir o escribir el almacén local indicado. | El almacén local no es accesible o no admite escritura de metadatos. | Verificar que el almacén local exista y admita escritura, y reinicializar; en consulta, el motor devuelve el estado en memoria y marca la cola como no legible. | CU-01, CU-05 |
| SESION_YA_INICIALIZADA | Conflicto de estado | Ya existe una sesión activa para el identificador de host {host}. | Se solicitó inicializar por segunda vez una sesión que ya está activa en memoria. | Reutilizar la referencia a la sesión vigente que devuelve el motor en lugar de inicializar otra. | CU-01 |

### 3.2 Cola de cambios pendientes

| Código | Categoría | Mensaje | Causa probable | Acción sugerida | CU |
| --- | --- | --- | --- | --- | --- |
| IDENTIFICADOR_CAMBIO_AUSENTE | Entrada inválida | El cambio local no trae un identificador de cambio estable. | Se intentó encolar un cambio sin identificador estable. | Asignar un identificador de cambio estable al cambio antes de encolarlo; la cola no se modificó. | CU-02 |
| SESION_NO_INICIALIZADA | Recurso ausente | No hay una sesión de sincronización inicializada. | Se intentó encolar o consultar sin haber inicializado la sesión. | Inicializar la sesión (CU-01) antes de encolar o consultar. | CU-02, CU-05 |
| ALMACEN_LOCAL_SIN_ESPACIO | Error transitorio | No se puede persistir el cambio por falta de espacio en el almacén local. | El almacén local no tiene espacio para la nueva entrada de la cola. | Liberar espacio en el almacén local y reintentar el encolado; el motor no dejó una entrada parcial. | CU-02 |

### 3.3 Ejecución de la sincronización y conectividad

| Código | Categoría | Mensaje | Causa probable | Acción sugerida | CU |
| --- | --- | --- | --- | --- | --- |
| BACKEND_INALCANZABLE | Error transitorio | El backend remoto no responde. | La conectividad se perdió al iniciar o durante la fase de subida. | Reintentar cuando vuelva la conectividad; el motor conservó la cola de pendientes, no inició la bajada y dejó la sesión recuperable o reanudable. | CU-03, CU-06 |
| CREDENCIAL_INVALIDA | Autenticación | El backend rechazó la credencial provista. | La credencial venció o no es válida para el backend. | Renovar la credencial en el host y reejecutar; el motor no subió ni bajó nada y no alteró la cola. | CU-03 |
| SUBIDA_INCOMPLETA | Error transitorio | La fase de subida terminó con cambios pendientes sin confirmar. | Un corte de conexión interrumpió la subida antes de confirmar todos los cambios. | Reanudar el ciclo (CU-06); el motor no inició la bajada, conservó los pendientes y dejó la sesión reanudable. | CU-03 |

### 3.4 Disparo automático por conectividad

| Código | Categoría | Mensaje | Causa probable | Acción sugerida | CU |
| --- | --- | --- | --- | --- | --- |
| DISPARO_AUTOMATICO_DESHABILITADO | Conflicto de estado | Llegó un evento de red disponible pero el disparo automático no está habilitado. | El host no habilitó el disparo automático. | Habilitar el disparo automático si se desea sincronización al recuperar la red, o disparar el ciclo manualmente (CU-03); el evento se registró como ignorado. | CU-04 |
| SESION_NO_AUTENTICADA | Autenticación | Hay red disponible pero la sesión no tiene credencial vigente. | La sesión se inicializó sin credencial o la credencial dejó de estar vigente. | Proveer una credencial vigente al motor; mientras tanto, el motor admite encolar pero no dispara el ciclo. | CU-04, CU-01 |
| FUENTE_CONECTIVIDAD_AUSENTE | Recurso ausente | No hay una fuente de eventos de conectividad suscripta. | Se intentó habilitar el modo automático sin suscribir una fuente de eventos de conectividad. | Suscribir una fuente de eventos de conectividad del host o de la plataforma antes de habilitar el modo automático. | CU-04 |

### 3.5 Reanudación y progreso

| Código | Categoría | Mensaje | Causa probable | Acción sugerida | CU |
| --- | --- | --- | --- | --- | --- |
| SESION_NO_REANUDABLE | Conflicto de estado | Se solicitó reanudar una sesión que no quedó en estado reanudable. | El último ciclo no terminó con una subida parcial. | No es necesario reanudar: el motor trata la solicitud como un ciclo normal (CU-03), o la rechaza si la sesión no está autenticada; el motor no inventa progreso. | CU-06 |
| PROGRESO_INCONSISTENTE | Error transitorio | La marca de progreso y la cola de pendientes no concuerdan al reanudar. | El registro de progreso quedó desfasado respecto de la cola tras un corte. | Ninguna acción manual: el motor adopta la cola persistida como fuente de verdad, reenvía los pendientes apoyándose en la idempotencia (RN-02) y registra la inconsistencia para diagnóstico. | CU-06 |

### 3.6 Convivencia con conflicto (condición reportada, no error de bloqueo)

| Código | Categoría | Mensaje | Causa probable | Acción sugerida | CU |
| --- | --- | --- | --- | --- | --- |
| ELEMENTO_EN_CONFLICTO | Conflicto de estado (reportado, no bloqueante) | El backend marcó una o más entidades como en conflicto; el motor las aplicó como estado válido y las reporta. | Dos o más estados del backend caen en una situación de conflicto que la solución resuelve en otro momento. | Consultar los elementos en conflicto (CU-05) y resolverlos en el backend o en el host; el motor convive con ellos, no aborta el ciclo y no decide la unificación (RN-03). | CU-03, CU-05 |

El elemento en conflicto no detiene la sincronización: se incluye en el resumen del ciclo y se expone en la consulta de estado como conviviente. No es un error de bloqueo, sino una condición que el motor reporta para que otro actor decida.

## 4. Tono y voz

- Voz del motor: descriptiva, neutral, en tercera persona sobre lo que el motor hizo o detectó ("el motor conservó la cola", "el backend rechazó la credencial").
- Registro: español rioplatense técnico, conciso, sin tecnicismos de un stack concreto. La acción sugerida se redacta en imperativo orientado al integrador ("proveer", "reintentar", "reanudar").
- Sin alarma innecesaria: las condiciones transitorias de conectividad se redactan como esperadas en el trabajo sin conexión, recalcando que no hay pérdida ni duplicación de datos, en coherencia con la garantía de la librería (RN-01, RN-02).
- Coherencia con la guía de estilo del proyecto: el tono se alinea con la redacción de la documentación de la categoría 10 (Technical Writer) cuando exista; hasta entonces, rige este documento como referencia de voz para los mensajes de la superficie pública.

## 5. Localización

- El código de error es estable, en ASCII y nunca se traduce: es la clave del contrato y la base de la trazabilidad y de la telemetría opt-in (`dx-developer-experience_v1.0.md` §7).
- El mensaje, la causa probable y la acción sugerida son texto traducible. El idioma base es el español rioplatense técnico de este catálogo.
- La traducción no altera la semántica del código ni la acción sugerida: una traducción que cambie el sentido de la acción se considera defectuosa.
- Los marcadores de detalle (por ejemplo {campo}, {host}) se preservan en cualquier traducción para conservar el diagnóstico específico.
- La política de traducción de los mensajes técnicos se coordina con la guía de estilo del proyecto (categoría 10); este documento fija el contrato de los códigos y la estructura de los mensajes, no el conjunto de idiomas, que se confirma con el negocio.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Catálogo inicial de errores del motor aplicada-sync: principios de redacción, taxonomía de cinco categorías, catálogo agrupado por escenario (inicialización, cola, ejecución y conectividad, disparo automático, reanudación y convivencia con conflicto) derivado de las tablas de excepciones de CU-01 a CU-06, tono y voz, y política de localización con código estable. |
