# Especificación funcional — geovial-web

**Proyecto:** geovial-web
**Documento:** especificacion-funcional_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional

## 1. Propósito

Este documento es el índice maestro de la especificación funcional de `geovial-web`, el front web de la solución GeoVial y herramienta de los roles administradores (usuario raíz, jefe general y jefe de área). El proyecto es del tipo `web-monolith`: cada caso de uso (CU) describe un flujo de experiencia que cruza la presentación y el dominio consumido por contrato a `geovial-api`, sin invadir el detalle de interfaz fina (categoría 03) ni el contrato de la API (proyecto `geovial-api`). La especificación define el qué del front; el cómo (stack, componentes concretos, tipos físicos) vive en la categoría 05.

`geovial-web` cubre el lado de experiencia de cinco necesidades de negocio de la solución: la administración jerárquica de usuarios y el ingreso (NB-01), la gestión y asignación de relevamientos (NB-02), la revisión sobre mapa y el cierre con resolución de conflictos (NB-05), la portabilidad del relevamiento (NB-06) y el almacenamiento de archivos configurable (NB-07). No tiene persistencia propia: el estado de dominio vive en `geovial-api` (intake §17 geovial-web P.4).

## 2. Alcance funcional cubierto

El alcance es la experiencia del usuario administrador en el front web: ingresar y cerrar sesión; administrar usuarios por jerarquía; crear, editar y listar relevamientos; asignar y reasignar agentes; crear marcadores iniciales sobre el mapa; revisar la evidencia sobre mapa con carrusel de fotos; resolver conflictos de marcadores al cierre; transicionar el estado del relevamiento y cerrarlo; cargar manualmente un relevamiento completo por el agente vía web; y, como capacidades Could Have, exportar e importar un relevamiento completo y configurar el destino de almacenamiento.

Por tratarse de un `web-monolith`, se produce el modelo conceptual de datos (02 §2.2). El modelo de geovial-web es una vista de consumo del modelo AUTORITATIVO de `geovial-api`: el front no posee invariantes de integridad propias, por lo que no se generan reglas conceptuales de modelo (RC). Se incorpora en los CU la sección opcional §13 de interacción multiusuario y concurrencia (02 §4.3), por la naturaleza `web-monolith` del proyecto.

Quedan fuera de esta especificación, por las exclusiones de alcance (alcance-proyecto §5): la captura en terreno offline-first (flujo de la aplicación de campo), el auto-registro de agentes, el análisis automático de imágenes y el ruteo o navegación asistida. No originan CU en el front web.

## 3. Catálogo de casos de uso

| CU | Nombre | Actor primario | NB | Estado |
| --- | --- | --- | --- | --- |
| CU-01 | Iniciar y cerrar sesión en el front web | Usuario administrador | NB-01 | Propuesto |
| CU-02 | Administrar usuarios por jerarquía desde el front web | Usuario administrador | NB-01 | Propuesto |
| CU-03 | Crear, editar y listar relevamientos | Jefe de área | NB-02 | Propuesto |
| CU-04 | Asignar y reasignar agentes a un relevamiento | Jefe de área | NB-02 | Propuesto |
| CU-05 | Crear marcadores geográficos iniciales sobre el mapa | Jefe de área | NB-02 | Propuesto |
| CU-06 | Revisar el relevamiento sobre el mapa con carrusel de fotos | Jefe de área | NB-05 | Propuesto |
| CU-07 | Resolver conflictos de marcadores al cierre | Jefe de área | NB-05 | Propuesto |
| CU-08 | Transicionar el estado del relevamiento y cerrarlo | Jefe de área | NB-05 | Propuesto |
| CU-09 | Cargar manualmente un relevamiento completo vía web | Agente de campo | NB-02 | Propuesto |
| CU-10 | Exportar e importar un relevamiento completo | Jefe de área o usuario raíz | NB-06 | Propuesto |
| CU-11 | Configurar el destino de almacenamiento de archivos | Usuario raíz | NB-07 | Propuesto |

