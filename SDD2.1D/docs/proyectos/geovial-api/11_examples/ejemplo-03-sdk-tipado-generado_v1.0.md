# Ejemplo 03 — Cliente tipado generado a partir del contrato

**Proyecto:** geovial-api
**Documento:** ejemplo-03-sdk-tipado-generado_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Sample Engineer + API Demo
**Nivel:** Avanzado
**Ubicación del código:** `/samples/geovial-api/03-sdk-tipado-generado/`

## 1. Objetivo del sample

Demostrar cómo generar un cliente tipado a partir del contrato OpenAPI de geovial-api y consumir la API de punta a punta, incluido un ciclo de sincronización subir-antes-de-bajar. Al terminar, el desarrollador sabe regenerar el cliente cuando el contrato evoluciona, invocar las operaciones con tipos derivados del contrato (no cadenas sueltas), ejecutar el ciclo de sincronización en su orden obligatorio y entender cómo la versión mayor del contrato protege al cliente generado frente a cambios incompatibles.

## 2. Nivel

Avanzado. Asume los samples 01 (token bearer, lectura, escritura) y 02 (encadenado, paginación, idempotencia). Agrega la generación del cliente a partir del contrato OpenAPI, el ciclo de sincronización de dos fases ordenadas y la lectura del versionado del contrato como garantía de compatibilidad. Es el sample de integración que un consumidor real (el front web o la app móvil) usa como base.

## 3. Prerequisites

- Un generador de clientes a partir de una especificación OpenAPI (cualquier versión moderna que consuma el documento OpenAPI publicado del contrato).
- El runtime del lenguaje del consumidor en el que se genera y se ejecuta el cliente tipado, en la versión mínima declarada para el sample.
- El documento OpenAPI del contrato de geovial-api, accesible desde el entorno de prueba o incluido en el sample como copia versionada.
- Dirección base del entorno de prueba con su prefijo de versión mayor (terminado en `/v1`).
- Credenciales de prueba de un usuario con rol de jefe de área y de un agente asignable por ese jefe; un relevamiento al que el agente quede asignado.

## 4. Cómo correrlo

1. Generar el cliente tipado desde el contrato: `./01-generar-cliente.sh` (lee el documento OpenAPI y emite el cliente en `cliente-generado/`).
2. Configurar el entorno: copiar `.env.example` a `.env` y completar la dirección base y las credenciales de prueba.
3. Ejecutar el recorrido de punta a punta: `./02-recorrido-e2e.sh` (autentica, crea marcador y observación, y corre el ciclo de sincronización subida-luego-bajada).
4. Persistir y reusar la marca de sincronización que devuelve la bajada (el script la guarda en `estado/marca.txt`).
5. Comparar la salida en consola con el output esperado de la sección 6.

## 5. Estructura del código

```
03-sdk-tipado-generado/
├── README.md                    # Resumen del sample y flujo de regeneración del cliente
├── .env.example                 # Plantilla de base y credenciales de prueba
├── openapi/
│   └── geovial-api.openapi.json # Copia versionada del contrato OpenAPI (entrada del generador)
├── 01-generar-cliente.sh        # Genera el cliente tipado desde el contrato OpenAPI
├── 02-recorrido-e2e.sh          # Recorrido punta a punta con el cliente generado
├── cliente-generado/            # Salida del generador (no se edita a mano; se regenera)
├── estado/
│   └── marca.txt                # Marca de sincronización persistida entre corridas
└── tests/
    └── ciclo-sincronizacion.test # Verifica el orden subir-antes-de-bajar y la idempotencia
```

El cliente de `cliente-generado/` no se edita a mano: cuando el contrato publica una versión nueva, se regenera con `01-generar-cliente.sh`. El recorrido invoca las operaciones a través de los tipos generados (sesión, marcadores, observaciones, sincronización), de modo que un cambio incompatible del contrato aparece como un error al regenerar o al compilar, antes de llegar a tiempo de ejecución.

## 6. Qué esperar

Generación del cliente (salida en consola). El generador resuelve las 35 operaciones del contrato bajo la versión mayor declarada en la ruta:

