# Contratos inter-proyecto — GeoVial

**Proyecto:** GeoVial (solución)
**Documento:** contratos-inter-proyecto_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Soluciones Senior

## 1. Objetivo y alcance

Este documento detalla los cuatro contratos que cruzan la frontera entre dos proyectos de la solución GeoVial. Por cada contrato indica qué proyecto es productor, qué proyecto es consumidor, qué arista del manifiesto materializa, qué expone el productor y qué consume el consumidor. No reescribe los contratos: cada uno está definido en el `contratos-<area>` del proyecto productor, que es la fuente normativa; aquí se referencia y se resume desde la perspectiva de la frontera. La política de versionado de cada contrato vive en su documento de origen y se gobierna a nivel solución por ADR-02; la naturaleza de cada comunicación (red o en proceso) por ADR-03.

Los cuatro contratos corresponden uno a uno con las cuatro aristas del grafo del manifiesto (`SOLUTION-MANIFEST-geovial_v1.0.md` §3). No hay contrato inter-proyecto sin arista, ni arista con contrato formal sin documentar.

## 2. Índice de contratos

| # | Productor | Consumidor | Arista del manifiesto | Naturaleza | Contrato de origen (normativo) |
| --- | --- | --- | --- | --- | --- |
| C-01 | `geovial-storage` | `geovial-api` | `geovial-api → geovial-storage` | En proceso (abstracción) | `proyectos/geovial-storage/05_arquitectura_tecnica/contratos-abstractions_v1.0.md` |
| C-02 | `geovial-api` | `geovial-web` | `geovial-web → geovial-api` | Red (REST autenticado) | `proyectos/geovial-api/05_arquitectura_tecnica/contratos-rest_v1.0.md` |
| C-03 | `geovial-api` | `geovial-mobile` | `geovial-mobile → geovial-api` | Red (REST autenticado) | `proyectos/geovial-api/05_arquitectura_tecnica/contratos-rest_v1.0.md` |
| C-04 | `aplicada-sync` | `geovial-mobile` | `geovial-mobile → aplicada-sync` | En proceso (superficie pública redistribuible) | `proyectos/aplicada-sync/05_arquitectura_tecnica/contratos-abstractions_v1.0.md` |

Nota sobre la dirección de la arista: en el manifiesto la arista apunta del consumidor al productor (quien depende → de quien provee). El contrato lo define y publica el productor; el consumidor lo consume.

## 3. C-01 — Abstracción de almacenamiento (geovial-storage → geovial-api)

- Productor: `geovial-storage` (library, `redistribuible: false`).
- Consumidor: `geovial-api`.
- Arista del manifiesto: `geovial-api → geovial-storage`.
- Contrato normativo: `geovial-storage` `contratos-abstractions_v1.0.md` (no se reescribe aquí).
- Naturaleza (ADR-03): invocación en proceso; la librería se integra embebida en el contenedor del backend y no expone contrato de red propio.

Qué expone el productor: una superficie pública estable de dos interfaces de la capa Abstractions: una interfaz de almacenamiento con las operaciones de datos (guardar, recuperar, eliminar, verificar existencia, listar bajo prefijo) y una interfaz de configuración del proveedor activo (configurar/validar el proveedor). El contrato es idéntico cualquiera sea el proveedor activo (transparencia, RN-01 del productor), garantiza igualdad binaria entre lo guardado y lo recuperado, y nunca devuelve credenciales ni parámetros sensibles en salidas ni en errores. Expone una taxonomía de errores uniforme y estable en mayúsculas.

Qué consume el consumidor: el backend implementa su puerto de almacenamiento delegando en la interfaz de almacenamiento (su adaptador de almacenamiento de archivos, `geovial-api` arquitectura §3) para persistir y recuperar los binarios de las fotos; guarda únicamente el identificador lógico opaco que devuelve la librería, no el binario, en su modelo de datos. La configuración del proveedor activo la consume el backend para materializar el caso de uso de configuración de almacenamiento que el usuario raíz dispara desde el contrato REST. Los códigos de error de la librería se normalizan al cruzar al contrato REST del backend.

