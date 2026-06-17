# Auditoría Fase D — Backlog técnico (06) y Plan de sprint (07) — Nivel 2 (geovial-web, geovial-mobile)

**Fase:** D (Backlog técnico y plan de iteración)
**Proyectos auditados:** geovial-web (`web-monolith`, equipo_n=1) y geovial-mobile (`mobile-app-maui`, equipo_n=1)
**Categorías:** 06_backlog-tecnico (modo inline en ambos) y 07_plan-sprint (modo mini-plan en ambos)
**Auditor:** Arquitecto de Soluciones + QA Senior (independiente, sin participación en la generación)
**Fecha:** 2026-06-16
**Reglas aplicadas:** `06_rules_backlog_tecnico.md` (§6; piso BT web-monolith ≥ 8, mobile-app-maui ≥ 10), `07_rules_plan_sprint.md` (§6, modo mini-plan equipo_n=1).
**Insumos upstream consultados:** 00 (`roadmap-producto_v1.0.md`), 01 (NB), 02 (CU/RN por proyecto) y 05 (ADR por proyecto). geovial-web: CU-01..11, RN-01..05, ADR-01..05. geovial-mobile: CU-01..07, RN-01..05, ADR-01..05.
**Alcance de lectura:** las cuatro piezas de 06 (product-backlog, backlog-tecnico, definition-of-ready, README) y las dos de 07 (mini-plan, README) de cada proyecto, íntegras. Verificación mecánica de IDs, encoding, finales de línea, referencias colgantes y aritmética de SP recomputada ítem a ítem.

---

## 1. Resumen ejecutivo

Ambos proyectos entregan el conjunto documental obligatorio de la Fase D completo: 06 con `product-backlog_v1.0.md`, `backlog-tecnico_v1.0.md`, `definition-of-ready_v1.0.md` y `README.md`; 07 únicamente con `mini-plan_v1.0.md` y su `README.md`. Ninguno generó los cuatro artefactos de sprint indebidos para equipo_n=1 (plan-iteracion-sprint-XX, template-sprint-review, template-sprint-retrospectiva, velocidad-equipo). Ambos operan en modo inline correcto (web 18 US / 14 BT; mobile 15 US / 13 BT; los dos por debajo de los umbrales de 20 US y 30 BT, sin subcarpetas). Los pisos de BT por tipo se cumplen: web 14 ≥ 8 (web-monolith), mobile 13 ≥ 10 (mobile-app-maui).

La trazabilidad es sólida y bidireccional en los dos proyectos: cobertura completa de CU (web CU-01..11, mobile CU-01..07) sin US huérfanas; cada BT con fuente upstream y al menos una US consumidora o justificación de infraestructura compartida con ADR; los IDs de US y BT de cada mini-plan referencian exactamente los del backlog (web 18 US + 14 BT cubiertos una sola vez; mobile 15 US + 13 BT cubiertos una sola vez). Cero referencias colgantes: web CU ≤ 11 / ADR ≤ 05 / RN ≤ 05; mobile CU ≤ 07 / ADR ≤ 05 / RN ≤ 05. Sin IDs de tres dígitos (la única aparición de `BT-001` es metalingüística, cita del antipatrón heredado para prohibirlo), sin patrón `.v`, sin doble separador `plan-iteracion_sprint-`, sin `--` antes del H1. Sin stacks concretos (.NET, Blazor, MAUI, SQLite, JWT, Leaflet, Android, iOS) ni vocabulario del dominio fuente del bootstrap (Motor DSL). El token D8 (`web-monolith` / `mobile-app-maui`) aparece solo en descriptores de tipo de documento y en referencias a reglas, conforme.

El hallazgo más relevante es la **inconsistencia aritmética de story points del `product-backlog_v1.0.md` de geovial-web** anticipada por ambos generadores. Se confirma y se cuantifica (H-WEB-01, P2): la §4 declara total 81 SP, 10 Must / 6 Should / 2 Could y MoSCoW SP 60 / 20 / 8, mientras la suma real ítem a ítem de las tablas por épica es **90 SP, 11 Must / 5 Should / 2 Could y MoSCoW SP 65 / 17 / 8**. El defecto se propaga al `README.md` (06) de geovial-web (H-WEB-02, P3). No rompe trazabilidad (todas las US y BT existen, se referencian y se cubren), por lo que se clasifica P2/P3 y no P0. El `product-backlog` de geovial-mobile, en cambio, reconcilia exactamente (15 US, 61 SP, 9/4/2, 44/12/5) y el mini-plan de geovial-web usa internamente la suma correcta de 90 SP para las US, dejando el defecto acotado a la §4 del product-backlog web y a su README.

