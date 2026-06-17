# Referencia de API — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** referencia-api_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-15
**Autor:** Technical Writer + SDK Documentation Lead (AG-10)
**Tipo Diátaxis:** Reference
**Audiencia:** Developer integrador que consume la librería desde su propia aplicación
**Nivel:** Avanzado
**Tiempo estimado de lectura:** 22 min

Referencia de la superficie pública del motor de sincronización. Documenta los tipos públicos, las operaciones que el host invoca, los contratos de extensión que el host implementa, los eventos publicados y las excepciones. Mantiene paridad uno a uno con `contratos-abstractions_v1.0.md` (05) y con `extensibilidad_v1.0.md` (05): toda operación, forma de datos y código de error de esos documentos aparece acá; nada que no esté en el contrato se agrega.

La referencia describe el contrato de forma abstracta (operaciones, formas de datos, condiciones), sin sintaxis de un stack. La materialización ejecutable de las firmas vive en la categoría 11 (samples); el stack concreto, en 05 y en el intake §17. El vocabulario en `código kebab` se define en `glosario-tecnico_v1.0.md`.

## 1. Alcance de la superficie pública

La `superficie-publica` versionada se compone de:

- Las seis operaciones del ciclo de vida que el host invoca (§2).
- Las formas de datos del contrato (§3).
- Los cuatro contratos de extensión que el host implementa e inyecta (§4).
- Los eventos publicados por el motor (§5).
- El catálogo de excepciones y códigos estables (§6).

El conjunto de `estado-de-sesion`, la garantía de `orden-subir-antes-de-bajar` y los códigos de error son parte del contrato (05 `contratos-abstractions_v1.0.md` §6). La implementación interna no lo es y puede cambiar sin versión mayor.

## 2. Operaciones

Cada operación lista propósito, entradas (nombre, tipo lógico, obligatorio/opcional), salida, CU de origen y excepciones que puede reportar. La paridad con 05 `contratos-abstractions_v1.0.md` §3 es estricta.

### 2.1 Inicializar sesión

| Aspecto | Detalle |
| --- | --- |
| Propósito | Configurar e inicializar la `sesion` y dejar el motor listo para encolar y ejecutar. |
| Entradas | `configuracion-de-sesion` (obligatorio): identificador de host (obligatorio), referencia al almacén local (obligatorio), referencia al backend remoto (obligatorio), proveedor de credencial (opcional). |
| Salida | Identificador de sesión no vacío + `estado-de-sesion` inicial (`listo`, o `no autenticada` si no se proveyó credencial). |
| CU | CU-01 |
| Excepciones | `CONFIGURACION_INCOMPLETA`, `ALMACEN_LOCAL_INACCESIBLE`, `SESION_YA_INICIALIZADA`. |
| Notas | El motor valida la presencia de los puntos de extensión obligatorios (almacén local, transporte). La ausencia de credencial no es un error: produce el estado `no autenticada`, que admite encolar pero no ejecutar. |

### 2.2 Encolar cambio local

| Aspecto | Detalle |
| --- | --- |
| Propósito | Registrar un `cambio-local` en la `cola-local` de pendientes. |
| Entradas | `cambio-local` (obligatorio): identificador estable (obligatorio), operación, carga útil opaca, marca de orden de creación. |
| Salida | Confirmación de encolado + tamaño de cola resultante. |
| CU | CU-02 |
| Excepciones | `IDENTIFICADOR_CAMBIO_AUSENTE`, `SESION_NO_INICIALIZADA`, `ALMACEN_LOCAL_SIN_ESPACIO`. |
| Notas | Reencolar un cambio con un identificador ya presente no agrega una segunda entrada (no duplica): el tamaño de cola no crece (CU-02 flujo 5.A). Se puede encolar mientras un ciclo está en curso. |

### 2.3 Ejecutar sincronización

