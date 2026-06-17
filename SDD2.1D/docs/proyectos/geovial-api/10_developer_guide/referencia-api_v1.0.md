# Referencia de la API — geovial-api

**Proyecto:** geovial-api
**Documento:** referencia-api_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Technical Writer + API Documentation Lead
**Tipo Diátaxis:** Reference
**Audiencia:** Developer consumidor de la API HTTP (equipos de geovial-web y geovial-mobile, integradores internos)
**Nivel:** Avanzado
**Tiempo estimado de lectura:** 28 min

Referencia curada del contrato público REST de geovial-api. Mantiene paridad estricta con la especificación OpenAPI lógica de `contratos-rest_v1.0.md` (05): mismas 35 operaciones, mismos esquemas lógicos, misma taxonomía de errores y misma política de versionado. Esta es la fuente de consulta puntual; el modelo mental vive en `conceptos-fundamentales_v1.0.md` y el recorrido guiado en `guia-onboarding-developer_v1.0.md`. Los términos en `codigo-kebab` se definen en `glosario-tecnico_v1.0.md`.

Los ejemplos usan un cliente HTTP de línea de comandos genérico (`http`) con la dirección base en `$BASE`; cualquier cliente equivale. El código ejecutable real vive en la categoría 11.

## 1. Convenciones generales

| Aspecto | Regla | Fuente |
| --- | --- | --- |
| Transporte | REST sobre HTTP, payloads JSON | intake §17.P.3 |
| Autenticación | Token bearer en el encabezado de autorización en toda operación salvo el inicio de sesión | ADR-03 |
| Versionado | Prefijo de versión mayor en la ruta (`/v1/...`) | ADR-10, CU-22 |
| Idempotencia | Las operaciones no seguras reintentables aceptan una clave de idempotencia en una cabecera dedicada | ADR-08, CU-21 |
| Paginación | Los listados aceptan tamaño y posición de página y devuelven referencias de navegación | ADR-04, CU-20 |
| Errores | problem+json (RFC 9457, que actualiza y reemplaza a RFC 7807; el tipo de contenido `application/problem+json` es idéntico), con código estable | ADR-05, CU-19 |

Nota de versión de RFC: `contratos-rest_v1.0.md` (05) cita RFC 7807 como predecesor. RFC 9457 obsoleta RFC 7807 sin cambiar el formato del cuerpo ni el tipo de contenido; toda la estructura descrita aquí es válida bajo ambas. Esta referencia adopta la nomenclatura RFC 9457.

## 2. Recursos y endpoints

Notación: rutas relativas al prefijo de versión `/v1`. La columna Seguridad indica el rol mínimo que la autorización exige (CU-18, RN-01); Idempotente indica si la operación admite clave de idempotencia (CU-21). Las 35 operaciones reproducen `contratos-rest_v1.0.md` §3.

### 2.1 Autenticación y sesión (CU-03)

| Operación | Método y ruta | Descripción | Seguridad | Idempotente |
| --- | --- | --- | --- | --- |
| Iniciar sesión | `POST /sesiones` | Emite un token bearer a partir de credenciales | Anónimo (credenciales) | No |
| Cerrar sesión | `DELETE /sesiones/actual` | Cierra la sesión completa para cambio de usuario | Cualquier rol | No |
| Revalidar | `POST /sesiones/revalidacion` | Revalida la sesión activa (la seguridad del dispositivo la gobierna en el cliente móvil) | Cualquier rol | No |

```text
http POST $BASE/sesiones identificadorAcceso="jefe.norte" secreto="<credencial>"
# -> 200 { "token": "<bearer>", "expiraEn": 3600, "rol": "jefe-de-area" }
```

### 2.2 Usuarios y agentes (CU-01, CU-02)

| Operación | Método y ruta | Descripción | Seguridad | Idempotente |
| --- | --- | --- | --- | --- |
| Listar usuarios | `GET /usuarios` | Lista los usuarios del alcance del solicitante (paginado) | Administrador del nivel | No |
| Crear usuario | `POST /usuarios` | Da de alta un usuario del nivel inmediato inferior (RN-01, RC-03) | Administrador del nivel | Sí |
| Consultar usuario | `GET /usuarios/{id}` | Recupera un usuario del alcance | Administrador del nivel | No |
| Dar de baja usuario | `DELETE /usuarios/{id}` | Inhabilita el acceso conservando la autoría (RN-02) | Administrador del nivel | Sí |
| Crear agente | `POST /agentes` | Alta directa de un agente por el jefe de área (CU-02, F-02) | Jefe de área | Sí |
| Dar de baja agente | `DELETE /agentes/{id}` | Baja de un agente por el jefe de área | Jefe de área | Sí |

