# US-09 — Dar de baja un relevamiento del alcance

**Proyecto:** geovial-api
**Documento:** US-09-baja-relevamiento_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-02 Relevamientos y ciclo de vida
**Prioridad MoSCoW:** Should
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero dar de baja un relevamiento de mi alcance, para retirar relevamientos que no corresponden sin afectar otros.

## 2. Contexto

CU-04 contempla, dentro de la gestión de relevamientos exigida por NB-02, la baja de un relevamiento que se creó por error o que ya no corresponde. RN-01 limita la operación al alcance del solicitante, de modo que el jefe no pueda dar de baja relevamientos de otro ámbito. Resuelve el problema de depurar el backlog de campo sin tocar el resto de los relevamientos.

## 3. Criterios de aceptación

- Given un jefe de área habilitado y un relevamiento de su alcance, When solicita su baja, Then el sistema da de baja ese relevamiento y no afecta a ningún otro.
- Given un relevamiento que pertenece a otro alcance, When el jefe intenta darlo de baja, Then el sistema rechaza la operación con el código FUERA_DE_ALCANCE.
- Given un identificador de relevamiento que no existe, When el jefe solicita su baja, Then el sistema responde con el código RECURSO_NO_ENCONTRADO.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-02 |
| CU cubiertos | CU-04 |
| BT derivadas | BT-05, BT-06, BT-08 |
| Tests previstos | acceptance/AT-09-baja-relevamiento; contract test de DELETE de relevamiento |

## 5. Prioridad y estimación

Should porque permite mantener limpio el conjunto de relevamientos, pero el ciclo de vida principal de NB-02 funciona sin la baja. 3 SP por Planning Poker (Fibonacci): es una operación acotada que requiere validar alcance, distinguir inexistencia de fuera de alcance y garantizar idempotencia.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-04)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-06, BT-08)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La baja es idempotente (RN-07): repetir la baja de un relevamiento ya dado de baja dentro del alcance devuelve el mismo resultado sin error de estado. La creación se trata en US-07 y la consulta en US-08.