Conteo de hallazgos consolidado: **P0 = 0 | P1 = 0 | P2 = 2 | P3 = 4.**

| Proyecto / categoría | P0 | P1 | P2 | P3 | Veredicto |
| --- | --- | --- | --- | --- | --- |
| geovial-web — 06_backlog-tecnico | 0 | 0 | 1 | 2 | APROBADO CON OBSERVACIONES |
| geovial-web — 07_plan-sprint (mini-plan) | 0 | 0 | 0 | 1 | APROBADO CON OBSERVACIONES |
| geovial-mobile — 06_backlog-tecnico | 0 | 0 | 1 | 1 | APROBADO CON OBSERVACIONES |
| geovial-mobile — 07_plan-sprint (mini-plan) | 0 | 0 | 0 | 1 | APROBADO CON OBSERVACIONES |
| **Consolidado nivel 2** | **0** | **0** | **2** | **4** | **APROBADO CON OBSERVACIONES** |

Sin P0 ni P1: ambos proyectos pueden promover a la fase siguiente.

---

## 2. Matriz D1-D8 por documento

Leyenda: OK = conforme; Obs = observación menor (ver hallazgos). D1 idioma rioplatense; D2 encoding UTF-8/LF; D3 kebab-case en nombre de archivo; D4 versionado `_vX.Y` (nunca `.v`); D5 sin stacks concretos; D6 sin vocabulario del dominio fuente del bootstrap (Motor DSL); D7 IDs de dos dígitos uniformes; D8 conjunto cerrado de documentos según modo y uso del token de tipo solo en cabeceras/referencias a reglas.

### 2.1 geovial-web

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 06/product-backlog_v1.0.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 06/backlog-tecnico_v1.0.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 06/definition-of-ready_v1.0.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 06/README.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 07/mini-plan_v1.0.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 07/README.md | OK | Obs | OK | OK | OK | OK | OK | OK |

### 2.2 geovial-mobile

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 06/product-backlog_v1.0.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 06/backlog-tecnico_v1.0.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 06/definition-of-ready_v1.0.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 06/README.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 07/mini-plan_v1.0.md | OK | Obs | OK | OK | OK | OK | OK | OK |
| 07/README.md | OK | Obs | OK | OK | OK | OK | OK | OK |

Notas de la matriz:

- **D1:** idioma rioplatense técnico, tildes correctas, sin emojis, en los doce archivos.
- **D2 (Obs en todos):** los doce archivos están en UTF-8 sin BOM (correcto), pero el working tree presenta finales de línea CRLF en lugar de LF. Condición de tooling repo-wide con `core.autocrlf=true` en este checkout Windows; los archivos están sin trackear en git al momento del audit. El `.gitattributes` del repositorio fija `*.md text eol=lf`, por lo que al commitear los blobs se normalizan a LF de forma determinística. Se documenta como reconciliación P3 (H-COM-01), no como defecto de contenido ni bloqueante.
- **D3/D4:** nombres `product-backlog_v1.0.md`, `backlog-tecnico_v1.0.md`, `definition-of-ready_v1.0.md`, `mini-plan_v1.0.md`, `README.md`; kebab-lowercase, guion bajo antes de la versión, sin ningún `.v`.
- **D5:** sin menciones a stacks concretos (verificado por barrido sobre los doce archivos: ningún `.NET`, `Blazor`, `MAUI`, `SQLite`, `JWT`, `Leaflet`, `Android`, `iOS`, ORM nombrado, etc.). El vocabulario abstracto presente —"render del lado servidor con circuito interactivo", "token custodiado del lado servidor", "almacén local", "almacenamiento seguro del dispositivo", "componente de mapa de terceros", "paquete de la app", "canal de distribución interno"— describe capacidades sin fijar tecnología; el web incluso difiere el formato físico del token y la mobile difiere el producto de almacén. Conforme.
- **D6:** sin rastros del dominio fuente del bootstrap (Motor DSL/DSL). La única aparición de `BT-001` (mobile 06/README §"Convenciones") es metalingüística (cita el antipatrón heredado para prohibirlo). Conforme.
- **D7:** web US-01..18, BT-01..14, EP-01..07, EP-T1..T6 con dos dígitos uniformes; mobile US-01..15, BT-01..13, EP-01..06, ET-01..06. Sin IDs de tres dígitos reales. El prefijo de épica técnica difiere del `EP-XX` estricto del §3.2 (web `EP-T`, mobile `ET-`); se trata como observación de prefijo (H-COM-02, P3), coherente con el criterio aplicado en niveles previos, sin afectar la unicidad de dos dígitos de los identificadores numéricos.
- **D8:** conjunto cerrado correcto en ambos. 06 con los cuatro documentos obligatorios sin subcarpetas (inline correcto). 07 con solo `mini-plan_v1.0.md` + README, sin ninguno de los cuatro artefactos de sprint. El token de tipo `web-monolith` / `mobile-app-maui` aparece solo en descriptores de tipo de documento ("tipo web-monolith", "**Tipo (D8):** mobile-app-maui") y en referencias a reglas (§2.2, §3.2). Conforme.

