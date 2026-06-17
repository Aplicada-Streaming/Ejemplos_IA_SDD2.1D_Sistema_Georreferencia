# Modelo de datos lógico — geovial-api

**Proyecto:** geovial-api
**Documento:** modelo-datos-logico_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer

## 0. Propósito y alcance

Este documento mapea las 12 entidades del modelo conceptual de 02 (`modelo-datos/modelo-conceptual_v1.0.md`) a un modelo lógico de tablas con tipos físicos, índices y restricciones, e incorpora las tablas técnicas de soporte que la arquitectura exige (claves de idempotencia). Aquí sí viven los tipos físicos —cadena, entero, decimal, fecha-hora, identificador, geográfico, lógico, binario-pequeño— expresados de forma neutral, sin nombrar el motor relacional concreto, que se fija en la categoría 09. La solución es single-tenant (multi_tenant=false, intake §17.P.4): no hay columna discriminadora de tenant ni partición por organización. Gobiernan este modelo ADR-02 (almacén relacional con migraciones versionadas), ADR-06 (tolerancia a conflictos), ADR-08 (idempotencia) y ADR-09 (referencia de foto en el almacén, binario fuera).

Convención de tipos físicos lógicos usados (sin producto comercial):

- identificador: clave sustituta opaca, única y estable (no se reusa).
- cadena(n): texto de longitud acotada n; cadena para texto sin límite fijo declarado.
- entero: número entero.
- decimal(p,e): número con precisión p y escala e, para coordenadas.
- geografico: tipo geográfico de punto (latitud/longitud) cuando el motor lo soporta; alternativa por par de decimal documentada por columna.
- fecha-hora: instante con zona, para auditoría y orden temporal.
- logico: verdadero/falso.
- enum(valores): catálogo cerrado de valores permitidos, materializado como cadena con restricción check o como tabla de catálogo.

## 1. Tablas

### 1.1 Rol — entidad conceptual 1.2 Rol

Catálogo cerrado de los cuatro niveles de la jerarquía. Materializa el atributo Nivel de un catálogo cerrado.

| Columna | Tipo | Nulabilidad | Default | Notas |
| --- | --- | --- | --- | --- |
| id_rol | identificador | No nulo | — | PK |
| nivel | enum(RAIZ, JEFE_GENERAL, JEFE_AREA, AGENTE) | No nulo | — | Único; posición jerárquica |
| orden_jerarquico | entero | No nulo | — | 1=RAIZ … 4=AGENTE; sostiene la comparación de nivel inmediato (RC-03) |

### 1.2 Usuario — entidad conceptual 1.1 Usuario

Persona u operador con identidad de acceso y rol. La baja inhabilita sin borrar (RN-02).

| Columna | Tipo | Nulabilidad | Default | Notas |
| --- | --- | --- | --- | --- |
| id_usuario | identificador | No nulo | — | PK |
| identificador_acceso | cadena(256) | No nulo | — | Identidad de autenticación; único en todo el sistema |
| rol_id | identificador | No nulo | — | FK a Rol |
| administrado_por | identificador | Nulo | — | FK autorreferente a Usuario; nulo solo para el rol RAIZ (RC-03) |
| estado_habilitacion | enum(HABILITADO, INHABILITADO) | No nulo | HABILITADO | La baja pasa a INHABILITADO sin borrar (RN-02) |
| secreto_credencial | cadena(512) | No nulo | — | Representación derivada de la credencial (nunca en claro); detalle en 09 |
| creado_en | fecha-hora | No nulo | — | Auditoría |
| actualizado_en | fecha-hora | No nulo | — | Auditoría |

### 1.3 Relevamiento — entidad conceptual 1.3 Relevamiento

Unidad de trabajo con ciclo recolección, revisión y cierre, creada por un jefe de área.

| Columna | Tipo | Nulabilidad | Default | Notas |
| --- | --- | --- | --- | --- |
| id_relevamiento | identificador | No nulo | — | PK |
| estado | enum(RECOLECCION, REVISION, CIERRE) | No nulo | RECOLECCION | Catálogo cerrado; transiciones acotadas (RC-04, RN-05) |
| creado_por | identificador | No nulo | — | FK a Usuario (jefe de área); conservado en la baja del autor (RN-02) |
| nombre | cadena(200) | No nulo | — | Rótulo del relevamiento |
| creado_en | fecha-hora | No nulo | — | Auditoría |
| actualizado_en | fecha-hora | No nulo | — | Sostiene el cálculo de novedades de bajada (CU-11) |
| cerrado_en | fecha-hora | Nulo | — | Instante del cierre; nulo mientras no esté cerrado |

