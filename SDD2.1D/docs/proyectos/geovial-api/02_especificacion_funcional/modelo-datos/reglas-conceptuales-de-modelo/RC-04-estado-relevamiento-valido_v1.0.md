# RC-04 — Estado del relevamiento dentro del ciclo válido

**Proyecto:** geovial-api
**Documento:** RC-04-estado-relevamiento-valido_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Enunciado

El estado de un relevamiento pertenece siempre al catálogo cerrado recolección, revisión, cierre, y solo cambia mediante una transición permitida por el ciclo; un relevamiento en cierre no conserva conflictos de marcadores pendientes.

## 2. Entidades involucradas

Relevamiento, ConflictoMarcadores.

## 3. Tipo de restricción

Valor permitido y derivación de consistencia entre estado y conflictos.

## 4. Mecanismo de verificación conceptual

Al persistir un cambio de estado se comprueba que el valor destino pertenece al catálogo y que la transición desde el valor origen está permitida; al pasar a cierre se comprueba que no quedan conflictos de marcadores en estado pendiente para ese relevamiento.

## 5. RN o CU que la justifican

RN-05, RN-03; CU-06, CU-13, CU-14.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla conceptual de estado del relevamiento dentro del ciclo válido. |
