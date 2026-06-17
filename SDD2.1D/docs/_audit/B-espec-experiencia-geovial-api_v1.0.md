# Auditoría independiente — Fase B (Especificación y experiencia) — geovial-api (nivel 1)

| Campo | Valor |
| --- | --- |
| Fase | B — Especificación y experiencia del proyecto |
| Alcance | Proyecto principal `geovial-api` (`rest-api`, nivel topológico 1). Categorías `02_especificacion_funcional` (índice, 22 CU, 7 RN, modelo conceptual, 6 RC) y `03_ux_ui_dx` (variante DX). Verificación de la omisión de `04_prompts_ai` (usa_llm=false) |
| Documento | B-espec-experiencia-geovial-api_v1.0.md |
| Versión | 1.0 |
| Auditor | Arquitecto de Soluciones + QA Senior (independiente; no participó de la generación) |
| Fecha | 2026-06-15 |
| Insumos de reglas | `02_rules_especificacion_funcional.md` (v1.2, §6 para `rest-api`), `03_rules_ux_ui_dx.md` (v1.2, §6 variante DX para `rest-api`), `master-prompt.md` §10 |
| Fuentes de verdad | `SOLUTION-INTAKE-geovial_v1.0.md` (v1.4, §17 de geovial-api), `SOLUTION-MANIFEST-geovial_v1.0.md` (v1.0 Aprobado), `00_contexto` y `01_necesidades_negocio` (NB-01 a NB-07) |

---

## 1. Resumen ejecutivo

Se auditaron los 40 documentos de la Fase B de `geovial-api`: en la categoría 02, el índice maestro, README, 22 casos de uso (17 de recursos públicos CU-01..CU-17 y 5 transversales CU-18..CU-22), 7 reglas de negocio (RN-01..RN-07), el modelo conceptual (12 entidades) y 6 reglas conceptuales de modelo (RC-01..RC-06); en la categoría 03 (variante DX), README y 3 DX docs (`dx-developer-experience`, `guia-onboarding-developer`, `dx-error-messages`). La categoría `04_prompts_ai` no existe: la omisión es correcta y consistente con `usa_llm=false` declarado para toda la solución y con la exclusión de análisis por IA del intake §9.

La calidad estructural es alta y la trazabilidad es sólida. Los 22 CU tienen las 11 secciones obligatorias, cabecera completa, 4 criterios Given/When/Then con valores concretos y 3 excepciones con código cada uno; las 7 RN tienen las 7 secciones con CU afectados explícitos; el modelo conceptual cumple las 9 secciones, tiene diagrama y NO usa tipos físicos; las 6 RC cumplen las 6 secciones; el índice publica una matriz NB→CU→RN→US bidireccional sin huérfanos. La categoría 02 está completamente limpia de stack y vocabulario del dominio fuente. El catálogo de errores de 03 reconcilia perfectamente con los CU de 02: los 48 códigos del catálogo aparecen declarados en los CU (cero códigos inventados). La omisión de `dx-portal-developers` está registrada con justificación (`tiene_portal_developers=false`). Los 4 casos límite PENDIENTE del intake §7 están tratados como supuestos explícitos "a confirmar".

Sin embargo, se detecta un hallazgo bloqueante: los DX docs de la categoría 03 (`dx-developer-experience` y `guia-onboarding-developer`) introducen en el cuerpo los protocolos concretos del stack fuente **ROPC** (cinco ocurrencias) y **JWT** (una ocurrencia). Estos términos están explícitamente prohibidos por D7 para el cuerpo de los documentos; el vocabulario REST genérico permitido se limita a "token bearer", "problem+json", "código de estado", "endpoint", "idempotencia", como el propio README de 03 reconoce. La filtración rompe la regla de abstracción de la naturaleza fuente.

Conteo de hallazgos: P0 = 1; P1 = 0; P2 = 2; P3 = 3. Veredicto: **RECHAZADO** (un P0 obliga a corrección y re-audit).

---

## 2. Matriz D1-D8 por documento