### 1.4 TramoVial — entidad conceptual 1.4 TramoVial

Alcance geográfico del relevamiento (uno por relevamiento), compuesto por puentes y caminos; composición no vacía (CU-04).

| Columna | Tipo | Nulabilidad | Default | Notas |
| --- | --- | --- | --- | --- |
| id_tramo | identificador | No nulo | — | PK |
| relevamiento_id | identificador | No nulo | — | FK a Relevamiento; único (relación 1—1) |
| descripcion | cadena(500) | No nulo | — | Descripción del tramo |

### 1.5 TramoComponente — soporte de la composición de TramoVial (atributo ComposicionPuentesCaminos)

Materializa el conjunto no vacío de puentes y caminos del tramo; desnormaliza el atributo conceptual ComposicionPuentesCaminos en filas.

| Columna | Tipo | Nulabilidad | Default | Notas |
| --- | --- | --- | --- | --- |
| id_componente | identificador | No nulo | — | PK |
| tramo_id | identificador | No nulo | — | FK a TramoVial |
| tipo | enum(PUENTE, CAMINO) | No nulo | — | Clase del componente |
| identificacion | cadena(200) | No nulo | — | Nombre o referencia del puente/camino |

### 1.6 Asignacion — entidad conceptual 1.5 Asignacion

Vínculo agente-relevamiento que habilita a recolectar; único por par vigente (RC-05).

| Columna | Tipo | Nulabilidad | Default | Notas |
| --- | --- | --- | --- | --- |
| id_asignacion | identificador | No nulo | — | PK |
| relevamiento_id | identificador | No nulo | — | FK a Relevamiento |
| agente_id | identificador | No nulo | — | FK a Usuario (rol AGENTE) |
| vigente | logico | No nulo | verdadero | La revocación deja el par sin asignación vigente (RC-05) |
| asignado_en | fecha-hora | No nulo | — | Auditoría |
| revocado_en | fecha-hora | Nulo | — | Instante de revocación |

### 1.7 MarcadorGeografico — entidad conceptual 1.6 MarcadorGeografico

Punto del mapa con identidad propia y estable; agrupa observaciones (RC-01).

| Columna | Tipo | Nulabilidad | Default | Notas |
| --- | --- | --- | --- | --- |
| id_marcador | identificador | No nulo | — | PK; identidad estable ante movimiento/etiquetado (RC-01) |
| relevamiento_id | identificador | No nulo | — | FK a Relevamiento |
| coordenada | geografico | No nulo | — | Punto (lat/long); alternativa: latitud decimal(9,6) + longitud decimal(9,6) |
| id_origen | cadena(128) | Nulo | — | Identificador de origen del cliente para idempotencia de sincronización (RN-07) |
| creado_en | fecha-hora | No nulo | — | Auditoría |
| actualizado_en | fecha-hora | No nulo | — | Cálculo de novedades de bajada (CU-11) |

### 1.8 ConflictoMarcadores — entidad conceptual 1.7 ConflictoMarcadores

Conflicto de dos o más marcadores en un mismo radio; estado válido que se resuelve al cierre (RN-03, RC-04, ADR-06).

| Columna | Tipo | Nulabilidad | Default | Notas |
| --- | --- | --- | --- | --- |
| id_conflicto | identificador | No nulo | — | PK |
| relevamiento_id | identificador | No nulo | — | FK a Relevamiento |
| estado | enum(PENDIENTE, RESUELTO) | No nulo | PENDIENTE | RESUELTO es precondición de cierre (RN-05, RC-04) |
| resolucion | enum(UNIFICAR, SEPARAR) | Nulo | — | Decisión del jefe al resolver (CU-13); nulo mientras PENDIENTE |
| detectado_en | fecha-hora | No nulo | — | Auditoría |
| resuelto_en | fecha-hora | Nulo | — | Instante de resolución |

### 1.9 ConflictoMarcadorMiembro — soporte de la relación Conflicto—Marcador (2..N)

