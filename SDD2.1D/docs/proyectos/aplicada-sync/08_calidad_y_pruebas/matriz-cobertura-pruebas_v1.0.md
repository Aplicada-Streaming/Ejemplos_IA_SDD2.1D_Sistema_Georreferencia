# Matriz de cobertura de pruebas — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** matriz-cobertura-pruebas_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (AG-08), variante QA + SDET Library

## 1. Propósito y alcance

Documento bisagra de la categoría 08: relaciona los casos de uso de 02, los NFR del intake §17 P.10 / 05 §8 y las reglas de negocio de 02 con los casos de prueba referenciales del catálogo `casos-prueba-referenciales_v1.0.md`. Contiene las tres tablas obligatorias (CU↔Tests, NFR↔Tests, RN↔Tests) más la tabla de cobertura por capa y los gaps identificados. Los TC son los TC-01 a TC-21 del catálogo; cada uno referencia al menos un CU, RN o NFR. El estado de los tests es Pendiente como línea de base previa al tramo R1 del mini-plan de 07; se actualiza al cierre de cada tramo (08_rules §4.10, anti-patrón de matriz desactualizada).

## 2. Trazabilidad CU↔Tests

Cada CU con su criterio Given/When/Then de 02, el TC que lo cubre, el tipo de test y el estado.

| CU | Criterio Given-When-Then | Test ID | Tipo | Estado |
| --- | --- | --- | --- | --- |
| CU-01 | Given config completa, When inicializa, Then devuelve sesión y estado "listo" (CA-01) | TC-01 | Unit | Pendiente |
| CU-01 | Given config sin backend, When inicializa, Then CONFIGURACION_INCOMPLETA y no crea sesión (CA-02) | TC-02 | Unit | Pendiente |
| CU-01 | Given config sin credencial, When inicializa, Then estado "no autenticada" admite encolar no ejecutar (CA-04) | TC-03 | Unit | Pendiente |
| CU-01 | Given sesión previa persistida, When reinicializa, Then recupera la cola sin perder pendientes (CA-03) | TC-21 (quick-start) y reuso de TC-01 con almacén persistente | Contract | Pendiente |
| CU-02 | Given cola vacía, When encola "chg-100", Then tamaño de cola = 1 (CA-01) | TC-04 | Unit | Pendiente |
| CU-02 | Given "chg-100" pendiente, When reencola "chg-100", Then una sola entrada, tamaño = 1 (CA-02) | TC-05 | Unit | Pendiente |
| CU-02 | Given sesión inicializada, When encola sin identificador, Then IDENTIFICADOR_CAMBIO_AUSENTE (CA-03) | TC-06 | Unit | Pendiente |
| CU-03 | Given 2 pendientes y backend alcanzable, When ejecuta, Then sube 2 antes de cualquier bajada (CA-01) | TC-07 | Integration | Pendiente |
| CU-03 | Given cola vacía, When ejecuta, Then 0 subidos y baja igual (CA-02) | TC-08 | Integration | Pendiente |
| CU-03 | Given backend cae tras 1 confirmado, When ejecuta, Then BACKEND_INALCANZABLE, conserva 2, deja reanudable (CA-03) | TC-09 | Integration | Pendiente |
| CU-03 | Given entidad en conflicto en la bajada, When ejecuta, Then aplica sin abortar y la reporta (CA-04) | TC-16 | Integration | Pendiente |
| CU-04 | Given disparo habilitado y 1 pendiente, When red disponible, Then dispara ciclo y notifica 1 subido (CA-01) | TC-15 | Integration | Pendiente |
| CU-04 | Given ciclo en curso, When dos eventos en 1 s, Then no inicia ciclos paralelos (CA-03) | TC-18 | Integration | Pendiente |
| CU-05 | Given 4 pendientes sin ciclo, When consulta, Then "listo" y 4 pendientes (CA-01) | TC-19 | Unit | Pendiente |
| CU-05 | Given ciclo que subió 1 de 3, When consulta, Then "sincronizando" 1 subido 2 restantes (CA-02) | TC-19 | Unit | Pendiente |
| CU-05 | Given 2 conflictos, When consulta conflictos, Then 2 identificadores convivientes no resueltos (CA-03) | TC-19 | Unit | Pendiente |
| CU-06 | Given reanudable 5 cambios 2 confirmados, When reanuda, Then reenvía solo 3 y luego baja (CA-01) | TC-10 | Integration | Pendiente |
| CU-06 | Given backend ya recibió los 5 no registrados, When reanuda, Then reconoce por identificador sin duplicar (CA-02) | TC-11 | Integration | Pendiente |
| CU-06 | Given reanudable y backend inalcanzable, When reanuda, Then BACKEND_INALCANZABLE conserva pendientes (CA-03) | TC-09 (reuso del corte) | Integration | Pendiente |

Cobertura CU: los seis CU tienen al menos un TC por cada criterio Given/When/Then crítico de su tabla de aceptación en 02. CU-03 y CU-06, los críticos del MVP, concentran property-based adicional (TC-12, TC-13) sumado a sus TC de escenario.

## 3. Trazabilidad NFR↔Tests

