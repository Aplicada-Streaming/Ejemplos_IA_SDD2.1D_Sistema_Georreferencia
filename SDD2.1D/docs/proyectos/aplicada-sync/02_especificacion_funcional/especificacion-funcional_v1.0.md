# Especificación funcional — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** especificacion-funcional_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Este índice maestro consolida la especificación funcional de `aplicada-sync`, el motor de sincronización para aplicaciones móviles. Cada caso de uso (CU) describe un contrato de uso de la superficie pública de la librería: qué expone, qué espera de quien la integra y qué garantiza a cambio. Las reglas de negocio (RN) recogen las invariantes atemporales que el motor mantiene en cualquier integración.

`aplicada-sync` es una librería redistribuible y agnóstica del dominio: sincroniza los cambios locales de una aplicación host contra un backend remoto, sin conocer la naturaleza de esos cambios. La política central del motor es subir primero los cambios locales del host y luego bajar las actualizaciones del backend. Por ese carácter reutilizable, los CU se redactan en términos genéricos de almacén local del host, cambio local, backend remoto y sesión de sincronización, y no en términos del dominio de la solución que la consume.

La trazabilidad de negocio apunta a NB-04 (trabajo sin conexión con sincronización confiable), que es la necesidad de la solución GeoVial que esta librería sirve a través de la aplicación de campo que la integra. El contrato, sin embargo, es general y reutilizable fuera de esa solución.

## 2. Alcance de la especificación

### 2.1 Qué cubre

La especificación cubre el contrato del ciclo de vida del motor de sincronización: configurar e inicializar una sesión de sincronización, registrar y encolar un cambio local, ejecutar la sincronización con el orden subir-luego-bajar, detectar la conectividad y disparar la sincronización de forma automática, consultar el estado del motor y la cola de pendientes, y reanudar una sincronización interrumpida por una subida parcial.

### 2.2 Qué queda fuera

Quedan fuera del contrato de esta librería, por pertenecer a la aplicación host o al backend remoto: la autenticación y la emisión de credenciales (el motor reutiliza el token que le provee el host), la semántica de dominio de cada cambio local, la persistencia primaria de los datos de dominio del host, la lógica de resolución de conflictos del lado del backend (el motor convive con el estado en conflicto y lo reporta, pero no decide la unificación) y toda interacción visual con la persona usuaria, que vive en la categoría 03.

### 2.3 Tipo de proyecto y mínimos

`aplicada-sync` es de tipo `library`. Según las reglas de la categoría 02 (§2.2), el mínimo es de cinco CU, el modelo conceptual de datos se omite y las reglas de negocio son recomendadas cuando hay reglas de dominio en la API pública. Esta especificación declara seis CU y tres RN, y no incluye modelo conceptual.

## 3. Catálogo de casos de uso

| ID | Caso de uso | Actor primario | NB | Estado | Enlace |
| --- | --- | --- | --- | --- | --- |
| CU-01 | Inicializar y configurar la sesión de sincronización | Aplicación host | NB-04 | Propuesto | [archivo](casos-de-uso/CU-01-inicializar-sesion-sincronizacion_v1.0.md) |
| CU-02 | Registrar y encolar un cambio local | Aplicación host | NB-04 | Propuesto | [archivo](casos-de-uso/CU-02-registrar-cambio-local_v1.0.md) |
| CU-03 | Ejecutar la sincronización subir-luego-bajar | Aplicación host | NB-04 | Propuesto | [archivo](casos-de-uso/CU-03-ejecutar-sincronizacion_v1.0.md) |
| CU-04 | Detectar conectividad y disparar la sincronización | Detector de conectividad | NB-04 | Propuesto | [archivo](casos-de-uso/CU-04-detectar-conectividad-disparar-sync_v1.0.md) |
| CU-05 | Consultar estado del motor y cola de pendientes | Aplicación host | NB-04 | Propuesto | [archivo](casos-de-uso/CU-05-consultar-estado-cola_v1.0.md) |
| CU-06 | Reanudar una sincronización interrumpida | Aplicación host | NB-04 | Propuesto | [archivo](casos-de-uso/CU-06-reanudar-sincronizacion-interrumpida_v1.0.md) |

Numeración: el catálogo de `aplicada-sync` numera sus CU desde CU-01 de forma local al proyecto. La numeración CU-10 y CU-11 reservada por NB-04 corresponde al proyecto principal `geovial-api`, que expone los endpoints de sincronización del lado del backend; no debe confundirse con la numeración de esta librería.

## 4. Catálogo de reglas de negocio

