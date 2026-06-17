# RN-04 — Estados visibles del relevamiento y habilitación de acciones

**Proyecto:** geovial-web
**Documento:** RN-04-estados-visibles-habilitacion-acciones_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional

## 1. Enunciado de la regla

El front web muestra siempre el estado vigente del relevamiento dentro de su ciclo —recolección, revisión o cierre— y habilita en pantalla solo las acciones válidas para ese estado: la edición de la composición del tramo y la creación de marcadores iniciales y la carga manual solo en recolección; la revisión, la resolución de conflictos y el cierre solo en revisión; y la consulta en solo lectura cuando el relevamiento está cerrado.

## 2. Justificación

El usuario debe ver con claridad en qué etapa está cada relevamiento y no debe poder intentar acciones que el backend rechazaría por estado; presentar acciones inválidas confunde al usuario y genera errores evitables. Deriva de la regla de transición de estados del backend (RN-05 de geovial-api) y de NB-02 y NB-05.

## 3. Ámbito de aplicación

Se evalúa al presentar y operar relevamientos (CU-03), al crear marcadores iniciales (CU-05), en la carga manual (CU-09), en la revisión (CU-06), en la resolución de conflictos (CU-07) y en la transición y cierre (CU-08), habilitando u ocultando las acciones según el estado vigente que reporta el backend.

## 4. Consecuencia si se viola

Si el front habilitara una acción inválida para el estado vigente, el backend la rechaza con un código de estado (RELEVAMIENTO_NO_EN_RECOLECCION, RELEVAMIENTO_NO_EN_REVISION, RELEVAMIENTO_CERRADO o TRANSICION_NO_PERMITIDA) y el front debe presentar la vista en solo lectura y reportar el motivo, sin alterar el relevamiento.

## 5. CU afectados

CU-03, CU-05, CU-06, CU-07, CU-08, CU-09.

## 6. Pruebas que la verifican

- Edición de la composición del tramo habilitada solo en recolección; en otros estados en solo lectura (08, sobre CU-03).
- Creación de marcadores iniciales y carga manual deshabilitadas fuera de recolección (08, sobre CU-05, CU-09).
- Resolución de conflictos y cierre habilitados solo en revisión (08, sobre CU-07, CU-08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de estados visibles del relevamiento y habilitación de acciones en el front web, alineada a RN-05 de geovial-api y derivada de NB-02 y NB-05. |
