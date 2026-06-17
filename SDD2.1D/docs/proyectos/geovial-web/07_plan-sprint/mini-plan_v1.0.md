# Mini-plan — geovial-web

**Proyecto:** geovial-web
**Documento:** mini-plan_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Scrum Master
**Modo:** mini-plan (equipo_n=1), conforme a la regla 07 §2.1 y §2.2

## 1. Información general

Tipo de proyecto: web-monolith. Equipo: un único desarrollador full-stack; el Scrum Master, el Analista Funcional, el Arquitecto y QA aportan revisiones acotadas según la regla 06 §1.3. Por tratarse de un proyecto de un solo dev, este mini-plan sustituye a los planes de iteración por sprint, las plantillas de review y retrospectiva y el tracking de velocidad (regla 07 §2.2); no se generan esos artefactos.

Unidad de estimación: story points con técnica Fibonacci (1, 2, 3, 5, 8, 13), la misma del backlog de 06. El plan organiza el trabajo en tramos secuenciales que respetan las dependencias técnicas del `backlog-tecnico_v1.0.md` y el orden topológico de fases del `roadmap-producto_v1.0.md` (F0 a F3). Cada tramo se cierra por criterios verificables antes de habilitar el siguiente; no se fijan fechas de calendario porque la cadencia la marca el avance de un único desarrollador y el proyecto no tiene fecha objetivo.

Alcance comprometido en este mini-plan: las 18 historias de usuario (US-01 a US-18) y las 14 tareas técnicas (BT-01 a BT-14) del backlog de 06, distribuidas en cinco tramos. El MVP (10 US Must, 60 SP) se concentra en los tramos 1 a 4; la portabilidad y la configuración (US-17, US-18, BT-14) quedan en el tramo 5, fuera del camino principal.

## 2. Objetivo

Entregar de punta a punta el front web de los roles administradores que permite ingresar con sesión, administrar usuarios por jerarquía, gestionar y asignar relevamientos con marcadores sobre el mapa, revisar la evidencia con el carrusel encadenado y cerrar el relevamiento tras resolver sus conflictos, sumando la carga manual de evidencia y las capacidades de portabilidad y configuración como cierre de alcance.

## 3. Estrategia de tramos

Cinco tramos secuenciales. El primero materializa el walking skeleton de la fase F0 del roadmap (autenticación y administración de usuarios de punta a punta) montado sobre los cimientos técnicos del front. Los tramos siguientes incorporan capacidad funcional en el orden que imponen las dependencias entre BT y las fases del roadmap.

| Tramo | Foco | Fase roadmap | Ítems comprometidos | SP del tramo |
| --- | --- | --- | --- | --- |
| Tramo 1 | Cimientos del front y walking skeleton de acceso y administración de usuarios | F0 | BT-01, BT-02, BT-03, BT-04, BT-05, BT-11, US-01, US-02, US-03 | 44 |
| Tramo 2 | Administración completa de usuarios y gestión de relevamientos | F0 / F1 | BT-12, BT-09, US-04, US-05, US-06 | 21 |
| Tramo 3 | Marcadores sobre el mapa y asignación de agentes | F1 | BT-06, BT-07, US-07, US-08, US-09 | 27 |
| Tramo 4 | Revisión con carrusel, resolución de conflictos y cierre | F3 | BT-08, BT-10, US-10, US-11, US-12, US-13, US-14 | 45 |
| Tramo 5 | Carga manual de evidencia y cierre de alcance | F2 (web) / F3 | BT-13, BT-14, US-15, US-16, US-17, US-18 | 34 |

Total comprometido: 171 SP entre tramos, suma de las 14 BT (81 SP) y las 18 US (90 SP) contadas cada una una sola vez en el tramo donde se compromete, con la estimación exacta que cada ítem trae del backlog de 06. El backlog de 06 reporta 81 SP para el conjunto de historias bajo su propia convención de conteo; este mini-plan no re-deriva ese total y suma además las BT que soportan el front, por lo que la cifra de tramos es superior.

### 3.1 Tramo 1 — Cimientos del front y walking skeleton de acceso y administración de usuarios

