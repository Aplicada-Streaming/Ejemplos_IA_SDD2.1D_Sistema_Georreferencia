# US-06 — Cerrar sesión y revalidar la sesión activa

**Proyecto:** geovial-api
**Documento:** US-06-cerrar-sesion-revalidar_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-01 Usuarios, sesión y autorización
**Prioridad MoSCoW:** Should
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como usuario de cualquier rol, quiero cerrar la sesión completa y revalidar la sesión activa, para cambiar de usuario en el dispositivo y mantener la sesión vigente cuando corresponde.

## 2. Contexto

CU-03 contempla, además del inicio de sesión, el cierre completo para permitir el cambio de usuario en un dispositivo compartido y la revalidación de la sesión activa para confirmar que el token sigue siendo válido. ADR-03 define el token bearer sobre el que operan ambas acciones. Resuelve el problema de soltar la identidad anterior antes de operar con otra y de comprobar la vigencia sin volver a pedir credenciales.

## 3. Criterios de aceptación

- Given un usuario con sesión iniciada, When solicita el cierre de la sesión actual, Then el sistema invalida el token y las operaciones posteriores con ese token quedan rechazadas.
- Given un usuario con sesión activa, When solicita la revalidación de su sesión, Then el sistema confirma la vigencia del token sin emitir credenciales nuevas.
- Given una solicitud de cierre o de revalidación sin un token válido, When llega al sistema, Then se rechaza con el código CREDENCIALES_INVALIDAS y no se altera ninguna sesión.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-01 |
| CU cubiertos | CU-03 |
| BT derivadas | BT-07 |
| Tests previstos | acceptance/AT-06-cierre-revalidacion; contract test de DELETE de sesión actual y POST de revalidación |

## 5. Prioridad y estimación

Should porque mejora la operación en dispositivos compartidos y da control de vigencia, pero la API sigue siendo usable con solo el inicio de sesión de US-05. 3 SP por Planning Poker (Fibonacci): son dos operaciones acotadas sobre el token ya emitido, sin nuevas reglas de jerarquía ni de alcance.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-03)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-07)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La seguridad física del dispositivo y el resguardo local del token los gobierna el cliente móvil; esta historia solo define el contrato del backend para cierre y revalidación. El inicio de sesión que provee el token se trata en US-05.
