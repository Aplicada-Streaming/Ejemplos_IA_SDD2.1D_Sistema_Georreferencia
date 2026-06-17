# Auditoría Fase G — Examples (11) — Nivel 2

**Documento:** G-examples-nivel2_v1.0.md
**Fase auditada:** G (categoría `11_examples`)
**Alcance:** proyectos de nivel 2 `geovial-web` (`web-monolith`, categoría 11 RECOMENDADA) y `geovial-mobile` (`mobile-app-maui`, categoría 11 OBLIGATORIA)
**Auditor:** Arquitecto de Soluciones + QA Senior (independiente, no participó de la generación)
**Fecha:** 2026-06-16
**Estado:** Vigente
**Reglas aplicadas:** `SDD2.1D/devs/rules/11_rules_examples.md` (§6; §2.2 web-monolith y mobile-app-maui); master-prompt §10
**Insumos upstream consultados:** `02_especificacion_funcional/` (CU/RN) y `05_arquitectura_tecnica/adrs/` de cada proyecto; `SOLUTION-INTAKE-geovial_v1.0.md` §16.1, §17 P.9/P.10/P.11.
**Nota de alcance:** en esta fase de documentación NO se materializa código en `/samples/`; se auditan solo los markdown explicativos y el README de cada proyecto.
**Veredicto consolidado:** APROBADO CON OBSERVACIONES (sin P0)

---

## 1. Resumen ejecutivo

Se auditaron 5 entregables: 2 de `geovial-web` (README + `ejemplo-01-datos-seed`) y 3 de `geovial-mobile` (README + `ejemplo-01-app-basica` + `ejemplo-02-sync-offline`). `geovial-web` es `web-monolith` (piso de 1-2 samples: datos seed obligatorio; tema custom solo con punto de extensión visual). `geovial-mobile` es `mobile-app-maui` (piso de 3 samples; el multiplataforma es omisible solo con justificación de plataforma única). Ambos proyectos cumplen su piso efectivo: web presenta `datos-seed` y omite `tema-custom` con justificación; mobile presenta los dos samples mínimos efectivos (`app-basica` + `sync-offline`) y omite `multiplatform-demo` con justificación de target Android único.

Ningún proyecto presenta hallazgos P0: no falta documento obligatorio efectivo, las omisiones están registradas con motivo, no hay slug por dominio del proyecto, no hay stack ni vocabulario del dominio fuente del bootstrap en prosa, las cabeceras y las nueve secciones obligatorias están completas y en orden, "Cómo correrlo" no supera los cinco pasos en ningún markdown, y la trazabilidad upstream (CU/RN/ADR de 02 y 05) resuelve contra artefactos vigentes (CU-01..11 y RN/ADR del web; CU-01..07 y RN/ADR del mobile, todos verificados en disco).

Los hallazgos abiertos son de severidad media y baja: el README de geovial-web afirma "el flag de extensibilidad del front es false" como dato del intake, pero ese flag no existe literalmente en §17 geovial-web (el intake solo declara `tiene_persistencia=false` y `tiene_observabilidad_critica=false`, y §16.1 deja el tema custom como condicional "si hay punto de extensión visual… Detalle: PENDIENTE"); la justificación de omisión es sólida por contenido pero atribuye al intake un flag inexistente (P2). El README de geovial-mobile §2.1 invoca "el mismo criterio que `aplicada-sync`" para la omisión multiplataforma, pero `aplicada-sync` es `library` y no omite ningún sample multiplataforma en §16.1; la analogía es imprecisa aunque la causa real (Android-only de §17 P.9) sea correcta y suficiente (P3). Inconsistencia menor de cabecera: el campo "Estado" es "Propuesto" en los 5 archivos (coherente entre sí), pero el README de cada proyecto suma un campo "Audiencia" no previsto en el bloque de cabecera de §4.3 (P3).