Convención: OK = conforme; n/a = no aplica al documento. D1 idioma rioplatense técnico; D2 UTF-8/LF; D3 kebab/filename; D4 versión `_vX.Y` (nunca `.v`); D5 estado/control de cambios; D6 trazabilidad; D7 sin stack/vocabulario fuente; D8 conjunto cerrado D8.

Nota D2: el repositorio fija `eol=lf` por `.gitattributes` (`* text=auto eol=lf`, `*.md text eol=lf`); CRLF eventual del working tree de Windows no es violación. Verificado: ningún `.md` de la fase presenta BOM (los primeros bytes son `23 20` = `# `) ni se halló el anti-patrón de filename `.v` con punto. Todos los filenames matchean `^(CU|RN|RC)-\d{2}-[a-z0-9-]+_v\d+\.\d+\.md$` o el patrón `<nombre>_v\d+\.\d+\.md` de los DX docs; slugs en minúsculas kebab estricto.

### 2.1 Categoría 02 — especificación funcional

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| especificacion-funcional_v1.0.md (índice) | OK | OK | OK | OK | OK | OK | OK | OK |
| README.md | OK | OK | OK | n/a (sin versión, correcto) | OK | OK | OK | OK |
| CU-01..CU-22 (los 22) | OK | OK | OK | OK | OK | OK | OK | OK |
| RN-01..RN-07 (las 7) | OK | OK | OK | OK | OK | OK | OK | OK |
| modelo-conceptual_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| RC-01..RC-06 (las 6) | OK | OK | OK | OK | OK | OK | OK | OK |

### 2.2 Categoría 03 — UX/UI/DX (variante DX)

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| README.md | OK | OK | OK | n/a (sin versión, correcto) | OK | OK | OK | OK |
| dx-developer-experience_v1.0.md | OK | OK | OK | OK | OK | OK | **VIOLA (ver H-01)** | OK |
| guia-onboarding-developer_v1.0.md | OK | OK | OK | OK | OK | OK | **VIOLA (ver H-01)** | OK |
| dx-error-messages_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |

Verificaciones de respaldo:

- D7 — scan léxico con límites de palabra sobre `02_especificacion_funcional/` (índice, 22 CU, 7 RN, modelo, 6 RC): cero coincidencias de `.net`, `c#`, `sql server`, `sqlite`, `entity framework`, `jwt`, `ropc`, `bearer`, `docker`, `s3`, `amazon`, `nuget`, `blazor`, `mudblazor`, `leaflet`, `openstreetmap`, `gps`, `exif`, `android`, `ios`, `zip`, `github`. La categoría 02 abstrae correctamente: usa "almacén relacional", "proveedor de almacenamiento remoto", "datos de ubicación incrustados en la imagen", "unidad transferible única", "token de autenticación", "cliente de campo". Conforme.
- D7 — scan sobre `03_ux_ui_dx/`: el catálogo `dx-error-messages` y el README están limpios y usan solo vocabulario REST genérico ("token bearer", "problem+json", "código de estado", "endpoint", "clave de idempotencia"). Los otros dos DX docs introducen `ROPC` y `JWT` (ver H-01).
- D2 — sin emojis ni negrita decorativa: las negritas se limitan al bloque de cabecera prescripto por §4.1.

---

## 3. Matriz de estructura obligatoria por documento

### 3.1 Índice maestro (02 §6 y §4.1)

`especificacion-funcional_v1.0.md` tiene cabecera completa, propósito, alcance, catálogo de CU de recursos públicos (§3.1) y transversales (§3.2), catálogo de RN (§4), referencia al modelo y RC (§5), matriz NB→CU→RN→US (§6), correspondencia con la numeración de 01 (§7), decisiones de recorte (§8), ambigüedades y supuestos abiertos (§9) y control de cambios (§10). Declara 22 CU, 7 RN, modelo de 12 entidades y 6 RC. Conforme.

Mínimo del tipo `rest-api` (02 §2.2): "1 CU por recurso público + 5 transversales". Se cumple con holgura: 17 CU de recursos públicos cubren los recursos derivados de NB-01..NB-07, más 5 CU transversales (autorización, errores, paginación, idempotencia, versionado). Conforme.

### 3.2 Casos de uso — 11 secciones (§4.2), G/W/T (≥3) y ≥1 excepción con código

