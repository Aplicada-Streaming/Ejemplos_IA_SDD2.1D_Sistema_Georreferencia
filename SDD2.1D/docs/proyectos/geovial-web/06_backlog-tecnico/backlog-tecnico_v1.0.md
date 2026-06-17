# Backlog técnico — geovial-web

**Proyecto:** geovial-web
**Documento:** backlog-tecnico_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Scrum Master

Este documento es la vista del backlog desde la lente técnica del front. Organiza el trabajo en épicas técnicas (EP-T) y tareas técnicas (BT-XX) que soportan las historias del `product-backlog_v1.0.md`. Cada BT declara su fuente upstream (necesidad de negocio, caso de uso, decisión de arquitectura o componente de la solución) y al menos una US consumidora; las BT de infraestructura compartida que sostienen toda la superficie justifican su existencia en una decisión de arquitectura. Estimación en story points con técnica Fibonacci (1, 2, 3, 5, 8, 13), la misma del product backlog. Total: 14 BT (menos de 30), por lo que el modo es inline conforme a la regla 06 §3.3.

## 1. Épicas técnicas

Las épicas técnicas agrupan las BT por preocupación de construcción. Cada una declara objetivo, alcance, fuente upstream y las BT que contiene. Las épicas técnicas no coinciden una a una con las épicas funcionales del product backlog: una capacidad funcional puede apoyarse en varias épicas técnicas transversales (cimientos, sesión, errores) y a la inversa.

### EP-T1 — Cimientos de capas y cliente del contrato

Objetivo: establecer la separación de capas en el cliente (presentación, aplicación de UI, cliente del contrato) y el punto único de consumo del contrato del servicio de dominio, sobre el cual se construyen todas las capacidades.
Alcance: andamiaje de las tres capas con dependencia unidireccional hacia el núcleo de la aplicación de UI; puerto de acceso al dominio; adaptador que traduce intenciones en llamadas al contrato y normaliza respuestas; armazón del circuito interactivo de render del lado servidor.
Fuente upstream: ADR-01 (estilo de render del lado servidor con circuito interactivo), ADR-04 (separación de capas), componente Cliente del contrato y Orquestadores de interacción (arquitectura §3).
BT contenidas: BT-01, BT-02, BT-03.

### EP-T2 — Sesión, token y visibilidad por rol

Objetivo: sostener el ciclo de sesión con el token custodiado del lado servidor y el control de presentación que muestra solo pantallas y acciones del alcance del rol.
Alcance: servicio de sesión y token del lado servidor del circuito; obtención y descarte del token; control de visibilidad y acciones por rol; restricción del acceso a roles administradores con la excepción de la carga manual del agente.
Fuente upstream: ADR-03 (autenticación con token del lado servidor), RN-01 (visibilidad por rol), RN-03 (acceso restringido a administradores), RN-02 (conservación de autoría), CU-01.
BT contenidas: BT-04, BT-05.

### EP-T3 — Componente de mapa, marcadores y carrusel

Objetivo: integrar el componente de mapa de terceros y construir el carrusel encadenado de fotos, capacidades interactivas distintivas del front.
Alcance: adaptador del componente de mapa (crear, mover y centrar marcadores) sincronizado con el estado de la aplicación de UI; componente de carrusel que encadena las fotos de marcadores contiguos y soporta ampliar, comentar, etiquetar y filtrar; control de habilitación de la edición del mapa según el estado del relevamiento.
Fuente upstream: ADR-01 (empuje de estado del servidor para mapa y carrusel), componentes Adaptador del componente de mapa, Vista de mapa y marcadores y Componente de carrusel de fotos (arquitectura §3), CU-05, CU-06, RN-04.
BT contenidas: BT-06, BT-07, BT-08.

### EP-T4 — Ciclo de estado y resolución de conflictos

Objetivo: materializar el control de habilitación por estado del relevamiento y la pantalla de resolución de conflictos, con el cierre condicionado a la ausencia de conflictos pendientes.
Alcance: control de habilitación por estado que habilita solo acciones válidas para el estado vigente; pantalla de resolución que presenta los conflictos pendientes y ofrece unificar o separar; bloqueo del cierre con conflictos pendientes y derivación a la resolución.
Fuente upstream: componentes Control de habilitación por estado, Vista de resolución de conflictos y Vista de ciclo del relevamiento (arquitectura §3), RN-04 (estados y habilitación), RN-05 (resolución como precondición del cierre), CU-07, CU-08.
BT contenidas: BT-09, BT-10.

