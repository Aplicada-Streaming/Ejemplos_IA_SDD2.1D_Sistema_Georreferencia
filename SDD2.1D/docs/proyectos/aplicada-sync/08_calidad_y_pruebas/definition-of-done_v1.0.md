# Definition of Done — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** definition-of-done_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (AG-08), variante QA + SDET Library

Este documento es la fuente canónica de la Definition of Done del proyecto. Los planes de la categoría 07 referencian esta DoD; no la redefinen (08_rules §4.8 y §4.10, anti-patrón de DoD redefinida por sprint). El mini-plan de 07 §5 declara que cada US y BT se cierra contra esta DoD. La DoD se delimita frente a la Definition of Ready de 06 (`definition-of-ready_v1.0.md` §5): la DoR fija las condiciones para empezar, esta DoD fija las condiciones para terminar.

Cada criterio responde a la pregunta "¿cómo se valida?" con una operación mecánica concreta: un comando, un check del pipeline o una métrica de un reporte. Los gates G1 a G9 referenciados son los de `estrategia-calidad_v1.0.md` §3.

## 1. DoD por capa

### 1.1 DoD de una historia de usuario (US)

Una US está Done cuando:

- [ ] Sus criterios de aceptación Given/When/Then (product-backlog de 06 §3) tienen cada uno al menos un TC verde en `matriz-cobertura-pruebas_v1.0.md`. Validación: el TC referenciado figura con estado Verde en la tabla CU↔Tests de la matriz.
- [ ] La US referencia su CU y sus TC quedan trazados en la matriz sin huérfanos. Validación: la fila del CU en la matriz lista el TC y el TC referencia el CU/RN/NFR (08_rules §6).
- [ ] La suite unitaria y de contrato pasa en el pipeline para el código de la US. Validación: gate G2 y G3 en verde en la corrida del PR.
- [ ] La cobertura por capa del código tocado por la US no baja por debajo del umbral de su capa. Validación: gate G4 (dominio 85/80, infraestructura 70/60, global 80/70).
- [ ] Ningún test introducido carece de assert y ningún error se enmascara con catch silencioso. Validación: gate G2 más revisión del análisis estático G9.
- [ ] Si la US toca una invariante (orden, idempotencia, no duplicación, convivencia), su property-based asociado pasa. Validación: gate G6 sobre TC-12/TC-13/TC-16 según corresponda.
- [ ] Si la US altera la superficie pública, el cambio está clasificado (mayor/menor/parche) según ADR-03. Validación: comparación contra la matriz de compatibilidad (gate G8).

### 1.2 DoD de una tarea técnica (BT)

Una BT está Done cuando:

- [ ] Cumple sus criterios de aceptación técnicos del backlog de 06 §2 de forma verificable. Validación: el criterio "compila / los tests pasan / el contrato se respeta" se observa en la corrida del pipeline.
- [ ] Compila sin advertencias tratadas como error. Validación: gate G1.
- [ ] Los TC que la BT habilita están implementados y verdes, o declarados Pendiente con su tramo asignado en la matriz. Validación: estado del TC en la matriz de cobertura.
- [ ] La cobertura por capa del código de la BT cumple el umbral de su capa. Validación: gate G4.
- [ ] Si es una BT de infraestructura compartida (BT-01, BT-13), su contrato no rompe a las US consumidoras. Validación: gate G3 (contract tests) verde para las interfaces afectadas.
- [ ] Si es un spike (BT-03), entrega su informe con recomendación o documenta el bloqueo dentro de su caja temporal. Validación: existe el informe del spike; criterio de cierre de la DoR de 06 §3.

### 1.3 DoD de un tramo de release (equivalente a sprint en modo release-driven)

Un tramo (R1, R2, R3 del mini-plan de 07) está Done cuando:

- [ ] Todas las US y BT comprometidas en el tramo están Done según §1.1 y §1.2. Validación: estado de cada ítem en el mini-plan de 07 §3.
- [ ] Todos los TC asignados al tramo en el plan de pruebas §5 están verdes. Validación: estado de los TC en la matriz de cobertura.
- [ ] La suite de regresión del tramo anterior sigue verde. Validación: ningún TC verde previo pasó a rojo sin justificación (criterios de validación §4).
- [ ] La cobertura por capa del tramo cumple los umbrales. Validación: gate G4 sobre el agregado del tramo.
- [ ] El mutation score del dominio del tramo es >= 60 %. Validación: gate G5 ejecutado al cierre del tramo.
- [ ] La matriz de cobertura está actualizada al cierre del tramo (sin "Pendiente" para los TC del tramo). Validación: revisión de la matriz (08_rules §4.10).
- [ ] Cada NFR cuyo módulo entra en el tramo tiene su TC de medición verde. Validación: gate G7 sobre TC-09/TC-10/TC-14/TC-20 según el tramo.

### 1.4 DoD de release (paquete publicable)

El release (primera versión publicable de la superficie pública) está Done cuando:

- [ ] Los tres tramos R1, R2, R3 están Done según §1.3. Validación: estado de los tramos en el mini-plan de 07 §7.
- [ ] Todos los criterios de validación de `criterios-validacion_v1.0.md` (§2 a §6) están cumplidos o tienen excepción con ADR. Validación: checklist de criterios de validación cerrado.
- [ ] Los seis NFR numéricos del intake §17 P.10 están medidos y dentro de su SLA. Validación: gate G7 (TC-14, TC-20, TC-09/TC-10, TC-11/TC-12, TC-07/TC-13, TC-16).
- [ ] La superficie pública verifica compatibilidad: snapshot del contrato sin diferencias no justificadas y verificación post-publicación reproduce el contrato en un proyecto limpio. Validación: gate G8 (TC-21, BT-14, intake §17 P.8).
- [ ] La versión publicada respeta el versionado semántico: ningún cambio incompatible sin incremento de versión mayor. Validación: auditoría del changelog contra la matriz de compatibilidad (ADR-03 §8).
- [ ] El análisis estático no tiene issues críticos. Validación: gate G9.
- [ ] La cobertura global del paquete cumple el gate del intake §17 P.6 (>= 80 % líneas / >= 70 % branches) además de las coberturas por capa. Validación: gate G4 sobre el agregado total.

## 2. Excepciones admitidas

- Deuda técnica documentada: se puede declarar Done un ítem con un criterio no cumplido solo si la deuda queda registrada como una BT explícita en el backlog de 06 con su plan de remediación (08_rules §4.8). El criterio no cumplido se nombra en la nota del ítem.
- NFR fuera del tramo: un NFR cuyo módulo no entra en el tramo en curso no bloquea la DoD de ese tramo, pero sí la DoD de release (§1.4). TC-14 y TC-20 deben estar verdes antes del release global.
- Umbral de cobertura: bajar un umbral de cobertura o de mutation score requiere un ADR registrado en `estrategia-calidad_v1.0.md` (08_rules §2.2, piso no techo); sin ese ADR, el gate G4/G5 en rojo bloquea la DoD.
- Spike sin recomendación clara (BT-03): se cierra documentando el bloqueo dentro de la caja temporal y adoptando el registro explícito por defecto de ADR-02 (mini-plan de 07 §6, riesgo del spike), sin frenar el tramo.

## 3. Vigencia

Este documento es la fuente canónica de la DoD del proyecto. Cualquier cambio en sus criterios versionables se registra en el control de cambios de §4 y se comunica al equipo en la siguiente revisión de release (08_rules §3.4). Los planes de 07 referencian esta DoD por nombre; no copian ni redefinen sus criterios. Al pasar de v1.0 a una versión mayor, la versión anterior se mueve a `_legacy/` con estado Superado (08_rules §3.4).

## 4. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | DoD canónica inicial de aplicada-sync con cuatro capas (US, BT, tramo de release, release), cada criterio con su operación mecánica de validación ligada a los gates G1 a G9 y a los TC de la matriz, excepciones admitidas (deuda con BT, NFR fuera de tramo, umbral con ADR, spike sin recomendación) y delimitación frente a la DoR de 06. Es la DoD que referencia el mini-plan de 07 §5. Derivada de 06, 07, del intake §17 P.6/P.10 y de las reglas 08 §4.8. |
