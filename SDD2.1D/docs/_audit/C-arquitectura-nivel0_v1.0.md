# Auditoría independiente — Fase C (Arquitectura técnica) — Nivel 0 de GeoVial

| Campo | Valor |
| --- | --- |
| Fase | C — Arquitectura del proyecto |
| Alcance | Proyectos de nivel topológico 0 de GeoVial, ambos `library`: `aplicada-sync` (redistribuible) y `geovial-storage` (no redistribuible). Categoría `05_arquitectura_tecnica` de cada proyecto. La vista de solución de `_solucion/` queda fuera de alcance (se genera en Fase H) |
| Documento | C-arquitectura-nivel0_v1.0.md |
| Versión | 1.0 |
| Auditor | Arquitecto de Soluciones + QA Senior (independiente; no participó de la generación) |
| Fecha | 2026-06-15 |
| Insumos de reglas | `05_rules_arquitectura_tecnica.md` (v1.2; §2.2 para `library`, §3.3 ADR individuales, §4 estructura, §6 criterios de aceptación), `master-prompt.md` §10 |
| Fuentes de verdad | `SOLUTION-INTAKE-geovial_v1.0.md` (contenido v1.3, §17 P.10 de cada proyecto), `SOLUTION-MANIFEST-geovial_v1.0.md` (v1.0 Aprobado), upstream `00_contexto`, `01_necesidades_negocio` (NB-04, NB-07), `02_especificacion_funcional` de cada proyecto |

---

## 1. Resumen ejecutivo

Se auditaron los 27 entregables de la Fase C de los dos proyectos de nivel 0. Para `aplicada-sync`: 14 documentos (documento maestro, índice de ADRs, contratos-abstractions, extensibilidad, flujo-ejecucion, README y 8 ADRs individuales). Para `geovial-storage`: 13 documentos (documento maestro, índice de ADRs, contratos-abstractions, extensibilidad, flujo-ejecucion, README y 5 ADRs individuales). Ninguno de los dos proyectos produce `modelo-datos-logico_v1.0.md`, omisión correcta y declarada para `library` (regla §2.2). No existe contenido de Fase C en `_solucion/`, coherente con que la vista de solución es Fase H.

Los entregables son de alta calidad y cumplen el §6 de `05_rules` para `library` en ambos proyectos: documento maestro con las cuatro vistas mínimas (lógica, procesos, despliegue, datos) y las diez secciones del §4.2; índice de ADRs que refleja el estado real; mínimo de tres ADRs individuales superado (8 en `aplicada-sync`, 5 en `geovial-storage`), cada uno con las diez secciones del §4.3 y estado declarado `Aceptado`; `contratos-abstractions` con operaciones, esquemas, taxonomía de errores y política de versionado; `extensibilidad` (ambos con `tiene_extensibilidad=true`). El estilo está justificado contra ≥2 alternativas tanto en el documento maestro como en el ADR de estilo, cada NFR tiene objetivo numérico y mecanismo de medición, y ningún ADR aceptado está consolidado dentro de otro documento. La convención crítica del §3.3 (un archivo por ADR bajo `adrs/`) se respeta sin excepción.

Conformidad D1-D8 sin violaciones: verificación byte a byte confirma UTF-8 puro, LF (cero bytes CR) y sin BOM en los 27 archivos; filenames en kebab estricto con sufijo `_v1.0`, sin el anti-patrón `.v`; los 13 ADRs matchean `ADR-XX-<kebab>_v1.0.md`. La neutralidad de stack se sostiene: la fuga de "NuGet" reportada por los generadores quedó efectivamente corregida (cero ocurrencias en la Fase C de `aplicada-sync`, que usa "paquete distribuible / repositorio de paquetes" de forma abstracta), y no hay vocabulario del dominio fuente del bootstrap (impresoras, ESC-POS, DSL, Bluetooth). La trazabilidad upstream resuelve: `aplicada-sync` ancla en NB-04 y cubre CU-01..CU-06 y RN-01..RN-03; `geovial-storage` ancla en NB-07 (NB-03/NB-06 de soporte) y cubre CU-01..CU-06 y RN-01..RN-03. Todos los IDs referenciados existen en 01 y 02; no hay ADR huérfana ni CU sin componente. Los NFR de la tabla de quality attributes coinciden con §17 P.10 del intake en ambos proyectos.

Los hallazgos son menores. La única fuga residual de stack es el nombre de carpeta de sample `03-avanzado-demo-maui` referenciado dos veces en `extensibilidad_v1.0.md` de `aplicada-sync` (contiene "maui", término de stack prohibido), tratado como P2 por ser una fuga real en el cuerpo aunque sea un identificador de artefacto definido aguas arriba en el intake. El resto son P3 cosméticos.