Total: 11 CU. El mínimo del tipo `web-monolith` (8 CU, 02 §2.2) se cumple y se supera por la cobertura de las cinco necesidades de negocio que el front cubre.

## 4. Catálogo de reglas de negocio

| RN | Nombre | Invariante (resumen) | CU afectados |
| --- | --- | --- | --- |
| RN-01 | Visibilidad y acciones por rol jerárquico en el front web | El front presenta solo pantallas y acciones del alcance del rol | CU-02, CU-03, CU-04, CU-05, CU-06, CU-07, CU-08, CU-09, CU-10, CU-11 |
| RN-02 | Conservación de la traza de autoría al dar de baja | La baja inhabilita el acceso y conserva la autoría visible | CU-02, CU-06 |
| RN-03 | Acceso al front web restringido a roles administradores | El front es de administradores; el agente solo entra a la carga manual | CU-01, CU-09 |
| RN-04 | Estados visibles del relevamiento y habilitación de acciones | El front habilita solo las acciones válidas para el estado vigente | CU-03, CU-05, CU-06, CU-07, CU-08, CU-09 |
| RN-05 | Resolución de conflictos como precondición visible del cierre | No se ofrece el cierre con conflictos pendientes; conviven sin bloquear | CU-06, CU-07, CU-08 |

Las cinco RN del front derivan de las RN del backend autoritativo (RN-01, RN-02, RN-03, RN-04, RN-05 de `geovial-api`) y las traducen a condiciones de presentación y flujo; no redefinen el dominio.

## 5. Modelo conceptual

El modelo conceptual (`modelo-datos/modelo-conceptual_v1.0.md`) es una vista de consumo del modelo AUTORITATIVO de `geovial-api`. Enumera las entidades que la interfaz presenta y manipula (Usuario, Rol, Relevamiento, TramoVial, Asignacion, MarcadorGeografico, ConflictoMarcadores, Observacion, Foto, Comentario, Etiqueta) más la proyección DestinoAlmacenamiento de la configuración de almacenamiento. El front no posee persistencia ni invariantes de integridad propias, por lo que no se generan reglas conceptuales de modelo (RC): la integridad la garantizan las RC-01 a RC-06 del modelo de `geovial-api`.

## 6. Matriz de trazabilidad NB → CU → RN → US

| NB upstream | CU | RN aplicables (geovial-web) | US a generar (en 06) |
| --- | --- | --- | --- |
| NB-01 | CU-01 Iniciar y cerrar sesión en el front web | RN-01, RN-03 | US-01, US-02 |
| NB-01 | CU-02 Administrar usuarios por jerarquía | RN-01, RN-02 | US-03, US-04, US-05 |
| NB-02 | CU-03 Crear, editar y listar relevamientos | RN-01, RN-04 | US-06, US-07, US-08 |
| NB-02 | CU-04 Asignar y reasignar agentes | RN-01, RN-04 | US-09, US-10 |
| NB-02 | CU-05 Crear marcadores iniciales sobre el mapa | RN-01, RN-02, RN-04 | US-11, US-12 |
| NB-05 | CU-06 Revisar sobre mapa con carrusel | RN-01, RN-02, RN-04 | US-13, US-14, US-15 |
| NB-05 | CU-07 Resolver conflictos al cierre | RN-05, RN-01 | US-16, US-17 |
| NB-05 | CU-08 Transicionar estado y cerrar | RN-05, RN-01 | US-18, US-19 |
| NB-02 | CU-09 Carga manual completa vía web | RN-04, RN-01 | US-20, US-21 |
| NB-06 | CU-10 Exportar e importar relevamiento | RN-01 | US-22, US-23 |
| NB-07 | CU-11 Configurar destino de almacenamiento | RN-01 | US-24, US-25 |

