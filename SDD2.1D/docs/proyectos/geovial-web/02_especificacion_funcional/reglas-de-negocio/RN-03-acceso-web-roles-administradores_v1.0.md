# RN-03 — Acceso al front web restringido a roles administradores

**Proyecto:** geovial-web
**Documento:** RN-03-acceso-web-roles-administradores_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional

## 1. Enunciado de la regla

El front web es la herramienta de los roles administradores —usuario raíz, jefe general y jefe de área— y admite el ingreso de un agente de campo únicamente para la carga manual de un relevamiento al que está asignado; ningún otro flujo del agente está disponible en el front web, ya que su operación de campo ocurre en la aplicación de campo.

## 2. Justificación

La separación de herramientas refleja la realidad del negocio: los administradores planifican, revisan y cierran desde el entorno web, mientras la captura en terreno es offline-first en la aplicación de campo; abrir todos los flujos del agente en el front diluiría esa separación. Deriva del rol del proyecto en la solución (intake §13, §17 geovial-web) y de NB-01.

## 3. Ámbito de aplicación

Se evalúa al iniciar sesión en el front (CU-01), al resolver qué pantallas se habilitan según el rol recibido y al permitir el acceso de un agente solo a la carga manual (CU-09).

## 4. Consecuencia si se viola

Si un rol sin acceso intentara ingresar al front fuera de la excepción de carga manual, el front cierra la sesión recién abierta e informa que ese rol no tiene acceso a esta herramienta (ROL_SIN_ACCESO_WEB); no se habilita ninguna pantalla de administración para el agente.

## 5. CU afectados

CU-01, CU-09.

## 6. Pruebas que la verifican

- Ingreso de un jefe de área, un jefe general y el usuario raíz habilitado; ingreso de un agente fuera de la carga manual rechazado (08, sobre CU-01).
- Agente asignado que ingresa solo a la carga manual de su relevamiento (08, sobre CU-09).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de acceso al front web restringido a roles administradores, con la excepción de carga manual del agente, derivada del rol del proyecto y de NB-01. |
