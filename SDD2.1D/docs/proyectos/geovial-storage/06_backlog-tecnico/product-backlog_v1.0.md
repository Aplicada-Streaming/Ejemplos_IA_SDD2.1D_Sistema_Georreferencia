# Product backlog — geovial-storage

**Proyecto:** geovial-storage
**Documento:** product-backlog_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + Backlog Curator
**Tipo (D8):** library
**Técnica de estimación:** Fibonacci (1, 2, 3, 5, 8, 13)

## 1. Objetivos del producto

El backlog construye la abstracción de almacenamiento de `geovial-storage`: una superficie pública estable que permite al backend consumidor guardar, recuperar, eliminar, verificar y listar archivos sin atarse al destino físico, con proveedores intercambiables (local / remoto / otro) seleccionables por el usuario raíz. El MVP es el contrato de operaciones de datos sobre al menos dos proveedores con transparencia total (RN-01) e integridad byte a byte del contenido (RN-02); la configuración segura del proveedor activo (RN-03) y el punto de extensión completan la propuesta de valor.

El MVP queda definido por las historias Must Have: cubre las cinco operaciones de datos del contrato (CU-01 a CU-05), la activación del proveedor activo (CU-06) y la verificación de transparencia sobre dos proveedores soportados, que es el criterio de éxito central de NB-07.

## 2. Épicas

Las épicas se organizan según la variante `library` (regla 06 §1.2): por superficie de Abstractions y por capacidad del motor interno, más un eje por proveedor.

| EP | Nombre | Descripción | Sprints estimados |
| --- | --- | --- | --- |
| EP-01 | Superficie de almacenamiento (operaciones de datos) | Operaciones públicas del contrato que el consumidor invoca: guardar, recuperar, eliminar, verificar y listar (CU-01 a CU-05) | 2 |
| EP-02 | Configuración del proveedor activo | Selección, validación y activación del proveedor por el usuario raíz, con resguardo de credenciales (CU-06) | 1 |
| EP-03 | Proveedores intercambiables y punto de extensión | Adaptadores de proveedor (local y de objetos remoto) y el puerto de extensión para proveedores nuevos | 2 |
| EP-04 | Fundaciones del contrato y verificación de transparencia | Capa de Abstracciones, núcleo de enrutado y validación, catálogo de errores uniforme, batería de contrato única y gate de cobertura | 2 |

## 3. Historias por épica

Las nueve historias provienen de la matriz NB→CU→RN→US de la especificación funcional (02 §5). La numeración US-01 a US-09 conserva la prevista por los CU. Estimación en Fibonacci.

| US | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-01 | Guardar un archivo y obtener su identificador lógico | Must | 5 | Borrador | CU-01 | EP-01 |
| US-02 | Controlar la colisión de identificadores al guardar (duplicado y sobrescritura) | Should | 3 | Borrador | CU-01 | EP-01 |
| US-03 | Recuperar un archivo idéntico al guardado | Must | 5 | Borrador | CU-02 | EP-01 |
| US-04 | Recuperar metadatos y segmentos por rango sin filtrar credenciales | Should | 3 | Borrador | CU-02 | EP-01 |
| US-05 | Eliminar un archivo de forma idempotente y por prefijo | Must | 3 | Borrador | CU-03 | EP-01 |
| US-06 | Verificar la existencia de un archivo sin transferir contenido | Must | 2 | Borrador | CU-04 | EP-01 |
| US-07 | Listar archivos bajo un prefijo con paginación por testigo | Should | 5 | Borrador | CU-05 | EP-01 |
| US-08 | Activar un proveedor de almacenamiento como usuario raíz | Must | 8 | Borrador | CU-06 | EP-02 |
| US-09 | Validar una configuración de proveedor sin activarla (prueba en seco) | Could | 3 | Borrador | CU-06 | EP-02 |

### US-01 — Guardar un archivo y obtener su identificador lógico

