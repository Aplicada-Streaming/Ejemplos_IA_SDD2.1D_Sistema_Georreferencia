# Definition of Done — geovial-api

**Proyecto:** geovial-api
**Documento:** definition-of-done_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (variante API Testing Specialist)

## 0. Vigencia y alcance

Este documento es la fuente canónica de la Definition of Done de `geovial-api`. La DoD habla del final (cuándo un ítem termina), en complemento de la Definition of Ready de 06 (cuándo un ítem puede empezar), con la que no se solapa. El mini-plan de 07 (`mini-plan_v1.0.md` §5) referencia esta DoD por enlace y no la redefine; ningún plan de tramo puede reescribir estos criterios (08 §4.10, anti-patrón "DoD redefinida en cada sprint plan"). Las capas son cuatro: US (historia de usuario), BT (tarea técnica), sprint→tramo (incremento de construcción) y release. Cada criterio responde a la pregunta "¿cómo se valida?" con una operación mecánica concreta.

Como el proyecto se construye por tramos (07 §3) en lugar de sprints de calendario (equipo_n=1), la capa "sprint" de la DoD canónica se aplica al cierre de cada tramo.

## 1. DoD por capa

### 1.1 DoD de US (historia de usuario)

Una US está Done cuando:

- [ ] El código que implementa la US compila sin warnings tratados como error. Validación: gate G1 del pipeline.
- [ ] Todos los criterios de aceptación Given/When/Then de la US (06, `historias-usuario/`) tienen su test automatizado y pasan. Validación: ejecución de la suite; el test referencia la US y su CU.
- [ ] La US tiene su TC asociado en `casos-prueba-referenciales_v1.0.md` y está reflejado en la matriz CU↔Tests. Validación: la matriz lista el TC y su estado es Verde.
- [ ] Si la US toca un endpoint público, ese endpoint tiene su contract test en verde. Validación: gate G4; TC-34 del recurso correspondiente pasa.
- [ ] La cobertura por capa del código de la US cumple los pisos (dominio 85/80, aplicación 80/70, infraestructura 70/60). Validación: reporte de cobertura por capa (gate G3).
- [ ] Cada RN que la US toca tiene su invariante verificada por un test. Validación: matriz RN↔Tests; el TC de la RN pasa.
- [ ] Si la US cerró un bug, existe un TC de regresión nuevo o extendido que lo previene. Validación: el TC está en la suite y en verde.
- [ ] No hay defecto blocker abierto atribuible a la US. Validación: tablero de defectos sin blocker para la US.

### 1.2 DoD de BT (tarea técnica)

Una BT está Done cuando:

- [ ] Cumple sus criterios de aceptación técnicos verificables declarados en su ficha (06, `backlog-tecnico_v1.0.md` §2): compila, los tests pasan, el contrato se respeta o la restricción del almacén se aplica. Validación: ejecución de los criterios técnicos de la ficha.
- [ ] La BT tiene tests automatizados que ejercitan su comportamiento (no "funciona correctamente" sin verificación). Validación: la suite incluye al menos un test con assert para la BT.
- [ ] La cobertura por capa del código de la BT cumple los pisos correspondientes. Validación: reporte de cobertura por capa (gate G3).
- [ ] Si la BT habilita o modifica un endpoint público, su contract test pasa. Validación: gate G4.
- [ ] Si la BT es de infraestructura compartida sin US directa (p. ej. BT-01, BT-03, BT-20), cita la ADR o el contrato que la sostiene. Validación: trazabilidad upstream en la ficha de la BT.
- [ ] El análisis estático no introduce issues críticos nuevos. Validación: gate G6.

Excepción de BT spike: una BT de tipo spike se da por Done cuando documenta el hallazgo o el bloqueo dentro de su caja temporal, aunque no produzca código de producción (06, DoR §3).

### 1.3 DoD de sprint→tramo (incremento de construcción)

Un tramo (07 §3, Tramos 1 a 4) está Done cuando:

- [ ] Todas las US Must comprometidas del tramo están Done según §1.1. Validación: la matriz CU↔Tests muestra sus TC en verde.
- [ ] Todas las BT del tramo están Done según §1.2. Validación: fichas de BT con criterios técnicos cumplidos.
- [ ] La suite completa del tramo pasa en verde y ningún test verde de la revisión anterior pasó a rojo sin justificación. Validación: gates G2 y G7.
- [ ] El 100 % de los endpoints públicos introducidos en el tramo tiene contract test en verde. Validación: gate G4; inventario de endpoints del tramo en la matriz §5.2.
- [ ] La cobertura por capa acumulada del tramo cumple los pisos y el agregado global (líneas ≥ 80 % / branches ≥ 70 %). Validación: gate G3.
- [ ] La matriz de cobertura (`matriz-cobertura-pruebas_v1.0.md`) se actualizó con el estado real de los TC del tramo. Validación: la matriz no dice "Pendiente" donde hay tests implementados (08 §4.10).
- [ ] Los criterios de transición de fase del roadmap (00 §5) del incremento correspondiente se satisfacen. Validación: checklist de transición de fase (07 §8).
- [ ] Las US Should/Could diferidas del tramo quedaron registradas con motivo en la bitácora (07 §9). Validación: bitácora del tramo.

### 1.4 DoD de release

Un release está Done cuando:

- [ ] Todos los criterios funcionales, no funcionales, de regresión y de calidad de código de `criterios-validacion_v1.0.md` se cumplen. Validación: checklist de criterios de validación firmado.
- [ ] Cada NFR numérico se midió y cumple en el ambiente equivalente al productivo. Validación: gate G8; TC-21, TC-22, TC-31, TC-29/TC-30, TC-33 en verde.
- [ ] El 100 % de los 35 endpoints públicos tiene contract test en verde y la especificación OpenAPI valida contra la implementación. Validación: gates G4 y G5; TC-34 completo.
- [ ] La suite de regresión completa está en verde. Validación: gate G2 sobre toda la suite.
- [ ] La cobertura por capa se reporta y cumple; el gate global del pipeline está en verde. Validación: gate G3 con reporte por capa.
- [ ] Toda excepción a un criterio está documentada en una ADR con plan de remediación. Validación: `criterios-validacion_v1.0.md` §6 y la ADR enlazada.
- [ ] El artefacto publicable (imagen de contenedor, intake §17.P.7) se construye y firma por el pipeline. Validación: stages de empaquetado y firma (09).
- [ ] El versionado del contrato respeta la política de compatibilidad (sin breaking change a mitad de versión mayor). Validación: TC-35 y gate G4 de compatibilidad.

## 2. Excepciones admitidas

Se puede declarar Done sin cumplir un criterio solo en los casos siguientes, siempre con registro explícito:

| Caso | Flexibilización | Registro requerido | Aprobador |
| --- | --- | --- | --- |
| Deuda técnica conocida | Un criterio de calidad de código no crítico puede quedar pendiente | BT explícita en el backlog con el criterio pendiente y su plan | API Product Owner |
| US Could diferida (CU-15, CU-16, CU-17) | La US no entra al alcance del release | Nota en la bitácora del tramo (07 §9); no afecta el MVP | API Product Owner |
| Mutation score ausente | No es gate en v1.0 | Métrica observada; gap planificable en la matriz §6 | QA/SDET |
| Umbral de cobertura por capa bajado | Solo con ADR que lo justifique | ADR de cambio de umbral + BT de remediación (08 §2.2) | QA/SDET + Arquitecto |
| BT spike sin código de producción | Se acepta el hallazgo documentado | Documento de hallazgo o bloqueo dentro de la caja temporal | Scrum Master |

Ninguna excepción exime de: los criterios funcionales del MVP (NB-01 a NB-05), los contract tests del 100 % de endpoints, ni la trazabilidad de cada TC a un CU, RN o NFR. Un ítem sin test con assert no está Done en ningún caso (08 §4.10).

## 3. Relación con otros documentos

- La DoR de 06 (`definition-of-ready_v1.0.md`) gobierna la entrada; esta DoD, la salida. No se solapan.
- Los criterios de release detallados viven en `criterios-validacion_v1.0.md`; la capa §1.4 los referencia, no los duplica.
- Los gates G1 a G8 se definen en `estrategia-calidad_v1.0.md` §3 y se materializan en el pipeline de 09.
- El mini-plan de 07 referencia esta DoD por enlace (07 §5); cuando esta DoD pasa a estado Vigente, el mini-plan la cita como su fuente canónica.

## 9. Registro de cambios de criterios versionables

Cualquier cambio en los criterios de esta DoD se registra acá y se comunica en la revisión del tramo siguiente (08 §3.4).

| Versión | Fecha | Cambio en los criterios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Versión inicial: DoD por las cuatro capas (US, BT, sprint→tramo, release), cada criterio con su operación de validación mecánica (gates G1 a G8, TC y matriz), excepciones admitidas con aprobador y registro, y delimitación frente a la DoR de 06 y a los criterios de validación de release. Es la fuente canónica referenciada por el mini-plan de 07. |
