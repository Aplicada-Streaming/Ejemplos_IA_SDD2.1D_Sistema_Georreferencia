# ADR-01 — Estilo de composición: backend monolítico, clientes y librerías

**Proyecto:** GeoVial (solución)
**Documento:** ADR-01-estilo-composicion-backend-clientes-librerias_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Soluciones Senior
**Categoría:** Estilo

## 1. Contexto

GeoVial es una solución de cinco proyectos autónomos que deben construirse, versionarse y desplegarse por separado, pero que componen un único sistema de relevamiento georreferenciado. El manifiesto (`SOLUTION-MANIFEST-geovial_v1.0.md` §2/§3) fija cinco proyectos con tipos D8 distintos: un backend `rest-api` (`geovial-api`, principal), dos clientes (`geovial-web` web-monolith y `geovial-mobile` mobile-app-maui) y dos librerías (`geovial-storage` library y `aplicada-sync` library redistribuible). El intake §14 fija explícitamente la forma de la composición: un backend monolítico que concentra lógica, persistencia y seguridad, dos clientes separados que lo consumen por contrato REST, una librería de almacenamiento transparente integrada al backend y una librería de sincronización redistribuible consumida por la app móvil.

La decisión de composición debe situarse por encima de cada arquitectura de proyecto: define cómo se reparten las responsabilidades entre proyectos y qué frontera cruza cada contrato, sin reescribir el estilo interno de cada uno (que vive en su propia `arquitectura-solucion`). Motiva esta decisión la necesidad de un sistema operable por un equipo de un desarrollador (intake §2, equipo_n=1) con valor demostrable end-to-end por incrementos verticales (intake §15), y la exigencia de que la librería de sincronización sea reutilizable fuera de la solución (intake §13, `redistribuible: true`).

## 2. Decisión

La solución se compone como un backend monolítico autoritativo (`geovial-api`) que es el único dueño del dominio, la persistencia y la seguridad; dos clientes sin dominio propio (`geovial-web`, `geovial-mobile`) que consumen su contrato REST; y dos librerías de soporte que no son servicios de red: `geovial-storage`, integrada en proceso al backend tras una abstracción de almacenamiento, y `aplicada-sync`, paquete redistribuible que la app móvil integra en proceso para sincronizar. Ningún cliente accede al almacén ni al almacenamiento de archivos directamente; toda mutación de dominio pasa por el backend dentro de su transacción.

## 3. Estado

Aceptado el 2026-06-15. Es la decisión de composición de nivel solución; gobierna las cuatro aristas del grafo del manifiesto y se referencia desde `vista-solucion_v1.0.md` §5.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Backend monolítico + clientes + librerías (elegida) | Un solo dominio autoritativo y una sola transacción; bajo costo operativo para un equipo de un desarrollador; clientes y librerías versionables por separado; reúso de la librería de sincronización | Acopla los clientes a la disponibilidad del backend; el backend concentra el riesgo de cambio del contrato |
| Backend partido en microservicios por contexto | Despliegue independiente por servicio; aislamiento de fallas | Introduce transacciones distribuidas y consistencia eventual donde el dominio pide consistencia inmediata (jerarquía, transición de estado, unicidad de asignación); sobredimensionado para un dominio único operado por una persona; contradice el requisito de monolito (intake §14) |
| Fusionar el front en el proceso del backend | Menos unidades desplegables | Rompe la frontera cliente-servidor del manifiesto y el versionado independiente del contrato; mezcla dos tipos D8 y dos ciclos de vida; impide el cliente móvil offline-first |
| Sincronización ad-hoc embebida en la app móvil (sin librería redistribuible) | Menos un proyecto que mantener | Contradice el requisito de reutilización fuera de la solución; reimplementaría orden, idempotencia y reanudación en cada app, multiplicando defectos |

## 5. Consecuencias positivas

1. El backend es la única fuente de verdad: la consistencia inmediata del dominio (jerarquía de usuarios, ciclo del relevamiento, unicidad de asignación) se garantiza en una sola transacción local, sin coordinación distribuida.
2. Los clientes y las librerías se construyen, prueban y despliegan por separado, respetando el orden topológico del manifiesto y habilitando los incrementos verticales del intake §15.
3. La librería de sincronización queda agnóstica del dominio y reutilizable fuera de la solución, cumpliendo el requisito de redistribuible.
4. La frontera entre cada par de proyectos queda mediada por un contrato explícito y versionado, lo que acota el impacto de los cambios.

## 6. Consecuencias negativas y trade-offs

1. Los clientes dependen de la disponibilidad y de la versión del contrato del backend; un cambio incompatible del contrato REST rompe a ambos clientes si no se versiona (mitigado por ADR-02 y por el versionado por URI del productor).
2. El backend concentra la carga y el riesgo de un único proceso; se acepta a cambio de simplicidad operativa (trade-off heredado del intake §17.P.12 de `geovial-api`).
3. Las dos librerías se integran en proceso, no como servicios; un defecto de una librería se manifiesta dentro de su host (el backend o la app), no de forma aislada. Se acepta a cambio de menor complejidad operativa y menor latencia.

## 7. Implementación

La composición se materializa en cuatro aristas del manifiesto, cada una con un contrato detallado en `contratos-inter-proyecto_v1.0.md`:

- `geovial-api → geovial-storage`: el backend integra la librería de almacenamiento en proceso y consume su abstracción (contrato de Abstractions del productor).
- `geovial-web → geovial-api` y `geovial-mobile → geovial-api`: los clientes consumen el contrato REST autenticado del backend.
- `geovial-mobile → aplicada-sync`: la app integra el paquete redistribuible y consume su contrato de sincronización.

Cada proyecto conserva su estilo interno (Clean Architecture en capas en el backend, render server-side en el front, MVVM offline-first en el móvil, Clean Architecture con capa Abstractions en ambas librerías), documentado en su propia `arquitectura-solucion`. Esta decisión no lo reescribe: lo referencia y fija la frontera entre proyectos.

## 8. Métricas de validación

- Cero accesos directos de un cliente al almacén relacional o al almacenamiento de archivos: todo cambio de dominio cruza el contrato REST (verificable por revisión de dependencias del manifiesto y pruebas de integración en 08).
- El orden topológico de construcción del manifiesto se respeta en el pipeline: las librerías de nivel 0 se construyen antes que el backend de nivel 1 y este antes que los clientes de nivel 2.
- La librería de sincronización se restaura y ejercita en un proyecto limpio ajeno a GeoVial (la demostración de evaluación del intake §16.1), demostrando su agnosticismo del dominio.

## 9. Referencias

- Manifiesto: `SOLUTION-MANIFEST-geovial_v1.0.md` §2 (tabla de proyectos), §3 (grafo de dependencias).
- Intake: `SOLUTION-INTAKE-geovial_v1.0.md` §13 (proyectos y aristas), §14 (estilo arquitectónico de la solución), §15 (descomposición y delivery).
- ADRs de nivel solución relacionados: ADR-02 (versionado inter-proyecto), ADR-03 (comunicación entre proyectos).
- Arquitecturas de proyecto referenciadas: `proyectos/geovial-api/05_arquitectura_tecnica/arquitectura-solucion_v1.0.md` §2 y las de los otros cuatro proyectos.
- Vista de solución: `vista-solucion_v1.0.md` §2, §3, §5.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión inicial de estilo de composición de la solución GeoVial: backend monolítico autoritativo, dos clientes sin dominio propio y dos librerías de soporte (una integrada al backend, una redistribuible). Para ADR aceptadas, la única edición permitida es el cambio de estado a Superado por ADR-YY. |