Las 11 secciones obligatorias (§1 Propósito, §2 Actores, §3 Precondiciones, §4 Flujo principal, §5 Flujos alternativos, §6 Excepciones con código, §7 Postcondiciones, §8 Criterios G/W/T, §9 Trazabilidad, §10 Notas y supuestos, §11 Control de cambios) están presentes en los 22 CU. Cada CU incorpora además las secciones opcionales habilitadas para `rest-api` por §4.3: §12 Performance esperado y §15 Idempotencia y reintento.

| Documento | 11 secciones | G/W/T (≥3, con valores) | Excepciones (c/código) | Secciones opcionales | Resultado |
| --- | --- | --- | --- | --- | --- |
| CU-01..CU-11 | OK | 4 (CA-01..04), valores concretos | 3, todas con código | §12 y §15 | Completo (ver H-02 orden) |
| CU-12 | OK | 4, valores concretos | 3, todas con código | §12 (omite §15 opcional) | Completo (ver H-02 orden) |
| CU-13..CU-22 | OK | 4 (CA-01..04), valores concretos | 3, todas con código | §12 y §15 | Completo (ver H-02 orden) |

Los 22 CU tienen cabecera de metadatos completa (Proyecto, Documento, Versión, Estado, Fecha, Autor), 4 criterios Given/When/Then con valores concretos (códigos de error literales y cantidades: "30 relevamientos", "tamaño 10", "5 cambios", "10 marcadores") y 3 excepciones, cada una con código en MAYÚSCULAS_CON_GUION_BAJO. Trazabilidad NB/RN/US presente en los 22. CU-19 y CU-22 declaran explícitamente que no introducen RN de dominio propias, correcto para CU transversales.

### 3.3 Reglas de negocio — 7 secciones (§4.2.1) y CU afectados explícitos

| Documento | 7 secciones | CU afectados explícitos | Enunciado atemporal | Resultado |
| --- | --- | --- | --- | --- |
| RN-01 jerarquía-altas-bajas | OK | CU-01,02,04,05,06,07,12,13,14,15,16,17,18,20 | OK | Completo |
| RN-02 conservacion-autoria-en-baja | OK | CU-01,02,03,18 | OK | Completo |
| RN-03 convivencia-conflictos-marcadores | OK | CU-07,08,10,11,12,13,14 | OK | Completo |
| RN-04 radio-agrupacion-fotos | OK | CU-07,09 | OK (la más procedimental; ver H-04) | Completo |
| RN-05 transicion-estados-relevamiento | OK | CU-04,05,06,13,14 | OK | Completo |
| RN-06 orden-subir-antes-de-bajar | OK | CU-10,11,21 | OK | Completo |
| RN-07 idempotencia-sincronizacion | OK | CU-01,02,04,05,06,08,09,10,13,14,16,21 | OK | Completo |

Las 7 RN tienen las 7 secciones (Enunciado, Justificación, Ámbito, Consecuencia si se viola, CU afectados, Pruebas que la verifican, Control de cambios), cabecera completa y enunciados declarativos atemporales (no son CU disfrazados). RN obligatorias para `rest-api` (02 §2.2): presentes. Conforme.

### 3.4 Modelo conceptual — 9 secciones (§4.2.2), diagrama y sin tipos físicos

`modelo-conceptual_v1.0.md` presenta las 9 secciones: §1 Entidades (12, con propósito y ejemplo de instancia), §2 Atributos clave (tabla nombre/semántica/restricción conceptual), §3 Relaciones verbalizadas, §4 Cardinalidades (notación 1, N, 0..1, 1..N, 2..N), §5 Reglas conceptuales (enlace a RC-01..RC-06), §6 Glosario (reutiliza el de la solución), §7 Diagrama (Mermaid erDiagram embebido), §8 Trazabilidad (entidad→CU→RN), §9 Control de cambios. Modelo presente por `tiene_persistencia=true` (02 §2.2). Las 12 entidades superan las 10, lo que activa correctamente las RC. **Sin tipos físicos**: scan negativo de `varchar`, `int`, `datetime`, `uuid`/`guid`, `PRIMARY KEY`, `FOREIGN KEY`, longitudes; el modelo expresa identidad/referencia/cardinalidad en términos conceptuales. Conforme.

