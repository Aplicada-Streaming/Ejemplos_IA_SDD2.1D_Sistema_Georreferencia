# CU-18 — Autorizar el acceso a cada recurso según el rol y el alcance

**Proyecto:** geovial-api
**Documento:** CU-18-autorizar-por-rol_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Garantizar de forma transversal que toda solicitud a un recurso del backend porte un token válido y que la acción solicitada esté permitida para el rol del solicitante dentro de su alcance jerárquico. Es el control de autorización común a todos los CU funcionales.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Usuario de cualquier rol | Primario | Solicita una acción sobre un recurso portando su token |
| Backend de autorización | Sistema | Valida el token y decide si el rol y el alcance permiten la acción |

## 3. Precondiciones

- El solicitante obtuvo un token de autenticación válido (CU-03).
- El recurso solicitado declara qué roles y alcances lo pueden operar.

## 4. Flujo principal

1. El usuario solicita una acción sobre un recurso incluyendo su token.
2. El backend valida que el token es legítimo y está vigente.
3. El backend extrae el rol del solicitante y el alcance que le corresponde en la jerarquía (RN-01).
4. El backend verifica que la acción solicitada está permitida para ese rol y que el recurso destino cae dentro de su alcance.
5. Si la verificación es positiva, el backend deriva la solicitud al CU funcional correspondiente; si es negativa, la rechaza antes de ejecutar la acción.

## 5. Flujos alternativos

- 5.A Recurso de acceso público restringido. Disparador: la solicitud apunta al recurso de autenticación, que no exige token previo. El backend omite la verificación de rol para ese recurso y aplica solo sus propias validaciones. Retorna al flujo de ese recurso.
- 5.B Acción permitida pero fuera de alcance. Disparador: el rol permite la acción en general, pero el recurso destino pertenece a otro ámbito (otro jefe, otra área). El backend rechaza por alcance aunque el rol sea correcto. Termina con rechazo de alcance.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| NO_AUTENTICADO | La solicitud no porta token o el token no es legítimo | Rechaza con estado de no autorizado antes de ejecutar la acción |
| ACCION_NO_PERMITIDA | El rol del solicitante no habilita la acción solicitada | Rechaza con estado de prohibido y no ejecuta la acción |
| FUERA_DE_ALCANCE | El recurso destino no pertenece al ámbito del solicitante | Rechaza con estado de prohibido y no expone ni modifica el recurso |

## 7. Postcondiciones

- Éxito: la solicitud queda autorizada y se deriva al CU funcional, con el rol y el alcance verificados.
- Fallo: la solicitud se rechaza antes de cualquier efecto sobre el recurso.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un jefe de área con token válido | Solicita crear un relevamiento | El backend autoriza la acción y la deriva al CU de gestión de relevamientos |
| CA-02 | Una solicitud sin token a un recurso protegido | El cliente la envía | El backend rechaza con el código NO_AUTENTICADO sin ejecutar la acción |
| CA-03 | Un agente de campo con token válido | Solicita dar de alta a otro agente | El backend rechaza con el código ACCION_NO_PERMITIDA |
| CA-04 | Un jefe de área con token válido | Solicita el detalle de un relevamiento de otro jefe | El backend rechaza con el código FUERA_DE_ALCANCE |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-01 |
| Reglas de negocio aplicables | RN-01, RN-02 |
| Historias de usuario a generar | US-37, US-38 (en 06) |
| Componentes esperados | Filtro de autorización transversal; servicio de evaluación de rol y alcance; validador de token (referencia tentativa a 05) |
| Tests previstos | Acción permitida autorizada; solicitud sin token rechazada; acción no permitida por rol rechazada; acceso fuera de alcance rechazado (en 08) |

## 10. Notas y supuestos

- Este CU transversal se aplica antes de todos los CU funcionales (CU-01 a CU-17) y por eso cada uno lo referencia en sus precondiciones, en vez de repetir su lógica.
- La emisión y revocación del token pertenecen a CU-03; este CU usa el token, no lo emite.
- El alcance jerárquico (cada rol opera su ámbito y administra el nivel inmediato inferior) es la invariante RN-01.

## 12. Performance esperado del CU

- La verificación de autorización debe agregar una sobrecarga mínima a cada solicitud, dentro de los objetivos de latencia del proyecto.

## 15. Idempotencia y reintento

- La autorización es una verificación sin efectos colaterales: repetir la misma solicitud autorizada no altera el estado por sí misma; la idempotencia del efecto la garantiza el CU funcional derivado (CU-21).

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU transversal de autorización por rol y alcance, derivado de NB-01 y de la naturaleza rest-api del proyecto (02 §2.2). |