### 2.3 Relevamientos y ciclo (CU-04, CU-06, CU-12, CU-14)

| Operación | Método y ruta | Descripción | Seguridad | Idempotente |
| --- | --- | --- | --- | --- |
| Listar relevamientos | `GET /relevamientos` | Lista relevamientos del alcance (paginado, filtros por estado y etiqueta) | Jefe de área | No |
| Crear relevamiento | `POST /relevamientos` | Crea un relevamiento con su tramo (composición no vacía, CU-04) | Jefe de área | Sí |
| Consultar relevamiento | `GET /relevamientos/{id}` | Recupera un relevamiento para revisión sobre mapa (CU-12), con marcadores y conflictos | Jefe de área | No |
| Dar de baja relevamiento | `DELETE /relevamientos/{id}` | Da de baja un relevamiento del alcance | Jefe de área | Sí |
| Transicionar estado | `POST /relevamientos/{id}/transiciones` | Avanza recolección→revisión, retorna revisión→recolección o reabre cierre→revisión (RN-05) | Jefe de área | Sí |
| Cerrar relevamiento | `POST /relevamientos/{id}/cierre` | Cierra el relevamiento; exige conflictos resueltos (RN-05, RC-04) | Jefe de área | Sí |

### 2.4 Asignaciones (CU-05)

| Operación | Método y ruta | Descripción | Seguridad | Idempotente |
| --- | --- | --- | --- | --- |
| Listar asignaciones | `GET /relevamientos/{id}/asignaciones` | Lista los agentes asignados al relevamiento | Jefe de área | No |
| Asignar agente | `POST /relevamientos/{id}/asignaciones` | Asigna un agente; único por par vigente (RC-05) | Jefe de área | Sí |
| Revocar asignación | `DELETE /relevamientos/{id}/asignaciones/{agenteId}` | Revoca la asignación vigente | Jefe de área | Sí |

### 2.5 Marcadores, observaciones y carga manual (CU-07, CU-08, CU-09)

| Operación | Método y ruta | Descripción | Seguridad | Idempotente |
| --- | --- | --- | --- | --- |
| Listar marcadores | `GET /relevamientos/{id}/marcadores` | Lista marcadores del relevamiento (paginado) | Agente o jefe del alcance | No |
| Crear marcador | `POST /relevamientos/{id}/marcadores` | Crea un marcador; convive con conflictos (RN-03) | Agente o jefe | Sí |
| Mover/etiquetar marcador | `PATCH /relevamientos/{id}/marcadores/{marcadorId}` | Cambia coordenada o etiquetas; identidad estable (RC-01) | Agente o jefe | Sí |
| Dar de baja marcador | `DELETE /relevamientos/{id}/marcadores/{marcadorId}` | Baja segura solo sin observaciones ancladas (RC-02) | Agente o jefe | Sí |
| Crear observación | `POST /marcadores/{marcadorId}/observaciones` | Ancla una observación a un marcador existente (RC-02) | Agente | Sí |
| Agregar foto | `POST /observaciones/{obsId}/fotos` | Adjunta una foto; delega el binario a la librería (ADR-09) | Agente | Sí |
| Comentar/etiquetar foto | `PATCH /fotos/{fotoId}` | Agrega comentario (a lo sumo uno) y etiquetas a la foto | Agente | Sí |
| Carga manual de fotos | `POST /relevamientos/{id}/carga-manual` | Carga fotos priorizando ubicación incrustada y agrupando por radio (RN-04) | Agente | Sí |

### 2.6 Sincronización (CU-10, CU-11)

| Operación | Método y ruta | Descripción | Seguridad | Idempotente |
| --- | --- | --- | --- | --- |
| Subir cambios locales | `POST /relevamientos/{id}/sincronizacion/subida` | Recibe el lote de cambios locales; deduplica por identificador de origen (RN-06, RN-07) | Agente asignado | Sí (id de origen por cambio) |
| Bajar actualizaciones | `POST /relevamientos/{id}/sincronizacion/bajada` | Entrega novedades posteriores a la marca; solo tras concluir la subida (RN-06) | Agente asignado | Segura/repetible |

### 2.7 Conflictos (CU-13)

