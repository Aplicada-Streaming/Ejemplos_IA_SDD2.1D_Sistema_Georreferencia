# Auditoría Fase F (DevOps + Developer guide) — Nivel 2 de GeoVial

**Fase:** F (09_devops + omisión de 10_developer_guide)
**Proyectos:** geovial-web (`web-monolith`), geovial-mobile (`mobile-app-maui`)
**Alcance:** 12 documentos de 09_devops (6 por proyecto) + 2 ADR-06 de omisión de 10 en 05 + índices `decisiones-arquitectura_v1.0.md` de cada proyecto. Categoría 10 omitida en ambos (opcional por tipo D8, audiencia interna).
**Auditor:** Arquitecto de Soluciones + QA Senior (independiente, sin participación en la generación)
**Fecha:** 2026-06-16
**Documento:** F-devops-devguide-nivel2_v1.0.md
**Versión:** 1.0

---

## 1. Resumen ejecutivo

Se auditaron los dos proyectos de nivel 2 de GeoVial. Ambas secciones 09 producen los seis documentos requeridos por la regla 09 §6, con el modelo de ambientes/canales correcto por tipo D8: geovial-web usa ambientes de servicio DEV/QA/STAGING/PROD con artefacto `image-docker`, y geovial-mobile usa canales de distribución móvil internal/alpha/beta/production con artefacto `store-mobile`, sin confundir canales con ambientes. El scan léxico del cuerpo de los 12 docs y de los 2 ADR-06 no encontró fugas del stack prohibido fuera de los usos permitidos por contexto. La omisión de la categoría 10 está correctamente registrada como ADR-06 de omisión en 05 de cada proyecto, con las 10 secciones de §4.3, estado Aceptado, la triple justificación (tipo D8 opcional + audiencia interna + tiene_portal_developers=false) y archivo individual no consolidado (§3.3); ambos índices la incluyen. Los gates del pipeline ejecutan la DoD de 08 sin redefinirla, cada NFR numérico de §17.P.10 tiene un stage que lo verifica antes de promover, y el rollback está documentado por tipo de artefacto.

**Conteo de hallazgos:** P0: 0 | P1: 0 | P2: 1 | P3: 3

**Veredicto consolidado:** APROBADO CON OBSERVACIONES (sin P0; permite avanzar).

---

## 2. Matriz D1-D8 por documento (con fila de scan de stack)

Leyenda: OK = conforme; n/a = no aplica.

### 2.1 geovial-web — 09_devops

| Documento | D1 idioma/tildes | D2 tablas completas | D3 kebab/ASCII | D4 sufijo `_v1.0` | D5 control de cambios | D6 trazabilidad | D7 sin vocabulario fuente | D8 set cerrado |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| pipeline-ci-cd_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| estrategia-versionado_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| entornos-deploy_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| guia-publicacion-image-docker_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| supply-chain-seguridad_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| README.md | OK | OK | OK | n/a | OK | OK | OK | OK |

### 2.2 geovial-mobile — 09_devops

| Documento | D1 idioma/tildes | D2 tablas completas | D3 kebab/ASCII | D4 sufijo `_v1.0` | D5 control de cambios | D6 trazabilidad | D7 sin vocabulario fuente | D8 set cerrado |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| pipeline-ci-cd_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| estrategia-versionado_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| entornos-deploy_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| guia-publicacion-store-mobile_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| supply-chain-seguridad_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| README.md | OK | OK | OK | OK | OK | OK | OK | OK |

### 2.3 ADR-06 de omisión de 10 (en 05)

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| geovial-web/05/adrs/ADR-06-omision-developer-guide_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| geovial-mobile/05/adrs/ADR-06-omision-developer-guide_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |

### 2.4 Fila de scan léxico de stack prohibido (D7)

Términos buscados en el CUERPO de los 12 docs de 09 y de los 2 ADR-06: `.NET`, `ASP.NET`, `Blazor`, `MudBlazor`, `SignalR`, `MAUI`, `SQLite`, `SQL Server`, `JWT`, `ROPC`, `OAuth`, `Leaflet`, `Docker`, `Kubernetes`.

