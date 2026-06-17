# US-38 — Acotar cada listado al alcance jerárquico antes de paginar

**Proyecto:** geovial-api
**Documento:** US-38-acotar-listado-alcance-antes-paginar_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-01 Usuarios, sesión y autorización
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como cliente consumidor de la API, quiero que todo listado se acote a mi alcance antes de aplicar la paginación, para no ver ni contar recursos fuera de mi ámbito.

## 2. Contexto

CU-18 fija la autorización por rol y alcance, y CU-20 describe los listados paginados de recursos. NB-01 enmarca usuarios, sesión y autorización. RN-01 establece que el alcance jerárquico se aplica antes de paginar, de modo que el conjunto que se pagina ya está acotado al ámbito del solicitante.

## 3. Criterios de aceptación

- Given un solicitante con su alcance definido, When pide un listado de recursos, Then el sistema solo incluye y solo cuenta los recursos de su alcance.
- Given un recurso de otro ámbito, When el solicitante lista ese tipo de recurso, Then el recurso no aparece en la página ni se suma al conteo total.
- Given un conjunto ya acotado al alcance, When se aplica la paginación, Then la paginación opera sobre el conjunto acotado y no sobre el total del sistema.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-01 |
| CU cubiertos | CU-18, CU-20 |
| BT derivadas | BT-08, BT-10 |
| Tests previstos | acceptance/AT-38-alcance-antes-paginar; contract test de listado acotado por alcance |

## 5. Prioridad y estimación

Must porque acotar antes de paginar es un invariante de seguridad de NB-01: si el alcance se aplicara después, los conteos y las páginas filtrarían información de ámbitos ajenos, violando RN-01. 5 SP por Planning Poker (Fibonacci): exige insertar el acotamiento por alcance en la construcción de toda consulta de listado y verificar que tanto la página como el conteo total respeten el orden invariante.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-18, CU-20)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-08, BT-10)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

Depende del servicio de paginación detallado en US-40, que opera sobre el conjunto ya acotado. El orden de aplicar el alcance antes de paginar es un invariante de seguridad y no debe poder invertirse desde la configuración de un listado.