Asocia cada conflicto con los dos o más marcadores que involucra (cardinalidad 1—2..N del modelo conceptual).

| Columna | Tipo | Nulabilidad | Default | Notas |
| --- | --- | --- | --- | --- |
| conflicto_id | identificador | No nulo | — | FK a ConflictoMarcadores; parte de PK compuesta |
| marcador_id | identificador | No nulo | — | FK a MarcadorGeografico; parte de PK compuesta |

### 1.10 Observacion — entidad conceptual 1.8 Observacion

Registro anclado a un marcador, con autor; ancla obligatoria a marcador existente (RC-02).

| Columna | Tipo | Nulabilidad | Default | Notas |
| --- | --- | --- | --- | --- |
| id_observacion | identificador | No nulo | — | PK |
| marcador_id | identificador | No nulo | — | FK a MarcadorGeografico; obligatoria y existente (RC-02) |
| autor_id | identificador | No nulo | — | FK a Usuario; conservado en la baja del autor (RN-02) |
| nota | cadena | Nulo | — | Texto de la observación |
| id_origen | cadena(128) | Nulo | — | Identificador de origen para idempotencia de sincronización (RN-07) |
| creado_en | fecha-hora | No nulo | — | Auditoría |
| actualizado_en | fecha-hora | No nulo | — | Cálculo de novedades de bajada (CU-11) |

### 1.11 Foto — entidad conceptual 1.9 Foto

Imagen de una observación; el binario vive en el almacén de archivos vía la librería; aquí solo la referencia lógica (ADR-09).

| Columna | Tipo | Nulabilidad | Default | Notas |
| --- | --- | --- | --- | --- |
| id_foto | identificador | No nulo | — | PK |
| observacion_id | identificador | No nulo | — | FK a Observacion |
| referencia_almacen | cadena(512) | Nulo | — | Identificador lógico devuelto por la librería de almacenamiento (ADR-09); nulo si el binario aún no se subió |
| ubicacion | geografico | Nulo | — | Coordenada incrustada o asignada; nula si pendiente de ubicación manual (RN-04) |
| pendiente_ubicacion | logico | No nulo | falso | Verdadero si la foto no tenía ubicación incrustada (RN-04) |
| id_origen | cadena(128) | Nulo | — | Identificador de origen para idempotencia de sincronización (RN-07) |
| creado_en | fecha-hora | No nulo | — | Auditoría |

### 1.12 Comentario — entidad conceptual 1.10 Comentario

Texto que describe una foto; a lo sumo uno por foto (cardinalidad 1—0..1).

| Columna | Tipo | Nulabilidad | Default | Notas |
| --- | --- | --- | --- | --- |
| id_comentario | identificador | No nulo | — | PK |
| foto_id | identificador | No nulo | — | FK a Foto; único (a lo sumo un comentario por foto) |
| texto | cadena | No nulo | — | Contenido del comentario |
| creado_en | fecha-hora | No nulo | — | Auditoría |

### 1.13 Etiqueta — entidad conceptual 1.11 Etiqueta

Marca reutilizable aplicable a fotos y marcadores.

| Columna | Tipo | Nulabilidad | Default | Notas |
| --- | --- | --- | --- | --- |
| id_etiqueta | identificador | No nulo | — | PK |
| nombre | cadena(100) | No nulo | — | Único por relevamiento; reutilizable entre fotos y marcadores |
| relevamiento_id | identificador | No nulo | — | FK a Relevamiento; acota el ámbito de la etiqueta |

### 1.14 EtiquetaFoto — soporte de la relación Etiqueta—Foto (N—N)

| Columna | Tipo | Nulabilidad | Default | Notas |
| --- | --- | --- | --- | --- |
| etiqueta_id | identificador | No nulo | — | FK a Etiqueta; parte de PK compuesta |
| foto_id | identificador | No nulo | — | FK a Foto; parte de PK compuesta |

### 1.15 EtiquetaMarcador — soporte de la relación Etiqueta—Marcador (N—N)

| Columna | Tipo | Nulabilidad | Default | Notas |
| --- | --- | --- | --- | --- |
| etiqueta_id | identificador | No nulo | — | FK a Etiqueta; parte de PK compuesta |
| marcador_id | identificador | No nulo | — | FK a MarcadorGeografico; parte de PK compuesta |