| Operación | Método y ruta | Descripción | Seguridad | Idempotente |
| --- | --- | --- | --- | --- |
| Listar conflictos | `GET /relevamientos/{id}/conflictos` | Lista los conflictos del relevamiento y su estado | Jefe de área | No |
| Resolver conflicto | `POST /relevamientos/{id}/conflictos/{conflictoId}/resolucion` | Unifica o separa marcadores antes del cierre (CU-13) | Jefe de área | Sí |

### 2.8 Portabilidad (CU-15, CU-16)

| Operación | Método y ruta | Descripción | Seguridad | Idempotente |
| --- | --- | --- | --- | --- |
| Exportar relevamiento | `POST /relevamientos/{id}/exportacion` | Produce una unidad transferible única con comentarios, etiquetas y fotos | Jefe de área | No |
| Importar relevamiento | `POST /relevamientos/importacion` | Reconstruye un relevamiento desde una unidad transferible (RN-07) | Jefe de área o raíz | Sí |

### 2.9 Configuración de almacenamiento (CU-17)

| Operación | Método y ruta | Descripción | Seguridad | Idempotente |
| --- | --- | --- | --- | --- |
| Consultar destino activo | `GET /configuracion/almacenamiento` | Consulta el proveedor activo (sin revelar credenciales) | Usuario raíz | No |
| Configurar destino | `PUT /configuracion/almacenamiento` | Activa un proveedor de almacenamiento (CU-17); delega en la librería (ADR-09) | Usuario raíz | Sí |
| Validar destino | `POST /configuracion/almacenamiento/validacion` | Valida un proveedor sin activarlo (CU-17 FA-02) | Usuario raíz | Segura/repetible |

## 3. Esquemas de datos

Esquemas lógicos (DTO) en forma abstracta; los tipos físicos viven en `modelo-datos-logico_v1.0.md` (05) y en el stack. Reproducen `contratos-rest_v1.0.md` §4.

| Esquema | Campos lógicos | Invariantes |
| --- | --- | --- |
| Credenciales | identificador de acceso, secreto | Entran en el inicio de sesión; no salen en ninguna respuesta |
| Token | token bearer opaco, vigencia, rol | El cliente lo presenta en cada solicitud; no se inspecciona su contenido |
| Usuario | identificador, identificador de acceso, rol, administrador, estado de habilitación | Nunca expone el secreto de credencial; identificador de acceso único (RC-03) |
| Relevamiento | identificador, estado, creador, nombre, tramo, marcas de tiempo | Estado en el ciclo recolección/revisión/cierre (RC-04); tramo no vacío (CU-04) |
| TramoVial | composición de puentes y caminos | No vacío (CU-04) |
| Asignacion | identificador, agente, relevamiento, vigencia | Único por par vigente (RC-05) |
| Marcador | identificador estable, coordenada, etiquetas, indicador de conflicto | Identidad estable ante movimiento y etiquetado (RC-01) |
| Observacion | identificador, marcador anclado, autor, nota, fotos | Marcador anclado obligatorio y existente (RC-02); autor conservado en la baja (RN-02) |
| Foto | identificador, referencia lógica al almacén, ubicación o indicador de pendiente, comentario, etiquetas | El binario no viaja incrustado; a lo sumo un comentario |
| LoteSincronizacion | colección de cambios (cada uno con identificador de origen); resultado con aplicados, reenvíos reconocidos y conflictos registrados | Deduplica por identificador de origen (RN-07) |
| ActualizacionesSincronizacion | novedades posteriores a la marca, marca nueva opaca | La marca solo avanza (RC-06) |
| Conflicto | identificador, estado (pendiente/resuelto), marcadores involucrados, resolución (unificar/separar) | Resuelto es precondición de cierre (RN-05) |
| UnidadTransferible | representación lógica del relevamiento completo (comentarios, etiquetas, fotos) | El formato físico de empaquetado pertenece al stack |
| ConfiguracionAlmacenamiento | proveedor seleccionado, parámetros | Las credenciales entran pero no salen |
| Pagina | elementos, tamaño efectivo, referencias a página siguiente y anterior | El tamaño se acota al máximo, no se rechaza (CU-20) |
| Problema (problem+json) | código estable, mensaje, estado, campo o recurso implicado | Código en mayúsculas, opaco al idioma (CU-19) |

### 3.1 Cabeceras del contrato

