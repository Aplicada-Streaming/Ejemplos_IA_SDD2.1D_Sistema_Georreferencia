# Guía de onboarding del developer — geovial-api

**Proyecto:** geovial-api
**Documento:** guia-onboarding-developer_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Technical Writer + API Documentation Lead
**Tipo Diátaxis:** Tutorial
**Audiencia:** Developer consumidor de la API HTTP (equipos de geovial-web y geovial-mobile, integradores internos)
**Nivel:** Básico
**Tiempo estimado de lectura:** 14 min

Este tutorial lleva al integrador de cero a una integración productiva sobre el contrato REST de geovial-api en menos de una hora: un primer request autenticado en menos de cinco minutos, un primer caso real en menos de treinta y una integración encadenada en menos de una hora. Los snippets usan un cliente HTTP de línea de comandos genérico (`http POST ...`) que cualquier cliente equivale; la dirección base se nota como `$BASE`. El modelo mental detrás de cada paso vive en `conceptos-fundamentales_v1.0.md`; la firma exacta de cada endpoint, en `referencia-api_v1.0.md`. Los términos en `codigo-kebab` se definen en `glosario-tecnico_v1.0.md`.

> Relación con 03: la `guia-onboarding-developer_v1.0.md` de la categoría 03 cubre la primerísima hora (token, primer error, primera paginación). Esta guía la continúa: completa un ciclo de escritura de punta a punta y encadena recolección, sincronización y revisión.

## 1. Prerequisites

Verificá cada prerequisito antes de empezar; cada uno trae cómo obtenerlo.

| Prerequisito | Cómo obtenerlo | Efecto esperado |
| --- | --- | --- |
| Dirección base del entorno de prueba donde corre geovial-api | La provee el equipo de geovial-api | Exportarla a `$BASE` (por ejemplo `export BASE=https://entorno-prueba/v1`); incluye el prefijo de versión mayor `/v1` (CU-22) |
| Credenciales de un usuario de prueba con rol de jefe de área | El alta la hace un rol superior; no hay auto-registro | Permiten crear, asignar, consultar y cerrar relevamientos |
| Credenciales de un usuario de prueba con rol de agente, asignable por ese jefe de área | El jefe de área da de alta al agente (Paso del primer caso real) | Permiten capturar y sincronizar |
| Un cliente HTTP capaz de enviar encabezados y cuerpo JSON y leer estado y cuerpo de respuesta | Cualquiera | Ejecutar los snippets de esta guía |

No hace falta conocer el dominio de relevamiento vial de antemano: esta guía lo introduce. Sí se asume familiaridad con APIs REST, token bearer y códigos de estado HTTP.

Hito de la sección: tenés `$BASE` y dos pares de credenciales (jefe de área y agente) a mano.

## 2. Hello world: primer request autenticado (< 5 min)

Objetivo: obtener un token bearer y hacer un primer request autenticado que devuelva datos.

Paso 1 — Obtener el token enviando credenciales.

```text
http POST $BASE/sesiones \
  identificadorAcceso="jefe.norte" \
  secreto="<credencial-de-prueba>"
```

Efecto esperado: estado `200 OK` y un cuerpo con el token bearer. Guardalo para los pasos siguientes.

```text
{ "token": "<token-bearer-opaco>", "expiraEn": 3600, "rol": "jefe-de-area" }
```

```text
export TOKEN=<token-bearer-opaco>
```

Paso 2 — Hacer el primer request autenticado a un recurso de lectura.

```text
http GET $BASE/relevamientos "Authorization: Bearer $TOKEN"
```

Efecto esperado: estado `200 OK` y la primera página de relevamientos visibles para el solicitante, acotada a su ámbito (nunca el conjunto completo sin paginar).

```text
{
  "elementos": [ { "id": "rel-001", "estado": "recoleccion", "nombre": "Tramo norte" } ],
  "tamanoEfectivo": 1,
  "paginaSiguiente": null,
  "paginaAnterior": null
}
```

Hito verificable: recibiste un token y, con él, una página de resultados de tu ámbito.