Conteo de hallazgos: P0 = 0; P1 = 0; P2 = 1; P3 = 3. Veredicto consolidado: APROBADO CON OBSERVACIONES.

---

## 2. Matriz D1-D8 por documento

Convención: OK = conforme; n/a = no aplica. D1 idioma rioplatense técnico; D2 UTF-8/LF; D3 kebab/filename; D4 versión `_vX.Y` (nunca `.v`); D5 estado y control de cambios; D6 trazabilidad; D7 sin stack/vocabulario fuente; D8 conjunto cerrado D8.

### 2.1 Proyecto `aplicada-sync`

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| arquitectura-solucion_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| decisiones-arquitectura_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| contratos-abstractions_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| extensibilidad_v1.0.md | OK | OK | OK | OK | OK | OK | Observación (ver H-01) | OK |
| flujo-ejecucion_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| README.md | OK | OK | OK | n/a (sin versión, correcto) | OK | OK | OK | OK |
| ADR-01 estilo-clean-architecture-abstractions | OK | OK | OK | OK | OK | OK | OK | OK |
| ADR-02 inversion-dependencias-adaptadores-host | OK | OK | OK | OK | OK | OK | OK | OK |
| ADR-03 versionado-superficie-publica | OK | OK | OK | OK | OK | OK | OK | OK |
| ADR-04 cola-local-persistente-ordenada | OK | OK | OK | OK | OK | OK | OK | OK |
| ADR-05 orden-subir-antes-de-bajar | OK | OK | OK | OK | OK | OK | OK | OK |
| ADR-06 reanudacion-por-marca-de-progreso | OK | OK | OK | OK | OK | OK | OK | OK |
| ADR-07 idempotencia-por-identificador-estable | OK | OK | OK | OK | OK | OK | OK | OK |
| ADR-08 convivencia-con-conflictos | OK | OK | OK | OK | OK | OK | OK | OK |

### 2.2 Proyecto `geovial-storage`

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| arquitectura-solucion_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| decisiones-arquitectura_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| contratos-abstractions_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| extensibilidad_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| flujo-ejecucion_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| README.md | OK | OK | OK | n/a (sin versión, correcto) | OK (ver H-04 cabecera) | OK | OK | OK |
| ADR-01 abstraccion-proveedores-intercambiables | OK | OK | OK | OK | OK | OK | OK | OK |
| ADR-02 superficie-publica-estable | OK | OK | OK | OK | OK | OK | OK | OK |
| ADR-03 estrategia-versionado-contrato | OK | OK | OK | OK | OK (ver H-03 categoría) | OK | OK | OK |
| ADR-04 transparencia-limites-proveedor | OK | OK | OK | OK | OK | OK | OK | OK |
| ADR-05 manejo-seguro-credenciales | OK | OK | OK | OK | OK | OK | OK | OK |

### 2.3 Verificaciones de respaldo (ambos proyectos)

- D2 encoding: verificación byte a byte (conteo de bytes 0x0d) sobre los 27 archivos: cero bytes CR en todos; sin BOM (los tres primeros bytes no son `EF BB BF`). LF puro. Conforme. (Una verificación preliminar con un patrón de grep arrojó falsos positivos de CRLF; la verificación autoritativa por bytes los descartó.)
- D3/D4 filenames: los 13 ADRs matchean `^ADR-[0-9]{2}-[a-z0-9-]+_v[0-9]+\.[0-9]+\.md$`; los documentos versionados usan `_v1.0.md`; los README no llevan versión (correcto). Scan negativo del anti-patrón `.v<X.Y>.md` con punto en ambas carpetas. Subcarpeta `adrs/` presente en los dos proyectos. Estructura bajo `proyectos/<kebab>/05_arquitectura_tecnica/` correcta.
- D7 scan léxico con límites de palabra sobre toda la Fase C de ambos proyectos: cero coincidencias de `.net`, `dotnet`, `c#`, `maui` (salvo el nombre de carpeta de sample, ver H-01), `sqlite`, `sql server`, `s3`, `amazon`, `nuget`, `blazor`, `mudblazor`, `leaflet`, `openstreetmap`, `signalr`, `docker`, `jwt`, `ropc`, `bearer`, `http(s)`, `json`, `grpc`, `android`, `ios`, `github`, `zip`, `exif`, `gps`, `esc-pos`, `bluetooth`, `impresora`, `térmica`, `dsl`. La fuga de "NuGet" reportada por los generadores quedó corregida: cero ocurrencias en `aplicada-sync` (usa "paquete distribuible", "repositorio de paquetes", "paquete redistribuible"). Las menciones de "SemVer 2.0.0" y "Conventional Commits" en `geovial-storage` (ADR-03 y contratos §6) se evalúan como convención de versionado/metodología abstracta (no producto/stack), válida y trazada al intake §17 P.7; no se cuenta como fuga (ver §4).
- D7 dominio: las menciones de "GeoVial" en `aplicada-sync` (ADR-01, ADR-02, extensibilidad, README) se usan para negar el acoplamiento ("agnóstico del dominio de GeoVial", "reutilizable fuera de GeoVila", "ajeno a la solución GeoVial"), uso legítimo coherente con su carácter redistribuible. No es vocabulario del dominio fuente del bootstrap.
- Sin emojis ni negrita decorativa: las negritas se limitan al bloque de cabecera del §4.1.

