# Backlog técnico — geovial-api

**Proyecto:** geovial-api
**Documento:** backlog-tecnico_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner

## 0. Modo de redacción y nota de convención

Las tareas técnicas viven inline en este documento. El proyecto tiene 21 BT (BT-01 a BT-21), por debajo del umbral de 30 que obligaría a archivos individuales bajo `tareas-tecnicas/` (regla 06 §3.3); el rango 15 a 30 recomienda archivos individuales pero los admite inline conservando la estructura de secciones. Todos los identificadores usan dos dígitos uniformes (BT-01 a BT-21, EP-T1 a EP-T8); no se emplea el patrón heredado `BT-001`. Estimación en Fibonacci, consistente con el product backlog. Cada BT declara su fuente upstream (NB, CU, ADR o contrato) y al menos una US consumidora, o justifica su existencia como infraestructura compartida.

## 1. Épicas técnicas

| Épica | Nombre | Objetivo | Alcance | Fuente upstream | BT contenidas |
| --- | --- | --- | --- | --- | --- |
| EP-T1 | Fundaciones de capas | Establecer el esqueleto de las cuatro capas con dependencia unidireccional al dominio | Estructura de proyecto, modelo de dominio puro, puertos y orquestación de casos de uso | ADR-01; arquitectura §2, §3 | BT-01, BT-02, BT-03 |
| EP-T2 | Persistencia y migraciones | Materializar el almacén relacional, sus restricciones e índices y la migración inicial | Esquema lógico, adaptadores de repositorio, frontera transaccional por comando | ADR-02; modelo lógico §1-§4 | BT-04, BT-05, BT-06 |
| EP-T3 | Autenticación y autorización | Emitir y validar tokens y acotar toda operación por rol y alcance | Adaptador de identidad/token, middleware de autorización jerárquica | ADR-03; CU-03, CU-18; RN-01, RN-02 | BT-07, BT-08 |
| EP-T4 | Comunicación transversal | Uniformar errores y acotar listados con paginación y filtros | Manejador problem+json y catálogo de códigos, servicio de paginación y filtros | ADR-04, ADR-05; CU-19, CU-20 | BT-09, BT-10 |
| EP-T5 | Sincronización, idempotencia y conflictos | Garantizar el ciclo subir-antes-de-bajar, la no duplicación y la convivencia con conflictos | Servicio de idempotencia, pipeline de subida y bajada, tolerancia a conflictos | ADR-06, ADR-07, ADR-08; CU-10, CU-11, CU-13, CU-21; RN-03, RN-05, RN-06, RN-07 | BT-11, BT-12, BT-13, BT-14 |
| EP-T6 | Almacenamiento de archivos | Integrar la abstracción de almacenamiento y la carga manual por radio | Puerto de almacenamiento, adaptador a la librería, agrupación por radio | ADR-09; CU-08, CU-09, CU-15, CU-16, CU-17; RN-04 | BT-15, BT-16 |
| EP-T7 | Versionado, contrato y contract tests | Versionar por URI, publicar el contrato y verificar el 100 % de endpoints | Política de versión por URI, materialización OpenAPI, contract tests | ADR-10; CU-22; contrato REST §2, §6, §7 | BT-17, BT-18, BT-19 |
| EP-T8 | Calidad y observabilidad | Gate de cobertura del pipeline y registros estructurados con correlación | Gate de CI, logging estructurado, puntos de medición de NFR | arquitectura §7, §8; intake §17.P.6, §17.P.8, §17.P.10 | BT-20, BT-21 |

## 2. BT por épica

### 2.1 EP-T1 Fundaciones de capas

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-01 | Esqueleto de las cuatro capas con dependencia unidireccional | feature | Must | 5 | ADR-01; arquitectura §2 | — | Las cuatro capas (Dominio, Aplicación, Infraestructura, API) existen; las referencias apuntan solo hacia el dominio; una verificación de dependencias falla si una capa interior referencia una exterior. |
| BT-02 | Modelo de dominio puro con invariantes | feature | Must | 8 | ADR-01; RN-01 a RN-07, RC-01 a RC-06 | BT-01 | Las entidades y agregados del dominio compilan sin dependencias salientes; las invariantes de jerarquía, ciclo de estado, identidad de marcador y autoría se prueban con tests unitarios sin infraestructura. |
| BT-03 | Puertos y orquestación de casos de uso | feature | Must | 5 | ADR-01; arquitectura §3 (Casos de uso de Aplicación) | BT-01, BT-02 | Existe un puerto declarado por cada dependencia saliente (repositorio, almacenamiento, identidad, idempotencia); cada caso de uso orquesta comandos/consultas validando invariantes vía dominio. |