**Épica:** EP-01 | **MoSCoW:** Must | **Estimación:** 5 SP (Fibonacci) | **Estado:** Borrador
**CU relacionados:** CU-01 | **NB upstream:** NB-07 (principal), NB-03 (soporte) | **RN:** RN-01, RN-02

Como backend consumidor de GeoVial, quiero guardar el contenido de un archivo en el proveedor activo y recibir un identificador lógico estable, para persistir las fotografías de un relevamiento sin conocer dónde quedan alojadas físicamente.

Criterios de aceptación (Given/When/Then):

- Given un proveedor activo configurado y un contenido de 245 KB con destino `relevamientos/2026/r-001/` y tipo `image/jpeg`, When el consumidor invoca guardar, Then la librería devuelve un identificador lógico no vacío y un tamaño persistido de 245 KB.
- Given un contenido de 0 bytes con destino válido, When el consumidor invoca guardar, Then la librería rechaza con el código CONTENIDO_VACIO y no crea ningún archivo.
- Given un contenido que excede el tamaño máximo configurado, When el consumidor invoca guardar, Then la librería rechaza con TAMANIO_EXCEDIDO sin contactar al proveedor.

INVEST check:

| Atributo | Verificación |
| --- | --- |
| Independent | Solo depende de las fundaciones del contrato (EP-04); planificable sin esperar a otra US de operación |
| Negotiable | El alcance de metadatos admite ajuste en refinamiento |
| Valuable | El valor está en persistir evidencia y obtener un identificador recuperable |
| Estimable | El contrato de la operación está fijado en `contratos-abstractions_v1.0.md` |
| Small | Una operación del contrato; cabe en un sprint |
| Testable | Criterios verificables con guardado, contenido vacío y límite de tamaño |

BT derivadas: BT-01, BT-02, BT-04, BT-05, BT-11, BT-13.

### US-02 — Controlar la colisión de identificadores al guardar (duplicado y sobrescritura)

**Épica:** EP-01 | **MoSCoW:** Should | **Estimación:** 3 SP (Fibonacci) | **Estado:** Borrador
**CU relacionados:** CU-01 | **NB upstream:** NB-07 (principal) | **RN:** RN-01, RN-02

Como backend consumidor de GeoVial, quiero que el guardado con un identificador explícito rechace la colisión salvo que pida sobrescribir, para no pisar evidencia ya almacenada por accidente y, cuando corresponda, reemplazarla de forma deliberada.

Criterios de aceptación (Given/When/Then):

- Given un identificador lógico `relevamientos/2026/r-001/foto-01.jpg` que ya existe, sin marca de sobrescritura, When el consumidor invoca guardar con ese identificador, Then la librería rechaza con IDENTIFICADOR_DUPLICADO y conserva el archivo preexistente.
- Given el mismo identificador existente con la marca de sobrescritura activada, When el consumidor invoca guardar con ese identificador, Then la librería reemplaza el contenido y devuelve el mismo identificador.
- Given un identificador lógico explícito que aún no existe, When el consumidor invoca guardar con ese identificador, Then la librería persiste el contenido y devuelve ese mismo identificador.

INVEST check:

| Atributo | Verificación |
| --- | --- |
| Independent | Construye sobre US-01 pero se planifica y prueba aparte (rama de colisión) |
| Negotiable | La política de sobrescritura admite refinamiento sobre el modo por defecto |
| Valuable | Evita pisar evidencia y habilita el reemplazo deliberado |
| Estimable | Flujos alternativos FA-01 y FA-02 de CU-01 ya especificados |
| Small | Variante de una sola operación |
| Testable | Tres escenarios con resultado objetivo |

BT derivadas: BT-01, BT-02.

### US-03 — Recuperar un archivo idéntico al guardado

**Épica:** EP-01 | **MoSCoW:** Must | **Estimación:** 5 SP (Fibonacci) | **Estado:** Borrador
**CU relacionados:** CU-02 | **NB upstream:** NB-07 (principal), NB-06 (soporte) | **RN:** RN-01, RN-02, RN-03