| Aspecto | Detalle |
| --- | --- |
| Propósito | Correr el `ciclo-de-sincronizacion` subir-luego-bajar sobre una sesión en estado `listo`. |
| Entradas | Referencia de sesión (obligatorio). |
| Salida | `resumen-del-ciclo`: subidos, bajados, elementos en conflicto, estado final. |
| CU | CU-03 |
| Excepciones | `BACKEND_INALCANZABLE`, `CREDENCIAL_INVALIDA`, `SUBIDA_INCOMPLETA`; condición reportada `ELEMENTO_EN_CONFLICTO`. |
| Notas | Si la cola está vacía, omite la subida y baja directamente (CU-03 flujo 5.A). Si ya hay un ciclo activo, no inicia un segundo: devuelve el estado de la ejecución vigente (CU-03 flujo 5.C). Un corte en la subida deja la sesión `reanudable` y no inicia la bajada. |

### 2.4 Habilitar disparo automático

| Aspecto | Detalle |
| --- | --- |
| Propósito | Disparar un ciclo ante recuperación de conectividad, sin invocación manual. |
| Entradas | Bandera de habilitación (obligatorio) + referencia a una `fuente-de-conectividad` (obligatorio para habilitar). |
| Salida | Notificación de resultado por cada ciclo disparado (ver §5, evento de resultado de ciclo). |
| CU | CU-04 |
| Excepciones | `DISPARO_AUTOMATICO_DESHABILITADO`, `SESION_NO_AUTENTICADA`, `FUENTE_CONECTIVIDAD_AUSENTE`. |
| Notas | El observador ignora eventos redundantes por rebote de red mientras hay un ciclo en curso (no reentrada, CU-04 flujo 5.C). El ciclo disparado respeta el mismo `orden-subir-antes-de-bajar` que el manual. |

### 2.5 Consultar estado y cola

| Aspecto | Detalle |
| --- | --- |
| Propósito | Leer el `estado-de-sesion`, la cola de pendientes y los elementos en conflicto. |
| Entradas | Referencia de sesión (obligatorio); opción de detalle (opcional; incluye la carga útil de las entradas si se solicita). |
| Salida | `estado-de-sesion` + entradas de cola + elementos en conflicto conocidos. |
| CU | CU-05 |
| Excepciones | `SESION_NO_INICIALIZADA`, `ALMACEN_LOCAL_INACCESIBLE`. |
| Notas | Durante un ciclo activo devuelve estado `sincronizando` con el progreso parcial (CU-05 flujo 5.A). Pidiendo el detalle de conflicto, lista los `elemento-en-conflicto` como convivientes, no resueltos (CU-05 flujo 5.B). |

### 2.6 Reanudar sincronización

| Aspecto | Detalle |
| --- | --- |
| Propósito | Continuar una subida que un corte dejó parcial, desde el punto de corte. |
| Entradas | Referencia de sesión reanudable (obligatorio). |
| Salida | `resumen-de-reanudacion`: nuevos confirmados, reconocidos como ya recibidos, bajados, estado final. |
| CU | CU-06 |
| Excepciones | `SESION_NO_REANUDABLE`, `BACKEND_INALCANZABLE`, `PROGRESO_INCONSISTENTE`. |
| Notas | Reenvía solo los no confirmados, apoyándose en la idempotencia (CU-06). Si todos ya habían sido recibidos, no reaplica ninguno y procede a la bajada (CU-06 flujo 5.A). Un nuevo corte deja la sesión nuevamente `reanudable` (CU-06 flujo 5.B). Si la sesión no quedó reanudable, la solicitud se trata como un ciclo normal (`SESION_NO_REANUDABLE`). |

## 3. Formas de datos

Paridad con 05 `contratos-abstractions_v1.0.md` §4. Las garantías de forma son parte del contrato.

### 3.1 Configuración de sesión

