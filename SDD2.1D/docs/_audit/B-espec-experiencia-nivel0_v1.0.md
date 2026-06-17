# Auditoría independiente — Fase B (Especificación y experiencia) — Nivel 0 de GeoVial

| Campo | Valor |
| --- | --- |
| Fase | B — Especificación y experiencia del proyecto |
| Alcance | Proyectos de nivel topológico 0 de GeoVial, ambos `library`: `aplicada-sync` (redistribuible) y `geovial-storage`. Categorías `02_especificacion_funcional` y `03_ux_ui_dx` (variante DX). Verificación de la omisión de `04_prompts_ai` (usa_llm=false) |
| Documento | B-espec-experiencia-nivel0_v1.0.md |
| Versión | 1.0 |
| Auditor | Arquitecto de Soluciones + QA Senior (independiente; no participó de la generación) |
| Fecha | 2026-06-15 |
| Insumos de reglas | `02_rules_especificacion_funcional.md` (v1.2, §6 para `library`), `03_rules_ux_ui_dx.md` (v1.2, §6 variante DX para `library`), `master-prompt.md` §10 |
| Fuentes de verdad | `SOLUTION-INTAKE-geovial_v1.0.md` (v1.2 vigente), `SOLUTION-MANIFEST-geovial_v1.0.md` (v1.0 Aprobado), `01_necesidades_negocio` (índice de NB v1.0) |

---

## 1. Resumen ejecutivo

Se auditaron los 25 documentos de la Fase B de los dos proyectos de nivel 0: para `aplicada-sync`, los 11 de la categoría 02 (índice, README, 6 CU, 3 RN) y los 5 de la categoría 03 (README + 4 DX docs); para `geovial-storage`, los 11 de la categoría 02 (índice, README, 6 CU, 3 RN) y los 4 de la categoría 03 (README + 3 DX docs). Ambos proyectos son `library` y aplican la variante DX en 03. La categoría `04_prompts_ai` no existe en ninguno de los dos proyectos: la omisión es correcta y consistente con `usa_llm=false` declarado para toda la solución.

Los entregables son de alta calidad. Respetan D1-D8 sin excepciones bloqueantes; no contienen vocabulario del dominio fuente del bootstrap (impresoras, ESC-POS, DSL, Bluetooth) ni stacks concretos (.NET, MAUI, SQLite, S3, NuGet, HTTP, JWT, Blazor, Leaflet); ambos proyectos cumplen el mínimo de 5 CU (6 cada uno) con las 11 secciones obligatorias, ≥3 Given/When/Then con valores concretos (4 por CU) y ≥1 excepción por CU; las RN tienen las 7 secciones; los DX docs cumplen las 9 secciones de `dx-developer-experience` con Diátaxis y onboarding 5/30/60 verificables, y los quick-start son verificables y abstractos (sin código de stack, diferido a 11). La trazabilidad upstream CU→NB es correcta y bidireccional en cada proyecto: `aplicada-sync` cubre NB-04, `geovial-storage` cubre NB-07 (con NB-03 y NB-06 de soporte). No hay CU huérfanos.

No se detectaron hallazgos P0 ni P1. Los hallazgos son menores: una reconciliación pendiente del índice de NB con la numeración real (esperada y prevista para la Fase H), un único uso de "token" donde el resto del corpus usa "credencial", y deviaciones cosméticas de orden y de cabecera.

Conteo de hallazgos: P0 = 0; P1 = 0; P2 = 1; P3 = 4. Veredicto consolidado: APROBADO CON OBSERVACIONES.

---

## 2. Matriz D1-D8 por documento

Convención: OK = conforme; n/a = no aplica al documento. D1 idioma rioplatense técnico; D2 UTF-8/LF; D3 kebab/filename; D4 versión `_vX.Y` (nunca `.v`); D5 estado/control de cambios; D6 trazabilidad; D7 sin stack/vocabulario fuente; D8 conjunto cerrado D8.

