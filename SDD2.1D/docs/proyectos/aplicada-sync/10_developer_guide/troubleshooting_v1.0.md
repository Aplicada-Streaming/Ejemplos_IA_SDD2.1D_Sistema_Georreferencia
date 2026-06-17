# Troubleshooting — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** troubleshooting_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-15
**Autor:** Technical Writer + SDK Documentation Lead (AG-10)
**Tipo Diátaxis:** How-to (orientado a diagnóstico)
**Audiencia:** Developer integrador que consume la librería desde su propia aplicación
**Nivel:** Medio
**Tiempo estimado de lectura:** 18 min

Guía de diagnóstico de las condiciones más frecuentes que el integrador encuentra al consumir el motor. Cada entrada lleva un código `ISSUE-XX` referenciable desde el código de error real, desde logs o desde tickets. Los códigos de error citados (`MAYUSCULAS_CON_GUION`) son los códigos estables del catálogo (`referencia-api_v1.0.md` §6; 03 `dx-error-messages_v1.0.md`). El vocabulario en `código kebab` se define en `glosario-tecnico_v1.0.md`.

Recordatorio de naturaleza: el motor distingue defecto de integración (entrada inválida, recurso ausente, conflicto de estado) de condición transitoria de conectividad (reintentable o reanudable, sin pérdida ni duplicación). Las segundas no son un bug del motor.

## 1. Errores comunes (síntoma / causa / solución)

| Issue | Síntoma observado | Causa probable | Solución | Código(s) |
| --- | --- | --- | --- | --- |
| ISSUE-01 | Aparecen entidades marcadas en conflicto tras una bajada. | El backend reportó un estado en conflicto; el motor lo aplicó y lo reporta. | Resolverlo fuera del motor; es condición reportada, no error de bloqueo. | `ELEMENTO_EN_CONFLICTO` |
| ISSUE-02 | El ciclo se detiene en la subida y no baja nada; la sesión queda `reanudable`. | Se perdió la conectividad durante la fase de subida. | Reanudar cuando vuelva la red; no se perdieron cambios. | `BACKEND_INALCANZABLE`, `SUBIDA_INCOMPLETA` |
| ISSUE-03 | El `disparo-automatico` no ejecuta al recuperar la red. | No habilitado, sin credencial vigente, o sin `fuente-de-conectividad` suscripta. | Habilitar, asegurar credencial y suscribir la fuente. | `DISPARO_AUTOMATICO_DESHABILITADO`, `SESION_NO_AUTENTICADA`, `FUENTE_CONECTIVIDAD_AUSENTE` |
| ISSUE-04 | Al reanudar, la marca de progreso y la cola no concuerdan. | El registro de progreso quedó desfasado tras un corte. | Ninguna acción manual: el motor adopta la cola como fuente de verdad y reenvía apoyado en idempotencia. | `PROGRESO_INCONSISTENTE`, `SESION_NO_REANUDABLE` |
| ISSUE-05 | La inicialización se rechaza apenas comienza. | Falta un campo obligatorio o el almacén local no es accesible. | Completar el campo o habilitar el almacén y reinicializar. | `CONFIGURACION_INCOMPLETA`, `ALMACEN_LOCAL_INACCESIBLE`, `SESION_YA_INICIALIZADA` |
| ISSUE-06 | La cola no se comporta como se espera (no crece, o falta espacio). | Cambio sin identificador estable, sin sesión, o almacén sin espacio. | Asignar identificador estable, inicializar, o liberar espacio. | `IDENTIFICADOR_CAMBIO_AUSENTE`, `SESION_NO_INICIALIZADA`, `ALMACEN_LOCAL_SIN_ESPACIO` |
| ISSUE-07 | La sincronización falla por autenticación del host. | La credencial venció o fue rechazada por el backend. | Renovar la credencial del host y reejecutar; la cola no se altera. | `CREDENCIAL_INVALIDA`, `SESION_NO_AUTENTICADA` |

## 2. Diagnóstico paso a paso

### ISSUE-01 — Elementos en conflicto tras una bajada

Naturaleza: condición reportada, no error (RN-03; ADR-08).

1. Confirmar que el ciclo concluyó: leer el `resumen-del-ciclo`; el estado final debe ser `listo`, no abortado.
2. Consultar estado con detalle de conflicto (CU-05 flujo 5.B): obtener la lista de `elemento-en-conflicto` conocidos.
3. Verificar que la cantidad de conflictos del estado coincide con la lista del resumen.
4. Confirmar que el motor no resolvió nada: los elementos siguen marcados como convivientes.

Solución: resolver el conflicto en el backend o en el host. El motor nunca decide la unificación. No es necesario reintentar el ciclo por este motivo.