---

## 3. Matriz de estructura obligatoria por documento

### 3.1 Documento maestro `arquitectura-solucion_v1.0.md` (§4.2: 10 secciones + 4 vistas mínimas)

| Sección §4.2 | aplicada-sync | geovial-storage |
| --- | --- | --- |
| Cabecera §4.1 (sin Categoría) | OK | OK |
| §1 Objetivo | OK | OK |
| §2 Estilo + justificación vs ≥2 alternativas | OK (3 alternativas descartadas) | OK (3 alternativas descartadas + tabla criterios) |
| §3 Vista lógica (componentes con CU cubiertos) | OK (6 componentes, CU-01..06) | OK (7 componentes, CU-01..06) |
| §4 Vista de procesos | OK | OK |
| §5 Vista de despliegue | OK | OK |
| §6 Vista de datos (referencia a modelo lógico = no aplica) | OK | OK |
| §7 Cross-cutting concerns (centralizado) | OK | OK |
| §8 Quality attributes (NFR num. + medición + ADR) | OK (6 NFR) | OK (6 NFR) |
| §9 Riesgos (impacto/probabilidad/mitigación) | OK (6 riesgos) | OK (6 riesgos) |
| §10 Trazabilidad (CU/RN/ADR/tests) | OK | OK |

Cada componente de la vista lógica lista los CU cubiertos y la unión cubre CU-01..CU-06 en ambos proyectos; no hay CU huérfano.

### 3.2 Índice de ADRs `decisiones-arquitectura_v1.0.md`

| Ítem | aplicada-sync | geovial-storage |
| --- | --- | --- |
| Cabecera §4.1 | OK | OK |
| Tabla índice (ID, título, categoría, estado, fecha) | OK (8 ADRs) | OK (5 ADRs) |
| Estado reflejado coincide con el cuerpo de cada ADR | OK (8/8 Aceptado) | OK (5/5 Aceptado) |
| Declara cobertura del mínimo `library` (estilo, superficie, versionado) | OK | OK |
| Trazabilidad de motivación por ADR (NB/CU/RN/NFR) | OK | OK |
| Es índice (no consolida cuerpos de ADR) | OK | OK |

### 3.3 ADRs individuales (§4.3: 10 secciones + estado declarado)

Las diez secciones requeridas: §1 Contexto, §2 Decisión, §3 Estado, §4 Alternativas (tabla con pros/contras, ≥2), §5 Consecuencias positivas, §6 Consecuencias negativas/trade-offs, §7 Implementación, §8 Métricas de validación, §9 Referencias, §10 Control de cambios.

| ADR | 10 secciones | Estado | Alternativas (≥2) | Motivación (NB/CU/RN/NFR) |
| --- | --- | --- | --- | --- |
| aplicada-sync ADR-01 (Estilo) | OK | Aceptado | 3 | NB-04, CU-01..06, intake §17 P.2/P.11 |
| aplicada-sync ADR-02 (Extensibilidad) | OK | Aceptado | 3 | CU-01, CU-04, intake §17 P.2 |
| aplicada-sync ADR-03 (Despliegue/versionado) | OK | Aceptado | 3 | Espec. §8, §17 de CU, intake §17 P.7/P.8 |
| aplicada-sync ADR-04 (Persistencia) | OK | Aceptado | 3 | CU-02/05/06, RN-02, NFR cola ≥1000 |
| aplicada-sync ADR-05 (Estilo) | OK | Aceptado | 3 | RN-01, CU-03/04/06, NFR lote 100 |
| aplicada-sync ADR-06 (Persistencia) | OK | Aceptado | 3 | CU-06, RN-01/02, NFR reanudación, intake §7 |
| aplicada-sync ADR-07 (Persistencia) | OK | Aceptado | 3 | RN-02, CU-02/03/06, NFR idempotencia |
| aplicada-sync ADR-08 (Estilo) | OK | Aceptado | 3 | RN-03, CU-03/05, NFR continuidad |
| geovial-storage ADR-01 (Estilo) | OK | Aceptado | 4 | NB-07, CU-01..06, RN-01, §17 P.2/P.11/P.12 |
| geovial-storage ADR-02 (Estilo) | OK | Aceptado | 4 | NB-07, CU-01..06, RN-01, §17 P.11; 02 §6 |
| geovial-storage ADR-03 (Estilo; ver H-03) | OK | Aceptado | 4 | NB-07, CU-01, RN-01, §17 P.7; 02 §6 |
| geovial-storage ADR-04 (Estilo) | OK | Aceptado | 4 | NB-07, CU-01/02/05, RN-01/02, §17 P.10 |
| geovial-storage ADR-05 (Seguridad) | OK | Aceptado | 4 | NB-07, CU-02/05/06, RN-03, §17 P.5 |