### 2.1 Proyecto `aplicada-sync`

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 02 especificacion-funcional_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 02 README.md | OK | OK | OK | n/a (sin versión, correcto) | OK | OK | OK | OK |
| CU-01 inicializar-sesion-sincronizacion | OK | OK | OK | OK | OK | OK | Observación (ver H-02) | OK |
| CU-02 registrar-cambio-local | OK | OK | OK | OK | OK | OK | OK | OK |
| CU-03 ejecutar-sincronizacion | OK | OK | OK | OK | OK | OK | OK | OK |
| CU-04 detectar-conectividad-disparar-sync | OK | OK | OK | OK | OK | OK | OK | OK |
| CU-05 consultar-estado-cola | OK | OK | OK | OK | OK | OK | OK | OK |
| CU-06 reanudar-sincronizacion-interrumpida | OK | OK | OK | OK | OK | OK | OK | OK |
| RN-01 orden-subir-antes-de-bajar | OK | OK | OK | OK | OK | OK | OK | OK |
| RN-02 idempotencia-sincronizacion | OK | OK | OK | OK | OK | OK | OK | OK |
| RN-03 convivencia-estados-en-conflicto | OK | OK | OK | OK | OK | OK | OK | OK |
| 03 README.md | OK | OK | OK | n/a (sin versión, correcto) | OK | OK | OK | OK |
| 03 dx-developer-experience_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 03 guia-onboarding-developer_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 03 dx-error-messages_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 03 dx-portal-developers_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |

### 2.2 Proyecto `geovial-storage`

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 02 especificacion-funcional_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 02 README.md | OK | OK | OK | n/a (sin versión, correcto) | OK | OK | OK | OK |
| CU-01 guardar-archivo | OK | OK | OK | OK | OK | OK | OK | OK |
| CU-02 recuperar-archivo | OK | OK | OK | OK | OK | OK | OK | OK |
| CU-03 eliminar-archivo | OK | OK | OK | OK | OK | OK | OK | OK |
| CU-04 verificar-existencia-archivo | OK | OK | OK | OK | OK | OK | OK | OK |
| CU-05 listar-archivos | OK | OK | OK | OK | OK | OK | OK | OK |
| CU-06 configurar-proveedor-activo | OK | OK | OK | OK | OK | OK | OK | OK |
| RN-01 transparencia-proveedor | OK | OK | OK | OK | OK | OK | OK | OK |
| RN-02 integridad-archivo-almacenado | OK | OK | OK | OK | OK | OK | OK | OK |
| RN-03 manejo-seguro-credenciales | OK | OK | OK | OK | OK | OK | OK | OK |
| 03 README.md | OK | OK | OK | n/a (sin versión, correcto) | OK | OK | OK | OK |
| 03 dx-developer-experience_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 03 guia-onboarding-developer_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 03 dx-error-messages_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |

Verificaciones de respaldo (ambos proyectos):

- D2 encoding: ningún archivo presenta BOM ni CR (LF puro). Conforme.
- D3/D4 filenames: los 12 CU y 6 RN matchean `^(CU|RN)-\d{2}-[a-z0-9-]+_v\d+\.\d+\.md$`; los DX docs matchean su patrón con `_v`. No se halló el anti-patrón `.v` con punto (scan negativo en todo `proyectos/`). Slugs en minúsculas kebab estricto.
- D7 scan léxico con límites de palabra sobre todo `proyectos/`: cero coincidencias de `.net`, `dotnet`, `maui`, `sqlite`, `sql server`, `s3`, `amazon`, `nuget`, `blazor`, `mudblazor`, `leaflet`, `openstreetmap`, `signalr`, `docker`, `jwt`, `ropc`, `http(s)`, `c#`, `android`, `ios`, `exif`, `gps`, `bluetooth`, `esc-pos`, `impresora`, `térmica`, `github`, `zip`, `sdk`. La única coincidencia de "endpoint" (aplicada-sync `dx-portal-developers` §4) se usa para negar que la librería exponga un endpoint (uso legítimo y abstracto). La única coincidencia de "token" (aplicada-sync índice 02, §2.2) se registra en H-02.
- Sin emojis ni negrita decorativa: las negritas se limitan al bloque de cabecera prescripto por §4.1 de cada regla.

