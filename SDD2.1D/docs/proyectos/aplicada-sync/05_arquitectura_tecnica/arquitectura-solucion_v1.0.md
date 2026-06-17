# Arquitectura de solución — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** arquitectura-solucion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer

## 1. Objetivo

Este documento describe la arquitectura técnica interna del motor de sincronización `aplicada-sync`, un paquete distribuible y agnóstico del dominio que propaga los cambios locales de una aplicación host hacia un backend remoto bajo la política subir-luego-bajar. Está dirigido a quien diseña, implementa y revisa la librería (categorías 06, 08, 10 y 11) y a quien la integra. Documenta el estilo, las cuatro vistas (lógica, procesos, despliegue, datos), los cross-cutting concerns y los atributos de calidad numéricos, sin entrar en el detalle de implementación de cada historia de usuario. La superficie pública versionada vive en `contratos-abstractions_v1.0.md`; el pipeline paso a paso vive en `flujo-ejecucion_v1.0.md`; los puntos de extensión, en `extensibilidad_v1.0.md`.

## 2. Estilo arquitectónico

El motor adopta Clean Architecture interna con una capa Abstractions estable en el centro y una orquestación de pipeline para el ciclo de sincronización. La dependencia apunta siempre hacia adentro: el núcleo del motor (orquestador del ciclo, cola de cambios, registro de estado) depende solo de la capa Abstractions, y los adaptadores concretos (contrato de transporte hacia el backend remoto, almacén local del host, fuente de eventos de conectividad, proveedor de credencial) se inyectan desde el host como implementaciones de esas abstracciones. Sobre ese núcleo, el ciclo de sincronización se expresa como un pipeline de dos fases en orden estricto: fase de subida y, solo después de concluida, fase de bajada (RN-01).

La combinación se elige porque la librería es redistribuible (`redistribuible: true`) y reutilizable fuera de la solución: necesita una superficie pública mínima y estable que no acople a quien la integra a ningún almacén local, transporte ni dominio concretos, y un motor de procesamiento determinista cuyo orden sea una garantía dura, no una opción configurable.

Tabla de estilos versus criterios para este proyecto:

| Criterio | Pipeline puro | Capas planas | Clean Architecture + capa Abstractions (elegido) | Microkernel/plugins |
| --- | --- | --- | --- | --- |
| Tamaño del equipo | 1-5 | 1-10 | 1-10 | 3-15 |
| Dominios de negocio | 1 | 1-3 | 1 (agnóstico) | 1-3 |
| Aislamiento de la superficie pública | Débil | Medio | Fuerte (Abstractions estable) | Fuerte |
| Sustituibilidad de transporte y almacén | Baja | Media | Alta (inversión de dependencia) | Alta |
| Complejidad operativa | Baja | Baja | Baja | Media |
| Testeo del núcleo sin infraestructura | Limitado | Limitado | Alto (núcleo sin dependencias externas) | Alto |

Alternativas descartadas:

- Sincronización ad-hoc embebida en la aplicación host (sin librería). Descartada porque contradice el requisito de reutilización: cada aplicación reimplementaría el orden subir-luego-bajar, la idempotencia y la reanudación, multiplicando defectos y rompiendo la garantía de negocio de NB-04.
- Capas planas con acceso directo del núcleo a un almacén local y un transporte concretos. Descartada porque acoplaría el motor a un almacén y un transporte fijos, impediría testear el núcleo sin infraestructura y haría que cualquier cambio de uno de esos componentes alterara la superficie pública, violando la política de compatibilidad (referencia: especificación funcional §8).

Detalle de la materialización del pipeline en `flujo-ejecucion_v1.0.md`; detalle de los puntos donde el host inyecta sus adaptadores en `extensibilidad_v1.0.md`.

## 3. Vista lógica

Componentes del motor con responsabilidad cohesiva, entradas, salidas y dependencias unidireccionales hacia la capa Abstractions. Ningún componente del núcleo depende de un adaptador concreto: depende de su abstracción y la recibe inyectada desde el host.

