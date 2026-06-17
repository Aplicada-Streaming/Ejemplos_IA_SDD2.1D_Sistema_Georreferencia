# Guía de integración — cliente HTTP de referencia

**Proyecto:** geovial-api
**Documento:** guia-integracion-cliente-http_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Technical Writer + API Documentation Lead
**Tipo Diátaxis:** How-to
**Audiencia:** Developer consumidor de la API HTTP (equipos de geovial-web y geovial-mobile, integradores internos)
**Nivel:** Medio
**Tiempo estimado de lectura:** 17 min

## 1. Objetivo

Integrar un cliente HTTP de referencia que consume el contrato REST de geovial-api de forma robusta: gestiona el token bearer, indica la versión del contrato, recorre listados paginados, hace escrituras idempotentes y reintentables, ejecuta el ciclo de sincronización subir-antes-de-bajar y trata los errores por su código estable. El slug `cliente-http` es genérico: aplica a cualquier cliente HTTP que un consumidor construya (el de geovial-web o el de geovial-mobile), no a un producto concreto.

Esta guía resuelve tareas; el porqué de cada garantía vive en `conceptos-fundamentales_v1.0.md` y la firma exacta de cada endpoint en `referencia-api_v1.0.md`. Los snippets usan un cliente HTTP de línea de comandos genérico (`http`) con la dirección base en `$BASE` (incluye el prefijo de versión `/v1`).

## 2. Prerequisites

- Completado el recorrido de `guia-onboarding-developer_v1.0.md` (al menos el Hello world): sabés obtener un token y hacer un request autenticado.
- `$BASE` apuntando al entorno con su prefijo de versión mayor (por ejemplo `.../v1`).
- Credenciales de prueba de un jefe de área y de un agente asignable.
- Un lugar seguro donde el cliente guarde el token y la `marca-de-sincronizacion` entre invocaciones (la API no las custodia).

## 3. Pasos

### Paso 1 — Obtener y portar el token bearer

Obtené el token enviando credenciales una sola vez y presentalo como bearer en cada request subsiguiente. El token porta el rol y tiene vigencia limitada.

```text
http POST $BASE/sesiones identificadorAcceso="jefe.norte" secreto="<credencial>"
# -> { "token": "<bearer>", "expiraEn": 3600, "rol": "jefe-de-area" }
export TOKEN=<bearer>
```

Efecto esperado: todo request siguiente lleva `Authorization: Bearer $TOKEN`. Cuando el token venza o la sesión se cierre por completo (`TOKEN_REVOCADO`), el cliente vuelve a este paso y obtiene un token nuevo. No ramifiques por el texto del error: ramificá por el código.

### Paso 2 — Indicar la versión del contrato en la ruta

Todas las rutas cuelgan del prefijo de versión mayor (`/v1`). Si la política exige versión explícita y se omite, el backend rechaza con `VERSION_REQUERIDA_AUSENTE`; una versión retirada o inexistente devuelve `VERSION_NO_SOPORTADA` informando las vigentes (CU-22). Concentrá el prefijo en `$BASE` para migrar de versión mayor cambiando un solo valor.

### Paso 3 — Recorrer un listado paginado de punta a punta

Ningún listado entrega el conjunto completo. Pedí la primera página con un tamaño acotado y seguí la referencia `paginaSiguiente` hasta que sea nula.

```text
http GET "$BASE/relevamientos?tamano=20" "Authorization: Bearer $TOKEN"
# -> { "elementos": [...], "tamanoEfectivo": 20, "paginaSiguiente": "<ref>", "paginaAnterior": null }
http GET "$BASE/relevamientos?pagina=<ref>" "Authorization: Bearer $TOKEN"
```

Efecto esperado: el cliente itera hasta `paginaSiguiente: null`. Si pedís un tamaño mayor al máximo, el backend lo acota y lo informa en `tamanoEfectivo`, sin rechazar (CU-20). Un filtro u orden no admitido se rechaza con `FILTRO_NO_SOPORTADO` u `ORDEN_NO_SOPORTADO`, que informan los valores válidos; una posición inválida, con `POSICION_INVALIDA`.

