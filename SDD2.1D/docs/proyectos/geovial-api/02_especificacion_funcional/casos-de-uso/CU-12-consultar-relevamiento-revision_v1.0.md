# CU-12 — Consultar el relevamiento para la revisión sobre mapa

**Proyecto:** geovial-api
**Documento:** CU-12-consultar-relevamiento-revision_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que el jefe de área consulte el relevamiento completo para revisarlo sobre el mapa: sus marcadores con sus coordenadas, sus observaciones, las fotos de cada marcador en orden, los marcadores contiguos y las etiquetas para filtrar. Provee al cliente web los datos para recorrer la evidencia en su contexto geográfico y armar el informe.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área | Primario | Consulta el relevamiento para revisarlo sobre el mapa |
| Backend de relevamientos | Sistema | Devuelve marcadores, observaciones, fotos y etiquetas con su orden geográfico |
| Almacén relacional y de archivos | Sistema | Aporta los datos del relevamiento y las referencias de fotos |

## 3. Precondiciones

- El solicitante está autenticado y su rol es jefe de área, dueño del relevamiento (CU-03).
- El relevamiento existe y se encuentra en un estado consultable (recolección, revisión o cierre).

## 4. Flujo principal

1. El jefe solicita el detalle del relevamiento para revisión, opcionalmente filtrando por una o varias etiquetas.
2. El backend valida el alcance del jefe sobre el relevamiento.
3. El backend devuelve los marcadores del relevamiento con su coordenada, sus observaciones y las fotos de cada marcador en orden, junto con las etiquetas presentes.
4. El backend incluye, por cada marcador, la referencia al marcador contiguo siguiente y anterior, para encadenar el recorrido y el carrusel de fotos.
5. Si se indicaron etiquetas, el backend devuelve solo los marcadores y fotos que las portan.

## 5. Flujos alternativos

- 5.A Filtro sin coincidencias. Disparador: el filtro por etiqueta no coincide con ningún marcador ni foto. El backend devuelve un conjunto vacío y la lista de etiquetas disponibles para reorientar el filtro. Retorna al paso 5.
- 5.B Marcadores en conflicto presentes. Disparador: el relevamiento tiene marcadores en conflicto sin resolver. El backend los entrega marcados como en conflicto, sin ocultarlos, manteniendo la información accesible durante la revisión (RN-03). Retorna al paso 4.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| RELEVAMIENTO_FUERA_DE_AMBITO | El relevamiento no pertenece al jefe solicitante | Rechaza con estado de prohibido y no expone el relevamiento |
| ETIQUETA_DESCONOCIDA | El filtro referencia una etiqueta inexistente en el relevamiento | Devuelve un conjunto vacío e informa las etiquetas válidas, sin error duro |
| RELEVAMIENTO_INEXISTENTE | El relevamiento solicitado no existe | Rechaza con estado de no encontrado |

## 7. Postcondiciones

- Éxito: el jefe recibe los marcadores, observaciones, fotos y etiquetas del relevamiento, con el encadenado de marcadores contiguos y aplicando el filtro indicado.
- Fallo: no se expone el relevamiento y se devuelve un problema con el código correspondiente.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un jefe con un relevamiento de 10 marcadores | Solicita el detalle para revisión | El backend devuelve los 10 marcadores con sus fotos en orden y el encadenado al marcador contiguo siguiente y anterior |
| CA-02 | Un relevamiento con marcadores etiquetados como fisura y otros sin esa etiqueta | El jefe filtra por la etiqueta fisura | El backend devuelve solo los marcadores y fotos con la etiqueta fisura |
| CA-03 | Un relevamiento con un par de marcadores en conflicto | El jefe solicita el detalle | El backend entrega los marcadores en conflicto señalados como tales, sin ocultarlos |
| CA-04 | Un relevamiento de otro jefe | El jefe actual solicita su detalle | El backend rechaza con el código RELEVAMIENTO_FUERA_DE_AMBITO |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-05 |
| Reglas de negocio aplicables | RN-01, RN-03 |
| Historias de usuario a generar | US-25, US-26 (en 06) |
| Componentes esperados | Recurso de consulta de relevamiento para revisión; servicio de encadenado de marcadores contiguos; servicio de filtro por etiqueta (referencia tentativa a 05) |
| Tests previstos | Detalle con encadenado de marcadores; filtro por etiqueta; conflicto visible en revisión; acceso fuera de ámbito rechazado (en 08) |

## 10. Notas y supuestos

- El carrusel de fotos por marcador y el encadenado al marcador siguiente y anterior (F-12) se renderizan en el cliente web; este CU provee los datos y el orden necesarios.
- El backend mantiene la información accesible aun con marcadores en conflicto (RN-03); la resolución es CU-13.
- La meta de cobertura de revisión sobre mapa (criterio de NB-05) exige que todos los marcadores del relevamiento sean consultables.

## 12. Performance esperado del CU

- La consulta del detalle para revisión debe mantenerse dentro del objetivo de lectura del proyecto (p95 menor o igual a 300 ms) en relevamientos de tamaño habitual, con paginación de fotos cuando el volumen lo requiera (CU-20).

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de consulta del relevamiento para la revisión sobre mapa, derivado de NB-05 (F-10, F-12). |
