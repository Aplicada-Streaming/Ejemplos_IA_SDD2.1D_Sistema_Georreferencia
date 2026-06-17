# 03 UX / UI / DX — geovial-mobile

**Proyecto:** geovial-mobile (mobile-app-maui, app de captura en terreno de la solución GeoVial)
**Variante aplicada:** UX/UI (Mobile UX Designer + Accessibility Specialist)
**Estado de la sección:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Mobile UX Designer + Accessibility Specialist

Punto de entrada navegable de la categoría 03 de `geovial-mobile`. Por ser un proyecto del tipo `mobile-app-maui` con interfaz visible al usuario final (`tiene_ui_final=true`), la sección aplica la variante UX/UI móvil con accesibilidad reforzada (03_rules §1.2). La audiencia única es el agente de campo (00 §2), que usa la app de forma táctil, en terreno, con guantes, bajo sol directo, en movilidad y con conectividad variable. El piso de accesibilidad es WCAG 2.2 nivel AA, reforzado por ese contexto.

## Artefactos vigentes

| Artefacto | Variante | Propósito | Estado |
| --- | --- | --- | --- |
| [experiencia-de-uso_v1.0.md](experiencia-de-uso_v1.0.md) | UX/UI | Marco de experiencia: audiencia y contexto en terreno, principios de diseño (Nielsen y leyes UX para uso táctil), siete flujos clave con captura offline como eje, estados con sin conexión y sincronizando de primera clase, accesibilidad WCAG 2.2 AA reforzada, internacionalización, performance percibida que nunca bloquea la captura, errores y recuperación, y trazabilidad | Propuesto |
| [wireframes-pantalla-login-relogueo_v1.0.md](wireframes-pantalla-login-relogueo_v1.0.md) | UX/UI | Inicio de sesión y relogueo por seguridad del dispositivo; deslogueo completo (CU-01) | Propuesto |
| [wireframes-lista-relevamientos-asignados_v1.0.md](wireframes-lista-relevamientos-asignados_v1.0.md) | UX/UI | Lista de relevamientos asignados servida del almacén local y selección de contexto activo (CU-02) | Propuesto |
| [wireframes-mapa-captura_v1.0.md](wireframes-mapa-captura_v1.0.md) | UX/UI | Mapa de captura: centrar por GPS, crear o mover marcador, capturar foto y abrir carga manual (CU-03, CU-04, CU-07) | Propuesto |
| [wireframes-detalle-observacion_v1.0.md](wireframes-detalle-observacion_v1.0.md) | UX/UI | Detalle de observación: fotos, comentarios, etiquetas y resultado de la carga manual (CU-05, CU-07) | Propuesto |
| [wireframes-estado-sincronizacion_v1.0.md](wireframes-estado-sincronizacion_v1.0.md) | UX/UI | Cola offline y progreso del ciclo subir-luego-bajar con resumen y conflictos (CU-06) | Propuesto |
| [glosario-ux_v1.0.md](glosario-ux_v1.0.md) | UX/UI | Vocabulario UX móvil propio de la sección, sin duplicar el dominio de 02 ni de la visión | Propuesto |

Cinco wireframes en portrait, que cumplen y cubren el mínimo del tipo `mobile-app-maui` (mínimo 5, 03_rules §2.2). Cada wireframe declara su CU origen, sus estados (incluido sin conexión y sincronizando) y su trazabilidad upstream y downstream. Cada uno incluye su nota responsive (sección 6).

## Cobertura de los CU de 02

| CU | Superficie que lo materializa |
| --- | --- |
| CU-01 Iniciar sesión, deslogueo y relogueo | wireframes-pantalla-login-relogueo |
| CU-02 Seleccionar relevamiento asignado | wireframes-lista-relevamientos-asignados |
| CU-03 Centrar por GPS y crear o mover marcador | wireframes-mapa-captura |
| CU-04 Capturar foto con resolución de coordenadas | wireframes-mapa-captura |
| CU-05 Agregar comentarios y etiquetas | wireframes-detalle-observacion |
| CU-06 Trabajar sin conexión y sincronizar | wireframes-estado-sincronizacion |
| CU-07 Cargar fotos manualmente con radio | wireframes-mapa-captura (acción) y wireframes-detalle-observacion (resultado) |

Los siete CU de 02 quedan cubiertos por al menos un wireframe; el marco de experiencia los enlaza a todos en su tabla de trazabilidad (§9).

## Artefactos omitidos y su motivo

| Artefacto | Decisión | Motivo |
| --- | --- | --- |
| `representacion-<concepto>_v1.0.md` | Omitido | No es obligatorio (03_rules §2.1, recomendado solo si hay una representación visual reutilizada que convenga centralizar). Los conceptos transversales (indicador de conectividad y sincronización, marca de pendiente de ubicación) se describen en cada wireframe que los usa y se anclan en el marco §9; no alcanzan la complejidad que justifique un documento focalizado propio en v1. Puede sumarse en una versión posterior si la cobertura lo exige. |
| Artefactos de la variante DX (`dx-*`, `guia-onboarding-developer`) | No aplican | La superficie de `geovial-mobile` es una UI final para el agente de campo, no una API ni un CLI. La variante DX corresponde a los proyectos hermanos `geovial-api`, `geovial-storage` y `aplicada-sync` (03_rules §1.2, §2.1). |

## Trazabilidad de la sección

- Upstream: persona objetivo (agente de campo) de la visión de producto (00 §2); los siete CU y las cinco RN de la especificación funcional (02); el modelo conceptual del almacén local (02) para reflejar la cola offline y los estados; la restricción de plataforma móvil única (compatibilidad-plataformas §2) que fija portrait como orientación primaria.
- Downstream: alimenta 06 con criterios de aceptación visuales y de ergonomía táctil (US-01 a US-15), alimenta 05 con requisitos no funcionales de la capa de presentación (ergonomía táctil, accesibilidad reforzada, performance percibida) y alimenta 08 con escenarios de snapshot test, de captura offline y de test de accesibilidad WCAG 2.2 AA.

## Estructura de la sección

```text
03_ux_ui_dx/
├── README.md                                       # este archivo
├── experiencia-de-uso_v1.0.md                      # marco de experiencia (11 secciones)
├── wireframes-pantalla-login-relogueo_v1.0.md      # CU-01
├── wireframes-lista-relevamientos-asignados_v1.0.md# CU-02
├── wireframes-mapa-captura_v1.0.md                 # CU-03, CU-04, CU-07
├── wireframes-detalle-observacion_v1.0.md          # CU-05, CU-07
├── wireframes-estado-sincronizacion_v1.0.md        # CU-06
└── glosario-ux_v1.0.md                             # vocabulario UX móvil
```