Frontera y trazabilidad: gobernada por ADR-09 de `geovial-api` (integración con la abstracción de almacenamiento) del lado consumidor, y por ADR-01/ADR-02/ADR-04/ADR-05 de `geovial-storage` del lado productor. CU del productor: CU-01 a CU-06 de `geovial-storage`. CU de la solución que cruzan esta frontera: la configuración de almacenamiento (CU-17 del backend) y el alojamiento de fotos que sostiene la captura, la carga manual y la portabilidad (CU-08, CU-09, CU-15, CU-16, CU-17 del backend).

## 4. C-02 — Contrato REST (geovial-api → geovial-web)

- Productor: `geovial-api` (rest-api, principal).
- Consumidor: `geovial-web` (web-monolith).
- Arista del manifiesto: `geovial-web → geovial-api`.
- Contrato normativo: `geovial-api` `contratos-rest_v1.0.md` (no se reescribe aquí).
- Naturaleza (ADR-03): REST sobre HTTP con payloads JSON, autenticado con token bearer, versionado por prefijo de versión mayor en la ruta.

Qué expone el productor: el contrato REST completo del backend, organizado por recurso bajo el prefijo de versión mayor: autenticación y sesión, usuarios y agentes, relevamientos y ciclo, asignaciones, marcadores y observaciones (fotos, comentarios, etiquetas, carga manual), sincronización, conflictos, portabilidad y configuración de almacenamiento. Toda operación salvo el inicio de sesión exige token bearer; los listados son paginados y filtrables; las operaciones no seguras reintentables aceptan clave de idempotencia; los errores se devuelven como problem+json RFC 7807 con código estable.

Qué consume el consumidor: el front administrador consume el subconjunto del contrato que cubre sus once casos de uso (CU-01 a CU-11 de `geovial-web`): ingreso y sesión, administración de usuarios y agentes, relevamientos y su ciclo, asignaciones, mapa y marcadores, carrusel de fotos, resolución de conflictos al cierre, transición y cierre, carga manual vía web, portabilidad y configuración de almacenamiento. El front no posee dominio propio (`tiene_persistencia=false`): consume el contrato a través de su Cliente de API, custodia el token bearer del lado servidor del circuito y mapea los errores problem+json a feedback de interfaz. No consume el subconjunto exclusivo del cliente móvil (los endpoints de sincronización subida/bajada son para la app offline-first).

Frontera y trazabilidad: gobernada por ADR-03 de `geovial-web` (token bearer del lado servidor) y ADR-05 de `geovial-web` (mapeo de problem+json a feedback) del lado consumidor, y por ADR-03/ADR-04/ADR-05/ADR-10 de `geovial-api` del lado productor. CU del productor que esta arista activa: el subconjunto administrador de CU-01 a CU-22 del backend, excluyendo los exclusivos de sincronización del cliente móvil (CU-10, CU-11).

## 5. C-03 — Contrato REST (geovial-api → geovial-mobile)

- Productor: `geovial-api` (rest-api, principal).
- Consumidor: `geovial-mobile` (mobile-app-maui).
- Arista del manifiesto: `geovial-mobile → geovial-api`.
- Contrato normativo: `geovial-api` `contratos-rest_v1.0.md` (no se reescribe aquí; mismo contrato que C-02).
- Naturaleza (ADR-03): REST sobre HTTP con payloads JSON, autenticado con token bearer, versionado por URI.

Qué expone el productor: el mismo contrato REST que en C-02, con énfasis en los endpoints que sirven al cliente offline-first: inicio de sesión (emisión del token bearer), selección y consulta de relevamientos asignados, marcadores y observaciones, y en particular los endpoints de sincronización de subida (recibe el lote de cambios locales, deduplica por identificador de origen) y bajada (entrega novedades posteriores a la marca, solo tras concluir la subida). El backend garantiza el orden subir-antes-de-bajar y la idempotencia por identificador de origen.

Qué consume el consumidor: la app consume el contrato a través de su Cliente del contrato REST presentando el token bearer; en la sincronización, el motor `aplicada-sync` consume los endpoints de subida y bajada a través del puerto de transporte que la app implementa (composición de C-03 con C-04). La app reutiliza el token de la sesión activa sin volver a pedir credenciales. Consume el subconjunto del contrato que cubre sus siete casos de uso de captura y sincronización (CU-01 a CU-07 de `geovial-mobile`).

