# Visión de Producto

**Proyecto:** GeoVial (solución)
**Documento:** vision-producto_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Product Manager + API Product Owner
**Trazabilidad upstream:** SOLUTION-INTAKE §1, §2, §3, §8, §10, §11, §12
**Trazabilidad downstream:** 01_necesidades_negocio, 02_especificacion_funcional, 03_ux_ui_dx, 05_arquitectura_tecnica, 07_plan-sprint, 11_examples

## 1. Problema de negocio

El relevamiento del estado de los tramos viales —los puentes y caminos que la organización tiene a su cargo— se hace hoy con métodos manuales: planillas en papel y fotografías sueltas sin una georreferencia confiable. Esto rompe la trazabilidad entre cada foto y el punto exacto del tramo donde fue tomada, obliga a retrabajo en oficina para reconstruir esa correspondencia y demora la confección de los informes rutinarios. Además, buena parte del trabajo ocurre en lugares sin conectividad, donde hoy directamente no se puede registrar la observación en el momento.

Si este problema no se resuelve, la organización seguirá produciendo informes lentos, difíciles de reproducir y de auditar, con riesgo de perder o confundir evidencia fotográfica entre relevamientos. El disparador para encarar la solución ahora es la necesidad de estandarizar y digitalizar el relevamiento, de modo que los informes del jefe de área sean trazables y reproducibles, y que la captura en campo deje de depender de la conectividad del lugar.

## 2. Audiencia y stakeholders

La audiencia se organiza en una jerarquía de cuatro niveles, de mayor a menor alcance: usuario raíz, jefe general, jefe de área y agente de campo. El propietario del problema es la Vialidad provincial y el implementador es el departamento de desarrollo de software, que en esta etapa cuenta con un único desarrollador.

| Rol | Nombre o cargo | Categoría | Nivel de involucramiento | Responsabilidad principal |
|---|---|---|---|---|
| Dueño del problema / aprobador | Vialidad provincial | Propietario | Aprobación y patrocinio | Aprueba el intake y la dirección del producto |
| Equipo de desarrollo | Departamento de desarrollo de software (1 desarrollador) | Implementador | Construcción y mantenimiento | Construye y mantiene la solución |
| Usuario raíz | Rol del sistema | Beneficiario | Administración del sistema | Administra todo el sistema con acceso pleno, lo configura y da de alta al jefe general |
| Jefe general | Rol del sistema | Beneficiario | Administración de jefes de área | Administra a los jefes de área |
| Jefe de área | Rol del sistema | Beneficiario | Operación y revisión | Administra agentes de campo, administra y asigna relevamientos, y los revisa para su cierre |
| Agente de campo | Rol del sistema | Beneficiario | Operación en terreno | Toma los relevamientos asignados e ingresa y administra sus observaciones |

## 3. Propuesta de valor

La promesa central de GeoVial es capturar observaciones georreferenciadas en el propio lugar del tramo vial, aun sin conexión, agrupándolas alrededor de un marcador geográfico que reúne fotos, comentarios y etiquetas, y revisarlas después sobre un mapa para confeccionar los informes de cierre.

La propuesta se apoya en estas líneas de valor, derivadas del alcance pretendido:

- Captura en campo que funciona sin conexión y sincroniza por sí sola cuando se recupera el acceso a la red.
- Georreferenciación en el momento de la captura y, en la carga manual, a partir de los datos de ubicación que la propia foto trae consigo, con un radio configurable para agrupar fotos en un mismo marcador.
- Tolerancia a los conflictos de marcadores: el relevamiento se crea y la información queda accesible aunque existan marcadores en conflicto; la decisión de unificarlos o separarlos se difiere al cierre.
- Revisión visual sobre mapa, con un carrusel de fotos por marcador que encadena los marcadores contiguos.
- Exportación e importación de un relevamiento completo —comentarios, etiquetas y fotos— en un único archivo comprimido.

La diferenciación defendible frente a alternativas externas y la caracterización detallada de lo que el cliente hace hoy quedan pendientes en el intake (§3) y se completarán cuando el negocio aporte esa línea de base; no se inventan aquí.

## 4. Visión a 3 años

A tres años, GeoVial es la herramienta estándar de la organización para relevar el estado de sus tramos viales: toda observación de campo nace georreferenciada y trazable, la captura en terreno deja de depender de la conectividad y los informes de cierre se arman sobre evidencia ordenada por marcador y ubicación, no sobre planillas sueltas.

En ese horizonte la solución consolida el ciclo completo de un relevamiento —creación y asignación, recolección en campo, revisión y cierre— para los cuatro niveles de la jerarquía de usuarios, con la información de cada relevamiento autocontenida y portable entre entornos. Quedan deliberadamente fuera de esta visión la evaluación automática del estado del tramo y la guía de desplazamiento del agente en terreno: GeoVial documenta y ordena la evidencia para que la persona responsable decida, no diagnostica ni rutea. La apertura del registro de agentes a un flujo de autogestión se mantiene como posibilidad futura, supeditada a que el negocio lo solicite.

## 5. Objetivos SMART

Los objetivos se derivan de las métricas de negocio del intake (§8). Los targets son objetivos iniciales del proyecto de investigación y se revisan al confirmar la línea de base operativa.

