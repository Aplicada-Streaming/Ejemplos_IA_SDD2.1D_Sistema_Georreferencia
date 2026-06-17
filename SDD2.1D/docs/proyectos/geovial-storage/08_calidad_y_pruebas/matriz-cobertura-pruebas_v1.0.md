# Matriz de cobertura de pruebas — geovial-storage

**Proyecto:** geovial-storage
**Documento:** matriz-cobertura-pruebas_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (variante QA + SDET Library)

## 1. Propósito y alcance

Documento bisagra de la categoría 08. Relaciona los seis casos de uso (CU-01 a CU-06), las tres reglas de negocio (RN-01 a RN-03) y las seis NFR (NFR-01 a NFR-06) con los casos de prueba de `casos-prueba-referenciales_v1.0.md`, y consolida la cobertura por capa. Las tres tablas obligatorias de la regla 08 §4.5 (CU↔Tests, NFR↔Tests, RN↔Tests) están presentes, más la tabla de cobertura por capa. El estado de cada test es Pendiente hasta su primera ejecución (el mini-plan de 07 arranca el 2026-06-15); la matriz se actualiza al cierre de cada tramo.

## 2. Trazabilidad CU↔Tests

| CU | Criterio Given-When-Then | Test ID | Tipo | Estado |
| --- | --- | --- | --- | --- |
| CU-01 | CA-01: Given proveedor activo y contenido de 245 KB con destino válido, When invoca guardar, Then devuelve identificador no vacío y tamaño 245 KB | TC-01 | Unit | Pendiente |
| CU-01 | CA-02: Given contenido de 0 bytes, When invoca guardar, Then rechaza con CONTENIDO_VACIO y no crea archivo | TC-02 | Unit | Pendiente |
| CU-01 | CA-03: Given identificador existente sin sobrescritura, When invoca guardar, Then rechaza con IDENTIFICADOR_DUPLICADO y conserva el preexistente | TC-03 | Unit | Pendiente |
| CU-01 | CA-04: Given identificador existente con sobrescritura, When invoca guardar, Then reemplaza el contenido y devuelve el mismo identificador | TC-04 | Unit | Pendiente |
| CU-02 | CA-01: Given archivo guardado de 245 KB, When invoca recuperar, Then devuelve contenido idéntico byte a byte y tipo image/jpeg | TC-05 | Unit | Pendiente |
| CU-02 | CA-02: Given identificador inexistente, When invoca recuperar, Then devuelve IDENTIFICADOR_INEXISTENTE sin contenido | TC-06 | Unit | Pendiente |
| CU-02 | CA-03: Given archivo de 245 KB y rango 0 a 1023, When invoca recuperar por rango, Then devuelve exactamente 1024 bytes | TC-07 | Unit | Pendiente |
| CU-02 | CA-04: Given proveedor que no responde, When invoca recuperar, Then devuelve PROVEEDOR_NO_DISPONIBLE sin exponer credenciales | TC-08 | Unit | Pendiente |
| CU-03 | CA-01: Given archivo guardado, When invoca eliminar, Then confirma y CU-04 lo reporta inexistente | TC-09 | Unit | Pendiente |
| CU-03 | CA-02: Given identificador inexistente, When invoca eliminar, Then informa éxito por idempotencia sin error | TC-10 | Unit | Pendiente |
| CU-03 | CA-03: Given tres archivos bajo el prefijo, When invoca eliminar bajo prefijo, Then devuelve cantidad eliminada de 3 | TC-11 | Unit | Pendiente |
| CU-04 | CA-01/CA-02/CA-03: Given guardado, inexistente y eliminado, When invoca verificar, Then presente=true con tamaño y presente=false respectivamente | TC-12 | Unit | Pendiente |
| CU-05 | CA-01/CA-03: Given tres archivos bajo un prefijo y ninguno bajo otro, When invoca listar, Then tres identificadores sin testigo y lista vacía sin error | TC-13 | Unit | Pendiente |
| CU-05 | CA-02: Given diez archivos y página de 4, When invoca listar, Then devuelve 4 identificadores y testigo de continuación no vacío | TC-14 | Unit | Pendiente |
| CU-05 | CA-04: Given proveedor que no responde, When invoca listar, Then devuelve PROVEEDOR_NO_DISPONIBLE sin exponer credenciales | TC-15 | Unit | Pendiente |
| CU-06 | CA-01: Given proveedor local y parámetros válidos de remoto, When activa el remoto, Then confirma el cambio y las operaciones siguen sin cambiar su invocación | TC-16 | Integration | Pendiente |
| CU-06 | CA-02: Given credenciales con formato inválido, When intenta activar, Then rechaza con CREDENCIALES_INVALIDAS y mantiene el local | TC-17 | Unit | Pendiente |
| CU-06 | CA-03: Given credenciales válidas pero proveedor que rechaza conexión, When intenta activar, Then rechaza con PROVEEDOR_INACCESIBLE y conserva el anterior | TC-18 | Integration | Pendiente |
| CU-06 | CA-04: Given actor sin alcance de raíz, When intenta cambiar el proveedor, Then rechaza con AUTORIZACION_INSUFICIENTE sin modificarlo | TC-19 | Unit | Pendiente |
| CU-06 | FA-02: Given configuración candidata y modo validación-en-seco, When solicita validar sin activar, Then reporta resultado sin cambiar el proveedor activo | TC-20 | Unit | Pendiente |

Cobertura bidireccional: los seis CU tienen al menos un TC; cada criterio Given-When-Then declarado en 02 tiene un test asociado. No hay CU huérfano ni criterio sin test.

## 3. Trazabilidad NFR↔Tests

