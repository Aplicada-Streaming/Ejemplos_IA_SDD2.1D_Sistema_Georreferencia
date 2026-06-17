# Auditoría Fase G — Examples (11) — Nivel 0

**Documento:** G-examples-nivel0_v1.0.md
**Fase auditada:** G (categoría `11_examples`)
**Alcance:** proyectos de nivel 0 `aplicada-sync` y `geovial-storage` (ambos `library`, `equipo_n=1`)
**Auditor:** Arquitecto de Soluciones + QA Senior (independiente, no participó de la generación)
**Fecha:** 2026-06-15
**Estado:** Vigente
**Reglas aplicadas:** `SDD2.1D/devs/rules/11_rules_examples.md` (§6, variante `library`); master-prompt §10
**Nota de alcance:** en esta fase de documentación NO se materializa código en `/samples/`; se auditan solo los markdown explicativos y el README de cada proyecto.
**Veredicto consolidado:** APROBADO CON OBSERVACIONES (sin P0)

---

## 1. Resumen ejecutivo

Se auditaron 8 entregables: 4 de `aplicada-sync` (README + 3 samples) y 4 de `geovial-storage` (README + 3 samples). Ambos proyectos son `library`, para los que la categoría 11 es obligatoria con piso de 3 samples (básico + intermedio + avanzado). Los dos proyectos cumplen ese piso, tienen README con tabla maestra de cinco columnas, y cada markdown trae cabecera con Nivel y Ubicación del código más las nueve secciones obligatorias en orden.

Ningún proyecto presenta hallazgos P0: no falta documento obligatorio, no hay slug nombrado por dominio del proyecto, no hay stack ni vocabulario del dominio fuente del bootstrap en prosa, las cabeceras y las nueve secciones están completas, hay 3 samples por proyecto y la trazabilidad upstream (02/05) está intacta y resuelve contra artefactos vigentes.

El punto crítico de la auditoría —el token `maui` del sample avanzado de aplicada-sync— está limpio: aparece exclusivamente como parte del nombre de carpeta canónico `03-avanzado-demo-maui`, definido literalmente en el intake §16.1 (árbol de `/samples`) y §18 (estrategia de samples), citado como Ubicación del código, en el árbol de `/samples` del README y en el comando `cd`. Nunca aparece en prosa como vocabulario de stack. El slug del markdown es por progresión (`ejemplo-03-avanzado-integracion-real_v1.0.md`), no por carpeta ni por stack. Cumple la regla crítica.

Los hallazgos abiertos son de severidad media y baja: la tabla maestra del README de aplicada-sync declara la cobertura de CU de forma anidada ("CU-04 (más CU-01, CU-03 en integración)") que dificulta la lectura columna a columna (P2); inconsistencia de Estado de cabecera entre proyectos (aplicada-sync "Propuesto" vs geovial-storage "Vigente") y respecto del upstream (P3); y el README de aplicada-sync incluye un campo "Audiencia" no contemplado en el bloque de cabecera mientras geovial-storage no lo lleva (P3).

**Conteo por nivel: P0: 0 — P1: 0 — P2: 1 — P3: 2.**

---

## 2. Matriz D1-D8 (idioma, codificación, nomenclatura, vocabulario)

| Criterio | aplicada-sync 11 | geovial-storage 11 |
| --- | --- | --- |
| D1 Idioma rioplatense técnico, sin emojis ni negrita decorativa | Cumple | Cumple |
| D2 UTF-8 sin BOM / LF | Cumple (verificado byte a byte) | Cumple (verificado byte a byte) |
| D3 kebab-case en filenames | Cumple | Cumple |
| D4 Sufijo `_vX.Y` (nunca `.v`) | Cumple | Cumple |
| D5 Sin vocabulario del dominio fuente del bootstrap (multa/factura/recibo/infracción) | Cumple (búsqueda negativa) | Cumple (búsqueda negativa) |
| D6 Slug por progresión, no por dominio del proyecto | Cumple | Cumple |
| D7 Cuerpo stack-abstract (código va en /samples; sin .NET/MAUI/SQLite/S3/NuGet en prosa) | Cumple | Cumple |
| D8 Sin slug comercial hardcodeado en el nombre del markdown | Cumple | Cumple |

