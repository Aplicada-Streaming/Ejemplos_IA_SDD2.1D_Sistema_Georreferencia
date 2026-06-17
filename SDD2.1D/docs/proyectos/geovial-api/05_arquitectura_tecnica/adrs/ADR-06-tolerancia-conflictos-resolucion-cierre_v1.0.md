# ADR-06 — Tolerancia a conflictos de marcadores y resolución al cierre

**Proyecto:** geovial-api
**Documento:** ADR-06-tolerancia-conflictos-resolucion-cierre_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Estilo

## 1. Contexto

Dos o más marcadores dentro de un mismo radio constituyen un conflicto de marcadores. El negocio decidió que ese conflicto es un estado válido durante la recolección y la revisión: el sistema convive con él, mantiene la información accesible y no bloquea ninguna operación por su causa; la unificación o separación se difiere al cierre y queda a cargo del jefe de área (RN-03, intake §7, §14). El cierre exige como precondición que no queden conflictos pendientes (RN-05, RC-04). El marcador conserva una identidad propia y estable que no cambia al moverse, etiquetarse o compartirse (RC-01). Esta tolerancia distingue a GeoVial de un sistema que bloquearía la captura ante un conflicto. Cubre CU-07, CU-08, CU-10, CU-11, CU-12, CU-13, CU-14.

## 2. Decisión

Se modela el conflicto de marcadores como una entidad de primera clase (ConflictoMarcadores) con estado pendiente o resuelto, no como un caso de error. Crear o mover marcadores, recibir y entregar cambios en la sincronización, y consultar el relevamiento para revisión nunca se bloquean por un conflicto: el conflicto se registra y la operación continúa (RN-03). El marcador mantiene su identidad estable ante movimiento, etiquetado y compartición (RC-01). El cierre (CU-14) valida la ausencia de conflictos pendientes y, en su defecto, se rechaza con CONFLICTOS_PENDIENTES; la resolución (CU-13) unifica o separa marcadores antes del cierre.

## 3. Estado

Aceptado el 2026-06-15. Decisión pre-tomada en el intake (§17.P.2, §17.P.4, §17.P.11): conflictos tolerados durante la recolección y resueltos al cierre.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Conflicto como estado válido resuelto al cierre (elegido) | No detiene la captura en campo; conserva toda la evidencia; el jefe decide con la información completa (RN-03) | El modelo y las consultas deben tolerar conflictos presentes; el cierre necesita una compuerta de resolución |
| Bloquear la operación ante un conflicto | Catalogación siempre consistente en el momento | Detiene el trabajo de campo sin aportar valor; contradice RN-03 y el riesgo R-03 del negocio; descartada por el intake |
| Unificar marcadores automáticamente por proximidad | Sin intervención manual | Decide por el jefe sin su criterio; puede fusionar marcadores que describen cosas distintas; pierde evidencia; rompe RC-01 |
| Resolver conflictos durante la recolección por el agente | Resolución temprana | El agente no tiene la evidencia completa ni la potestad; la catalogación correcta requiere el criterio del jefe al cierre |

## 5. Consecuencias positivas

1. La recolección en campo no se detiene ante un conflicto: la subida (CU-10) incorpora marcadores en conflicto sin bloquear (RN-03).
2. La información queda accesible durante la revisión pese a conflictos presentes (CU-12).
3. El jefe resuelve con la evidencia completa al cierre, unificando o separando según su criterio (CU-13).
4. La recolección puede entregarse antes que la resolución de conflictos sin romper el camino end-to-end (intake §15).

## 6. Consecuencias negativas y trade-offs

1. El modelo de datos y las consultas de revisión y sincronización deben tolerar marcadores en conflicto, lo que agrega la entidad ConflictoMarcadores y su estado.
2. El cierre necesita una compuerta que verifique la ausencia de conflictos pendientes (RC-04), una validación adicional respecto de un cierre sin precondición.
3. Diferir la resolución acumula conflictos hasta el cierre; se acepta porque la resolución temprana no aporta valor y perjudicaría la captura.

## 7. Implementación

- ConflictoMarcadores se persiste con estado pendiente o resuelto, vinculado al relevamiento y a dos o más marcadores (modelo lógico).
- Los casos de uso de marcadores y sincronización (CU-07, CU-08, CU-10, CU-11) registran el conflicto y continúan, sin rechazar la operación.
- La identidad estable del marcador (RC-01) se materializa con un identificador propio que no cambia al mover o etiquetar.
- El cierre (CU-14) valida que no haya conflictos en estado pendiente (RC-04, RN-05) y, si los hay, devuelve CONFLICTOS_PENDIENTES; la resolución (CU-13) cambia el estado a resuelto.
- Convención impuesta: ninguna operación de recolección o revisión se bloquea por un conflicto; solo el cierre lo exige resuelto.

## 8. Métricas de validación

- Creación y sincronización de marcadores en conflicto sin bloquear la operación (RN-03, verificado en 08 sobre CU-07, CU-10, CU-11).
- Información accesible durante la revisión pese a conflictos presentes (CU-12).
- Cierre rechazado con conflictos pendientes y aceptado tras resolverlos (CU-13, CU-14).
- Mover o reetiquetar un marcador no genera un marcador nuevo ni reancla sus observaciones (RC-01, verificado en 08).

## 9. Referencias

- NB-04, NB-05; CU-07, CU-08, CU-10, CU-11, CU-12, CU-13, CU-14; RN-03, RN-05; RC-01, RC-04.
- Intake §7 (casos límite), §14 (estilo de la solución), §17.P.2, §17.P.4, §17.P.11.
- ADRs relacionadas: ADR-02 (persistencia), ADR-07 (orden de sincronización), ADR-08 (idempotencia).
- `flujo-ejecucion_v1.0.md`; `modelo-datos-logico_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de dominio: el conflicto de marcadores es un estado válido que convive con la operación y se resuelve al cierre; identidad estable del marcador. Aceptada (pre-tomada en intake §17.P.11). |