---

## 3. Matriz de estructura obligatoria

### 3.1 geovial-web — 06 backlog-tecnico

| Documento | Cabecera | Secciones obligatorias (§4) | Resultado |
| --- | --- | --- | --- |
| product-backlog | OK (H1 + metadatos) | 1 Objetivos, 2 Épicas (EP-01..07, dos dígitos), 3 Historias por épica (18 US con detalle GWT de Must/Should), 4 Métricas, 5 Refinamiento | Completo (5/5) — §4 con inconsistencia aritmética (H-WEB-01) |
| backlog-tecnico | OK | 1 Épicas técnicas (EP-T1..T6), 2 BT por épica (14 BT inline), 3 Matriz BT↔US↔CU | Completo (3/3) |
| definition-of-ready | OK | 1 DoR US (7 criterios, rango 5-8 OK), 2 DoR BT (5 criterios, rango 4-6 OK), 3 Excepciones, 4 Aprobador (Scrum Master) | Completo |
| README | OK | Artefactos, modo, épicas, US Must, BT prioritarias, DoR | Completo — replica el conteo MoSCoW inconsistente (H-WEB-02) |

- **BT mínimas:** 14 ≥ 8 (piso web-monolith §2.2). OK.
- **Umbral inline:** 18 US < 20 y 14 BT < 30 → inline, sin `historias-usuario/` ni `tareas-tecnicas/`. Aplicado correctamente.
- **US con ≥1 CU (cobertura bidireccional):** cada US declara su CU en la tabla por épica; CU-01..11 todos cubiertos por al menos una US (CU-01 por US-01/US-02; CU-02 por US-03/US-04; CU-03 por US-05/US-06; CU-04 por US-08/US-09; CU-05 por US-07; CU-06 por US-10/US-11; CU-07 por US-12/US-14; CU-08 por US-13/US-14; CU-09 por US-15/US-16; CU-10 por US-17; CU-11 por US-18). Sin US huérfana. OK.
- **Cada BT con fuente upstream y ≥1 US consumidora:** la matriz §3 declara, por BT, US consumidoras, CU upstream y fuente principal (ADR/componente/CU/RN). Las BT de cimientos (BT-01, BT-02, BT-03) se justifican como infraestructura compartida con ADR-01/02/04 además de declarar US que las ejercitan. OK.
- **US Must/Should con Given/When/Then (≥2 escenarios):** verificado en las 16 US Must/Should (todas con 2 a 4 escenarios, happy path + edge case). Las 2 Could (US-17, US-18) traen un escenario de referencia, conforme al §4.8 y a la excepción de la DoR. OK.
- **MoSCoW no 100% Must:** hay reparto entre Must, Should y Could. OK (el conteo declarado en §4 es inexacto; ver H-WEB-01).

### 3.2 geovial-web — 07 mini-plan (equipo_n=1)

