# RN-06 — Orden de sincronización subir antes de bajar

**Proyecto:** geovial-api
**Documento:** RN-06-orden-subir-antes-de-bajar_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Enunciado de la regla

En todo ciclo de sincronización, el backend incorpora primero los cambios locales que el cliente sube y solo después entrega las actualizaciones del relevamiento que el cliente baja; la fase de bajada no se atiende hasta que la fase de subida del mismo ciclo concluyó.

## 2. Justificación

Subir antes de bajar evita que el cliente sobrescriba con datos del servidor cambios locales aún no enviados, y hace predecible la sincronización, reduciendo el riesgo de pérdida o duplicación de datos que el negocio identificó como de alto impacto (NB-04, intake §11 R-03, §17 P.3).

## 3. Ámbito de aplicación

Se evalúa en cada ciclo de sincronización entre el cliente de campo y el backend: en la recepción del lote de cambios locales y en la entrega de actualizaciones. Se aplica al ordenar las dos fases del ciclo.

## 4. Consecuencia si se viola

Si el cliente solicita la bajada sin haber concluido la subida del ciclo, el backend rechaza con el código SUBIDA_NO_CONCLUIDA y no entrega actualizaciones, forzando a completar la subida primero.

## 5. CU afectados

CU-10, CU-11, CU-21.

## 6. Pruebas que la verifican

- Subida aplicada antes de cualquier entrega de actualizaciones (08, sobre CU-10).
- Bajada solicitada sin subida concluida rechazada con SUBIDA_NO_CONCLUIDA (08, sobre CU-11).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de orden de sincronización subir antes de bajar, derivada de NB-04. |
