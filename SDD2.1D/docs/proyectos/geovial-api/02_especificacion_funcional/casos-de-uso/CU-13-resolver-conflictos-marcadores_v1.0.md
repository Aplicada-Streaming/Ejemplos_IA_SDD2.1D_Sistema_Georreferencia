# CU-13 — Resolver los conflictos de marcadores al cierre

**Proyecto:** geovial-api
**Documento:** CU-13-resolver-conflictos-marcadores_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que el jefe de área resuelva, en el momento previo al cierre, los conflictos de marcadores que convivieron durante la recolección —dos o más marcadores dentro de un mismo radio—, decidiendo unificarlos en uno solo o mantenerlos separados. Pone la decisión de catalogación en manos del jefe, en el cierre y no antes.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área | Primario | Decide unificar o separar cada conflicto de marcadores |
| Backend de marcadores | Sistema | Aplica la resolución conservando las observaciones afectadas |
| Almacén relacional | Sistema | Persiste la unificación o la separación y reasigna las observaciones |

## 3. Precondiciones

- El solicitante está autenticado y es el jefe dueño del relevamiento (CU-03).
- El relevamiento está en estado de revisión (CU-06).
- Existe al menos un conflicto de marcadores pendiente de resolución.

## 4. Flujo principal

1. El jefe solicita la lista de conflictos de marcadores pendientes del relevamiento.
2. El backend devuelve cada conflicto con los marcadores involucrados y sus observaciones.
3. El jefe decide, por cada conflicto, unificar los marcadores en uno o mantenerlos separados.
4. Al unificar, el backend reasigna las observaciones de los marcadores involucrados al marcador resultante, conserva sus fotos, comentarios y etiquetas, y marca el conflicto como resuelto (RC de referencia observación a marcador).
5. Al mantener separados, el backend marca el conflicto como resuelto sin alterar las observaciones.
6. El backend responde con el conflicto resuelto y el estado de los conflictos restantes.

## 5. Flujos alternativos

- 5.A Unificación de marcadores con etiquetas distintas. Disparador: los marcadores a unificar tienen etiquetas diferentes. El backend conserva la unión de las etiquetas en el marcador resultante, sin perder ninguna. Retorna al paso 6.
- 5.B Resolución reabierta. Disparador: el jefe reabre un conflicto ya resuelto antes del cierre para cambiar la decisión. El backend permite revertir la resolución mientras el relevamiento no esté cerrado y vuelve a dejar el conflicto pendiente. Retorna al paso 3.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| RELEVAMIENTO_NO_EN_REVISION | El relevamiento no está en revisión al resolver conflictos | Rechaza con estado de conflicto y no aplica la resolución |
| CONFLICTO_INEXISTENTE | El conflicto a resolver no existe o ya fue resuelto y cerrado | Rechaza con estado de no encontrado o conflicto, según el caso |
| ROL_NO_AUTORIZADO | El solicitante no es el jefe dueño del relevamiento | Rechaza con estado de prohibido y no aplica la resolución |

## 7. Postcondiciones

- Éxito en unificar: existe un único marcador resultante con todas las observaciones, fotos, comentarios y etiquetas de los marcadores unificados; el conflicto queda resuelto.
- Éxito en separar: los marcadores se conservan y el conflicto queda resuelto.
- Fallo: el conflicto y los marcadores no cambian y se devuelve un problema con el código correspondiente.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un relevamiento en revisión con dos marcadores en conflicto dentro de un radio | El jefe decide unificarlos | El backend reasigna las observaciones al marcador resultante, conserva sus fotos y etiquetas y marca el conflicto resuelto |
| CA-02 | Un conflicto de dos marcadores con etiquetas distintas | El jefe los unifica | El marcador resultante conserva la unión de las etiquetas de ambos |
| CA-03 | Un relevamiento en recolección con un conflicto pendiente | El jefe intenta resolver el conflicto | El backend rechaza con el código RELEVAMIENTO_NO_EN_REVISION |
| CA-04 | Un conflicto que el jefe decide mantener separado | El jefe lo resuelve como separado | El backend conserva ambos marcadores y marca el conflicto resuelto |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-05 |
| Reglas de negocio aplicables | RN-03, RN-05 |
| Historias de usuario a generar | US-27, US-28 (en 06) |
| Componentes esperados | Recurso de conflictos de marcadores; servicio de unificación y reasignación de observaciones; registro de resolución (referencia tentativa a 05) |
| Tests previstos | Unificación reasigna observaciones; unión de etiquetas al unificar; resolución fuera de revisión rechazada; separación conserva marcadores (en 08) |

## 10. Notas y supuestos

- Los conflictos conviven con la operación durante la recolección y solo se resuelven aquí, en el momento del cierre (RN-03).
- La resolución de todos los conflictos pendientes es precondición del cierre del relevamiento (CU-14): no se cierra con conflictos sin resolver (RN-05).
- La política para conflictos surgidos de sincronizaciones de distintos agentes se asume igual a la de convivencia y resolución al cierre, a confirmar con el cliente (ver §9 del índice).

## 12. Performance esperado del CU

- La resolución de un conflicto debe resolverse dentro del objetivo de escritura (p95 menor o igual a 500 ms), incluida la reasignación de observaciones del conflicto.

## 15. Idempotencia y reintento

- Resolver un conflicto ya resuelto con la misma decisión deja el estado sin cambios y responde con éxito.
- La unificación admite clave de idempotencia para que un reintento no genere un marcador resultante duplicado (CU-21).

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de resolución de conflictos de marcadores al cierre, derivado de NB-05 (F-13). |