---

## 3. Matriz de estructura obligatoria por documento

### 3.1 Categoría 02 — casos de uso (11 secciones del §4.2) y reglas de negocio (7 secciones del §4.2.1)

CU: §1 Propósito, §2 Actores, §3 Precondiciones, §4 Flujo principal, §5 Flujos alternativos, §6 Excepciones, §7 Postcondiciones, §8 Criterios G/W/T, §9 Trazabilidad, §10 Notas, §11 Control de cambios. La sección §17 (Compatibilidad de versión pública) es la opcional habilitada para `library` por §4.3.

| Documento | 11 secciones | G/W/T (≥3) | ≥1 excepción | §17 opcional | Resultado |
| --- | --- | --- | --- | --- | --- |
| aplicada-sync CU-01 | OK | 4 | 3 | Presente | Completo (ver H-03 orden) |
| aplicada-sync CU-02 | OK | 4 | 3 | Presente | Completo (ver H-03 orden) |
| aplicada-sync CU-03 | OK | 4 | 3 | Presente | Completo (ver H-03 orden) |
| aplicada-sync CU-04 | OK | 4 | 3 | Presente | Completo (ver H-03 orden) |
| aplicada-sync CU-05 | OK | 4 | 2 | Presente | Completo (ver H-03 orden) |
| aplicada-sync CU-06 | OK | 4 | 3 | Presente | Completo (ver H-03 orden) |
| geovial-storage CU-01 | OK | 4 | 4 | Consolidado en índice §6 | Completo |
| geovial-storage CU-02 | OK | 4 | 4 | Consolidado en índice §6 | Completo |
| geovial-storage CU-03 | OK | 4 | 3 | Consolidado en índice §6 | Completo |
| geovial-storage CU-04 | OK | 4 | 2 | Consolidado en índice §6 | Completo |
| geovial-storage CU-05 | OK | 4 | 3 | Consolidado en índice §6 | Completo |
| geovial-storage CU-06 | OK | 4 | 4 | Consolidado en índice §6 | Completo |

RN (7 secciones: Enunciado, Justificación, Ámbito, Consecuencia si se viola, CU afectados, Pruebas que la verifican, Control de cambios):

| Documento | 7 secciones | CU afectados explícitos | Enunciado atemporal | Resultado |
| --- | --- | --- | --- | --- |
| aplicada-sync RN-01 | OK | CU-03, CU-04, CU-06 | OK | Completo |
| aplicada-sync RN-02 | OK | CU-02, CU-03, CU-06 | OK | Completo |
| aplicada-sync RN-03 | OK | CU-03, CU-05 | OK | Completo |
| geovial-storage RN-01 | OK | CU-01..CU-06 | OK | Completo |
| geovial-storage RN-02 | OK | CU-01, CU-02, CU-04 | OK | Completo |
| geovial-storage RN-03 | OK | CU-02, CU-05, CU-06 | OK | Completo |

Índices maestros 02: ambos `especificacion-funcional_v1.0.md` tienen cabecera completa (§4.1), propósito, alcance, catálogo de CU, catálogo de RN, matriz NB→CU→RN→US y control de cambios. Ninguno incluye modelo conceptual, correcto para `library` (§2.2). Ambos declaran explícitamente la omisión del modelo conceptual y la numeración local del proyecto.

Modelo conceptual: omitido en ambos. Correcto: `library` no exige modelo conceptual (§2.2). Declarado en cada índice y README.

