# DX — Experiencia de developer de geovial-api

**Proyecto:** geovial-api
**Documento:** dx-developer-experience_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** API DX Designer + Developer Advocate
**Variante:** DX

## 0. Superficie pública documentada

geovial-api es un backend REST cuyo contrato público es la superficie que consumen los proyectos hermanos de la solución. Esa superficie se compone de los recursos derivados de los 22 casos de uso de 02:

| Familia de recursos | Recursos / endpoints (vocabulario REST genérico) | CU origen |
| --- | --- | --- |
| Autenticación y sesión | Recurso de token (emisión, renovación, cierre) | CU-03 |
| Usuarios | Recurso de usuarios de la jerarquía; recurso de agentes de campo | CU-01, CU-02 |
| Relevamientos | Recurso de relevamientos (alta, baja, consulta); recurso de transición de estado | CU-04, CU-06 |
| Asignaciones | Recurso de asignación de agentes a relevamiento | CU-05 |
| Marcadores y observaciones | Recurso de marcadores; recurso de observaciones con fotos, comentarios y etiquetas; carga manual de fotos | CU-07, CU-08, CU-09 |
| Sincronización | Recurso de subida de cambios locales; recurso de bajada de actualizaciones | CU-10, CU-11 |
| Revisión y cierre | Recurso de consulta para revisión; recurso de conflictos; recurso de cierre | CU-12, CU-13, CU-14 |
| Portabilidad | Recurso de exportación; recurso de importación | CU-15, CU-16 |
| Configuración | Recurso de configuración de destino de almacenamiento | CU-17 |

Cinco comportamientos transversales atraviesan toda la superficie y no son recursos propios sino contratos uniformes: autorización por rol y alcance (CU-18), formato de error uniforme problem+json (CU-19), paginación y filtros (CU-20), idempotencia de operaciones no seguras (CU-21) y versionado del contrato (CU-22). El integrador los encuentra de la misma forma en cada recurso.

La referencia formal y exhaustiva de cada endpoint (rutas, formas de payload, descriptores) se produce en la categoría 10 (developer guide); este documento define la experiencia de integración, no el catálogo de firmas.

## 1. Audiencia developer

El consumidor de geovial-api es un developer integrador interno, no un developer externo de un portal público. Concretamente, los equipos de los dos proyectos hermanos de la solución (intake §14):

| Perfil | Tipo de developer | Qué integra | Qué ya conoce |
| --- | --- | --- | --- |
| Integrador del front web (geovial-web) | Integrador | Creación, asignación, revisión sobre mapa y cierre de relevamientos | Consumo de APIs REST autenticadas, manejo de token bearer, paginación de listados |
| Integrador de la app móvil (geovial-mobile) | Integrador | Captura en terreno offline-first y ciclo de sincronización subir-antes-de-bajar | Consumo de APIs REST, reintento ante cortes de red, idempotencia de escrituras |

Nivel de experiencia esperado: developer con uno a tres años de práctica en consumo de APIs REST. Conoce los conceptos de método, código de estado, encabezado y cuerpo JSON, y el patrón de token bearer. No necesita conocer el dominio de relevamiento vial de antemano: el onboarding lo introduce.

No hay portal de developers externo ni audiencia de integradores de terceros (intake: tiene_portal_developers=false). La distribución del conocimiento de la API es interna a la solución; por eso la experiencia se optimiza para arranque rápido y para que el integrador encuentre el comportamiento transversal una sola vez y lo reuse en todos los recursos.

No son audiencia de este documento el usuario final (raíz, jefe general, jefe de área, agente de campo): esos roles consumen las apps cliente, no la API directamente. Su experiencia se diseña en las categorías 03 de geovial-web y geovial-mobile.

## 2. Onboarding por tramos

Cada tramo cierra con un hito verificable: el integrador sabe que lo logró porque obtiene un resultado observable concreto.

| Tramo | Objetivo verificable |
| --- | --- |
| 5 minutos | Obtener un token bearer válido por el flujo de autenticación por credenciales entregando credenciales de un usuario de prueba, y comprobar que el token porta un rol. Hito: la respuesta de autenticación devuelve un token y un código de estado de éxito. |
| 30 minutos | Hacer el primer request autenticado a un recurso de lectura dentro del alcance del rol (por ejemplo, listar los relevamientos visibles), provocar deliberadamente un error de autorización para ver el cuerpo problem+json, y leer su código estable. Hito: se obtiene una página de resultados y, por separado, un cuerpo de error con un código de la solución (por ejemplo FUERA_DE_ALCANCE). |
| 1 hora | Recorrer un listado paginado de punta a punta siguiendo las referencias de página, y completar una escritura no segura con clave de idempotencia reintentándola para confirmar que no se duplica el efecto. Hito: el reintento con la misma clave devuelve el mismo resultado sin crear un segundo recurso. |