| Campo | Obligatorio | Descripción |
| --- | --- | --- |
| Identificador de host | Sí | Identidad del host para el que se inicializa la sesión. |
| Referencia al almacén local | Sí | `estrategia-de-almacen-local` inyectada (§4.1). |
| Referencia al backend remoto | Sí | `estrategia-de-transporte` inyectada (§4.2). |
| Proveedor de credencial | No | `proveedor-de-credencial` (§4.3); su ausencia produce estado `no autenticada`. |

### 3.2 Cambio local

| Campo | Obligatorio | Descripción |
| --- | --- | --- |
| Identificador de cambio estable | Sí | Clave de idempotencia; única por cambio (`identificador-de-cambio-estable`). |
| Tipo u operación | — | Naturaleza del cambio, opaca para el motor. |
| Carga útil | — | `carga-util-opaca`; el motor no la interpreta. |
| Marca de orden de creación | — | Conserva el orden relativo en que se crearon los cambios. |

Invariante: el identificador estable es obligatorio y único; el orden de creación se conserva.

### 3.3 Estado de la sesión

| Campo | Descripción |
| --- | --- |
| Situación | Valor del conjunto cerrado: `listo`, `no autenticada`, `sincronizando`, `reanudable`. |
| Cantidad de cambios pendientes | Tamaño actual de la cola. |
| Marca de última sincronización | Límite desde el cual baja la próxima fase de bajada. |
| Cantidad de elementos en conflicto conocidos | Cuántos `elemento-en-conflicto` reportó el backend. |
| Progreso parcial | Presente solo durante un ciclo en curso. |

Invariante: el conjunto de situaciones es cerrado y forma parte del contrato.

### 3.4 Entrada de cola

| Campo | Descripción |
| --- | --- |
| Identificador de cambio estable | Clave de la entrada. |
| Marca de orden de creación | Orden relativo. |
| Carga útil | Presente solo si el host la solicita en la consulta (§2.5). |

### 3.5 Resumen del ciclo

| Campo | Descripción |
| --- | --- |
| Cantidad de cambios subidos | Pendientes confirmados en la fase de subida. |
| Cantidad de actualizaciones bajadas | Aplicadas en la fase de bajada. |
| Lista de elementos en conflicto | `elemento-en-conflicto` aplicados como válidos y reportados. |
| Estado final | `estado-de-sesion` al cierre del ciclo. |

### 3.6 Resumen de reanudación

| Campo | Descripción |
| --- | --- |
| Cambios efectivamente nuevos confirmados | Reenviados que el backend recibió por primera vez. |
| Cambios reconocidos como ya recibidos | Reenviados que el backend ya tenía (no reaplicados). |
| Actualizaciones bajadas | Aplicadas tras concluir la subida. |
| Estado final | `estado-de-sesion` al cierre. |

## 4. Contratos de extensión (implementados por el host)

Paridad con 05 `extensibilidad_v1.0.md` §3. El host implementa estos contratos y los inyecta; el motor programa contra la abstracción.

### 4.1 Estrategia de almacén local

| Aspecto | Detalle |
| --- | --- |
| Responsabilidad | Persistir y leer la `cola-local` y los metadatos de sincronización en el almacén del host. |
| Obligatorio | Sí. |
| Inyección | En la configuración de sesión (§3.1). |
| CU que lo usa | CU-01, CU-02, CU-05, CU-06. |
| Error si falta o falla | `CONFIGURACION_INCOMPLETA`, `ALMACEN_LOCAL_INACCESIBLE`. |

### 4.2 Estrategia de transporte

| Aspecto | Detalle |
| --- | --- |
| Responsabilidad | Enviar un cambio al backend por `identificador-de-cambio-estable` y obtener actualizaciones posteriores a una marca. |
| Obligatorio | Sí. |
| Inyección | En la configuración de sesión (§3.1). |
| CU que lo usa | CU-03, CU-06. |
| Error si falta o falla | `CONFIGURACION_INCOMPLETA`; en ejecución, `BACKEND_INALCANZABLE`. |
| Obligación de contrato | Debe reconocer el identificador estable (idempotencia, ADR-07) y poder reportar un estado en conflicto como condición no bloqueante (ADR-08). |

