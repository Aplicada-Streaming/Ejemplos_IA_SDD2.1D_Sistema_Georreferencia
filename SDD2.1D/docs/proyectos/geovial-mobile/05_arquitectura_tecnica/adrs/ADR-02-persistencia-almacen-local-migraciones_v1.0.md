# ADR-02 — Persistencia en almacén local con migraciones versionadas

**Proyecto:** geovial-mobile
**Documento:** ADR-02-persistencia-almacen-local-migraciones_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto Móvil
**Categoría:** Persistencia

## 1. Contexto

La captura de campo es offline-first: toda observación (crear y mover marcadores, capturar fotos con resolución de coordenadas, registrar notas, comentarios y etiquetas) debe persistirse sin conexión y conservarse hasta su sincronización confirmada, sin pérdida (RN-05). La app necesita un almacén local del dispositivo que (a) replique las entidades de recolección del dominio autoritativo, (b) sostenga una cola local persistente de cambios con orden de creación e identificador de origen estable, (c) registre la marca de sincronización por relevamiento y (d) evolucione su esquema de forma auditable entre versiones de la app. El intake fija el almacén local y las migraciones versionadas como decisión pre-tomada (intake §17.P.4, §17.P.11) y declara `tiene_persistencia = true`. El modelo conceptual local define 8 entidades (02). Cubre CU-02 a CU-07.

## 2. Decisión

Se adopta un almacén local persistente del dispositivo como soporte de trabajo offline, con un esquema de 8 entidades (RelevamientoLocal, MarcadorLocal, ObservacionLocal, FotoLocal, ComentarioLocal, EtiquetaLocal, CambioEncolado y MarcaSincronizacionLocal) y migraciones versionadas aplicadas en el arranque de la app. La cola de cambios (`CambioEncolado`) conserva el orden de creación y un identificador de origen estable por cambio para la idempotencia; tolera al menos 1000 cambios pendientes. El binario de cada foto se aloja en el dispositivo y se referencia lógicamente desde `FotoLocal`, sin guardarse en la fila de datos. El almacén local es réplica parcial del dominio autoritativo de la API, que prevalece al sincronizar; no es la fuente de verdad.

## 3. Estado

Aceptado el 2026-06-15. Decisión pre-tomada en el intake (§17.P.4, §17.P.11).

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Almacén local persistente con migraciones versionadas (elegida) | Captura offline durable; cola persistente con orden e idempotencia; esquema auditable entre versiones; soporta ≥ 1000 cambios | Requiere mantener migraciones y disciplina de réplica del dominio autoritativo |
| Estado en memoria volcado a archivo plano | Implementación simple | No soporta consultas, índices ni integridad de la cola; riesgo de pérdida ante reinicio; no escala a 1000 cambios |
| Sin almacén local, cola en memoria | Mínima superficie | Pierde la captura ante cierre o reinicio; contradice RN-05 y la capacidad offline-first |
| Esquema sin migraciones versionadas | Menos tooling | Imposible reconstruir o auditar el esquema entre versiones de la app; rompe el criterio de migración versionada |

## 5. Consecuencias positivas

1. La captura offline se persiste de forma durable y se conserva hasta la confirmación (RN-05).
2. La cola con orden de creación e identificador de origen estable habilita la idempotencia y la reanudación de la sincronización (RN-02, ADR-03).
3. Las migraciones versionadas permiten reconstruir y auditar el esquema local entre versiones de la app.
4. El almacén soporta al menos 1000 cambios pendientes sin pérdida (NFR de capacidad de cola).

## 6. Consecuencias negativas y trade-offs

1. Mantener el esquema local replicado del dominio autoritativo exige disciplina: un cambio del dominio puede requerir una migración local; se acepta a cambio de la capacidad offline.
2. Las migraciones suman tooling y un paso en el arranque; se acepta por la auditabilidad del esquema.
3. Alojar binarios fuera de la fila de datos requiere coordinar el ciclo de vida del binario con su referencia lógica; se acepta para no inflar el almacén de datos.

## 7. Implementación

- El repositorio del almacén local expone las operaciones de persistencia y consulta de las 8 entidades y de la cola; las migraciones versionadas corren en el arranque, con identificador de migración (ver `modelo-datos-logico_v1.0.md`).
- Cada captura se persiste como transacción local: la entidad y su `CambioEncolado` se escriben juntos o no se escribe ninguno (RN-05).
- `CambioEncolado` lleva identificador de origen estable y orden de creación monótono; se retira de la cola solo tras confirmación de la subida (ADR-03).
- `MarcaSincronizacionLocal` persiste el punto de sincronización por relevamiento; los metadatos los gestiona la librería de sincronización (intake §17.P.4) y la app los persiste.
- El binario de la foto se aloja en el dispositivo; si no hay espacio, no se persiste el binario y se avisa al agente (ADR-04).
- El detalle de tipos físicos, índices, restricciones y migración inicial vive en `modelo-datos-logico_v1.0.md`.

## 8. Métricas de validación

- La cola tolera ≥ 1000 cambios pendientes sin pérdida ni alteración del orden de creación (NFR de capacidad, verificado en 08).
- Una captura offline interrumpida no deja entidades sin su cambio encolado ni a la inversa (atomicidad local, 08).
- El esquema se reconstruye desde la migración inicial identificada (auditoría de migraciones, 08).

## 9. Referencias

- NB-03, NB-04; CU-02 a CU-07; RN-05; RN-02.
- Intake §17.P.4, §17.P.10, §17.P.11; modelo conceptual del almacén local (02).
- ADRs relacionadas: ADR-01 (estilo offline-first), ADR-03 (sincronización), ADR-04 (permisos y espacio).
- `modelo-datos-logico_v1.0.md`; `arquitectura-solucion_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de persistencia: almacén local persistente de 8 entidades con migraciones versionadas en el arranque, cola con orden de creación e identificador de origen estable y binarios de foto referenciados lógicamente. Aceptada (pre-tomada en intake §17.P.4, §17.P.11). |
