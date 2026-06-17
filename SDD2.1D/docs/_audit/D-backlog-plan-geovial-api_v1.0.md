# Auditoría Fase D — Backlog técnico (06) y Plan de sprint (07) — geovial-api

**Fase:** D (Backlog técnico y plan de iteración)
**Proyecto auditado:** geovial-api (`rest-api`, equipo_n=1, nivel 1 — proyecto principal)
**Categorías:** 06_backlog-tecnico (US en archivos individuales), 07_plan-sprint (modo mini-plan)
**Auditor:** Arquitecto de Soluciones + QA Senior (independiente, sin participación en la generación)
**Fecha:** 2026-06-15
**Reglas aplicadas:** `06_rules_backlog_tecnico.md` (§6, variante rest-api), `07_rules_plan_sprint.md` (§6, modo mini-plan equipo_n=1)
**Insumos upstream consultados:** 00 (`roadmap-producto_v1.0.md`), 01 (NB-01 a NB-07), 02 (CU-01 a CU-22, RN-01 a RN-07, RC-01 a RC-06) y 05 (ADR-01 a ADR-10, contrato REST, modelo lógico) de geovial-api.
**Muestreo declarado:** dado que el proyecto tiene 44 US individuales, se auditaron en profundidad 15 US representativas más todas las del primer tramo del mini-plan. US leídas íntegras: US-01, US-02, US-03, US-04, US-05, US-06, US-21, US-30, US-31, US-37, US-38, US-39, US-42, US-44 y, por verificación estructural cruzada, US-16. El primer tramo del mini-plan (US-01..06, US-37, US-38, US-39, US-44) quedó cubierto en su totalidad. Las 44 US se verificaron además mecánicamente por nombre de archivo, encoding y referencias.

---

## 1. Resumen ejecutivo

El proyecto entrega el conjunto documental obligatorio de la Fase D completo y correcto. La categoría 06 incluye `product-backlog_v1.0.md` (5 secciones), `backlog-tecnico_v1.0.md` (3 secciones con matriz BT↔US↔CU completa), `definition-of-ready_v1.0.md` (DoR US 7 / BT 5) y `README.md`, con las 44 historias de usuario individualizadas bajo `historias-usuario/US-XX-<kebab>_v1.0.md` conforme al umbral (44 > 20). Las 21 BT viven inline (21 < 30, modo correcto). La categoría 07 entrega únicamente `mini-plan_v1.0.md` y su `README.md`, sin generar ninguno de los cuatro artefactos de sprint indebidos para 1 dev (planes de iteración, plantillas de review/retro, velocidad-equipo).

La trazabilidad es sólida y bidireccional: los 22 CU quedan cubiertos por al menos una US (CU-01..22), no hay US huérfana de CU, cada BT declara fuente upstream y al menos una US consumidora o justificación de infraestructura compartida, y los 44 US / 21 BT del mini-plan referencian exactamente los IDs del backlog. El primer tramo del mini-plan se alinea al walking skeleton del roadmap (F0: autenticación + jerarquía de usuarios). No hay referencias colgantes: todo CU ∈ [01..22], RN ∈ [01..07], ADR ∈ [01..10], RC ∈ [01..06], NB ∈ [01..07]. La aritmética de SP reconcilia exactamente entre product-backlog, README y mini-plan. No hay IDs de tres dígitos heredados (`BT-001`), ni patrón `.v`, ni doble separador, ni apertura con `--`. Los archivos están en UTF-8 sin BOM y con finales de línea LF (mejora respecto del CRLF repo-wide observado en nivel 0).

Conteo de hallazgos: **P0 = 0** | **P1 = 0** | **P2 = 1** | **P3 = 2**.

| Categoría | P0 | P1 | P2 | P3 | Veredicto |
| --- | --- | --- | --- | --- | --- |
| 06_backlog-tecnico | 0 | 0 | 0 | 1 | APROBADO CON OBSERVACIONES |
| 07_plan-sprint (mini-plan) | 0 | 0 | 1 | 1 | APROBADO CON OBSERVACIONES |
| **Consolidado** | **0** | **0** | **1** | **2** | **APROBADO CON OBSERVACIONES** |