### 3.5 Reglas conceptuales de modelo — 6 secciones (§4.2.3)

| Documento | 6 secciones | Entidades involucradas | Tipo de restricción | RN/CU que la justifican | Resultado |
| --- | --- | --- | --- | --- | --- |
| RC-01 identidad-marcador | OK | MarcadorGeografico, Observacion, Etiqueta | Identidad | RN-03; CU-07,08,09,13 | Completo |
| RC-02 referencia-observacion-marcador | OK | Observacion, MarcadorGeografico, Relevamiento | Referencial | RN-03; CU-07,08,13 | Completo |
| RC-03 integridad-jerarquia-usuarios | OK | Usuario, Rol | Referencial y cardinalidad | RN-01; CU-01,02,18 | Completo |
| RC-04 estado-relevamiento-valido | OK | Relevamiento, ConflictoMarcadores | Valor permitido y derivación | RN-05,RN-03; CU-06,13,14 | Completo |
| RC-05 unicidad-asignacion | OK | Asignacion, Usuario, Relevamiento | Identidad y cardinalidad | RN-01,RN-07; CU-05 | Completo |
| RC-06 monotonia-marca-sincronizacion | OK | MarcaSincronizacion, Relevamiento, Usuario | Derivación y valor permitido | RN-06,RN-07; CU-10,11,21 | Completo |

Las 6 RC tienen las 6 secciones (Enunciado, Entidades involucradas, Tipo de restricción, Mecanismo de verificación conceptual, RN o CU que la justifican, Control de cambios), sin tipos físicos. RC obligatorias por superar 10 entidades (02 §2.2): presentes. Conforme.

### 3.6 Categoría 03 — variante DX

| Documento | Cabecera + Variante | Secciones obligatorias | Diátaxis | Onboarding 5/30/60 | Quick-start verificable | Trazabilidad up/down | Resultado |
| --- | --- | --- | --- | --- | --- | --- | --- |
| dx-developer-experience | OK (Variante: DX) | 9/9 (§1..§8 obligatorias + §0 superficie) | Sí (§4, 4 modos enlazados) | Sí, hitos verificables (§2) | Sí (§3, código diferido a 11) | OK | Completo salvo D7 (H-01) |
| guia-onboarding-developer | OK (Variante: DX) | 6/6 (§1..§6 + §0) | Enlaza 4 modos (§5) | Hitos por sección | Sí (primer ejemplo abstracto) | OK | Completo salvo D7 (H-01) |
| dx-error-messages | OK (Variante: DX) | 6/6 (§1..§6 + §0) | Modo reference de errores | n/a | n/a | OK | Completo |

`dx-developer-experience` cumple las 9 secciones del §4.2.3 (audiencia developer, onboarding por tramos, quick-start, Diátaxis, mensajes de error, métricas DX, feedback loop, trazabilidad, control de cambios), con onboarding 5/30/60 con hitos verificables y quick-start descrito en pasos y comportamiento (código diferido a 11, stack a 05/09). `guia-onboarding-developer` cumple las 6 secciones del §4.2.4. `dx-error-messages` cumple las 6 del §4.2.5 con catálogo accionable (código/categoría/causa/acción) sobre problem+json.

DX docs obligatorios para `rest-api` (03 §2.2): `dx-developer-experience`, `guia-onboarding-developer`, `dx-portal-developers`. Los dos primeros están presentes; `dx-developer-experience` se incluye como obligatorio. `dx-error-messages` (recomendado para `rest-api` en 03 §2.1) se incluye además. La omisión de `dx-portal-developers` está registrada y justificada en el README §"Artefactos omitidos": el intake declara `tiene_portal_developers=false` para `geovial-api` y la tabla maestra (03 §2.1) marca ese documento obligatorio solo para "rest-api con portal visible". La referencia formal de la API se difiere a la categoría 10. Omisión conforme al gating (master-prompt §4, flag `tiene_portal_developers`).

Accesibilidad WCAG 2.2 AA: aplica solo a artefactos con superficie de experiencia visual; ningún DX doc de `geovial-api` describe un portal/sitio (no hay `dx-portal-developers`), por lo que no corresponde el compromiso WCAG en esta sección. Conforme.

