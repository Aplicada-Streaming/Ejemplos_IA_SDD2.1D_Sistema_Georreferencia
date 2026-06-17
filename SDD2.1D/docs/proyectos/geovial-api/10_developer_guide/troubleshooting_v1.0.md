# Troubleshooting — geovial-api

**Proyecto:** geovial-api
**Documento:** troubleshooting_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Technical Writer + API Documentation Lead
**Tipo Diátaxis:** How-to (orientado a diagnóstico)
**Audiencia:** Developer consumidor de la API HTTP (equipos de geovial-web y geovial-mobile, integradores internos)
**Nivel:** Medio
**Tiempo estimado de lectura:** 20 min

Guía de diagnóstico de las condiciones más frecuentes que el consumidor de la API encuentra. Cada entrada lleva un código `ISSUE-XX` referenciable desde el código de error real, desde logs o desde tickets. Los códigos de error citados (`MAYUSCULAS_CON_GUION`) son los códigos estables del catálogo, en paridad con los CU de 02 y con `contratos-rest_v1.0.md` §5; su causa y acción accionable viven en `dx-error-messages_v1.0.md` (03). El vocabulario en `codigo-kebab` se define en `glosario-tecnico_v1.0.md`.

Recordatorio: todo error llega como problem+json con un código estable, opaco al idioma. El consumidor decide por el código, nunca por el texto del mensaje. La firma exacta de cada endpoint está en `referencia-api_v1.0.md`.

## 1. Errores comunes (síntoma / causa / solución)

| Issue | Síntoma observado | Causa probable | Solución | Código(s) |
| --- | --- | --- | --- | --- |
| ISSUE-01 | El inicio de sesión o un request autenticado se rechaza con estado 401. | Credenciales que no coinciden, usuario dado de baja, token ausente o sesión cerrada. | Verificar el par de credenciales o reautenticar para obtener un token nuevo. | `CREDENCIALES_INVALIDAS`, `USUARIO_INHABILITADO`, `NO_AUTENTICADO`, `TOKEN_REVOCADO` |
| ISSUE-02 | El request lleva token válido pero se rechaza con estado 403. | El rol no habilita la acción, o el recurso pertenece a otro ámbito o nivel jerárquico. | Usar un rol y un ámbito habilitados; administrar solo el nivel inmediato inferior. | `ROL_NO_AUTORIZADO`, `ACCION_NO_PERMITIDA`, `FUERA_DE_ALCANCE`, `JERARQUIA_NO_PERMITIDA`, `RELEVAMIENTO_NO_ASIGNADO` |
| ISSUE-03 | La solicitud se rechaza con estado 400 por la entrada. | La estructura, el rango o las opciones de la solicitud no son válidas (tramo vacío, filtro u orden no soportado, posición de página inválida). | Corregir el campo señalado en el problema y reintentar; un problema enumera todos los campos inválidos. | `FORMATO_SOLICITUD_INVALIDO`, `TRAMO_INCOMPLETO`, `FILTRO_NO_SOPORTADO`, `ORDEN_NO_SOPORTADO`, `POSICION_INVALIDA` |
| ISSUE-04 | El recurso pedido devuelve estado 404, o un reintento de escritura crea un duplicado. | El identificador o la versión no existen; o la escritura se reintentó sin clave de idempotencia estable. | Verificar el identificador o la versión; adjuntar una clave de idempotencia estable por operación. | `RECURSO_NO_ENCONTRADO`, `RELEVAMIENTO_INEXISTENTE`, `RECURSO_NO_EN_VERSION`, `CLAVE_REQUERIDA_AUSENTE` |
| ISSUE-05 | Una operación de estado se rechaza con estado 409: cerrar, transicionar, bajar sin subir, o escribir sobre un relevamiento cerrado. | El recurso está en un estado que no admite la operación: conflictos pendientes, transición no válida, subida no concluida o relevamiento cerrado. | Llevar el recurso al estado previo correcto antes de reintentar; dejar de escribir sobre un relevamiento cerrado. | `CONFLICTOS_PENDIENTES`, `TRANSICION_NO_PERMITIDA`, `RELEVAMIENTO_NO_EN_REVISION`, `SUBIDA_NO_CONCLUIDA`, `RELEVAMIENTO_CERRADO` |
| ISSUE-06 | Un reintento con clave se rechaza con estado 409, o la clave no surte efecto. | La clave se reusó con un contenido distinto, o se envió clave a una operación que no la admite. | Usar una clave nueva para una operación de contenido distinto; reusar la clave solo para el reintento exacto. | `CLAVE_REUTILIZADA_INCONSISTENTE`, `OPERACION_NO_IDEMPOTENTE`, `CLAVE_REQUERIDA_AUSENTE` |