Frontera y trazabilidad: gobernada por ADR-05 de `geovial-mobile` (token seguro y relogueo por dispositivo) y ADR-03 de `geovial-mobile` (motor de sincronización) del lado consumidor, y por ADR-03/ADR-07/ADR-08/ADR-10 de `geovial-api` del lado productor. CU del productor que esta arista activa: el subconjunto de captura y sincronización, incluidos CU-10 y CU-11 (subida y bajada). Esta arista es la única que activa los endpoints de sincronización del backend.

## 6. C-04 — Contrato de sincronización (aplicada-sync → geovial-mobile)

- Productor: `aplicada-sync` (library, `redistribuible: true`).
- Consumidor: `geovial-mobile` (mobile-app-maui).
- Arista del manifiesto: `geovial-mobile → aplicada-sync`.
- Contrato normativo: `aplicada-sync` `contratos-abstractions_v1.0.md` (no se reescribe aquí).
- Naturaleza (ADR-03): superficie pública de un paquete redistribuible, invocada en proceso; no es un contrato de red (el transporte hacia el backend es una abstracción que el host implementa).

Qué expone el productor: una superficie pública versionada (capa Abstractions) con dos caras. La cara consumida por el integrador son las operaciones del ciclo de vida: inicializar sesión, encolar cambio local, ejecutar el ciclo subir-luego-bajar, habilitar el disparo automático ante recuperación de conectividad, consultar estado y cola, y reanudar una subida parcial. La cara implementada por el integrador son los contratos de extensión que el host inyecta (almacén local, transporte hacia el backend remoto, proveedor de credencial). Las invariantes orden subir-antes-de-bajar, idempotencia por identificador de cambio estable y convivencia con conflicto reportado sin bloquear son garantías duras del contrato, no opciones configurables. Expone un catálogo de errores estable que distingue defecto de integración de condición transitoria reanudable.

Qué consume el consumidor: la app encola sus cambios locales con un identificador de origen estable, ejecuta el ciclo de sincronización delegando íntegramente en el motor (no reimplementa orden, idempotencia ni reanudación) e implementa los puertos que el motor requiere: su almacén local, su Cliente del contrato REST como transporte hacia el backend (puente con C-03) y el proveedor de credencial que reutiliza el token de sesión. La app refleja en su interfaz el estado, la cola y los conflictos que el motor reporta.

Frontera y trazabilidad: gobernada por ADR-03 de `geovial-mobile` (sincronización delegada al motor) del lado consumidor, y por ADR-01/ADR-02/ADR-03/ADR-07/ADR-08 de `aplicada-sync` del lado productor. CU del productor: CU-01 a CU-06 de `aplicada-sync`. CU de la solución que cruzan esta frontera: el trabajo offline y la sincronización (CU-06 de `geovial-mobile`), que a su vez se apoya en los endpoints de sincronización del backend (CU-10, CU-11 vía C-03).

## 7. Trazabilidad consolidada

| Contrato | Arista del manifiesto | Productor / contrato de origen | Consumidor | CU que cruzan la frontera |
| --- | --- | --- | --- | --- |
| C-01 | `geovial-api → geovial-storage` | `geovial-storage` / `contratos-abstractions_v1.0.md` | `geovial-api` | CU-08, CU-09, CU-15, CU-16, CU-17 (backend); CU-01..CU-06 (storage) |
| C-02 | `geovial-web → geovial-api` | `geovial-api` / `contratos-rest_v1.0.md` | `geovial-web` | CU-01..CU-11 (web) ↔ subconjunto administrador de CU-01..CU-22 (backend) |
| C-03 | `geovial-mobile → geovial-api` | `geovial-api` / `contratos-rest_v1.0.md` | `geovial-mobile` | CU-01..CU-07 (móvil) ↔ subconjunto de captura/sync incl. CU-10, CU-11 (backend) |
| C-04 | `geovial-mobile → aplicada-sync` | `aplicada-sync` / `contratos-abstractions_v1.0.md` | `geovial-mobile` | CU-06 (móvil); CU-01..CU-06 (sync); compone C-03 (CU-10, CU-11) |

Cada fila corresponde a una arista real del grafo del manifiesto y referencia el `contratos-<area>` del proyecto productor. No existe contrato inter-proyecto sin arista ni arista con contrato formal sin fila en esta tabla.

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Detalle inicial de los cuatro contratos inter-proyecto de GeoVial (C-01 a C-04), cada uno referenciando el contrato de origen del productor y resumiendo qué consume el consumidor, con trazabilidad uno a uno a las aristas del manifiesto y a los CU que cruzan la frontera. |
