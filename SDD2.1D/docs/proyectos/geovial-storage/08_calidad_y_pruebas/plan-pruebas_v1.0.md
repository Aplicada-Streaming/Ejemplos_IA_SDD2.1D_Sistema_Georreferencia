# Plan de pruebas — geovial-storage

**Proyecto:** geovial-storage
**Documento:** plan-pruebas_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (variante QA + SDET Library)

## 1. Alcance del plan

Este plan cubre el release inicial (v1.0) de la librería `geovial-storage`, organizado según los cinco tramos del mini-plan de 07 (release-driven, equipo_n=1, 2026-06-15 a 2026-08-21). Incluye la verificación de los seis casos de uso del contrato (CU-01 a CU-06), las tres reglas de negocio (RN-01, RN-02, RN-03) y las seis NFR (NFR-01 a NFR-06).

Módulos incluidos:

- Capa de Abstracciones (superficie pública y puerto de proveedor).
- Núcleo de enrutado, validación de entrada y normalización de errores.
- Registro de proveedores y activación/validación-en-seco.
- Resguardo de credenciales.
- Adaptador de proveedor local.
- Adaptador de proveedor de objetos remoto.
- Batería de contrato única por proveedor y gate de cobertura.

Módulos excluidos del alcance de pruebas de este proyecto:

- El backend consumidor `geovial-api` y su persistencia SQL Server (se prueban en su propio proyecto).
- El mecanismo físico de almacenamiento seguro de credenciales en reposo, delegado a la categoría 09 (ADR-05): aquí se verifica solo el comportamiento de no filtración por la superficie pública.
- Productos de proveedor concretos: se verifican por su conformidad con el puerto, no por su implementación interna.

## 2. Criterios de entrada

Para ejecutar el plan en un tramo dado debe cumplirse:

- Build verde: compilación sin warnings tratados como error (G-01).
- Las US y BT del tramo cumplen la Definition of Ready de 06 (criterios de aceptación Given-When-Then disponibles).
- El doble de proveedor en memoria (BT-13) está disponible antes de los tramos que prueban el núcleo.
- Los fixtures binarios sintéticos están versionados y regenerables.
- El ambiente efímero del proveedor local está disponible para los tramos de integración.

## 3. Criterios de salida

El plan se declara ejecutado con éxito cuando, para el release:

- Todos los criterios Given-When-Then de los CU críticos tienen al menos un TC en verde (ver `matriz-cobertura-pruebas_v1.0.md`).
- Cobertura por capa cumplida: dominio ≥ 85 % líneas / 80 % branches / mutation ≥ 60 %; infraestructura ≥ 70 % líneas / 60 % branches (G-04, G-05).
- Cobertura global cumplida: ≥ 80 % líneas / ≥ 70 % branches (G-03, intake §17 P.6).
- La batería de contrato única pasa contra cada proveedor soportado con resultados equivalentes (G-06, RN-01).
- 0 ocurrencias de credenciales en resultados, errores y registros (G-07, RN-03).
- Latencia p95 ≤ 1 s para archivos ≤ 5 MB con proveedor local (G-08, NFR-01).
- Defectos blockers cerrados; cada bug cerrado generó al menos un TC de regresión.
- La suite de regresión completa en verde; ningún test verde de la versión anterior pasó a rojo sin justificación documentada.

Los criterios de salida formales y numéricos se consolidan en `criterios-validacion_v1.0.md`.

## 4. Riesgos de calidad

Alineados con los riesgos de negocio (intake §11) y arquitectónicos (05).

| Riesgo | Impacto | Probabilidad | Mitigación |
| --- | --- | --- | --- |
| RQ-01 Concentración en un solo desarrollador en el camino crítico (equipo_n=1) | Alto | Media | Priorizar el Tramo 1 (fundaciones del contrato y doble en memoria); automatizar la suite en CI para que la verificación no dependa de ejecución manual |
| RQ-02 El adaptador remoto tiene capacidades dispares respecto del local y rompe la transparencia (RN-01) | Alto | Media | La batería de contrato única se ejecuta contra cada proveedor; el MVP no depende del adaptador remoto (es Should, BT-09); el doble en memoria fija el comportamiento esperado |
| RQ-03 Filtración de credenciales por un mensaje de error de proveedor (RN-03) | Alto | Media | Property-based test que fuerza errores y verifica ausencia de secretos; análisis estático; el resguardo de credenciales no expone operación de lectura |
| RQ-04 El mecanismo de almacenamiento seguro en reposo está pendiente de 09 | Medio | Media | Probar contra el comportamiento fijo de ADR-05 (no filtración por la superficie); marcar como dependencia externa el almacenamiento físico |
| RQ-05 Mutation testing no integrado a CI desde el inicio | Medio | Media | Correr mutation localmente en dominio desde el Tramo 1; integrarlo al gate antes del release (planificado en BT-10) |
| RQ-06 Snapshots de contrato regenerados sin control para que pasen | Medio | Baja | Toda regeneración de snapshot requiere PR con justificación y revisión (regla 08 §4.10) |

