# Wireframe — Mapa de captura

**Proyecto:** geovial-mobile
**Documento:** wireframes-mapa-captura_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Mobile UX Designer + Accessibility Specialist
**Variante:** UX/UI

## 1. Pantalla y propósito

Superficie central de recolección en terreno. Sobre el mapa del relevamiento activo, el agente se centra por GPS, crea o mueve marcadores georreferenciados, captura fotos con resolución de coordenadas en el momento y abre la carga manual de fotos. Es el gesto base de georreferenciación y el origen de la evidencia, ejecutable por completo sin conexión. CU origen: CU-03 y CU-04 (captura), con la acción de carga manual de CU-07.

## 2. Layout

Pantalla en portrait, el mapa ocupa casi toda el área; las acciones primarias de captura se ubican en la zona alcanzable con el pulgar; el indicador de conectividad y sincronización es persistente.

```
+------------------------------------------+
|  < Tramo Norte       [ en cola: 3   v ]  |  <- volver / indicador conectividad-sync
+------------------------------------------+
|                                          |
|              . pin .                     |
|        ( ! conflicto )  . pin .          |  <- marcador con marca de conflicto
|                                          |
|                  (o) <- posicion agente  |
|             . pin .                      |
|                                          |
|                          [ centrar GPS ] |  <- accion, esquina alcanzable
|                                          |
+------------------------------------------+
|  [ + Marcador ]  [ (camara) Foto ]       |  <- acciones primarias grandes
|  [ Cargar fotos del dispositivo ]        |  <- abre carga manual (CU-07)
+------------------------------------------+
```

Al tocar un pin se abre una hoja inferior con sus acciones (capturar foto, abrir detalle de observación, mover marcador, etiquetar):

```
+------------------------------------------+
|  Marcador  - 2 observaciones             |
|  ( ! ) En conflicto con otro marcador    |  <- no bloqueante (RN-03)
|  [ (camara) Capturar foto aqui ]         |
|  [ Ver observaciones ]                   |
|  [ Mover marcador ]   [ Etiquetar ]      |
+------------------------------------------+
```

## 3. Componentes principales

| Componente | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- |
| Componente de mapa | Situar al agente y la evidencia del relevamiento | Posición del agente, pines de marcadores, marcas de conflicto | Renderiza pines; tocar un pin abre su hoja de acciones; arrastrar un pin lo mueve |
| Acción Centrar por GPS | Centrar el mapa sobre la posición del agente | Ícono o rótulo de acción | Pide el permiso de ubicación la primera vez; centra y señala la precisión; sin señal ofrece fijación manual |
| Acción Crear marcador | Crear un marcador en la posición tomada del GPS o tocada en el mapa | Rótulo de acción | Guarda el marcador local con identidad estable y lo encola; convive con conflicto por radio sin bloquear |
| Acción Capturar foto | Tomar una foto anclada al marcador activo | Rótulo de acción | Pide el permiso de cámara la primera vez; abre la cámara del dispositivo; resuelve coordenada y ancla la foto |
| Acción Cargar fotos del dispositivo | Iniciar la carga manual por radio (CU-07) | Rótulo de acción | Pide el permiso de acceso al almacenamiento la primera vez; abre el selector y procesa el lote |
| Pin de marcador | Representar un marcador en el mapa | Ubicación; marca de conflicto si aplica; conteo de observaciones | Tocar abre la hoja de acciones; la marca de conflicto es informativa, no bloquea |
| Hoja de acciones del marcador | Operar sobre un marcador seleccionado | Conteo de observaciones, estado de conflicto | Capturar foto, ver observaciones, mover, etiquetar |
| Indicador de conectividad y sincronización | Comunicar red y cola siempre visible | Sin conexión / cambios en cola / sincronizando / al día | Persistente; tocarlo abre el estado de sincronización |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Centrar por GPS | El agente toca Centrar GPS | La app obtiene la posición y centra el mapa, señalando la precisión | Permiso de ubicación concedido; hay señal de GPS |
| Crear marcador por GPS | El agente toca Crear marcador sobre la posición | La app crea el marcador local con identidad estable y lo encola | Relevamiento en recolección; posición disponible |
| Crear o fijar marcador manual | El agente toca un punto del mapa (sin GPS o por elección) | La app crea o fija el marcador en la coordenada tocada y lo encola | Relevamiento en recolección |
| Mover marcador | El agente arrastra un pin o usa Mover marcador | La app actualiza la coordenada conservando la identidad del marcador y encola el cambio | Marcador existente; relevamiento en recolección |
| Capturar foto | El agente toca Capturar foto sobre el marcador | La app abre la cámara, resuelve la coordenada y ancla la foto a una observación del marcador, encolando | Permisos de cámara y ubicación; marcador del entorno o recién creado |
| Crear marcador al capturar sin marcador | El agente captura sin marcador del entorno | La app crea un marcador en la coordenada del momento y ancla la foto (CU-04 5.C) | Relevamiento en recolección |
| Abrir carga manual | El agente toca Cargar fotos del dispositivo | La app abre el selector de fotos y procesa el lote por radio (CU-07) | Permiso de almacenamiento; radio de agrupación aplicable |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío | Relevamiento sin marcadores | Mapa centrado y acción destacada de crear el primer marcador |
| Cargando | Carga del mapa o de la copia local; obtención de la posición del GPS | Skeleton del mapa; indicador de obtención de posición |
| Con datos | Hay marcadores y posición del agente | Mapa con pines, posición y acciones de captura |
| Sin conexión | No hay red | Captura plenamente operativa; cada cambio se guarda local y se encola; el indicador muestra modo sin conexión |
| Sincronizando | Un ciclo de sincronización corre en segundo plano | Indicador de progreso de cola; la captura no se interrumpe |
| Error | PERMISO_UBICACION_DENEGADO, SIN_SENAL_GPS, PERMISO_CAMARA_DENEGADO, ALMACEN_LOCAL_SIN_ESPACIO, RELEVAMIENTO_CERRADO | Permiso de ubicación denegado: explica y ofrece fijación manual; sin señal: fijación manual sin coordenada inventada; permiso de cámara denegado: no abre la cámara; sin espacio: avisa que libere; cerrado: modo lectura |
| En conflicto | Un marcador cae dentro del radio de otro | Marca de conflicto no bloqueante sobre el pin; la recolección continúa (RN-03) |
| Pendiente de ubicación | Foto capturada sin señal de GPS | La foto queda anclada al marcador y marcada como pendiente de ubicación, con acceso a ubicarla luego en el mapa |