Notas de verificación:

- Codificación: los 8 archivos son UTF-8 sin BOM y con terminadores LF (verificado con `od`/búsqueda de `\r`); no hay CRLF ni BOM.
- No aparece ningún patrón heredado `.vX` en filenames ni cuerpos (búsqueda negativa confirmada). Todos los markdown explicativos llevan `_v1.0.md`; los dos README van sin sufijo por convención de índice (admitido por §3.1).
- D7 (stack-abstract): la prosa usa neutralizadores de stack consistentes ("runtime objetivo del ecosistema", "gestor de paquetes del ecosistema", "paquete distribuible de la librería", `Programa.<ext>`, `Program.<ext>`, `.<ext>`). Búsqueda negativa de `.NET`, `dotnet`, `MAUI`, `SQLite`, `S3`, `NuGet`, `Xamarin`, `Android`, `Azure`, `AWS`, `blob`, `MinIO`, `Aplicada.Sync` en prosa: sin coincidencias en ninguno de los dos proyectos.
- D5: búsqueda negativa de `multa`, `factura`, `recibo`, `infracción`, `nuget` en ambas carpetas: sin coincidencias.

---

## 3. Punto crítico de la auditoría — token `maui` (regla §10.3)

Verificación exhaustiva del token `maui` en los entregables de aplicada-sync (todas las ocurrencias):

| Archivo:línea | Contexto de la ocurrencia | ¿Path canónico o prosa de stack? |
| --- | --- | --- |
| `ejemplo-03-...:10` | `**Ubicación del código:** `/samples/aplicada-sync/03-avanzado-demo-maui/`` | Path canónico (Ubicación del código) |
| `ejemplo-03-...:30` | `cd samples/aplicada-sync/03-avanzado-demo-maui` (paso de "Cómo correrlo") | Path canónico (comando) |
| `ejemplo-03-...:39` | `03-avanzado-demo-maui/` (raíz del árbol de §5) | Path canónico (estructura del código) |
| `README.md:27` | Columna Ubicación de la tabla maestra | Path canónico |
| `README.md:58` | Árbol de `/samples` (§6, replica de §2.3) | Path canónico |
| `README.md:60` | "La carpeta del sample avanzado conserva el nombre canónico `03-avanzado-demo-maui` definido en el intake §16.1" | Nota que explicita que es nombre canónico de carpeta |

Conclusión: el token aparece SOLO como nombre de carpeta canónico, nunca como vocabulario de stack en prosa. El nombre `03-avanzado-demo-maui` está literalmente fijado upstream: intake §16.1 (árbol de `/samples`, línea con comentario "demo MAUI ajeno al sistema") e intake §18 (sample distintivo pedido), y reconfirmado en `05/extensibilidad_v1.0.md` §6 que apunta a `/samples/aplicada-sync/03-avanzado-demo-maui/`. El markdown explicativo se nombra por progresión (`ejemplo-03-avanzado-integracion-real`), separando correctamente el nombre de carpeta canónico del slug del documento. **Sin hallazgo.**

---

## 4. Matriz de estructura — 9 secciones obligatorias por markdown (§4.2)

Las nueve secciones aparecen en orden y con el título canónico en los 6 markdown. Cabecera (§4.1) con H1, Proyecto, Documento, Versión, Estado, Fecha, Autor, Nivel y Ubicación del código presente en los 6.