Mínimo de tres ADRs para `library`: superado en ambos (8 y 5). Las tres categorías obligatorias del §2.2 (estilo, superficie pública, estrategia de versionado) están cubiertas: en `aplicada-sync` por ADR-01/ADR-05 (estilo), ADR-02 (superficie/extensión), ADR-03 (versionado); en `geovial-storage` por ADR-01 (estilo), ADR-02 (superficie pública), ADR-03 (versionado). Ningún ADR está consolidado dentro de otro documento: cada decisión vive en su archivo individual bajo `adrs/` (regla §3.3 conforme).

### 3.4 `contratos-abstractions_v1.0.md` (§4.5: alcance, formato, operaciones, esquemas, errores, versionado, trazabilidad)

| Sección §4.5 | aplicada-sync | geovial-storage |
| --- | --- | --- |
| §1 Alcance (CU que materializa) | OK (CU-01..06) | OK (CU-01..06) |
| §2 Formato (abstracto, sin esquema de red) | OK | OK |
| §3 Operaciones | OK (6 operaciones) | OK (6 operaciones) |
| §4 Esquemas de datos | OK | OK |
| §5 Manejo de errores (taxonomía, códigos estables) | OK | OK |
| §6 Versionado del contrato (compat. hacia atrás/deprecación) | OK | OK |
| §7 Trazabilidad (CU/RN/ADR; CU que consume) | OK | OK |

Ambos contratos referencian los CU que consumen y el ADR de versionado que los gobierna. `geovial-storage` además declara el contrato inter-proyecto `geovial-storage → geovial-api` y lo remite a la vista de solución (Fase H), correcto.

### 3.5 `extensibilidad_v1.0.md` (ambos con `tiene_extensibilidad=true`)

| Ítem | aplicada-sync | geovial-storage |
| --- | --- | --- |
| Cabecera §4.1 | OK | OK |
| Puntos de extensión con contrato | OK (4 estrategias) | OK (1 puerto de proveedor) |
| ADR que justifica su existencia | OK (ADR-01/02/03/07/08) | OK (ADR-01/04/05) |
| Ejemplo de extensión referenciado a 11 | OK (sample de intake §18/§16.1) | OK (samples de intake §16.1) |
| Trazabilidad (CU/RN/ADR/tests) | OK | OK |

### 3.6 `flujo-ejecucion_v1.0.md` (recomendado para `library` con motor de procesamiento)

Presente en ambos y declarado. `aplicada-sync`: justificado por el motor de sincronización (pipeline subir-luego-bajar). `geovial-storage`: declarado como opcional incluido en el README, justificado por el pipeline de enrutado validar → resolver → delegar → normalizar. Ambos con estados, pasos, transformaciones y trazabilidad. Conforme (recomendado, no obligatorio).

### 3.7 `modelo-datos-logico_v1.0.md`

Omitido en ambos proyectos. Correcto para `library` (regla §2.2: "library puro sin estado" no lo exige). La omisión está declarada explícitamente en el README y en la vista de datos (§6) de cada documento maestro. Conforme.

---

## 4. Chequeos específicos solicitados