Como backend consumidor de GeoVial, quiero recuperar el contenido de un archivo a partir de su identificador lógico y recibirlo idénticamente igual al guardado, para que la evidencia fotográfica conserve su valor probatorio sin importar el proveedor activo.

Criterios de aceptación (Given/When/Then):

- Given un archivo previamente guardado con identificador `relevamientos/2026/r-001/foto-01.jpg` de 245 KB y tipo `image/jpeg`, When el consumidor invoca recuperar con ese identificador, Then la librería devuelve un contenido de 245 KB idénticamente igual al guardado y el tipo `image/jpeg`.
- Given un identificador `relevamientos/2026/r-001/inexistente.jpg` que nunca fue guardado, When el consumidor invoca recuperar con ese identificador, Then la librería devuelve IDENTIFICADOR_INEXISTENTE sin contenido.
- Given un proveedor activo que no responde, When el consumidor invoca recuperar un identificador válido, Then la librería devuelve PROVEEDOR_NO_DISPONIBLE sin exponer las credenciales del proveedor.

INVEST check:

| Atributo | Verificación |
| --- | --- |
| Independent | Para probar la ida y vuelta usa US-01, pero la operación de lectura es planificable por separado |
| Negotiable | El alcance del modo solo-metadatos y rango se separa en US-04 |
| Valuable | La integridad byte a byte es la propiedad central de la evidencia (RN-02) |
| Estimable | Contrato de la operación de recuperación fijado en 05 |
| Small | Una operación del contrato |
| Testable | Igualdad binaria, inexistente y proveedor no disponible son verificables |

BT derivadas: BT-03, BT-04, BT-05, BT-11, BT-12, BT-13.

### US-04 — Recuperar metadatos y segmentos por rango sin filtrar credenciales

**Épica:** EP-01 | **MoSCoW:** Should | **Estimación:** 3 SP (Fibonacci) | **Estado:** Borrador
**CU relacionados:** CU-02 | **NB upstream:** NB-07 (principal), NB-06 (soporte) | **RN:** RN-02, RN-03

Como backend consumidor de GeoVial, quiero recuperar solo los metadatos o un rango de bytes de un archivo, para inspeccionar o transferir por tramos grandes volúmenes de evidencia sin materializar el contenido completo en memoria.

Criterios de aceptación (Given/When/Then):

- Given un archivo guardado de 245 KB y una solicitud del rango de bytes 0 a 1023, When el consumidor invoca recuperar por rango, Then la librería devuelve exactamente 1024 bytes correspondientes al inicio del archivo.
- Given un archivo guardado de 245 KB y una solicitud de rango que excede su tamaño, When el consumidor invoca recuperar por rango, Then la librería rechaza con RANGO_INVALIDO.
- Given un archivo guardado y una solicitud en modo solo-metadatos, When el consumidor invoca recuperar, Then la librería devuelve el tipo de contenido y el tamaño sin transferir el binario.

INVEST check:

| Atributo | Verificación |
| --- | --- |
| Independent | Extiende la operación de US-03 pero es una rama separable (rango/metadatos) |
| Negotiable | El detalle de transferencia por tramos admite ajuste en refinamiento |
| Valuable | Permite no materializar contenidos completos y sostener la latencia |
| Estimable | FA-01 y FA-02 de CU-02 y el contrato fijan el alcance |
| Small | Variantes de una operación ya existente |
| Testable | Rango válido, rango inválido y solo-metadatos son verificables |

BT derivadas: BT-03, BT-12.

### US-05 — Eliminar un archivo de forma idempotente y por prefijo

**Épica:** EP-01 | **MoSCoW:** Must | **Estimación:** 3 SP (Fibonacci) | **Estado:** Borrador
**CU relacionados:** CU-03 | **NB upstream:** NB-07 (principal) | **RN:** RN-01

