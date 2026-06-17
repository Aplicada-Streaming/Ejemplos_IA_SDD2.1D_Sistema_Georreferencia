# Product backlog — geovial-api

**Proyecto:** geovial-api
**Documento:** product-backlog_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner

## 1. Objetivos del producto

El propósito de este backlog es ordenar la construcción del contrato público REST de geovial-api en historias de usuario planificables, priorizadas por valor de negocio y trazadas a los 22 casos de uso de la especificación funcional (CU-01 a CU-22) y a las siete necesidades de negocio (NB-01 a NB-07). El MVP buscado es el camino end-to-end de las cinco necesidades Must Have (NB-01 a NB-05): administrar la jerarquía de usuarios y autenticar, gestionar y asignar relevamientos, capturar observaciones georreferenciadas, sincronizar el trabajo sin conexión y revisar sobre mapa con cierre y resolución de conflictos, todo sostenido por las capacidades transversales del contrato (autorización por rol, errores uniformes, paginación, idempotencia y versionado). La portabilidad (NB-06) y el almacenamiento configurable (NB-07) son Could Have que se incorporan si la cadencia lo permite.

El API Product Owner arbitra la retro-compatibilidad y las deprecaciones del contrato a lo largo del backlog: ninguna historia introduce un cambio incompatible sin pasar por la política de versionado (US-44).

## 2. Épicas

Las épicas se organizan por recurso o dominio del contrato público (regla 06 §1.2, tipo rest-api). Las cinco primeras cubren los recursos funcionales del ciclo del relevamiento; EP-06 y EP-07 cubren las necesidades Could Have; EP-08 agrupa las capacidades transversales que atraviesan toda la superficie REST.

| Épica | Nombre | Descripción | NB / CU | Sprints estimados |
| --- | --- | --- | --- | --- |
| EP-01 | Usuarios, sesión y autorización | Jerarquía de usuarios en cuatro niveles, alta y baja de agentes, inicio y cierre de sesión, revalidación y autorización por rol y alcance | NB-01; CU-01, CU-02, CU-03, CU-18 | 2 |
| EP-02 | Relevamientos y ciclo de vida | Alta, baja y visualización de relevamientos con su tramo, asignación y reasignación de agentes, transición de estados del relevamiento | NB-02; CU-04, CU-05, CU-06 | 2 |
| EP-03 | Marcadores, observaciones y carga manual | Marcadores geográficos, observaciones con notas, fotos, comentarios y etiquetas, y carga manual con priorización de ubicación y radio de agrupación | NB-03; CU-07, CU-08, CU-09 | 2 |
| EP-04 | Sincronización sin conexión | Subida del lote de cambios locales y bajada de actualizaciones, con orden subir-antes-de-bajar e idempotencia por identificador de origen | NB-04; CU-10, CU-11 | 2 |
| EP-05 | Revisión, conflictos y cierre | Consulta del relevamiento para revisión sobre mapa, resolución de conflictos de marcadores y cierre como hito | NB-05; CU-12, CU-13, CU-14 | 2 |
| EP-06 | Portabilidad del relevamiento | Exportación e importación de un relevamiento completo en una unidad transferible única | NB-06; CU-15, CU-16 | 1 |
| EP-07 | Configuración de almacenamiento | Configuración, consulta y validación del destino de almacenamiento de archivos por el usuario raíz | NB-07; CU-17 | 1 |
| EP-08 | Capacidades transversales del contrato | Errores uniformes problem+json, paginación y filtros, idempotencia de operaciones no seguras y versionado del contrato público | NB-01 a NB-05; CU-19, CU-20, CU-21, CU-22 | 1 |

La autorización por rol y alcance (CU-18) se modela como historias de la EP-01 (US-37, US-38) por ser la capacidad fundacional de control de acceso de NB-01, y se apoya en la BT transversal del backlog técnico que la materializa como middleware previo a todo efecto.

## 3. Historias por épica

Técnica de estimación adoptada: Fibonacci (1, 2, 3, 5, 8, 13). Story points relativos al equipo de un desarrollador; el forecast se afina con el historial de velocity en 07. Las US individuales viven cada una en `historias-usuario/US-XX-<kebab>_v1.0.md` por superar el umbral de 20 US (regla 06 §3.3); esta tabla es el índice maestro priorizado.

### 3.1 EP-01 Usuarios, sesión y autorización

| US | Título | MoSCoW | SP | Estado | CU relacionados |
| --- | --- | --- | --- | --- | --- |
| US-01 | Administrar la jerarquía de usuarios del nivel inmediato inferior | Must | 8 | Borrador | CU-01 |
| US-02 | Dar de baja un usuario conservando su autoría histórica | Must | 5 | Borrador | CU-01 |
| US-03 | Dar de alta agentes de campo por el jefe de área | Must | 5 | Borrador | CU-02 |
| US-04 | Dar de baja un agente de campo sin perder su trabajo registrado | Must | 3 | Borrador | CU-02 |
| US-05 | Iniciar sesión y obtener un token de acceso | Must | 5 | Borrador | CU-03 |
| US-06 | Cerrar sesión y revalidar la sesión activa | Should | 3 | Borrador | CU-03 |
| US-37 | Autorizar cada operación según el rol y el alcance del solicitante | Must | 8 | Borrador | CU-18, CU-01 |
| US-38 | Acotar cada listado al alcance jerárquico antes de paginar | Must | 5 | Borrador | CU-18, CU-20 |