| Aspecto del §6 mini-plan | Resultado |
| --- | --- |
| Existe `mini-plan_v1.0.md` | OK |
| NO existen plan-iteracion-sprint-XX, templates review/retro, velocidad-equipo | OK (omisión justificada en §1 del mini-plan y §1 del README) |
| Objetivo en una sola frase | OK — §2 es una única frase orientada a valor, sin bullets |
| DoD por referencia a 08 | OK — §4 referencia la DoD canónica de 08 (pendiente) sin redefinirla; agrega criterio de cierre de tramo |
| Trazabilidad a CU y NB | OK — §5 tabla por tramo con CU-01..11 y NB-01/02/05/06/07, más ADR gobernantes |
| ≥2 riesgos | OK — 4 riesgos con probabilidad/impacto y mitigación concreta |
| Nomenclatura sin doble separador | OK — `mini-plan_v1.0.md` |
| Sin `--` antes del H1 | OK — H1 directo |

### 3.3 geovial-mobile — 06 backlog-tecnico

| Documento | Cabecera | Secciones obligatorias (§4) | Resultado |
| --- | --- | --- | --- |
| product-backlog | OK | 1 Objetivos, 2 Épicas (EP-01..06), 3 Historias por épica (15 US con detalle GWT), 4 Métricas, 5 Refinamiento | Completo (5/5) — §4 reconciliada |
| backlog-tecnico | OK | 1 Épicas técnicas (ET-01..06), 2 BT por épica (13 BT inline), 3 Matriz BT↔US↔CU + cobertura por CU | Completo (3/3) |
| definition-of-ready | OK | 1 DoR US (7 criterios), 2 DoR BT (5 criterios), 3 Excepciones, 4 Aprobador, 5 Relación con DoD | Completo |
| README | OK | Documentos, épicas EP/ET, US Must, BT prioritarias, DoR, convenciones, trazabilidad | Completo |

- **BT mínimas:** 13 ≥ 10 (piso mobile-app-maui §2.2). OK.
- **Umbral inline:** 15 US < 20 y 13 BT < 30 → inline correcto.
- **US con ≥1 CU (cobertura bidireccional):** CU-01..07 todos cubiertos (matriz §3 y tabla "Cobertura por CU"); sin US huérfana. OK.
- **Cada BT con fuente upstream y ≥1 US consumidora:** matriz §3 completa; BT-01, BT-02, BT-11 justificadas como infraestructura compartida con ADR-02/ADR-03 además de declarar US consumidoras. OK.
- **US Must/Should con Given/When/Then (≥2 escenarios):** verificado en las 13 US Must/Should (2 a 4 escenarios). Las 2 Could (US-10, US-15) con un escenario, conforme a la excepción de la DoR §3. OK.
- **MoSCoW no 100% Must:** 9 Must (60 %) / 4 Should (27 %) / 2 Could (13 %). Reparto dentro de los rangos sugeridos. OK.

### 3.4 geovial-mobile — 07 mini-plan (equipo_n=1)

| Aspecto del §6 mini-plan | Resultado |
| --- | --- |
| Existe `mini-plan_v1.0.md` | OK |
| NO existen los cuatro artefactos de sprint | OK (omisión justificada en §intro y README) |
| Objetivo en una sola frase | OK — §1 es una única frase orientada a valor |
| DoD por referencia a 08 | OK — §4 referencia la DoD canónica de 08 (pendiente) con criterio provisional explícito sin sustituirla |
| Trazabilidad a CU y NB | OK — §5 tabla por tramo con CU-01..07, NB-01/03/04 y ADR gobernantes |
| ≥2 riesgos | OK — 4 riesgos con probabilidad/impacto y mitigación concreta |
| Nomenclatura sin doble separador | OK — `mini-plan_v1.0.md` |
| Sin `--` antes del H1 | OK — H1 directo |

---

## 4. Coherencia cross-doc

### 4.1 geovial-web

