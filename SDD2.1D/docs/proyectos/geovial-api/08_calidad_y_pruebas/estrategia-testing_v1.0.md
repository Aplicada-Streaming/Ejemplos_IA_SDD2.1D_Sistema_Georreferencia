# Estrategia de testing — geovial-api

**Proyecto:** geovial-api
**Documento:** estrategia-testing_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (variante API Testing Specialist)

## 1. Pirámide de testing deseada

Se adopta la pirámide objetivo del tipo `rest-api` (08 §2.2): 70 % unitario, 20 % integración, 10 % extremo a extremo. Los contract tests cuentan dentro del nivel de integración, porque ejercitan la frontera entre el contrato declarado (OpenAPI) y la implementación a través de un cliente HTTP de pruebas contra una base de datos efímera.

| Nivel | Qué cubre | Tooling (rol abstracto) | Porcentaje objetivo |
| --- | --- | --- | --- |
| Unit | Lógica de dominio aislada (invariantes RN-01 a RN-07, RC-01 a RC-06), casos de uso de aplicación con puertos mockeados, servicios transversales (autorización, paginación, idempotencia, versionado) sin infraestructura | Framework de pruebas unitarias del runtime | 70 % |
| Integration | Adaptadores de persistencia contra base efímera, transacciones y restricciones del almacén, contract tests del contrato REST por endpoint, pipeline de sincronización subida/bajada de extremo a extremo HTTP | Cliente HTTP de pruebas; framework de validación de contrato; base de datos efímera | 20 % |
| E2E | Journeys críticos completos sobre la API desplegada: ciclo de vida del relevamiento (crear → asignar → capturar → sincronizar → revisar → resolver conflictos → cerrar) | Cliente HTTP de pruebas contra el contenedor de backend | 10 % |

Justificación contra las dos pirámides degeneradas:

- Contra la pirámide invertida (e2e pesado): el dominio de `geovial-api` concentra invariantes verificables sin infraestructura (jerarquía, transición de estados, identidad de marcador, convivencia con conflictos, idempotencia). Probarlas mayoritariamente por e2e sería lento, frágil y difícil de diagnosticar para un único desarrollador; por eso el grueso es unitario sobre dominio y casos de uso.
- Contra la pirámide aplanada (cobertura cuantitativa sin distinguir capas): se reporta cobertura por capa con umbrales diferenciados (§2) y se exige trazabilidad de cada TC a un CU, RN o NFR, no un número global. Ver `matriz-cobertura-pruebas_v1.0.md`.

Particularidad del tipo `rest-api`: el 100 % de los endpoints públicos del contrato (35 operaciones, ver `matriz-cobertura-pruebas_v1.0.md` §2 y §5) debe estar cubierto por al menos un contract test por versión mayor. Es un piso adicional a los porcentajes de la pirámide, no sustituible por unit tests.

## 2. Cobertura mínima por capa

Pisos por capa de la Clean Architecture de 05 (Dominio, Aplicación, Infraestructura, API). El proyecto no fija mutation score como gate en esta versión; se reporta como métrica observada cuando la herramienta esté en CI (gap planificable, ver matriz §6).

| Capa | Líneas (mín.) | Branches (mín.) | Mutation score | Umbral de referencia (08 §4.9) |
| --- | --- | --- | --- | --- |
| Dominio (entidades, invariantes RN/RC) | 85 % | 80 % | — (objetivo informativo ≥ 60 %) | 85 / 80 / 60 |
| Aplicación (casos de uso, servicios transversales) | 80 % | 70 % | — | 80 / 70 / — |
| Infraestructura (persistencia, identidad/token, almacenamiento) | 70 % | 60 % | — | 70 / 60 / — |
| API / Presentación (superficie REST) | 100 % de endpoints con contract test | — | — | 60 / 50 / — más contract test total |

Notas:

- El intake fija explícitamente aplicación ≥ 80 % e infraestructura ≥ 70 % (§17.P.6); se respetan como piso. El dominio se eleva a 85/80 por ser el núcleo de invariantes, coherente con la referencia de 08 §4.9; subir el piso está permitido (08 §2.2).
- La capa API no se mide por porcentaje de líneas sino por el criterio del tipo `rest-api`: 100 % de endpoints cubiertos por contract test. Cumplir esto domina la cobertura de transporte.
- Reconciliación con el gate global de líneas ≥ 80 % / branches ≥ 70 % del intake §17.P.6: cumplir la cobertura por capa (con aplicación a 80/70 e infraestructura a 70/60) satisface el agregado global; el gate global es el piso del pipeline y la cobertura por capa su descomposición rectora (ver `estrategia-calidad_v1.0.md` §3.1).

## 3. Tooling

Frameworks por nivel y por tipo de test, expresados por rol abstracto (sin productos comerciales ni stack concreto).

