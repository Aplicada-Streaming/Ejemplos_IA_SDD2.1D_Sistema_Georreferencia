# Auditoría independiente — Fase A (Fundamentos de la solución) — GeoVial

| Campo | Valor |
| --- | --- |
| Fase | A — Fundamentos de la solución |
| Alcance | Categorías de nivel solución `00_contexto` y `01_necesidades_negocio` de GeoVial (proyecto principal `geovial-api`, tipo `rest-api`) |
| Documento | A-fundamentos_v1.0.md |
| Versión | 1.0 |
| Auditor | Arquitecto de Soluciones + QA Senior (independiente; no participó de la generación) |
| Fecha | 2026-06-15 |
| Insumos de reglas | `00_rules_contexto.md` (v1.3, §6 con 11 ítems), `01_rules_necesidades_negocio.md` (v1.2, §6 con 14 ítems), `_root_rules.md`, `master-prompt.md` §10 |
| Fuentes de verdad | `SOLUTION-INTAKE-geovial_v1.0.md` (v1.2 vigente), `SOLUTION-MANIFEST-geovial_v1.0.md` (v1.0 Aprobado) |

---

## 1. Resumen ejecutivo

Se auditaron los 5 documentos de `00_contexto` (visión, alcance, roadmap, compatibilidad-plataformas, README) y los 9 de `01_necesidades_negocio` (índice maestro, README y 7 NB). Los entregables son sólidos: respetan D1-D8, mantienen el dominio de negocio sin filtrar stack en visión/alcance/roadmap/NB, cubren íntegramente F-01 a F-18, declaran trazabilidad coherente con el intake consolidado y cumplen los criterios numéricos de §6 de ambas reglas. No se detectaron hallazgos P0: ningún documento obligatorio falta, no hay vocabulario prohibido del bootstrap, todas las cabeceras y checklists están presentes y la trazabilidad D6 no se rompe.

Conteo de hallazgos: P0 = 0; P1 = 0; P2 = 1; P3 = 4. Veredicto: APROBADO CON OBSERVACIONES.

---

## 2. Matriz D1-D8 por documento

Convención: OK = conforme; n/a = no aplica al documento.

| Documento | D1 idioma | D2 UTF-8/LF | D3 kebab/filename | D4 versión `_vX.Y` | D5 estado/control cambios | D6 trazabilidad | D7 sin stack/vocab fuente | D8 conjunto cerrado |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| vision-producto_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| alcance-proyecto_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| roadmap-producto_v1.0.md | OK | OK | OK | OK | OK | OK (ver H-02) | OK | OK |
| compatibilidad-plataformas_v1.0.md | OK | OK | OK | OK | OK | OK | OK (stack legítimo aquí) | OK |
| 00_contexto/README.md | OK | OK | OK | n/a (sin versión, correcto) | OK | OK | Observación (ver H-01) | OK |
| necesidades-negocio_v1.0.md (índice) | OK | OK | OK | OK | OK | OK | OK | OK |
| 01_necesidades_negocio/README.md | OK | OK | OK | n/a (sin versión, correcto) | OK | OK | OK | OK |
| NB-01 … NB-07 (7 archivos) | OK | OK | OK (regex valida) | OK | OK | OK | OK | OK |

Verificaciones de respaldo:
- Encoding: `file` reporta UTF-8 en los 14 archivos; sin BOM; sin CR (LF puro). D2 conforme.
- Filenames NB: los 7 matchean `^NB-\d{2}-[a-z0-9-]+_v\d+\.\d+\.md$`; no se halló el anti-patrón `.v` con punto. D3/D4 conformes.
- Sin emojis en ningún documento. Las flechas `→` aparecen solo en los grafos DAG de dependencias (uso técnico legítimo, no decorativo).
- Negritas: los documentos de `00_contexto` usan `**Campo:**` exclusivamente en el bloque de cabecera prescripto por `00_rules` §4.1 (8 marcadores = 8 campos de cabecera); no hay negrita decorativa en el cuerpo. Las NB usan cabecera en tabla (sin negrita), conforme a `01_rules` §4.1.
- D7 matiz: el stack real (.NET, Linux, Android, navegadores, contenedor) aparece únicamente en `compatibilidad-plataformas` (legítimo) y, como referencia de propósito, en el README de la sección 00 (ver H-01). Visión, alcance, roadmap y las 7 NB están limpias de stack (scan con límites de palabra: sin coincidencias de `.NET`, `blazor`, `maui`, `sqlite`, `sql server`, `android`, `ios`, `jwt`, `ropc`, `s3`, `leaflet`, `signalr`, `docker`, `exif`, `gps`, `zip`). No hay vocabulario del dominio fuente del bootstrap (impresora térmica, ESC-POS, DSL, Bluetooth, NuGet) en ningún entregable de la fase.