---

## 4. Chequeos específicos solicitados

| Chequeo | Resultado | Evidencia |
| --- | --- | --- |
| Matriz NB→CU→RN→US completa y bidireccional | Cumple | Índice §6: NB-01..NB-07 con ≥1 CU cada una; cada CU declara ≥1 NB; CU transversales (CU-18..22) anclados a las NB cuyo cumplimiento sostienen. Cobertura bidireccional declarada (§6 cierre). NB-01..NB-07 existen en `01_necesidades_negocio/necesidades-de-negocio/` (verificado). Sin huérfanos |
| 4 casos límite del intake §7 como supuestos "a confirmar" | Cumple con observación | Centralizados en índice §9; ver detalle más abajo y H-03 |
| Modelo conceptual sin tipos físicos (eso es 05) | Cumple | Scan negativo en modelo y RC; restricciones expresadas en términos conceptuales |
| Códigos de error del catálogo de 03 coinciden con los CU de 02 | Cumple | Los 48 códigos del catálogo `dx-error-messages` aparecen declarados en los CU de 02; diff (catálogo − CU) = vacío. Cero códigos inventados |
| DX docs referencian recursos/CU reales | Cumple | `dx-developer-experience` §0/§8 y `guia-onboarding-developer` §0/§5 citan CU-01..CU-22 y recursos derivados; el catálogo de errores ata cada código a su CU origen |
| `04_prompts_ai` inexistente y omisión correcta | Cumple | Solo existen `02_` y `03_` bajo `proyectos/geovial-api/`. `usa_llm=false`; intake §9 excluye análisis por IA. Omisión conforme (master-prompt §4) |
| Stack/protocolo del dominio fuente fuera del cuerpo | No cumple en 03 | Categoría 02 limpia; categoría 03 introduce `ROPC` (5x) y `JWT` (1x). Ver H-01 |

Detalle de los 4 casos límite (índice §9 "Ambigüedades y supuestos abiertos", master-prompt §9):

1. **Foto sin ubicación incrustada (EXIF sin ubicación)** → índice §9: "se asume que la foto queda pendiente de ubicación manual sin inventarle coordenada (RN-04), reflejado en CU-09. A confirmar." Materializado en CU-09 (flujo, criterio y nota). Tratado como supuesto, no inventado ni omitido.
2. **Conflicto entre dos agentes** → índice §9 y **CU-13 §10**: "se asume igual a la de convivencia y resolución al cierre, a confirmar con el cliente (ver §9 del índice)." Rótulo "a confirmar" presente en el cuerpo de CU-13.
3. **Sync parcial por corte (subida parcial)** → índice §9: "se asume reanudación idempotente sin pérdida ni duplicación (RN-07), reflejada en CU-10. A confirmar." Comportamiento en CU-10 flujo 5.A/CA-02.
4. **Cierre con cambios sin sincronizar** → índice §9 y **CU-10 §10**: "este CU asume que el cierre bloquea nuevas subidas y devuelve RELEVAMIENTO_CERRADO, a confirmar (ver §9 del índice)."

Los 4 están tratados como supuestos explícitos a confirmar; ninguno inventado como hecho cerrado ni omitido. Observación menor sobre la consistencia del rótulo dentro de cada CU en H-03.

---

## 5. Coherencia cross-doc

