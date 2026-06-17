# 07 Plan de sprint — geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** README.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-16
**Autor:** Scrum Master + Mobile Release Manager
**Tipo (D8):** mobile-app-maui
**Modo:** mini-plan (equipo_n = 1)

Punto de entrada navegable de la planificación de ejecución de la app de campo offline-first `geovial-mobile`. Por tratarse de un proyecto de un solo desarrollador, la sección opera en modo mini-plan (§2.1/§2.2 de 07_rules_plan_sprint.md): un único plan condensado sustituye a los planes de iteración por sprint, las plantillas de review y retrospectiva y el tracking de velocidad. No existen `plan-iteracion-sprint-XX_v1.0.md`, `template-sprint-review_v1.0.md`, `template-sprint-retrospectiva_v1.0.md` ni `velocidad-equipo_v1.0.md`.

## Documentos de la sección

- [mini-plan_v1.0.md](mini-plan_v1.0.md) — plan único condensado: objetivo orientado a valor, ítems comprometidos por tramos (13 BT y 15 US de 06), DoD por referencia a la canónica de 08, trazabilidad a CU y NB por tramo, riesgos con mitigación y bitácora de avance.

## Tramos del mini-plan

| Tramo | Foco | Ítems | SP |
| --- | --- | --- | --- |
| 1 | Esqueleto de sesión y almacén local | BT-01, BT-02, BT-09, BT-10; US-01, US-02 | 28 |
| 2 | Captura georreferenciada | BT-03, BT-04, BT-05, BT-06, BT-07, BT-08; US-03, US-04, US-05, US-06, US-07, US-08, US-09, US-10, US-14, US-15 | 63 |
| 3 | Sincronización | BT-11, BT-12, BT-13; US-11, US-12, US-13 | 32 |
| Total | — | 13 BT (62 SP) + 15 US (61 SP) | 123 |

## Modo mini-plan (equipo_n = 1)

- Sustituye a los cuatro artefactos de equipo multi-dev; un único `mini-plan_v1.0.md` cubre objetivo, compromiso por tramos y bitácora de avance.
- Sin velocity ni capacidad efectiva por sprint: sin línea de base de cadencia de un equipo de un solo dev, el compromiso se gestiona por tramos secuenciados por dependencias y por los criterios de transición de la fase F2 del roadmap, no por tope de puntos.
- Cadencia atada al ciclo de distribución del paquete de la app (tipo `mobile-app-maui`, 07_rules §3.2): los cierres de tramo dependen de la verificación sobre el paquete por el canal interno.

## Fase de roadmap

geovial-mobile entra en la fase F2 — Captura en campo y sincronización (roadmap-producto_v1.0.md §2). Al cierre de los tres tramos se cubren los criterios de transición F1→F2 del roadmap §5 (captura offline con foto y coordenadas, carga manual con radio de agrupación y sincronización subir-luego-bajar sobre el relevamiento asignado).

## Trazabilidad upstream/downstream

- Upstream: 06 (product-backlog EP-01..06 y US-01..15; backlog-tecnico ET-01..06 y BT-01..13; definition-of-ready); 05 (ADR-01 a ADR-05); 02 (CU-01 a CU-07, RN-01 a RN-05); 01 (NB-01, NB-03, NB-04); roadmap-producto (fase F2).
- Downstream: 08 (acceptance tests y DoD canónica que el mini-plan referencia; pendiente de publicación); 09 (DevOps si el ciclo de distribución del paquete introduce cambios de pipeline).

## DoD canónica

El mini-plan referencia la Definition of Done canónica del proyecto, que vive en 08 y aún no está publicada (pendiente de 08). La Definition of Ready de 06 (definition-of-ready_v1.0.md) gobierna la entrada de cada ítem al tramo.

## Convenciones aplicadas

- `mini-plan_v1.0.md` con un único separador `_v` antes de la versión; sin doble separador.
- H1 directo seguido del bloque de metadatos; ningún archivo abre con `--` ni separador previo al H1.
- Cada ítem referencia el identificador exacto del backlog de 06 (US-XX, BT-XX); sin invención de identificadores.
- Estimación en story points Fibonacci (1, 2, 3, 5, 8, 13), coherente con 06.
- Vocabulario abstracto de plataforma móvil (app móvil, almacén local, almacenamiento seguro del dispositivo, paquete de la app, canal de distribución interno); sin stacks, productos ni protocolos concretos.
- UTF-8, LF, fechas YYYY-MM-DD, sin emojis.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-16 | README inicial de la sección 07 de geovial-mobile en modo mini-plan (equipo_n=1): índice del mini-plan único, tramos de compromiso, modo simplificado sin velocity, fase F2 del roadmap, trazabilidad upstream/downstream y convenciones aplicadas. |