---

## 3. Matriz de estructura obligatoria por documento

### 3.1 `00_contexto` (cabecera §4.1 + secciones §4.2)

| Documento | Cabecera completa | Secciones obligatorias | Resultado |
| --- | --- | --- | --- |
| vision-producto | OK (8 campos, upstream+downstream) | §1 a §10 presentes y en orden | Completo |
| alcance-proyecto | OK | §1 a §10 presentes (incluida §5 con 3 exclusiones justificadas y §8 con criterios `- [ ]`) | Completo |
| roadmap-producto | OK | §1 a §6 presentes (4 fases, matriz fase→épica→sprint→release, dependencias, criterios `- [ ]`, trazabilidad) | Completo |
| compatibilidad-plataformas | OK | §1 a §6 presentes (resumen, matriz, restricciones, alternativas, estado, trazabilidad) | Completo |
| README sección 00 | OK (proyecto, fecha, autor) | Tabla de documentos, orden de lectura, stakeholders, nota de omisión de `acuerdo-equipo` | Completo |

### 3.2 `01_necesidades_negocio` (índice + README + NB §4.2: 10 secciones)

| Documento | Cabecera | §1 | §2 | §3 | §4 | §5 (≥4 SMART) | §6 (≥3 stk) | §7 (CU `a generar`) | §8 (deps ≤3, acíclicas) | §9 (MoSCoW) | §10 (control cambios) |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Índice necesidades-negocio | OK (incluye Cantidad de NB y Versión del catálogo) | Propósito | Tabla resumen 7 NB | Mapa dependencias DAG | Trazabilidad agregada | n/a | n/a | n/a | n/a | n/a | Control de cambios |
| README sección | OK | Tabla NB | Mapa deps | Orden lectura | RACI | — | — | — | — | — | Control de cambios |
| NB-01 | OK | OK | OK | OK | OK | 5 criterios | 4 (P/I/B) | CU-01..03 | Sin deps | Must | OK |
| NB-02 | OK | OK | OK | OK | OK | 4 | 4 (P/I/B) | CU-04..06 | dep NB-01 | Must | OK |
| NB-03 | OK | OK | OK | OK | OK | 4 | 4 (P/I/B) | CU-07..09 | dep NB-02 | Must | OK |
| NB-04 | OK | OK | OK | OK | OK | 4 | 4 (P/I/B) | CU-10..11 | dep NB-03 | Must | OK |
| NB-05 | OK | OK | OK | OK | OK | 4 | 4 (P/I/B) | CU-12..14 | dep NB-03,04 (2) | Must | OK |
| NB-06 | OK | OK | OK | OK | OK | 4 | 4 (P/I/B) | CU-15..16 | dep NB-05 | Could | OK |
| NB-07 | OK | OK | OK | OK | OK | 4 | 4 (P/I/B) | CU-17 | dep NB-03 | Could | OK |

Todas las NB tienen las 10 secciones obligatorias en el orden de `01_rules` §4.2, ≥4 criterios SMART con métrica numérica + unidad + plazo, mínimo 3 stakeholders cubriendo propietario/implementador/beneficiario (de hecho 4 cada una), estado de CU `a generar` y prioridad MoSCoW con justificación de una línea. El enum de estado declarado ("Propuesto") pertenece al conjunto cerrado.

---

## 4. Chequeos específicos solicitados

