# 05 Arquitectura técnica — geovial-storage

**Proyecto:** geovial-storage
**Tipo (D8):** library
**Variante:** Arquitecto de Software + API Designer
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer

Punto de entrada navegable de la sección 05 de `geovial-storage`, la librería que expone al backend de GeoVial una abstracción de alojamiento de archivos transparente con proveedores intercambiables (local / remoto / otro) seleccionables por el usuario raíz. El estilo es hexagonal: una capa de Abstracciones estable con proveedores conectados por estrategia (ADR-01). Por ser `library` (regla 05 §2.2) no se produce `modelo-datos-logico`.

## Documento maestro

- [arquitectura-solucion_v1.0.md](arquitectura-solucion_v1.0.md) — estilo arquitectónico, cuatro vistas mínimas (lógica, procesos, despliegue, datos), cross-cutting concerns, NFR con objetivos numéricos y mecanismo de medición, riesgos y trazabilidad.

## Decisiones de arquitectura (ADRs)

Índice navegable en [decisiones-arquitectura_v1.0.md](decisiones-arquitectura_v1.0.md). ADRs vigentes (cada una en archivo individual bajo `adrs/`):

| ADR | Título | Categoría | Estado |
| --- | --- | --- | --- |
| [ADR-01](adrs/ADR-01-abstraccion-proveedores-intercambiables_v1.0.md) | Abstracción de almacenamiento con proveedores intercambiables por estrategia | Estilo | Aceptado |
| [ADR-02](adrs/ADR-02-superficie-publica-estable_v1.0.md) | Superficie pública estable: una interfaz de almacenamiento única | Estilo | Aceptado |
| [ADR-03](adrs/ADR-03-estrategia-versionado-contrato_v1.0.md) | Estrategia de versionado del contrato público | Estilo | Aceptado |
| [ADR-04](adrs/ADR-04-transparencia-limites-proveedor_v1.0.md) | Transparencia del proveedor e integridad del contenido | Estilo | Aceptado |
| [ADR-05](adrs/ADR-05-manejo-seguro-credenciales_v1.0.md) | Manejo seguro de las credenciales del proveedor | Seguridad | Aceptado |

Las tres primeras cubren el mínimo de `library` (estilo, superficie pública, versionado); ADR-04 y ADR-05 cubren los invariantes de dominio (transparencia/integridad y credenciales).

## Contratos

- [contratos-abstractions_v1.0.md](contratos-abstractions_v1.0.md) — superficie pública de Abstractions expuesta a `geovial-api`: dos interfaces (almacenamiento y configuración del proveedor), seis operaciones (CU-01 a CU-06), esquemas de datos lógicos, taxonomía de errores uniforme y política de versionado.

## Extensibilidad

- [extensibilidad_v1.0.md](extensibilidad_v1.0.md) — punto de extensión (el puerto de proveedor de almacenamiento), contrato del proveedor, pasos de registro de un proveedor nuevo y referencia al ejemplo en 11.

## Flujo de ejecución

- [flujo-ejecucion_v1.0.md](flujo-ejecucion_v1.0.md) — incluido (opcional para `library`). Documenta el pipeline de enrutado validar → resolver proveedor activo → delegar → normalizar para guardar/recuperar/eliminar y la transición de cambio de proveedor. Se consideró útil porque ese enrutado es el mecanismo que materializa la transparencia (RN-01) y la normalización de errores (ADR-04), común a todas las operaciones.

## NFR vigentes (resumen)

| NFR | Objetivo numérico | ADR |
| --- | --- | --- |
| Latencia p95 (proveedor local) | ≤ 1 s para archivos de hasta 5 MB | ADR-01, ADR-04 |
| Tamaño máximo de archivo | Configurable; por defecto 25 MB | ADR-04 |
| Transparencia entre proveedores | 0 diferencias observables; 0 ramas por proveedor | ADR-01, ADR-04 |
| Integridad del contenido | 100 % igualdad binaria | ADR-03 (RN-02) |
| No filtración de credenciales | 0 ocurrencias en resultados, errores y registros | ADR-05 |
| Cobertura (gate de CI) | Líneas ≥ 80 %; branches ≥ 70 % | ADR-01 |

Detalle y mecanismo de medición en `arquitectura-solucion_v1.0.md` §8.

## Artefactos omitidos (declarados)

- `modelo-datos-logico_v1.0.md`: no se genera. La regla 05 §2.2 lo excluye para `library`; la persistencia de blobs se delega al proveedor activo y se documenta en la vista de datos del documento maestro (§6).

## Trazabilidad

- Upstream: NB-07 (principal), NB-03 y NB-06 (soporte); CU-01 a CU-06 y RN-01, RN-02, RN-03 de 02; visión de producto y restricciones de 00; intake §17 (P.2, P.5, P.6, P.7, P.8, P.9, P.10, P.11) y §14 (contratos inter-proyecto).
- Downstream: 06 (US-01 a US-09 y backlog técnico), 08 (batería de contrato por proveedor, igualdad binaria, no filtración de credenciales), 09 (despliegue de la librería embebida y mecanismo de almacenamiento seguro), 11 (ejemplo de extensión y consumidores progresivos).
- Inter-proyecto: `geovial-storage → geovial-api` (el productor expone su contrato de Abstractions al consumidor); se indexa en la vista de solución `_solucion/`.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 05 de geovial-storage: índice del documento maestro, cinco ADRs aceptadas, contrato de Abstractions, extensibilidad, flujo de ejecución (incluido), resumen de NFR, omisión declarada de modelo-datos-logico y trazabilidad. |
