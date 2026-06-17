# Especificación funcional — geovial-api

**Proyecto:** geovial-api
**Documento:** especificacion-funcional_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Este documento es el índice maestro de la especificación funcional de `geovial-api`, el backend monolítico y proyecto principal de la solución GeoVial. El proyecto es del tipo `rest-api`: cada recurso público se especifica como uno o más casos de uso (CU) con su contrato declarativo, sus códigos de error y sus criterios de aceptación, más un conjunto de CU transversales comunes a toda la superficie REST. La especificación define el qué del contrato; el cómo (tipos físicos, interfaces concretas y stack) vive en la categoría 05.

`geovial-api` implementa el lado servidor de las siete necesidades de negocio de la solución (NB-01 a NB-07): la jerarquía de usuarios y el control de acceso, la gestión y asignación de relevamientos, la captura georreferenciada de observaciones, la sincronización del trabajo sin conexión, la revisión sobre mapa y el cierre con resolución de conflictos, la portabilidad del relevamiento y el almacenamiento de archivos configurable.

## 2. Alcance funcional cubierto

El alcance de esta especificación es el contrato público del backend: los recursos de usuarios y sesión, de relevamientos y su ciclo, de asignaciones, de marcadores y observaciones con sus fotos, comentarios y etiquetas, de sincronización (subida y bajada), de portabilidad (exportación e importación) y de configuración de almacenamiento. A esto se suman cinco CU transversales propios del tipo `rest-api`: autorización por rol y alcance, manejo uniforme de errores, paginación y filtros de listados, idempotencia de operaciones no seguras y versionado del contrato público.

Por tratarse de un proyecto con persistencia (regla 02 §2.2), se produce el modelo conceptual de datos. Dado que el modelo supera las diez entidades, se acompañan reglas conceptuales de modelo (RC). Se incorporan, en los CU donde aplica, las secciones opcionales de performance del CU (02 §4.3 §12) y de idempotencia y reintento (02 §4.3 §15), por la naturaleza `rest-api` del proyecto.

Quedan fuera de esta especificación, por las exclusiones de alcance (alcance-proyecto §5) y por la prioridad Won't Have v1 (intake §4 F-18): el auto-registro o la solicitud self-service de agentes, el análisis automático de imágenes y el ruteo o navegación asistida. No originan CU.

## 3. Catálogo de casos de uso

### 3.1 Casos de uso de recursos públicos

| CU | Nombre | Recurso | Actor primario | NB | Estado |
| --- | --- | --- | --- | --- | --- |
| CU-01 | Administrar la jerarquía de usuarios en cuatro niveles | Usuarios | Administrador del nivel superior | NB-01 | Propuesto |
| CU-02 | Dar de alta y de baja agentes de campo por el jefe de área | Usuarios (agentes) | Jefe de área | NB-01 | Propuesto |
| CU-03 | Iniciar sesión, cerrar sesión completa y revalidar credenciales | Autenticación | Usuario de cualquier rol | NB-01 | Propuesto |
| CU-04 | Crear, dar de baja y visualizar relevamientos de un tramo vial | Relevamientos | Jefe de área | NB-02 | Propuesto |
| CU-05 | Asignar y reasignar agentes de campo a un relevamiento | Asignaciones | Jefe de área | NB-02 | Propuesto |
| CU-06 | Transicionar el estado del relevamiento de recolección a revisión | Estado del relevamiento | Jefe de área | NB-02 | Propuesto |
| CU-07 | Administrar marcadores geográficos del relevamiento | Marcadores | Agente de campo o jefe de área | NB-03 | Propuesto |
| CU-08 | Administrar observaciones con notas, fotos, comentarios y etiquetas | Observaciones | Agente de campo | NB-03 | Propuesto |
| CU-09 | Cargar fotos manualmente con priorización de ubicación y radio de agrupación | Carga manual | Agente de campo | NB-03 | Propuesto |
| CU-10 | Recibir el lote de cambios locales del agente (subida de sincronización) | Sincronización (subida) | Cliente de campo del agente | NB-04 | Propuesto |
| CU-11 | Entregar las actualizaciones del relevamiento asignado (bajada de sincronización) | Sincronización (bajada) | Cliente de campo del agente | NB-04 | Propuesto |
| CU-12 | Consultar el relevamiento para la revisión sobre mapa | Relevamiento (revisión) | Jefe de área | NB-05 | Propuesto |
| CU-13 | Resolver los conflictos de marcadores al cierre | Conflictos de marcadores | Jefe de área | NB-05 | Propuesto |
| CU-14 | Cerrar el relevamiento como hito que habilita el informe | Cierre del relevamiento | Jefe de área | NB-05 | Propuesto |
| CU-15 | Exportar un relevamiento completo en una unidad transferible única | Portabilidad (exportación) | Jefe de área | NB-06 | Propuesto |
| CU-16 | Importar un relevamiento completo reconstruyendo su estructura | Portabilidad (importación) | Jefe de área o usuario raíz | NB-06 | Propuesto |
| CU-17 | Configurar el destino de almacenamiento de archivos | Configuración de almacenamiento | Usuario raíz | NB-07 | Propuesto |