- **Índice ↔ README ↔ CU ↔ RN**: las tablas de CU (índice §3, README) y de RN (índice §4, README) coinciden en IDs, nombres, NB y RN aplicables. La matriz NB→CU→RN→US (índice §6) es coherente con la §9 de cada CU y con los CU afectados de cada RN. Los enlaces relativos del README resuelven a archivos existentes. Sin contradicciones.
- **Modelo ↔ CU/RN/RC**: la tabla de trazabilidad del modelo (§8) liga cada entidad a sus CU y RN; las 6 RC referenciadas en §5 existen como archivos y declaran las RN/CU que las justifican. IDs de entidad consistentes entre §1, §2, §3, §4, §7 y §8.
- **Catálogo de errores (03) ↔ excepciones de los CU (02)**: reconciliación completa. Los 48 códigos del catálogo `dx-error-messages` están todos declarados en las secciones de excepciones de los CU. Códigos puntuales verificados: PROVEEDOR_NO_DISPONIBLE y CREDENCIALES_PROVEEDOR_INVALIDAS (CU-17), ETIQUETA_DESCONOCIDA (CU-12), FOTO_NO_ALMACENABLE (CU-08), FOTO_NO_RECUPERABLE (CU-15), TRAMO_INCOMPLETO (CU-04), MARCADOR_CON_OBSERVACIONES (CU-07), OPERACION_NO_IDEMPOTENTE (CU-21), ROL_NO_AUTORIZADO (7 CU). El catálogo agrega taxonomía y degradación informada sin introducir códigos ausentes en la fuente funcional.
- **DX docs ↔ CU reales**: `dx-developer-experience` y `guia-onboarding-developer` citan CU-01..CU-22 y RN-01/03/05/06/07; sus US a generar (US-05/06, US-37..US-44) son coherentes con la matriz del índice.
- **Upstream solución (00/01) y downstream (05/06/08)**: los documentos citan vision (00 §2/§9), intake §14/§17 y las NB de 01 (que existen: NB-01..NB-07). Downstream, cada CU enumera US a generar en 06, componentes esperados en 05 y tests previstos en 08 en términos tentativos (conforme 02 §3.3). IDs locales de US no colisionan.

---

## 6. Hallazgos enumerados

### H-01 — P0 — Protocolos del stack fuente (ROPC, JWT) en el cuerpo de dos DX docs de 03
- Archivos y secciones:
  - `03_ux_ui_dx/dx-developer-experience_v1.0.md` §2 (línea 52) y §3 (línea 65): "Obtener un token bearer válido por **el flujo ROPC** entregando credenciales…" y "Solicitar un token… **por el flujo ROPC**."
  - `03_ux_ui_dx/guia-onboarding-developer_v1.0.md` §2 (encabezado, línea 27): "obtener un token **por el flujo ROPC**"; §2 (línea 29): "geovial-api autentica por **el flujo ROPC** y emite un **token bearer JWT**"; §6 control de cambios (línea 103): "obtener token por **ROPC**".
- Evidencia: cinco ocurrencias de `ROPC` y una de `JWT` en el cuerpo de los documentos (no en notas de evolución histórica). `ROPC` (Resource Owner Password Credentials) es un grant concreto del stack de seguridad fuente declarado en el intake §17.P.5 ("flujo ROPC con bearer token JWT"), y `JWT` es un formato de token concreto. Ambos están entre los términos que D7 prohíbe expresamente en el cuerpo ("JWT/ROPC como producto"); el vocabulario REST genérico permitido se limita a "token bearer", "problem+json", "código de estado", "endpoint", "idempotencia". El propio README de 03 (línea 25) enumera el vocabulario permitido y NO incluye ROPC ni JWT, lo que evidencia la inconsistencia interna.
- Análisis: no rompe trazabilidad ni completitud de secciones, pero es una filtración del stack/protocolo del dominio fuente al cuerpo. Por la matriz de niveles (master-prompt §10 y la consigna de la fase), "stack/vocabulario fuente prohibido en cuerpo" es P0 (bloqueante). La filtración está acotada a 2 de los 4 documentos de 03; la categoría 02 entera y el `dx-error-messages` están limpios y usan correctamente "token bearer".
- Recomendación: reemplazar `ROPC` por una formulación genérica del mecanismo, por ejemplo "flujo de credenciales directas (identificador de acceso y credencial)" o "intercambio de credenciales por token", y `token bearer JWT` por "token bearer". El detalle del grant y del formato del token pertenece a 05 (arquitectura/seguridad) y al intake. Tras la corrección, re-auditar la categoría 03.

