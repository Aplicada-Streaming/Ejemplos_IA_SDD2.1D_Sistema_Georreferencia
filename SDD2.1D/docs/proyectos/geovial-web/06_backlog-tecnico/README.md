# Backlog técnico — geovial-web (índice de sección)

**Proyecto:** geovial-web
**Documento:** README.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Scrum Master

Punto de entrada navegable del backlog de `geovial-web`, el front web de los roles administradores de la solución. Sirve a los revisores funcionales (trazabilidad a CU), de arquitectura (justificación de BT), de planificación (secuencia y capacidad) y de calidad (verificabilidad de criterios). Tipo de proyecto: web-monolith.

## 1. Artefactos de la sección

| Artefacto | Descripción |
| --- | --- |
| `product-backlog_v1.0.md` | Índice maestro priorizado: objetivos, épicas EP-01 a EP-07, 18 historias de usuario con MoSCoW, story points y trazabilidad a CU, métricas de avance y política de refinement |
| `backlog-tecnico_v1.0.md` | Vista técnica: 6 épicas técnicas (EP-T1 a EP-T6), 14 tareas técnicas BT-01 a BT-14 con fuente upstream y dependencias, y matriz BT↔US↔CU |
| `definition-of-ready_v1.0.md` | Criterios DoR para historias (7) y tareas técnicas (5), excepciones y aprobador |
| `README.md` | Este índice de la sección |

## 2. Modo de organización

Modo inline conforme a la regla 06 §3.3. Con 18 historias de usuario (menos de 20) las US viven inline en el `product-backlog_v1.0.md` sin carpeta `historias-usuario/`. Con 14 tareas técnicas (menos de 30) las BT viven inline en el `backlog-tecnico_v1.0.md` sin carpeta `tareas-tecnicas/`. Cada US conserva su historia, trazabilidad y criterios; cada BT, su justificación, dependencias y trazabilidad.

## 3. Épicas vigentes

Épicas funcionales (product backlog):

| Épica | Nombre | CU cubiertos |
| --- | --- | --- |
| EP-01 | Acceso y administración de usuarios | CU-01, CU-02 |
| EP-02 | Gestión de relevamientos y marcadores | CU-03, CU-05 |
| EP-03 | Asignación de agentes | CU-04 |
| EP-04 | Revisión sobre mapa con carrusel | CU-06 |
| EP-05 | Resolución de conflictos y cierre | CU-07, CU-08 |
| EP-06 | Carga manual de evidencia vía web | CU-09 |
| EP-07 | Portabilidad y configuración | CU-10, CU-11 |

Épicas técnicas (backlog técnico):

| Épica técnica | Nombre | BT |
| --- | --- | --- |
| EP-T1 | Cimientos de capas y cliente del contrato | BT-01, BT-02, BT-03 |
| EP-T2 | Sesión, token y visibilidad por rol | BT-04, BT-05 |
| EP-T3 | Componente de mapa, marcadores y carrusel | BT-06, BT-07, BT-08 |
| EP-T4 | Ciclo de estado y resolución de conflictos | BT-09, BT-10 |
| EP-T5 | Manejo de errores y feedback de UI | BT-11 |
| EP-T6 | Formularios, validación y carga de archivos | BT-12, BT-13, BT-14 |

## 4. Historias Must del MVP

El MVP queda definido por las 11 historias Must (EP-01 a EP-06), 65 SP, que cubren el camino principal del relevamiento de extremo a extremo:

| US | Título | SP | CU |
| --- | --- | --- | --- |
| US-01 | Ingresar al front con credenciales y obtener una sesión con rol | 5 | CU-01 |
| US-02 | Cerrar la sesión y dejar el acceso liberado | 3 | CU-01 |
| US-03 | Listar y dar de alta usuarios del nivel inmediato inferior | 5 | CU-02 |
| US-05 | Crear y listar relevamientos sobre un tramo vial | 5 | CU-03 |
| US-07 | Crear y ubicar marcadores iniciales sobre el mapa | 8 | CU-05 |
| US-08 | Asignar agentes de campo a un relevamiento | 5 | CU-04 |
| US-10 | Recorrer marcadores y navegar el carrusel encadenado de fotos | 8 | CU-06 |
| US-12 | Resolver un conflicto de marcadores unificando o separando | 8 | CU-07 |
| US-13 | Transicionar el estado del relevamiento por su ciclo | 5 | CU-08 |
| US-14 | Cerrar el relevamiento solo sin conflictos pendientes | 5 | CU-08, CU-07 |
| US-15 | Cargar fotos con agrupación por radio desde el front | 8 | CU-09 |

Distribución MoSCoW: 11 Must, 5 Should, 2 Could sobre 18 historias y 90 SP.

## 5. Tareas técnicas prioritarias

Las BT de cimientos y de sesión son prerrequisito del resto y se planifican primero:

| BT | Título | Tipo | Prioridad |
| --- | --- | --- | --- |
| BT-01 | Andamiaje de las tres capas con dependencia unidireccional | feature | Must |
| BT-02 | Cliente del contrato con puerto de acceso al dominio | feature | Must |
| BT-03 | Armazón del circuito interactivo y estado de UI efímero | feature | Must |
| BT-04 | Servicio de sesión y token custodiado del lado servidor | feature | Must |
| BT-06 | Spike de integración del componente de mapa de terceros | spike | Must |
| BT-11 | Mapeador de errores del contrato a feedback de UI | feature | Must |

## 6. Definition of Ready vigente

La `definition-of-ready_v1.0.md` define 7 criterios para historias y 5 para tareas técnicas, con excepciones para spike exploratorio (BT-06), historias Could en promoción (US-17, US-18) y dependencias de Sprint 0 (ADR-04, ADR-05). Aprobador: Scrum Master, con revisiones acotadas del Analista Funcional, el Arquitecto y QA. La DoR habla de cuándo empezar; la Definition of Done de 08, de cuándo terminar.

## 7. Trazabilidad y revisión

- Upstream: necesidades de negocio NB-01, NB-02, NB-05, NB-06, NB-07 (01); casos de uso CU-01 a CU-11 y reglas de negocio RN-01 a RN-05 (02); decisiones de arquitectura ADR-01 a ADR-05 y componentes (05).
- Downstream: sprint plan (07) y acceptance tests y pruebas de componente (08).
- Identificadores con dos dígitos uniformes (US-XX, BT-XX, EP-XX, EP-TX); estimación Fibonacci consistente en todo el backlog.

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice inicial de la sección 06 de geovial-web: artefactos, modo inline, épicas funcionales y técnicas, historias Must del MVP, BT prioritarias y DoR vigente. |
| 1.0 | 2026-06-15 | Corrección de consistencia: reconciliación de la tabla de métricas SP con la suma ítem-a-ítem de las historias. |