| Chequeo | Resultado | Evidencia |
| --- | --- | --- |
| `acuerdo-equipo_v1.0.md` omitido con nota en README de 00 | Cumple | README 00 §"Nota de omisión": equipo_n=1, amparo en `00_rules` §2.1 y §1.3 |
| compatibilidad declara Android, contenedor y navegadores | Cumple | §1, §2, §3, §5 |
| iOS/Windows marcados fuera de v1 | Cumple | §1, §2 (nota), §4 (tabla de alternativas) |
| Versiones mínimas marcadas como propuestas, no definitivas | Cumple | §3 "Estado del dato: Propuesto, a confirmar al cerrar §17 P.9"; §1 lo declara explícito |
| Cada NB con 10 secciones | Cumple | Ver §3.2 |
| Cada NB con ≥4 criterios SMART numéricos | Cumple | NB-01 = 5; NB-02..07 = 4 cada una; todos con número y unidad |
| MoSCoW coherente con §4 del intake | Cumple con matiz | Ver H-03 (NB-02 Must agrupa también F-14 Should; NB-05 Must agrupa F-12/F-13 Should). MoSCoW de NB declarada por la prioridad dominante, justificada |
| ≥3 stakeholders por categoría | Cumple | 4 por NB, cubriendo P/I/B |
| Dependencias acíclicas (≤3) | Cumple | DAG verificado; dependencia máxima = 2 (NB-05) |
| CU previstas con estado `a generar` | Cumple | Las 17 CU previstas en estado `a generar` |
| Sin stack en visión/alcance/roadmap/NB | Cumple | Scan con límites de palabra sin coincidencias |
| Cobertura F-01 a F-18 consistente | Cumple | Índice §5 mapea las 18 capacidades; F-18 Won't Have v1 sin NB (excluido), coherente con alcance §5 |

---

## 5. Coherencia cross-doc

- IDs no duplicados: NB-01 a NB-07 únicos; CU-01 a CU-17 sin colisión entre NB; F-01 a F-18 consistentes con el intake §4.
- Enlaces del índice maestro: las 7 referencias `necesidades-de-negocio/NB-XX-...` resuelven a archivos existentes; los enlaces del README también. Sin enlaces rotos.
- Glosario: el glosario de visión §9 (10 términos) es coherente con el del intake §12; sin contradicciones de definición. La nota de "marcador compartido por varias observaciones" es consistente entre visión, alcance, NB-03 y el intake.
- Coherencia NB ↔ visión/alcance: el ciclo del relevamiento (administrar → gestionar → capturar → sincronizar → revisar/cerrar) de las NB respeta la visión §4 y el alcance §4.1; las NB Could Have (NB-06, NB-07) corresponden a F-16/F-17 Could Have del alcance.
- Mapa de dependencias entre NB: idéntico en índice §3 y README; ambos declaran DAG y dependencia máxima 2. Verificado acíclico.
- Trazabilidad upstream: cada cabecera de NB cita SOLUTION-INTAKE + vision + alcance; coherente con `01_rules` §3.3. La visión cita §1,§2,§3,§8,§10,§11,§12; el alcance cita §4,§9,§10; compatibilidad cita §17 P.9. Todas consistentes con el intake v1.2.
- Stakeholders: la tabla de visión §2, la del README de 00 y el RACI del README de 01 coinciden (Vialidad provincial / Departamento de desarrollo 1 dev / 4 roles de sistema).

---

## 6. Hallazgos enumerados

### H-01 — P2 — Referencia al stack en el README de la sección 00
- Archivo: `00_contexto/README.md`, fila 4 de la tabla "Documentos de la sección".
- Evidencia: la descripción de `compatibilidad-plataformas` dice "Plataformas target soportadas (contenedor con runtime .NET, navegadores evergreen, Android)". El token `.NET` y `Android` aparecen en un documento de la sección 00 distinto de `compatibilidad-plataformas`.
- Análisis: el matiz D7 reserva el stack a `compatibilidad-plataformas`. El README solo describe el propósito del documento de compatibilidad y no toma decisiones de stack, por lo que el riesgo es bajo; aun así, mencionar `.NET` y `Android` fuera del documento habilitado roza el matiz. Se clasifica P2 (cabecera/contenido con detalle marginalmente fuera de lugar), no P0/P1, porque no es la visión/alcance/roadmap ni una NB y no rompe trazabilidad.
- Recomendación: reformular la descripción a términos de plataforma neutros, p. ej. "Plataformas target soportadas (contenedor de backend/front, navegadores y dispositivo de campo), versiones mínimas propuestas y plataformas fuera de v1", dejando el stack concreto dentro de `compatibilidad-plataformas`.

### H-02 — P3 — Etiqueta de categoría downstream no canónica en el roadmap
- Archivo: `roadmap-producto_v1.0.md`, cabecera "Trazabilidad downstream" y §3/§6.
- Evidencia: se referencia "06_backlog"; el nombre canónico de la categoría (master-prompt §3.5 y plan §6) es `06_backlog-tecnico`.
- Recomendación: usar `06_backlog-tecnico` para alinear con la nomenclatura de categorías y evitar ambigüedad al resolver enlaces futuros. (El índice de NB ya usa `06_backlog-tecnico` correctamente.)

