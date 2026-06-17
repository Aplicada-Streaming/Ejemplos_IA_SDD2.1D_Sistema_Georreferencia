# Arquitectura de solución — geovial-storage

**Proyecto:** geovial-storage
**Documento:** arquitectura-solucion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer

## 1. Objetivo

Este documento describe la arquitectura técnica interna de `geovial-storage`, la librería que expone al backend consumidor (`geovial-api`) una abstracción de alojamiento de archivos transparente, con proveedores de almacenamiento intercambiables (proveedor local, proveedor de almacenamiento de objetos remoto u otros proveedores) seleccionables por el usuario raíz. Está dirigido al equipo de desarrollo que construye y mantiene la librería, a los revisores funcionales (que validan la cobertura de CU-01 a CU-06) y a las categorías downstream 06 (backlog técnico), 08 (testing) y 09 (despliegue). Define el cómo estructural —estilo, vistas, atributos de calidad y decisiones gobernantes— sin entrar en el detalle de implementación de cada operación, que vive en `contratos-abstractions_v1.0.md` y en los ADRs.

## 2. Estilo arquitectónico

Se adopta una arquitectura hexagonal (puertos y adaptadores) con una capa de Abstracciones que define la superficie pública estable, y proveedores intercambiables conectados a un puerto de almacenamiento mediante el patrón estrategia. El núcleo de la librería —validación de entrada, enrutado al proveedor activo y normalización de errores— no conoce ningún proveedor concreto: depende solo del puerto de almacenamiento (la interfaz de proveedor). Cada proveedor (local, de objetos remoto u otro) es un adaptador que implementa ese puerto. El proveedor activo se resuelve en tiempo de ejecución según la configuración fijada por el usuario raíz (CU-06).

Esta elección se sostiene en tres requisitos del proyecto: la transparencia hacia el consumidor (RN-01), que exige que el contrato público no dependa del proveedor; la extensibilidad declarada (intake §17.P.11, agregar proveedores nuevos), que exige un punto de extensión limpio; y el aislamiento del núcleo respecto de los SDK de cada proveedor, que permite probar el dominio sin infraestructura real.

Justificación contra alternativas descartadas:

| Criterio | Capas clásicas | Hexagonal (elegido) | Acceso directo acoplado |
| --- | --- | --- | --- |
| Transparencia del proveedor (RN-01) | Parcial: la capa de infraestructura suele filtrar detalles del proveedor hacia arriba | Total: el núcleo depende de un puerto, no de un proveedor | Nula: el consumidor queda atado al destino físico |
| Extensibilidad (proveedor nuevo) | Requiere tocar la capa de infraestructura existente | Se agrega un adaptador sin tocar el núcleo | Reescritura del acceso a archivos |
| Testeo del núcleo sin infraestructura | Posible pero la dependencia suele ser hacia adentro de afuera | Directo: doble del puerto en memoria | Imposible sin el destino real |
| Complejidad operativa | Baja | Baja-media (una indirección por puerto) | Muy baja pero frágil |

- Capas clásicas (Domain, Application, Infrastructure, Presentation): descartada porque la dependencia tiende a fluir hacia la infraestructura y la transparencia exigida por RN-01 quedaría sujeta a disciplina y no garantizada por la estructura; para una librería sin UI ni orquestación de aplicación, la separación en cuatro capas agrega ceremonia sin resolver el problema central, que es desacoplar del proveedor.
- Acoplar el acceso al destino físico directamente en el backend (sin librería): descartada de raíz por el intake (§17.P.2). Ataría al consumidor a un único destino, violaría RN-01 y obligaría a ramas de código por proveedor; es el anti-patrón que la librería existe para evitar.
- Integrar un único proveedor fijo dentro de la librería: descartada porque incumple NB-07 (mínimo dos destinos configurables) y elimina el punto de extensión.

Vistas C4 incluidas (cuatro vistas mínimas): vista lógica (§3, equivalente a nivel componentes C4), vista de procesos (§4), vista de despliegue (§5, equivalente a contenedores C4 acotada a una librería embebida) y vista de datos (§6).

## 3. Vista lógica

La librería se descompone en una capa de Abstracciones (la superficie pública y los puertos), un núcleo de enrutado y validación, un registro de proveedores y un conjunto de adaptadores de proveedor. La dependencia es unidireccional: los adaptadores dependen del puerto definido en Abstracciones; el núcleo depende de Abstracciones; Abstracciones no depende de nadie.

