# Decisiones de arquitectura — geovial-storage

**Proyecto:** geovial-storage
**Documento:** decisiones-arquitectura_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer

## 1. Objetivo

Índice navegable de los Architecture Decision Records (ADR) de `geovial-storage`. Cada ADR vive en un archivo individual bajo `adrs/` (regla 05 §3.3); este documento solo lista identificador, título, categoría, estado y fecha, sin reproducir el cuerpo de las decisiones. Una ADR aceptada es inmutable: si una decisión cambia, se crea una ADR nueva y la anterior pasa a `Superado por ADR-YY` (regla 05 §3.5).

## 2. Índice de ADRs

| ADR | Título | Categoría | Estado | Fecha |
| --- | --- | --- | --- | --- |
| [ADR-01](adrs/ADR-01-abstraccion-proveedores-intercambiables_v1.0.md) | Abstracción de almacenamiento con proveedores intercambiables por estrategia | Estilo | Aceptado | 2026-06-15 |
| [ADR-02](adrs/ADR-02-superficie-publica-estable_v1.0.md) | Superficie pública estable: una interfaz de almacenamiento única | Estilo | Aceptado | 2026-06-15 |
| [ADR-03](adrs/ADR-03-estrategia-versionado-contrato_v1.0.md) | Estrategia de versionado del contrato público | Estilo | Aceptado | 2026-06-15 |
| [ADR-04](adrs/ADR-04-transparencia-limites-proveedor_v1.0.md) | Transparencia del proveedor e integridad del contenido | Estilo | Aceptado | 2026-06-15 |
| [ADR-05](adrs/ADR-05-manejo-seguro-credenciales_v1.0.md) | Manejo seguro de las credenciales del proveedor | Seguridad | Aceptado | 2026-06-15 |

## 3. Cobertura del mínimo por tipo

El tipo `library` exige al menos tres ADRs (estilo, superficie pública, estrategia de versionado; regla 05 §2.2). Se registran cinco: las tres obligatorias (ADR-01 estilo, ADR-02 superficie pública, ADR-03 versionado) más dos adicionales motivadas por los invariantes de dominio del proyecto (ADR-04 transparencia e integridad, ADR-05 manejo seguro de credenciales). Todas en estado `Aceptado` por estar pre-decididas en el intake (§17.P.11) y en las reglas de negocio de 02.

## 4. Trazabilidad upstream de cada ADR

| ADR | NB | CU | RN | NFR / Intake |
| --- | --- | --- | --- | --- |
| ADR-01 | NB-07 | CU-01 a CU-06 | RN-01 | §17.P.2, §17.P.11, §17.P.12 |
| ADR-02 | NB-07 | CU-01 a CU-06 | RN-01 | §17.P.11; 02 §6 |
| ADR-03 | NB-07 | CU-01 | RN-01 | §17.P.7; 02 §6 |
| ADR-04 | NB-07 | CU-01, CU-02, CU-05 | RN-01, RN-02 | §17.P.10 |
| ADR-05 | NB-07 | CU-02, CU-05, CU-06 | RN-03 | §17.P.5 |

## 5. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice inicial de ADRs de geovial-storage: cinco ADRs individuales (tres del mínimo de `library` más transparencia/integridad y manejo seguro de credenciales), todas aceptadas, con su trazabilidad upstream. |
