# Auditoría Fase F — DevOps (09) + Developer guide (10) — Nivel 0

**Documento:** F-devops-devguide-nivel0_v1.0.md
**Fase auditada:** F (categorías `09_devops` y `10_developer_guide`)
**Alcance:** proyectos de nivel 0 `aplicada-sync` y `geovial-storage` (ambos `library`, `equipo_n=1`)
**Auditor:** Arquitecto de Soluciones + QA Senior (independiente, no participó de la generación)
**Fecha:** 2026-06-15
**Estado:** Vigente
**Veredicto consolidado:** APROBADO CON OBSERVACIONES (sin P0)

---

## 1. Resumen ejecutivo

Se auditaron 26 entregables: 11 de `09_devops` (6 de aplicada-sync, 5 de geovial-storage) y 15 de `10_developer_guide` (7 de aplicada-sync, 7 de geovial-storage; ambas categorías obligatorias para `library`). Ningún proyecto presenta hallazgos P0: no hay documentos obligatorios faltantes, no se confunde canales con ambientes, no hay slug comercial hardcodeado, no hay stack ni vocabulario del dominio fuente prohibido, y la trazabilidad upstream (05/08) está intacta.

La guía de publicación de aplicada-sync existe y es correcta (`guia-publicacion-paquete-nuget_v1.0.md`, redistribuible). En geovial-storage está correctamente OMITIDA y la omisión está justificada en el README citando 09_rules §2.1.

Los dos proyectos cumplen el contrato de la Fase F. Los hallazgos abiertos son de severidad media y baja: una NFR numérica de geovial-storage sin stage explícito en el pipeline (P2), una referencia cruzada colgada en el glosario de geovial-storage (P2), la cabecera del README de 09 de geovial-storage que no sigue el bloque §4.1 (P2), e inconsistencias de estado de cabecera entre proyectos y respecto del upstream (P3).

Conteo por nivel: **P0: 0 — P1: 0 — P2: 4 — P3: 3.**

---

## 2. Matriz D1-D8 (idioma, codificación, nomenclatura, vocabulario)

Recordatorio metodológico: el repo declara `.gitattributes` con `*.md text eol=lf`; el CRLF del working tree no se computa como violación.

| Criterio | aplicada-sync 09 | aplicada-sync 10 | geovial-storage 09 | geovial-storage 10 |
| --- | --- | --- | --- | --- |
| D1 Idioma rioplatense técnico | Cumple | Cumple | Cumple | Cumple |
| D2 UTF-8 / LF (vía .gitattributes) | Cumple | Cumple | Cumple | Cumple |
| D3 kebab-case en filenames | Cumple | Cumple | Cumple | Cumple |
| D4 Sufijo `_vX.Y` (nunca `.v`) | Cumple | Cumple | Cumple | Cumple |
| D5 Sin vocabulario del dominio fuente del bootstrap | Cumple | Cumple | Cumple | Cumple |
| D6 Gestor concreto solo donde el proyecto lo usa (09) | Cumple | n/a | Cumple (no nombra gestor) | n/a |
| D7 Cuerpo de 10 stack-abstract (código en 11) | n/a | Cumple | n/a | Cumple |
| D8 Sin slug comercial hardcodeado | Cumple | Cumple (`aplicacion-movil`) | Cumple | Cumple (`servicio-backend`) |

Notas de verificación:

- No aparece ningún patrón heredado `.vX` en ningún filename ni cuerpo (búsqueda negativa confirmada).
- El token `maui` aparece únicamente como nombre del sample de la categoría 11 (`03-avanzado-demo-maui`, demo explícitamente pedido en intake §18) y como mención del runtime objetivo del paquete (`net8.0-android` / workload MAUI) en la guía de publicación y el pipeline de aplicada-sync, contexto admitido (el proyecto efectivamente publica contra ese target). El slug del archivo de integración de 10 es genérico (`guia-integracion-aplicacion-movil_v1.0.md`), no `guia-integracion-maui`. No es violación.
- En 09, el gestor concreto (`paquete-nuget`, `dotnet pack/push`) aparece solo en `guia-publicacion-paquete-nuget_v1.0.md` de aplicada-sync (donde el proyecto sí publica) y en su §0 de justificación; no se filtra al resto de la categoría. geovial-storage no nombra gestor alguno (coherente con no publicar). Cumple D6.
- El cuerpo de 10 describe operaciones, formas de datos y comportamiento de forma neutral de stack; los snippets son pseudo-secuencias (`text`) que delegan el código ejecutable a 11. Cumple D7.

---

