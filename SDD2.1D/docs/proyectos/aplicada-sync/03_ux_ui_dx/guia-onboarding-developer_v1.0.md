# Guía de onboarding del developer — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** guia-onboarding-developer_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** DX Lead
**Variante:** DX

## 0. Superficie pública que documenta

Esta guía es el modo tutorial del plan Diátaxis del paquete distribuible `aplicada-sync` (ver `dx-developer-experience_v1.0.md` §4). Acompaña al integrador en su primera hora con la librería, recorriendo la superficie pública del motor de sincronización descrita en la categoría 02: inicializar la sesión (CU-01), encolar un cambio local (CU-02), ejecutar un ciclo subir-luego-bajar (CU-03), habilitar el disparo automático (CU-04), consultar el estado (CU-05) y reanudar tras una interrupción (CU-06). El recorrido se describe en pasos y comportamiento observable; el código ejecutable vive en la categoría 11 y el stack en la categoría 05.

## 1. Audiencia y prerrequisitos

Audiencia: developer integrador de nivel intermedio que incorpora el paquete distribuible a una aplicación que trabaja parte del tiempo sin conexión (ver `dx-developer-experience_v1.0.md` §1).

Prerrequisitos conceptuales que el integrador ya debe poder resolver en su aplicación host, porque el motor los delega y no los implementa:

| Prerrequisito | Por qué lo necesita el motor |
| --- | --- |
| Un almacén local del host accesible para escritura | El motor conserva allí la cola de cambios pendientes y los metadatos de sincronización (CU-01, CU-02). |
| Un proveedor de credencial vigente | El motor reutiliza la credencial del host; no la emite ni la renueva (CU-01, CU-03). |
| El punto de acceso de un backend remoto que implemente el contrato de sincronización | El motor sube cambios por identificador estable y baja actualizaciones; el backend debe reconocer los identificadores para sostener la idempotencia (RN-02). |
| Una fuente de eventos de conectividad (solo para el modo automático) | El motor observa esos eventos para disparar la sincronización; no detecta la red de bajo nivel (CU-04). |

No es prerrequisito conocer patrones de sincronización offline-first: la garantía de orden y de no duplicación la aporta el motor.

## 2. Instalación o acceso

Pasos mínimos verificables para dejar el motor disponible y una sesión inicializada. Descritos en comportamiento; la materialización ejecutable está en el sample `01-basico` (11).

1. Incorporar el paquete distribuible a la aplicación host desde el repositorio público de distribución del proyecto. Verificación: el paquete queda disponible para configurar el motor en la aplicación.
2. Preparar un almacén local del host accesible para escritura de metadatos. Verificación: la aplicación host puede abrir y escribir en ese almacén.
3. Armar la configuración de la sesión y solicitar la inicialización (CU-01): identificador del host, referencia al almacén local, referencia al backend remoto y proveedor de credencial. Verificación: el motor devuelve un identificador de sesión no vacío y estado "listo" (o "no autenticada" si todavía no hay credencial).

Hito verificable del tramo de 5 minutos: consultar el estado de la sesión recién inicializada (CU-05) devuelve estado "listo" y cola vacía.

## 3. Primer ejemplo ejecutable

Recorrido que produce un resultado visible: un cambio local capturado sin conexión llega al backend y la cola vuelve a cero. El snippet ejecutable concreto vive en el sample `01-basico` de la categoría 11; aquí se describe la secuencia y el comportamiento esperado, sin sintaxis de un stack.

1. Con la sesión inicializada y autenticada, encolar un cambio local con un identificador de cambio estable, su tipo u operación, su carga útil (opaca para el motor) y su marca de orden de creación (CU-02). Resultado visible: el motor confirma el encolado y reporta tamaño de cola igual a 1.
2. Ejecutar el ciclo de sincronización (CU-03). Resultado visible: el motor sube primero el cambio pendiente y, recién después, baja las actualizaciones disponibles; devuelve un resumen con un cambio subido antes de cualquier bajada y la cantidad de actualizaciones bajadas.
3. Consultar el estado (CU-05). Resultado visible: estado "listo", cero pendientes y la marca de última sincronización avanzada.

Comprobación del primer valor: si se repite el encolado del mismo cambio con su identificador estable antes de sincronizar (CU-02 flujo 5.A), la cola no crece: el motor conserva una sola entrada por identificador. Esa observación muestra la idempotencia (RN-02) en acción y anticipa por qué la reanudación es segura.

