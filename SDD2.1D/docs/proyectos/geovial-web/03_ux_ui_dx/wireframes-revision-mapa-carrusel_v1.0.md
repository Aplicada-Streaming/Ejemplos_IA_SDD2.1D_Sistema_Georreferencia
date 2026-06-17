# Wireframe — Revisión sobre mapa con carrusel de fotos

**Proyecto:** geovial-web
**Documento:** wireframes-revision-mapa-carrusel_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** UX/UI Designer + Frontend Lead
**Variante:** UX/UI

## 1. Pantalla y propósito

Revisión de un relevamiento sobre el mapa: el flujo principal del front. El jefe de área recorre los marcadores en su contexto geográfico, abre el carrusel de fotos de cada marcador con sus comentarios y etiquetas, avanza y retrocede encadenando con el marcador contiguo, amplía una foto y filtra por etiqueta para acotar la evidencia. Ordena el material para confeccionar el informe; revisar no cambia el estado del relevamiento. CU origen: CU-06. Marco aplicado: `experiencia-de-uso_v1.0.md` (flujo 3.6, estados §4.2, errores §8). El carrusel reutiliza la representación de `representacion-carrusel-fotos_v1.0.md`.

## 2. Layout

Dos paneles principales: el componente de mapa con los marcadores a la izquierda y un panel de evidencia a la derecha; sobre ambos, una barra de contexto del relevamiento con su estado y los filtros por etiqueta. El carrusel se abre como modal sobre el conjunto al seleccionar un marcador.

```text
+----------------------------------------------------------------------+
| < Relevamientos   Tramo Norte   Estado: Revision   [ aviso conflictos ]|
+----------------------------------------------------------------------+
| Filtrar por etiqueta: [ fisura ] [ junta ] [ + ]      [ limpiar ]     |
+--------------------------------------+-------------------------------+
|  Componente de mapa                  |  Evidencia del marcador        |
|                                      |  ----------------------------- |
|     (o)----(o)        (o)            |  Marcador M-03                 |
|       \\     \\         |             |  Etiquetas: fisura, junta      |
|        (o)   (o*)      (o)           |  Autor: <agente> (registrada)  |
|              ^seleccionado           |  Fotos: 6                      |
|                                      |  [ miniatura ][ miniatura ]    |
|   (o) marcador   (o*) seleccionado   |  [ miniatura ][ miniatura ]    |
|   (X) en conflicto                   |  [ Abrir carrusel ]            |
+--------------------------------------+-------------------------------+

Carrusel (modal sobre la pantalla), ver representacion-carrusel-fotos:
+----------------------------------------------------------------------+
|  Marcador M-03  ·  foto 3 de 6                                  [ X ] |
|                                                                      |
|   [ < ]              [   FOTO AMPLIADA   ]              [ > ]         |
|                                                                      |
|  Comentario: "fisura longitudinal en junta de dilatacion"            |
|  Etiqueta: fisura     Autor: <agente>                                |
|  ........ al llegar al extremo, encadena con el marcador contiguo ...|
+----------------------------------------------------------------------+
```

## 3. Componentes principales

| Componente | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- |
| Barra de contexto | Situar el relevamiento y su estado | Nombre del tramo, estado del ciclo, aviso de conflictos pendientes | Estado siempre visible (RN-04); el aviso de conflictos no bloquea (RN-05) |
| Componente de mapa | Presentar los marcadores en su contexto | Marcadores con su posición y señal de conflicto | Selección de marcador; render incremental; el marcador seleccionado se distingue por forma e ícono, no solo por color |
| Panel de evidencia del marcador | Resumir la evidencia del marcador elegido | Etiquetas, autoría (visible aun si el autor fue dado de baja, RN-02), miniaturas, conteo de fotos | Abre el carrusel; lista miniaturas con texto alternativo |
| Filtro por etiqueta | Acotar marcadores y fotos | Etiquetas disponibles y activas | Filtra el conjunto; avisa si no hay coincidencias y permite limpiar (CU-06 5.C) |
| Carrusel de fotos | Recorrer las fotos del marcador y los contiguos | Foto, comentario, etiqueta, autoría, posición (foto N de M) | Avanza, retrocede, amplía; encadena al marcador contiguo en los extremos; reutiliza `representacion-carrusel-fotos_v1.0.md` |
| Aviso de conflictos | Señalar que hay conflictos pendientes | Cantidad de conflictos pendientes y acceso a resolverlos | No bloquea la revisión; deriva a CU-07 (RN-05) |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Seleccionar un marcador | Clic o teclado sobre un marcador | El panel de evidencia muestra sus fotos y datos (CU-06 paso 3) | Relevamiento abierto con evidencia |
| Abrir el carrusel | Acción Abrir carrusel o miniatura | Se abre el carrusel en la foto elegida (CU-06 paso 3) | El marcador tiene fotos |
| Avanzar o retroceder | Controles anterior y siguiente o teclas de flecha | Cambia la foto; en el extremo encadena con el marcador contiguo (CU-06 paso 4, CA-01) | Carrusel abierto |
| Ampliar una foto | Acción de ampliar | Muestra la foto en detalle con comentario y etiqueta (CU-06 paso 5) | Carrusel abierto |
| Filtrar por etiqueta | Selección de una etiqueta | Muestra solo marcadores y fotos con esa etiqueta (CU-06 paso 6, CA-02) | — |
| Limpiar filtro sin coincidencias | Acción limpiar | Restaura el conjunto completo (CU-06 5.C) | Filtro activo sin resultados |
| Reintentar evidencia | Acción en el banner de evidencia | Reintenta traer la evidencia (EVIDENCIA_NO_DISPONIBLE) | Falla transitoria del backend |
| Ir a resolver conflictos | Acción en el aviso de conflictos | Navega a la pantalla de resolución (CU-07) | Hay conflictos pendientes |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío | El relevamiento aún no tiene evidencia sincronizada | Aviso de que todavía no hay evidencia para revisar; mapa sin marcadores de evidencia |
| Marcador sin fotos | El marcador seleccionado no tiene fotos | Mensaje "este marcador no tiene fotos" y ofrecimiento de pasar al contiguo (CU-06 5.A, CA-04) |
| Cargando | Carga del mapa, los marcadores o las fotos del carrusel | Skeleton del mapa y del panel; spinner en el carrusel al traer un binario; precarga de fotos contiguas |
| Con datos | El backend entregó la evidencia | Mapa con marcadores, panel de evidencia y carrusel operables; autoría visible |
| Con datos y conflictos presentes | Hay marcadores en conflicto | Toda la evidencia accesible y un aviso no bloqueante de conflictos pendientes (CU-06 5.B, CA-03, RN-05) |
| Filtro sin coincidencias | La etiqueta filtrada no tiene marcadores | Mensaje de sin coincidencias y acción de limpiar (CU-06 5.C) |
| Error: foto no disponible | El binario de una foto no se recupera | Marcador de foto no disponible inline; el carrusel continúa con el resto (FOTO_NO_DISPONIBLE) |
| Error recuperable: evidencia no disponible | El backend no entrega la evidencia ahora | Banner con reintento (EVIDENCIA_NO_DISPONIBLE) |
| Sin conexión al circuito | El backend no responde o la sesión expiró | Banner persistente de servicio no disponible o aviso de sesión expirada |
| Fuera de alcance | El relevamiento no pertenece al jefe | No se abre; ante rechazo del backend, mensaje y retorno al listado (FUERA_DE_ALCANCE) |