### H-03 — P3 — MoSCoW de la NB agrega capacidades Should bajo prioridad Must
- Archivo: `NB-02` §9 (agrupa F-03/F-04/F-11 Must + F-14 Should) y `NB-05` §9 (F-11 Must + F-12/F-13 Should).
- Evidencia: la NB declara prioridad Must y dentro de ella convive una capacidad Should (F-14 en NB-02; F-12/F-13 en NB-05).
- Análisis: no es un defecto de coherencia MoSCoW: la regla pide una prioridad por NB con justificación, y la NB la fija por su capacidad dominante (Must), explicitando que las Should "enriquecen sin alterar la prioridad fundacional". Es trazable y defendible. Se deja como P3 por claridad: convendría que el índice o la NB hagan explícito el desglose de prioridad por capacidad para que 06/07 no asuman que toda la NB es Must.
- Recomendación: anotar en §9 (o en la tabla de cobertura del índice) qué capacidades internas son Should, para que el backlog las pueda diferir sin reabrir la NB.

### H-04 — P3 — Propuesta de valor con diferenciador deferido
- Archivo: `vision-producto_v1.0.md` §3, último párrafo.
- Evidencia: "La diferenciación defendible frente a alternativas externas y la caracterización detallada de lo que el cliente hace hoy quedan pendientes en el intake (§3)".
- Análisis: es una deferición transparente y correcta (el intake §3 marca esos puntos como PENDIENTE; el generador no inventó, conforme master-prompt §9). No es un placeholder filtrado de plantilla ni incumple `00_rules` §6, que no exige diferenciador. Se registra como P3 informativo.
- Recomendación: al confirmarse la línea de base del negocio, completar el diferenciador y subir versión menor de la visión.

### H-05 — P3 — Redacción de la cabecera de trazabilidad de la sección §6 del roadmap
- Archivo: `roadmap-producto_v1.0.md` §6 (titulada "Trazabilidad downstream") abre con un bloque "Upstream:" y luego "Downstream:".
- Evidencia: la sección obligatoria de `00_rules` §4.2 para roadmap es "§6 Trazabilidad downstream a 06 backlog y 07 sprint plan"; el documento incluye además el upstream, lo cual es correcto pero el título nombra solo downstream.
- Recomendación: renombrar la sección a "Trazabilidad" o "Trazabilidad upstream/downstream" para que el encabezado refleje el contenido. Cambio cosmético.

---

## 7. Veredicto final

VEREDICTO: APROBADO CON OBSERVACIONES.

Fundamento: no se detectó ningún hallazgo P0 ni P1. La Fase A cumple los 11 ítems de `00_rules` §6 y los 14 ítems de `01_rules` §6 para el tipo `rest-api`, respeta D1-D8 (incluido el matiz D7 sobre stack), mantiene la trazabilidad upstream/downstream consistente con el intake v1.2 y el manifiesto, y la estructura de carpetas y filenames es correcta (00 y 01 a nivel solución, subcarpeta `necesidades-de-negocio/`, índice maestro en la raíz, regex de NB válido). Conforme a la regla del veredicto (master-prompt §10), la ausencia de P0 habilita avanzar a la siguiente fase.

Condiciones recomendadas (no bloquean la promoción a Fase B):
1. Resolver H-01 reformulando la descripción de `compatibilidad-plataformas` en el README de 00 para no nombrar el stack fuera del documento habilitado.
2. Normalizar la etiqueta `06_backlog` → `06_backlog-tecnico` en el roadmap (H-02).
3. Explicitar el desglose Must/Should por capacidad en NB-02 y NB-05 (H-03), para que 06/07 difieran las Should sin reabrir la NB.

Hallazgos por nivel: P0 = 0 · P1 = 0 · P2 = 1 · P3 = 4.

---

## Control de cambios

| Versión | Fecha | Cambios | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-06-15 | Auditoría independiente inicial de la Fase A (00_contexto y 01_necesidades_negocio) de la solución GeoVial. Veredicto APROBADO CON OBSERVACIONES (0 P0, 0 P1, 1 P2, 4 P3). | Auditor independiente (Arquitecto de Soluciones + QA Senior) |
