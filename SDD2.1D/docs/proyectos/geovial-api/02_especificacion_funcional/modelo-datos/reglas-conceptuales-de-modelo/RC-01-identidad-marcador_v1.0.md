# RC-01 — Identidad estable del marcador geográfico

**Proyecto:** geovial-api
**Documento:** RC-01-identidad-marcador_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Enunciado

Cada marcador geográfico tiene una identidad propia y estable dentro de su relevamiento que no cambia cuando el marcador se mueve a otra coordenada, se etiqueta o se comparte entre observaciones; mover o reetiquetar un marcador nunca genera un marcador nuevo.

## 2. Entidades involucradas

MarcadorGeografico, Observacion, Etiqueta.

## 3. Tipo de restricción

Identidad.

## 4. Mecanismo de verificación conceptual

Al modificar la coordenada o las etiquetas de un marcador, la identidad referenciada por sus observaciones permanece la misma; las observaciones siguen ancladas al mismo marcador antes y después del cambio, sin reanclaje ni duplicación.

## 5. RN o CU que la justifican

RN-03; CU-07, CU-08, CU-09, CU-13.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla conceptual de identidad estable del marcador geográfico. |
