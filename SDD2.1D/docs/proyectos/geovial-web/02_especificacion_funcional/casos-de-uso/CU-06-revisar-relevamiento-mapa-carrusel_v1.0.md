# CU-06 — Revisar el relevamiento sobre el mapa con carrusel de fotos

**Proyecto:** geovial-web
**Documento:** CU-06-revisar-relevamiento-mapa-carrusel_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional

## 1. Propósito

Permitir que el jefe de área revise sobre el mapa del front web la evidencia recolectada de un relevamiento: recorrer los marcadores en su contexto geográfico, ver en un carrusel las fotos de cada marcador encadenando con el marcador siguiente y anterior, ampliar fotos, leer sus comentarios y filtrar por etiqueta. Ordena la evidencia para confeccionar el informe de cierre.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área | Primario | Recorre marcadores, navega el carrusel de fotos y filtra por etiqueta |
| Front web | Sistema | Presenta el mapa, el carrusel y los filtros, consumiendo del backend la evidencia del relevamiento |
| Backend de dominio | Sistema | Entrega los marcadores, observaciones, fotos, comentarios y etiquetas del relevamiento |

## 3. Precondiciones

- El jefe de área tiene una sesión activa en el front web (CU-01) y es el dueño del relevamiento.
- El relevamiento tiene evidencia sincronizada disponible para revisar.

## 4. Flujo principal

1. El jefe de área abre un relevamiento en revisión sobre el componente de mapa del front web.
2. El front web consume del backend los marcadores del relevamiento con sus observaciones, fotos, comentarios y etiquetas, y los presenta sobre el mapa.
3. El jefe selecciona un marcador; el front web abre el carrusel de fotos de ese marcador con sus comentarios.
4. El jefe avanza y retrocede en el carrusel; al llegar al extremo, el front encadena con las fotos del marcador contiguo.
5. El jefe amplía una foto para verla en detalle y lee su comentario y etiqueta.
6. El jefe aplica un filtro por etiqueta; el front muestra solo los marcadores y fotos que llevan esa etiqueta.
7. El jefe recorre así la evidencia hasta tener el panorama necesario para su informe.

## 5. Flujos alternativos

- 5.A Marcador sin fotos. Disparador: el jefe selecciona un marcador que aún no tiene fotos. El front web informa que el marcador no tiene fotos y ofrece pasar al marcador contiguo. Retorna al paso 3.
- 5.B Evidencia en conflicto presente. Disparador: el relevamiento tiene marcadores en conflicto. El front web muestra todos los marcadores y su evidencia sin bloquear, señalando que hay conflictos pendientes de resolver al cierre. Retorna al paso 2.
- 5.C Filtro sin coincidencias. Disparador: el jefe filtra por una etiqueta que ningún marcador lleva. El front informa que no hay coincidencias y permite limpiar el filtro. Retorna al paso 6.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| FUERA_DE_ALCANCE | El jefe intenta revisar un relevamiento que no le pertenece | El front no lo abre; ante el rechazo del backend informa que está fuera de su alcance |
| EVIDENCIA_NO_DISPONIBLE | El backend no puede entregar la evidencia del relevamiento en ese momento | El front informa que la evidencia no está disponible y ofrece reintentar |
| FOTO_NO_DISPONIBLE | El binario de una foto no se puede recuperar del almacén | El front muestra un marcador de foto no disponible y continúa el carrusel con el resto |

## 7. Postcondiciones

- Éxito: el jefe revisó los marcadores y su evidencia en su contexto geográfico, con el carrusel encadenado y los filtros aplicados; el estado del relevamiento no cambia por la revisión.
- Fallo: la revisión no se completa y el front informa la causa, sin alterar la evidencia.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un relevamiento en revisión con tres marcadores con fotos | El jefe selecciona el primer marcador y avanza en el carrusel hasta el final | El front encadena con las fotos del marcador contiguo sin cerrar el carrusel |
| CA-02 | Un relevamiento con fotos etiquetadas "fisura" y otras sin esa etiqueta | El jefe filtra por la etiqueta "fisura" | El front muestra solo los marcadores y fotos etiquetados "fisura" |
| CA-03 | Un relevamiento con dos marcadores en conflicto dentro de un radio | El jefe abre la revisión sobre el mapa | El front muestra toda la evidencia accesible y señala que hay conflictos pendientes de resolver al cierre |
| CA-04 | Un marcador sin fotos cargadas | El jefe lo selecciona | El front informa que no tiene fotos y ofrece pasar al marcador contiguo |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-05 |
| Reglas de negocio aplicables | RN-01 (geovial-web), RN-02 (geovial-web), RN-04 (geovial-web) |
| Historias de usuario a generar | US-13, US-14, US-15 (en 06) |
| Componentes esperados | Componente de mapa de revisión; carrusel de fotos encadenado; filtros por etiqueta; consumo del recurso de revisión del backend (referencia tentativa a 05) |
| Tests previstos | Carrusel encadena al marcador contiguo; filtro por etiqueta; evidencia accesible con conflictos presentes; marcador sin fotos (en 08) |

## 10. Notas y supuestos

- El detalle visual del carrusel, del mapa y de los filtros (animaciones, controles, layout) pertenece a la categoría 03; aquí se fija el qué de la experiencia de revisión.
- La convivencia con conflictos durante la revisión la garantiza el backend (RN-03 de geovial-api); este CU presenta la evidencia sin bloquear y la resolución se hace en CU-07.
- El front no persiste evidencia; consume el contrato de revisión del backend (intake §17 geovial-web P.4).

## 13. Interacción multiusuario y concurrencia

- Si un agente sincroniza nueva evidencia mientras el jefe revisa, el jefe la verá al recargar; la revisión no bloquea la sincronización ni viceversa.
- Dos jefes no comparten relevamientos; cada uno revisa solo los propios por alcance de rol.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de revisión sobre mapa con carrusel de fotos desde el front web, derivado de NB-05 (F-12). |
