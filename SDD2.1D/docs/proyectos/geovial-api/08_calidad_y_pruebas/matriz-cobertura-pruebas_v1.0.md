# Matriz de cobertura de pruebas — geovial-api

**Proyecto:** geovial-api
**Documento:** matriz-cobertura-pruebas_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (variante API Testing Specialist)

## 1. Propósito y alcance

Este documento es la bisagra de trazabilidad de la categoría 08. Relaciona, en tres tablas obligatorias, los 22 casos de uso (CU-01 a CU-22) con sus casos de prueba (CU↔TC), los NFR numéricos del intake §17.P.10 y 05 §8 con sus tests (NFR↔TC), y las siete reglas de negocio (RN-01 a RN-07) con los TC que verifican su cumplimiento (RN↔TC). Agrega la tabla de cobertura por capa y los gaps identificados. Los TC referenciados se especifican en `casos-prueba-referenciales_v1.0.md`. El estado de cada test refleja el plan: a la fecha del documento, todos los TC están en estado Pendiente (la construcción aún no inició; ver 07 §9, bitácora por iniciar); la columna Estado se actualiza al cierre de cada tramo.

Se declara que el 100 % de los 35 endpoints públicos del contrato REST (`contratos-rest_v1.0.md` §3) está cubierto por al menos un contract test por versión mayor, materializado en TC-34 (catálogo de contract tests por recurso/endpoint) y exigido como gate G4 (`estrategia-calidad_v1.0.md` §3). El detalle endpoint por endpoint vive en §5.

## 2. Trazabilidad CU↔Tests

Cada CU lista su criterio Given/When/Then principal (happy path y/o edge dominante), el TC que lo cubre, el tipo de test y el estado. Los 22 CU están cubiertos; no hay CU huérfano.

