# RN-02 — Idempotencia de la sincronización

**Proyecto:** aplicada-sync
**Documento:** RN-02-idempotencia-sincronizacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Enunciado

Un mismo cambio local, identificado por su identificador de cambio estable, se aplica una sola vez con efecto en el backend remoto y una sola vez en el almacén local, sin importar cuántas veces se encole, se reintente o se reanude su sincronización.

## 2. Justificación

La idempotencia es la garantía que permite reintentar y reanudar con seguridad ante cortes de conectividad, que son la condición normal del trabajo en campo. Sin ella, un reenvío tras un corte produciría duplicados; con ella, el motor puede reenviar libremente porque el efecto neto es el mismo. Deriva del requisito de no perder ni duplicar datos en la sincronización sin conexión.

## 3. Ámbito de aplicación

Se evalúa al encolar un cambio (no se duplica una entrada por identificador), al subir durante un ciclo, al reintentar tras un error transitorio y al reanudar una subida parcial. La idempotencia descansa en el identificador de cambio estable que el host provee y que el backend reconoce.

## 4. Consecuencia si se viola

Si un cambio se aplicara dos veces por efecto de un reintento o una reanudación, se produciría una duplicación de datos que rompe el criterio de integridad de NB-04. Una implementación que no preserve la idempotencia incumple el contrato y debe corregirse; en operación, el motor descarta el efecto repetido apoyándose en el identificador estable.

## 5. CU afectados

CU-02 (registrar y encolar un cambio local), CU-03 (ejecutar la sincronización subir-luego-bajar), CU-06 (reanudar una sincronización interrumpida).

## 6. Pruebas que la verifican

- Verificación de que reencolar un cambio con un identificador ya presente no duplica la entrada en la cola (test previsto en 08 asociado a CU-02).
- Verificación de que un reenvío del mismo cambio tras un corte no aplica el efecto dos veces en el backend (test previsto en 08 asociado a CU-03 y CU-06).
- Verificación de que una actualización descendente ya aplicada no se vuelve a aplicar en el almacén local (test previsto en 08 asociado a CU-03).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de idempotencia de la sincronización, derivada de NB-04 y del criterio de integridad de la sincronización. |
