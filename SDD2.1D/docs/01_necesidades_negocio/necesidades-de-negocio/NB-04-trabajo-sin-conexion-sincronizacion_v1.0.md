# NB-04 — Trabajo sin conexión con sincronización confiable

| Campo | Valor |
| --- | --- |
| Proyecto | geovial-api |
| Documento | NB-04-trabajo-sin-conexion-sincronizacion_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-06-15 |
| Autor | Analista de Negocio + API Product Analyst |
| Trazabilidad upstream | SOLUTION-INTAKE §1, §3, §4, §8, §11; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-10, CU-11 (previstas en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

La organización necesita poder relevar tramos viales en lugares donde no hay conectividad, que es donde ocurre buena parte del trabajo de campo. Hoy, en esos lugares, directamente no se puede registrar la observación en el momento: el relevador anota en papel o pospone la carga, lo que vuelve a romper la trazabilidad y agrega demoras hasta que la información llega a quien la revisa. La dependencia de la red en terreno es una limitación que el negocio quiere eliminar.

El dolor concreto es la imposibilidad de capturar en el momento sin internet y la falta de un mecanismo que, una vez recuperado el acceso a la red, ponga la información recolectada a disposición del jefe sin pérdidas ni duplicaciones. El negocio necesita que el trabajo de campo continúe sin conexión y que la sincronización posterior sea predecible: primero suben los cambios que se hicieron en terreno y luego bajan las actualizaciones del relevamiento asignado, conviviendo con eventuales conflictos sin bloquear la operación.

La necesidad importa porque sin trabajo sin conexión confiable la captura georreferenciada (NB-03) no puede ejercitarse donde más se necesita, y porque la pérdida o duplicación de datos en la sincronización es uno de los riesgos de mayor impacto identificados por el negocio.

## 2. Ejemplo de uso desde la perspectiva del negocio

Una cuadrilla pasa la jornada relevando un camino rural alejado, sin señal. Durante todo el día los relevadores registran observaciones, fotos y notas con normalidad, como si estuvieran conectados. Al regresar hacia una zona con cobertura, la información recolectada se envía por sí sola: primero se suben las observaciones del día y luego se incorporan las novedades que el jefe pudo haber hecho sobre ese relevamiento. A la mañana siguiente, el jefe ya tiene en su pantalla de revisión todo lo recolectado, sin haber tenido que pedir nada a la cuadrilla.

## 3. Impacto

- Habilita la captura en terreno donde no hay conectividad, eliminando la dependencia de la red en campo.
- Pone la información recolectada a disposición del jefe poco después de recuperar conexión, sin gestión manual.
- Reduce el riesgo de pérdida o duplicación de datos mediante un orden de sincronización predecible.
- Permite que la operación continúe aun cuando existan conflictos, difiriendo su resolución.
- Si queda sin resolver, los tramos sin cobertura siguen sin poder relevarse en el momento y las demoras de los informes persisten.

## 4. Problema específico que resuelve

- En lugares sin conectividad no se puede registrar la observación en el momento.
- No hay un mecanismo que sincronice por sí solo lo recolectado al recuperar la red.
- El orden de la sincronización (subir primero, bajar después) no está garantizado, lo que abre la puerta a pérdidas o duplicaciones.
- La operación se bloquea ante conflictos en lugar de convivir con ellos hasta el cierre.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Continuidad de la captura sin conexión | Porcentaje de observaciones capturables sin red sobre el total intentado en campo | 100 % | release 1.0 |
| Disponibilidad de datos para revisión | Tiempo entre el fin de la recolección y los datos sincronizados tras recuperar conexión | ≤ 24 h | por relevamiento |
| Integridad de la sincronización | Observaciones perdidas o duplicadas por relevamiento sincronizado | 0 | continuo |
| Convivencia con conflictos | Sincronizaciones que se bloquean ante un conflicto de marcadores | 0 % | release 1.0 |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Vialidad provincial | Propietario | Aprueba el trabajo sin conexión como capacidad central del relevamiento |
| Departamento de desarrollo de software (1 desarrollador) | Implementador | Construye y mantiene la captura sin conexión y la sincronización subir-luego-bajar |
| Agente de campo | Beneficiario | Releva sin conexión y valida que la sincronización no pierda ni duplique su trabajo |
| Jefe de área | Beneficiario | Recibe los datos sincronizados a tiempo para iniciar la revisión |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-04 | CU-10 capturar y administrar observaciones sin conexión en terreno | a generar |
| NB-04 | CU-11 sincronizar subiendo cambios locales y bajando luego las actualizaciones del relevamiento | a generar |

## 8. Dependencias con otras NB

- Depende de NB-03 (captura georreferenciada de observaciones): la sincronización transporta las observaciones georreferenciadas capturadas en terreno.

## 9. Prioridad MoSCoW

Must Have. La capacidad F-07 del intake (§4) es Must Have y el trabajo sin conexión es la condición que hace viable la captura en campo y mitiga el riesgo R-03 de pérdida o duplicación de datos.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la necesidad a partir de SOLUTION-INTAKE §1, §3, §4 (F-07), §8, §11 y de la visión y el alcance de la categoría 00. |
