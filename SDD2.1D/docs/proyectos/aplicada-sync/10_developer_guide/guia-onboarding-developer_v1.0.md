# Guía de onboarding del developer — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** guia-onboarding-developer_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-15
**Autor:** Technical Writer + SDK Documentation Lead (AG-10)
**Tipo Diátaxis:** Tutorial
**Audiencia:** Developer integrador que consume la librería desde su propia aplicación
**Nivel:** Básico
**Tiempo estimado de lectura:** 14 min

Este tutorial lleva a un integrador nuevo desde cero hasta integrar el motor de sincronización en su aplicación, en menos de una hora, en tres tramos verificables: Hello world (5 min), primer caso real (30 min) e integración con un sistema host (1 hora). Cada paso declara su efecto esperado en comportamiento observable. El código ejecutable concreto vive en la categoría 11 (samples `01-basico`, `02-intermedio`); acá se describen los pasos y lo que el integrador debe observar. El vocabulario en `código kebab` se define en `glosario-tecnico_v1.0.md`.

## 1. Prerequisites

El motor delega cuatro recursos al host y no los implementa. Antes de empezar, el integrador debe poder proveer al menos los dos primeros.

| Prerequisito | Cómo obtenerlo | Por qué lo necesita el motor |
| --- | --- | --- |
| El paquete distribuible incorporado al proyecto host | Incorporarlo desde el repositorio público de distribución del proyecto (ver el README de la sección). | Da acceso a la superficie pública para configurar el motor. |
| Una `estrategia-de-almacen-local` (obligatoria) | Implementar el contrato de almacén local sobre el almacenamiento del host. | El motor conserva allí la `cola-local` y los metadatos. |
| Una `estrategia-de-transporte` (obligatoria) | Implementar el contrato de transporte contra un backend de prueba que reconozca el `identificador-de-cambio-estable`. | El motor sube por identificador y baja actualizaciones. |
| Un `proveedor-de-credencial` (opcional) | Implementar el contrato que entrega la credencial vigente del host. | Sin él la sesión queda `no autenticada`: encola pero no ejecuta. |
| Una `fuente-de-conectividad` (opcional) | Implementar el contrato que notifica transiciones de red. | Solo para el `disparo-automatico` (tramo de integración). |

No es prerequisito dominar patrones de sincronización offline-first: el `orden-subir-antes-de-bajar`, la `idempotencia` y la `reanudacion` los aporta el motor (ver `conceptos-fundamentales_v1.0.md` §3). El detalle de cada contrato está en `referencia-api_v1.0.md` §4.

## 2. Hello world (5 min)

Objetivo: dejar el motor disponible y una `sesion` inicializada. Materialización ejecutable: sample `01-basico` (11).

| Paso | Acción | Efecto esperado |
| --- | --- | --- |
| 1 | Incorporar el paquete distribuible al proyecto host. | El paquete queda disponible para configurar el motor. |
| 2 | Preparar un almacén local accesible para escritura e inyectar la `estrategia-de-almacen-local` y la `estrategia-de-transporte` (contra un backend de prueba). | Las dos estrategias obligatorias quedan listas para registrarse. |
| 3 | Armar la `configuracion-de-sesion` (identificador de host, almacén local, transporte; sin credencial por ahora) y solicitar Inicializar sesión (CU-01). | El motor devuelve un identificador de sesión no vacío y estado `no autenticada` (o `listo` si ya se proveyó credencial). |
| 4 | Consultar estado y cola (CU-05). | Estado válido del conjunto cerrado y cola vacía (tamaño 0). |

Hito verificable del tramo: la consulta del paso 4 devuelve un estado conocido y cola en cero. Si la inicialización se rechaza, ver `troubleshooting_v1.0.md` ISSUE-05 (`CONFIGURACION_INCOMPLETA`).

## 3. Primer caso real (30 min)

Objetivo: encolar uno o más cambios y correr un `ciclo-de-sincronizacion` completo, observando el `resumen-del-ciclo`. Datos representativos pero acotados: tres cambios con identificadores estables. Materialización ejecutable: sample `01-basico` / `02-intermedio` (11).

