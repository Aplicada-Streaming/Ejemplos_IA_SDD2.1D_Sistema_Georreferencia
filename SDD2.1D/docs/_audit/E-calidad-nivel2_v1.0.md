# Auditoría Fase E (Calidad y pruebas) — Nivel 2 (geovial-web, geovial-mobile)

**Fase:** E — Calidad y pruebas (categoría 08_calidad_y_pruebas)
**Proyectos auditados:** geovial-web (web-monolith) y geovial-mobile (mobile-app-maui), nivel topológico 2, equipo_n=1
**Alcance:** los 7 artefactos obligatorios + README de cada proyecto, contra `08_rules_calidad_y_pruebas.md` (v1.2) §6, intake `SOLUTION-INTAKE-geovial_v1.0.md` (v1.5) §17 P.6/P.10, y upstream 02/05/06/07 de cada proyecto.
**Auditor:** Arquitecto de Soluciones + QA Senior (independiente; no participó de la generación)
**Fecha:** 2026-06-16
**Documento:** E-calidad-nivel2_v1.0.md

---

## 1. Resumen ejecutivo

Ambos proyectos presentan las 8 piezas esperadas (7 obligatorias + README), con `guia-testing-extensibilidad` correctamente omitida y registrada por `tiene_extensibilidad=false`. La trazabilidad TC↔CU/RN/NFR es completa y bidireccional en los dos proyectos: no hay TC huérfanos ni CU/RN/NFR sin TC. Las pirámides numéricas (web 70/20/10; mobile 70/15/15) están justificadas contra la invertida y la aplanada; la cobertura se reporta por capa (no como número global) y la reconciliación del gate global §17 P.6 con los pisos por capa está declarada en los tres documentos que la tocan (estrategia-calidad, estrategia-testing, matriz). Los NFR numéricos de §17 P.10 coinciden con la arquitectura 05 §8 y tienen TC. La DoD es canónica por las cuatro capas y no se redefine en el mini-plan de 07 (ambos mini-planes la referencian explícitamente sin redefinirla). Encoding UTF-8/LF sin BOM, kebab-case, sufijo `_v1.0.md` sin marcador de dominio, sin stacks concretos en el cuerpo.

No se detectó ningún hallazgo P0 en ninguno de los dos proyectos. Hallazgos: 0 P0, 0 P1, 2 P2, 3 P3.

| Proyecto | P0 | P1 | P2 | P3 | Veredicto |
| --- | --- | --- | --- | --- | --- |
| geovial-web | 0 | 0 | 0 | 2 | APROBADO |
| geovial-mobile | 0 | 0 | 1 | 1 | APROBADO CON OBSERVACIONES |
| Consolidado nivel 2 | 0 | 0 | 2 | 3 | APROBADO CON OBSERVACIONES |

Ambos proyectos pueden avanzar a la Fase F.

---

## 2. Matriz D1-D8 por documento

Leyenda: OK = conforme; — = no aplica. Verificado: idioma rioplatense técnico, encoding UTF-8/LF sin BOM (CR=0 byte en todos los archivos), kebab-case, sufijo `_v<X.Y>.md` (nunca `.v`), sin marcador de dominio en el nombre, sin stacks concretos en el cuerpo, trazabilidad D6 en cabecera/cuerpo. El carácter `↔` aparece solo en títulos de tabla CU↔TC/NFR↔TC/RN↔TC (notación prescrita por la regla 08, no es emoji; no se reporta).

### geovial-web

| Documento | Idioma | UTF-8/LF | kebab+`_vX.Y` | Sin sufijo dominio | Sin stack en cuerpo | Trazabilidad D6 |
| --- | --- | --- | --- | --- | --- | --- |
| estrategia-calidad_v1.0.md | OK | OK | OK | OK | OK | OK |
| estrategia-testing_v1.0.md | OK | OK | OK | OK | OK | OK |
| plan-pruebas_v1.0.md | OK | OK | OK | OK | OK | OK |
| matriz-cobertura-pruebas_v1.0.md | OK | OK | OK | OK | OK | OK |
| casos-prueba-referenciales_v1.0.md | OK | OK | OK | OK | OK | OK |
| criterios-validacion_v1.0.md | OK | OK | OK | OK | OK | OK |
| definition-of-done_v1.0.md | OK | OK | OK | OK | OK | OK |
| README.md | OK | OK | OK | OK | OK | OK |

