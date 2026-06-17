# Conceptos fundamentales — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** conceptos-fundamentales_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-15
**Autor:** Technical Writer + SDK Documentation Lead (AG-10)
**Tipo Diátaxis:** Explanation
**Audiencia:** Developer integrador que consume la librería desde su propia aplicación
**Nivel:** Medio
**Tiempo estimado de lectura:** 16 min

Este documento construye el modelo mental del motor de sincronización para quien lo integra. Explica qué hace, cómo razona el ciclo de sincronización, por qué toma las decisiones que afectan cómo se usa, y dónde termina su responsabilidad. No documenta la implementación interna (eso vive en la categoría 05) ni provee código ejecutable (eso vive en la categoría 11): documenta el comportamiento observable que el integrador necesita comprender para usar el motor correctamente. El vocabulario en `código kebab` se define en `glosario-tecnico_v1.0.md`.

## 1. Concepto central

`aplicada-sync` es un `motor-de-sincronizacion` que un `host` incorpora a su aplicación para propagar los cambios capturados localmente hacia un backend remoto cuando hay conectividad, sin perder ni duplicar datos ante los cortes que son normales en el trabajo de campo. Recibe cambios locales que el host encola, los sube a un backend remoto y después baja las actualizaciones disponibles; entrega un resumen verificable de cada ciclo. El motor es agnóstico del dominio: no interpreta el contenido de los cambios, solo los transporta de forma confiable.

La promesa que justifica adoptarlo: el integrador no tiene que escribir su propio motor de sincronización offline-first. El orden correcto, la no duplicación y la reanudación segura son garantías del motor, no responsabilidad del host.

Identificadores de concepto usados en este documento, para enlazar desde la referencia y desde los samples de 11:

| Concepto | Identificador |
| --- | --- |
| El motor de sincronización | `concepto-motor` |
| La sesión | `concepto-sesion` |
| La cola local de pendientes | `concepto-cola` |
| El ciclo subir-luego-bajar | `concepto-ciclo` |
| La reanudación tras un corte | `concepto-reanudacion` |
| La convivencia con conflicto | `concepto-conflicto` |

## 2. Modelo mental

El flujo principal tiene cinco etapas. El host nunca habla con el backend a través del motor de forma directa: provee las piezas (almacén local, transporte, credencial, conectividad) y el motor las orquesta.

```text
   captura del host
        │
        ▼
 [1] ENCOLAR ────► cola-local persistente y ordenada (una entrada por identificador)
        │
        ▼
 [2] DISPARAR ────► manual (el host pide ejecutar) o automático (evento de conectividad)
        │
        ▼
 [3] SUBIR ───────► envía pendientes en orden; confirma por identificador; retira de la cola
        │            (un corte acá deja la sesión "reanudable", NO baja nada)
        ▼
 [4] BAJAR ───────► solo si la subida concluyó: aplica actualizaciones una sola vez
        │            (las marcadas en conflicto se aplican y se reportan, no abortan)
        ▼
 [5] RESUMIR ─────► resumen-del-ciclo: subidos, bajados, en conflicto, estado final
```

Cada etapa, en una línea, con su concepto y un ejemplo del comportamiento observable:

| Etapa | Qué es | Ejemplo observable |
| --- | --- | --- |
| Encolar | El host registra un `cambio-local` en la `cola-local` (`concepto-cola`). | Tras encolar un cambio, el tamaño de cola reportado pasa de 0 a 1. |
| Disparar | Se inicia un `ciclo-de-sincronizacion` (`concepto-ciclo`) manual o por `disparo-automatico`. | Al recuperar conectividad con disparo automático habilitado, arranca un ciclo sin intervención. |
| Subir | La `fase-de-subida` envía los pendientes en orden y los confirma por identificador. | El resumen muestra los subidos antes que cualquier bajado. |
| Bajar | La `fase-de-bajada` aplica las actualizaciones posteriores a la `marca-de-ultima-sincronizacion`. | Tras un ciclo exitoso, la cola queda en cero y la marca de última sincronización avanza. |
| Resumir | El motor compone el `resumen-del-ciclo`. | El integrador lee cuántos subió, cuántos bajó y qué quedó en conflicto. |

