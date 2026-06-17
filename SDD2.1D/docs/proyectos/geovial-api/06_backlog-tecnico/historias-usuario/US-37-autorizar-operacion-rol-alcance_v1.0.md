# US-37 — Autorizar cada operación según el rol y el alcance del solicitante

**Proyecto:** geovial-api
**Documento:** US-37-autorizar-operacion-rol-alcance_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-01 Usuarios, sesión y autorización
**Prioridad MoSCoW:** Must
**Estimación:** 8 SP (Fibonacci)

## 1. Historia

Como cliente consumidor de la API, quiero que cada operación se autorice según mi rol y mi alcance antes de ejecutarse, para impedir que un rol opere fuera de su ámbito jerárquico.

## 2. Contexto

CU-18 describe la autorización por rol y alcance que precede a toda operación con efecto. NB-01 enmarca usuarios, sesión y autorización. RN-01 fija que cada nivel opera su propio ámbito jerárquico, RN-02 conserva la autoría en las bajas y RC-03 exige integridad de la jerarquía. ADR-03 resuelve materializar la autorización como un control previo y transversal.

## 3. Criterios de aceptación

- Given un rol con su ámbito definido, When solicita una operación sobre su nivel inmediato inferior dentro de su ámbito, Then el sistema la autoriza y la ejecuta.
- Given una operación dirigida a un recurso fuera del alcance del solicitante, When se evalúa la autorización, Then el sistema responde con el código FUERA_DE_ALCANCE y no produce ningún efecto.
- Given un rol sin permiso para la operación solicitada, When se evalúa la autorización, Then el sistema responde con el código ROL_NO_AUTORIZADO o JERARQUIA_NO_PERMITIDA y no produce ningún efecto.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-01 |
| CU cubiertos | CU-18, CU-01 |
| BT derivadas | BT-07, BT-08 |
| Tests previstos | acceptance/AT-37-autorizacion-rol; contract test de acceso fuera de alcance rechazado |

## 5. Prioridad y estimación

Must porque la autorización por rol y alcance es un invariante de seguridad de NB-01: sin ella, cualquier operación podría salir del ámbito jerárquico y violar RN-01 y RC-03. 8 SP por Planning Poker (Fibonacci): es un control transversal que se interpone antes de todo efecto, debe resolver rol y alcance para cada operación y distinguir los códigos FUERA_DE_ALCANCE, ROL_NO_AUTORIZADO y JERARQUIA_NO_PERMITIDA.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-18, CU-01)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-07, BT-08)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La autorización es transversal a todos los CU y se materializa como middleware previo a todo efecto (BT-08), de modo que ninguna operación con consecuencias se ejecuta sin pasar por el control. El acotamiento de los listados al alcance antes de paginar se detalla en US-38.
