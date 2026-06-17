# CU-09 — Cargar manualmente un relevamiento completo vía web

**Proyecto:** geovial-web
**Documento:** CU-09-carga-manual-relevamiento-web_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional

## 1. Propósito

Permitir que un agente de campo, desde el front web, cargue manualmente la evidencia de un relevamiento asignado: subir fotos cuya ubicación se prioriza a partir de los datos incrustados en la imagen, agrupándolas en marcadores según un radio de agrupación, y completar comentarios y etiquetas. Cubre el caso en que el agente trabaja desde el entorno web en lugar de capturar en terreno.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Agente de campo | Primario | Sube fotos y completa la evidencia del relevamiento desde el front web |
| Front web | Sistema | Presenta la carga, el radio de agrupación y la edición de evidencia, y envía las fotos y datos al backend |
| Backend de dominio | Sistema | Prioriza la ubicación incrustada, agrupa por radio en marcadores y registra la evidencia |

## 3. Precondiciones

- El agente de campo tiene una sesión activa en el front web (CU-01).
- El agente está asignado al relevamiento sobre el que carga.
- El relevamiento está en estado de recolección.

## 4. Flujo principal

1. El agente abre, en el front web, un relevamiento al que está asignado y la pantalla de carga manual.
2. El agente define el radio de agrupación a aplicar a la carga.
3. El agente selecciona una o varias fotos para subir.
4. El front web envía las fotos al backend, que toma la ubicación incrustada de cada foto y la agrupa en un marcador existente dentro del radio o crea uno nuevo en la ubicación de la foto.
5. El backend devuelve los marcadores resultantes con las fotos agrupadas; el front los presenta sobre el mapa.
6. El agente agrega a cada foto su comentario y su etiqueta y completa la nota de la observación.
7. El front envía los comentarios, etiquetas y notas al backend y refleja la evidencia cargada.

## 5. Flujos alternativos

- 5.A Foto sin ubicación incrustada. Disparador: una foto subida no trae datos de ubicación. El front la presenta como pendiente de ubicación manual, sin inventarle coordenada, y permite ubicarla sobre el mapa. Retorna al paso 5.
- 5.B Carga sin radio definido. Disparador: el agente intenta subir fotos sin haber definido el radio de agrupación. El front no envía la carga y solicita definir primero el radio. Retorna al paso 2.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| RADIO_NO_DEFINIDO | Se intenta cargar fotos sin un radio de agrupación aplicable | El front bloquea la carga e informa que falta definir el radio |
| RELEVAMIENTO_NO_EN_RECOLECCION | El relevamiento no está en recolección al cargar | El front presenta la carga en solo lectura y no envía fotos |
| NO_ASIGNADO | El agente intenta cargar en un relevamiento al que no está asignado | El front no abre la carga; ante el rechazo del backend informa que no está asignado |

## 7. Postcondiciones

- Éxito: las fotos quedan agrupadas en marcadores según su ubicación y el radio, con sus comentarios, etiquetas y notas, visibles sobre el mapa del relevamiento.
- Éxito parcial: las fotos sin ubicación incrustada quedan pendientes de ubicación manual, sin coordenada inventada.
- Fallo: la evidencia del relevamiento no cambia y el front informa la causa.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un agente asignado a un relevamiento en recolección con radio de 15 metros definido | Sube tres fotos con ubicación incrustada dentro de ese radio | El front las muestra agrupadas en un único marcador |
| CA-02 | El mismo agente con una foto cuya ubicación está lejos de todo marcador | Sube esa foto | El front la muestra en un marcador nuevo creado en la ubicación de la foto |
| CA-03 | Una foto sin datos de ubicación incrustados | El agente la sube | El front la deja pendiente de ubicación manual sin asignarle coordenada |
| CA-04 | Un agente que no definió el radio de agrupación | Intenta subir fotos | El front bloquea la carga con RADIO_NO_DEFINIDO |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-02 |
| Reglas de negocio aplicables | RN-04 (geovial-web), RN-01 (geovial-web) |
| Historias de usuario a generar | US-20, US-21 (en 06) |
| Componentes esperados | Pantalla de carga manual; control de radio de agrupación; subida de fotos; editor de comentarios y etiquetas; consumo del recurso de carga manual del backend (referencia tentativa a 05) |
| Tests previstos | Fotos dentro del radio agrupadas en un marcador; foto lejana en marcador nuevo; foto sin ubicación pendiente; carga sin radio bloqueada (en 08) |

## 10. Notas y supuestos

- Este CU es la materialización web de la carga manual completa por el agente (intake §4 F-15, Should Have); la captura en terreno (con foto en el momento y GPS) es un flujo del proyecto de campo, no del front web.
- La priorización de la ubicación incrustada, el radio de agrupación y el tratamiento de fotos sin ubicación los gobierna el backend (RN-04 de geovial-api); el front presenta y envía, no decide la agrupación.
- El front no persiste fotos ni evidencia; el binario de las fotos lo aloja el backend a través de la librería de almacenamiento (intake §17 geovial-web P.4).

## 13. Interacción multiusuario y concurrencia

- Si otro agente o el jefe cargan o crean marcadores en el mismo relevamiento, el front los verá al recargar; las cargas conviven sin bloquearse.
- Las fotos que caen dentro del radio de marcadores creados por otra persona se agrupan en esos marcadores; los marcadores próximos conviven como conflicto hasta la resolución al cierre.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de carga manual completa del relevamiento vía web por el agente, derivado de NB-02 y del alcance F-15. |