### H-02 — P2 — Numeración de secciones no contigua en los 22 CU (12 y 15 intercaladas antes de 11)
- Archivos: los 22 CU (CU-01..CU-22).
- Evidencia: el orden impreso de secciones es §1..§10, luego §12 "Performance esperado del CU" y §15 "Idempotencia y reintento" (secciones opcionales habilitadas para `rest-api` por 02 §4.3), y recién al cierre §11 "Control de cambios". La numeración resulta 1..10, 12, 15, 11.
- Análisis: las 11 secciones obligatorias del §4.2 están todas presentes y las opcionales son legítimas para el tipo; la anomalía es de orden/etiquetado, no de completitud ni de trazabilidad. El anti-patrón "numeración no contigua de CU sin justificación" del 02 §4.5 aplica de forma análoga a la numeración interna de secciones. Cosmético pero uniforme en los 22.
- Recomendación: ubicar §11 Control de cambios como última sección y mover §12 y §15 entre §10 y §11, renumerándolas como secciones posteriores a las 11 obligatorias, para que la numeración sea contigua y el cierre sea siempre el control de cambios.

### H-03 — P2 — Cobertura parcial del rótulo "a confirmar" dentro del cuerpo de los CU para 2 de los 4 casos límite
- Archivos: índice §9; CU-09; CU-10; CU-13; CU-14.
- Evidencia: los 4 supuestos a confirmar están declarados de forma explícita en el índice §9. Dentro del cuerpo del CU, el rótulo "a confirmar" aparece en CU-13 (conflicto entre dos agentes) y CU-10 (cierre con cambios sin sincronizar). Para el caso de la foto sin ubicación (CU-09) y la subida parcial por corte (CU-10, caso 3), el comportamiento aparece como flujo/criterio cerrado dentro del CU y delega el rótulo "a confirmar" al índice §9. CU-14, que el índice §9 vincula al caso de cierre con cambios sin sincronizar, no rotula ese supuesto en su propio cuerpo.
- Análisis: la consigna exige que los 4 casos estén "tratados como supuestos explícitos a confirmar en los CU, no inventados ni omitidos". El requisito se cumple a nivel de fase porque el índice §9 los centraliza y los CU los referencian; no hay invención ni omisión. La observación es de consistencia: convendría que cada CV afectado rotule el supuesto en su propio §10 Notas y supuestos para que la lectura aislada del CU no presente el comportamiento como hecho cerrado.
- Recomendación: añadir en el §10 de CU-09 y CU-14 (y reforzar en CU-10 para la subida parcial) la nota "supuesto a confirmar con el negocio (ver índice §9)", para uniformar el tratamiento con CU-13.

### H-04 — P3 — RN-04 con redacción más procedimental que invariante
- Archivo: `reglas-de-negocio/RN-04-radio-agrupacion-fotos_v1.0.md` §1 Enunciado.
- Evidencia: el enunciado describe el algoritmo de agrupación de fotos ("en la carga manual… se agrupan por radio… si no hay marcador… se crea uno nuevo") más que una invariante atemporal pura, a diferencia de las otras seis RN.
- Análisis: sigue siendo una regla verificable y acotada al CU de carga manual; no es un CU disfrazado. Es la más cercana al anti-patrón "RN escrita como CU" (02 §4.5) sin cruzarlo. P3 de estilo.
- Recomendación: reformular el enunciado como invariante ("toda foto cargada manualmente se ancla al marcador cuyo radio la contiene; cuando ninguno la contiene, su ancla es un marcador propio"), dejando el procedimiento para el flujo del CU-09.

### H-05 — P3 — "Pruebas que la verifican" de algunas RN cubren un subconjunto de los CU afectados
- Archivos: RN-01 (declara 14 CU afectados, §6 referencia pruebas sobre 3) y RN-07 (declara 12 CU afectados, §6 sobre 2), entre otras.
- Evidencia: la sección §5 "CU afectados" enumera más CU que los que la §6 "Pruebas que la verifican" referencia explícitamente.
- Análisis: la §6 obligatoria existe en las 7 RN y referencia 08; no es un defecto de formato. La cobertura de prueba declarada es parcial respecto del conjunto de CU afectados, lo que se consolidará al generar 08. P3.
- Recomendación: al producir 08, asegurar que la matriz de cobertura cierre cada RN contra todos sus CU afectados, o anotar en la §6 de cada RN que la verificación se completa en 08.

