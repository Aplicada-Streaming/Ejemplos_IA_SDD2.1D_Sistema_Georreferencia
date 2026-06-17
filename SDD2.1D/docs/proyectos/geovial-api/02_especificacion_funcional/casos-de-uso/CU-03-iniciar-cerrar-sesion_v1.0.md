# CU-03 — Iniciar sesión, cerrar sesión completa y revalidar credenciales

**Proyecto:** geovial-api
**Documento:** CU-03-iniciar-cerrar-sesion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que un usuario obtenga un token de autenticación entregando sus credenciales, que cierre por completo su sesión para liberar un dispositivo compartido y que el backend valide el token en cada solicitud. Es el punto de control único que asocia toda acción a un usuario identificado.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Usuario de cualquier rol | Primario | Entrega credenciales y obtiene o invalida su token |
| Backend de autenticación | Sistema | Valida credenciales, emite y revoca el token, lo verifica por solicitud |
| Almacén relacional | Sistema | Aporta las credenciales del usuario y su rol vigente |

## 3. Precondiciones

- El usuario existe y está habilitado en el almacén relacional.
- Para el cierre de sesión, el usuario presenta un token vigente a invalidar.

## 4. Flujo principal

1. El usuario solicita un token entregando su identificador de acceso y su credencial.
2. El backend valida las credenciales contra el almacén relacional y verifica que el usuario está habilitado.
3. El backend emite un token de autenticación que porta el rol del usuario y una vigencia limitada.
4. El usuario incluye el token en cada solicitud posterior; el backend lo valida antes de atender el recurso (ver CU-18).
5. Para cerrar sesión, el usuario solicita la invalidación de su sesión; el backend revoca el token y deja el dispositivo libre de la identidad anterior.

## 5. Flujos alternativos

- 5.A Revalidación en sesión activa. Disparador: el usuario ya autenticó antes en el dispositivo y la app cliente solicita continuar la sesión sin reingresar credenciales (relogueo por seguridad del propio dispositivo). El backend acepta una renovación del token mientras la sesión previa no haya sido cerrada por completo ni revocada. Retorna al paso 4.
- 5.B Token vencido. Disparador: el usuario presenta un token expirado. El backend rechaza la solicitud del recurso e indica que se requiere renovar el token; el usuario obtiene uno nuevo desde el paso 1. Retorna al paso 3.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| CREDENCIALES_INVALIDAS | El identificador o la credencial no coinciden | Rechaza con estado de no autorizado y no emite token |
| USUARIO_INHABILITADO | El usuario fue dado de baja y conserva traza pero no acceso | Rechaza con estado de no autorizado e indica que el acceso está revocado |
| TOKEN_REVOCADO | El usuario presenta un token ya invalidado por un cierre de sesión | Rechaza con estado de no autorizado y solicita autenticación nueva |

## 7. Postcondiciones

- Éxito en inicio: el usuario posee un token vigente que porta su rol y habilita las solicitudes a los recursos según su alcance.
- Éxito en cierre: el token queda revocado y ninguna identidad anterior persiste en el dispositivo.
- Fallo: no se emite token y la solicitud queda sin autorizar.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un agente de campo habilitado con credenciales válidas | Solicita un token entregando su identificador y credencial | El backend emite un token que porta el rol agente con vigencia limitada |
| CA-02 | Un usuario con credenciales incorrectas | Solicita un token | El backend rechaza con el código CREDENCIALES_INVALIDAS y no emite token |
| CA-03 | Un agente con sesión activa que cierra por completo su sesión en un dispositivo compartido | Otro agente intenta usar el token anterior | El backend rechaza con el código TOKEN_REVOCADO |
| CA-04 | Un usuario dado de baja que conserva sus credenciales | Solicita un token | El backend rechaza con el código USUARIO_INHABILITADO |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-01 |
| Reglas de negocio aplicables | RN-01, RN-02 |
| Historias de usuario a generar | US-05, US-06 (en 06) |
| Componentes esperados | Recurso de autenticación; emisor y validador de token; servicio de revocación de sesión (referencia tentativa a 05) |
| Tests previstos | Emisión con credenciales válidas; rechazo de credenciales inválidas; cierre revoca token; usuario inhabilitado sin acceso (en 08) |

## 10. Notas y supuestos

- El relogueo por la seguridad del propio dispositivo (patrón o huella) lo gestiona la app cliente; el backend solo expone la renovación del token sobre una sesión no cerrada. El detalle del mecanismo del dispositivo pertenece al proyecto móvil.
- La autorización por rol en cada recurso se especifica como CU transversal (CU-18); este CU cubre la autenticación y el ciclo de vida de la sesión.
- El backend es el emisor y validador del token; no hay proveedor de identidad externo.

## 12. Performance esperado del CU

- La emisión y la validación del token deben mantenerse dentro de los objetivos de latencia del proyecto, sin agregar sobrecarga apreciable a las solicitudes de recursos.

## 15. Idempotencia y reintento

- El cierre de sesión es idempotente: repetir el cierre de una sesión ya cerrada deja el estado sin cambios y responde con éxito.
- La solicitud de token no se reintenta con clave de idempotencia: cada emisión produce una sesión nueva con su propia vigencia.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de inicio y cierre de sesión y revalidación, derivado de NB-01 (F-08). |
