# ADR-07 — Idempotencia por identificador de cambio estable

**Proyecto:** aplicada-sync
**Documento:** ADR-07-idempotencia-por-identificador-estable_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Persistencia

## 1. Contexto

Para reintentar y reanudar con seguridad ante cortes —la condición normal del trabajo en campo— el motor debe poder reenviar un cambio sin riesgo de aplicarlo dos veces, y aplicar una actualización descendente una sola vez. Sin idempotencia, un reenvío tras un corte produciría duplicados que rompen la integridad. Lo motivan RN-02 (idempotencia), CU-02 (no duplicar al encolar), CU-03 (no aplicar dos veces al subir o bajar), CU-06 (reanudación segura) y el NFR de idempotencia del intake §17 P.10.

## 2. Decisión

La idempotencia descansa en un identificador de cambio estable que el host provee por cada cambio local y que el backend reconoce. Un mismo cambio se aplica una sola vez con efecto en el backend y una sola vez en el almacén local, sin importar cuántas veces se encole, se reintente o se reanude. El motor usa ese identificador como clave de no duplicación en la cola y como clave de reconocimiento en la subida; la fase de bajada aplica cada actualización una sola vez por su identidad.

## 3. Estado

Aceptado el 2026-06-15. Pre-tomada por la política de integridad de NB-04 (cero observaciones perdidas o duplicadas) y formalizada por RN-02.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Identificador de cambio estable provisto por el host (elegida) | Reintento y reanudación seguros; clave única de cola; reconocimiento por el backend | El host debe garantizar la estabilidad y unicidad del identificador |
| Deduplicación por contenido (hash de la carga útil) | No exige identificador explícito | Rompe la opacidad de la carga útil; dos cambios con igual contenido y distinta intención colisionan |
| Sin idempotencia, confiando en no reintentar | Implementación trivial | Cualquier corte produce duplicados; inviable para trabajo sin conexión |

## 5. Consecuencias positivas

- El motor puede reenviar libremente tras un corte porque el efecto neto es único (cumple RN-02 y la reanudación de ADR-06).
- Reencolar el mismo cambio no duplica la entrada en la cola (CU-02 5.A).
- Una actualización descendente ya aplicada no se vuelve a aplicar en el almacén local (CU-03).
- Mantiene la carga útil opaca: la idempotencia no inspecciona el contenido de dominio.

## 6. Consecuencias negativas y trade-offs

- Se acepta trasladar al host la responsabilidad de generar identificadores estables y únicos; un identificador inestable rompería la garantía.
- Se acepta que la idempotencia efectiva en el backend depende de que este reconozca el identificador; es una expectativa del contrato de transporte.
- El motor rechaza con código estable un cambio sin identificador (CU-02), imponiendo una precondición al integrador.

## 7. Implementación

Materializada en la Cola (clave por identificador) y en los Ejecutores de fase de subida y de bajada (arquitectura §3). Convención: el identificador estable es obligatorio en cada cambio local (CU-02); la fase de subida lo envía como clave de reconocimiento; la fase de bajada aplica cada actualización por su identidad una sola vez. El contrato de transporte exige que el backend reconozca el identificador (ver `contratos-abstractions_v1.0.md`).

## 8. Métricas de validación

- 100 % de los cambios reenviados o reaplicados producen efecto neto único (NFR de idempotencia, arquitectura §8; intake §17 P.10).
- Reencolar un identificador presente no duplica la entrada (CU-02 CA-02).
- Un reenvío tras un corte no aplica el efecto dos veces en el backend (CU-03, CU-06 CA-02).
- Una actualización descendente ya aplicada no se reaplica en el almacén local (test previsto en 08; RN-02).

## 9. Referencias

- RN-02; CU-02, CU-03, CU-06.
- NB-04; SOLUTION-INTAKE §17 P.10 (aplicada-sync).
- ADR-04 (cola con clave por identificador), ADR-06 (reanudación).
- `contratos-abstractions_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión inicial de idempotencia basada en el identificador de cambio estable provisto por el host. |