- **IDs del mini-plan ↔ backlog:** las 18 US (US-01..18) y 14 BT (BT-01..14) del mini-plan existen idénticas en product-backlog y backlog-tecnico. Cada ítem aparece exactamente una vez entre los cinco tramos; verificado mecánicamente (32 ítems distintos, sin duplicados ni faltantes). Sin invención de IDs. OK.
- **US ↔ CU de 02:** cada US referencia CU reales; los 11 CU existen como archivos en 02/casos-de-uso (CU-01..CU-11). Sin huérfanas. OK.
- **BT ↔ ADR/CU de 05/02:** las BT referencian ADR-01..05 (las cinco existen en 05/adrs), componentes de arquitectura, CU-01..11 y RN-01..05. OK.
- **Alineación con el roadmap (00):** el Tramo 1 materializa el walking skeleton de la fase F0 (autenticación y administración de usuarios de punta a punta), correcto para el front administrativo. Los rótulos de fase por tramo (T1=F0, T2=F0/F1, T3=F1, T4=F3, T5=F2(web)/F3) respetan el orden topológico del roadmap; el orden F3 antes de F2 en los rótulos es coherente con la nota del roadmap §2 (la recolección F2 puede entregarse antes que la resolución de conflictos F3 sin romper el camino, y el front no realiza la captura de terreno). Alineación correcta. OK.
- **Aritmética de SP:** ver §5. El defecto está acotado a la §4 del product-backlog y su README; el mini-plan §3 usa la suma correcta de 90 SP para las US y los subtotales de tramo reconcilian exactamente (44 + 21 + 27 + 45 + 34 = 171 = 81 BT + 90 US).
- **DoR ↔ DoD de 08:** la DoR (06 §5) delimita su frontera con la DoD de 08 sin solaparse (cuándo empezar vs cuándo terminar). OK.

### 4.2 geovial-mobile

- **IDs del mini-plan ↔ backlog:** las 15 US (US-01..15) y 13 BT (BT-01..13) del mini-plan existen idénticas en product-backlog y backlog-tecnico; cada ítem aparece una sola vez entre los tres tramos (28 ítems distintos, sin duplicados ni faltantes). OK.
- **US ↔ CU de 02:** cada US referencia CU reales; los 7 CU existen como archivos en 02/casos-de-uso (CU-01..CU-07). Sin huérfanas. OK.
- **BT ↔ ADR/CU de 05/02:** las BT referencian ADR-01..05 (las cinco existen), modelo lógico del almacén local, flujo de ejecución, contrato consumido de la librería de sincronización, CU-01..07 y RN-01..05. OK.
- **Alineación con el roadmap (00):** el mini-plan ubica geovial-mobile en la fase F2 (captura en campo y sincronización), capacidades F-05/F-07/F-09 del roadmap §2; el primer tramo (esqueleto de sesión y almacén local) es la base de su propia capacidad de campo y el cierre de los tres tramos materializa los criterios verificables de transición que habilitan F2 (captura offline con foto y coordenadas, carga manual con radio, sincronización subir-luego-bajar). Alineación correcta. Observación menor de redacción: el README §"Fase de roadmap" rotula esos criterios como "F1→F2" mientras el mini-plan los nombra como "criterios de transición de la fase F2"; ambas lecturas apuntan a la misma fila del roadmap §5 (los criterios que la capacidad F2 satisface), pero la doble rotulación puede confundir (H-MOB-02, P3). No afecta trazabilidad.
- **Aritmética de SP:** reconciliación exacta en los tres niveles. product-backlog §4: 15 US, 61 SP, Must 9/44, Should 4/12, Could 2/5 — coincide con la suma ítem a ítem y con el README ("44 SP en 9 US Must"). mini-plan §3.4: BT 62 SP + US 61 SP = 123 SP; subtotales de tramo 28 + 63 + 32 = 123, cada uno recomputado coincide. Sin inconsistencia. OK.
- **DoR ↔ DoD de 08:** la DoR (06 §5) delimita explícitamente su frontera con la DoD de 08. OK.

---

## 5. Chequeo específico de aritmética de SP

Verificación solicitada: ambos generadores reportaron métricas de story points inconsistentes en el `product-backlog_v1.0.md` de geovial-web (total declarado 81 vs suma ítem-a-ítem 90, y MoSCoW que no cuadra). Se recomputó la suma real desde las tablas por épica de cada product-backlog.

### 5.1 geovial-web — INCONSISTENCIA CONFIRMADA (P2)

Suma real de las 18 US desde las tablas de §3 (por épica):

| Dimensión | Declarado en §4 (y README) | Suma real ítem a ítem | ¿Cuadra? |
| --- | --- | --- | --- |
| Total de US | 18 | 18 | Sí |
| Total de SP | **81** | **90** | **No (−9)** |
| Cantidad Must / Should / Could | **10 / 6 / 2** | **11 / 5 / 2** | **No (Must y Should)** |
| SP Must / Should / Could | **60 / 20 / 8** | **65 / 17 / 8** | **No (Must y Should)** |

