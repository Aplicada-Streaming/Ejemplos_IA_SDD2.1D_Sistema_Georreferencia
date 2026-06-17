# CU-09 — Cargar fotos manualmente con priorización de ubicación y radio de agrupación

**Proyecto:** geovial-api
**Documento:** CU-09-cargar-fotos-manualmente_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir la carga manual posterior de fotos a un relevamiento, resolviendo la ubicación de cada foto a partir de los datos de ubicación que la propia imagen trae consigo y agrupándolas en marcadores según un radio de agrupación. Evita reubicar cada foto a mano y aprovecha la georreferenciación incrustada en la imagen.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Agente de campo | Primario | Carga manualmente un conjunto de fotos a un relevamiento |
| Backend de carga manual | Sistema | Lee los datos de ubicación de la foto, agrupa por radio y crea o reutiliza marcadores |
| Almacén de archivos | Sistema | Aloja los binarios de las fotos cargadas |

## 3. Precondiciones

- El solicitante está autenticado (CU-03) y tiene acceso al relevamiento.
- El relevamiento no está cerrado.
- Se define un radio de agrupación aplicable a la carga (parámetro de configuración del relevamiento o del sistema).

## 4. Flujo principal

1. El agente carga un conjunto de fotos al relevamiento.
2. Por cada foto, el backend extrae la coordenada de los datos de ubicación incrustados en la imagen y la prioriza como ubicación de la foto (RN-04).
3. El backend busca un marcador existente dentro del radio de agrupación respecto de esa coordenada.
4. Si existe un marcador dentro del radio, el backend agrupa la foto en ese marcador; si no existe, crea un marcador nuevo en la coordenada de la foto.
5. El backend crea u asocia la observación correspondiente y aloja el binario en el almacén de archivos.
6. El backend responde con el resultado de la carga: fotos agrupadas en marcadores existentes, marcadores nuevos creados y fotos sin ubicación resuelta.

## 5. Flujos alternativos

- 5.A Foto sin datos de ubicación. Disparador: una foto cargada no trae coordenada incrustada. El backend la registra como pendiente de ubicación manual, sin agruparla por radio, y la incluye en la respuesta como foto sin ubicación resuelta. Retorna al paso 6.
- 5.B Varias fotos en el mismo radio. Disparador: varias fotos del lote caen dentro del radio del mismo marcador. El backend las agrupa todas en ese único marcador, que queda compartido por las observaciones resultantes (RN-04, RC de identidad de marcador). Retorna al paso 6.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| RADIO_NO_DEFINIDO | No hay un radio de agrupación aplicable a la carga | Rechaza con estado de solicitud inválida y no procesa el lote |
| FORMATO_FOTO_NO_SOPORTADO | Una foto del lote tiene un formato que el sistema no puede procesar | Omite esa foto, continúa con el resto y la señala en la respuesta |
| RELEVAMIENTO_CERRADO | El relevamiento está cerrado y no admite carga de fotos | Rechaza con estado de conflicto y no procesa el lote |

## 7. Postcondiciones

- Éxito: cada foto con ubicación queda agrupada en un marcador dentro del radio o en un marcador nuevo; las fotos sin ubicación quedan pendientes de ubicación manual; los binarios quedan alojados.
- Fallo del lote: no se crean marcadores ni observaciones y se devuelve un problema con el código correspondiente.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un radio de agrupación definido y tres fotos con coordenada dentro de ese radio | El agente carga las tres fotos | El backend las agrupa en un único marcador y reporta cero marcadores nuevos adicionales para esas fotos |
| CA-02 | Un radio definido y una foto con coordenada lejana a todo marcador existente | El agente carga la foto | El backend crea un marcador nuevo en la coordenada de la foto y agrupa la foto en él |
| CA-03 | Un lote con una foto sin datos de ubicación incrustados | El agente carga el lote | El backend registra esa foto como pendiente de ubicación manual y la incluye en la respuesta como sin ubicación resuelta |
| CA-04 | Una carga sin radio de agrupación aplicable | El agente intenta cargar fotos | El backend rechaza con el código RADIO_NO_DEFINIDO |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-03 |
| Reglas de negocio aplicables | RN-04, RN-03 |
| Historias de usuario a generar | US-19, US-20 (en 06) |
| Componentes esperados | Recurso de carga manual; servicio de extracción de ubicación incrustada; servicio de agrupación por radio; integración con el almacén de archivos (referencia tentativa a 05) |
| Tests previstos | Agrupación de fotos dentro del radio; creación de marcador para foto lejana; foto sin ubicación queda pendiente; carga sin radio rechazada (en 08) |

## 10. Notas y supuestos

- La priorización de la ubicación incrustada en la foto y el radio de agrupación son la invariante RN-04.
- La carga manual completa del relevamiento desde el entorno web (F-15) se apoya en este mismo recurso de carga.
- Las fotos sin ubicación resuelta requieren intervención posterior del agente o del jefe para ubicarlas en el mapa; ese ajuste manual usa el recurso de marcadores (CU-07).

## 12. Performance esperado del CU

- El procesamiento de la carga debe escalar con el tamaño del lote sin degradar el resto del sistema; el alojamiento de cada binario depende del proveedor de almacenamiento.

## 15. Idempotencia y reintento

- La carga admite clave de idempotencia por lote para que un reintento no duplique fotos ni marcadores ya creados (CU-21).
- Reenviar una foto idéntica ya agrupada bajo la misma referencia lógica no la duplica ni crea un marcador adicional.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de carga manual de fotos con ubicación incrustada y radio de agrupación, derivado de NB-03 (F-09, F-15). |
