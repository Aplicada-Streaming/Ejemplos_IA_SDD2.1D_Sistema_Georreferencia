# RN-01 — Jerarquía de altas, bajas y alcance

**Proyecto:** geovial-api
**Documento:** RN-01-jerarquia-altas-bajas_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Enunciado de la regla

Cada nivel de la jerarquía de usuarios administra exclusivamente al nivel inmediato inferior y solo opera los recursos que caen dentro de su ámbito: el usuario raíz administra al jefe general, el jefe general administra a los jefes de área, el jefe de área administra a los agentes de campo, y el agente de campo no administra a ningún usuario.

## 2. Justificación

La organización necesita delegar la administración del personal hacia abajo sin un administrador central único y acotar lo que cada rol ve y hace, para que toda acción tenga un responsable identificable y no se mezclen tramos de distintas áreas (NB-01, intake §2).

## 3. Ámbito de aplicación

Se evalúa en toda alta o baja de usuarios, en toda asignación de relevamientos y agentes, y en cada solicitud autorizada a un recurso del backend. Se aplica antes de ejecutar la acción, como parte del control de autorización transversal.

## 4. Consecuencia si se viola

El backend rechaza la operación con un estado de prohibido y un problema con código JERARQUIA_NO_PERMITIDA, ROL_NO_AUTORIZADO, FUERA_DE_ALCANCE o equivalente, según el recurso, y no crea ni modifica ningún dato.

## 5. CU afectados

CU-01, CU-02, CU-04, CU-05, CU-06, CU-07, CU-12, CU-13, CU-14, CU-15, CU-16, CU-17, CU-18, CU-20.

## 6. Pruebas que la verifican

- Alta del nivel inmediato inferior aceptada y de un nivel no inmediato rechazada (08, sobre CU-01, CU-02).
- Acceso a recursos fuera del ámbito del solicitante rechazado (08, sobre CU-18).
- Listados acotados al alcance del solicitante antes de paginar (08, sobre CU-20).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de jerarquía de altas, bajas y alcance, derivada de NB-01. |
