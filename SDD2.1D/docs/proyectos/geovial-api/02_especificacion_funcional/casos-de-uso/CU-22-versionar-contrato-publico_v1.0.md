# CU-22 — Versionar el contrato público de la API

**Proyecto:** geovial-api
**Documento:** CU-22-versionar-contrato-publico_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Garantizar de forma transversal que el contrato público de la API se exponga bajo una versión explícita y que los cambios incompatibles obliguen a una versión mayor nueva, manteniendo la versión previa disponible mientras los clientes web y de campo migran. Protege a los consumidores de roturas inesperadas del contrato.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Cliente consumidor | Primario | Solicita los recursos indicando la versión del contrato que consume |
| Backend de la API | Sistema | Resuelve la versión solicitada y aplica la política de compatibilidad |

## 3. Precondiciones

- Cada recurso de la API se expone bajo una versión mayor explícita del contrato.
- Existe una política declarada de compatibilidad y de retiro de versiones.

## 4. Flujo principal

1. El cliente solicita un recurso indicando la versión mayor del contrato que utiliza.
2. El backend resuelve la solicitud contra la versión indicada.
3. Un cambio compatible (agregar un campo opcional, un nuevo recurso o un valor adicional) se incorpora dentro de la misma versión mayor, sin romper a los clientes existentes.
4. Un cambio incompatible (quitar un campo, volver obligatorio uno opcional o cambiar la semántica) se publica como una versión mayor nueva, conservando la anterior por un período de convivencia.
5. El backend atiende ambas versiones mayores durante el período de convivencia y comunica el plan de retiro de la versión previa.

## 5. Flujos alternativos

- 5.A Versión no soportada. Disparador: el cliente solicita una versión mayor que ya fue retirada o nunca existió. El backend rechaza indicando las versiones vigentes para que el cliente migre. Termina con rechazo de versión.
- 5.B Versión no indicada. Disparador: el cliente no indica versión. El backend resuelve contra la versión vigente declarada por defecto, o rechaza solicitando una versión explícita, según la política. Retorna al paso 2.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| VERSION_NO_SOPORTADA | El cliente solicita una versión retirada o inexistente | Rechaza con estado de solicitud inválida e informa las versiones vigentes |
| VERSION_REQUERIDA_AUSENTE | La política exige versión explícita y el cliente no la indicó | Rechaza con estado de solicitud inválida e indica cómo expresar la versión, cuando aplica |
| RECURSO_NO_EN_VERSION | El recurso solicitado no existe en la versión indicada | Rechaza con estado de no encontrado para esa versión |

## 7. Postcondiciones

- Éxito: el cliente recibe la respuesta de la versión que consume; los cambios compatibles no lo afectan y los incompatibles solo lo alcanzan al migrar de versión mayor.
- Garantía: ninguna evolución incompatible del contrato rompe a un cliente que permanece en su versión durante el período de convivencia.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un cliente que consume la versión mayor vigente del contrato | El backend agrega un campo opcional a un recurso | El cliente sigue funcionando sin cambios dentro de la misma versión mayor |
| CA-02 | Un cambio incompatible en un recurso | El backend lo publica | El backend expone una versión mayor nueva y conserva la anterior durante el período de convivencia |
| CA-03 | Un cliente que solicita una versión ya retirada | El cliente envía la solicitud | El backend rechaza con el código VERSION_NO_SOPORTADA e informa las versiones vigentes |
| CA-04 | Un recurso inexistente en la versión solicitada | El cliente lo pide en esa versión | El backend rechaza con el código RECURSO_NO_EN_VERSION |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-01, NB-02, NB-03, NB-04, NB-05 |
| Reglas de negocio aplicables | No introduce RN de dominio; fija la política de compatibilidad del contrato |
| Historias de usuario a generar | US-44 (en 06) |
| Componentes esperados | Servicio transversal de resolución de versión; política de compatibilidad y retiro; descriptor de versiones vigentes (referencia tentativa a 05) |
| Tests previstos | Cambio compatible no rompe al cliente; cambio incompatible publica versión nueva; versión retirada rechazada; recurso ausente en versión rechazado (en 08) |

## 10. Notas y supuestos

- Este CU transversal define el QUÉ de la compatibilidad del contrato: una versión por evolución incompatible y convivencia de la versión previa.
- El mecanismo concreto de expresar la versión (por ejemplo, prefijo de versión en la ruta) y el período exacto de convivencia pertenecen a la categoría 05, alineados con la decisión técnica del intake (§17 P.3).
- Esta política protege la dependencia de geovial-web y geovial-mobile sobre el contrato REST del backend (intake §14).

## 12. Performance esperado del CU

- La resolución de la versión no debe agregar sobrecarga apreciable al procesamiento de cada solicitud.

## 15. Idempotencia y reintento

- La resolución de versión es una operación sin efectos colaterales: repetir la misma solicitud bajo la misma versión produce el mismo comportamiento.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU transversal de versionado del contrato público, derivado de la naturaleza rest-api del proyecto (02 §2.2) y del intake §17 P.3. |
