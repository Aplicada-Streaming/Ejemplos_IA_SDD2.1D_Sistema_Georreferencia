# CU-08 — Transicionar el estado del relevamiento y cerrarlo

**Proyecto:** geovial-web
**Documento:** CU-08-transicionar-estado-cerrar_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional

## 1. Propósito

Permitir que el jefe de área haga avanzar desde el front web el estado de un relevamiento por su ciclo —de recolección a revisión, y de revisión a cierre—, con el retorno controlado de revisión a recolección, y que cierre el relevamiento como hito que habilita el informe, siempre que no queden conflictos de marcadores sin resolver. Formaliza el avance del trabajo y el hito de cierre.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área | Primario | Solicita la transición de estado y el cierre del relevamiento |
| Front web | Sistema | Presenta el estado vigente y las transiciones habilitadas y envía la solicitud al backend |
| Backend de dominio | Sistema | Valida la transición y la ausencia de conflictos y aplica el nuevo estado |

## 3. Precondiciones

- El jefe de área tiene una sesión activa en el front web (CU-01) y es el dueño del relevamiento.
- El relevamiento está en un estado desde el que existe una transición válida.

## 4. Flujo principal

1. El jefe de área abre un relevamiento y ve su estado vigente del ciclo en el front web.
2. El front web presenta solo las transiciones válidas desde el estado actual.
3. El jefe solicita pasar el relevamiento de recolección a revisión.
4. El front web envía la transición al backend, que la aplica, y el front muestra el nuevo estado de revisión.
5. Tras revisar y resolver los conflictos pendientes (CU-06, CU-07), el jefe solicita cerrar el relevamiento.
6. El front web verifica que no haya conflictos pendientes señalados por el backend y envía el cierre.
7. El backend cierra el relevamiento y el front lo muestra como cerrado, habilitado para el informe.

## 5. Flujos alternativos

- 5.A Retorno controlado a recolección. Disparador: durante la revisión el jefe detecta que falta evidencia y necesita devolver el relevamiento a recolección. El front ofrece la transición de revisión a recolección y la envía al backend, que la aplica. Retorna al paso 2.
- 5.B Cierre con conflictos pendientes. Disparador: el jefe intenta cerrar un relevamiento que todavía tiene conflictos sin resolver. El front no habilita el cierre, informa que hay conflictos pendientes y deriva a la pantalla de resolución (CU-07). Retorna al paso 5.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| TRANSICION_NO_PERMITIDA | El jefe intenta una transición que no es válida desde el estado actual | El front no ofrece esa transición; ante el rechazo del backend informa que la transición no está permitida |
| CONFLICTOS_PENDIENTES | El jefe intenta cerrar con conflictos de marcadores sin resolver | El front bloquea el cierre, informa los conflictos pendientes y deriva a la resolución |
| FUERA_DE_ALCANCE | El jefe intenta transicionar un relevamiento ajeno | El front no lo abre; ante el rechazo del backend informa que está fuera de su alcance |

## 7. Postcondiciones

- Éxito en transición: el relevamiento queda en el nuevo estado válido y el front lo refleja.
- Éxito en cierre: el relevamiento queda cerrado, sin conflictos pendientes, habilitado para el informe.
- Fallo: el estado del relevamiento no cambia y el front informa la causa.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un relevamiento "Tramo Norte" en recolección | El jefe solicita pasarlo a revisión | El front lo muestra en estado de revisión |
| CA-02 | Un relevamiento en revisión con un conflicto de marcadores sin resolver | El jefe intenta cerrarlo | El front bloquea el cierre con CONFLICTOS_PENDIENTES y deriva a la resolución |
| CA-03 | Un relevamiento en revisión sin conflictos pendientes | El jefe solicita cerrarlo | El front lo muestra cerrado y habilitado para el informe |
| CA-04 | Un relevamiento ya cerrado | El jefe intenta pasarlo de nuevo a recolección | El front no ofrece esa transición (TRANSICION_NO_PERMITIDA) |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-05 |
| Reglas de negocio aplicables | RN-05 (geovial-web), RN-01 (geovial-web) |
| Historias de usuario a generar | US-18, US-19 (en 06) |
| Componentes esperados | Indicador de estado del ciclo; control de transiciones habilitadas; consumo del recurso de estado y cierre del backend (referencia tentativa a 05) |
| Tests previstos | Transición recolección a revisión; cierre bloqueado con conflictos; cierre sin conflictos; transición no permitida desde cierre (en 08) |

## 10. Notas y supuestos

- Las transiciones válidas y la precondición de cierre sin conflictos las gobierna el backend (RN-05 de geovial-api); el front solo expone las transiciones habilitadas que el backend reporta (RN-05 de geovial-web).
- Se admite la reapertura de un relevamiento recién cerrado a revisión, según la política del backend; el front la ofrece cuando el backend la habilita.
- El supuesto sobre el cierre mientras un agente tiene cambios locales sin sincronizar (el cierre bloquea nuevas subidas) está reflejado en el backend (geovial-api 02 §9), a confirmar con el negocio.

## 13. Interacción multiusuario y concurrencia

- Si el estado del relevamiento cambió en otra sesión mientras el jefe lo tenía abierto, al solicitar una transición el front refleja el rechazo del backend y recarga el estado vigente.
- El cierre es un hito único: una segunda solicitud de cierre sobre un relevamiento ya cerrado no produce efecto adicional y el front muestra el estado vigente.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de transición de estado y cierre del relevamiento desde el front web, derivado de NB-05 (F-11). |
