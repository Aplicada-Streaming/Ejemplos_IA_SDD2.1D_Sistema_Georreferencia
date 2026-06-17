# Estrategia de testing — geovial-web

Proyecto: geovial-web
Documento: estrategia-testing_v1.0.md
Versión: 1.0
Estado: Propuesto
Fecha: 2026-06-15
Autor: Ingeniero QA / SDET (web-monolith)

## 1. Pirámide de testing deseada

`geovial-web` es un proyecto de tipo `web-monolith`; adopta la pirámide clásica 70 / 20 / 10 (regla 08 §2.2): 70 % de pruebas unitarias, 20 % de integración y 10 % de extremo a extremo más snapshot. El 10 % superior reparte extremo a extremo y snapshot de vistas, dado que el front es interactivo y de render server-side.

| Nivel | Qué cubre en el front | Tooling (rol abstracto) | Porcentaje objetivo |
| --- | --- | --- | --- |
| Unitario | Lógica aislada de la Aplicación de UI: orquestadores de interacción de cada CU, control de visibilidad por rol, control de habilitación por estado, mapeador de errores a feedback y normalización del Cliente de API | Framework de pruebas unitarias del runtime objetivo | 70 % |
| Integración | Consumo del contrato REST contra una base efímera a través de la API: flujos que cruzan Aplicación de UI y Cliente de API contra una instancia real de `geovial-api` respaldada por una base de datos efímera | Framework de pruebas de integración; base de datos efímera (contenedor descartable) | 20 % |
| Snapshot | Output estable de vistas clave de render server-side (listado de relevamientos, revisión sobre mapa con carrusel, resolución de conflictos, ingreso) | Framework de snapshot de vistas | parte del 10 % superior |
| Extremo a extremo / componente de UI | Journey crítico end-to-end y pruebas de componente de UI sobre las vistas interactivas, ejecutadas con un motor headless de UI | Motor headless de UI | parte del 10 % superior |

Justificación contra las dos degeneraciones de la pirámide:

- Contra la pirámide invertida (e2e pesado): el front es delgado en lógica de dominio (no la posee) pero rico en lógica de presentación y de consumo de contrato; esa lógica se valida más barata y determinista en unitario. Concentrar el esfuerzo en e2e produciría una suite lenta, frágil ante cambios de layout y de difícil diagnóstico, contraria al equipo de un solo dev. Los e2e se reservan al journey crítico (ingresar, crear relevamiento, crear marcador, revisar, resolver conflicto, cerrar).
- Contra la pirámide aplanada (cobertura cuantitativa sin distinguir capas): un número global de cobertura puede esconder una Presentación poco probada detrás de una Aplicación de UI muy cubierta. Por eso la cobertura se reporta y se exige por capa (§2), con piso propio para Presentación.

El reparto interno del 10 % superior prioriza snapshot de vistas clave sobre e2e exhaustivo: el snapshot detecta regresiones estructurales del render server-side con bajo costo de mantenimiento, mientras el e2e se acota al journey crítico para sostener la proporción declarada.

## 2. Cobertura mínima por capa

Las capas del front son las de la arquitectura 05 §2: Presentación, Aplicación de UI y Cliente de API. El front no tiene capa de Dominio propia (el dominio es de `geovial-api`), por lo que la fila Dominio no aplica. La capa de infraestructura del front es el Cliente de API (adaptador del contrato y del componente de mapa).

| Capa | Líneas (%) | Branches (%) | Mutation score (%) | Umbral mínimo |
| --- | --- | --- | --- | --- |
| Dominio | no aplica | no aplica | no aplica | el dominio es de geovial-api |
| Aplicación de UI | 80 | 70 | — | 80 / 70 / — |
| Infraestructura (Cliente de API y adaptador de mapa) | 70 | 60 | — | 70 / 60 / — |
| Presentación (vistas y componentes) | 60 | 50 | — | 60 / 50 / — |

Reconciliación con el gate global de §17 P.6: el intake fija un gate global de líneas ≥ 80 % y branches ≥ 70 % para todo el proyecto, y un piso de presentación ≥ 60 %. La tabla por capa desagrega ese gate: Aplicación de UI sostiene el agregado en 80 / 70, infraestructura aporta 70 / 60 y presentación 60 / 50. El gate global se mide sobre la unión de las tres capas y debe alcanzar 80 / 70; los pisos por capa se miden por separado y ninguno puede caer por debajo de su umbral aunque el agregado se cumpla. Mutation score no se exige en `web-monolith` (regla 08 §2.2 lo reserva a `library`).

## 3. Tooling

Frameworks por nivel y por tipo de test, nombrados por rol abstracto y sin atar el documento a productos concretos.

| Nivel / tipo | Framework (rol abstracto) |
| --- | --- |
| Unitario | Framework de pruebas unitarias del runtime objetivo |
| Integración a través de la API | Framework de pruebas de integración con cliente del contrato REST |
| Componente de UI | Motor headless de UI que renderiza componentes y vistas del front sin navegador real interactivo |
| Snapshot de vistas | Framework de snapshot de salida de render |
| Extremo a extremo | Motor headless de UI conduciendo el journey crítico sobre el front desplegado |
| Rendimiento de interacción y carga de circuitos | Cliente de pruebas de rendimiento y de carga capaz de abrir y sostener circuitos concurrentes |
| Cobertura | Reporte de cobertura por capa del runtime |
| Análisis estático | Analizador estático del runtime |