Detalle del recuento real de Must (11 US): US-01 (5), US-02 (3), US-03 (5), US-05 (5), US-07 (8), US-08 (5), US-10 (8), US-12 (8), US-13 (5), US-14 (5), US-15 (8) = 65 SP. Should (5 US): US-04 (3), US-06 (3), US-09 (3), US-11 (3), US-16 (5) = 17 SP. Could (2 US): US-17 (5), US-18 (3) = 8 SP. Total 90 SP.

La narrativa reconciliadora de §4 es además internamente incoherente: afirma "la suma de SP por prioridad es 60 + 20 + 8 = 88; el total de 81 SP no se obtiene de sumar las tres filas porque US-14 traza a dos CU pero es una sola historia de 5 SP contada una sola vez". Esta explicación no cierra por tres motivos: (a) 60 + 20 + 8 = 88, que no reconcilia a 81; (b) las celdas reales por MoSCoW son 65 / 17 / 8 = 90, no 60 / 20 / 8; (c) que US-14 trace a dos CU no afecta la suma de SP, porque cada US ya se cuenta una sola vez en la suma de puntos. La métrica derivada "MVP (historias Must) 10 US, 60 SP" hereda el mismo error: las Must reales son 11 US, 65 SP.

Severidad **P2**: es una inconsistencia de la tabla de métricas §4 (reconciliación aritmética). No rompe trazabilidad —las 18 US existen, se referencian, se cubren bidireccionalmente y el mini-plan usa la suma correcta de 90—, por lo que no califica como P0. Recomendación: corregir la §4 a Total 90 SP, Must 11 / Should 5 / Could 2, SP 65 / 17 / 8 (participaciones 72,2 % / 18,9 % / 8,9 %), eliminar la narrativa de "US-14 con dos CU" y actualizar la métrica del MVP a 11 US / 65 SP. Registrado como H-WEB-01.

### 5.2 geovial-web README (06) — PROPAGACIÓN (P3)

El `README.md` de 06 replica el conteo inconsistente: §4 declara "El MVP queda definido por las 10 historias Must (EP-01 a EP-06), 60 SP", pero la propia tabla de §4 lista **11 filas Must que suman 65 SP** (US-01, US-02, US-03, US-05, US-07, US-08, US-10, US-12, US-13, US-14, US-15). El pie de §4 repite "10 Must, 6 Should, 2 Could sobre 18 historias y 81 SP". Contradicción interna entre la tabla (11 / 65) y la prosa (10 / 60 / 81). Severidad **P3** (propagación de estilo/conteo derivada de H-WEB-01). Registrado como H-WEB-02.

### 5.3 geovial-mobile — SIN INCONSISTENCIA

Recomputado por las dudas: la §4 del product-backlog de geovial-mobile reconcilia exactamente con la suma ítem a ítem (15 US, 61 SP; Must 9 US / 44 SP; Should 4 US / 12 SP; Could 2 US / 5 SP). El README ("44 SP en 9 US Must") y el mini-plan (US 61 SP, BT 62 SP, total 123) son coherentes. No hay hallazgo de aritmética en geovial-mobile.

### 5.4 Referencias colgantes

Barrido exhaustivo de identificadores fuera de rango sobre los seis archivos de Fase D de cada proyecto.

| Proyecto | CU válido | máx CU ref | ADR válido | máx ADR ref | RN máx ref | Fuera de rango |
| --- | --- | --- | --- | --- | --- | --- |
| geovial-web | CU-01..11 | CU-11 | ADR-01..05 | ADR-05 | RN-05 | Ninguno |
| geovial-mobile | CU-01..07 | CU-07 | ADR-01..05 | ADR-05 | RN-05 | Ninguno |

No se detectó ningún `CU-12+` ni `ADR-06+` en web, ni `CU-08+` ni `ADR-06+` en mobile, ni `RN-06+` en ninguno. La única ocurrencia de `BT-001` (mobile README) es metalingüística. **Cero referencias colgantes** en la Fase D de nivel 2.

---

## 6. Hallazgos

Nivel / archivo / sección / evidencia / recomendación.

