# CU-07 — Resolver conflictos de marcadores al cierre

**Proyecto:** geovial-web
**Documento:** CU-07-resolver-conflictos-cierre_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional

## 1. Propósito

Permitir que el jefe de área resuelva, desde el front web y en el momento previo al cierre, los conflictos de marcadores que convivieron durante la recolección y la revisión —dos o más marcadores dentro de un mismo radio—, decidiendo por cada conflicto unificar los marcadores en uno o mantenerlos separados. Deja la catalogación de la evidencia ordenada para habilitar el cierre.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área | Primario | Decide unificar o separar cada conflicto de marcadores |
| Front web | Sistema | Presenta los conflictos pendientes con su evidencia y envía la decisión al backend |
| Backend de dominio | Sistema | Aplica la resolución, conserva las observaciones y marca el conflicto como resuelto |

## 3. Precondiciones

- El jefe de área tiene una sesión activa en el front web (CU-01) y es el dueño del relevamiento.
- El relevamiento está en estado de revisión.
- Existe al menos un conflicto de marcadores pendiente de resolución.

## 4. Flujo principal

1. El jefe de área abre la pantalla de conflictos pendientes del relevamiento en el front web.
2. El front web consume del backend la lista de conflictos, cada uno con los marcadores involucrados y su evidencia (fotos, comentarios y etiquetas).
3. El jefe selecciona un conflicto y compara sobre el mapa los marcadores involucrados.
4. El jefe decide unificarlos en uno solo o mantenerlos separados.
5. El front web envía la decisión al backend.
6. Al unificar, el backend reasigna las observaciones al marcador resultante, conserva sus fotos, comentarios y etiquetas y marca el conflicto resuelto; el front actualiza la lista de conflictos restantes.
7. Al mantener separados, el backend marca el conflicto resuelto sin alterar las observaciones; el front lo retira de los pendientes.

## 5. Flujos alternativos

- 5.A Reabrir una resolución antes del cierre. Disparador: el jefe quiere cambiar una decisión ya tomada sobre un conflicto, antes de cerrar el relevamiento. El front web solicita al backend reabrir el conflicto, que vuelve a quedar pendiente, y permite decidir de nuevo. Retorna al paso 4.
- 5.B Unificación de marcadores con etiquetas distintas. Disparador: los marcadores a unificar tienen etiquetas diferentes. El front advierte que el marcador resultante conservará la unión de las etiquetas y, al confirmar, el backend las conserva todas. Retorna al paso 6.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| RELEVAMIENTO_NO_EN_REVISION | El relevamiento no está en revisión al intentar resolver | El front presenta la pantalla en solo lectura e informa que la resolución se hace en revisión |
| CONFLICTO_INEXISTENTE | El conflicto seleccionado ya fue resuelto o no existe | El front lo retira de la lista y refresca los conflictos pendientes |
| FUERA_DE_ALCANCE | El jefe intenta resolver conflictos de un relevamiento ajeno | El front no abre el relevamiento; ante el rechazo del backend informa que está fuera de su alcance |

## 7. Postcondiciones

- Éxito en unificar: existe un único marcador resultante con toda la evidencia de los unificados y el conflicto queda resuelto.
- Éxito en separar: los marcadores se conservan y el conflicto queda resuelto.
- Fallo: el conflicto y su evidencia no cambian y el front informa la causa.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un relevamiento en revisión con dos marcadores en conflicto dentro de un radio | El jefe decide unificarlos | El front muestra un único marcador resultante con la evidencia de ambos y un conflicto menos pendiente |
| CA-02 | Un conflicto entre marcadores con etiquetas "fisura" y "junta" | El jefe los unifica | El front advierte y el marcador resultante conserva ambas etiquetas |
| CA-03 | Un relevamiento todavía en recolección con un conflicto presente | El jefe intenta resolver el conflicto | El front presenta la pantalla en solo lectura (RELEVAMIENTO_NO_EN_REVISION) |
| CA-04 | Un conflicto que el jefe ya resolvió como "separados" antes del cierre | El jefe lo reabre y lo unifica | El front vuelve a dejarlo pendiente y aplica la nueva decisión de unificación |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-05 |
| Reglas de negocio aplicables | RN-05 (geovial-web), RN-01 (geovial-web) |
| Historias de usuario a generar | US-16, US-17 (en 06) |
| Componentes esperados | Pantalla de conflictos pendientes; comparador de marcadores sobre el mapa; consumo del recurso de conflictos del backend (referencia tentativa a 05) |
| Tests previstos | Unificación reasigna evidencia; unión de etiquetas; resolución fuera de revisión en solo lectura; reapertura antes del cierre (en 08) |

## 10. Notas y supuestos

- La resolución de todos los conflictos pendientes es precondición del cierre del relevamiento (CU-08): el front no habilita el cierre con conflictos sin resolver (RN-05 de geovial-web).
- La política para conflictos surgidos de sincronizaciones de distintos agentes se asume igual a la de convivencia y resolución al cierre, a confirmar con el negocio (geovial-api 02 §9).
- El front no aplica la resolución por sí mismo: la ejecuta el backend (CU-13 de geovial-api); el front presenta la evidencia y envía la decisión del jefe.

## 13. Interacción multiusuario y concurrencia

- Si un agente sincroniza cambios que generan un nuevo conflicto mientras el jefe resuelve, el front mostrará el nuevo conflicto pendiente al recargar la lista.
- La reapertura de una resolución es posible mientras el relevamiento no esté cerrado; tras el cierre, las resoluciones quedan firmes.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de resolución de conflictos de marcadores al cierre desde el front web, derivado de NB-05 (F-13). |
