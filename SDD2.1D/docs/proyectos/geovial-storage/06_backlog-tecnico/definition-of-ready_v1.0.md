# Definition of Ready — geovial-storage

**Proyecto:** geovial-storage
**Documento:** definition-of-ready_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + Backlog Curator
**Tipo (D8):** library

La Definition of Ready (DoR) define cuándo un ítem está listo para entrar al Sprint Planning de 07. Filtra trabajo no refinado antes de comprometerlo a un sprint. No se solapa con la Definition of Done de 08: la DoR habla de cuándo empezar (el ítem está suficientemente especificado para estimarse y comprometerse); la DoD de 08 habla de cuándo terminar (el ítem está construido, probado y la batería de contrato y el gate de cobertura pasan). Cada criterio se responde con sí o no de forma objetiva.

## 1. Criterios DoR para US

Una historia de usuario está Ready cuando cumple los siete criterios:

1. La historia está expresada como `Como [rol], quiero [acción], para [valor]` con el valor para el rol explícito (consumidor o usuario raíz), no un sinónimo de la acción.
2. La columna `CU relacionados` está poblada con al menos un CU de la especificación funcional (CU-01 a CU-06); la US no es huérfana de CU.
3. La US declara su NB upstream (NB-07 principal; NB-03 o NB-06 de soporte cuando aplica) y las RN aplicables (RN-01, RN-02, RN-03).
4. Tiene criterios de aceptación en formato Given/When/Then con al menos dos escenarios para las US Must y Should (al menos un happy path y un caso de error o borde).
5. Está estimada en story points con la técnica Fibonacci adoptada, y la estimación cabe en un sprint (Small de INVEST); si no cabe, se descompone antes de entrar.
6. Tiene su prioridad MoSCoW declarada y justificada, coherente con la distribución del product backlog (no todo Must).
7. No tiene dependencias bloqueantes sin resolver: las BT de fundación que consume (capa de Abstracciones, núcleo, catálogo de errores) están planificadas o terminadas, y los datos de prueba necesarios están disponibles o identificados.

## 2. Criterios DoR para BT

Una tarea técnica está Ready cuando cumple los cinco criterios:

1. Tiene fuente upstream declarada y verificable: una NB, un CU, una ADR (ADR-01 a ADR-05), un componente de la arquitectura o el contrato de Abstractions; sin justificación upstream no entra.
2. Declara al menos una US consumidora, o se justifica explícitamente como infraestructura compartida con su ADR o componente de respaldo (caso de BT-10, BT-11 y BT-13).
3. Su alcance es ejecutable en menos de un sprint y está acotado a una tarea (no encubre una US con valor de negocio; si lo tiene, se reformula como US).
4. Tiene sus dependencias técnicas identificadas (BT o US que deben estar terminadas antes) y sus bloqueos externos, si los hay, registrados.
5. Tiene tipo declarado (feature, spike, refactor, devops o docs) y estimación en Fibonacci; si es spike, lleva caja temporal explícita.

## 3. Excepciones admitidas

| Caso | Flexibilización admitida | Quién aprueba |
| --- | --- | --- |
| Spike exploratorio | Puede entrar sin criterios Given/When/Then ni alcance cerrado, siempre con caja temporal explícita y una pregunta de investigación clara; al cierre eleva su resultado como insumo de una ADR o una US/BT posterior | AG-06 (Scrum Master + Backlog Curator) |
| BT de infraestructura compartida sin US consumidora directa | Puede entrar sin una US 1:1 si su ADR o componente de respaldo está declarado y se asocia a US representativas (BT-10, BT-11, BT-13) | AG-05 (Arquitecto), validado por AG-06 |
| US con dependencia de un mecanismo pendiente del intake | Puede refinarse y estimarse con el supuesto documentado cuando el mecanismo físico concreto está pendiente (por ejemplo, el almacenamiento seguro en reposo, intake §17.P.5, delegado a 09), siempre que el comportamiento esté fijado por la ADR | AG-06, con revisión de AG-05 |

Ninguna excepción exime de la trazabilidad upstream ni de la verificabilidad: un ítem sin fuente upstream o con criterios no verificables no entra al sprint bajo ninguna excepción.

## 4. Aprobador

El rol responsable de validar que un ítem cumple la DoR antes de entrar al Sprint Planning es el Scrum Master + Backlog Curator (AG-06), titular del backlog. Para las excepciones de infraestructura compartida y de dependencias arquitectónicas pendientes, AG-06 valida con el visto de AG-05 (Arquitecto). La trazabilidad US↔CU la firma AG-02 (Analista Funcional) y la verificabilidad de criterios para los acceptance tests la revisa AG-08 (QA), ambas como revisiones acotadas sin transferir la titularidad del artefacto.

## 5. Documentos relacionados

- Vista de producto: [product-backlog_v1.0.md](product-backlog_v1.0.md) — filtro de entrada de las US.
- Vista técnica: [backlog-tecnico_v1.0.md](backlog-tecnico_v1.0.md) — filtro de entrada de las BT.
- Definition of Done: vive en la categoría 08 (testing y QA); la DoR no la reproduce ni la solapa.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Definition of Ready inicial de geovial-storage: siete criterios DoR para US, cinco para BT, tres excepciones admitidas con su aprobador y rol aprobador titular (AG-06), sin solapamiento con la Definition of Done de 08. |
