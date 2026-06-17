# Ejemplo 01 — Cliente HTTP básico de línea de comandos

**Proyecto:** geovial-api
**Documento:** ejemplo-01-cliente-http-basico_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Sample Engineer + API Demo
**Nivel:** Básico
**Ubicación del código:** `/samples/geovial-api/01-cliente-http-basico/`

## 1. Objetivo del sample

Demostrar el camino feliz mínimo del contrato REST de geovial-api desde un cliente HTTP de línea de comandos: enviar credenciales para recibir un token bearer, ejecutar un request de lectura (listar relevamientos del propio alcance) y ejecutar un request de escritura (dar de alta un agente del área). Al terminar, el desarrollador sabe autenticarse contra la API, portar el token en la cabecera de autorización, indicar la versión del contrato en la ruta y reconocer una respuesta problem+json.

## 2. Nivel

Básico. Es el punto de entrada absoluto del proyecto: no requiere cliente tipado ni colección de pruebas, solo un cliente HTTP de línea de comandos y un par de credenciales de prueba. Sienta las bases (token bearer, versión en la ruta, lectura, escritura) que los samples 02 y 03 dan por conocidas.

## 3. Prerequisites

- Un cliente HTTP de línea de comandos capaz de enviar cabeceras y cuerpo JSON y de mostrar el código de estado y el cuerpo de la respuesta (cualquier versión moderna).
- Dirección base del entorno de prueba donde corre geovial-api, incluido el prefijo de versión mayor (por ejemplo, terminado en `/v1`), exportable a `$BASE`.
- Credenciales de prueba de un usuario con rol de jefe de área (identificador de acceso y secreto). El alta la realiza un rol superior; no hay auto-registro.
- Un lugar donde el cliente conserve el token bearer entre invocaciones (variable de entorno); la API no lo custodia.

## 4. Cómo correrlo

1. Exportar la dirección base con su prefijo de versión: `export BASE=<direccion-del-entorno-de-prueba>/v1`.
2. Autenticarse y guardar el token: `./01-login.sh` (envía credenciales y exporta el token bearer recibido a `$TOKEN`).
3. Ejecutar el request de lectura: `./02-listar-relevamientos.sh` (lista la primera página de relevamientos del alcance).
4. Ejecutar el request de escritura: `./03-alta-agente.sh` (da de alta un agente del área con clave de idempotencia).
5. Comparar las respuestas con el output esperado de la sección 6.

## 5. Estructura del código

```
01-cliente-http-basico/
├── README.md                    # Resumen del sample y enlace a este markdown
├── .env.example                 # Plantilla de BASE y credenciales de prueba
├── 01-login.sh                  # POST /v1/sesiones: envía credenciales, recibe token bearer
├── 02-listar-relevamientos.sh   # GET /v1/relevamientos: request de lectura autenticado
├── 03-alta-agente.sh            # POST /v1/agentes: request de escritura con clave de idempotencia
├── ejemplo-alta-agente.json     # Payload de ejemplo del POST de escritura
└── tests/
    └── camino-feliz.test.sh     # Verifica los tres pasos contra el output esperado
```

## 6. Qué esperar

Paso 2 — respuesta del envío de credenciales (estado `200 OK`). El cuerpo trae el token bearer, su vigencia en segundos y el rol que porta:

```json
{
  "token": "<token-bearer-opaco>",
  "expiraEn": 3600,
  "rol": "jefe-de-area"
}
```

Paso 3 — respuesta del request de lectura (estado `200 OK`). El listado siempre llega paginado y acotado al alcance del solicitante, nunca el conjunto completo:

```json
{
  "elementos": [
    { "id": "rel-001", "estado": "recoleccion", "nombre": "Tramo norte" }
  ],
  "tamanoEfectivo": 1,
  "paginaSiguiente": null,
  "paginaAnterior": null
}
```

Paso 4 — respuesta del request de escritura (estado `201 Created`). El agente queda creado y habilitado; reenviar la misma operación con la misma clave de idempotencia devuelve el mismo resultado sin crear un segundo agente:

```json
{
  "id": "agente-007",
  "identificadorAcceso": "ana.campo",
  "rol": "agente-de-campo",
  "habilitado": true
}
```

Ejemplo de problem+json. Si el envío de credenciales no coincide, la API responde con estado `401` y un cuerpo con un código estable, opaco al idioma; el cliente decide por el código, nunca por el texto del mensaje:

```json
{
  "codigo": "CREDENCIALES_INVALIDAS",
  "mensaje": "El identificador o el secreto no coinciden.",
  "estado": 401,
  "recurso": "sesiones"
}
```

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado |
| --- | --- | --- |
| Forzar un problem+json de autenticación | Enviar un secreto incorrecto en el paso 2 | Respuesta problem+json con código `CREDENCIALES_INVALIDAS` y estado 401 |
| Forzar un problem+json de validación | Crear un relevamiento con un tramo sin puentes ni caminos | Respuesta problem+json con código `TRAMO_INCOMPLETO` y estado 400 |
| Pedir un recurso fuera del alcance | Consultar un relevamiento de otro jefe de área | Respuesta problem+json con código `FUERA_DE_ALCANCE` y estado 403 |
| Solicitar una versión retirada | Cambiar el prefijo de la ruta a una versión inexistente | Respuesta problem+json con código `VERSION_NO_SOPORTADA` informando las versiones vigentes (CU-22) |

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| CU-03 | Caso de uso | Envía credenciales y obtiene un token bearer que porta en los requests siguientes |
| CU-04 | Caso de uso | Lista relevamientos del alcance como request de lectura |
| CU-02 | Caso de uso | Da de alta un agente del área como request de escritura |
| CU-19 | Caso de uso | Muestra una respuesta problem+json con código estable ante credenciales inválidas |
| CU-22 | Caso de uso | Indica la versión mayor del contrato en el prefijo de la ruta |
| ADR-03 | Decisión arquitectónica | Porta el token bearer en la cabecera de autorización en toda operación salvo el inicio de sesión |
| ADR-05 | Decisión arquitectónica | Reconoce el error como problem+json con código estable, no por el texto del mensaje |
| ADR-10 | Decisión arquitectónica | Concentra el prefijo de versión mayor en la dirección base |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Versión inicial del sample de cliente HTTP básico: autenticación con token bearer, un request de lectura, un request de escritura idempotente y reconocimiento de problem+json. |