### 3.2 Categoría 03 — variante DX (9 secciones de `dx-developer-experience` §4.2.3; 6 de `guia-onboarding-developer` §4.2.4; 6 de `dx-error-messages` §4.2.5; 8 de `dx-portal-developers` §4.2.6)

| Documento | Cabecera + Variante | Secciones obligatorias | Diátaxis | Onboarding 5/30/60 | Quick-start verificable y sin stack | Trazabilidad up/down | Resultado |
| --- | --- | --- | --- | --- | --- | --- | --- |
| aplicada-sync dx-developer-experience | OK (Variante: DX) | 9/9 (más §0 superficie pública) | Sí (§4, 4 modos enlazados) | Sí, hitos verificables | Sí (§3, diferido a 11) | OK | Completo |
| aplicada-sync guia-onboarding-developer | OK | 6/6 (más §0) | Enlaza modos (§5) | Hitos por tramo | Sí (primer ejemplo abstracto) | OK | Completo |
| aplicada-sync dx-error-messages | OK | 6/6 (más §0) | Modo reference de errores | n/a | n/a | OK | Completo |
| aplicada-sync dx-portal-developers | OK | 8/8 (más §0) | Sí (estructura del portal) | Declara tramos en ejemplos | Sí (sin sandbox hospedado, justificado) | OK | Completo (recomendado) |
| geovial-storage dx-developer-experience | OK (Variante: DX) | 9/9 (más §0) | Sí (§4, 4 modos enlazados) | Sí, hitos verificables | Sí (§3, diferido a 11) | OK | Completo |
| geovial-storage guia-onboarding-developer | OK | 6/6 (más §0) | Enlaza modos (§5) | Hitos por tramo | Sí (primer ejemplo abstracto) | OK | Completo |
| geovial-storage dx-error-messages | OK | 6/6 (más §0) | Modo reference de errores | n/a | n/a | OK | Completo |

DX docs obligatorios para `library` (§2.2): `dx-developer-experience`, `guia-onboarding-developer`, `dx-error-messages`. Los tres presentes en ambos proyectos.

- `aplicada-sync` incluye además `dx-portal-developers` (recomendado en §2.1 para `library con portal hospedado`). Inclusión justificada por su carácter redistribuible con repositorio público y vocación de reutilización; el README de 03 lo declara y adapta las páginas a una librería offline-first. No es omisión.
- `geovial-storage` omite `dx-portal-developers`. Omisión correcta y declarada: `redistribuible = false`, sin portal público, único consumidor `geovial-api`; las reglas no lo exigen para `library`. Conforme.

Accesibilidad WCAG 2.2 AA: aplica solo donde hay superficie con experiencia visual; el único artefacto que describe un sitio (aplicada-sync `dx-portal-developers` §6) declara WCAG 2.2 AA como piso con criterios prioritarios. Conforme.

---

## 4. Chequeos específicos solicitados