| CU | Criterio Given-When-Then (condensado) | Test ID | Tipo | Estado |
| --- | --- | --- | --- | --- |
| CU-01 | Given jefe de área, When da de alta agente del nivel inmediato inferior, Then crea y vincula; salto de nivel → JERARQUIA_NO_PERMITIDA | TC-01 | Integration | Pendiente |
| CU-02 | Given jefe de área, When da de baja agente de otra área, Then AGENTE_FUERA_DE_AREA; baja conserva relevamientos | TC-02 | Integration | Pendiente |
| CU-03 | Given credenciales válidas, When inicia sesión, Then emite token; credenciales inválidas → CREDENCIALES_INVALIDAS | TC-03 | Integration | Pendiente |
| CU-04 | Given jefe, When crea relevamiento con tramo no vacío, Then estado recolección; sin tramo → TRAMO_INCOMPLETO | TC-04 | Integration | Pendiente |
| CU-05 | Given jefe con relevamiento abierto, When asigna agente del área, Then registra asignación; relevamiento cerrado → RELEVAMIENTO_CERRADO | TC-05 | Integration | Pendiente |
| CU-06 | Given relevamiento en recolección, When transiciona a revisión, Then cambia estado; transición no contemplada → TRANSICION_NO_PERMITIDA | TC-06 | Integration | Pendiente |
| CU-07 | Given agente asignado, When crea marcador en el radio de otro, Then convive y registra conflicto; baja con observaciones → MARCADOR_CON_OBSERVACIONES | TC-07 | Integration | Pendiente |
| CU-08 | Given marcador existente, When crea observación con fotos, Then ancla y aloja; marcador inexistente → MARCADOR_INEXISTENTE | TC-09 | Integration | Pendiente |
| CU-09 | Given radio definido y fotos en el radio, When carga manual, Then agrupa en un marcador; sin radio → RADIO_NO_DEFINIDO; foto sin ubicación → pendiente manual | TC-10 | Integration | Pendiente |
| CU-10 | Given agente asignado y lote de cambios, When sube, Then aplica una vez; relevamiento no asignado → RELEVAMIENTO_NO_ASIGNADO | TC-11 | Integration | Pendiente |
| CU-11 | Given subida concluida, When baja, Then entrega novedades y marca nueva; sin subida previa → SUBIDA_NO_CONCLUIDA | TC-12 | Integration | Pendiente |
| CU-12 | Given relevamiento del alcance, When consulta para revisión, Then entrega marcadores y conflictos señalados; fuera de ámbito → RELEVAMIENTO_FUERA_DE_AMBITO | TC-13 | Contract | Pendiente |
| CU-13 | Given relevamiento en revisión con conflicto, When unifica, Then reasigna observaciones; fuera de revisión → RELEVAMIENTO_NO_EN_REVISION | TC-14 | Integration | Pendiente |
| CU-14 | Given relevamiento en revisión sin conflictos, When cierra, Then transiciona a cierre; con conflicto pendiente → CONFLICTOS_PENDIENTES | TC-15 | Integration | Pendiente |
| CU-15 | Given relevamiento cerrado, When exporta, Then unidad transferible completa; foto no recuperable → FOTO_NO_RECUPERABLE | TC-16 | Contract | Pendiente |
| CU-16 | Given unidad transferible válida, When importa, Then reconstruye estructura; unidad corrupta → UNIDAD_INVALIDA; reimport idempotente | TC-32 | Integration | Pendiente |
| CU-17 | Given usuario raíz, When configura destino activo, Then activa sin exponer credenciales; rol jefe → ROL_NO_AUTORIZADO | TC-28 | Integration | Pendiente |
| CU-18 | Given solicitud sin token a recurso protegido, When la envía, Then NO_AUTENTICADO; agente intenta acción de nivel superior → ACCION_NO_PERMITIDA | TC-25 | Integration | Pendiente |
| CU-19 | Given solicitud con varios campos inválidos, When la envía, Then un único problem+json que los enumera; fallo interno → ERROR_INTERNO sin detalles | TC-27 | Integration | Pendiente |
| CU-20 | Given 30 relevamientos, When pide página tamaño 10, Then 10 con referencia siguiente; filtro inexistente → FILTRO_NO_SOPORTADO | TC-23 | Integration | Pendiente |
| CU-21 | Given alta con clave de idempotencia, When reintenta misma clave, Then no duplica; clave reutilizada inconsistente → CLAVE_REUTILIZADA_INCONSISTENTE | TC-29 | Integration | Pendiente |
| CU-22 | Given versión mayor vigente, When agrega campo opcional, Then no rompe; versión retirada → VERSION_NO_SOPORTADA | TC-35 | Contract | Pendiente |

Cobertura complementaria por TC adicionales (escenarios edge y de seguridad que refuerzan los CU): TC-08 (transición inversa revisión→recolección, CU-06/RN-05), TC-17/TC-18/TC-19 (corte y reenvío de sincronización, CU-10/CU-11), TC-20 (foto sin ubicación incrustada, CU-09/RN-04), TC-24 (alcance antes de paginar, CU-20/RN-01), TC-26 (acceso fuera de alcance entre pares del mismo nivel, CU-18/RN-01), TC-30 (idempotencia del lote por identificador de origen, CU-21/RN-07), TC-33 (concurrencia de jerarquía y unicidad de asignación). Todos detallados en `casos-prueba-referenciales_v1.0.md`.

## 3. Trazabilidad NFR↔Tests

Cada NFR numérico del intake §17.P.10 y de 05 §8 tiene un test asociado y un tooling de medición. Los NFR no numéricos sostenidos por monitoreo se indican como tales.