### 1.16 MarcaSincronizacion — entidad conceptual 1.12 MarcaSincronizacion

Punto de sincronización por relevamiento y cliente; opaco y monótono (RC-06).

| Columna | Tipo | Nulabilidad | Default | Notas |
| --- | --- | --- | --- | --- |
| id_marca | identificador | No nulo | — | PK |
| relevamiento_id | identificador | No nulo | — | FK a Relevamiento |
| cliente_id | identificador | No nulo | — | FK a Usuario (cliente de campo) |
| valor | fecha-hora | No nulo | — | Punto de sincronización opaco para el cliente; solo avanza (RC-06) |
| subida_concluida | logico | No nulo | falso | Compuerta del orden subir-antes-de-bajar del ciclo en curso (RN-06) |
| actualizado_en | fecha-hora | No nulo | — | Auditoría |

### 1.17 ClaveIdempotencia — soporte técnico de la idempotencia (ADR-08, CU-21)

Registro de claves de idempotencia y de identificadores de origen ya procesados, con el resultado registrado para devolverlo ante un reenvío (RN-07).

| Columna | Tipo | Nulabilidad | Default | Notas |
| --- | --- | --- | --- | --- |
| id_clave | identificador | No nulo | — | PK |
| clave | cadena(200) | No nulo | — | Clave de idempotencia o identificador de origen; único por ámbito de operación |
| huella_solicitud | cadena(128) | No nulo | — | Huella del contenido; una clave reutilizada con huella distinta se rechaza (CLAVE_REUTILIZADA_INCONSISTENTE) |
| resultado | cadena | Nulo | — | Resultado registrado a devolver ante el reenvío |
| estado | enum(EN_CURSO, COMPLETADA) | No nulo | EN_CURSO | Un reintento durante EN_CURSO no inicia una segunda ejecución (CU-21 FA-01) |
| creado_en | fecha-hora | No nulo | — | Base de la política de retención (06) |

## 2. Índices

| Índice | Tabla | Columnas | Tipo | Motivación |
| --- | --- | --- | --- | --- |
| ux_usuario_identificador | Usuario | identificador_acceso | Único | Identidad de acceso única en todo el sistema (modelo conceptual §2) |
| ix_usuario_administrado_por | Usuario | administrado_por | No único | Recorrer la cadena de administración (RC-03) |
| ix_usuario_rol | Usuario | rol_id | No único | Resolver rol y alcance en autorización (CU-18) |
| ix_relevamiento_creado_por | Relevamiento | creado_por | No único | Listar relevamientos por jefe acotados al alcance (CU-04, RN-01) |
| ix_relevamiento_estado | Relevamiento | estado | No único | Filtrar listados por estado (CU-20) |
| ux_tramo_relevamiento | TramoVial | relevamiento_id | Único | Relación 1—1 relevamiento-tramo |
| ux_asignacion_par_vigente | Asignacion | relevamiento_id, agente_id (parcial: vigente=verdadero) | Único parcial | Unicidad de la asignación vigente por par (RC-05) |
| ix_marcador_relevamiento | MarcadorGeografico | relevamiento_id | No único | Listar y sincronizar marcadores por relevamiento (CU-07, CU-11) |
| ix_marcador_actualizado | MarcadorGeografico | relevamiento_id, actualizado_en | Compuesto | Cálculo incremental de novedades por marca (CU-11, RC-06) |
| ix_marcador_coordenada | MarcadorGeografico | coordenada | Espacial (si el motor lo soporta) | Detección de marcadores dentro de un radio (RN-03, RN-04) |
| ix_conflicto_relevamiento_estado | ConflictoMarcadores | relevamiento_id, estado | Compuesto | Verificar ausencia de conflictos pendientes al cierre (RC-04, CU-14) |
| ix_observacion_marcador | Observacion | marcador_id | No único | Recuperar observaciones por marcador; baja segura de marcador (RC-02) |
| ix_observacion_actualizado | Observacion | actualizado_en | No único | Cálculo de novedades de bajada (CU-11) |
| ix_foto_observacion | Foto | observacion_id | No único | Recuperar fotos por observación (CU-08, CU-12) |
| ux_comentario_foto | Comentario | foto_id | Único | A lo sumo un comentario por foto (cardinalidad 1—0..1) |
| ux_etiqueta_nombre_relevamiento | Etiqueta | relevamiento_id, nombre | Único | Nombre de etiqueta único por relevamiento |
| ux_marca_relevamiento_cliente | MarcaSincronizacion | relevamiento_id, cliente_id | Único | Una marca por par relevamiento-cliente (RC-06) |
| ux_clave_idempotencia | ClaveIdempotencia | clave | Único | Unicidad de la clave para reconocer reintentos (RN-07, ADR-08) |

