# NB-07 — Almacenamiento de archivos configurable

| Campo | Valor |
| --- | --- |
| Proyecto | geovial-api |
| Documento | NB-07-almacenamiento-archivos-configurable_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-06-15 |
| Autor | Analista de Negocio + API Product Analyst |
| Trazabilidad upstream | SOLUTION-INTAKE §3, §4; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-17 (prevista en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

La organización necesita poder decidir dónde se guardan las fotografías de los relevamientos, porque el volumen de evidencia fotográfica crece con cada tramo relevado y el lugar adecuado para alojarla depende del contexto: a veces conviene mantenerla dentro de la propia infraestructura y otras veces conviene delegarla en un servicio de alojamiento externo por costo, capacidad o disponibilidad. Hoy esa decisión estaría fijada de antemano, sin posibilidad de adaptarla a las condiciones de cada despliegue.

El dolor concreto es la falta de control del negocio sobre el destino del almacenamiento de los archivos. Sin esa configurabilidad, la organización queda atada a una única opción y no puede acompañar el crecimiento del volumen ni optimizar el costo de resguardar la evidencia a lo largo del tiempo. La elección del destino debe quedar a cargo del usuario raíz, que es quien configura el sistema.

La necesidad importa para la sostenibilidad operativa y económica del almacenamiento de evidencia, pero no integra el camino principal del relevamiento, por lo que se aborda si la cadencia del proyecto lo permite.

## 2. Ejemplo de uso desde la perspectiva del negocio

En el arranque, la organización despliega el sistema con las fotografías alojadas dentro de su propia infraestructura, porque el volumen es bajo y prefiere mantener todo en casa. Al cabo de un año, con muchos relevamientos acumulados, el costo y el espacio empiezan a pesar; el usuario raíz cambia la configuración para que las nuevas fotografías se alojen en un servicio externo de mayor capacidad, sin que los relevadores ni los jefes noten diferencia alguna en su trabajo.

## 3. Impacto

- Da al negocio control sobre dónde se aloja la evidencia fotográfica, según costo, capacidad y contexto del despliegue.
- Permite acompañar el crecimiento del volumen de fotos sin rehacer la solución.
- Mantiene transparente para los demás roles el lugar donde se guardan los archivos.
- Si queda sin resolver, el destino del almacenamiento queda fijo y la organización pierde margen para optimizar costo y capacidad, aunque el camino principal del negocio no se vea afectado.

## 4. Problema específico que resuelve

- El destino del almacenamiento de las fotografías estaría fijado y no se puede adaptar a cada despliegue.
- No hay forma de migrar el alojamiento al crecer el volumen sin impacto operativo.
- La decisión sobre el destino del almacenamiento no tiene un responsable definido en la jerarquía.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Opciones de destino disponibles | Destinos de almacenamiento configurables | ≥ 2 | release de la capacidad |
| Transparencia para los demás roles | Cambios de comportamiento percibidos por agentes y jefes al cambiar el destino | 0 | release de la capacidad |
| Responsable del cambio de destino | Roles habilitados para cambiar el destino del almacenamiento | 1 (usuario raíz) | release de la capacidad |
| Esfuerzo de cambio de destino | Horas de interrupción del servicio para cambiar el destino | ≤ 1 h | 12 meses post-despliegue |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Vialidad provincial | Propietario | Aprueba que el destino del almacenamiento de evidencia sea configurable según costo y contexto |
| Departamento de desarrollo de software (1 desarrollador) | Implementador | Construye y mantiene la configurabilidad del destino de almacenamiento |
| Usuario raíz | Beneficiario | Configura y cambia el destino del almacenamiento de archivos |
| Jefe de área | Beneficiario | Valida que el cambio de destino sea transparente para su operación |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-07 | CU-17 configurar el destino de almacenamiento de archivos por el usuario raíz | a generar |

## 8. Dependencias con otras NB

- Depende de NB-03 (captura georreferenciada de observaciones): la configurabilidad aplica al almacenamiento de las fotografías que la captura produce.

## 9. Prioridad MoSCoW

Could Have. La capacidad F-17 del intake (§4) es Could Have; aporta sostenibilidad operativa y económica del almacenamiento pero no integra el camino principal del relevamiento, por lo que se incorpora si la cadencia del proyecto lo permite.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la necesidad a partir de SOLUTION-INTAKE §3, §4 (F-17) y de la visión y el alcance de la categoría 00. |
