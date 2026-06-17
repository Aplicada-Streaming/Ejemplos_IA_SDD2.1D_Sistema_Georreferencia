# Glosario UX — geovial-web

**Proyecto:** geovial-web
**Documento:** glosario-ux_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** UX/UI Designer + Frontend Lead
**Variante:** UX/UI

## 1. Propósito y alcance

Vocabulario canónico de la sección 03 de `geovial-web`: términos de experiencia, interacción, estados y accesibilidad que aparecen en más de un artefacto de esta sección. No reemplaza ni redefine el glosario del dominio de 02 (relevamiento, tramo vial, observación, marcador geográfico, conflicto de marcadores, agente de campo, jefe de área, sincronización, radio de agrupación, etiqueta): esos términos se usan aquí con la misma semántica de 02 y solo se referencian. Este glosario agrega los términos propios de UX/UI que 02 no define.

## 2. Términos del dominio referenciados desde 02 (no se redefinen)

| Término | Referencia canónica |
| --- | --- |
| Relevamiento, tramo vial, observación, marcador geográfico, conflicto de marcadores, agente de campo, jefe de área, sincronización, radio de agrupación, etiqueta | Glosario del dominio en 00 (vision-producto §9) y en 02 (especificación funcional). Se usan con idéntica semántica. |
| Estado del ciclo (recolección, revisión, cierre) | Definido por el ciclo del relevamiento en 02 (RN-04). Aquí se trata su presentación, no su definición. |

## 3. Términos UX propios de la sección

| Término | Definición | Notas |
| --- | --- | --- |
| Superficie | Unidad de experiencia con estados propios: una pantalla, un panel, un modal con flujo o una vista. Es la unidad que un wireframe documenta. | Un wireframe por superficie (regla §3.2 de 03) |
| Pantalla | Superficie de página completa con su propia ruta de navegación (ingreso, panel de relevamientos, revisión, resolución y cierre). | — |
| Panel | Región persistente dentro de una pantalla (panel de evidencia, panel de navegación lateral). | — |
| Modal | Superficie superpuesta que concentra la atención en una tarea acotada (carrusel, confirmación de baja, confirmación de cierre). | Devuelve el foco al cerrarse; no atrapa el teclado |
| Carrusel encadenado | Componente que recorre las fotos de un marcador y continúa con las del marcador contiguo al llegar a un extremo. | Concepto centralizado en `representacion-carrusel-fotos_v1.0.md` |
| Comparador de marcadores | Disposición que muestra lado a lado los marcadores de un conflicto con su evidencia para decidir unificar o separar. | Superficie de CU-07 |
| Estado vacío | Estado de una superficie sin datos que mostrar todavía, con texto orientativo y acción siguiente. | No es un error; orienta al próximo paso |
| Estado cargando | Estado de espera de una operación asíncrona, representado con skeleton o spinner según el umbral percibido. | — |
| Estado con datos | Estado de render normal cuando el backend respondió con contenido. | — |
| Estado de error recuperable | Estado de falla con vía de recuperación visible (reintentar, corregir). | Banner o inline según el alcance |
| Sin conexión al circuito | Estado de degradación digna cuando el front no alcanza el backend o la sesión expiró; no es modo offline de trabajo. | El offline-first pertenece a la aplicación de campo, no al front |
| Solo lectura | Presentación de una superficie sin acciones de modificación, por estado del recurso o por alcance del rol. | Disparado por RN-04 fuera de la etapa válida |
| Skeleton | Silueta de la estructura esperada que se muestra mientras llega el contenido, para dar sensación de respuesta. | Técnica de performance percibida |
| Spinner | Indicador de espera indeterminada que se muestra solo si la espera supera el umbral percibido. | — |
| Banner | Mensaje de alcance de pantalla o de región, no bloqueante salvo el de sesión expirada. | Para errores recuperables y avisos de circuito |
| Mensaje inline | Mensaje ubicado junto al campo o control que lo origina (validación de formulario, foto no disponible). | — |
| Confirmación sutil | Feedback breve de que una acción se completó, con la próxima acción posible. | Para creación, asignación, resolución, transición y cierre |
| Modal de confirmación | Modal que pide ratificar una acción destructiva o de avance de ciclo antes de aplicarla (baja, cierre). | Confirmable y cancelable |
| Optimistic UI | Reflejo inmediato de una acción segura antes de la confirmación del backend, con refresco si el backend acota el resultado. | Se usa con cautela en el filtrado local ya cargado |
| Encadenamiento | Comportamiento del carrusel por el que el recorrido continúa con el marcador contiguo en los extremos. | — |
| CTA (acción primaria) | Control de mayor jerarquía que dispara la tarea central de la superficie (ingresar, crear, resolver, cerrar). | Destino amplio (Ley de Fitts) |
| Región de estado | Zona anunciada a lectores de pantalla donde se publican cambios dinámicos (avance del carrusel, resultado de una resolución, bloqueo de cierre). | WCAG 4.1.3 |
| Foco visible | Indicación perceptible del control que tiene el foco de teclado en cada momento. | WCAG 2.4.7 |
| Foco no oscurecido | Garantía de que el control enfocado no queda tapado por overlays. | WCAG 2.4.11, novedad 2.2 |
| Tamaño de objetivo | Superficie mínima accionable de un control para uso cómodo y accesible. | WCAG 2.5.8, novedad 2.2 |
| Tamaño mínimo de objetivo | Forma abreviada de "tamaño de objetivo (mínimo)" usada en los wireframes. | Sinónimo operativo del anterior |
| Expansión de texto | Margen de crecimiento del texto al traducir, que el layout debe tolerar sin truncar ni romperse. | Previsto hasta 35 % |
| Movimiento reducido | Preferencia del sistema operativo que simplifica o suprime animaciones para quien la activa. | Se respeta en transiciones del carrusel y banners |
| Degradar con dignidad | Comportarse de forma comprensible y recuperable ante una caída de red o de servicio, sin operar como si fuera offline. | Aplica al estado sin conexión al circuito |
| Handoff humano | Derivación del usuario a una persona (quien administra la cuenta) cuando el front no puede resolver el caso. | Aplica a acceso revocado |

## 4. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Glosario UX inicial de la sección: términos propios de experiencia, estados, componentes y accesibilidad de geovial-web, con referencia a los términos del dominio de 00 y 02 sin redefinirlos. |
