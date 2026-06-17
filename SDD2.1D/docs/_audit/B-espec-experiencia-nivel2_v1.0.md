# Auditoría independiente — Fase B (Especificación y experiencia) — Nivel 2 (geovial-web y geovial-mobile)

| Campo | Valor |
| --- | --- |
| Fase | B — Especificación y experiencia del proyecto |
| Alcance | Proyectos de nivel topológico 2: `geovial-web` (`web-monolith`) y `geovial-mobile` (`mobile-app-maui`). Categorías `02_especificacion_funcional` y `03_ux_ui_dx` (variante UX/UI) de cada proyecto. Verificación de la omisión de `04_prompts_ai` (usa_llm=false) en ambos |
| Documento | B-espec-experiencia-nivel2_v1.0.md |
| Versión | 1.0 |
| Auditor | Arquitecto de Soluciones + QA Senior (independiente; no participó de la generación) |
| Fecha | 2026-06-16 |
| Insumos de reglas | `02_rules_especificacion_funcional.md` (v1.2, §6; web-monolith y mobile-app-maui), `03_rules_ux_ui_dx.md` (v1.2, §6 variante UX/UI), `master-prompt.md` §10 |
| Fuentes de verdad | `SOLUTION-INTAKE-geovial_v1.0.md` (v1.5, §17 de geovial-web y geovial-mobile), `00_contexto`, `01_necesidades_negocio` (NB-01..NB-07), `geovial-api/02_especificacion_funcional/modelo-datos/modelo-conceptual_v1.0.md` (dominio autoritativo) |
| Lección aplicada | P0 de la Fase B de `geovial-api` (filtración de `ROPC`/`JWT` al cuerpo de dos DX docs): énfasis reforzado en D7, con scan léxico del cuerpo y de los wireframes |

---

## 1. Resumen ejecutivo

Se auditaron los entregables de la Fase B de los dos proyectos de nivel 2. En `geovial-web` (web-monolith): índice maestro, README, 11 casos de uso (CU-01..CU-11), 5 reglas de negocio (RN-01..RN-05), modelo conceptual (12 entidades, como vista de consumo del dominio autoritativo de geovial-api, sin RC propias) en la categoría 02; y en la categoría 03 (variante UX/UI) el marco `experiencia-de-uso`, 4 wireframes (login, home, flujo principal, error/cierre), una `representacion-carrusel-fotos`, `glosario-ux` y README. En `geovial-mobile` (mobile-app-maui): índice maestro, README, 7 casos de uso (CU-01..CU-07), 5 reglas de negocio (RN-01..RN-05), modelo conceptual del almacén local (8 entidades, réplica parcial del dominio autoritativo, sin RC por no superar las 10 entidades) en la categoría 02; y en la categoría 03 (variante UX/UI móvil) el marco `experiencia-de-uso`, 5 wireframes en portrait, `glosario-ux` y README. La categoría `04_prompts_ai` no existe en ninguno de los dos proyectos: la omisión es correcta y consistente con `usa_llm=false` y con la exclusión de análisis por IA del intake §9.

La calidad estructural es alta y la trazabilidad es sólida en ambos proyectos. Los CU tienen las 11 secciones obligatorias, cabecera completa, 4 a 5 criterios Given/When/Then con valores concretos y 3 a 4 excepciones con código cada uno; las RN tienen las 7 secciones con CU afectados explícitos y enunciados atemporales; los modelos conceptuales cumplen las 9 secciones, tienen diagrama Mermaid y NO usan tipos físicos; los índices publican matrices NB→CU→RN→US bidireccionales sin huérfanos. **La lección del P0 de geovial-api se respetó:** el scan léxico del cuerpo de ambos proyectos (02 y 03) está limpio de stack y protocolos prohibidos (.NET, Blazor, MudBlazor, SignalR, MAUI, SQLite, SQL Server, JWT, ROPC, OAuth, Leaflet, OpenStreetMap, EXIF, Android, S3, NuGet); los wireframes no contienen CSS, colores ni tipografías; los modelos no contienen tipos físicos. El token `web-monolith`/`mobile-app-maui` solo aparece como valor D8 en cabeceras y referencias a §2.2 de las reglas, nunca como stack en prosa.

Los dos clientes refieren correctamente el dominio autoritativo de `geovial-api` sin duplicar tipos físicos (web como "vista de consumo", mobile como "réplica parcial del almacén local"), justifican la ausencia de RC propias y tratan los casos límite del intake §7 como supuestos explícitos "a confirmar" alineados con geovial-api 02 §9.

No se detectaron hallazgos bloqueantes en ninguno de los dos proyectos. Los hallazgos son de orden cosmético y de reconciliación menor.

Conteo de hallazgos consolidado: P0 = 0; P1 = 0; P2 = 2; P3 = 3.

Veredicto consolidado: **APROBADO CON OBSERVACIONES** (sin P0; permite avanzar a Fase C de ambos proyectos).

---

## 2. Matriz D1-D8 por documento (fila de scan de stack en D7)

