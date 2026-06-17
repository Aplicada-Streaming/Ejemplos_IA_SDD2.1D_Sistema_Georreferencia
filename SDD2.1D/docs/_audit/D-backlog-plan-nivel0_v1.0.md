# Auditoría Fase D — Backlog técnico (06) y Plan de sprint (07) — Nivel 0

**Fase:** D (Backlog técnico y plan de iteración)
**Proyectos auditados:** aplicada-sync, geovial-storage (ambos `library`, equipo_n=1)
**Categorías:** 06_backlog-tecnico, 07_plan-sprint (modo mini-plan)
**Auditor:** Arquitecto de Soluciones + QA Senior (independiente, sin participación en la generación)
**Fecha:** 2026-06-15
**Reglas aplicadas:** `06_rules_backlog_tecnico.md` (§6, variante library), `07_rules_plan_sprint.md` (§6, modo mini-plan equipo_n=1)
**Insumos upstream consultados:** 01 (NB-01 a NB-07), 02 (CU, RN) y 05 (ADR, componentes, contratos) de cada proyecto.

---

## 1. Resumen ejecutivo

Ambos proyectos entregan el conjunto documental obligatorio de la Fase D completo y correcto: 06 con product-backlog, backlog-tecnico y definition-of-ready (US y BT inline, por debajo de los umbrales 20/30), y 07 con `mini-plan_v1.0.md` único, sin generar los cuatro artefactos de sprint indebidos para 1 dev. La trazabilidad upstream/cross-doc es sólida: los IDs US/BT del mini-plan referencian exactamente los del backlog, cada US referencia CU reales de 02, y cada BT referencia ADR/componentes/contratos reales de 05. No se detectaron referencias colgantes a RN ni ADR fuera de rango en ninguno de los dos proyectos; el phantom "RN-07" del ADR-04 de aplicada-sync (Fase C) NO se propagó al backlog de Fase D. No hay IDs de tres dígitos heredados (`BT-001`), ni patrón `.v`, ni doble separador, ni apertura con `--`.

Conteo de hallazgos: **P0 = 0** | **P1 = 0** | **P2 = 3** | **P3 = 4**.

| Proyecto | P0 | P1 | P2 | P3 | Veredicto |
| --- | --- | --- | --- | --- | --- |
| aplicada-sync | 0 | 0 | 1 | 2 | APROBADO CON OBSERVACIONES |
| geovial-storage | 0 | 0 | 1 | 2 | APROBADO CON OBSERVACIONES |
| Común a ambos | 0 | 0 | 1 | 0 | — |
| **Consolidado** | **0** | **0** | **3** | **4** | **APROBADO CON OBSERVACIONES** |

Sin P0: ambos proyectos pueden avanzar a la fase siguiente.

---

## 2. Matriz D1-D8 por documento

Leyenda: OK = conforme; Obs = observación menor (ver hallazgos). D1 idioma rioplatense; D2 encoding UTF-8/LF; D3 kebab-case filename; D4 versionado `_vX.Y` (no `.v`); D5 sin stacks concretos; D6 sin vocabulario del dominio fuente del bootstrap (Motor DSL); D7 IDs dos dígitos uniformes; D8 conjunto cerrado de documentos.

### aplicada-sync

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 06/product-backlog_v1.0.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 06/backlog-tecnico_v1.0.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 06/definition-of-ready_v1.0.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 06/README.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 07/mini-plan_v1.0.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 07/README.md | OK | Obs | OK | OK | OK | OK | OK | OK |

### geovial-storage

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 06/product-backlog_v1.0.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 06/backlog-tecnico_v1.0.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 06/definition-of-ready_v1.0.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 06/README.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 07/mini-plan_v1.0.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 07/README.md | OK | Obs | OK | OK | OK | OK | OK | OK |

Notas de la matriz:

- **D2 (Obs en todos):** los doce archivos de Fase D están en UTF-8 sin BOM (correcto), pero el working tree presenta finales de línea CRLF en lugar de LF. Es una condición repo-wide (también la presentan 01, 02 y 05) atribuible a `core.autocrlf=true` en este checkout Windows; los archivos de reglas, en cambio, están en LF. Los entregables de Fase D están sin trackear en git al momento del audit; al commitearse con autocrlf activo se normalizan a LF en el repositorio. Se documenta como reconciliación P2 común, no como defecto bloqueante propio de Fase D (ver hallazgo H-COM-01).
- **D5/D6:** vocabulario neutral de librería en ambos proyectos. Los códigos de error en mayúsculas (CONTENIDO_VACIO, PROVEEDOR_NO_DISPONIBLE, etc.) y términos como "cola", "sesión", "proveedor" son vocabulario de contrato propio, no stacks comerciales ni términos del fuente Motor DSL. Conforme.
- **D7:** US-XX, BT-XX, EP-XX de dos dígitos uniformes en ambos. aplicada-sync introduce además un prefijo de épica técnica `ET-XX` (dos dígitos); geovial-storage reutiliza `EP-XX` como encabezado de sus épicas técnicas. Ambas opciones son admisibles bajo §3.2 (que solo fija `EP-XX` como prefijo de épica). Ver H-COM-02 (P3) por la inconsistencia inter-proyecto, no bloqueante.

---

## 3. Matriz de estructura obligatoria

### 3.1 aplicada-sync — 06 backlog-tecnico

| Documento | Cabecera | Secciones obligatorias (§6/§4) | Resultado |
| --- | --- | --- | --- |
| product-backlog | OK (H1 + metadatos) | 1 Objetivos, 2 Épicas (EP-01..06), 3 Historias por épica (13 US inline), 4 Métricas, 5 Refinamiento | Completo (5/5) |
| backlog-tecnico | OK | 1 Épicas técnicas (ET-01..06), 2 BT por épica (14 BT inline), 3 Matriz BT↔US↔CU | Completo (3/3) |
| definition-of-ready | OK | 1 DoR US (7 criterios, rango 5-8 OK), 2 DoR BT (5 criterios, rango 4-6 OK), 3 Excepciones, 4 Aprobador (AG-06) | Completo |

- BT mínimas: 14 ≥ 10 (mínimo library). OK.
- US con ≥1 CU: las 13 US tienen columna CU relacionados poblada; sin huérfanas. OK.
- Cada BT con fuente upstream y ≥1 US consumidora o justificación de infraestructura compartida (BT-01 y BT-13 justificadas con ADR/contrato). OK.
- US Must/Should con Given/When/Then ≥2 escenarios: verificado en las 6 Must y 5 Should (las Could US-09/US-11 incluyen BDD por buena práctica, declarado en nota). OK.
- MoSCoW no 100% Must: 6 Must / 5 Should / 2 Could / 0 Won't. OK (ver H-AS-01 P3 sobre Should por encima del rango sugerido).

### 3.2 aplicada-sync — 07 mini-plan

| Aspecto del §6 mini-plan (equipo_n=1) | Resultado |
| --- | --- |
| Existe `mini-plan_v1.0.md` | OK |
| NO existen plan-iteracion-sprint-XX, templates review/retro, velocidad-equipo | OK (omisión justificada en README y §1 del mini-plan) |
| Sprint goal como una sola frase | OK — §2 "Objetivo del release" es una sola frase orientada a valor (NB-04) |
| DoD por referencia a 08 | OK — §5 referencia la DoD canónica de 08 (pendiente) sin redefinirla |
| Trazabilidad a CU y NB | OK — §8 tabla con NB-04, CU-01..06, RN-01..03, ADR-01..08 |
| ≥2 riesgos | OK — 4 riesgos con mitigación concreta |
| Nomenclatura sin doble separador / sin `--` antes del H1 | OK |

### 3.3 geovial-storage — 06 backlog-tecnico

