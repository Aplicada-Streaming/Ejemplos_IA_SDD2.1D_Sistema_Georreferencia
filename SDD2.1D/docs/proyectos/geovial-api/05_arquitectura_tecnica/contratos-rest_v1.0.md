# Contrato REST — geovial-api

**Proyecto:** geovial-api
**Documento:** contratos-rest_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer

## 1. Alcance del contrato

Este documento define el contrato público REST de `geovial-api` que consumen `geovial-web` y `geovial-mobile` (intake §14). Es el contrato de área `rest` que la regla 05 §2.2 obliga a documentar para un proyecto `rest-api`. Materializa los 22 casos de uso de la especificación funcional:

| Recurso / área | CU materializados |
| --- | --- |
| Autenticación y sesión | CU-03 |
| Usuarios y agentes | CU-01, CU-02 |
| Relevamientos y ciclo | CU-04, CU-06, CU-12, CU-14 |
| Asignaciones | CU-05 |
| Marcadores y observaciones (fotos, comentarios, etiquetas, carga manual) | CU-07, CU-08, CU-09 |
| Sincronización (subida y bajada) | CU-10, CU-11 |
| Conflictos | CU-13 |
| Portabilidad (exportación e importación) | CU-15, CU-16 |
| Configuración de almacenamiento | CU-17 |
| Transversales (autorización, errores, paginación, idempotencia, versionado) | CU-18, CU-19, CU-20, CU-21, CU-22 |

El contrato se describe en términos de mecanismo abstracto (recursos, operaciones, esquemas lógicos, errores problem+json, versionado por URI); las firmas físicas, el formato exacto del token y la base concreta pertenecen al stack (intake §17) y a la categoría 09. Los ejemplos ejecutables y la colección de pruebas viven en 11.

## 2. Formato

El contrato se expresa como una especificación OpenAPI descriptiva, inline en este documento como estructura de recursos, operaciones, esquemas y errores. La materialización como archivo OpenAPI `.yaml` versionado y su publicación pertenecen a 06/09; aquí se fija el contrato lógico. Convenciones generales:

- Transporte: REST sobre HTTP, payloads en JSON (intake §17.P.3).
- Autenticación: token bearer en la cabecera de autorización en toda operación salvo el inicio de sesión (ADR-03).
- Versionado: prefijo de versión mayor en la ruta, por ejemplo `/v1/...` (ADR-10, CU-22).
- Idempotencia: las operaciones no seguras reintentables aceptan una clave de idempotencia en una cabecera dedicada (ADR-08, CU-21).
- Paginación: los listados aceptan tamaño y posición de página y devuelven referencias de navegación (ADR-04, CU-20).
- Errores: problem+json RFC 7807 con código estable (ADR-05, CU-19).

## 3. Operaciones

Notación: las operaciones se agrupan por recurso bajo el prefijo de versión `/v1`. Seguridad indica el rol mínimo que la autorización exige (CU-18, RN-01); idempotente indica si la operación admite clave de idempotencia (CU-21).

### 3.1 Autenticación y sesión (CU-03)

| Operación | Método y ruta | Descripción | Seguridad | Idempotente |
| --- | --- | --- | --- | --- |
| Iniciar sesión | POST /v1/sesiones | Emite un token bearer a partir de credenciales | Anónimo (credenciales) | No |
| Cerrar sesión | DELETE /v1/sesiones/actual | Cierra la sesión completa para cambio de usuario | Cualquier rol | No |
| Revalidar | POST /v1/sesiones/revalidacion | Revalida la sesión activa (la seguridad del dispositivo la gobierna en el cliente móvil) | Cualquier rol | No |

### 3.2 Usuarios y agentes (CU-01, CU-02)

| Operación | Método y ruta | Descripción | Seguridad | Idempotente |
| --- | --- | --- | --- | --- |
| Listar usuarios | GET /v1/usuarios | Lista los usuarios del alcance del solicitante (paginado) | Administrador del nivel | No |
| Crear usuario | POST /v1/usuarios | Da de alta un usuario del nivel inmediato inferior (RN-01, RC-03) | Administrador del nivel | Sí |
| Consultar usuario | GET /v1/usuarios/{id} | Recupera un usuario del alcance | Administrador del nivel | No |
| Dar de baja usuario | DELETE /v1/usuarios/{id} | Inhabilita el acceso conservando la autoría (RN-02) | Administrador del nivel | Sí |
| Crear agente | POST /v1/agentes | Alta directa de un agente por el jefe de área (CU-02, F-02) | Jefe de área | Sí |
| Dar de baja agente | DELETE /v1/agentes/{id} | Baja de un agente por el jefe de área | Jefe de área | Sí |

