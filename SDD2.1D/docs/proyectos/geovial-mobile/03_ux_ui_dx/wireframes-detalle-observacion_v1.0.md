# Wireframe — Detalle de observación

**Proyecto:** geovial-mobile
**Documento:** wireframes-detalle-observacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Mobile UX Designer + Accessibility Specialist
**Variante:** UX/UI

## 1. Pantalla y propósito

Superficie de enriquecimiento de la evidencia anclada a un marcador. El agente revisa las fotos de la observación, escribe la nota de la observación, comenta cada foto y aplica etiquetas a fotos y al marcador, todo en el almacén local y sin conexión. También muestra el resultado de la carga manual: fotos agrupadas y fotos sin ubicación resuelta. CU origen: CU-05 (comentarios y etiquetas), con el resultado de CU-07 (carga manual por radio).

## 2. Layout

Pantalla en portrait, scroll vertical: nota de la observación arriba, lista de fotos con su comentario y etiquetas debajo, y el editor de etiqueta del marcador. El indicador de conectividad y sincronización es persistente.

```
+------------------------------------------+
|  < Marcador          [ en cola: 3   v ]  |
+------------------------------------------+
|  Observacion                             |
|  Nota: [_______________________________] |
|        [_______________________________] |
|                                          |
|  Etiquetas del marcador:                 |
|  [ fisura ] [ + agregar etiqueta ]       |
+------------------------------------------+
|  Fotos                                   |
|  +-------------+  +-------------+         |
|  | [ foto ]    |  | [ foto ]    |         |
|  | ( ! ) sin   |  | georref.    |         |  <- marca pendiente de ubicacion
|  |   ubicacion |  |             |         |
|  | Comentario: |  | Comentario: |         |
|  | [_________] |  | [_________] |         |
|  | [fisura][+] |  | [grieta][+] |         |
|  | [Ubicar en  |  |             |         |  <- accion ubicar foto pendiente
|  |  el mapa]   |  |             |         |
|  +-------------+  +-------------+         |
+------------------------------------------+
```

## 3. Componentes principales

| Componente | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- |
| Campo Nota de la observación | Describir la observación | Texto de la nota | Edición libre; guardado local optimista; se encola |
| Galería de fotos | Mostrar las fotos del marcador | Miniaturas, estado georreferenciada o pendiente de ubicación | Tocar una foto la amplía; el orden encadena las fotos del marcador |
| Marca de pendiente de ubicación | Señalar fotos sin coordenada | Marca sobre la miniatura | No bloquea comentar ni etiquetar; ofrece Ubicar en el mapa |
| Acción Ubicar en el mapa | Resolver la ubicación de una foto pendiente | Rótulo de acción | Lleva al mapa de captura para fijar o mover el marcador (CU-03) |
| Campo Comentario de foto | Describir una foto | Texto del comentario (a lo sumo uno por foto) | Edición y corrección; guardado local optimista; se encola |
| Selector de etiquetas | Aplicar etiquetas a foto y al marcador | Etiquetas aplicadas y catálogo del relevamiento | Reutiliza etiquetas existentes sin duplicar; crea nuevas; quita; rechaza etiqueta vacía |
| Indicador de conectividad y sincronización | Comunicar red y cola siempre visible | Sin conexión / cambios en cola / sincronizando / al día | Persistente; tocarlo abre el estado de sincronización |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Escribir nota de la observación | El agente edita el campo Nota | La app registra la nota en el almacén local y la encola | Relevamiento en recolección; observación existente |
| Comentar una foto | El agente escribe el comentario de una foto | La app registra el comentario (a lo sumo uno por foto) y lo encola | La foto existe; relevamiento en recolección |
| Aplicar etiqueta existente | El agente elige una etiqueta del catálogo | La app la aplica sin duplicarla, compartida entre fotos y marcadores | Existe el catálogo de etiquetas del relevamiento |
| Crear etiqueta nueva | El agente escribe un nombre y confirma | La app crea la etiqueta y la aplica; rechaza nombre vacío (ETIQUETA_VACIA) | Relevamiento en recolección |
| Quitar comentario o etiqueta | El agente edita o quita | La app actualiza el registro local y encola el cambio | Relevamiento en recolección |
| Comentar foto pendiente de ubicación | El agente comenta una foto sin coordenada | La app permite comentar y etiquetar sin requerir coordenada (CU-05 5.B) | La foto está pendiente de ubicación |
| Ubicar foto en el mapa | El agente toca Ubicar en el mapa | La app abre el mapa de captura para fijar o mover el marcador de esa foto | La foto está pendiente de ubicación |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío | Observación sin fotos ni comentarios | Texto orientativo y acción de capturar (CU-04) o cargar fotos (CU-07) |
| Cargando | Lectura de fotos y observación desde el almacén local | Skeleton de la galería y de los campos |
| Con datos | Hay fotos, nota o comentarios | Render de la nota, las fotos con su comentario y etiquetas |
| Sin conexión | No hay red | Edición de nota, comentarios y etiquetas plenamente operativa; los cambios se encolan |
| Sincronizando | Un ciclo de sincronización corre en segundo plano | Indicador de progreso de cola; la edición no se interrumpe |
| Error | ETIQUETA_VACIA, OBSERVACION_INEXISTENTE, RELEVAMIENTO_CERRADO | Etiqueta vacía rechazada inline; observación o foto inexistente avisada; relevamiento cerrado en solo lectura |
| Pendiente de ubicación | Una foto quedó sin coordenada (CU-04 5.A o CU-07 5.A) | Marca de pendiente de ubicación sobre la foto y acción Ubicar en el mapa; comentar y etiquetar siguen disponibles |