### Paso 4 — Hacer escrituras idempotentes y reintentables

Toda operación no segura reintentable lleva una clave de idempotencia en la cabecera dedicada. Generá una clave estable por operación lógica (no una nueva por reintento) y reusala exactamente al reintentar tras una respuesta no recibida.

```text
http POST $BASE/relevamientos "Authorization: Bearer $TOKEN" \
  "Idempotency-Key: alta-rel-norte-001" \
  nombre="Tramo norte" tramo:='{ "puentes": ["P1"], "caminos": ["C1"] }'
# Reintento exacto tras un timeout: misma clave, mismo cuerpo -> mismo resultado, sin duplicar
```

Efecto esperado: el recurso se crea una sola vez; el reintento devuelve el mismo resultado. Reusar la clave con un contenido distinto se rechaza con `CLAVE_REUTILIZADA_INCONSISTENTE` (estado 409): usá una clave nueva para una operación de contenido distinto. Omitir la clave en una operación que la exige devuelve `CLAVE_REQUERIDA_AUSENTE`.

Patrón de reintento recomendado del cliente:

```text
intentar escritura con Idempotency-Key estable
  si timeout o sin respuesta -> reintentar con la MISMA clave
  si 409 CLAVE_REUTILIZADA_INCONSISTENTE -> es un bug del cliente: generó la misma clave para otro contenido
  si 2xx -> registrar el resultado y no reintentar
```

### Paso 5 — Ejecutar el ciclo de sincronización subir-antes-de-bajar

El ciclo tiene dos fases ordenadas. El cliente nunca baja antes de concluir la subida (RN-06, ADR-07).

```text
# Fase 1: subir el lote local. Cada cambio porta idOrigen estable (idempotencia de sync).
http POST $BASE/relevamientos/$REL/sincronizacion/subida "Authorization: Bearer $TOKEN_AGENTE" \
  cambios:='[ { "idOrigen": "c-001", "tipo": "marcador", "datos": {} },
              { "idOrigen": "c-002", "tipo": "observacion", "datos": {} } ]'
# -> { "aplicados": 2, "reconocidosYaRecibidos": 0, "conflictosRegistrados": 1 }

# Fase 2: solo tras concluir la subida, bajar novedades posteriores a la marca.
http POST $BASE/relevamientos/$REL/sincronizacion/bajada "Authorization: Bearer $TOKEN_AGENTE" \
  marca="<marca-opaca-actual>"
# -> { "novedades": [...], "marca": "<marca-nueva-opaca>" }
```

Efecto esperado: la subida se aplica completa antes de cualquier bajada; el cliente persiste la marca nueva solo después de aplicar las novedades, para no retroceder ante un corte. Si la red cae a mitad de la subida, el cliente reenvía el mismo lote: los cambios ya aplicados se reconocen como recibidos y no se duplican (idempotencia por `idOrigen`). Bajar sin subida concluida devuelve `SUBIDA_NO_CONCLUIDA`; una marca no reconocible, `MARCA_INVALIDA`, que obliga a una sincronización completa.

### Paso 6 — Tratar los errores por su código estable

Todo error llega como problem+json con un código estable, opaco al idioma. El cliente decide por el código, nunca por el texto del mensaje.

```text
{ "codigo": "CONFLICTOS_PENDIENTES", "mensaje": "...", "estado": 409, "recurso": "rel-001" }
```

Mapeá cada código a una acción del cliente (catálogo completo en `referencia-api_v1.0.md` §7):