| Chequeo | Resultado | Evidencia |
| --- | --- | --- |
| Estilo justificado contra ≥2 alternativas en el documento maestro y en el ADR de estilo | Cumple | aplicada-sync: maestro §2 descarta 2 alternativas + tabla de criterios, ADR-01 tabla con 3 alternativas. geovial-storage: maestro §2 descarta 3 alternativas + tabla de criterios, ADR-01 tabla con 4 alternativas |
| Cada NFR con objetivo numérico Y mecanismo de medición | Cumple | aplicada-sync §8: 6 NFR, cada uno con valor numérico (lote 100 ≤30 s, cola ≥1000, 0 perdidos/duplicados, 100 % idempotencia, 0 bajadas prematuras, 0 ciclos abortados) y prueba descripta. geovial-storage §8: 6 NFR (p95 ≤1 s/5 MB, máx 25 MB, 0 diferencias, 100 % igualdad binaria, 0 filtraciones, cobertura ≥80/70 %) con mecanismo |
| NFR coinciden con §17 P.10 del intake v1.3 | Cumple | aplicada-sync §8 reproduce literalmente §17 P.10 de aplicada-sync (lote 100 ≤30 s, cola ≥1000, reanudación sin pérdida, idempotencia). geovial-storage §8 reproduce §17 P.10 de geovial-storage (p95 ≤1 s para ≤5 MB local, máx configurable por defecto 25 MB, transparencia sin degradación) |
| Ningún ADR aceptado consolidado dentro de otro documento | Cumple | 13 ADRs en archivos individuales bajo `adrs/`; `decisiones-arquitectura` es índice puro sin cuerpos; regla §3.3 conforme |
| Fuga de "NuGet" corregida | Cumple | Scan negativo de "nuget" en toda la Fase C de aplicada-sync; usa "paquete distribuible / repositorio de paquetes / paquete redistribuible" de forma abstracta |
| Cuerpo arquitectónico sin productos/stacks concretos | Cumple con una excepción | Scan negativo de .NET/MAUI/SQLite/S3/HTTP/JSON literal etc. Única fuga residual: nombre de carpeta de sample `03-avanzado-demo-maui` en aplicada-sync extensibilidad (ver H-01) |
| Sin vocabulario del dominio fuente del bootstrap (impresoras, ESC-POS, DSL, Bluetooth) | Cumple | Scan negativo en ambos proyectos |
| Índice de ADRs refleja los ADR reales con su estado | Cumple | 8/8 y 5/5 ADRs indexados, todos `Aceptado`, coincidente con el cuerpo |
| Cada componente de la vista lógica lista los CU cubiertos (CU-01..CU-06) | Cumple | Tablas de §3 en ambos maestros; unión = CU-01..06; sin CU huérfano |
| Cada ADR referencia NB/CU/RN/NFR que la motivan (sin ADR huérfana) | Cumple | §1 y §9 de cada ADR + tabla de motivación del índice; 13/13 con motivación |
| contratos-abstractions referencia los CU que consume | Cumple | §1 de cada contrato lista CU-01..06; §7 traza ADR |
| modelo-datos-logico ausente (library) | Cumple | Omitido y declarado en ambos |
| Trazabilidad upstream a 00/01/02 | Cumple | aplicada-sync → NB-04 (existe en 01) + CU-01..06/RN-01..03 (existen en 02). geovial-storage → NB-07 (+NB-03/NB-06) + CU-01..06/RN-01..03 (existen en 02) |
| Trazabilidad downstream a 06/08 | Cumple | Cada maestro/ADR enumera US (06) y tests previstos (08) de forma tentativa; conforme §3.4 |
| Filenames ADR matchean `ADR-XX-<kebab>_v1.0.md` | Cumple | 13/13 matchean; subcarpeta `adrs/` correcta |

Nota sobre "SemVer 2.0.0" y "Conventional Commits" (geovial-storage ADR-03, contratos §6; aplicada-sync ADR-03 los referencia vía intake): son convenciones de versionado/etiquetado, no productos ni stacks de implementación, y están fijadas en el intake §17 P.7 para todos los proyectos. Se describen como mecanismo abstracto (clasificación mayor/menor, deprecación) sin nombrar herramienta concreta (la mención de la herramienta de cálculo de versión queda diferida al intake/09). No se cuenta como fuga de stack. Observación de criterio, sin hallazgo.

---

## 5. Coherencia cross-doc

### 5.1 `aplicada-sync`

- Índice de ADRs ↔ README ↔ cuerpos: las ocho filas del índice (`decisiones-arquitectura` §2) coinciden con la tabla del README §"Decisiones de arquitectura" y con la cabecera de cada ADR (título, categoría, estado `Aceptado`). Sin divergencias.
- Estilo: el documento maestro §2 y ADR-01 declaran el mismo estilo (Clean Architecture con capa Abstractions + pipeline subir-luego-bajar) con las mismas alternativas descartadas; ADR-05 formaliza el orden del pipeline. Coherente.
- ADR ↔ NFR ↔ arquitectura §8: la columna "ADR relacionada" de la tabla de NFR (§8) referencia ADRs existentes (ADR-04/05/06/07/08) y cada uno reaparece en su archivo con la métrica numérica. La trazabilidad NFR↔arquitectura↔ADR está explícita en §8 del maestro y en §8 de cada ADR.
- ADR ↔ CU/RN: cada ADR referencia los CU/RN que la motivan; los IDs CU-01..06 y RN-01..03 existen en 02. No hay ADR huérfana. La matriz de motivación del índice (§4) coincide con las referencias §9 de cada ADR.
- contratos-abstractions ↔ extensibilidad ↔ flujo-ejecucion: el catálogo de errores y las operaciones del contrato son coherentes con las estrategias de extensión (almacén local, transporte, credencial, conectividad) y con los pasos del pipeline; los códigos (CONFIGURACION_INCOMPLETA, BACKEND_INALCANZABLE, SUBIDA_INCOMPLETA, etc.) son consistentes entre los tres documentos y con el catálogo de 03.
- Enlaces relativos del índice y del README resuelven a archivos existentes; IDs de ADR no duplicados.

