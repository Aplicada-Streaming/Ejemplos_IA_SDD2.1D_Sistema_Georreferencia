# 05 — Arquitectura técnica — geovial-api

**Proyecto:** geovial-api
**Tipo D8:** rest-api (proyecto principal de la solución GeoVial)
**Estado:** Propuesto (ADRs Aceptados)
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer

Punto de entrada navegable de la arquitectura técnica de `geovial-api`, el backend monolítico que expone la API REST consumida por `geovial-web` y `geovial-mobile`, concentra la lógica de negocio, la persistencia en un almacén relacional y la seguridad por token bearer, e integra la librería `geovial-storage` para las fotos.

## Documento maestro

- [`arquitectura-solucion_v1.0.md`](arquitectura-solucion_v1.0.md) — Estilo (Clean Architecture en capas), cuatro vistas mínimas (lógica, procesos, despliegue, datos), cross-cutting, NFR con métricas numéricas, riesgos y trazabilidad.

## Decisiones de arquitectura (ADRs)

- [`decisiones-arquitectura_v1.0.md`](decisiones-arquitectura_v1.0.md) — Índice navegable de los ADRs.

| ADR | Título | Categoría | Estado |
| --- | --- | --- | --- |
| [ADR-01](adrs/ADR-01-estilo-clean-architecture-capas_v1.0.md) | Estilo: Clean Architecture en capas | Estilo | Aceptado |
| [ADR-02](adrs/ADR-02-persistencia-almacen-relacional-migraciones_v1.0.md) | Persistencia en almacén relacional con migraciones versionadas | Persistencia | Aceptado |
| [ADR-03](adrs/ADR-03-autenticacion-token-bearer-rol-jerarquico_v1.0.md) | Autenticación por token bearer y autorización por rol jerárquico | Seguridad | Aceptado |
| [ADR-04](adrs/ADR-04-paginacion-filtros-listados_v1.0.md) | Estrategia de paginación y filtros de listados | Comunicación | Aceptado |
| [ADR-05](adrs/ADR-05-manejo-errores-problem-json_v1.0.md) | Manejo de errores con problem+json RFC 7807 | Comunicación | Aceptado |
| [ADR-06](adrs/ADR-06-tolerancia-conflictos-resolucion-cierre_v1.0.md) | Tolerancia a conflictos de marcadores y resolución al cierre | Estilo | Aceptado |
| [ADR-07](adrs/ADR-07-orden-sincronizacion-subir-antes-de-bajar_v1.0.md) | Orden de sincronización subir antes de bajar | Comunicación | Aceptado |
| [ADR-08](adrs/ADR-08-idempotencia-operaciones-no-seguras_v1.0.md) | Idempotencia de operaciones no seguras y de la sincronización | Comunicación | Aceptado |
| [ADR-09](adrs/ADR-09-integracion-abstraccion-almacenamiento_v1.0.md) | Integración con la abstracción de almacenamiento | Persistencia | Aceptado |
| [ADR-10](adrs/ADR-10-versionado-contrato-por-uri_v1.0.md) | Versionado del contrato público por URI | Comunicación | Aceptado |

El mínimo de `rest-api` (5 ADRs: estilo, persistencia, autenticación, paginación, errores) se cumple con ADR-01 a ADR-05; ADR-06 a ADR-10 cubren los invariantes de dominio y la naturaleza del backend de sincronización.

## Modelo de datos lógico

- [`modelo-datos-logico_v1.0.md`](modelo-datos-logico_v1.0.md) — Mapeo de las 12 entidades conceptuales de 02 a 16 tablas de dominio más una tabla técnica de idempotencia, con tipos físicos, índices, restricciones, migración inicial (`M0001_inicial`) y trazabilidad entidad por entidad. Single-tenant (multi_tenant=false).

## Contratos externos

- [`contratos-rest_v1.0.md`](contratos-rest_v1.0.md) — Contrato OpenAPI descriptivo de la API: recursos y operaciones por área para los 22 CU, esquemas (DTO), errores problem+json RFC 7807, versionado por URI y los endpoints de sincronización subida/bajada.

## Flujo de ejecución

- [`flujo-ejecucion_v1.0.md`](flujo-ejecucion_v1.0.md) — Pipeline de sincronización subir-luego-bajar paso a paso, con tolerancia a conflictos, orden e idempotencia.

## NFR vigentes (intake §17.P.10)

| NFR | Objetivo |
| --- | --- |
| Latencia p95 de lecturas | ≤ 300 ms |
| Latencia p95 de escrituras | ≤ 500 ms |
| Disponibilidad mensual | ≥ 99,5 % |
| Capacidad del lote de sincronización | ≥ 1000 cambios por relevamiento |
| Cobertura de pruebas (gate de CI) | Líneas ≥ 80 %, branches ≥ 70 %, 100 % de endpoints con contract test |

La observabilidad no es crítica en esta versión (tiene_observabilidad_critica=false): sin SLO de 99,9 % ni objetivo de latencia p99 numérico.

## Notas de alcance

- No se produce `extensibilidad_v1.0.md` (tiene_extensibilidad=false).
- La vista de solución y los contratos inter-proyecto viven en `_solucion/` (Fase H) y aquí solo se referencian. Contratos del proyecto: consume `geovial-storage` (`contratos-abstractions_v1.0.md`, vía ADR-09) y expone su contrato REST a `geovial-web` y `geovial-mobile`.

## Downstream

Esta arquitectura ancla 06 (US y backlog técnico), 08 (testing técnico y de integración del ciclo de sincronización) y 09 (DevOps, despliegue del contenedor de backend, base y migraciones).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 05 de geovial-api: documento maestro, diez ADRs, modelo lógico, contrato REST, flujo de sincronización y NFR vigentes. |