La `sesion` (`concepto-sesion`) es el contexto que sostiene todo esto entre ciclos. Transita un conjunto cerrado de estados: listo, no autenticada, sincronizando y reanudable. Ese conjunto es parte del contrato; el host puede confiar en que no aparecerán otros estados.

```text
   inicializar
        │
        ▼
 ┌──────────────┐  provee credencial   ┌──────────┐
 │ no autenticada│ ───────────────────► │  listo   │
 └──────────────┘                       └────┬─────┘
        ▲                                     │ ejecutar
        │ sin credencial                      ▼
        │                              ┌──────────────┐
        │                              │ sincronizando │
        │                              └──────┬────────┘
        │                  ciclo OK ┌─────────┴─────────┐ corte en subida
        └──────────────────────────┤                   ├────────────────► ┌─────────────┐
                                    ▼                   ▼                  │  reanudable  │
                                 listo               (reanudar) ◄─────────┴─────────────┘
```

## 3. Decisiones de diseño relevantes para el consumidor

Estas decisiones cambian cómo el integrador usa el motor; cada una cita su ADR de origen en 05. Las decisiones puramente internas (esquema físico, componentes) no aparecen acá.

| ID | Decisión | Por qué afecta al consumidor | ADR fuente |
| --- | --- | --- | --- |
| D-01 | El host inyecta cuatro `punto-de-extension`: almacén local, transporte, credencial y conectividad. El motor nunca instancia un adaptador concreto. | El integrador debe implementar y registrar al menos el almacén local y el transporte; sin ellos la inicialización se rechaza. La credencial y la conectividad son opcionales. | ADR-01, ADR-02 |
| D-02 | El `orden-subir-antes-de-bajar` es una invariante dura, no una opción. | El integrador no puede invertir ni paralelizar el orden: el motor garantiza que ninguna bajada pisa un cambio local todavía no propagado. Esto simplifica el razonamiento, pero implica que la bajada espera a que toda la subida confirme. | ADR-05 |
| D-03 | La `cola-local` es persistente y ordenada, con una entrada por identificador. | Los cambios sobreviven a reinicios y cortes, y reencolar el mismo cambio no lo duplica. El integrador puede encolar con seguridad aunque la app se cierre antes de sincronizar. | ADR-04 |
| D-04 | La `idempotencia` descansa en un `identificador-de-cambio-estable` que provee el host. | El integrador es responsable de asignar un identificador estable y único por cambio. Si el identificador es inestable, la garantía de no duplicación se rompe: esta es la obligación más importante del host. | ADR-07 |
| D-05 | El motor convive con el `elemento-en-conflicto`: lo aplica, lo reporta y nunca lo resuelve. | El integrador recibe los conflictos en el resumen y en la consulta de estado como condición reportada, no como error. La resolución ocurre fuera del motor, en el backend o en el host. | ADR-08 |
| D-06 | La `reanudacion` reenvía solo los no confirmados, apoyada en una `marca-de-progreso` persistida. | Tras un corte, el integrador reanuda (o vuelve a ejecutar) y el motor no reenvía de forma efectiva lo ya confirmado. La cola persistida es la fuente de verdad ante una inconsistencia. | ADR-06 |
| D-07 | La `superficie-publica` se versiona con SemVer; un cambio de contrato exige versión mayor. | El integrador puede adoptar versiones menores y de parche con expectativa de compatibilidad; un salto de versión mayor señala que algo del contrato cambió. | ADR-03 |

## 4. Vocabulario crítico

Subconjunto mínimo para entender el motor. La definición canónica completa vive en `glosario-tecnico_v1.0.md`.