Convención: OK = conforme; n/a = no aplica al documento. D1 idioma rioplatense técnico; D2 UTF-8/LF, sin emojis; D3 kebab/filename; D4 versión `_vX.Y` (nunca `.v`); D5 estado/control de cambios; D6 trazabilidad; D7 sin stack/protocolo/vocabulario fuente en el cuerpo (y sin CSS/colores en wireframes); D8 conjunto cerrado D8.

Nota de verificación de filename: ningún `.md` de la fase presenta el anti-patrón `.v` con punto; todos matchean `^(CU|RN)-\d{2}-[a-z0-9-]+_v\d+\.\d+\.md$` o `<nombre>_v\d+\.\d+\.md`; slugs en minúsculas kebab estricto.

### 2.1 geovial-web — categoría 02 (especificación funcional)

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| especificacion-funcional_v1.0.md (índice) | OK | OK | OK | OK | OK | OK | OK | OK |
| README.md | OK | OK | OK | n/a (sin versión, correcto) | OK | OK | OK | OK |
| CU-01..CU-11 (los 11) | OK | OK | OK | OK | OK | OK | OK | OK |
| RN-01..RN-05 (las 5) | OK | OK | OK | OK | OK | OK | OK | OK |
| modelo-conceptual_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |

### 2.2 geovial-web — categoría 03 (UX/UI)

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| experiencia-de-uso_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| wireframes-pantalla-login_v1.0.md | OK | OK | OK | OK | OK | OK | OK (sin CSS/color) | OK |
| wireframes-panel-relevamientos_v1.0.md | OK | OK | OK | OK | OK | OK | OK (sin CSS/color) | OK |
| wireframes-revision-mapa-carrusel_v1.0.md | OK | OK | OK | OK | OK | OK | OK (sin CSS/color) | OK |
| wireframes-resolucion-conflictos-cierre_v1.0.md | OK | OK | OK | OK | OK | OK | OK (sin CSS/color) | OK |
| representacion-carrusel-fotos_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| glosario-ux_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| README.md | OK | OK | OK | n/a (sin versión, correcto) | OK | OK | OK | OK |

### 2.3 geovial-mobile — categoría 02 (especificación funcional)

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| especificacion-funcional_v1.0.md (índice) | OK | OK | OK | OK | OK | OK | OK | OK |
| README.md | OK | OK | OK | n/a (sin versión, correcto) | OK | OK | OK | OK |
| CU-01..CU-07 (los 7) | OK | OK | OK | OK | OK | OK | OK | OK |
| RN-01..RN-05 (las 5) | OK | OK | OK | OK | OK | OK | OK | OK |
| modelo-conceptual_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |

### 2.4 geovial-mobile — categoría 03 (UX/UI móvil)

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| experiencia-de-uso_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| wireframes-pantalla-login-relogueo_v1.0.md | OK | OK | OK | OK | OK | OK | OK (sin CSS/color) | OK |
| wireframes-lista-relevamientos-asignados_v1.0.md | OK | OK | OK | OK | OK | OK | OK (sin CSS/color) | OK |
| wireframes-mapa-captura_v1.0.md | OK | OK | OK | OK | OK | OK | OK (sin CSS/color) | OK |
| wireframes-detalle-observacion_v1.0.md | OK | OK | OK | OK | OK | OK | OK (sin CSS/color) | OK |
| wireframes-estado-sincronizacion_v1.0.md | OK | OK | OK | OK | OK | OK | OK (sin CSS/color) | OK |
| glosario-ux_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| README.md | OK | OK | OK | n/a (sin versión, correcto) | OK | OK | OK | OK |

### 2.5 Fila de scan de stack/protocolo (D7) — evidencia

- **Scan del cuerpo, stack/protocolo prohibido** (`.net`, `asp.net`, `blazor`, `mudblazor`, `signalr`, `maui`, `sqlite`, `sql server`, `jwt`, `ropc`, `oauth`, `leaflet`, `openstreetmap`, `exif`, `android`, `ios`, `amazon s3`/`s3`, `nuget`, `github`, `entity framework`, `c#`, `kestrel`, `xamarin`): **cero coincidencias en `geovial-web`** (las dos categorías). **En `geovial-mobile`, las únicas coincidencias son del token `mobile-app-maui` usado como valor D8** (cabecera "Proyecto:" del README y referencias a "el tipo `mobile-app-maui`" del §2.2 de las reglas). Conforme a la consigna, el token D8 está permitido como valor en cabeceras y no constituye fuga de stack. No hay `.NET`, `MAUI` (como framework en prosa), `SQLite`, `Blazor` ni `JWT`/`ROPC` en el cuerpo.
- **Vocabulario funcional/UX genérico (permitido) presente y bien empleado:** "componente de mapa", "formulario", "modal", "token"/"token bearer", "almacenamiento seguro del dispositivo", "almacén local", "seguridad del dispositivo (huella, patrón)", "ubicación/GPS como permiso", "datos de ubicación incrustados", "pendiente de ubicación", "unidad transferible/archivo único", "verificación del sistema operativo". geovial-web reemplaza correctamente el grant y el formato de token por "enviando credenciales y lo usa como token bearer"; geovial-mobile usa "seguridad del propio dispositivo" sin nombrar biometría de plataforma.
- **Wireframes — sin CSS/colores/tipografías:** scan negativo de hex, `rgb(`, `font-family`, `font-size`, unidades `px`/`rem`/`em`, `margin:`/`padding:`, `sans-serif` y familias tipográficas en los 4 wireframes de web y los 5 de mobile. La única mención de "color" es una nota WCAG correcta ("el estado no se comunica solo por color: hay texto e ícono", 1.4.1) en `wireframes-estado-sincronizacion`, que es la formulación accesible esperada, no una especificación visual.
- **Modelos conceptuales — sin tipos físicos:** scan negativo de `varchar`, `int`, `datetime`, `uuid`/`guid`, `PRIMARY KEY`, `FOREIGN KEY`, longitudes y `sqlite` en ambos modelos. Las restricciones se expresan en términos conceptuales (identidad, referencia, cardinalidad, valor permitido).
- **D2 — sin emojis ni negrita decorativa:** las negritas se limitan al bloque de cabecera prescripto por §4.1.

