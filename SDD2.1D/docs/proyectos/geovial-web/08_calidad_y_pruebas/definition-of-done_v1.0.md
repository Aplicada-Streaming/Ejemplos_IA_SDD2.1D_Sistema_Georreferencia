# Definition of Done — geovial-web

Proyecto: geovial-web
Documento: definition-of-done_v1.0.md
Versión: 1.0
Estado: Propuesto
Fecha: 2026-06-15
Autor: Ingeniero QA / SDET (web-monolith)

Esta es la Definition of Done canónica de `geovial-web`. Es la fuente única de la condición de terminación: el `mini-plan_v1.0.md` de 07 la referencia y no la redefine, y la `definition-of-ready_v1.0.md` de 06 (cuándo empezar) no se solapa con ella (cuándo terminar). Cada criterio responde a la pregunta "¿cómo se valida?" con una operación mecánica: un comando, un check del pipeline o una métrica del reporte. Las cuatro capas son acumulativas: el release exige la DoD de sprint, que exige la de BT y la de US.

## 1. DoD por capa

### 1.1 Historia de usuario (US)

- [ ] Los criterios de aceptación Given/When/Then de la US están cubiertos por al menos un TC del catálogo (`casos-prueba-referenciales_v1.0.md`), happy path y edge case. Validación: la matriz de cobertura (§2) referencia el TC de cada CU asociado a la US y el TC está en verde en el reporte de la suite.
- [ ] La US referencia al menos un CU y la RN aplicable; la trazabilidad no tiene huérfanas. Validación: revisión de la fila de la US contra la matriz CU↔Tests y RN↔Tests.
- [ ] La lógica de presentación y de orquestación de la US pasa sus pruebas unitarias y, si cruza el contrato, sus pruebas de integración a través de la API contra base efímera. Validación: la suite de unidad e integración de la US está en verde en CI.
- [ ] Las acciones de la US se habilitan solo dentro del alcance del rol (RN-01, RN-03) y del estado vigente del relevamiento (RN-04); un rechazo del backend se mapea a feedback (ADR-05). Validación: TC de visibilidad por rol y de habilitación por estado en verde.
- [ ] Si la US toca una vista clave, su render es estable contra el snapshot baseline. Validación: TC-22 (snapshot) en verde sin diferencias no aprobadas.
- [ ] La cobertura del código nuevo de la US no baja los pisos por capa. Validación: el gate de cobertura por capa del pipeline pasa.

### 1.2 Tarea técnica (BT)

- [ ] La BT compila sin warnings tratados como error. Validación: stage de build del pipeline en verde.
- [ ] La BT tiene sus pruebas verificables: el código nuevo está cubierto por pruebas unitarias y, si integra el contrato o el componente de mapa, por pruebas de integración o de componente de UI. Validación: la suite de la BT en verde y el delta de cobertura no baja los pisos por capa.
- [ ] La BT respeta la dependencia unidireccional de capas (Presentación → Aplicación de UI → Cliente de API) y centraliza el consumo del contrato y el mapeo de errores donde corresponde (ADR-04, ADR-05). Validación: revisión de arquitectura sobre el cambio y análisis estático sin issues críticos nuevos.
- [ ] La BT no introduce deuda no registrada. Validación: cualquier atajo queda como BT explícita en el backlog (excepción §2).
- [ ] Si la BT es un spike (BT-06), entrega su informe o recomendación dentro de su caja temporal. Validación: el informe del spike existe y se elevó la decisión.

### 1.3 Sprint (tramo del mini-plan)

