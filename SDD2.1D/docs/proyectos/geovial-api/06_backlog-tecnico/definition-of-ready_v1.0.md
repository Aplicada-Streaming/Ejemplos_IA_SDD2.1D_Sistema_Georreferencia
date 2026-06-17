# Definition of Ready — geovial-api

**Proyecto:** geovial-api
**Documento:** definition-of-ready_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner

La Definition of Ready (DoR) fija cuándo un ítem puede entrar a Sprint Planning. Habla del comienzo, no del final: la Definition of Done de la categoría 08 gobierna cuándo un ítem termina, y esta DoR no se solapa con ella. Cada criterio se responde con sí o no de manera objetiva (regla 06 §4.6, anti-patrón "DoR sin criterios verificables").

## 1. Criterios DoR para US

Una historia de usuario está Ready cuando cumple los siete criterios siguientes:

1. La historia está escrita en formato `Como [rol], quiero [acción], para [valor]` con el valor para el rol explícito y no redundante con la acción.
2. Tiene al menos un CU relacionado declarado en su tabla de trazabilidad (columna `CU relacionados` poblada; sin US huérfana).
3. Tiene criterios de aceptación en formato Given/When/Then con al menos dos escenarios, incluyendo un happy path y un edge case (obligatorio para Must y Should).
4. Está estimada en story points con la técnica Fibonacci adoptada por el equipo y cabe en un sprint con holgura (atributo Small de INVEST); si no cabe, se descompone antes de entrar.
5. Tiene su prioridad MoSCoW asignada por el API Product Owner con justificación.
6. No tiene dependencias bloqueantes sin resolver: las BT y US prerequisito están identificadas y planificadas antes o en el mismo sprint sin acoplarla (atributo Independent de INVEST).
7. Los datos de prueba o el insumo de contrato necesarios para verificarla están identificados o disponibles (fixtures, esquema del DTO, códigos de error implicados).

## 2. Criterios DoR para BT

Una tarea técnica está Ready cuando cumple los cinco criterios siguientes:

1. Tiene fuente upstream declarada (una NB, un CU, una ADR o un contrato de 05); sin justificación upstream no entra (anti-patrón "BT sin justificación").
2. Declara al menos una US consumidora o se justifica explícitamente como infraestructura compartida con la ADR o el contrato que la sostiene.
3. Tiene criterios de aceptación técnicos verificables (compila, los tests pasan, el contrato se respeta, la restricción del almacén se aplica), no expresados como "funciona correctamente".
4. Su alcance es ejecutable en menos de un sprint y sus dependencias técnicas (otras BT) están identificadas.
5. Tiene tipo declarado (feature, spike, refactor, devops o docs) y estimación Fibonacci; si es spike, declara su caja temporal explícita.

## 3. Excepciones admitidas

| Caso | Flexibilización | Aprobador |
| --- | --- | --- |
| Spike exploratorio | Una BT de tipo spike puede entrar sin criterios de aceptación cerrados de resultado, siempre que declare su caja temporal y la pregunta a responder; al cierre del plazo documenta hallazgo o bloqueo. | Scrum Master |
| US Could de épica diferida (EP-06, EP-07, US-30) | Puede entrar al refinement con criterios de aceptación en borrador mientras su prioridad no la habilite al sprint; se completa la DoR antes de planificarla. | API Product Owner |
| Ítem dependiente de una confirmación de negocio pendiente (supuestos abiertos de la especificación funcional §9) | Puede prepararse con el supuesto explícito documentado; no entra al sprint hasta que el supuesto se confirma o se acepta formalmente como decisión de alcance. | API Product Owner |
| BT de infraestructura compartida sin US directa (BT-01, BT-03, BT-20) | Se exime del criterio 2 de US consumidora individual si la ADR o el intake que la sostiene está citado; la justificación de infraestructura compartida reemplaza la US consumidora. | API Product Owner |

Ninguna excepción exime de los criterios de trazabilidad upstream (criterio 2 de US, criterio 1 de BT): un ítem sin fuente no entra al backlog en ningún caso.

## 4. Aprobador

El API Product Owner es el rol responsable de validar que un ítem cumple la DoR antes de entrar a Sprint Planning, con el apoyo del Scrum Master en la facilitación del refinement. La trazabilidad a CU la firma el Analista Funcional (AG-02) y la justificación técnica de cada BT la valida el Arquitecto (AG-05), como revisiones acotadas (regla 06 §1.3). La validación de que los criterios de aceptación son aptos para los acceptance tests la aporta QA (AG-08).

## 5. Relación con la Definition of Done de 08

| Dimensión | Definition of Ready (esta, 06) | Definition of Done (08) |
| --- | --- | --- |
| Momento | Antes de Sprint Planning | Al cierre del ítem en el sprint |
| Foco | El ítem está suficientemente refinado para empezar | El ítem está construido, probado y aceptado |
| Criterios de prueba | Los criterios de aceptación existen y son verificables | Los criterios de aceptación pasan y la cobertura cumple el gate |
| Titular | API Product Owner | Equipo de desarrollo y QA |

No hay solapamiento: esta DoR no exige que los tests pasen (eso es DoD), solo que existan y sean verificables.

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | DoR inicial de geovial-api: siete criterios verificables para US y cinco para BT, excepciones admitidas con aprobador, rol aprobador declarado y delimitación frente a la Definition of Done de 08. |
