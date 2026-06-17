# Guía de onboarding del developer integrador — geovial-api

**Proyecto:** geovial-api
**Documento:** guia-onboarding-developer_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** API DX Designer + Developer Advocate
**Variante:** DX

## 0. Superficie pública que cubre esta guía

Esta guía recorre la primera hora del integrador sobre el contrato REST de geovial-api: el recurso de autenticación (CU-03), un recurso de lectura paginado (CU-12, CU-04 con CU-20), el formato de error uniforme problem+json (CU-19, CU-18) y una escritura no segura con clave de idempotencia (CU-21). Es el modo tutorial del plan Diátaxis definido en `dx-developer-experience_v1.0.md` §4. Se describe en pasos y comportamiento, con vocabulario REST genérico; el código ejecutable real vive en la categoría 11 y el stack en 05/09.

## 1. Audiencia y prerrequisitos

Audiencia: developer integrador de geovial-web o de geovial-mobile que va a consumir el contrato REST de geovial-api por primera vez (vision §2; intake §14). Se asume familiaridad con consumo de APIs REST, token bearer y códigos de estado HTTP; no se asume conocimiento previo del dominio de relevamiento vial.

Prerrequisitos verificables antes de empezar:

- Acceso al entorno de prueba donde corre geovial-api (la dirección base la provee el equipo de geovial-api).
- Credenciales de un usuario de prueba con un rol conocido. El alta de usuarios la hace un rol superior de la jerarquía; no hay auto-registro (vision §4). Para el recorrido completo conviene un usuario con rol de jefe de área, que puede leer relevamientos y realizar escrituras.
- Un cliente HTTP cualquiera capaz de enviar requests con encabezados y cuerpo JSON y de leer el código de estado y el cuerpo de la respuesta.

Hito de la sección: el integrador tiene la dirección base del entorno y un par de credenciales de prueba a mano.

## 2. Acceso: obtener un token por el flujo de autenticación por credenciales

El acceso a la API no se instala: se obtiene un token. geovial-api autentica por el flujo de autenticación por credenciales y emite un token bearer que el propio backend valida (CU-03; intake §17.P.5).

Pasos:

1. Enviar un request al recurso de autenticación entregando el identificador de acceso y la credencial del usuario de prueba, indicando la versión mayor vigente del contrato en la ruta (CU-22).
2. Leer la respuesta. Comportamiento esperado: código de estado de éxito y un cuerpo que contiene el token bearer. El token porta el rol del usuario y tiene vigencia limitada.
3. Guardar el token para usarlo como credencial bearer en los requests siguientes.

Hito verificable: la respuesta de autenticación trae un token y un código de estado de éxito.

Si falla:

- Código CREDENCIALES_INVALIDAS con estado de no autorizado: el identificador o la credencial no coinciden. Revisar el par de credenciales de prueba.
- Código USUARIO_INHABILITADO con estado de no autorizado: el usuario de prueba fue dado de baja; pedir al equipo un usuario habilitado.

## 3. Primer ejemplo: primer request autenticado a un recurso de lectura

Con el token en mano, el integrador hace su primer request autenticado a un recurso de lectura dentro del alcance de su rol y obtiene un resultado visible.

Pasos:

1. Elegir un recurso de lectura acorde al rol: por ejemplo, el listado de relevamientos visibles para un jefe de área (CU-04, CU-12).
2. Enviar el request incluyendo el token como credencial bearer en el encabezado de autorización y la versión mayor del contrato en la ruta.
3. Leer la respuesta. Comportamiento esperado: código de estado de éxito y un cuerpo con la primera página de resultados, acotada al alcance del solicitante (CU-20, RN-01). Nunca se devuelve el conjunto completo sin paginar.

Hito verificable: se obtiene una página de resultados con datos del ámbito del usuario de prueba.

Si falla:

- Código NO_AUTENTICADO con estado de no autorizado: el token no viajó o no es legítimo. Revisar que el encabezado bearer lleve el token del paso anterior.
- Código FUERA_DE_ALCANCE o ACCION_NO_PERMITIDA con estado de prohibido: el rol no alcanza ese recurso o ese ámbito. Elegir un recurso dentro del alcance del rol (CU-18, RN-01).

## 4. Diagnóstico de problemas frecuentes en la primera hora

Todo error llega con el formato uniforme problem+json: un código estable, un mensaje legible, el estado de la respuesta y el contexto cuando aporta (CU-19). El integrador decide por el código, no por el texto.

