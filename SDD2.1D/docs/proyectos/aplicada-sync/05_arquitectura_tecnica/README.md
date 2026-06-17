# 05 Arquitectura técnica — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** README.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer

Punto de entrada navegable de la arquitectura técnica del motor de sincronización `aplicada-sync` (tipo `library`, redistribuible). El motor propaga los cambios locales de una aplicación host hacia un backend remoto bajo la política subir-luego-bajar, de forma agnóstica del dominio y reutilizable fuera de la solución GeoVial.

## Documento maestro

- [arquitectura-solucion_v1.0.md](arquitectura-solucion_v1.0.md) — estilo, cuatro vistas (lógica, procesos, despliegue, datos), cross-cutting, atributos de calidad numéricos, riesgos y trazabilidad.

## Decisiones de arquitectura (ADRs)

- [decisiones-arquitectura_v1.0.md](decisiones-arquitectura_v1.0.md) — índice navegable de los ADRs.

| ADR | Título | Categoría | Estado |
| --- | --- | --- | --- |
| [ADR-01](adrs/ADR-01-estilo-clean-architecture-abstractions_v1.0.md) | Estilo Clean Architecture con capa Abstractions | Estilo | Aceptado |
| [ADR-02](adrs/ADR-02-inversion-dependencias-adaptadores-host_v1.0.md) | Inversión de dependencias hacia adaptadores del host | Extensibilidad | Aceptado |
| [ADR-03](adrs/ADR-03-versionado-superficie-publica_v1.0.md) | Versionado de la superficie pública | Despliegue | Aceptado |
| [ADR-04](adrs/ADR-04-cola-local-persistente-ordenada_v1.0.md) | Cola local persistente y ordenada | Persistencia | Aceptado |
| [ADR-05](adrs/ADR-05-orden-subir-antes-de-bajar_v1.0.md) | Pipeline de orden estricto subir-antes-de-bajar | Estilo | Aceptado |
| [ADR-06](adrs/ADR-06-reanudacion-por-marca-de-progreso_v1.0.md) | Reanudación por marca de progreso | Persistencia | Aceptado |
| [ADR-07](adrs/ADR-07-idempotencia-por-identificador-estable_v1.0.md) | Idempotencia por identificador estable | Persistencia | Aceptado |
| [ADR-08](adrs/ADR-08-convivencia-con-conflictos_v1.0.md) | Convivencia con estados en conflicto | Estilo | Aceptado |

## Contratos

- [contratos-abstractions_v1.0.md](contratos-abstractions_v1.0.md) — superficie pública (capa Abstractions): operaciones del ciclo de vida (CU-01 a CU-06), formas de datos, errores y política de versionado.

## Motor de procesamiento

- [flujo-ejecucion_v1.0.md](flujo-ejecucion_v1.0.md) — pipeline subir-luego-bajar paso a paso: ciclo manual, disparo por conectividad y reanudación.

## Puntos de extensión

- [extensibilidad_v1.0.md](extensibilidad_v1.0.md) — estrategias de almacén local, transporte, credencial y conectividad; contrato de extensión y referencia al ejemplo de 11.

## Modelo de datos lógico

No aplica. El tipo `library` sin modelo de datos de dominio propio omite `modelo-datos-logico` (regla §2.2). El motor administra metadatos de sincronización cuya forma lógica se describe en `arquitectura-solucion_v1.0.md` §6.

## Atributos de calidad (NFR del intake §17 P.10)

| NFR | Objetivo |
| --- | --- |
| Tiempo de sincronización de lote | Lote de 100 cambios en <= 30 s en red móvil típica |
| Capacidad de cola local | >= 1000 cambios pendientes sin degradación |
| Reanudación sin pérdida | 0 perdidos y 0 duplicados tras un corte en la subida |
| Idempotencia ante reintento | 100 % de efecto neto único |
| Orden subir-antes-de-bajar | 0 bajadas mientras quedan pendientes confirmables |
| Continuidad ante conflicto | 0 ciclos abortados por un conflicto reportado |

## Trazabilidad upstream/downstream

- Upstream: NB-04 (00/01), especificación funcional de 02 (CU-01 a CU-06, RN-01 a RN-03), marco DX de 03 y SOLUTION-INTAKE §17 (aplicada-sync).
- Downstream: 06 (US-01 a US-13 del proyecto), 08 (suites de prueba referenciadas en cada artefacto).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 05 de aplicada-sync con el índice de artefactos, ADRs vigentes y NFR. |
