# Guía de integración — aplicación móvil host

**Proyecto:** aplicada-sync
**Documento:** guia-integracion-aplicacion-movil_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-15
**Autor:** Technical Writer + SDK Documentation Lead (AG-10)
**Tipo Diátaxis:** How-to
**Audiencia:** Developer integrador que incorpora el motor a una aplicación host móvil
**Nivel:** Medio
**Tiempo estimado de lectura:** 18 min

How-to para integrar el motor de sincronización en una aplicación host móvil genérica que trabaja parte del tiempo sin conexión. El sistema objetivo es una aplicación móvil host abstracta, no un stack comercial concreto: los pasos describen el contrato y el comportamiento, no la sintaxis de una plataforma; el código ejecutable vive en la categoría 11 (sample `03-avanzado-demo-maui`). El vocabulario en `código kebab` se define en `glosario-tecnico_v1.0.md`. El porqué de cada decisión vive en `conceptos-fundamentales_v1.0.md`; acá solo el cómo.

## 1. Objetivo

Dejar una aplicación móvil host capturando cambios sin conexión y propagándolos a un backend remoto de forma confiable: subir primero los cambios locales y luego bajar las actualizaciones, con disparo automático al recuperar la red y reanudación sin pérdida ante cortes. Al terminar, el host tiene el motor integrado con sus cuatro `punto-de-extension` registrados y un ciclo verificado de punta a punta.

## 2. Prerequisites

Estado inicial mínimo de la aplicación móvil host:

| Prerequisito | Estado esperado |
| --- | --- |
| Paquete distribuible incorporado | El motor está disponible para configurarse en el host (ver README de la sección). |
| Almacenamiento local del host | Disponible y accesible para escritura de metadatos. |
| Backend remoto alcanzable | Implementa el contrato de transporte: recibe cambios por `identificador-de-cambio-estable` y entrega actualizaciones posteriores a una marca. |
| Credencial del host | El host sabe obtener y renovar la credencial de autenticación (el motor la reutiliza, no la emite). |
| Señal de conectividad de la plataforma | El host puede observar transiciones de red para alimentar la `fuente-de-conectividad`. |

Haber completado `guia-onboarding-developer_v1.0.md` hasta el tramo de 30 min es recomendable pero no obligatorio.

## 3. Pasos

### Paso 1. Implementar la estrategia de almacén local

Implementar la `estrategia-de-almacen-local` (`referencia-api_v1.0.md` §4.1) sobre el almacenamiento local del host: persistir y leer la `cola-local` y los metadatos de sincronización.
Efecto esperado: la estrategia persiste una entrada por `identificador-de-cambio-estable`, conserva el orden de creación y sobrevive a reinicios de la aplicación.

### Paso 2. Implementar la estrategia de transporte

Implementar la `estrategia-de-transporte` (`referencia-api_v1.0.md` §4.2) contra los endpoints de sincronización del backend: enviar un cambio por identificador estable y obtener actualizaciones posteriores a una marca.
Efecto esperado: el backend reconoce el identificador estable (idempotencia) y puede reportar un `elemento-en-conflicto` como condición no bloqueante (ADR-07, ADR-08).

### Paso 3. Implementar el proveedor de credencial

Implementar el `proveedor-de-credencial` (`referencia-api_v1.0.md` §4.3) que entrega la credencial vigente del host (reutiliza el token de autenticación de la app).
Efecto esperado: la sesión puede pasar de `no autenticada` a `listo` cuando hay credencial vigente.

### Paso 4. Inicializar la sesión

Armar la `configuracion-de-sesion` con el identificador de host y las estrategias de los pasos 1 a 3, y solicitar Inicializar sesión (CU-01).
Efecto esperado: identificador de sesión no vacío y estado `listo` (o `no autenticada` si la credencial aún no está disponible). Un rechazo apunta a una estrategia obligatoria ausente.

### Paso 5. Capturar y encolar cambios sin conexión

Por cada captura del host, encolar un `cambio-local` con un identificador estable, su operación, su carga útil opaca y su marca de orden (CU-02), sin requerir red.
Efecto esperado: la cola crece de a uno; reencolar el mismo identificador no la hace crecer.

### Paso 6. Implementar la fuente de conectividad y habilitar el disparo automático

Implementar la `fuente-de-conectividad` (`referencia-api_v1.0.md` §4.4) sobre la señal de red de la plataforma, inyectarla y habilitar el `disparo-automatico` (CU-04).
Efecto esperado: el motor queda a la espera; al recuperar la red, dispara un ciclo y notifica el resultado por el evento de resultado de ciclo. Ante rebote de red durante un ciclo, ignora los eventos redundantes (no reentrada).