## 2. Diagnóstico paso a paso

### ISSUE-01 — Autenticación o credenciales

Naturaleza: condición de credencial o de sesión (ADR-03; CU-03, CU-18).

1. Leer el estado y el código. `401` con `CREDENCIALES_INVALIDAS`: el identificador o el secreto no coinciden. `401` con `USUARIO_INHABILITADO`: el usuario fue dado de baja y conserva traza pero no acceso.
2. Si es `NO_AUTENTICADO`: confirmar que el request lleva el encabezado `Authorization: Bearer <token>` y que el token es el emitido por el último inicio de sesión, no uno vencido.
3. Si es `TOKEN_REVOCADO`: la sesión se cerró por completo (cambio de usuario en un dispositivo compartido). El token anterior ya no sirve.
4. Confirmar que el token no venció: el inicio de sesión devuelve la vigencia (`expiraEn`). Un token vencido se trata como no autenticado.

Solución: verificar el par de credenciales o reautenticar (`POST /sesiones`) para obtener un token nuevo y reusarlo en los requests siguientes. La revalidación por seguridad del dispositivo ocurre en el cliente móvil, no en el backend.

### ISSUE-02 — Autorización por rol o alcance insuficiente

Naturaleza: defecto de autorización; no se reintenta igual (ADR-03; CU-18, RN-01, RC-03).

1. Leer el código. `ROL_NO_AUTORIZADO` o `ACCION_NO_PERMITIDA`: el rol del token no habilita esa acción (por ejemplo, un agente intentando crear un relevamiento).
2. Si es `FUERA_DE_ALCANCE`, `USUARIO_FUERA_DE_AMBITO`, `AGENTE_FUERA_DE_AREA` o `RELEVAMIENTO_FUERA_DE_AMBITO`: el recurso pertenece a otro ámbito jerárquico. Operar solo recursos del propio ámbito.
3. Si es `JERARQUIA_NO_PERMITIDA`: se intentó administrar un nivel que no es el inmediato inferior (por ejemplo, un jefe general dando de alta un agente directamente). Administrar solo el nivel inmediato inferior.
4. Si es `RELEVAMIENTO_NO_ASIGNADO`: el agente intenta sincronizar un relevamiento que no tiene asignado. Sincronizar solo relevamientos asignados.
5. Confirmar el rol del token: el inicio de sesión devuelve `rol`. Si no coincide con la acción, usar un usuario con el rol adecuado.

Solución: realizar la acción con un usuario cuyo rol y ámbito la habiliten. No reintentar con el mismo rol: el resultado será el mismo.

### ISSUE-03 — Validación de la entrada

Naturaleza: defecto de la solicitud (ADR-05; CU-19, CU-20).

1. Leer el problema completo: el campo o recurso implicado viene en el cuerpo. Ante varios campos inválidos, un único problema los enumera con su motivo (CU-19 flujo 5.A).
2. Si es `TRAMO_INCOMPLETO`: el tramo no define ningún puente ni camino. Incluir al menos uno antes de crear el relevamiento.
3. Si es `FILTRO_NO_SOPORTADO` u `ORDEN_NO_SOPORTADO`: el recurso no admite ese filtro o campo de orden. La respuesta informa los válidos; usar uno de ellos.
4. Si es `POSICION_INVALIDA`: la posición de página pedida no es válida. Empezar por la primera página y seguir las referencias de navegación que trae la respuesta.
5. Si es `FORMATO_SOLICITUD_INVALIDO`: la estructura del cuerpo no respeta la esperada. Corregirla según el defecto señalado.

Solución: corregir la entrada según el campo que indica el problema y reintentar. La validación no altera el estado del sistema: reintentar la misma solicitud inválida produce el mismo problema.

### ISSUE-04 — Recurso inexistente (y duplicación por reintento sin clave)

Naturaleza: recurso ausente o reintento sin idempotencia (ADR-08, ADR-10; CU-19, CU-21, CU-22).

