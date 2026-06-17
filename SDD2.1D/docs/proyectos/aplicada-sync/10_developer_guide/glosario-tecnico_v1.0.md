# Glosario técnico — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** glosario-tecnico_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-15
**Autor:** Technical Writer + SDK Documentation Lead (AG-10)
**Tipo Diátaxis:** Reference
**Audiencia:** Developer integrador que consume la librería desde su propia aplicación
**Nivel:** Básico
**Tiempo estimado de lectura:** 8 min

Vocabulario canónico del consumidor del motor de sincronización. Esta es la fuente única de los términos: el resto de los documentos de esta carpeta enlaza acá en lugar de redefinir. La columna de referencia cruzada indica dónde se profundiza cada término (concepto en `conceptos-fundamentales_v1.0.md`, tipo u operación en `referencia-api_v1.0.md`, o regla de origen en la categoría 05).

## 1. Términos del consumidor

| Término | Definición operativa | Referencia cross-doc |
| --- | --- | --- |
| `motor-de-sincronizacion` | Componente que el integrador incorpora a su aplicación host para propagar cambios locales a un backend remoto bajo la política subir-luego-bajar. Es agnóstico del dominio: no interpreta el contenido de los cambios. | `conceptos-fundamentales_v1.0.md` §1 (`concepto-motor`) |
| `host` | Aplicación que incorpora el motor y le provee los recursos que el motor no implementa: almacén local, transporte, credencial y fuente de conectividad. | `conceptos-fundamentales_v1.0.md` §5; 05 `arquitectura-solucion_v1.0.md` §5 |
| `sesion` | Contexto activo del motor para un host, creado al inicializar; conserva la cola, los metadatos y el estado a lo largo de los ciclos. | `referencia-api_v1.0.md` §2 (Inicializar sesión); `conceptos-fundamentales_v1.0.md` §1 (`concepto-sesion`) |
| `estado-de-sesion` | Situación de la sesión dentro de un conjunto cerrado: listo, no autenticada, sincronizando, reanudable. El conjunto forma parte del contrato. | `referencia-api_v1.0.md` §3 (Estado de la sesión); 05 `flujo-ejecucion_v1.0.md` §2 |
| `cola-local` | Registro persistente y ordenado de los cambios locales pendientes de subir; una sola entrada por identificador estable; conserva el orden de creación. | `conceptos-fundamentales_v1.0.md` §1 (`concepto-cola`); 05 ADR-04 |
| `cambio-local` | Unidad que el host encola para subir: identificador estable, operación, carga útil opaca y marca de orden de creación. | `referencia-api_v1.0.md` §3 (Cambio local); 05 `contratos-abstractions_v1.0.md` §4 |
| `identificador-de-cambio-estable` | Clave única que el host asigna a cada cambio; el motor la usa como clave de no duplicación en la cola y de reconocimiento en la subida; sostiene la idempotencia. | `conceptos-fundamentales_v1.0.md` §3 (decisión D-04); 05 ADR-07 |
| `carga-util-opaca` | Contenido de dominio del cambio que el motor transporta y persiste sin interpretar ni validar. | `conceptos-fundamentales_v1.0.md` §4; 05 `arquitectura-solucion_v1.0.md` §6 |
| `ciclo-de-sincronizacion` | Ejecución completa del pipeline de dos fases: primero subir todos los pendientes confirmables, luego bajar las actualizaciones. | `conceptos-fundamentales_v1.0.md` §1 (`concepto-ciclo`); 05 `flujo-ejecucion_v1.0.md` §3 |
| `fase-de-subida` | Primera fase del ciclo: envía los pendientes al backend en orden de creación y los retira de la cola al confirmarse. | `referencia-api_v1.0.md` §2 (Ejecutar sincronización); 05 `flujo-ejecucion_v1.0.md` §3 |
| `fase-de-bajada` | Segunda fase del ciclo: solicita y aplica las actualizaciones posteriores a la última marca de sincronización; solo arranca tras concluir la subida. | `referencia-api_v1.0.md` §2 (Ejecutar sincronización); 05 `flujo-ejecucion_v1.0.md` §3 |
| `orden-subir-antes-de-bajar` | Invariante dura del motor: ninguna bajada se aplica mientras queden pendientes confirmables. No es configurable. | `conceptos-fundamentales_v1.0.md` §3 (decisión D-02); 05 ADR-05 |
| `idempotencia` | Garantía de que un mismo cambio produce un único efecto neto sin importar cuántas veces se encole, reintente o reanude, apoyada en el identificador estable. | `conceptos-fundamentales_v1.0.md` §3 (decisión D-04); 05 ADR-07 |
| `reanudacion` | Continuación de una subida que un corte dejó parcial: reenvía solo los no confirmados y luego baja, sin pérdida ni duplicación. | `referencia-api_v1.0.md` §2 (Reanudar sincronización); 05 ADR-06 |
| `marca-de-progreso` | Metadato persistido que registra hasta dónde confirmó la subida en curso, para reanudar desde el punto de corte. | `conceptos-fundamentales_v1.0.md` §2; 05 `flujo-ejecucion_v1.0.md` §3 |
| `marca-de-ultima-sincronizacion` | Límite a partir del cual la fase de bajada solicita actualizaciones; avanza solo tras una bajada exitosa. | `referencia-api_v1.0.md` §3 (Estado de la sesión); 05 `arquitectura-solucion_v1.0.md` §6 |
| `disparo-automatico` | Modo en el que el motor ejecuta un ciclo al recibir un evento de recuperación de conectividad, sin invocación manual del host. | `referencia-api_v1.0.md` §2 (Habilitar disparo automático); 05 `flujo-ejecucion_v1.0.md` §4 |
| `elemento-en-conflicto` | Entidad que el backend marcó en conflicto; el motor la aplica como estado válido y la reporta sin abortar ni resolver. Condición reportada, no error de bloqueo. | `conceptos-fundamentales_v1.0.md` §3 (decisión D-05); 05 ADR-08 |
| `convivencia-con-conflicto` | Política del motor de aplicar, transportar y exponer un estado en conflicto sin bloquear el ciclo ni decidir la resolución. | `conceptos-fundamentales_v1.0.md` §3 (decisión D-05); 05 ADR-08 |
| `resumen-del-ciclo` | Salida de un ciclo: cantidad de subidos, cantidad de bajados, lista de elementos en conflicto y estado final. | `referencia-api_v1.0.md` §3 (Resumen del ciclo) |
| `resumen-de-reanudacion` | Salida de una reanudación: cambios efectivamente nuevos confirmados, cambios reconocidos como ya recibidos, bajados y estado final. | `referencia-api_v1.0.md` §3 (Resumen de reanudación) |
| `punto-de-extension` | Abstracción que el host implementa e inyecta para adaptar el motor a su entorno: almacén local, transporte, credencial o fuente de conectividad. | `conceptos-fundamentales_v1.0.md` §3 (decisión D-01); 05 `extensibilidad_v1.0.md` §3 |
| `estrategia-de-almacen-local` | Punto de extensión obligatorio que persiste y lee la cola y los metadatos en el almacén del host. | `referencia-api_v1.0.md` §4; 05 `extensibilidad_v1.0.md` §3 |
| `estrategia-de-transporte` | Punto de extensión obligatorio que envía un cambio por identificador estable y obtiene actualizaciones posteriores a una marca. | `referencia-api_v1.0.md` §4; 05 `extensibilidad_v1.0.md` §3 |
| `proveedor-de-credencial` | Punto de extensión opcional que entrega la credencial vigente del host al motor; su ausencia produce estado no autenticada. | `referencia-api_v1.0.md` §4; 05 `extensibilidad_v1.0.md` §3 |
| `fuente-de-conectividad` | Punto de extensión opcional que notifica transiciones de red para el disparo automático. | `referencia-api_v1.0.md` §4; 05 `extensibilidad_v1.0.md` §3 |
| `codigo-de-error-estable` | Identificador invariante de una condición que el motor reporta; no se traduce ni cambia entre versiones menores. | `troubleshooting_v1.0.md`; 03 `dx-error-messages_v1.0.md` §3 |
| `superficie-publica` | Conjunto versionado que forma el contrato del paquete: operaciones, formas de datos, conjunto de estados, garantía de orden y códigos de error. | `referencia-api_v1.0.md` §1; 05 `contratos-abstractions_v1.0.md` §6 |

## 2. Referencias cruzadas

- 05 `contratos-abstractions_v1.0.md` §3 y §4: operaciones y formas de datos que dan origen a los términos del contrato.
- 05 `extensibilidad_v1.0.md` §3: definición de los cuatro puntos de extensión.
- 05 ADR-04, ADR-05, ADR-06, ADR-07, ADR-08: decisiones que fijan la semántica de cola, orden, reanudación, idempotencia y conflicto.
- `conceptos-fundamentales_v1.0.md`: modelo mental que articula estos términos.
- `referencia-api_v1.0.md`: contrato exacto de las operaciones y tipos nombrados.

## 3. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Glosario canónico inicial del consumidor de aplicada-sync: 28 términos en kebab-case con definición operativa y referencia cruzada al concepto, al tipo de la referencia o a la decisión de 05. Fuente única del vocabulario para el resto de la categoría 10. |
