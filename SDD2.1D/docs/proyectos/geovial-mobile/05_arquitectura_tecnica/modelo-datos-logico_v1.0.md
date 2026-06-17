# Modelo de datos lógico — almacén local de geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** modelo-datos-logico_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto Móvil

## 0. Propósito y alcance

Este documento es el modelo lógico del almacén local offline del dispositivo: las 8 entidades del modelo conceptual local (02) con sus tipos físicos abstractos, índices, restricciones y la migración inicial, más la cola de cambios y los metadatos de sincronización. El almacén local es una réplica local del dominio autoritativo de la API de relevamientos (ver `proyectos/geovial-api/05_arquitectura_tecnica/modelo-datos-logico_v1.0.md`): no es la fuente de verdad; al sincronizar, la app sube los cambios locales y luego baja las actualizaciones del backend, que prevalecen. La integridad de dominio fina (identidad estable del marcador, referencia obligatoria de observación a marcador, monotonía de la marca de sincronización) la gobierna el backend; el cliente la respeta como réplica.

Los tipos físicos se expresan de forma abstracta (identificador, cadena, entero, decimal, fecha-hora, geográfico, binario/blob, booleano) sin nombrar el motor del almacén local. Trazabilidad al modelo conceptual local en §8.

## 1. Tablas o colecciones

Una tabla por entidad del almacén local. Cada tabla referencia su entidad conceptual de origen (02) y, cuando aplica, la entidad del dominio autoritativo de la API que replica.

### 1.1 relevamiento_local

Copia local de un relevamiento asignado al agente; contexto de captura offline. Entidad conceptual de origen: RelevamientoLocal (réplica de Relevamiento del backend).

| Atributo | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| id | identificador | no nulo | — |
| nombre | cadena | no nulo | — |
| estado | cadena (enum: recoleccion, revision, cierre) | no nulo | recoleccion |
| tramo_resumen | cadena | nulo | — |
| marca_creacion | fecha-hora | no nulo | — |
| marca_actualizacion | fecha-hora | no nulo | — |

### 1.2 marcador_local

Copia local de un marcador geográfico del relevamiento; agrupador de observaciones; identidad estable. Entidad de origen: MarcadorLocal (réplica de MarcadorGeografico).

| Atributo | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| id | identificador | no nulo | — |
| relevamiento_local_id | identificador | no nulo | — |
| coordenada | geográfico | nulo | — |
| pendiente_ubicacion | booleano | no nulo | falso |
| en_conflicto | booleano | no nulo | falso |
| marca_creacion | fecha-hora | no nulo | — |

### 1.3 observacion_local

Copia local de una observación anclada a un marcador local; reúne fotos. Entidad de origen: ObservacionLocal (réplica de Observacion).

| Atributo | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| id | identificador | no nulo | — |
| marcador_local_id | identificador | no nulo | — |
| autor | cadena | no nulo | — |
| nota | cadena | nulo | — |
| marca_creacion | fecha-hora | no nulo | — |

### 1.4 foto_local

Copia local de una foto de una observación; su binario se aloja en el dispositivo hasta sincronizar. Entidad de origen: FotoLocal (réplica de Foto).

| Atributo | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| id | identificador | no nulo | — |
| observacion_local_id | identificador | no nulo | — |
| ubicacion | geográfico | nulo | — |
| pendiente_ubicacion | booleano | no nulo | falso |
| referencia_binario_local | cadena | no nulo | — |
| origen_ubicacion | cadena (enum: gps_momento, incrustada, pendiente) | no nulo | pendiente |
| marca_creacion | fecha-hora | no nulo | — |

Nota: el binario de la imagen se aloja en el dispositivo (tipo binario/blob fuera de la fila de datos) y `referencia_binario_local` apunta a él; no se almacena el binario en esta tabla (ADR-02).

### 1.5 comentario_local

Copia local del texto asociado a una foto local. Entidad de origen: ComentarioLocal (réplica de Comentario).

| Atributo | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| id | identificador | no nulo | — |
| foto_local_id | identificador | no nulo | — |
| texto | cadena | no nulo | — |
| marca_creacion | fecha-hora | no nulo | — |

### 1.6 etiqueta_local

Copia local de una etiqueta aplicable a fotos y a marcadores locales; reutilizable en el relevamiento. Entidad de origen: EtiquetaLocal (réplica de Etiqueta).

