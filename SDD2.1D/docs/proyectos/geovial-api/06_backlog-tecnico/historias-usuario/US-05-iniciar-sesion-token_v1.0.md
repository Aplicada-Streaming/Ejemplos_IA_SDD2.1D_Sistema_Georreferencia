# US-05 — Iniciar sesión y obtener un token de acceso

**Proyecto:** geovial-api
**Documento:** US-05-iniciar-sesion-token_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-01 Usuarios, sesión y autorización
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como usuario de cualquier rol, quiero iniciar sesión con mis credenciales y recibir un token bearer, para autenticarme una vez y operar la API con ese token.

## 2. Contexto

CU-03 describe el inicio de sesión, base del control de acceso exigido por NB-01. ADR-03 fija el uso de un token bearer como mecanismo de autenticación de las operaciones posteriores. RN-02 impone que un usuario inhabilitado no puede iniciar sesión. Resuelve el problema de autenticar una sola vez y portar la identidad en cada operación sin reenviar credenciales.

## 3. Criterios de aceptación

- Given un usuario habilitado con credenciales válidas, When solicita iniciar sesión, Then el sistema devuelve un token bearer con su vigencia sin exponer ningún secreto de credencial.
- Given un usuario que envía credenciales inválidas, When solicita iniciar sesión, Then el sistema rechaza la operación con el código CREDENCIALES_INVALIDAS y no emite token.
- Given un usuario inhabilitado con credenciales por lo demás correctas, When solicita iniciar sesión, Then el sistema rechaza la operación con el código USUARIO_INHABILITADO y no emite token.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-01 |
| CU cubiertos | CU-03 |
| BT derivadas | BT-07, BT-09 |
| Tests previstos | acceptance/AT-05-inicio-sesion; contract test de POST de sesiones |

## 5. Prioridad y estimación

Must porque sin inicio de sesión no hay token y, por lo tanto, ninguna operación autenticada de la API es posible; es la puerta de entrada de NB-01. 5 SP por Planning Poker (Fibonacci): exige verificar credenciales, contemplar el estado de habilitación, emitir el token con su vigencia y devolver errores estables sin filtrar qué factor falló más allá de lo previsto.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-03)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-07)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

El formato físico del token y su firma pertenecen al stack y no se fijan en esta historia, que solo define el contrato. El inicio de sesión es la única operación anónima de la API: el resto exige token bearer. El cierre y la revalidación de sesión se tratan en US-06.