### 3.3 Relevamientos y ciclo (CU-04, CU-06, CU-12, CU-14)

| Operación | Método y ruta | Descripción | Seguridad | Idempotente |
| --- | --- | --- | --- | --- |
| Listar relevamientos | GET /v1/relevamientos | Lista relevamientos del alcance (paginado, filtros por estado y etiqueta) | Jefe de área | No |
| Crear relevamiento | POST /v1/relevamientos | Crea un relevamiento con su tramo (composición no vacía, CU-04) | Jefe de área | Sí |
| Consultar relevamiento | GET /v1/relevamientos/{id} | Recupera un relevamiento para revisión sobre mapa (CU-12), con marcadores y conflictos | Jefe de área | No |
| Dar de baja relevamiento | DELETE /v1/relevamientos/{id} | Da de baja un relevamiento del alcance | Jefe de área | Sí |
| Transicionar estado | POST /v1/relevamientos/{id}/transiciones | Avanza recolección→revisión, retorna revisión→recolección o reabre cierre→revisión (RN-05) | Jefe de área | Sí |
| Cerrar relevamiento | POST /v1/relevamientos/{id}/cierre | Cierra el relevamiento; exige conflictos resueltos (RN-05, RC-04) | Jefe de área | Sí |

### 3.4 Asignaciones (CU-05)

| Operación | Método y ruta | Descripción | Seguridad | Idempotente |
| --- | --- | --- | --- | --- |
| Listar asignaciones | GET /v1/relevamientos/{id}/asignaciones | Lista los agentes asignados al relevamiento | Jefe de área | No |
| Asignar agente | POST /v1/relevamientos/{id}/asignaciones | Asigna un agente; único por par vigente (RC-05) | Jefe de área | Sí |
| Revocar asignación | DELETE /v1/relevamientos/{id}/asignaciones/{agenteId} | Revoca la asignación vigente | Jefe de área | Sí |

### 3.5 Marcadores, observaciones y carga manual (CU-07, CU-08, CU-09)

| Operación | Método y ruta | Descripción | Seguridad | Idempotente |
| --- | --- | --- | --- | --- |
| Listar marcadores | GET /v1/relevamientos/{id}/marcadores | Lista marcadores del relevamiento (paginado) | Agente o jefe del alcance | No |
| Crear marcador | POST /v1/relevamientos/{id}/marcadores | Crea un marcador; convive con conflictos (RN-03) | Agente o jefe | Sí |
| Mover/etiquetar marcador | PATCH /v1/relevamientos/{id}/marcadores/{marcadorId} | Cambia coordenada o etiquetas; identidad estable (RC-01) | Agente o jefe | Sí |
| Dar de baja marcador | DELETE /v1/relevamientos/{id}/marcadores/{marcadorId} | Baja segura solo sin observaciones ancladas (RC-02) | Agente o jefe | Sí |
| Crear observación | POST /v1/marcadores/{marcadorId}/observaciones | Ancla una observación a un marcador existente (RC-02) | Agente | Sí |
| Agregar foto | POST /v1/observaciones/{obsId}/fotos | Adjunta una foto; delega el binario a la librería (ADR-09) | Agente | Sí |
| Comentar/etiquetar foto | PATCH /v1/fotos/{fotoId} | Agrega comentario (a lo sumo uno) y etiquetas a la foto | Agente | Sí |
| Carga manual de fotos | POST /v1/relevamientos/{id}/carga-manual | Carga fotos priorizando ubicación incrustada y agrupando por radio (RN-04) | Agente | Sí |

### 3.6 Sincronización (CU-10, CU-11)

| Operación | Método y ruta | Descripción | Seguridad | Idempotente |
| --- | --- | --- | --- | --- |
| Subir cambios locales | POST /v1/relevamientos/{id}/sincronizacion/subida | Recibe el lote de cambios locales; deduplica por identificador de origen (RN-06, RN-07) | Agente asignado | Sí (id de origen por cambio) |
| Bajar actualizaciones | POST /v1/relevamientos/{id}/sincronizacion/bajada | Entrega novedades posteriores a la marca; solo tras concluir la subida (RN-06) | Agente asignado | Segura/repetible |

### 3.7 Conflictos (CU-13)

