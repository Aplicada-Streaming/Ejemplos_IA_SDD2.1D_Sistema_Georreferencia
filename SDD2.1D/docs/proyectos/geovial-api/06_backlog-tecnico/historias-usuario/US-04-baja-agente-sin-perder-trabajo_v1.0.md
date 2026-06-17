# US-04 — Dar de baja un agente de campo sin perder su trabajo registrado

**Proyecto:** geovial-api
**Documento:** US-04-baja-agente-sin-perder-trabajo_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-01 Usuarios, sesión y autorización
**Prioridad MoSCoW:** Must
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero dar de baja a un agente de campo de mi área, para quitarle acceso conservando sus observaciones y fotos.

## 2. Contexto

CU-02 cubre la gestión de agentes de campo por el jefe de área, incluida su baja. RN-02 fija que la baja inhabilita la cuenta pero conserva la autoría de lo registrado, y RN-01 limita la operación al alcance del jefe. Resuelve el problema de desvincular a un agente del equipo sin perder las observaciones y fotos que aportó al relevamiento.

## 3. Criterios de aceptación

- Given un jefe de área habilitado y un agente activo de su área, When solicita su baja, Then el sistema inhabilita la cuenta del agente y sus observaciones conservan al mismo autor.
- Given un agente que pertenece a otra área, When el jefe intenta darlo de baja, Then el sistema rechaza la operación con el código FUERA_DE_ALCANCE y no altera ninguna fila.
- Given un agente ya inhabilitado dentro del alcance del jefe, When se repite la baja, Then el sistema devuelve el mismo resultado sin error de estado y sin alterar la autoría.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-01 |
| CU cubiertos | CU-02 |
| BT derivadas | BT-02, BT-05, BT-08 |
| Tests previstos | acceptance/AT-04-baja-agente; contract test de DELETE de agentes |

## 5. Prioridad y estimación

Must porque la rotación de agentes de campo es habitual y revocar su acceso sin perder su trabajo es un requisito duro de NB-01. 3 SP por Planning Poker (Fibonacci): es una transición de estado acotada que reutiliza la baja conservando autoría de US-02, sumando solo la verificación de alcance del jefe de área.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-02)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-05, BT-08)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

Las asignaciones vigentes del agente quedan revocadas al darlo de baja según RC-05, dejando los pares sin asignación vigente sin borrar el histórico. La baja es idempotente. La reasignación de otro agente a los relevamientos afectados se trata en US-11, fuera del scope de esta historia.