**Conteo por nivel: P0: 0 — P1: 0 — P2: 1 — P3: 2.**

---

## 2. Matriz D1-D8 (idioma, codificación, nomenclatura, vocabulario)

| Criterio | geovial-web 11 | geovial-mobile 11 |
| --- | --- | --- |
| D1 Idioma rioplatense técnico, sin emojis ni negrita decorativa | Cumple | Cumple |
| D2 UTF-8 sin BOM / LF | Cumple (verificado byte a byte) | Cumple (verificado byte a byte) |
| D3 kebab-case en filenames | Cumple | Cumple |
| D4 Sufijo `_vX.Y` (nunca `.v`) | Cumple | Cumple |
| D5 Sin vocabulario del dominio fuente del bootstrap (multa/factura/recibo/infracción/nuget) | Cumple (búsqueda negativa) | Cumple (búsqueda negativa) |
| D6 Slug por capacidad, no por dominio del proyecto | Cumple (`datos-seed`) | Cumple (`app-basica`, `sync-offline`) |
| D7 Cuerpo stack-abstract (sin .NET/Blazor/MudBlazor/SignalR/MAUI/SQLite/JWT/Leaflet/OpenStreetMap en prosa) | Cumple | Cumple (Android admisible con mesura, ver §3) |
| D8 Sin slug comercial/dominio hardcodeado en el nombre del markdown | Cumple | Cumple |

Notas de verificación:

- Codificación (D2): los 5 archivos son UTF-8 sin BOM y con terminadores LF puros, verificado byte a byte (CRLF=0, lone-LF=N, lone-CR=0, primeros 3 bytes `23 20 …` = `# `, sin secuencia BOM `ef bb bf`; em-dash multibyte `e2 80 94` correcto). No hay CRLF ni BOM. El `.gitattributes` del repo fija `text=set, eol=lf` para estos paths, coherente con lo observado en disco.
- Sufijo de versión (D4): los tres markdown explicativos llevan `_v1.0.md`; los dos README van sin sufijo por convención de índice (admitido por §3.1). No aparece el patrón heredado `.vX`.
- D5: búsqueda negativa de `multa`, `factura`, `recibo`, `infracción`, `nuget` en las dos carpetas: sin coincidencias.
- D6/D8 (slug por capacidad): web usa `datos-seed`; mobile usa `app-basica` y `sync-offline`; los tres son valores admitidos por §3.1 y describen la capacidad demostrada, no la entidad del dominio. El vocabulario de dominio propio del producto (relevamiento, marcador, tramo vial, agente) aparece en la prosa como objeto que el sample siembra/captura, lo cual es legítimo: D5 prohíbe el dominio fuente del bootstrap (sistema de multas), no el dominio del propio producto GeoVial.
- D7 (stack-abstract): la prosa usa neutralizadores consistentes ("motor de contenedores", "composición de demostración", "gestor de paquetes del ecosistema", "ecosistema móvil", "almacenamiento seguro del dispositivo", "componente de mapa", "componente de sincronización del producto", `App.<ext>`, `.<ext>`). Búsqueda negativa de `.NET`, `dotnet`, `Blazor`, `MudBlazor`, `SignalR`, `MAUI`, `SQLite`, `JWT`, `Leaflet`, `OpenStreetMap`, `Xamarin`, `net8`: sin coincidencias en prosa en ninguno de los dos proyectos. El ADR-03 del web se cita como "autenticación por token bearer" (concepto), no como JWT. Cumple.

---

## 3. Token `android` (admisibilidad con mesura, geovial-mobile)

El master-prompt §10.1 admite "Android" con mesura como plataforma target exclusivamente en `geovial-mobile`. Verificación de todas las ocurrencias:

| Archivo:línea | Contexto | ¿Admisible? |
| --- | --- | --- |
| `README.md:34,36,63,72` | Justificación de la omisión del sample `03-multiplatform-demo` por target Android único | Sí: nombrar la plataforma target es necesario para justificar la omisión |
| `ejemplo-01-...:23` / `ejemplo-02-...:24` | Prerequisite: "plataforma target Android, versión mínima Android API 26" remitida a §17 P.9 | Sí: target y versión mínima, alineado con §17 P.9 |
| `ejemplo-01-...:24,31` / `ejemplo-02-...:25,32` | "dispositivo Android conectado por USB en modo desarrollador" | Sí: identifica el dispositivo de prueba, en línea con §17 P.9 (depuración USB) |

Conclusión: `Android` aparece solo como plataforma target / dispositivo de prueba en `geovial-mobile`, nunca como vocabulario de stack (no aparece `net8.0-android`, ni `MAUI`, ni el framework). El uso es proporcional y trazable a §17 P.9. No aparece "Android" en ningún archivo de `geovial-web`. **Sin hallazgo.**

---

## 4. Matriz de estructura — 9 secciones obligatorias por markdown (§4.2)

Cabecera (§4.1) con H1, Proyecto, Documento, Versión, Estado, Fecha, Autor, Nivel y Ubicación del código presente en los 3 markdown explicativos. Las nueve secciones aparecen en orden y con su título canónico.

| Sección (§4.2) | web e01 (datos-seed) | mobile e01 (app-basica) | mobile e02 (sync-offline) |
| --- | --- | --- | --- |
| Cabecera con Nivel + Ubicación del código | Sí (Básico, `/samples/geovial-web/01-datos-seed/`) | Sí (Básico, `/samples/geovial-mobile/01-app-basica/`) | Sí (Intermedio/Avanzado, `/samples/geovial-mobile/02-sync-offline/`) |
| 1. Objetivo del sample | Sí | Sí | Sí |
| 2. Nivel (declarado + justificado) | Sí | Sí | Sí |
| 3. Prerequisites (con versión mínima cuando aplica) | Sí | Sí (API 26 vía §17 P.9) | Sí (API 26 vía §17 P.9) |
| 4. Cómo correrlo (≤5 pasos) | Sí (5) | Sí (5) | Sí (5) |
| 5. Estructura del código | Sí (árbol) | Sí (árbol) | Sí (árbol) |
| 6. Qué esperar (output exacto) | Sí (bloque + traza de bootstrap) | Sí (3 bloques de traza) | Sí (4 bloques de traza por fase) |
| 7. Variaciones sugeridas (tabla 2-4) | Sí (4) | Sí (4) | Sí (5) |
| 8. Trazabilidad (≥1 CU/ADR/NFR) | Sí (8 CU + 3 RN + 4 ADR + NFR) | Sí (2 CU + 1 RN + 2 ADR + NFR) | Sí (5 CU + 4 RN + 4 ADR + NFR) |
| 9. Control de cambios | Sí | Sí | Sí |

Observaciones de estructura:

- Nivel declarado y justificado en §2 en los 3 markdown. web e01 justifica que es el único del piso `web-monolith`; mobile e01 declara "punto de entrada absoluto"; mobile e02 declara "intermedio/avanzado… equivalente al nivel avanzado del tipo" y explicita qué agrega respecto del sample 01. Cumple §4.2.2 y el anti-patrón "samples sin nivel declarado".
- "Cómo correrlo" tiene exactamente 5 pasos copiables en los 3 markdown; ninguno supera el tope de §4.2.4.
- "Qué esperar" trae output exacto en bloque literal en los 3. mobile e02 lo segmenta por fase (captura sin conexión / sincronización / reanudación) con la traza exacta de cada una, e incluso enuncia el criterio de éxito de orden ("la fase de bajada nunca aparece antes…"). web e01 documenta tanto el recorrido por pantalla como la traza textual que imprime el bootstrap. Cumple §4.2.6.
- Variaciones sugeridas dentro del rango 2-4 de §4.2.7 en web e01 (4) y mobile e01 (4); mobile e02 trae 5, un ítem por encima del rango sugerido. Es un excedente menor, no un incumplimiento (la regla pide "dos a cuatro"; cinco no rompe trazabilidad ni criterio de aceptación de §6, que no fija tope). No se eleva a hallazgo; se anota como nota de estilo.
- Prerequisites con versión mínima: mobile remite API 26 a §17 P.9 (consistente con el intake). web no fija versiones numéricas de runtime de contenedor/navegador pero declara "navegador evergreen (últimas dos versiones mayores)" y "motor de contenedores… versión moderna con soporte de composición multiservicio", criterio admitido por §4.2.3 (versión mínima "cuando aplica"). Cumple.

