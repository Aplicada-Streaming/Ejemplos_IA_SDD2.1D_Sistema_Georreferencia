# 06 Backlog técnico — geovial-storage

**Proyecto:** geovial-storage
**Tipo (D8):** library
**Variante:** Scrum Master + Backlog Curator
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + Backlog Curator
**Técnica de estimación:** Fibonacci (1, 2, 3, 5, 8, 13)

Punto de entrada navegable de la sección 06 de `geovial-storage`, la librería que expone al backend de GeoVial una abstracción de alojamiento de archivos transparente con proveedores intercambiables (local / remoto / otro) seleccionables por el usuario raíz. Las épicas se arman por superficie de Abstractions y por capacidad del motor interno, según la variante `library` (regla 06 §1.2). Las nueve US y las trece BT viven inline (proyecto por debajo de los umbrales de 20 US y 30 BT, regla 06 §3.3).

## Documentos de la sección

- [product-backlog_v1.0.md](product-backlog_v1.0.md) — objetivos del producto, épicas EP-XX, historias por épica (US-01 a US-09) con MoSCoW, story points e INVEST, métricas de avance y política de refinement.
- [backlog-tecnico_v1.0.md](backlog-tecnico_v1.0.md) — épicas técnicas, BT-01 a BT-13 con tipo, prioridad, estimación, fuente upstream, dependencias y criterios, y matriz cruzada BT↔US↔CU.
- [definition-of-ready_v1.0.md](definition-of-ready_v1.0.md) — DoR para US (7 criterios) y BT (5 criterios), excepciones admitidas y aprobador.

## Épicas vigentes

| EP | Nombre | Foco | Sprints |
| --- | --- | --- | --- |
| EP-01 | Superficie de almacenamiento (operaciones de datos) | Guardar, recuperar, eliminar, verificar, listar (CU-01 a CU-05) | 2 |
| EP-02 | Configuración del proveedor activo | Selección y validación del proveedor por el usuario raíz (CU-06) | 1 |
| EP-03 | Proveedores intercambiables y punto de extensión | Adaptadores local y de objetos remoto, puerto de extensión | 2 |
| EP-04 | Fundaciones del contrato y verificación de transparencia | Abstracciones, núcleo, errores, batería de contrato, cobertura | 2 |

## US Must Have del MVP

| US | Título | SP | CU | Épica |
| --- | --- | --- | --- | --- |
| US-01 | Guardar un archivo y obtener su identificador lógico | 5 | CU-01 | EP-01 |
| US-03 | Recuperar un archivo idéntico al guardado | 5 | CU-02 | EP-01 |
| US-05 | Eliminar un archivo de forma idempotente y por prefijo | 3 | CU-03 | EP-01 |
| US-06 | Verificar la existencia de un archivo sin transferir contenido | 2 | CU-04 | EP-01 |
| US-08 | Activar un proveedor de almacenamiento como usuario raíz | 8 | CU-06 | EP-02 |

El MVP suma 23 SP en cinco US Must (56 % de las historias). Las Should (US-02, US-04, US-07) y la Could (US-09) completan el contrato sin bloquear el MVP. Distribución MoSCoW: 5 Must / 3 Should / 1 Could; no todo es Must.

## BT prioritarias

| BT | Título | Tipo | Prioridad | SP |
| --- | --- | --- | --- | --- |
| BT-11 | Declarar la capa de Abstracciones y el puerto de proveedor | feature | Must | 5 |
| BT-05 | Definir el catálogo de errores uniforme | feature | Must | 3 |
| BT-04 | Núcleo de enrutado, validación y normalización de errores | feature | Must | 8 |
| BT-13 | Doble de proveedor en memoria para pruebas del núcleo | feature | Must | 3 |
| BT-01 | Operación de guardado con marca de sobrescritura | feature | Must | 5 |
| BT-03 | Recuperación, eliminación, verificación y listado | feature | Must | 8 |
| BT-06 | Registro de proveedores, activación y validación-en-seco | feature | Must | 8 |
| BT-07 | Resguardo de credenciales que entra pero no sale | feature | Must | 5 |
| BT-08 | Adaptador de proveedor local | feature | Must | 5 |
| BT-10 | Batería de contrato única por proveedor y gate de cobertura | devops | Must | 5 |

Las trece BT superan el mínimo de diez exigido para `library` (regla 06 §2.2). BT-02, BT-09 y BT-12 son Should.

## DoR vigente

- US: 7 criterios (valor explícito, CU relacionado, NB/RN, Given/When/Then con ≥ 2 escenarios en Must y Should, estimación Fibonacci que cabe en un sprint, MoSCoW justificada, sin dependencias bloqueantes).
- BT: 5 criterios (fuente upstream verificable, US consumidora o infraestructura compartida justificada, alcance < 1 sprint, dependencias identificadas, tipo y estimación declarados).
- Excepciones: spike con caja temporal, BT de infraestructura compartida, US con mecanismo pendiente del intake.
- Aprobador: AG-06 (Scrum Master + Backlog Curator), con visto de AG-05 para excepciones arquitectónicas.

## Trazabilidad

- Upstream: NB-07 (principal), NB-03 y NB-06 (soporte); CU-01 a CU-06 y RN-01, RN-02, RN-03 de 02; ADR-01 a ADR-05, componentes, `contratos-abstractions_v1.0.md` y `extensibilidad_v1.0.md` de 05; intake §17 (P.5, P.6, P.7, P.8, P.10, P.11).
- Downstream: 07 (asignación de US a sprints y velocity), 08 (acceptance tests a partir de los Given/When/Then y la batería de contrato), 10 (onboarding al backlog).
- Cada US referencia al menos un CU; cada BT referencia su fuente upstream y al menos una US consumidora o justificación de infraestructura compartida.

## Revisores acotados

- AG-02 (Analista Funcional): firma la trazabilidad US↔CU sin huérfanas.
- AG-05 (Arquitecto): valida que cada BT se justifique en una ADR, un componente o un contrato de 05.
- AG-08 (QA): valida que los criterios de aceptación sean verificables y aptos para 08.
- AG-07 (PM ágil): alinea capacidad y secuencia con el plan de sprints de 07 sin invadir su titularidad.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 06 de geovial-storage: índice del product backlog, el backlog técnico y la DoR; resumen de cuatro épicas, cinco US Must del MVP, diez BT prioritarias y DoR vigente, con trazabilidad upstream/downstream y revisores acotados. |