Sin P0 ni P1: la Fase D de geovial-api puede promover a la fase siguiente.

---

## 2. Matriz D1-D8 por documento

Leyenda: OK = conforme. D1 idioma rioplatense; D2 encoding UTF-8/LF; D3 kebab-case en nombre de archivo; D4 versionado `_vX.Y` (nunca `.v`); D5 sin stacks concretos (.NET, ASP.NET, SQL Server, JWT/ROPC, Docker); D6 sin vocabulario del dominio fuente del bootstrap (Motor DSL); D7 IDs de dos dígitos uniformes; D8 conjunto cerrado de documentos según modo.

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 06/product-backlog_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 06/backlog-tecnico_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 06/definition-of-ready_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 06/README.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 06/historias-usuario/US-01..US-44 (44 archivos) | OK | OK | OK | OK | OK | OK | OK | OK |
| 07/mini-plan_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 07/README.md | OK | OK | OK | OK | OK | OK | OK | OK |

Notas de la matriz:

- **D2:** los 48 archivos de Fase D (4 de 06 + 44 US + 2 de 07) están en UTF-8 sin BOM y con finales de línea LF en el working tree (verificado sobre la muestra y sobre el barrido de todos los `.md` de 06/07). Sin la condición CRLF que afectó al nivel 0. Conforme.
- **D3/D4:** las 44 US siguen `US-XX-<kebab-lowercase>_v1.0.md` con dos dígitos y guion bajo antes de la versión; no se detectó ningún `.v` ni mayúsculas/camelCase en slugs. Conforme.
- **D5:** sin menciones a stacks concretos. El vocabulario REST genérico presente —"token bearer", "problem+json", "RFC 7807", "OpenAPI", "URI", "DTO", "HTTP"— está explícitamente permitido por la matriz de evaluación. Las US incluso difieren al stack el formato físico del token ("pertenece al stack y no se fija en esta historia", US-05 §7) y el empaquetado de la exportación (US-31 §7), evitando comprometer tecnología. Los códigos de error en mayúsculas (FUERA_DE_ALCANCE, ROL_NO_AUTORIZADO, CREDENCIALES_INVALIDAS, etc.) son vocabulario de contrato propio. Conforme.
- **D6:** sin rastros del dominio fuente del bootstrap (Motor DSL/DSL). Las menciones a `BT-001` son metalingüísticas (citan el antipatrón heredado para prohibirlo). Conforme.
- **D7:** US-01..US-44, BT-01..BT-21, EP-01..EP-08 y EP-T1..EP-T8 con dos dígitos uniformes. El prefijo `EP-TX` para épicas técnicas es admisible bajo §3.2 (solo fija `EP-XX` como prefijo de épica). Sin IDs de tres dígitos. Conforme.
- **D8:** conjunto cerrado correcto. 06 con los cuatro documentos obligatorios + carpeta `historias-usuario/` (sin `tareas-tecnicas/`, correcto: 21 BT < 30). 07 con solo `mini-plan_v1.0.md` + README (sin los cuatro artefactos de sprint). Conforme.

---

## 3. Matriz de estructura obligatoria

### 3.1 06 backlog-tecnico

| Documento | Cabecera | Secciones obligatorias (§4) | Resultado |
| --- | --- | --- | --- |
| product-backlog | OK (H1 + metadatos) | 1 Objetivos, 2 Épicas (EP-01..08, dos dígitos), 3 Historias por épica (44 US, índice + cobertura CU), 4 Métricas, 5 Refinamiento | Completo (5/5) |
| backlog-tecnico | OK | 1 Épicas técnicas (EP-T1..T8), 2 BT por épica (21 BT inline), 3 Matriz BT↔US↔CU | Completo (3/3) |
| definition-of-ready | OK | 1 DoR US (7 criterios, rango 5-8 OK), 2 DoR BT (5 criterios, rango 4-6 OK), 3 Excepciones, 4 Aprobador (API Product Owner) | Completo |
| historias-usuario/US-XX (muestra) | OK (H1 em-dash, sin `--`; metadatos completos) | 1 Historia, 2 Contexto, 3 Criterios GWT, 4 Trazabilidad, 5 Prioridad y estimación, 6 DoR check, 7 Notas | Completo (7/7) |

