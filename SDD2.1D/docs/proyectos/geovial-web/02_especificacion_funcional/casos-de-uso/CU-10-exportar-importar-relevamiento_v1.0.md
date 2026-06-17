# CU-10 — Exportar e importar un relevamiento completo

**Proyecto:** geovial-web
**Documento:** CU-10-exportar-importar-relevamiento_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional

## 1. Propósito

Permitir que el jefe de área (o el usuario raíz, para mover entre entornos) exporte desde el front web un relevamiento completo —comentarios, etiquetas y fotos— como una única unidad transferible descargable, y que importe esa unidad para reconstruir el relevamiento conservando su estructura. Habilita compartir, archivar y mover relevamientos.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área o usuario raíz | Primario | Solicita exportar un relevamiento y descargar la unidad, o importar una unidad existente |
| Front web | Sistema | Inicia la exportación o importación contra el backend y entrega o recibe la unidad transferible |
| Backend de dominio | Sistema | Empaqueta el relevamiento completo o lo reconstruye desde la unidad recibida |

## 3. Precondiciones

- El solicitante tiene una sesión activa en el front web (CU-01) con rol jefe de área o usuario raíz.
- Para exportar, existe un relevamiento dentro del alcance del solicitante.
- Para importar, el solicitante dispone de una unidad transferible válida.

## 4. Flujo principal

1. El solicitante abre un relevamiento dentro de su alcance y solicita exportarlo desde el front web.
2. El front web pide al backend empaquetar el relevamiento completo en una única unidad transferible.
3. El backend produce la unidad con todos los comentarios, etiquetas y fotos del relevamiento; el front la ofrece para descargar.
4. Para importar, el solicitante selecciona una unidad transferible en la pantalla de importación.
5. El front web envía la unidad al backend, que reconstruye el relevamiento con su estructura.
6. El backend devuelve el relevamiento reconstruido; el front lo muestra en el listado del solicitante con toda su evidencia en su lugar.

## 5. Flujos alternativos

- 5.A Exportación de un relevamiento extenso. Disparador: el relevamiento tiene muchas fotos y el empaquetado tarda. El front web informa que la exportación está en curso y avisa cuando la unidad está lista para descargar. Retorna al paso 3.
- 5.B Importación de una unidad ya presente. Disparador: el relevamiento de la unidad ya existe en el entorno destino. El front web advierte que se trata de un relevamiento ya conocido y solicita confirmar si se importa como copia, según lo que admita el backend. Retorna al paso 5.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| FUERA_DE_ALCANCE | El solicitante intenta exportar un relevamiento que no le pertenece | El front no lo lista; ante el rechazo del backend informa que está fuera de su alcance |
| UNIDAD_INVALIDA | La unidad a importar está dañada o no tiene el formato esperado | El front rechaza la importación e informa que la unidad no es válida |
| IMPORTACION_INCOMPLETA | El backend no pudo reconstruir toda la estructura del relevamiento | El front informa que la importación no se completó y no muestra un relevamiento parcial como válido |

## 7. Postcondiciones

- Éxito en exportación: el solicitante obtiene una única unidad transferible con el relevamiento completo.
- Éxito en importación: el relevamiento queda reconstruido en el entorno destino con toda su evidencia y estructura.
- Fallo: no se produce la unidad o no se reconstruye el relevamiento, y el front informa la causa.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un jefe de área con un relevamiento cerrado con fotos, comentarios y etiquetas | Solicita exportarlo | El front entrega una única unidad transferible para descargar con todo el relevamiento |
| CA-02 | Un usuario raíz con una unidad transferible válida en otro entorno | La importa desde el front web | El front muestra el relevamiento reconstruido con toda su evidencia en su lugar |
| CA-03 | Un usuario con una unidad dañada | La intenta importar | El front rechaza con UNIDAD_INVALIDA y no crea un relevamiento parcial |
| CA-04 | Un jefe de área que intenta exportar un relevamiento de otro jefe | Abre la exportación | El front no lo lista (FUERA_DE_ALCANCE) |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-06 |
| Reglas de negocio aplicables | RN-01 (geovial-web) |
| Historias de usuario a generar | US-22, US-23 (en 06) |
| Componentes esperados | Acción de exportación con descarga; pantalla de importación con carga de unidad; consumo de los recursos de exportación e importación del backend (referencia tentativa a 05) |
| Tests previstos | Exportación produce unidad única; importación reconstruye estructura; unidad inválida rechazada; exportación fuera de alcance (en 08) |

## 10. Notas y supuestos

- La portabilidad es Could Have (NB-06, intake §4 F-16): se incorpora si la cadencia lo permite y no integra el camino principal del relevamiento.
- El formato y el empaquetado de la unidad transferible los define el backend (CU-15 y CU-16 de geovial-api); el front inicia la operación y maneja la descarga o la carga de la unidad.
- El front no persiste el relevamiento ni la unidad; consume el contrato del backend (intake §17 geovial-web P.4).

## 13. Interacción multiusuario y concurrencia

- La exportación es una lectura que no altera el relevamiento; puede convivir con otras consultas sobre el mismo relevamiento.
- Si dos usuarios importan la misma unidad, el backend define si se crean copias o se trata como ya conocido; el front refleja esa decisión.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de exportación e importación de relevamiento completo desde el front web, derivado de NB-06 (F-16). |