### geovial-mobile

| Documento | Idioma | UTF-8/LF | kebab+`_vX.Y` | Sin sufijo dominio | Sin stack en cuerpo | Trazabilidad D6 |
| --- | --- | --- | --- | --- | --- | --- |
| estrategia-calidad_v1.0.md | OK | OK | OK | OK | OK (1) | OK |
| estrategia-testing_v1.0.md | OK | OK | OK | OK | OK (1) | OK |
| plan-pruebas_v1.0.md | OK | OK | OK | OK | OK (1) | OK |
| matriz-cobertura-pruebas_v1.0.md | OK | OK | OK | OK | OK (1) | OK |
| casos-prueba-referenciales_v1.0.md | OK | OK | OK | OK | OK | OK |
| criterios-validacion_v1.0.md | OK | OK | OK | OK | OK (1) | OK |
| definition-of-done_v1.0.md | OK | OK | OK | OK | OK (1) | OK |
| README.md | OK | OK | OK | OK | OK (1) | OK |

(1) El término "Android" aparece en el cuerpo como única plataforma target. No es un stack del dominio fuente del bootstrap ni un producto comercial de tooling; es la plataforma declarada en intake §17 geovial-mobile P.9 (Android únicamente, sin iOS ni Windows en v1). Verificación específica: NO se hallaron `.NET`, `MAUI`, `Blazor`, `SQLite`, `JWT`, `Leaflet`, `MudBlazor`, `SQL Server`, `S3`, `net8.0` ni `API 26` en el cuerpo de ningún documento de 08 (búsqueda exhaustiva, 0 coincidencias). El tooling se nombra por rol abstracto en ambos proyectos. La presencia de "Android" se trata como P2 (ver hallazgo H-02), no P0.

Resultado D1-D8: sin violaciones P0 en ninguno de los 16 documentos.

---

## 3. Matriz de estructura obligatoria (§6 de 08_rules)

| Criterio §6 / §2.1 | geovial-web | geovial-mobile |
| --- | --- | --- |
| `estrategia-calidad` con atributos ISO 25010 priorizados + quality gates | OK (8 atributos, 9 gates) | OK (8 atributos, 8 gates) |
| `estrategia-testing` con pirámide numérica + cobertura por capa + tooling | OK (70/20/10) | OK (70/15/15) |
| `plan-pruebas` con criterios entrada/salida + riesgos por sprint | OK (5 tramos, 6 riesgos) | OK (3 tramos, 7 riesgos) |
| `matriz-cobertura` con las TRES tablas (CU↔TC, NFR↔TC, RN↔TC) + cobertura por capa | OK | OK |
| `casos-prueba-referenciales` (TC con setup/pasos/expected/status) | OK (22 TC, TC-01..22 contiguos) | OK (28 TC, TC-01..28 contiguos) |
| `criterios-validacion` con criterios numéricos de release | OK | OK |
| `definition-of-done` con DoD por capa (US/BT/sprint/release) verificable | OK | OK |
| `guia-testing-extensibilidad` OMITIDA y registrada (tiene_extensibilidad=false) | OK (README §2) | OK (README §2) |
| README de sección (índice) | OK | OK |
| Pirámide justificada contra invertida y aplanada | OK | OK |
| Cobertura por capa (no número global) | OK (App UI 80/70, Infra 70/60, Pres 60/50) | OK (Lógica 75/70, Infra 70/60, Pres 60/50) |
| Cada NFR numérico con TC | OK (disponibilidad como SLO observado, declarado) | OK (todos con TC ejecutable) |
| Cada TC referencia ≥1 CU/RN/NFR | OK | OK |
| DoD por capa verificable mecánicamente | OK | OK |
| DoD no redefinida en 07 | OK (mini-plan §4 referencia, no redefine) | OK (mini-plan §4 referencia, no redefine) |
| Cabecera obligatoria (§4.1) | OK | OK |
| Control de cambios | OK | OK |

