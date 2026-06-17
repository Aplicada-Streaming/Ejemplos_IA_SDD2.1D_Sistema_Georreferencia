# CU-15 — Exportar un relevamiento completo en una unidad transferible única

**Proyecto:** geovial-api
**Documento:** CU-15-exportar-relevamiento_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que el jefe de área exporte un relevamiento completo —marcadores, observaciones, fotos, comentarios y etiquetas— como una única unidad transferible, para compartirlo, auditarlo o archivarlo fuera del sistema conservando la correspondencia entre todas sus piezas.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área | Primario | Solicita la exportación de un relevamiento |
| Backend de portabilidad | Sistema | Empaqueta el relevamiento completo en una unidad transferible |
| Almacén relacional y de archivos | Sistema | Aporta los datos y los binarios de las fotos a empaquetar |

## 3. Precondiciones

- El solicitante está autenticado y es el jefe dueño del relevamiento (CU-03).
- El relevamiento existe y está cerrado (preferentemente) o estructurado de forma consistente.

## 4. Flujo principal

1. El jefe solicita exportar un relevamiento.
2. El backend valida el alcance del jefe sobre el relevamiento.
3. El backend reúne los marcadores, observaciones, comentarios, etiquetas y las fotos del relevamiento.
4. El backend empaqueta todo en una única unidad transferible, conservando las referencias entre fotos, comentarios, etiquetas, observaciones y marcadores.
5. El backend entrega la unidad transferible como un único archivo descargable.

## 5. Flujos alternativos

- 5.A Exportación con fotos en almacenamiento remoto. Disparador: las fotos del relevamiento están en un proveedor de almacenamiento remoto. El backend recupera los binarios a través de la librería de almacenamiento y los incluye en la unidad, de forma transparente al destino configurado (RN sobre transparencia del almacenamiento, en geovial-storage). Retorna al paso 4.
- 5.B Exportación de un relevamiento voluminoso. Disparador: el relevamiento supera un umbral de tamaño. El backend genera la unidad de forma diferida y entrega un identificador para descargarla cuando esté lista, sin bloquear la solicitud. Termina con la entrega diferida.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| RELEVAMIENTO_FUERA_DE_AMBITO | El relevamiento no pertenece al jefe solicitante | Rechaza con estado de prohibido y no exporta |
| FOTO_NO_RECUPERABLE | Una foto del relevamiento no puede recuperarse del almacén | Detiene la exportación e informa la foto faltante, para no entregar una unidad incompleta |
| RELEVAMIENTO_INEXISTENTE | El relevamiento solicitado no existe | Rechaza con estado de no encontrado |

## 7. Postcondiciones

- Éxito: existe una unidad transferible única con la totalidad de los comentarios, etiquetas y fotos del relevamiento, con su estructura conservada.
- Fallo: no se entrega ninguna unidad y se devuelve un problema con el código correspondiente.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un jefe con un relevamiento cerrado de varios marcadores y fotos | Solicita exportarlo | El backend entrega una única unidad transferible con el 100 por ciento de comentarios, etiquetas y fotos |
| CA-02 | Un relevamiento cuyas fotos están en un proveedor remoto | El jefe lo exporta | El backend recupera las fotos del proveedor y las incluye en la unidad de forma transparente |
| CA-03 | Un relevamiento con una foto que no puede recuperarse del almacén | El jefe lo exporta | El backend detiene la exportación con el código FOTO_NO_RECUPERABLE y no entrega una unidad incompleta |
| CA-04 | Un relevamiento de otro jefe | El jefe actual intenta exportarlo | El backend rechaza con el código RELEVAMIENTO_FUERA_DE_AMBITO |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-06 |
| Reglas de negocio aplicables | RN-01 |
| Historias de usuario a generar | US-31, US-32 (en 06) |
| Componentes esperados | Recurso de exportación; empaquetador de relevamiento; integración con la librería de almacenamiento (referencia tentativa a 05) |
| Tests previstos | Exportación completa en una unidad; inclusión transparente de fotos remotas; foto no recuperable detiene exportación; exportación fuera de ámbito rechazada (en 08) |

## 10. Notas y supuestos

- La capacidad de portabilidad es Could Have (NB-06); se incorpora si la cadencia del proyecto lo permite, sin comprometer el camino principal.
- La unidad transferible es una sola, de acuerdo con el criterio de NB-06 (una unidad por relevamiento exportado).
- El formato concreto del empaquetado pertenece a la categoría 05; aquí solo se fija que es una unidad única autocontenida.

## 12. Performance esperado del CU

- El esfuerzo de exportar y entregar un relevamiento completo debe mantenerse en el orden de pocos minutos para relevamientos de tamaño habitual, según el criterio de NB-06.

## 15. Idempotencia y reintento

- La exportación es una operación segura sobre el relevamiento: repetirla no altera su estado y produce una unidad equivalente.
- En la modalidad diferida, reintentar con el mismo identificador devuelve la unidad ya generada sin regenerarla.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de exportación del relevamiento completo, derivado de NB-06 (F-16). |