Como backend consumidor de GeoVial, quiero eliminar un archivo por su identificador o todos los archivos bajo un prefijo, tratando como éxito la eliminación de algo inexistente, para liberar identificadores y limpiar relevamientos sin escribir lógica de control de estado en el consumidor.

Criterios de aceptación (Given/When/Then):

- Given un archivo guardado con identificador `relevamientos/2026/r-001/foto-01.jpg`, When el consumidor invoca eliminar con ese identificador, Then la librería confirma la eliminación y la verificación posterior reporta el identificador como inexistente.
- Given un identificador `relevamientos/2026/r-001/inexistente.jpg` que no existe, When el consumidor invoca eliminar con ese identificador, Then la librería informa éxito por idempotencia, sin error.
- Given tres archivos bajo el prefijo `relevamientos/2026/r-001/`, When el consumidor invoca eliminar bajo ese prefijo, Then la librería devuelve una cantidad eliminada de 3.

INVEST check:

| Atributo | Verificación |
| --- | --- |
| Independent | Operación autónoma; usa US-01 solo para preparar datos de prueba |
| Negotiable | El comportamiento ante eliminación parcial admite refinamiento |
| Valuable | Libera identificadores y limpia relevamientos sin lógica en el consumidor |
| Estimable | CU-03 con FA-01 (idempotencia) y FA-02 (prefijo) especificados |
| Small | Una operación del contrato |
| Testable | Eliminación efectiva, idempotencia y borrado múltiple verificables |

BT derivadas: BT-03, BT-04, BT-05, BT-11.

### US-06 — Verificar la existencia de un archivo sin transferir contenido

**Épica:** EP-01 | **MoSCoW:** Must | **Estimación:** 2 SP (Fibonacci) | **Estado:** Borrador
**CU relacionados:** CU-04 | **NB upstream:** NB-07 (principal), NB-03 (soporte) | **RN:** RN-01, RN-02

Como backend consumidor de GeoVial, quiero consultar si un identificador corresponde a un archivo presente sin transferir su contenido, para decidir flujos de guardado, recuperación o eliminación con bajo costo y latencia.

Criterios de aceptación (Given/When/Then):

- Given un archivo guardado con identificador `relevamientos/2026/r-001/foto-01.jpg` de 245 KB, When el consumidor invoca verificar con ese identificador, Then la librería devuelve presencia verdadera y un tamaño de 245 KB.
- Given un identificador `relevamientos/2026/r-001/inexistente.jpg` que no existe, When el consumidor invoca verificar con ese identificador, Then la librería devuelve presencia falsa.
- Given un identificador que fue eliminado por la operación de eliminación, When el consumidor invoca verificar con ese identificador, Then la librería devuelve presencia falsa de forma coherente con el estado real (RN-02).

INVEST check:

| Atributo | Verificación |
| --- | --- |
| Independent | Operación de solo lectura; planificable de forma autónoma |
| Negotiable | La devolución opcional de metadatos admite ajuste |
| Valuable | Permite decidir flujos con menor costo que la recuperación |
| Estimable | CU-04 con un único flujo alternativo, alcance acotado |
| Small | La operación más pequeña del contrato |
| Testable | Presencia verdadera, falsa y coherencia tras eliminar son verificables |

BT derivadas: BT-03, BT-04, BT-11.

### US-07 — Listar archivos bajo un prefijo con paginación por testigo

**Épica:** EP-01 | **MoSCoW:** Should | **Estimación:** 5 SP (Fibonacci) | **Estado:** Borrador
**CU relacionados:** CU-05 | **NB upstream:** NB-07 (principal), NB-06 (soporte) | **RN:** RN-01, RN-03

Como backend consumidor de GeoVial, quiero enumerar los identificadores presentes bajo un prefijo con paginación por testigo de continuación, para recorrer todos los archivos de un relevamiento sin conocer de antemano sus identificadores ni materializar listados completos.

Criterios de aceptación (Given/When/Then):