Cada NFR con objetivo numérico del intake §17 P.10 / arquitectura 05 §8, su SLA, el TC que lo valida y el tooling de medición (rol abstracto). Todo NFR con objetivo numérico tiene un test asociado (08_rules §6).

| NFR | SLA | Test | Tooling de medición |
| --- | --- | --- | --- |
| NFR Tiempo de sincronización de lote | Lote de 100 cambios en <= 30 s en red móvil típica | TC-20 | Cliente de benchmark del runtime con backend de prueba que simula latencia móvil |
| NFR Capacidad de cola local | Tolera >= 1000 cambios pendientes sin degradación funcional | TC-14 | Generador determinista de 1000 cambios; doble de almacén persistente efímero |
| NFR Reanudación sin pérdida | 0 perdidos y 0 duplicados tras un corte en la subida | TC-09, TC-10 | Test de integración con corte simulado y comparación del conjunto aplicado contra el esperado |
| NFR Idempotencia ante reintento | 100 % de los reenvíos/reaplicaciones con efecto neto único | TC-11, TC-12 | Property-based con reenvíos y reanudaciones; doble de transporte idempotente por identificador |
| NFR Orden subir-antes-de-bajar | 0 bajadas mientras quedan pendientes confirmables | TC-07, TC-13 | Property-based; doble de transporte que registra el orden global de llamadas |
| NFR Continuidad ante conflicto | 0 ciclos abortados por un conflicto reportado | TC-16 | Test de integración con doble de transporte que reporta conflicto en la bajada |

## 4. Trazabilidad RN↔Tests

Cada RN de 02 con el TC que verifica su cumplimiento.

| RN | Enunciado | TC | Tipo |
| --- | --- | --- | --- |
| RN-01 | Orden estricto subir-antes-de-bajar | TC-07, TC-09, TC-13 | Integration, property-based |
| RN-02 | Idempotencia de la sincronización | TC-04, TC-05, TC-11, TC-12 | Unit, integration, property-based |
| RN-03 | Convivencia con estados en conflicto sin bloqueo | TC-16, TC-19 | Integration, unit |

## 5. Cobertura por capa

Umbrales por capa según 08_rules §2.2 para library y `estrategia-testing_v1.0.md` §2. Los valores observados se completan al ejecutar la suite por capa en CI; la línea de base es 0 (suite aún no implementada, ver 07 §9). La cobertura se reporta por capa, nunca como número global único (08_rules §4.10).

| Capa | Líneas (%) observado | Branches (%) observado | Mutation score (%) observado | Umbral mínimo |
| --- | --- | --- | --- | --- |
| Dominio (núcleo del motor: orquestador, cola, ejecutores, registro de estado, observador, catálogo de errores) | pendiente | pendiente | pendiente | 85 / 80 / 60 |
| API pública (capa Abstractions) | pendiente (objetivo 100 % de operaciones con contract test) | pendiente | pendiente | contract 100 % operaciones / 90 branches / 60 mutation |
| Infraestructura (adaptadores de estrategia de prueba/referencia) | pendiente | pendiente | — | 70 / 60 / — |

Reconciliación con el gate global del intake §17 P.6 (>= 80 % líneas / >= 70 % branches): el agregado ponderado de las capas anteriores no puede caer por debajo del piso global; el dominio se exige por encima (85 / 80) y la infraestructura al piso de su capa (70 / 60), de modo que cumplir las coberturas por capa implica cumplir el gate global. El gate G4 de `estrategia-calidad_v1.0.md` §3 verifica las tres condiciones de capa y la global simultáneamente.

## 6. Gaps identificados

- Toda la suite está en estado Pendiente: es la línea de base previa al tramo R1 del mini-plan de 07. No es un gap de diseño sino el punto de partida; se cierra a medida que cada tramo implementa sus TC (plan de pruebas §5).
- CU-01 CA-03 (recuperación de sesión persistida) se cubre con un TC de contract apoyado en almacén persistente (reuso de TC-01) y por TC-21; queda pendiente formalizar un TC-XX dedicado si la verificación de recuperación gana escenarios adicionales en R2 (US-02). Plan de remediación: revisar al cierre de R2.
- El mutation score por capa solo se mide al cierre de tramo y antes del release (gate G5), no en cada PR, por su costo; durante el desarrollo del tramo el dominio se valida con cobertura y property-based. Plan de remediación: ejecución de mutation obligatoria antes de cerrar R1.
- Los NFR de tiempo de lote (TC-20) y capacidad (TC-14) se validan en R3 según el plan de pruebas; hasta entonces figuran como Pendiente sin ser un gap de cobertura del MVP. Plan de remediación: ejecutar TC-14 y TC-20 en R3 antes de declarar el paquete publicable.

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Matriz de cobertura inicial de aplicada-sync con las tres tablas obligatorias (CU↔Tests sobre los criterios Given/When/Then de 02, NFR↔Tests sobre los seis objetivos numéricos del intake §17 P.10, RN↔Tests sobre RN-01 a RN-03), tabla de cobertura por capa con umbrales de library reconciliados con el gate global del intake §17 P.6, y gaps con plan de remediación por tramo. Todos los tests en estado Pendiente como línea de base previa a R1. |
