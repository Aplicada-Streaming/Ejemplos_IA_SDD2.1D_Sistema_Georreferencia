# NB-05 — Revisión sobre mapa y cierre con resolución de conflictos

| Campo | Valor |
| --- | --- |
| Proyecto | geovial-api |
| Documento | NB-05-revision-mapa-cierre-conflictos_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-06-15 |
| Autor | Analista de Negocio + API Product Analyst |
| Trazabilidad upstream | SOLUTION-INTAKE §1, §3, §4, §8; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-12, CU-13, CU-14 (previstas en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

La organización necesita que el jefe de área pueda revisar la evidencia recolectada de forma visual y ordenada para confeccionar sus informes rutinarios y cerrar el relevamiento. Hoy esa confección es lenta y difícil de reproducir, porque la información llega como planillas y fotos sueltas que hay que recomponer manualmente. El jefe necesita ver las observaciones sobre un mapa, recorrer las fotos de cada marcador y de los marcadores contiguos, y filtrar por etiqueta para encontrar lo relevante.

El dolor concreto tiene dos caras. Por un lado, la falta de una vista de revisión que recorra la evidencia en su contexto geográfico, que es lo que permite armar un informe trazable. Por el otro, la necesidad de resolver al cierre los conflictos de marcadores —dos o más marcadores que caen dentro de un mismo radio— que durante la recolección convivieron sin bloquear el acceso a la información. El negocio quiere que esa decisión de unificar o separar quede en manos del jefe, en el momento del cierre, y no antes.

La necesidad importa porque cierra el ciclo del relevamiento: es el punto donde la evidencia recolectada se transforma en un informe reproducible, que es el objetivo último del negocio.

## 2. Ejemplo de uso desde la perspectiva del negocio

Un jefe de área abre sobre el mapa un relevamiento que su cuadrilla terminó de recolectar. Recorre los marcadores siguiendo el camino, mira en un carrusel las fotos de cada uno encadenando con el marcador siguiente, y filtra por la etiqueta de fisuras para concentrarse en los puntos críticos. Al llegar al cierre, advierte que dos marcadores muy próximos describen la misma junta del puente desde ángulos distintos; decide unificarlos en uno solo. Una vez resueltos los conflictos pendientes, cierra el relevamiento y queda en condiciones de confeccionar su informe sobre evidencia ordenada.

## 3. Impacto

- Da al jefe una vista de revisión sobre mapa que ordena la evidencia por marcador y ubicación.
- Acelera y hace reproducible la confección de los informes de cierre frente al método manual.
- Pone la decisión de unificar o separar marcadores en conflicto en el momento del cierre, a cargo del jefe.
- Mantiene la información accesible durante toda la recolección, sin bloquear por conflictos.
- Si queda sin resolver, los informes siguen siendo lentos y poco reproducibles y los conflictos no tienen un punto formal de resolución.

## 4. Problema específico que resuelve

- No existe una vista que permita revisar la evidencia en su contexto geográfico sobre un mapa.
- Recorrer y comparar las fotos de marcadores contiguos es engorroso con fotos sueltas.
- Los conflictos de marcadores no tienen un momento ni un responsable definidos para resolverse.
- El cierre del relevamiento no está formalizado como hito que habilita el informe.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Eficiencia del cierre | Reducción del tiempo de confección del informe de cierre respecto del método manual | ≥ 30 % | 6 meses post-despliegue |
| Resolución de conflictos al cierre | Porcentaje de relevamientos cerrados sin conflictos de marcadores pendientes | 100 % | release 1.0 |
| Cobertura de la revisión sobre mapa | Marcadores de un relevamiento revisables en su contexto sobre el mapa | 100 % | release 1.0 |
| Accesibilidad durante la recolección | Relevamientos con información accesible pese a tener marcadores en conflicto | 100 % | continuo |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Vialidad provincial | Propietario | Aprueba el cierre con resolución de conflictos como hito que habilita el informe |
| Departamento de desarrollo de software (1 desarrollador) | Implementador | Construye y mantiene la revisión sobre mapa y la resolución de conflictos al cierre |
| Jefe de área | Beneficiario | Revisa sobre el mapa, resuelve los conflictos y cierra el relevamiento |
| Agente de campo | Beneficiario | Aporta la evidencia recolectada que el jefe revisa y valida |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-05 | CU-12 revisar observaciones sobre mapa con carrusel de fotos por marcador | a generar |
| NB-05 | CU-13 resolver conflictos de marcadores al cierre del relevamiento | a generar |
| NB-05 | CU-14 cerrar el relevamiento como hito que habilita el informe | a generar |

## 8. Dependencias con otras NB

- Depende de NB-03 (captura georreferenciada de observaciones): la revisión opera sobre la evidencia georreferenciada y sus marcadores.
- Depende de NB-04 (trabajo sin conexión con sincronización): la revisión requiere que lo recolectado en campo ya esté sincronizado.

## 9. Prioridad MoSCoW

Must Have. La capacidad F-11 (transición a revisión y cierre) del intake (§4) es Must Have y materializa la métrica de eficiencia del cierre (§8); las capacidades de carrusel (F-12) y resolución de conflictos (F-13) son Should Have y enriquecen esta misma necesidad sin alterar su prioridad fundacional.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la necesidad a partir de SOLUTION-INTAKE §1, §3, §4 (F-11, F-12, F-13), §8 y de la visión y el alcance de la categoría 00. |