### H-06 — P3 — CU-12 omite la sección opcional §15 que el resto de los CU incluye
- Archivo: `casos-de-uso/CU-12-consultar-relevamiento-revision_v1.0.md`.
- Evidencia: CU-12 incluye §12 Performance esperado pero omite §15 Idempotencia y reintento, sección opcional presente en los otros 21 CU.
- Análisis: §15 es opcional (02 §4.3) y CU-12 es una consulta de lectura, operación segura para la que la idempotencia de escritura no aplica; la omisión es razonable. Se anota por consistencia, no por incumplimiento. P3.
- Recomendación: dejar una nota breve en CU-12 indicando que §15 no aplica por ser un recurso de solo lectura, para que la ausencia sea deliberada y visible.

---

## 7. Veredicto final

VEREDICTO: **RECHAZADO**.

Fundamento: se detectó un hallazgo P0 (H-01): los DX docs `dx-developer-experience_v1.0.md` y `guia-onboarding-developer_v1.0.md` de la categoría 03 introducen en el cuerpo los protocolos concretos del stack fuente `ROPC` (cinco ocurrencias) y `JWT` (una), términos que D7 prohíbe expresamente y que ni siquiera figuran en el vocabulario permitido que el propio README de 03 declara. Conforme a la regla del veredicto (master-prompt §10), cualquier P0 obliga a RECHAZADO y a corrección con re-audit antes de promover a la Fase C de `geovial-api`.

El resto de la fase es sólido y no presenta otros hallazgos bloqueantes: la categoría 02 (índice, 22 CU, 7 RN, modelo conceptual de 12 entidades, 6 RC) cumple las 11/7/9/6 secciones obligatorias, los mínimos y secciones opcionales del tipo `rest-api`, los criterios G/W/T con valores concretos y las excepciones con código; el modelo no usa tipos físicos; la matriz NB→CU→RN→US es bidireccional sin huérfanos; el catálogo de errores de 03 reconcilia al 100 % con los CU de 02; la omisión de `dx-portal-developers` está justificada por `tiene_portal_developers=false`; los 4 casos límite del intake §7 están tratados como supuestos a confirmar; y `04_prompts_ai` está correctamente ausente (usa_llm=false). La categoría 02 está limpia de stack y de vocabulario del dominio fuente.

Correcciones requeridas para re-audit:
1. **Bloqueante (H-01):** eliminar `ROPC` y `JWT` del cuerpo de `dx-developer-experience_v1.0.md` (§2, §3) y `guia-onboarding-developer_v1.0.md` (§2, §6), reemplazándolos por formulación genérica ("flujo de credenciales directas", "token bearer"). Difiere el detalle a 05 y al intake.

Condiciones recomendadas (no bloquean por sí solas, resolver en la misma iteración o en Fase H):
2. Renumerar las secciones de los 22 CU para que el cierre sea siempre §11 Control de cambios y las opcionales §12/§15 queden contiguas (H-02).
3. Rotular el supuesto "a confirmar" en el §10 de CU-09 y CU-14 (y reforzar en CU-10) para uniformar con CU-13 (H-03).
4. Reformular el enunciado de RN-04 como invariante (H-04).
5. Cerrar en 08 la cobertura de prueba de cada RN contra todos sus CU afectados (H-05).
6. Anotar en CU-12 que §15 no aplica por ser recurso de solo lectura (H-06).

Hallazgos por nivel: P0 = 1 · P1 = 0 · P2 = 2 · P3 = 3.

---

## Control de cambios

| Versión | Fecha | Cambios | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-06-15 | Auditoría independiente inicial de la Fase B (02 y 03 DX) del proyecto principal `geovial-api` (rest-api, nivel 1) de GeoVial. Veredicto RECHAZADO por un P0 (filtración de ROPC/JWT al cuerpo de dos DX docs de 03). Resto de la fase conforme: 22 CU, 7 RN, modelo conceptual de 12 entidades sin tipos físicos, 6 RC, matriz NB→CU→RN→US bidireccional, catálogo de errores reconciliado con los CU, omisión justificada de dx-portal-developers y de la categoría 04. Conteo: 1 P0, 0 P1, 2 P2, 3 P3. | Auditor independiente (Arquitecto de Soluciones + QA Senior) |
