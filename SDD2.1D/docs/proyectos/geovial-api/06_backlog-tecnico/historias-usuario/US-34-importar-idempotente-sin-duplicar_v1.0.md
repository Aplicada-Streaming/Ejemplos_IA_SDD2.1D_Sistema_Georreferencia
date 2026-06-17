# US-34 — Importar de forma idempotente sin duplicar un relevamiento

**Proyecto:** geovial-api
**Documento:** US-34-importar-idempotente-sin-duplicar_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-06 Portabilidad del relevamiento
**Prioridad MoSCoW:** Could
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como jefe de área o usuario raíz, quiero reimportar la misma unidad transferible sin duplicar el relevamiento, para repetir una importación interrumpida sin crear copias.

## 2. Contexto

CU-16 describe la importación de un relevamiento, que puede interrumpirse y reintentarse. NB-06 enmarca la portabilidad del relevamiento como capacidad Could Have de la API. RN-07 fija la idempotencia por clave: un reintento con la misma clave de idempotencia no debe producir un segundo relevamiento.

## 3. Criterios de aceptación

- Given una importación previa registrada bajo una clave de idempotencia, When se reimporta la misma unidad transferible con la misma clave, Then el sistema devuelve el resultado ya registrado sin duplicar el relevamiento.
- Given una clave de idempotencia ya usada para una importación, When se reenvía con un contenido distinto, Then el sistema responde con el código CLAVE_REUTILIZADA_INCONSISTENTE y no incorpora nada.
- Given una política que exige clave de idempotencia para importar, When la solicitud llega sin la clave, Then el sistema responde con el código CLAVE_REQUERIDA_AUSENTE.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-06 |
| CU cubiertos | CU-16 |
| BT derivadas | BT-11, BT-15 |
| Tests previstos | acceptance/AT-34-importar-idempotente; contract test de POST de importación reintentada |

## 5. Prioridad y estimación

Could porque acompaña a la importación de NB-06, que es Could Have; sin embargo, dentro de esa épica es necesaria para que una importación interrumpida pueda repetirse sin riesgo. 5 SP por Planning Poker (Fibonacci): se apoya en el servicio de idempotencia transversal y agrega la comparación de contenido y los rechazos por clave, con complejidad moderada.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-16)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-11)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La importación se apoya en el servicio de idempotencia transversal: la unicidad de la clave y la comparación de la huella del contenido provienen de esa capacidad y no se reimplementan en este flujo. La reconstrucción de la estructura del relevamiento se detalla en US-33.