| Chequeo | Resultado | Evidencia |
| --- | --- | --- |
| `aplicada-sync` agnóstica del dominio de GeoVial (no acoplada a relevamientos/marcadores) | Cumple | Índice §1 y §9, README: vocabulario neutral (almacén local del host, cambio local, backend remoto, sesión de sincronización). Los 6 CU y 3 RN no nombran relevamientos ni marcadores; la única mención de NB-04 es como trazabilidad de origen, declarando el contrato reutilizable fuera de la solución |
| `aplicada-sync` coherente con su carácter redistribuible | Cumple | Índice §8 y cada CU §17: política de compatibilidad de versión pública con incremento mayor ante cambios incompatibles; `dx-portal-developers` para repositorio público |
| `geovial-storage` describe proveedores de forma abstracta (local / remoto / otro) | Cumple | Índice, CU-06, RN-01/RN-03 y DX docs usan "proveedor local / proveedor de almacenamiento de objetos remoto / otro proveedor"; cero menciones de productos comerciales (scan negativo de `s3`, `amazon`) |
| Quick-start de los DX docs sin código de stack concreto (diferido a 11) | Cumple | Los 4 quick-start (2 por proyecto) se describen en pasos y comportamiento observable, con remisión explícita del código a la categoría 11 y del stack a la 05 |
| `04_prompts_ai` efectivamente inexistente y omisión correcta | Cumple | No existe carpeta `04*` en ninguno de los dos proyectos (búsqueda negativa). `usa_llm=false` para toda la solución; el intake §9 excluye análisis por IA. Omisión conforme al gating de master-prompt §4 |
| Mínimo de 5 CU para `library` | Cumple | 6 CU en cada proyecto (piso 5) |
| 11 secciones por CU, ≥3 G/W/T con valores concretos, ≥1 excepción por CU | Cumple | Ver §3.1: 12/12 CU con 11 secciones, 4 G/W/T con valores concretos cada uno, ≥2 excepciones cada uno |
| RN con 7 secciones | Cumple | Ver §3.1: 6/6 RN completas |
| dx-developer-experience con 9 secciones, Diátaxis y onboarding 5/30/60 | Cumple | Ver §3.2 |
| Trazabilidad upstream CU→NB correcta por proyecto | Cumple con reconciliación pendiente | aplicada-sync→NB-04; geovial-storage→NB-07 (NB-03/NB-06 soporte). Ver H-01 (desalineación del índice de NB, no P0 por convención del orquestador) |

---

## 5. Coherencia cross-doc

### 5.1 `aplicada-sync`

- Índice ↔ README ↔ CU ↔ RN: las tablas de CU y RN del índice (§3, §4) y del README coinciden en IDs, títulos, actor primario y RN aplicables. La matriz NB→CU→RN→US del índice (§5) es coherente con la sección §9 de cada CU y con la lista de CU afectados de cada RN (RN-01: CU-03/04/06; RN-02: CU-02/03/06; RN-03: CU-03/05). Sin contradicciones.
- Cada CU enlaza a NB-04 (sin huérfanos). NB-04 queda cubierta por los 6 CU; cobertura bidireccional declarada y verificada.
- Los DX docs referencian los CU/RN del mismo proyecto: `dx-developer-experience` §0/§8, `guia-onboarding-developer`, `dx-error-messages` y `dx-portal-developers` citan CU-01..CU-06 y RN-01..RN-03 de la categoría 02 de `aplicada-sync`. Los códigos de error del catálogo derivan de las tablas de excepciones de los CU (verificado: CONFIGURACION_INCOMPLETA, SESION_*, BACKEND_INALCANZABLE, SUBIDA_INCOMPLETA, etc.).
- IDs no duplicados; enlaces relativos del índice y del README resuelven a archivos existentes.

### 5.2 `geovial-storage`

- Índice ↔ README ↔ CU ↔ RN: tablas coincidentes en IDs, títulos y RN aplicables. La matriz NB→CU→RN→US del índice (§5) es coherente con la §9 de cada CU y con los CU afectados de cada RN (RN-01: CU-01..06; RN-02: CU-01/02/04; RN-03: CU-02/05/06). Sin contradicciones.
- Cada CU enlaza a NB-07 como principal (sin huérfanos); NB-03 y NB-06 aparecen como soporte donde corresponde (CU-01/CU-04 con NB-03; CU-02/CU-05 con NB-06). Cobertura bidireccional declarada.
- Los DX docs referencian CU-01..CU-06 y RN-01..RN-03 del propio proyecto. El catálogo de errores deriva de las excepciones de los CU; agrega PROVEEDOR_NO_CONFIGURADO y TAMANIO_EXCEDIDO con nota explícita de que no figuran textualmente en 02 y se confirman en 05 (ver H-04).
- IDs no duplicados; enlaces relativos del índice y del README resuelven.

