# RN-04 — Priorización de la ubicación incrustada y radio de agrupación de fotos

**Proyecto:** geovial-api
**Documento:** RN-04-radio-agrupacion-fotos_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Enunciado de la regla

En la carga manual de fotos, la ubicación de cada foto se determina con prioridad a partir de los datos de ubicación incrustados en la propia imagen, y las fotos cuya ubicación cae dentro del radio de agrupación de un marcador se agrupan en ese marcador; si no hay marcador dentro del radio, se crea uno nuevo en la ubicación de la foto.

## 2. Justificación

Aprovechar la georreferenciación que la foto trae consigo evita la reubicación manual en oficina y agrupa la evidencia por cercanía de forma consistente, que es el núcleo de la propuesta de valor de la captura estructurada (NB-03, intake §3, §4 F-09).

## 3. Ámbito de aplicación

Se evalúa en cada carga manual de fotos a un relevamiento y en la consecuente creación o reutilización de marcadores. El radio de agrupación es un parámetro configurable aplicable a la carga.

## 4. Consecuencia si se viola

Una carga sin radio de agrupación aplicable se rechaza con el código RADIO_NO_DEFINIDO. Una foto sin datos de ubicación incrustados no se agrupa por radio y queda registrada como pendiente de ubicación manual, sin inventarle una coordenada.

## 5. CU afectados

CU-07, CU-09.

## 6. Pruebas que la verifican

- Fotos dentro del radio agrupadas en un único marcador y foto lejana en un marcador nuevo (08, sobre CU-09).
- Foto sin ubicación incrustada queda pendiente de ubicación manual (08, sobre CU-09).
- Carga sin radio aplicable rechazada con RADIO_NO_DEFINIDO (08, sobre CU-09).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de priorización de la ubicación incrustada y radio de agrupación de fotos, derivada de NB-03. |