| Proyecto | Término detectado | Ubicación | ¿Permitido por contexto? | Veredicto |
| --- | --- | --- | --- | --- |
| geovial-web | `image-docker` | Solo en el nombre de archivo `guia-publicacion-image-docker_v1.0.md` y referencias a ese archivo | Sí: el nombre de guía / valor de tipo-artefacto es admitido (regla 09 §3.1, §2.2) | Sin fuga |
| geovial-web | `.NET`, `Blazor`, `SignalR`, `Docker`, `Leaflet`, `JWT`, `ROPC`, `OAuth`, `SQL Server`, `MudBlazor` | No aparecen en el cuerpo | El cuerpo usa abstracciones permitidas: "imagen de contenedor", "render server-side", "circuito interactivo", "orquestador de contenedores", "registro de imágenes", "token bearer" | Sin fuga |
| geovial-mobile | `Android` / `net8.0-android` / `.NET 8` | matriz de plataforma (pipeline §4, guia §2.1/§2.2), declaración de runtime/target | Sí: plataforma/runtime declarado, permitido y usado con mesura (3 menciones de `.NET 8`/`net8.0-android`) | Sin fuga |
| geovial-mobile | `store-mobile`, `aab-android` | Nombre de archivo `guia-publicacion-store-mobile_v1.0.md` y tabla de tipo-artefacto (regla 09 §3.1) | Sí: valores de tipo-artefacto / nombre de guía, usados con mesura | Sin fuga |
| geovial-mobile | `mobile-app-maui` | Referencias a la regla (`regla 09 §2.2 para mobile-app-maui`) | Sí: es el valor D8 del project_type (set cerrado D8), no el stack "MAUI" | Sin fuga |
| geovial-mobile | `MAUI`, `SQLite`, `Blazor`, `MudBlazor`, `JWT` (standalone) | No aparecen en el cuerpo | Abstracciones: "almacén local", "paquete de aplicación", "token bearer", "librería de sincronización" | Sin fuga |
| Ambos ADR-06 | ninguno | — | Cuerpo abstracto ("contrato del backend", "contrato de sincronización", "kit de desarrollo", "portal de developers") | Sin fuga |

Nota sobre `GitVersion`: aparece en ambas estrategias de versionado como herramienta de auto-versioning. No está en la lista de stack prohibido, está nombrado explícitamente en el intake §17.P.7 de ambos proyectos y la regla 09 §4.3 lo cita como ejemplo de herramienta admitida. No constituye fuga.

**Resultado del scan: sin fugas de stack en el cuerpo de ninguno de los 14 documentos.**

---

## 3. Matriz de estructura obligatoria (regla 09 §6)

### 3.1 geovial-web (web-monolith)

| Requisito §6 | Documento / sección | Cumple |
| --- | --- | --- |
| `pipeline-ci-cd` con stages obligatorios (lint, build, test, SCA, SBOM, firma, publish), matriz SO/runtime, caché, artefactos, promotion, rollback, notificaciones | pipeline-ci-cd §1-§6 (16 stages, matriz §2, caché §3, promotion §4, rollback §5, notificaciones §6) | Sí |
| `estrategia-versionado` con SemVer 2.0.0, Conventional Commits, herramienta declarada, branching alineado, canales, deprecation | estrategia-versionado §1-§6 (SemVer, CC, GitVersion, trunk-based, prerelease, deprecation) | Sí |
| `entornos-deploy` con modelo correcto: DEV/QA/STAGING/PROD para web-monolith (modelo de SERVICIO) | entornos-deploy §1 (cuatro ambientes de servicio) | Sí |
| Afinidad de sesión del circuito persistente tratada | entornos-deploy §6 (afinidad obligatoria con ≥ 2 réplicas, drenaje, verificación del NFR de concurrencia bajo afinidad) | Sí |
| `guia-publicacion-<tipo-artefacto>` con pre-requisitos, comando/stage, verificación post-publish, rollback, métricas | guia-publicacion-image-docker §1-§5 | Sí |
| `supply-chain-seguridad` con SBOM, firma, SLSA, dependency scanning, SAST/DAST, política CVE | supply-chain §1-§6 (SBOM, firma, SLSA L2, SCA, SAST/DAST, CVE) | Sí |
| README de la sección | README (índice, orden de lectura, mapeo de gates) | Sí |
| Patrón `guia-publicacion-<tipo-artefacto>_v1.0.md` sin gestor hardcodeado | `image-docker` es valor admitido | Sí |
| Gates ejecutan la DoD de 08 sin redefinirla | pipeline §0/§1 y README "Gates ejecutados" mapean gates de 08 §3 a stages | Sí |
| Cada NFR numérico con stage que lo verifica | STAGE-08 (custodia token), STAGE-09 (p95 ≤ 200 ms), STAGE-10 (≥ 50 circuitos) | Sí |
| Rollback documentado por tipo de artefacto con comando | pipeline §5 y guia §4 (redepliegue de imagen previa, 7 pasos) | Sí |
| Cabecera obligatoria (§4.1) en cada doc | Las 6 cabeceras presentes | Sí |

