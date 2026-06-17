# CU-21 — Garantizar la idempotencia de las operaciones no seguras

**Proyecto:** geovial-api
**Documento:** CU-21-garantizar-idempotencia-operaciones_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Garantizar de forma transversal que las operaciones no seguras del backend —altas, asignaciones, subidas de sincronización, importaciones— puedan reintentarse de manera segura mediante una clave de idempotencia, de modo que un reintento tras una respuesta no recibida no produzca efectos duplicados. Sostiene la confiabilidad de la sincronización sin conexión y de toda escritura reintentable.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Cliente consumidor | Primario | Reintenta una operación no segura con la misma clave de idempotencia |
| Backend de la API | Sistema | Reconoce la clave y evita duplicar el efecto de la operación |

## 3. Precondiciones

- La operación es no segura (crea o modifica estado) y declara que admite clave de idempotencia.
- El cliente provee una clave de idempotencia estable para la operación que desea reintentar.

## 4. Flujo principal

1. El cliente envía una operación no segura acompañada de una clave de idempotencia.
2. El backend verifica si esa clave ya fue procesada para esa operación.
3. Si la clave es nueva, el backend ejecuta la operación, registra su resultado asociado a la clave y responde con ese resultado.
4. Si la clave ya fue procesada, el backend no ejecuta de nuevo la operación y devuelve el mismo resultado registrado.
5. El cliente recibe siempre el mismo resultado para la misma clave, haya o no haya reintentado.

## 5. Flujos alternativos

- 5.A Reintento durante el procesamiento en curso. Disparador: el cliente reintenta con la misma clave mientras la operación original todavía se procesa. El backend no inicia una segunda ejecución y responde indicando que la operación está en curso, sin duplicar el efecto. Retorna al paso 4 cuando concluye.
- 5.B Clave reutilizada con contenido distinto. Disparador: el cliente reutiliza una clave ya procesada pero con un contenido de solicitud diferente. El backend rechaza la reutilización indebida de la clave para no confundir dos operaciones distintas. Termina con rechazo de clave reutilizada.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| CLAVE_REUTILIZADA_INCONSISTENTE | La clave ya procesada se reutiliza con un contenido distinto | Rechaza con estado de conflicto y no ejecuta la operación |
| CLAVE_REQUERIDA_AUSENTE | La operación no segura exige clave y el cliente no la proveyó | Rechaza con estado de solicitud inválida e indica que la clave es obligatoria, cuando aplica |
| OPERACION_NO_IDEMPOTENTE | El cliente envía clave a una operación que no admite idempotencia | Ignora la clave o la rechaza según el recurso, sin alterar el resultado |

## 7. Postcondiciones

- Éxito: la operación se ejecutó una sola vez para la clave dada y todo reintento devuelve el mismo resultado, sin efectos duplicados.
- Garantía: las escrituras reintentables son seguras ante respuestas no recibidas y cortes de conexión.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un alta de agente con una clave de idempotencia nueva | El cliente la envía y reintenta con la misma clave tras no recibir respuesta | El backend crea el agente una sola vez y el reintento devuelve el mismo agente sin duplicarlo |
| CA-02 | Una subida de sincronización con clave ya procesada | El cliente reenvía el mismo lote con la misma clave | El backend devuelve el resultado registrado sin reaplicar los cambios |
| CA-03 | Una clave ya procesada reutilizada con un contenido distinto | El cliente la envía | El backend rechaza con el código CLAVE_REUTILIZADA_INCONSISTENTE |
| CA-04 | Un reintento con la misma clave mientras la operación original aún se procesa | El cliente lo envía | El backend no duplica la ejecución e informa que la operación está en curso |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-04, NB-01, NB-02, NB-06 |
| Reglas de negocio aplicables | RN-07, RN-06 |
| Historias de usuario a generar | US-42, US-43 (en 06) |
| Componentes esperados | Servicio transversal de idempotencia; registro de claves y resultados; integración con la subida de sincronización (referencia tentativa a 05) |
| Tests previstos | Alta idempotente sin duplicar; subida reenviada sin reaplicar; clave reutilizada inconsistente rechazada; reintento durante ejecución sin duplicar (en 08) |

## 10. Notas y supuestos

- Este CU transversal sostiene la garantía de idempotencia que invocan los CU no seguros (por ejemplo CU-01, CU-02, CU-04, CU-05, CU-06, CU-10, CU-13, CU-14, CU-16).
- La idempotencia de la sincronización por identificador de origen (RN-07) es la aplicación más crítica, porque la captura sin conexión reenvía lotes tras cortes.
- El alcance temporal de retención de las claves y su representación exacta pertenecen a la categoría 05.

## 12. Performance esperado del CU

- La verificación de idempotencia debe agregar una sobrecarga mínima a cada operación no segura, dentro de los objetivos de latencia de escritura del proyecto.

## 15. Idempotencia y reintento

- Este CU es, en sí mismo, la especificación de la idempotencia transversal: define cómo un reintento con la misma clave no produce efectos duplicados en ninguna operación no segura del backend.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU transversal de idempotencia de operaciones no seguras, derivado de NB-04 y de la naturaleza rest-api del proyecto (02 §2.2, §4.3 §15). |