| Sección (§4.2) | a-sync e01 | a-sync e02 | a-sync e03 | gv e01 | gv e02 | gv e03 |
| --- | --- | --- | --- | --- | --- | --- |
| Cabecera con Nivel + Ubicación | Sí | Sí | Sí | Sí | Sí | Sí |
| 1. Objetivo del sample | Sí | Sí | Sí | Sí | Sí | Sí |
| 2. Nivel (declarado + justificado) | Sí | Sí | Sí | Sí | Sí | Sí |
| 3. Prerequisites | Sí | Sí | Sí | Sí (tabla c/versión) | Sí (tabla c/versión) | Sí (tabla c/versión) |
| 4. Cómo correrlo (≤5 pasos) | Sí (5) | Sí (5) | Sí (5) | Sí (5) | Sí (5) | Sí (5) |
| 5. Estructura del código | Sí | Sí | Sí | Sí | Sí | Sí |
| 6. Qué esperar (output exacto) | Sí | Sí | Sí | Sí | Sí | Sí |
| 7. Variaciones sugeridas (tabla) | Sí (3) | Sí (4) | Sí (4) | Sí (3) | Sí (4) | Sí (4) |
| 8. Trazabilidad (≥1 CU/ADR/NFR) | Sí | Sí | Sí | Sí | Sí | Sí |
| 9. Control de cambios | Sí | Sí | Sí | Sí | Sí | Sí |

Observaciones de estructura:

- Nivel declarado en §2 en los 6 (Básico/Intermedio/Avanzado), con justificación explícita de qué agrega respecto del sample anterior. Cumple §4.2.2 y el criterio anti-patrón "samples sin nivel declarado".
- "Cómo correrlo" tiene exactamente 5 pasos copiables en los 6 samples; ninguno supera el tope de 5. Cumple §4.2.4.
- "Qué esperar" trae output exacto de consola (bloques de texto literal) en los 6, con criterio de éxito señalado en la última línea. geovial declara además el rechazo por error como ruta alternativa. Cumple §4.2.6.
- Prerequisites: geovial usa tabla con columna "Versión mínima / cómo obtenerlo"; aplicada-sync usa lista con versión remitida a intake §17 P.9. Ambos satisfacen §4.2.3 (sin ambigüedad, versión mínima cuando aplica).

---

## 5. Matriz README de la sección (§4.3 y §4.4)

| Requisito | aplicada-sync README | geovial-storage README |
| --- | --- | --- |
| Propósito de la carpeta (docs vs /samples) | §1 presente | §1 presente |
| Tabla maestra con 5 columnas (Sample, Nivel, Tiempo de setup, CU ilustrados, Ubicación) | §2 completa | §2 completa |
| Convenciones de los samples | §3 presente | §3 presente |
| Cómo agregar un sample nuevo (ref §6 reglas) | §4 presente | §4 presente |
| Vínculo con 10 y 05 | §5 presente, archivos verificados | §5 presente, archivos verificados |
| Replica resumida tipo D8 → /samples (§2.3) | §6 presente | §6 presente |
| Control de cambios | §7 presente | §7 presente |

Tiempo de setup declarado por sample en ambas tablas (< 5 min / 10-15 min / 20-30 min). Cumple el criterio §6 "cada sample declara tiempo de setup en la tabla maestra".

Hallazgo P2 (aplicada-sync README §2): la celda de CU ilustrados del sample avanzado dice "CU-04 (más CU-01, CU-03 en integración)". El contenido es correcto y trazable, pero el anidamiento entre paréntesis rompe la lectura columnar limpia que el formato de tabla maestra busca (§4.4). Recomendación: listar "CU-04, CU-01, CU-03" en la columna y mover la aclaración "(reusados en integración)" a la nota de cobertura inmediatamente debajo de la tabla, como ya se hace en geovial-storage ("que reusa CU-06 para registrar y seleccionar").

---

## 6. Coherencia cross-doc y trazabilidad

### 6.1 aplicada-sync