| Documento | Cabecera | Secciones obligatorias | Resultado |
| --- | --- | --- | --- |
| product-backlog | OK | 1 Objetivos, 2 Épicas (EP-01..04), 3 Historias por épica (9 US inline), 4 Métricas, 5 Refinamiento | Completo (5/5) |
| backlog-tecnico | OK | 1 Épicas técnicas, 2 BT por épica (13 BT inline), 3 Matriz BT↔US↔CU | Completo (3/3) |
| definition-of-ready | OK | 1 DoR US (7 criterios), 2 DoR BT (5 criterios), 3 Excepciones, 4 Aprobador (AG-06) | Completo |

- BT mínimas: 13 ≥ 10. OK.
- US con ≥1 CU: las 9 US tienen CU relacionado; sin huérfanas. OK.
- Cada BT con fuente upstream y US consumidora o justificación (BT-10, BT-11, BT-13 justificadas como infraestructura compartida con ADR/contrato). OK.
- US Must/Should con Given/When/Then ≥2 escenarios: verificado en las 5 Must y 3 Should; la Could US-09 trae 2 escenarios igualmente. OK.
- MoSCoW no 100% Must: 5 Must / 3 Should / 1 Could / 0 Won't (56%/33%/11%). OK; el propio documento reconoce el Should ligeramente por encima del rango por el bajo número total de historias.

### 3.4 geovial-storage — 07 mini-plan

| Aspecto del §6 mini-plan | Resultado |
| --- | --- |
| Existe `mini-plan_v1.0.md` | OK |
| NO existen los cuatro artefactos de sprint | OK (omisión justificada en README, tabla "Modo de esta sección") |
| Sprint goal como una sola frase | OK — §2 "Objetivo del plan" es una sola frase orientada a valor |
| DoD por referencia a 08 | OK — §5 referencia la DoD canónica de 08 (pendiente) y suma criterios específicos del plan |
| Trazabilidad a CU y NB | OK — §8 con CU-01..06, NB-07/NB-03/NB-06, ADR-01..05 |
| ≥2 riesgos | OK — 4 riesgos con mitigación concreta |
| Nomenclatura sin doble separador / sin `--` antes del H1 | OK |

---

## 4. Coherencia cross-doc

### 4.1 aplicada-sync

- **IDs del mini-plan ↔ backlog:** las 13 US (US-01..13) y 14 BT (BT-01..14) del mini-plan existen idénticas en product-backlog y backlog-tecnico. No se inventan IDs. OK.
- **US ↔ CU de 02:** cada US referencia CU-01..06; los seis CU existen como archivos en 02. Sin huérfanas. OK.
- **BT ↔ ADR/componentes de 05:** las BT referencian ADR-01..08 (las ocho ADR existen), `contratos-abstractions_v1.0.md`, `extensibilidad_v1.0.md`, `flujo-ejecucion_v1.0.md`, `arquitectura-solucion_v1.0.md` y componentes nombrados en 05. OK.
- **Aritmética interna:** MoSCoW SP (Must 32 / Should 17 / Could 5 = 54) consistente entre product-backlog §4 y README; MVP 32 SP (6 Must) coincide; R1=32 SP, R2=22 SP en el mini-plan suman 54. Cadena de dependencias del mini-plan §4 consistente con la columna Dependencias del backlog-tecnico §2. OK.
- **README:** índice fiel; épicas, US Must, BT prioritarias y DoR coinciden con los documentos fuente. OK.

### 4.2 geovial-storage