| Cabecera | Cuándo | Semántica |
| --- | --- | --- |
| Autorización (token bearer) | Todas salvo el inicio de sesión | Porta la identidad y el rol del solicitante (ADR-03) |
| Clave de idempotencia | Operaciones no seguras reintentables | Reintentar con la misma clave no duplica el efecto (CU-21) |
| Marca de sincronización | La bajada de sincronización (en cuerpo o parámetro) | Valor opaco y monótono por relevamiento y cliente (RC-06) |

## 4. Códigos de estado

Mapeo de la naturaleza del resultado al estado de la respuesta. Las operaciones de listado y consulta devuelven `200`; las altas, `201`; las operaciones de efecto sin cuerpo nuevo, `200` o `204` según el recurso.

| Estado | Significado en este contrato |
| --- | --- |
| 200 | Éxito de una lectura o de una operación de efecto que devuelve representación |
| 201 | Recurso creado (altas con cuerpo del recurso nuevo) |
| 204 | Efecto aplicado sin cuerpo (por ejemplo, una baja o una revocación) |
| 400 | Solicitud inválida (validación) — ver §7 |
| 401 | No autenticado / no autorizado por credenciales — ver §7 |
| 403 | Prohibido por rol o alcance — ver §7 |
| 404 | Recurso no encontrado — ver §7 |
| 409 | Conflicto de estado, relevamiento cerrado o idempotencia — ver §7 |
| 500 | Error interno no previsto, sin filtrar detalles — ver §7 |

## 5. Autenticación

- El acceso no se instala: se obtiene un token. `POST /sesiones` recibe el identificador de acceso y el secreto y devuelve un token bearer con su vigencia y el rol del portador (ADR-03). No hay proveedor de identidad externo: el propio backend emite y valida el token.
- El cliente presenta el token en el encabezado de autorización (`Authorization: Bearer <token>`) en toda operación salvo el inicio de sesión.
- Cerrar sesión (`DELETE /sesiones/actual`) cierra la sesión completa, para cambiar de usuario en un dispositivo compartido; el token cerrado deja de ser válido (`TOKEN_REVOCADO`).
- Revalidar (`POST /sesiones/revalidacion`) revalida la sesión activa. En el cliente móvil, la revalidación por seguridad del dispositivo (patrón o huella) ocurre en el cliente, sin volver a pedir credenciales; el backend solo reconoce el token vigente.
- Una baja inhabilita el acceso (`USUARIO_INHABILITADO`) sin borrar la autoría histórica (RN-02). Un token emitido antes de la baja deja de servir al vencer o al detectarse el usuario inhabilitado en la validación.

## 6. Paginación, idempotencia y versionado

### 6.1 Paginación (CU-20, ADR-04)

- Cada listado acepta tamaño y posición de página, aplica los filtros declarados por el recurso en conjunción y ordena por el campo solicitado o por un orden por defecto estable.
- La respuesta (`Pagina`) trae los elementos, el tamaño efectivo y las referencias a la página siguiente y anterior. El cliente navega siguiendo esas referencias.
- El tamaño tiene un máximo: un pedido por encima se acota y se informa en `tamanoEfectivo`, sin rechazar.
- El alcance jerárquico (RN-01) se aplica antes de filtrar y paginar: nunca se paginan recursos fuera del ámbito.
- Filtro, orden o posición no admitidos se rechazan con `FILTRO_NO_SOPORTADO`, `ORDEN_NO_SOPORTADO` o `POSICION_INVALIDA`, que informan los valores válidos.

### 6.2 Idempotencia (CU-21, ADR-08)

- Las operaciones no seguras reintentables (altas, asignaciones, transiciones, resoluciones, importaciones y la subida de sincronización) aceptan una clave de idempotencia estable en la cabecera dedicada.
- El backend, antes de ejecutar el efecto, verifica si la clave o el identificador de origen ya se procesó: si es nuevo, ejecuta y registra el resultado; si ya se procesó, devuelve el resultado registrado sin reejecutar.
- Reusar la clave con un contenido distinto se rechaza con `CLAVE_REUTILIZADA_INCONSISTENTE`. Omitir la clave donde se exige devuelve `CLAVE_REQUERIDA_AUSENTE`. Enviar clave a una operación que no la admite devuelve `OPERACION_NO_IDEMPOTENTE`.
- En la subida de sincronización, la idempotencia es por identificador de origen de cada cambio del lote (RN-07).

### 6.3 Versionado (CU-22, ADR-10)

