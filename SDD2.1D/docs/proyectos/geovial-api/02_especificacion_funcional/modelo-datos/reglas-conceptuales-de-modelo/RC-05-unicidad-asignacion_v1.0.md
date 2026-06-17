# RC-05 — Unicidad de la asignación agente-relevamiento

**Proyecto:** geovial-api
**Documento:** RC-05-unicidad-asignacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Enunciado

Un agente de campo está asignado a un relevamiento a lo sumo una vez vigente; no existen dos asignaciones vigentes del mismo agente al mismo relevamiento.

## 2. Entidades involucradas

Asignacion, Usuario (agente de campo), Relevamiento.

## 3. Tipo de restricción

Identidad y cardinalidad sobre el par agente-relevamiento.

## 4. Mecanismo de verificación conceptual

Al registrar una asignación se comprueba que no existe otra asignación vigente para el mismo par agente y relevamiento; un intento de asignar a un agente ya asignado no crea una asignación adicional. La revocación deja el par sin asignación vigente, habilitando una asignación futura.

## 5. RN o CU que la justifican

RN-01, RN-07; CU-05.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla conceptual de unicidad de la asignación agente-relevamiento. |
