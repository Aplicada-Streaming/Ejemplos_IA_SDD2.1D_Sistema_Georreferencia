# Estrategia de testing — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** estrategia-testing_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (AG-08), variante QA + SDET Library

## 1. Pirámide de testing deseada

`aplicada-sync` es de tipo `library`; la pirámide objetivo de la variante (08_rules §2.2) es 80 unit / 15 integration / 5 snapshot, sin nivel e2e. La librería es transformación entrada-salida sobre una superficie pública: no tiene UI, ni ambiente desplegable, ni journey end-to-end propio, de modo que el e2e clásico no aplica. El "extremo a extremo" del motor es un ciclo de sincronización completo contra dobles de las estrategias de extensión, que se cubre como integración, no como e2e de UI.

| Nivel | Qué cubre | Tooling (rol abstracto) | Porcentaje objetivo | Justificación |
| --- | --- | --- | --- | --- |
| Unit | Lógica aislada del núcleo del motor: cola única por identificador, orquestador del ciclo, registro de estado, observador de conectividad, catálogo de errores | Framework de tests unitarios; framework de property-based testing para invariantes | 80 % | La mayoría del valor del motor son invariantes deterministas verificables sobre la superficie pública sin infraestructura; Clean Architecture deja el núcleo testeable sin almacén ni transporte reales |
| Integration | Interacción del núcleo con las estrategias de extensión mediante dobles y contract tests por interfaz; ciclo completo subir-luego-bajar; reanudación con cortes simulados; volumen de cola >= 1000 | Framework de tests unitarios sobre dobles de contrato; doble de almacén local en memoria y persistente efímero | 15 % | El comportamiento de orden, reanudación y convivencia con conflicto solo se observa al combinar el núcleo con sus adaptadores; los contract tests por interfaz son el corazón de una librería con puntos de extensión |
| Snapshot | Forma estable del resumen del ciclo, del resumen de reanudación, del estado consultable y del conjunto de códigos de error como contrato publicable | Framework de snapshot testing | 5 % | Fija el contrato observable de la superficie pública para detectar cambios incompatibles silenciosos (ADR-03); la matriz de compatibilidad se apoya en estas baselines |

El nivel e2e no aplica: la librería no tiene UI ni ambiente desplegado (08_rules §2.2 ejemplo de library). El sample de demostración MAUI ajeno a la solución (intake §18) se valida en la categoría 11 como evidencia de integración del integrador, no como e2e de este proyecto.

Justificación contra los anti-patrones de pirámide:

- Contra la pirámide invertida (e2e pesado): se evita porque una suite dominada por ciclos completos sería lenta, frágil y de difícil diagnóstico para invariantes que se prueban mejor de forma aislada; el orden, la idempotencia y la no duplicación se verifican como propiedades unitarias antes que como ciclos integrados.
- Contra la pirámide aplanada (cobertura cuantitativa sin distinguir capas): se evita reportando cobertura por capa con umbrales diferenciados (§2) y mutation score en el dominio, no un número global único (08_rules §4.10).

## 2. Cobertura mínima por capa

Las capas se mapean a la arquitectura de 05: el dominio es el núcleo del motor (orquestador, cola, registro de estado, ejecutores de fase, observador de conectividad, catálogo de errores); la infraestructura son los adaptadores de las estrategias de extensión que el proyecto provee para prueba o como referencia; la capa Abstractions es la superficie pública versionada.

| Capa | Líneas (%) | Branches (%) | Mutation score (%) | Umbral mínimo (08_rules §2.2) |
| --- | --- | --- | --- | --- |
| Dominio (núcleo del motor) | >= 85 | >= 80 | >= 60 | 85 / 80 / 60 |
| API pública (capa Abstractions) | 100 de operaciones con contract test | >= 90 | >= 60 | cubierta como dominio (contrato crítico) |
| Infraestructura (adaptadores de estrategia de prueba) | >= 70 | >= 60 | — | 70 / 60 / — |

Gate global del intake §17 P.6: >= 80 % líneas y >= 70 % branches sobre el agregado. Es compatible con las coberturas por capa de esta tabla: el dominio se exige por encima del piso global y la infraestructura al piso de su capa; el agregado ponderado nunca cae por debajo de 80 / 70. Cumplir las coberturas por capa implica cumplir el gate global. La cobertura se reporta siempre por capa, nunca como número global único (08_rules §4.10).

## 3. Tooling

Frameworks descritos por rol abstracto, sin atar el documento a productos comerciales (08_rules §2.2, prohibición de stacks concretos). El stack concreto del runtime vive en el intake §17 P.1/P.9, no acá.

| Nivel / propósito | Framework (rol abstracto) |
| --- | --- |
| Tests unitarios del núcleo | Framework de tests unitarios del runtime objetivo |
| Invariantes (orden, idempotencia, no duplicación) | Framework de property-based testing |
| Contract tests por interfaz de extensión | Framework de tests unitarios sobre dobles que implementan los contratos de extensión |
| Snapshot del contrato (resumen, estado, errores) | Framework de snapshot testing |
| Mutation testing del dominio | Framework de mutation testing |
| Rendimiento del ciclo (lote de 100, cola de 1000) | Cliente de benchmark / carga del runtime con backend de prueba que simula latencia móvil |
| Cobertura por capa | Herramienta de cobertura del runtime con reporte segmentado por capa |

## 4. BDD si aplica