### 3.2 geovial-mobile (mobile-app-maui)

| Requisito §6 | Documento / sección | Cumple |
| --- | --- | --- |
| `pipeline-ci-cd` con stages obligatorios, triggers, matriz, caché, promotion, rollback, notificaciones | pipeline-ci-cd §2-§8 (triggers §2, stages §3, matriz §4, caché §5, promotion §6, rollback §7, notificaciones §8) | Sí |
| `estrategia-versionado` con SemVer, CC, herramienta, branching, canales, deprecation | estrategia-versionado §2-§6 (SemVer, CC, GitVersion, trunk-based, canales, deprecation) | Sí |
| `entornos-deploy` con modelo correcto: CANALES internal/alpha/beta/production (NO DEV/QA/STAGING/PROD) | entornos-deploy §1 (cuatro canales de distribución móvil) | Sí |
| NO confunde canales con ambientes | entornos-deploy §1 nombra el anti-patrón 09 §4.8 "confundir publicación con despliegue" y mantiene la distinción; sin mención de DEV/QA/STAGING/PROD | Sí |
| `guia-publicacion-store-mobile` (distribución por canal interno en v1) | guia-publicacion-store-mobile §1-§6 (canal interno, sin tienda pública en v1, ruta futura documentada) | Sí |
| `supply-chain-seguridad` con SBOM, firma, SLSA, dependency scanning, SAST/DAST, CVE | supply-chain §1-§6 (DAST justificadamente no aplica: app cliente sin superficie de red propia) | Sí |
| README de la sección | README (índice, orden de lectura, mapeo de gates, modelo de canales) | Sí |
| Patrón de nombre de guía sin gestor hardcodeado | `store-mobile` es valor admitido (§3.1) | Sí |
| Gates ejecutan la DoD de 08 sin redefinirla | pipeline §1/§3 y README mapean gates de 08 §3 a stages | Sí |
| Cada NFR numérico con stage que lo verifica | Stage "NFR de campo" (captura offline, cola ≥ 1000, ciclo ≤ 30 s, reanudación, arranque ≤ 3 s) | Sí |
| Rollback documentado por tipo de artefacto con comando | pipeline §7 y guia §4 (redistribución de la versión previa por canal interno) | Sí |
| Cabecera obligatoria (§4.1) en cada doc | Las 6 cabeceras presentes | Sí |

---

## 4. Registro de omisión de 10 (regla 10 §6 / master-prompt §6)

