# ADR-05 — Pipeline de orden estricto subir-antes-de-bajar

**Proyecto:** aplicada-sync
**Documento:** ADR-05-orden-subir-antes-de-bajar_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Estilo

## 1. Contexto

La garantía de negocio que hace confiable la sincronización en campo es que lo registrado en terreno llegue al backend antes de que el almacén local sea sobrescrito por novedades remotas. Si una bajada se aplicara mientras quedan cambios locales pendientes, podría pisar o descartar trabajo todavía no propagado. Lo motivan RN-01 (orden estricto subir-antes-de-bajar), CU-03 (ejecución del ciclo), CU-04 (disparo automático) y CU-06 (reanudación), además de la política central de la librería declarada en el intake §17 P.2 y P.11.

## 2. Decisión

El ciclo de sincronización se implementa como un pipeline de dos fases en orden estricto: primero la fase de subida completa de los pendientes confirmables y, solo tras verificar que no quedan pendientes confirmables, la fase de bajada de actualizaciones. El orden es una invariante dura del motor y no se expone como configurable hacia un orden inverso.

## 3. Estado

Aceptado el 2026-06-15. Pre-tomada por la política central de la librería (intake §17 P.2/P.11) y formalizada por RN-01.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Pipeline subir-luego-bajar con orden no configurable (elegida) | Garantiza no pisar cambios locales; comportamiento determinista y verificable; contrato simple | Una bajada nunca puede adelantarse aunque convenga por latencia |
| Orden configurable (subir-bajar o bajar-subir) | Flexibilidad para distintos hosts | Habilita la pérdida de cambios locales; rompe la garantía de negocio; superficie pública más ambigua |
| Subida y bajada concurrentes | Menor tiempo total del ciclo | No garantiza que la subida concluya antes de aplicar la bajada; riesgo de sobrescritura; difícil de razonar ante cortes |

## 5. Consecuencias positivas

- Ningún cambio local pendiente se pierde por una bajada prematura (cumple RN-01 y la integridad de NB-04).
- El ciclo es determinista y verificable: un corte en la subida nunca dispara la bajada (CU-03, CU-06).
- El mismo orden rige el ciclo manual y el automático, simplificando el razonamiento y las pruebas (CU-04).

## 6. Consecuencias negativas y trade-offs

- Se acepta que una bajada no pueda adelantarse aunque pudiera reducir el tiempo total: el orden prima sobre la latencia.
- Se acepta que, ante una cola grande con backend lento, la bajada espere a que toda la subida confirme.
- El orden fijo reduce la flexibilidad de configuración a cambio de una garantía dura, trade-off explícito y aceptado.

## 7. Implementación

Materializado por el Orquestador del ciclo y los Ejecutores de fase de subida y de bajada (arquitectura §3). El detalle paso a paso del pipeline, con los puntos de corte y las transiciones de estado, vive en `flujo-ejecucion_v1.0.md`. Convención: la fase de bajada solo arranca tras la verificación de cero pendientes confirmables; cualquier corte en la subida deja la sesión reanudable sin iniciar la bajada.

## 8. Métricas de validación

- 0 actualizaciones descendentes aplicadas mientras quedan pendientes confirmables (NFR de orden, arquitectura §8; CU-03 CA-01).
- Un corte en la fase de subida no dispara la fase de bajada (CU-03 CA-03, CU-06).
- El ciclo disparado por conectividad respeta el mismo orden (CU-04).
- Sincronización de un lote de 100 cambios en <= 30 s en red móvil típica (NFR de tiempo, arquitectura §8; intake §17 P.10).

## 9. Referencias

- RN-01; CU-03, CU-04, CU-06.
- NB-04; SOLUTION-INTAKE §17 P.2, P.10, P.11 (aplicada-sync).
- ADR-06 (reanudación), ADR-07 (idempotencia).
- `flujo-ejecucion_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión inicial del pipeline de orden estricto subir-antes-de-bajar, no configurable. |