### 5.3 Coherencia con upstream de solución (00/01) y downstream (05/06/08)

- Upstream: ambos índices y CU citan SOLUTION-INTAKE (§17 del bloque del proyecto, §3, §7, §18) y la NB de origen del catálogo de 01. La NB de origen existe en `01_necesidades_negocio` (NB-04 y NB-07 confirmadas en el índice de NB).
- Downstream: cada CU enumera US a generar en 06, componentes esperados en 05 y tests previstos en 08, en términos tentativos y no vinculantes (conforme §3.3 y §4.4 de 02_rules). Los DX docs enumeran US a generar (US-01..US-13 en aplicada-sync; US-01..US-09 en geovial-storage) y tests en 08. El stack y el código se difieren correctamente a 05 y 11.
- IDs locales de US no colisionan dentro de cada proyecto. La numeración de US es local por proyecto, coherente con la convención adoptada por el orquestador.

---

## 6. Hallazgos enumerados

### H-01 — P2 — Desalineación del índice de NB con la numeración real de CU (reconciliación pendiente, ambos proyectos)
- Archivos: `01_necesidades_negocio/necesidades-negocio_v1.0.md` §2 (CU previstas) frente a `aplicada-sync/02_.../especificacion-funcional_v1.0.md` §3 y `geovial-storage/02_.../especificacion-funcional_v1.0.md` §3.
- Evidencia: el índice de NB de la Fase A proyectó un set plano tentativo CU-01..CU-17 ("a generar") y asignó a NB-04 las CU-10/CU-11 y a NB-07 la CU-17. En la generación real, `aplicada-sync` materializa NB-04 en 6 CU renumeradas localmente CU-01..CU-06, y `geovial-storage` materializa NB-07 en 6 CU renumeradas localmente CU-01..CU-06. La cantidad y la numeración difieren de la estimación del índice de NB.
- Análisis: la numeración local por proyecto (cada proyecto arranca en CU-01) es la convención explícita del orquestador y no se penaliza como P0. La trazabilidad upstream CU→NB es correcta en ambos proyectos (cada CU declara su NB; no hay huérfanos). Ambos índices documentan la renumeración local y la naturaleza tentativa del set plano de la Fase A (aplicada-sync §3 nota de numeración; geovial-storage §5 párrafo de cobertura). Es un ítem de reconciliación que corresponde resolver en la consolidación de Fase H, no un defecto de trazabilidad. Se clasifica P2 por ser una reconciliación pendiente entre el índice de 01 y la realidad de 02.
- Recomendación: en la Fase H, actualizar la columna "CU previstas" del índice de NB para reflejar la numeración local real por proyecto (NB-04 → aplicada-sync CU-01..CU-06; NB-07 → geovial-storage CU-01..CU-06) o anotar que las CU se numeran localmente por proyecto y que la estimación plana de Fase A queda superada. No bloquea la promoción de Fase B.

### H-02 — P3 — Uso aislado de "token" donde el resto del corpus usa "credencial" (aplicada-sync)
- Archivo: `aplicada-sync/02_especificacion_funcional/especificacion-funcional_v1.0.md` §2.2.
- Evidencia: "la autenticación y la emisión de credenciales (el motor reutiliza el token que le provee el host)". Es la única ocurrencia de "token" en los 25 documentos de la fase; el resto usa el término neutral "credencial" / "proveedor de credencial".
- Análisis: "token" es un término genérico de seguridad, no un stack concreto (no es JWT, OAuth ni bearer; el scan de stack es negativo). No rompe D7 ni trazabilidad. Se registra como P3 de consistencia léxica para mantener el vocabulario neutral uniforme y evitar que se lea como una insinuación de mecanismo concreto.
- Recomendación: reemplazar "token" por "credencial" para uniformar el vocabulario neutral del índice.

