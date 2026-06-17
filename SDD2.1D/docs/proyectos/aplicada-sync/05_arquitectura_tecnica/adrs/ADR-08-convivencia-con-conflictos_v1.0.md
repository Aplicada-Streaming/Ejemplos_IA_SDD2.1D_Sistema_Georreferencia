# ADR-08 — Convivencia con estados en conflicto sin bloqueo

**Proyecto:** aplicada-sync
**Documento:** ADR-08-convivencia-con-conflictos_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Estilo

## 1. Contexto

La política de la solución difiere la resolución de conflictos a un momento posterior, fuera del alcance de la librería. Mientras tanto, la operación de campo debe continuar: una librería que bloqueara la sincronización ante un conflicto detendría el flujo de datos, que es justamente lo que el negocio quiere evitar. El motor es agnóstico del dominio y no tiene criterio para resolver un conflicto. Lo motivan RN-03 (convivencia con estados en conflicto sin bloqueo), CU-03 (el backend reporta conflictos durante la bajada), CU-05 (consulta de elementos en conflicto) y el NFR de continuidad ante conflicto del intake §17 P.10 (cero ciclos bloqueados).

## 2. Decisión

El motor trata un estado en conflicto reportado por el backend como estado válido: lo sube, lo baja y lo expone como conviviente, sin abortar el ciclo ni bloquear la cola, y nunca decide por sí mismo la resolución. Los elementos en conflicto se incluyen en el resumen del ciclo y se exponen en la consulta de estado como convivientes, no resueltos.

## 3. Estado

Aceptado el 2026-06-15. Pre-tomada por la política de convivencia y resolución diferida de la solución (intake §17 P.3/P.11) y formalizada por RN-03.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Convivencia: aplicar y reportar sin resolver (elegida) | La operación no se detiene; la información queda accesible; el motor permanece agnóstico | El conflicto persiste hasta que otro actor lo resuelva |
| Bloquear el ciclo ante un conflicto | Fuerza una resolución inmediata | Detiene el flujo de datos en campo; contradice NB-04 y RN-03 |
| Resolver el conflicto dentro del motor | Conflicto resuelto sin intervención | Excede el rol agnóstico; puede descartar información que otro actor debía conservar; viola RN-03 |

## 5. Consecuencias positivas

- La sincronización nunca se bloquea por un conflicto: la operación de campo continúa (cumple RN-03 y la continuidad de NB-04).
- La información en conflicto queda accesible y visible para quien deba resolverla más tarde (CU-05).
- El motor permanece neutral y reutilizable: no incorpora criterio de dominio para resolver.

## 6. Consecuencias negativas y trade-offs

- Se acepta que el conflicto persista hasta que el backend o la aplicación host lo resuelvan; el motor no lo cierra.
- Se acepta exponer una condición reportada (no un error de bloqueo) que el integrador debe saber interpretar y atender fuera del motor.
- La resolución queda fuera del contrato de la librería; un consumidor que espere resolución automática debe ajustar su expectativa.

## 7. Implementación

Materializada por el Ejecutor de fase de bajada (aplica el estado en conflicto), el Orquestador del ciclo (lo incluye en el resumen) y el Registro de estado (lo expone en la consulta) (arquitectura §3). Convención: el elemento en conflicto se reporta con un código de condición no bloqueante (ver catálogo de errores de 03); jamás dispara un aborto del ciclo ni un bloqueo de la cola.

## 8. Métricas de validación

- 0 ciclos abortados por un estado en conflicto reportado por el backend (NFR de continuidad, arquitectura §8; intake §17 P.10).
- Una bajada con una entidad en conflicto se aplica sin abortar y se reporta en el resumen (CU-03 CA-04; RN-03).
- La consulta de estado expone los elementos en conflicto como convivientes y no resueltos por el motor (CU-05 CA-03).

## 9. Referencias

- RN-03; CU-03, CU-05.
- NB-04; SOLUTION-INTAKE §17 P.3, P.10, P.11 (aplicada-sync).
- ADR-05 (el ciclo no se detiene por conflicto).
- Catálogo de errores de la categoría 03 (condición ELEMENTO_EN_CONFLICTO).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión inicial de convivencia con estados en conflicto sin bloqueo y sin resolución por el motor. |
