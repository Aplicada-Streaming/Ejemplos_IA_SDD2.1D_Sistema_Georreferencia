# NB-06 — Portabilidad del relevamiento completo

| Campo | Valor |
| --- | --- |
| Proyecto | geovial-api |
| Documento | NB-06-portabilidad-relevamiento_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-06-15 |
| Autor | Analista de Negocio + API Product Analyst |
| Trazabilidad upstream | SOLUTION-INTAKE §3, §4; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-15, CU-16 (previstas en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

La organización necesita poder llevarse un relevamiento completo fuera del sistema y volver a incorporarlo, de modo que la información de un tramo —comentarios, etiquetas y fotos— quede autocontenida y portable entre entornos. Hoy, una vez que un relevamiento queda dentro del sistema, no hay forma sencilla de compartirlo con un tercero, archivarlo fuera de línea o moverlo a otro entorno sin perder la correspondencia entre las piezas de evidencia.

El dolor concreto es la falta de un mecanismo de portabilidad que empaquete todo el relevamiento en una sola unidad transferible y que permita reincorporarlo después conservando su estructura. Sin esto, compartir o resguardar un relevamiento implica recolectar manualmente sus fotos y datos por separado, con el riesgo de que la evidencia se desarme en el camino.

La necesidad importa para la reproducibilidad y el respaldo del trabajo, pero no forma parte del camino principal del relevamiento: la operación de campo y la revisión funcionan sin ella, por lo que se aborda si la cadencia del proyecto lo permite.

## 2. Ejemplo de uso desde la perspectiva del negocio

Un jefe de área necesita entregar a una auditoría externa el relevamiento completo de un puente, con todas sus fotos, comentarios y etiquetas. Exporta el relevamiento como una única unidad transferible y la comparte. Meses después, en un entorno distinto preparado para una capacitación, esa misma unidad se importa y el relevamiento aparece reconstruido con toda su evidencia en su lugar, tal como fue cerrado.

## 3. Impacto

- Permite compartir, archivar y mover un relevamiento completo como una sola unidad.
- Conserva la correspondencia entre fotos, comentarios y etiquetas al transferir entre entornos.
- Aporta una vía de respaldo del trabajo fuera del sistema.
- Si queda sin resolver, compartir o resguardar un relevamiento sigue siendo un proceso manual y propenso a desarmar la evidencia, pero el camino principal del negocio no se ve afectado.

## 4. Problema específico que resuelve

- No hay forma de empaquetar un relevamiento completo en una unidad transferible única.
- Compartir o archivar un relevamiento obliga a recolectar sus piezas por separado.
- Reincorporar un relevamiento a otro entorno no conserva su estructura.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Completitud de la exportación | Porcentaje de comentarios, etiquetas y fotos incluidos respecto del relevamiento original | 100 % | release de la capacidad |
| Fidelidad de la importación | Porcentaje de relevamientos reimportados que reconstruyen su estructura sin pérdidas | 100 % | release de la capacidad |
| Unidad de transferencia | Cantidad de archivos a transferir por relevamiento exportado | 1 | release de la capacidad |
| Esfuerzo de compartir un relevamiento | Minutos para exportar y entregar un relevamiento completo | ≤ 5 min | 6 meses post-despliegue |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Vialidad provincial | Propietario | Aprueba la portabilidad como capacidad de respaldo y de intercambio del relevamiento |
| Departamento de desarrollo de software (1 desarrollador) | Implementador | Construye y mantiene la exportación e importación del relevamiento completo |
| Jefe de área | Beneficiario | Exporta e importa relevamientos para compartir, auditar o archivar |
| Usuario raíz | Beneficiario | Valida que la portabilidad sirva para mover relevamientos entre entornos del sistema |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-06 | CU-15 exportar un relevamiento completo en una unidad transferible única | a generar |
| NB-06 | CU-16 importar un relevamiento completo reconstruyendo su estructura | a generar |

## 8. Dependencias con otras NB

- Depende de NB-05 (revisión sobre mapa y cierre con resolución de conflictos): la portabilidad se ejerce sobre relevamientos ya estructurados y cerrados.

## 9. Prioridad MoSCoW

Could Have. La capacidad F-16 del intake (§4) es Could Have; aporta valor de respaldo e intercambio pero no integra el camino principal del relevamiento, por lo que se incorpora si la cadencia del proyecto lo permite.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la necesidad a partir de SOLUTION-INTAKE §3, §4 (F-16) y de la visión y el alcance de la categoría 00. |