1. Si el estado es `404`: leer el código. `RECURSO_NO_ENCONTRADO` o `RELEVAMIENTO_INEXISTENTE`: el identificador no existe en el ámbito del solicitante. Verificar el identificador.
2. Si es `RECURSO_NO_EN_VERSION`: el recurso no existe en la versión indicada del contrato. Pedirlo en una versión que lo exponga (revisar el prefijo `/v1`).
3. Distinguir 404 de 403: un recurso fuera del ámbito puede aparecer como `FUERA_DE_ALCANCE` (403), no como 404. Si el identificador es correcto pero el estado es 403, es ISSUE-02.
4. Si en cambio el síntoma es un recurso duplicado tras un reintento: confirmar que la escritura llevó una clave de idempotencia estable. Sin ella, dos envíos crean dos recursos. Si el código fue `CLAVE_REQUERIDA_AUSENTE`, la operación exige clave.

Solución: verificar el identificador o la versión; para evitar duplicados, adjuntar una `clave-de-idempotencia` estable por operación y reusarla exactamente al reintentar (ver ISSUE-06).

### ISSUE-05 — Conflicto de estado o relevamiento cerrado

Naturaleza: el recurso está en un estado que no admite la operación (ADR-06, ADR-07; CU-06, CU-10, CU-11, CU-14, RN-05).

1. Leer el código y el estado (`409`).
2. Si es `CONFLICTOS_PENDIENTES` al cerrar: quedan conflictos de marcadores sin resolver. Listar los conflictos (`GET /relevamientos/{id}/conflictos`), resolverlos uno a uno (`POST .../conflictos/{conflictoId}/resolucion`) y reintentar el cierre.
3. Si es `RELEVAMIENTO_NO_EN_REVISION` al resolver o cerrar: el relevamiento no está en revisión. Transicionarlo a revisión (`POST .../transiciones`) antes de resolver o cerrar.
4. Si es `TRANSICION_NO_PERMITIDA`: el estado origen no admite la transición pedida. Avanzar respetando el ciclo recolección → revisión → cierre.
5. Si es `SUBIDA_NO_CONCLUIDA` al bajar: el ciclo de sincronización pidió la bajada sin concluir la subida. Completar primero la subida y luego bajar (garantía subir-antes-de-bajar).
6. Si es `RELEVAMIENTO_CERRADO` en cualquier escritura: el relevamiento cerrado no admite cambios (marcadores, observaciones, fotos, asignaciones, transiciones ni subida). Dejar de escribir sobre ese relevamiento; en la sincronización, el cliente deja de reintentar la subida de ese relevamiento.
7. Verificar el estado actual del relevamiento con `GET /relevamientos/{id}` antes de reintentar.

Solución: llevar el recurso al estado previo correcto (resolver conflictos, transicionar a revisión, concluir la subida) y reintentar; o dejar de escribir si está cerrado. El conflicto de marcadores no es un error durante la recolección ni la revisión: solo el cierre lo exige resuelto (ADR-06).

### ISSUE-06 — Idempotencia: clave reutilizada o no admitida

Naturaleza: disciplina de la clave de idempotencia del cliente (ADR-08; CU-21, RN-07).

1. Leer el código (estado `409` o `400`).
2. Si es `CLAVE_REUTILIZADA_INCONSISTENTE`: una clave ya procesada se reutilizó con un contenido distinto. El cliente generó la misma clave para dos operaciones diferentes. Usar una clave nueva para la operación de contenido distinto.
3. Si es `OPERACION_NO_IDEMPOTENTE`: se envió clave a una operación segura o no reintentable. No adjuntar clave a esas operaciones.
4. Si es `CLAVE_REQUERIDA_AUSENTE`: la operación no segura exige clave y no se proveyó. Adjuntar una clave estable.
5. Confirmar el patrón del cliente: una clave estable por operación lógica, reusada exactamente al reintentar tras una respuesta no recibida; nunca una clave nueva por reintento.
6. Para la subida de sincronización, la idempotencia es por identificador de origen de cada cambio del lote: confirmar que cada cambio porta un `idOrigen` estable y único por cambio.

Solución: corregir la generación de claves en el cliente. El reintento exacto con la misma clave devuelve el mismo resultado sin duplicar; el reenvío de un lote ya aplicado se reconoce sin reaplicar.

## 3. Logs útiles

El backend emite diagnóstico estructurado; el destino del log lo provee el despliegue (05 `arquitectura-solucion_v1.0.md`; 09).