- [ ] Todos los ítems comprometidos del tramo cumplen su DoD de US o de BT. Validación: checklist por ítem del tramo en la bitácora de avance del mini-plan de 07.
- [ ] Los CU que avanzan en el tramo (trazabilidad por tramo de 07 §5) tienen sus TC en verde. Validación: la matriz de cobertura del tramo está actualizada y verde.
- [ ] La suite de regresión completa acumulada hasta el tramo está en verde; ningún TC verde anterior pasó a rojo sin justificación. Validación: corrida de regresión en CI.
- [ ] El gate de cobertura global (líneas ≥ 80 %, branches ≥ 70 %) y los pisos por capa (Aplicación de UI 80/70, infraestructura 70/60, presentación 60/50) se cumplen sobre el alcance del tramo. Validación: reporte de cobertura por capa del pipeline.
- [ ] La capacidad construida en el tramo es demostrable de punta a punta sobre el entorno de prueba (criterio de cierre de tramo de 07 §3). Validación: demostración del journey del tramo sobre el ambiente de prueba.
- [ ] Los defectos blocker y críticos del tramo están cerrados y cada uno generó su TC de regresión. Validación: tablero de defectos sin blockers abiertos del tramo.

### 1.4 Release

- [ ] Todos los criterios de validación de `criterios-validacion_v1.0.md` se cumplen. Validación: checklist de criterios de validación firmado.
- [ ] Cada CU crítico (CU-01 a CU-08) está cubierto por al menos un TC verde. Validación: matriz CU↔Tests sin pendientes en los CU críticos.
- [ ] Los NFR numéricos de P.10 están validados en el ambiente de referencia: latencia de interacción p95 ≤ 200 ms (TC-20), ≥ 50 circuitos concurrentes (TC-21), custodia del token con 0 exposiciones (TC-19). Validación: corrida de las pruebas de rendimiento, carga y no exposición en verde.
- [ ] La disponibilidad objetivo ≥ 99,5 % se mide o se deja en seguimiento operativo declarado (criterios-validacion §6). Validación: reporte de disponibilidad del ambiente de referencia o nota de seguimiento.
- [ ] El gate de cobertura global y por capa se cumple sobre el proyecto completo. Validación: reporte de cobertura del pipeline.
- [ ] El análisis estático no tiene issues críticos; la imagen de contenedor del front se construye, firma y publica. Validación: stages de análisis, firma y publicación del pipeline en verde (09).
- [ ] Las ADR en estado Propuesto que gobiernan el front (ADR-04, ADR-05) están ratificadas. Validación: índice de decisiones de 05 con ambas en estado Aceptado.

## 2. Excepciones admitidas

- Deuda técnica documentada: un ítem puede declararse Done con un atajo conocido solo si la deuda queda registrada como BT explícita en el backlog con su plan de remediación; el atajo no puede violar una RN ni un NFR crítico.
- Capacidad Could diferida: un CU de capacidad Could Have (CU-10, CU-11) que no entre en el release no bloquea la DoD de release del camino principal; se documenta como exclusión de alcance, no como excepción.
- Spike sin criterios de implementación cerrados: una BT de tipo spike (BT-06) se declara Done al entregar su informe o recomendación dentro de su caja temporal, sin exigir cobertura de implementación.
- Criterio de NFR no cumplido: solo se acepta el release con un NFR fuera de objetivo mediante ADR explícita y plan de remediación (regla 08 §4.8); no se admite excepción silenciosa.
- Toda excepción se registra en la nota del ítem y la aprueba el rol QA / SDET (estrategia-calidad §4).

## 3. Vigencia

Este documento es la fuente canónica de la Definition of Done de `geovial-web`. Los planes de ejecución (el `mini-plan_v1.0.md` de 07) la referencian y no la redefinen. Cualquier cambio en los criterios versionables de esta DoD se registra en la §4 (control de cambios) y se comunica al equipo en la revisión del tramo siguiente (regla 08 §3.4). El mini-plan de 07 vincula su sección "Definition of Done aplicada" a este documento.

## 4. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Definition of Done canónica inicial de geovial-web: DoD por las cuatro capas (US, BT, sprint/tramo, release) con criterios verificables mecánicamente (comando, check de pipeline o métrica de reporte), reconciliación del gate global de cobertura con los pisos por capa, validación de los NFR de P.10 en el release y excepciones explícitas (deuda registrada, capacidad Could diferida, spike, NFR fuera de objetivo solo con ADR). Es la DoD que referencia el mini-plan de 07. |