- El contrato se versiona por la ruta: cada recurso cuelga de un prefijo de versión mayor (`/v1`).
- Cambio compatible (dentro de la misma versión mayor): agregar un campo opcional, un recurso, un valor de enum adicional o una traducción de mensaje de error. No rompe a los clientes.
- Cambio incompatible (versión mayor nueva): quitar un campo, volver obligatorio uno opcional, cambiar la semántica de una operación, o quitar/renombrar un código de error. Publica una versión mayor nueva conservando la anterior durante un período de convivencia de al menos un MINOR.
- Errores de versión: `VERSION_NO_SOPORTADA` (retirada o inexistente), `VERSION_REQUERIDA_AUSENTE` (si la política exige versión explícita), `RECURSO_NO_EN_VERSION` (recurso ausente en la versión indicada).

## 7. Errores problem+json (RFC 9457)

Todo error se devuelve como problem+json con un código estable en mayúsculas sin tildes, independiente del idioma (ADR-05, CU-19). El cliente decide por el código, nunca por el texto. Un error de validación con varios campos se devuelve en un único problema que los enumera; un fallo no contemplado devuelve `ERROR_INTERNO` sin exponer detalles.

Estructura del cuerpo:

```text
{ "codigo": "CONFLICTOS_PENDIENTES", "mensaje": "<texto legible>", "estado": 409, "recurso": "rel-001" }
```

El catálogo accionable por código (qué pasó / por qué / qué hacer) es `dx-error-messages_v1.0.md` (03). Esta tabla consolida la taxonomía por estado, en paridad con `contratos-rest_v1.0.md` §5.

| Estado | Naturaleza | Códigos |
| --- | --- | --- |
| 400 | Solicitud inválida (validación) | `FORMATO_SOLICITUD_INVALIDO`, `TRAMO_INCOMPLETO`, `COORDENADA_INVALIDA`, `AGENTE_INHABILITADO`, `RADIO_NO_DEFINIDO`, `FORMATO_FOTO_NO_SOPORTADO`, `MARCADOR_INEXISTENTE`, `UNIDAD_INVALIDA`, `UNIDAD_INCOMPLETA`, `LOTE_MALFORMADO`, `MARCA_INVALIDA`, `FILTRO_NO_SOPORTADO`, `ORDEN_NO_SOPORTADO`, `POSICION_INVALIDA`, `PROVEEDOR_NO_DISPONIBLE`, `CREDENCIALES_PROVEEDOR_INVALIDAS`, `VERSION_NO_SOPORTADA`, `VERSION_REQUERIDA_AUSENTE`, `CLAVE_REQUERIDA_AUSENTE` |
| 401 | No autenticado / credenciales | `CREDENCIALES_INVALIDAS`, `USUARIO_INHABILITADO`, `NO_AUTENTICADO`, `TOKEN_REVOCADO` |
| 403 | Prohibido por rol o alcance | `JERARQUIA_NO_PERMITIDA`, `ROL_NO_AUTORIZADO`, `ACCION_NO_PERMITIDA`, `FUERA_DE_ALCANCE`, `USUARIO_FUERA_DE_AMBITO`, `AGENTE_FUERA_DE_AREA`, `RELEVAMIENTO_FUERA_DE_AMBITO`, `RELEVAMIENTO_NO_ASIGNADO` |
| 404 | No encontrado | `RECURSO_NO_ENCONTRADO`, `RELEVAMIENTO_INEXISTENTE`, `RECURSO_NO_EN_VERSION`, `CONFLICTO_INEXISTENTE` |
| 409 | Conflicto de estado / cerrado / idempotencia | `TRANSICION_NO_PERMITIDA`, `RELEVAMIENTO_NO_EN_REVISION`, `CONFLICTOS_PENDIENTES`, `MARCADOR_CON_OBSERVACIONES`, `IDENTIFICADOR_DUPLICADO`, `SUBIDA_NO_CONCLUIDA`, `RELEVAMIENTO_CERRADO`, `CLAVE_REUTILIZADA_INCONSISTENTE`, `OPERACION_NO_IDEMPOTENTE`, `PROVEEDOR_NO_DISPONIBLE` |
| 500 | Error interno | `ERROR_INTERNO` |

Degradación informada (no rechazo duro): algunas condiciones completan la operación parcialmente e informan el faltante en el problema, para que el cliente decida: `FOTO_NO_ALMACENABLE` (la observación se conservó sin esa foto), `FOTO_NO_RECUPERABLE` (la exportación se detuvo para no entregar una unidad incompleta) y `ETIQUETA_DESCONOCIDA` (el filtro por una etiqueta inexistente devuelve conjunto vacío e informa las etiquetas válidas). Detalle en `dx-error-messages_v1.0.md` §3.8 y §3.10.

