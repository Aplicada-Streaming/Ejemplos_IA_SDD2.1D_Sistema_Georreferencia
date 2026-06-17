# Definition of Ready — geovial-web

**Proyecto:** geovial-web
**Documento:** definition-of-ready_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Scrum Master

La Definition of Ready (DoR) es el filtro de entrada al Sprint Planning: ningún ítem del backlog entra a un sprint sin cumplirla. Habla de cuándo un ítem está listo para empezar, no de cuándo está terminado; la condición de terminación es la Definition of Done de la categoría 08, con la que esta DoR no se solapa. La DoR aplica sobre las historias del `product-backlog_v1.0.md` y las tareas técnicas del `backlog-tecnico_v1.0.md`. Cada criterio se responde con sí o no de manera objetiva.

## 1. Criterios DoR para historias de usuario

Una historia de usuario está Ready cuando cumple los siguientes criterios:

1. La historia está redactada en el formato Como rol, quiero acción, para valor, con el valor explícito y no un sinónimo de la acción.
2. La historia declara al menos un caso de uso relacionado en su columna CU relacionados y la necesidad de negocio de origen; la trazabilidad no tiene huérfanas.
3. La historia tiene criterios de aceptación en formato Given/When/Then con al menos dos escenarios, incluido al menos un happy path y un edge case (obligatorio para Must y Should; las Could se completan al promoverse).
4. La historia está estimada en story points con la técnica Fibonacci adoptada y su tamaño cabe en un sprint con holgura; si no cabe, se descompone antes de entrar.
5. La historia tiene su prioridad MoSCoW declarada con justificación y no depende de otra historia no terminada del mismo sprint que la bloquee.
6. Las dependencias técnicas de la historia están identificadas en el backlog técnico y las BT bloqueantes están resueltas o planificadas en el mismo sprint o antes.
7. Los datos o el estado de prueba necesarios para verificar los escenarios están disponibles o su provisión está acordada (por ejemplo, un relevamiento en cada estado del ciclo, fotos con y sin ubicación incrustada).

## 2. Criterios DoR para tareas técnicas

Una tarea técnica está Ready cuando cumple los siguientes criterios:

1. La BT declara su justificación upstream: una necesidad de negocio, un caso de uso, una decisión de arquitectura (ADR-01 a ADR-05) o un componente de la arquitectura de solución; sin justificación no hay BT.
2. La BT declara al menos una US consumidora o se justifica explícitamente como infraestructura compartida con la ADR que la sostiene.
3. La BT tiene criterios de aceptación verificables (compila, las pruebas pasan, el contrato se respeta, la deuda queda saldada) y su alcance cabe en menos de un sprint.
4. La BT tiene sus dependencias identificadas (BT o US previas y bloqueos externos) y las bloqueantes están resueltas o planificadas antes.
5. La BT declara su tipo (feature, spike, refactor, devops o docs) y su estimación con la técnica Fibonacci; si es un spike, declara su caja temporal explícita.

## 3. Excepciones admitidas

- Spike exploratorio: una BT de tipo spike puede entrar al sprint sin criterios de aceptación cerrados de implementación, siempre que declare su caja temporal y el resultado esperado (informe, recomendación o decisión a elevar). El caso de BT-06 (integración del componente de mapa) entra bajo esta excepción.
- Historia Could en promoción: una historia Could del product backlog que solo tiene un escenario de referencia puede iniciar refinamiento sin los dos escenarios Given/When/Then completos, pero no entra a Sprint Planning como comprometida hasta completarlos. Aplica a US-17 y US-18.
- Dependencia de Sprint 0: las decisiones de arquitectura en estado Propuesto (ADR-04 y ADR-05) se ratifican en Sprint 0; las BT que dependen de ellas (BT-01, BT-11 y derivadas) pueden refinarse en paralelo, pero su entrada definitiva al sprint queda condicionada a la ratificación.

Toda excepción se registra en la nota del ítem y requiere la aprobación explícita del aprobador de la §4.

## 4. Aprobador

El Scrum Master es el responsable de validar que un ítem cumple la DoR antes de entrar a Sprint Planning, con las revisiones acotadas previstas en la regla 06 §1.3: el Analista Funcional firma la trazabilidad a CU de las historias, el Arquitecto valida la justificación upstream de las BT y QA valida la verificabilidad de los criterios de aceptación. El Scrum Master mantiene la titularidad de la decisión de admisión y aprueba las excepciones de la §3.

## 5. Vinculación cross-doc

- Esta DoR filtra la entrada de los ítems del `product-backlog_v1.0.md` y del `backlog-tecnico_v1.0.md`.
- No se solapa con la Definition of Done de la categoría 08: la DoR define cuándo empezar; la DoD, cuándo terminar.
- El paso de estado Borrador a Ready de una historia o tarea se gobierna por el cumplimiento de esta DoR en la sesión de refinement.

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Definition of Ready inicial de geovial-web: 7 criterios verificables para historias de usuario y 5 para tareas técnicas, excepciones para spike exploratorio, historias Could en promoción y dependencias de Sprint 0, y aprobador declarado (Scrum Master con revisiones acotadas de AG-02, AG-05 y AG-08). |
