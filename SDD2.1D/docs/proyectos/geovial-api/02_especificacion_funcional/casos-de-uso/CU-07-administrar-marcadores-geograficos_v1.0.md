# CU-07 — Administrar marcadores geográficos del relevamiento

**Proyecto:** geovial-api
**Documento:** CU-07-administrar-marcadores-geograficos_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir crear, mover, etiquetar y dar de baja marcadores geográficos dentro de un relevamiento, como puntos del mapa que agrupan observaciones, fotos, comentarios y textos. El marcador es el ancla geográfica de la evidencia y puede ser compartido por varias observaciones.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Agente de campo o jefe de área | Primario | Crea, mueve, etiqueta y da de baja marcadores |
| Backend de marcadores | Sistema | Valida la pertenencia al relevamiento, persiste el marcador y sus etiquetas |
| Almacén relacional | Sistema | Persiste el marcador, su coordenada y sus etiquetas |

## 3. Precondiciones

- El solicitante está autenticado (CU-03) y tiene acceso al relevamiento: es el jefe dueño o un agente asignado.
- El relevamiento existe y no está cerrado.

## 4. Flujo principal

1. El solicitante crea un marcador en el relevamiento indicando su coordenada geográfica y, opcionalmente, etiquetas.
2. El backend valida que el solicitante tiene acceso al relevamiento y que la coordenada es válida.
3. El backend crea el marcador con identidad propia, lo vincula al relevamiento y registra su coordenada y etiquetas (RC de identidad de marcador).
4. El backend responde con la representación del marcador y la ubicación del recurso.
5. El solicitante puede mover el marcador a una nueva coordenada, agregar o quitar etiquetas y darlo de baja; el backend aplica cada cambio conservando la identidad del marcador.

## 5. Flujos alternativos

- 5.A Creación dentro del radio de otro marcador. Disparador: la coordenada del nuevo marcador cae dentro del radio de agrupación de un marcador existente. El backend crea el marcador igualmente y registra la situación como conflicto de marcadores que convive con la operación, sin bloquear (RN-03). Retorna al paso 4.
- 5.B Baja de un marcador con observaciones. Disparador: el marcador a dar de baja tiene observaciones vinculadas. El backend impide la baja directa para no dejar observaciones huérfanas y solicita reasignar o dar de baja antes las observaciones (RC de referencia observación a marcador). Termina con un problema explicativo.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| COORDENADA_INVALIDA | La coordenada indicada está fuera del rango geográfico admitido | Rechaza con estado de solicitud inválida y no crea ni mueve el marcador |
| RELEVAMIENTO_CERRADO | El relevamiento está cerrado y no admite cambios en marcadores | Rechaza con estado de conflicto y no modifica el marcador |
| MARCADOR_CON_OBSERVACIONES | La baja del marcador dejaría observaciones sin ancla | Rechaza con estado de conflicto y conserva el marcador (RC de referencia) |

## 7. Postcondiciones

- Éxito en creación: existe un marcador con identidad propia, coordenada y etiquetas, vinculado al relevamiento.
- Éxito en movimiento o etiquetado: el marcador conserva su identidad y refleja la nueva coordenada o sus etiquetas.
- Éxito en baja: el marcador sin observaciones queda dado de baja.
- Fallo: el estado de marcadores no cambia y se devuelve un problema con el código correspondiente.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un agente asignado a un relevamiento en recolección | Crea un marcador con una coordenada válida y una etiqueta | El backend crea el marcador con identidad propia y responde con la ubicación del recurso |
| CA-02 | Un marcador existente con su radio de agrupación | El agente crea otro marcador dentro de ese radio | El backend crea el segundo marcador y registra el conflicto sin bloquear la operación |
| CA-03 | Un marcador con dos observaciones vinculadas | El agente intenta dar de baja el marcador | El backend rechaza con el código MARCADOR_CON_OBSERVACIONES y conserva el marcador |
| CA-04 | Un marcador de un relevamiento cerrado | El jefe intenta moverlo | El backend rechaza con el código RELEVAMIENTO_CERRADO |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-03 |
| Reglas de negocio aplicables | RN-03, RN-04 |
| Historias de usuario a generar | US-14, US-15 (en 06) |
| Componentes esperados | Recurso de marcadores; servicio de validación de coordenada y de conflicto por radio; repositorio de marcadores (referencia tentativa a 05) |
| Tests previstos | Creación de marcador con identidad propia; convivencia con conflicto por radio; baja con observaciones rechazada; cambio en relevamiento cerrado rechazado (en 08) |

## 10. Notas y supuestos

- La identidad del marcador es estable: mover el marcador o cambiar sus etiquetas no genera un marcador nuevo (RC de identidad de marcador).
- El conflicto de marcadores por radio convive durante la recolección y se resuelve al cierre (RN-03 y CU-13); este CU solo lo registra, no lo resuelve.
- El radio de agrupación se aplica de forma plena en la carga manual de fotos (CU-09 y RN-04).

## 12. Performance esperado del CU

- La creación y el movimiento de un marcador deben resolverse dentro del objetivo de escritura (p95 menor o igual a 500 ms).

## 15. Idempotencia y reintento

- La creación admite clave de idempotencia para evitar marcadores duplicados ante reintentos, especialmente al subir cambios desde la captura sin conexión (CU-11, CU-21).
- El movimiento y el etiquetado convergen al mismo estado al repetirse con los mismos valores.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de administración de marcadores geográficos, derivado de NB-03 (F-06, F-10). |