Cobertura bidireccional sobre las NB que el front cubre. Cada NB de las cinco (NB-01, NB-02, NB-05, NB-06, NB-07) tiene al menos un CU; cada CU declara al menos una NB. No hay CU huérfano. Las NB-03 (captura georreferenciada) y NB-04 (trabajo sin conexión y sincronización) corresponden al lado de la aplicación de campo y del backend; el front solo las toca de forma acotada en la carga manual (CU-09), anclada a NB-02.

## 7. Relación con el proyecto autoritativo geovial-api

La numeración de CU y RN es propia de `geovial-web` y no coincide con la de `geovial-api`. Las correspondencias de consumo son: CU-01 del front consume el recurso de autenticación (CU-03 de la API); CU-02 los recursos de usuarios (CU-01, CU-02 de la API); CU-03 a CU-05 los de relevamientos, asignaciones y marcadores (CU-04, CU-05, CU-07 de la API); CU-06 la consulta de revisión (CU-12 de la API); CU-07 la resolución de conflictos (CU-13 de la API); CU-08 la transición y cierre (CU-06, CU-14 de la API); CU-09 la carga manual (CU-09 de la API); CU-10 la portabilidad (CU-15, CU-16 de la API); y CU-11 la configuración de almacenamiento (CU-17 de la API). El front consume; no redefine el contrato ni el dominio.

## 8. Decisiones de recorte (02 §5.2)

- El ingreso y el cierre de sesión se especifican en un único CU (CU-01) porque comparten actor y flujo continuo de la sesión web; el relogueo por seguridad del dispositivo no aparece aquí por ser un flujo de la aplicación de campo (RN-03).
- La administración de usuarios por jerarquía se unifica en un CU (CU-02) en lugar de uno por nivel, porque el flujo es el mismo y solo cambia el alcance que el backend resuelve; separarlo diluiría el actor primario único.
- La transición de estado y el cierre se unen en CU-08 porque forman un mismo recorrido de ciclo del relevamiento, con el cierre como hito final condicionado a la resolución de conflictos (CU-07).
- La exportación y la importación se unen en CU-10 por ser las dos caras de la portabilidad y compartir actor; ambas son Could Have (NB-06).
- La carga manual del agente vía web (CU-09) se ancla a NB-02 por ser el agente poblando un relevamiento asignado desde el front; la semántica de priorización de ubicación y radio la gobierna el backend (RN-04 de geovial-api), reflejada en RN-04 del front.

## 9. Ambigüedades y supuestos abiertos (02 §5, master-prompt §9)

El front no introduce supuestos de dominio nuevos: hereda los del proyecto autoritativo, marcados en `geovial-api` 02 §9, sin redefinirlos.

- Conflictos entre cambios de dos agentes sobre el mismo relevamiento o marcador (intake §7): el front asume la misma política de convivencia y resolución al cierre que para los conflictos por radio, reflejada en CU-06 y CU-07. A confirmar con el negocio.
- Cierre de un relevamiento mientras un agente tiene cambios locales sin sincronizar (intake §7): el front asume que el backend bloquea nuevas subidas al cerrar y reporta el estado cerrado, reflejado en la nota de CU-08. A confirmar.
- Foto sin datos de ubicación incrustados en la carga manual (intake §7): el front la presenta como pendiente de ubicación manual sin inventarle coordenada, según la regla del backend, reflejado en CU-09. A confirmar.

Ninguno de estos supuestos bloquea la especificación del front; se resuelven en el proyecto autoritativo al confirmar el negocio, sin alterar la estructura de los CU del front.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice maestro inicial de la especificación funcional de geovial-web: 11 CU de flujos de experiencia, 5 RN de presentación y flujo, modelo conceptual como vista de consumo (sin RC propias) y matriz de trazabilidad NB→CU→RN→US sobre NB-01, NB-02, NB-05, NB-06 y NB-07. |