---

## 3. Matriz de estructura obligatoria por documento

### 3.1 geovial-web — índice maestro (02 §6 y §4.1)

`especificacion-funcional_v1.0.md` tiene cabecera completa, propósito (§1), alcance (§2), catálogo de 11 CU (§3), catálogo de 5 RN (§4), modelo conceptual (§5), matriz NB→CU→RN→US (§6), relación con el proyecto autoritativo geovial-api (§7), decisiones de recorte (§8), ambigüedades y supuestos abiertos (§9) y control de cambios (§10). Mínimo del tipo `web-monolith` (02 §2.2: 8 CU + modelo conceptual): se cumple con holgura (11 CU). Conforme.

### 3.2 geovial-web — casos de uso (11 secciones §4.2, G/W/T ≥3, ≥1 excepción)

Los 11 CU presentan las 11 secciones obligatorias (§1 Propósito, §2 Actores, §3 Precondiciones, §4 Flujo principal, §5 Flujos alternativos, §6 Excepciones con código, §7 Postcondiciones, §8 G/W/T, §9 Trazabilidad, §10 Notas y supuestos, §11 Control de cambios), cabecera de metadatos completa, 4 criterios Given/When/Then con valores concretos (identificadores como "jarea.norte", "Tramo Norte") y 3 excepciones con código en MAYÚSCULAS_CON_GUION_BAJO cada uno. Incorporan además la sección opcional §13 "Interacción multiusuario y concurrencia", legítima para web-monolith (02 §4.3). Trazabilidad NB/RN/US presente en los 11. Actor primario único por CU. Conforme (ver H-01 sobre el orden de §13 frente a §11).

| CU | 11 secciones | G/W/T (≥3, con valores) | Excepciones (c/código) | NB | Actor primario único |
| --- | --- | --- | --- | --- | --- |
| CU-01..CU-11 | OK | 4 (CA-01..04) | 3, todas con código | NB-01/02/05/06/07 | OK |

### 3.3 geovial-web — reglas de negocio (7 secciones §4.2.1)

| Documento | 7 secciones | CU afectados explícitos | Enunciado atemporal | Resultado |
| --- | --- | --- | --- | --- |
| RN-01 visibilidad-acciones-por-rol | OK | CU-02,03,04,05,06,07,08,09,10,11 | OK | Completo |
| RN-02 conservacion-traza-autoria | OK | CU-02,06 | OK | Completo |
| RN-03 acceso-web-roles-administradores | OK | CU-01,09 | OK | Completo |
| RN-04 estados-visibles-habilitacion-acciones | OK | CU-03,05,06,07,08,09 | OK | Completo |
| RN-05 conflictos-precondicion-cierre | OK | CU-06,07,08 | OK | Completo |

Las 5 RN tienen las 7 secciones, enunciados declarativos atemporales (no CU disfrazados) y derivan explícitamente de las RN del backend autoritativo traduciéndolas a condiciones de presentación. Conforme.

### 3.4 geovial-web — modelo conceptual (9 secciones §4.2.2, sin tipos físicos)

`modelo-conceptual_v1.0.md` presenta las 9 secciones, declara explícitamente que es "una referencia de consumo, no una redefinición" del modelo autoritativo de `geovial-api` (cita la ruta upstream), enumera 12 entidades (las 11 del dominio más la proyección `DestinoAlmacenamiento`), tiene diagrama Mermaid y tabla de trazabilidad entidad→CU→RN. **Sin tipos físicos** (scan negativo). Justifica la ausencia de RC propias: el front no posee invariantes de integridad propias; la integridad la garantizan las RC-01..RC-06 de geovial-api. Conforme con 02 §2.2 (web-monolith exige modelo; RC solo si > 10 entidades, que aquí son vista de consumo sin invariantes propias).