### 5.2 `geovial-storage`

- Índice de ADRs ↔ README ↔ cuerpos: las cinco filas del índice coinciden con la tabla del README y con la cabecera de cada ADR. Estado `Aceptado` en los cinco. Sin divergencias salvo la categoría de ADR-03 (ver H-03).
- Estilo: el maestro §2 y ADR-01 declaran el mismo estilo (hexagonal/puertos y adaptadores con proveedores por estrategia) con las mismas alternativas descartadas (capas clásicas, acceso acoplado, proveedor único fijo). Coherente.
- ADR ↔ NFR ↔ arquitectura §8: la tabla de NFR (§8) referencia ADR-01/03/04/05 y cada métrica reaparece en el ADR correspondiente (p. ej. p95 ≤1 s y máx 25 MB en ADR-04). Trazabilidad NFR↔arquitectura↔ADR explícita.
- ADR ↔ CU/RN/NB: cada ADR referencia NB-07, los CU y RN que la motivan; los IDs existen en 01 y 02. No hay ADR huérfana. La tabla de trazabilidad upstream del índice (§4) coincide con las referencias §9 de cada ADR.
- contratos-abstractions ↔ extensibilidad ↔ flujo-ejecucion: las seis operaciones, la taxonomía de errores uniforme y el puerto de proveedor son coherentes entre los tres documentos; el catálogo de códigos (CONTENIDO_VACIO, IDENTIFICADOR_INEXISTENTE, TAMANIO_EXCEDIDO, PROVEEDOR_NO_CONFIGURADO, etc.) coincide con el de 03. Los dos códigos antes señalados por la auditoría de Fase B (PROVEEDOR_NO_CONFIGURADO y TAMANIO_EXCEDIDO) quedan ahora consolidados en el contrato de 05 §3/§5 y atados a ADR-04, cerrando el bucle previsto en H-04 de la Fase B.
- Enlaces relativos del índice y del README resuelven; IDs de ADR no duplicados.

### 5.3 Coherencia con upstream de solución (00/01/02) y downstream (06/08) y nivel solución

- Upstream: ambos maestros y ADRs citan SOLUTION-INTAKE §17 del bloque del proyecto, la NB de origen (NB-04 / NB-07) y los CU/RN de 02. Todas las NB, CU y RN referenciadas existen en los entregables de 01 y 02.
- Downstream: cada documento enumera US a generar en 06 (US-01..US-13 en aplicada-sync; US-01..US-09 en geovial-storage) y tests previstos en 08 de forma tentativa y no vinculante (conforme §3.4). El stack concreto se difiere correctamente al intake §17 y a 09; el código ejecutable a 11.
- Nivel solución: no hay contenido de Fase C en `_solucion/`. Correcto: la vista de solución (`vista-solucion_v1.0.md`) es Fase H y queda fuera de este audit. `geovial-storage` declara su contrato inter-proyecto hacia `geovial-api` y lo remite a la vista de solución, sin adelantarla.

---

## 6. Hallazgos enumerados

### H-01 — P2 — Fuga residual del término de stack "maui" vía el nombre de carpeta de sample en aplicada-sync
- Archivo: `proyectos/aplicada-sync/05_arquitectura_tecnica/extensibilidad_v1.0.md` §6 (Ejemplo de extensión) y §7 (tabla de trazabilidad).
- Evidencia: el documento referencia dos veces la carpeta de sample `03-avanzado-demo-maui` (`/samples/aplicada-sync/03-avanzado-demo-maui/` en §6 y `Sample 03-avanzado-demo-maui` en §7). El segmento "maui" es un término de stack explícitamente prohibido por la matriz de neutralidad (D7) del alcance del audit.
- Análisis: es la única fuga de stack residual de toda la Fase C de ambos proyectos. El término aparece únicamente como identificador literal de un artefacto (carpeta de sample) definido aguas arriba en el intake §16.1 y §18; el texto que lo rodea describe el punto de extensión de forma abstracta ("motor de sincronización reutilizable", "adaptadores propios del integrador") y no toma ninguna decisión arquitectónica sobre el stack. No rompe trazabilidad ni la neutralidad del cuerpo decisional. Se clasifica P2 (no P0) por ser una fuga real de un término prohibido en el cuerpo del documento, atenuada por tratarse de un nombre de artefacto heredado del intake y no de una decisión de stack; los demás artefactos de la solución (incluida la auditoría de Fase B) ya tratan este sample con el mismo nombre.
- Recomendación: renombrar la referencia a un identificador neutral (por ejemplo `03-avanzado-demo-integracion` o "sample avanzado de integración ajeno a la solución") y, si el nombre de carpeta debe conservarse por estar fijado en el intake, anotar explícitamente que el sufijo es un identificador de artefacto del intake §16.1 y no una decisión de stack de esta categoría. Reconciliar el nombre del sample con el intake en la Fase H/G. No bloquea la promoción.