| Atributo | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| id | identificador | no nulo | — |
| relevamiento_local_id | identificador | no nulo | — |
| nombre | cadena | no nulo | — |

Relación N a N con fotos y marcadores mediante tablas de asociación:

#### 1.6.1 etiqueta_foto_local (asociación)

| Atributo | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| etiqueta_local_id | identificador | no nulo | — |
| foto_local_id | identificador | no nulo | — |

#### 1.6.2 etiqueta_marcador_local (asociación)

| Atributo | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| etiqueta_local_id | identificador | no nulo | — |
| marcador_local_id | identificador | no nulo | — |

### 1.7 cambio_encolado

Cola local persistente: registro de un cambio local pendiente de sincronizar, con identificador de origen estable para la idempotencia y orden de creación. Entidad de origen: CambioEncolado (artefacto de sincronización del cliente).

| Atributo | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| id | identificador | no nulo | — |
| relevamiento_local_id | identificador | no nulo | — |
| identificador_origen | identificador | no nulo | — |
| tipo_operacion | cadena (enum: crear, mover, anclar, comentar, etiquetar, carga_manual) | no nulo | — |
| elemento_tipo | cadena (enum: marcador, observacion, foto, comentario, etiqueta) | no nulo | — |
| elemento_local_id | identificador | no nulo | — |
| orden_creacion | entero | no nulo | — |
| estado_sincronizacion | cadena (enum: pendiente, confirmado) | no nulo | pendiente |
| marca_creacion | fecha-hora | no nulo | — |

### 1.8 marca_sincronizacion_local

Metadato del punto de sincronización del relevamiento en el dispositivo; lo gestiona la librería de sincronización. Entidad de origen: MarcaSincronizacionLocal (réplica de MarcaSincronizacion).

| Atributo | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| relevamiento_local_id | identificador | no nulo | — |
| valor | cadena (opaco) | nulo | — |
| marca_ultima_sincronizacion | fecha-hora | nulo | — |

## 2. Índices

| Índice | Tabla | Columnas | Tipo | Motivación |
| --- | --- | --- | --- | --- |
| pk_relevamiento_local | relevamiento_local | id | único (PK) | Identidad replicada del backend |
| ix_marcador_relevamiento | marcador_local | relevamiento_local_id | compuesto | Listar marcadores del relevamiento activo (CU-02, CU-03) |
| ix_marcador_conflicto | marcador_local | relevamiento_local_id, en_conflicto | parcial | Localizar marcadores en conflicto que conviven (RN-03) |
| ix_observacion_marcador | observacion_local | marcador_local_id | compuesto | Reunir observaciones de un marcador (CU-04, CU-05) |
| ix_foto_observacion | foto_local | observacion_local_id | compuesto | Reunir fotos de una observación (CU-04, CU-05) |
| ix_foto_pendiente | foto_local | pendiente_ubicacion | parcial | Localizar fotos pendientes de ubicación (RN-01, CU-07) |
| ix_comentario_foto | comentario_local | foto_local_id | único | A lo sumo un comentario por foto (RN del dominio) |
| ix_etiqueta_relevamiento | etiqueta_local | relevamiento_local_id, nombre | único | Etiqueta reutilizable y no repetida por nombre en el relevamiento |
| ux_cambio_identificador_origen | cambio_encolado | identificador_origen | único | Idempotencia: reconoce reenvíos del mismo cambio (RN-02) |
| ix_cambio_cola | cambio_encolado | relevamiento_local_id, estado_sincronizacion, orden_creacion | compuesto | Drenar la cola en orden de creación, solo pendientes (RN-02, RN-05) |
| pk_marca_sincronizacion | marca_sincronizacion_local | relevamiento_local_id | único (PK) | Una marca vigente por relevamiento (RC-06 replicada) |

## 3. Restricciones