| Término | Definición operativa | Ejemplo |
| --- | --- | --- |
| `cambio-local` | Unidad que el host encola para subir: identificador estable, operación, carga útil opaca y marca de orden. | Una observación capturada en campo, lista para propagar. |
| `carga-util-opaca` | Contenido de dominio que el motor transporta sin interpretar. | El motor no sabe si la carga es una foto, una nota o un registro; la trata como un blob. |
| `identificador-de-cambio-estable` | Clave única del cambio que sostiene la no duplicación y la idempotencia. | El mismo identificador reencolado no agrega una segunda entrada a la cola. |
| `estado-de-sesion` | Situación dentro del conjunto cerrado listo / no autenticada / sincronizando / reanudable. | Tras un corte en la subida, la sesión queda en "reanudable". |
| `elemento-en-conflicto` | Entidad que el backend marcó en conflicto; condición reportada, no error de bloqueo. | El ciclo concluye y lista el elemento en conflicto sin abortar. |

## 5. Qué NO hace el motor

Esta tabla delimita la frontera de responsabilidad. Todo lo de la columna "responsabilidad del host" debe resolverlo el integrador; el motor no lo cubre.

| El motor NO hace | Responsabilidad del host | Referencia |
| --- | --- | --- |
| No implementa el almacén local ni el transporte. | El host provee e inyecta esos `punto-de-extension`. | 05 `extensibilidad_v1.0.md` §3 |
| No emite, renueva ni resguarda credenciales. | El host provee una credencial vigente vía `proveedor-de-credencial`. | 05 `arquitectura-solucion_v1.0.md` §7 |
| No detecta la red de bajo nivel. | El host suscribe una `fuente-de-conectividad` para el disparo automático. | 05 `flujo-ejecucion_v1.0.md` §4 |
| No interpreta ni valida la `carga-util-opaca`. | El host define y valida el contenido de dominio del cambio. | 05 `arquitectura-solucion_v1.0.md` §6 |
| No genera el `identificador-de-cambio-estable`. | El host asigna un identificador estable y único por cambio. | 05 ADR-07 |
| No resuelve conflictos ni decide unificaciones. | El backend o el host resuelven el conflicto fuera del ciclo. | 05 ADR-08 |
| No invierte ni paraleliza el `orden-subir-antes-de-bajar`. | Ninguna: el orden es garantía del motor, no configurable. | 05 ADR-05 |
| No despliega ni opera el backend remoto. | El host y su plataforma operan el backend, externo al motor. | 05 `arquitectura-solucion_v1.0.md` §5 |
| No expone una categoría de error interno. | El host trata cada `codigo-de-error-estable` como diagnosticable y accionable. | 03 `dx-error-messages_v1.0.md` §2 |

## 6. Referencias cruzadas

- 05 `contratos-abstractions_v1.0.md` §3 y §4: operaciones y formas de datos de la superficie pública (paridad con la referencia).
- 05 `extensibilidad_v1.0.md` §3: los cuatro puntos de extensión que materializan la decisión D-01.
- 05 ADR-04, ADR-05, ADR-06, ADR-07, ADR-08: decisiones de cola, orden, reanudación, idempotencia y conflicto.
- 05 `flujo-ejecucion_v1.0.md` §2 y §3: estados de la sesión y pipeline detallado.
- `glosario-tecnico_v1.0.md`: definición canónica de todo el vocabulario usado acá.
- `referencia-api_v1.0.md`: contrato exacto de cada operación y tipo.

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Modelo mental inicial del motor de sincronización para el integrador: concepto central, flujo de cinco etapas y máquina de estados, siete decisiones de diseño con ADR fuente, vocabulario crítico y delimitación de responsabilidades host/motor. Derivado de la especificación funcional de 02 (CU-01 a CU-06, RN-01 a RN-03), de la arquitectura de 05 y de los ADR-01 a ADR-08. |