| Componente | Responsabilidad | Entradas | Salidas | Dependencias | CU cubiertos |
| --- | --- | --- | --- | --- | --- |
| Abstracciones (superficie pública) | Define el contrato de almacenamiento que invoca el consumidor (guardar, recuperar, eliminar, verificar, listar) y el contrato de configuración del proveedor activo; declara el puerto de proveedor y el catálogo de errores | Invocaciones del consumidor con contenido, identificadores y prefijos | Identificadores lógicos, contenido, metadatos, resultados de presencia, listados, errores normalizados | Ninguna (capa estable, sin dependencias salientes) | CU-01 a CU-06 |
| Núcleo de enrutado y validación | Valida la entrada antes de delegar (contenido no vacío, formato de destino, rango, testigo), resuelve el proveedor activo y enruta la operación hacia él; normaliza los errores del proveedor a códigos uniformes | Llamadas a la superficie pública | Llamadas al puerto de proveedor; errores uniformes | Abstracciones; Registro de proveedores | CU-01, CU-02, CU-03, CU-04, CU-05 |
| Registro de proveedores | Mantiene el conjunto de proveedores soportados, valida la selección del usuario raíz, resguarda las credenciales sin exponerlas y fija el proveedor activo tras la comprobación de conectividad y permisos | Selección y parámetros del usuario raíz (CU-06) | Proveedor activo resuelto; confirmación de activación o error | Abstracciones; Resguardo de credenciales; adaptadores | CU-06 |
| Resguardo de credenciales | Custodia los parámetros sensibles del proveedor de modo que entren por configuración pero no salgan por ningún resultado ni mensaje de error | Credenciales y parámetros de CU-06 | Acceso interno controlado para los adaptadores; nunca devuelve secretos por la superficie pública | Ninguna saliente hacia la superficie pública (RN-03) | CU-06 (soporte a CU-02, CU-05) |
| Adaptador de proveedor local | Implementa el puerto de proveedor contra una ubicación local accesible y escribible | Llamadas del puerto | Persistencia, lectura, borrado, presencia y enumeración locales | Puerto de Abstracciones | CU-01 a CU-05 |
| Adaptador de proveedor de objetos remoto | Implementa el puerto de proveedor contra un servicio de almacenamiento de objetos remoto | Llamadas del puerto | Operaciones contra el servicio remoto | Puerto de Abstracciones; Resguardo de credenciales | CU-01 a CU-05 |
| Adaptador de otros proveedores (punto de extensión) | Cualquier proveedor futuro que implemente el puerto y se registre | Llamadas del puerto | Operaciones contra el destino correspondiente | Puerto de Abstracciones | CU-01 a CU-05 |

Cobertura de CU: los seis casos de uso quedan cubiertos. CU-01 a CU-05 atraviesan el núcleo de enrutado hacia el proveedor activo a través del puerto; CU-06 vive en el registro de proveedores con el resguardo de credenciales. No hay CU huérfano y ningún componente excede el alcance funcional de la especificación 02.

## 4. Vista de procesos

La librería se ejecuta dentro del proceso del backend consumidor; no tiene proceso ni hilo propio de larga vida. Cada operación de la superficie pública es una unidad de trabajo dirigida por la llamada del consumidor (request-scoped) y debe ser segura para invocación concurrente desde múltiples solicitudes del backend.

- Concurrencia. Las operaciones de almacenamiento (CU-01 a CU-05) son sin estado a nivel de la librería: no comparten estado mutable entre invocaciones, de modo que pueden ejecutarse en paralelo. El único estado compartido es la referencia al proveedor activo y su configuración, gestionada por el registro de proveedores.
- Modelo de ejecución asincrónico. Las operaciones se modelan como asincrónicas de extremo a extremo (no bloqueantes), porque el proveedor de objetos remoto implica entrada/salida de red; el proveedor local también expone la misma forma asincrónica para sostener la transparencia (RN-01). La recuperación y el listado admiten transferencia por tramos (rango y paginación por testigo de continuación) para no materializar contenidos ni listados completos en memoria.
- Transacciones y atomicidad. No hay transacciones distribuidas. La atomicidad se acota a cada operación individual: un guardado fallido no deja un archivo parcial asociado al identificador (CU-01, postcondición); una eliminación múltiple parcial informa qué identificadores quedaron sin eliminar (CU-03, ELIMINACION_PARCIAL) en vez de simular atomicidad que el proveedor remoto no garantiza.
- Cambio de proveedor activo (CU-06). La activación se trata como una transición controlada: se valida soporte, formato de credenciales y conectividad/permisos antes de fijar el proveedor; si cualquier paso falla, el proveedor activo previo se conserva intacto (sin estado de configuración a medias). La librería no migra los archivos existentes al cambiar de proveedor (decisión funcional de CU-06).
- Manejo de estado en memoria. La librería no cachea contenidos de archivos. El único estado en memoria es la configuración del proveedor activo y el registro de proveedores soportados, ambos de tamaño acotado.