Si falla:
- `CREDENCIALES_INVALIDAS` (estado 401): el identificador o el secreto no coinciden. Revisá el par de prueba.
- `NO_AUTENTICADO` (estado 401): el token no viajó o no es legítimo. Revisá que el encabezado lleve `Bearer $TOKEN`.
- `FUERA_DE_ALCANCE` (estado 403): el recurso pertenece a otro ámbito. Operá recursos de tu propio ámbito (`troubleshooting_v1.0.md` ISSUE-02).

## 3. Primer caso real: crear un relevamiento y dejarlo en recolección (< 30 min)

Objetivo: recorrer una escritura de punta a punta con datos representativos. Como jefe de área, vas a dar de alta un agente, crear un relevamiento con su tramo, asignarle el agente y crear un marcador inicial. Todas las escrituras llevan `clave-de-idempotencia` (CU-21).

Paso 1 — Dar de alta un agente del área.

```text
http POST $BASE/agentes "Authorization: Bearer $TOKEN" \
  "Idempotency-Key: alta-agente-ana-001" \
  identificadorAcceso="ana.campo" rol="agente-de-campo"
```

Efecto esperado: estado `201 Created` con el agente creado y habilitado. Guardá su `id` como `$AGENTE`.

Paso 2 — Crear el relevamiento con un tramo no vacío.

```text
http POST $BASE/relevamientos "Authorization: Bearer $TOKEN" \
  "Idempotency-Key: alta-rel-norte-001" \
  nombre="Tramo norte" \
  tramo:='{ "puentes": ["Puente arroyo norte"], "caminos": ["Camino vecinal"] }'
```

Efecto esperado: estado `201 Created` con el relevamiento en estado `recoleccion`. Guardá su `id` como `$REL`.

Si el tramo no define ningún puente ni camino, el backend rechaza con `TRAMO_INCOMPLETO` (estado 400): incluí al menos un puente o camino.

Paso 3 — Asignar el agente al relevamiento.

```text
http POST $BASE/relevamientos/$REL/asignaciones "Authorization: Bearer $TOKEN" \
  "Idempotency-Key: asig-ana-norte-001" \
  agenteId="$AGENTE"
```

Efecto esperado: estado `201 Created` con la asignación vigente. Reenviar exactamente esta operación con la misma clave devuelve el mismo resultado, sin crear una segunda asignación (RC-05, CU-21).

Paso 4 — Crear un marcador inicial para previsualizar la experiencia.

```text
http POST $BASE/relevamientos/$REL/marcadores "Authorization: Bearer $TOKEN" \
  "Idempotency-Key: marcador-inicial-001" \
  coordenada:='{ "lat": -31.42, "lon": -64.18 }' \
  etiquetas:='["referencia"]'
```

Efecto esperado: estado `201 Created` con el marcador y su identidad estable. Mover o reetiquetar ese marcador después no cambia su identidad (RC-01).

Hito verificable: existe un relevamiento en recolección, con un agente asignado y un marcador, todo creado por vos y acotado a tu ámbito. Verificalo:

```text
http GET $BASE/relevamientos/$REL "Authorization: Bearer $TOKEN"
```

## 4. Integración encadenada: recolección, sincronización y revisión (< 1 hora)

Objetivo: encadenar los tres recursos que un cliente real combina, atravesando dos roles. Esta sección es el puente al how-to: el detalle completo de cómo lo orquesta un cliente HTTP vive en `guia-integracion-cliente-http_v1.0.md`.

Paso 1 — Cambiar de rol: obtener el token del agente.

```text
http POST $BASE/sesiones identificadorAcceso="ana.campo" secreto="<credencial>"
export TOKEN_AGENTE=<token-del-agente>
```

Efecto esperado: token con `rol: agente-de-campo`. El agente solo opera relevamientos asignados.

Paso 2 — El agente ancla una observación al marcador y le adjunta una foto.

```text
http POST $BASE/marcadores/<marcadorId>/observaciones "Authorization: Bearer $TOKEN_AGENTE" \
  "Idempotency-Key: obs-fisura-001" \
  nota="Fisura longitudinal en la junta"

http POST $BASE/observaciones/<obsId>/fotos "Authorization: Bearer $TOKEN_AGENTE" \
  "Idempotency-Key: foto-fisura-001" \
  referencia="foto-0001" \
  ubicacion:='{ "lat": -31.4201, "lon": -64.1802 }'
```