### EP-T5 — Manejo de errores y feedback de UI

Objetivo: centralizar la traducción de los errores del contrato a feedback comprensible, por código estable, sin filtrar detalles del backend.
Alcance: normalización del error del contrato a una forma interna en el cliente del contrato; mapeador de errores a estado de pantalla y mensaje, alineado con el catálogo de mensajes de UX; feedback específico para validación, autorización y alcance, estado y ciclo, versión del contrato y error genérico.
Fuente upstream: ADR-05 (mapeo de errores a feedback), componente Mapeador de errores a feedback de UI (arquitectura §3, §7), RN-01, RN-03, RN-04, RN-05, catálogo de mensajes de 03.
BT contenidas: BT-11.

### EP-T6 — Formularios, validación y carga de archivos

Objetivo: dotar al front de un patrón consistente de formularios con validación de entrada y de la carga asincrónica de archivos para la carga manual y la portabilidad.
Alcance: patrón de formulario con validación de entrada de pantalla y resalte de campos sin perder lo ingresado; componente de carga asincrónica de archivos que reporta progreso sin bloquear el circuito; carga manual de fotos con radio de agrupación y exportación e importación como operaciones de larga duración.
Fuente upstream: componentes Orquestadores de interacción y Vista de carga manual y Vista de portabilidad (arquitectura §3), vista de procesos §4 (interacción de larga duración), CU-09, CU-10, CU-11, RN-04.
BT contenidas: BT-12, BT-13, BT-14.

## 2. BT por épica

Tipos posibles: feature, spike, refactor, devops, docs. Un spike implica caja temporal explícita. Prioridad alineada con la prioridad de las US que la consumen.

### EP-T1 — Cimientos de capas y cliente del contrato

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-01 | Andamiaje de las tres capas con dependencia unidireccional | feature | Must | 5 SP (Fibonacci) | ADR-01, ADR-04 | — | La presentación depende de la aplicación de UI y esta del puerto de acceso al dominio; ninguna vista accede a la red; una prueba de arquitectura verifica la dirección de la dependencia |
| BT-02 | Cliente del contrato con puerto de acceso al dominio y normalización de respuestas | feature | Must | 8 SP (Fibonacci) | ADR-04, componente Cliente del contrato (arquitectura §3) | BT-01 | El consumo del contrato queda confinado a un único componente; las respuestas se normalizan a representaciones internas; el puerto se prueba con dobles sin levantar la red |
| BT-03 | Armazón del circuito interactivo de render del lado servidor y estado de UI efímero | feature | Must | 5 SP (Fibonacci) | ADR-01, ADR-02, vista de procesos (arquitectura §4) | BT-01 | El circuito mantiene estado de UI y de sesión por usuario; la pérdida del circuito reconstruye la pantalla consultando al servicio de dominio sin pérdida de dato; no hay persistencia durable en el front |

### EP-T2 — Sesión, token y visibilidad por rol

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-04 | Servicio de sesión y token custodiado del lado servidor | feature | Must | 5 SP (Fibonacci) | ADR-03, CU-01, RN-03 | BT-02, BT-03 | El token se obtiene presentando credenciales y se retiene del lado servidor del circuito; nunca se serializa al navegador; el cierre descarta token y estado del circuito; una prueba verifica cero exposiciones del token |
| BT-05 | Control de visibilidad y acciones por rol jerárquico | feature | Must | 3 SP (Fibonacci) | RN-01, RN-03, RN-02, componente Control de visibilidad por rol (arquitectura §3) | BT-04 | El front muestra solo pantallas y acciones del alcance del rol; una acción fuera de alcance queda oculta o deshabilitada y, si se fuerza, es rechazada y mapeada a feedback; la autoría visible se conserva en la presentación |

