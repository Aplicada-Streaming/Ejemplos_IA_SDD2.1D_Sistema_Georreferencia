# RN-03 — Convivencia con conflictos de marcadores en el cliente

**Proyecto:** geovial-mobile
**Documento:** RN-03-convivencia-con-conflictos-en-el-cliente_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + Mobile UX Analyst

## 1. Enunciado de la regla

La app convive con los marcadores en conflicto como estado válido: cuando dos o más marcadores de un relevamiento caen dentro de un mismo radio, la app los crea, los mantiene accesibles y los sincroniza sin bloquear la recolección, difiriendo la resolución de unificación o separación al cierre del relevamiento desde la web.

## 2. Justificación

El negocio definió que el conflicto de marcadores solo afecta la estructura de catalogación y no debe interrumpir el trabajo de campo; la información debe quedar accesible durante toda la recolección y la decisión de resolver corresponde al jefe al cierre (intake §7, §12; NB-03; NB-05). La regla replica en el cliente la invariante del backend (geovial-api RN-03) y del motor de sincronización (aplicada-sync RN-03).

## 3. Ámbito de aplicación

Se evalúa al crear o mover marcadores en terreno (CU-03), al cargar fotos manualmente que generan marcadores próximos (CU-07) y al aplicar actualizaciones bajadas que vienen marcadas en conflicto por el backend (CU-06). No habilita a la app a resolver el conflicto: solo a registrarlo y mostrarlo.

## 4. Consecuencia si se viola

Si la app bloqueara la recolección o forzara una unificación ante un conflicto, se violaría la regla: la información dejaría de estar accesible y se le quitaría al jefe la decisión de cierre. La respuesta correcta es siempre aceptar el conflicto, mantenerlo accesible y reportarlo en el resumen de sincronización, sin abortar el ciclo.

## 5. CU afectados

CU-03 (centrar por GPS y crear o mover marcador), CU-06 (trabajar sin conexión y sincronizar), CU-07 (carga manual con radio de agrupación).

## 6. Pruebas que la verifican

- Crear un marcador dentro del radio de otro no bloquea la recolección y deja ambos accesibles (08, sobre CU-03).
- Una actualización bajada marcada en conflicto se aplica sin abortar el ciclo y se reporta como elemento en conflicto (08, sobre CU-06).
- Varias fotos manuales en un mismo radio se agrupan sin que el conflicto interrumpa la carga (08, sobre CU-07).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de convivencia con conflictos de marcadores en el cliente móvil, derivada de NB-03 y NB-05 y alineada con geovial-api RN-03 y aplicada-sync RN-03. |