Primer tramo alineado al walking skeleton de la fase F0 del roadmap: ingreso de sesión y administración jerárquica de usuarios de punta a punta. Establece los cimientos de capas, el cliente del contrato, el armazón del circuito interactivo, la sesión con token custodiado, el control de visibilidad por rol y el mapeo de errores a feedback, sobre los que se construye el resto del front.

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-01 | Backlog técnico | Andamiaje de las tres capas con dependencia unidireccional | Must | 5 | Dev | Pendiente |
| BT-02 | Backlog técnico | Cliente del contrato con puerto de acceso al dominio y normalización | Must | 8 | Dev | Pendiente |
| BT-03 | Backlog técnico | Armazón del circuito interactivo y estado de UI efímero | Must | 5 | Dev | Pendiente |
| BT-04 | Backlog técnico | Servicio de sesión y token custodiado del lado servidor | Must | 5 | Dev | Pendiente |
| BT-05 | Backlog técnico | Control de visibilidad y acciones por rol jerárquico | Must | 3 | Dev | Pendiente |
| BT-11 | Backlog técnico | Mapeador de errores del contrato a feedback de UI por código estable | Must | 5 | Dev | Pendiente |
| US-01 | Historia | Ingresar al front con credenciales y obtener una sesión con rol | Must | 5 | Dev | Pendiente |
| US-02 | Historia | Cerrar la sesión y dejar el acceso liberado | Must | 3 | Dev | Pendiente |
| US-03 | Historia | Listar y dar de alta usuarios del nivel inmediato inferior | Must | 5 | Dev | Pendiente |

Total del tramo: 44 SP. Dependencias internas: BT-01 abre el tramo; BT-02 y BT-03 dependen de BT-01; BT-04 depende de BT-02 y BT-03; BT-05 depende de BT-04; BT-11 depende de BT-02. US-01 y US-02 se apoyan en BT-03, BT-04 y BT-11; US-03 se apoya en BT-05, BT-11 y, para el formulario de alta, anticipa la necesidad de BT-12 (formularios) que se completa en el Tramo 2; el alta básica de US-03 entra con la validación mínima del circuito y se refina con BT-12. Las ADR-04 y ADR-05 en estado Propuesto se ratifican al inicio de este tramo (excepción de Sprint 0 de la DoR).

### 3.2 Tramo 2 — Administración completa de usuarios y gestión de relevamientos

Cierra la administración de usuarios con la baja que conserva autoría y abre la gestión de relevamientos sobre un tramo vial con el control de habilitación por estado. Incorpora el patrón de formulario con validación que sostiene las altas, ediciones y bajas.

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-12 | Backlog técnico | Patrón de formulario con validación de entrada de pantalla | Must | 5 | Dev | Pendiente |
| BT-09 | Backlog técnico | Control de habilitación de acciones por estado del relevamiento | Must | 5 | Dev | Pendiente |
| US-04 | Historia | Dar de baja un usuario conservando su autoría visible | Should | 3 | Dev | Pendiente |
| US-05 | Historia | Crear y listar relevamientos sobre un tramo vial | Must | 5 | Dev | Pendiente |
| US-06 | Historia | Editar y dar de baja un relevamiento según su estado | Should | 3 | Dev | Pendiente |

Total del tramo: 21 SP. Dependencias internas: BT-12 depende de BT-01 y BT-11 (Tramo 1); BT-09 depende de BT-02 (Tramo 1) y BT-05 (Tramo 1). US-04 se apoya en BT-05 y BT-12; US-05 y US-06 se apoyan en BT-09, BT-12 y BT-11.

### 3.3 Tramo 3 — Marcadores sobre el mapa y asignación de agentes

Integra el componente de mapa de terceros mediante un spike acotado y construye el adaptador de marcadores con identidad estable, habilitando la creación y ubicación de marcadores iniciales y la asignación de agentes de campo.

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-06 | Backlog técnico | Spike de integración del componente de mapa de terceros (caja temporal 2 días) | Must | 3 | Dev | Pendiente |
| BT-07 | Backlog técnico | Adaptador del componente de mapa con marcadores e identidad estable | Must | 8 | Dev | Pendiente |
| US-07 | Historia | Crear y ubicar marcadores iniciales sobre el mapa | Must | 8 | Dev | Pendiente |
| US-08 | Historia | Asignar agentes de campo a un relevamiento | Must | 5 | Dev | Pendiente |
| US-09 | Historia | Reasignar y quitar agentes conservando lo recolectado | Should | 3 | Dev | Pendiente |

Total del tramo: 27 SP. Dependencias internas: BT-06 depende de BT-01 (Tramo 1) y es spike con caja temporal de 2 días; BT-07 depende de BT-06 y de BT-02 (Tramo 1). US-07 se apoya en BT-06 y BT-07; US-08 y US-09 se apoyan en BT-05 (Tramo 1), BT-09 (Tramo 2) y BT-12 (Tramo 2).