### ISSUE-02 — Sincronización interrumpida (corte en la subida)

Naturaleza: condición transitoria; sin pérdida ni duplicación (ADR-06).

1. Leer el código devuelto: `BACKEND_INALCANZABLE` (no respondió) o `SUBIDA_INCOMPLETA` (quedaron pendientes sin confirmar).
2. Consultar estado: la situación debe ser `reanudable` y la cola debe conservar los no confirmados.
3. Verificar que NINGUNA bajada se aplicó: la `marca-de-ultima-sincronizacion` no avanzó (la compuerta de orden lo garantiza, RN-01).
4. Confirmar conectividad real con el backend antes de reanudar (check fuera del motor).
5. Reanudar (CU-06): el motor reenvía solo los faltantes.

Solución: invocar Reanudar cuando vuelva la red. Si la red sigue caída, el motor vuelve a dejar la sesión `reanudable` sin pérdida; reintentar más tarde.

### ISSUE-03 — El disparo automático no ejecuta

Naturaleza: conflicto de estado o recurso ausente.

1. Confirmar que el `disparo-automatico` está habilitado; si no, el evento se registra como ignorado (`DISPARO_AUTOMATICO_DESHABILITADO`).
2. Consultar estado: si es `no autenticada`, el motor admite encolar pero no dispara (`SESION_NO_AUTENTICADA`).
3. Verificar que hay una `fuente-de-conectividad` suscripta; sin ella no llega ningún evento (`FUENTE_CONECTIVIDAD_AUSENTE`).
4. Verificar que la fuente realmente emite la transición a red disponible (check del adaptador del host).
5. Descartar reentrada: si ya hay un ciclo en curso, los eventos redundantes por rebote se ignoran a propósito (CU-04 flujo 5.C).

Solución: habilitar el disparo, proveer credencial vigente y suscribir la fuente. Como alternativa, disparar el ciclo manualmente (CU-03).

### ISSUE-04 — Progreso inconsistente al reanudar

Naturaleza: condición transitoria; el motor se autorrepara (ADR-06).

1. Leer el código: `PROGRESO_INCONSISTENTE` (marca desfasada) o `SESION_NO_REANUDABLE` (no quedó subida parcial).
2. Para `PROGRESO_INCONSISTENTE`: el motor adopta la `cola-local` persistida como fuente de verdad; no se requiere acción manual.
3. Verificar el `resumen-de-reanudacion`: los reenviados que el backend ya tenía aparecen como reconocidos como ya recibidos (no reaplicados, gracias a la idempotencia).
4. Para `SESION_NO_REANUDABLE`: el motor trata la solicitud como un ciclo normal; no inventa progreso.

Solución: ninguna acción manual ante inconsistencia; confiar en la idempotencia (RN-02). Si reaparece de forma sistemática, reportarlo como posible defecto (§4) con el identificador de correlación del ciclo.

### ISSUE-05 — La inicialización se rechaza

Naturaleza: defecto de integración o de entorno.

1. Leer el código: `CONFIGURACION_INCOMPLETA` indica en el detalle `{campo}` cuál falta (almacén local, transporte o identificador de host).
2. Verificar que las estrategias obligatorias (almacén local, transporte) están registradas en la `configuracion-de-sesion`.
3. Si es `ALMACEN_LOCAL_INACCESIBLE`: comprobar que el almacén existe y admite escritura de metadatos.
4. Si es `SESION_YA_INICIALIZADA`: reutilizar la referencia a la sesión vigente en lugar de inicializar otra.

Solución: completar el campo faltante o habilitar el almacén y reinicializar; el motor no deja sesión a medias.

### ISSUE-06 — La cola no se comporta como se espera

Naturaleza: defecto de integración o condición transitoria.

1. Si el encolado se rechaza con `IDENTIFICADOR_CAMBIO_AUSENTE`: el cambio llegó sin identificador estable; asignarlo antes de encolar.
2. Si es `SESION_NO_INICIALIZADA`: inicializar la sesión (CU-01) antes de encolar o consultar.
3. Si reencolar no aumenta la cola: es el comportamiento esperado (no duplica por identificador, RN-02); no es un error.
4. Si es `ALMACEN_LOCAL_SIN_ESPACIO`: liberar espacio en el almacén y reintentar; no quedó entrada parcial.
5. Si la cola crece más de lo esperado: verificar que el host asigna un identificador único por cambio y no uno nuevo para el mismo cambio.

Solución: corregir la asignación de identificadores o el espacio del almacén según el código.

### ISSUE-07 — Autenticación del host rechazada

Naturaleza: condición de credencial del host.