### 3.2 EP-02 Relevamientos y ciclo de vida

| US | Título | MoSCoW | SP | Estado | CU relacionados |
| --- | --- | --- | --- | --- | --- |
| US-07 | Crear un relevamiento con su tramo vial no vacío | Must | 5 | Borrador | CU-04 |
| US-08 | Visualizar y consultar los relevamientos del alcance | Must | 3 | Borrador | CU-04 |
| US-09 | Dar de baja un relevamiento del alcance | Should | 3 | Borrador | CU-04 |
| US-10 | Asignar un agente de campo a un relevamiento | Must | 5 | Borrador | CU-05 |
| US-11 | Reasignar y revocar agentes de un relevamiento | Should | 3 | Borrador | CU-05 |
| US-12 | Avanzar el relevamiento de recolección a revisión | Must | 5 | Borrador | CU-06 |
| US-13 | Retornar el relevamiento de revisión a recolección | Should | 3 | Borrador | CU-06 |

### 3.3 EP-03 Marcadores, observaciones y carga manual

| US | Título | MoSCoW | SP | Estado | CU relacionados |
| --- | --- | --- | --- | --- | --- |
| US-14 | Crear y mover marcadores geográficos con identidad estable | Must | 5 | Borrador | CU-07 |
| US-15 | Dar de baja un marcador solo si no tiene observaciones ancladas | Should | 3 | Borrador | CU-07 |
| US-16 | Crear una observación anclada a un marcador existente | Must | 5 | Borrador | CU-08 |
| US-17 | Adjuntar fotos a una observación delegando el binario al almacén | Must | 5 | Borrador | CU-08 |
| US-18 | Comentar y etiquetar fotos y marcadores | Should | 3 | Borrador | CU-08 |
| US-19 | Cargar fotos manualmente priorizando la ubicación incrustada | Must | 5 | Borrador | CU-09 |
| US-20 | Agrupar por radio las fotos sin ubicación o cercanas | Should | 5 | Borrador | CU-09 |

### 3.4 EP-04 Sincronización sin conexión

| US | Título | MoSCoW | SP | Estado | CU relacionados |
| --- | --- | --- | --- | --- | --- |
| US-21 | Subir el lote de cambios locales del agente | Must | 8 | Borrador | CU-10 |
| US-22 | Reanudar una subida interrumpida sin duplicar cambios | Must | 5 | Borrador | CU-10 |
| US-23 | Bajar las actualizaciones del relevamiento posteriores a la marca | Must | 8 | Borrador | CU-11 |
| US-24 | Rechazar la bajada hasta concluir la subida del ciclo | Must | 3 | Borrador | CU-11 |

### 3.5 EP-05 Revisión, conflictos y cierre

| US | Título | MoSCoW | SP | Estado | CU relacionados |
| --- | --- | --- | --- | --- | --- |
| US-25 | Consultar el relevamiento completo para la revisión sobre mapa | Must | 5 | Borrador | CU-12 |
| US-26 | Recuperar las fotos por marcador para el carrusel de revisión | Should | 3 | Borrador | CU-12 |
| US-27 | Listar los conflictos de marcadores del relevamiento | Must | 3 | Borrador | CU-13 |
| US-28 | Resolver un conflicto unificando o separando marcadores | Must | 5 | Borrador | CU-13 |
| US-29 | Cerrar el relevamiento exigiendo los conflictos resueltos | Must | 5 | Borrador | CU-14 |
| US-30 | Reabrir un relevamiento cerrado a revisión | Could | 3 | Borrador | CU-14 |

### 3.6 EP-06 Portabilidad del relevamiento

| US | Título | MoSCoW | SP | Estado | CU relacionados |
| --- | --- | --- | --- | --- | --- |
| US-31 | Exportar un relevamiento completo en una unidad transferible | Could | 8 | Borrador | CU-15 |
| US-32 | Incluir comentarios, etiquetas y fotos en la exportación | Could | 5 | Borrador | CU-15 |
| US-33 | Importar un relevamiento reconstruyendo su estructura | Could | 8 | Borrador | CU-16 |
| US-34 | Importar de forma idempotente sin duplicar un relevamiento | Could | 5 | Borrador | CU-16 |

### 3.7 EP-07 Configuración de almacenamiento

| US | Título | MoSCoW | SP | Estado | CU relacionados |
| --- | --- | --- | --- | --- | --- |
| US-35 | Configurar el destino de almacenamiento activo por el usuario raíz | Could | 5 | Borrador | CU-17 |
| US-36 | Validar un destino de almacenamiento sin activarlo | Could | 3 | Borrador | CU-17 |

### 3.8 EP-08 Capacidades transversales del contrato