Todos los criterios mecánicos de §6 se cumplen en ambos proyectos. Cabeceras: en ambos proyectos el bloque de metadatos usa líneas planas (`Proyecto:`, `Documento:`, ...) en lugar del formato exacto en negrita del template §4.1; es consistente con el resto de la solución y no degrada la información (ver H-04, P3).

---

## 4. Coherencia cross-doc y trazabilidad

### 4.1 Existencia de CU/RN/NFR referenciados

- geovial-web: la especificación 02 tiene CU-01 a CU-11 (11 archivos) y RN-01 a RN-05 (5). La matriz cubre exactamente CU-01..11 y RN-01..05. Sin referencia colgante.
- geovial-mobile: la especificación 02 tiene CU-01 a CU-07 (7) y RN-01 a RN-05 (5). La matriz cubre CU-01..07 y RN-01..05. Sin referencia colgante.

### 4.2 NFR vs §17 P.10 v1.5 y arquitectura 05 §8

- geovial-web: interacción p95 ≤ 200 ms, ≥ 50 circuitos concurrentes, disponibilidad ≥ 99,5 %, custodia del token (0 exposiciones). Coinciden con intake §17 geovial-web P.10 y con la tabla de NFR de 05 §8 (arquitectura-solucion §8 verificada: p95 ≤ 200 ms / ≥ 50 circuitos / 99,5 %). La matriz declara explícitamente que disponibilidad es un SLO observado en operación (09), no un TC de CI; el resto de los NFR numéricos tiene TC ejecutable (TC-19 token, TC-20 p95, TC-21 concurrencia). Correcto.
- geovial-mobile: captura 100 % offline, cola ≥ 1000, ciclo de 100 cambios ≤ 30 s, reanudación sin pérdida, arranque ≤ 3 s. Coinciden con intake §17 geovial-mobile P.10 y con 05 §8 (verificada). Todos con TC (TC-08/TC-12 offline, TC-24 cola, TC-25 ciclo, TC-19 reanudación, TC-26 arranque). Correcto.

### 4.3 Reconciliación del gate global §17 P.6 con la cobertura por capa

Declarada y consistente en ambos proyectos. En los dos casos los documentos explicitan que el gate global (líneas ≥ 80 %, branches ≥ 70 %) se mide sobre la unión de capas y los pisos por capa se miden por separado, sin compensación. La reconciliación aparece coherente en estrategia-calidad §3, estrategia-testing §2 y matriz §5 (y se replica en criterios-validacion §5 y DoD). web: App UI 80/70, infra 70/60, presentación 60/50. mobile: lógica 75/70, infra 70/60, presentación 60/50; coincide con la regla 08 §2.2 (75 % lógica, 60 % presentación) y agrega branches e infra como elaboración admisible. Mutation score correctamente NO exigido (reservado a `library`).

### 4.4 DoD canónica referenciada por 07

- web: `mini-plan_v1.0.md` §4 "La Definition of Done canónica del proyecto vive en la categoría 08 [...] este mini-plan referencia esa DoD por adelantado y no la redefine". DoD de 08 §3 confirma que el mini-plan la referencia. Sin redefinición.
- mobile: `mini-plan_v1.0.md` §4 "Este mini-plan referencia la Definition of Done canónica del proyecto, que vive en la categoría 08 [...] sin sustituir a la DoD canónica". DoD de 08 §3 confirma. Sin redefinición.

Nota de orden topológico: ambos mini-planes se generaron antes de 08 y referencian la DoD "por adelantado / pendiente de 08"; la DoD de 08 cierra el vínculo hacia atrás. No constituye redefinición; el criterio de cierre de tramo provisional declarado en el mini-plan no sustituye a la DoD canónica. Conforme.

### 4.5 Trazabilidad TC↔CU/RN/NFR (sin huérfanos)

- geovial-web (22 TC): cada CU-01..11 tiene ≥1 TC; cada RN-01..05 tiene ≥1 TC; cada NFR (token, p95, concurrencia) tiene TC; cada TC declara su origen CU/RN/NFR. La tabla resumen §3 del catálogo y las tablas §2/§3/§4 de la matriz son mutuamente consistentes. Sin TC huérfano ni requisito sin TC.
- geovial-mobile (28 TC): cada CU-01..07 tiene ≥1 TC; cada RN-01..05 tiene ≥1 TC; cada NFR numérico tiene TC; cada TC declara su origen. Tablas del catálogo §3 y matriz §2/§3/§4 consistentes. Sin huérfanos.

