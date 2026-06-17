# 06 Backlog técnico — geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** README.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Scrum Master + Mobile Lead

Punto de entrada navegable del backlog de la app de campo offline-first `geovial-mobile` (tipo `mobile-app-maui`, no redistribuible). El backlog organiza el trabajo por capacidad móvil (sesión y relogueo por seguridad del dispositivo, selección de relevamiento, mapa y captura georreferenciada, observación con comentarios y etiquetas, trabajo offline y sincronización, carga manual) y traza cada historia a un CU de 02 y cada tarea técnica a un NB, CU, RN, ADR, componente, modelo lógico o contrato consumido de 05. US y BT viven inline por estar por debajo de los umbrales de 20 US y 30 BT (§3.3 de las reglas).

## Documentos de la sección

- [product-backlog_v1.0.md](product-backlog_v1.0.md) — objetivos y MVP, épicas EP-XX, 15 US inline con MoSCoW, SP Fibonacci, criterios Given/When/Then, métricas de avance y refinement.
- [backlog-tecnico_v1.0.md](backlog-tecnico_v1.0.md) — épicas técnicas ET-XX, 13 BT inline con tipo, prioridad, fuente upstream y dependencias, matriz BT↔US↔CU y cobertura por CU.
- [definition-of-ready_v1.0.md](definition-of-ready_v1.0.md) — criterios DoR para US (7) y para BT (5), excepciones y aprobador.

## Épicas vigentes

| EP | Nombre | Capacidad móvil | US |
| --- | --- | --- | --- |
| EP-01 | Sesión y relogueo por seguridad del dispositivo | Inicio en línea, relogueo por seguridad del dispositivo y deslogueo completo | US-01, US-02 |
| EP-02 | Selección de relevamiento asignado | Lista local de asignaciones y contexto activo de captura | US-03, US-04 |
| EP-03 | Mapa y captura georreferenciada | Centrado por ubicación, marcadores y foto con resolución de coordenada | US-05, US-06, US-07, US-08 |
| EP-04 | Observación con comentarios y etiquetas | Nota, comentario por foto y etiquetas reutilizables | US-09, US-10 |
| EP-05 | Trabajo offline y sincronización | Cola local y ciclo subir-luego-bajar con reanudación y conflictos | US-11, US-12, US-13 |
| EP-06 | Carga manual de fotos | Carga con priorización de ubicación incrustada y radio de agrupación | US-14, US-15 |

## Épicas técnicas vigentes

| ET | Nombre | Capacidad técnica | BT |
| --- | --- | --- | --- |
| ET-01 | Almacén local, esquema y migraciones | Almacén local + migraciones versionadas | BT-01, BT-02 |
| ET-02 | Cola de cambios y trabajo offline | Cola de cambios persistente y ordenada | BT-03, BT-04 |
| ET-03 | Mapa, ubicación y captura georreferenciada | Mapa local + captura de foto y resolución de coordenadas | BT-05, BT-06 |
| ET-04 | Permisos del sistema operativo y degradación | Permisos del sistema operativo | BT-07, BT-08 |
| ET-05 | Sesión, almacenamiento seguro del token y ciclo de vida | Token seguro + relogueo + ciclo de vida de la app | BT-09, BT-10 |
| ET-06 | Sincronización por consumo del motor y conflictos | Motor de sincronización por consumo, conectividad y conflictos en cliente | BT-11, BT-12, BT-13 |

## US Must Have del MVP

| US | Título | SP | CU |
| --- | --- | --- | --- |
| US-01 | Iniciar sesión en línea con credenciales | 5 | CU-01 |
| US-02 | Reloguear por seguridad del dispositivo y deslogueo completo | 5 | CU-01 |
| US-03 | Ver y seleccionar un relevamiento asignado | 3 | CU-02 |
| US-05 | Centrar por ubicación y crear un marcador | 5 | CU-03 |
| US-07 | Capturar una foto con resolución de coordenada | 5 | CU-04 |
| US-09 | Agregar nota, comentario y etiquetas a la observación | 3 | CU-05 |
| US-11 | Sincronizar subiendo antes de bajar | 8 | CU-06 |
| US-12 | Reanudar una sincronización interrumpida sin duplicar | 5 | CU-06 |
| US-14 | Cargar fotos manualmente con radio de agrupación | 5 | CU-07 |