- CU referenciados (CU-01..06) existen todos en `02/casos-de-uso/` (verificado: 6 archivos CU-01..CU-06). Cobertura entre los 3 samples: e01 → CU-01, CU-03; e02 → CU-02, CU-05, CU-06; e03 → CU-04 (+ CU-01, CU-03 en integración). Los seis CU quedan ilustrados; sin CU crítico sin cubrir.
- RN referenciados (RN-01, RN-02, RN-03) existen en `02/reglas-de-negocio/`.
- ADR referenciados (ADR-01, 02, 04, 05, 06, 07, 08) existen en `05/adrs/` (catálogo ADR-01..08). Sin referencia colgada.
- NFR referenciados por nombre ("Orden subir-antes-de-bajar", "Reanudación sin pérdida", "Continuidad ante conflicto", "Tiempo de sincronización de lote", "Capacidad de cola local") existen literalmente en la tabla §8 de `05/arquitectura-solucion_v1.0.md`. La cita "Atributo de calidad (arquitectura §8)" es exacta.
- Punto de extensión principal: `05/extensibilidad_v1.0.md` define las cuatro estrategias (almacén local, transporte, credencial, conectividad) y nombra el sample `03-avanzado-demo-maui` como ejemplo de extensión (§6, §7). El sample avanzado las implementa las cuatro y ejercita el motor reutilizable. El punto de extensión está exhibido por al menos un sample. Cumple §10.5.
- Ubicación del código coincide con el árbol del intake §16.1 (`01-basico/`, `02-intermedio/`, `03-avanzado-demo-maui/`) y con §2.3 de las reglas para `library` (las dos primeras carpetas son las canónicas; la tercera conserva el nombre de carpeta pedido por el intake, admisible porque §2.2/§2.3 permiten variantes por capacidad y el README lo documenta).
- Guías de 10 citadas en README §5 (`conceptos-fundamentales`, `guia-onboarding-developer`, `guia-integracion-aplicacion-movil`) y contratos/ADR de 05 (`extensibilidad`, `contratos-abstractions`): todos los archivos existen.

### 6.2 geovial-storage

- CU referenciados (CU-01..06) existen todos en `02/casos-de-uso/`. Cobertura: e01 → CU-01, CU-02; e02 → CU-03, CU-04, CU-05, CU-06; e03 → CU-06 (registro/selección) + punto de extensión. Los seis CU quedan ilustrados; sin CU crítico sin cubrir.
- RN (RN-01, RN-02, RN-03) existen en `02/reglas-de-negocio/`.
- ADR referenciados (ADR-01, 04, 05) existen en `05/adrs/` (catálogo ADR-01..05). Sin referencia colgada.
- NFR referenciados por número (NFR-04 integridad en e01; NFR-03 transparencia en e02 y e03; NFR-05 no filtración en e03) corresponden a la numeración canónica NFR-01..NFR-06 establecida y mapeada en `08/criterios-validacion_v1.0.md` y `08/casos-prueba-referenciales_v1.0.md`, y a las filas descriptivas de la tabla §8 de `05/arquitectura-solucion_v1.0.md` (Integridad = NFR-04; Transparencia = NFR-03; No filtración = NFR-05). La trazabilidad numérica es consistente con el resto de la cadena. (Observación menor de upstream, fuera del alcance de esta fase: la tabla §8 de 05 lista las NFR por nombre sin imprimir el identificador numérico, que sí aparece en 08; no es defecto del entregable de 11.)
- Acceptance/flujos alternativos citados (CA-01..CA-04, FA-02) existen en los CU correspondientes (verificado CU-01 con CA-0X; CU-06 con FA-02 "validación sin activación / prueba en seco").
- Punto de extensión principal: `05/extensibilidad_v1.0.md` declara un único punto de extensión (puerto de proveedor de almacenamiento, 6 operaciones) y remite a 11 como ejemplo. El sample avanzado (e03) implementa el puerto, lo registra y lo valida con la suite de conformidad contra el doble en memoria. El punto de extensión está exhibido. Cumple §10.5.
- Guías citadas en README §5 (`guia-onboarding-developer`, `guia-integracion-servicio-backend` de 10; `guia-testing-extensibilidad_v1.0.md` de 08; `extensibilidad`, `contratos-abstractions` de 05): todos los archivos existen. Nota: e03 §3 y README §5 referencian `guia-testing-extensibilidad_v1.0.md` rotulándola "(08)"; el archivo efectivamente vive en `08_calidad_y_pruebas/`. Cita correcta.

---

## 7. Cobertura de CU y del punto de extensión (síntesis §10.5)