| Verificación | geovial-web | geovial-mobile |
| --- | --- | --- |
| 10 es opcional para el tipo D8 (10_rules §1.2/§2.2) | Sí (web-monolith opcional) | Sí (mobile-app-maui opcional) |
| Omisión registrada como ADR de omisión en 05 | Sí: ADR-06 en 05/adrs | Sí: ADR-06 en 05/adrs |
| ADR-06 con las 10 secciones de §4.3 | Sí (Contexto, Decisión, Estado, Alternativas, Consec. positivas, Consec. negativas, Implementación, Métricas, Referencias, Control de cambios) | Sí (las 10 secciones) |
| Estado Aceptado | Sí (Aceptado 2026-06-15) | Sí (Aceptado 2026-06-15) |
| Justificación: tipo D8 opcional | Sí | Sí |
| Justificación: audiencia interna | Sí | Sí |
| Justificación: tiene_portal_developers=false | Sí (bandera en false en el manifiesto) | Sí (bandera en false en el manifiesto) |
| ADR-06 NO consolidado (§3.3, archivo individual) | Sí (archivo individual bajo adrs/) | Sí (archivo individual bajo adrs/) |
| ADR-06 inmutable / respeta §3.3 | Sí | Sí |
| Índice decisiones-arquitectura incluye ADR-06 | Sí (fila ADR-06, Despliegue, Aceptado) | Sí (fila ADR-06, Despliegue, Aceptado) |
| No existe carpeta 10_developer_guide | Confirmado (ausente) | Confirmado (ausente) |

La omisión de 10 está correctamente materializada en ambos proyectos. No hay omisión silenciosa: la ausencia de la carpeta 10 es deliberada, registrada y auditable.

---

## 5. Coherencia cross-doc

| Verificación | geovial-web | geovial-mobile |
| --- | --- | --- |
| El pipeline de 09 ejecuta los gates de 08 | Sí: gates de 08 §3 mapeados a STAGE-01..STAGE-16; DoD canónica de 08 no redefinida | Sí: gates de 08 §3 mapeados a stages; DoD de 08 no redefinida |
| Versionado usa §17.P.7 | Sí: SemVer + CC + GitVersion + trunk-based + image-docker + DEV/QA/STAGING/PROD | Sí: SemVer + CC + GitVersion + trunk-based + canales internal/alpha/beta/production |
| NFR coinciden con §17.P.10 y 05 §8 | Sí: p95 ≤ 200 ms, ≥ 50 circuitos, 99,5 % (verificados contra 05 §8 líneas 120-122) | Sí: captura offline, cola ≥ 1000, ciclo ≤ 30 s, arranque ≤ 3 s (verificados contra 05 §8 líneas 96-99) |
| Referencias inter-archivo de 09 resuelven | Sí (pipeline ↔ entornos ↔ guia ↔ supply chain ↔ versionado coherentes) | Sí (idem) |
| Estructura de 05 con 6 ADR coherente | Sí: índice lista ADR-01..ADR-06; mínimo del tipo (5) cumplido + ADR-06 por encima | Sí: índice lista ADR-01..ADR-06; mínimo del tipo (4) cumplido + ADR-05 y ADR-06 por encima |
| tiene_observabilidad_critica=false respetado (sin SLO ≥ 99,9 % ni p99) | Sí (entornos §1 nota) | Sí (entornos §1; guia §5) |

La trazabilidad upstream es genuina: los NFR, los IDs de TC, los gates de 08 y los ADR citados en las tablas de trazabilidad de 09 existen en los documentos upstream verificados (05 §8 y 08).

---

## 6. Hallazgos enumerados

### P2

**H-01 (P2) — Estado "Propuesto" en los 12 documentos de 09.**
- Archivo: los 6 docs de geovial-web/09_devops y los 6 de geovial-mobile/09_devops.
- Sección: cabecera (campo Estado).
- Evidencia: todas las cabeceras declaran `Estado: Propuesto`. Es un valor admitido por §4.1, pero el entregable de cierre de fase suele promoverse a `Aprobado`/`Vigente`. Los ADR-06 sí están en `Aceptado`, lo que crea una asimetría de madurez entre la omisión (firme) y los artefactos de 09 (propuestos).
- Recomendación: confirmar si el cierre de Fase F exige promover los 12 docs de 09 a `Aprobado`/`Vigente` antes de avanzar a Fase G, o si el estado `Propuesto` es intencional hasta el Sprint 0 de ratificación. No bloquea: el valor es conforme a §4.1.

### P3