### 4.3 Proveedor de credencial

| Aspecto | Detalle |
| --- | --- |
| Responsabilidad | Entregar la credencial vigente del host al motor. |
| Obligatorio | No (su ausencia produce estado `no autenticada`). |
| Inyección | En la configuración de sesión, al inicializar o más tarde. |
| CU que lo usa | CU-01, CU-03. |
| Error relacionado | `SESION_NO_AUTENTICADA`, `CREDENCIAL_INVALIDA`. |

### 4.4 Fuente de eventos de conectividad

| Aspecto | Detalle |
| --- | --- |
| Responsabilidad | Notificar transiciones de red para el `disparo-automatico`. |
| Obligatorio | No (solo para el modo automático). |
| Inyección | Al habilitar el disparo automático (§2.4). |
| CU que lo usa | CU-04. |
| Error si falta | `FUENTE_CONECTIVIDAD_AUSENTE`. |

## 5. Eventos

El motor publica notificaciones que el host puede observar. La semántica de orden y entrega forma parte del contrato observable.

| Evento | Payload | Semántica de orden y entrega |
| --- | --- | --- |
| Resultado de ciclo (disparo automático) | `resumen-del-ciclo` del ciclo disparado. | Se emite una vez por cada ciclo que el disparo automático ejecuta, al concluir; no se emite por eventos de conectividad ignorados o redundantes (CU-04 flujo 5.C). |
| Diagnóstico estructurado | Identificador de sesión, fase del ciclo, código de condición, identificador de correlación del ciclo. | Emitido a lo largo del ciclo; nunca incluye la `carga-util-opaca` de dominio. El destino del log lo provee el host (05 `arquitectura-solucion_v1.0.md` §7). |

## 6. Excepciones

Catálogo completo de códigos estables de la superficie pública. Paridad con 03 `dx-error-messages_v1.0.md` §3. El `codigo-de-error-estable` es invariante a través de versiones menores y no se traduce; alterarlo es un cambio incompatible (ADR-03). El diagnóstico paso a paso de cada uno vive en `troubleshooting_v1.0.md`.

| Código | Categoría | Cuándo se lanza | Operación |
| --- | --- | --- | --- |
| `CONFIGURACION_INCOMPLETA` | Entrada inválida | Falta un campo obligatorio de la configuración de sesión. | Inicializar sesión |
| `ALMACEN_LOCAL_INACCESIBLE` | Recurso ausente | No se puede abrir o escribir el almacén local. | Inicializar sesión, Consultar estado |
| `SESION_YA_INICIALIZADA` | Conflicto de estado | Se inicializa una segunda sesión para un host ya activo. | Inicializar sesión |
| `IDENTIFICADOR_CAMBIO_AUSENTE` | Entrada inválida | Se encola un cambio sin identificador estable. | Encolar cambio local |
| `SESION_NO_INICIALIZADA` | Recurso ausente | Se encola o consulta sin sesión inicializada. | Encolar cambio local, Consultar estado |
| `ALMACEN_LOCAL_SIN_ESPACIO` | Error transitorio | No hay espacio para persistir la nueva entrada de cola. | Encolar cambio local |
| `BACKEND_INALCANZABLE` | Error transitorio | El backend no responde al iniciar o durante la subida. | Ejecutar sincronización, Reanudar |
| `CREDENCIAL_INVALIDA` | Autenticación | El backend rechazó la credencial provista. | Ejecutar sincronización |
| `SUBIDA_INCOMPLETA` | Error transitorio | La subida terminó con pendientes sin confirmar por un corte. | Ejecutar sincronización |
| `DISPARO_AUTOMATICO_DESHABILITADO` | Conflicto de estado | Llega un evento de red pero el disparo automático no está habilitado. | Habilitar disparo automático |
| `SESION_NO_AUTENTICADA` | Autenticación | Hay red disponible pero la sesión no tiene credencial vigente. | Habilitar disparo automático, Inicializar |
| `FUENTE_CONECTIVIDAD_AUSENTE` | Recurso ausente | Se habilita el modo automático sin fuente de conectividad suscripta. | Habilitar disparo automático |
| `SESION_NO_REANUDABLE` | Conflicto de estado | Se reanuda una sesión que no quedó reanudable. | Reanudar sincronización |
| `PROGRESO_INCONSISTENTE` | Error transitorio | La marca de progreso y la cola no concuerdan al reanudar. | Reanudar sincronización |
| `ELEMENTO_EN_CONFLICTO` | Conflicto reportado (no bloqueante) | El backend marcó entidades en conflicto; el motor las aplica y reporta. | Ejecutar sincronización, Consultar estado |

