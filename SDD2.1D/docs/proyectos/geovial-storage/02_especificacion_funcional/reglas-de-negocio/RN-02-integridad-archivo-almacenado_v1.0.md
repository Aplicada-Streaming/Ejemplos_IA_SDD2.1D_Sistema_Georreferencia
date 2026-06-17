# RN-02 — Integridad del archivo almacenado

**Proyecto:** geovial-storage
**Documento:** RN-02-integridad-archivo-almacenado_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Enunciado

El contenido recuperado bajo un identificador lógico es idénticamente igual, byte a byte, al contenido que se guardó bajo ese mismo identificador, mientras el archivo no haya sido sobrescrito ni eliminado.

## 2. Justificación

Las fotografías son la evidencia de los relevamientos (NB-03): una imagen alterada o truncada invalida la observación y el informe de cierre. El negocio depende de que la evidencia se conserve fiel desde la captura hasta la revisión, y de que un relevamiento exportado e importado (NB-06) reconstruya sus fotos sin pérdidas.

## 3. Ámbito de aplicación

Se evalúa al guardar (lo persistido debe poder recuperarse igual), al recuperar (lo entregado debe coincidir con lo persistido) y al verificar existencia (un identificador recuperable se reporta presente y uno eliminado se reporta ausente, de forma coherente con el estado real). La regla es atemporal: rige para todo proveedor y toda versión de la librería.

## 4. Consecuencia si se viola

Si una recuperación devuelve un contenido distinto del guardado, o si la verificación reporta una presencia incoherente con la recuperación, la operación se considera fallida y debe rechazarse; la librería no debe entregar contenido corrupto como si fuera válido.

## 5. CU afectados

CU-01, CU-02, CU-04.

## 6. Pruebas que la verifican

Pruebas de ida y vuelta que guardan un contenido conocido, lo recuperan y comparan la igualdad binaria completa; pruebas de coherencia que verifican que un identificador guardado se reporta presente, y que tras eliminarlo se reporta ausente; pruebas de recuperación por rango que verifican que el segmento devuelto coincide con el tramo correspondiente del original (referencia a casos previstos en 08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de integridad del archivo almacenado. |
