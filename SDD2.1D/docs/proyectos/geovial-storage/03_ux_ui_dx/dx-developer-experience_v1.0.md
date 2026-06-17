# DX — Experiencia de developer de geovial-storage

**Proyecto:** geovial-storage
**Documento:** dx-developer-experience_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** DX Lead
**Variante:** DX

## 0. Superficie pública documentada

La superficie pública de `geovial-storage` es una abstracción de alojamiento de archivos transparente: un contrato único de operaciones que el consumidor invoca sin saber dónde queda físicamente el archivo. El proveedor que aloja el contenido (proveedor local, proveedor de almacenamiento de objetos remoto u otro proveedor) es intercambiable y lo selecciona el usuario raíz, sin que el consumidor cambie su forma de invocar las operaciones.

Las seis operaciones del contrato, una por caso de uso de la especificación funcional (02), son:

| Operación | CU | Qué hace |
| --- | --- | --- |
| Guardar | CU-01 | Persiste un contenido y devuelve un identificador lógico estable |
| Recuperar | CU-02 | Devuelve el contenido idénticamente igual al guardado, por identificador |
| Eliminar | CU-03 | Quita el contenido asociado a un identificador (idempotente) |
| Verificar existencia | CU-04 | Informa si un identificador corresponde a un archivo presente |
| Listar bajo prefijo | CU-05 | Enumera los identificadores presentes bajo un prefijo, con paginación |
| Configurar proveedor activo | CU-06 | Selecciona y valida el proveedor activo y sus credenciales (usuario raíz) |

Este documento describe el comportamiento esperado y los pasos del recorrido, no el código. Las firmas de tipo, las interfaces concretas y los nombres de proveedor viven en la categoría 05; los ejemplos ejecutables, en la categoría 11.

## 1. Audiencia developer

El consumidor de esta librería es el developer backend que integra `geovial-storage` dentro de `geovial-api`. Es un integrador, no un contributor ni un operador: usa la superficie pública desde su código, no modifica la librería ni opera un servicio en segundo plano.

| Atributo | Valor esperado |
| --- | --- |
| Tipo de developer | Integrador del backend (consume el contrato desde `geovial-api`) |
| Nivel de experiencia | Intermedio: maneja manejo de archivos, inyección de dependencias y manejo de errores; no necesariamente conoce el detalle de cada proveedor de almacenamiento |
| Herramientas que ya conoce | Edición de configuración del backend, ejecución de pruebas, lectura de logs del backend, manejo de binarios |
| Qué busca | Guardar y recuperar las fotografías de los relevamientos sin atarse a un destino físico, y poder cambiar ese destino sin reescribir su código |
| Qué no quiere | Ramas de código por proveedor, mensajes de error ambiguos, ni descubrir credenciales filtradas en un resultado o en un log |

Hay un segundo lector indirecto: el usuario raíz, que no escribe código pero configura el proveedor activo (CU-06). El developer backend es quien le expone esa configuración; por eso la guía de configuración del proveedor también es parte de la experiencia DX, aunque su destinatario final sea el usuario raíz.

Contexto del proyecto: el implementador cuenta con un único desarrollador (00 §2), por lo que la documentación prioriza autoservicio y diagnóstico autónomo sobre soporte sincrónico.

## 2. Onboarding por tramos

Cada tramo tiene un objetivo verificable: un hecho observable que confirma que el developer llegó al hito. Los tramos son acumulativos.

| Tramo | Objetivo | Hito verificable |
| --- | --- | --- |
| 5 minutos | Obtener un primer resultado exitoso con el proveedor local | El developer guarda un contenido de prueba con el proveedor local activo y recibe a cambio un identificador lógico no vacío; al recuperar ese identificador obtiene un contenido idénticamente igual al guardado |
| 30 minutos | Integrar el contrato en su propio flujo con manejo de errores | El developer guarda una fotografía de un relevamiento bajo un prefijo propio (por ejemplo `relevamientos/2026/r-001/`), verifica su existencia, la recupera por rango y maneja al menos un error catalogado (identificador inexistente o contenido vacío) sin que el flujo se caiga |
| 60 minutos | Cambiar el proveedor activo sin tocar su código de integración | El usuario raíz, asistido por el developer, valida en seco y luego activa un proveedor de almacenamiento de objetos remoto; las mismas operaciones que el developer ya escribió siguen funcionando sin cambios, y un listado bajo prefijo enumera los archivos del relevamiento |