```text
Leyendo contrato OpenAPI: openapi/geovial-api.openapi.json
Version del contrato detectada: v1
Operaciones generadas: 35
Cliente tipado emitido en: cliente-generado/
```

Ciclo de sincronización — fase de subida (estado `200 OK`). Cada cambio del lote porta un identificador de origen estable; el resumen distingue los aplicados de los ya recibidos y registra los conflictos sin bloquear:

```json
{
  "aplicados": 2,
  "reconocidosYaRecibidos": 0,
  "conflictosRegistrados": 1
}
```

Ciclo de sincronización — fase de bajada (estado `200 OK`), solo después de concluir la subida. La respuesta trae las novedades posteriores a la marca y una marca nueva opaca que el cliente persiste solo tras aplicar las novedades:

```json
{
  "novedades": [
    { "tipo": "marcador", "id": "marcador-031", "accion": "actualizado" }
  ],
  "marca": "<marca-nueva-opaca>"
}
```

Resumen del recorrido (salida en consola):

```text
Autenticacion: OK (rol agente-de-campo)
Observacion anclada al marcador: OK
Sincronizacion subida: 2 aplicados, 1 conflicto registrado
Sincronizacion bajada: 1 novedad, marca persistida
Recorrido punta a punta: OK
```

Ejemplo de problem+json del ciclo de sincronización. Si el cliente pide la bajada antes de concluir la subida, la API responde con estado `409` y un código estable que el cliente generado mapea a su acción de recuperación (concluir la subida y reintentar):

```json
{
  "codigo": "SUBIDA_NO_CONCLUIDA",
  "mensaje": "No se puede bajar actualizaciones hasta concluir la subida del ciclo.",
  "estado": 409,
  "recurso": "rel-001"
}
```

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado |
| --- | --- | --- |
| Reanudar una subida cortada | Reenviar el mismo lote con los mismos identificadores de origen | Los cambios ya aplicados se reconocen como recibidos y no se duplican (idempotencia de sincronización) |
| Invertir el orden del ciclo | Pedir la bajada antes de la subida | Respuesta problem+json con código `SUBIDA_NO_CONCLUIDA` y estado 409 (orden subir-antes-de-bajar) |
| Usar una marca no reconocible | Enviar una marca de sincronización inválida en la bajada | Respuesta problem+json con código `MARCA_INVALIDA`, que obliga a una sincronización completa |
| Regenerar contra una versión mayor nueva | Apuntar el generador a una versión mayor distinta del contrato | El cliente se regenera contra la versión nueva; un cambio incompatible aparece al regenerar, no en producción (CU-22) |

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| CU-10 | Caso de uso | Sube el lote de cambios locales con identificador de origen por cambio |
| CU-11 | Caso de uso | Baja las novedades posteriores a la marca solo tras concluir la subida |
| CU-22 | Caso de uso | Genera el cliente bajo la versión mayor del contrato y lo regenera al evolucionar |
| CU-07 | Caso de uso | Crea y mueve un marcador a través del cliente tipado |
| CU-08 | Caso de uso | Ancla una observación con su foto al marcador |
| CU-12 | Caso de uso | Consulta el relevamiento para la revisión tras sincronizar |
| CU-13 | Caso de uso | Observa los conflictos registrados que conviven con la operación |
| CU-21 | Caso de uso | Reenvía el lote con el mismo identificador de origen sin duplicar |
| ADR-07 | Decisión arquitectónica | Respeta el orden subir-antes-de-bajar del ciclo de sincronización |
| ADR-08 | Decisión arquitectónica | Materializa la idempotencia de la subida por identificador de origen |
| ADR-10 | Decisión arquitectónica | Genera y regenera el cliente contra la versión mayor del contrato en la ruta |
| NFR (intake §17.P.10) | Requisito no funcional | El ciclo tolera un lote de al menos 1000 cambios por relevamiento |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Versión inicial del sample de cliente tipado generado: generación a partir del contrato OpenAPI, recorrido punta a punta con tipos derivados del contrato, ciclo de sincronización subir-antes-de-bajar con idempotencia por identificador de origen y lectura del versionado del contrato como garantía de compatibilidad. |
