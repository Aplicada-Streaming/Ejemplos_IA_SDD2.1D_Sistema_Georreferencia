# Estrategia de testing — geovial-mobile

Proyecto: geovial-mobile
Documento: estrategia-testing_v1.0.md
Versión: 1.0
Estado: Propuesto
Fecha: 2026-06-15
Autor: Ingeniero QA / SDET (mobile)

## 1. Pirámide de testing deseada

`geovial-mobile` es un proyecto de tipo `mobile-app-maui`; adopta la pirámide 70 / 15 / 15 (regla 08 §2.2): 70 % de pruebas unitarias, 15 % de integración y 15 % de extremo a extremo, donde las pruebas de interfaz móvil cuentan como extremo a extremo y el snapshot de pantallas críticas se reparte en ese 15 % superior junto a las pruebas de ciclo de vida.

| Nivel | Qué cubre en la app | Tooling (rol abstracto) | Porcentaje objetivo |
| --- | --- | --- | --- |
| Unitario | Lógica aislada de la capa de Aplicación y del Dominio local: servicios de sesión, de captura y de orquestación de sincronización; reglas de la cola (orden de creación, identificador de origen, idempotencia); priorización de ubicación incrustada y agrupación por radio; mapeo de degradaciones por permiso, sin señal y sin espacio | Framework de pruebas unitarias del runtime objetivo | 70 % |
| Integración | Interacción entre componentes y persistencia: servicios contra el almacén local real (transacción local atómica entidad + cambio encolado), migraciones versionadas en el arranque, y el adaptador de la librería de sincronización contra un doble del backend que ejercita el contrato subir-luego-bajar | Framework de pruebas de integración; almacén local efímero | 15 % |
| Interfaz móvil (extremo a extremo) | Journey crítico sobre las pantallas críticas con el framework de pruebas de interfaz móvil: ingreso/relogueo, selección de relevamiento, mapa y creación/movimiento de marcador, captura de foto, comentarios y etiquetas, y estado de sincronización | Framework de pruebas de interfaz móvil | parte del 15 % superior |
| Snapshot de pantallas críticas | Output de render estable de las pantallas críticas (login/relogueo, lista de relevamientos asignados, mapa de captura, detalle de observación, estado de sincronización) | Framework de snapshot de vistas | parte del 15 % superior |

Justificación contra las dos degeneraciones de la pirámide:

- Contra la pirámide invertida (e2e pesado): el valor del proyecto está en la lógica de captura offline y de sincronización (cola, orden subir-antes-de-bajar, idempotencia, reanudación, convivencia con conflictos), que es lógica pura y se valida más barata, determinista y reproducible en unitario, con dobles de los adaptadores de plataforma. Cargar el esfuerzo sobre pruebas de interfaz móvil produciría una suite lenta, dependiente de dispositivo y de difícil diagnóstico, contraria al equipo de un solo dev y al riesgo de demora del ciclo de distribución del paquete (mini-plan 07 §6). Las pruebas de interfaz móvil se reservan al journey crítico y a las pantallas críticas.
- Contra la pirámide aplanada (cobertura cuantitativa sin distinguir capas): un número global de cobertura puede esconder una capa de lógica de sincronización poco probada detrás de una presentación trivialmente cubierta, o al revés. Por eso la cobertura se reporta y se exige por capa (§2), con piso propio diferenciado para lógica y para presentación.

El reparto interno del 15 % superior prioriza el snapshot de pantallas críticas y las pruebas de ciclo de vida sobre un e2e exhaustivo: el snapshot detecta regresiones estructurales del render con bajo costo de mantenimiento y las pruebas de ciclo de vida cubren reinicio, desbloqueo y reanudación, mientras la prueba de interfaz móvil se acota al journey crítico para sostener la proporción declarada.

## 2. Cobertura mínima por capa

Las capas internas son las de la arquitectura 05 §2 (ADR-01): Presentación (vistas y modelos de vista MVVM, más el componente de mapa), Aplicación (servicios de sesión, captura y orquestación de sincronización), Dominio local (entidades replicadas y reglas de la cola) e Infraestructura (repositorio del almacén local, adaptadores de plataforma y cliente del contrato REST). El intake (§17 P.6) fija pisos para dos agrupaciones de testabilidad: la capa de lógica (Aplicación + Dominio local) y la capa de presentación.

| Capa | Líneas (%) | Branches (%) | Mutation score (%) | Umbral mínimo |
| --- | --- | --- | --- | --- |
| Lógica (Aplicación + Dominio local) | 75 | 70 | — | 75 / 70 / — |
| Infraestructura (almacén local, adaptadores de plataforma, cliente REST) | 70 | 60 | — | 70 / 60 / — |
| Presentación (vistas, modelos de vista y componente de mapa) | 60 | 50 | — | 60 / 50 / — |

