# RN-02 — Conservación de la autoría histórica ante la baja

**Proyecto:** geovial-api
**Documento:** RN-02-conservacion-autoria-en-baja_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Enunciado de la regla

La baja de un usuario revoca su acceso al sistema pero conserva intacta la autoría histórica de todo lo que registró: sus observaciones, fotos, comentarios, etiquetas y relevamientos permanecen atribuidos a su identidad, sin borrarse.

## 2. Justificación

La trazabilidad y la reproducibilidad de los informes de cierre exigen que cada registro quede asociado de forma permanente a un responsable identificado, aun cuando ese usuario ya no opere; sin esto, la baja destruiría evidencia y rompería la accountability (NB-01, intake §1).

## 3. Ámbito de aplicación

Se evalúa en toda baja de usuario y en toda autenticación posterior de un usuario dado de baja. Se aplica al momento de inhabilitar el acceso y al consultar la autoría de registros históricos.

## 4. Consecuencia si se viola

Si la baja intentara borrar o desatribuir registros, la operación se rechaza y no se ejecuta; un usuario dado de baja que intente autenticarse es rechazado con el código USUARIO_INHABILITADO, conservando su traza.

## 5. CU afectados

CU-01, CU-02, CU-03, CU-18.

## 6. Pruebas que la verifican

- La baja de un agente conserva sus observaciones con su autoría (08, sobre CU-01 y CU-02).
- Un usuario dado de baja no puede autenticarse pero su traza permanece (08, sobre CU-03).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de conservación de la autoría histórica ante la baja, derivada de NB-01. |
