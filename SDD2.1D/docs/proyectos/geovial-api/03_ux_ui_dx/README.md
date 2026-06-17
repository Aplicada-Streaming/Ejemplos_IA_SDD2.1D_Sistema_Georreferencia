# 03 UX / UI / DX — geovial-api

**Proyecto:** geovial-api (rest-api, principal de la solución GeoVial)
**Variante aplicada:** DX (API DX Designer + Developer Advocate)
**Estado de la sección:** Propuesto
**Fecha:** 2026-06-15
**Autor:** API DX Designer + Developer Advocate

Punto de entrada navegable de la categoría 03 de `geovial-api`. Por ser un proyecto del tipo `rest-api` cuya superficie pública es el contrato REST y no una pantalla de usuario final, la sección aplica la variante DX (03_rules §1.2). El consumidor de la API es un developer integrador interno: los equipos de los proyectos hermanos `geovial-web` (front) y `geovial-mobile` (app), que dependen del contrato REST del backend (intake §14).

## Artefactos vigentes

| Artefacto | Variante | Propósito | Estado |
| --- | --- | --- | --- |
| [dx-developer-experience_v1.0.md](dx-developer-experience_v1.0.md) | DX | Marco DX: audiencia integradora interna, onboarding por tramos 5/30/60 verificables, quick-start de autenticación y lectura, plan Diátaxis, errores problem+json, métricas DX y feedback loop entre proyectos hermanos | Propuesto |
| [guia-onboarding-developer_v1.0.md](guia-onboarding-developer_v1.0.md) | DX | Recorrido de primera hora del integrador: obtener token por el flujo de autenticación por credenciales, primer request autenticado, diagnosticar un error y paginar un listado; modo tutorial de Diátaxis | Propuesto |
| [dx-error-messages_v1.0.md](dx-error-messages_v1.0.md) | DX | Catálogo de errores accionable alineado a los códigos de los CU, agrupado por taxonomía, con causa y acción por código, sobre formato problem+json | Propuesto |

## Artefactos omitidos y su motivo

| Artefacto | Decisión | Motivo |
| --- | --- | --- |
| `dx-portal-developers_v1.0.md` | Omitido | El intake declara `tiene_portal_developers=false` para `geovial-api`: no hay portal público de developers visible. La superficie es de consumo interno entre proyectos hermanos. La tabla maestra de artefactos (03_rules §2.1) marca este documento como obligatorio solo para `rest-api con portal visible`; sin portal visible, no aplica. La referencia formal de la API (catálogo de endpoints y reference) se produce en la categoría 10 (developer guide), no aquí. |

Los artefactos de la variante UX/UI (`experiencia-de-uso`, `wireframes-*`, `representacion-*`, `glosario-ux`) no aplican: `geovial-api` no tiene UI final propia (03_rules §2.1). La experiencia del usuario final se diseña en las secciones 03 de `geovial-web` y `geovial-mobile`. No se produce `glosario-ux` propio: el vocabulario del dominio ya está canonizado en el glosario de la visión (00 §9) y los términos REST usados (endpoint, método, código de estado, token bearer, problem+json, paginación, clave de idempotencia) son del tipo de proyecto, no propios de la sección.

## Trazabilidad de la sección

- Upstream: audiencia integradora interna definida en la visión de producto (00 §2) y en el intake (§14, §17.P.3, §17.P.5); superficie pública derivada de los 22 CU de la especificación funcional (02), recursos de CU-01 a CU-17 y contratos transversales CU-18 a CU-22.
- Downstream: alimenta 06 con criterios de ergonomía de API para las US US-05, US-06 y US-37 a US-44; alimenta 08 con escenarios de test de contrato y de onboarding verificable; el modo reference formal de la API se consolida en 10.

## Estructura de la sección

```text
03_ux_ui_dx/
├── README.md                            # este archivo
├── dx-developer-experience_v1.0.md      # marco DX
├── guia-onboarding-developer_v1.0.md    # primera hora del integrador
└── dx-error-messages_v1.0.md            # catálogo de errores accionable
```
