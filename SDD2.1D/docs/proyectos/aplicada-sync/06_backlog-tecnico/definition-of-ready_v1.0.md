# Definition of Ready — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** definition-of-ready_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + Backlog Curator (AG-06)

La Definition of Ready (DoR) define cuándo un ítem del backlog está listo para entrar al Sprint Planning de la categoría 07. Habla de las condiciones para empezar; no se solapa con la Definition of Done de la categoría 08, que define las condiciones para terminar. Cada criterio se responde con sí o no de manera objetiva (sin "la historia debe estar clara"). Aplica al `product-backlog_v1.0.md` (US) y al `backlog-tecnico_v1.0.md` (BT).

## 1. Criterios DoR para US

Una historia de usuario está Ready cuando cumple los siete criterios siguientes:

1. La historia está redactada en la forma "Como [rol], quiero [acción], para [valor]" con el valor para el rol explícito y no un sinónimo de la acción.
2. La US referencia al menos un CU de la categoría 02 en su columna CU relacionados, sin quedar huérfana de caso de uso.
3. La US tiene criterios de aceptación en formato Given/When/Then con al menos dos escenarios para las US Must y Should (un happy path y un edge case).
4. La US está estimada en story points con la escala Fibonacci declarada en el `product-backlog_v1.0.md`, y la estimación cabe en un sprint (no es una épica encubierta).
5. La US tiene prioridad MoSCoW declarada y consistente con su aporte al MVP.
6. La US no tiene dependencias bloqueantes sin resolver dentro del mismo sprint, o sus dependencias están explicitadas y secuenciadas (por ejemplo, US-05 antes de US-12).
7. La US identifica las BT que la soportan en la matriz BT↔US↔CU del `backlog-tecnico_v1.0.md` y los datos o dobles de prueba necesarios para verificar sus criterios están disponibles o identificados.

## 2. Criterios DoR para BT

Una tarea técnica está Ready cuando cumple los cinco criterios siguientes:

1. La BT declara su fuente upstream (NB, CU, ADR, componente o contrato de 05); sin justificación upstream no entra al sprint.
2. La BT declara al menos una US consumidora en la matriz BT↔US↔CU, o se justifica explícitamente como infraestructura compartida con la ADR que la sustenta.
3. La BT tiene tipo declarado (feature, spike, refactor, devops o docs); si es spike, lleva caja temporal explícita.
4. La BT tiene criterios de aceptación técnicos verificables (compila, los tests pasan, el contrato se respeta, la deuda queda saldada) y alcance acotado a menos de un sprint.
5. La BT tiene sus dependencias técnicas identificadas y las que la bloquean están terminadas o secuenciadas antes en el mismo backlog.

## 3. Excepciones admitidas

- Spike exploratorio: una BT de tipo spike puede entrar al sprint con criterios de aceptación expresados como preguntas a responder (no como entregables cerrados), siempre que lleve caja temporal explícita y un criterio de cierre del bloqueo si no hay recomendación clara. Aplica a BT-03.
- US Could de endurecimiento: una US Could (US-09, US-11) puede entrar con un solo escenario Given/When/Then en lugar de dos, dado que la regla solo exige dos escenarios para Must y Should; la excepción se documenta en la nota de la US.
- Dependencia upstream en curso: un ítem cuya única dependencia pendiente es la publicación de una versión menor compatible de la superficie pública puede declararse Ready condicional, registrando la dependencia como riesgo a confirmar en el daily.

Toda excepción se registra en la sección de notas del ítem y se revisa en el refinement del sprint siguiente.

## 4. Aprobador

- Aprobador titular de la DoR: Scrum Master + Backlog Curator (AG-06), responsable de validar que un ítem cumple la DoR antes de habilitarlo para el Sprint Planning de 07.
- Revisiones acotadas que el aprobador puede requerir antes de marcar Ready: AG-02 (trazabilidad de la US a CU sin huérfanas), AG-05 (justificación de la BT en ADR, componente o contrato de 05) y AG-08 (verificabilidad de los criterios Given/When/Then para los acceptance tests de 08).
- Aprobador de excepciones: el AG-06 aprueba las excepciones de spike y de US Could; las excepciones de dependencia upstream las co-aprueba con el AG-05 por su impacto en la superficie pública.

## 5. Relación con la Definition of Done

La DoR no se solapa con la Definition of Done de la categoría 08. La DoR verifica que el ítem está suficientemente refinado para empezar (valor, trazabilidad, criterios redactados, estimación, dependencias). La DoD verifica que el ítem terminado cumple sus criterios de aceptación, sus tests pasan y la superficie pública respeta la política de compatibilidad. Un criterio que hable de "los tests pasan" pertenece a la DoD, no a la DoR; la DoR solo exige que los datos o dobles de prueba estén disponibles para poder escribir esos tests.

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | DoR inicial de aplicada-sync: 7 criterios para US, 5 para BT, excepciones (spike, US Could, dependencia upstream), aprobador (AG-06 con revisiones acotadas de AG-02/AG-05/AG-08) y delimitación frente a la Definition of Done de 08. |
