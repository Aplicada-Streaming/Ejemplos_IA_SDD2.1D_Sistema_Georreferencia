# 07 Plan de sprint — geovial-storage

**Proyecto:** geovial-storage
**Tipo (D8):** library (release-driven)
**Variante:** Scrum Master + Maintainer Lead
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Scrum Master + Maintainer Lead
**Modo:** mini-plan (equipo_n=1)

Punto de entrada navegable de la sección 07 de `geovial-storage`, la librería que expone al backend de GeoVial una abstracción de alojamiento de archivos transparente con proveedores intercambiables (local / remoto / otro) seleccionables por el usuario raíz. Por ser un proyecto de un solo desarrollador (equipo_n=1), la categoría 07 se reduce al modo mini-plan documentado (regla 07 §2.1/§2.2): un plan único condensado sustituye a los cuatro artefactos completos de sprint y no se omite.

## Modo de esta sección

| Aspecto | Valor |
| --- | --- |
| Tamaño de equipo | 1 desarrollador (equipo_n=1) |
| Artefacto vigente | `mini-plan_v1.0.md` |
| Artefactos no generados | `plan-iteracion-sprint-XX_v1.0.md`, `template-sprint-review_v1.0.md`, `template-sprint-retrospectiva_v1.0.md`, `velocidad-equipo_v1.0.md` |
| Justificación de la omisión | Regla 07 §2.2, escenario equipo de 1 dev: el mini-plan sustituye a los cuatro anteriores |
| Naturaleza del plan | Release-driven (variante `library`, regla 07 §1.2) |

## Documentos de la sección

- [mini-plan_v1.0.md](mini-plan_v1.0.md) — plan único condensado: objetivo de valor, 22 ítems comprometidos (US-01 a US-09 y BT-01 a BT-13, 108 SP) en cinco tramos por dependencias, orden de construcción, DoD por referencia a la canónica de 08, riesgos con mitigación, bitácora semanal y trazabilidad a CU, NB y ADRs.

## Resumen del plan

- Objetivo: publicar una versión inicial estable de la abstracción de almacenamiento con las cinco operaciones de datos, la activación del proveedor activo y la transparencia verificada sobre al menos dos proveedores soportados.
- Estimación: story points en escala Fibonacci (1, 2, 3, 5, 8, 13), coherente con 06.
- Volumen: 37 SP en historias y 71 SP en tareas técnicas; 108 SP en total.
- Tramos release-driven: fundaciones del contrato (EP-04), guardado (EP-01), operaciones de lectura/borrado/verificación/listado (EP-01), configuración del proveedor (EP-02), proveedores intercambiables y gate de transparencia (EP-03).

## Trazabilidad

- Upstream: 06 (product-backlog US-01 a US-09, backlog-tecnico BT-01 a BT-13, DoR); 05 (ADR-01 a ADR-05, `contratos-abstractions_v1.0.md`, `extensibilidad_v1.0.md`); 02 (CU-01 a CU-06, NB-07/NB-03/NB-06, RN-01/RN-02/RN-03).
- CU que avanzan: CU-01 a CU-06.
- NB que avanzan: NB-07 (principal), NB-03 y NB-06 (soporte).
- Downstream: 08 (acceptance tests por US y batería de contrato; DoD canónica referenciada y pendiente de publicar), 09 (mecanismo de almacenamiento seguro en reposo, intake §17.P.5).

## Revisores acotados

- AG-06 (Product Owner / Backlog): confirma que las US y BT comprometidas son las correctas según prioridad.
- AG-05 (Arquitecto): valida que el orden de construcción respeta las ADRs y dependencias de 05.
- AG-08 (QA): acuerda los casos de prueba que acompañan a cada US y la DoD aplicada.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 07 de geovial-storage en modo mini-plan (equipo_n=1): índice del mini-plan, declaración del modo y de los artefactos omitidos con su justificación, resumen del plan release-driven y trazabilidad upstream/downstream con revisores acotados. |