## 8. Ejemplos por área

Snippets breves de las operaciones de comportamiento menos obvio. El recorrido completo vive en `guia-onboarding-developer_v1.0.md` y `guia-integracion-cliente-http_v1.0.md`.

Crear relevamiento (alta idempotente con tramo no vacío):

```text
http POST $BASE/relevamientos "Authorization: Bearer $TOKEN" \
  "Idempotency-Key: alta-rel-001" \
  nombre="Tramo norte" tramo:='{ "puentes": ["P1"], "caminos": ["C1"] }'
# -> 201 { "id": "rel-001", "estado": "recoleccion", ... }
# tramo vacío -> 400 TRAMO_INCOMPLETO
```

Mover un marcador (la identidad no cambia, RC-01):

```text
http PATCH $BASE/relevamientos/rel-001/marcadores/mar-007 "Authorization: Bearer $TOKEN" \
  "Idempotency-Key: mover-mar-007-001" \
  coordenada:='{ "lat": -31.421, "lon": -64.181 }'
# -> 200 { "id": "mar-007", ... }   (mismo id; observaciones siguen ancladas)
```

Sincronización subir-antes-de-bajar:

```text
http POST $BASE/relevamientos/rel-001/sincronizacion/subida "Authorization: Bearer $TOKEN_AGENTE" \
  cambios:='[ { "idOrigen": "c-001", "tipo": "observacion", "datos": {} } ]'
# -> 200 { "aplicados": 1, "reconocidosYaRecibidos": 0, "conflictosRegistrados": 0 }
http POST $BASE/relevamientos/rel-001/sincronizacion/bajada "Authorization: Bearer $TOKEN_AGENTE" \
  marca="<marca-opaca>"
# bajada antes de concluir la subida -> 409 SUBIDA_NO_CONCLUIDA
```

Cerrar un relevamiento (exige conflictos resueltos):

```text
http POST $BASE/relevamientos/rel-001/cierre "Authorization: Bearer $TOKEN" \
  "Idempotency-Key: cierre-rel-001"
# conflictos pendientes -> 409 CONFLICTOS_PENDIENTES
# relevamiento no en revisión -> 409 RELEVAMIENTO_NO_EN_REVISION
```

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Paridad de contrato | `contratos-rest_v1.0.md` §3 (35 operaciones), §4 (esquemas), §5 (errores), §6 (versionado) |
| CU cubiertos | CU-01 a CU-22 (02) |
| RN aplicables | RN-01 a RN-07 (02) |
| RC aplicables | RC-01 a RC-06 (02) |
| ADRs que lo gobiernan | ADR-03, ADR-04, ADR-05, ADR-06, ADR-07, ADR-08, ADR-09, ADR-10 (05) |
| Catálogo de errores accionable | `dx-error-messages_v1.0.md` (03) |
| Tests de paridad | Contract test del 100 % de endpoints públicos por versión (08 `estrategia-testing_v1.0.md` §1) |

## 10. Referencias cruzadas

- 05 `contratos-rest_v1.0.md`: contrato fuente; esta referencia mantiene paridad uno a uno con sus 35 operaciones, esquemas y errores.
- 05 ADR-03, ADR-04, ADR-05, ADR-07, ADR-08, ADR-10: decisiones que fijan autenticación, paginación, errores, sincronización, idempotencia y versionado.
- 02 `modelo-conceptual_v1.0.md` y CU-01 a CU-22: entidades y casos de uso que los endpoints materializan.
- 03 `dx-error-messages_v1.0.md`: catálogo accionable por código (causa y acción).
- 08 `estrategia-testing_v1.0.md` §1, §2: contract tests que verifican la paridad endpoint a endpoint.
- `conceptos-fundamentales_v1.0.md`, `guia-onboarding-developer_v1.0.md`, `guia-integracion-cliente-http_v1.0.md`, `troubleshooting_v1.0.md`, `glosario-tecnico_v1.0.md`.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Referencia inicial curada desde `contratos-rest_v1.0.md` (05): las 35 operaciones por área con seguridad e idempotencia, esquemas lógicos con invariantes, cabeceras, códigos de estado, autenticación, paginación, idempotencia, versionado y taxonomía completa de errores problem+json RFC 9457 (que reemplaza RFC 7807) con ejemplos por área. Paridad uno a uno con el contrato de 05. Vocabulario REST genérico, sin productos del stack (D7). |
