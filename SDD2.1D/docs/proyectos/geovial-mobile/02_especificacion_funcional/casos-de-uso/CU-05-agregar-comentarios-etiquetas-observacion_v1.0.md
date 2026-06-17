# CU-05 — Agregar comentarios y etiquetas a la observación

**Proyecto:** geovial-mobile
**Documento:** CU-05-agregar-comentarios-etiquetas-observacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + Mobile UX Analyst

## 1. Propósito

Permitir que el agente de campo enriquezca la observación recolectada agregando una nota, comentarios por foto y etiquetas a las fotos y al marcador, de modo que la evidencia quede clasificada y descrita en el lugar, en el almacén local y sin conexión. Completa la observación para que el jefe pueda filtrar y revisar después.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Agente de campo | Primario | Escribe la nota, comenta cada foto y aplica etiquetas |
| App móvil | Sistema | Registra notas, comentarios y etiquetas en el almacén local y encola los cambios |

## 3. Precondiciones

- El agente tiene una sesión activa (CU-01) y un relevamiento abierto en recolección (CU-02).
- Existe la observación, su marcador y, al menos, una foto a la cual asociar comentario y etiqueta (CU-03, CU-04).

## 4. Flujo principal

1. El agente abre una observación o una foto del marcador activo.
2. El agente escribe la nota de la observación, un comentario para la foto y aplica una o más etiquetas a la foto o al marcador.
3. La app registra la nota, el comentario y las etiquetas en el almacén local, asociándolos a la observación, a la foto y al marcador correspondientes.
4. La app encola los cambios como pendientes de sincronizar (CU-06).
5. El agente puede reutilizar etiquetas ya existentes del relevamiento o crear nuevas, y puede agregar o quitar comentarios y etiquetas mientras el relevamiento esté en recolección.

## 5. Flujos alternativos

- 5.A Reutilización de una etiqueta existente. Disparador: el agente elige una etiqueta ya usada en el relevamiento. La app la aplica sin duplicarla, de modo que la misma etiqueta marca varias fotos y marcadores. Retorna al paso 4.
- 5.B Comentario sobre una foto sin ubicación resuelta. Disparador: la foto a comentar quedó pendiente de ubicación (CU-04 5.A). La app permite igualmente comentar y etiquetar; la falta de coordenada no impide describir la evidencia. Retorna al paso 4.
- 5.C Edición de un comentario o etiqueta previo. Disparador: el agente corrige un comentario o quita una etiqueta. La app actualiza el registro local y encola el cambio. Retorna al paso 4.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| ETIQUETA_VACIA | El agente intenta crear una etiqueta sin nombre | La app no crea la etiqueta y solicita un nombre |
| OBSERVACION_INEXISTENTE | La observación o la foto a comentar ya no existe en el almacén local | La app no aplica el cambio y avisa que la observación no está disponible |
| RELEVAMIENTO_CERRADO | El relevamiento activo está cerrado | La app no permite agregar ni editar comentarios o etiquetas y lo deja en modo lectura |

## 7. Postcondiciones

- Éxito: la observación tiene su nota, las fotos tienen sus comentarios y las etiquetas quedan aplicadas a fotos y marcadores en el almacén local, con los cambios encolados.
- Éxito en reutilización de etiqueta: la etiqueta existente marca un elemento adicional sin duplicarse.
- Fallo: el registro local no cambia y el agente recibe la indicación correspondiente.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una foto de una observación en un relevamiento en recolección | El agente le escribe el comentario "fisura longitudinal de 30 cm" y le aplica la etiqueta fisura | La app registra el comentario y la etiqueta en el almacén local y encola los cambios |
| CA-02 | Una etiqueta fisura ya usada en el relevamiento | El agente la aplica a otra foto | La app aplica la misma etiqueta sin duplicarla y la deja compartida entre las dos fotos |
| CA-03 | Una foto que quedó pendiente de ubicación precisa | El agente le agrega un comentario y una etiqueta | La app registra el comentario y la etiqueta sin requerir coordenada de la foto |
| CA-04 | Un agente que intenta crear una etiqueta sin nombre | El agente confirma la etiqueta vacía | La app responde con ETIQUETA_VACIA y no crea la etiqueta |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-03 |
| Reglas de negocio aplicables | RN-05, RN-03 |
| Historias de usuario a generar | US-09, US-10 (en 06) |
| Componentes esperados | Editor de nota y comentario; selector y catálogo local de etiquetas; repositorio local de observaciones, comentarios y etiquetas; cola local de cambios (referencia tentativa a 05) |
| Tests previstos | Comentario y etiqueta registrados y encolados; etiqueta reutilizada sin duplicar; comentario sobre foto sin ubicación; etiqueta vacía rechazada (en 08) |

## 10. Notas y supuestos

- Una foto tiene a lo sumo un comentario; una etiqueta puede marcar varias fotos y varios marcadores, alineado con el dominio autoritativo de geovial-api.
- La nota es de la observación, el comentario es por foto y la etiqueta aplica a fotos y a marcadores; este reparto sigue el modelo de observación del backend.
- El filtrado por etiqueta en la revisión es una capacidad del lado de revisión (jefe de área); aquí la app solo registra las etiquetas que ese filtrado consume.
- La interacción táctil y el diseño del editor pertenecen a la categoría 03 (UX/UI).

## 14. Permisos del sistema operativo

- Este CU no requiere permisos de ubicación ni cámara; opera sobre el almacén local de la app introduciendo texto y etiquetas.

## 12. Performance esperado del CU

- El registro de notas, comentarios y etiquetas se resuelve contra el almacén local sin depender de la red; funciona 100 % sin conexión.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de agregado de comentarios y etiquetas a la observación, derivado de NB-03 (F-06). |