| Componente | Responsabilidad | Entradas | Salidas | Dependencias | CU cubiertos |
| --- | --- | --- | --- | --- | --- |
| Coordinador de sesión de sincronización | Punto de entrada público; configura, inicializa y conserva la sesión; valida coherencia de la configuración; orquesta el resto | Configuración de sesión (identificador de host, referencia al almacén local, referencia al backend remoto, proveedor de credencial) | Identificador de sesión y estado inicial; órdenes a los demás componentes | Abstracciones de almacén local, transporte, credencial y conectividad | CU-01 |
| Cola de cambios locales pendientes | Registro persistente y ordenado de los cambios a subir; garantiza una sola entrada por identificador estable; conserva el orden de creación | Cambio local (identificador estable, operación, carga útil opaca, marca de orden) | Confirmación de encolado; tamaño de cola; secuencia ordenada de pendientes | Abstracción de almacén local del host | CU-02, CU-05, CU-06 |
| Orquestador del ciclo subir-luego-bajar | Ejecuta el pipeline de dos fases en orden estricto; controla que la bajada no inicie hasta concluir la subida; arma el resumen del ciclo | Solicitud de ejecución; cola de pendientes; marca de progreso | Resumen del ciclo (subidos, bajados, en conflicto, estado final) | Cola, ejecutor de fases, registro de estado, abstracción de transporte | CU-03, CU-06 |
| Ejecutor de fase de subida | Envía los pendientes al backend remoto en orden, confirma por identificador estable y retira de la cola lo confirmado | Secuencia ordenada de pendientes; credencial vigente | Cambios confirmados; marca de progreso; pendientes no confirmados | Abstracción de transporte; abstracción de credencial; cola | CU-03, CU-06 |
| Ejecutor de fase de bajada | Solicita al backend las actualizaciones posteriores a la última marca y las aplica al almacén local de forma idempotente | Marca de última sincronización; actualizaciones del backend | Actualizaciones aplicadas; elementos en conflicto; nueva marca | Abstracción de transporte; abstracción de almacén local | CU-03, CU-06 |
| Observador de conectividad | Recibe eventos de cambio de conectividad y dispara a lo sumo un ciclo ante recuperación de red, sin reentrada | Eventos de conectividad; bandera de disparo automático; estado de la sesión | Disparo de un ciclo; notificación de resultado al host | Abstracción de fuente de conectividad; coordinador | CU-04 |
| Registro de estado y progreso | Compone y expone el estado de la sesión, el progreso parcial, la cantidad de pendientes y los elementos en conflicto conocidos | Estado interno del ciclo; cola; marca de progreso | Estado consultable; progreso; lista de elementos en conflicto | Abstracción de almacén local; cola | CU-05, CU-06 |

Capa Abstractions (frontera estable, dependencia entrante desde el host):

- Contrato de almacén local del host: persistir y leer la cola y los metadatos de sincronización.
- Contrato de transporte hacia el backend remoto: enviar un cambio por identificador estable y obtener actualizaciones posteriores a una marca.
- Contrato de proveedor de credencial: entregar la credencial vigente del host.
- Contrato de fuente de eventos de conectividad: notificar transiciones de red.
- Contrato de resolución/estrategia de cambio: describir el cambio local de forma opaca al motor.

## 4. Vista de procesos

El motor expone operaciones que el host invoca de forma asincrónica, con un único ciclo de sincronización activo por sesión. La concurrencia se controla así:

- Exclusión mutua del ciclo. El coordinador admite un solo ciclo de subir-luego-bajar en curso por sesión. Una solicitud de ejecución mientras hay un ciclo activo no inicia un segundo ciclo: devuelve el estado de la ejecución vigente (CU-03 flujo 5.C). El observador de conectividad ignora eventos redundantes durante un ciclo en curso para evitar reentrada por rebote de red (CU-04 flujo 5.C).
- Orden de fases como invariante de proceso. La fase de bajada no comienza hasta que la fase de subida confirma que no quedan pendientes confirmables (RN-01). Un corte durante la subida detiene el proceso sin iniciar la bajada y deja la sesión en estado reanudable (CU-03, CU-06).
- Transaccionalidad de grano fino sobre el almacén local. Cada confirmación de subida retira su entrada de la cola y avanza la marca de progreso como una unidad; cada actualización bajada se aplica una sola vez por su identidad (RN-02). No hay transacción distribuida con el backend remoto: la consistencia se reconstruye por idempotencia ante reintento o reanudación.
- Estados de la sesión en memoria y persistidos. La sesión transita entre listo, no autenticada, sincronizando y reanudable. El progreso de la subida se persiste para que la reanudación continúe desde el punto de corte; el estado consultable refleja el progreso parcial durante un ciclo (CU-05 flujo 5.A).
- Encolado concurrente con el ciclo. El host puede encolar cambios mientras un ciclo está en curso; la cola conserva el orden de creación y la no duplicación por identificador, de modo que un cambio encolado durante la subida se subirá en un ciclo posterior sin romper la idempotencia.

## 5. Vista de despliegue

`aplicada-sync` no es una unidad de despliegue autónoma: es un paquete distribuible que se incorpora al proceso de una aplicación host y se ejecuta dentro de su ciclo de vida.