| Naturaleza | Estado | Acción del cliente |
| --- | --- | --- |
| Autenticación (`NO_AUTENTICADO`, `TOKEN_REVOCADO`) | 401 | Reautenticar (Paso 1) y reintentar |
| Autorización (`FUERA_DE_ALCANCE`, `ROL_NO_AUTORIZADO`) | 403 | No reintentar igual; corregir rol o ámbito |
| Validación (`TRAMO_INCOMPLETO`, `FILTRO_NO_SOPORTADO`) | 400 | Corregir la entrada según el campo señalado y reintentar |
| Recurso inexistente (`RECURSO_NO_ENCONTRADO`) | 404 | Verificar el identificador o la versión |
| Conflicto de estado (`SUBIDA_NO_CONCLUIDA`, `CONFLICTOS_PENDIENTES`) | 409 | Resolver el estado previo antes de reintentar |
| Relevamiento cerrado (`RELEVAMIENTO_CERRADO`) | 409 | Dejar de escribir sobre ese relevamiento |
| Error interno (`ERROR_INTERNO`) | 500 | Reintentar más tarde con espera; escalar si persiste |

## 4. Verificación

La integración funciona cuando el cliente, en una corrida contra el entorno de prueba, logra todo lo siguiente:

1. Obtiene un token y lo reusa en varios requests sin reautenticar en cada uno.
2. Recorre un listado paginado de punta a punta siguiendo `paginaSiguiente` hasta `null`.
3. Crea un recurso con clave de idempotencia y, al reenviar la misma operación con la misma clave, no crea un segundo recurso.
4. Completa una subida y una bajada en ese orden; al pedir la bajada primero, recibe `SUBIDA_NO_CONCLUIDA`.
5. Ante un error, ramifica por el código estable y no por el texto del mensaje.

## 5. Troubleshooting específico de esta integración

Subconjunto de problemas frecuentes al integrar un cliente HTTP. El diagnóstico paso a paso completo vive en `troubleshooting_v1.0.md`.

| Síntoma | Código | Issue global | Qué hacer |
| --- | --- | --- | --- |
| El cliente reautentica en cada request | — | — | Cachear el token hasta que venza; reautenticar solo ante 401 |
| Un reintento crea recursos duplicados | falta clave | ISSUE-04 | Generar una `clave-de-idempotencia` estable por operación, no una por reintento |
| La bajada de sincronización se rechaza | `SUBIDA_NO_CONCLUIDA` | ISSUE-05 | Concluir la subida del ciclo antes de bajar |
| La sincronización rehace todo cada vez | `MARCA_INVALIDA` | ISSUE-05 | Persistir y reenviar la `marca-de-sincronizacion` que devuelve la bajada |
| El cliente trata mal un error tras una traducción | — | ISSUE-01 | Ramificar por el código estable, nunca por el texto del mensaje |
| Una escritura se rechaza pese al token | `ROL_NO_AUTORIZADO`, `FUERA_DE_ALCANCE` | ISSUE-02, ISSUE-03 | Usar un rol y un ámbito habilitados para el recurso |

## 6. Referencias cruzadas

- 05 `contratos-rest_v1.0.md` §2 (convenciones), §3 (operaciones), §5 (errores), §6 (versionado): contrato que este cliente consume.
- 05 ADR-04 (paginación), ADR-07 (orden de sincronización), ADR-08 (idempotencia), ADR-10 (versionado): garantías que el cliente debe respetar.
- 02 CU-20 (paginación), CU-21 (idempotencia), CU-10/CU-11 (sincronización), CU-22 (versionado).
- 08 `estrategia-testing_v1.0.md` §1, §3: el cliente HTTP de pruebas y los contract tests que ejercitan esta superficie.
- `guia-onboarding-developer_v1.0.md`, `referencia-api_v1.0.md`, `troubleshooting_v1.0.md`, `conceptos-fundamentales_v1.0.md`.

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | How-to inicial de integración de un cliente HTTP de referencia contra geovial-api: gestión del token bearer, versión en la ruta, recorrido de listados paginados, escrituras idempotentes con patrón de reintento, ciclo de sincronización subir-antes-de-bajar y tratamiento de errores por código estable; con verificación y troubleshooting específico. Slug genérico `cliente-http` (D7, sin productos del stack). |
