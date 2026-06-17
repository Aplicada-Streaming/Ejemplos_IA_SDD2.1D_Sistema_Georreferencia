# Definition of Ready — geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** definition-of-ready_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Scrum Master + Mobile Lead

La Definition of Ready (DoR) define cuándo un ítem del backlog está listo para entrar al Sprint Planning de la categoría 07. Habla de las condiciones para empezar; no se solapa con la Definition of Done de la categoría 08, que define las condiciones para terminar. Cada criterio se responde con sí o no de manera objetiva (sin "la historia debe estar clara"). Aplica al `product-backlog_v1.0.md` (US) y al `backlog-tecnico_v1.0.md` (BT).

## 1. Criterios DoR para US

Una historia de usuario está Ready cuando cumple los siete criterios siguientes:

1. La historia está redactada en la forma "Como [rol], quiero [acción], para [valor]" con el valor para el agente de campo explícito y no un sinónimo de la acción.
2. La US referencia al menos un CU de la categoría 02 en su columna CU relacionados, sin quedar huérfana de caso de uso.
3. La US tiene criterios de aceptación en formato Given/When/Then con al menos dos escenarios para las US Must y Should (un happy path y un edge case, incluyendo el camino degradado offline o de permiso cuando aplica).
4. La US está estimada en story points con la escala Fibonacci declarada en el `product-backlog_v1.0.md`, y la estimación cabe en un sprint (no es una épica encubierta).
5. La US tiene prioridad MoSCoW declarada y consistente con su aporte al MVP de campo.
6. La US no tiene dependencias bloqueantes sin resolver dentro del mismo sprint, o sus dependencias están explicitadas y secuenciadas (por ejemplo, US-01 antes de US-02; el esquema del almacén local antes de la captura).
7. La US identifica las BT que la soportan en la matriz BT↔US↔CU del `backlog-tecnico_v1.0.md` y los datos o dobles de prueba necesarios para verificar sus criterios están disponibles o identificados (por ejemplo, un doble del adaptador de ubicación, de cámara o del motor de sincronización para probar sin dispositivo físico ni red).

## 2. Criterios DoR para BT

Una tarea técnica está Ready cuando cumple los cinco criterios siguientes:

1. La BT declara su fuente upstream (NB, CU, RN, ADR, componente, modelo lógico o contrato consumido de 05); sin justificación upstream no entra al sprint.
2. La BT declara al menos una US consumidora en la matriz BT↔US↔CU, o se justifica explícitamente como infraestructura compartida con la ADR que la sustenta (por ejemplo, BT-01 con ADR-02, BT-11 con ADR-03).
3. La BT tiene tipo declarado (feature, spike, refactor, devops o docs); si es spike, lleva caja temporal explícita y un criterio de cierre del bloqueo.
4. La BT tiene criterios de aceptación técnicos verificables (el esquema se reconstruye, la transacción local es atómica, la degradación no inventa datos, el ciclo respeta el orden subir-antes-de-bajar, el contrato consumido se respeta) y alcance acotado a menos de un sprint.
5. La BT tiene sus dependencias técnicas identificadas y las que la bloquean están terminadas o secuenciadas antes en el mismo backlog (por ejemplo, BT-01 antes de BT-03; BT-11 antes de BT-12).

## 3. Excepciones admitidas

- US Could con un solo escenario: una US Could (US-10, US-15) puede entrar al sprint con un solo escenario Given/When/Then en lugar de dos, dado que la regla exige dos escenarios solo para las US Must y Should; la excepción se documenta en la nota de la US.
- Spike exploratorio: si en un refinement futuro se desglosa una BT de tipo spike (por ejemplo, para evaluar un adaptador de plataforma o el comportamiento del componente de mapa offline), puede entrar al sprint con criterios de aceptación expresados como preguntas a responder en lugar de entregables cerrados, siempre que lleve caja temporal explícita y un criterio de cierre del bloqueo si no hay recomendación clara. El backlog v1.0 no incluye spikes; la excepción queda disponible.
- Dependencia de contrato consumido en curso: un ítem cuya única dependencia pendiente es la publicación de una versión menor compatible del contrato de la librería de sincronización o del contrato REST puede declararse Ready condicional, registrando la dependencia como riesgo a confirmar en el daily.
- Confirmación de supuesto abierto: un ítem que dependa de uno de los supuestos abiertos de 02 §9 (foto sin metadatos de ubicación, captura sin señal de ubicación, corte durante la subida, cierre con cambios sin sincronizar, conflictos entre agentes) puede declararse Ready con el supuesto explícito registrado en su nota, a confirmar con el negocio sin alterar la estructura del ítem.

Toda excepción se registra en la sección de notas del ítem y se revisa en el refinement del sprint siguiente.

## 4. Aprobador

- Aprobador titular de la DoR: Scrum Master + Mobile Lead, responsable de validar que un ítem cumple la DoR antes de habilitarlo para el Sprint Planning de 07. Con equipo_n = 1, el rol único sostiene la curaduría apoyado en las revisiones acotadas.
- Revisiones acotadas que el aprobador puede requerir antes de marcar Ready: AG-02 (trazabilidad de la US a CU sin huérfanas), AG-05 (justificación de la BT en ADR, componente, modelo lógico o contrato consumido de 05) y AG-08 (verificabilidad de los criterios Given/When/Then para los acceptance tests de 08).
- Aprobador de excepciones: el Scrum Master + Mobile Lead aprueba las excepciones de US Could, de spike y de supuesto abierto; las excepciones de dependencia de contrato consumido las co-aprueba con AG-05 por su impacto en la integración con los contratos externos.

## 5. Relación con la Definition of Done

La DoR no se solapa con la Definition of Done de la categoría 08. La DoR verifica que el ítem está suficientemente refinado para empezar (valor, trazabilidad a CU, criterios redactados, estimación, dependencias, dobles de prueba identificados). La DoD verifica que el ítem terminado cumple sus criterios de aceptación, sus tests pasan, la captura funciona sin conexión, la sincronización respeta el orden subir-antes-de-bajar y el contrato consumido se respeta. Un criterio que hable de "los tests pasan" o de "la captura offline se verifica en el dispositivo" pertenece a la DoD, no a la DoR; la DoR solo exige que los datos o dobles de prueba estén disponibles para poder escribir esos tests.

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | DoR inicial de geovial-mobile: 7 criterios para US, 5 para BT, excepciones (US Could, spike, dependencia de contrato consumido y supuesto abierto de 02 §9), aprobador (Scrum Master + Mobile Lead con revisiones acotadas de AG-02/AG-05/AG-08) y delimitación frente a la Definition of Done de 08. |
