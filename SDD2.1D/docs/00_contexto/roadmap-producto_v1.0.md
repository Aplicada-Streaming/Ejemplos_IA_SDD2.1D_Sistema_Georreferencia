# Roadmap de Producto

**Proyecto:** GeoVial (solución)
**Documento:** roadmap-producto_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Product Manager + API Product Owner
**Trazabilidad upstream:** SOLUTION-INTAKE §4, §15
**Trazabilidad downstream:** 06_backlog-tecnico, 07_plan-sprint, 11_examples

## 1. Propósito

Este documento ordena la construcción de GeoVial en fases sucesivas, cada una de las cuales entrega valor demostrable y se prueba antes de pasar a la siguiente. Las fases se derivan del esquema de descomposición y entrega del intake (§15), que adopta cortes verticales con un esqueleto que recorre toda la solución de punta a punta desde el primer incremento. El orden es topológico: primero la autenticación y la jerarquía de usuarios de punta a punta, luego los relevamientos y los marcadores, después la captura y la sincronización en campo, y al final la revisión y resolución de conflictos al cierre, la exportación e importación, y la configuración del almacenamiento.

No se fijan fechas: el proyecto no tiene fecha objetivo (intake §10) y la cadencia la fija el avance de un equipo de un único desarrollador. Por eso las fases se cierran por criterios verificables de transición, no por calendario.

## 2. Fases del producto

| Fase | Objetivo | Capacidades asociadas | Sprints estimados | Entregable | Release target |
|---|---|---|---|---|---|
| F0 — Esqueleto y jerarquía de usuarios | Recorrer la solución de punta a punta con autenticación y administración de usuarios por jerarquía | F-01, F-02, F-08 (inicio de sesión) | Por estimar | Jerarquía de cuatro roles operativa de punta a punta, con altas y bajas según alcance e inicio de sesión funcional | Incremento 1 |
| F1 — Relevamientos y marcadores | Crear y administrar relevamientos sobre un tramo vial, asignar agentes y modelar los marcadores | F-03, F-04, F-06, F-10 | Por estimar | Alta, baja, visualización y asignación de relevamientos, con marcadores geográficos visibles sobre el mapa | Incremento 2 |
| F2 — Captura en campo y sincronización | Capturar observaciones en terreno sin conexión y sincronizarlas al recuperar la red | F-05, F-07, F-09 | Por estimar | Captura sin conexión con foto y coordenadas, y sincronización que sube cambios locales y baja actualizaciones | Incremento 3 |
| F3 — Revisión, cierre y cierre de alcance | Revisar sobre mapa, resolver conflictos al cierre, y completar exportación/importación y configuración de almacenamiento | F-11, F-12, F-13, F-14, F-15, F-16, F-17 | Por estimar | Revisión con carrusel por marcador, resolución de conflictos al cierre, transición a revisión y cierre, y capacidades de cierre de alcance | Incremento 4 |

Nota sobre el orden: dado que los conflictos de marcadores conviven con la operación y se resuelven recién al cierre, la recolección (F2) puede entregarse antes que la resolución de conflictos (F3) sin romper el camino de punta a punta.

## 3. Matriz fase → épica → sprint → release

La asignación de sprints y la apertura de épicas se concretan en 06_backlog-tecnico y 07_plan-sprint. La cantidad de sprints por fase queda por estimar mientras no haya una línea de base de cadencia del equipo.

| Fase | Épica (a abrir en 06) | Sprint (a planificar en 07) | Release |
|---|---|---|---|
| F0 | Autenticación y jerarquía de usuarios | Por planificar | Incremento 1 |
| F1 | Gestión de relevamientos y marcadores | Por planificar | Incremento 2 |
| F2 | Captura en campo y sincronización | Por planificar | Incremento 3 |
| F3 | Revisión, conflictos al cierre y cierre de alcance | Por planificar | Incremento 4 |

## 4. Dependencias entre fases

- F0 es prerrequisito de todas las demás: sin jerarquía de usuarios e inicio de sesión no se puede asignar ni operar nada.
- F1 depende de F0: los relevamientos se asignan a agentes que ya existen en la jerarquía.
- F2 depende de F1: la captura en campo opera sobre relevamientos ya creados, asignados y con su modelo de marcadores definido.
- F3 depende de F2: la revisión y la resolución de conflictos al cierre operan sobre observaciones ya recolectadas y sincronizadas; la exportación e importación y la configuración de almacenamiento cierran el alcance una vez que el ciclo del relevamiento está completo.

## 5. Criterios de transición entre fases

| Fase origen | Fase destino | Criterios verificables |
|---|---|---|
| (inicio) | F0 | - [ ] La jerarquía de cuatro roles está modelada de punta a punta.<br>- [ ] Se puede dar de alta y de baja usuarios respetando el alcance de cada nivel.<br>- [ ] El inicio de sesión con credenciales funciona de punta a punta.<br>- [ ] El recorrido de punta a punta está cubierto por pruebas automáticas. |
| F0 | F1 | - [ ] El jefe de área puede crear un relevamiento sobre un tramo vial.<br>- [ ] El jefe de área puede asignar agentes de campo a un relevamiento.<br>- [ ] Un marcador geográfico con notas, fotos, comentarios y etiquetas queda modelado y es compartible por varias observaciones.<br>- [ ] Los relevamientos y sus marcadores se visualizan sobre el mapa. |
| F1 | F2 | - [ ] El agente puede capturar una observación con foto y coordenadas en el momento, sin conexión.<br>- [ ] La carga manual prioriza los datos de ubicación de la foto y aplica el radio de agrupación.<br>- [ ] La sincronización sube primero los cambios locales y luego baja las actualizaciones del relevamiento asignado.<br>- [ ] La captura sin conexión y la sincronización están cubiertas por pruebas automáticas. |
| F2 | F3 | - [ ] El jefe puede recorrer los marcadores sobre el mapa con el carrusel de fotos encadenado.<br>- [ ] El jefe puede resolver los conflictos de marcadores al cierre.<br>- [ ] El relevamiento transita de recolección a revisión y luego a cierre.<br>- [ ] La exportación e importación de un relevamiento completo y la configuración del almacenamiento quedan disponibles si la cadencia lo permite. |

## 6. Trazabilidad downstream

Upstream: las fases se derivan del esquema de descomposición y entrega del intake (§15) y de la priorización MoSCoW (§4).

Downstream: este roadmap alimenta 06_backlog-tecnico (apertura de épicas por fase y definición de su backlog), 07_plan-sprint (asignación de sprints y secuenciación dentro de cada fase) y 11_examples (ejemplos alineados a la capacidad entregada en cada fase). Cada criterio de transición de §5 es la condición de salida que 07 usa para dar por cerrada una fase antes de habilitar la siguiente.
