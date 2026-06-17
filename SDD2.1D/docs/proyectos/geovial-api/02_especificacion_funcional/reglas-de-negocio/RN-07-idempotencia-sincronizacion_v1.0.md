# RN-07 — Idempotencia de la sincronización y de las escrituras reintentables

**Proyecto:** geovial-api
**Documento:** RN-07-idempotencia-sincronizacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Enunciado de la regla

Todo cambio subido en la sincronización porta un identificador de origen estable y toda operación no segura reintentable porta una clave de idempotencia; reenviar un cambio ya aplicado o reintentar una operación con la misma clave no produce efectos duplicados: el backend reconoce el reenvío y devuelve el resultado ya registrado.

## 2. Justificación

La captura sin conexión reenvía lotes tras cortes de red, y cualquier escritura puede reintentarse ante una respuesta no recibida; sin idempotencia, esos reintentos duplicarían marcadores, observaciones, fotos o usuarios, materializando el riesgo de duplicación de datos identificado por el negocio (NB-04, intake §11 R-03).

## 3. Ámbito de aplicación

Se evalúa en la subida de sincronización y en cada operación no segura que declara admitir clave de idempotencia (altas, asignaciones, transiciones, resoluciones, importaciones). Se aplica al recibir la operación, antes de ejecutar su efecto.

## 4. Consecuencia si se viola

Un reenvío reconocido no se reaplica y devuelve el resultado previo; una clave reutilizada con contenido inconsistente se rechaza con el código CLAVE_REUTILIZADA_INCONSISTENTE. Si se duplicara un efecto pese a un identificador o clave repetidos, se considera un defecto.

## 5. CU afectados

CU-01, CU-02, CU-04, CU-05, CU-06, CU-08, CU-09, CU-10, CU-13, CU-14, CU-16, CU-21.

## 6. Pruebas que la verifican

- Reenvío de un lote tras un corte reconocido sin duplicar cambios (08, sobre CU-10).
- Alta reintentada con la misma clave que no duplica el recurso (08, sobre CU-21).
- Clave reutilizada con contenido distinto rechazada (08, sobre CU-21).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de idempotencia de la sincronización y de las escrituras reintentables, derivada de NB-04. |