- **IDs del mini-plan ↔ backlog:** las 9 US (US-01..09) y 13 BT (BT-01..13) del mini-plan existen idénticas en el backlog. No se inventan IDs. OK.
- **US ↔ CU de 02:** cada US referencia CU-01..06; los seis CU existen. Sin huérfanas. OK.
- **BT ↔ ADR/componentes de 05:** las BT referencian ADR-01..05 (las cinco existen), `contratos-abstractions_v1.0.md`, `extensibilidad_v1.0.md` y componentes de 05. OK.
- **NB upstream:** NB-07 (principal), NB-03 y NB-06 (soporte) existen como archivos en 01. OK.
- **Aritmética interna:** MoSCoW (Must 23 SP / Should 11 / Could 3 = 37 US SP); BT 71 SP; total mini-plan 108 SP; subtotales de tramos (19+16+26+24+23) consistentes con la suma; tabla de dependencias §4 coherente con la columna Dependencias del backlog-tecnico. OK.
- **Recorte de alcance:** la nota sobre migración de archivos y subida por fragmentos fuera de alcance está respaldada por 02 §7 "Decisiones de recorte". OK.

---

## 5. Chequeo específico de referencias colgantes

Búsqueda exhaustiva de identificadores fuera de rango en los entregables de Fase D (06 y 07) de ambos proyectos.

| Proyecto | Rango RN válido | Rango ADR válido | RN fuera de rango | ADR fuera de rango | CU fuera de rango |
| --- | --- | --- | --- | --- | --- |
| aplicada-sync | RN-01..03 | ADR-01..08 | Ninguna | Ninguna | Ninguna |
| geovial-storage | RN-01..03 | ADR-01..05 | Ninguna | Ninguna | Ninguna |

- En aplicada-sync, el backlog usa exclusivamente RN-01, RN-02, RN-03 y ADR-01..ADR-08. No aparece el phantom "RN-07" del ADR-04 de Fase C; **no hubo propagación a Fase D**. No hay hallazgo de propagación.
- En geovial-storage, el backlog usa exclusivamente RN-01..03 y ADR-01..05. Sin referencias colgantes.
- Las NB referenciadas (NB-04 en aplicada-sync; NB-07/NB-06/NB-03 en geovial-storage) existen todas en 01 (NB-01..07).

Resultado: **cero referencias colgantes** en Fase D para ambos proyectos.

---

## 6. Hallazgos

Nivel / archivo / sección / evidencia / recomendación.

### Comunes a ambos proyectos

**H-COM-01 (P2) — Finales de línea CRLF en el working tree.**
- Archivos: los doce entregables de Fase D (06 y 07 de ambos proyectos).
- Sección: encoding (D2).
- Evidencia: todos los `.md` de Fase D presentan CRLF en el working tree; la condición es repo-wide (01, 02 y 05 también) y `core.autocrlf=true` está activo en este checkout Windows. Los archivos de reglas están en LF. Los entregables están sin trackear en git al momento del audit.
- Recomendación: confirmar que al commitear (con autocrlf activo) los blobs se normalizan a LF, o fijar un `.gitattributes` con `*.md text eol=lf` para garantizar LF en el repositorio de forma determinística. No bloquea la fase; es reconciliación de tooling, no defecto de contenido.

**H-COM-02 (P3) — Inconsistencia inter-proyecto en el prefijo de épicas técnicas.**
- Archivos: aplicada-sync/06/backlog-tecnico_v1.0.md §1 vs. geovial-storage/06/backlog-tecnico_v1.0.md §1.
- Sección: épicas técnicas (§4.3).
- Evidencia: aplicada-sync nombra sus épicas técnicas `ET-01..ET-06` (prefijo propio); geovial-storage reutiliza `EP-01..EP-04` (el mismo prefijo de las épicas de producto) para las épicas técnicas. Ambas son admisibles bajo §3.2 (solo fija `EP-XX` como prefijo de épica de dos dígitos; no prohíbe ni exige un prefijo técnico distinto).
- Recomendación: unificar el criterio en una revisión transversal de la solución para legibilidad inter-proyecto. No bloqueante.

### aplicada-sync

