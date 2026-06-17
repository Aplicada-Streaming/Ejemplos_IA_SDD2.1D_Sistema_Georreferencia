# RN-01 — Visibilidad y acciones por rol jerárquico en el front web

**Proyecto:** geovial-web
**Documento:** RN-01-visibilidad-acciones-por-rol_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional

## 1. Enunciado de la regla

El front web presenta a cada usuario solo las pantallas, los recursos y las acciones que su rol jerárquico alcanza: el usuario raíz ve la configuración del sistema y la administración del jefe general; el jefe general administra a los jefes de área; el jefe de área administra a sus agentes y opera sus propios relevamientos; y ningún usuario ve ni opera recursos fuera de su ámbito.

## 2. Justificación

El front es la herramienta de los roles administradores y debe reflejar fielmente el control de acceso jerárquico del dominio, para que no se ofrezcan acciones que el backend rechazaría ni se expongan datos de otros ámbitos; deriva de la regla de jerarquía del backend (RN-01 de geovial-api) y de NB-01 (intake §2).

## 3. Ámbito de aplicación

Se evalúa al construir cada pantalla, cada listado y cada menú de acciones del front, y antes de habilitar cualquier operación: administración de usuarios (CU-02), relevamientos (CU-03), asignaciones (CU-04), marcadores iniciales (CU-05), revisión (CU-06), resolución de conflictos (CU-07), transición y cierre (CU-08), carga manual (CU-09), portabilidad (CU-10) y configuración de almacenamiento (CU-11).

## 4. Consecuencia si se viola

Si el front ofreciera una acción fuera del alcance del rol, el backend la rechazaría con un problema de jerarquía o alcance (JERARQUIA_NO_PERMITIDA, ROL_NO_AUTORIZADO o FUERA_DE_ALCANCE) y el front debe informar el rechazo sin aplicar cambios; ofrecer acciones inválidas se considera un defecto de presentación.

## 5. CU afectados

CU-02, CU-03, CU-04, CU-05, CU-06, CU-07, CU-08, CU-09, CU-10, CU-11.

## 6. Pruebas que la verifican

- Listados y formularios acotados al ámbito del rol; rol no ofrecido para niveles no inmediatos (08, sobre CU-02).
- Pantalla de configuración de almacenamiento no disponible para roles distintos del raíz (08, sobre CU-11).
- Acción sobre un recurso ajeno rechazada y reportada por el front (08, sobre CU-03, CU-04).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de visibilidad y acciones por rol jerárquico en el front web, alineada a RN-01 de geovial-api y derivada de NB-01. |