El MVP (44 SP en 9 US Must) habilita el lado de campo completo de NB-01, NB-03 y NB-04: identidad y sesión segura en dispositivo compartido, captura georreferenciada offline de marcadores y fotos con su descripción, sincronización subir-antes-de-bajar con reanudación y carga manual con radio de agrupación.

## BT prioritarias (prioridad Alta)

| BT | Título | Épica técnica | Fuente |
| --- | --- | --- | --- |
| BT-01 | Esquema del almacén local con índices y restricciones de réplica | ET-01 | ADR-02; modelo lógico |
| BT-02 | Migraciones versionadas en el arranque | ET-01 | ADR-02; modelo lógico §4 |
| BT-03 | Cola de cambios con orden e identificador de origen | ET-02 | ADR-02; RN-05, RN-02 |
| BT-05 | Componente de mapa local y marcadores | ET-03 | ADR-01; RN-03, RN-05 |
| BT-06 | Captura de foto y resolución de coordenada | ET-03 | RN-01, RN-05 |
| BT-07 | Permisos centralizados en adaptadores | ET-04 | ADR-04 |
| BT-09 | Token en almacenamiento seguro y contrato de sesión | ET-05 | ADR-05; NB-01 |
| BT-10 | Tres modos de sesión y relogueo por seguridad del dispositivo | ET-05 | ADR-05; RN-04 |
| BT-11 | Adaptador de la librería de sincronización | ET-06 | ADR-03; contrato consumido |
| BT-12 | Ciclo subir-luego-bajar con conectividad y reanudación | ET-06 | ADR-03; RN-02, RN-05 |

## DoR vigente

DoR v1.0: 7 criterios para US y 5 para BT, con excepciones de US Could (US-10, US-15), spike (disponible, sin spikes en v1.0), dependencia de contrato consumido y supuesto abierto de 02 §9. Aprobador titular: Scrum Master + Mobile Lead, con revisiones acotadas de AG-02 (trazabilidad a CU), AG-05 (justificación en 05) y AG-08 (verificabilidad para 08). Detalle en [definition-of-ready_v1.0.md](definition-of-ready_v1.0.md).

## Convenciones aplicadas

- Identificadores de dos dígitos uniformes: US-01 a US-15, BT-01 a BT-13, EP-01 a EP-06, ET-01 a ET-06. Sin rastros del patrón heredado `BT-001`.
- Estimación Fibonacci (1, 2, 3, 5, 8, 13) declarada y mantenida en todo el backlog; los spikes, si se desglosan, llevan caja temporal.
- MoSCoW con reparto realista sobre 15 US: 9 Must (60 %), 4 Should (27 %), 2 Could (13 %), 0 Won't.
- Modo inline en los dos artefactos: 15 US < 20 y 13 BT < 30 (§3.3 de las reglas); no se generan carpetas `historias-usuario/` ni `tareas-tecnicas/`.
- Vocabulario abstracto de plataforma móvil (app móvil, almacén local, almacenamiento seguro del dispositivo, componente de mapa); sin stacks, productos ni protocolos concretos (viven en el intake §17).

## Trazabilidad upstream/downstream

- Upstream: NB-01, NB-03, NB-04 (01); especificación funcional, CU-01 a CU-07 y RN-01 a RN-05 (02); arquitectura de solución, ADR-01 a ADR-05, modelo lógico del almacén local y flujo de ejecución (05); contratos consumidos de la librería de sincronización (`aplicada-sync`) y del backend (`geovial-api`).
- Downstream: 07 (sprint plan, asignación a sprint y velocity), 08 (acceptance tests desde los escenarios Given/When/Then y pruebas de modo offline, migración, capacidad de cola, reanudación y convivencia con conflictos).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 06 de geovial-mobile: índice de los tres artefactos, épicas EP-XX y técnicas ET-XX vigentes, US Must Have del MVP, BT prioritarias y DoR vigente. Modo inline por debajo de los umbrales de 20 US y 30 BT. |