**H-WEB-01 (P2) — Métricas de SP inconsistentes en el product-backlog de geovial-web.**
- Archivo: `geovial-web/06_backlog-tecnico/product-backlog_v1.0.md` §4 (Métricas de avance).
- Evidencia: la §4 declara total 81 SP, conteo 10 Must / 6 Should / 2 Could y SP MoSCoW 60 / 20 / 8, mientras la suma real de las 18 US en las tablas de §3 es 90 SP, 11 Must / 5 Should / 2 Could y 65 / 17 / 8. La narrativa de reconciliación ("60 + 20 + 8 = 88… US-14 traza a dos CU") es internamente incoherente (88 ≠ 81; las celdas reales son 65/17/8; el doble CU no afecta la suma de puntos). La métrica derivada del MVP ("10 US, 60 SP") hereda el error (real: 11 US, 65 SP).
- Recomendación: rehacer la tabla §4 a Total 90 SP / Must 11 (65) / Should 5 (17) / Could 2 (8); eliminar la frase de reconciliación; corregir la métrica del MVP a 11 US / 65 SP. No bloqueante: no afecta trazabilidad ni el mini-plan, que ya usa 90 SP.

**H-WEB-02 (P3) — Propagación del conteo inconsistente al README de geovial-web.**
- Archivo: `geovial-web/06_backlog-tecnico/README.md` §4.
- Evidencia: el §4 afirma "10 historias Must, 60 SP" y "10 Must, 6 Should, 2 Could… 81 SP", pero su propia tabla lista 11 filas Must que suman 65 SP. Contradicción interna tabla vs prosa.
- Recomendación: alinear el README con la §4 corregida del product-backlog (11 Must / 65 SP; total 90 SP). No bloqueante.

**H-MOB-01 (P2) — Prefijo de épica técnica `ET-` en lugar de `EP-` en geovial-mobile.**
- Archivo: `geovial-mobile/06_backlog-tecnico/backlog-tecnico_v1.0.md` §1-§2 y README; (análogo en web con `EP-T`).
- Evidencia: el §3.2 de 06_rules fija `EP-XX` como prefijo de épica (incluidas las técnicas, según interpretación estricta). geovial-mobile usa `ET-01..ET-06` y geovial-web usa `EP-T1..EP-T6`. Las épicas funcionales sí usan `EP-XX` correctamente en ambos; los identificadores numéricos mantienen dos dígitos uniformes. La práctica de prefijo propio para la lente técnica ya fue observada en niveles previos como divergencia menor de convención.
- Recomendación: unificar el prefijo de épica técnica en una revisión transversal (adoptar `EP-XX` continuo o documentar `EP-TX`/`ET-XX` como convención explícita y consistente entre proyectos). No rompe trazabilidad ni la regla de dos dígitos; se mantiene en P2 de reconciliación de convención (sin impacto en P0/P1). Aplica también a geovial-web como observación equivalente.

**H-MOB-02 (P3) — Doble rotulación de fase de roadmap (F1→F2 vs fase F2) en geovial-mobile.**
- Archivo: `geovial-mobile/07_plan-sprint/mini-plan_v1.0.md` (§intro, §5) y `README.md` (§"Fase de roadmap").
- Evidencia: el mini-plan nombra "criterios de transición de la fase F2 (roadmap §5)" y el README los rotula "criterios de transición F1→F2". Ambas refieren a la misma fila del roadmap §5 (los criterios que la capacidad F2 satisface), pero la rotulación mixta puede confundir al lector sobre si la app entra o sale de F2.
- Recomendación: unificar la referencia (por ejemplo, "satisface los criterios de salida F1→F2 que habilitan la entrega de la fase F2"). No bloqueante.

**H-COM-01 (P3) — Finales de línea CRLF en el working tree (los doce archivos).**
- Archivo: los seis `.md` de Fase D de geovial-web y los seis de geovial-mobile.
- Evidencia: UTF-8 sin BOM (correcto) pero CRLF en el working tree, con `core.autocrlf=true` y los archivos sin trackear. El repositorio tiene `.gitattributes` con `*.md text eol=lf`, que normaliza a LF de forma determinística al commitear.
- Recomendación: confirmar la normalización a LF al commitear (la garantía ya existe vía `.gitattributes`). Reconciliación de tooling, no defecto de contenido. No bloqueante; clasificado P3 por la existencia de la garantía explícita de normalización.