---

## 5. Matriz README de la sección (§4.3 y §4.4)

| Requisito | geovial-web README | geovial-mobile README |
| --- | --- | --- |
| Propósito de la carpeta (docs vs /samples) | §1 presente | §1 presente |
| Tabla maestra con 5 columnas (Sample, Nivel, Tiempo de setup, CU ilustrados, Ubicación) | §2 completa | §2 completa |
| Convenciones de los samples | §3 presente | §3 presente |
| Cómo agregar un sample nuevo (ref §6 reglas) | §5 presente | §4 presente |
| Vínculo con 10 y 05 | §6 presente, ADR/CU verificados | §5 presente, ADR/CU verificados |
| Replica resumida tipo D8 → /samples (§2.3) | §7 presente | §6 presente |
| Registro de omisión (con motivo) | §4 (tema-custom) + §7 | §2.1 (multiplatform-demo) + §6 |
| Control de cambios | §8 presente | §7 presente |

- Tiempo de setup declarado por sample en ambas tablas maestras: web `< 5 min`; mobile `< 5 min` y `15-25 min`. Cumple el criterio §6 "cada sample declara tiempo de setup en la tabla maestra".
- Cobertura de CU declarada bajo la tabla maestra: web lista CU-01..08 en la columna y enumera CU-09/10/11 como variaciones sugeridas (nota bajo la tabla); mobile reparte CU-01,02 (e01) y CU-03..07 (e02) y declara la cobertura total CU-01..07. Lectura columnar limpia en ambos. Cumple §4.4.
- Ambos README registran la omisión del segundo/tercer sample con motivo explícito y referencia al intake, satisfaciendo el requisito de "omisión registrada con motivo" del master-prompt §10.3.

---

## 6. Coherencia cross-doc y trazabilidad

### 6.1 geovial-web

- CU referenciados existen todos en `02_especificacion_funcional/casos-de-uso/`: el sample enlaza CU-01..08 con ruta relativa `../02_especificacion_funcional/casos-de-uso/CU-0X-...-_v1.0.md`; los 8 archivos existen en disco (catálogo verificado CU-01..CU-11). Los enlaces relativos resuelven (el markdown vive en `geovial-web/11_examples/`, `../02_...` apunta a `geovial-web/02_...`).
- CU-09 (carga manual web), CU-10 (exportar/importar) y CU-11 (configurar destino de almacenamiento) no se ilustran directamente pero se declaran explícitamente como variaciones sugeridas sobre el mismo conjunto seed (§7 del markdown y nota de cobertura del README §2), cumpliendo el criterio de cobertura "al menos tangencialmente, declarado" del master-prompt §10.4.
- RN referenciados (RN-01, RN-04, RN-05) existen en `02/reglas-de-negocio/` (catálogo RN-01..05). Sin referencia colgada.
- ADR referenciados (ADR-01, ADR-02, ADR-03, ADR-04) existen en `05/adrs/` (catálogo ADR-01..06). El markdown cita el ADR-05 por concepto (mapeo de errores a feedback) en el README §6 y no lo enlaza en §8; no hay referencia rota. Sin ADR colgado.
- NFR: el markdown cita "intake §17 geovial-web P.10" con "latencia de interacción p95 ≤ 200 ms". Verificado contra §17 geovial-web P.10 (texto literal: "latencia de interacción p95 <= 200 ms sobre el circuito en red estable"). Cita exacta.
- Ubicación del código (`/samples/geovial-web/01-datos-seed/`) coincide con la matriz §2.3 para `web-monolith` (`01-datos-seed/`) y con el intake §16.1 ("`geovial-web`… datos seed"). La namespace `geovial-web/` bajo `/samples/` es consistente con la estructura multiproyecto del intake §16.
- Omisión `02-tema-custom`: registrada en README §4 y §7 con motivo (sin punto de extensión visual). Coherencia parcial: la base es §17 geovial-web P.11 (biblioteca de componentes y mapa fijados sin mecanismo de tematización extensible), que es correcta; pero la afirmación "el flag de extensibilidad del front es false" atribuye al intake un flag que NO existe literalmente en §17 geovial-web (solo están `tiene_persistencia=false` en P.4 y `tiene_observabilidad_critica=false` en P.10). Además §16.1 deja el tema custom condicional ("si hay punto de extensión visual… Detalle: PENDIENTE"), no como negación cerrada. La omisión es razonable por contenido pero la justificación invoca un flag inexistente → hallazgo G-01 (P2).

