# CU-08 — Administrar observaciones con notas, fotos, comentarios y etiquetas

**Proyecto:** geovial-api
**Documento:** CU-08-administrar-observaciones_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir registrar y administrar observaciones del estado de un tramo vial ancladas a un marcador geográfico, compuestas por notas, fotos, un comentario y una etiqueta por foto. Estructura la evidencia para que un mismo punto del tramo concentre toda la información asociada.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Agente de campo | Primario | Registra y administra observaciones, fotos, comentarios y etiquetas |
| Backend de observaciones | Sistema | Valida el anclaje a marcador, persiste la observación y delega el alojamiento de fotos |
| Almacén de archivos | Sistema | Aloja los binarios de las fotos a través de la librería de almacenamiento |

## 3. Precondiciones

- El solicitante está autenticado (CU-03) y tiene acceso al relevamiento como agente asignado o como jefe dueño.
- Existe el marcador al que se anclará la observación (CU-07).
- El relevamiento no está cerrado.

## 4. Flujo principal

1. El agente crea una observación indicando el marcador al que se ancla y una nota descriptiva.
2. El backend valida que el marcador existe y pertenece al relevamiento accesible por el solicitante (RC de referencia observación a marcador).
3. El backend registra la observación vinculada al marcador y a su autor.
4. El agente agrega una o más fotos a la observación; por cada foto puede registrar un comentario y una etiqueta.
5. El backend delega el alojamiento del binario de cada foto a la librería de almacenamiento y conserva la referencia lógica de la foto junto con su comentario y etiqueta.
6. El backend responde con la observación, sus fotos y la ubicación de cada recurso creado.

## 5. Flujos alternativos

- 5.A Marcador compartido por varias observaciones. Disparador: el agente ancla una nueva observación a un marcador que ya tiene observaciones. El backend acepta el anclaje compartido sin duplicar el marcador (RC de identidad de marcador). Retorna al paso 3.
- 5.B Baja de una foto de la observación. Disparador: el agente quita una foto. El backend desvincula la foto de la observación y solicita la eliminación del binario al almacén de archivos, conservando el resto de la observación. Retorna al paso 6.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| MARCADOR_INEXISTENTE | La observación referencia un marcador que no existe en el relevamiento | Rechaza con estado de solicitud inválida y no crea la observación (RC de referencia) |
| FOTO_NO_ALMACENABLE | El almacén de archivos no puede alojar el binario de la foto | Conserva la observación sin esa foto y devuelve un problema indicando la foto no alojada |
| RELEVAMIENTO_CERRADO | El relevamiento está cerrado y no admite nuevas observaciones | Rechaza con estado de conflicto y no crea ni modifica la observación |

## 7. Postcondiciones

- Éxito: la observación queda anclada a su marcador, con sus notas, fotos, comentarios y etiquetas, y cada foto alojada en el almacén de archivos.
- Fallo total: no se crea la observación. Fallo parcial de una foto: la observación persiste sin esa foto, señalada en la respuesta.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un agente con acceso a un marcador existente | Crea una observación con una nota y dos fotos, cada una con comentario y etiqueta | El backend ancla la observación al marcador, aloja las dos fotos y responde con la observación completa |
| CA-02 | Un marcador que ya tiene una observación | El agente ancla una segunda observación al mismo marcador | El backend acepta el anclaje compartido sin duplicar el marcador |
| CA-03 | Una observación cuya foto no puede alojarse en el almacén | El agente intenta agregar esa foto | El backend conserva la observación sin la foto y devuelve el código FOTO_NO_ALMACENABLE |
| CA-04 | Una observación que referencia un marcador inexistente | El agente intenta crearla | El backend rechaza con el código MARCADOR_INEXISTENTE |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-03 |
| Reglas de negocio aplicables | RN-03, RN-04 |
| Historias de usuario a generar | US-16, US-17, US-18 (en 06) |
| Componentes esperados | Recurso de observaciones; recurso de fotos, comentarios y etiquetas; integración con la librería de almacenamiento; repositorio de observaciones (referencia tentativa a 05) |
| Tests previstos | Observación con notas y fotos completas; marcador compartido por observaciones; foto no almacenable señalada; marcador inexistente rechazado (en 08) |

## 10. Notas y supuestos

- Toda observación se ancla a un marcador; no existe observación sin marcador (RC de referencia observación a marcador).
- El alojamiento físico de las fotos lo resuelve la librería de almacenamiento; este CU solo conserva la referencia lógica y los metadatos de comentario y etiqueta.
- La meta de integridad de la evidencia (foto, ubicación y etiqueta completas) proviene de los criterios de éxito de NB-03.

## 12. Performance esperado del CU

- El registro de la observación debe resolverse dentro del objetivo de escritura (p95 menor o igual a 500 ms); el alojamiento de cada foto depende del proveedor de almacenamiento configurado.

## 15. Idempotencia y reintento

- La creación de observación y el alta de cada foto admiten clave de idempotencia, indispensable porque la captura sin conexión sube estos cambios mediante sincronización con reintentos (CU-11, CU-21).
- Reenviar una foto ya alojada bajo la misma referencia lógica no la duplica.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de administración de observaciones, fotos, comentarios y etiquetas, derivado de NB-03 (F-05, F-06). |