### 4.6 Downstream a 09/11

Ambos README §5 y los quality gates de estrategia-calidad §3 declaran el handoff a 09 (gates como stages del pipeline) y a 10/11 (correr tests / test ejecutable por ejemplo), conforme a la regla 08 §3.3. Declaración correcta; la materialización corresponde a fases posteriores.

---

## 5. Hallazgos

### H-01 — Referencia interna stale a "§9" de la DoD (P3, geovial-web)

- Nivel: P3
- Archivo: `geovial-web/08_calidad_y_pruebas/estrategia-calidad_v1.0.md`
- Sección: §5 (Cadencia de revisión)
- Evidencia: "la `definition-of-done_v1.0.md` [...] cualquier cambio en sus criterios versionables se registra en su §9 y se comunica al equipo en la revisión del tramo siguiente". La DoD de geovial-web tiene cuatro secciones; el control de cambios es la §4, no la §9. La referencia "§9" proviene de la redacción de la regla 08 §3.4 (que cita "§9 del propio documento" de forma genérica) y no apunta a una sección existente en este DoD. La propia DoD §3 ya remite correctamente a "la §4 (control de cambios)". El documento equivalente de geovial-mobile (estrategia-calidad §5) lo redacta bien: "se registra en el control de cambios de ese documento".
- Recomendación: reemplazar "su §9" por "su control de cambios" (o "su §4") para alinear con la estructura real del DoD. No bloqueante.

### H-02 — "Android" como plataforma target en el cuerpo (P2, geovial-mobile)

- Nivel: P2
- Archivo: README.md, estrategia-calidad_v1.0.md (§2, §3), estrategia-testing_v1.0.md (§7), plan-pruebas_v1.0.md (§1, §2, §6), matriz-cobertura-pruebas_v1.0.md (§6), criterios-validacion_v1.0.md (§1), definition-of-done_v1.0.md (§1.4)
- Sección: múltiples (cuerpo)
- Evidencia: "dispositivo de referencia Android conectado por USB en modo desarrollador", "el paquete de aplicación Android se firma con el keystore", "target único (Android)", "plataformas distintas de Android (sin iOS ni Windows en v1, P.9)".
- Clasificación y justificación: "Android" es admisible. Es la única plataforma declarada por el cliente (intake §17 geovial-mobile P.9: Android únicamente, decisión confirmada), no es vocabulario del dominio fuente del bootstrap ni un producto comercial de tooling, y el contexto (firma del paquete, dispositivo de referencia para NFR, distribución por canal interno, exclusión de iOS/Windows) es testing/empaquetado móvil donde nombrar la plataforma aporta precisión verificable. No es P0. El resto del tooling se mantiene por rol abstracto y no hay otros stacks (.NET/MAUI/Blazor/SQLite/JWT/Leaflet) en el cuerpo.
- Recomendación (opcional, no bloqueante): donde el sentido se preserve, abstraer a "la plataforma móvil objetivo" o "el paquete de aplicación móvil" (p. ej. en gates de firma y en la fila de atributo Portabilidad); conservar la mención explícita a Android únicamente donde es load-bearing para la verificación (exclusión de iOS/Windows, API mínima del dispositivo de referencia). Mantener la referencia a P.9 como ancla. Dado que el único target declarado es Android, la abstracción es cosmética y queda a criterio del equipo.

### H-03 — "ROPC"/"JWT bearer" no aparecen en 08, pero la custodia del token se nombra de forma abstracta (verificación, sin hallazgo)

- Nivel: ninguno (control)
- Evidencia: la búsqueda de `JWT`/`ROPC` en el cuerpo de 08 de ambos proyectos devolvió 0 coincidencias; los documentos usan "token bearer" / "token de acceso" / "almacén seguro del dispositivo", que son nociones de seguridad genéricas y no stacks. Conforme con D7. Se deja constancia para descartar el chequeo específico del alcance.

