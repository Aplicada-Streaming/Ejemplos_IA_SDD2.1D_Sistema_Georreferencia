# US-01 — Administrar la jerarquía de usuarios del nivel inmediato inferior

**Proyecto:** geovial-api
**Documento:** US-01-administrar-jerarquia-usuarios_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-01 Usuarios, sesión y autorización
**Prioridad MoSCoW:** Must
**Estimación:** 8 SP (Fibonacci)

## 1. Historia

Como administrador de un nivel de la jerarquía, quiero crear, consultar y listar los usuarios del nivel inmediato inferior dentro de mi alcance, para mantener actualizada la estructura de la organización sin poder operar fuera de mi ámbito.

## 2. Contexto

NB-01 exige una administración jerárquica de usuarios en cuatro niveles con control de acceso. CU-01 describe la administración de la jerarquía y RN-01 fija que cada nivel administra solo el inmediato inferior y opera solo su ámbito; RC-03 garantiza la integridad de la cadena de administración (un usuario que no es raíz tiene siempre un administrador). Sin esta historia no existe la estructura de cuentas sobre la que se apoyan los relevamientos y la autorización.

## 3. Criterios de aceptación

- Given un administrador habilitado, When crea un usuario del nivel inmediato inferior dentro de su alcance, Then el sistema da de alta el usuario con su rol y administrador y devuelve su representación sin exponer el secreto de credencial.
- Given un administrador habilitado, When intenta crear un usuario de un nivel que no es el inmediato inferior o fuera de su alcance, Then el sistema rechaza la operación con el código de jerarquía no permitida y no crea nada.
- Given un administrador habilitado, When lista los usuarios de su alcance, Then el sistema devuelve solo los usuarios que administra, paginados, sin incluir cuentas de otro ámbito.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-01 |
| CU cubiertos | CU-01 |
| BT derivadas | BT-01, BT-02, BT-04, BT-05, BT-06, BT-08, BT-10 |
| Tests previstos | acceptance/AT-01-administracion-jerarquia; contract test de POST y GET de usuarios |

## 5. Prioridad y estimación

Must porque sin la jerarquía de usuarios no hay control de acceso ni propietarios de relevamientos; es fundacional de NB-01. 8 SP por Planning Poker (Fibonacci): involucra el agregado de jerarquía, la restricción de cadena de administración a nivel del almacén (RC-03) y la autorización por alcance.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-01)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-02, BT-05, BT-08)
- [x] Insumos de prueba identificados (esquema de Usuario, códigos de jerarquía)

## 7. Notas y supuestos

La autorización por rol y alcance que esta US invoca se materializa en US-37 y en BT-08; aquí se asume disponible. La baja de usuario con conservación de autoría se trata en US-02 para no mezclar alta y baja en una sola historia. El secreto de credencial nunca sale en ninguna respuesta (modelo lógico §1.2).
