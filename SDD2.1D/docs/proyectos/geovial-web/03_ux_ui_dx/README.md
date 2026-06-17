# 03 UX / UI / DX — geovial-web

**Proyecto:** geovial-web (web-monolith, front web de la solución GeoVial)
**Variante aplicada:** UX/UI (UX/UI Designer + Frontend Lead)
**Estado de la sección:** Propuesto
**Fecha:** 2026-06-15
**Autor:** UX/UI Designer + Frontend Lead

Punto de entrada navegable de la sección de experiencia del front web `geovial-web`. La sección define cómo se siente el front de los roles administradores (raíz, jefe general, jefe de área, más la excepción de carga manual del agente): marco de experiencia, wireframes de las superficies clave, la representación del carrusel y el vocabulario UX. No define el qué funcional (02) ni el cómo técnico de la capa de presentación (05).

## Variante y tipo

Tipo de proyecto `web-monolith` con UI final, por lo que se aplica la variante UX/UI (03_rules §1.2). El piso de wireframes del tipo es cuatro superficies clave (login, home, flujo principal, error); esta sección cubre las cuatro. WCAG 2.2 nivel AA es el piso de accesibilidad declarado en todos los artefactos.

## Artefactos vigentes

| Artefacto | Variante | Propósito | Estado |
| --- | --- | --- | --- |
| [experiencia-de-uso_v1.0.md](experiencia-de-uso_v1.0.md) | UX/UI | Marco de experiencia: audiencia, principios, flujos, estados, accesibilidad, i18n, performance percibida, errores y trazabilidad | Propuesto |
| [wireframes-pantalla-login_v1.0.md](wireframes-pantalla-login_v1.0.md) | UX/UI | Superficie de ingreso (login) — CU-01 | Propuesto |
| [wireframes-panel-relevamientos_v1.0.md](wireframes-panel-relevamientos_v1.0.md) | UX/UI | Superficie inicial / listado (home) — CU-03 | Propuesto |
| [wireframes-revision-mapa-carrusel_v1.0.md](wireframes-revision-mapa-carrusel_v1.0.md) | UX/UI | Flujo principal: mapa con marcadores y carrusel de fotos — CU-06 | Propuesto |
| [wireframes-resolucion-conflictos-cierre_v1.0.md](wireframes-resolucion-conflictos-cierre_v1.0.md) | UX/UI | Resolución de conflictos y cierre; cubre el estado de error de avance de ciclo — CU-07, CU-08 | Propuesto |
| [representacion-carrusel-fotos_v1.0.md](representacion-carrusel-fotos_v1.0.md) | UX/UI | Representación reutilizable del carrusel encadenado | Propuesto |
| [glosario-ux_v1.0.md](glosario-ux_v1.0.md) | UX/UI | Vocabulario UX de la sección, sin duplicar el glosario de 02 | Propuesto |

## Cobertura de superficies del tipo web-monolith

| Superficie del piso | Artefacto | CU origen |
| --- | --- | --- |
| Login | wireframes-pantalla-login_v1.0.md | CU-01 |
| Home | wireframes-panel-relevamientos_v1.0.md | CU-03 |
| Flujo principal | wireframes-revision-mapa-carrusel_v1.0.md | CU-06 |
| Error / avance de ciclo bloqueado | wireframes-resolucion-conflictos-cierre_v1.0.md | CU-07, CU-08 |

Las superficies no priorizadas como wireframe mínimo (administración de usuarios CU-02, asignación CU-04, marcadores iniciales CU-05, carga manual CU-09, portabilidad CU-10, configuración CU-11) quedan cubiertas por el marco de experiencia y sus CU; pueden recibir wireframe propio en una versión posterior (el mínimo del tipo es piso, no techo).

## Trazabilidad de la sección

- Upstream: persona objetivo de 00 (visión §2), once CU de 02 con interacción humana (CU-01 a CU-11) y cinco RN de presentación (RN-01 a RN-05).
- Downstream: 05 (requisitos no funcionales de la capa de presentación), 06 (US con criterios de aceptación visuales, US-01 a US-25 según la trazabilidad del marco), 08 (tests de UI, snapshot y accesibilidad).
- La matriz completa flujo → persona → CU → RN → wireframe → US → tests está en la §9 de `experiencia-de-uso_v1.0.md`.

## Convenciones

- Nomenclatura `<nombre>_v1.0.md` con guion bajo antes de `v`; slug en minúsculas kebab-case. UTF-8 LF, fechas YYYY-MM-DD, sin emojis.
- Los wireframes describen layout, jerarquía, comportamiento y estados; no incluyen colores, tipografías ni CSS, ni stacks concretos. Los componentes se nombran por su rol (componente de mapa, tabla de datos, formulario, modal, carrusel).
- Política de versionado (03_rules §3.5): una sola versión vigente por nombre lógico; las superadas irían a `_legacy/`. Hoy no hay versiones superadas.

## Estructura de la sección

```text
03_ux_ui_dx/
├── README.md                                       # este archivo
├── experiencia-de-uso_v1.0.md                      # marco de experiencia (11 secciones)
├── wireframes-pantalla-login_v1.0.md               # login (CU-01)
├── wireframes-panel-relevamientos_v1.0.md          # home / listado (CU-03)
├── wireframes-revision-mapa-carrusel_v1.0.md       # flujo principal (CU-06)
├── wireframes-resolucion-conflictos-cierre_v1.0.md # conflictos y cierre (CU-07, CU-08)
├── representacion-carrusel-fotos_v1.0.md           # carrusel reutilizable
└── glosario-ux_v1.0.md                             # vocabulario UX
```