### 3.5 geovial-mobile — índice maestro (02 §6 y §4.1)

`especificacion-funcional_v1.0.md` tiene cabecera completa, propósito (§1), alcance (§2), catálogo de 7 CU (§3), catálogo de 5 RN (§4), modelo conceptual del almacén local (§5), matriz NB→CU→RN→US (§6), correspondencia de numeración (§7), decisiones de recorte (§8), ambigüedades y supuestos abiertos (§9) y control de cambios (§10). Mínimo del tipo `mobile-app-maui` (02 §2.2: 6 CU + modelo conceptual por almacenamiento offline): se cumple (7 CU). Conforme.

### 3.6 geovial-mobile — casos de uso (11 secciones §4.2, G/W/T ≥3, ≥1 excepción)

Los 7 CU presentan las 11 secciones obligatorias, cabecera completa, 4 a 5 criterios Given/When/Then con valores concretos y 3 a 4 excepciones con código. Incorporan las secciones opcionales §14 "Permisos del sistema operativo" y §12 "Performance esperado del CU", legítimas para mobile-app-maui (02 §4.3). Trazabilidad NB/RN/US presente en los 7. Actor primario único (agente de campo) en todos. Conforme (ver H-01 sobre el orden de §14/§12 frente a §11).

| CU | 11 secciones | G/W/T (≥3, con valores) | Excepciones (c/código) | NB | Actor primario único |
| --- | --- | --- | --- | --- | --- |
| CU-01 | OK | 5 | 4 | NB-01 | OK |
| CU-02 | OK | 4 | 3 | NB-04, NB-03 | OK |
| CU-03 | OK | 5 | 3 | NB-03 | OK |
| CU-04 | OK | 4 | 4 | NB-03 | OK |
| CU-05 | OK | 4 | 3 | NB-03 | OK |
| CU-06 | OK | 4 | 3 | NB-04 | OK |
| CU-07 | OK | 4 | 4 | NB-03 | OK |

### 3.7 geovial-mobile — reglas de negocio (7 secciones §4.2.1)

| Documento | 7 secciones | CU afectados explícitos | Enunciado atemporal | Resultado |
| --- | --- | --- | --- | --- |
| RN-01 prioridad-ubicacion-radio-agrupacion | OK | CU-07 (directo), CU-03 (indirecto) | OK | Completo |
| RN-02 orden-sincronizacion-subir-antes-de-bajar | OK | CU-06, CU-02 | OK | Completo |
| RN-03 convivencia-con-conflictos-en-el-cliente | OK | CU-03, CU-06, CU-07 | OK | Completo |
| RN-04 relogueo-por-seguridad-del-dispositivo | OK | CU-01, CU-06 | OK | Completo |
| RN-05 captura-sin-conexion | OK | CU-02,03,04,05,07 | OK | Completo |

Las 5 RN tienen las 7 secciones, enunciados atemporales y se alinean explícitamente con las invariantes equivalentes de geovial-api (RN-04, RN-03, RN-06) y de aplicada-sync. Conforme.

### 3.8 geovial-mobile — modelo conceptual del almacén local (9 secciones §4.2.2, sin tipos físicos)

`modelo-conceptual_v1.0.md` presenta las 9 secciones, declara que el dominio autoritativo es el de geovial-api y que el modelo local es "una réplica parcial para el trabajo offline", enumera 8 entidades (RelevamientoLocal, MarcadorLocal, ObservacionLocal, FotoLocal, ComentarioLocal, EtiquetaLocal, CambioEncolado, MarcaSincronizacionLocal), tiene diagrama Mermaid y trazabilidad. **Sin tipos físicos** (scan negativo, incluido `sqlite`). Por no superar las 10 entidades (8 < 10), no acompaña RC: las invariantes finas las gobierna el backend y el cliente las respeta como réplica. Conforme con 02 §2.2 (mobile-app-maui exige modelo conceptual por almacenamiento offline; RC solo si > 10 entidades).

### 3.9 geovial-web — categoría 03 (UX/UI)

| Documento | Cabecera + Variante | Secciones obligatorias | WCAG 2.2 AA con criterios | Estados mínimos | CU origen declarado | Resultado |
| --- | --- | --- | --- | --- | --- | --- |
| experiencia-de-uso | OK (Variante: UX/UI) | 11/11 (§1..§11, con §0 preámbulo) | Sí (§5: 1.4.3, 2.4.7, 2.1.1, 1.3.1, 1.1.1, 4.1.3, 2.5.8, 3.2.6, 3.3.7) | Mapa de estados por superficie (§4) | Sí (10 flujos anclados a CU) | Completo |
| wireframes-pantalla-login (login) | OK | 9/9 (§1..§9) | Sí (notas §7) | vacío/cargando/datos/error (+ sin conexión) | CU-01 | Completo |
| wireframes-panel-relevamientos (home) | OK | 9/9 | Sí | vacío/cargando/datos/error | CU-03 | Completo |
| wireframes-revision-mapa-carrusel (flujo principal) | OK | 9/9 | Sí | vacío/cargando/datos/error | CU-06 | Completo |
| wireframes-resolucion-conflictos-cierre (error/cierre) | OK | 9/9 | Sí | vacío/cargando/datos/error | CU-07, CU-08 | Completo |
| representacion-carrusel-fotos | OK | 7/7 (§4.2.2) | Sí (§5) | n/a | Reutilizado por 2 wireframes | Completo |
| glosario-ux | OK | Propósito + términos referenciados + propios + CC | n/a | n/a | n/a | Completo |