Este es el resultado que mide el time-to-first-success del marco DX (`dx-developer-experience_v1.0.md` §6).

## 4. Diagnóstico de problemas frecuentes en la primera hora

| Síntoma observado | Causa probable | Acción sugerida | Referencia |
| --- | --- | --- | --- |
| La inicialización se rechaza apenas comienza | Falta un campo obligatorio de la configuración (almacén local, backend remoto o identificador de host) | Completar el campo faltante que indica el detalle del error y reinicializar | CU-01, error CONFIGURACION_INCOMPLETA |
| El motor admite encolar pero no deja ejecutar el ciclo | La sesión quedó en estado "no autenticada" por haberse inicializado sin credencial | Proveer una credencial vigente al motor y reintentar la ejecución | CU-01 flujo 5.B; CU-03 |
| El encolado se rechaza y la cola no cambia | El cambio local llegó sin identificador de cambio estable | Asignar un identificador estable al cambio antes de encolarlo | CU-02, error IDENTIFICADOR_CAMBIO_AUSENTE |
| Volver a encolar un cambio no aumenta la cola | Comportamiento esperado: el motor no duplica entradas por identificador | Ninguna; es la idempotencia (RN-02) operando | CU-02 flujo 5.A |
| El ciclo se detiene en la subida y no baja nada | El backend dejó de responder durante la fase de subida | Reintentar cuando vuelva la conectividad; la sesión queda reanudable y no se pierden cambios | CU-03, error BACKEND_INALCANZABLE; CU-06 |
| La sesión quedó en estado "reanudable" tras un corte | La fase de subida terminó con cambios no confirmados por un corte de conexión | Reanudar el ciclo; el motor reenvía solo los faltantes y no duplica los ya recibidos | CU-03, error SUBIDA_INCOMPLETA; CU-06 |
| El disparo automático no ejecuta al volver la red | El disparo automático no está habilitado, o la sesión no está autenticada, o no hay fuente de conectividad suscripta | Habilitar el disparo automático, asegurar credencial vigente y suscribir una fuente de eventos de conectividad | CU-04, errores DISPARO_AUTOMATICO_DESHABILITADO, SESION_NO_AUTENTICADA, FUENTE_CONECTIVIDAD_AUSENTE |
| Aparecen elementos marcados en conflicto tras una bajada | El backend reportó entidades en conflicto; el motor convive con ellas y no las resuelve | Consultar los elementos en conflicto y resolverlos en el backend o en el host; el motor no decide la unificación | CU-05 flujo 5.B; RN-03 |

El catálogo completo de errores, con código, categoría, causa probable y acción sugerida, está en `dx-error-messages_v1.0.md`.

## 5. Próximos pasos

Enlaces explícitos a los modos de documentación del plan Diátaxis (`dx-developer-experience_v1.0.md` §4) para continuar después de la primera hora:

- Modo how-to (`docs/how-to/`): habilitar el disparo automático ante recuperación de conectividad (CU-04); diagnosticar y reanudar una sincronización interrumpida (CU-06); consultar la cola y los elementos en conflicto (CU-05); operar el modo no autenticado (CU-01 flujo 5.B).
- Modo reference (`docs/reference/`): el contrato completo de cada operación de la superficie pública (CU-01 a CU-06), con sus flujos alternativos y sus códigos de error.
- Modo explanation (`docs/explanation/`): por qué el orden subir-antes-de-bajar no es configurable (RN-01); por qué la idempotencia descansa en el identificador de cambio estable (RN-02); por qué el motor convive con el conflicto y no lo resuelve (RN-03).
- Catálogo de errores: `dx-error-messages_v1.0.md` para diagnosticar cualquier código devuelto por el motor.
- Sample de integración avanzada: la demostración ajena al sistema del proyecto (intake §18) para evaluar la reutilización del motor fuera de la solución, en la categoría 11.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la guía de onboarding del integrador de aplicada-sync: prerrequisitos delegados al host, acceso e inicialización verificables, primer ejemplo que muestra el ciclo y la idempotencia, diagnóstico de la primera hora y próximos pasos por modo Diátaxis. Derivada de los CU-01 a CU-06 y RN-01 a RN-03 de la categoría 02 y del marco DX. |