| Qué buscar | Dónde | Patrón / nivel | Para qué sirve |
| --- | --- | --- | --- |
| Código estable del problema | Cuerpo de la respuesta de error | El campo `codigo` (`MAYUSCULAS_CON_GUION`) | Mapear el síntoma al `ISSUE-XX` correspondiente |
| Estado de la respuesta | Estado HTTP | 400 / 401 / 403 / 404 / 409 / 500 | Acotar la naturaleza del fallo antes de leer el código |
| Identificador de correlación del request | Diagnóstico estructurado | Presente por request | Rastrear un request de punta a punta y citarlo en el reporte |
| Recurso o campo implicado | Cuerpo del problema | Campo `recurso` o lista de campos | Localizar el dato que disparó la validación |
| Rol y ámbito resueltos | Diagnóstico estructurado | Rol del token y ámbito | Confirmar por qué se rechazó por autorización (ISSUE-02) |
| Fase de sincronización | Diagnóstico estructurado | `subida` / `bajada` | Saber si el corte ocurrió antes o después de la compuerta de orden (ISSUE-05) |

El backend nunca expone detalles internos sensibles en el cuerpo de un `ERROR_INTERNO` (CU-19 flujo 5.B): para diagnosticar un 500 se usa el identificador de correlación, no el cuerpo.

## 4. Cómo reportar un bug

Antes de reportar, confirmar que no es una condición esperada del contrato: un 403 por rol (ISSUE-02), un 409 por estado (ISSUE-05) o un conflicto de marcadores durante la recolección no son defectos del backend, sino comportamiento contractual. Un fallo no contemplado por el catálogo de códigos, o un código devuelto en una situación que el contrato no prevé, sí es un posible defecto.

Dónde: reporte etiquetado como `dx` en el canal de incidencias del repositorio de la solución; preguntas de integración en la sección de discusiones (03 `dx-developer-experience_v1.0.md` §7). Las revisiones de contrato entre el equipo de geovial-api y los equipos de geovial-web y geovial-mobile preceden cada versión mayor.

Datos mínimos a adjuntar:

```text
Título: [ISSUE-XX o código] resumen breve de la condición

Versión del contrato: /v1
Operación: <método y ruta, por ejemplo POST /relevamientos/{id}/cierre>
Rol del usuario de prueba: raiz | jefe-general | jefe-de-area | agente-de-campo
Estado HTTP devuelto: <400 | 401 | 403 | 404 | 409 | 500>
Código de error devuelto: <CODIGO_ESTABLE o "ninguno">
Identificador de correlación del request: <de los logs>

Pasos para reproducir:
1. ...
2. ...
3. ...

Comportamiento esperado: ...
Comportamiento observado: ...

Clave de idempotencia usada (si aplica): <valor o "ninguna">
Cuerpo del problema (sin datos sensibles): ...
```

Política de severidad y respuesta: una duplicación de datos por fallo de idempotencia, una escritura aceptada sobre un relevamiento cerrado, o una bajada atendida sin subida concluida son severidad máxima (violan una garantía del contrato). El resto se prioriza por impacto en la integración. Los tiempos de respuesta concretos los fija la operación del repositorio; este documento define los datos mínimos y la severidad, no el SLA.

## 5. Referencias cruzadas

- 05 `contratos-rest_v1.0.md` §5: taxonomía de errores por estado, fuente de los códigos citados en cada issue.
- 05 ADR-03 (autenticación/autorización), ADR-06 (conflictos), ADR-07 (orden de sincronización), ADR-08 (idempotencia): garantías que explican por qué un 403 o un 409 es comportamiento contractual y no un bug.
- 03 `dx-error-messages_v1.0.md` §2 y §3: causa probable y acción accionable de cada código estable.
- 02 CU-18 (autorización), CU-19 (errores), CU-21 (idempotencia), CU-14 (cierre), CU-11 (bajada): casos de uso que originan cada condición.
- 08 `estrategia-testing_v1.0.md` §1: contract tests que reproducen estas condiciones de forma determinista.
- `referencia-api_v1.0.md` §7, `guia-integracion-cliente-http_v1.0.md` §5, `glosario-tecnico_v1.0.md`.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Troubleshooting inicial con seis entradas ISSUE-01 a ISSUE-06 (autenticación/credenciales, autorización por rol o alcance, validación, recurso inexistente y duplicación por reintento, conflicto de estado y relevamiento cerrado, idempotencia), diagnóstico paso a paso por issue con códigos coincidentes con los CU/05, tabla de logs útiles y plantilla de reporte de bug con política de severidad. Derivado del catálogo de errores de 03, del contrato de 05 y de los ADR-03/06/07/08. |