### 2.2 EP-T2 Persistencia y migraciones

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-04 | Esquema relacional y migración inicial M0001 | feature | Must | 8 | ADR-02; modelo lógico §1, §4 | BT-02 | La migración `M0001_inicial` crea las 16 tablas de dominio y la tabla técnica de idempotencia con sus PK, FK, únicas, parciales y check; siembra el catálogo de roles y el usuario raíz sin administrador (RC-03); se aplica antes de habilitar el tráfico. |
| BT-05 | Adaptadores de repositorio con restricciones e índices | feature | Must | 8 | ADR-02; modelo lógico §2, §3 | BT-03, BT-04 | Cada puerto de repositorio tiene su adaptador relacional; las restricciones de unicidad (RC-05), integridad de jerarquía (RC-03), referencia observación-marcador (RC-02) y estado válido (RC-04) se materializan a nivel del almacén; los índices del §2 del modelo lógico existen. |
| BT-06 | Frontera transaccional atómica por comando | feature | Must | 5 | ADR-02; arquitectura §4 (Transacciones y atomicidad) | BT-05 | Cada comando que muta estado se ejecuta en una transacción local; un fallo no deja efectos parciales; las invariantes que exigen consistencia inmediata se verifican bajo concurrencia sin estados inválidos. |

### 2.3 EP-T3 Autenticación y autorización

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-07 | Adaptador de emisión y validación de token bearer | feature | Must | 8 | ADR-03; CU-03; intake §17.P.5 | BT-03 | El adaptador emite un token opaco con vigencia a partir de credenciales válidas y lo valida en cada solicitud salvo el inicio de sesión; las credenciales inválidas y el usuario inhabilitado se rechazan con el código correspondiente; la clave de firma se inyecta desde el entorno, nunca del control de versiones. |
| BT-08 | Middleware de autorización por rol y alcance | feature | Must | 8 | ADR-03; CU-18; RN-01, RN-02; RC-03 | BT-07 | Toda solicitud autenticada pasa por el middleware antes de cualquier efecto y antes de paginar; un rol que opera fuera de su nivel inmediato inferior o de su ámbito se rechaza (prohibido); la baja de un usuario revoca acceso sin desatribuir su autoría. |

### 2.4 EP-T4 Comunicación transversal

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-09 | Manejador de errores problem+json y catálogo de códigos | feature | Must | 5 | ADR-05; CU-19; contrato REST §5 | BT-01 | Todo fallo se traduce a una representación problem+json RFC 7807 con código estable en mayúsculas sin tildes y estado HTTP acorde; un error de validación con varios campos se devuelve en un único problema que los enumera; un fallo no contemplado devuelve un código genérico sin filtrar detalles. |
| BT-10 | Servicio de paginación, filtros y orden de listados | feature | Must | 5 | ADR-04; CU-20; RN-01 | BT-05, BT-08 | Todo listado acota por alcance antes de paginar, aplica los filtros y el orden soportados y devuelve una página con tamaño efectivo y referencias de navegación; un filtro u orden no soportado se rechaza con el código correspondiente; el tamaño de página tiene tope. |

### 2.5 EP-T5 Sincronización, idempotencia y conflictos

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-11 | Servicio de idempotencia con almacén de claves | feature | Must | 8 | ADR-08; CU-21; RN-07; modelo lógico §1.17 | BT-05 | Una operación no segura con la misma clave de idempotencia no inicia una segunda ejecución y devuelve el resultado registrado; una clave reutilizada con huella distinta se rechaza; la unicidad de la clave se garantiza con una restricción del almacén, no con bloqueos en memoria. |
| BT-12 | Pipeline de subida del lote de cambios locales | feature | Must | 13 | ADR-07, ADR-08; CU-10; RN-06, RN-07; NFR capacidad de lote | BT-11 | La subida procesa el lote cambio por cambio, deduplica por identificador de origen, registra los conflictos sin bloquear y soporta al menos 1000 cambios por relevamiento sin pérdida ni duplicación; una subida interrumpida se reanuda sin reaplicar lo confirmado; intentar subir a un relevamiento cerrado se rechaza. |
| BT-13 | Bajada incremental de actualizaciones por marca | feature | Must | 8 | ADR-07; CU-11; RN-06; RC-06; modelo lógico §1.16 | BT-12 | La bajada entrega solo las novedades posteriores a la marca del cliente y devuelve una marca nueva opaca y monótona; la bajada se rechaza con el código de subida no concluida hasta cerrar la subida del ciclo; el cálculo incremental usa los índices de novedades del modelo lógico. |
| BT-14 | Tolerancia a conflictos de marcadores como estado válido | feature | Must | 5 | ADR-06; CU-07, CU-13; RN-03, RN-05; RC-04 | BT-05 | El dominio admite el conflicto de marcadores como estado válido durante recolección y revisión sin bloquear; la resolución (unificar o separar) se difiere al cierre; el cierre con conflictos pendientes se rechaza. |