Wireframes web: 4 superficies clave (login, home, flujo principal, error) — cumple el piso del tipo (03 §2.2: web-monolith mínimo 4). Cada wireframe declara su CU origen en §1 y §8. Las superficies no priorizadas (CU-02/04/05/09/10/11) quedan cubiertas por el marco de experiencia, documentado en el README ("el mínimo es piso, no techo"). Conforme.

### 3.10 geovial-mobile — categoría 03 (UX/UI móvil)

| Documento | Cabecera + Variante | Secciones obligatorias | WCAG 2.2 AA | Estados (vacío/cargando/datos/error + sin conexión/sincronizando) | Portrait + nota responsive | CU origen | Resultado |
| --- | --- | --- | --- | --- | --- | --- | --- |
| experiencia-de-uso | OK (Variante: UX/UI) | 11/11 (con §0) | Sí (§5, 11 criterios incl. 2.5.8, 3.3.8) | Mapa por superficie con sin conexión/sincronizando de primera clase (§4) | n/a | Sí (7 flujos a CU) | Completo |
| wireframes-pantalla-login-relogueo | OK | 9/9 | Sí | vacío/cargando/datos/error/sin conexión; sincronizando N/A justificado | Portrait + §6 | CU-01 | Completo |
| wireframes-lista-relevamientos-asignados | OK | 9/9 | Sí | completos + sin conexión/sincronizando | Portrait + §6 | CU-02 | Completo |
| wireframes-mapa-captura | OK | 9/9 | Sí | completos + sin conexión/sincronizando/conflicto/pendiente ubicación | Portrait + §6 | CU-03, CU-04, CU-07 | Completo |
| wireframes-detalle-observacion | OK | 9/9 | Sí | completos + sin conexión/sincronizando/pendiente ubicación | Portrait + §6 | CU-05, CU-07 | Completo |
| wireframes-estado-sincronizacion | OK | 9/9 | Sí | completos + sin conexión/sincronizando/éxito parcial/conflicto | Portrait + §6 | CU-06 | Completo |
| glosario-ux | OK | Propósito + términos móviles + accesibilidad + CC | n/a | n/a | n/a | Completo |

Wireframes mobile: 5 superficies en portrait — cumple el piso del tipo (03 §2.2: mobile-app-maui mínimo 5 portrait + nota responsive). Cada wireframe declara CU origen, los estados mínimos y los estados extra obligatorios de móvil (sin conexión, sincronizando). El estado "sincronizando" en la pantalla de login se marca N/A con justificación correcta (la sesión no sincroniza datos del relevamiento). Conforme.

---

## 4. Coherencia cross-doc por proyecto

### 4.1 geovial-web

- **Índice ↔ README ↔ CU ↔ RN:** las tablas de CU (índice §3) y RN (índice §4) coinciden en IDs, nombres, NB y RN con las cabeceras de cada CU y los CU afectados de cada RN. La matriz NB→CU→RN→US (índice §6) es coherente con la §9 de cada CU. Sin contradicciones.
- **Cada CU enlaza ≥1 NB (sin huérfanos):** los 11 CU se anclan a NB-01, NB-02, NB-05, NB-06 o NB-07 (NB existentes en `01_necesidades_negocio/`). El índice documenta que NB-03 y NB-04 corresponden al lado de campo y no originan CU en el front; el front las toca solo de forma acotada en CU-09 (carga manual), anclado a NB-02. No hay CU huérfano ni NB del alcance del front sin CU.
- **Cada wireframe enlaza un CU real de 02:** login→CU-01, panel→CU-03, revisión→CU-06, conflictos/cierre→CU-07/CU-08, todos CU existentes. La `representacion-carrusel-fotos` declara su reutilización por 2 wireframes.
- **Modelo conceptual referencia el dominio autoritativo sin duplicar tipos físicos:** declarado como vista de consumo que cita el modelo de geovial-api; sin RC propias; sin tipos físicos.
- **Casos límite §7 como supuestos "a confirmar":** índice §9 trata los 3 casos que tocan al front (conflicto entre dos agentes, cierre con cambios sin sincronizar, foto sin ubicación en carga manual) como supuestos heredados de geovial-api 02 §9, marcados "a confirmar", reflejados en CU-06/07/08/09 sin redefinir el dominio.
- **Upstream/downstream:** se citan la visión (00), el intake §17 y las NB de 01; cada CU enumera US a generar en 06, componentes en 05 y tests en 08 en términos tentativos (conforme 02 §3.3); el marco UX y los wireframes enlazan a 06/08.

