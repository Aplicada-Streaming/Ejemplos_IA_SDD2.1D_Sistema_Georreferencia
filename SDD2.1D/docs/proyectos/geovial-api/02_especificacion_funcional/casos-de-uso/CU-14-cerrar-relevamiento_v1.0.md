# CU-14 — Cerrar el relevamiento como hito que habilita el informe

**Proyecto:** geovial-api
**Documento:** CU-14-cerrar-relevamiento_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que el jefe de área cierre un relevamiento en revisión una vez resueltos todos los conflictos de marcadores, dejando la evidencia consolidada y el relevamiento como hito que habilita la confección del informe. Es la transición final del ciclo del relevamiento.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área | Primario | Solicita el cierre del relevamiento |
| Backend de relevamientos | Sistema | Verifica las precondiciones de cierre y registra el hito |
| Almacén relacional | Sistema | Persiste el estado de cierre y su momento y autor |

## 3. Precondiciones

- El solicitante está autenticado y es el jefe dueño del relevamiento (CU-03).
- El relevamiento está en estado de revisión (CU-06).
- No quedan conflictos de marcadores pendientes de resolución (CU-13, RN-05).

## 4. Flujo principal

1. El jefe solicita cerrar el relevamiento.
2. El backend verifica que el relevamiento está en revisión y que no quedan conflictos de marcadores pendientes (RN-05).
3. El backend transiciona el relevamiento al estado de cierre y registra el momento y el autor del cierre.
4. El backend deja la evidencia consolidada e inmutable para nuevos cambios de recolección, conservándola para consulta e informe.
5. El backend responde con el relevamiento cerrado.

## 5. Flujos alternativos

- 5.A Reapertura controlada antes de archivar. Disparador: el jefe necesita reabrir un relevamiento recién cerrado para corregir la revisión. El backend admite devolverlo a revisión si la política del relevamiento lo permite, registrando la reapertura, sin perder evidencia. Retorna al paso 5.
- 5.B Cierre sin conflictos previos. Disparador: el relevamiento nunca tuvo conflictos de marcadores. El backend omite la verificación de conflictos pendientes y procede directamente al cierre. Retorna al paso 3.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| CONFLICTOS_PENDIENTES | Quedan conflictos de marcadores sin resolver al intentar cerrar | Rechaza con estado de conflicto y no cierra el relevamiento (RN-05) |
| RELEVAMIENTO_NO_EN_REVISION | El relevamiento no está en revisión al intentar cerrarlo | Rechaza con estado de conflicto e indica el estado actual |
| ROL_NO_AUTORIZADO | El solicitante no es el jefe dueño del relevamiento | Rechaza con estado de prohibido y no cierra el relevamiento |

## 7. Postcondiciones

- Éxito: el relevamiento queda en estado de cierre, con su momento y autor registrados y su evidencia consolidada, habilitando el informe.
- Fallo: el relevamiento permanece en revisión y se devuelve un problema con el código correspondiente.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un relevamiento en revisión sin conflictos pendientes | El jefe solicita cerrarlo | El backend lo transiciona a cierre y registra el momento y el autor |
| CA-02 | Un relevamiento en revisión con un conflicto de marcadores pendiente | El jefe solicita cerrarlo | El backend rechaza con el código CONFLICTOS_PENDIENTES y no cierra |
| CA-03 | Un relevamiento aún en recolección | El jefe solicita cerrarlo | El backend rechaza con el código RELEVAMIENTO_NO_EN_REVISION |
| CA-04 | Un relevamiento recién cerrado que el jefe necesita corregir | El jefe solicita reabrirlo a revisión | El backend lo devuelve a revisión registrando la reapertura, sin perder evidencia |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-05 |
| Reglas de negocio aplicables | RN-05, RN-03 |
| Historias de usuario a generar | US-29, US-30 (en 06) |
| Componentes esperados | Recurso de cierre de relevamiento; verificador de conflictos pendientes; máquina de estados del relevamiento (referencia tentativa a 05) |
| Tests previstos | Cierre sin conflictos pendientes; cierre con conflictos rechazado; cierre desde recolección rechazado; reapertura controlada (en 08) |

## 10. Notas y supuestos

- El cierre es la transición que exige como precondición la resolución de todos los conflictos (RN-05), a diferencia de la transición a revisión (CU-06), que no la exige.
- El cierre habilita la portabilidad del relevamiento (CU-15, CU-16), que opera sobre relevamientos ya estructurados y cerrados.
- La meta de eficiencia del cierre (criterio de NB-05) se materializa al disponer de la evidencia consolidada para el informe.

## 12. Performance esperado del CU

- El cierre debe resolverse dentro del objetivo de escritura del proyecto (p95 menor o igual a 500 ms), incluida la verificación de conflictos pendientes.

## 15. Idempotencia y reintento

- Solicitar el cierre de un relevamiento ya cerrado deja el estado sin cambios y responde con éxito.
- El cierre admite clave de idempotencia para reintentos seguros ante respuestas no recibidas (CU-21).

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de cierre del relevamiento, derivado de NB-05 (F-11). |