## 3. Matriz de estructura — categoría 09 (library, §6 de 09_rules)

| Requisito §6 (library) | aplicada-sync | geovial-storage |
| --- | --- | --- |
| `pipeline-ci-cd_v1.0.md` con stages lint/build/test/SCA/SBOM/firma/publish | Presente; stages completos + cobertura/mutation/NFR/compatibilidad/post-publish | Presente; 12 stages; sin Publish propio (justificado: no redistribuible) |
| Matriz SO/runtime, caché, artefactos | Presente | Presente |
| Promotion rules (canales, no DEV/QA/STAGING/PROD) | Canales preview→stable; aprobador humano en stable | Promoción heredada del backend; sin canales ni ambientes propios |
| Rollback por tipo de artefacto con comando | Unlist + fix, comando concreto | Reversión de la imagen del backend + git revert |
| Notificaciones | Presente | Presente |
| `estrategia-versionado_v1.0.md` (SemVer+ConvCommits+herramienta+canales+deprecation) | Completa; GitVersion; canales preview/stable; deprecation alineada a contratos §6 | Completa; versión derivada del tag alineada al backend; canal único lógico; deprecation |
| `entornos-deploy_v1.0.md` con modelo de CANALES (no ambientes) | Canales preview/stable sobre feed; declara no-aplicabilidad de SLA de ambiente | Declara explícitamente que no hay feed ni ambientes propios; hereda los del backend |
| `supply-chain-seguridad_v1.0.md` (SBOM/firma/SLSA/SCA/SAST-DAST/CVE) | Completo; SLSA L3 objetivo; DAST no aplica (justificado) | Completo; SLSA L2 heredado; DAST no aplica (justificado); credenciales ADR-05 |
| Guía de publicación | PRESENTE y correcta (`guia-publicacion-paquete-nuget`, redistribuible) | OMITIDA con justificación en README (no se publica externamente) |
| README de la sección | Presente; índice + orden de lectura + mapa gates→stages | Presente; índice + omisiones registradas + mapa gates→stages |
| Gates ejecutan la DoD de 08 sin redefinirla | Cumple (G1–G9 mapeados a stages; DoD no redefinida) | Cumple (G-01..G-09 mapeados; DoD no redefinida) |
| Cada NFR numérico de 05 con stage que lo verifica | Cumple (G7 + G6 property-based cubren las 6 NFR) | **Parcial: NFR-02 (tamaño máx. 25 MB) sin stage explícito** (H-01, P2) |

Anti-patrón "confundir publicación con despliegue": correctamente evitado en ambos proyectos; ambos lo declaran de forma explícita y citan el anti-patrón de 09_rules §4.8.

---

## 4. Matriz de estructura — categoría 10 (library obligatoria, §6 de 10_rules)

| Requisito §6 (library) | aplicada-sync | geovial-storage |
| --- | --- | --- |
| `conceptos-fundamentales` (Explanation) | Presente; concepto, modelo mental, 7 decisiones con ADR, vocabulario, qué NO hace | Presente; concepto, 4 etapas, 6 decisiones con ADR, vocabulario, qué NO hace |
| `guia-onboarding-developer` (TTFS 5/30/60) | Presente; Hello world 5min / caso real 30min / integración 1h | Presente; Hello world 5min / caso real 30min / integración 1h |
| `guia-integracion-<sistema-objetivo>` con slug GENÉRICO | `aplicacion-movil` (genérico) | `servicio-backend` (genérico) |
| `referencia-api` con paridad 1:1 con contratos de 05 | Cumple (6 ops, 6 formas, 4 extensiones, 2 eventos, 15 códigos) | Cumple (6 ops, tipos, 14 códigos) |
| `troubleshooting` (≥5 ISSUE-XX con diagnóstico paso a paso) | 7 entradas (ISSUE-01..07) + plantilla de bug | 6 entradas (ISSUE-01..06) + plantilla de bug |
| `glosario-tecnico` | 28 términos kebab + cross-doc | 20 términos kebab + cross-doc |
| `README` de la sección | Presente; índice + orden + prerequisitos + quick-start | Presente; índice + orden + prerequisitos + quick-start |
| Cabecera Diátaxis/Audiencia/Nivel/Tiempo por doc | Cumple en los 6 docs versionables | Cumple en los 6 docs versionables |
| Referencias cruzadas con ≥1 enlace a 05 por doc | Cumple en todos | Cumple en todos |
| Sufijo `_v1.0.md` en todos los filenames | Cumple | Cumple |

Verificación de paridad 1:1 (10 referencia-api ↔ 05 contratos):