**H-02 (P3) — Doble fila con misma versión en el control de cambios del índice de ADR de geovial-web.**
- Archivo: geovial-web/05_arquitectura_tecnica/decisiones-arquitectura_v1.0.md.
- Sección: §5 Control de cambios.
- Evidencia: dos filas con `1.0 | 2026-06-15` (la segunda incorpora ADR-06) sin incremento de versión. Higiene menor de versionado.
- Recomendación: consolidar en una sola fila o subir a `1.1` la incorporación de ADR-06. Documento de 05 (fuera del alcance estricto de 09); se anota por coherencia cross-fase.

**H-03 (P3) — Índice de ADR de geovial-mobile sin enlaces markdown.**
- Archivo: geovial-mobile/05_arquitectura_tecnica/decisiones-arquitectura_v1.0.md.
- Sección: §2 Índice de ADRs.
- Evidencia: la tabla lista los ADR como texto plano (`ADR-01`, `omision-developer-guide`, etc.), mientras el índice equivalente de geovial-web usa enlaces relativos clicables a cada archivo. Inconsistencia de estilo entre proyectos hermanos; no rompe trazabilidad porque los nombres de archivo bajo adrs/ son resolubles.
- Recomendación: alinear el índice de geovial-mobile al formato enlazado de geovial-web para navegabilidad uniforme. Documento de 05.

**H-04 (P3) — Mismo patrón de doble fila 1.0 en el control de cambios del índice de geovial-mobile.**
- Archivo: geovial-mobile/05_arquitectura_tecnica/decisiones-arquitectura_v1.0.md.
- Sección: §5 Control de cambios.
- Evidencia: dos filas `1.0 | 2026-06-15` (la segunda incorpora ADR-06). Mismo criterio que H-02.
- Recomendación: idéntica a H-02.

Los cuatro hallazgos son no bloqueantes. H-02, H-03 y H-04 recaen en documentos de 05 tocados por la incorporación de ADR-06; se reportan como observaciones de coherencia cross-fase, no como defectos de los entregables de 09.

---

## 7. Veredicto

### 7.1 Por proyecto

| Proyecto | P0 | P1 | P2 | P3 | Veredicto |
| --- | --- | --- | --- | --- | --- |
| geovial-web | 0 | 0 | 1 (compartido H-01) | 1 (H-02) | APROBADO CON OBSERVACIONES |
| geovial-mobile | 0 | 0 | 1 (compartido H-01) | 2 (H-03, H-04) | APROBADO CON OBSERVACIONES |

### 7.2 Consolidado

**APROBADO CON OBSERVACIONES.** Sin hallazgos P0 ni P1. Ambos proyectos cumplen la regla 09 §6 para su tipo D8, no presentan fugas de stack en el cuerpo, modelan correctamente ambientes (web) y canales (mobile) sin confundirlos, ejecutan la DoD de 08 como gates sin redefinirla, verifican cada NFR numérico de §17.P.10 con un stage antes de promover y documentan el rollback por tipo de artefacto. La omisión de la categoría 10 está registrada como ADR-06 de omisión en 05 de cada proyecto, con las 10 secciones de §4.3, estado Aceptado, la triple justificación exigida, archivo individual no consolidado e inclusión en el índice. Se habilita el avance a la Fase G.

### 7.3 Condiciones para promover

Ninguna condición bloqueante. Se sugiere, sin bloquear el avance: (a) resolver H-01 confirmando la política de estado de los 12 docs de 09 al cierre de fase; (b) corregir H-02/H-03/H-04 en la próxima edición de los índices de 05.

---

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-16 | Auditoría independiente inicial de la Fase F (09_devops + omisión de 10) de geovial-web y geovial-mobile (nivel 2 de GeoVial): matriz D1-D8 con fila de scan de stack (sin fugas), matriz de estructura §6 por tipo D8, registro de omisión de 10 (ADR-06 con 10 secciones, Aceptado, triple justificación, no consolidado, indexado), coherencia cross-doc y veredicto APROBADO CON OBSERVACIONES por proyecto y consolidado (P0: 0, P1: 0, P2: 1, P3: 3). |