- **BT mínimas:** 21 ≥ 10 (mínimo rest-api §2.2). OK.
- **Umbral de archivos individuales:** 44 US > 20 → archivos individuales en `historias-usuario/`. **Aplicado correctamente** (los 44 archivos existen). 21 BT < 30 → inline, sin carpeta `tareas-tecnicas/`. **Aplicado correctamente.**
- **US con ≥1 CU (cobertura bidireccional):** la §3.9 del product-backlog mapea CU-01..22 a sus US; sin US huérfana ni CU sin US. Verificado en la muestra de 15 US (cada una declara su CU en la tabla §4 de trazabilidad). OK.
- **Cada BT con fuente upstream y ≥1 US consumidora:** la matriz §3 del backlog-tecnico declara, por BT, fuente (ADR/CU/contrato/intake), US consumidoras y CU upstream. Las BT de infraestructura compartida sin US directa (BT-01, BT-03, BT-20) se justifican con ADR/intake y quedan exoneradas del criterio de US consumidora por la excepción de la DoR §3. OK.
- **US Must/Should con Given/When/Then (≥2 escenarios):** verificado en las 15 US de la muestra; todas presentan 3 escenarios (happy path + casos de borde con código de rechazo). Incluso las Could muestreadas (US-30, US-31) traen 3 escenarios, por encima del mínimo. OK.
- **MoSCoW no 100% Must:** 27 Must (61,4 %) / 10 Should (22,7 %) / 7 Could (15,9 %) / 0 Won't. Reparto razonable dentro de los rangos sugeridos del §4.7. OK.

### 3.2 07 mini-plan (equipo_n=1)

| Aspecto del §6 mini-plan | Resultado |
| --- | --- |
| Existe `mini-plan_v1.0.md` | OK |
| NO existen plan-iteracion-sprint-XX, templates review/retro, velocidad-equipo | OK (omisión justificada en README §2 y §1 del mini-plan) |
| Objetivo en una sola frase | OK — §2 es una única frase orientada a valor, sin bullets ni listas |
| DoD por referencia a 08 | OK — §5 referencia la DoD canónica de 08 (pendiente de generación) sin redefinirla; agrega criterios específicos del plan |
| Trazabilidad a CU y NB | OK — §6 tabla por tramo con NB-01..07, CU-01..22 y ADR gobernantes |
| ≥2 riesgos | OK — 4 riesgos con probabilidad/impacto y mitigación concreta |
| Nomenclatura sin doble separador | OK — `mini-plan_v1.0.md`, sin patrón `_sprint-XX_v` |
| Sin `--` antes del H1 | OK — H1 directo seguido del bloque de metadatos |

---

## 4. Coherencia cross-doc

- **IDs del mini-plan ↔ backlog:** los 44 US (US-01..44) y 21 BT (BT-01..21) del mini-plan existen idénticos en product-backlog y backlog-tecnico. Cada BT-01..21 aparece exactamente una vez entre los cuatro tramos; las 44 US aparecen exactamente una vez. No se inventan IDs. **Verificado mecánicamente.** OK.
- **US ↔ CU de 02:** cada US referencia CU reales de 02. Los 22 CU existen como archivos en `02_especificacion_funcional/casos-de-uso/CU-01..CU-22`. Sin huérfanas. OK.
- **BT ↔ ADR/CU/contrato de 05/02:** las BT referencian ADR-01..10 (las diez existen como archivos en 05/adrs/), `contratos-rest_v1.0.md`, `modelo-datos-logico_v1.0.md` y `arquitectura-solucion_v1.0.md`, además de CU y RN/RC reales. OK.
- **Alineación con el walking skeleton del roadmap (00):** el Tramo 1 del mini-plan ("Esqueleto, autenticación y jerarquía de usuarios", F0, Incremento 1) materializa exactamente la fase F0 del roadmap ("Esqueleto y jerarquía de usuarios", auth + jerarquía de punta a punta). Los cuatro tramos mapean uno a uno con F0→F3 y con los Incrementos 1→4. **Alineación correcta.** OK.
- **Aritmética de SP (subtotales/rollup):** reconciliación exacta (verificada por cálculo):
  - US: 44 historias, 211 SP. MoSCoW SP: Must 142 (67,3 %), Should 32 (15,2 %), Could 37 (17,5 %). Coincide con product-backlog §4 y README.
  - BT: 21 tareas, 140 SP.
  - Mini-plan: subtotales de tramo 117 + 74 + 79 + 81 = 351 SP; cada subtotal recomputado coincide (T1 65 BT + 52 US = 117; T2 18 + 56 = 74; T3 34 + 45 = 79; T4 23 + 58 = 81). El total general 351 = 211 (US) + 140 (BT), explícitamente desglosado y declarado coherente con 06 §4. OK (ver H-07-01 sobre la suma US+BT como método).