| Objetivo | Métrica | Target | Plazo | Responsable |
|---|---|---|---|---|
| Asegurar la calidad de la georreferenciación | Porcentaje de observaciones con coordenada geográfica válida asociada | ≥ 95 % | 3 meses post-despliegue | Jefe de área |
| Disponer de los datos para revisión sin demora | Tiempo entre el fin de la recolección en campo y la disponibilidad de los datos sincronizados tras recuperar conexión | ≤ 24 h por relevamiento | Por relevamiento | Agente de campo |
| Acelerar el cierre frente al método manual | Reducción del tiempo de confección del informe de cierre respecto del método manual actual | ≥ 30 % | 6 meses post-despliegue | Jefe de área |

## 6. Métricas de éxito

| Criterio | Métrica | Target | Plazo | Fuente del dato |
|---|---|---|---|---|
| Calidad de georreferenciación | Porcentaje de observaciones con coordenada geográfica válida asociada | ≥ 95 % | 3 meses post-despliegue | Registro de observaciones de cada relevamiento |
| Disponibilidad de datos para revisión | Tiempo entre fin de recolección y datos sincronizados tras recuperar conexión | ≤ 24 h | Por relevamiento | Marcas de tiempo de recolección y sincronización |
| Eficiencia del cierre | Reducción del tiempo de confección del informe de cierre respecto del método manual | ≥ 30 % | 6 meses post-despliegue | Medición comparada del cierre antes y después del despliegue |

## 7. Restricciones

Restricciones de negocio declaradas por el cliente (intake §10):

- Sin fecha objetivo. Es un proyecto de investigación educativo, no atado a un hito externo; la cadencia la fija el avance del equipo.
- Sin presupuesto formal asignado. Proyecto de investigación educativo con fines comerciales futuros.
- Sin exigencias regulatorias declaradas.
- Sin integraciones obligatorias con sistemas existentes.

Restricción de equipo: el implementador cuenta con un único desarrollador, lo que condiciona la cadencia y obliga a un alcance incremental y verificable por fases.

## 8. Riesgos

| ID | Riesgo | Probabilidad | Impacto | Mitigación | Responsable |
|---|---|---|---|---|---|
| R-01 | Baja adopción en campo por curva de aprendizaje o desconfianza de los agentes | Media | Alto | Experiencia de uso simple, capacitación y piloto acotado antes del despliegue masivo | Jefe de área |
| R-02 | Georreferenciación imprecisa (señal de ubicación pobre o fotos sin datos de ubicación) que genera marcadores mal ubicados | Media | Medio | Radio de agrupación configurable, edición manual del punto en el mapa y validación del jefe al cierre | Agente de campo |
| R-03 | Pérdida o duplicación de datos en la sincronización sin conexión (cortes, conflictos) | Media | Alto | Cola local persistente, sincronización idempotente que sube primero y baja después, convivencia con conflictos y resolución al cierre | Equipo de desarrollo |

## 9. Glosario del dominio

| Término | Definición | Sinónimos o notas |
|---|---|---|
| Relevamiento | Tarea que registra una serie de observaciones del estado de un tramo vial; tiene un ciclo de recolección, revisión y cierre | — |
| Tramo vial | Extensión a relevar que puede abarcar uno o varios puentes y caminos; es el alcance de un relevamiento | — |
| Observación | Registro anclado a un marcador geográfico, compuesto por notas, comentarios y fotos, con comentario y etiqueta por foto | — |
| Marcador geográfico | Punto en el mapa que agrupa observaciones, fotos, comentarios y textos; es etiquetable y puede ser compartido por varias observaciones | Marcador |
| Conflicto de marcadores | Situación en la que dos o más marcadores caen dentro de un mismo radio; convive con la operación, solo afecta la estructura de catalogación y se resuelve al cierre | — |
| Agente de campo | Persona que toma relevamientos asignados e ingresa y administra sus observaciones, fotos, comentarios y etiquetas en terreno | Relevador |
| Jefe de área | Usuario que administra agentes de campo, administra y asigna relevamientos, y los revisa para su cierre | — |
| Sincronización | Proceso que sube primero los cambios locales del agente y luego baja las últimas actualizaciones de sus relevamientos asignados | — |
| Radio de agrupación | Parámetro que, en la carga manual, agrupa fotos dentro de un mismo marcador según su georreferenciación | — |
| Etiqueta | Marca aplicable a fotos y a marcadores para su filtrado posterior | — |

## 10. Trazabilidad

Upstream: este documento toma su contenido del SOLUTION-INTAKE de GeoVial: §1 (problema y disparador), §2 (audiencia y stakeholders), §3 (propuesta de valor), §8 (métricas de negocio que originan los objetivos SMART y las métricas de éxito), §10 (restricciones del cliente), §11 (riesgos de negocio) y §12 (glosario del dominio).

Downstream: las decisiones de esta visión alimentan 01_necesidades_negocio (necesidades y objetivos del negocio), 02_especificacion_funcional (casos de uso y reglas derivados de la propuesta de valor y del glosario), 03_ux_ui_dx (experiencias de las audiencias identificadas), 05_arquitectura_tecnica (restricciones y riesgos que condicionan decisiones técnicas), 07_plan-sprint (priorización por objetivos) y 11_examples (ejemplos alineados al dominio y al glosario).
