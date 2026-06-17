# Criterios de validación — geovial-web

Proyecto: geovial-web
Documento: criterios-validacion_v1.0.md
Versión: 1.0
Estado: Propuesto
Fecha: 2026-06-15
Autor: Ingeniero QA / SDET (web-monolith)

## 1. Propósito

Define cuándo `geovial-web` está validado para release: el conjunto de criterios numéricos y verificables que, cumplidos en su totalidad sobre el ambiente de pruebas equivalente al productivo, habilitan declarar al front aceptable para promover. Estos criterios consolidan los quality gates de la estrategia de calidad y los objetivos de NFR de P.10, y se apoyan en la matriz de cobertura y en el catálogo de casos de prueba.

## 2. Criterios funcionales

- Cada CU crítico del front está cubierto por al menos un TC verde. Se consideran CU críticos los del camino principal del relevamiento y de la administración: CU-01 (sesión), CU-02 (usuarios por jerarquía), CU-03 (relevamientos), CU-04 (asignación), CU-05 (marcadores), CU-06 (revisión), CU-07 (resolución de conflictos) y CU-08 (transición y cierre).
- Los CU de capacidad Could Have (CU-10 portabilidad, CU-11 configuración de almacenamiento) y el CU-09 (carga manual, Should/Must según backlog) están cubiertos por al menos un TC verde si su tramo (Tramo 5) entró en el release; si quedan diferidos, se documenta la exclusión de alcance.
- Cada escenario Given/When/Then de los CU incluidos tiene su verificación en el TC correspondiente (matriz §2): happy path y al menos un edge case por CU.
- Las 5 RN tienen al menos un TC verde que verifica su invariante de presentación y flujo (matriz §4): visibilidad por rol (RN-01), conservación de autoría (RN-02), acceso restringido a administradores (RN-03), habilitación por estado (RN-04) y conflictos como precondición visible del cierre (RN-05).

## 3. Criterios no funcionales

Cada NFR cumple su objetivo numérico medido en el ambiente de pruebas equivalente al productivo (arquitectura §8, intake P.10).

| NFR | Objetivo | TC / mecanismo | Condición de validación |
| --- | --- | --- | --- |
| Latencia de interacción p95 | ≤ 200 ms sobre el circuito en red estable | TC-20 | El p95 de interacción sobre las vistas clave (CU-03, CU-06, CU-08) es ≤ 200 ms, excluyendo la latencia atribuible al backend |
| Circuitos concurrentes | ≥ 50 circuitos interactivos | TC-21 | Se sostienen al menos 50 circuitos concurrentes con p95 estable y sin pérdida de estado de sesión |
| Custodia del token | 0 exposiciones al navegador | TC-19 | El token bearer no se serializa al navegador en ninguna vista clave |
| Disponibilidad mensual | ≥ 99,5 % | Medición operativa (09) | La disponibilidad del contenedor de front medida en el ambiente de referencia alcanza ≥ 99,5 %; se reporta junto a la disponibilidad de geovial-api |

## 4. Criterios de regresión

- La suite de regresión completa (unidad, integración, componente de UI y snapshot) se ejecuta y queda en verde antes del release.
- Ningún TC que estaba verde en la versión anterior pasa a rojo en la nueva sin justificación documentada (regla 08 §4.10).
- Todo bug cerrado generó al menos un TC de regresión nuevo o extendió uno existente; ese TC está verde.
- Los snapshots de vistas clave (TC-22) coinciden con su baseline aprobado; cualquier regeneración tiene justificación y revisión registradas.

## 5. Criterios de calidad de código

- Cobertura por capa cumplida: Aplicación de UI ≥ 80 % líneas / ≥ 70 % branches; infraestructura (Cliente de API y adaptador de mapa) ≥ 70 % líneas / ≥ 60 % branches; presentación ≥ 60 % líneas / ≥ 50 % branches.
- Gate global de cobertura cumplido: líneas ≥ 80 %, branches ≥ 70 % sobre el conjunto del proyecto (intake §17 P.6). El gate global y los pisos por capa deben cumplirse simultáneamente: el global no compensa una capa por debajo de su piso.
- Mutation score: no aplica a `web-monolith` (regla 08 §2.2 lo reserva a `library`).
- Análisis estático sin issues críticos nuevos; compilación sin warnings tratados como error.

## 6. Excepciones documentadas

- Cualquier criterio no cumplido se acepta para release solo con ADR explícita y plan de remediación con BT asociada en el backlog (regla 08 §4.8).
- Un CU de capacidad Could (CU-10, CU-11) que quede fuera del release por priorización no es una excepción sino una exclusión de alcance: se documenta en el plan de pruebas y no bloquea el release del camino principal.
- Las ADR-04 y ADR-05 en estado Propuesto deben estar ratificadas antes de validar el release, porque condicionan la separación de capas y el mapeo de errores que los criterios verifican; mientras estén Propuestas, el release no se valida.
- El SLO de disponibilidad se valida por medición operativa y no por TC en CI; si el ambiente de referencia no permite la medición previa al release, la validación de disponibilidad se difiere a la operación con seguimiento explícito (no se considera incumplimiento si se monitorea).

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Criterios de validación iniciales de geovial-web: funcionales (CU críticos y 5 RN con TC verde), no funcionales (interacción p95 ≤ 200 ms, ≥ 50 circuitos, custodia del token, disponibilidad ≥ 99,5 %), regresión, calidad de código (cobertura por capa y gate global reconciliados, sin mutation por tipo) y excepciones solo con ADR y plan de remediación. |