| US | Título | MoSCoW | SP | Estado | CU relacionados |
| --- | --- | --- | --- | --- | --- |
| US-39 | Devolver todos los errores con un formato de problema uniforme | Must | 5 | Borrador | CU-19 |
| US-40 | Paginar los listados de recursos con referencias de navegación | Must | 5 | Borrador | CU-20 |
| US-41 | Filtrar y ordenar los listados por criterios soportados | Should | 3 | Borrador | CU-20 |
| US-42 | Aceptar una clave de idempotencia en las operaciones no seguras | Must | 8 | Borrador | CU-21 |
| US-43 | Rechazar una clave de idempotencia reutilizada de forma inconsistente | Should | 3 | Borrador | CU-21 |
| US-44 | Versionar el contrato público y preservar la compatibilidad | Must | 5 | Borrador | CU-22 |

### 3.9 Cobertura de CU

Los 22 CU quedan cubiertos por al menos una US: CU-01 (US-01, US-02), CU-02 (US-03, US-04), CU-03 (US-05, US-06), CU-04 (US-07, US-08, US-09), CU-05 (US-10, US-11), CU-06 (US-12, US-13), CU-07 (US-14, US-15), CU-08 (US-16, US-17, US-18), CU-09 (US-19, US-20), CU-10 (US-21, US-22), CU-11 (US-23, US-24), CU-12 (US-25, US-26), CU-13 (US-27, US-28), CU-14 (US-29, US-30), CU-15 (US-31, US-32), CU-16 (US-33, US-34), CU-17 (US-35, US-36), CU-18 (US-37, US-38), CU-19 (US-39), CU-20 (US-40, US-41), CU-21 (US-42, US-43), CU-22 (US-44). No hay US huérfana de CU ni CU sin US.

## 4. Métricas de avance

Total de historias: 44 US. Distribución por prioridad MoSCoW y story points (Fibonacci):

| Prioridad | Cantidad | % del total | Story points | % de SP |
| --- | --- | --- | --- | --- |
| Must | 27 | 61,4 % | 142 | 67,3 % |
| Should | 10 | 22,7 % | 32 | 15,2 % |
| Could | 7 | 15,9 % | 37 | 17,5 % |
| Won't (v1.0) | 0 | 0 % | 0 | 0 % |

Total de story points del backlog: 211 SP. La distribución respeta el reparto sugerido (regla 06 §4.7): Must en torno al 60 %, Should y Could entre 15 y 25 % cada uno. No hay backlog 100 % Must; hay reparto real entre las tres prioridades.

| Métrica | Valor |
| --- | --- |
| Historias cerradas (Done) | 0 de 44 |
| Porcentaje cerrado | 0 % |
| Story points cerrados | 0 de 211 |
| Deuda en backlog (US Could no planificadas en MVP) | 7 US (EP-06, EP-07 y US-30) |

El MVP (US Must) cubre el camino end-to-end de NB-01 a NB-05 más las capacidades transversales Must (US-39, US-40, US-42, US-44), suficiente para cumplir el propósito del sistema declarado en 00. Las US Could (EP-06, EP-07, US-30) quedan documentadas como deuda planificable para una iteración posterior.

## 5. Refinamiento

| Aspecto | Definición |
| --- | --- |
| Cadencia | Una sesión de refinement por sprint, mínimo (regla 06 §2.2, tipo rest-api). |
| Responsables | API Product Owner (titular del backlog y prioridad MoSCoW), Scrum Master (facilitación y DoR), equipo de desarrollo (estimación). |
| Formato de estimación | Planning Poker con escala Fibonacci; las US sin consenso quedan marcadas para spike antes de entrar al sprint. |
| Entradas del refinement | CU y RN de 02, ADRs y contrato de 05, errores y experiencia de desarrollador de 03. |
| Salida del refinement | US que cumplen la Definition of Ready (`definition-of-ready_v1.0.md`), estimadas y trazadas a CU. |
| Criterio de corte | Una US que no cumple la DoR no entra a Sprint Planning; se devuelve al refinement con el motivo del bloqueo. |

La política de retro-compatibilidad del contrato (CU-22, US-44) se revisa en cada refinement: toda US que toque un recurso ya publicado declara si el cambio es compatible o exige versión mayor nueva, y el API Product Owner aprueba la decisión.

## 6. Referencias cruzadas

- Vista técnica del backlog: `backlog-tecnico_v1.0.md` (épicas técnicas, BT y matriz BT↔US↔CU).
- Filtro de entrada: `definition-of-ready_v1.0.md`.
- Historias individuales: `historias-usuario/US-XX-<kebab>_v1.0.md`.
- Upstream: 01 (NB-01 a NB-07), 02 (CU-01 a CU-22, RN-01 a RN-07), 05 (ADR-01 a ADR-10, contrato REST, modelo lógico).
- Downstream: 07 (sprint plan), 08 (acceptance tests).

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Product backlog inicial de geovial-api: 8 épicas por recurso/dominio del contrato, 44 historias de usuario priorizadas con MoSCoW y estimación Fibonacci, trazabilidad a CU-01 a CU-22, métricas de avance y política de refinement. Modo de US: archivos individuales bajo `historias-usuario/` por superar el umbral de 20 US (regla 06 §3.3). |
