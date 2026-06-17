# Glosario técnico — geovial-api

**Proyecto:** geovial-api
**Documento:** glosario-tecnico_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Technical Writer + API Documentation Lead
**Tipo Diátaxis:** Reference
**Audiencia:** Developer consumidor de la API HTTP (equipos de geovial-web y geovial-mobile, integradores internos)
**Nivel:** Básico
**Tiempo estimado de lectura:** 9 min

Vocabulario canónico del consumidor de la API de geovial-api. Es la fuente única de términos: el resto de los documentos de esta categoría enlaza a este glosario en lugar de redefinir. Cada término se nombra en kebab-case, lleva una definición operativa de una a tres oraciones y una referencia cruzada al documento donde se desarrolla.

## 1. Términos del consumidor de la API

| Término | Definición operativa | Referencia cross-doc |
| --- | --- | --- |
| `token-bearer` | Credencial opaca que el backend emite a partir de credenciales y que el cliente presenta en el encabezado de autorización de cada request salvo el inicio de sesión; porta el rol del portador y tiene vigencia limitada. | `conceptos-fundamentales_v1.0.md` §2.3; `referencia-api_v1.0.md` §5 |
| `endpoint` | Combinación de método y ruta versionada que expone una operación del contrato (por ejemplo `POST /relevamientos`). | `referencia-api_v1.0.md` §2 |
| `rol` | Nivel del solicitante en la jerarquía de cuatro niveles que determina su alcance: `usuario-raiz`, `jefe-general`, `jefe-de-area` o `agente-de-campo`. | `conceptos-fundamentales_v1.0.md` §2.3 |
| `usuario-raiz` | Rol de mayor alcance: configura el destino de almacenamiento, da de alta al jefe general e importa relevamientos. | `conceptos-fundamentales_v1.0.md` §2.3 |
| `jefe-general` | Rol que administra a los jefes de área. | `conceptos-fundamentales_v1.0.md` §2.3 |
| `jefe-de-area` | Rol que da de alta y de baja agentes de su área, crea y asigna relevamientos, los revisa, resuelve conflictos y los cierra. | `conceptos-fundamentales_v1.0.md` §2.3; `referencia-api_v1.0.md` §2.3 |
| `agente-de-campo` | Rol que crea marcadores y observaciones, adjunta fotos y sincroniza, solo en relevamientos asignados. | `conceptos-fundamentales_v1.0.md` §2.3 |
| `ambito` | Conjunto de recursos que un solicitante puede operar según su rol y su posición jerárquica; operar fuera de él se rechaza. | `conceptos-fundamentales_v1.0.md` §2.3; `troubleshooting_v1.0.md` ISSUE-02 |
| `relevamiento` | Unidad de trabajo que registra observaciones del estado de un tramo vial y recorre el ciclo recolección, revisión y cierre. | `conceptos-fundamentales_v1.0.md` §2.1; `referencia-api_v1.0.md` §2.3 |
| `tramo-vial` | Alcance geográfico de un relevamiento, compuesto por una colección no vacía de puentes y caminos. | `conceptos-fundamentales_v1.0.md` §2.2; `referencia-api_v1.0.md` §3 |
| `asignacion` | Vínculo que habilita a un agente de campo a recolectar en un relevamiento; único por par vigente. | `referencia-api_v1.0.md` §2.4 |
| `marcador-geografico` | Punto del mapa, con una coordenada, que agrupa observaciones, fotos y etiquetas dentro de un relevamiento y conserva una identidad propia y estable. | `conceptos-fundamentales_v1.0.md` §2.2; `referencia-api_v1.0.md` §2.5 |
| `observacion` | Registro anclado a un marcador, con autor identificado, una nota y un conjunto de fotos. | `conceptos-fundamentales_v1.0.md` §2.2; `referencia-api_v1.0.md` §3 |
| `foto` | Imagen asociada a una observación; el recurso lleva una referencia lógica al almacén (no el binario), su ubicación, a lo sumo un comentario y etiquetas. | `referencia-api_v1.0.md` §3 |
| `etiqueta` | Marca aplicable a fotos y a marcadores para clasificarlos y filtrarlos en la revisión. | `conceptos-fundamentales_v1.0.md` §2.2 |
| `conflicto-marcadores` | Estado válido en que dos o más marcadores de un relevamiento caen dentro de un mismo radio; convive con la operación y se resuelve al cierre como unificación o separación. | `conceptos-fundamentales_v1.0.md` §2.5; `troubleshooting_v1.0.md` ISSUE-05 |
| `sincronizacion` | Ciclo de dos fases ordenadas que sube primero los cambios locales del agente y luego baja las novedades de sus relevamientos asignados. | `conceptos-fundamentales_v1.0.md` §2.4; `referencia-api_v1.0.md` §2.6 |
| `orden-subir-antes-de-bajar` | Garantía del ciclo de sincronización por la cual ninguna bajada se atiende antes de concluir la subida del mismo ciclo. | `conceptos-fundamentales_v1.0.md` §2.4; `guia-integracion-cliente-http_v1.0.md` §3 |
| `marca-de-sincronizacion` | Referencia opaca y monótona, por relevamiento y cliente, que registra el punto de sincronización para que la bajada entregue solo las novedades posteriores. | `referencia-api_v1.0.md` §3.1; `troubleshooting_v1.0.md` ISSUE-05 |
| `identificador-de-origen` | Identificador estable que porta cada cambio del lote de subida para que un reenvío tras un corte no duplique el cambio. | `referencia-api_v1.0.md` §6.2; `conceptos-fundamentales_v1.0.md` §2.4 |
| `clave-de-idempotencia` | Valor estable que el cliente adjunta en una cabecera dedicada a una operación no segura reintentable para que un reintento no duplique el efecto. | `referencia-api_v1.0.md` §6.2; `troubleshooting_v1.0.md` ISSUE-06 |
| `paginacion` | Contrato uniforme por el cual cada listado entrega una página con tamaño efectivo y referencias de navegación, aplicando el alcance antes de paginar. | `referencia-api_v1.0.md` §6.1; `guia-integracion-cliente-http_v1.0.md` §3 |
| `problem-json` | Formato uniforme de error (RFC 9457, que reemplaza RFC 7807) con un código estable, un mensaje legible, el estado de la respuesta y el contexto cuando aporta. | `referencia-api_v1.0.md` §7; `conceptos-fundamentales_v1.0.md` §2.6 |
| `codigo-estable` | Identificador en mayúsculas sin tildes, opaco al idioma, por el que el cliente decide el tratamiento de un error, nunca por el texto del mensaje. | `referencia-api_v1.0.md` §7; `troubleshooting_v1.0.md` §1 |
| `versionado-por-uri` | Política por la cual cada recurso cuelga de un prefijo de versión mayor en la ruta (`/v1`) y un cambio incompatible publica una versión mayor nueva conservando la anterior. | `referencia-api_v1.0.md` §6.3; `conceptos-fundamentales_v1.0.md` §2.6 |
| `unidad-transferible` | Representación lógica de un relevamiento completo (comentarios, etiquetas y fotos) que la exportación produce y la importación reconstruye. | `referencia-api_v1.0.md` §3 |
| `destino-de-almacenamiento` | Proveedor de alojamiento de los binarios de las fotos que el usuario raíz configura; el backend lo opera de forma transparente vía una librería. | `referencia-api_v1.0.md` §2.9; `conceptos-fundamentales_v1.0.md` §5 |
| `autoria` | Atribución permanente de un registro al usuario que lo creó, conservada aunque ese usuario sea dado de baja. | `conceptos-fundamentales_v1.0.md` §2.3 |

## 2. Referencias cruzadas

- 05 `contratos-rest_v1.0.md` §4: esquemas lógicos que dan origen a los términos de datos (relevamiento, marcador, observación, foto, conflicto, lote de sincronización, página, problema).
- 02 `modelo-conceptual_v1.0.md` §1, §6: entidades del dominio y su glosario de origen.
- 03 `dx-error-messages_v1.0.md`: vocabulario de errores (código estable, taxonomía).
- `conceptos-fundamentales_v1.0.md`, `referencia-api_v1.0.md`, `guia-integracion-cliente-http_v1.0.md`, `troubleshooting_v1.0.md`: documentos que enlazan a este glosario.

## 3. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Glosario inicial del consumidor de geovial-api con 28 términos canónicos en kebab-case, definición operativa y referencia cruzada por término, alineado al contrato de 05 y al modelo conceptual de 02. Vocabulario REST genérico, sin productos del stack ni jerga interna del equipo (D7). |