`ELEMENTO_EN_CONFLICTO` no es un error de bloqueo: el ciclo concluye y lo incluye en el resumen. El motor no expone una categoría de error interno: toda condición es diagnosticable y accionable (03 `dx-error-messages_v1.0.md` §2).

## 7. Ejemplos breves por bloque

Los ejemplos describen el método más relevante de cada bloque en secuencia y comportamiento observable, no en sintaxis de un stack; el código ejecutable vive en los samples de la categoría 11.

Inicializar y verificar (operación más rica del bloque de configuración):

```text
1. Armar configuración: host="campo-01", almacén local inyectado, transporte inyectado, sin credencial.
2. Inicializar sesión.
   Esperado: identificador de sesión no vacío; estado "no autenticada".
3. Proveer una credencial vigente.
   Esperado: estado pasa a "listo".
```

Encolar e idempotencia (comportamiento clave de la cola):

```text
1. Encolar cambio con identificador "obs-001".  Esperado: tamaño de cola = 1.
2. Reencolar el MISMO "obs-001".               Esperado: tamaño de cola = 1 (no duplica).
3. Encolar cambio con identificador "obs-002".  Esperado: tamaño de cola = 2.
```

Ejecutar y leer el resumen (operación central):

```text
1. Con cola = 2 y sesión "listo", ejecutar sincronización.
   Esperado: resumen con subidos=2 ANTES de cualquier bajado; luego bajados=N.
2. Consultar estado.
   Esperado: estado "listo", pendientes=0, marca de última sincronización avanzada.
```

Reanudar tras un corte (operación más compleja del bloque de recuperación):

```text
1. Ejecutar con cola = 5; simular corte tras confirmar 2.
   Esperado: error SUBIDA_INCOMPLETA; estado "reanudable"; ninguna bajada aplicada.
2. Reanudar.
   Esperado: resumen de reanudación con nuevos confirmados=3, reconocidos=0, luego bajados=N; estado "listo".
```

## 8. Referencias cruzadas

- 05 `contratos-abstractions_v1.0.md` §3, §4, §5, §6: fuente de paridad de operaciones, formas de datos, errores y versionado.
- 05 `extensibilidad_v1.0.md` §3 y §4: contratos de extensión de §4 de esta referencia.
- 05 `flujo-ejecucion_v1.0.md` §3, §4, §5: comportamiento de las operaciones de ejecución, disparo y reanudación.
- 05 ADR-03: política de versionado de la superficie pública.
- 03 `dx-error-messages_v1.0.md` §3: catálogo de mensajes que acompaña a los códigos de §6.
- `troubleshooting_v1.0.md`: diagnóstico paso a paso de cada código.
- `glosario-tecnico_v1.0.md`: definición canónica del vocabulario.

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Referencia inicial de la superficie pública de aplicada-sync: seis operaciones del ciclo de vida, seis formas de datos, cuatro contratos de extensión, dos eventos publicados y los 15 códigos del catálogo de excepciones, con ejemplos breves por bloque. Paridad uno a uno con `contratos-abstractions_v1.0.md` y `extensibilidad_v1.0.md` de 05 y con `dx-error-messages_v1.0.md` de 03. |