| Síntoma en la primera hora | Código probable | Causa | Qué hacer |
| --- | --- | --- | --- |
| El request de lectura se rechaza sin token | NO_AUTENTICADO | No viajó token o no es legítimo | Adjuntar el token bearer obtenido en la sección 2 |
| El request se rechaza pese a llevar token | ACCION_NO_PERMITIDA | El rol no habilita esa acción | Usar un usuario con el rol adecuado, o un recurso permitido para el rol actual |
| El request se rechaza por ámbito | FUERA_DE_ALCANCE | El recurso pertenece a otro ámbito | Operar solo recursos del propio ámbito del usuario (RN-01) |
| El listado rechaza un filtro | FILTRO_NO_SOPORTADO | El recurso no admite ese filtro | Leer los filtros válidos que informa la respuesta y reintentar (CU-20) |
| El listado rechaza una página | POSICION_INVALIDA | La posición de página pedida no es válida | Empezar por la primera página y seguir las referencias de página |
| La escritura se rechaza pidiendo clave | CLAVE_REQUERIDA_AUSENTE | La operación no segura exige clave de idempotencia | Adjuntar una clave de idempotencia estable (sección 5) |
| El token deja de funcionar | TOKEN_REVOCADO | La sesión se cerró por completo | Volver a autenticarse y obtener un token nuevo (CU-03) |

Ejercicio guiado de paginación: pedir la primera página de un listado con un tamaño de página chico, leer la referencia a la página siguiente que trae la respuesta, y seguir esa referencia hasta agotar el conjunto. Si se pide un tamaño mayor al máximo, el backend lo acota al máximo y lo informa sin rechazar (CU-20, flujo 5.A). Hito verificable: se recorre el listado de punta a punta siguiendo las referencias de página.

Ejercicio guiado de escritura idempotente: realizar una escritura no segura (por ejemplo, un alta) adjuntando una clave de idempotencia estable; luego reenviar exactamente la misma operación con la misma clave, simulando un reintento tras una respuesta no recibida. Comportamiento esperado: el recurso se crea una sola vez y el reintento devuelve el mismo resultado, sin duplicar (CU-21, RN-07). Si se reutiliza la misma clave con un contenido distinto, el backend rechaza con CLAVE_REUTILIZADA_INCONSISTENTE y estado de conflicto. Hito verificable: el reintento no crea un segundo recurso.

## 5. Próximos pasos

Enlaces a los modos del plan Diátaxis (`dx-developer-experience_v1.0.md` §4):

- Tutorial (continuación): completar el ciclo de un recurso de escritura de punta a punta. Continúa en la developer guide de la categoría 10.
- How-to (tarea): guías por tarea en 10. Las más útiles tras esta guía son: recorrer el ciclo de sincronización subir-antes-de-bajar (CU-10, CU-11, RN-06), resolver un error problem+json por su código, y exportar e importar un relevamiento completo (CU-15, CU-16).
- Reference (información): catálogo de endpoints y catálogo de errores en 10; el catálogo de errores accionable nace en `dx-error-messages_v1.0.md` de esta sección.
- Explanation (comprensión): notas de diseño en 10 sobre por qué la sincronización ordena subir antes de bajar (RN-06), por qué los conflictos de marcadores conviven y se resuelven al cierre (RN-03, CU-13, CU-14) y cómo evoluciona el contrato versionado sin romper a los consumidores (CU-22).

Trazabilidad de la guía:

| Dimensión | Referencia |
| --- | --- |
| Audiencia / persona objetivo | Developer integrador interno de geovial-web y geovial-mobile (00 vision §2; intake §14) |
| Superficie pública documentada | Recurso de autenticación (CU-03), recurso de lectura paginado (CU-04, CU-12, CU-20), error uniforme (CU-19, CU-18), escritura idempotente (CU-21) |
| CU origen | CU-03, CU-04, CU-12, CU-18, CU-19, CU-20, CU-21, CU-22 |
| Reglas de negocio relevantes | RN-01 (alcance), RN-06 (orden de sync), RN-07 (idempotencia) |
| US a generar | US-05, US-06 (sesión), US-37, US-38 (autorización), US-40, US-41 (paginación), US-42, US-43 (idempotencia), en 06 |
| Tests previstos | Onboarding del quick-start verificable; recorrido de paginación; reintento idempotente sin duplicar; lectura de error por código estable (referencia tentativa a 08) |

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Guía inicial de primera hora del integrador de geovial-api: obtener token por el flujo de autenticación por credenciales, primer request autenticado, diagnóstico de errores problem+json frecuentes, paginar un listado y completar una escritura idempotente, con enlaces a los cuatro modos Diátaxis. |
| 1.0 | 2026-06-15 | Corrección D7: se reemplaza vocabulario de protocolo (ROPC/JWT) por términos genéricos permitidos, sin cambio semántico. |