## 6. Versión móvil o responsive

En anchos reducidos el mapa y el panel de evidencia se apilan: el mapa arriba y la evidencia debajo, con un control para alternar entre ambos y aprovechar la pantalla. El carrusel ocupa la pantalla completa con controles anterior y siguiente amplios y fijos, y soporte de gesto de deslizamiento sin acoplar la dirección de avance a la dirección de lectura. Los filtros de etiqueta se colapsan en un control desplegable. Se conserva el tamaño mínimo de objetivo en los controles del carrusel.

## 7. Notas de implementación

- Accesibilidad: cada foto expone su comentario y etiqueta como alternativa textual accesible (1.1.1); el marcador de foto no disponible se anuncia como tal; el avance del carrusel y el cambio de marcador se anuncian por región de estado (4.1.3); el carrusel y los modales devuelven el foco al cerrarse y no atrapan el teclado (2.1.1, 2.1.2); el foco nunca queda oscurecido por el overlay del carrusel (2.4.11); los marcadores y el seleccionado se distinguen por forma e ícono además del color (1.4.1, 1.4.11); los controles del carrusel cumplen el tamaño mínimo de objetivo (2.5.8).
- Performance percibida: render incremental de marcadores; precarga de fotos contiguas para respuesta inmediata entre fotos ya cargadas; placeholder mientras llega un binario; el carrusel nunca se bloquea por una foto faltante; spinner solo si la espera supera el umbral.
- Internacionalización: los comentarios y etiquetas son contenido del usuario y se muestran tal cual; las coordenadas se presentan con separador decimal según la configuración regional; las etiquetas largas se truncan con texto completo accesible.
- Conflictos: la presencia de conflictos nunca bloquea el acceso a la evidencia durante la revisión (RN-05); solo se señala y se ofrece su resolución.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Jefe de área (00) |
| CU origen | CU-06 (revisar el relevamiento sobre el mapa con carrusel de fotos) |
| Reglas de negocio relevantes | RN-01 (visibilidad por rol), RN-02 (autoría visible en la revisión), RN-04 (estados visibles), RN-05 (conflictos conviven sin bloquear) |
| Marco de experiencia aplicado | experiencia-de-uso_v1.0.md (flujo 3.6, estados §4.2, errores §8) |
| Representación reutilizada | representacion-carrusel-fotos_v1.0.md |
| US a generar | US-13, US-14, US-15 (06) |
| Tests previstos | Carrusel encadena al contiguo; filtro por etiqueta; evidencia accesible con conflictos presentes; marcador sin fotos; foto no disponible continúa el carrusel; autoría visible tras baja del autor (08) |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Wireframe inicial de la revisión sobre mapa con carrusel (flujo principal), anclado a CU-06, al marco de experiencia y a la representación del carrusel. Layout de mapa más panel de evidencia y carrusel modal, estados (vacío, marcador sin fotos, cargando, con datos, con conflictos, filtro sin coincidencias, foto no disponible, evidencia no disponible, sin conexión y fuera de alcance), reflujo apilado en móvil y notas de accesibilidad WCAG 2.2 AA. |
