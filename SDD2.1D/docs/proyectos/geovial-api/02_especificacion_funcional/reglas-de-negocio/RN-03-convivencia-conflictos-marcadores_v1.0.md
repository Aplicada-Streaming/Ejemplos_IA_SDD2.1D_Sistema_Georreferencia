# RN-03 — Convivencia con conflictos de marcadores y resolución al cierre

**Proyecto:** geovial-api
**Documento:** RN-03-convivencia-conflictos-marcadores_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Enunciado de la regla

Dos o más marcadores dentro de un mismo radio constituyen un conflicto de marcadores que es un estado válido durante la recolección y la revisión: el sistema convive con él, mantiene la información accesible y no bloquea ninguna operación por su causa; la decisión de unificarlos o mantenerlos separados se difiere al cierre del relevamiento y queda a cargo del jefe de área.

## 2. Justificación

El trabajo de campo no debe detenerse ante un conflicto, y la catalogación correcta requiere el criterio del jefe sobre la evidencia completa, que solo está disponible al cierre; bloquear durante la recolección perjudicaría la captura sin aportar valor (NB-04, NB-05, intake §7, §14).

## 3. Ámbito de aplicación

Se evalúa al crear o mover marcadores, al recibir y entregar cambios en la sincronización, al consultar el relevamiento para revisión y al cerrar el relevamiento. El conflicto convive hasta la revisión y se resuelve como precondición del cierre.

## 4. Consecuencia si se viola

Si una operación se bloqueara por un conflicto durante la recolección o la revisión, se considera un defecto: el conflicto debe registrarse y la operación continuar. Inversamente, cerrar un relevamiento con conflictos sin resolver se rechaza con el código CONFLICTOS_PENDIENTES.

## 5. CU afectados

CU-07, CU-08, CU-10, CU-11, CU-12, CU-13, CU-14.

## 6. Pruebas que la verifican

- Creación y sincronización de marcadores en conflicto sin bloquear la operación (08, sobre CU-07, CU-10, CU-11).
- Información accesible durante la revisión pese a conflictos presentes (08, sobre CU-12).
- Cierre rechazado con conflictos pendientes y aceptado tras resolverlos (08, sobre CU-13, CU-14).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de convivencia con conflictos de marcadores y resolución al cierre, derivada de NB-04 y NB-05. |