### EP-T3 — Componente de mapa, marcadores y carrusel

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-06 | Spike de integración del componente de mapa de terceros | spike | Must | 3 SP (Fibonacci), caja temporal de 2 días | ADR-01, componente Adaptador del componente de mapa (arquitectura §3), CU-05 | BT-01 | Existe un informe con la integración del componente de mapa (crear, mover y centrar marcadores) y su sincronización con el estado de la aplicación de UI; si al cierre del plazo no hay camino claro, se documenta el bloqueo y se eleva al arquitecto |
| BT-07 | Adaptador del componente de mapa con marcadores e identidad estable | feature | Must | 8 SP (Fibonacci) | CU-05, RN-04, componente Vista de mapa y marcadores (arquitectura §3) | BT-06, BT-02 | El mapa crea y mueve marcadores conservando su identidad; la edición se habilita solo en estado de recolección y queda en solo lectura en otros estados; los marcadores en conflicto conviven y se muestran accesibles |
| BT-08 | Componente de carrusel encadenado de fotos con filtro por etiqueta | feature | Must | 8 SP (Fibonacci) | CU-06, componente Componente de carrusel de fotos (arquitectura §3) | BT-07 | El carrusel encadena las fotos de marcadores contiguos sin cerrarse; soporta ampliar, comentar y etiquetar; el filtro por etiqueta muestra solo coincidencias e informa cuando no hay; un marcador sin fotos se informa y ofrece pasar al contiguo |

### EP-T4 — Ciclo de estado y resolución de conflictos

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-09 | Control de habilitación de acciones por estado del relevamiento | feature | Must | 5 SP (Fibonacci) | RN-04, componente Control de habilitación por estado (arquitectura §3), CU-08 | BT-02, BT-05 | El front habilita solo las acciones válidas para el estado vigente y presenta el resto en solo lectura; las transiciones inválidas no se ofrecen; el estado se consulta al servicio de dominio que es la fuente de verdad |
| BT-10 | Pantalla de resolución de conflictos y cierre condicionado | feature | Must | 8 SP (Fibonacci) | RN-05, CU-07, CU-08, componentes Vista de resolución de conflictos y Vista de ciclo del relevamiento (arquitectura §3) | BT-08, BT-09 | La pantalla presenta los conflictos pendientes y ofrece unificar o separar; la unificación funde la evidencia y la unión de etiquetas; el cierre se bloquea con conflictos pendientes y deriva a la resolución; reabrir y reunir aplica la nueva decisión |

### EP-T5 — Manejo de errores y feedback de UI

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-11 | Mapeador de errores del contrato a feedback de UI por código estable | feature | Must | 5 SP (Fibonacci) | ADR-05, componente Mapeador de errores (arquitectura §3, §7), RN-01, RN-03, RN-04, RN-05, catálogo de mensajes de 03 | BT-02 | Cada código estable relevante mapea a un estado de pantalla y un mensaje del catálogo, o cae explícitamente en el feedback genérico; la validación resalta campos sin perder lo ingresado; el cierre con conflictos pendientes presenta el bloqueo; un error interno se presenta como fallo genérico sin filtrar detalles |

### EP-T6 — Formularios, validación y carga de archivos

| BT | Título | Tipo | Prioridad | Estimación | Fuente | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-12 | Patrón de formulario con validación de entrada de pantalla | feature | Must | 5 SP (Fibonacci) | componente Orquestadores de interacción (arquitectura §3), CU-02, CU-03, CU-04, RN-04 | BT-01, BT-11 | Los formularios validan la entrada antes de enviar y resaltan los campos con error sin perder lo ingresado; el duplicado de identificador y el tramo vacío se informan y mantienen el formulario para corregir; la entrada válida se delega al puerto de acceso al dominio |
| BT-13 | Carga asincrónica de fotos con radio de agrupación | feature | Must | 8 SP (Fibonacci) | CU-09, RN-04, vista de procesos §4 (interacción de larga duración), componente Vista de carga manual (arquitectura §3) | BT-07, BT-12 | La carga de varias fotos se ejecuta de forma asincrónica reportando progreso sin bloquear el circuito; las fotos se agrupan en marcadores según el radio definido; sin radio definido la carga se bloquea e informa; una foto sin ubicación queda pendiente de ubicación manual |
| BT-14 | Operaciones de larga duración de exportación e importación | feature | Could | 5 SP (Fibonacci) | CU-10, CU-11, vista de procesos §4, componentes Vista de portabilidad y Vista de configuración de almacenamiento (arquitectura §3) | BT-12, BT-11 | La exportación produce una unidad transferible única reportando progreso sin bloquear el circuito; la importación reconstruye el relevamiento y una unidad dañada se rechaza sin crear un relevamiento parcial; la configuración del destino se ofrece solo al rol raíz y un proveedor no disponible se mapea a feedback |

## 3. Trazabilidad BT↔US↔CU

Para cada BT se identifican las US que la consumen y los CU upstream. Las BT de cimientos (BT-01, BT-02, BT-03) son infraestructura compartida que sostiene toda la superficie del front, justificadas en ADR-01, ADR-02 y ADR-04; consumen además US concretas que las ejercitan primero.