| NFR | SLA / objetivo numérico | Test | Tooling (rol abstracto) |
| --- | --- | --- | --- |
| NFR latencia p95 lecturas | ≤ 300 ms en ambiente equivalente al productivo (CU-04, CU-12, CU-20) | TC-21 | Cliente de carga / generador de carga HTTP |
| NFR latencia p95 escrituras | ≤ 500 ms de extremo a extremo, incluida transacción e idempotencia (CU-01, CU-04, CU-08) | TC-22 | Cliente de carga / generador de carga HTTP |
| NFR capacidad de lote de sincronización | ≥ 1000 cambios por relevamiento en una subida sin pérdida ni duplicación (CU-10) | TC-31 | Cliente de carga con dataset de ≥ 1000 cambios por semilla fija |
| NFR idempotencia de operaciones no seguras | 100 % de operaciones repetidas con la misma clave sin efecto duplicado (CU-21) | TC-29, TC-30 | Test de integración con reintentos por clave e identificador de origen |
| NFR integridad de jerarquía y ciclo | 0 violaciones de jerarquía, transición de estado o unicidad de asignación bajo concurrencia (RC-03, RC-04, RC-05) | TC-33 | Test de concurrencia contra base efímera con restricciones del almacén |
| NFR disponibilidad mensual | ≥ 99,5 % mensual; sin SLO de 99,9 % | Monitoreo de sondas de salud en 09 | Métrica observada (sondas de salud del contenedor), no test unitario |
| NFR cobertura de pruebas (gate de CI) | Líneas ≥ 80 %; branches ≥ 70 %; aplicación ≥ 80 %; infraestructura ≥ 70 %; 100 % de endpoints con contract test | TC-34 (contract test total) + gate G3/G4 del pipeline | Reporte de cobertura por capa + framework de validación de contrato |

Todos los NFR con objetivo numérico tienen al menos un test (08 §6). La disponibilidad ≥ 99,5 % se valida por monitoreo en 09, no por test automatizado, por su naturaleza de métrica observada en el tiempo.

## 4. Trazabilidad RN↔Tests

Cada una de las siete reglas de negocio tiene al menos un TC que verifica su invariante.

| RN | Invariante (resumen) | TC | Tipo |
| --- | --- | --- | --- |
| RN-01 | Cada nivel administra solo el inmediato inferior y opera solo su ámbito | TC-01, TC-25, TC-26, TC-24 | Integration |
| RN-02 | La baja revoca acceso pero conserva la autoría histórica | TC-02 | Integration |
| RN-03 | El conflicto de marcadores convive sin bloquear y se resuelve al cierre | TC-07, TC-14, TC-15 | Integration |
| RN-04 | La carga manual prioriza la ubicación incrustada y agrupa por radio | TC-10, TC-20 | Integration |
| RN-05 | El relevamiento avanza recolección→revisión→cierre sin saltos ni cierre con conflictos | TC-06, TC-08, TC-15 | Integration |
| RN-06 | La bajada no se atiende hasta concluir la subida del ciclo | TC-12, TC-19 | Integration |
| RN-07 | Un reenvío o reintento con la misma clave no duplica efectos | TC-29, TC-30, TC-18 | Integration |

## 5. Cobertura por capa y por endpoint

### 5.1 Cobertura por capa

Valores objetivo (piso); los valores observados se completan al cierre de cada tramo a medida que la construcción avanza (07 §9). A la fecha, la construcción no ha iniciado y los observados figuran como pendientes.

| Capa | Líneas (%) | Branches (%) | Mutation score (%) | Umbral mínimo |
| --- | --- | --- | --- | --- |
| Dominio (entidades, invariantes RN/RC) | Pendiente | Pendiente | — (objetivo informativo ≥ 60) | 85 / 80 / 60 |
| Aplicación (casos de uso, servicios transversales) | Pendiente | Pendiente | — | 80 / 70 / — |
| Infraestructura (persistencia, identidad/token, almacenamiento) | Pendiente | Pendiente | — | 70 / 60 / — |
| API / Presentación (superficie REST) | 100 % de endpoints con contract test | — | — | 100 % de endpoints con contract test |

Reconciliación con el gate global del intake §17.P.6 (líneas ≥ 80 % / branches ≥ 70 %): la cobertura por capa es el criterio rector y su cumplimiento (aplicación 80/70, infraestructura 70/60, dominio 85/80) satisface el agregado global del pipeline. Se reporta por capa, no como número global único, para evitar el anti-patrón de la cobertura que esconde capas débiles (08 §4.10). Detalle en `estrategia-calidad_v1.0.md` §3.1 y `estrategia-testing_v1.0.md` §2.

