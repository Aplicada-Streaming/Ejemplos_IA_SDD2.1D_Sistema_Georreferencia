# RN-02 — Conservación de la traza de autoría al dar de baja desde el front web

**Proyecto:** geovial-web
**Documento:** RN-02-conservacion-traza-autoria_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional

## 1. Enunciado de la regla

Cuando el front web da de baja a un usuario, lo presenta como inhabilitado para acceder pero conserva visible la traza de autoría de lo que ese usuario registró: las observaciones, fotos y marcadores cargados mantienen su autor identificado y siguen siendo consultables en la revisión.

## 2. Justificación

La trazabilidad de la evidencia exige que toda observación quede asociada a un responsable identificado aun después de la baja del usuario, para que los informes de cierre del jefe sean trazables y reproducibles; deriva de la regla de conservación de autoría del backend (RN-02 de geovial-api) y de NB-01 (intake §2).

## 3. Ámbito de aplicación

Se evalúa al dar de baja a un usuario desde el front (CU-02) y al presentar la evidencia en la revisión sobre mapa y carrusel (CU-06), donde la autoría de cada registro permanece visible pese a la baja de su autor.

## 4. Consecuencia si se viola

Si el front presentara una baja como borrado de la evidencia o de su autoría, se considera un defecto: la baja solo inhabilita el acceso. El backend conserva la autoría histórica; el front debe reflejar esa conservación y nunca mostrar la evidencia como huérfana de autor.

## 5. CU afectados

CU-02, CU-06.

## 6. Pruebas que la verifican

- Baja de un agente con observaciones cargadas: el front lo muestra dado de baja y las observaciones conservan su autoría (08, sobre CU-02).
- Revisión de un relevamiento cuya evidencia fue cargada por un usuario ya dado de baja: la autoría permanece visible (08, sobre CU-06).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de conservación de la traza de autoría al dar de baja desde el front web, alineada a RN-02 de geovial-api y derivada de NB-01. |