Los criterios de aceptación de los CU (categoría 02) y de las US (categoría 06) están redactados en Given/When/Then. Esta estrategia los toma como fuente directa de los acceptance tests, pero no exige un runner de especificaciones `.feature` separado: dado que la superficie es una librería sin UI, los escenarios Given/When/Then se materializan como tests unitarios y de integración con nombres que reflejan el escenario, manteniendo la trazabilidad uno a uno entre cada criterio Given/When/Then y su TC (ver `matriz-cobertura-pruebas_v1.0.md`). Si en una versión futura se adopta un runner de especificaciones ejecutables, los archivos vivirían junto a la suite de integración; hasta entonces, el mapeo CU/US -> TC de la matriz es el contrato BDD.

## 5. Mocks y fixtures

- Política de aislamiento: el núcleo del motor se prueba contra dobles de las cuatro estrategias de extensión (almacén local, transporte, credencial, conectividad). No hay dependencia de servicios externos reales en la suite unitaria ni de integración; el backend remoto se sustituye por un doble de transporte controlable que puede simular confirmación por identificador, latencia, corte en medio de la subida y reporte de conflicto.
- Dobles centralizados: un único doble de almacén local en memoria y un doble persistente efímero reutilizables por todos los TC; un único doble de transporte parametrizable (confirma, falla, corta, reporta conflicto); un doble de fuente de conectividad que emite transiciones programadas (red disponible, pérdida, rebote). Los dobles viven en un módulo de soporte de tests versionado con el código y se reutilizan entre niveles para evitar duplicación (08_rules §5.4).
- Versionado y control de duplicación: los dobles y fixtures se versionan en el repositorio junto al código de producción; un cambio en un contrato de extensión obliga a actualizar el doble correspondiente en el mismo PR, de modo que el contract test detecte la deriva.
- Sin mocks de carga útil de dominio: la carga útil del cambio local es opaca para el motor (CU-02, contrato §4); los dobles la tratan como un blob arbitrario y nunca la interpretan.

## 6. Datos de prueba

- Origen sintético: todos los datos de prueba se generan sintéticamente. No hay datos reales de producción ni anonimizados, porque la librería es agnóstica del dominio y su carga útil es opaca; un cambio local de prueba es un identificador estable, una operación, un blob opaco y una marca de orden.
- Generación property-based: los conjuntos de cambios para verificar invariantes (orden, idempotencia, no duplicación) se generan con el framework de property-based para cubrir el espacio de entradas: identificadores repetidos, órdenes de creación arbitrarios, tamaños de cola hasta y por encima de 1000, cortes en posiciones arbitrarias de la subida.
- Fixtures de volumen: un generador determinista produce una cola de >= 1000 cambios para el TC de capacidad (NFR Capacidad de cola), con semilla fija para reproducibilidad.
- Versionado y regeneración: los fixtures deterministas se versionan; los datos property-based se regeneran por semilla en cada corrida y la semilla que provoca un fallo se fija como TC de regresión dedicado. Las baselines de snapshot se regeneran solo mediante PR con justificación y revisión (08_rules §4.10, anti-patrón de regeneración no controlada).

## 7. Ambiente de testing

- Aislamiento entre tests: cada TC parte de un estado limpio; el doble de almacén local se reinstancia por test y los TC no dependen del orden de ejecución (08_rules §5.4, determinismo y no dependencia de orden).
- Sin base de datos ni contenedores productivos: la librería no posee infraestructura propia (arquitectura de 05 §5); el almacén local se simula con un doble en memoria para la mayoría de los TC y con un almacén persistente efímero (temporal, descartado al finalizar) para los TC de reanudación que requieren persistencia real de la marca de progreso.
- Sin secretos productivos: la credencial es un valor de prueba provisto por un doble de proveedor de credencial; el motor no emite ni persiste credenciales (cross-cutting §7 de 05). Ningún secreto real se usa en la suite.
- Reproducibilidad: las corridas son deterministas por semilla; los TC de rendimiento declaran sus condiciones de medición (lote de 100, latencia móvil simulada) para que el resultado sea comparable entre corridas y reproducible en CI.
- Ejecución en CI: la suite completa corre en el pipeline (categoría 09) como los gates G2-G9; la suite unitaria y de contrato debe correr en cada PR; las pruebas de NFR numéricos (G7) y mutation (G5) corren al menos al cierre de tramo y antes del release.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU cubiertos | CU-01 a CU-06 (categoría 02) |
| RN verificadas | RN-01 (orden), RN-02 (idempotencia), RN-03 (convivencia con conflicto) |
| NFR medidos | Tiempo de lote, capacidad de cola, reanudación sin pérdida, idempotencia, orden, continuidad ante conflicto (intake §17 P.10; 05 §8) |
| Quality gates | G1 a G9 de `estrategia-calidad_v1.0.md` §3 |
| Downstream | 09 (gates como stages del pipeline), 10 (cómo correr la suite), 11 (al menos un test ejecutable por sample) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Estrategia de testing inicial de aplicada-sync: pirámide 80/15/5 justificada para library sin e2e, cobertura por capa (dominio 85/80/60, API pública 100 % contract, infraestructura 70/60) reconciliada con el gate global del intake §17 P.6, tooling por rol abstracto incluyendo property-based y mutation, política de BDD por mapeo CU/US -> TC, dobles centralizados y fixtures sintéticos/property-based, datos y ambiente de prueba deterministas y aislados. Derivada de la arquitectura de 05, del intake §17 P.6/P.10 y de las reglas 08 §2.2/§4.3. |