### H-02 — P3 — "SemVer 2.0.0" y "Conventional Commits" nombrados en el cuerpo arquitectónico
- Archivos: `geovial-storage/05_.../contratos-abstractions_v1.0.md` §6 y `adrs/ADR-03-estrategia-versionado-contrato_v1.0.md` (varias líneas); referencia análoga en `aplicada-sync/.../ADR-03` y `contratos-abstractions` a versionado semántico.
- Evidencia: el cuerpo nombra el estándar "SemVer 2.0.0" y la convención "Conventional Commits" como mecanismo de versionado del contrato.
- Análisis: son convenciones de versionado y de etiquetado de commits, no productos ni stacks de implementación; están fijadas en el intake §17 P.7 para toda la solución y se describen junto con su mecanismo abstracto (clasificación mayor/menor, deprecación). La regla §4.2/§4.5 admite expresar la política de versionado y los NFR numéricos. No es una fuga de stack en sentido estricto y no rompe ninguna invariante. Se deja como P3 de criterio: para máxima pureza de neutralidad, la decisión podría expresarse como "versionado semántico con compatibilidad hacia atrás" sin citar la marca de la especificación ni la convención de commits, dejando ambas como referencia del intake §17 P.7.
- Recomendación: opcionalmente, sustituir "SemVer 2.0.0" por "versionado semántico" y remitir "Conventional Commits" al intake §17 P.7, conservando la semántica. Mejora de pureza, no bloqueante.

### H-03 — P3 — Categoría de ADR-03 de geovial-storage rotulada "Estilo" en lugar de la categoría de versionado/despliegue
- Archivos: `geovial-storage/05_.../decisiones-arquitectura_v1.0.md` §2, `README.md` y la cabecera de `adrs/ADR-03-estrategia-versionado-contrato_v1.0.md` (campo `Categoría: Estilo`).
- Evidencia: ADR-03 documenta la estrategia de versionado del contrato pero su `Categoría` figura como `Estilo`. El conjunto enumerado de categorías del §4.1 es `Estilo | Persistencia | Comunicación | Seguridad | Observabilidad | Despliegue | Extensibilidad`; "Versionado" no está en el enum, y el ADR-03 equivalente de `aplicada-sync` clasifica el versionado como `Despliegue`.
- Análisis: la categoría es un valor del conjunto enumerado del §4.1 y "Estilo" pertenece a ese conjunto, por lo que no hay violación dura de D-cabecera. El problema es de coherencia semántica y de consistencia entre proyectos: tres de los cinco ADRs de geovial-storage quedan rotulados "Estilo" (ADR-01, ADR-02, ADR-03), aunque ADR-03 trata versionado. `aplicada-sync` rotula su decisión análoga como `Despliegue`. No afecta trazabilidad ni completitud. P3 de claridad/consistencia.
- Recomendación: alinear la categoría de ADR-03 de geovial-storage con la del ADR-03 de aplicada-sync (`Despliegue`) o adoptar una categoría más precisa dentro del enum; criterio a unificar en la Fase H.

### H-04 — P3 — Cabecera del README de geovial-storage con campos no estándar respecto del bloque §4.1
- Archivo: `geovial-storage/05_.../README.md`.
- Evidencia: la cabecera abre con `Tipo (D8): library` y `Variante: ...` en lugar del par `Documento:` / `Versión:` que usan el resto de los artefactos y el README de aplicada-sync. Mantiene Proyecto, Estado, Fecha y Autor.
- Análisis: el README de sección es recomendado, no obligatorio (regla §3.5), y el §4.1 prescribe la cabecera para los artefactos versionados, no necesariamente para el README navegable. La cabecera transmite la misma información y agrega Tipo/Variante; no afecta D1-D8 ni trazabilidad. Es una inconsistencia cosmética de estilo entre los dos README de la solución. P3 de consistencia.
- Recomendación: unificar el estilo de cabecera de los README de sección entre proyectos en la Fase H (decisión de estilo). No bloqueante.

---

## 7. Veredicto final

### Por proyecto

