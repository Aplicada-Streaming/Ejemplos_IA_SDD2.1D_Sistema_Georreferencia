# RN-05 — Captura sin conexión con cola local persistente

**Proyecto:** geovial-mobile
**Documento:** RN-05-captura-sin-conexion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + Mobile UX Analyst

## 1. Enunciado de la regla

Toda la captura de campo —crear y mover marcadores, tomar fotos con resolución de coordenadas en el momento, registrar notas, comentarios y etiquetas— es realizable sin conexión, registrándose en el almacén local del dispositivo y encolándose como cambio local pendiente que se conserva hasta su sincronización confirmada, sin pérdida.

## 2. Justificación

El trabajo de campo ocurre en lugares sin conectividad y la captura en el momento no puede depender de la red; sin captura offline confiable, la georreferenciación no puede ejercitarse donde más se necesita (NB-04, NB-03, intake §1, §3, §4 F-07, §11 R-03). La cola local persistente es la base de la sincronización predecible (RN-02) y de la mitigación del riesgo de pérdida de datos.

## 3. Ámbito de aplicación

Se evalúa en toda operación de recolección sin conexión: selección del relevamiento desde la copia local (CU-02), creación y movimiento de marcadores (CU-03), captura de fotos (CU-04), registro de comentarios y etiquetas (CU-05) y carga manual de fotos (CU-07). La sincronización que vacía la cola se rige por CU-06.

## 4. Consecuencia si se viola

Si un cambio capturado sin conexión no se registrara en el almacén local ni se encolara, se perdería trabajo de campo, lo que viola la regla. La respuesta correcta es persistir cada cambio localmente y conservarlo en la cola hasta que el backend lo confirme en una sincronización; un corte de sincronización no debe descartar cambios pendientes (ver RN-02).

## 5. CU afectados

CU-02 (seleccionar relevamiento asignado), CU-03 (centrar por GPS y crear o mover marcador), CU-04 (capturar foto con resolución de coordenadas), CU-05 (agregar comentarios y etiquetas), CU-07 (carga manual de fotos).

## 6. Pruebas que la verifican

- Crear marcador, capturar foto y registrar comentario sin conexión deja los cambios en el almacén local y en la cola (08, sobre CU-03, CU-04, CU-05).
- La cola local conserva al menos 1000 cambios pendientes sin pérdida (08, sobre CU-06).
- Tras un corte de sincronización, los cambios no confirmados permanecen en la cola (08, sobre CU-06).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de captura sin conexión con cola local persistente, derivada de NB-04 y NB-03 (F-07). |