### H-03 — P3 — Orden de la sección opcional §17 antes de §11 en los CU de aplicada-sync
- Archivos: los 6 CU de `aplicada-sync` (CU-01 a CU-06).
- Evidencia: el orden de secciones es §10 Notas y supuestos → §17 Compatibilidad de versión pública → §11 Control de cambios. La sección opcional §17 (habilitada para `library` por 02_rules §4.3) se intercala antes del Control de cambios (§11), en lugar de cerrar el documento.
- Análisis: las 11 secciones obligatorias del §4.2 están todas presentes y §17 es la sección opcional legítima para `library`. La numeración no es contigua (10, 17, 11), lo que es puramente cosmético y no afecta completitud ni trazabilidad. `geovial-storage` evita el punto consolidando la compatibilidad en el §6 del índice en lugar de por CU, lo que también es válido. Se deja como P3 de claridad.
- Recomendación: ubicar §17 entre §10 y §11 sin reordenar el cierre, renumerándola como sección opcional posterior, o moverla después del Control de cambios; alternativamente, consolidar la nota de compatibilidad en el índice como hace geovial-storage. Mejora de orden, no bloqueante.

### H-04 — P3 — Dos códigos de error del catálogo de geovial-storage no provienen literalmente de las tablas de 02
- Archivo: `geovial-storage/03_ux_ui_dx/dx-error-messages_v1.0.md` §3 (códigos PROVEEDOR_NO_CONFIGURADO y TAMANIO_EXCEDIDO).
- Evidencia: ambos códigos figuran en el catálogo de 03 pero no aparecen con esos nombres en las secciones de excepciones de los CU de 02; el documento lo declara en sus "Notas del catálogo" y los deriva de las precondiciones comunes (proveedor activo configurado) y de la postcondición de guardado, marcando que el nombre definitivo se confirma en 05 y que el umbral de tamaño es un NFR pendiente (intake §17.P.10).
- Análisis: no es una invención silenciosa ni una incoherencia: el catálogo declara la derivación de forma transparente y la ata a precondiciones reales de los CU y a un PENDIENTE del intake, conforme al manejo de ambigüedad (master-prompt §9). No rompe trazabilidad. Aun así, conviene cerrar el bucle agregando estos dos escenarios como excepciones en los CU de 02 para que el catálogo de 03 no introduzca códigos ausentes en la fuente funcional. Se clasifica P3.
- Recomendación: en una iteración menor de 02, incorporar PROVEEDOR_NO_CONFIGURADO (CU-01..CU-05, derivado de la precondición de proveedor activo) y TAMANIO_EXCEDIDO (CU-01, derivado de la postcondición de guardado y del NFR de tamaño cuando 05 lo fije), o bien anotar en el índice de 02 que ambos se materializan en el contrato de 05.

### H-05 — P3 — Cabecera de los README de sección con campos parciales respecto del bloque §4.1
- Archivos: `aplicada-sync/02_.../README.md` y `aplicada-sync/03_.../README.md` (encabezado en prosa sin bloque de metadatos) frente a `geovial-storage/02_.../README.md` y `geovial-storage/03_.../README.md` (bloque de metadatos parcial: Proyecto, Tipo D8, Estado, Fecha, Autor; sin Versión por ser README, correcto).
- Evidencia: los README de `aplicada-sync` abren directamente con un H1 y prosa, sin el bloque de cabecera de campos; los de `geovial-storage` incluyen un bloque parcial. Ninguno de los dos estilos es incorrecto (el README de sección es recomendado, no obligatorio, y §4.1 prescribe la cabecera para los artefactos versionados, no necesariamente para el README navegable), pero la inconsistencia de estilo entre proyectos es visible.
- Análisis: cosmético; no afecta D1-D8 ni trazabilidad (ambos README declaran trazabilidad y enlaces que resuelven). P3 de consistencia.
- Recomendación: unificar el estilo de cabecera de los README de sección entre proyectos para una lectura homogénea de la solución (decisión de estilo a tomar en Fase H).