Los tres tramos cubren las cuatro tareas fundacionales del integrador: autenticarse, leer dentro del alcance, interpretar un error y escribir de forma reintentable. El detalle paso a paso de la primera hora vive en `guia-onboarding-developer_v1.0.md`.

## 3. Quick-start

Objetivo del quick-start: autenticarse y llamar a un endpoint de lectura, produciendo el primer resultado exitoso. Descrito en pasos y comportamiento, con vocabulario REST genérico; el código ejecutable real vive en la categoría 11 y el stack en 05/09.

Pasos:

1. Conseguir credenciales de un usuario de prueba con un rol conocido (por ejemplo, un jefe de área de un área de prueba). El alta de usuarios la realiza un rol superior; el integrador no se autorregistra (no existe auto-registro: vision §4).
2. Solicitar un token al recurso de autenticación entregando identificador de acceso y credencial por el flujo de autenticación por credenciales. Comportamiento esperado: código de estado de éxito y un cuerpo que incluye el token bearer; el token porta el rol y tiene vigencia limitada (CU-03).
3. Tomar el token y enviarlo como credencial bearer en el encabezado de autorización de un request de lectura a un recurso dentro del alcance del rol, indicando la versión mayor vigente del contrato en la ruta (CU-22).
4. Comprobar el primer resultado exitoso: el recurso de lectura responde con un código de estado de éxito y un cuerpo con los datos paginados acotados al alcance del solicitante (CU-20).

Comportamiento de verificación del quick-start:

- Si el paso 2 falla con código CREDENCIALES_INVALIDAS, las credenciales no coinciden; revisar identificador y credencial del usuario de prueba.
- Si el paso 3 falla con código NO_AUTENTICADO, el token no viajó o no es legítimo; revisar que el encabezado bearer lleve el token emitido en el paso 2.
- Si el paso 3 falla con código FUERA_DE_ALCANCE o ACCION_NO_PERMITIDA, el rol del usuario de prueba no alcanza ese recurso; elegir un recurso dentro de su ámbito.

El quick-start se valida a mano contra un entorno de prueba antes de cada publicación de la guía, para sostener el criterio de quick-start verificable (03_rules §6 y §4.4).

## 4. Diátaxis

Plan de los cuatro modos de documentación para la API. Cada modo tiene una ubicación y un enlace explícito a los demás. La referencia formal vive en 10; los modos orientados a integración nacen aquí en 03 y se enlazan downstream.

| Modo | Orientación | Ubicación | Propósito | Enlaza a |
| --- | --- | --- | --- | --- |
| Tutorial | Aprendizaje | `guia-onboarding-developer_v1.0.md` (03) y su continuación en 10 | Llevar al integrador de cero a su primera hora productiva: token, primer request, primer error, primera paginación | How-to y reference |
| How-to | Tarea | Guías de tarea en 10 (por familia de recursos) | Resolver tareas concretas: paginar un listado largo, reintentar una escritura con clave de idempotencia, ejecutar el ciclo subir-antes-de-bajar, exportar e importar un relevamiento | Reference y explanation |
| Reference | Información | Developer guide en 10 (catálogo de endpoints y catálogo de errores) | Consultar cada recurso, método, código de estado y el catálogo completo de códigos de error; el catálogo de errores nace en `dx-error-messages_v1.0.md` (03) | Tutorial y how-to |
| Explanation | Comprensión | Notas de diseño en 10, ancladas a las RN de 02 | Comprender por qué la sincronización sube antes de bajar (RN-06), por qué los conflictos de marcadores conviven y se resuelven al cierre (RN-03), por qué cada operación no segura admite clave de idempotencia (RN-07) y cómo evoluciona el contrato versionado (CU-22) | Reference y how-to |

Regla de separación: el tutorial no documenta cada endpoint (eso es reference) y la reference no enseña el recorrido completo (eso es tutorial). Los cuatro modos se enlazan entre sí pero no se mezclan, para evitar el anti-patrón de documentación mezclada (03_rules §4.4).

## 5. Mensajes de error y diagnóstico

Principio de redacción de cada error de la API: decir qué pasó, por qué pasó y qué hacer al respecto. El backend devuelve todo error con un formato de problema uniforme, problem+json, que el integrador trata de manera homogénea en todos los recursos (CU-19).