## 5. Vista de despliegue

`geovial-storage` no es una unidad de despliegue independiente: es una librería embebida que se integra al backend consumidor y se distribuye dentro del artefacto del backend (no se publica como paquete redistribuible). Por lo tanto, su "despliegue" es el del contenedor del backend que la aloja.

| Unidad | Naturaleza | Runtime objetivo | Dependencias de infraestructura |
| --- | --- | --- | --- |
| Librería `geovial-storage` | Componente embebido en el proceso del backend | Runtime del backend, dentro del contenedor del backend | Ninguna propia; usa el destino del proveedor activo |
| Destino del proveedor local | Ubicación de almacenamiento accesible y escribible asociada al contenedor del backend | Sistema de archivos del contenedor del backend o volumen montado | Espacio de almacenamiento persistente para el contenedor del backend |
| Destino del proveedor de objetos remoto | Servicio externo de almacenamiento de objetos | Red saliente desde el contenedor del backend | Conectividad de red y credenciales válidas del servicio remoto |

Notas de despliegue, sin detalle de proveedor concreto:

- El proveedor local guarda los binarios en el almacenamiento asociado al contenedor del backend; para que la evidencia sobreviva al reciclado del contenedor, ese almacenamiento debe ser persistente (volumen montado), decisión que corresponde a la categoría 09.
- El cambio de destino (CU-06) no requiere redesplegar la librería: es una reconfiguración en caliente del proveedor activo. El criterio de éxito de NB-07 (interrupción del servicio para cambiar el destino ≤ 1 h) se sostiene en que la activación es una operación de configuración, no un redespliegue.
- La runtime concreta y la base del contenedor se fijan en la categoría 09 a partir del intake §17.P.9.

## 6. Vista de datos

La librería persiste datos binarios (los archivos: típicamente fotografías de relevamientos) y un conjunto mínimo de metadatos por archivo; no administra una base de datos relacional ni un modelo lógico de entidades. Por ser `library` (regla 05 §2.2), no se produce `modelo-datos-logico_v1.0.md`.

- Unidad de dato. El archivo, identificado por un identificador lógico opaco para el consumidor en cuanto a su ubicación física (CU-01, nota). Bajo un mismo prefijo se agrupan los archivos de un relevamiento, lo que habilita el listado por prefijo (CU-05) y la eliminación múltiple (CU-03, FA-02).
- Metadatos por archivo. Tipo de contenido y tamaño persistido, devueltos en la recuperación y la verificación. La librería no transforma ni recodifica el binario (RN-02): el contenido recuperado es idénticamente igual, byte a byte, al guardado.
- Persistencia física. Delegada al proveedor activo: en el almacenamiento del contenedor del backend para el proveedor local, o en el servicio remoto para el proveedor de objetos remoto. La organización física de los binarios y su esquema de nombres internos es responsabilidad de cada adaptador y queda oculta tras el identificador lógico.
- Caché. No hay caché de contenidos en esta versión (CU-02, nota); cada recuperación consulta al proveedor activo.
- Particionamiento. No aplica un esquema de sharding a nivel de la librería; el agrupamiento lógico por prefijo es el único mecanismo de organización expuesto por el contrato.
- Credenciales. Los parámetros sensibles del proveedor son un dato de configuración custodiado por el resguardo de credenciales; entran por CU-06 y no salen por ninguna operación (RN-03).

## 7. Cross-cutting concerns

Decisiones transversales centralizadas para toda la librería:

- Manejo de errores. La librería expone un conjunto único y uniforme de códigos de error, idéntico cualquiera sea el proveedor activo (RN-01). El núcleo de enrutado normaliza cualquier fallo del adaptador a uno de los códigos catalogados (ver catálogo en `dx-error-messages_v1.0.md` de 03 y en `contratos-abstractions_v1.0.md`). Los errores se clasifican en entrada inválida (rechazo antes de delegar), recurso ausente, conflicto de estado, error transitorio (reintentable) y permiso insuficiente. Ningún mensaje de error incluye credenciales ni parámetros de conexión (RN-03).
- Configuración y secretos. La configuración del proveedor activo y sus credenciales entran exclusivamente por CU-06; el resguardo de credenciales garantiza que entran pero no salen. La librería no lee secretos de su propia superficie pública ni los devuelve en resultados; el mecanismo concreto de almacenamiento seguro lo fija ADR-05 y el detalle de stack vive en intake §17.P.5.
- Logging y tracing. La librería emite registros de diagnóstico de sus operaciones a través de una abstracción de logging provista por el consumidor (no acopla un mecanismo de logging propio). Los registros nunca contienen contenido de archivos ni credenciales (RN-03). La correlación de trazas se propaga desde el backend consumidor; la librería no inicia su propio contexto de correlación. El detalle de propagación inter-proyecto vive en la vista de solución.
- Métricas. La librería expone puntos de medición para latencia de cada operación y para el conteo de errores por código, de modo que los NFR de §8 puedan medirse desde el backend que la integra, sin que la librería imponga un sistema de métricas concreto.
- Validación de entrada. Centralizada en el núcleo de enrutado: toda operación valida formato de destino, no-vacuidad del contenido, rango y testigo antes de delegar en el proveedor, de modo que los errores de entrada inválida se rechacen sin contactar al proveedor (CU-01 a CU-05).

## 8. Quality attributes (NFR)

Los objetivos numéricos provienen del intake §17.P.10 (propuestos y ratificables). La transparencia es un NFR derivado de RN-01 y del criterio de éxito de NB-07 (cero cambios de comportamiento percibidos al cambiar el destino).

| NFR | Objetivo numérico | Mecanismo de medición | ADR relacionada |
| --- | --- | --- | --- |
| Latencia de subida/descarga p95 (proveedor local) | ≤ 1 s para archivos de hasta 5 MB | Prueba de carga sobre las operaciones de guardar y recuperar con el proveedor local y archivos de 5 MB; se registra el percentil 95 de la latencia desde la invocación de la superficie pública hasta el resultado, instrumentado por los puntos de medición de §7 (referencia a pruebas de rendimiento en 08) | ADR-01, ADR-04 |
| Tamaño máximo de archivo | Configurable; valor por defecto 25 MB | Prueba de límite: un contenido por encima del máximo configurado dispara TAMANIO_EXCEDIDO sin contactar al proveedor; un contenido en el límite se persiste; el valor por defecto se verifica en la configuración inicial (referencia a 08) | ADR-04 |
| Transparencia entre proveedores | 0 diferencias de comportamiento observable y 0 ramas de código por proveedor en el consumidor | Batería de pruebas de contrato única ejecutada contra cada proveedor soportado (local y de objetos remoto, al menos); para las mismas entradas debe producir resultados equivalentes y el mismo conjunto de códigos de error (RN-01, referencia a 08); sin degradación apreciable de comportamiento al cambiar de proveedor | ADR-01, ADR-04 |
| Integridad del contenido | 100 % de igualdad binaria entre lo guardado y lo recuperado | Prueba de ida y vuelta guardar-recuperar con comparación byte a byte del contenido y verificación del segmento en la recuperación por rango (RN-02, referencia a 08) | ADR-03 |
| No filtración de credenciales | 0 ocurrencias de credenciales o parámetros de conexión en resultados, mensajes de error y registros | Pruebas que fuerzan los errores de proveedor no disponible/inaccesible y verifican que el mensaje no contiene secretos; revisión de que no existe operación pública que devuelva la configuración sensible (RN-03, referencia a 08) | ADR-05 |
| Cobertura de pruebas (gate de CI) | Líneas ≥ 80 %; branches ≥ 70 % | Medición de cobertura en el pipeline de construcción del backend que integra la librería, como gate bloqueante (intake §17.P.6, §17.P.8) | ADR-01 |

