# CU-07 — Cargar fotos manualmente priorizando ubicación con radio de agrupación

**Proyecto:** geovial-mobile
**Documento:** CU-07-cargar-fotos-manualmente-radio-agrupacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + Mobile UX Analyst

## 1. Propósito

Permitir que el agente de campo cargue desde el dispositivo un conjunto de fotos ya tomadas (por ejemplo, con otro equipo) y que la app resuelva la ubicación de cada una priorizando los datos de ubicación incrustados en la propia imagen, agrupándolas en marcadores según un radio de agrupación, todo en el almacén local y sin necesidad de conexión. Evita reubicar cada foto a mano y aprovecha la georreferenciación que la imagen trae consigo.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Agente de campo | Primario | Selecciona y carga un conjunto de fotos desde el dispositivo |
| App móvil | Sistema | Lee la ubicación incrustada de cada foto, agrupa por radio, crea o reutiliza marcadores y observaciones locales y encola los cambios |
| Almacenamiento del dispositivo | Sistema | Aporta las fotos seleccionadas desde la galería o el almacenamiento del dispositivo |

## 3. Precondiciones

- El agente tiene una sesión activa (CU-01) y un relevamiento abierto en recolección (CU-02).
- La app cuenta con el permiso de acceso al almacenamiento o galería del dispositivo concedido por el sistema operativo.
- Hay un radio de agrupación aplicable a la carga (parámetro del relevamiento o del sistema).

## 4. Flujo principal

1. El agente selecciona un conjunto de fotos del almacenamiento del dispositivo para cargar al relevamiento activo.
2. Por cada foto, la app extrae la coordenada de los datos de ubicación incrustados en la imagen y la prioriza como ubicación de la foto.
3. La app busca un marcador local existente dentro del radio de agrupación respecto de esa coordenada.
4. Si hay un marcador dentro del radio, la app agrupa la foto en ese marcador; si no, crea un marcador local nuevo en la coordenada de la foto.
5. La app crea o asocia la observación correspondiente, aloja la imagen en el almacén local y encola los cambios como pendientes de sincronizar (CU-06).
6. La app muestra el resultado de la carga: fotos agrupadas en marcadores existentes, marcadores nuevos creados y fotos sin ubicación resuelta.

## 5. Flujos alternativos

- 5.A Foto sin datos de ubicación incrustados. Disparador: una foto del conjunto no trae coordenada en sus metadatos. La app la registra como pendiente de ubicación manual, sin agruparla por radio y sin inventar coordenada, y la incluye en el resultado como sin ubicación resuelta. Retorna al paso 6.
- 5.B Varias fotos en el mismo radio. Disparador: varias fotos del conjunto caen dentro del radio del mismo marcador. La app las agrupa todas en ese único marcador, que queda compartido por las observaciones resultantes. Retorna al paso 6.
- 5.C Formato de foto no soportado. Disparador: una foto del conjunto tiene un formato que la app no puede procesar. La app omite esa foto, continúa con el resto y la señala en el resultado. Retorna al paso 6.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| RADIO_NO_DEFINIDO | No hay un radio de agrupación aplicable a la carga | La app no procesa el conjunto y solicita un radio aplicable |
| PERMISO_ALMACENAMIENTO_DENEGADO | El usuario no concedió el acceso al almacenamiento o galería del dispositivo | La app no accede a las fotos y explica que el permiso es necesario |
| FORMATO_FOTO_NO_SOPORTADO | Una foto del conjunto tiene un formato que no se puede procesar | La app omite esa foto, continúa con el resto y la señala en el resultado |
| RELEVAMIENTO_CERRADO | El relevamiento activo está cerrado | La app no procesa la carga y lo deja en modo lectura |

## 7. Postcondiciones

- Éxito: cada foto con ubicación quedó agrupada en un marcador dentro del radio o en un marcador nuevo, las fotos sin ubicación quedaron pendientes de ubicación manual, los binarios quedaron en el almacén local y los cambios encolados.
- Fallo de la carga: no se crean marcadores ni observaciones y el agente recibe la indicación correspondiente.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un radio de agrupación definido y tres fotos con coordenada incrustada dentro de ese radio | El agente carga las tres fotos | La app las agrupa en un único marcador y reporta cero marcadores nuevos adicionales para esas fotos |
| CA-02 | Un radio definido y una foto con coordenada incrustada lejana a todo marcador existente | El agente carga la foto | La app crea un marcador local nuevo en la coordenada de la foto y agrupa la foto en él |
| CA-03 | Un conjunto con una foto sin datos de ubicación incrustados | El agente carga el conjunto | La app registra esa foto como pendiente de ubicación manual y la incluye en el resultado como sin ubicación resuelta |
| CA-04 | Una carga sin radio de agrupación aplicable | El agente intenta cargar fotos | La app responde con RADIO_NO_DEFINIDO y no procesa el conjunto |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-03 |
| Reglas de negocio aplicables | RN-01, RN-03 |
| Historias de usuario a generar | US-14, US-15 (en 06) |
| Componentes esperados | Selector de fotos del dispositivo; servicio de extracción de ubicación incrustada; servicio de agrupación por radio; repositorio local de marcadores y observaciones; cola local de cambios (referencia tentativa a 05) |
| Tests previstos | Agrupación de fotos dentro del radio en un marcador; creación de marcador para foto lejana; foto sin ubicación queda pendiente; carga sin radio rechazada (en 08) |

## 10. Notas y supuestos

- La priorización de la ubicación incrustada y el radio de agrupación son la invariante RN-01, alineada con la regla equivalente del backend (geovial-api RN-04).
- La carga manual produce marcadores y observaciones locales que se sincronizan con la misma cola y el mismo ciclo que la captura en terreno (CU-06).
- El caso de foto sin metadatos de ubicación figura como pendiente de respuesta del cliente en el intake (§7); aquí se asume que la foto queda pendiente de ubicación manual sin inventarle coordenada, a confirmar con el negocio (alineado con geovial-api 02 §9).
- Las fotos sin ubicación resuelta se ubican luego manualmente en el mapa usando la creación o el movimiento de marcadores (CU-03).

## 14. Permisos del sistema operativo

- Requiere el permiso de acceso al almacenamiento o galería del dispositivo para seleccionar las fotos; si se deniega, la carga no procede (PERMISO_ALMACENAMIENTO_DENEGADO).
- No requiere ubicación en vivo ni cámara: la ubicación se toma de los metadatos incrustados de cada imagen, no del GPS del momento.

## 12. Performance esperado del CU

- El procesamiento de la carga escala con el tamaño del conjunto sin degradar el resto de la app y funciona sin conexión; la agrupación por radio se resuelve localmente.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de carga manual de fotos con priorización de ubicación incrustada y radio de agrupación, derivado de NB-03 (F-09). |