---

## 7. Veredicto final

### Por proyecto

- `aplicada-sync` (`library`, redistribuible): VEREDICTO APROBADO CON OBSERVACIONES. Cumple §6 de 02_rules (6 CU ≥ 5, 11 secciones por CU, 4 G/W/T con valores concretos, ≥1 excepción por CU, 3 RN con 7 secciones, sin modelo conceptual) y §6 de 03_rules variante DX (los tres DX docs obligatorios más el portal recomendado justificado, 9 secciones de dx-developer-experience, Diátaxis y onboarding 5/30/60 verificables, quick-start verificable y abstracto). Especificación agnóstica del dominio de GeoVial y coherente con su carácter redistribuible. Hallazgos: H-01 (P2, compartido), H-02 (P3), H-03 (P3). Sin P0 ni P1.

- `geovial-storage` (`library`, no redistribuible): VEREDICTO APROBADO CON OBSERVACIONES. Cumple §6 de 02_rules (6 CU ≥ 5, 11 secciones por CU, 4 G/W/T con valores concretos, ≥1 excepción por CU, 3 RN con 7 secciones, sin modelo conceptual) y §6 de 03_rules variante DX (los tres DX docs obligatorios, omisión justificada y declarada de dx-portal-developers, 9 secciones de dx-developer-experience, Diátaxis y onboarding 5/30/60 verificables, quick-start verificable y abstracto). Proveedores descritos de forma abstracta (local / remoto / otro), sin productos comerciales. Hallazgos: H-01 (P2, compartido), H-04 (P3), H-05 (P3 parcial). Sin P0 ni P1.

### Consolidado

VEREDICTO: APROBADO CON OBSERVACIONES.

Fundamento: no se detectó ningún hallazgo P0 ni P1 en ninguno de los dos proyectos de nivel 0. Ambos cumplen los criterios de aceptación del §6 de `02_rules_especificacion_funcional.md` y del §6 de `03_rules_ux_ui_dx.md` (variante DX) para el tipo `library`, respetan D1-D8 (incluida la prohibición de stack y de vocabulario del dominio fuente del bootstrap, con scan léxico negativo), mantienen la trazabilidad upstream CU→NB correcta y bidireccional, y presentan filenames y estructura de carpetas correctos bajo `proyectos/<kebab>/` con subcarpetas `casos-de-uso/` y `reglas-de-negocio/`. La categoría `04_prompts_ai` está correctamente ausente en ambos proyectos (usa_llm=false). Conforme a la regla del veredicto (master-prompt §10), la ausencia de P0 habilita avanzar a la Fase C de estos proyectos.

Condiciones recomendadas (no bloquean la promoción a Fase C):
1. Reconciliar el índice de NB con la numeración local real de CU en la Fase H (H-01).
2. Uniformar "token" → "credencial" en el índice de aplicada-sync (H-02).
3. Reubicar la sección opcional §17 al cierre de los CU de aplicada-sync, o consolidarla en el índice (H-03).
4. Incorporar PROVEEDOR_NO_CONFIGURADO y TAMANIO_EXCEDIDO como excepciones en los CU de 02 de geovial-storage, o anotarlos en su índice (H-04).
5. Unificar el estilo de cabecera de los README de sección entre proyectos (H-05).

Hallazgos por nivel: P0 = 0 · P1 = 0 · P2 = 1 · P3 = 4.

---

## Control de cambios

| Versión | Fecha | Cambios | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-06-15 | Auditoría independiente inicial de la Fase B (02 y 03 DX) de los proyectos de nivel 0 de GeoVial (`aplicada-sync` y `geovial-storage`). Veredicto consolidado APROBADO CON OBSERVACIONES (0 P0, 0 P1, 1 P2, 4 P3). Verificada la omisión correcta de la categoría 04. | Auditor independiente (Arquitecto de Soluciones + QA Senior) |
