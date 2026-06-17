# Decisiones de arquitectura — geovial-api

**Proyecto:** geovial-api
**Documento:** decisiones-arquitectura_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer

## 1. Objetivo

Índice navegable de los Architecture Decision Records (ADR) de `geovial-api`. Cada ADR vive en un archivo individual bajo `adrs/` (regla 05 §3.3); este documento solo lista identificador, título, categoría, estado y fecha, sin reproducir el cuerpo de las decisiones. Una ADR aceptada es inmutable: si una decisión cambia, se crea una ADR nueva con identificador siguiente y la anterior pasa a `Superado por ADR-YY` sin reescribirse (regla 05 §3.3, §3.5).

## 2. Índice de ADRs

| ADR | Título | Categoría | Estado | Fecha |
| --- | --- | --- | --- | --- |
| [ADR-01](adrs/ADR-01-estilo-clean-architecture-capas_v1.0.md) | Estilo: Clean Architecture en capas para el backend monolítico | Estilo | Aceptado | 2026-06-15 |
| [ADR-02](adrs/ADR-02-persistencia-almacen-relacional-migraciones_v1.0.md) | Persistencia en almacén relacional con migraciones versionadas | Persistencia | Aceptado | 2026-06-15 |
| [ADR-03](adrs/ADR-03-autenticacion-token-bearer-rol-jerarquico_v1.0.md) | Autenticación por token bearer y autorización por rol jerárquico | Seguridad | Aceptado | 2026-06-15 |
| [ADR-04](adrs/ADR-04-paginacion-filtros-listados_v1.0.md) | Estrategia de paginación y filtros de listados | Comunicación | Aceptado | 2026-06-15 |
| [ADR-05](adrs/ADR-05-manejo-errores-problem-json_v1.0.md) | Manejo de errores con problem+json RFC 7807 | Comunicación | Aceptado | 2026-06-15 |
| [ADR-06](adrs/ADR-06-tolerancia-conflictos-resolucion-cierre_v1.0.md) | Tolerancia a conflictos de marcadores y resolución al cierre | Estilo | Aceptado | 2026-06-15 |
| [ADR-07](adrs/ADR-07-orden-sincronizacion-subir-antes-de-bajar_v1.0.md) | Orden de sincronización subir antes de bajar | Comunicación | Aceptado | 2026-06-15 |
| [ADR-08](adrs/ADR-08-idempotencia-operaciones-no-seguras_v1.0.md) | Idempotencia de operaciones no seguras y de la sincronización | Comunicación | Aceptado | 2026-06-15 |
| [ADR-09](adrs/ADR-09-integracion-abstraccion-almacenamiento_v1.0.md) | Integración con la abstracción de almacenamiento de archivos | Persistencia | Aceptado | 2026-06-15 |
| [ADR-10](adrs/ADR-10-versionado-contrato-por-uri_v1.0.md) | Versionado del contrato público por URI | Comunicación | Aceptado | 2026-06-15 |

## 3. Cobertura del mínimo por tipo

El tipo `rest-api` exige al menos cinco ADRs: estilo, persistencia, autenticación, paginación y manejo de errores (regla 05 §2.2). Se registran diez: las cinco obligatorias (ADR-01 estilo, ADR-02 persistencia, ADR-03 autenticación, ADR-04 paginación, ADR-05 manejo de errores) más cinco adicionales motivadas por los invariantes de dominio y la naturaleza del backend de sincronización: ADR-06 (tolerancia a conflictos), ADR-07 (orden de sincronización), ADR-08 (idempotencia), ADR-09 (integración con la abstracción de almacenamiento) y ADR-10 (versionado del contrato). Todas en estado `Aceptado` por estar pre-decididas en el intake (§17.P.11) y derivadas de las reglas de negocio de 02.

## 4. Trazabilidad upstream de cada ADR

| ADR | NB | CU | RN / RC | NFR / Intake |
| --- | --- | --- | --- | --- |
| ADR-01 | NB-01 a NB-07 | CU-01 a CU-22 | RN-01 a RN-07 | §17.P.2, §17.P.11, §17.P.12 |
| ADR-02 | NB-01 a NB-06 | CU-01 a CU-17 | RC-02, RC-03, RC-04, RC-05 | §17.P.4, §17.P.10 |
| ADR-03 | NB-01 | CU-03, CU-18 | RN-01, RN-02; RC-03 | §17.P.5, §17.P.3 |
| ADR-04 | NB-02, NB-03, NB-05 | CU-20 | RN-01 | §17.P.10 |
| ADR-05 | NB-01 a NB-05 | CU-19 | (uniformiza las RN) | §17.P.3; 02 §2.2 |
| ADR-06 | NB-04, NB-05 | CU-07, CU-08, CU-10, CU-11, CU-12, CU-13, CU-14 | RN-03, RN-05; RC-01, RC-04 | §17.P.2, §17.P.4, §17.P.11 |
| ADR-07 | NB-04 | CU-10, CU-11 | RN-06; RC-06 | §17.P.3, §17.P.10 |
| ADR-08 | NB-04, NB-01, NB-02, NB-06 | CU-10, CU-21 | RN-07; RC-05, RC-06 | §17.P.10, §17.P.11 |
| ADR-09 | NB-07, NB-03, NB-06 | CU-08, CU-09, CU-15, CU-16, CU-17 | RN-04 | §14, §17.P.1, §17.P.11 |
| ADR-10 | NB-01 a NB-05 | CU-22 | (política de compatibilidad) | §17.P.3, §17.P.7 |

## 5. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice inicial de ADRs de geovial-api: diez ADRs individuales (cinco del mínimo de `rest-api` más tolerancia a conflictos, orden de sincronización, idempotencia, integración con almacenamiento y versionado del contrato), todas aceptadas, con su trazabilidad upstream. |