## 4. BDD (si aplica)

Los criterios de aceptación de los 11 CU ya están redactados en Given/When/Then (02). La estrategia los toma como fuente directa de los casos de prueba: cada escenario Given/When/Then de un CU se traduce a un TC del catálogo `casos-prueba-referenciales_v1.0.md`. No se introduce un motor de especificaciones ejecutables `.feature` dedicado en esta versión por el tamaño del equipo (un dev) y porque la traducción Given/When/Then a TC ya da la trazabilidad requerida; los pasos de cada TC conservan la forma Given/When/Then para preservar el vínculo con el CU de origen.

## 5. Mocks y fixtures

- Aislamiento por nivel. En unitario, el Cliente de API y el puerto de acceso al dominio se sustituyen por dobles que devuelven representaciones de dominio y errores normalizados; así la lógica de la Aplicación de UI (visibilidad por rol, habilitación por estado, mapeo de errores) se prueba sin red. En integración, no se mockea la API: se levanta una instancia real de `geovial-api` contra base efímera.
- Reuso y versionado. Los fixtures viven junto al código de pruebas, versionados con el repositorio: usuarios por rol (raíz, jefe general, jefe de área, agente), relevamientos en cada estado del ciclo (recolección, revisión, cerrado), marcadores con y sin conflicto por radio, fotos con y sin ubicación incrustada, y representaciones de error problem+json por código estable (CREDENCIALES_INVALIDAS, USUARIO_INHABILITADO, ROL_SIN_ACCESO_WEB, JERARQUIA_NO_PERMITIDA, FUERA_DE_ALCANCE, RELEVAMIENTO_NO_EN_RECOLECCION, RELEVAMIENTO_NO_EN_REVISION, RELEVAMIENTO_CERRADO, TRANSICION_NO_PERMITIDA, CONFLICTOS_PENDIENTES, RADIO_NO_DEFINIDO, UNIDAD_INVALIDA, ROL_NO_AUTORIZADO).
- Control de duplicación. Los fixtures se centralizan en una biblioteca de datos de prueba compartida por toda la suite; ningún TC redefine inline un fixture que ya exista. Los dobles del Cliente de API se construyen con una factoría única para evitar divergencias de contrato entre tests.

## 6. Datos de prueba

- Origen. Datos sintéticos, generados a partir del modelo conceptual (02 §5) y de los escenarios Given/When/Then de los CU. No se usan datos de producción ni anonimizados: el front no es fuente de verdad y los escenarios son construibles en su totalidad.
- Versionado. El dataset sintético se versiona con el código; cada cambio del esquema autoritativo de `geovial-api` que rompa un fixture obliga a regenerar y revisar el dataset en el mismo cambio.
- Regeneración. Los datasets de integración se siembran en la base efímera al inicio de cada corrida mediante el sembrado de datos seed del front (intake §16.1: geovial-web produce datos seed). Los snapshots de vistas se regeneran solo mediante cambio con justificación y revisión explícita (regla 08 §4.10, antipatrón de snapshot sin política de regeneración); un snapshot no se regenera para que pase.

## 7. Ambiente de testing

- Aislamiento entre tests. Cada test unitario es independiente del orden de ejecución y no comparte estado mutable global. Cada test de integración levanta o reusa una base efímera limpia y siembra su propio dataset; al terminar, la base se descarta.
- Base de datos efímera. La integración corre a través de la API contra una base efímera en contenedor descartable (regla 08 §2.2 para web-monolith: integración contra base efímera). El front no tiene base propia; la base efímera respalda a la instancia de `geovial-api` que la prueba consume.
- Circuito interactivo. Las pruebas de componente de UI y e2e ejercitan el circuito interactivo con un motor headless de UI; cada prueba abre su propio circuito y lo cierra al final, sin compartir sesión ni token entre pruebas.
- Variables de entorno y secretos. La dirección del contrato REST, los parámetros del componente de mapa y las credenciales del front hacia el backend se inyectan desde el entorno de pruebas con valores no productivos; ningún secreto vive en el repositorio (intake §17.P.5).
- Ambiente de referencia para NFR. Las pruebas de rendimiento de interacción (p95 ≤ 200 ms) y de carga de circuitos (≥ 50 concurrentes) corren en un ambiente equivalente al productivo, midiendo la latencia de interacción del front separada de la latencia atribuible al backend (arquitectura §8).

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Estrategia de testing inicial de geovial-web: pirámide 70/20/10 justificada contra la invertida y la aplanada; cobertura por capa (Aplicación de UI 80/70, infraestructura 70/60, presentación 60/50) reconciliada con el gate global ≥ 80 % / ≥ 70 %; tooling por rol abstracto (motor headless de UI, framework de integración, snapshot); Given/When/Then de los CU como fuente de los TC; fixtures centralizados y versionados; datos sintéticos con regeneración controlada de snapshots; ambiente con base efímera a través de la API y ambiente de referencia para los NFR. |
