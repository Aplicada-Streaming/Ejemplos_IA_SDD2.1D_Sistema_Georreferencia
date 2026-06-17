# CU-06 — Transicionar el estado del relevamiento de recolección a revisión

**Proyecto:** geovial-api
**Documento:** CU-06-transicionar-estado-relevamiento_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que el jefe de área haga avanzar un relevamiento a lo largo de su ciclo, pasándolo de recolección a revisión, de modo que el avance del trabajo quede explícito y consultable. Es el control que ordena el ciclo del relevamiento antes del cierre.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área | Primario | Solicita la transición de estado de su relevamiento |
| Backend de relevamientos | Sistema | Valida la transición permitida y registra el nuevo estado |
| Almacén relacional | Sistema | Persiste el estado del relevamiento y su historial de transición |

## 3. Precondiciones

- El solicitante está autenticado y su rol es jefe de área (CU-03).
- El relevamiento existe, pertenece al jefe solicitante y está en un estado desde el cual la transición solicitada es válida (RN-05).

## 4. Flujo principal

1. El jefe de área solicita transicionar un relevamiento al estado de revisión.
2. El backend verifica que el relevamiento está en recolección y que la transición a revisión es válida (RN-05).
3. El backend cambia el estado a revisión y registra el momento y el autor de la transición.
4. El backend responde con el relevamiento en su nuevo estado.
5. En revisión, el relevamiento queda disponible para la vista de mapa y la resolución de conflictos previas al cierre (CU-12, CU-13, CU-14).

## 5. Flujos alternativos

- 5.A Retorno de revisión a recolección. Disparador: el jefe detecta que falta recolectar y solicita devolver el relevamiento a recolección. El backend admite la transición inversa permitida por RN-05 y reabre la captura para los agentes asignados. Retorna al paso 4.
- 5.B Transición sin observaciones. Disparador: el jefe solicita pasar a revisión un relevamiento sin ninguna observación cargada. El backend permite la transición y deja constancia de que la revisión opera sobre un relevamiento vacío, sin bloquear el ciclo. Retorna al paso 3.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| TRANSICION_NO_PERMITIDA | El estado origen no admite la transición solicitada según RN-05 | Rechaza con estado de conflicto y no cambia el estado |
| ROL_NO_AUTORIZADO | El solicitante no es el jefe dueño del relevamiento | Rechaza con estado de prohibido y no cambia el estado |
| RELEVAMIENTO_CERRADO | El relevamiento ya está cerrado y no admite transiciones de avance | Rechaza con estado de conflicto y no cambia el estado |

## 7. Postcondiciones

- Éxito: el relevamiento queda en el estado destino válido, con el momento y el autor de la transición registrados.
- Fallo: el estado del relevamiento no cambia y se devuelve un problema con el código correspondiente.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un relevamiento en estado de recolección de un jefe de área | El jefe solicita pasarlo a revisión | El backend cambia el estado a revisión y registra el autor y el momento |
| CA-02 | Un relevamiento ya en estado de cierre | El jefe solicita pasarlo a revisión | El backend rechaza con el código RELEVAMIENTO_CERRADO |
| CA-03 | Un relevamiento en revisión que necesita más recolección | El jefe solicita devolverlo a recolección | El backend aplica la transición inversa permitida y reabre la captura |
| CA-04 | Un relevamiento de otro jefe en recolección | El jefe actual solicita pasarlo a revisión | El backend rechaza con el código ROL_NO_AUTORIZADO |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-02 |
| Reglas de negocio aplicables | RN-01, RN-05 |
| Historias de usuario a generar | US-12, US-13 (en 06) |
| Componentes esperados | Recurso de transición de estado; máquina de estados del relevamiento; repositorio de historial de transición (referencia tentativa a 05) |
| Tests previstos | Transición recolección a revisión válida; transición desde cierre rechazada; transición inversa permitida; transición por no dueño rechazada (en 08) |

## 10. Notas y supuestos

- El cierre del relevamiento es una transición distinta, especificada en CU-14, porque exige resolver conflictos pendientes como precondición.
- El conjunto de estados y transiciones válidas (recolección, revisión, cierre) es la invariante RN-05.

## 12. Performance esperado del CU

- La transición de estado debe resolverse dentro del objetivo de escritura del proyecto (p95 menor o igual a 500 ms).

## 15. Idempotencia y reintento

- Solicitar la transición a un estado en el que el relevamiento ya se encuentra deja el estado sin cambios y responde con éxito.
- La transición admite clave de idempotencia para reintentos seguros ante respuestas no recibidas (CU-21).

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de transición de estado del relevamiento, derivado de NB-02 (F-11). |