- Given tres archivos guardados bajo el prefijo `relevamientos/2026/r-001/`, When el consumidor invoca listar con ese prefijo, Then la librería devuelve los tres identificadores y ningún testigo de continuación.
- Given diez archivos bajo el prefijo `relevamientos/2026/r-002/` y un tamaño de página de 4, When el consumidor invoca listar con ese prefijo y ese tamaño de página, Then la librería devuelve 4 identificadores y un testigo de continuación no vacío.
- Given ningún archivo bajo el prefijo `relevamientos/2026/r-999/`, When el consumidor invoca listar con ese prefijo, Then la librería devuelve una lista vacía sin error.

INVEST check:

| Atributo | Verificación |
| --- | --- |
| Independent | Operación de solo lectura; usa US-01 para preparar datos de prueba |
| Negotiable | El tamaño de página por defecto y el manejo de testigo vencido admiten refinamiento |
| Valuable | Permite recorrer un relevamiento completo sin conocer los identificadores |
| Estimable | CU-05 con FA-01 (paginación) y FA-02 (sin coincidencias) especificados |
| Small | Una operación del contrato con paginación acotada |
| Testable | Listado completo, paginación con testigo y prefijo vacío verificables |

BT derivadas: BT-03, BT-04, BT-11.

### US-08 — Activar un proveedor de almacenamiento como usuario raíz

**Épica:** EP-02 | **MoSCoW:** Must | **Estimación:** 8 SP (Fibonacci) | **Estado:** Borrador
**CU relacionados:** CU-06 | **NB upstream:** NB-07 (principal) | **RN:** RN-01, RN-03

Como usuario raíz, quiero seleccionar el proveedor de almacenamiento activo entregando sus credenciales y validándolas, para controlar dónde se aloja la evidencia sin que el consumidor cambie su forma de invocar las operaciones (RN-01) y sin exponer las credenciales (RN-03).

Criterios de aceptación (Given/When/Then):

- Given que el proveedor activo es el local y el usuario raíz entrega parámetros válidos de un proveedor de objetos remoto con credenciales correctas, When el usuario raíz activa el proveedor remoto, Then la librería confirma el cambio y las operaciones siguientes usan el proveedor remoto sin cambiar su forma de invocación.
- Given que el usuario raíz entrega credenciales con formato inválido para un proveedor remoto, When el usuario raíz intenta activar ese proveedor, Then la librería rechaza con CREDENCIALES_INVALIDAS y mantiene el proveedor local como activo.
- Given un actor sin alcance de usuario raíz, When ese actor intenta cambiar el proveedor activo, Then la librería rechaza con AUTORIZACION_INSUFICIENTE y no modifica el proveedor activo.

INVEST check:

| Atributo | Verificación |
| --- | --- |
| Independent | Depende de las fundaciones (EP-04) y del registro/resguardo, no de otra US de configuración |
| Negotiable | El conjunto inicial de proveedores soportados admite ajuste en refinamiento |
| Valuable | Da al negocio control del destino, propósito central de NB-07 |
| Estimable | CU-06 con cuatro escenarios y errores ya catalogados |
| Small | Mayor de las US (8 SP) pero acotada a una transición de configuración en un sprint |
| Testable | Cambio efectivo, credenciales inválidas y autorización insuficiente verificables |

BT derivadas: BT-06, BT-07, BT-08, BT-09, BT-11, BT-13.

### US-09 — Validar una configuración de proveedor sin activarla (prueba en seco)

**Épica:** EP-02 | **MoSCoW:** Could | **Estimación:** 3 SP (Fibonacci) | **Estado:** Borrador
**CU relacionados:** CU-06 | **NB upstream:** NB-07 (principal) | **RN:** RN-03

Como usuario raíz, quiero validar una configuración de proveedor sin fijarla como activa, para comprobar credenciales y conectividad antes de comprometer el cambio de destino y sin interrumpir la operación vigente.

Criterios de aceptación (Given/When/Then):