## 6. Versión móvil o responsive

App de campo en portrait como orientación primaria. Notas de adaptación:

- El mapa aprovecha toda el área disponible; las acciones primarias permanecen ancladas a la franja inferior alcanzable con el pulgar en cualquier alto de pantalla.
- En pantallas más anchas, los pines y los objetivos táctiles conservan su tamaño mínimo; no se reduce el área de toque al aumentar la densidad del mapa.
- En landscape (no primario), el mapa se ensancha y las acciones primarias se reubican a un lateral alcanzable; ninguna tarea de captura exige rotar (1.3.4). La hoja de acciones del marcador no debe tapar el pin seleccionado (2.4.11).

## 7. Notas de implementación

- Accesibilidad: las acciones de captura superan con holgura el tamaño mínimo de objetivo, dimensionadas para guantes, y están separadas (2.5.8); cada gesto tiene alternativa de un solo toque: arrastrar el marcador equivale a Mover marcador y elegir coordenada (2.5.1); los pines y las marcas de conflicto tienen nombre accesible y no se comunican solo por color (1.4.1, 4.1.2); el guardado de cada captura y el cambio de conectividad se anuncian por región de estado (4.1.3); contraste alto para legibilidad bajo sol directo (1.4.3, 1.4.11). El permiso se pide con microcopy que justifica su uso.
- Performance percibida: centrar por GPS responde al instante sobre la última posición conocida y refina al llegar la fija; crear o mover un marcador es optimista y el encolado ocurre detrás; la cámara abre rápido y la foto aparece anclada de inmediato; la sincronización nunca bloquea la captura.
- Internacionalización: rótulos externalizados con tolerancia a expansión; las coordenadas y la precisión se presentan en formato localizable; las etiquetas del marcador son contenido del usuario.
- No se inventan coordenadas: ante falta de señal o permiso, el marcador se fija manualmente y la foto queda pendiente de ubicación.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Agente de campo (00) |
| CU origen | CU-03, CU-04 (acción de carga manual: CU-07) |
| Marco experiencia aplicado | experiencia-de-uso_v1.0.md §3.3, §3.4, §3.7, §4 (estados), §5 (accesibilidad), §8 (errores) |
| Reglas de negocio relevantes | RN-03, RN-05, RN-01 |
| US a generar | US-05, US-06, US-07, US-08 (en 06); la carga manual aporta a US-14, US-15 |
| Tests previstos | Centrar por GPS y crear marcador encolado; mover marcador conserva identidad; convivencia con conflicto por radio; sin señal de GPS ofrece fijación manual sin coordenada inventada; permiso de ubicación o cámara denegado degrada; captura resuelve coordenada y ancla foto; captura sin GPS deja pendiente de ubicación (en 08) |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Wireframe inicial del mapa de captura: centrado por GPS, creación y movimiento de marcadores con identidad estable, captura de foto con resolución de coordenadas, hoja de acciones del marcador, acceso a la carga manual, convivencia con conflicto y marca de pendiente de ubicación, estados (incluido sin conexión y sincronizando) y trazabilidad a CU-03, CU-04 y CU-07 con RN-03, RN-05 y RN-01. |
