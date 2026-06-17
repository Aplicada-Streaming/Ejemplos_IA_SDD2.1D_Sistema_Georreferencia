# DX — Motor de sincronización aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** dx-developer-experience_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** DX Lead
**Variante:** DX

## 0. Superficie pública que documenta

Este documento define el marco de experiencia de developer (DX) del paquete distribuible `aplicada-sync`, un motor de sincronización agnóstico del dominio. La superficie pública documentada es el contrato del ciclo de vida del motor descrito en la especificación funcional de la categoría 02:

- Inicializar y configurar la sesión de sincronización (CU-01).
- Registrar y encolar un cambio local (CU-02).
- Ejecutar la sincronización subir-luego-bajar (CU-03).
- Detectar conectividad y disparar la sincronización de forma automática (CU-04).
- Consultar el estado del motor y la cola de pendientes (CU-05).
- Reanudar una sincronización interrumpida (CU-06).

Y las tres invariantes que el motor garantiza en cualquier integración: orden estricto subir-antes-de-bajar (RN-01), idempotencia de la sincronización (RN-02) y convivencia con estados en conflicto sin bloqueo (RN-03).

La superficie se describe de forma abstracta. El código ejecutable real del quick-start y de los ejemplos vive en la categoría 11 (examples); el stack concreto y el transporte viven en la categoría 05 (arquitectura). Este documento no menciona stacks ni protocolos.

## 1. Audiencia developer

El consumidor primario es un developer integrador: incorpora `aplicada-sync` a una aplicación que corre en un dispositivo del lado del cliente, que trabaja parte del tiempo sin conexión y necesita propagar sus cambios a un backend remoto cuando la red vuelve.

| Atributo | Descripción |
| --- | --- |
| Tipo de developer | Integrador. Consume el paquete distribuible desde su aplicación; no es contributor del motor ni operador de un servicio. |
| Nivel de experiencia esperado | Intermedio. Conoce el manejo de un almacén local del host, el consumo de un backend remoto y el manejo de credenciales de su aplicación; no necesariamente domina patrones de sincronización offline-first. |
| Qué ya conoce | Cómo persistir datos en el almacén local de su aplicación, cómo obtener y renovar la credencial de autenticación de su host, cómo observar eventos de cambio de conectividad de su plataforma. |
| Qué no necesita saber | El detalle interno del motor, el transporte hacia el backend, la estructura física de los metadatos de sincronización (todo eso lo encapsula la librería y se describe en 05). |
| Qué busca al llegar | Propagar cambios capturados sin conexión a un backend remoto con la garantía de no perder ni duplicar datos, sin tener que escribir su propio motor de sincronización. |

Audiencias secundarias, fuera del foco principal de este marco pero contempladas en el feedback loop (§7): el contributor que evalúa el motor para reutilizarlo en otro proyecto (atendido por el sample de demostración ajeno al sistema descrito en el intake §18) y la persona usuaria final de la aplicación host, que nunca interactúa con la librería de forma directa y cuya experiencia pertenece a la categoría 03 de la aplicación host, no a esta.

## 2. Onboarding por tramos

El onboarding se mide en tres tramos verificables. Cada tramo declara un objetivo con un criterio de verificación objetivo, de modo que el integrador (o una prueba con cinco developers) pueda confirmar el avance sin ambigüedad.

| Tramo | Objetivo | Criterio de verificación |
| --- | --- | --- |
| 5 minutos | Incorporar el paquete distribuible e inicializar una sesión de sincronización contra un almacén local y un backend remoto de prueba (CU-01). | El integrador obtiene un identificador de sesión no vacío y un estado inicial "listo". Verificable consultando el estado (CU-05) inmediatamente después de inicializar. |
| 30 minutos | Encolar uno o más cambios locales (CU-02) y ejecutar manualmente un ciclo de sincronización subir-luego-bajar (CU-03), observando el resumen del ciclo. | El integrador ejecuta un ciclo y recibe un resumen con la cantidad de cambios subidos antes de cualquier bajada y la cantidad de actualizaciones bajadas; la cola de pendientes queda vacía de los cambios confirmados. |
| 1 hora | Habilitar el disparo automático ante recuperación de conectividad (CU-04), provocar una interrupción de la subida y comprobar la reanudación sin duplicar datos (CU-06), consultando el estado a lo largo del ciclo (CU-05). | El integrador simula un corte durante la fase de subida, confirma que el estado pasa a "reanudable" y que ninguna actualización descendente se aplicó, y al reanudar verifica que solo se reenvían los cambios faltantes y no se duplica ningún dato. |

El primer tramo es el time-to-first-success (ver §6): el integrador prueba que el motor quedó listo. El tercer tramo es el time-to-first-value: el integrador comprueba en su propio escenario la garantía de negocio que justifica adoptar la librería (no perder ni duplicar datos ante cortes de conexión).