| ID | Regla | Naturaleza | CU afectados | Estado | Enlace |
| --- | --- | --- | --- | --- | --- |
| RN-01 | Orden estricto subir-antes-de-bajar | Invariante de orden | CU-03, CU-04, CU-06 | Propuesto | [archivo](reglas-de-negocio/RN-01-orden-subir-antes-de-bajar_v1.0.md) |
| RN-02 | Idempotencia de la sincronización | Invariante de integridad | CU-02, CU-03, CU-06 | Propuesto | [archivo](reglas-de-negocio/RN-02-idempotencia-sincronizacion_v1.0.md) |
| RN-03 | Convivencia con estados en conflicto sin bloqueo | Invariante de continuidad | CU-03, CU-05 | Propuesto | [archivo](reglas-de-negocio/RN-03-convivencia-estados-en-conflicto_v1.0.md) |

## 5. Matriz de trazabilidad NB → CU → RN → US

La matriz liga cada CU con su necesidad de negocio upstream, las reglas que lo restringen y las historias de usuario que se generarán en la categoría 06. Las US se enumeran con identificadores locales del proyecto `aplicada-sync`.

| NB | CU | RN aplicables | US a generar en 06 |
| --- | --- | --- | --- |
| NB-04 | CU-01 inicializar y configurar la sesión de sincronización | RN-02 | US-01, US-02 |
| NB-04 | CU-02 registrar y encolar un cambio local | RN-02 | US-03, US-04 |
| NB-04 | CU-03 ejecutar la sincronización subir-luego-bajar | RN-01, RN-02, RN-03 | US-05, US-06, US-07 |
| NB-04 | CU-04 detectar conectividad y disparar la sincronización | RN-01 | US-08, US-09 |
| NB-04 | CU-05 consultar estado del motor y cola de pendientes | RN-03 | US-10, US-11 |
| NB-04 | CU-06 reanudar una sincronización interrumpida | RN-01, RN-02 | US-12, US-13 |

Cobertura bidireccional: NB-04 queda cubierta por los seis CU; ningún CU queda huérfano de NB. Cada RN está referenciada por al menos un CU y cada CU referencia al menos una RN aplicable.

## 6. Componentes esperados en 05

Referencia tentativa, no vinculante, a los componentes que la categoría 05 detallará para realizar estos CU. Se expresan de forma abstracta, sin stack:

- Coordinador de sesión de sincronización: punto de entrada público que el host configura e inicializa (CU-01).
- Cola de cambios locales pendientes: registro persistente y ordenado de los cambios a subir (CU-02, CU-05, CU-06).
- Ejecutor de subida y de bajada: realiza la fase de subida y la de bajada respetando el orden (CU-03, CU-06).
- Observador de conectividad: fuente de eventos de cambio de conectividad que dispara el ciclo (CU-04).
- Registro de estado y progreso de la sincronización: expone estado, progreso y diagnóstico (CU-05, CU-06).

## 7. Tests previstos en 08

Cada CU declara en su sección de trazabilidad los tests previstos. A nivel de la especificación se anticipan, como mínimo, suites de prueba para: el orden subir-luego-bajar (RN-01), la idempotencia ante reenvíos y reanudaciones (RN-02), la continuidad ante estados en conflicto (RN-03), el comportamiento ante pérdida de conectividad durante una subida parcial (CU-06) y el disparo automático ante un evento de conectividad recuperada (CU-04).

## 8. Compatibilidad de versión pública

Por ser redistribuible, `aplicada-sync` se rige por una política de compatibilidad de su superficie pública. Todo cambio que altere el contrato descrito en estos CU (firmas de la superficie pública, semántica del orden subir-luego-bajar, contrato de los identificadores de cambio local o de los códigos de error) constituye un cambio incompatible y obliga a un incremento de versión mayor. Las aclaraciones y correcciones que no alteran el contrato avanzan la versión menor o de parche. Cada CU registra en su sección §17 las consideraciones de compatibilidad específicas.

## 9. Convenciones

- Idioma rioplatense técnico, con tildes y eñes en el cuerpo; nombres de archivo en ASCII.
- Fechas en formato YYYY-MM-DD. Codificación UTF-8 con fin de línea LF.
- Nomenclatura de artefactos según §3 de las reglas de la categoría: `CU-XX-<kebab>_v<X.Y>.md` y `RN-XX-<kebab>_v<X.Y>.md`, con `_v` y slug en minúsculas.
- Vocabulario neutral: almacén local del host, cambio local, backend remoto, sesión de sincronización, paquete distribuible. No se mencionan stacks, productos comerciales ni protocolos concretos; esos detalles viven en la categoría 05.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice maestro inicial de la especificación funcional de aplicada-sync: 6 CU, 3 RN, matriz NB→CU→RN→US y referencias downstream a 05, 06 y 08. Derivado de NB-04 y del SOLUTION-INTAKE §17 (bloque aplicada-sync) y §3. |