El tramo de 60 minutos materializa la promesa de transparencia (RN-01): el éxito se mide en que el código de integración del developer no cambia al cambiar el proveedor. Es el time-to-first-value de la librería.

## 3. Quick-start

Recorrido mínimo, verificable y reproducible para producir el primer resultado exitoso (hito del tramo de 5 minutos). Se describe en pasos y comportamiento observable, sin código de stack concreto. El código vive en 11; el stack y los nombres de proveedor, en 05.

Precondiciones:

- El developer tiene la librería disponible para el backend (el detalle de incorporación pertenece a 05/11).
- No se requieren credenciales remotas: el proveedor local siempre está disponible como mínimo (CU-06, precondición).

Pasos:

1. Configurar el proveedor activo como proveedor local, indicando una ubicación local accesible y escribible (CU-06, FA-01). Resultado esperado: la librería confirma que el proveedor local quedó activo.
2. Invocar la operación de guardado entregando un contenido de prueba no vacío, un prefijo de destino válido (por ejemplo `pruebas/quick-start/`) y un tipo de contenido. Resultado esperado: la librería devuelve un identificador lógico no vacío y el tamaño persistido (CU-01, paso 5).
3. Invocar la operación de verificación de existencia con ese identificador. Resultado esperado: la librería devuelve presencia verdadera (CU-04, CA-01).
4. Invocar la operación de recuperación con ese identificador. Resultado esperado: la librería devuelve un contenido idénticamente igual al guardado, byte a byte (CU-02, CA-01; RN-02).
5. Opcional: invocar la operación de eliminación con ese identificador y verificar de nuevo. Resultado esperado: la eliminación se confirma y la verificación pasa a presencia falsa (CU-03, CA-01; CU-04, CA-03).

Criterio de éxito del quick-start: el contenido recuperado en el paso 4 es idéntico al guardado en el paso 2. Si esto se cumple, el developer logró el primer resultado exitoso.

Verificación previa a publicar: el quick-start se ejecuta a mano contra el proveedor local y contra un proveedor de almacenamiento de objetos remoto antes de cada liberación; ambos deben producir el mismo comportamiento observable (RN-01).

## 4. Diátaxis

Plan explícito de los cuatro modos de documentación. Cada modo tiene una ubicación lógica y enlaces a los demás. La ubicación física exacta (rutas de archivo, formato) la concreta 11; aquí se fija el plan y el reparto de responsabilidades.

| Modo | Orientación | Ubicación lógica | Contenido | Enlaza a |
| --- | --- | --- | --- | --- |
| Tutorial | Aprendizaje | `guia-onboarding-developer_v1.0.md` (esta sección) y su materialización en 11 | Primera hora del integrador: del proveedor local al cambio de proveedor, paso a paso | How-to y reference como próximos pasos |
| How-to | Tarea | Recetas por operación, materializadas en 11 | Guardar una foto de relevamiento, recuperar por rango, listar con paginación, eliminar bajo prefijo, validar un proveedor remoto en seco | Reference de cada operación; tutorial para el contexto |
| Reference | Información | Categoría 05 (contrato y tipos) más el catálogo de errores de esta sección | Cada operación con sus parámetros, resultados y códigos de error; nota de compatibilidad de versión pública (02 §6) | Catálogo `dx-error-messages_v1.0.md`; how-to que la usan |
| Explanation | Comprensión | Esta sección (§0 y §1) y la categoría 05 (decisión de arquitectura) | Por qué la abstracción es transparente (RN-01), por qué la integridad es byte a byte (RN-02), por qué las credenciales nunca salen (RN-03) | Reglas de negocio de 02; reference del contrato |

Regla de separación: el tutorial no documenta cada parámetro (eso es reference); el reference no enseña un recorrido (eso es tutorial); el how-to resuelve una tarea concreta sin explicar la teoría (eso es explanation). Los cuatro modos se enlazan entre sí, no se mezclan dentro de un mismo documento.

## 5. Mensajes de error y diagnóstico

Principio rector: cada error que la librería devuelve por su superficie pública dice qué pasó, por qué pasó y qué hacer al respecto, en lenguaje plano y sin culpar al developer.