- Unidad entregable. Un paquete distribuible versionado, consumible desde un repositorio de paquetes. No hay servicio, contenedor ni proceso propio del motor; corre embebido en la aplicación host.
- Runtime objetivo. El runtime gestionado del host de la aplicación de campo; las versiones mínimas concretas viven en el intake §17 P.9, no se repiten acá.
- Dependencias de infraestructura. El motor no provee infraestructura: consume, mediante la capa Abstractions, un almacén local del host, un contrato de transporte hacia el backend remoto, una fuente de eventos de conectividad y un proveedor de credencial. Esas implementaciones son responsabilidad del host y de su plataforma.
- Frontera de despliegue. El backend remoto es un sistema externo al motor, alcanzable por red; el motor no lo despliega ni lo opera. La librería tolera por diseño que esa frontera esté indisponible (trabajo sin conexión).
- Distribución y compatibilidad. Por ser redistribuible, cada versión publicada respeta la política de compatibilidad de su superficie pública (ver `contratos-abstractions_v1.0.md` §6). Un sample de demostración ajeno a la solución acompaña la distribución como evidencia de integración (referencia a 11).

## 6. Vista de datos

El motor administra exclusivamente metadatos de sincronización sobre el almacén local del host; no es dueño de los datos de dominio, que son opacos para él, ni de la persistencia primaria del host. Por tratarse de una librería sin modelo de datos propio del dominio, no se genera `modelo-datos-logico` (regla §2.2 para `library`).

- Estructuras de metadatos en el almacén local del host:
  - Cola de cambios pendientes: una entrada por identificador de cambio estable, con la operación, la carga útil opaca y la marca de orden de creación. Clave de unicidad: el identificador estable (RN-02).
  - Marca de progreso de la subida en curso: permite reanudar desde el punto de corte sin reenviar de forma efectiva lo ya confirmado.
  - Marca de última sincronización: límite a partir del cual la fase de bajada solicita actualizaciones.
  - Identidad y estado de la sesión: identificador de host, identificador de sesión y situación (listo, no autenticada, sincronizando, reanudable).
  - Registro de elementos en conflicto conocidos: identificadores que el backend marcó en conflicto y que el motor expone como convivientes (RN-03).
- Opacidad de la carga útil. El motor no interpreta ni valida el contenido de dominio del cambio local; lo transporta y lo persiste como dato opaco.
- Caches y particionamiento. No aplica cache propia ni particionamiento; el volumen objetivo (cola de al menos 1000 cambios pendientes) se sirve desde el almacén local del host sin sharding.
- Esquema físico. El esquema concreto del almacén local lo aporta el adaptador del host a través de la capa Abstractions; el motor define la forma lógica de los metadatos, no su materialización física. Referencia al modelo lógico: no aplica para esta librería.

## 7. Cross-cutting concerns

Decisiones transversales centralizadas:

- Logging y diagnóstico. El motor emite eventos de diagnóstico estructurados con el identificador de sesión, la fase del ciclo y el código de condición; no registra la carga útil de dominio (opaca). El destino del log lo provee el host; el motor solo produce los eventos.
- Trazado del ciclo. Cada ciclo de sincronización lleva un identificador de correlación que atraviesa la fase de subida y la de bajada y se incluye en el resumen del ciclo, de modo que un ciclo disparado manual o automáticamente sea rastreable de punta a punta.
- Métricas. El motor expone contadores de cambios subidos, actualizaciones bajadas, elementos en conflicto, reintentos y reanudaciones, y el tamaño de la cola, como datos consultables (CU-05) y como base de los atributos de calidad de §8.
- Manejo de errores. Catálogo de códigos estables que distingue defecto de integración (entrada inválida, recurso ausente, conflicto de estado) de condición transitoria de conectividad (reintentable o reanudable, sin pérdida ni duplicación). Los códigos son parte del contrato público y no se traducen (referencia: catálogo de errores de la categoría 03). Ante un corte, el motor preserva la cola y deja la sesión recuperable o reanudable; nunca aborta perdiendo datos.
- Configuración y secretos. La configuración de la sesión la arma el host; el motor no almacena credenciales: recibe una credencial vigente de un proveedor inyectado y la usa solo durante la fase que la requiere. La gestión y el resguardo del secreto pertenecen al host.
- Convivencia con conflicto. Tratada como invariante transversal: el motor reporta el conflicto y nunca lo resuelve (RN-03), tanto en el resumen del ciclo como en la consulta de estado.

## 8. Quality attributes (NFR)

Los objetivos numéricos provienen del intake §17 P.10 del proyecto aplicada-sync. El mecanismo de medición se describe de forma abstracta.

