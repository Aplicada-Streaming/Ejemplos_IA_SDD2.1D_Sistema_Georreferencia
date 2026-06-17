# US-02 — Dar de baja un usuario conservando su autoría histórica

**Proyecto:** geovial-api
**Documento:** US-02-baja-usuario-conserva-autoria_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-01 Usuarios, sesión y autorización
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como administrador de un nivel, quiero dar de baja un usuario de mi alcance inhabilitando su acceso, para revocar el acceso sin borrar ni desatribuir lo que ese usuario registró.

## 2. Contexto

CU-01 describe la administración de la jerarquía, dentro de la cual la baja de un usuario debe revocar el acceso sin destruir el rastro de lo trabajado. RN-02 fija que la baja inhabilita la cuenta pero conserva la autoría: el modelo lógico §1.2 lleva el estado_habilitacion a INHABILITADO sin borrar la fila y las relaciones FK no tienen cascada (RN-02). Así se resuelve el problema de quitar acceso a una persona manteniendo intactos sus relevamientos y observaciones para la trazabilidad.

## 3. Criterios de aceptación

- Given un administrador habilitado y un usuario activo de su alcance, When solicita la baja del usuario, Then el sistema deja el estado_habilitacion en INHABILITADO, le revoca el acceso y sus relevamientos y observaciones conservan al mismo autor.
- Given un usuario ya inhabilitado o un usuario fuera del alcance del administrador, When se solicita su baja, Then el sistema rechaza la operación con el código FUERA_DE_ALCANCE y no altera ninguna fila.
- Given un usuario con relevamientos y observaciones registrados, When se confirma su baja, Then ninguna fila asociada se elimina y la autoría de cada registro permanece intacta.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-01 |
| CU cubiertos | CU-01 |
| BT derivadas | BT-02, BT-05, BT-06, BT-08 |
| Tests previstos | acceptance/AT-02-baja-conserva-autoria; contract test de DELETE de usuario |

## 5. Prioridad y estimación

Must porque sin baja no se puede revocar el acceso de una cuenta comprometida o desvinculada, y hacerlo sin perder autoría es un requisito duro de NB-01. 5 SP por Planning Poker (Fibonacci): la operación es acotada, pero exige verificar el alcance, garantizar la no cascada en el almacén relacional y asegurar la idempotencia de la transición de estado.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-01)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-05, BT-08)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La baja es idempotente (RN-07): repetir la baja sobre un usuario ya inhabilitado dentro del alcance no cambia el resultado ni produce error de estado. El alta de usuarios se trata en US-01 para no mezclar alta y baja en una sola historia. Queda fuera del scope cualquier borrado físico de cuentas, que no está contemplado en el modelo lógico §1.2.