| NFR | SLA | Test | Tooling (rol abstracto) |
| --- | --- | --- | --- |
| NFR-01 latencia p95 (proveedor local) | ≤ 1 s para archivos ≤ 5 MB | TC-25 | Banco de medición de latencia p95 |
| NFR-02 tamaño máximo de archivo | Configurable; por defecto 25 MB | TC-26 | Test unitario de límite con fixtures en el límite y por encima |
| NFR-03 transparencia entre proveedores | 0 diferencias de comportamiento observable; 0 ramas por proveedor en el consumidor | TC-21 | Corredor de contract tests parametrizado por proveedor |
| NFR-04 integridad del contenido | 100 % igualdad binaria byte a byte | TC-05, TC-23 | Test de ida y vuelta; property-based con semilla |
| NFR-05 no filtración de credenciales | 0 ocurrencias en resultados, errores y registros | TC-08, TC-15, TC-24 | Property-based y analizador estático |
| NFR-06 cobertura de pruebas (gate de CI) | Líneas ≥ 80 %; branches ≥ 70 % | Gate G-03 sobre la suite completa | Medidor de cobertura del runtime |

Cada NFR con objetivo numérico tiene un test asociado y un tooling identificado. NFR-06 se valida como gate de cobertura sobre la suite completa, no como un TC individual; se consolida en la tabla de cobertura por capa (§5).

## 4. Trazabilidad RN↔Tests

| RN | Invariante | Test ID | Tipo |
| --- | --- | --- | --- |
| RN-01 transparencia del proveedor | El contrato público es idéntico cualquiera sea el proveedor activo | TC-16, TC-21, TC-22, TC-27, TC-28 | Integration / Contract / Snapshot |
| RN-02 integridad del archivo | Lo recuperado es idénticamente igual byte a byte a lo guardado | TC-01, TC-03, TC-04, TC-05, TC-07, TC-09, TC-12, TC-23 | Unit / Property-based |
| RN-03 manejo seguro de credenciales | Las credenciales nunca se exponen por la superficie pública ni los errores | TC-08, TC-15, TC-17, TC-19, TC-24 | Unit / Property-based |

Las tres RN tienen contract test o property-based que verifica su invariante. La transparencia (RN-01) se valida con la batería única ejecutada por proveedor (TC-21) y con el snapshot del contrato (TC-22), que detecta cambios incompatibles no intencionales.

## 5. Cobertura por capa

| Capa | Componentes | Líneas (%) | Branches (%) | Mutation score (%) | Umbral mínimo |
| --- | --- | --- | --- | --- | --- |
| Dominio | Abstracciones, Núcleo de enrutado/validación, Registro de proveedores, Resguardo de credenciales | objetivo ≥ 85 | objetivo ≥ 80 | objetivo ≥ 60 | 85 / 80 / 60 |
| Infraestructura | Adaptadores de proveedor (local / remoto / otro) | objetivo ≥ 70 | objetivo ≥ 60 | — | 70 / 60 / — |
| Global (agregado, gate de intake §17 P.6) | Toda la base de código | objetivo ≥ 80 | objetivo ≥ 70 | — | 80 / 70 / — |

Los valores son objetivo hasta la primera medición en CI; se reemplazan por los valores reales al cierre de cada tramo. La cobertura se reporta por capa, no como número global único (regla 08 §4.10).

Reconciliación del gate global con la cobertura por capa: el gate de intake §17 P.6 fija un piso agregado de ≥ 80 % líneas / ≥ 70 % branches sobre la base completa. La regla 08 §2.2 fija pisos por capa (dominio 85/80/60, infraestructura 70/60). Son compatibles: el umbral de dominio supera al global; el de infraestructura (70/60) queda por debajo del global solo en el plano de su propia capa, pero el global se evalúa ponderado sobre toda la base de código, donde el dominio concentra la lógica de la librería y los adaptadores son delgados. El conjunto ponderado satisface el piso global. Se declara explícitamente que ambos gates conviven: G-04 (por capa, más estricto en dominio) y G-03 (global, piso agregado).

## 6. Gaps identificados

- GAP-01 Mutation testing aún no integrado al gate de CI: se corre localmente en dominio desde el Tramo 1 y se integra al gate antes del release (BT-10). Mitiga RQ-05 del plan de pruebas.
- GAP-02 El adaptador de proveedor remoto es Should (BT-09): el MVP verifica la transparencia con el proveedor local y el doble en memoria; el contract test contra el remoto (parte de TC-21) se completa en el Tramo 5. No bloquea el MVP.
- GAP-03 El banco de medición de desempeño para NFR-01 (TC-25) requiere un ambiente equivalente al productivo declarado por 09: hasta entonces se mide en el ambiente de CI como aproximación y se ratifica antes del release.
- GAP-04 El almacenamiento físico seguro de credenciales en reposo (ADR-05) está delegado a 09: en 08 se verifica solo la no filtración por la superficie pública (TC-08, TC-15, TC-24).

## 7. Trazabilidad upstream/downstream

| Dimensión | Referencia |
| --- | --- |
| Upstream | CU-01 a CU-06, RN-01 a RN-03 (02); NFR-01 a NFR-06, ADR-01 a ADR-05 (05); BT-10, BT-13 (06); intake §17 P.6, P.10 |
| Downstream | 09 (los gates G-01 a G-09 se materializan como stages del pipeline), 10 (cómo correr cada categoría de test), 11 (un test ejecutable por ejemplo) |
| Catálogo de tests | `casos-prueba-referenciales_v1.0.md` (TC-01 a TC-28) |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Matriz inicial de cobertura de geovial-storage: tres tablas obligatorias (CU↔Tests, NFR↔Tests, RN↔Tests) más cobertura por capa con reconciliación explícita del gate global de intake y los umbrales por capa de la regla 08, y cuatro gaps con plan de remediación. |
