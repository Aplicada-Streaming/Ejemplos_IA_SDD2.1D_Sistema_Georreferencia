# Decisiones de arquitectura — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** decisiones-arquitectura_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer

## 1. Objetivo

Índice navegable de los Architecture Decision Records (ADR) del motor de sincronización `aplicada-sync`. Cada ADR vive en su propio archivo bajo `adrs/` como decisión individual e inmutable; este documento no contiene el cuerpo de las decisiones, solo su registro y estado vigente. Si una decisión evoluciona, se crea una ADR nueva con identificador siguiente y la anterior pasa a estado `Superado por ADR-YY` sin reescribirse.

## 2. Índice de ADRs

| ADR | Título | Categoría | Estado | Fecha | Enlace |
| --- | --- | --- | --- | --- | --- |
| ADR-01 | estilo-clean-architecture-abstractions | Estilo | Aceptado | 2026-06-15 | [archivo](adrs/ADR-01-estilo-clean-architecture-abstractions_v1.0.md) |
| ADR-02 | inversion-dependencias-adaptadores-host | Extensibilidad | Aceptado | 2026-06-15 | [archivo](adrs/ADR-02-inversion-dependencias-adaptadores-host_v1.0.md) |
| ADR-03 | versionado-superficie-publica | Despliegue | Aceptado | 2026-06-15 | [archivo](adrs/ADR-03-versionado-superficie-publica_v1.0.md) |
| ADR-04 | cola-local-persistente-ordenada | Persistencia | Aceptado | 2026-06-15 | [archivo](adrs/ADR-04-cola-local-persistente-ordenada_v1.0.md) |
| ADR-05 | orden-subir-antes-de-bajar | Estilo | Aceptado | 2026-06-15 | [archivo](adrs/ADR-05-orden-subir-antes-de-bajar_v1.0.md) |
| ADR-06 | reanudacion-por-marca-de-progreso | Persistencia | Aceptado | 2026-06-15 | [archivo](adrs/ADR-06-reanudacion-por-marca-de-progreso_v1.0.md) |
| ADR-07 | idempotencia-por-identificador-estable | Persistencia | Aceptado | 2026-06-15 | [archivo](adrs/ADR-07-idempotencia-por-identificador-estable_v1.0.md) |
| ADR-08 | convivencia-con-conflictos | Estilo | Aceptado | 2026-06-15 | [archivo](adrs/ADR-08-convivencia-con-conflictos_v1.0.md) |

## 3. Cobertura mínima por tipo

El tipo `library` exige un mínimo de tres ADRs: estilo, superficie pública y estrategia de versionado (regla §2.2). El piso se cubre y se supera:

- Estilo: ADR-01 (Clean Architecture con capa Abstractions) y ADR-05 (pipeline subir-antes-de-bajar).
- Superficie pública / contrato de sincronización: ADR-02 (inversión de dependencias y contratos de extensión) y ADR-08 (convivencia con conflictos como parte del contrato).
- Estrategia de versionado: ADR-03 (versionado semántico de la superficie pública).
- Decisiones adicionales que la trazabilidad requiere registrar: ADR-04 (cola local), ADR-06 (reanudación) y ADR-07 (idempotencia).

## 4. Cobertura de motivación por CU/RN/NFR

| ADR | Motivada por |
| --- | --- |
| ADR-01 | NB-04; CU-01 a CU-06; intake §17 P.2/P.11 |
| ADR-02 | CU-01, CU-04; intake §17 P.2 |
| ADR-03 | Especificación funcional §8; secciones §17 de CU-01 a CU-06; intake §17 P.7/P.8 |
| ADR-04 | CU-02, CU-05, CU-06; RN-02; NFR cola >= 1000 (intake §17 P.10) |
| ADR-05 | RN-01; CU-03, CU-04, CU-06; NFR lote 100 <= 30 s (intake §17 P.10) |
| ADR-06 | CU-06; RN-01, RN-02; NFR reanudación sin pérdida (intake §17 P.10); intake §7 |
| ADR-07 | RN-02; CU-02, CU-03, CU-06; NFR idempotencia (intake §17 P.10) |
| ADR-08 | RN-03; CU-03, CU-05; NFR continuidad ante conflicto (intake §17 P.10) |

## 5. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice inicial de los 8 ADRs aceptados del motor aplicada-sync, con cobertura del mínimo del tipo library y trazabilidad de motivación. |
