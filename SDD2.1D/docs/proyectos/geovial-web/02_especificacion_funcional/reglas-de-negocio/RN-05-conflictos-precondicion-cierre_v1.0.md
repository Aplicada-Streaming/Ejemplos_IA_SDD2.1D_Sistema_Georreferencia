# RN-05 — Resolución de conflictos como precondición visible del cierre

**Proyecto:** geovial-web
**Documento:** RN-05-conflictos-precondicion-cierre_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional

## 1. Enunciado de la regla

El front web no habilita el cierre de un relevamiento mientras queden conflictos de marcadores sin resolver: mientras existan conflictos pendientes los presenta como accesibles y derivables a la pantalla de resolución, y solo ofrece el cierre cuando todos los conflictos del relevamiento están resueltos. Durante la recolección y la revisión, los conflictos conviven sin bloquear el acceso a la información.

## 2. Justificación

El cierre es el hito que habilita el informe y debe apoyarse en una catalogación resuelta de la evidencia; permitir cerrar con conflictos pendientes rompería la consistencia de la evidencia, pero bloquear el acceso antes del cierre perjudicaría la operación sin aportar valor. Deriva de las reglas de convivencia y transición del backend (RN-03 y RN-05 de geovial-api) y de NB-05.

## 3. Ámbito de aplicación

Se evalúa al presentar la revisión sobre mapa con conflictos presentes (CU-06), al resolver conflictos al cierre (CU-07) y al habilitar la transición de cierre del relevamiento (CU-08), donde la ausencia de conflictos pendientes es la condición visible para ofrecer el cierre.

## 4. Consecuencia si se viola

Si el front ofreciera el cierre con conflictos pendientes, el backend lo rechaza con el código CONFLICTOS_PENDIENTES y el front debe bloquear el cierre, informar los conflictos pendientes y derivar a su resolución; inversamente, si el front bloqueara el acceso a la información por la mera presencia de conflictos durante la recolección o la revisión, se considera un defecto.

## 5. CU afectados

CU-06, CU-07, CU-08.

## 6. Pruebas que la verifican

- Información accesible durante la revisión pese a conflictos presentes (08, sobre CU-06).
- Cierre no habilitado con conflictos pendientes y habilitado tras resolverlos (08, sobre CU-07, CU-08).
- Derivación desde el intento de cierre a la pantalla de resolución de conflictos (08, sobre CU-08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de resolución de conflictos como precondición visible del cierre en el front web, alineada a RN-03 y RN-05 de geovial-api y derivada de NB-05. |