## 5. Plan por sprint (tramos del mini-plan de 07)

| Tramo | Alcance de testing | Tipos de test predominantes | TC referenciales | Entregable de calidad |
| --- | --- | --- | --- | --- |
| Tramo 1 — Fundaciones del contrato (BT-11, BT-05, BT-13, BT-04) | Superficie pública estable, catálogo de errores uniforme, núcleo de enrutado y validación contra el doble en memoria | Unit, snapshot del catálogo de errores | TC-01, TC-02 | Núcleo cubierto al umbral de dominio contra el doble; snapshot del contrato base |
| Tramo 2 — Guardado y colisión (BT-01, BT-02, US-01, US-02) | CU-01 y sus flujos (duplicado, sobrescritura), integridad al guardar | Unit, property-based (RN-02) | TC-01, TC-02, TC-03, TC-04 | CU-01 verde con sus cuatro CA; invariante de integridad parcial |
| Tramo 3 — Lectura, borrado, verificación, listado (BT-03, US-03 a US-07) | CU-02 a CU-05 con rango, solo-metadatos, idempotencia, paginación | Unit, integration, property-based (RN-02) | TC-05 a TC-15, TC-23 | CU-02 a CU-05 verdes; ida y vuelta binaria completa |
| Tramo 4 — Configuración del proveedor y credenciales (BT-07, BT-06, US-08, US-09) | CU-06, autorización de usuario raíz, validación-en-seco, no filtración | Unit, integration, property-based (RN-03) | TC-16, TC-17, TC-18, TC-19, TC-20, TC-24 | CU-06 verde; G-07 (no filtración) cumplido |
| Tramo 5 — Proveedores intercambiables, transferencia por tramos, gate (BT-08, BT-12, BT-09, BT-10) | Transparencia entre proveedores, latencia p95, tamaño máximo, batería única, gate de cobertura | Integration, contract, desempeño | TC-21, TC-22, TC-25, TC-26, TC-27, TC-28 | Batería de contrato verde por proveedor; NFR-01 y NFR-02 medidos; gates G-03 a G-08 cumplidos |

La numeración de tramos sigue el mini-plan de 07; este plan no redefine los sprint goals, los consume.

## 6. Recursos

- Personas: un desarrollador que ejerce las facetas QA y SDET (intake §2, equipo_n=1).
- Ambientes: ejecución en proceso para unit; ambiente efímero del proveedor local; contenedor efímero o doble de conformidad para el proveedor remoto; banco de medición de desempeño equivalente al productivo para NFR-01.
- Datasets: fixtures binarios sintéticos versionados (245 KB de referencia, 0 bytes, límite 25 MB y por encima); semillas registradas para property-based.
- Herramientas: las del §3 de `estrategia-testing_v1.0.md`, por rol abstracto.

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU | CU-01 a CU-06 (02) |
| RN | RN-01, RN-02, RN-03 (02) |
| NFR | NFR-01 a NFR-06 (05) |
| Backlog / plan | US-01 a US-09, BT-01 a BT-13 (06); cinco tramos del mini-plan (07) |
| DoD | `definition-of-done_v1.0.md` (canónica; los tramos la referencian, no la redefinen) |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Plan de pruebas inicial de geovial-storage: alcance por release, criterios de entrada y salida, seis riesgos de calidad con impacto/probabilidad/mitigación, plan de testing por los cinco tramos del mini-plan de 07 y recursos para equipo_n=1. |
| 1.0 | 2026-06-15 | Corrección de consistencia interna (sin cambio de alcance): se reasignan los TC del §5 al tramo coherente con el CU/RN que cubren según el catálogo y la matriz, alineados con los tramos del mini-plan de 07. Tramo 3 incorpora los TC de CU-05 listar (TC-13/14/15) y la property-based de integridad (TC-23); Tramo 4 queda con los TC de CU-06 (TC-16 a TC-20) y la property-based de no filtración (TC-24); Tramo 5 corrige la batería de transparencia a TC-21/TC-22/TC-27 y suma los NFR numéricos (TC-25/TC-26) y la extensibilidad (TC-28). Todos los TC del catálogo (TC-01 a TC-28) quedan con tramo asignado. No se altera la numeración ni el mapeo TC↔CU/RN/NFR. |