| NFR | Objetivo numérico | Mecanismo de medición | ADR relacionada |
| --- | --- | --- | --- |
| Tiempo de sincronización de lote | Lote de 100 cambios sincronizado en <= 30 s en condiciones de red móvil típica | Prueba de rendimiento del ciclo subir-luego-bajar con un lote de 100 cambios contra un backend de prueba que simula latencia móvil; se mide el tiempo total del ciclo | ADR-05 |
| Capacidad de cola local | Tolera una cola local de >= 1000 cambios pendientes sin degradación funcional | Prueba de carga que encola 1000 cambios y verifica encolado, consulta y ejecución correctos; se observa el tamaño de cola reportado | ADR-04 |
| Reanudación sin pérdida | 0 cambios perdidos y 0 duplicados tras un corte en la fase de subida | Prueba de reanudación que interrumpe la subida en un punto arbitrario, reanuda y compara el conjunto aplicado en el backend con el esperado | ADR-06, ADR-07 |
| Idempotencia ante reintento | 100 % de los cambios reenviados o reaplicados producen efecto neto único | Prueba que reenvía cambios y reaplica actualizaciones por su identificador estable y verifica una sola aplicación efectiva | ADR-07 |
| Orden subir-antes-de-bajar | 0 actualizaciones descendentes aplicadas mientras quedan pendientes confirmables | Prueba que ejecuta un ciclo con cola no vacía y verifica que ninguna bajada precede a la última confirmación de subida | ADR-05 |
| Continuidad ante conflicto | 0 ciclos abortados por un estado en conflicto reportado por el backend | Prueba que baja una entidad en conflicto y verifica que el ciclo concluye y la reporta sin abortar | ADR-08 |

## 9. Riesgos arquitectónicos

| Riesgo | Impacto | Probabilidad | Mitigación |
| --- | --- | --- | --- |
| Subida parcial por corte de conectividad que deje datos sin propagar o duplicados | Alto | Alta | Marca de progreso persistida, reanudación desde el punto de corte e idempotencia por identificador estable (ADR-06, ADR-07; CU-06) |
| Disparos concurrentes por rebote de conectividad que generen ciclos paralelos | Medio | Media | Exclusión mutua de un único ciclo activo por sesión y descarte de eventos redundantes (CU-03 5.C, CU-04 5.C) |
| Acoplamiento involuntario de la superficie pública a un almacén o transporte concreto | Alto | Media | Capa Abstractions estable e inversión de dependencia; toda dependencia concreta se inyecta desde el host (ADR-01, ADR-02) |
| Cambio incompatible silencioso que rompa a los consumidores del paquete | Alto | Media | Política de versionado de la superficie pública con incremento de versión mayor ante cambio de contrato (ADR-03; contratos §6) |
| Crecimiento de la cola por encima del volumen objetivo que degrade la operación | Medio | Baja | Diseño de cola ordenada con una entrada por identificador y verificación del objetivo de >= 1000 pendientes (ADR-04; §8) |
| El host inyecta una resolución de conflicto que el motor no debe asumir | Medio | Baja | El motor permanece neutral: reporta el conflicto y nunca lo resuelve (RN-03; ADR-08) |

## 10. Trazabilidad

| CU | RN aplicables | ADRs que lo gobiernan | Componentes | Tests previstos (08) |
| --- | --- | --- | --- | --- |
| CU-01 inicializar y configurar la sesión | RN-02 | ADR-01, ADR-02, ADR-03 | Coordinador de sesión; cola | Inicialización completa; rechazo por configuración incompleta; recuperación de sesión persistida; sesión no autenticada |
| CU-02 registrar y encolar un cambio local | RN-02 | ADR-02, ADR-04, ADR-07 | Cola de cambios pendientes | Encolado incrementa la cola; reencolado no duplica; rechazo sin identificador; rechazo sin sesión |
| CU-03 ejecutar la sincronización subir-luego-bajar | RN-01, RN-02, RN-03 | ADR-05, ADR-07, ADR-08 | Orquestador del ciclo; ejecutores de fase; registro de estado | Orden subir-antes-de-bajar; cola vacía omite subida; corte no dispara bajada; convivencia con conflicto |
| CU-04 detectar conectividad y disparar | RN-01 | ADR-05 | Observador de conectividad; coordinador | Disparo ante red recuperada; no disparo deshabilitado; no reentrada por rebote; no disparo sin credencial |
| CU-05 consultar estado y cola | RN-03 | ADR-04, ADR-08 | Registro de estado; cola | Estado con cola pendiente; progreso parcial; listado de elementos en conflicto; error sin sesión |
| CU-06 reanudar una sincronización interrumpida | RN-01, RN-02 | ADR-05, ADR-06, ADR-07 | Orquestador del ciclo; ejecutores de fase; registro de estado | Reanudación reenvía solo faltantes; reconocimiento por identificador sin duplicar; backend aún inalcanzable; corte durante la reanudación |

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Arquitectura inicial del motor de sincronización aplicada-sync: Clean Architecture con capa Abstractions y pipeline subir-luego-bajar, cuatro vistas, cross-cutting, NFR numéricos del intake §17 P.10, riesgos y trazabilidad CU/RN/ADR. Derivada de NB-04, de la especificación funcional de 02, del marco DX de 03 y del SOLUTION-INTAKE §17 (aplicada-sync). |