## 3. Restricciones

| Restricción | Tabla | Tipo | Detalle |
| --- | --- | --- | --- |
| pk_* | todas | PK | Clave primaria por identificador sustituto (o compuesta en tablas de unión) |
| fk_usuario_rol | Usuario | FK | rol_id → Rol(id_rol) |
| fk_usuario_admin | Usuario | FK autorreferente | administrado_por → Usuario(id_usuario); nulo solo para RAIZ (RC-03) |
| ck_usuario_admin_raiz | Usuario | Check | administrado_por nulo si y solo si el rol es RAIZ (RC-03) |
| fk_relevamiento_creador | Relevamiento | FK | creado_por → Usuario(id_usuario) |
| ck_relevamiento_estado | Relevamiento | Check | estado ∈ {RECOLECCION, REVISION, CIERRE} (RC-04) |
| fk_tramo_relevamiento | TramoVial | FK | relevamiento_id → Relevamiento; único (1—1) |
| fk_componente_tramo | TramoComponente | FK | tramo_id → TramoVial; el tramo debe tener al menos un componente (CU-04; validado en aplicación + verificación de no-vacío) |
| ux_asignacion_par_vigente | Asignacion | Único parcial | Un par agente-relevamiento vigente a lo sumo una vez (RC-05) |
| fk_asignacion_relevamiento | Asignacion | FK | relevamiento_id → Relevamiento |
| fk_asignacion_agente | Asignacion | FK | agente_id → Usuario |
| fk_marcador_relevamiento | MarcadorGeografico | FK | relevamiento_id → Relevamiento |
| fk_conflicto_relevamiento | ConflictoMarcadores | FK | relevamiento_id → Relevamiento |
| ck_conflicto_estado | ConflictoMarcadores | Check | estado ∈ {PENDIENTE, RESUELTO}; resolucion no nula solo si RESUELTO (CU-13) |
| pk_conflicto_miembro | ConflictoMarcadorMiembro | PK compuesta | (conflicto_id, marcador_id); al menos dos miembros por conflicto (2..N, validado en aplicación) |
| fk_observacion_marcador | Observacion | FK | marcador_id → MarcadorGeografico; restringe la baja de marcador con observaciones (RC-02) |
| fk_observacion_autor | Observacion | FK | autor_id → Usuario; sin borrado en cascada para conservar la autoría (RN-02) |
| ux_comentario_foto | Comentario | Único | A lo sumo un comentario por foto |
| fk_comentario_foto | Comentario | FK | foto_id → Foto |
| fk_foto_observacion | Foto | FK | observacion_id → Observacion |
| fk_etiqueta_relevamiento | Etiqueta | FK | relevamiento_id → Relevamiento |
| pk_etiqueta_foto / pk_etiqueta_marcador | EtiquetaFoto / EtiquetaMarcador | PK compuesta | Unicidad del vínculo N—N |
| ux_marca_par | MarcaSincronizacion | Único | Una marca por relevamiento-cliente (RC-06) |
| ux_clave_idempotencia | ClaveIdempotencia | Único | Unicidad de la clave (RN-07, ADR-08) |

Las invariantes de no borrado de la autoría (RN-02) se materializan como FK sin cascada de borrado y con la baja de usuario modelada como cambio de estado (estado_habilitacion=INHABILITADO), no como eliminación de filas.

## 4. Migración inicial

La migración inicial se referencia con el identificador `M0001_inicial` y se aplica con la herramienta de migraciones del runtime (ADR-02), en un arranque controlado del despliegue antes de habilitar el tráfico (categoría 09). Resumen del cambio:

- Crea las tablas de catálogo y dominio: Rol (con las cuatro filas de catálogo sembradas), Usuario, Relevamiento, TramoVial, TramoComponente, Asignacion, MarcadorGeografico, ConflictoMarcadores, ConflictoMarcadorMiembro, Observacion, Foto, Comentario, Etiqueta, EtiquetaFoto, EtiquetaMarcador, MarcaSincronizacion.
- Crea la tabla técnica de soporte ClaveIdempotencia.
- Crea los índices del §2 y las restricciones del §3 (PK, FK, únicas, parciales y check).
- Siembra el usuario raíz inicial conforme a RC-03 (sin administrador), con su credencial provista por configuración del entorno (intake §17.P.5), no en la migración.

No se nombra el producto de migraciones; la herramienta concreta y la base se fijan en la categoría 09 a partir del intake §17.P.9.

## 5. Estrategia multi-tenant

No aplica. La solución es single-tenant para una única organización (Vialidad provincial), con multi_tenant=false (intake §17.P.4). La jerarquía de cuatro roles es control de acceso (RN-01, RC-03), no aislamiento por tenant: no hay columna discriminadora de tenant, ni esquema por tenant, ni base por tenant. El alcance de cada operación se acota por la cadena de administración y por la propiedad del relevamiento, no por una frontera de tenant.

## 6. Trazabilidad

| Tabla lógica | Entidad conceptual de origen (02 §1) | CU que la consumen | RN / RC |
| --- | --- | --- | --- |
| Rol | 1.2 Rol | CU-01, CU-02, CU-03, CU-18 | RN-01 |
| Usuario | 1.1 Usuario | CU-01, CU-02, CU-03, CU-18 | RN-01, RN-02; RC-03 |
| Relevamiento | 1.3 Relevamiento | CU-04, CU-05, CU-06, CU-12, CU-14, CU-15, CU-16 | RN-05; RC-04 |
| TramoVial | 1.4 TramoVial | CU-04 | RN-05 |
| TramoComponente | 1.4 TramoVial (atributo ComposicionPuentesCaminos) | CU-04 | RN-05 |
| Asignacion | 1.5 Asignacion | CU-05, CU-10, CU-11 | RN-01; RC-05 |
| MarcadorGeografico | 1.6 MarcadorGeografico | CU-07, CU-08, CU-09, CU-12, CU-13 | RN-03, RN-04; RC-01 |
| ConflictoMarcadores | 1.7 ConflictoMarcadores | CU-07, CU-10, CU-12, CU-13, CU-14 | RN-03, RN-05; RC-04 |
| ConflictoMarcadorMiembro | 1.7 ConflictoMarcadores (relación 2..N) | CU-13 | RN-03 |
| Observacion | 1.8 Observacion | CU-08, CU-09, CU-10, CU-11, CU-13 | RN-03, RN-04; RC-02 |
| Foto | 1.9 Foto | CU-08, CU-09, CU-15, CU-16 | RN-04 |
| Comentario | 1.10 Comentario | CU-08, CU-12, CU-15, CU-16 | — |
| Etiqueta | 1.11 Etiqueta | CU-07, CU-08, CU-12, CU-13 | RN-04 |
| EtiquetaFoto | 1.11 Etiqueta (relación N—N con Foto) | CU-08, CU-12 | — |
| EtiquetaMarcador | 1.11 Etiqueta (relación N—N con Marcador) | CU-07, CU-12 | — |
| MarcaSincronizacion | 1.12 MarcaSincronizacion | CU-10, CU-11, CU-21 | RN-06, RN-07; RC-06 |
| ClaveIdempotencia | soporte técnico (sin origen conceptual; deriva de ADR-08) | CU-10, CU-21 | RN-07 |

Las 12 entidades conceptuales tienen tabla de origen. Las tablas TramoComponente, ConflictoMarcadorMiembro, EtiquetaFoto y EtiquetaMarcador son materializaciones de atributos de composición y de relaciones N—N o 2..N del modelo conceptual, no entidades nuevas. ClaveIdempotencia es la única tabla sin origen conceptual: deriva de la decisión técnica ADR-08 para sostener RN-07.

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Modelo lógico inicial de geovial-api: mapeo de las 12 entidades conceptuales a 16 tablas de dominio (con tablas de composición y de unión) más una tabla técnica de idempotencia; tipos físicos, índices, restricciones (PK/FK/únicas/parciales/check), migración inicial referenciada M0001_inicial y trazabilidad tabla→entidad conceptual→CU→RN/RC. Single-tenant. |
