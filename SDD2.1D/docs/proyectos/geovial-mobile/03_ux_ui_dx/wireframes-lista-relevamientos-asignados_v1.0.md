# Wireframe — Lista de relevamientos asignados

**Proyecto:** geovial-mobile
**Documento:** wireframes-lista-relevamientos-asignados_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Mobile UX Designer + Accessibility Specialist
**Variante:** UX/UI

## 1. Pantalla y propósito

Superficie de entrada al trabajo, posterior al inicio de sesión. El agente ve los relevamientos que tiene asignados, servidos del almacén local del dispositivo, y elige uno para trabajar, que queda como contexto activo de captura. Es el punto de partida de toda la recolección. CU origen: CU-02.

## 2. Layout

Pantalla en portrait, lista vertical de tarjetas, con un indicador de conectividad y de sincronización persistente en la cabecera y una acción de refresco.

```
+------------------------------------------+
|  Relevamientos        [ sin conexion v]  |  <- indicador conectividad/sync
|                                  [refrescar] |
+------------------------------------------+
|  +------------------------------------+  |
|  | Tramo Norte - Ruta 12              |  |
|  | Estado: Recoleccion                |  |
|  | 3 cambios en cola                  |  |  <- pendientes locales del relevamiento
|  +------------------------------------+  |
|  +------------------------------------+  |
|  | Puente Arroyo Seco                 |  |
|  | Estado: Recoleccion                |  |
|  | Al dia                             |  |
|  +------------------------------------+  |
|  +------------------------------------+  |
|  | Tramo Sur - Camino vecinal         |  |
|  | Estado: Cerrado (solo lectura)     |  |  <- abre en modo lectura
|  +------------------------------------+  |
|                                          |
+------------------------------------------+
```

El indicador de conectividad y de sincronización de la cabecera es el patrón persistente del marco (sin conexión, cambios en cola, sincronizando, al día). Tocarlo abre la pantalla de estado de sincronización.

## 3. Componentes principales

| Componente | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- |
| Indicador de conectividad y sincronización | Comunicar el estado de red y de la cola siempre visible | Modo sin conexión / cambios en cola / sincronizando / al día | Persistente; tocarlo lleva al estado de sincronización (CU-06) |
| Acción Refrescar | Traer asignaciones nuevas o cambios cuando hay red | Rótulo o ícono de refresco | Dispara un ciclo de sincronización (CU-06); deshabilitado o diferido sin red |
| Tarjeta de relevamiento | Representar un relevamiento asignado | Nombre del tramo, estado del ciclo, cambios locales en cola del relevamiento | Tocarla lo fija como contexto activo y abre el mapa de captura; si está cerrado, abre en solo lectura |
| Marca de solo lectura | Señalar un relevamiento cerrado | Etiqueta "Cerrado (solo lectura)" | La tarjeta abre en modo lectura sin habilitar capturas |
| Estado vacío | Orientar cuando no hay relevamientos locales | Texto orientativo y acción de refrescar | Invita a refrescar si hay red |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Seleccionar relevamiento | El agente toca una tarjeta en recolección | La app fija el relevamiento como contexto activo y abre su mapa con los marcadores y observaciones locales | Sesión activa; el relevamiento existe en la copia local |
| Abrir relevamiento cerrado | El agente toca una tarjeta cerrada | La app abre el relevamiento en solo lectura sin habilitar capturas (RELEVAMIENTO_CERRADO) | El relevamiento figura cerrado en la copia local |
| Refrescar la lista | El agente toca Refrescar con red | La app sincroniza, actualiza la copia local y refleja asignaciones nuevas o cambios (CU-06) | Hay conexión disponible |
| Abrir estado de sincronización | El agente toca el indicador de la cabecera | La app abre la pantalla de estado de sincronización | — |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío | Sin relevamientos sincronizados en el dispositivo | Texto orientativo "Todavía no tenés relevamientos en el dispositivo" y acción de refrescar si hay red |
| Cargando | Lectura de la copia local o refresco en curso | Skeleton de tarjetas |
| Con datos | Hay relevamientos en la copia local | Lista de tarjetas con tramo, estado y estado de cola por relevamiento |
| Sin conexión | No hay red | La lista se sirve del almacén local; el refresco se difiere; sin copia local: SIN_RELEVAMIENTOS_LOCALES con aviso de que no hay datos disponibles sin conexión |
| Sincronizando | Un refresco o un ciclo de sincronización está en curso | Indicador de progreso en la cabecera; la lista permanece usable |
| Error | RELEVAMIENTO_NO_ASIGNADO tras refresco; RELEVAMIENTO_CERRADO al seleccionar | El relevamiento no asignado se retira de la lista; el cerrado abre en solo lectura |

## 6. Versión móvil o responsive

App de campo en portrait como orientación primaria. Notas de adaptación:

- En pantallas más anchas, las tarjetas conservan una sola columna legible; no se compactan a varias columnas para no reducir el tamaño del objetivo táctil.
- En pantallas más altas, se muestran más tarjetas; la lista hace scroll vertical. El indicador de la cabecera permanece fijo.
- En landscape (no primario), la cabecera y la lista se mantienen; las tarjetas conservan su altura para uso con guantes. No se exige rotar (1.3.4).

## 7. Notas de implementación

- Accesibilidad: cada tarjeta es un objetivo táctil grande con nombre accesible que incluye tramo, estado y estado de cola (2.5.8, 4.1.2); el estado del ciclo y de la cola se comunica por texto e ícono, no solo por color (1.4.1); foco visible al navegar con lector de pantalla (2.4.7); los cambios de estado del indicador de cabecera se anuncian por región de estado (4.1.3).
- Performance percibida: la lista se sirve del almacén local con sensación inmediata; el refresco con red es secundario y muestra progreso sin bloquear la lectura.
- Internacionalización: el nombre del tramo es contenido del usuario y se trunca con texto completo accesible; los rótulos de estado se externalizan y toleran expansión.
- Offline-first: la lista nunca depende de la red para mostrarse; sin red solo se difiere el refresco.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Agente de campo (00) |
| CU origen | CU-02 |
| Marco experiencia aplicado | experiencia-de-uso_v1.0.md §3.2, §4 (estados), §5 (accesibilidad), §8 (errores) |
| Reglas de negocio relevantes | RN-05, RN-02 |
| US a generar | US-03, US-04 (en 06) |
| Tests previstos | Selección fija contexto y abre mapa local; lista vacía sin conexión rechazada (SIN_RELEVAMIENTOS_LOCALES); refresco con conexión agrega asignación; relevamiento cerrado abre en solo lectura (en 08) |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Wireframe inicial de la lista de relevamientos asignados servida del almacén local, con indicador persistente de conectividad y sincronización, estado de cola por relevamiento, modo solo lectura para relevamientos cerrados, estados (incluido sin conexión y sincronizando) y trazabilidad a CU-02, RN-05 y RN-02. |
