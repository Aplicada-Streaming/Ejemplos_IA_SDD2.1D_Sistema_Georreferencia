# CU-19 — Devolver errores con un formato de problema uniforme

**Proyecto:** geovial-api
**Documento:** CU-19-manejar-errores-problem-json_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Garantizar de forma transversal que todo error que devuelve el backend se exprese con una estructura de problema uniforme —un código estable, un mensaje legible, el estado de la respuesta y datos de contexto— para que los clientes web y de campo traten los fallos de manera consistente. Unifica el contrato de error de toda la superficie REST.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Cliente consumidor | Primario | Recibe y trata la respuesta de error uniforme |
| Backend de la API | Sistema | Construye la representación de problema ante cualquier fallo |

## 3. Precondiciones

- Una solicitud a cualquier recurso produjo un fallo de validación, de autorización, de estado o interno.

## 4. Flujo principal

1. Un CU funcional detecta un fallo y lo señala con un código estable de la solución (por ejemplo TRAMO_INCOMPLETO).
2. El backend traduce el fallo a una representación de problema con código, mensaje legible, estado de la respuesta y, cuando aporta, el campo o recurso implicado.
3. El backend devuelve la representación de problema con el estado de respuesta acorde a la naturaleza del fallo (solicitud inválida, no autorizado, prohibido, no encontrado, conflicto o error interno).
4. El cliente consumidor lee el código estable para decidir su tratamiento y el mensaje legible para mostrar al usuario.

## 5. Flujos alternativos

- 5.A Error de validación con múltiples campos. Disparador: una solicitud falla por varios campos a la vez. El backend devuelve un único problema que enumera cada campo inválido con su motivo, sin emitir varias respuestas. Retorna al paso 3.
- 5.B Error interno no previsto. Disparador: ocurre un fallo no contemplado por ningún código de CU. El backend devuelve un problema con un código genérico de error interno y no expone detalles internos sensibles. Termina con el problema genérico.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| FORMATO_SOLICITUD_INVALIDO | La solicitud no respeta la estructura esperada del recurso | Devuelve un problema con estado de solicitud inválida señalando el defecto |
| ERROR_INTERNO | Un fallo no previsto en el procesamiento | Devuelve un problema con estado de error interno y un código genérico, sin filtrar detalles |
| RECURSO_NO_ENCONTRADO | El recurso solicitado no existe | Devuelve un problema con estado de no encontrado |

## 7. Postcondiciones

- Éxito: todo error queda expresado con la estructura de problema uniforme y un código estable, cualquiera sea el recurso de origen.
- Garantía: el formato de error no varía entre recursos; los clientes lo tratan de manera homogénea.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una creación de relevamiento sin tramo definido | El cliente la envía | El backend devuelve un problema con código TRAMO_INCOMPLETO, mensaje legible y estado de solicitud inválida |
| CA-02 | Una solicitud con varios campos inválidos | El cliente la envía | El backend devuelve un único problema que enumera cada campo inválido con su motivo |
| CA-03 | Una solicitud a un recurso inexistente | El cliente la envía | El backend devuelve un problema con código RECURSO_NO_ENCONTRADO y estado de no encontrado |
| CA-04 | Un fallo interno no contemplado | El cliente realiza la solicitud | El backend devuelve un problema con código ERROR_INTERNO sin exponer detalles internos |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-01, NB-02, NB-03, NB-04, NB-05 |
| Reglas de negocio aplicables | No introduce RN propias; uniformiza la respuesta de las RN de los demás CU |
| Historias de usuario a generar | US-39 (en 06) |
| Componentes esperados | Manejador de errores transversal; catálogo de códigos estables; traductor de fallo a representación de problema (referencia tentativa a 05) |
| Tests previstos | Error de validación uniforme; múltiples campos en un solo problema; recurso inexistente; error interno sin filtración (en 08) |

## 10. Notas y supuestos

- Este CU transversal recoge los códigos de error declarados por los CU funcionales (CU-01 a CU-17) y fija su forma de presentación común.
- El catálogo concreto de estados de respuesta y la representación exacta del problema pertenecen a la categoría 05; aquí se fija el contrato funcional de uniformidad.
- Los códigos estables son opacos al idioma del mensaje: el cliente decide por el código, no por el texto.

## 12. Performance esperado del CU

- La construcción de la representación de error no debe agregar sobrecarga apreciable respecto del costo de procesar la solicitud fallida.

## 15. Idempotencia y reintento

- Devolver un error es una operación sin efectos colaterales: repetir una solicitud inválida produce el mismo problema estable, sin alterar el estado del sistema.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU transversal de manejo uniforme de errores, derivado de la naturaleza rest-api del proyecto (02 §2.2). |