- **README (06) ↔ product-backlog:** la lista de las 27 US Must del README coincide exactamente con la clasificación Must del product-backlog; las 7 Could (US-30..36) coinciden; la tabla de distribución MoSCoW del README replica la del product-backlog. La tabla "Modo de archivos aplicado" del README refleja correctamente los umbrales. OK.
- **DoR ↔ DoD de 08:** la DoR (06) delimita explícitamente su frontera con la DoD de 08 (§5 con tabla comparativa), sin solaparse: la DoR exige que los criterios existan y sean verificables, no que pasen. OK.

---

## 5. Chequeo específico de referencias colgantes

Búsqueda exhaustiva de identificadores fuera de rango en los entregables de Fase D (06 y 07).

| Taxonomía | Rango válido (geovial-api) | Máximo referenciado | Fuera de rango |
| --- | --- | --- | --- |
| CU | CU-01..CU-22 | CU-22 | Ninguno |
| RN | RN-01..RN-07 | RN-07 | Ninguno |
| ADR | ADR-01..ADR-10 | ADR-10 | Ninguno |
| RC | RC-01..RC-06 | RC-06 | Ninguno |
| NB | NB-01..NB-07 | NB-07 | Ninguno |

- No se detectó ningún `CU-23+`, `RN-08+`, `ADR-11+`, `RC-07+` ni `NB-08+` en ningún documento de 06 ni 07 (barrido sobre los 48 archivos).
- Las menciones a `BT-001` y `M0001` no son referencias colgantes: `BT-001` cita el antipatrón heredado para prohibirlo y `M0001_inicial` es el nombre de la migración inicial (criterio de aceptación de BT-04).

Resultado: **cero referencias colgantes** en la Fase D de geovial-api.

---

## 6. Hallazgos

Nivel / archivo / sección / evidencia / recomendación.

**H-07-01 (P2) — El total general del mini-plan suma SP de US y de BT (351 = 211 + 140).**
- Archivo: 07/mini-plan_v1.0.md §3 (cierre de "Ítems comprometidos por tramo") y §9 (bitácora).
- Sección: ítems comprometidos / total de puntos.
- Evidencia: el mini-plan declara "Total general comprometido: 351 SP" sumando los 211 SP de las 44 US y los 140 SP de las 21 BT. Cuando una US ya estima el trabajo end-to-end que incluye el de sus BT derivadas, sumar ambos SP puede double-contar el esfuerzo y distorsionar el forecast de capacidad. El documento es transparente al respecto (desglosa "211 SP de US (coherente con 06 §4) … 140 SP del esfuerzo de construcción interno") y los subtotales por tramo son internamente consistentes, por lo que no rompe trazabilidad ni aritmética; es una decisión metodológica de medición. El §4.2.3 de 07_rules pide "total de puntos comprometidos al pie" sin prescribir si US y BT son aditivas.
- Recomendación: aclarar en §3 que las dos magnitudes (211 SP de valor de producto y 140 SP de construcción interna) miden dimensiones distintas y no deben leerse como un único forecast aditivo de capacidad; o adoptar una sola unidad de compromiso para la calibración de la bitácora §9. No bloqueante.

