# NB-03 — Captura georreferenciada de observaciones en campo

| Campo | Valor |
| --- | --- |
| Proyecto | geovial-api |
| Documento | NB-03-captura-georreferenciada-observaciones_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-06-15 |
| Autor | Analista de Negocio + API Product Analyst |
| Trazabilidad upstream | SOLUTION-INTAKE §1, §3, §4, §8; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-07, CU-08, CU-09 (previstas en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

La organización necesita que cada observación del estado de un tramo vial nazca atada de manera confiable al punto geográfico donde fue tomada. Hoy el relevamiento se hace con planillas en papel y fotografías sueltas sin georreferencia confiable, lo que rompe la correspondencia entre cada foto y el lugar exacto del tramo. Esa ruptura obliga a un retrabajo en oficina para reconstruir, de memoria o por contexto, dónde fue tomada cada imagen, con riesgo de confundir o perder evidencia entre relevamientos.

El dolor concreto es la pérdida de trazabilidad entre la evidencia fotográfica y su ubicación, y la ausencia de una estructura que agrupe esa evidencia de forma consistente. El negocio necesita organizar las observaciones alrededor de marcadores geográficos que reúnan notas, fotos, comentarios y etiquetas, de modo que un mismo punto del tramo concentre toda la evidencia asociada y un mismo marcador pueda ser compartido por varias observaciones. También necesita resolver la ubicación tanto en el momento de la captura como, cuando la foto se carga después, a partir de los datos de ubicación que la propia imagen trae consigo, agrupando por cercanía mediante un radio.

La necesidad importa porque es el núcleo de la propuesta de valor: sin captura georreferenciada y estructurada, los informes de cierre siguen siendo lentos, difíciles de reproducir y de auditar.

## 2. Ejemplo de uso desde la perspectiva del negocio

Un relevador llega a la pila central de un puente y necesita documentar una fisura. Toma la fotografía en el lugar y la observación queda anclada al punto donde está parado, junto con una nota sobre la fisura y una etiqueta que la clasifica. Más adelante, ya en la oficina, descubre que tiene fotos adicionales del mismo puente tomadas con otro equipo; las carga manualmente y el sistema las ubica a partir de los datos de ubicación de cada imagen y las agrupa con las que están cerca, dentro de un radio definido, sin obligarlo a reubicar cada foto a mano.

## 3. Impacto

- Asegura que cada observación quede asociada a una ubicación, eliminando la reconstrucción manual posterior.
- Estructura la evidencia alrededor de marcadores que agrupan fotos, notas, comentarios y etiquetas.
- Reduce el riesgo de confundir o perder evidencia entre relevamientos.
- Habilita tanto la captura en el momento como la carga manual posterior con agrupación por cercanía.
- Si queda sin resolver, persiste el retrabajo en oficina y la baja calidad de la georreferenciación que el negocio quiere superar.

## 4. Problema específico que resuelve

- La correspondencia entre cada foto y el punto del tramo donde fue tomada se pierde con los métodos manuales actuales.
- No hay una estructura que agrupe notas, fotos, comentarios y etiquetas alrededor de un punto geográfico.
- La carga manual posterior de fotos no aprovecha los datos de ubicación de la propia imagen ni agrupa por cercanía.
- Evidencia de distintos tramos se mezcla por falta de anclaje geográfico.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Calidad de la georreferenciación | Porcentaje de observaciones con coordenada geográfica válida asociada | ≥ 95 % | 3 meses post-despliegue |
| Retrabajo de reubicación en oficina | Fotos que requieren reubicación manual sobre el total | ≤ 10 % | 6 meses post-despliegue |
| Agrupación por marcador en carga manual | Fotos agrupadas automáticamente dentro del radio sobre el total cargado manualmente | ≥ 80 % | 6 meses post-despliegue |
| Integridad de la evidencia por observación | Observaciones con al menos foto, ubicación y etiqueta completas | 100 % | release 1.0 |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Vialidad provincial | Propietario | Aprueba el modelo de observación georreferenciada como evidencia de los informes |
| Departamento de desarrollo de software (1 desarrollador) | Implementador | Construye y mantiene la captura georreferenciada y el modelo de marcadores |
| Agente de campo | Beneficiario | Captura observaciones en terreno y valida que la georreferenciación refleje el lugar |
| Jefe de área | Beneficiario | Consume la evidencia estructurada para confeccionar los informes de cierre |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-03 | CU-07 capturar observación con foto y resolución de coordenadas en el momento | a generar |
| NB-03 | CU-08 administrar el marcador geográfico con notas, fotos, comentarios y etiquetas | a generar |
| NB-03 | CU-09 cargar fotos manualmente con priorización de los datos de ubicación y radio de agrupación | a generar |

## 8. Dependencias con otras NB

- Depende de NB-02 (gestión y asignación de relevamientos): las observaciones se capturan dentro de un relevamiento previamente creado y asignado al agente.

## 9. Prioridad MoSCoW

Must Have. Las capacidades F-05, F-06, F-09 y F-10 del intake (§4) son Must Have y constituyen el núcleo de la propuesta de valor (§3); sin captura georreferenciada estructurada no hay MVP defendible.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la necesidad a partir de SOLUTION-INTAKE §1, §3, §4 (F-05, F-06, F-09, F-10, F-15), §8 y de la visión y el alcance de la categoría 00. |
