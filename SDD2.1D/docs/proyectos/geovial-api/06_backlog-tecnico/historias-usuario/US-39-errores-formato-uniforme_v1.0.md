# US-39 — Devolver todos los errores con un formato de problema uniforme

**Proyecto:** geovial-api
**Documento:** US-39-errores-formato-uniforme_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-08 Capacidades transversales del contrato
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como cliente consumidor de la API, quiero recibir todos los errores con un formato de problema uniforme y un código estable, para manejar los fallos de forma predecible sin acoplarme al idioma.

## 2. Contexto

CU-19 describe la respuesta uniforme de errores del contrato. NB-01 a NB-05 enmarcan las capacidades de negocio cuyos fallos deben representarse de manera consistente. ADR-05 resuelve usar problem+json RFC 7807 con un código estable en mayúsculas y sin tildes, de modo que el cliente programe contra el código y no contra el mensaje.

## 3. Criterios de aceptación

- Given cualquier operación que falla, When el sistema responde, Then la respuesta es un problem+json con código estable, mensaje legible y estado.
- Given un error de validación que afecta a varios campos, When el sistema responde, Then devuelve un único problema que enumera los campos afectados.
- Given un fallo no contemplado, When el sistema responde, Then devuelve el código ERROR_INTERNO sin filtrar detalles internos.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-01 a NB-05 |
| CU cubiertos | CU-19 |
| BT derivadas | BT-09 |
| Tests previstos | acceptance/AT-39-error-uniforme; contract test de respuesta problem+json |

## 5. Prioridad y estimación

Must porque la forma uniforme de error es transversal a NB-01 a NB-05 y condiciona la integración de todos los clientes: sin un código estable y un formato común, el manejo de fallos quedaría acoplado al idioma y al detalle interno. 5 SP por Planning Poker (Fibonacci): es un manejador transversal que normaliza toda salida de error a problem+json, agrupa errores de validación multicampo y enmascara los fallos no contemplados.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-19)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-09)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

Los códigos son opacos al idioma: el mensaje puede traducirse, pero el código permanece estable en mayúsculas y sin tildes. El catálogo de códigos vive en el contrato REST y se alinea con dx-error-messages, que provee los textos legibles.
