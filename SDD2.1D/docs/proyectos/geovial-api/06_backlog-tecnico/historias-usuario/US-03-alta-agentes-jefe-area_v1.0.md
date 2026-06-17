# US-03 — Dar de alta agentes de campo por el jefe de área

**Proyecto:** geovial-api
**Documento:** US-03-alta-agentes-jefe-area_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-01 Usuarios, sesión y autorización
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero dar de alta directamente a un agente de campo de mi área, para incorporar agentes a mi equipo sin pasar por un nivel superior.

## 2. Contexto

CU-02 describe el alta directa de un agente de campo por parte del jefe de área (F-02), de modo que el jefe pueda armar su equipo sin escalar a un nivel superior. RN-01 fija que cada nivel opera solo dentro de su alcance y RC-03 garantiza la integridad de la cadena de administración (el agente creado queda colgado del jefe que lo da de alta). Resuelve el problema de incorporar personal de campo con autonomía operativa en el área.

## 3. Criterios de aceptación

- Given un jefe de área habilitado, When da de alta un agente de campo dentro de su área, Then el sistema crea el agente con su rol y con el jefe como administrador, y devuelve su representación sin exponer el secreto de credencial.
- Given un usuario cuyo rol no es jefe de área, When intenta dar de alta un agente de campo, Then el sistema rechaza la operación con el código ROL_NO_AUTORIZADO y no crea nada.
- Given un jefe de área que reintenta el alta con la misma clave de idempotencia, When repite la solicitud, Then el sistema no duplica el agente y devuelve la representación del alta ya realizada.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-01 |
| CU cubiertos | CU-02 |
| BT derivadas | BT-02, BT-04, BT-05, BT-08 |
| Tests previstos | acceptance/AT-03-alta-agente; contract test de POST de agentes |

## 5. Prioridad y estimación

Must porque sin agentes de campo dados de alta no hay quién recolecte en el terreno; es habilitante del ciclo de relevamientos de NB-01. 5 SP por Planning Poker (Fibonacci): el alta es directa, pero suma validación de rol y alcance, integridad de la cadena de administración (RC-03) y manejo de idempotencia por clave.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-02)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-05, BT-08)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

El auto-registro self-service de agentes queda fuera del alcance de la versión 1 (Won't Have v1, F-18); el alta es siempre iniciada por el jefe de área. La baja del agente de campo se trata en US-04 para no mezclar alta y baja en una sola historia. El secreto de credencial nunca sale en ninguna respuesta.