| BT | Título | US consumidoras | CU upstream | Fuente upstream principal |
| --- | --- | --- | --- | --- |
| BT-01 | Andamiaje de las tres capas | US-01 a US-18 (infraestructura compartida) | CU-01 a CU-11 | ADR-01, ADR-04 |
| BT-02 | Cliente del contrato y puerto de acceso al dominio | US-01 a US-18 (infraestructura compartida) | CU-01 a CU-11 | ADR-04 |
| BT-03 | Armazón del circuito interactivo y estado efímero | US-01, US-02, US-10 (infraestructura compartida) | CU-01, CU-06 | ADR-01, ADR-02 |
| BT-04 | Servicio de sesión y token | US-01, US-02 | CU-01 | ADR-03 |
| BT-05 | Control de visibilidad por rol | US-01, US-03, US-04, US-08, US-18 | CU-01, CU-02, CU-04, CU-11 | RN-01, RN-03 |
| BT-06 | Spike de integración del componente de mapa | US-07 | CU-05 | ADR-01 |
| BT-07 | Adaptador del mapa con marcadores | US-07, US-10, US-15 | CU-05, CU-06, CU-09 | CU-05, RN-04 |
| BT-08 | Carrusel encadenado con filtro por etiqueta | US-10, US-11 | CU-06 | CU-06 |
| BT-09 | Habilitación de acciones por estado | US-05, US-06, US-08, US-13 | CU-03, CU-04, CU-08 | RN-04 |
| BT-10 | Resolución de conflictos y cierre condicionado | US-12, US-13, US-14 | CU-07, CU-08 | RN-05 |
| BT-11 | Mapeador de errores a feedback | US-01, US-03, US-05, US-12, US-14, US-15, US-18 | CU-01, CU-02, CU-03, CU-07, CU-08, CU-09, CU-11 | ADR-05 |
| BT-12 | Patrón de formulario con validación | US-03, US-04, US-05, US-06, US-08, US-16 | CU-02, CU-03, CU-04, CU-09 | RN-04 |
| BT-13 | Carga asincrónica de fotos con radio | US-15, US-16 | CU-09 | CU-09 |
| BT-14 | Exportación, importación y configuración | US-17, US-18 | CU-10, CU-11 | CU-10, CU-11 |

Cobertura: las 14 BT trazan a al menos una US consumidora y a una fuente upstream (NB, CU, ADR o componente de arquitectura). Las 18 US del product backlog tienen al menos una BT que las soporta: las US de acceso (US-01, US-02) por BT-03, BT-04, BT-11; las de administración (US-03, US-04) por BT-05, BT-12, BT-11; las de gestión (US-05, US-06) por BT-09, BT-12, BT-11; los marcadores (US-07) por BT-06, BT-07; la asignación (US-08, US-09) por BT-05, BT-09, BT-12; la revisión (US-10, US-11) por BT-07, BT-08; la resolución y el cierre (US-12, US-13, US-14) por BT-09, BT-10, BT-11; la carga manual (US-15, US-16) por BT-07, BT-12, BT-13; y la portabilidad y configuración (US-17, US-18) por BT-14, BT-05, BT-11. No hay BT huérfana ni US sin soporte técnico.

## 4. Vinculación cross-doc

- Este backlog técnico es la contraparte del `product-backlog_v1.0.md`: las US viven allí y las BT que las soportan, aquí; la matriz §3 mantiene el cruce BT↔US↔CU.
- Upstream: cada BT referencia una necesidad de negocio, un caso de uso, una decisión de arquitectura (ADR-01 a ADR-05) o un componente de la arquitectura de solución (05).
- Downstream: cada BT alimenta el sprint plan (07) por su estimación y dependencias, y los acceptance tests y pruebas de componente (08) por sus criterios de aceptación.
- La `definition-of-ready_v1.0.md` define cuándo una BT está lista para entrar a Sprint Planning.

## 5. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Backlog técnico inicial de geovial-web: 6 épicas técnicas (EP-T1 a EP-T6) y 14 tareas técnicas (BT-01 a BT-14) con tipo, prioridad, estimación Fibonacci, fuente upstream (ADR-01 a ADR-05, componentes de arquitectura, CU y RN) y dependencias; matriz de trazabilidad BT↔US↔CU completa. Supera el piso de 8 BT del tipo web-monolith (regla 06 §2.2). Modo inline (14 BT < 30). |
