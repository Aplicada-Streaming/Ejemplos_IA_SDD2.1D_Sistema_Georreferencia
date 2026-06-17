# ADR-06 — Reanudación por marca de progreso ante subida parcial

**Proyecto:** aplicada-sync
**Documento:** ADR-06-reanudacion-por-marca-de-progreso_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Persistencia

## 1. Contexto

Los cortes de conectividad son la condición normal del trabajo en campo. Una sincronización puede interrumpirse durante la fase de subida, dejando una subida parcial: algunos cambios confirmados por el backend y otros no. El motor debe poder retomar desde el punto de corte sin reenviar de forma efectiva lo ya confirmado ni haber bajado actualizaciones antes de concluir la subida. Lo motivan CU-06 (reanudar una sincronización interrumpida), RN-01 (no bajar antes de concluir la subida), RN-02 (idempotencia) y el NFR de reanudación sin pérdida del intake §17 P.10.

## 2. Decisión

El motor persiste una marca de progreso de la fase de subida y, ante una subida parcial, deja la sesión en estado reanudable. Al reanudar, el motor reenvía únicamente los cambios no confirmados, en orden de creación, apoyándose en la idempotencia del backend por identificador estable; solo tras concluir la subida procede a la bajada. La cola persistida es la fuente de verdad ante una inconsistencia entre la marca de progreso y la cola.

## 3. Estado

Aceptado el 2026-06-15. Deriva de la política de integridad de NB-04 y del caso límite del intake §7 (pérdida de conexión en medio de una sincronización con subida parcial).

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Marca de progreso persistida + reanudación idempotente (elegida) | Continúa desde el punto de corte; no reenvía de forma efectiva lo confirmado; sin pérdida ni duplicación | Requiere persistir y conciliar la marca con la cola |
| Reinicio completo del ciclo desde cero ante cualquier corte | Lógica más simple | Reenvía todo; depende por completo de la idempotencia del backend; desperdicia red en campo |
| Confianza exclusiva en la marca de progreso sin cola como fuente de verdad | Menos lectura de la cola | Una marca desfasada perdería cambios; viola la integridad de NB-04 |

## 5. Consecuencias positivas

- Un corte en campo no produce pérdida ni duplicación de datos (cumple NB-04 y el NFR de reanudación).
- La reanudación reenvía solo los faltantes, ahorrando red en condiciones móviles (CU-06 CA-01).
- Adoptar la cola como fuente de verdad ante inconsistencia prioriza no perder cambios (CU-06 error PROGRESO_INCONSISTENTE).

## 6. Consecuencias negativas y trade-offs

- Se acepta el costo de persistir y conciliar la marca de progreso con la cola en cada avance de subida.
- Se acepta que, ante inconsistencia, se reenvíen pendientes que el backend ya pudo haber recibido, confiando en su idempotencia (RN-02).
- La garantía de no duplicación en la reanudación depende de que el backend reconozca los identificadores estables; es una expectativa del contrato de transporte.

## 7. Implementación

Materializada por el Orquestador del ciclo, los Ejecutores de fase y el Registro de estado y progreso (arquitectura §3), con la marca de progreso y la marca de última sincronización como metadatos del almacén local (§6). El detalle de la continuación desde el punto de corte vive en `flujo-ejecucion_v1.0.md`. Convención: la bajada nunca inicia durante una reanudación hasta concluir la subida pendiente (RN-01).

## 8. Métricas de validación

- 0 cambios perdidos y 0 duplicados tras un corte en la fase de subida (NFR de reanudación, arquitectura §8; intake §17 P.10).
- La reanudación reenvía solo los faltantes y recién entonces baja (CU-06 CA-01).
- Un nuevo corte durante la reanudación conserva el avance y deja la sesión reanudable (CU-06 CA-04).

## 9. Referencias

- CU-06; RN-01, RN-02.
- NB-04; SOLUTION-INTAKE §7 y §17 P.10 (aplicada-sync).
- ADR-05 (orden del pipeline), ADR-07 (idempotencia).
- `flujo-ejecucion_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión inicial de reanudación por marca de progreso ante subida parcial, con la cola como fuente de verdad. |