- `aplicada-sync` (`library`, redistribuible): VEREDICTO APROBADO CON OBSERVACIONES. Cumple §6 de `05_rules` para `library`: documento maestro con 4 vistas mínimas y §1-§10; índice de ADRs que refleja el estado real; 8 ADRs individuales (mínimo 3 superado), cada uno con las 10 secciones del §4.3, estado `Aceptado` y ≥3 alternativas; contratos-abstractions con esquema/errores/versionado; extensibilidad; flujo-ejecucion (recomendado por el motor de procesamiento); sin modelo-datos-logico (correcto). Estilo justificado contra ≥2 alternativas en maestro y ADR-01. Cada NFR con objetivo numérico y mecanismo, coincidentes con §17 P.10. Ningún ADR consolidado. Fuga de "NuGet" corregida. Trazabilidad NB-04 / CU-01..06 / RN-01..03 íntegra. Hallazgos: H-01 (P2), H-02 (P3). Sin P0 ni P1.

- `geovial-storage` (`library`, no redistribuible): VEREDICTO APROBADO CON OBSERVACIONES. Cumple §6 de `05_rules` para `library`: documento maestro con 4 vistas mínimas y §1-§10; índice de ADRs; 5 ADRs individuales (mínimo 3 superado), cada uno con las 10 secciones del §4.3, estado `Aceptado` y ≥4 alternativas; contratos-abstractions con esquema/errores/versionado y contrato inter-proyecto declarado; extensibilidad; flujo-ejecucion (opcional declarado); sin modelo-datos-logico (correcto). Estilo hexagonal justificado contra ≥3 alternativas en maestro y ADR-01. Cada NFR con objetivo numérico y mecanismo, coincidentes con §17 P.10. Ningún ADR consolidado. Cuerpo sin productos/stacks concretos ni vocabulario del dominio fuente. Trazabilidad NB-07 (+NB-03/NB-06) / CU-01..06 / RN-01..03 íntegra. Hallazgos: H-02 (P3, compartido), H-03 (P3), H-04 (P3). Sin P0 ni P1.

### Consolidado

VEREDICTO: APROBADO CON OBSERVACIONES.

Fundamento: no se detectó ningún hallazgo P0 ni P1 en ninguno de los dos proyectos de nivel 0. Ambos cumplen los criterios de aceptación del §6 de `05_rules_arquitectura_tecnica.md` para el tipo `library`, respetan D1-D8 (encoding UTF-8/LF verificado byte a byte, filenames kebab con `_v1.0` y sin el anti-patrón `.v`, ADR individuales bajo `adrs/` conforme §3.3, sin ADR consolidado), mantienen la trazabilidad upstream a 00/01/02 y downstream a 06/08, declaran NFR numéricos con mecanismo de medición que coinciden con §17 P.10 del intake v1.3, y justifican el estilo contra ≥2 alternativas en el documento maestro y en el ADR de estilo. La fuga de "NuGet" reportada por los generadores quedó efectivamente corregida. La única fuga residual de stack (el nombre de carpeta de sample `03-avanzado-demo-maui`) es un identificador de artefacto heredado del intake, no una decisión de stack del cuerpo decisional, y se clasifica P2. Conforme a la regla del veredicto (master-prompt §10), la ausencia de P0 habilita avanzar a la Fase D de estos proyectos.

Condiciones recomendadas (no bloquean la promoción a Fase D):
1. Neutralizar la referencia al nombre de carpeta de sample con "maui" en `aplicada-sync/extensibilidad_v1.0.md`, o anotar que es un identificador de artefacto del intake §16.1 (H-01).
2. Opcionalmente expresar el versionado como "versionado semántico" remitiendo SemVer/Conventional Commits al intake §17 P.7 (H-02).
3. Alinear la categoría de ADR-03 de geovial-storage (versionado) con la de aplicada-sync (`Despliegue`) (H-03).
4. Unificar el estilo de cabecera de los README de sección entre proyectos (H-04).

Hallazgos por nivel: P0 = 0 · P1 = 0 · P2 = 1 · P3 = 3.

---

## Control de cambios

| Versión | Fecha | Cambios | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-06-15 | Auditoría independiente inicial de la Fase C (05 arquitectura técnica) de los proyectos de nivel 0 de GeoVial (`aplicada-sync` y `geovial-storage`, ambos `library`). Veredicto consolidado APROBADO CON OBSERVACIONES (0 P0, 0 P1, 1 P2, 3 P3). Verificadas: 4 vistas mínimas, ≥3 ADR individuales con 10 secciones y estado, contratos-abstractions, extensibilidad, omisión correcta de modelo-datos-logico, neutralidad de stack (fuga de NuGet corregida; única fuga residual "maui" en nombre de sample), NFR coincidentes con intake §17 P.10 y trazabilidad NB/CU/RN íntegra. | Auditor independiente (Arquitecto de Soluciones + QA Senior) |
