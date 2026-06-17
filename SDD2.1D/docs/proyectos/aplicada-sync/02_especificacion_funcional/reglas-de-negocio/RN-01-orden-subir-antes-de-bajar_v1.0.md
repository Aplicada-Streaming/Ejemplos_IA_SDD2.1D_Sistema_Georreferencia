# RN-01 — Orden estricto subir-antes-de-bajar

**Proyecto:** aplicada-sync
**Documento:** RN-01-orden-subir-antes-de-bajar_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Enunciado

En todo ciclo de sincronización, el motor sube por completo los cambios locales pendientes del host antes de bajar cualquier actualización del backend remoto; nunca se aplica una actualización descendente mientras queden cambios locales pendientes confirmables por subir.

## 2. Justificación

Es la política central de la librería y la garantía de negocio que la hace confiable para el trabajo sin conexión: asegura que lo registrado en terreno llegue al backend antes de que el local sea sobrescrito por novedades remotas, evitando que una bajada pise o descarte cambios locales todavía no propagados. Deriva de la política de la solución y del riesgo de pérdida o duplicación de datos en la sincronización.

## 3. Ámbito de aplicación

Se evalúa en cada ejecución de un ciclo de sincronización, tanto el disparado manualmente como el automático y la reanudación de un ciclo interrumpido. Es una invariante de orden que el motor no expone como configurable hacia un orden inverso.

## 4. Consecuencia si se viola

Una implementación o una configuración que permitiera bajar antes de concluir la subida se considera defectuosa: incumple el contrato de la librería, habilita la pérdida de cambios locales y debe rechazarse en revisión. En ejecución, si la subida no puede concluir, el motor detiene el ciclo sin iniciar la bajada y deja la sesión reanudable.

## 5. CU afectados

CU-03 (ejecutar la sincronización subir-luego-bajar), CU-04 (detectar conectividad y disparar la sincronización), CU-06 (reanudar una sincronización interrumpida).

## 6. Pruebas que la verifican

- Verificación de que, con cola no vacía, ninguna actualización descendente se aplica antes de confirmar la última subida (test previsto en 08 asociado a CU-03).
- Verificación de que un corte en la fase de subida no dispara la fase de bajada (test previsto en 08 asociado a CU-03 y CU-06).
- Verificación de que el ciclo disparado por conectividad respeta el mismo orden (test previsto en 08 asociado a CU-04).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de orden estricto subir-antes-de-bajar, derivada de NB-04 y de la política central de aplicada-sync. |