- Qué pasó: la operación afectada y el resultado (rechazo antes de delegar en el proveedor, o fallo del proveedor).
- Por qué pasó: la condición que disparó el error (contenido vacío, identificador inexistente, proveedor no disponible, etcétera).
- Qué hacer: la acción concreta de corrección o el siguiente paso.

Dos invariantes condicionan los mensajes:

- Transparencia (RN-01): el conjunto de códigos de error es el mismo cualquiera sea el proveedor activo. Un mensaje no debe contener un código ni un texto que dependa del proveedor; el developer no debe necesitar ramas de código por proveedor para interpretarlo.
- Manejo seguro de credenciales (RN-03): ningún mensaje de error incluye credenciales ni parámetros de conexión del proveedor. El error de proveedor no disponible se propaga de forma uniforme y silenciosa respecto de la configuración sensible.

El catálogo completo de errores de la abstracción de almacenamiento, con código, categoría, causa probable y acción sugerida, vive en `dx-error-messages_v1.0.md` (esta sección).

## 6. Métricas DX

| Métrica | Definición | Objetivo | Cómo se mide |
| --- | --- | --- | --- |
| TTFS | Tiempo desde que el developer tiene la librería disponible hasta el primer resultado exitoso (recuperar un contenido idéntico al guardado con el proveedor local) | <= 5 minutos | Pruebas con tres a cinco developers ejecutando el quick-start, cronometradas; telemetría opt-in si el equipo la habilita |
| TTFV | Tiempo hasta el primer valor: cambiar el proveedor activo sin modificar el código de integración del backend | <= 1 hora | Recorrido completo de los tres tramos de onboarding con un developer de adopción reciente |
| Tasa de error en onboarding | Porcentaje de developers que abandonan antes de completar el tramo de 5 minutos | <= 20 % | Observación de las pruebas de onboarding y registro de puntos de abandono |
| Tasa de errores autodiagnosticables | Porcentaje de errores del catálogo que el developer resuelve solo con el mensaje, sin consultar a otra persona | >= 90 % | Pruebas con developers frente a cada error catalogado; dado el implementador de un único desarrollador, el autoservicio es prioritario |

## 7. Feedback loop

Como el implementador es un único desarrollador (00 §2), el ciclo de feedback es liviano y orientado al autoservicio:

- Issues etiquetados como `dx` en el repositorio de la solución, separando "error de integración" de "documentación poco clara".
- Sección de discusiones del repositorio para preguntas de integración del backend que no son defectos.
- Encuesta breve al cierre del primer mes de integración, centrada en el TTFS, la claridad de los mensajes de error y la transparencia percibida al cambiar de proveedor.
- Telemetría opt-in (con consentimiento explícito) sobre el quick-start, solo si el equipo decide habilitarla; nunca registra contenido de archivos ni credenciales (RN-03).

Incorporación al ciclo: cada hallazgo recurrente de "documentación poco clara" se traduce en un ajuste del modo Diátaxis correspondiente; cada error mal diagnosticado, en una corrección del catálogo de `dx-error-messages`. Las correcciones siguen la política de versionado de §3.5 de las reglas 03.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Developer backend integrador de `geovial-api`; usuario raíz como lector indirecto de la configuración (00 §2) |
| Superficie pública documentada | Abstracción de almacenamiento: guardar, recuperar, eliminar, verificar, listar, configurar proveedor activo |
| CU upstream | CU-01, CU-02, CU-03, CU-04, CU-05, CU-06 (02) |
| Reglas de negocio relevantes | RN-01 (transparencia), RN-02 (integridad), RN-03 (manejo seguro de credenciales) |
| Necesidad de negocio raíz | NB-07 (almacenamiento configurable); NB-03 y NB-06 como soporte |
| Artefactos hermanos en 03 | guia-onboarding-developer_v1.0.md, dx-error-messages_v1.0.md |
| US a generar (en 06) | US-01 a US-09 (las nueve US que originan los seis CU) |
| Tests previstos (en 08) | Prueba de quick-start automatizada; batería de contrato ejecutada contra cada proveedor (RN-01); prueba de igualdad binaria guardar-recuperar (RN-02); prueba de que ningún error filtra credenciales (RN-03) |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Marco DX inicial de geovial-storage: audiencia integrador del backend, onboarding por tramos 5/30/60 verificables, quick-start del proveedor local, plan Diátaxis de los cuatro modos, principios de mensajes de error, métricas DX y feedback loop para un equipo de un solo desarrollador. |