1. Leer el código: `CREDENCIAL_INVALIDA` (rechazada por el backend) o `SESION_NO_AUTENTICADA` (no hay credencial vigente).
2. Verificar que el `proveedor-de-credencial` entrega una credencial vigente (check del adaptador del host).
3. Confirmar que el motor no subió ni bajó nada y no alteró la cola ante `CREDENCIAL_INVALIDA`.
4. Renovar la credencial en el host y reejecutar.

Solución: proveer o renovar la credencial vigente. El motor no emite ni renueva credenciales: es responsabilidad del host (`conceptos-fundamentales_v1.0.md` §5).

## 3. Logs útiles

El motor emite diagnóstico estructurado; el destino del log lo provee el host (`referencia-api_v1.0.md` §5; 05 `arquitectura-solucion_v1.0.md` §7).

| Qué buscar | Dónde | Patrón / nivel | Para qué sirve |
| --- | --- | --- | --- |
| Identificador de correlación del ciclo | Diagnóstico estructurado | Presente en cada evento de un mismo ciclo | Rastrear subida y bajada de un ciclo de punta a punta. |
| Código de condición | Diagnóstico estructurado | El código estable (`MAYUSCULAS_CON_GUION`) | Mapear el síntoma al `ISSUE-XX` correspondiente. |
| Fase del ciclo | Diagnóstico estructurado | `subida` / `bajada` | Saber si el corte ocurrió antes o después de la compuerta de orden. |
| Tamaño de cola reportado | Consulta de estado (CU-05) | Contador de pendientes | Confirmar encolado, idempotencia y vaciado tras un ciclo. |
| Contadores del resumen | Resumen del ciclo / de reanudación | Subidos, bajados, en conflicto, reconocidos | Verificar orden, no duplicación y reanudación. |

El motor nunca registra la `carga-util-opaca` de dominio: no buscar el contenido del cambio en los logs.

## 4. Cómo reportar un bug

Antes de reportar, confirmar que no es una condición transitoria esperada (ISSUE-02, ISSUE-04, ISSUE-07) ni una condición reportada (ISSUE-01): esas no son defectos del motor. Un fallo no contemplado por el catálogo de códigos sí se considera un posible defecto de la librería.

Dónde: reporte etiquetado como `dx` en el repositorio público del paquete; preguntas de integración en la sección de discusiones del repositorio (03 `dx-developer-experience_v1.0.md` §7).

Datos mínimos a adjuntar:

```text
Título: [ISSUE-XX o código] resumen breve de la condición

Versión del paquete: <X.Y.Z>
Código de error devuelto: <CODIGO_ESTABLE o "ninguno">
Estado de la sesión observado: listo | no autenticada | sincronizando | reanudable
Identificador de correlación del ciclo: <de los logs>

Pasos para reproducir:
1. ...
2. ...
3. ...

Comportamiento esperado: ...
Comportamiento observado: ...

Estrategias inyectadas: almacén local (sí/no), transporte (sí/no),
                        credencial (sí/no), conectividad (sí/no)
Tamaño de cola al momento del fallo: <N>
Diagnóstico estructurado relevante (sin carga útil de dominio): ...
```

Política de severidad y respuesta: la pérdida o duplicación de datos, o el incumplimiento del `orden-subir-antes-de-bajar`, son severidad máxima (violan la garantía de negocio). El resto se prioriza según impacto en la adopción. Los tiempos de respuesta concretos los fija la operación del repositorio público; este documento define los datos mínimos y la severidad, no el SLA.

## 5. Referencias cruzadas

- `referencia-api_v1.0.md` §6: catálogo de los 15 códigos estables citados en cada issue.
- 03 `dx-error-messages_v1.0.md` §2 y §3: taxonomía y mensajes (qué pasó / por qué / qué hacer) de cada código.
- 05 `flujo-ejecucion_v1.0.md` §3, §4, §5: comportamiento del corte, la reanudación y el disparo automático que sustenta el diagnóstico.
- 05 ADR-06 (reanudación), ADR-07 (idempotencia), ADR-08 (conflicto): garantías que explican por qué las condiciones transitorias no son bugs.
- 08 `estrategia-testing_v1.0.md` §5: dobles que reproducen corte, conflicto y rebote para aislar la causa.
- `conceptos-fundamentales_v1.0.md`, `guia-integracion-aplicacion-movil_v1.0.md` §5, `glosario-tecnico_v1.0.md`.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Troubleshooting inicial con siete entradas ISSUE-01 a ISSUE-07 (conflicto, sync interrumpido, disparo automático, progreso inconsistente, inicialización, cola y autenticación del host), diagnóstico paso a paso por issue, tabla de logs útiles y plantilla de reporte de bug con política de severidad. Derivado del catálogo de errores de 03, del flujo de ejecución de 05 y de los ADR-06/07/08. |