**H-06-01 (P3) — Reabsorción de US-37/US-38 en EP-01 documentada pero con índice de épica que la duplica.**
- Archivo: 06/product-backlog_v1.0.md §2 (cierre) y §3.1; 06/README.md (Resumen de épicas).
- Sección: épicas / historias por épica.
- Evidencia: US-37 y US-38 (autorización por rol y alcance, CU-18) se modelan dentro de EP-01 por ser fundacionales de NB-01, lo que está explicado en el texto. La numeración no correlativa (US-37/US-38 conviviendo con US-01..06 en EP-01, mientras EP-08 agrupa US-39..44) es coherente y trazable, pero obliga al lector a saltar entre rangos para reconstruir EP-01. Es una elección de diseño, no un defecto de trazabilidad.
- Recomendación: opcional, una nota al pie en la tabla §3.1 que remita a la cobertura de CU-18/CU-20 ya presente en §3.9 para facilitar la lectura. No bloqueante.

**H-07-02 (P3) — Numeración de tramos sin objetivo de valor por tramo explícito.**
- Archivo: 07/mini-plan_v1.0.md §3.1–§3.4.
- Sección: ítems comprometidos por tramo.
- Evidencia: el objetivo único de valor (§2, una sola frase) cumple el criterio del §6. Cada tramo tiene un encabezado descriptivo y se alinea a una fase del roadmap, pero no enuncia un "objetivo de tramo" en una frase de valor propia (a diferencia de un sprint goal por iteración). En modo mini-plan de 1 dev esto es admisible (no hay sprint goal por sprint), y los criterios de hecho por tramo (§8) y los criterios de transición de fase (00 §5) cubren la verificación.
- Recomendación: opcional, agregar una frase de valor por tramo para reforzar la orientación a resultado de cada incremento. No bloqueante.

---

## 7. Veredicto

**APROBADO CON OBSERVACIONES.**

Sin hallazgos P0 ni P1. La Fase D de geovial-api (06 backlog-tecnico con 44 US individualizadas y 21 BT inline; 07 mini-plan para equipo_n=1) está completa, es trazable y coherente:

- Conjunto documental obligatorio completo: product-backlog (5 secciones), backlog-tecnico (3 secciones + matriz BT↔US↔CU completa), definition-of-ready (DoR US 7 / BT 5) y README; 44 US individuales bajo `historias-usuario/` por superar el umbral de 20; 21 BT inline por estar bajo el de 30; mini-plan único sin artefactos de sprint indebidos para 1 dev.
- Trazabilidad bidireccional sólida: cobertura CU-01..22 sin huérfanas; cada BT con fuente upstream y US consumidora o justificación de infraestructura compartida; IDs del mini-plan idénticos a los del backlog; primer tramo alineado al walking skeleton F0 (auth + jerarquía) del roadmap.
- Cero referencias colgantes (CU ≤22, RN ≤07, ADR ≤10, RC ≤06, NB ≤07); aritmética de SP reconciliada exactamente entre product-backlog, README y mini-plan; MoSCoW no 100 % Must (61,4/22,7/15,9); IDs de dos dígitos uniformes sin `BT-001`; sin `.v`, sin doble separador, sin `--` antes del H1; UTF-8/LF sin BOM.

Las observaciones registradas (1 P2 sobre la suma aditiva US+BT del total del mini-plan; 2 P3 de legibilidad/forma) no afectan el cumplimiento sustantivo ni la trazabilidad y no bloquean la promoción. **La Fase D de geovial-api puede avanzar a la fase siguiente.**

---

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Informe inicial del audit independiente de Fase D (06 backlog-tecnico con US individuales y 07 plan-sprint en modo mini-plan) del proyecto principal geovial-api (rest-api, equipo_n=1, nivel 1). Matriz D1-D8, matriz de estructura, coherencia cross-doc, chequeo de referencias colgantes, 3 hallazgos (0 P0 / 0 P1 / 1 P2 / 2 P3) y veredicto APROBADO CON OBSERVACIONES. Muestreo declarado de 15 US representativas más la totalidad del primer tramo del mini-plan. |