- aplicada-sync: los 15 códigos de `referencia-api §6` coinciden con el catálogo de `contratos-abstractions §5` y `dx-error-messages`; las 6 operaciones, las 6 formas de datos y los 4 contratos de extensión reflejan `contratos-abstractions §3/§4` y `extensibilidad §3`. Paridad estricta confirmada.
- geovial-storage: los 14 códigos de `referencia-api §4` coinciden con la taxonomía de `contratos-abstractions §3/§5`; las 6 operaciones (5 datos + CU-06) y los tipos lógicos reflejan `contratos-abstractions §3/§4`. Paridad estricta confirmada.

---

## 5. Coherencia cross-doc

| Verificación | Resultado |
| --- | --- |
| referencia-api de 10 == contrato de 05 | Coincide en ambos proyectos (ver §4) |
| Pipeline de 09 ejecuta los gates de 08 (sin redefinir DoD) | Cumple; aplicada-sync mapea G1–G9, geovial-storage mapea G-01..G-09, ambos citan la DoD canónica |
| Versionado usa los valores de §17 P.7 | aplicada-sync: SemVer+ConvCommits+GitVersion+GitHub Packages (P.7) — coincide. geovial-storage: SemVer+ConvCommits+GitVersion alineado al backend, no NuGet (P.7) — coincide |
| Rollback usa §17 P.8 | aplicada-sync: unlist (P.8) — coincide. geovial-storage: reversión de imagen del backend (P.8) — coincide |
| Gate de cobertura usa §17 P.6 | Ambos: líneas ≥80 % / branches ≥70 % — coincide |
| Consistencia de estados de cabecera entre proyectos | **Inconsistente** (H-06, P3): aplicada-sync/10 = Vigente; geovial-storage/10 = Borrador; ambos 09 y todo el upstream 05/08 = Propuesto |

Hallazgo cross-doc adicional (geovial-storage 10): el glosario cita `conceptos-fundamentales §2 (concepto-proveedor-activo)` y `§2 (concepto-abstraccion)`, pero en conceptos-fundamentales esos identificadores `concepto-*` viven en §3 (decisiones de diseño), no en §2, y `concepto-proveedor-activo` no existe como identificador kebab (la fila de §4 es `proveedor-activo` a secas). Referencia cruzada colgada (H-02, P2).

---

## 6. Hallazgos

### geovial-storage

| ID | Nivel | Archivo / Sección | Evidencia | Recomendación |
| --- | --- | --- | --- | --- |
| H-01 | P2 | `09_devops/pipeline-ci-cd_v1.0.md` §1 y §8 | De las NFR numéricas de 05 §8, solo NFR-01 (latencia, STAGE-08) y NFR-06 (cobertura, STAGE-05) tienen stage explícito. NFR-02 (tamaño máximo configurable, 25 MB) no tiene stage ni gate en el pipeline, pese a ser objetivo numérico de 05. La DoD §1.4 lo cubre vía TC-26, pero el pipeline no lo surfacea. | Agregar un check de límite de tamaño (TC-26) como criterio de un stage existente (p. ej. en STAGE-03/STAGE-04) o una fila en §8 de trazabilidad que ligue NFR-02 a un gate, para cumplir "cada NFR numérico con stage que lo verifica" (09_rules §6). |
| H-02 | P2 | `10_developer_guide/glosario-tecnico_v1.0.md` §2 | Cita `conceptos-fundamentales §2 (concepto-proveedor-activo)` y `§2 (concepto-abstraccion)`; en el origen esos `concepto-*` están en §3 y `concepto-proveedor-activo` no existe como identificador. | Corregir el ancla de sección (§3) y reemplazar `concepto-proveedor-activo` por el identificador real (o agregar el identificador en conceptos-fundamentales §4 si se quiere conservar el enlace). |
| H-03 | P2 | `09_devops/README.md` cabecera | La cabecera usa `Tipo (D8)` y `Variante` en lugar del bloque §4.1 de 09_rules: faltan los campos `**Documento:**` y `**Versión:**`. | Alinear la cabecera al bloque uniforme §4.1 (Proyecto / Documento / Versión / Estado / Fecha / Autor). El README de 10 del mismo proyecto sí los lleva; replicar ese formato. |
| H-04 | P3 | `10_developer_guide/*` cabeceras | Estado `Borrador` en los 7 docs de 10, mientras el upstream 05/08 está en `Propuesto` y el par aplicada-sync/10 está en `Vigente`. | Reconciliar el estado con el ciclo de vida real del proyecto y con el estado del upstream (ver H-06). |