### 4.2 geovial-mobile

- **Índice ↔ README ↔ CU ↔ RN:** coinciden en IDs, nombres, NB, RN. La matriz NB→CU→RN→US (índice §6) es coherente con la §9 de cada CU y los CU afectados de cada RN. Sin contradicciones.
- **Cada CU enlaza ≥1 NB (sin huérfanos):** los 7 CU se anclan a NB-01, NB-03 o NB-04. El índice documenta que NB-02 y NB-05 se cubren en geovial-api/geovial-web y que la app solo consume sus resultados. No hay CU huérfano.
- **Cada wireframe enlaza un CU real de 02:** login→CU-01, lista→CU-02, mapa-captura→CU-03/04/07, detalle→CU-05/07, sincronización→CU-06. Cobertura completa de CU-01..CU-07.
- **Modelo conceptual referencia el dominio autoritativo sin duplicar tipos físicos:** declarado como réplica parcial del dominio de geovial-api; 8 entidades "Local"; sin RC (8 < 10); sin tipos físicos ni `sqlite`.
- **Casos límite §7 como supuestos "a confirmar":** índice §9 trata los 5 casos que tocan al móvil (foto sin metadatos de ubicación, captura sin señal de GPS, sync parcial por corte, cierre con cambios sin sincronizar, conflicto entre dos agentes) como supuestos explícitos alineados con geovial-api 02 §9, marcados "a confirmar", reflejados en CU-03/04/06/07 con sus excepciones (SIN_SENAL_GPS, etc.).
- **Upstream/downstream:** se cita 00/01, el intake §17 y la dependencia de aplicada-sync (la mecánica de sincronización se delega y no se duplica); cada CU enumera US/05/08 tentativos.

### 4.3 Coherencia entre los dos clientes y el dominio autoritativo

Ambos clientes usan numeración de CU/RN propia (no colisiona con la de geovial-api) y declaran explícitamente la correspondencia de consumo con los CU del backend (geovial-web §7; geovial-mobile §7). Las RN de cada cliente trazan a la RN equivalente del backend. Los dos modelos conceptuales declaran al modelo de geovial-api como autoritativo. La partición de responsabilidades es consistente: el front cubre administración/revisión/cierre (NB-01/02/05/06/07), el móvil cubre captura de campo (NB-01/03/04), y ninguna NB del alcance global queda sin cobertura entre los proyectos del sistema.

---

## 5. Chequeos específicos solicitados

| Chequeo | geovial-web | geovial-mobile | Evidencia |
| --- | --- | --- | --- |
| Mínimo de CU del tipo | Cumple (11 ≥ 8) | Cumple (7 ≥ 6) | Índice §3 de cada proyecto |
| Modelo conceptual presente | Cumple (vista de consumo) | Cumple (almacén local) | modelo-conceptual_v1.0.md |
| RC presentes solo si > 10 entidades | Correcto (vista sin invariantes propias; sin RC) | Correcto (8 entidades; sin RC) | No existe carpeta reglas-conceptuales en ningún cliente (verificado) |
| 11 secciones + ≥3 G/W/T + ≥1 excepción por CU | Cumple | Cumple | §3.2 y §3.6 |
| 7 secciones por RN con CU afectados | Cumple | Cumple | §3.3 y §3.7 |
| experiencia-de-uso con 11 secciones | Cumple | Cumple | §3.9 y §3.10 |
| Wireframes mínimos (web ≥4 / mobile ≥5 portrait) | Cumple (4) | Cumple (5 portrait + nota responsive) | §3.9 y §3.10 |
| 9 secciones por wireframe + estados mínimos | Cumple | Cumple (incl. sin conexión/sincronizando) | §3.9 y §3.10 |
| Cada wireframe declara CU origen | Cumple | Cumple | §4.1 y §4.2 |
| WCAG 2.2 AA con criterios | Cumple | Cumple (reforzada exteriores/guantes) | experiencia-de-uso §5 |
| glosario-ux sin duplicar 02 | Cumple | Cumple | glosario-ux §1 declara no redefinir el dominio |
| Sin CSS/colores/tipografías en wireframes | Cumple | Cumple | Scan negativo (§2.5) |
| Sin stack/protocolo prohibido en el cuerpo | Cumple | Cumple (solo token D8 como valor) | Scan negativo (§2.5) |
| `04_prompts_ai` inexistente y omisión correcta | Cumple | Cumple | Solo existen 02_ y 03_; usa_llm=false; intake §9 |
| Casos límite §7 como supuestos "a confirmar" | Cumple | Cumple | Índice §9 de cada proyecto, alineado a geovial-api 02 §9 |

---

## 6. Hallazgos enumerados

### H-01 — P2 — Numeración de secciones no contigua en los CU de ambos proyectos (opcionales intercaladas antes de §11)