| Operación | Método y ruta | Descripción | Seguridad | Idempotente |
| --- | --- | --- | --- | --- |
| Listar conflictos | GET /v1/relevamientos/{id}/conflictos | Lista los conflictos del relevamiento y su estado | Jefe de área | No |
| Resolver conflicto | POST /v1/relevamientos/{id}/conflictos/{conflictoId}/resolucion | Unifica o separa marcadores antes del cierre (CU-13) | Jefe de área | Sí |

### 3.8 Portabilidad (CU-15, CU-16)

| Operación | Método y ruta | Descripción | Seguridad | Idempotente |
| --- | --- | --- | --- | --- |
| Exportar relevamiento | POST /v1/relevamientos/{id}/exportacion | Produce una unidad transferible única con comentarios, etiquetas y fotos | Jefe de área | No |
| Importar relevamiento | POST /v1/relevamientos/importacion | Reconstruye un relevamiento desde una unidad transferible (RN-07) | Jefe de área o raíz | Sí |

### 3.9 Configuración de almacenamiento (CU-17)

| Operación | Método y ruta | Descripción | Seguridad | Idempotente |
| --- | --- | --- | --- | --- |
| Consultar destino activo | GET /v1/configuracion/almacenamiento | Consulta el proveedor activo (sin revelar credenciales) | Usuario raíz | No |
| Configurar destino | PUT /v1/configuracion/almacenamiento | Activa un proveedor de almacenamiento (CU-17); delega en la librería (ADR-09) | Usuario raíz | Sí |
| Validar destino | POST /v1/configuracion/almacenamiento/validacion | Valida un proveedor sin activarlo (CU-17 FA-02) | Usuario raíz | Segura/repetible |

## 4. Esquemas de datos

Esquemas lógicos (DTO) en forma abstracta; los tipos físicos viven en `modelo-datos-logico_v1.0.md` y en el stack.

- Credenciales: identificador de acceso y secreto; entran en el inicio de sesión, no salen en ninguna respuesta.
- Token: token bearer opaco para el cliente, con vigencia; el cliente lo presenta en cada solicitud.
- Usuario: identificador, identificador de acceso, rol, administrador, estado de habilitación; nunca expone el secreto de credencial.
- Relevamiento: identificador, estado, creador, nombre, tramo (composición no vacía de puentes y caminos), marcas de tiempo.
- Asignacion: identificador, agente, relevamiento, vigencia.
- Marcador: identificador estable, coordenada, etiquetas, indicador de conflicto.
- Observacion: identificador, marcador anclado, autor, nota, fotos.
- Foto: identificador, referencia lógica al almacén (no el binario), ubicación o indicador de pendiente de ubicación, comentario, etiquetas.
- LoteSincronizacion: colección de cambios, cada uno con su identificador de origen; resultado de subida con aplicados, reenvíos reconocidos y conflictos registrados.
- ActualizacionesSincronizacion: conjunto de novedades posteriores a la marca y una marca nueva opaca.
- Conflicto: identificador, estado (pendiente/resuelto), marcadores involucrados, resolución (unificar/separar).
- UnidadTransferible: representación lógica del relevamiento completo (comentarios, etiquetas, fotos) para exportar/importar; su formato físico de empaquetado pertenece al stack.
- ConfiguracionAlmacenamiento: proveedor seleccionado y parámetros; las credenciales entran pero no salen (RN-03 de storage).
- Pagina: elementos, tamaño efectivo, referencias a página siguiente y anterior.
- Problema (problem+json RFC 7807): código estable, mensaje, estado, campo o recurso implicado.

Cabeceras de contrato:

- Autorización: token bearer (todas salvo inicio de sesión).
- Clave de idempotencia: cabecera dedicada en operaciones no seguras reintentables (CU-21).
- Marca de sincronización: la bajada porta la marca del cliente (cuerpo o parámetro), opaca (RC-06).

## 5. Manejo de errores

Todo error se devuelve como problem+json RFC 7807 (ADR-05, CU-19) con un código estable en mayúsculas sin tildes, independiente del idioma. El catálogo completo con mensaje, causa y acción se alinea con `dx-error-messages_v1.0.md` (03); aquí se consolida la taxonomía y los códigos reservados por área.