### 3.4 Tramo 4 — Revisión con carrusel, resolución de conflictos y cierre

Construye el carrusel encadenado con filtro por etiqueta y la pantalla de resolución de conflictos con el cierre condicionado, completando el camino principal del relevamiento hasta su cierre. Corresponde a la fase F3 del roadmap.

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-08 | Backlog técnico | Componente de carrusel encadenado de fotos con filtro por etiqueta | Must | 8 | Dev | Pendiente |
| BT-10 | Backlog técnico | Pantalla de resolución de conflictos y cierre condicionado | Must | 8 | Dev | Pendiente |
| US-10 | Historia | Recorrer marcadores y navegar el carrusel encadenado de fotos | Must | 8 | Dev | Pendiente |
| US-11 | Historia | Filtrar la evidencia por etiqueta durante la revisión | Should | 3 | Dev | Pendiente |
| US-12 | Historia | Resolver un conflicto de marcadores unificando o separando | Must | 8 | Dev | Pendiente |
| US-13 | Historia | Transicionar el estado del relevamiento por su ciclo | Must | 5 | Dev | Pendiente |
| US-14 | Historia | Cerrar el relevamiento solo sin conflictos pendientes | Must | 5 | Dev | Pendiente |

Total del tramo: 45 SP. Dependencias internas: BT-08 depende de BT-07 (Tramo 3); BT-10 depende de BT-08 y de BT-09 (Tramo 2). US-10 y US-11 se apoyan en BT-07 (Tramo 3) y BT-08; US-12, US-13 y US-14 se apoyan en BT-09 (Tramo 2), BT-10 y BT-11 (Tramo 1).

### 3.5 Tramo 5 — Carga manual de evidencia y cierre de alcance

Incorpora la carga manual asincrónica de fotos con radio de agrupación y, como cierre de alcance, las capacidades de portabilidad y configuración de almacenamiento, ambas fuera del camino principal. Las historias Could (US-17, US-18) se promueven con sus dos escenarios Given/When/Then completos antes de comprometerse, según la excepción de la DoR.

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-13 | Backlog técnico | Carga asincrónica de fotos con radio de agrupación | Must | 8 | Dev | Pendiente |
| BT-14 | Backlog técnico | Operaciones de larga duración de exportación e importación | Could | 5 | Dev | Pendiente |
| US-15 | Historia | Cargar fotos con agrupación por radio desde el front | Must | 8 | Dev | Pendiente |
| US-16 | Historia | Completar comentarios, etiquetas y ubicación manual de la evidencia | Should | 5 | Dev | Pendiente |
| US-17 | Historia | Exportar e importar un relevamiento completo | Could | 5 | Dev | Pendiente |
| US-18 | Historia | Configurar el destino de almacenamiento de archivos | Could | 3 | Dev | Pendiente |

Total del tramo: 34 SP. Dependencias internas: BT-13 depende de BT-07 (Tramo 3) y BT-12 (Tramo 2); BT-14 depende de BT-12 (Tramo 2) y BT-11 (Tramo 1). US-15 se apoya en BT-07 (Tramo 3), BT-12 (Tramo 2) y BT-13; US-16 se apoya en BT-12 (Tramo 2) y BT-13; US-17 y US-18 se apoyan en BT-14 y BT-05 (Tramo 1).

## 4. Definition of Done aplicada

La Definition of Done canónica del proyecto vive en la categoría 08, pendiente de generación. Mientras 08 no exista, este mini-plan referencia esa DoD por adelantado y no la redefine: ningún tramo cierra un ítem con criterio improvisado. Al producirse 08 se vincula explícitamente la DoD canónica desde este documento. Los criterios de entrada al trabajo de cada tramo se gobiernan por la `definition-of-ready_v1.0.md` de 06; los criterios de terminación, por la DoD de 08.

Criterio de cierre de tramo específico de este mini-plan: un tramo se considera completo cuando todos sus ítems están terminados según la DoD de 08, los criterios de transición de fase del roadmap aplicables al tramo se verifican, y la capacidad construida queda demostrable de punta a punta sobre el entorno de prueba.

## 5. Trazabilidad por tramo

Cada tramo declara qué casos de uso (CU-01 a CU-11) avanzan y qué necesidades de negocio (NB-01, NB-02, NB-05, NB-06, NB-07) progresan al cierre del tramo.

