# Contratos de Abstractions — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** contratos-abstractions_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer

## 1. Alcance del contrato

Este documento define la superficie pública versionada del motor de sincronización `aplicada-sync`: la capa Abstractions que quien la integra consume y las abstracciones que el host implementa e inyecta. Es el contrato estable del paquete distribuible; su implementación interna no forma parte de él (ADR-03).

CU que se materializan a través de este contrato:

| CU | Qué materializa en el contrato |
| --- | --- |
| CU-01 | Operación de configuración e inicialización de la sesión; forma de la configuración y del estado inicial |
| CU-02 | Operación de encolado de un cambio local; forma del cambio local con identificador estable |
| CU-03 | Operación de ejecución del ciclo subir-luego-bajar; forma del resumen del ciclo |
| CU-04 | Operación de habilitación del disparo automático y notificación de resultado |
| CU-05 | Operación de consulta de estado y de la cola de pendientes; forma del estado |
| CU-06 | Operación de reanudación; forma del resumen de reanudación |

Las invariantes RN-01 (orden subir-antes-de-bajar), RN-02 (idempotencia) y RN-03 (convivencia con conflicto) son garantías del contrato, no opciones configurables.

## 2. Formato

Contrato de superficie pública de un paquete distribuible (capa Abstractions), descrito de forma abstracta: operaciones, formas de datos y códigos de error. No es un contrato de red (no hay esquema de transporte propio del motor); el transporte hacia el backend remoto es una abstracción que el host implementa. La materialización concreta de las firmas y los tipos vive en la categoría 11 (examples) y en el código del paquete; el stack concreto vive en el intake §17, no acá.

El contrato se compone de dos caras:

- Cara consumida por el integrador: las operaciones del ciclo de vida que el host invoca.
- Cara implementada por el integrador: los contratos de extensión que el host provee al motor (detallados en `extensibilidad_v1.0.md`).

## 3. Operaciones

Operaciones públicas que el host invoca sobre el motor:

| Operación | Propósito | Entradas | Salida | CU | Errores posibles |
| --- | --- | --- | --- | --- | --- |
| Inicializar sesión | Configurar e inicializar la sesión y dejar el motor listo | Configuración de sesión | Identificador de sesión + estado inicial | CU-01 | CONFIGURACION_INCOMPLETA, ALMACEN_LOCAL_INACCESIBLE, SESION_YA_INICIALIZADA |
| Encolar cambio local | Registrar un cambio en la cola de pendientes | Cambio local | Confirmación + tamaño de cola | CU-02 | IDENTIFICADOR_CAMBIO_AUSENTE, SESION_NO_INICIALIZADA, ALMACEN_LOCAL_SIN_ESPACIO |
| Ejecutar sincronización | Correr el ciclo subir-luego-bajar | Referencia de sesión | Resumen del ciclo | CU-03 | BACKEND_INALCANZABLE, CREDENCIAL_INVALIDA, SUBIDA_INCOMPLETA |
| Habilitar disparo automático | Disparar el ciclo ante recuperación de conectividad | Bandera de habilitación + fuente de conectividad | Notificación de resultado por ciclo | CU-04 | DISPARO_AUTOMATICO_DESHABILITADO, SESION_NO_AUTENTICADA, FUENTE_CONECTIVIDAD_AUSENTE |
| Consultar estado y cola | Leer el estado del motor y los pendientes | Referencia de sesión; opción de detalle | Estado + cola + elementos en conflicto | CU-05 | SESION_NO_INICIALIZADA, ALMACEN_LOCAL_INACCESIBLE |
| Reanudar sincronización | Continuar una subida parcial desde el punto de corte | Referencia de sesión reanudable | Resumen de reanudación | CU-06 | SESION_NO_REANUDABLE, BACKEND_INALCANZABLE, PROGRESO_INCONSISTENTE |

## 4. Esquemas de datos

Formas de datos del contrato, descritas de forma abstracta:

- Configuración de sesión: identificador de host (obligatorio), referencia al almacén local (obligatorio), referencia al backend remoto (obligatorio), proveedor de credencial (opcional; su ausencia produce estado no autenticada).
- Cambio local: identificador de cambio estable (obligatorio, clave de idempotencia), tipo u operación, carga útil opaca para el motor, marca de orden de creación.
- Estado de la sesión: situación (listo, no autenticada, sincronizando, reanudable), cantidad de cambios pendientes, marca de última sincronización, cantidad de elementos en conflicto conocidos, progreso parcial cuando hay un ciclo en curso.
- Entrada de cola: identificador de cambio estable, marca de orden de creación y, solo si el host lo solicita, la carga útil.
- Resumen del ciclo: cantidad de cambios subidos, cantidad de actualizaciones bajadas, lista de elementos en conflicto, estado final.
- Resumen de reanudación: cambios efectivamente nuevos confirmados, cambios reconocidos como ya recibidos, actualizaciones bajadas, estado final.

Garantías de forma: el identificador de cambio estable es obligatorio y único por cambio; la carga útil es opaca; el orden de creación se conserva; el conjunto de estados de la sesión es cerrado y forma parte del contrato.

## 5. Manejo de errores

El contrato expone un catálogo de códigos estables (definido en detalle en el catálogo de errores de la categoría 03). Características del contrato de errores:

- Código estable y único por condición; el código no se traduce ni cambia entre versiones menores.
- Taxonomía: entrada inválida y recurso ausente (defecto de integración), conflicto de estado, error transitorio de conectividad (reintentable o reanudable, sin pérdida ni duplicación) y autenticación.
- Distinción explícita entre defecto de integración y condición transitoria: las segundas no implican pérdida de datos y se resuelven reintentando o reanudando.
- Condición reportada no bloqueante: el elemento en conflicto se reporta en el resumen del ciclo y en la consulta de estado sin abortar (RN-03); no es un error de bloqueo.
- El contrato no expone una categoría de error interno: toda condición es diagnosticable y accionable por el integrador.

## 6. Versionado del contrato

Política de compatibilidad hacia atrás de la superficie pública (ADR-03):

- Versionado semántico. La superficie pública versionada es la capa Abstractions y el contrato del ciclo de vida (operaciones, formas de datos, conjunto de estados, garantía de orden, códigos de error).
- Cambio incompatible (incrementa la versión mayor): quitar o renombrar una operación, un campo o un estado; cambiar la obligatoriedad de un campo; invertir o relajar la garantía de orden subir-antes-de-bajar; alterar la semántica de la no duplicación por identificador; cambiar o quitar un código de error.
- Cambio compatible (incrementa la versión menor): agregar una operación, un campo opcional, un estado nuevo no excluyente o un código de error nuevo, preservando el comportamiento existente.
- Corrección (incrementa la versión de parche): arreglos que no alteran el contrato.
- Deprecación: ningún elemento del contrato se remueve sin un período de deprecación documentado y un incremento de versión mayor en la remoción.
- La implementación interna (componentes, esquema físico de los metadatos, adaptadores concretos del host) no es parte del contrato y puede cambiar sin versión mayor mientras preserve el comportamiento observable.
- Verificación post-publicación: el paquete publicado se restaura en un proyecto limpio y el quick-start reproduce el contrato (intake §17 P.8); un quick-start que no reproduzca el comportamiento bloquea la publicación.

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU cubiertos | CU-01, CU-02, CU-03, CU-04, CU-05, CU-06 |
| RN aplicables | RN-01, RN-02, RN-03 |
| ADRs que lo gobiernan | ADR-01 (qué es superficie pública), ADR-02 (contratos de extensión), ADR-03 (versionado), ADR-07 (idempotencia), ADR-08 (conflicto reportado) |
| Artefactos hermanos | `contratos` de extensión en `extensibilidad_v1.0.md`; catálogo de errores y portal de developers de la categoría 03 |
| Tests previstos (08) | Reproducción del quick-start; contrato de orden subir-antes-de-bajar; no duplicación por identificador; conjunto de estados estable; matriz de compatibilidad de la superficie pública |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Contrato inicial de la superficie pública (capa Abstractions) del motor aplicada-sync: operaciones del ciclo de vida (CU-01 a CU-06), formas de datos, catálogo de errores, política de versionado semántico y trazabilidad. Derivado de la especificación funcional §8, del marco DX de 03 y de los ADR-01/02/03/07/08. |
