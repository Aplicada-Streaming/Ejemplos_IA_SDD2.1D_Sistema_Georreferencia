# RN-02 — Orden de sincronización subir antes de bajar

**Proyecto:** geovial-mobile
**Documento:** RN-02-orden-sincronizacion-subir-antes-de-bajar_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + Mobile UX Analyst

## 1. Enunciado de la regla

En todo ciclo de sincronización de la app, primero se suben al backend los cambios locales pendientes del agente y solo después de que la subida concluye se bajan las actualizaciones del relevamiento asignado; la bajada no se atiende hasta que la subida del ciclo finaliza.

## 2. Justificación

Garantizar que el trabajo recolectado en terreno llegue antes de incorporar novedades del backend evita la pérdida o el pisado de cambios locales y hace predecible la sincronización, mitigando el riesgo de pérdida o duplicación de datos identificado por el negocio (NB-04, intake §11 R-03). La regla replica en el cliente la garantía dura del motor de sincronización (aplicada-sync RN-01) y la del backend (geovial-api RN-06).

## 3. Ámbito de aplicación

Se evalúa en cada ejecución de sincronización, automática (al detectar conexión) o forzada por el agente, sobre un relevamiento asignado. Aplica a la orquestación que la app delega en la librería de sincronización.

## 4. Consecuencia si se viola

Si la subida no concluye (por ejemplo, por corte de conexión), no se inicia la bajada: los cambios confirmados quedan aplicados, el resto permanece en la cola local y el relevamiento queda reanudable, sin pérdida ni duplicación. Un intento de bajar antes de concluir la subida se descarta y el ciclo se detiene de forma recuperable.

## 5. CU afectados

CU-06 (trabajar sin conexión y sincronizar). De forma indirecta, CU-02 (refresco de la lista de relevamientos con conexión, que dispara un ciclo de sincronización).

## 6. Pruebas que la verifican

- Un ciclo con cambios pendientes sube todos antes de cualquier bajada (08, sobre CU-06).
- Un corte durante la subida no dispara la bajada y deja el relevamiento reanudable sin duplicar (08, sobre CU-06).
- Con la cola vacía, el ciclo omite la subida y procede a la bajada (08, sobre CU-06).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de orden de sincronización subir-antes-de-bajar en el cliente móvil, derivada de NB-04 y alineada con aplicada-sync RN-01 y geovial-api RN-06. |