### 6.2 geovial-mobile

- CU referenciados existen todos en `02/casos-de-uso/`: e01 cita CU-01, CU-02; e02 cita CU-03..07. Catálogo verificado CU-01..CU-07 (los 7 archivos en disco). Entre los dos samples se cubre la totalidad CU-01..07, cumpliendo §2.2 ("entre los 2 samples se cubre el grueso de CU-01..07") y la cobertura del master-prompt §10.4 (ningún CU del MVP sin ilustrar). Nota: la trazabilidad mobile cita los CU por identificador sin hipervínculo relativo (a diferencia del web), lo cual es admisible: §4.2.8 pide "enlaza al artefacto fuente" y el identificador resuelve unívocamente contra el catálogo; se anota como diferencia de estilo, no hallazgo.
- RN referenciados (e01: RN-04; e02: RN-01, RN-02, RN-03, RN-05) existen en `02/reglas-de-negocio/` (catálogo RN-01..05). Sin referencia colgada.
- ADR referenciados (e01: ADR-01, ADR-05; e02: ADR-01, ADR-02, ADR-03, ADR-04) existen en `05/adrs/` (catálogo ADR-01..06). El README §5 enumera ADR-01..05 de forma consistente con su descripción (offline-first, almacén local con migraciones, motor subir-luego-bajar, permisos con degradación, token seguro con relogueo). Sin ADR colgado.
- NFR: e01 cita "arranque en frío ≤ 3 s (intake §17 P.10)"; e02 cita "captura 100 % offline, cola ≥ 1000 cambios, lote de 100 en ≤ 30 s, reanuda sin pérdida (intake §17 P.10)". Verificado contra §17 geovial-mobile P.10 (texto literal: "captura… 100 % sin conexión; la cola local tolera >= 1000 cambios; un ciclo de sincronización de 100 cambios completa <= 30 s… reanuda sin pérdida… arranque en frío de la app <= 3 s"). Citas exactas.
- Ubicación del código (`/samples/geovial-mobile/01-app-basica/`, `…/02-sync-offline/`) coincide con la matriz §2.3 para `mobile-app-maui` (las dos primeras carpetas canónicas) y con el intake §16.1 ("`geovial-mobile`… samples de app básica y sync offline").
- Omisión `03-multiplatform-demo`: registrada en README §2.1 y §6 con motivo (target Android único). Verificada contra el intake §16.1 ("El sample multiplataforma se omite porque el único target es Android, ver §17 P.9"), §17 geovial-mobile P.9 ("Android únicamente; no se soportan iOS ni Windows en v1, decisión confirmada por el cliente") y el changelog del intake v1.2 ("§16.1 elimina el sample multiplataforma de geovial-mobile"). La causa es correcta y suficiente. Salvedad: la frase "registrada en el intake §16.1 con el mismo criterio que `aplicada-sync`" es imprecisa: `aplicada-sync` es `library` y su entrada en §16.1 no omite ningún sample multiplataforma; la analogía correcta es que ambos comparten la decisión Android-only de P.9, no una omisión homóloga → hallazgo G-02 (P3).