### 2.6 EP-T6 Almacenamiento de archivos

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-15 | Puerto de almacenamiento y adaptador a la abstracción | feature | Must | 8 | ADR-09; CU-08, CU-17; contrato consumido geovial-storage | BT-03 | El puerto de almacenamiento delega el binario a la abstracción de la librería y persiste solo la referencia lógica; el proveedor activo es transparente al contrato; los códigos de la librería se normalizan al cruzar al contrato del backend; las credenciales del proveedor entran pero no salen. |
| BT-16 | Carga manual con priorización de ubicación y agrupación por radio | feature | Should | 5 | ADR-09; CU-09; RN-04 | BT-15 | La carga manual prioriza la ubicación incrustada de la foto y agrupa por radio; una foto sin ubicación incrustada queda pendiente de ubicación manual sin inventar coordenada; un radio no definido se rechaza con el código correspondiente. |

### 2.7 EP-T7 Versionado, contrato y contract tests

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-17 | Versionado por URI y política de compatibilidad | feature | Must | 5 | ADR-10; CU-22; contrato REST §6 | BT-01 | Cada recurso se expone bajo un prefijo de versión mayor; un cambio compatible se incorpora dentro de la misma versión mayor sin romper clientes; una versión retirada o un recurso ausente en la versión se rechazan con los códigos de versión; una versión nueva conserva la anterior durante el período de convivencia. |
| BT-18 | Materialización del contrato como especificación OpenAPI versionada | docs | Should | 5 | contrato REST §2; intake §17.P.3 | BT-17 | El contrato lógico de los 22 CU se materializa como una especificación OpenAPI versionada y publicable; los esquemas (DTO), las operaciones por recurso y el catálogo de errores quedan reflejados; la especificación es la fuente de los contract tests. |
| BT-19 | Contract tests del 100 % de endpoints públicos por versión | feature | Must | 8 | contrato REST §7; intake §17.P.6 | BT-18 | Existe un contract test por cada endpoint público de la versión; el error uniforme, la paginación, la idempotencia, el orden subir-antes-de-bajar y el versionado compatible/incompatible se verifican; el gate de CI exige el 100 % de endpoints cubiertos. |

### 2.8 EP-T8 Calidad y observabilidad

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-20 | Gate de cobertura del pipeline de integración continua | devops | Must | 5 | arquitectura §8; intake §17.P.6, §17.P.8 | BT-19 | El pipeline mide cobertura y bloquea si líneas < 80 %, branches < 70 %, aplicación < 80 % o infraestructura < 70 %, o si algún endpoint público carece de contract test; el gate es bloqueante. |
| BT-21 | Registros estructurados con correlación y puntos de medición | feature | Should | 5 | arquitectura §7, §8; intake §17.P.10 | BT-09 | El backend emite registros estructurados con un identificador de correlación por solicitud, sin credenciales ni binarios; expone puntos de medición de latencia por operación, conteo de errores por código y volumen de cambios por ciclo de sincronización, suficientes para los NFR de §8 de arquitectura. |

## 3. Trazabilidad BT↔US↔CU

Para cada BT, las US que la consumen y los CU upstream. Toda BT tiene al menos una US consumidora o se justifica como infraestructura compartida (regla 06 §4.5; anti-patrón "BT sin US consumidora").