### Paso 7. Manejar el resumen y los conflictos

Suscribir el host al evento de resultado de ciclo y leer el `resumen-del-ciclo`: subidos, bajados y lista de `elemento-en-conflicto`.
Efecto esperado: el ciclo concluye aun con elementos en conflicto; el host los presenta para que se resuelvan fuera del motor (el motor no decide la unificación, ADR-08).

### Paso 8. Reanudar tras un corte

Cuando el estado quede `reanudable`, solicitar Reanudar sincronización (CU-06).
Efecto esperado: el motor reenvía solo los no confirmados y, tras concluir la subida, baja; el `resumen-de-reanudacion` distingue nuevos confirmados de reconocidos como ya recibidos.

## 4. Verificación

Confirmar que la integración funciona con estos checks observables:

| Check | Cómo confirmarlo | Criterio de éxito |
| --- | --- | --- |
| Encolado offline | Encolar con la red desconectada y consultar estado (CU-05). | La cola crece y persiste; no requiere red. |
| Orden subir-antes-de-bajar | Ejecutar con cola no vacía y leer el resumen. | Los subidos aparecen antes que cualquier bajado. |
| Disparo automático | Recuperar la red con cambios pendientes. | Arranca un ciclo sin invocación manual y llega el evento de resultado. |
| Reanudación sin pérdida | Cortar la subida, consultar estado, reanudar. | Estado `reanudable`; al reanudar, ningún dato perdido ni duplicado; ninguna bajada antes de concluir la subida. |
| Convivencia con conflicto | Bajar una entidad marcada en conflicto. | El ciclo concluye y la reporta; no aborta. |
| Idempotencia | Reencolar y reenviar el mismo cambio. | Efecto neto único; la cola no duplica. |

## 5. Troubleshooting específico de esta integración

Subconjunto de problemas frecuentes al integrar en una app móvil host. El diagnóstico paso a paso completo está en `troubleshooting_v1.0.md`.

| Síntoma en la app host | Issue global | Acción |
| --- | --- | --- |
| La inicialización se rechaza al armar la sesión. | ISSUE-05 (`CONFIGURACION_INCOMPLETA`) | Verificar que las estrategias obligatorias (almacén local, transporte) estén registradas en la configuración. |
| El disparo automático no ejecuta al recuperar la red. | ISSUE-03 | Confirmar habilitación, credencial vigente y `fuente-de-conectividad` suscripta. |
| El ciclo se corta y la app no sabe qué hacer. | ISSUE-02 | La sesión quedó `reanudable`; invocar Reanudar (paso 8); no se perdieron datos. |
| Aparecen elementos en conflicto tras una bajada. | ISSUE-01 | Es condición reportada, no error; presentarlos para resolución fuera del motor. |
| La cola crece más de lo esperado. | ISSUE-06 | Verificar que el host asigna un `identificador-de-cambio-estable` único por cambio. |
| La app no autentica contra el backend al sincronizar. | ISSUE-07 (`CREDENCIAL_INVALIDA` / `SESION_NO_AUTENTICADA`) | Renovar la credencial del host y reintentar; el motor no altera la cola ante credencial inválida. |

## 6. Referencias cruzadas

- 05 `extensibilidad_v1.0.md` §3, §4, §5: los cuatro puntos de extensión, su contrato y su registro explícito por el host.
- 05 `contratos-abstractions_v1.0.md` §3: operaciones que esta integración invoca.
- 05 ADR-02 (inversión de dependencias hacia adaptadores del host), ADR-08 (convivencia con conflicto).
- 08 `estrategia-testing_v1.0.md` §5: dobles de transporte y conectividad para verificar los checks de §4 sin backend real.
- 11 sample `03-avanzado-demo-maui`: integración ejecutable en una app host móvil ajena a la solución.
- `referencia-api_v1.0.md`, `troubleshooting_v1.0.md`, `conceptos-fundamentales_v1.0.md`, `glosario-tecnico_v1.0.md`.

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | How-to inicial de integración en una aplicación móvil host genérica: ocho pasos imperativos (implementar las cuatro estrategias, inicializar, encolar offline, disparo automático, manejo de resumen/conflicto y reanudación), verificación con checks observables y troubleshooting específico enlazado al global. Slug genérico `aplicacion-movil`, sin stack comercial. Derivado de `extensibilidad_v1.0.md` y `contratos-abstractions_v1.0.md` de 05 y del sample de 11. |
