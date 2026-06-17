# ADR-08 — Idempotencia de operaciones no seguras y de la sincronización

**Proyecto:** geovial-api
**Documento:** ADR-08-idempotencia-operaciones-no-seguras_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Comunicación

## 1. Contexto

La captura sin conexión reenvía lotes tras cortes de red, y cualquier escritura puede reintentarse ante una respuesta no recibida. Sin idempotencia, esos reintentos duplicarían marcadores, observaciones, fotos, usuarios o asignaciones, materializando el riesgo de duplicación de datos (R-03). La regla exige que todo cambio subido porte un identificador de origen estable y que toda operación no segura reintentable porte una clave de idempotencia; reenviar un cambio ya aplicado o reintentar con la misma clave no debe producir efectos duplicados (RN-07). Aplica a altas, asignaciones, transiciones, resoluciones, importaciones y a la subida de sincronización. Cubre CU-21 (transversal) y CU-10 (subida). La unicidad de la asignación (RC-05) y la monotonía de la marca (RC-06) refuerzan la garantía.

## 2. Decisión

Se adopta idempotencia explícita para las operaciones no seguras reintentables mediante una clave de idempotencia provista por el cliente, y para la subida de sincronización mediante un identificador de origen estable por cada cambio del lote. El backend verifica, antes de ejecutar el efecto, si la clave o el identificador ya fue procesado: si es nuevo, ejecuta y registra el resultado asociado a la clave; si ya fue procesado, devuelve el resultado registrado sin reejecutar. La clave reutilizada con un contenido distinto se rechaza con CLAVE_REUTILIZADA_INCONSISTENTE. Un reintento concurrente con la misma clave no inicia una segunda ejecución. La unicidad de la clave se garantiza con una restricción del almacén.

## 3. Estado

Aceptado el 2026-06-15. Decisión pre-tomada en el intake (§17.P.11) y derivada de RN-07.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Clave de idempotencia + identificador de origen con registro persistido (elegido) | Reintentos seguros ante cortes; reenvío reconocido sin duplicar (RN-07); unicidad garantizada por el almacén | Requiere persistir claves y su resultado; política de retención de claves |
| Sin idempotencia (confiar en que el cliente no reintenta) | Implementación trivial | Cualquier corte duplica datos; materializa R-03; descartada por RN-07 |
| Deduplicación por comparación de contenido | Sin clave explícita | Costosa y ambigua; dos altas legítimas iguales serían tratadas como duplicado; no distingue reintento de operación nueva |
| Idempotencia solo en la sincronización | Cubre el caso más crítico | Deja sin proteger altas, asignaciones e importaciones reintentables fuera del ciclo de sync; insuficiente para RN-07 |

## 5. Consecuencias positivas

1. Un reenvío de lote tras un corte se reconoce y no reaplica cambios ya confirmados (RN-07, CU-10).
2. Un alta o asignación reintentada con la misma clave no duplica el recurso (CU-21, RC-05).
3. Las escrituras reintentables son seguras ante respuestas no recibidas y cortes de conexión (CU-21, garantía).
4. La unicidad de la clave a nivel del almacén evita condiciones de carrera entre reintentos concurrentes.

## 6. Consecuencias negativas y trade-offs

1. El backend persiste claves de idempotencia y su resultado, con una política de retención temporal (la representación exacta y el alcance de retención se fijan en el modelo lógico y en 06).
2. El cliente debe generar claves estables por operación; una clave inconsistente con su contenido se rechaza (CLAVE_REUTILIZADA_INCONSISTENTE), lo que traslada disciplina al cliente.
3. La verificación de idempotencia agrega una sobrecarga mínima a cada operación no segura; se acepta dentro del objetivo de latencia de escritura.

## 7. Implementación

- El servicio de idempotencia (capa de Aplicación) consulta y registra claves contra una tabla técnica con restricción única por clave (ADR-02).
- La subida de sincronización deduplica por identificador de origen de cada cambio del lote antes de aplicar el efecto (CU-10, RN-07).
- Un reintento durante el procesamiento en curso no inicia una segunda ejecución e informa que la operación está en curso (CU-21, FA-01).
- Las operaciones que admiten clave se declaran en el contrato (`contratos-rest_v1.0.md`); las que no la admiten ignoran o rechazan la clave según el recurso (OPERACION_NO_IDEMPOTENTE).
- Convención impuesta: toda operación no segura reintentable declara si admite clave de idempotencia y se comporta de forma idempotente cuando la admite.

## 8. Métricas de validación

- 100 % de las operaciones repetidas con la misma clave sin efecto duplicado (intake §17.P.10, CU-21).
- Reenvío de un lote tras un corte reconocido sin duplicar cambios (RN-07, verificado en 08 sobre CU-10).
- Alta reintentada con la misma clave que no duplica el recurso; clave reutilizada con contenido distinto rechazada (CU-21).
- El lote de ≥ 1000 cambios se aplica una sola vez aun ante reenvío (intake §17.P.10).

## 9. Referencias

- NB-04, NB-01, NB-02, NB-06; CU-10, CU-21; RN-07; RC-05, RC-06.
- Intake §11 R-03 (riesgo), §17.P.10 (NFR), §17.P.11.
- ADRs relacionadas: ADR-02 (persistencia de claves), ADR-07 (orden de sincronización), ADR-05 (errores).
- `contratos-rest_v1.0.md`; `flujo-ejecucion_v1.0.md`; `modelo-datos-logico_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de comunicación: idempotencia explícita por clave para operaciones no seguras y por identificador de origen para la subida de sincronización, con registro persistido y unicidad garantizada por el almacén. Aceptada (pre-tomada en intake §17.P.11). |