Efecto esperado: la observación queda anclada al marcador con su autoría, y la foto queda asociada con su referencia al almacén (el binario lo aloja la librería de almacenamiento, no viaja incrustado).

Paso 3 — Sincronizar: subir antes de bajar.

```text
http POST $BASE/relevamientos/$REL/sincronizacion/subida "Authorization: Bearer $TOKEN_AGENTE" \
  cambios:='[ { "idOrigen": "cambio-001", "tipo": "observacion", "datos": { } } ]'
```

Efecto esperado: estado `200 OK` con el resumen de subida (aplicados y reenvíos reconocidos). Solo después de concluir la subida:

```text
http POST $BASE/relevamientos/$REL/sincronizacion/bajada "Authorization: Bearer $TOKEN_AGENTE" \
  marca="<marca-opaca-actual>"
```

Efecto esperado: estado `200 OK` con las novedades posteriores a la marca y una marca nueva. Pedir la bajada antes de concluir la subida devuelve `SUBIDA_NO_CONCLUIDA` (estado 409): es la garantía subir-antes-de-bajar (`troubleshooting_v1.0.md` ISSUE-05).

Paso 4 — Volver al rol de jefe y revisar sobre mapa.

```text
http GET $BASE/relevamientos/$REL "Authorization: Bearer $TOKEN"
http GET $BASE/relevamientos/$REL/conflictos "Authorization: Bearer $TOKEN"
```

Efecto esperado: el relevamiento trae sus marcadores, observaciones y fotos para la revisión, y el listado de conflictos muestra los conflictos pendientes (si dos marcadores cayeron en un mismo radio). La información es accesible aunque haya conflictos: no bloquean la revisión.

Verificación de la integración: con un solo relevamiento recorriste alta, asignación, captura, sincronización subir-luego-bajar y revisión. Si todos los pasos devolvieron estados de éxito y la revisión muestra la observación capturada por el agente, la integración funciona.

## 5. Siguientes pasos

- Profundizar el modelo mental: `conceptos-fundamentales_v1.0.md` (por qué la sync sube antes de bajar, por qué los conflictos conviven, cómo evoluciona el contrato versionado).
- Resolver una tarea concreta de integración: `guia-integracion-cliente-http_v1.0.md` (orquestar el ciclo completo desde un cliente HTTP de referencia con reintentos y manejo de errores por código).
- Consultar la firma exacta de un endpoint o un código de error: `referencia-api_v1.0.md`.
- Diagnosticar un error: `troubleshooting_v1.0.md` (entradas ISSUE-01 a ISSUE-06).
- Ver código ejecutable: los examples de la categoría 11 (cliente HTTP de referencia, colección de pruebas) ilustran este recorrido.

## 6. Referencias cruzadas

- 05 `contratos-rest_v1.0.md` §3: operaciones exactas de cada paso (autenticación, agentes, relevamientos, asignaciones, marcadores, observaciones, fotos, sincronización, conflictos).
- 05 ADR-07 (orden de sincronización), ADR-08 (idempotencia), ADR-03 (autenticación): garantías que sostienen los pasos 3 y 4.
- 02 CU-03, CU-02, CU-04, CU-05, CU-07, CU-08, CU-10, CU-11, CU-12: casos de uso que el recorrido materializa.
- 08 `estrategia-testing_v1.0.md` §6: el comando reproducible que regenera el lote de sincronización de prueba.
- `conceptos-fundamentales_v1.0.md`, `guia-integracion-cliente-http_v1.0.md`, `referencia-api_v1.0.md`, `troubleshooting_v1.0.md`.

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Tutorial inicial de onboarding de geovial-api: Hello world con primer request autenticado en menos de cinco minutos, primer caso real (alta de agente, relevamiento, asignación y marcador) en menos de treinta y integración encadenada (captura, sincronización subir-luego-bajar y revisión) en menos de una hora, con efecto esperado por paso y puente al how-to. Vocabulario REST genérico (token bearer), sin productos ni protocolos del stack (D7). |
