# ADR-02 — Persistencia en almacén relacional con migraciones versionadas

**Proyecto:** geovial-api
**Documento:** ADR-02-persistencia-almacen-relacional-migraciones_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Persistencia

## 1. Contexto

El backend debe persistir las 12 entidades del modelo conceptual con invariantes que exigen consistencia inmediata: integridad referencial de la jerarquía de usuarios (RC-03), referencia obligatoria de observación a marcador (RC-02), unicidad de la asignación agente-relevamiento (RC-05), estado del relevamiento dentro de un catálogo cerrado con transiciones acotadas (RC-04, RN-05) y monotonía de la marca de sincronización (RC-06). Estas reglas se materializan mejor como restricciones declarativas del almacén que como lógica de aplicación. El esquema debe poder reconstruirse y auditarse, y evolucionar de forma versionada (intake §17.P.4). La solución es single-tenant para una única organización (intake §17.P.4, multi_tenant=false). Cubre CU-01 a CU-17.

## 2. Decisión

Se adopta un almacén relacional como persistencia principal del backend, con el esquema definido y evolucionado mediante migraciones versionadas de la herramienta de migraciones del runtime, aplicadas en un arranque controlado del despliegue antes de habilitar el tráfico. Las invariantes de integridad (PK, FK, unicidad, check, catálogos cerrados) se declaran a nivel del almacén dentro de la misma transacción que la operación de dominio. Cada comando que muta estado se ejecuta en una transacción local atómica. No hay multi-tenancy: la jerarquía de roles es control de acceso, no aislamiento por tenant.

## 3. Estado

Aceptado el 2026-06-15. Decisión pre-tomada en el intake (§17.P.4, §17.P.11): persistencia relacional con migraciones versionadas; multi_tenant=false.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Almacén relacional con migraciones versionadas (elegido) | Transacciones atómicas; integridad referencial y restricciones declarativas (RC-02, RC-03, RC-05); esquema auditable y reconstruible | Costo operativo de gestionar el esquema y las migraciones |
| Almacén documental | Esquema flexible; escala horizontal trivial | No garantiza integridad referencial; complica la unicidad de asignación y la jerarquía; obligaría a reimplementar consistencia en la aplicación |
| Almacén clave-valor | Latencia muy baja | Requiere reimplementar transacciones y restricciones en la aplicación; inviable para el modelo relacional de 12 entidades vinculadas |
| Esquema sin migraciones versionadas | Menos ceremonia inicial | Imposible reconstruir o auditar el esquema; rompe el criterio de aceptación de la regla 05 §4.4 |

## 5. Consecuencias positivas

1. RC-05 (unicidad de asignación) y RC-03 (integridad de la jerarquía) se materializan con restricciones únicas y foráneas a nivel del almacén, sin lógica adicional ni condiciones de carrera.
2. RC-04 y RN-05 (estado dentro del ciclo válido) se sostienen con un catálogo cerrado y la transición validada en la misma transacción.
3. La atomicidad por transacción evita estados parciales en altas, transiciones y subidas de sincronización.
4. El esquema es auditable y reconstruible desde la migración inicial referenciada en `modelo-datos-logico_v1.0.md`.

## 6. Consecuencias negativas y trade-offs

1. Requiere un plan de migraciones versionado desde el inicio y un arranque controlado que las aplique antes del tráfico.
2. Acopla el backend al modelo relacional; un cambio futuro de paradigma implicaría reescribir los repositorios (mitigado por el puerto de repositorio de ADR-01).
3. Las restricciones del almacén deben mantenerse alineadas con las invariantes del Dominio; una divergencia silenciosa sería un defecto (mitigado por pruebas de integración en 08).

## 7. Implementación

- El mapeo de las 12 entidades a tablas con tipos físicos, índices y restricciones vive en `modelo-datos-logico_v1.0.md`, con la migración inicial referenciada.
- Los repositorios de la capa de Infraestructura implementan los puertos declarados por la Aplicación (ADR-01).
- Las claves de idempotencia (ADR-08) se persisten en una tabla técnica con restricción única por clave.
- Las migraciones se ejecutan con la herramienta de migraciones del runtime en el arranque controlado del despliegue (categoría 09); no se nombra el producto concreto.
- Convención impuesta: toda invariante de integridad referencial o de unicidad se declara a nivel del almacén, no solo en la aplicación.

## 8. Métricas de validación

- Latencia p95 de escrituras ≤ 500 ms incluyendo la transacción del almacén (intake §17.P.10).
- Cero violaciones de RC-03, RC-04 y RC-05 bajo concurrencia, verificado por pruebas de concurrencia en 08.
- El esquema se reconstruye desde cero aplicando las migraciones en orden, verificado en el pipeline contra una base efímera.

## 9. Referencias

- NB-01 a NB-06; CU-01 a CU-17; RC-02, RC-03, RC-04, RC-05; RN-05.
- Intake §17.P.4 (persistencia, multi_tenant=false), §17.P.10 (NFR), §17.P.11.
- ADRs relacionadas: ADR-01 (estilo), ADR-06 (tolerancia a conflictos), ADR-08 (idempotencia).
- `modelo-datos-logico_v1.0.md`; `arquitectura-solucion_v1.0.md` §6.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de persistencia: almacén relacional con migraciones versionadas, restricciones declarativas y transacciones atómicas; single-tenant. Aceptada (pre-tomada en intake §17.P.4, §17.P.11). |