- Archivos: los 11 CU de geovial-web y los 7 CU de geovial-mobile.
- Evidencia: el orden impreso de secciones es §1..§10, luego la(s) opcional(es) habilitada(s) por el tipo, y recién al cierre §11 "Control de cambios". En geovial-web resulta 1..10, 13, 11 (§13 "Interacción multiusuario y concurrencia"); en geovial-mobile resulta 1..10, 14, 12, 11 (§14 "Permisos del sistema operativo" y §12 "Performance esperado del CU"). Verificado en CU-06 (web) y CU-04 (mobile), uniforme en el resto.
- Análisis: las 11 secciones obligatorias del §4.2 están todas presentes y las opcionales son legítimas para cada tipo (02 §4.3). La anomalía es de orden/etiquetado, no de completitud ni de trazabilidad. Es el mismo patrón observado en la Fase B de geovial-api (H-02 de ese informe). El anti-patrón "numeración no contigua sin justificación" (02 §4.5) aplica de forma análoga a la numeración interna de secciones. No bloqueante.
- Recomendación: ubicar §11 Control de cambios como última sección y mover las opcionales (§13 en web; §14/§12 en mobile) entre §10 y §11, renumerándolas como posteriores a las 11 obligatorias, para que el cierre sea siempre el control de cambios. Aplicar de forma consistente con la corrección equivalente de geovial-api.

### H-02 — P2 — La entidad `DestinoAlmacenamiento` del modelo de geovial-web no figura en el modelo autoritativo de geovial-api

- Archivos: `geovial-web/02_especificacion_funcional/modelo-datos/modelo-conceptual_v1.0.md` (§1, §5 del índice) frente a `geovial-api/.../modelo-conceptual_v1.0.md` (12 entidades, sin `DestinoAlmacenamiento`).
- Evidencia: el modelo de consumo del front declara 12 entidades = las 11 del dominio autoritativo más una proyección `DestinoAlmacenamiento` (configuración de almacenamiento, CU-11). El modelo autoritativo de geovial-api modela la configuración de destino vía CU-17 pero no la expone como entidad conceptual con ese nombre.
- Análisis: el front se declara explícitamente como "vista de consumo, no redefinición". Introducir una entidad de proyección que no existe en el dominio autoritativo es una inconsistencia menor de reconciliación: o bien es una vista de configuración legítima del front (y conviene rotularla como proyección de configuración, no como entidad del dominio), o bien revela una entidad faltante en el modelo autoritativo. No rompe trazabilidad (CU-11 la consume y existe) ni introduce tipos físicos. Reconciliación, no bloqueante.
- Recomendación: anotar en el §5 del índice y en el modelo del front que `DestinoAlmacenamiento` es una proyección de configuración local de la vista (no una entidad del dominio autoritativo), o evaluar con el equipo de geovial-api si corresponde incorporarla al modelo autoritativo. Reconciliar en la misma iteración o al revisar el modelo de geovial-api.

### H-03 — P3 — Estado "sincronizando" marcado N/A en la pantalla de login de geovial-mobile

- Archivo: `geovial-mobile/03_ux_ui_dx/wireframes-pantalla-login-relogueo_v1.0.md` §5 Estados.
- Evidencia: la tabla de estados incluye "Sincronizando — No aplica en esta superficie — La sesión no sincroniza datos del relevamiento; el estado de sincronización vive en sus pantallas".
- Análisis: la regla de móvil exige los estados sin conexión y sincronizando además de los mínimos. En la superficie de login el estado sincronizando no tiene contenido funcional y se documenta como N/A con justificación explícita, lo cual es razonable y deja la ausencia deliberada y visible. Se anota por completitud, no por incumplimiento.
- Recomendación: aceptable como está; sin acción requerida más allá de mantener la justificación visible.

### H-04 — P3 — "Pruebas que la verifican" de algunas RN cubren un subconjunto de los CU afectados

- Archivos: RN-01 de geovial-web (declara 10 CU afectados; §6 referencia un subconjunto) y RN-05 de geovial-mobile (declara 5 CU; §6 sobre un subconjunto), entre otras.
- Evidencia: la sección §5 "CU afectados" enumera más CU que los que la §6 "Pruebas que la verifican" referencia explícitamente.
- Análisis: la §6 obligatoria existe en las 10 RN de la fase y referencia 08; no es un defecto de formato. La cobertura de prueba declarada es parcial respecto del conjunto de CU afectados, lo que se consolidará al generar 08. Es el mismo patrón observado en geovial-api (H-05). P3.
- Recomendación: al producir 08 de cada cliente, cerrar la matriz de cobertura de cada RN contra todos sus CU afectados, o anotar en la §6 que la verificación se completa en 08.

### H-05 — P3 — Cobertura de wireframes de geovial-web acotada al piso mínimo (4 de 11 CU)