- Given que el usuario raíz entrega parámetros y credenciales válidos de un proveedor remoto en modo validación-en-seco, When el usuario raíz solicita validar sin activar, Then la librería reporta validación satisfactoria y conserva el proveedor activo anterior.
- Given que el usuario raíz entrega credenciales con formato válido pero el proveedor remoto rechaza la conexión, en modo validación-en-seco, When el usuario raíz solicita validar sin activar, Then la librería reporta PROVEEDOR_INACCESIBLE y conserva el proveedor activo anterior.

INVEST check:

| Atributo | Verificación |
| --- | --- |
| Independent | Reutiliza los pasos 2 a 4 de CU-06 sin fijar estado; separable de US-08 |
| Negotiable | El formato del reporte de validación admite ajuste |
| Valuable | Permite comprobar sin riesgo antes de comprometer el cambio |
| Estimable | FA-02 de CU-06 ya especificado |
| Small | Variante sin efecto de estado de una operación existente |
| Testable | Validación satisfactoria e inaccesible son verificables |

BT derivadas: BT-06, BT-07.

## 4. Métricas de avance

Distribución por prioridad MoSCoW (las nueve US, 37 SP totales):

| Prioridad | Cantidad de US | SP | Porcentaje de US | Porcentaje de SP |
| --- | --- | --- | --- | --- |
| Must | 5 (US-01, US-03, US-05, US-06, US-08) | 23 | 56 % | 62 % |
| Should | 3 (US-02, US-04, US-07) | 11 | 33 % | 30 % |
| Could | 1 (US-09) | 3 | 11 % | 8 % |
| Won't (v1.0) | 0 | 0 | 0 % | 0 % |

- Distribución dentro del rango sugerido (06 §4.7): Must 56 % de las US (objetivo 50 a 60 %), Should 33 % (objetivo 20 a 30 %, ligeramente por encima por el bajo número total de historias), Could 11 %.
- Porcentaje cerrado: 0 % (todas en estado Borrador; ninguna US Done todavía).
- Deuda en backlog: ningún ítem Won't (v1.0) registrado. La migración de archivos al cambiar de proveedor y la subida por fragmentos quedaron fuera de alcance por decisión de la especificación funcional (02 §7 y CU-06 nota); no generan US en esta versión y se documentan como recorte upstream, no como deuda del backlog.

## 5. Refinamiento

- Cadencia: una sesión de refinement por sprint (mínimo para `library`, regla 06 §2.2), conducida por el Scrum Master + Backlog Curator (AG-06).
- Formato de estimación: Planning Poker con técnica Fibonacci (1, 2, 3, 5, 8, 13), declarada en la cabecera y mantenida en todo el backlog y en el backlog técnico.
- Participantes: el desarrollador del equipo (equipo_n=1), con revisiones acotadas de AG-02 (trazabilidad US↔CU), AG-05 (justificación de BT en ADR/componente/contrato) y AG-08 (verificabilidad de criterios para 08).
- Foco de la curaduría: que cada US Must y Should tenga criterios Given/When/Then con al menos dos escenarios, que ninguna US quede huérfana de CU y que la distribución MoSCoW no derive a 100 % Must.
- Entrada al sprint: solo ingresan al Sprint Planning de 07 las US que cumplen la Definition of Ready (`definition-of-ready_v1.0.md`).

## 6. Documentos relacionados

- Vista técnica: [backlog-tecnico_v1.0.md](backlog-tecnico_v1.0.md) — épicas técnicas, BT-01 a BT-13 y matriz BT↔US↔CU.
- Filtro de entrada: [definition-of-ready_v1.0.md](definition-of-ready_v1.0.md) — DoR para US y BT.
- Índice de la sección: [README.md](README.md).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Product backlog inicial de geovial-storage: cuatro épicas (superficie de almacenamiento, configuración del proveedor, proveedores intercambiables, fundaciones del contrato), nueve US inline (US-01 a US-09) con MoSCoW, SP en Fibonacci, trazabilidad a CU e INVEST, métricas de avance y política de refinement. |