## 9. Riesgos arquitectónicos

| Riesgo | Impacto | Probabilidad | Mitigación |
| --- | --- | --- | --- |
| Filtración de un detalle del proveedor a través de la superficie pública (rompe RN-01) | Alto: el consumidor terminaría con ramas de código por proveedor y se perdería la transparencia | Media | Mantener el catálogo de errores y los tipos del contrato sin referencia a proveedor; batería de contrato única por proveedor en CI; revisión de superficie pública en cada cambio (ADR-01, ADR-02) |
| Filtración de credenciales en un resultado, mensaje de error o registro (rompe RN-03) | Alto: compromete toda la evidencia almacenada | Media | Resguardo de credenciales que entran pero no salen; normalización de errores sin parámetros de conexión; prueba específica de no filtración en CI (ADR-05) |
| Diferencias de semántica sutiles entre adaptadores (por ejemplo, orden del listado o idempotencia del borrado) que rompen la equivalencia observable | Medio: el consumidor podría depender de un comportamiento no garantizado | Media | Fijar en el contrato qué se garantiza (cardinalidad y pertenencia del listado, idempotencia del borrado) y qué no (orden); batería de contrato que verifica las garantías en todos los proveedores (ADR-01) |
| Erosión de la superficie pública por cambios incompatibles no detectados a tiempo | Medio: rompería al consumidor `geovial-api` de forma silenciosa | Baja | Política de versionado explícita con compatibilidad hacia atrás y clasificación de cambios mayor/menor; revisión de la superficie pública versionada (ADR-02) |
| Pérdida de evidencia del proveedor local al reciclar el contenedor del backend (almacenamiento no persistente) | Alto: se perderían las fotografías de los relevamientos | Media | Requerir almacenamiento persistente (volumen) para el destino local; decisión y verificación en la categoría 09; documentado en §5 |
| Latencia del proveedor remoto que incumple el objetivo p95 al cambiar de destino | Medio: degradaría la experiencia sin romper el contrato | Media | El objetivo p95 numérico se fija para el proveedor local; para el remoto se mide y se acepta la dependencia de la red; transferencia por tramos para no materializar contenidos completos (ADR-03, ADR-04) |

## 10. Trazabilidad

| Componente / decisión | CU upstream | RN upstream | NFR | ADRs que lo gobiernan | Tests previstos (en 08) |
| --- | --- | --- | --- | --- | --- |
| Abstracciones (superficie pública) | CU-01 a CU-06 | RN-01 | Transparencia, cobertura | ADR-01, ADR-02 | Batería de contrato única por proveedor; verificación de superficie pública estable |
| Núcleo de enrutado y validación | CU-01 a CU-05 | RN-01, RN-02 | Transparencia, integridad, latencia | ADR-01, ADR-03 | Pruebas unitarias de validación y enrutado; rechazo de entrada inválida sin contactar al proveedor |
| Registro de proveedores | CU-06 | RN-01, RN-03 | Transparencia | ADR-01, ADR-05 | Cambio efectivo de proveedor con continuidad del contrato; rechazo por proveedor no soportado/inaccesible/autorización insuficiente |
| Resguardo de credenciales | CU-06 (soporte CU-02, CU-05) | RN-03 | No filtración de credenciales | ADR-05 | Prueba de que ninguna operación ni error filtra credenciales |
| Adaptadores de proveedor | CU-01 a CU-05 | RN-01, RN-02 | Transparencia, integridad, latencia, tamaño máximo | ADR-01, ADR-03, ADR-04 | Pruebas de integración por proveedor con dobles o contenedores efímeros; igualdad binaria; límite de tamaño |
| Punto de extensión de proveedores | CU-01 a CU-05 | RN-01 | Transparencia | ADR-01 | Prueba de que un proveedor nuevo registrado pasa la batería de contrato (ver `extensibilidad_v1.0.md`) |

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Arquitectura de solución inicial de geovial-storage: estilo hexagonal con capa de Abstracciones y proveedores intercambiables por estrategia; cuatro vistas mínimas (lógica, procesos, despliegue, datos); cross-cutting; tabla de NFR con objetivos numéricos del intake §17.P.10 y su mecanismo de medición; riesgos; y trazabilidad CU/RN/NFR/ADR. |