Estructura del problema que recibe el integrador:

- Un código estable de la solución, opaco al idioma, sobre el que el cliente decide su tratamiento (por ejemplo RELEVAMIENTO_CERRADO). El cliente decide por el código, nunca por el texto.
- Un mensaje legible para mostrar al usuario final de la app cliente.
- El código de estado de la respuesta acorde a la naturaleza del fallo: solicitud inválida, no autorizado, prohibido, no encontrado, conflicto o error interno.
- Datos de contexto cuando aportan: el campo o el recurso implicado. Ante varios campos inválidos, un único problema enumera cada campo con su motivo (CU-19, flujo 5.A).

El catálogo completo de códigos, con causa y acción accionable por cada uno, vive en `dx-error-messages_v1.0.md`. El integrador no debería inventar manejo por código de estado solo: el código estable es la clave de decisión.

## 6. Métricas DX

| Métrica | Definición | Objetivo | Cómo se mide |
| --- | --- | --- | --- |
| TTFS | Tiempo desde que el integrador tiene credenciales hasta el primer resultado exitoso (token + primer request de lectura) | <= 5 minutos | Pruebas de onboarding con los integradores de geovial-web y geovial-mobile siguiendo el quick-start |
| TTFV | Tiempo hasta el primer valor de integración: completar una escritura idempotente y recorrer un listado paginado | <= 1 hora | Observación del recorrido de primera hora con cada equipo integrador hermano |
| Tasa de error de onboarding | Porcentaje de intentos de onboarding que no alcanzan el primer éxito sin ayuda externa | <= 20 % | Registro de los tramos completados en las sesiones de onboarding con los equipos hermanos |
| Claridad del error | Porcentaje de errores que el integrador resuelve solo con el código estable y la acción del catálogo, sin escalar | >= 90 % | Revisión de los reportes de integración y de las consultas escaladas al equipo de geovial-api |

Las métricas se miden con los dos equipos integradores internos, no con telemetría de un portal público (no aplica: tiene_portal_developers=false). No se recoge telemetría sin consentimiento.

## 7. Feedback loop

Al ser una API de consumo interno entre proyectos hermanos, el feedback del integrador se recoge y se incorpora así:

- Canal de incidencias del repositorio de la solución, con una etiqueta de experiencia de integración, para reportar contratos confusos, errores poco accionables o pasos del onboarding que fallan.
- Revisión de contrato entre el equipo de geovial-api y los equipos de geovial-web y geovial-mobile antes de publicar cada versión mayor del contrato (CU-22), de modo que los breaking changes se acuerden y no rompan a los consumidores por sorpresa.
- Registro de los códigos de error que más se escalan, para reescribir su acción sugerida en `dx-error-messages` cuando un código resulte poco accionable.
- Encuesta breve a los integradores tras su primera integración completa de un recurso, para ajustar el quick-start y el onboarding.

El ciclo de mejora prioriza primero los errores que rompen el onboarding (tramo de 5 minutos), luego los que afectan el primer valor (tramo de 1 hora) y por último las mejoras de comprensión (modo explanation de Diátaxis).

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Audiencia / persona objetivo | Developer integrador interno de geovial-web y geovial-mobile (00 vision §2; intake §14) |
| Superficie pública documentada | Recursos de los CU-01 a CU-17 y contratos transversales CU-18 a CU-22 (02) |
| CU origen | CU-01 a CU-22 (02) |
| Reglas de negocio relevantes | RN-01 (alcance), RN-03 (conflictos), RN-05 (estados), RN-06 (orden de sync), RN-07 (idempotencia) |
| US a generar | US-05, US-06 (sesión), US-37 a US-44 (transversales: autorización, errores, paginación, idempotencia, versionado), en 06 |
| Tests previstos | Quick-start verificable de autenticación y lectura; recorrido de paginación; reintento idempotente; lectura de error problem+json (referencia tentativa a 08; test de contrato de endpoints por intake §17.P.6) |
| Documentos relacionados de la sección | `guia-onboarding-developer_v1.0.md`, `dx-error-messages_v1.0.md` |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Marco DX inicial de geovial-api: audiencia integradora interna (web y móvil), onboarding por tramos 5/30/60 verificables, quick-start de autenticación y lectura, plan Diátaxis de los cuatro modos, principios de error problem+json, métricas DX, feedback loop entre proyectos hermanos y trazabilidad a CU-01 a CU-22. |
| 1.0 | 2026-06-15 | Corrección D7: se reemplaza vocabulario de protocolo (ROPC/JWT) por términos genéricos permitidos, sin cambio semántico. |
