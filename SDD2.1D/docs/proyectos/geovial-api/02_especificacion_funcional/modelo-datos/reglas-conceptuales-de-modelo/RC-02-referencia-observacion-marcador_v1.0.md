# RC-02 — Referencia obligatoria de observación a marcador

**Proyecto:** geovial-api
**Documento:** RC-02-referencia-observacion-marcador_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Enunciado

Toda observación se ancla a un marcador geográfico existente del mismo relevamiento; no existe observación sin marcador, y un marcador no puede darse de baja mientras conserve observaciones ancladas.

## 2. Entidades involucradas

Observacion, MarcadorGeografico, Relevamiento.

## 3. Tipo de restricción

Referencial.

## 4. Mecanismo de verificación conceptual

Al crear una observación se comprueba que el marcador referenciado existe y pertenece al relevamiento accesible; al intentar dar de baja un marcador se comprueba que no quedan observaciones ancladas. Una referencia a marcador inexistente o una baja que dejaría observaciones huérfanas se impide.

## 5. RN o CU que la justifican

RN-03; CU-07, CU-08, CU-13.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla conceptual de referencia obligatoria de observación a marcador. |
