# Ejemplo 02 — Colección de pruebas reproducible

**Proyecto:** geovial-api
**Documento:** ejemplo-02-postman-collection_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Sample Engineer + API Demo
**Nivel:** Intermedio
**Ubicación del código:** `/samples/geovial-api/02-postman-collection/`

## 1. Objetivo del sample

Demostrar una colección de pruebas reproducible que recorre, en orden y de un tirón, los flujos principales del contrato REST de geovial-api: autenticarse, crear un relevamiento con su tramo, asignarle un agente, crear un marcador inicial, listar con paginación y provocar de forma deliberada un error problem+json para verificar su forma. Al terminar, el desarrollador sabe encadenar requests pasando valores de una respuesta a la siguiente (token, identificadores) y validar automáticamente el código de estado y el cuerpo de cada paso.

## 2. Nivel

Intermedio. Asume el sample 01 (token bearer, lectura, escritura, versión en la ruta). Agrega tres capacidades que el sample anterior no demostraba: el encadenado de requests con variables de colección, la paginación de un listado y la clave de idempotencia estable en cada escritura. Es una corrida reproducible completa, no requests sueltos.

## 3. Prerequisites

- Un ejecutor de colecciones de pruebas HTTP, en su versión de interfaz o en su versión de línea de comandos para correr la colección sin intervención (cualquier versión moderna que importe el formato de colección incluido).
- Dirección base del entorno de prueba con su prefijo de versión mayor (terminado en `/v1`), cargada en la variable de entorno de la colección.
- Credenciales de prueba de un usuario con rol de jefe de área (identificador de acceso y secreto).
- El identificador de acceso de un agente del área que el jefe pueda dar de alta y asignar.

## 4. Cómo correrlo

1. Importar el archivo de colección `geovial-api.collection.json` y el archivo de entorno `entorno-prueba.env.json` en el ejecutor.
2. Completar en el entorno la dirección base `base` y las credenciales de prueba `identificadorAcceso` y `secreto`.
3. Correr la colección completa en orden (en la interfaz, con el ejecutor de la colección; en línea de comandos, con el runner incluido en `run.sh`).
4. Observar el panel de resultados: cada request muestra su código de estado y el resultado de sus aserciones.
5. Comparar el resumen final con el output esperado de la sección 6.

## 5. Estructura del código

```
02-postman-collection/
├── README.md                       # Resumen del sample y cómo importar la colección
├── geovial-api.collection.json     # Colección de pruebas: requests encadenados con aserciones
├── entorno-prueba.env.json         # Variables de entorno (base, credenciales, ids capturados)
├── run.sh                          # Corre la colección sin interfaz (runner de línea de comandos)
└── tests/
    └── resumen-esperado.json       # Resumen de aserciones contra el que se compara la corrida
```

La colección agrupa los requests en carpetas por flujo: `01-sesion` (login), `02-relevamiento` (alta), `03-asignacion` (alta de agente y asignación), `04-marcador` (alta de marcador), `05-listados` (paginación) y `06-error` (problem+json deliberado). Cada request guarda en variables de colección los valores que el siguiente necesita: el token bearer tras el login y los identificadores de relevamiento, agente y marcador tras cada alta.

## 6. Qué esperar

Login (estado `200 OK`). La colección captura el token en una variable y lo inyecta como bearer en los requests siguientes:

```json
{ "token": "<token-bearer-opaco>", "expiraEn": 3600, "rol": "jefe-de-area" }
```

Crear relevamiento (estado `201 Created`). El relevamiento nace en estado `recoleccion`; su identificador se captura para los pasos siguientes:

```json
{
  "id": "rel-001",
  "estado": "recoleccion",
  "nombre": "Tramo norte",
  "tramo": { "puentes": ["Puente arroyo norte"], "caminos": ["Camino vecinal"] }
}
```

Asignar agente (estado `201 Created`). La asignación queda vigente; reenviarla con la misma clave de idempotencia no crea una segunda asignación:

```json
{ "id": "asig-001", "agenteId": "agente-007", "relevamientoId": "rel-001", "vigente": true }
```

Listar con paginación (estado `200 OK`). La colección pide un tamaño acotado y sigue `paginaSiguiente` hasta `null`. Si se pide un tamaño mayor al máximo, el backend lo acota e informa el tamaño aplicado en `tamanoEfectivo`, sin rechazar:

```json
{
  "elementos": [
    { "id": "rel-001", "estado": "recoleccion", "nombre": "Tramo norte" }
  ],
  "tamanoEfectivo": 20,
  "paginaSiguiente": "<referencia-opaca-de-pagina>",
  "paginaAnterior": null
}
```

Error problem+json deliberado. El último request crea un relevamiento con un tramo sin puentes ni caminos; la API responde con estado `400` y un cuerpo problem+json con código estable. La aserción de la colección verifica el código, no el texto:

```json
{
  "codigo": "TRAMO_INCOMPLETO",
  "mensaje": "El tramo debe incluir al menos un puente o un camino.",
  "estado": 400,
  "recurso": "relevamientos"
}
```

Resumen final de la corrida (estado de la colección):

```text
Colección: geovial-api
Requests ejecutados: 7
Aserciones: 14 / 14 OK
Fallos: 0
```

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado |
| --- | --- | --- |
| Reintentar una escritura con la misma clave | Volver a correr el request de asignación con la misma clave de idempotencia | La API devuelve el mismo resultado sin crear una segunda asignación (CU-21) |
| Recorrer varias páginas | Cargar más relevamientos y bajar el tamaño de página a 1 | La colección itera siguiendo `paginaSiguiente` hasta recibir `null` (CU-20) |
| Probar un filtro no soportado | Agregar un filtro inexistente al listado de relevamientos | Respuesta problem+json con código `FILTRO_NO_SOPORTADO` informando los filtros válidos |
| Operar fuera del rol | Usar credenciales de agente para crear un relevamiento | Respuesta problem+json con código `ROL_NO_AUTORIZADO` y estado 403 (CU-18) |

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| CU-03 | Caso de uso | Inicia sesión y captura el token bearer para el resto de la corrida |
| CU-04 | Caso de uso | Crea un relevamiento con su tramo y lo deja en recolección |
| CU-05 | Caso de uso | Asigna un agente al relevamiento de forma idempotente |
| CU-02 | Caso de uso | Da de alta el agente que luego asigna |
| CU-07 | Caso de uso | Crea un marcador inicial sobre el relevamiento |
| CU-20 | Caso de uso | Recorre un listado paginado siguiendo las referencias de navegación |
| CU-19 | Caso de uso | Provoca y verifica una respuesta problem+json por código estable |
| CU-21 | Caso de uso | Usa una clave de idempotencia estable en cada escritura |
| CU-18 | Caso de uso | Ejercita la autorización por rol como variación sugerida |
| ADR-04 | Decisión arquitectónica | Recorre la paginación con tamaño y referencias de página |
| ADR-05 | Decisión arquitectónica | Asienta las aserciones sobre el código estable del problem+json |
| ADR-08 | Decisión arquitectónica | Materializa la idempotencia con clave estable por operación |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Versión inicial de la colección de pruebas reproducible: login, alta de relevamiento, asignación de agente, alta de marcador, paginación y error problem+json deliberado, con aserciones automáticas y encadenado por variables. |
