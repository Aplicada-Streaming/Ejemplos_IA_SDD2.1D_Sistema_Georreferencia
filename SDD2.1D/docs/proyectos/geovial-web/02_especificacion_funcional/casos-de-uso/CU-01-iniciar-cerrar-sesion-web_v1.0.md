# CU-01 — Iniciar y cerrar sesión en el front web

**Proyecto:** geovial-web
**Documento:** CU-01-iniciar-cerrar-sesion-web_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional

## 1. Propósito

Permitir que un jefe de área, un jefe general o el usuario raíz ingrese al front web entregando sus credenciales y obtenga una sesión de trabajo, y que cierre esa sesión para abandonar el front sin dejar acceso disponible. Es la puerta de entrada que asocia toda acción posterior del front a un usuario identificado con su rol jerárquico.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Usuario administrador (raíz, jefe general o jefe de área) | Primario | Entrega credenciales para ingresar y solicita el cierre de su sesión |
| Front web | Sistema | Recibe credenciales, las envía al backend y mantiene la sesión del lado del servidor |
| Backend de dominio | Sistema | Valida las credenciales y entrega o invalida el token de acceso |

## 3. Precondiciones

- El usuario existe y está habilitado en el backend de dominio.
- El front web puede alcanzar el backend de dominio sobre la red.

## 4. Flujo principal

1. El usuario abre el front web y solicita ingresar.
2. El usuario entrega su identificador de acceso y su credencial en el formulario de ingreso.
3. El front web envía las credenciales al backend y obtiene un token de acceso que porta el rol del usuario y una vigencia limitada.
4. El front web conserva el token del lado del servidor, asociado a la sesión del usuario, sin exponerlo al navegador.
5. El front web habilita las pantallas y acciones que corresponden al rol recibido y muestra al usuario su identidad y su rol.
6. El usuario solicita cerrar su sesión; el front web pide al backend invalidar el token y descarta el estado de sesión del lado del servidor.
7. El front web vuelve a la pantalla de ingreso, sin rastros de la identidad anterior.

## 5. Flujos alternativos

- 5.A Sesión expirada durante el uso. Disparador: el token asociado a la sesión vence mientras el usuario navega. El front web detecta el rechazo del backend, informa que la sesión expiró y lleva al usuario a la pantalla de ingreso para reautenticarse. Retorna al paso 2.
- 5.B Front sin alcance al backend. Disparador: al enviar las credenciales el backend no responde. El front web informa que el servicio no está disponible y conserva el identificador ingresado para reintentar, sin emitir sesión. Retorna al paso 2.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| CREDENCIALES_INVALIDAS | El identificador o la credencial no coinciden en el backend | El front web informa credenciales inválidas y permanece en el ingreso, sin abrir sesión |
| USUARIO_INHABILITADO | El usuario fue dado de baja y conserva traza pero no acceso | El front web informa que el acceso está revocado y no abre sesión |
| ROL_SIN_ACCESO_WEB | El rol recibido es agente de campo, que opera desde la aplicación de campo y no desde el front web | El front web informa que ese rol no tiene acceso a esta herramienta y cierra la sesión recién abierta |

## 7. Postcondiciones

- Éxito en ingreso: el usuario tiene una sesión activa en el front web con su rol, y las pantallas habilitadas reflejan ese rol.
- Éxito en cierre: la sesión queda cerrada, el token invalidado y el front no conserva la identidad anterior.
- Fallo: no se abre sesión y el usuario permanece en la pantalla de ingreso.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un jefe de área habilitado con identificador "jarea.norte" y credencial válida | Ingresa sus credenciales en el front web | El front abre una sesión con rol jefe de área y muestra las pantallas de relevamientos y agentes |
| CA-02 | Un usuario con credencial incorrecta | Ingresa al front web | El front informa CREDENCIALES_INVALIDAS y permanece en la pantalla de ingreso sin sesión |
| CA-03 | Un jefe general con sesión activa en el front web | Solicita cerrar su sesión y luego intenta volver con el botón de retroceso del navegador | El front no recupera la sesión anterior y exige ingresar de nuevo |
| CA-04 | Un usuario dado de baja que conserva su identificador "agente.baja" | Ingresa al front web | El front informa USUARIO_INHABILITADO y no abre sesión |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-01 |
| Reglas de negocio aplicables | RN-01 (geovial-web), RN-03 (geovial-web) |
| Historias de usuario a generar | US-01, US-02 (en 06) |
| Componentes esperados | Pantalla de ingreso; servicio de sesión del lado del front; consumo del recurso de autenticación del backend (referencia tentativa a 05) |
| Tests previstos | Ingreso con credenciales válidas habilita el rol; rechazo de credenciales inválidas; cierre no recupera la sesión; rol sin acceso web rechazado (en 08) |

## 10. Notas y supuestos

- El front web no persiste estado de dominio: la validez de las credenciales y la vigencia de la sesión son competencia del backend (intake §17 geovial-web P.4, P.5). El front solo mantiene estado efímero de sesión del lado del servidor.
- El relogueo por la seguridad del propio dispositivo (patrón o huella) es un flujo de la aplicación de campo, no del front web; aquí el reingreso es siempre por credenciales.
- El rol agente de campo opera desde la aplicación de campo; el front web es la herramienta de los roles administradores. La carga manual del agente vía web (CU-09) es la excepción acotada en que un agente sí ingresa al front.

## 13. Interacción multiusuario y concurrencia

- Cada usuario administra su propia sesión; el cierre de uno no afecta las sesiones de otros usuarios sobre el mismo front.
- El front no comparte estado de dominio entre sesiones: lo que cada usuario ve depende del alcance de su rol resuelto por el backend.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de inicio y cierre de sesión en el front web, derivado de NB-01 (F-08). |