---

## 7. Cobertura de CU (síntesis §10.4)

| Proyecto | CU ilustrados directamente | CU declarados como variación / tangencial | CU del MVP sin cubrir |
| --- | --- | --- | --- |
| geovial-web | CU-01, CU-02, CU-03, CU-04, CU-05, CU-06, CU-07, CU-08 (e01) | CU-09, CU-10, CU-11 (variaciones sugeridas §7 + nota README) | Ninguno (todos cubiertos o declarados) |
| geovial-mobile | CU-01, CU-02 (e01); CU-03, CU-04, CU-05, CU-06, CU-07 (e02) | — | Ninguno (CU-01..07 íntegramente ilustrados) |

Ningún CU crítico del MVP queda sin ilustrar al menos tangencialmente y declarado. En web, los CU-09/10/11 (carga manual, portabilidad y configuración de almacenamiento) se cubren mediante variaciones sobre el conjunto seed, explícitamente enunciadas; el seed deja el estado inicial sobre el que se ejercitan. En mobile, los dos samples cubren la totalidad CU-01..07 sin huecos. Cumple los criterios de cobertura del master-prompt §10.4.

---

## 8. Hallazgos

| ID | Nivel | Proyecto | Archivo / sección | Evidencia | Recomendación |
| --- | --- | --- | --- | --- | --- |
| G-01 | P2 | geovial-web | `11_examples/README.md` §4 (sample omitido: tema custom) | La justificación de la omisión afirma "el flag de extensibilidad del front es false". Ese flag no existe en `SOLUTION-INTAKE-geovial_v1.0.md` §17 geovial-web (solo `tiene_persistencia=false` en P.4 y `tiene_observabilidad_critica=false` en P.10); §16.1 deja el tema custom condicional ("si hay punto de extensión visual… Detalle: PENDIENTE"). La omisión es válida por contenido (P.11 fija biblioteca de componentes y mapa sin tematización extensible), pero atribuye al intake un flag inexistente | Reformular la justificación apoyándola en §17 geovial-web P.11 (decisiones pre-tomadas que fijan la biblioteca de componentes y el componente de mapa, sin mecanismo de tematización extensible) y en §16.1 (condicionalidad PENDIENTE resuelta como "sin punto de extensión visual"), sin invocar un flag de extensibilidad que el intake no declara |
| G-02 | P3 | geovial-mobile | `11_examples/README.md` §2.1 (sample omitido) | "La omisión está registrada en el intake §16.1 con el mismo criterio que `aplicada-sync`". `aplicada-sync` es `library` y su entrada en §16.1 no omite ningún sample multiplataforma; la analogía es imprecisa aunque la causa real (Android-only de §17 P.9) sea correcta y suficiente | Quitar la analogía con `aplicada-sync` o reformularla como "comparte con `aplicada-sync` la decisión de target Android único de §17 P.9", sin sugerir una omisión homóloga inexistente |
| G-03 | P3 | ambos | Cabecera de los dos README (campo "Audiencia") | Ambos README incluyen un campo "Audiencia:" no previsto en el bloque de cabecera del README de §4.3; los markdown explicativos no lo llevan. Inocuo y consistente entre proyectos, pero fuera del modelo de cabecera de la regla | Inocuo; opcionalmente documentar el campo "Audiencia" como extensión admitida del README o retirarlo para ceñirse a §4.3 |