### 5.2 Cobertura de endpoints por contract test (100 %)

Los 35 endpoints públicos del contrato (`contratos-rest_v1.0.md` §3) están cubiertos por contract test, consolidados en TC-34. Inventario por área:

| Área del contrato | Endpoints | CU | Contract test |
| --- | --- | --- | --- |
| Autenticación y sesión (§3.1) | POST /v1/sesiones; DELETE /v1/sesiones/actual; POST /v1/sesiones/revalidacion | CU-03 | TC-34.1 |
| Usuarios y agentes (§3.2) | GET/POST /v1/usuarios; GET/DELETE /v1/usuarios/{id}; POST /v1/agentes; DELETE /v1/agentes/{id} | CU-01, CU-02 | TC-34.2 |
| Relevamientos y ciclo (§3.3) | GET/POST /v1/relevamientos; GET/DELETE /v1/relevamientos/{id}; POST /v1/relevamientos/{id}/transiciones; POST /v1/relevamientos/{id}/cierre | CU-04, CU-06, CU-12, CU-14 | TC-34.3 |
| Asignaciones (§3.4) | GET/POST /v1/relevamientos/{id}/asignaciones; DELETE /v1/relevamientos/{id}/asignaciones/{agenteId} | CU-05 | TC-34.4 |
| Marcadores, observaciones y carga manual (§3.5) | GET/POST/PATCH/DELETE marcadores; POST observaciones; POST fotos; PATCH fotos; POST carga-manual | CU-07, CU-08, CU-09 | TC-34.5 |
| Sincronización (§3.6) | POST /v1/relevamientos/{id}/sincronizacion/subida; .../bajada | CU-10, CU-11 | TC-34.6 |
| Conflictos (§3.7) | GET /v1/relevamientos/{id}/conflictos; POST .../conflictos/{conflictoId}/resolucion | CU-13 | TC-34.7 |
| Portabilidad (§3.8) | POST /v1/relevamientos/{id}/exportacion; POST /v1/relevamientos/importacion | CU-15, CU-16 | TC-34.8 |
| Configuración de almacenamiento (§3.9) | GET/PUT /v1/configuracion/almacenamiento; POST .../validacion | CU-17 | TC-34.9 |

Total: 35 endpoints, 100 % cubiertos por contract test. Cada error problem+json del catálogo (`contratos-rest_v1.0.md` §5) se ejercita por el contract test del endpoint que lo origina y por TC-27 (formato uniforme de error).

## 6. Gaps identificados

| Gap | Estado | Plan de remediación |
| --- | --- | --- |
| Mutation testing no corre en CI | No bloqueante | Se reporta como métrica observada cuando la herramienta esté integrada; objetivo informativo ≥ 60 % en dominio. Planificable post-MVP, no es gate en v1.0 |
| Cobertura observada por capa aún en Pendiente | Esperado | Se completa al cierre de cada tramo (07 §9); el Tramo 4 cierra el reporte completo con el gate G3 (BT-20) |
| TC de portabilidad y almacenamiento (TC-16, TC-28, TC-32) sujetos a cadencia | Could Have | CU-15, CU-16, CU-17 son Could; si el Tramo 4 no alcanza, sus TC se difieren como deuda planificable sin afectar el MVP (NB-01 a NB-05); registrado en 07 §6 y §7 |
| Disponibilidad ≥ 99,5 % validada por monitoreo, no por test | Por diseño | Es métrica observada en 09 (sondas de salud), no test unitario; sin SLO de 99,9 % por tiene_observabilidad_critica=false |

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Matriz de cobertura inicial de geovial-api: tabla CU↔Tests con los 22 CU, tabla NFR↔Tests con los NFR numéricos del intake §17.P.10 y su tooling, tabla RN↔Tests con las 7 RN, tabla de cobertura por capa con reconciliación del gate global, inventario de los 35 endpoints con declaración del 100 % cubierto por contract test, y gaps identificados con plan de remediación. |
