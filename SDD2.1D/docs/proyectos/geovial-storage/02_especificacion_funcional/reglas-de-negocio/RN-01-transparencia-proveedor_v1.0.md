# RN-01 — Transparencia del proveedor hacia el consumidor

**Proyecto:** geovial-storage
**Documento:** RN-01-transparencia-proveedor_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Enunciado

El contrato público de la librería es idéntico cualquiera sea el proveedor de almacenamiento activo: las operaciones, sus parámetros, sus resultados y sus códigos de error no cambian al cambiar de proveedor local a proveedor remoto o a otro proveedor.

## 2. Justificación

Es la razón de ser de la librería declarada en NB-07: el negocio debe poder cambiar el destino del almacenamiento sin que los demás roles ni el consumidor noten diferencia. El criterio de éxito de NB-07 fija en cero los cambios de comportamiento percibidos al cambiar el destino, y esa transparencia se sostiene en este invariante.

## 3. Ámbito de aplicación

Se evalúa en toda operación de la superficie pública: en cada invocación de guardado, recuperación, eliminación, verificación y listado, y en el momento en que el usuario raíz cambia el proveedor activo. La regla rige durante toda la vida de la librería, no solo en el cambio de proveedor.

## 4. Consecuencia si se viola

Si una operación expone un comportamiento, un parámetro o un código de error que depende del proveedor, se considera una violación del contrato y un cambio incompatible que debe rechazarse en revisión. El consumidor no debe necesitar ramas de código por proveedor.

## 5. CU afectados

CU-01, CU-02, CU-03, CU-04, CU-05, CU-06.

## 6. Pruebas que la verifican

Una misma batería de pruebas de contrato se ejecuta contra cada proveedor soportado (local y remoto, al menos) y debe producir resultados equivalentes para las mismas entradas; las pruebas de cambio de proveedor activo verifican que las operaciones siguientes funcionan sin cambiar su forma de invocación (referencia a casos previstos en 08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de transparencia del proveedor. |