No se detectaron hallazgos P0 ni P1.

Notas de estilo (no elevadas a hallazgo): mobile e02 §7 trae 5 variaciones sugeridas, una por encima del rango "dos a cuatro" de §4.2.7 (sin tope en §6, no rompe criterio de aceptación). La trazabilidad de mobile cita los CU por identificador sin hipervínculo relativo, a diferencia del web que sí enlaza; ambas formas resuelven unívocamente contra el catálogo de 02. El campo "Estado: Propuesto" es uniforme en los 5 archivos.

---

## 9. Veredicto

### 9.1 Por proyecto

- **geovial-web — APROBADO CON OBSERVACIONES (sin P0).** Cumple el piso efectivo de `web-monolith`: sample `datos-seed` presente con las nueve secciones y cabecera (Nivel + Ubicación del código), `tema-custom` omitido con justificación, README con tabla maestra de cinco columnas, `≤5` pasos en "Cómo correrlo", output exacto en §6, trazabilidad a CU-01..08, RN-01/04/05, ADR-01..04 y NFR P.10, todos resueltos contra artefactos vigentes, y CU-09/10/11 cubiertos como variaciones declaradas. Observación abierta: G-01 (P2, justificación de omisión invoca un flag de extensibilidad inexistente en el intake) y participación en G-03 (P3). Habilitado para avanzar.
- **geovial-mobile — APROBADO CON OBSERVACIONES (sin P0).** Cumple los dos samples mínimos efectivos de `mobile-app-maui`: `app-basica` + `sync-offline` con las nueve secciones y cabecera, `multiplatform-demo` omitido con justificación de target Android único (trazada a §16.1, §17 P.9 y changelog v1.2 del intake), README con tabla maestra, `≤5` pasos, output exacto por fase en §6, cobertura íntegra de CU-01..07 entre los dos samples, y trazabilidad a RN-01..05 y ADR-01..05 vigentes. "Android" usado con mesura solo como plataforma target. Observaciones abiertas: G-02 (P3, analogía imprecisa con `aplicada-sync`) y participación en G-03 (P3). Habilitado para avanzar.

### 9.2 Consolidado

**APROBADO CON OBSERVACIONES (sin P0).** Ambos proyectos cumplen el contrato de la Fase G para su `project_type`: web-monolith con su sample obligatorio y la omisión justificada del tema custom; mobile-app-maui con sus dos samples mínimos efectivos y la omisión justificada del multiplataforma. No hay ningún hallazgo P0 ni P1 que obligue a RECHAZADO: trazabilidad intacta, D1-D8 conformes (incluida D2 LF/UTF-8 sin BOM verificada byte a byte), sin slug por dominio, sin stack ni vocabulario del dominio fuente en prosa, cabeceras y nueve secciones completas, y todas las omisiones registradas con motivo. Los hallazgos abiertos (1×P2, 2×P3) son de precisión de la justificación y de consistencia de cabecera, y no rompen trazabilidad, nomenclatura ni cobertura.

**Conteo por nivel: P0: 0 — P1: 0 — P2: 1 — P3: 2.**

---

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-16 | Auditoría inicial de la Fase G (Examples / categoría 11) para los proyectos de nivel 2 geovial-web (web-monolith) y geovial-mobile (mobile-app-maui). Verifica D1-D8 (con chequeo byte a byte de LF/UTF-8 sin BOM), las 9 secciones obligatorias por markdown, la tabla maestra de cada README, la admisibilidad con mesura del token Android en mobile, la coherencia cross-doc con 02/05 y el intake §16.1/§17 P.9/P.10/P.11, las omisiones registradas (tema-custom; multiplatform-demo) y la cobertura de CU (web CU-01..11; mobile CU-01..07). Veredicto consolidado APROBADO CON OBSERVACIONES (sin P0). Hallazgos: G-01 (P2), G-02 (P3), G-03 (P3). |