### aplicada-sync

| ID | Nivel | Archivo / Sección | Evidencia | Recomendación |
| --- | --- | --- | --- | --- |
| H-05 | P3 | `10_developer_guide/*` cabeceras | Estado `Vigente` en los 6 docs versionables de 10, mientras el contrato upstream que documentan (`05/contratos-abstractions_v1.0.md`, `05/extensibilidad`) y la DoD de 08 están en `Propuesto`, y el propio 09 del proyecto está en `Propuesto`. Un doc "Vigente" que documenta un contrato aún "Propuesto" es prematuro. | Bajar el estado de 10 a `Propuesto`/`Aprobado` hasta que el contrato de 05 esté `Aprobado`/`Vigente`, o promover coordinadamente toda la cadena. |

### Consolidado (transversal)

| ID | Nivel | Alcance | Evidencia | Recomendación |
| --- | --- | --- | --- | --- |
| H-06 | P3 | Ambos proyectos, ambas categorías | Tres estados conviven sin criterio común: aplicada-sync/10 = `Vigente`, geovial-storage/10 = `Borrador`, todo 09 y todo el upstream 05/08 = `Propuesto`. Es la inconsistencia de estados anticipada en el alcance. | Definir y aplicar un único criterio de estado por fase/ola de generación; al menos, que ningún documento downstream esté en un estado más avanzado que su upstream directo. |

---

## 7. Verificación de criterios bloqueantes (ausencia de P0)

| Condición P0 | aplicada-sync | geovial-storage |
| --- | --- | --- |
| Documento obligatorio omitido (incl. guía de publicación de aplicada-sync) | No — guía de publicación presente | No — omisión justificada (no redistribuible) |
| Confunde canales con ambientes en library | No | No |
| Slug comercial hardcodeado en 10 | No (`aplicacion-movil`) | No (`servicio-backend`) |
| Stack/vocabulario del dominio fuente prohibido | No | No |
| Rompe trazabilidad 05/08 | No | No (gap NFR-02 es P2, no rompe trazabilidad: la DoD lo cubre) |
| Falta cabecera/secciones obligatorias | No | Cabecera de README/09 incompleta (P2, no P0: es un README, recomendado) |
| Filename de 10 sin sufijo `_v1.0.md` | No | No |
| Viola D1-D8 | No | No |

No se identifica ninguna condición P0 en ninguno de los dos proyectos.

---

## 8. Veredicto

### aplicada-sync

**APROBADO CON OBSERVACIONES.** Las dos categorías cumplen §6 de sus reglas. La guía de publicación redistribuible existe y es correcta; el modelo de canales es el adecuado al tipo `library`; la paridad 10↔05 es estricta; los gates ejecutan la DoD de 08 sin redefinirla y las 6 NFR numéricas tienen verificación (G7 + G6). Única observación: P3 de estado de cabecera (H-05/H-06). Sin P0 ni P1. Habilitado para avanzar.

### geovial-storage

**APROBADO CON OBSERVACIONES.** Las dos categorías cumplen §6. La omisión de la guía de publicación está correctamente justificada; el documento de entornos declara de forma explícita que no hay feed ni ambientes propios (evita el anti-patrón); la paridad 10↔05 es estricta. Observaciones: H-01 (NFR-02 sin stage, P2), H-02 (cross-ref colgada en glosario, P2), H-03 (cabecera de README de 09 fuera del bloque §4.1, P2), H-04/H-06 (estado, P3). Sin P0 ni P1. Habilitado para avanzar; se recomienda resolver los P2 antes de promover el estado de la categoría.

### Consolidado

**APROBADO CON OBSERVACIONES (sin P0).** Ambos proyectos de nivel 0 superan la Fase F. Conteo total: **P0: 0 — P1: 0 — P2: 4 — P3: 3.** Cualquier P0 habría obligado a RECHAZADO; no hay ninguno. Se habilita el avance. Las recomendaciones P2 (sobre todo H-01, por ser de trazabilidad NFR→gate) deberían cerrarse en la próxima iteración del pipeline de geovial-storage; los P3 de estado se reconcilian en una pasada de consistencia transversal.

---

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Auditoría inicial de Fase F (09_devops + 10_developer_guide) de los proyectos de nivel 0 aplicada-sync y geovial-storage. Matrices D1-D8 y de estructura por categoría, coherencia cross-doc, 6 hallazgos (4 P2, 3 P3; sin P0/P1 — H-04 y H-06 comparten el eje de estado), veredicto APROBADO CON OBSERVACIONES por proyecto y consolidado. |
