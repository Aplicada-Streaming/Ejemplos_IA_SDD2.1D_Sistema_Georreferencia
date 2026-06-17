# Developer guide — geovial-api

**Proyecto:** geovial-api
**Documento:** README.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Technical Writer + API Documentation Lead
**Audiencia:** Developer consumidor de la API HTTP (equipos de geovial-web y geovial-mobile, integradores internos)

Índice navegable de la documentación de consumo del contrato REST de geovial-api, el backend que recibe, conserva y entrega el relevamiento fotográfico georreferenciado de tramos viales. Esta carpeta es la ventana del proyecto al developer que va a consumir la API: siguiéndola, alguien que nunca vio el contrato debería poder hacerlo funcionar sin pedir ayuda al equipo que lo construyó. La audiencia son los integradores internos de los dos clientes de la solución (el front web y la app móvil), no un portal de terceros.

Este README es una tabla de contenidos viva; no duplica el contenido de los artículos.

## 1. Artefactos de la sección

| Documento | Tipo Diátaxis | Nivel | Para qué sirve |
| --- | --- | --- | --- |
| `conceptos-fundamentales_v1.0.md` | Explanation | Medio | Modelo mental de la API: recursos, jerarquía de roles y autorización, sincronización subir-luego-bajar, tolerancia a conflictos; decisiones de diseño y qué NO hace. |
| `guia-onboarding-developer_v1.0.md` | Tutorial | Básico | De cero a integración en una hora: primer request autenticado, primer caso real, integración encadenada. |
| `guia-integracion-cliente-http_v1.0.md` | How-to | Medio | Integrar un cliente HTTP de referencia que consume la API de forma robusta, paso a paso. |
| `referencia-api_v1.0.md` | Reference | Avanzado | Contrato exacto curado desde OpenAPI: 35 operaciones, esquemas, códigos de estado, autenticación, paginación, idempotencia, versionado y errores problem+json. |
| `troubleshooting_v1.0.md` | How-to (diagnóstico) | Medio | Diagnóstico paso a paso de las condiciones frecuentes (ISSUE-01 a ISSUE-06) y reporte de bugs. |
| `glosario-tecnico_v1.0.md` | Reference | Básico | Vocabulario canónico del consumidor con referencia cruzada. Fuente única de términos. |

## 2. Orden de lectura recomendado

1. `conceptos-fundamentales_v1.0.md` — para entender el modelo mental antes de tocar la API.
2. `guia-onboarding-developer_v1.0.md` — para llegar al primer ciclo exitoso en menos de una hora.
3. `guia-integracion-cliente-http_v1.0.md` — cuando la tarea concreta es integrar un cliente HTTP.
4. `referencia-api_v1.0.md` — para consultar la firma exacta de un endpoint, un esquema o un código de error.
5. `troubleshooting_v1.0.md` — cuando algo falla; cada entrada lleva un código `ISSUE-XX`.

`glosario-tecnico_v1.0.md` se consulta en cualquier momento: el resto de los documentos enlaza a él para los términos.

## 3. Prerequisitos para empezar

- La dirección base del entorno de prueba donde corre geovial-api, con su prefijo de versión mayor (`/v1`); la provee el equipo de geovial-api.
- Credenciales de un usuario de prueba con un rol conocido. El alta de usuarios la hace un rol superior de la jerarquía; no hay auto-registro. Para el recorrido completo conviene un jefe de área y un agente asignable.
- Un cliente HTTP capaz de enviar encabezados y cuerpo JSON y de leer el estado y el cuerpo de la respuesta.

No hace falta conocer el dominio de relevamiento vial de antemano: el onboarding lo introduce. Sí se asume familiaridad con APIs REST, token bearer y códigos de estado HTTP.

## 4. Quick-start

```text
1. Obtener un token: enviar credenciales al recurso de autenticación.
   -> Esperado: estado de éxito y un cuerpo con el token bearer y el rol.
2. Presentar el token como bearer en un request de lectura dentro del alcance del rol.
   -> Esperado: la primera página de resultados, acotada al ámbito del solicitante.
3. Crear un recurso con una clave de idempotencia estable (por ejemplo, un relevamiento).
   -> Esperado: estado de creación; reenviar la misma operación con la misma clave no duplica.
4. Sincronizar: subir el lote de cambios y, solo después, bajar las novedades.
   -> Esperado: la subida se aplica antes que cualquier bajada; bajar primero devuelve SUBIDA_NO_CONCLUIDA.
5. Tratar todo error por su código estable problem+json, nunca por el texto del mensaje.
```

Recorrido completo, con el primer caso real y la integración encadenada, en `guia-onboarding-developer_v1.0.md`.

## 5. Referencias cruzadas

- 05 `contratos-rest_v1.0.md`: contrato público REST del que esta carpeta deriva (paridad con `referencia-api_v1.0.md`); ADR-03 a ADR-10.
- 02 casos de uso CU-01 a CU-22 y reglas RN-01 a RN-07, RC-01 a RC-06: origen funcional del contrato.
- 03 `dx-developer-experience_v1.0.md` (marco Diátaxis), `guia-onboarding-developer_v1.0.md` (primera hora) y `dx-error-messages_v1.0.md` (catálogo de errores accionable): insumos previos que esta categoría continúa y formaliza.
- 08 `estrategia-testing_v1.0.md`: contract tests del 100 % de endpoints que verifican la paridad del contrato.
- 11 examples: cliente HTTP de referencia y colección de pruebas que ilustran esta guía (categoría downstream).

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice inicial de la categoría 10 de geovial-api: seis artefactos con nivel y tipo Diátaxis, orden de lectura, prerequisitos y quick-start de cinco pasos. La guía de integración usa el slug genérico `cliente-http`. |