## 3. Quick-start

Quick-start verificable y reproducible, descrito en pasos y comportamiento. El código ejecutable concreto vive en la categoría 11 (samples `01-basico` y `02-intermedio` del intake §16.1); el stack vive en la categoría 05. Aquí se describe la secuencia de pasos y el comportamiento observable, no la sintaxis de un stack.

Precondiciones del quick-start:

- El integrador dispone de un almacén local del host accesible para escritura de metadatos.
- El integrador conoce el punto de acceso de un backend remoto de prueba que implemente el contrato de sincronización (recibir cambios locales por identificador estable y entregar actualizaciones).
- El integrador puede proveer una credencial vigente, o bien iniciar sin credencial para el modo no autenticado.

Pasos (cinco pasos o menos, reproducibles):

1. Incorporar el paquete distribuible a la aplicación host. Comportamiento esperado: el paquete queda disponible para configurar el motor.
2. Armar la configuración de la sesión (identificador del host, referencia al almacén local, referencia al backend remoto, proveedor de credencial) y solicitar la inicialización (CU-01). Comportamiento esperado: el motor devuelve un identificador de sesión no vacío y estado "listo"; si no se provee credencial, devuelve estado "no autenticada".
3. Encolar un cambio local con un identificador de cambio estable y una marca de orden de creación (CU-02). Comportamiento esperado: el motor confirma el encolado y reporta tamaño de cola igual a 1.
4. Ejecutar el ciclo de sincronización (CU-03). Comportamiento esperado: el motor sube primero el cambio pendiente, luego baja las actualizaciones disponibles y devuelve un resumen con un cambio subido antes de cualquier bajada.
5. Consultar el estado del motor (CU-05). Comportamiento esperado: el motor reporta estado "listo", cero pendientes y la marca de última sincronización avanzada.

Resultado exitoso del quick-start: el integrador observa un cambio local viajar al backend y la cola volver a cero, comprobando de punta a punta el contrato del motor. Este resultado es el primer éxito que mide el TTFS (§6).

Validación del quick-start: el equipo verifica esta secuencia a mano contra el sample `01-basico` antes de cada publicación del paquete. Un quick-start que no reproduzca este comportamiento se considera defectuoso y bloquea la publicación.

## 4. Diátaxis

Plan explícito de los cuatro modos de documentación del paquete distribuible. Cada modo tiene una ubicación lógica en la documentación del paquete y un propósito acotado; los modos se enlazan entre sí para que el integrador transite del aprendizaje a la consulta sin mezclar registros.

| Modo | Orientación | Ubicación lógica | Propósito | Contenido derivado de la superficie pública |
| --- | --- | --- | --- | --- |
| Tutorial | Aprendizaje | `docs/tutorial/` del paquete; materializado por la guía de onboarding (`guia-onboarding-developer_v1.0.md`) y por el sample `01-basico` (11). | Llevar al integrador desde cero hasta el primer ciclo de sincronización exitoso. | Inicializar (CU-01), encolar (CU-02), ejecutar un ciclo (CU-03) en un recorrido guiado. |
| How-to | Tarea | `docs/how-to/` del paquete. | Resolver tareas concretas del integrador. | Habilitar el disparo automático (CU-04); diagnosticar una sincronización interrumpida y reanudarla (CU-06); consultar la cola y los elementos en conflicto (CU-05); operar el modo no autenticado (CU-01 flujo 5.B). |
| Reference | Información | `docs/reference/` del paquete. | Consultar cada operación de la superficie pública, sus entradas, su contrato de retorno y sus códigos de error. | Las seis operaciones (CU-01 a CU-06) con sus flujos alternativos y el catálogo de errores (`dx-error-messages_v1.0.md`). |
| Explanation | Comprensión | `docs/explanation/` del paquete. | Explicar por qué el motor se comporta como lo hace. | El orden subir-antes-de-bajar y por qué no es configurable (RN-01); por qué la idempotencia descansa en el identificador de cambio estable (RN-02); por qué el motor convive con el conflicto y no lo resuelve (RN-03). |

Enlaces entre modos:

- El tutorial cierra cada paso con un enlace al how-to de la tarea relacionada y al reference de la operación usada.
- Cada how-to enlaza al reference de las operaciones que invoca y al modo explanation de la invariante que lo gobierna.
- El reference enlaza al catálogo de errores (`dx-error-messages_v1.0.md`) en cada operación que puede fallar.
- El modo explanation enlaza a las reglas de negocio RN-01, RN-02 y RN-03 de la categoría 02 como fuente normativa.

## 5. Mensajes de error y diagnóstico

Principios de redacción de los mensajes de error de la librería, alineados con el catálogo completo en `dx-error-messages_v1.0.md`. Cada mensaje del motor responde tres preguntas:

- Qué pasó: la condición concreta que el motor detectó, en términos del contrato (por ejemplo, falta un campo de configuración, el backend no respondió, la sesión no está autenticada).
- Por qué pasó: la causa probable expresada de forma accionable, sin culpar al integrador ni a la persona usuaria.
- Qué hacer al respecto: la acción sugerida que devuelve al integrador a un estado correcto (proveer el campo faltante, renovar la credencial, reanudar el ciclo, esperar conectividad).

El motor nunca devuelve mensajes genéricos del tipo "ocurrió un problema": cada condición de error de los CU tiene un código estable y una acción sugerida. Como la librería trabaja sin conexión por diseño, distingue siempre entre errores de configuración del integrador (que requieren corregir la integración) y condiciones transitorias de conectividad (que requieren reintentar o reanudar y no implican un defecto). El catálogo completo, con código, categoría, causa probable y acción sugerida, vive en `dx-error-messages_v1.0.md`.

## 6. Métricas DX

| Métrica | Definición | Objetivo | Cómo se mide |
| --- | --- | --- | --- |
| TTFS (time-to-first-success) | Tiempo desde incorporar el paquete hasta inicializar la sesión y obtener estado "listo" (tramo de 5 minutos del §2). | <= 5 minutos | Pruebas con cinco developers integradores externos sobre el sample `01-basico`; telemetría opt-in del sample de demostración. |
| TTFV (time-to-first-value) | Tiempo desde la primera sincronización hasta comprobar la garantía de no pérdida ni duplicación ante un corte (tramo de 1 hora del §2). | <= 1 hora | Encuesta a integradores en sus primeras dos semanas de adopción. |
| Tasa de error en onboarding | Porcentaje de integradores que abandonan antes de completar el primer ciclo de sincronización exitoso (tramo de 30 minutos). | <= 20 % | Telemetría opt-in del sample y de la guía de onboarding. |
| Claridad de errores | Porcentaje de errores del catálogo que el integrador resuelve sin abrir una consulta de soporte. | >= 80 % | Correlación entre códigos de error reportados y consultas abiertas en el canal de feedback (§7). |

Las métricas son objetivos iniciales del proyecto y se revisan al confirmar la línea de base de adopción, en coherencia con la política de targets revisables de la visión de producto (00).

## 7. Feedback loop

El feedback del integrador se recoge y se incorpora al ciclo de mejora del paquete distribuible por estos canales:

- Reporte de problemas y solicitudes etiquetados como `dx` en el repositorio público del paquete (el intake declara un repositorio público para el proyecto redistribuible).
- Sección de discusiones del repositorio para preguntas de integración y validación del quick-start.
- Encuesta breve al cierre del primer mes de adopción, alineada con la medición de TTFV (§6).
- Telemetría opt-in, con consentimiento explícito del integrador, limitada a los hitos de onboarding (§2) y a los códigos de error catalogados; nunca incluye la carga útil de dominio del host, que es opaca para el motor.

Incorporación al ciclo: cada hallazgo de DX se traduce en una historia de usuario de la categoría 06 o en un ajuste de la documentación Diátaxis (§4); los cambios que alteran el contrato de la superficie pública siguen la política de compatibilidad de la especificación funcional (02 §8) y obligan a un incremento de versión mayor del paquete.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Developer integrador de aplicación móvil offline-first (00, audiencia del implementador y de la aplicación de campo que integra la librería). |
| Superficie pública documentada | Contrato del ciclo de vida del motor: CU-01 a CU-06 (02), gobernado por RN-01, RN-02 y RN-03 (02). |
| CU origen | CU-01, CU-02, CU-03, CU-04, CU-05, CU-06 (02). |
| Reglas de negocio relevantes | RN-01 (orden subir-antes-de-bajar), RN-02 (idempotencia), RN-03 (convivencia con conflicto). |
| US a generar en 06 | US-01 a US-13 del proyecto aplicada-sync, según la matriz NB→CU→RN→US del índice de 02. |
| Tests previstos en 08 | Orden subir-antes-de-bajar; idempotencia ante reenvío y reanudación; continuidad ante estados en conflicto; reanudación tras subida parcial; disparo automático ante conectividad recuperada; verificación reproducible del quick-start. |
| Artefactos DX hermanos | `guia-onboarding-developer_v1.0.md` (recorrido de primera hora), `dx-error-messages_v1.0.md` (catálogo de errores). |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del marco DX de aplicada-sync: audiencia integrador, onboarding por tramos 5/30/60 verificables, quick-start abstracto reproducible, plan Diátaxis sobre la superficie pública (CU-01 a CU-06, RN-01 a RN-03), métricas TTFS/TTFV y feedback loop. Derivado de la especificación funcional de 02, de la visión de producto de 00 y del SOLUTION-INTAKE §17 (aplicada-sync) y §18. |