| Uso | Rol de herramienta |
| --- | --- |
| Pruebas unitarias de dominio y aplicación | Framework de pruebas unitarias del runtime |
| Dobles de prueba de puertos (repositorios, almacenamiento, identidad, idempotencia) | Framework de mocking / dobles del runtime |
| Pruebas de integración contra base efímera | Base de datos efímera (contenedor descartable o instancia en memoria equivalente) + framework de pruebas |
| Contract tests del contrato REST | Framework de validación de contrato sobre OpenAPI + cliente HTTP de pruebas |
| Fuzz y negativos de endpoints | Generador de casos a partir del esquema OpenAPI (fuzz de contrato) |
| Pruebas e2e de journeys | Cliente HTTP de pruebas contra el contenedor de backend desplegado |
| Pruebas de rendimiento (NFR de latencia y de lote) | Cliente de carga / generador de carga HTTP |
| Cobertura | Reporte de cobertura del runtime por capa |
| Property-based de invariantes (opcional) | Framework de property-based testing equivalente |

## 4. BDD si aplica

Los criterios de aceptación de los CU (02) y de las US (06) ya están en formato Given/When/Then; la DoR (06) los exige como condición de entrada. La estrategia los usa como fuente directa de los acceptance tests: cada escenario Given/When/Then de un CU o una US se materializa como al menos un TC en `casos-prueba-referenciales_v1.0.md`, con el mismo Given/When/Then condensado en los pasos del TC. No se mantiene una capa separada de archivos `.feature` en esta versión; el Given/When/Then vive en la ficha del TC y se referencia desde la US/CU. Si en una iteración posterior se introduce un framework de especificaciones ejecutables, los `.feature` derivarían de estos TC sin redefinir los criterios.

## 5. Mocks y fixtures

- Política de aislamiento: cada test es determinista, reproducible y no depende del orden de ejecución (08 §5.4). Los unit tests aíslan el dominio mockeando los puertos (repositorio, almacenamiento, identidad, idempotencia); los integration y contract tests usan base efímera real, no mocks de persistencia.
- Centralización y reuso: los fixtures se centralizan en `tests/GeoVial.WebApi.Tests/fixtures/` (árbol de 16 §16 del intake), versionados con el código. Builders de datos de dominio (usuario por rol, relevamiento por estado, marcador con/sin conflicto, lote de sincronización) se reutilizan entre tests para evitar duplicación.
- Versionado y control de duplicación: un cambio de fixture compartido pasa por revisión (PR); se prohíbe duplicar un fixture para que un test pase en lugar de corregir la causa.
- Dobles de servicios externos: el adaptador de almacenamiento (`geovial-storage`) se reemplaza por un doble en los unit/integration de la capa de aplicación; existe al menos un test de integración que ejercita el adaptador real contra un destino local efímero. La identidad/token se mockea en unit y se ejercita real en integración.

## 6. Datos de prueba

- Origen: sintéticos, generados por builders y fixtures versionados. No se usan datos reales de campo ni snapshots de producción (no existe producción; proyecto de investigación).
- Cobertura de datos: la jerarquía de cuatro roles (raíz, jefe general, jefe de área, agente), relevamientos en los tres estados (recolección, revisión, cierre), marcadores en conflicto y sin conflicto, fotos con y sin ubicación incrustada, lotes de sincronización de tamaño normal y de ≥ 1000 cambios para el NFR de capacidad.
- Versionado: los datasets de fixtures se versionan con el repositorio; el dataset de carga (lote ≥ 1000 cambios) se genera de forma reproducible por semilla fija.
- Regeneración: los datos derivados (por ejemplo, el lote grande de sincronización o las imágenes sintéticas con/sin metadatos de ubicación) se regeneran por un comando reproducible documentado en `10` (developer guide); la regeneración no es manual ni dependiente de un entorno particular.

## 7. Ambiente de testing

- Aislamiento entre tests: cada test de integración corre contra una base de datos efímera que se crea y descarta por ejecución (o por clase de test), con las migraciones versionadas aplicadas (ADR-02). Ningún test comparte estado mutable con otro.
- Base de datos efímera: contenedor descartable de la base relacional o instancia en memoria equivalente; se siembra con fixtures y se reinicia entre suites para garantizar independencia del orden.
- Contenedores: los e2e corren contra el contenedor de backend levantado con sus dependencias (base y destino de almacenamiento local), reproduciendo la vista de despliegue de 05 §5.
- Variables de entorno y secretos: el ambiente de pruebas usa secretos no productivos inyectados por el entorno (clave de firma de tokens de prueba, credenciales de proveedor de almacenamiento de prueba), nunca commiteados (intake §17.P.5). El proveedor de almacenamiento por defecto en pruebas es el local sobre un volumen efímero.
- Ambiente equivalente al productivo para NFR: las pruebas de rendimiento (latencia p95, lote ≥ 1000) se ejecutan en un ambiente dimensionado de forma equivalente al productivo, condición de los criterios de validación (`criterios-validacion_v1.0.md`).

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Estrategia de testing inicial de geovial-api: pirámide 70/20/10 justificada contra las pirámides degeneradas, cobertura mínima por capa con piso del intake y reconciliación con el gate global, tooling por rol abstracto, uso del Given/When/Then de CU/US como acceptance tests, política de mocks y fixtures, datos de prueba sintéticos reproducibles y ambiente con base de datos efímera y secretos no productivos. Declara el 100 % de endpoints públicos cubiertos por contract test como piso del tipo rest-api. |