## 6. Versión móvil o responsive

App de campo en portrait como orientación primaria. Notas de adaptación:

- La galería usa una rejilla de pocas columnas en portrait; en pantallas más anchas suma columnas sin reducir el tamaño de la miniatura ni del objetivo táctil de comentar y etiquetar.
- Los campos de texto y los selectores de etiqueta permanecen al alcance del pulgar; al abrir el teclado del sistema, el campo en foco no queda tapado (2.4.11) y la pantalla hace scroll.
- En landscape (no primario), la nota y la galería se reorganizan en dos áreas; ninguna tarea de edición exige rotar (1.3.4).

## 7. Notas de implementación

- Accesibilidad: cada foto expone su comentario y etiqueta como texto alternativo accesible y su estado (georreferenciada o pendiente de ubicación) se anuncia (1.1.1, 4.1.2); campos con etiqueta asociada (1.3.1, 3.3.2); objetivos táctiles grandes para guantes (2.5.8); foco visible y no oscurecido por el teclado (2.4.7, 2.4.11); los cambios guardados se anuncian por región de estado (4.1.3); las etiquetas no se comunican solo por color (1.4.1).
- Performance percibida: edición con guardado local optimista; reutilización de etiquetas sin recargar el catálogo; la sincronización no interrumpe la edición.
- Internacionalización: la nota, los comentarios y las etiquetas son contenido del usuario y se muestran tal cual; las etiquetas largas se truncan con texto completo accesible; los rótulos se externalizan y toleran expansión.
- Regla de dominio reflejada: una foto tiene a lo sumo un comentario; una etiqueta puede marcar varias fotos y varios marcadores (02 modelo conceptual).

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Agente de campo (00) |
| CU origen | CU-05 (resultado de carga manual: CU-07) |
| Marco experiencia aplicado | experiencia-de-uso_v1.0.md §3.5, §3.7, §4 (estados), §5 (accesibilidad), §8 (errores) |
| Reglas de negocio relevantes | RN-05, RN-03, RN-01 |
| US a generar | US-09, US-10 (en 06); el resultado de carga manual aporta a US-14, US-15 |
| Tests previstos | Comentario y etiqueta registrados y encolados; etiqueta reutilizada sin duplicar; comentario sobre foto sin ubicación; etiqueta vacía rechazada; foto sin ubicación queda pendiente y ubicable en el mapa (en 08) |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Wireframe inicial del detalle de observación: nota, galería de fotos con comentario y etiquetas, reutilización de etiquetas sin duplicar, marca de pendiente de ubicación con acción de ubicar en el mapa, resultado de la carga manual, estados (incluido sin conexión y sincronizando) y trazabilidad a CU-05 y CU-07 con RN-05, RN-03 y RN-01. |
