# NB-02 — Gestión y asignación de relevamientos por el jefe de área

| Campo | Valor |
| --- | --- |
| Proyecto | geovial-api |
| Documento | NB-02-gestion-asignacion-relevamientos_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-06-15 |
| Autor | Analista de Negocio + API Product Analyst |
| Trazabilidad upstream | SOLUTION-INTAKE §1, §4; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-04, CU-05, CU-06 (previstas en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

La organización necesita organizar el trabajo de campo alrededor de una unidad clara —el relevamiento de un tramo vial— y poder repartirlo entre los relevadores disponibles. Hoy el reparto de tareas se hace de manera informal, sin un registro de qué tramo le corresponde a cada persona ni del estado en que se encuentra cada trabajo, lo que dificulta saber qué está pendiente, qué está en recolección y qué quedó listo para revisar.

El dolor concreto es que el jefe de área no tiene una forma estructurada de definir el alcance de un relevamiento (que puede abarcar varios puentes y caminos), asignarle agentes y seguir su avance a lo largo del ciclo. Cuando cambian las condiciones del campo —un agente que se enferma, una cuadrilla que se reorganiza— tampoco puede reacomodar las asignaciones con agilidad. Esta falta de gestión se traduce en tramos sin cubrir, esfuerzo duplicado sobre el mismo tramo y demoras en el arranque de la recolección.

La necesidad importa porque ordena el ciclo completo del relevamiento desde su origen: sin una creación y asignación trazable, la recolección en campo y la posterior revisión del jefe carecen de un punto de partida confiable.

## 2. Ejemplo de uso desde la perspectiva del negocio

Un jefe de área planifica el relevamiento de un tramo que incluye dos puentes y un camino vecinal. Crea el relevamiento delimitando ese alcance, deja algunos puntos de referencia iniciales sobre el mapa para orientar a la cuadrilla y asigna a dos relevadores que conocen la zona. A los pocos días, uno de los relevadores queda afectado a otra urgencia; el jefe lo reemplaza por otro agente y el relevamiento continúa sin perder lo ya recolectado. Cuando considera que la recolección está completa, el jefe pasa el relevamiento a la etapa de revisión.

## 3. Impacto

- Da al jefe de área una unidad de trabajo concreta para planificar, repartir y seguir el relevamiento de un tramo vial.
- Evita tramos sin cubrir y esfuerzo duplicado, al dejar explícita la asignación de agentes.
- Permite reacomodar la cuadrilla ante cambios de campo sin perder lo recolectado.
- Hace visible el estado de cada relevamiento dentro de su ciclo (recolección, revisión, cierre).
- Si queda sin resolver, el reparto del trabajo sigue siendo informal y no auditable, y la recolección arranca sin un alcance claro.

## 4. Problema específico que resuelve

- No existe una unidad de trabajo formal que represente el relevamiento de un tramo vial con su alcance de puentes y caminos.
- El reparto de agentes a los tramos es informal y no queda registrado.
- No hay forma ágil de reasignar agentes cuando cambian las condiciones de campo.
- El avance del relevamiento a lo largo de su ciclo no es visible para quien coordina.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Cobertura de tramos planificados | Porcentaje de tramos con relevamiento creado y agentes asignados antes de salir a campo | 100 % | 3 meses post-despliegue |
| Esfuerzo duplicado sobre un mismo tramo | Relevamientos solapados sobre el mismo tramo por trimestre | 0 | continuo |
| Agilidad de reasignación | Minutos para reasignar un agente a un relevamiento existente | ≤ 5 min | 3 meses post-despliegue |
| Visibilidad del ciclo del relevamiento | Estados del ciclo representados y consultables | 3 estados (recolección, revisión, cierre) | release 1.0 |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Vialidad provincial | Propietario | Aprueba que el relevamiento sea la unidad de planificación del trabajo de campo |
| Departamento de desarrollo de software (1 desarrollador) | Implementador | Construye y mantiene la gestión y asignación de relevamientos |
| Jefe de área | Beneficiario | Crea relevamientos, asigna y reasigna agentes y sigue el avance del ciclo |
| Agente de campo | Beneficiario | Recibe relevamientos asignados y confirma que el alcance del tramo es claro |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-02 | CU-04 crear, dar de baja y visualizar relevamientos de un tramo vial | a generar |
| NB-02 | CU-05 asignar y reasignar agentes de campo a un relevamiento | a generar |
| NB-02 | CU-06 transicionar el estado del relevamiento de recolección a revisión y cierre | a generar |

## 8. Dependencias con otras NB

- Depende de NB-01 (administración jerárquica de usuarios y control de acceso): la creación y asignación de relevamientos requiere que existan el rol de jefe de área y los agentes a asignar.

## 9. Prioridad MoSCoW

Must Have. Las capacidades F-03, F-04 y F-11 del intake (§4) son Must Have y la reasignación F-14 es Should Have; sin la gestión y asignación de relevamientos no hay punto de partida para la recolección ni para el cierre.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la necesidad a partir de SOLUTION-INTAKE §1, §4 (F-03, F-04, F-11, F-14) y de la visión y el alcance de la categoría 00. |
