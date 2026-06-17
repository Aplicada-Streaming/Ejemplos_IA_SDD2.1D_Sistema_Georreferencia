# RN-01 — Prioridad de la ubicación incrustada y radio de agrupación en la carga manual

**Proyecto:** geovial-mobile
**Documento:** RN-01-prioridad-ubicacion-radio-agrupacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + Mobile UX Analyst

## 1. Enunciado de la regla

En la carga manual de fotos desde el dispositivo, la ubicación de cada foto se determina con prioridad a partir de los datos de ubicación incrustados en la propia imagen, y las fotos cuya ubicación cae dentro del radio de agrupación de un marcador local se agrupan en ese marcador; si no hay marcador dentro del radio, se crea uno nuevo en la ubicación de la foto.

## 2. Justificación

Aprovechar la georreferenciación que la foto trae consigo evita la reubicación manual en oficina y agrupa la evidencia por cercanía de forma consistente, núcleo de la propuesta de valor de la captura estructurada (NB-03, intake §3, §4 F-09). La regla replica en el cliente la invariante equivalente del backend (geovial-api RN-04) para que la carga manual sea coherente con y sin conexión.

## 3. Ámbito de aplicación

Se evalúa en cada carga manual de fotos a un relevamiento dentro de la app móvil y en la consecuente creación o reutilización de marcadores locales, antes de su sincronización. El radio de agrupación es un parámetro aplicable a la carga. No aplica a la captura en terreno, donde la coordenada se resuelve del GPS en el momento (CU-04).

## 4. Consecuencia si se viola

Una carga sin radio de agrupación aplicable se rechaza con el código RADIO_NO_DEFINIDO y no se procesa el conjunto. Una foto sin datos de ubicación incrustados no se agrupa por radio y queda registrada como pendiente de ubicación manual, sin inventarle una coordenada.

## 5. CU afectados

CU-07 (carga manual con radio de agrupación). De forma indirecta, las fotos pendientes de ubicación se ubican luego mediante CU-03 (creación o movimiento de marcadores).

## 6. Pruebas que la verifican

- Fotos con ubicación incrustada dentro del radio agrupadas en un único marcador local y foto lejana en un marcador nuevo (08, sobre CU-07).
- Foto sin ubicación incrustada queda pendiente de ubicación manual, sin coordenada inventada (08, sobre CU-07).
- Carga sin radio aplicable rechazada con RADIO_NO_DEFINIDO (08, sobre CU-07).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de prioridad de ubicación incrustada y radio de agrupación en la carga manual del cliente móvil, derivada de NB-03 y alineada con geovial-api RN-04. |
