# Especificación funcional — aplicada-sync (guía de la sección)

Esta sección reúne la especificación funcional del proyecto `aplicada-sync`, el motor de sincronización redistribuible para aplicaciones móviles. El punto de entrada formal es el índice maestro [especificacion-funcional_v1.0.md](especificacion-funcional_v1.0.md); este README complementa con la guía de navegación, el orden de lectura y la naturaleza de cada artefacto.

`aplicada-sync` es de tipo `library`. Por ello no incluye modelo conceptual de datos (reglas de la categoría 02, §2.2) y sus reglas de negocio son recomendadas, presentes aquí porque la API pública expone reglas de dominio del motor.

## Alcance y vocabulario

La librería es agnóstica del dominio de la solución que la consume. Los CU se redactan en términos genéricos: almacén local del host, cambio local, backend remoto, sesión de sincronización y paquete distribuible. La política central del motor es subir primero los cambios locales del host y luego bajar las actualizaciones del backend. La trazabilidad de negocio apunta a NB-04, la necesidad de GeoVial que esta librería sirve a través de la aplicación de campo que la integra.

## Tabla de casos de uso

| CU | Título | Actor primario | RN aplicables | Estado | Enlace |
| --- | --- | --- | --- | --- | --- |
| CU-01 | Inicializar y configurar la sesión de sincronización | Aplicación host | RN-02 | Propuesto | [archivo](casos-de-uso/CU-01-inicializar-sesion-sincronizacion_v1.0.md) |
| CU-02 | Registrar y encolar un cambio local | Aplicación host | RN-02 | Propuesto | [archivo](casos-de-uso/CU-02-registrar-cambio-local_v1.0.md) |
| CU-03 | Ejecutar la sincronización subir-luego-bajar | Aplicación host | RN-01, RN-02, RN-03 | Propuesto | [archivo](casos-de-uso/CU-03-ejecutar-sincronizacion_v1.0.md) |
| CU-04 | Detectar conectividad y disparar la sincronización | Detector de conectividad | RN-01 | Propuesto | [archivo](casos-de-uso/CU-04-detectar-conectividad-disparar-sync_v1.0.md) |
| CU-05 | Consultar estado del motor y cola de pendientes | Aplicación host | RN-03 | Propuesto | [archivo](casos-de-uso/CU-05-consultar-estado-cola_v1.0.md) |
| CU-06 | Reanudar una sincronización interrumpida | Aplicación host | RN-01, RN-02 | Propuesto | [archivo](casos-de-uso/CU-06-reanudar-sincronizacion-interrumpida_v1.0.md) |

## Tabla de reglas de negocio

| RN | Título | Naturaleza | CU afectados | Estado | Enlace |
| --- | --- | --- | --- | --- | --- |
| RN-01 | Orden estricto subir-antes-de-bajar | Invariante de orden | CU-03, CU-04, CU-06 | Propuesto | [archivo](reglas-de-negocio/RN-01-orden-subir-antes-de-bajar_v1.0.md) |
| RN-02 | Idempotencia de la sincronización | Invariante de integridad | CU-02, CU-03, CU-06 | Propuesto | [archivo](reglas-de-negocio/RN-02-idempotencia-sincronizacion_v1.0.md) |
| RN-03 | Convivencia con estados en conflicto sin bloqueo | Invariante de continuidad | CU-03, CU-05 | Propuesto | [archivo](reglas-de-negocio/RN-03-convivencia-estados-en-conflicto_v1.0.md) |

## Orden de lectura sugerido

1. CU-01 — el punto de entrada: cómo se configura e inicializa el motor.
2. CU-02 — cómo entra el trabajo al motor: el encolado de cambios locales.
3. CU-03 — el corazón del contrato: el ciclo subir-luego-bajar.
4. CU-04 — el disparo automático ante recuperación de conectividad.
5. CU-05 — la observabilidad: estado y cola de pendientes.
6. CU-06 — la resiliencia: reanudar tras una subida parcial interrumpida.

Las tres RN se leen junto a CU-03, donde convergen las tres invariantes del motor.

## Modelo de datos

No aplica. `aplicada-sync` es de tipo `library`; las reglas de la categoría 02 (§2.2) omiten el modelo conceptual para este tipo. La gestión de metadatos de sincronización sobre el almacén local del host se describe a nivel de comportamiento en los CU; su estructura física pertenece a la categoría 05.

## Compatibilidad de versión pública

Por ser redistribuible, cada CU incluye una sección §17 con las consideraciones de compatibilidad de su porción de la superficie pública. El índice maestro (§8) consolida la política: todo cambio que altere el contrato descrito en estos CU obliga a un incremento de versión mayor del paquete.

## Trazabilidad

Upstream: los seis CU cubren NB-04 (trabajo sin conexión con sincronización confiable). Ningún CU queda huérfano y NB-04 no queda sin CU del lado de la librería. Downstream: cada CU enumera las US a generar en 06, los componentes esperados en 05 y los tests previstos en 08, consolidados en la matriz del índice maestro.

## Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección con tablas de CU y RN, orden de lectura, nota de modelo de datos y trazabilidad, para un catálogo de 6 CU y 3 RN. |