Reconciliación con el gate global de §17 P.6: el intake fija un gate global de líneas ≥ 80 % y branches ≥ 70 % para todo el proyecto, y pisos de lógica ≥ 75 % y presentación ≥ 60 %. La tabla por capa desagrega ese gate: la capa de lógica sostiene el grueso del agregado, la infraestructura aporta 70 / 60 y la presentación 60 / 50. El gate global se mide sobre la unión de las capas y debe alcanzar 80 / 70; los pisos por capa se miden por separado y ninguno puede caer por debajo de su umbral aunque el agregado se cumpla por compensación. La presentación, con piso de 60 %, no compensa al gate global: para llegar a 80 / 70 agregado, la lógica y la infraestructura deben superar holgadamente sus pisos individuales, lo que es coherente con que la mayor parte del código verificable del proyecto es lógica de captura y sincronización. Mutation score no se exige en `mobile-app-maui` (regla 08 §2.2 lo reserva a `library`).

## 3. Tooling

Frameworks por nivel y por tipo de test, nombrados por rol abstracto y sin atar el documento a productos concretos.

| Nivel / tipo | Framework (rol abstracto) |
| --- | --- |
| Unitario | Framework de pruebas unitarias del runtime objetivo |
| Integración con almacén local | Framework de pruebas de integración con almacén local efímero |
| Contrato de sincronización | Framework de pruebas de contrato de sincronización contra un doble del backend |
| Interfaz móvil (e2e) | Framework de pruebas de interfaz móvil que conduce las pantallas críticas sobre la app instalada |
| Snapshot de pantallas críticas | Framework de snapshot de salida de render de las pantallas críticas |
| Ciclo de vida | Framework de pruebas de interfaz móvil capaz de simular reinicio de la app, desbloqueo del dispositivo y reanudación |
| Modo offline / sincronización | Doble del adaptador de conectividad y del backend que simula conexión, corte y recuperación |
| Rendimiento de sincronización y de arranque | Medidor de tiempo de ciclo y de arranque en frío sobre el dispositivo de referencia |
| Cobertura | Reporte de cobertura por capa del runtime |
| Análisis estático | Analizador estático del runtime |

## 4. BDD (si aplica)

Los criterios de aceptación de los 7 CU ya están redactados en Given/When/Then (02). La estrategia los toma como fuente directa de los casos de prueba: cada escenario Given/When/Then de un CU se traduce a un TC del catálogo `casos-prueba-referenciales_v1.0.md`, y los pasos de cada TC conservan la forma Given/When/Then para preservar el vínculo con el CU de origen. No se introduce un motor de especificaciones ejecutables `.feature` dedicado en esta versión por el tamaño del equipo (un dev) y porque la traducción Given/When/Then a TC ya da la trazabilidad requerida. La DoR de 06 exige que los criterios de aceptación de cada US estén en Given/When/Then antes de entrar al sprint, lo que hace cada US testeable desde su definición.

## 5. Mocks y fixtures

- Aislamiento por nivel. En unitario, los adaptadores de plataforma (ubicación, cámara, almacenamiento de archivos, almacén seguro de credenciales) y el cliente del contrato REST se sustituyen por dobles, de modo que la lógica de captura, de cola y de orquestación de sincronización se prueba sin dispositivo físico ni red (la DoR §1.7 exige que esos dobles estén disponibles para escribir los tests). En integración, no se mockea el almacén local: se usa un almacén local efímero real para verificar la transacción atómica entidad + cambio encolado y las migraciones de arranque.
- Reuso y versionado. Los fixtures viven junto al código de pruebas, versionados con el repositorio: relevamientos en cada estado del ciclo (recolección, revisión, cerrado), marcadores con y sin conflicto por radio, fotos con ubicación de GPS en el momento, con ubicación incrustada y sin ubicación (pendiente), colas de cambios de distintos tamaños (incluido un lote de ≥ 1000 para el NFR de capacidad y uno de 100 para el NFR de tiempo de ciclo), y representaciones de error por código estable (CREDENCIALES_INVALIDAS, SIN_CONEXION_INICIO, VERIFICACION_DISPOSITIVO_FALLIDA, DISPOSITIVO_SIN_SEGURIDAD, SIN_RELEVAMIENTOS_LOCALES, RELEVAMIENTO_CERRADO, PERMISO_UBICACION_DENEGADO, SIN_SENAL_GPS, PERMISO_CAMARA_DENEGADO, ALMACEN_LOCAL_SIN_ESPACIO, ETIQUETA_VACIA, RADIO_NO_DEFINIDO, PERMISO_ALMACENAMIENTO_DENEGADO, FORMATO_FOTO_NO_SOPORTADO, BACKEND_INALCANZABLE, TOKEN_INVALIDO, SUBIDA_NO_CONCLUIDA, MARCA_INVALIDA).
- Control de duplicación. Los fixtures se centralizan en una biblioteca de datos de prueba compartida por toda la suite; ningún TC redefine inline un fixture que ya exista. Los dobles del backend y de los adaptadores de plataforma se construyen con una factoría única para evitar divergencias de contrato entre tests.