### 3.2 Casos de uso transversales (tipo rest-api, 02 §2.2)

| CU | Nombre | Naturaleza | Actor primario | Estado |
| --- | --- | --- | --- | --- |
| CU-18 | Autorizar el acceso a cada recurso según el rol y el alcance | Autorización por rol | Usuario de cualquier rol | Propuesto |
| CU-19 | Devolver errores con un formato de problema uniforme | Manejo de errores | Cliente consumidor | Propuesto |
| CU-20 | Paginar y filtrar los listados de recursos | Paginación y filtros | Cliente consumidor | Propuesto |
| CU-21 | Garantizar la idempotencia de las operaciones no seguras | Idempotencia | Cliente consumidor | Propuesto |
| CU-22 | Versionar el contrato público de la API | Versionado del contrato | Cliente consumidor | Propuesto |

Total: 17 CU de recursos públicos y 5 CU transversales, 22 CU. El mínimo del tipo `rest-api` (un CU por recurso público más cinco transversales) se cumple y se supera por la cobertura completa de NB-01 a NB-07.

## 4. Catálogo de reglas de negocio

| RN | Nombre | Invariante (resumen) | CU afectados |
| --- | --- | --- | --- |
| RN-01 | Jerarquía de altas, bajas y alcance | Cada nivel administra solo el inmediato inferior y opera solo su ámbito | CU-01, CU-02, CU-04, CU-05, CU-06, CU-07, CU-12, CU-13, CU-14, CU-15, CU-16, CU-17, CU-18, CU-20 |
| RN-02 | Conservación de la autoría histórica ante la baja | La baja revoca acceso pero conserva la autoría de lo registrado | CU-01, CU-02, CU-03, CU-18 |
| RN-03 | Convivencia con conflictos de marcadores y resolución al cierre | El conflicto convive sin bloquear y se resuelve al cierre | CU-07, CU-08, CU-10, CU-11, CU-12, CU-13, CU-14 |
| RN-04 | Priorización de la ubicación incrustada y radio de agrupación | La carga manual prioriza la ubicación de la foto y agrupa por radio | CU-07, CU-09 |
| RN-05 | Transición de estados del relevamiento | El relevamiento avanza recolección, revisión, cierre, sin saltos ni cierre con conflictos | CU-04, CU-05, CU-06, CU-13, CU-14 |
| RN-06 | Orden de sincronización subir antes de bajar | La bajada no se atiende hasta concluir la subida del ciclo | CU-10, CU-11, CU-21 |
| RN-07 | Idempotencia de la sincronización y de las escrituras reintentables | Un reenvío o reintento con la misma clave no duplica efectos | CU-01, CU-02, CU-04, CU-05, CU-06, CU-08, CU-09, CU-10, CU-13, CU-14, CU-16, CU-21 |

## 5. Modelo conceptual y reglas conceptuales

El modelo conceptual (`modelo-datos/modelo-conceptual_v1.0.md`) define 12 entidades del dominio: Usuario, Rol, Relevamiento, TramoVial, Asignacion, MarcadorGeografico, ConflictoMarcadores, Observacion, Foto, Comentario, Etiqueta y MarcaSincronizacion. Por superar las diez entidades (02 §2.2), se acompañan seis reglas conceptuales de modelo en `modelo-datos/reglas-conceptuales-de-modelo/`:

| RC | Nombre | Tipo de restricción |
| --- | --- | --- |
| RC-01 | Identidad estable del marcador geográfico | Identidad |
| RC-02 | Referencia obligatoria de observación a marcador | Referencial |
| RC-03 | Integridad de la jerarquía de usuarios | Referencial y cardinalidad |
| RC-04 | Estado del relevamiento dentro del ciclo válido | Valor permitido y derivación |
| RC-05 | Unicidad de la asignación agente-relevamiento | Identidad y cardinalidad |
| RC-06 | Monotonía de la marca de sincronización | Derivación y valor permitido |

## 6. Matriz de trazabilidad NB → CU → RN → US

