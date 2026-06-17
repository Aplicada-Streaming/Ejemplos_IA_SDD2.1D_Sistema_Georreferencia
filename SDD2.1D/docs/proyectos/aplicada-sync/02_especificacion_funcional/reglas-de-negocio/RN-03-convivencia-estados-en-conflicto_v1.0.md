# RN-03 — Convivencia con estados en conflicto sin bloqueo

**Proyecto:** aplicada-sync
**Documento:** RN-03-convivencia-estados-en-conflicto_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Enunciado

El motor de sincronización trata un estado en conflicto reportado por el backend remoto como estado válido: lo sube, lo baja y lo expone como conviviente, sin abortar el ciclo ni bloquear la sincronización, y nunca decide por sí mismo la resolución del conflicto.

## 2. Justificación

La política de la solución difiere la resolución de conflictos a un momento posterior, fuera del alcance de la librería; mientras tanto, la operación debe continuar. Una librería que bloqueara la sincronización ante un conflicto detendría el flujo de datos del trabajo en campo, que es justamente lo que el negocio quiere evitar. La convivencia preserva la continuidad y mantiene la información accesible.

## 3. Ámbito de aplicación

Se evalúa durante la fase de bajada de un ciclo, cuando el backend marca una o más entidades como en conflicto, y en cualquier consulta de estado posterior. La regla mantiene al motor neutral respecto de la semántica del conflicto: lo registra y lo reporta, pero la decisión de unificar o separar pertenece al backend o a la aplicación host.

## 4. Consecuencia si se viola

Si el motor abortara el ciclo o bloqueara la cola ante un conflicto, incumpliría el contrato y dejaría datos sin propagar, contraviniendo el criterio de convivencia de NB-04. Una implementación que intente resolver el conflicto por su cuenta también viola la regla, porque excede el rol agnóstico de la librería y puede descartar información que otro actor debía conservar.

## 5. CU afectados

CU-03 (ejecutar la sincronización subir-luego-bajar), CU-05 (consultar estado del motor y cola de pendientes).

## 6. Pruebas que la verifican

- Verificación de que una bajada con una entidad marcada en conflicto se aplica sin abortar el ciclo (test previsto en 08 asociado a CU-03).
- Verificación de que el ciclo reporta los elementos en conflicto en su resumen sin bloquear la cola (test previsto en 08 asociado a CU-03).
- Verificación de que la consulta de estado expone los elementos en conflicto como convivientes y no resueltos por el motor (test previsto en 08 asociado a CU-05).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de convivencia con estados en conflicto sin bloqueo, derivada de NB-04 y de la política de convivencia y resolución diferida. |