**H-AS-01 (P3) — Distribución Should por encima del rango sugerido.**
- Archivo: 06/product-backlog_v1.0.md §4 (Métricas de avance).
- Evidencia: Should = 38% de las US (5 de 13), por encima del rango sugerido 20-30% del §4.7. La regla solo prohíbe el 100% Must (que no ocurre: 6 Must / 5 Should / 2 Could / 0 Won't).
- Recomendación: aceptable por el bajo número total de historias; opcionalmente reclasificar alguna Should a Could o documentar el desvío como ya hace geovial-storage. No bloqueante.

**H-AS-02 (P3) — Terminología "release/mini-plan" en lugar de "sprint goal" literal.**
- Archivo: 07/mini-plan_v1.0.md §2.
- Evidencia: el objetivo se titula "Objetivo del release" y el plan habla de "tramos de release" en vez de "sprint goal" / "sprint". Es coherente con la variante library release-driven (§1.2 de 07_rules) y cumple el fondo del §4.2.2 (una sola frase orientada a valor), pero se aparta del rótulo nominal del criterio.
- Recomendación: opcional, mantener una referencia explícita al término "sprint goal" para alinear con la matriz de §6. No afecta el cumplimiento sustantivo.

### geovial-storage

**H-GS-01 (P3) — Encabezados de cabecera del README sin campo "Documento".**
- Archivo: 06/README.md (cabecera).
- Evidencia: la cabecera del README usa campos "Variante" y "Tipo (D8)" pero omite el campo "Documento:" presente en el resto de los artefactos de la sección y en el README de aplicada-sync. El README es recomendado (no obligatorio), por lo que es cosmético.
- Recomendación: agregar `**Documento:** README.md` por uniformidad de cabecera. No bloqueante.

**H-GS-02 (P3) — Métrica MoSCoW: el documento declara el desvío pero no lo recorta.**
- Archivo: 06/product-backlog_v1.0.md §4.
- Evidencia: Should = 33% (objetivo 20-30%), reconocido explícitamente en el propio texto ("ligeramente por encima por el bajo número total de historias"). Cumplimiento sustantivo OK; solo se anota la transparencia del desvío.
- Recomendación: ninguna acción obligatoria; el reconocimiento documentado es buena práctica.

---

## 7. Veredicto

### Por proyecto

- **aplicada-sync: APROBADO CON OBSERVACIONES.** Sin P0 ni P1. Backlog (06) y mini-plan (07) completos, trazables y coherentes; sin referencias colgantes; IDs de dos dígitos uniformes; mini-plan correcto para equipo_n=1 sin artefactos de sprint indebidos. Observaciones P2/P3 (CRLF repo-wide, prefijo ET-XX, distribución Should, rótulo release) no bloquean. Puede avanzar.

- **geovial-storage: APROBADO CON OBSERVACIONES.** Sin P0 ni P1. Backlog (06) y mini-plan (07) completos, trazables y coherentes; sin referencias colgantes (RN-01..03, ADR-01..05, NB existentes); 13 BT ≥ 10; mini-plan correcto para equipo_n=1. Observaciones P2/P3 (CRLF repo-wide, prefijo EP en épicas técnicas, README sin campo Documento, desvío Should documentado) no bloquean. Puede avanzar.

### Consolidado

**APROBADO CON OBSERVACIONES.** Conteo total: **P0 = 0, P1 = 0, P2 = 3, P3 = 4.** Ningún hallazgo bloqueante. La Fase D de nivel 0 puede promover a la fase siguiente. Se recomienda atender H-COM-01 (normalización LF) como reconciliación de tooling antes del cierre del repositorio y unificar el criterio de prefijo de épica técnica (H-COM-02) en una revisión transversal.

---

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Informe inicial del audit independiente de Fase D (06 backlog-tecnico y 07 plan-sprint, modo mini-plan) para los proyectos de nivel 0 aplicada-sync y geovial-storage. Matriz D1-D8, matriz de estructura, coherencia cross-doc, chequeo de referencias colgantes, 7 hallazgos (0 P0 / 0 P1 / 3 P2 / 4 P3) y veredicto APROBADO CON OBSERVACIONES por proyecto y consolidado. |