## 6. Datos de prueba

- Origen. Datos sintéticos, generados a partir del modelo de datos lógico local (05) y de los escenarios Given/When/Then de los CU. No se usan datos de producción ni anonimizados: la app es una réplica parcial del dominio y los escenarios son construibles en su totalidad, incluidas las fotos sintéticas con y sin datos de ubicación incrustados.
- Versionado. El dataset sintético se versiona con el código; cada cambio del esquema local autoritativo o del contrato del backend que rompa un fixture obliga a regenerar y revisar el dataset en el mismo cambio.
- Regeneración. Las colas de gran volumen (≥ 1000) y los lotes de 100 cambios se generan por factoría determinista al inicio de cada corrida. Los snapshots de pantallas críticas se regeneran solo mediante cambio con justificación y revisión explícita (regla 08 §4.10, antipatrón de snapshot sin política de regeneración); un snapshot no se regenera para que pase.

## 7. Ambiente de testing

- Aislamiento entre tests. Cada test unitario es independiente del orden de ejecución y no comparte estado mutable global. Cada test de integración crea o reusa un almacén local efímero limpio y siembra su propio dataset; al terminar, el almacén se descarta. La migración inicial se aplica al inicio de cada corrida de integración para validar el reconstruido del esquema desde la migración base (ADR-02).
- Almacén local efímero. La integración corre contra un almacén local efímero descartable por test; los binarios de foto se referencian lógicamente fuera de la fila de datos (ADR-02), de modo que el test controla tanto la fila como el binario referenciado.
- Modo offline y conectividad. Las pruebas de modo offline/sincronización ejercitan un doble del adaptador de conectividad que simula los tres estados relevantes: sin conexión (la captura debe funcionar al 100 %), conexión disponible (dispara el ciclo) y corte en medio de la subida (la reanudación debe conservar la cola sin pérdida ni duplicación). El backend se sustituye por un doble que implementa el contrato subir-luego-bajar, deduplica por identificador de origen y reporta conflictos sin abortar.
- Ciclo de vida. Las pruebas de ciclo de vida simulan reinicio de la app y desbloqueo del dispositivo para ejercitar el relogueo por seguridad del dispositivo (RN-04, ADR-05) y la salvaguarda `DISPOSITIVO_SIN_SEGURIDAD`, sin compartir token ni sesión entre pruebas.
- Variables de entorno y secretos. La dirección del contrato REST, los parámetros del componente de mapa y las credenciales de prueba se inyectan desde el entorno de pruebas con valores no productivos; el token de prueba se aloja en un doble del almacén seguro del dispositivo y ningún secreto vive en el repositorio (intake §17.P.5; el keystore de firma se resguarda fuera del repositorio).
- Ambiente de referencia para NFR. Las pruebas de tiempo de ciclo de sincronización (100 cambios ≤ 30 s), de capacidad de cola (≥ 1000) y de arranque en frío (≤ 3 s) corren sobre el dispositivo de referencia Android conectado por USB en modo desarrollador (intake §17.P.8 y P.9), en red móvil típica para el ciclo de sincronización.

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Estrategia de testing inicial de geovial-mobile: pirámide 70/15/15 justificada contra la invertida y la aplanada, con pruebas de interfaz móvil en el e2e y snapshot de pantallas críticas más pruebas de ciclo de vida en el 15 % superior; cobertura por capa (lógica 75/70, infraestructura 70/60, presentación 60/50) reconciliada con el gate global ≥ 80 % / ≥ 70 %; tooling por rol abstracto (framework de interfaz móvil, snapshot, doble de conectividad y de backend para el modo offline/sincronización); Given/When/Then de los CU como fuente de los TC; fixtures centralizados y versionados, incluidas colas de ≥ 1000 y de 100 cambios; ambiente con almacén local efímero, doble de conectividad y dispositivo de referencia para los NFR. |