- Archivos: `geovial-web/03_ux_ui_dx/` (4 wireframes para 11 CU).
- Evidencia: 6 CU con interacción humana (CU-02 administración de usuarios, CU-04 asignación, CU-05 marcadores iniciales, CU-09 carga manual, CU-10 portabilidad, CU-11 configuración) no tienen wireframe propio; quedan cubiertos por el marco `experiencia-de-uso`.
- Análisis: el README lo documenta explícitamente ("el mínimo del tipo es piso, no techo") y el piso del tipo web-monolith (4 superficies) se cumple. CU-05 (marcadores sobre mapa) y CU-09 (carga manual con radio) son flujos con interacción visual relevante que se beneficiarían de un wireframe propio. No es incumplimiento de §6 (el mínimo se respeta); es una recomendación de cobertura. P3.
- Recomendación: considerar wireframes adicionales para CU-05 y CU-09 en una versión posterior, por ser flujos con interacción de mapa y agrupación visual; no bloquea la fase.

---

## 7. Veredicto

### 7.1 Veredicto por proyecto

- **geovial-web (web-monolith):** APROBADO CON OBSERVACIONES. Sin P0 ni P1. Categoría 02 (índice, 11 CU, 5 RN, modelo conceptual de 12 entidades sin tipos físicos como vista de consumo) y categoría 03 (variante UX/UI: marco de experiencia con 11 secciones, 4 wireframes con 9 secciones y estados mínimos, representación de carrusel, glosario-ux sin duplicar 02) conformes. D7 limpio en cuerpo y wireframes. Omisión correcta de 04. Observaciones: H-01 (P2, orden de secciones), H-02 (P2, entidad de proyección no reconciliada), H-04 y H-05 (P3).
- **geovial-mobile (mobile-app-maui):** APROBADO CON OBSERVACIONES. Sin P0 ni P1. Categoría 02 (índice, 7 CU, 5 RN, modelo conceptual del almacén local de 8 entidades sin tipos físicos ni RC) y categoría 03 (variante UX/UI móvil: marco de experiencia con 11 secciones, 5 wireframes portrait con 9 secciones y estados sin conexión/sincronizando, glosario-ux sin duplicar 02) conformes. D7 limpio (solo token D8 como valor en cabeceras). Omisión correcta de 04. Observaciones: H-01 (P2), H-03 y H-04 (P3).

### 7.2 Veredicto consolidado

VEREDICTO: **APROBADO CON OBSERVACIONES**.

Fundamento: no se detectó ningún hallazgo P0 ni P1 en ninguno de los dos proyectos. La lección del P0 de la Fase B de geovial-api (filtración de `ROPC`/`JWT` al cuerpo) se respetó de forma explícita: el scan léxico del cuerpo de las categorías 02 y 03 de ambos clientes está limpio de stack y protocolos prohibidos, los wireframes no contienen CSS/colores/tipografías y los modelos no contienen tipos físicos. La estructura, los mínimos por tipo (web 8 CU + modelo; mobile 6 CU + modelo offline; web ≥4 wireframes, mobile ≥5 portrait), la trazabilidad NB→CU→RN→US bidireccional sin huérfanos, la referencia al dominio autoritativo de geovial-api sin duplicar tipos físicos, el tratamiento de los casos límite del intake §7 como supuestos "a confirmar" y la omisión justificada de la categoría 04 son todos conformes. Conforme a la regla del veredicto (master-prompt §10), un informe sin P0 con veredicto APROBADO CON OBSERVACIONES permite avanzar a la Fase C de ambos proyectos.

Condiciones recomendadas (no bloquean; resolver en la misma iteración o en una fase de saneamiento):
1. Renumerar las secciones de los CU de ambos proyectos para que el cierre sea siempre §11 y las opcionales queden contiguas posteriores (H-01).
2. Reconciliar la entidad de proyección `DestinoAlmacenamiento` del modelo de geovial-web con el dominio autoritativo de geovial-api (H-02).
3. Cerrar en 08 la cobertura de prueba de cada RN contra todos sus CU afectados (H-04).
4. Evaluar wireframes adicionales para CU-05 y CU-09 de geovial-web; aceptar el N/A justificado de "sincronizando" en el login de geovial-mobile (H-05, H-03).

Hallazgos por nivel (consolidado): P0 = 0 · P1 = 0 · P2 = 2 · P3 = 3.

---

## Control de cambios

| Versión | Fecha | Cambios | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-06-16 | Auditoría independiente inicial de la Fase B (02 y 03 UX/UI) de los dos proyectos de nivel 2 de GeoVial: geovial-web (web-monolith) y geovial-mobile (mobile-app-maui). Veredicto consolidado APROBADO CON OBSERVACIONES (sin P0 ni P1). Énfasis D7 con scan léxico de cuerpo y wireframes: limpio en ambos proyectos (la lección del P0 de geovial-api se respetó). Conforme: web 11 CU + 5 RN + modelo de 12 entidades (vista de consumo, sin RC); mobile 7 CU + 5 RN + modelo de 8 entidades (almacén local, sin RC); 4 wireframes web y 5 mobile portrait con 9 secciones y estados; matrices NB→CU→RN→US bidireccionales sin huérfanos; omisión correcta de la categoría 04. Conteo: 0 P0, 0 P1, 2 P2, 3 P3. | Auditor independiente (Arquitecto de Soluciones + QA Senior) |