| Paso | Acción | Efecto esperado |
| --- | --- | --- |
| 1 | Proveer una credencial vigente vía `proveedor-de-credencial`. | La sesión pasa de `no autenticada` a `listo`. |
| 2 | Encolar un `cambio-local` con identificador `obs-001`, su operación, su carga útil opaca y su marca de orden (CU-02). | El motor confirma el encolado y reporta tamaño de cola = 1. |
| 3 | Reencolar el mismo `obs-001`. | El tamaño de cola sigue en 1: el motor no duplica por identificador (idempotencia, RN-02; ver `conceptos-fundamentales_v1.0.md` decisión D-04). |
| 4 | Encolar `obs-002` y `obs-003`. | Tamaño de cola = 3. |
| 5 | Ejecutar sincronización (CU-03). | El `resumen-del-ciclo` muestra los subidos (3) antes de cualquier bajado, y luego la cantidad de bajados. |
| 6 | Consultar estado (CU-05). | Estado `listo`, pendientes = 0 y la `marca-de-ultima-sincronizacion` avanzada. |

Hito verificable del tramo: un cambio capturado localmente viaja al backend y la cola vuelve a cero. La observación del paso 3 muestra la idempotencia en acción y anticipa por qué la reanudación es segura.

## 4. Integración con un sistema host (1 hora)

Objetivo: integrar el motor en una aplicación host real, habilitando el `disparo-automatico` y comprobando la `reanudacion` sin pérdida ante un corte. Este tramo es el puente al how-to: para el detalle completo, ver `guia-integracion-aplicacion-movil_v1.0.md`.

| Paso | Acción | Efecto esperado |
| --- | --- | --- |
| 1 | Inyectar una `fuente-de-conectividad` del host y habilitar el disparo automático (CU-04). | El motor queda a la espera de eventos de recuperación de red. |
| 2 | Con cambios encolados, simular una recuperación de conectividad. | El motor dispara un ciclo sin invocación manual y notifica el resultado (evento de resultado de ciclo, `referencia-api_v1.0.md` §5). |
| 3 | Encolar cinco cambios y simular un corte en la fase de subida tras confirmar dos. | El ciclo termina con `SUBIDA_INCOMPLETA`, la sesión queda `reanudable` y ninguna bajada se aplicó. |
| 4 | Consultar estado durante el corte. | Estado `reanudable`; la cola conserva los tres no confirmados. |
| 5 | Reanudar sincronización (CU-06). | El `resumen-de-reanudacion` muestra que solo se reenviaron los tres faltantes (nuevos confirmados = 3, reconocidos = 0) y luego bajó; estado final `listo`. |

Hito verificable del tramo (criterio duro de éxito): el integrador comprueba en su propia aplicación que un corte no perdió ni duplicó datos y que el orden subir-antes-de-bajar se respetó. Si al final del paso 5 esto no se cumple, la integración no está terminada: revisar `troubleshooting_v1.0.md` ISSUE-02 y ISSUE-04.

## 5. Siguientes pasos

Tres rutas para continuar después de la primera hora:

- Profundizar en el modelo mental: `conceptos-fundamentales_v1.0.md` (por qué el orden no es configurable, por qué la idempotencia descansa en el identificador, por qué el motor convive con el conflicto).
- Consultar el contrato exacto: `referencia-api_v1.0.md` (operaciones, formas de datos, contratos de extensión, eventos y excepciones).
- Ver código ejecutable: samples `01-basico`, `02-intermedio` y `03-avanzado-demo-maui` en la categoría 11 (el último es la demostración de integración en una app host móvil ajena a la solución).

Para resolver una tarea concreta de integración, ir directo a `guia-integracion-aplicacion-movil_v1.0.md`. Para diagnosticar un error, a `troubleshooting_v1.0.md`.

## 6. Referencias cruzadas

- 05 `contratos-abstractions_v1.0.md` §3: operaciones que el tutorial ejercita.
- 05 `extensibilidad_v1.0.md` §3 y §5: los puntos de extensión que el integrador implementa e inyecta, y el registro explícito.
- 05 `flujo-ejecucion_v1.0.md` §3 y §5: pipeline y reanudación que sustentan los tramos 3 y 4.
- 08 `estrategia-testing_v1.0.md` §3 y §5: cómo el integrador ejercita el motor contra dobles del backend para reproducir el corte del tramo 4, sin redefinir la estrategia.
- `conceptos-fundamentales_v1.0.md`, `referencia-api_v1.0.md`, `guia-integracion-aplicacion-movil_v1.0.md`, `troubleshooting_v1.0.md`, `glosario-tecnico_v1.0.md`.

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Tutorial de onboarding inicial del integrador de aplicada-sync: prerequisites delegados al host, Hello world en 5 min (inicializar y consultar), primer caso real en 30 min (encolar, idempotencia, ciclo completo) e integración en 1 hora (disparo automático y reanudación sin pérdida), con efecto esperado por paso y puente al how-to. Derivado de CU-01 a CU-06, del marco DX de 03 y de la arquitectura de 05. |