**H-MOB-03 / H-WEB-03 (P3) — (Observación de legibilidad, no defecto) numeración de tramos sin objetivo de valor por tramo en el mini-plan web.**
- Archivo: `geovial-web/07_plan-sprint/mini-plan_v1.0.md` §3.1-§3.5.
- Evidencia: el objetivo único de valor (§2, una sola frase) cumple el §6. Cada tramo del mini-plan web trae un encabezado descriptivo y se alinea a una fase del roadmap, pero no enuncia un objetivo de valor por tramo en frase propia (a diferencia del mini-plan mobile, que sí abre cada tramo con un "Objetivo del tramo"). En modo mini-plan de 1 dev es admisible (no hay sprint goal por sprint).
- Recomendación: opcional, homogeneizar agregando una frase de valor por tramo en el mini-plan web, como ya hace el de mobile. No bloqueante. (Se cuenta como un único P3 de estilo en el consolidado.)

---

## 7. Veredicto

### 7.1 geovial-web

**APROBADO CON OBSERVACIONES.** Sin P0 ni P1. La Fase D de geovial-web (06 backlog-tecnico con 18 US y 14 BT inline; 07 mini-plan para equipo_n=1) está completa, es trazable y coherente: conjunto documental obligatorio completo; cobertura CU-01..11 sin huérfanas; cada BT con fuente upstream y US consumidora o justificación de infraestructura compartida; 14 BT ≥ 8 (piso web-monolith); IDs del mini-plan idénticos a los del backlog; primer tramo alineado al walking skeleton F0; cero referencias colgantes; sin `.v`, sin doble separador, sin `--`; sin stacks ni dominio fuente. Observaciones: 1 P2 (inconsistencia aritmética de SP en product-backlog §4, total 81 vs 90 real y MoSCoW 60/20/8 vs 65/17/8) y 2 P3 (propagación al README; CRLF/estilo). No bloquean.

### 7.2 geovial-mobile

**APROBADO CON OBSERVACIONES.** Sin P0 ni P1. La Fase D de geovial-mobile (06 con 15 US y 13 BT inline; 07 mini-plan para equipo_n=1) está completa, trazable y coherente: cobertura CU-01..07 sin huérfanas; matriz BT↔US↔CU y cobertura por CU completas; 13 BT ≥ 10 (piso mobile-app-maui); aritmética de SP reconciliada exactamente en product-backlog, README y mini-plan; IDs del mini-plan idénticos a los del backlog; alineación a la fase F2 del roadmap; cero referencias colgantes; MoSCoW 9/4/2 dentro de rangos. Observaciones: 1 P2 (prefijo de épica técnica `ET-` en lugar de `EP-`) y 1 P3 (doble rotulación F1→F2 / fase F2; CRLF compartido). No bloquean.

### 7.3 Consolidado nivel 2

**APROBADO CON OBSERVACIONES.** Conteo total: **P0 = 0, P1 = 0, P2 = 2, P3 = 4.** Ningún hallazgo bloqueante. Ambos proyectos pueden promover a la fase siguiente. Se recomienda atender H-WEB-01 (corrección de la tabla de métricas §4 del product-backlog de geovial-web y su README) y unificar en una revisión transversal el prefijo de épica técnica (H-MOB-01, aplicable también a geovial-web) y la normalización LF (H-COM-01).

---

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-16 | Informe inicial del audit independiente de Fase D (06 backlog-tecnico inline y 07 plan-sprint en modo mini-plan) de los dos proyectos de nivel 2: geovial-web (web-monolith, equipo_n=1) y geovial-mobile (mobile-app-maui, equipo_n=1). Matriz D1-D8 por documento, matriz de estructura, coherencia cross-doc, chequeo específico de aritmética de SP (inconsistencia confirmada en product-backlog §4 de geovial-web: 81 vs 90 SP, MoSCoW 60/20/8 vs 65/17/8 — P2; geovial-mobile reconciliado), chequeo de referencias colgantes (cero), 6 hallazgos (0 P0 / 0 P1 / 2 P2 / 4 P3) y veredicto APROBADO CON OBSERVACIONES por proyecto y consolidado. |