| Categoría | Estado HTTP típico | Códigos | Origen |
| --- | --- | --- | --- |
| Solicitud inválida | 400 | FORMATO_SOLICITUD_INVALIDO, TRAMO_INCOMPLETO, RADIO_NO_DEFINIDO, FILTRO_NO_SOPORTADO, ORDEN_NO_SOPORTADO, POSICION_INVALIDA, MARCA_INVALIDA, VERSION_NO_SOPORTADA, VERSION_REQUERIDA_AUSENTE, CLAVE_REQUERIDA_AUSENTE, CREDENCIALES_PROVEEDOR_INVALIDAS, LOTE_MALFORMADO | CU-04, CU-09, CU-19, CU-20, CU-11, CU-22, CU-21, CU-17, CU-10 |
| No autorizado | 401 | CREDENCIALES_INVALIDAS, USUARIO_INHABILITADO | CU-03, RN-02 |
| Prohibido | 403 | JERARQUIA_NO_PERMITIDA, ROL_NO_AUTORIZADO, FUERA_DE_ALCANCE, RELEVAMIENTO_NO_ASIGNADO | RN-01, CU-17, CU-18, CU-10 |
| No encontrado | 404 | RECURSO_NO_ENCONTRADO, RECURSO_NO_EN_VERSION | CU-19, CU-22 |
| Conflicto | 409 | CONFLICTOS_PENDIENTES, TRANSICION_NO_PERMITIDA, RELEVAMIENTO_NO_EN_REVISION, RELEVAMIENTO_CERRADO, SUBIDA_NO_CONCLUIDA, CLAVE_REUTILIZADA_INCONSISTENTE, PROVEEDOR_NO_DISPONIBLE | RN-05, RN-06, RN-07, CU-10, CU-14, CU-17 |
| Error interno | 500 | ERROR_INTERNO | CU-19 (sin filtrar detalles) |

Reglas de error: los códigos son estables y opacos al idioma; un error de validación con varios campos se devuelve en un único problema que los enumera (CU-19 FA-01); un fallo no contemplado devuelve ERROR_INTERNO sin exponer detalles (CU-19 FA-02); los códigos de la librería de almacenamiento se normalizan al cruzar al contrato del backend (ADR-05, ADR-09).

## 6. Versionado del contrato

Gobernado por ADR-10 (versionado por URI) y CU-22, alineado con SemVer del proyecto (intake §17.P.7):

- Versión en la ruta: cada recurso bajo un prefijo de versión mayor (`/v1`).
- Cambio compatible (dentro de la misma versión mayor): agregar un campo opcional, un recurso, un valor de enum adicional o una traducción de mensaje de error. No rompe a los clientes.
- Cambio incompatible (versión mayor nueva): quitar un campo, volver obligatorio uno opcional, cambiar la semántica de una operación, o quitar/renombrar un código de error. Se publica una versión mayor nueva conservando la anterior durante un período de convivencia de al menos un MINOR (intake §17.P.3).
- Deprecación: un recurso o código se marca obsoleto en una versión menor antes de removerse en la mayor siguiente; el backend comunica el plan de retiro.
- Errores de versión: VERSION_NO_SOPORTADA (retirada o inexistente), VERSION_REQUERIDA_AUSENTE (si la política exige versión explícita), RECURSO_NO_EN_VERSION (recurso ausente en la versión indicada).

Esta política protege la dependencia de `geovial-web` y `geovial-mobile` sobre el contrato (intake §14).

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU cubiertos | CU-01 a CU-22 |
| RN aplicables | RN-01 (alcance), RN-02 (autoría), RN-03 (conflictos), RN-04 (carga manual), RN-05 (transición), RN-06 (orden de sync), RN-07 (idempotencia) |
| RC aplicables | RC-01 a RC-06 |
| NB upstream | NB-01 a NB-07 |
| ADRs que lo gobiernan | ADR-01 (estilo), ADR-03 (autenticación/autorización), ADR-04 (paginación), ADR-05 (errores), ADR-06 (conflictos), ADR-07 (orden de sync), ADR-08 (idempotencia), ADR-09 (almacenamiento), ADR-10 (versionado) |
| Contrato consumido | `geovial-storage` — `contratos-abstractions_v1.0.md` (vía ADR-09; arista del manifiesto §13) |
| Contratos inter-proyecto | `geovial-api → geovial-web` y `geovial-api → geovial-mobile` (productor expone a consumidores; se indexan en la vista de solución de `_solucion/`, Fase H) |
| Tests previstos (en 08) | Contract test del 100 % de endpoints públicos por versión (intake §17.P.6); error uniforme problem+json; paginación y filtros; idempotencia por clave e identificador de origen; orden subir-antes-de-bajar; versionado compatible/incompatible |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Contrato REST inicial de geovial-api: recursos y operaciones por área para los 22 CU, esquemas lógicos (DTO), taxonomía de errores problem+json RFC 7807 con códigos estables, versionado por URI y trazabilidad CU/RN/RC/NB/ADR, incluidos los endpoints de sincronización subida y bajada. |