| BT | Fuente upstream | US consumidoras | CU upstream | Justificación si no hay US directa |
| --- | --- | --- | --- | --- |
| BT-01 | ADR-01 | (todas) | CU-01 a CU-22 | Infraestructura compartida: el esqueleto de capas sostiene toda la superficie REST (ADR-01). |
| BT-02 | ADR-01; RN/RC | US-01, US-07, US-14, US-29 y derivadas | CU-01 a CU-17 | Infraestructura de dominio compartida; las US listadas son representativas de cada invariante. |
| BT-03 | ADR-01 | (todas las US de recursos) | CU-01 a CU-17 | Infraestructura compartida: puertos y orquestación que consumen todos los casos de uso. |
| BT-04 | ADR-02 | US-01, US-03, US-07, US-10, US-14, US-16 | CU-01 a CU-17 | Persistencia de las entidades que las altas y consultas requieren. |
| BT-05 | ADR-02 | US-01, US-02, US-07, US-10, US-14, US-16, US-40 | CU-01 a CU-17, CU-20 | Adaptadores y restricciones que sostienen altas, bajas y listados. |
| BT-06 | ADR-02 | US-01, US-07, US-10, US-21, US-28, US-29 | CU-01, CU-04, CU-05, CU-10, CU-13, CU-14 | Atomicidad de los comandos que mutan estado. |
| BT-07 | ADR-03; CU-03 | US-05, US-06 | CU-03 | Emisión y validación de token de la sesión. |
| BT-08 | ADR-03; CU-18 | US-37, US-38 | CU-18, CU-01, CU-20 | Autorización jerárquica previa a todo efecto y a la paginación. |
| BT-09 | ADR-05; CU-19 | US-39 | CU-19 | Errores uniformes consumidos por toda US; US-39 es la consumidora directa. |
| BT-10 | ADR-04; CU-20 | US-08, US-25, US-27, US-40, US-41 | CU-04, CU-12, CU-13, CU-20 | Paginación y filtros de todos los listados. |
| BT-11 | ADR-08; CU-21 | US-42, US-43 | CU-21 | Servicio de idempotencia consumido por las operaciones no seguras. |
| BT-12 | ADR-07, ADR-08; CU-10 | US-21, US-22 | CU-10 | Pipeline de subida de la sincronización. |
| BT-13 | ADR-07; CU-11 | US-23, US-24 | CU-11 | Bajada incremental de la sincronización. |
| BT-14 | ADR-06; CU-07, CU-13 | US-14, US-27, US-28, US-29 | CU-07, CU-13, CU-14 | Convivencia con conflictos y su diferimiento al cierre. |
| BT-15 | ADR-09; CU-08, CU-17 | US-17, US-32, US-33, US-35 | CU-08, CU-15, CU-16, CU-17 | Integración con la abstracción de almacenamiento. |
| BT-16 | ADR-09; CU-09 | US-19, US-20 | CU-09 | Carga manual con priorización de ubicación y radio. |
| BT-17 | ADR-10; CU-22 | US-44 | CU-22 | Versionado por URI y política de compatibilidad. |
| BT-18 | contrato REST §2 | US-44, US-39, US-40 | CU-22, CU-19, CU-20 | Especificación OpenAPI que documenta el contrato; soporta US-44 y los contract tests. |
| BT-19 | contrato REST §7 | US-44 | CU-22, CU-19, CU-20, CU-21 | Contract tests del 100 % de endpoints; protege el contrato de US-44. |
| BT-20 | intake §17.P.6 | (todas) | CU-01 a CU-22 | Infraestructura compartida: gate de cobertura que protege todo el backlog (intake §17.P.6, §17.P.8). |
| BT-21 | arquitectura §7 | US-21, US-39 | CU-10, CU-19 | Observabilidad de la sincronización y de los errores; infraestructura compartida de medición. |

Cobertura inversa US→BT (cada US Must y Should consume al menos una BT): las US de cada épica funcional consumen las BT de su épica técnica afín más las transversales (BT-01, BT-03, BT-08, BT-09). Las US Could de EP-06 y EP-07 (US-31 a US-36) consumen BT-15 y las fundaciones; su construcción se difiere con la prioridad de su épica. La matriz completa US→BT detallada por historia vive en la sección 4 de cada `historias-usuario/US-XX-<kebab>_v1.0.md` (tabla de trazabilidad, fila "BT derivadas").

## 4. Referencias cruzadas

- Vista de producto del backlog: `product-backlog_v1.0.md`.
- Filtro de entrada: `definition-of-ready_v1.0.md`.
- Upstream: 05 (ADR-01 a ADR-10, `contratos-rest_v1.0.md`, `modelo-datos-logico_v1.0.md`, `arquitectura-solucion_v1.0.md`); 02 (CU, RN, RC); 01 (NB).
- Downstream: 07 (secuencia de sprints), 08 (acceptance y contract tests).

## 5. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Backlog técnico inicial de geovial-api: 8 épicas técnicas y 21 BT (BT-01 a BT-21) inline, con tipo, prioridad, estimación Fibonacci, fuente upstream (ADR, CU, contrato, intake), dependencias, criterios de aceptación y matriz de trazabilidad BT↔US↔CU. BT inline por estar por debajo del umbral de 30 (regla 06 §3.3). |