| NB upstream | CU | RN aplicables | US a generar (en 06) |
| --- | --- | --- | --- |
| NB-01 | CU-01 Administrar la jerarquía de usuarios | RN-01, RN-02 | US-01, US-02 |
| NB-01 | CU-02 Dar de alta y de baja agentes de campo | RN-01, RN-02 | US-03, US-04 |
| NB-01 | CU-03 Iniciar y cerrar sesión y revalidar | RN-01, RN-02 | US-05, US-06 |
| NB-02 | CU-04 Crear, dar de baja y visualizar relevamientos | RN-01, RN-05 | US-07, US-08, US-09 |
| NB-02 | CU-05 Asignar y reasignar agentes | RN-01, RN-05 | US-10, US-11 |
| NB-02 | CU-06 Transicionar el estado del relevamiento | RN-01, RN-05 | US-12, US-13 |
| NB-03 | CU-07 Administrar marcadores geográficos | RN-03, RN-04 | US-14, US-15 |
| NB-03 | CU-08 Administrar observaciones | RN-03, RN-04 | US-16, US-17, US-18 |
| NB-03 | CU-09 Cargar fotos manualmente | RN-04, RN-03 | US-19, US-20 |
| NB-04 | CU-10 Recibir cambios locales (subida) | RN-03, RN-06, RN-07 | US-21, US-22 |
| NB-04 | CU-11 Entregar actualizaciones (bajada) | RN-03, RN-06 | US-23, US-24 |
| NB-05 | CU-12 Consultar el relevamiento para revisión | RN-01, RN-03 | US-25, US-26 |
| NB-05 | CU-13 Resolver conflictos de marcadores al cierre | RN-03, RN-05 | US-27, US-28 |
| NB-05 | CU-14 Cerrar el relevamiento | RN-05, RN-03 | US-29, US-30 |
| NB-06 | CU-15 Exportar un relevamiento completo | RN-01 | US-31, US-32 |
| NB-06 | CU-16 Importar un relevamiento completo | RN-01 | US-33, US-34 |
| NB-07 | CU-17 Configurar el destino de almacenamiento | RN-01 | US-35, US-36 |
| NB-01 (transversal) | CU-18 Autorizar por rol y alcance | RN-01, RN-02 | US-37, US-38 |
| NB-01 a NB-05 (transversal) | CU-19 Manejo uniforme de errores | (uniformiza las RN existentes) | US-39 |
| NB-02, NB-03, NB-05 (transversal) | CU-20 Paginar y filtrar listados | RN-01 | US-40, US-41 |
| NB-04, NB-01, NB-02, NB-06 (transversal) | CU-21 Idempotencia de operaciones no seguras | RN-07, RN-06 | US-42, US-43 |
| NB-01 a NB-05 (transversal) | CU-22 Versionar el contrato público | (política de compatibilidad) | US-44 |

Cobertura bidireccional. Cada NB de NB-01 a NB-07 tiene al menos un CU; cada CU declara al menos una NB. No hay CU huérfano. Los CU transversales (CU-18 a CU-22) se anclan a las NB cuyo cumplimiento sostienen, principalmente NB-01 (control de acceso) y NB-04 (sincronización confiable), además de servir a toda la superficie REST.

## 7. Correspondencia con la numeración de CU prevista en 01

El catálogo de necesidades (01) previó CU-01 a CU-17 mapeadas una a una a los recursos públicos; esta especificación conserva esa numeración y mapeo, y agrega CU-18 a CU-22 como los cinco CU transversales que el tipo `rest-api` exige (02 §2.2). La numeración es contigua y sin huecos.

## 8. Decisiones de recorte (02 §5.2)

- CU-01 (jerarquía general) y CU-02 (alta y baja de agentes por el jefe) se mantienen como CU separados, pese a que el segundo es un tramo del primero, porque tienen actor primario distinto y consecuencias propias sobre los relevamientos del agente; no se fusionan para no diluir el actor primario único de cada uno (02 §4.5).
- La sincronización se especifica en dos CU (CU-10 subida y CU-11 bajada) en vez de uno solo, para que la garantía de orden subir antes de bajar (RN-06) sea verificable de forma independiente en cada fase.
- Los flujos de error repetidos (autorización, formato y paginación) se extrajeron a CU transversales (CU-18, CU-19, CU-20) en vez de repetirse en cada CU funcional, según la guía de CU transversal de manejo de errores (02 §5.2).
- El carrusel de fotos y el mapa con pines (F-10, F-12) se especifican aquí como el contrato de datos que el backend provee (CU-12); su render y su interacción visual pertenecen a las categorías de cliente, no a este backend.

## 9. Ambigüedades y supuestos abiertos (master-prompt §9)

El intake declara como PENDIENTE de respuesta del cliente algunos casos límite de §7 que afectan el detalle fino de la sincronización y el cierre. Se especificaron con un supuesto explícito, marcado en los CU correspondientes, a confirmar con el negocio:

- Conflictos entre cambios de dos agentes sobre el mismo relevamiento o marcador (intake §7): se asume la misma política de convivencia y resolución al cierre que para los conflictos por radio (RN-03), reflejada en CU-13. A confirmar.
- Pérdida de conexión durante una sincronización con subida parcial (intake §7): se asume reanudación idempotente sin pérdida ni duplicación (RN-07), reflejada en CU-10. A confirmar.
- Cierre de un relevamiento mientras un agente tiene cambios locales sin sincronizar (intake §7): se asume que el cierre bloquea nuevas subidas y el backend responde RELEVAMIENTO_CERRADO, reflejado en CU-10 y CU-14. A confirmar.
- Foto sin datos de ubicación incrustados y captura sin señal de ubicación (intake §7): se asume que la foto queda pendiente de ubicación manual sin inventarle coordenada (RN-04), reflejado en CU-09. A confirmar.

Ninguno de estos supuestos bloquea la especificación; cada uno se resolverá al confirmar el negocio, sin alterar la estructura de los CU.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice maestro inicial de la especificación funcional de geovial-api: 22 CU (17 de recursos públicos y 5 transversales), 7 RN, modelo conceptual de 12 entidades, 6 RC y matriz de trazabilidad NB→CU→RN→US. |