### H-04 — Cabecera de metadatos en texto plano en lugar del bloque en negrita de §4.1 (P3, ambos proyectos)

- Nivel: P3
- Archivo: los 8 documentos de cada proyecto (16 en total)
- Sección: cabecera
- Evidencia: el template §4.1 prescribe `**Proyecto:** {{...}}`, etc., con clave en negrita; los documentos de 08 usan `Proyecto: geovial-web` sin negrita. Todos los campos requeridos (Proyecto, Documento, Versión, Estado, Fecha, Autor) están presentes y completos.
- Recomendación: unificar el estilo de cabecera con §4.1 (negrita en la clave) por consistencia con otras categorías que sí la usan. Es estilístico; no afecta trazabilidad ni completitud. Nota: el upstream del mismo proyecto (p. ej. 07 usa el bloque en negrita), por lo que conviene homogeneizar; no bloqueante.

### H-05 — Estados "Propuesto" en toda la sección (P3, ambos proyectos)

- Nivel: P3
- Archivo: los 16 documentos
- Sección: cabecera (Estado)
- Evidencia: todos los artefactos de 08 están en Estado "Propuesto" y todos los TC en status "Pendiente" por proyecto no implementado. Es coherente con el momento del ciclo (documentación previa a codificación) y la matriz declara el plan de actualización al cierre de cada tramo; no es un defecto de completitud. Se anota porque, al promover a Fase F, conviene confirmar la cadencia de transición de "Propuesto" a "Vigente" descrita en estrategia-calidad §5.
- Recomendación: ninguna acción bloqueante; verificar la transición de estado en la cadencia de revisión declarada. Informativo.

---

## 6. Veredicto

### geovial-web — APROBADO

Cumple íntegramente §6 de 08_rules y D1-D8. Los 7 obligatorios + README presentes; `guia-testing-extensibilidad` omitida y registrada; pirámide 70/20/10 justificada; cobertura por capa (80/70 · 70/60 · 60/50) reconciliada con el gate global 80/70; matriz con las tres tablas + cobertura por capa; 22 TC con trazabilidad completa a CU-01..11, RN-01..05 y NFR de P.10; DoD canónica por capa no redefinida en 07. Sin P0/P1. Hallazgos: 2 P3 (referencia stale §9; estilo de cabecera). Avanza a Fase F.

### geovial-mobile — APROBADO CON OBSERVACIONES

Cumple §6 de 08_rules y D1-D8. Los 7 obligatorios + README presentes; `guia-testing-extensibilidad` omitida y registrada (sin plugins); pirámide 70/15/15 justificada (interfaz móvil en e2e); cobertura por capa (lógica 75/70 · infra 70/60 · presentación 60/50) reconciliada con el gate global 80/70; matriz con las tres tablas + cobertura por capa; 28 TC con trazabilidad completa a CU-01..07, RN-01..05 y los cinco NFR numéricos de P.10 (todos con TC ejecutable); DoD canónica por capa no redefinida en 07. Sin P0/P1. Hallazgos: 1 P2 (uso de "Android" en el cuerpo como plataforma target, admisible y abstraíble de forma opcional) y 1 P3 (estilo de cabecera). La observación P2 no bloquea. Avanza a Fase F.

### Consolidado nivel 2 — APROBADO CON OBSERVACIONES

Conteo total: 0 P0, 0 P1, 2 P2, 3 P3. Sin hallazgos bloqueantes; los dos proyectos de nivel 2 quedan habilitados para avanzar a la Fase F. Se recomienda, sin carácter bloqueante, decidir sobre la abstracción opcional de "Android" en geovial-mobile y homogeneizar el estilo de cabecera y la referencia interna stale a §9 antes de promover los artefactos de "Propuesto" a "Vigente".

---

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-16 | Auditoría independiente de Fase E (08_calidad_y_pruebas) de los proyectos de nivel 2 geovial-web y geovial-mobile. Veredicto: geovial-web APROBADO; geovial-mobile APROBADO CON OBSERVACIONES; consolidado APROBADO CON OBSERVACIONES (0 P0, 0 P1, 2 P2, 3 P3). |