- Claves primarias: `id` en relevamiento_local, marcador_local, observacion_local, foto_local, comentario_local, etiqueta_local y cambio_encolado; `relevamiento_local_id` en marca_sincronizacion_local; clave compuesta en las tablas de asociación.
- Claves foráneas locales: marcador_local.relevamiento_local_id → relevamiento_local.id; observacion_local.marcador_local_id → marcador_local.id (referencia obligatoria, replica RC-02 del backend); foto_local.observacion_local_id → observacion_local.id; comentario_local.foto_local_id → foto_local.id; etiqueta_local.relevamiento_local_id → relevamiento_local.id; las asociaciones referencian etiqueta_local, foto_local y marcador_local; cambio_encolado.relevamiento_local_id → relevamiento_local.id; marca_sincronizacion_local.relevamiento_local_id → relevamiento_local.id.
- Únicas: identificador_origen único en cambio_encolado (idempotencia, RN-02); a lo sumo un comentario por foto (único en comentario_local.foto_local_id); etiqueta no repetida por nombre dentro del relevamiento.
- Check / valores permitidos: estado del relevamiento en {recoleccion, revision, cierre} (solo lectura en el cliente; la transición la gobierna el backend); estado_sincronizacion en {pendiente, confirmado}; tipo_operacion y elemento_tipo en sus conjuntos cerrados; origen_ubicacion en {gps_momento, incrustada, pendiente}.
- Invariantes replicadas (gobernadas por el backend, respetadas por el cliente): identidad estable del marcador ante movimiento y etiquetado (RC-01); referencia obligatoria de observación a marcador (RC-02); monotonía del valor de la marca de sincronización (RC-06). Una foto puede quedar pendiente de ubicación sin coordenada inventada (RN-01).

## 4. Migración inicial

- Identificador de migración: `0001_inicial_almacen_local`.
- Tooling: el versionado del esquema local se aplica con la herramienta de migraciones del runtime de la app, ejecutada en el arranque (intake §17.P.4). Sin nombrar producto concreto.
- Resumen del cambio: crea las 8 tablas (relevamiento_local, marcador_local, observacion_local, foto_local, comentario_local, etiqueta_local, cambio_encolado, marca_sincronizacion_local) y las dos tablas de asociación de etiquetas, con sus claves, restricciones e índices de §2 y §3. Establece la versión de esquema local 1.
- Política de evolución: cada cambio de esquema local es una migración versionada nueva, aplicada en el arranque; el esquema se reconstruye desde `0001` (ADR-02). Un cambio del dominio autoritativo que altere la réplica origina una migración local nueva.

## 5. Estrategia multi-tenant

No aplica. El almacén local es de un único agente y un único dispositivo; la solución es single-tenant (intake §17.P.4, `multi_tenant = false`). No hay columna discriminadora de tenant ni partición por tenant.

## 6. Trazabilidad

| Tabla local | Entidad conceptual de origen (02) | Entidad del dominio autoritativo (API) | CU que la consumen | RN que la restringen |
| --- | --- | --- | --- | --- |
| relevamiento_local | RelevamientoLocal | Relevamiento | CU-02, CU-03, CU-04, CU-05, CU-06, CU-07 | RN-05 |
| marcador_local | MarcadorLocal | MarcadorGeografico | CU-03, CU-04, CU-07 | RN-01, RN-03, RN-05 |
| observacion_local | ObservacionLocal | Observacion | CU-04, CU-05, CU-07 | RN-05 |
| foto_local | FotoLocal | Foto | CU-04, CU-05, CU-07 | RN-01, RN-05 |
| comentario_local | ComentarioLocal | Comentario | CU-05 | RN-05 |
| etiqueta_local (+ asociaciones) | EtiquetaLocal | Etiqueta | CU-05, CU-07 | RN-05 |
| cambio_encolado | CambioEncolado | — (artefacto de sincronización del cliente) | CU-03, CU-04, CU-05, CU-06, CU-07 | RN-02, RN-05 |
| marca_sincronizacion_local | MarcaSincronizacionLocal | MarcaSincronizacion | CU-06 | RN-02 |

Cada tabla lógica tiene una entidad conceptual de origen en el modelo conceptual local (02). ADRs que gobiernan el modelo: ADR-02 (persistencia y migraciones), ADR-03 (cola e idempotencia), ADR-04 (foto pendiente de ubicación). Downstream: 06 (US del almacén local), 08 (pruebas de migración, capacidad de cola e integridad de la réplica).

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Modelo lógico inicial del almacén local: 8 tablas (más dos de asociación de etiquetas) con tipos físicos abstractos, índices, restricciones y migración inicial `0001_inicial_almacen_local`. Réplica local del dominio autoritativo de la API; cola con identificador de origen único para idempotencia y marca de sincronización monótona. Trazabilidad al modelo conceptual local de 02. |
