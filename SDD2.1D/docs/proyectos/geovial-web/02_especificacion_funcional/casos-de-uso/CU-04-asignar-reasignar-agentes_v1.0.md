# CU-04 — Asignar y reasignar agentes a un relevamiento

**Proyecto:** geovial-web
**Documento:** CU-04-asignar-reasignar-agentes_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional

## 1. Propósito

Permitir que el jefe de área asigne, desde el front web, a uno o varios agentes de campo de su ámbito a un relevamiento, y que reacomode esas asignaciones (reasignar o quitar agentes) cuando cambian las condiciones de campo, sin perder lo ya recolectado. Reparte el trabajo de campo de forma trazable.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área | Primario | Asigna, reasigna y quita agentes de un relevamiento |
| Front web | Sistema | Presenta el relevamiento, sus agentes asignados y los disponibles, y envía las asignaciones al backend |
| Backend de dominio | Sistema | Valida el alcance y la unicidad, crea o revoca la asignación y devuelve el estado |

## 3. Precondiciones

- El jefe de área tiene una sesión activa en el front web (CU-01).
- Existe un relevamiento del jefe y al menos un agente de campo de su ámbito.

## 4. Flujo principal

1. El jefe de área abre un relevamiento y la sección de agentes asignados en el front web.
2. El front web solicita al backend los agentes ya asignados y los agentes disponibles del ámbito del jefe, y los presenta.
3. El jefe selecciona uno o más agentes disponibles y confirma la asignación.
4. El front web envía cada asignación al backend; el backend la crea y el front actualiza la lista de asignados.
5. Para reasignar, el jefe quita un agente de la lista de asignados y agrega otro disponible.
6. El front web envía la revocación y la nueva asignación al backend y refleja el resultado, conservando lo ya recolectado por el agente saliente.

## 5. Flujos alternativos

- 5.A Agente ya asignado al mismo relevamiento. Disparador: el jefe intenta asignar a un agente que ya está asignado a ese relevamiento. El front web no ofrece a ese agente entre los disponibles; si la operación llegara al backend, este la trata como sin efecto y el front mantiene una sola asignación. Retorna al paso 4.
- 5.B Quitar al único agente asignado. Disparador: el jefe quita al último agente de un relevamiento en recolección. El front web advierte que el relevamiento quedará sin agentes asignados y pide confirmación antes de enviar la revocación. Retorna al paso 5.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| FUERA_DE_ALCANCE | El jefe intenta asignar un agente que no pertenece a su ámbito o un relevamiento ajeno | El front no ofrece ese agente ni ese relevamiento; ante el rechazo del backend informa que está fuera de su alcance |
| ASIGNACION_DUPLICADA | Se intenta asignar dos veces el mismo agente al mismo relevamiento | El front mantiene una sola asignación y no duplica el vínculo |
| RELEVAMIENTO_CERRADO | El jefe intenta asignar o reasignar en un relevamiento ya cerrado | El front presenta la sección en solo lectura y no envía cambios de asignación |

## 7. Postcondiciones

- Éxito en asignación: el agente queda vinculado al relevamiento y habilitado para recolectar en él.
- Éxito en reasignación: el agente saliente pierde la asignación conservando lo recolectado, y el entrante queda asignado.
- Fallo: las asignaciones del relevamiento no cambian y el front informa la causa.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un relevamiento "Tramo Norte" en recolección y dos agentes disponibles del jefe | El jefe asigna a ambos agentes | El front los muestra como asignados al relevamiento |
| CA-02 | Un agente "agente.lopez" ya asignado al "Tramo Norte" | El jefe intenta asignarlo de nuevo al mismo relevamiento | El front mantiene una sola asignación (ASIGNACION_DUPLICADA) |
| CA-03 | Un relevamiento con "agente.lopez" asignado que ya cargó observaciones | El jefe lo reemplaza por "agente.gomez" | El front muestra a gomez asignado, a lopez sin asignación y las observaciones de lopez conservadas |
| CA-04 | Un relevamiento ya cerrado | El jefe intenta asignar un agente | El front presenta la sección en solo lectura (RELEVAMIENTO_CERRADO) y no envía cambios |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-02 |
| Reglas de negocio aplicables | RN-01 (geovial-web), RN-04 (geovial-web) |
| Historias de usuario a generar | US-09, US-10 (en 06) |
| Componentes esperados | Sección de agentes del relevamiento; selector de agentes disponibles; consumo del recurso de asignaciones del backend (referencia tentativa a 05) |
| Tests previstos | Asignación de varios agentes; rechazo de asignación duplicada; reasignación conserva lo recolectado; sección en solo lectura al cierre (en 08) |

## 10. Notas y supuestos

- La reasignación desde la aplicación de campo (F-14) es un flujo del proyecto móvil; este CU cubre la asignación y reasignación desde el front web.
- La unicidad del vínculo agente-relevamiento y el alcance jerárquico los garantiza el backend; el front evita ofrecer combinaciones inválidas pero no es la autoridad.
- El front no persiste asignaciones; consume el contrato del backend (intake §17 geovial-web P.4).

## 13. Interacción multiusuario y concurrencia

- Si un agente está siendo reasignado en otra sesión, el front refleja el estado vigente del backend al recargar y evita asignaciones duplicadas.
- Mientras un agente recolecta en campo, el jefe puede reasignar sin bloquear la recolección en curso; lo recolectado se conserva por construcción del backend.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de asignación y reasignación de agentes desde el front web, derivado de NB-02 (F-04). |