| Proyecto | CU-01 | CU-02 | CU-03 | CU-04 | CU-05 | CU-06 | Punto de extensión |
| --- | --- | --- | --- | --- | --- | --- | --- |
| aplicada-sync | e01, e03 | e02 | e01, e03 | e03 | e02 | e02 | e03 (motor reutilizable, 4 estrategias) |
| geovial-storage | e01 | e01 | e02 | e02 | e02 | e02, e03 | e03 (puerto de proveedor, suite de conformidad) |

Ningún CU crítico queda sin ilustrar en ninguno de los dos proyectos. El punto de extensión declarado en 05 tiene, en cada proyecto, un sample (el avanzado) que lo exhibe end-to-end. Cumple los criterios de cobertura del master-prompt §10.5.

---

## 8. Hallazgos

| ID | Nivel | Proyecto | Archivo / sección | Evidencia | Recomendación |
| --- | --- | --- | --- | --- | --- |
| G-01 | P2 | aplicada-sync | `11_examples/README.md` §2 (tabla maestra), fila e03 | Columna "CU ilustrados" = "CU-04 (más CU-01, CU-03 en integración)": anidamiento entre paréntesis que rompe la lectura columnar de §4.4 | Listar "CU-04, CU-01, CU-03" en la celda y bajar la aclaración a la nota de cobertura bajo la tabla (como hace geovial-storage) |
| G-02 | P3 | ambos | Cabecera de los 8 archivos (campo Estado) | aplicada-sync declara Estado "Propuesto" en sus 4 archivos; geovial-storage declara "Vigente" en los suyos. Inconsistencia de madurez entre proyectos hermanos de la misma fase y fecha | Unificar el Estado de cabecera entre proyectos de un mismo nivel/fase, o documentar por qué difieren |
| G-03 | P3 | aplicada-sync | `11_examples/README.md` cabecera | El README incluye un campo "Audiencia:" no previsto en el bloque de cabecera de §4.3; geovial-storage no lo lleva | Inocuo; opcionalmente armonizar la cabecera de README entre proyectos (quitar el campo extra o agregarlo en ambos) |

No se detectaron hallazgos P0 ni P1.

---

## 9. Veredicto

### 9.1 Por proyecto

- **aplicada-sync — APROBADO CON OBSERVACIONES (sin P0).** Tres samples (básico/intermedio/avanzado), README con tabla maestra de cinco columnas, nueve secciones por markdown, ≤5 pasos, output exacto, trazabilidad CU/RN/ADR/NFR intacta, punto de extensión exhibido por el sample avanzado y token `maui` confinado al nombre de carpeta canónico del intake. Observaciones abiertas: G-01 (P2), G-02 (P3), G-03 (P3). Habilitado para avanzar.
- **geovial-storage — APROBADO (sin P0).** Tres samples, README completo, nueve secciones por markdown, ≤5 pasos, output exacto, trazabilidad CU/RN/ADR/NFR consistente con la numeración de 05/08, punto de extensión (puerto de proveedor) exhibido por el sample avanzado y suite de conformidad. Sin P0 ni P1; participa de G-02 (P3) por la inconsistencia de Estado entre proyectos hermanos. Habilitado para avanzar.

### 9.2 Consolidado

**APROBADO CON OBSERVACIONES (sin P0).** Ambos proyectos cumplen el contrato de la Fase G para `library`. No hay ningún hallazgo P0 ni P1 que obligue a RECHAZADO. Los hallazgos abiertos (1×P2, 2×P3) son de presentación/consistencia y no rompen trazabilidad, nomenclatura ni cobertura.

**Conteo por nivel: P0: 0 — P1: 0 — P2: 1 — P3: 2.**

---

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Auditoría inicial de la Fase G (Examples / categoría 11) para los proyectos de nivel 0 aplicada-sync y geovial-storage. Verifica D1-D8, las 9 secciones obligatorias por markdown, la tabla maestra del README, la regla crítica de nomenclatura por progresión y el confinamiento del token `maui` al nombre de carpeta canónico del intake, la coherencia cross-doc con 02/05/10 y la cobertura de CU/punto de extensión. Veredicto consolidado APROBADO CON OBSERVACIONES (sin P0). |
