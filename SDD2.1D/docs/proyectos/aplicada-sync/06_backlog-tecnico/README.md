# 06 Backlog técnico — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** README.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + Backlog Curator (AG-06)

Punto de entrada navegable del backlog del motor de sincronización `aplicada-sync` (tipo `library`, redistribuible). El backlog organiza el trabajo por superficie de la capa Abstractions y por capacidad del motor interno, y traza cada historia a un CU de 02 y cada tarea técnica a un ADR, componente o contrato de 05. US y BT viven inline por estar por debajo de los umbrales de 20 US y 30 BT (§3.3 de las reglas).

## Documentos de la sección

- [product-backlog_v1.0.md](product-backlog_v1.0.md) — objetivos y MVP, épicas EP-XX, 13 US inline con MoSCoW, SP Fibonacci, criterios Given/When/Then e INVEST, métricas de avance y refinement.
- [backlog-tecnico_v1.0.md](backlog-tecnico_v1.0.md) — épicas técnicas ET-XX, 14 BT inline con fuente upstream y dependencias, y matriz BT↔US↔CU.
- [definition-of-ready_v1.0.md](definition-of-ready_v1.0.md) — criterios DoR para US (7) y para BT (5), excepciones y aprobador.

## Épicas vigentes

| EP | Nombre | Superficie / capacidad | US |
| --- | --- | --- | --- |
| EP-01 | Sesión de sincronización | Inicializar sesión + contrato de configuración | US-01, US-02 |
| EP-02 | Encolado de cambios locales | Encolar cambio + contrato de cola | US-03, US-04 |
| EP-03 | Motor subir-luego-bajar | Ejecutar sincronización + orquestador del ciclo | US-05, US-06, US-07 |
| EP-04 | Disparo automático por conectividad | Habilitar disparo + observador de conectividad | US-08, US-09 |
| EP-05 | Observabilidad de estado y cola | Consultar estado y cola + registro de estado | US-10, US-11 |
| EP-06 | Reanudación tras interrupción | Reanudar sincronización + marca de progreso | US-12, US-13 |

## US Must Have del MVP

| US | Título | SP | CU |
| --- | --- | --- | --- |
| US-01 | Inicializar una sesión con estrategias obligatorias | 5 | CU-01 |
| US-03 | Encolar un cambio local con identificador estable | 3 | CU-02 |
| US-04 | Reencolar sin duplicar por identificador | 3 | CU-02 |
| US-05 | Ejecutar el ciclo subir-luego-bajar en orden | 8 | CU-03 |
| US-06 | Convivir con estados en conflicto sin abortar | 5 | CU-03 |
| US-12 | Reanudar una subida interrumpida sin duplicar | 8 | CU-06 |

El MVP (32 SP) entrega las garantías que NB-04 exige: integridad 0 perdidos y 0 duplicados, orden subir-antes-de-bajar y convivencia con conflicto sin bloqueo.

## BT prioritarias (prioridad Alta)

| BT | Título | Épica técnica | Fuente |
| --- | --- | --- | --- |
| BT-01 | Contratos de extensión de la capa Abstractions | ET-01 | ADR-01, ADR-02 |
| BT-02 | Operación de inicialización y armado de configuración | ET-01 | Contrato §3; CU-01 |
| BT-04 | Cola persistente única por identificador estable | ET-02 | ADR-04, ADR-07 |
| BT-06 | Orquestación del ciclo de dos fases en orden estricto | ET-03 | ADR-05, RN-01 |
| BT-07 | Ejecutores de fase de subida y de bajada | ET-03 | ADR-05, ADR-07 |
| BT-09 | Marca de progreso y reanudación desde el corte | ET-04 | ADR-06, RN-01 |

## DoR vigente

DoR v1.0: 7 criterios para US y 5 para BT, con excepciones de spike (BT-03), US Could (US-09, US-11) y dependencia upstream. Aprobador titular: AG-06, con revisiones acotadas de AG-02 (trazabilidad a CU), AG-05 (justificación en 05) y AG-08 (verificabilidad para 08). Detalle en [definition-of-ready_v1.0.md](definition-of-ready_v1.0.md).

## Convenciones aplicadas

- Identificadores de dos dígitos uniformes: US-01 a US-13, BT-01 a BT-14, EP-01 a EP-06, ET-01 a ET-06. Sin rastros del patrón heredado `BT-001`.
- Estimación Fibonacci declarada y mantenida en todo el backlog; los spikes llevan caja temporal.
- MoSCoW con reparto realista: 6 Must, 5 Should, 2 Could, 0 Won't.
- Vocabulario neutral de librería; sin stacks, productos ni protocolos concretos (viven en el intake §17 y en 11).

## Trazabilidad upstream/downstream

- Upstream: NB-04 (01); especificación funcional, CU-01 a CU-06 y RN-01 a RN-03 (02); arquitectura, ADR-01 a ADR-08, contratos-abstractions y extensibilidad (05).
- Downstream: 07 (sprint plan, asignación a sprint y velocity), 08 (acceptance tests desde los escenarios Given/When/Then).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 06 de aplicada-sync: índice de los tres artefactos, épicas vigentes, US Must Have del MVP, BT prioritarias y DoR vigente. |
