# RN-05 — Transición de estados del relevamiento

**Proyecto:** geovial-api
**Documento:** RN-05-transicion-estados-relevamiento_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Enunciado de la regla

Un relevamiento recorre los estados recolección, revisión y cierre en ese sentido de avance; nace en recolección, avanza a revisión y de revisión a cierre. El cierre exige como precondición que no queden conflictos de marcadores sin resolver. Se admite el retorno controlado de revisión a recolección y la reapertura de un relevamiento recién cerrado a revisión; ninguna otra transición es válida.

## 2. Justificación

El ciclo del relevamiento debe ser visible y ordenado para coordinar el trabajo y para que el cierre sea un hito confiable que habilita el informe; permitir transiciones arbitrarias o cerrar con conflictos pendientes rompería la consistencia de la evidencia (NB-02, NB-05, intake §6).

## 3. Ámbito de aplicación

Se evalúa en cada solicitud de transición de estado y en el cierre. Se aplica al validar el estado origen, el estado destino y, para el cierre, la ausencia de conflictos pendientes.

## 4. Consecuencia si se viola

Una transición no contemplada se rechaza con el código TRANSICION_NO_PERMITIDA o RELEVAMIENTO_NO_EN_REVISION según el caso; un cierre con conflictos pendientes se rechaza con CONFLICTOS_PENDIENTES; el estado del relevamiento no cambia.

## 5. CU afectados

CU-04, CU-05, CU-06, CU-13, CU-14.

## 6. Pruebas que la verifican

- Transición recolección a revisión válida y transición desde cierre rechazada (08, sobre CU-06).
- Retorno controlado de revisión a recolección permitido (08, sobre CU-06).
- Cierre con conflictos pendientes rechazado y aceptado tras resolverlos (08, sobre CU-13, CU-14).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de transición de estados del relevamiento, derivada de NB-02 y NB-05. |
