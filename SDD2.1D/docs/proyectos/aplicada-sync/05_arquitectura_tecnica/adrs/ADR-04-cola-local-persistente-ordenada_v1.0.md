# ADR-04 — Cola local persistente y ordenada de cambios pendientes

**Proyecto:** aplicada-sync
**Documento:** ADR-04-cola-local-persistente-ordenada_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Persistencia

## 1. Contexto

El trabajo de campo ocurre sin conexión: los cambios producidos deben conservarse de forma durable hasta poder subirse, sobrevivir a reinicios de la aplicación y a cortes, y respetar el orden en que se crearon. El motor debe tolerar una cola de al menos 1000 cambios pendientes y garantizar una sola entrada por cambio. Lo motivan CU-02 (registrar y encolar), CU-05 (consultar la cola), CU-06 (reanudar a partir de la cola), RN-02 (idempotencia por identificador estable) y el NFR de capacidad de cola del intake §17 P.10 (>= 1000 pendientes).

## 2. Decisión

Se adopta una cola local persistente y ordenada sobre el almacén local del host como estructura central de los pendientes. Cada cambio se almacena con su identificador estable como clave de unicidad, su operación, su carga útil opaca y su marca de orden de creación. La cola conserva el orden relativo de creación y mantiene una sola entrada por identificador (no duplica al reencolar el mismo cambio).

## 3. Estado

Aceptado el 2026-06-15. Deriva de la política de trabajo sin conexión de NB-04 y del modelo de cambio local definido en CU-02.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Cola persistente ordenada con clave por identificador estable (elegida) | Durable ante cortes y reinicios; orden de creación preservado; no duplica; soporta reanudación | Requiere disciplina de unicidad y de orden en el almacén local |
| Buffer en memoria sin persistencia | Máxima velocidad | Pierde todo ante un cierre o un corte; incompatible con el trabajo sin conexión |
| Bitácora de eventos append-only sin clave de unicidad | Orden trivial; simple de escribir | El reencolado duplicaría entradas; complica la idempotencia y el cómputo del tamaño de cola |

## 5. Consecuencias positivas

- Los cambios sobreviven a cortes y reinicios, condición del trabajo en campo (NB-04).
- El orden de creación preservado permite que el backend reciba la secuencia tal como ocurrió (CU-02 flujo 5.B).
- La clave por identificador estable habilita la no duplicación al encolar (CU-02 5.A) y la idempotencia al subir (RN-02).
- El tamaño de cola consultable refleja exactamente las entradas únicas pendientes (CU-05).

## 6. Consecuencias negativas y trade-offs

- Se acepta el costo de escritura durable en el almacén local por cada encolado.
- Se acepta que el almacén local pueda quedarse sin espacio; el motor lo reporta con un código estable sin dejar entrada parcial (CU-02).
- La forma física del almacén la define el adaptador del host, no el motor; el motor solo fija la forma lógica.

## 7. Implementación

Materializada por el componente Cola de cambios locales pendientes (arquitectura §3) sobre la abstracción de almacén local (ADR-02). Convención: el identificador estable es la clave de unicidad; el orden de creación se conserva como atributo persistido; la carga útil permanece opaca. El esquema lógico de metadatos se describe en `arquitectura-solucion_v1.0.md` §6.

## 8. Métricas de validación

- Tolera una cola de >= 1000 cambios pendientes sin degradación funcional (prueba de carga de 08; NFR del intake §17 P.10).
- Reencolar un identificador presente no incrementa el tamaño de cola (CU-02 CA-02).
- Los pendientes persisten y se recuperan tras un reinicio simulado (test previsto en 08; CU-01 5.A, CU-06).

## 9. Referencias

- CU-02, CU-05, CU-06; RN-02.
- NB-04; SOLUTION-INTAKE §17 P.10 (aplicada-sync).
- ADR-02 (almacén local como adaptador), ADR-07 (idempotencia).
- `arquitectura-solucion_v1.0.md` §6.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión inicial de cola local persistente y ordenada con clave por identificador estable. |