| Tramo | CU que avanzan | NB que avanzan |
| --- | --- | --- |
| Tramo 1 | CU-01, CU-02 | NB-01 (administración jerárquica de usuarios y control de acceso) |
| Tramo 2 | CU-02, CU-03 | NB-01, NB-02 (gestión y asignación de relevamientos) |
| Tramo 3 | CU-04, CU-05 | NB-02 |
| Tramo 4 | CU-06, CU-07, CU-08 | NB-05 (revisión sobre mapa y cierre con resolución de conflictos) |
| Tramo 5 | CU-09, CU-10, CU-11 | NB-02, NB-06 (portabilidad del relevamiento), NB-07 (almacenamiento configurable) |

ADR que gobiernan las decisiones técnicas implicadas: ADR-01 (estilo de render del lado servidor con circuito interactivo), ADR-02 (sin persistencia de dominio, estado efímero), ADR-03 (autenticación con token del lado servidor), ADR-04 (separación de capas), ADR-05 (mapeo de errores a feedback). ADR-04 y ADR-05 se ratifican al inicio del Tramo 1.

## 6. Riesgos y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El spike de integración del componente de mapa de terceros (BT-06) no encuentra un camino claro de sincronización con el estado de la aplicación de UI dentro de su caja temporal de 2 días | Media | Alto | Caja temporal explícita de 2 días al inicio del Tramo 3; si al cierre del plazo no hay camino claro, se documenta el bloqueo y se eleva al Arquitecto antes de comprometer US-07; BT-07 no inicia hasta tener informe del spike |
| Con un único desarrollador, el camino crítico de cimientos (BT-01 a BT-04, BT-11) concentra dependencias y un atraso temprano propaga el retraso a todos los tramos | Alta | Alto | Cimientos completos al frente del Tramo 1 antes de abrir capacidad funcional; los tramos se cierran por criterios verificables y no por fecha; ante atraso se difiere primero la capacidad Should y Could (US-04, US-06, US-09, US-11, US-16, US-17, US-18, BT-14) sin tocar el camino principal Must |
| Las ADR-04 y ADR-05 en estado Propuesto condicionan BT-01, BT-11 y derivadas; un cambio en la ratificación obliga a reelaborar los cimientos | Media | Alto | Ratificación de ADR-04 y ADR-05 al inicio del Tramo 1 como primera actividad (excepción de Sprint 0 de la DoR); las BT dependientes se refinan en paralelo pero no se dan por terminadas hasta la ratificación |
| El cierre condicionado por ausencia de conflictos (RN-05) en BT-10 y US-14 depende de que la resolución (US-12) y la habilitación por estado (US-13) estén firmes; un acoplamiento mal resuelto bloquea el cierre del camino principal | Media | Medio | Secuenciar dentro del Tramo 4 BT-08 y BT-10 antes de las US de cierre; verificar el bloqueo de cierre con conflictos pendientes y la derivación a la pantalla de resolución como criterio de cierre del tramo |

## 7. Bitácora de avance

Registro de avance por tramo. Se actualiza al cierre de cada tramo con la fecha real, los ítems terminados y las observaciones (carry-over diferido, alcance ajustado, bloqueos elevados). Para un proyecto de un solo dev sustituye al tracking de velocidad y al review por sprint.

| Fecha | Tramo | Ítems terminados | SP terminados | Observaciones |
| --- | --- | --- | --- | --- |
| — | Tramo 1 | — | — | Pendiente de inicio. Ratificar ADR-04 y ADR-05 como primera actividad |
| — | Tramo 2 | — | — | Pendiente |
| — | Tramo 3 | — | — | Pendiente. BT-06 es spike con caja temporal de 2 días |
| — | Tramo 4 | — | — | Pendiente |
| — | Tramo 5 | — | — | Pendiente. Promover US-17 y US-18 con escenarios completos antes de comprometer |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Mini-plan inicial de geovial-web (modo mini-plan, equipo_n=1). Objetivo único orientado a valor; 18 US (US-01 a US-18) y 14 BT (BT-01 a BT-14) distribuidas en cinco tramos que respetan las dependencias del backlog técnico y el orden de fases F0 a F3 del roadmap; primer tramo alineado al walking skeleton de acceso y administración de usuarios (F0). Trazabilidad por tramo a CU-01 a CU-11 y a NB-01, NB-02, NB-05, NB-06, NB-07; DoD por referencia a la canónica de 08 (pendiente); cuatro riesgos con mitigación; bitácora de avance por tramo. |
