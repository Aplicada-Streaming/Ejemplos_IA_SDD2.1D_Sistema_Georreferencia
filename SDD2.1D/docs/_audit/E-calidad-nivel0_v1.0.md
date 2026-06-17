# Auditoría Fase E — Calidad y pruebas (08) — Nivel 0

**Fase:** E (Calidad y pruebas)
**Proyectos auditados:** aplicada-sync, geovial-storage (ambos `library`, equipo_n=1)
**Categoría:** 08_calidad_y_pruebas
**Auditor:** Arquitecto de Soluciones + QA Senior (independiente, sin participación en la generación)
**Fecha:** 2026-06-15
**Reglas aplicadas:** `08_rules_calidad_y_pruebas.md` (§6, variante library), `SOLUTION-INTAKE-geovial_v1.0.md` v1.3 (§17 P.6 cobertura, P.10 NFR de cada proyecto)
**Insumos upstream consultados:** 02 (CU-01..06, RN-01..03), 05 (arquitectura §8 NFR, ADR, contratos, extensibilidad), 06 (US/BT, DoR) y 07 (mini-plan) de cada proyecto.

---

## 1. Resumen ejecutivo

Ambos proyectos entregan el conjunto documental obligatorio de la Fase E completo: los siete artefactos obligatorios de la regla 08 §6 (estrategia-calidad, estrategia-testing, plan-pruebas, matriz-cobertura-pruebas, casos-prueba-referenciales, criterios-validacion, definition-of-done) más `guia-testing-extensibilidad_v1.0.md` (ambos proyectos declaran `tiene_extensibilidad=true`, por lo que la guía es obligatoria) y `README.md`. Nueve archivos por proyecto, dieciocho en total. Nomenclatura uniforme `_v1.0.md`, kebab-case, sin patrón heredado `.v`, sin sufijo de dominio (`-sync`, `-storage`, `-motor`) en ningún nombre de archivo.

La trazabilidad es sólida en ambos: los CU-01..06 y RN-01..03 referenciados existen como archivos en 02; los NFR referenciados existen en 05 §8 y coinciden con el intake §17 P.10 (valores numéricos idénticos); la reconciliación del gate global del intake §17 P.6 (≥ 80 % líneas / ≥ 70 % branches) con las coberturas por capa (dominio 85/80, infraestructura 70/60) está declarada y es coherente en estrategia-calidad, estrategia-testing y matriz §5 de cada proyecto. La matriz de cada proyecto tiene las tres tablas obligatorias (CU↔TC, NFR↔TC, RN↔TC) más cobertura por capa; cada TC referencia al menos un CU/RN/NFR; cada NFR numérico tiene TC; no hay TC huérfanos ni requisitos huérfanos. La DoD es canónica por capa (US, BT, tramo/sprint, release) con validación mecánica por criterio, y NO se redefine en 07: el mini-plan de cada proyecto la referencia como pendiente de 08 (07 §5) y la nota de pendencia queda satisfecha por el artefacto de Fase E.

El único defecto material es interno a geovial-storage: el `plan-pruebas` §5 (plan por tramo) y una frase de `casos-prueba-referenciales` §1 asignan/etiquetan TC de forma cruzada respecto del propio catálogo y de la matriz (TC de listado CU-05 colocados en el tramo de configuración CU-06; TC-18/TC-19 presentados como la batería de transparencia cuando la batería real es TC-21/TC-22/TC-27; TC-23 a TC-28 sin tramo asignado). No rompe la trazabilidad upstream/downstream (en la matriz cada CU/RN/NFR conserva su TC y cada TC su requisito), por lo que es P1 de consistencia interna, no P0. aplicada-sync no presenta defectos internos análogos.

Conteo de hallazgos: **P0 = 0** | **P1 = 1** | **P2 = 2** | **P3 = 3**.

| Proyecto | P0 | P1 | P2 | P3 | Veredicto |
| --- | --- | --- | --- | --- | --- |
| aplicada-sync | 0 | 0 | 0 | 1 | APROBADO |
| geovial-storage | 0 | 1 | 1 | 2 | APROBADO CON OBSERVACIONES |
| Común a ambos | 0 | 0 | 1 | 0 | — |
| **Consolidado** | **0** | **1** | **2** | **3** | **APROBADO CON OBSERVACIONES** |

Sin P0: ambos proyectos pueden avanzar a la fase siguiente.

---

## 2. Matriz D1-D8 por documento

Leyenda: OK = conforme; Obs = observación menor (ver hallazgos). D1 idioma rioplatense; D2 encoding UTF-8/LF; D3 kebab-case filename; D4 versionado `_vX.Y` (no `.v`); D5 sin stacks/productos comerciales (tooling por rol abstracto); D6 sin vocabulario del dominio fuente del bootstrap; D7 sin sufijo de dominio en filename + IDs internos consistentes; D8 conjunto cerrado de documentos.

### aplicada-sync

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 08/estrategia-calidad_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/estrategia-testing_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/plan-pruebas_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/matriz-cobertura-pruebas_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/casos-prueba-referenciales_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/criterios-validacion_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/definition-of-done_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/guia-testing-extensibilidad_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/README.md | OK | OK | OK | OK | OK | OK | OK | OK |

### geovial-storage

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 08/estrategia-calidad_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/estrategia-testing_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/plan-pruebas_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/matriz-cobertura-pruebas_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/casos-prueba-referenciales_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/criterios-validacion_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/definition-of-done_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/guia-testing-extensibilidad_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/README.md | OK | OK | OK | OK | OK | OK | OK | OK |

Notas de la matriz:

- **D2 (encoding):** los dieciocho archivos están en UTF-8 sin BOM y, verificado contra el índice de git (`git show :<path>`), los blobs commiteados están en LF (cero bytes CR). El working tree en este checkout Windows muestra CRLF por `core.autocrlf=true`, pero el `.gitattributes` del repositorio fija `*.md text eol=lf`, lo que garantiza LF en el repositorio (invariante D2). A diferencia de la Fase D (donde se anotó la CRLF como reconciliación P2 pendiente), aquí el `.gitattributes` ya está presente y los blobs son LF: **D2 conforme**, sin hallazgo.
- **D5 (tooling):** todo el tooling se nombra por rol abstracto ("framework de tests unitarios", "framework de property-based testing", "framework de mutation testing", "corredor de contract tests", "banco de medición de latencia p95", etc.). No aparece ningún producto comercial ni framework concreto (xUnit, Moq, Stryker, FsCheck, etc.). Las únicas menciones de stack son referencias de contexto admisibles: geovial-storage/plan-pruebas §1 nombra `geovial-api` y "SQL Server" para excluirlos del alcance (es el consumidor, no tooling propio); aplicada-sync referencia el directorio de sample `03-avanzado-demo-maui` y "demo MAUI" tomados del intake §18/§16.1 (nombre de carpeta de sample, no adopción de stack). Conforme.
- **D6 (dominio fuente del bootstrap):** vocabulario neutral de librería. Los códigos de error en mayúsculas (CONTENIDO_VACIO, PROVEEDOR_NO_DISPONIBLE, IDENTIFICADOR_DUPLICADO, BACKEND_INALCANZABLE, etc.) y términos como "cola", "marcador" (en el dominio GeoVial, no del bootstrap), "proveedor", "sesión" son vocabulario de contrato propio. Sin términos del fuente SDD 1.0 (Motor DSL). Conforme.
- **D7 (sufijo de dominio en filename):** ninguno de los dieciocho archivos lleva `-sync`, `-storage`, `-motor`, `-library` ni otro marcador de dominio; todos describen el rol del documento. Los IDs internos (TC-XX, CU-XX, RN-XX, NFR-XX) son de dos dígitos y contiguos. Conforme.

---

## 3. Matriz de estructura obligatoria (§6 library)

### 3.1 Conjunto de documentos

| Requisito §6 | aplicada-sync | geovial-storage |
| --- | --- | --- |
| estrategia-calidad_v1.0.md | Presente | Presente |
| estrategia-testing_v1.0.md | Presente | Presente |
| plan-pruebas_v1.0.md | Presente | Presente |
| matriz-cobertura-pruebas_v1.0.md | Presente | Presente |
| casos-prueba-referenciales_v1.0.md | Presente | Presente |
| criterios-validacion_v1.0.md | Presente | Presente |
| definition-of-done_v1.0.md | Presente | Presente |
| guia-testing-extensibilidad_v1.0.md (obligatoria: tiene_extensibilidad) | Presente | Presente |
| README.md (recomendado) | Presente | Presente |

Ambos `tiene_extensibilidad=true` (aplicada-sync: cuatro estrategias de extensión —almacén local, transporte, credencial, conectividad—; geovial-storage: puerto de proveedor de almacenamiento). La guía es exigible y está presente en ambos. Cabecera obligatoria (H1 + bloque de metadatos Proyecto/Documento/Versión/Estado/Fecha/Autor) presente en los dieciocho archivos.

### 3.2 Cumplimiento sustantivo del §6 — aplicada-sync

| Criterio §6 | Resultado |
| --- | --- |
| Pirámide 80/15/5 numérica + justificada | OK — estrategia-testing §1: 80 unit / 15 integration / 5 snapshot, sin e2e (justificado: library sin UI); tabla con justificación por nivel y contra pirámide invertida/aplanada |
| Cobertura por capa (no número global único) | OK — dominio 85/80/60, API pública 100 % contract / 90 / 60, infraestructura 70/60; declarado "nunca como número global único" (§2) |
| Matriz con las TRES tablas + cobertura por capa | OK — §2 CU↔TC, §3 NFR↔TC, §4 RN↔TC, §5 cobertura por capa, §6 gaps |
| Cada TC referencia ≥1 CU/RN/NFR | OK — los 21 TC (TC-01..21) declaran "Cubre:" con CU/RN/NFR; §3 del catálogo lo consolida y declara "sin TC huérfanos" |
| Cada NFR numérico tiene TC | OK — 6 NFR de 05 §8 con TC asignado (lote→TC-20, cola→TC-14, reanudación→TC-09/10, idempotencia→TC-11/12, orden→TC-07/13, conflicto→TC-16) |
| DoD por capa, criterios verificables mecánicamente | OK — §1.1 US, §1.2 BT, §1.3 tramo, §1.4 release; cada criterio con "Validación: gate Gx / estado en matriz / métrica" |
| DoD no redefinida en 07 | OK — 07 §5 referencia la DoD de 08 como pendiente; DoD §3 "los planes de 07 referencian, no redefinen" |

### 3.3 Cumplimiento sustantivo del §6 — geovial-storage

| Criterio §6 | Resultado |
| --- | --- |
| Pirámide 80/15/5 numérica + justificada | OK — estrategia-testing §1: 80 unit / 15 integration / 5 e2e+snapshot, justificada contra invertida/aplanada; contract tests cuentan como integración (§2.2 lo admite). Obs P3: el README rotula la pirámide como "80/15/5 e2e+snapshot" mientras estrategia-testing la detalla como "5 e2e"; matiz de redacción sin impacto numérico (H-GS-02) |
| Cobertura por capa (no número global único) | OK — dominio 85/80/60, infraestructura 70/60; "se reporta por capa, no como número global único" (matriz §5) |
| Matriz con las TRES tablas + cobertura por capa | OK — §2 CU↔TC, §3 NFR↔TC, §4 RN↔TC, §5 cobertura por capa, §6 gaps |
| Cada TC referencia ≥1 CU/RN/NFR | OK — los 28 TC (TC-01..28) declaran "Cubre:"; §3 del catálogo consolida la cobertura |
| Cada NFR numérico tiene TC | OK — NFR-01→TC-25, NFR-02→TC-26, NFR-03→TC-21, NFR-04→TC-05/23, NFR-05→TC-08/15/24; NFR-06 (cobertura) se valida como gate G-03 sobre la suite (no como TC individual), declarado explícitamente y admisible (es un gate de cobertura, no un SLA con caso de prueba propio) |
| DoD por capa, criterios verificables mecánicamente | OK — §1.1 US, §1.2 BT, §1.3 sprint/tramo, §1.4 release; cada criterio con "Validación: gate G-0x / TC / métrica" |
| DoD no redefinida en 07 | OK — 07 §5 referencia la DoD de 08 como pendiente y suma criterios específicos del plan "sin reemplazarla"; DoD §3 confirma vigencia canónica |

---

## 4. Coherencia cross-doc

### 4.1 aplicada-sync

- **Upstream CU/RN:** CU-01..06 y RN-01..03 referenciados en matriz, catálogo, estrategias y DoD existen como archivos en `02/casos-de-uso/` y `02/reglas-de-negocio/`. Sin referencias colgantes.
- **NFR vs 05 §8 vs intake §17 P.10:** los seis NFR de la matriz (Tiempo de lote, Capacidad de cola, Reanudación sin pérdida, Idempotencia, Orden, Continuidad ante conflicto) coinciden literalmente con `05/arquitectura-solucion_v1.0.md` §8 (mismos objetivos numéricos: lote 100 ≤ 30 s, cola ≥ 1000, 0/0 reanudación, 100 % idempotencia, 0 bajadas antes de subida, 0 ciclos abortados) y con el intake §17 P.10 de aplicada-sync. Coherente.
- **Gate global §17 P.6 ↔ cobertura por capa:** reconciliación declarada y coherente en estrategia-calidad §3 (nota), estrategia-testing §2 y matriz §5: el dominio (85/80) supera el piso global (80/70), la infraestructura (70/60) cumple el piso de su capa, el agregado ponderado no cae bajo el global; el gate G4 verifica las tres condiciones simultáneamente.
- **DoD ↔ mini-plan de 07:** el mini-plan §5 declara "la DoD canónica vive en 08 y aún no está generada... este mini-plan la referencia como pendiente"; la DoD §1.4 y §3 confirman que es la fuente canónica que 07 referencia. Sin redefinición.
- **TC ↔ tramos:** el plan-pruebas §5 asigna TC a R1/R2/R3 de forma consistente con el catálogo y con los tramos del mini-plan de 07 (R1 MVP, R2 disparo/observabilidad, R3 errores/capacidad/compatibilidad). Coherente.
- **README:** índice fiel; los gates G1-G9, la pirámide, los TC-01..21 y la DoD canónica coinciden con los documentos fuente.

### 4.2 geovial-storage

- **Upstream CU/RN:** CU-01..06 y RN-01..03 referenciados existen como archivos en `02/casos-de-uso/` y `02/reglas-de-negocio/`. Sin referencias colgantes.
- **NFR vs 05 §8 vs intake §17 P.10:** los seis NFR (NFR-01 latencia p95 ≤ 1 s para ≤ 5 MB local, NFR-02 tamaño máx 25 MB configurable, NFR-03 transparencia 0 diferencias, NFR-04 integridad byte a byte, NFR-05 no filtración, NFR-06 cobertura ≥ 80/≥ 70) coinciden con `05/arquitectura-solucion_v1.0.md` §8 y con el intake §17 P.10 de geovial-storage. Coherente.
- **Gate global §17 P.6 ↔ cobertura por capa:** reconciliación declarada y coherente en estrategia-calidad §3, estrategia-testing §2 y matriz §5, con el argumento de ponderación (dominio concentra la lógica, adaptadores delgados); G-03 (global) y G-04 (por capa) declarados compatibles. Coherente.
- **DoD ↔ mini-plan de 07:** el mini-plan §5 referencia la DoD de 08 como pendiente ("enlazar este plan a la DoD canónica cuando 08 publique su artefacto"); la DoD de 08 declara que esa nota de pendencia "queda satisfecha por este documento". Sin redefinición.
- **TC ↔ tramos (DEFECTO):** el plan-pruebas §5 desalinea TC con los cinco tramos del mini-plan de 07 (ver H-GS-01, P1). El mini-plan §3 fija Tramo 4 = BT-07/BT-06/US-08/US-09 (configuración del proveedor, CU-06) y Tramo 5 = BT-08/BT-12/BT-09/BT-10 (proveedores intercambiables y batería de contrato); el plan-pruebas §5 asigna al Tramo 4 los TC-13/TC-14/TC-15 (que cubren CU-05 listar, no CU-06) y nombra al Tramo 5 los "TC-18, TC-19" como batería de transparencia (cuando TC-18/TC-19 son CU-06 CA-03/CA-04 y la batería real es TC-21/TC-22/TC-27). Los TC-23 a TC-28 no se asignan a ningún tramo en esa tabla (TC-25/TC-26 solo aparecen implícitos en la prosa). No rompe la matriz ni la trazabilidad upstream/downstream.
- **README:** índice mayormente fiel; gates G-01..G-09, DoD canónica y cobertura por capa coinciden. Único matiz: el rótulo de pirámide (H-GS-02, P3).

---

## 5. Trazabilidad TC ↔ CU/RN/NFR (TC huérfanos y requisitos huérfanos)

### 5.1 aplicada-sync

| Dimensión | Resultado |
| --- | --- |
| CU sin TC | Ninguno — CU-01..06 cubiertos (resumen del catálogo §3 y matriz §2) |
| RN sin TC | Ninguno — RN-01 (TC-07/09/13), RN-02 (TC-04/05/11/12), RN-03 (TC-16/19) |
| NFR numérico sin TC | Ninguno — los 6 NFR de 05 §8 tienen TC en matriz §3 |
| TC huérfano (sin upstream) | Ninguno — los 21 TC declaran CU/RN/NFR; TC-21 referencia ADR-03/contrato/CU-01..06/BT-14 |

Cada criterio Given/When/Then de la tabla de aceptación de cada CU tiene su fila en la matriz §2. CU-03 y CU-06 (críticos del MVP) suman property-based (TC-12, TC-13). Sin huérfanos en ninguna dirección.

### 5.2 geovial-storage

| Dimensión | Resultado |
| --- | --- |
| CU sin TC | Ninguno — CU-01..06 cubiertos (catálogo §3 y matriz §2) |
| RN sin TC | Ninguno — RN-01 (TC-16/21/22/27/28), RN-02 (TC-01/03/04/05/07/09/12/23), RN-03 (TC-08/15/17/19/24) |
| NFR numérico sin TC | Ninguno — NFR-01..05 con TC; NFR-06 (cobertura) como gate G-03, admisible |
| TC huérfano (sin upstream) | Ninguno — los 28 TC declaran CU/RN/NFR o extensibilidad/ADR (TC-22 ADR-02/03; TC-27/28 RN-01+extensibilidad) |

La trazabilidad bidireccional en la matriz §2 (cada criterio CA-XX de 02 con su TC) y el resumen §3 del catálogo están completos. El defecto de H-GS-01 es de asignación de TC a tramos en el plan-pruebas, NO de cobertura: en la matriz todo requisito conserva su TC y todo TC su requisito.

---

## 6. Hallazgos

Nivel / archivo / sección / evidencia / recomendación.

### Comunes a ambos proyectos

**H-COM-01 (P2) — Finales de línea CRLF en el working tree (reconciliación de tooling).**
- Archivos: los dieciocho entregables de Fase E.
- Sección: encoding (D2).
- Evidencia: el working tree muestra CRLF por `core.autocrlf=true` en este checkout Windows; sin embargo `git show :<path>` confirma que los blobs commiteados están en LF (cero bytes CR) y el `.gitattributes` del repositorio fija `* text=auto eol=lf` y `*.md text eol=lf`. El encoding es UTF-8 sin BOM.
- Recomendación: ninguna acción de contenido; D2 se cumple en el repositorio. Se documenta solo como reconciliación de la diferencia working-tree vs índice, ya resuelta por `.gitattributes` (mejora respecto del estado de la Fase D). No bloqueante.

### aplicada-sync

**H-AS-01 (P3) — Estado uniforme "Pendiente" en toda la suite como línea de base.**
- Archivo: 08/matriz-cobertura-pruebas_v1.0.md §2-§5, 08/casos-prueba-referenciales_v1.0.md §2.
- Sección: estado de los TC y cobertura por capa.
- Evidencia: los 21 TC y la cobertura por capa figuran como "Pendiente" / "pendiente". El propio documento lo justifica como línea de base previa al tramo R1 (matriz §1 y §6, catálogo §1) y se compromete a actualizar al cierre de cada tramo (anti-patrón de matriz desactualizada, 08_rules §4.10). Es coherente con un proyecto que aún no implementó la suite (07 arranca 2026-06-15).
- Recomendación: ninguna acción obligatoria; el estado "Pendiente" está correctamente declarado como línea de base, no es un gap de diseño. No bloqueante.

### geovial-storage

**H-GS-01 (P1) — Desalineación de TC con los tramos del mini-plan en el plan de pruebas y etiquetado erróneo de la batería de transparencia.**
- Archivos: 08/plan-pruebas_v1.0.md §5 (Plan por tramo); 08/casos-prueba-referenciales_v1.0.md §1 (Propósito).
- Sección: plan por sprint/tramo; convenciones del catálogo.
- Evidencia:
  1. plan-pruebas §5, Tramo 4 (BT-07/BT-06/US-08/US-09, que el mini-plan de 07 §3 dedica a configuración del proveedor, CU-06) lista como TC referenciales "TC-13, TC-14, TC-15, TC-20"; pero TC-13/TC-14/TC-15 cubren CU-05 (listar) según el catálogo y la matriz, no CU-06. Los TC de CU-06 son TC-16, TC-17, TC-18, TC-19, TC-20.
  2. plan-pruebas §5, Tramo 5 (BT-08/BT-12/BT-09/BT-10, proveedores y batería de contrato) lista "TC-18, TC-19" como la batería de transparencia por proveedor; pero TC-18 (proveedor inaccesible, CU-06 CA-03) y TC-19 (autorización insuficiente, CU-06 CA-04) no son la batería de transparencia: la batería única de transparencia es TC-21 (contrato por proveedor), TC-22 (snapshot del contrato) y TC-27 (conformidad de proveedor nuevo).
  3. La misma confusión aparece en casos-prueba §1: "Los contract tests (TC-18, TC-19) validan que la superficie pública no cambia entre proveedores (transparencia, RN-01)" — debería citar TC-21/TC-22/TC-27.
  4. Los TC-23 a TC-28 no se asignan a ningún tramo en la tabla §5 del plan-pruebas (TC-25/TC-26 solo figuran implícitos en la prosa del Tramo 5; TC-23/TC-24/TC-27/TC-28 no aparecen).
- Impacto: defecto de consistencia interna entre 08 (plan) y 08 (catálogo/matriz) y entre 08 y 07. NO rompe la trazabilidad upstream/downstream (en la matriz cada CU/RN/NFR mantiene su TC y cada TC su requisito), por lo que es P1, no P0.
- Recomendación: corregir el §5 del plan-pruebas para que las filas por tramo citen los TC que efectivamente corresponden a sus BT/US (Tramo 3 lectura/borrado/verificación/listado → TC-05..15; Tramo 4 configuración CU-06 → TC-16..20; Tramo 5 transparencia/desempeño → TC-21, TC-22, TC-25, TC-26, TC-27, TC-28); y corregir la frase del catálogo §1 para nombrar la batería de transparencia real (TC-21/TC-22/TC-27).

**H-GS-02 (P3) — Rótulo de la pirámide en el README discordante con la estrategia de testing.**
- Archivo: 08/README.md (encabezado de sección).
- Sección: descripción de la pirámide.
- Evidencia: el README dice "La pirámide objetivo es 80 unit / 15 integration / 5 e2e+snapshot"; estrategia-testing §1 la detalla como "80 unit / 15 integration / 5 e2e" (con e2e reducido a snapshots de contrato). Mismo reparto numérico, distinta etiqueta del último nivel.
- Recomendación: alinear el rótulo del README con estrategia-testing. Cosmético, no bloqueante.

**H-GS-03 (P3) — README sin campo "Documento" en la cabecera.**
- Archivo: 08/README.md (cabecera).
- Sección: cabecera obligatoria (§4.1).
- Evidencia: la cabecera del README usa "Tipo (D8)" y "Variante" pero omite el campo `**Documento:**` presente en el resto de artefactos de la sección y en el README de aplicada-sync. El README es recomendado, no obligatorio, por lo que es cosmético (es el mismo patrón ya anotado en la Fase D para este proyecto).
- Recomendación: agregar `**Documento:** README.md` por uniformidad. No bloqueante.

---

## 7. Veredicto

### Por proyecto

- **aplicada-sync: APROBADO.** Sin P0, P1 ni P2 propios. Los nueve artefactos están completos y conformes a la regla 08 §6: pirámide 80/15/5 justificada, cobertura por capa diferenciada, matriz con las tres tablas más cobertura por capa, los 21 TC con upstream explícito, los seis NFR numéricos con TC, DoD canónica por capa con validación mecánica y no redefinida en 07. Trazabilidad a 02/05/06/07 íntegra; reconciliación del gate global §17 P.6 con la cobertura por capa declarada y coherente. Único P3 (estado "Pendiente" como línea de base, correctamente declarado). Puede avanzar.

- **geovial-storage: APROBADO CON OBSERVACIONES.** Sin P0. Los nueve artefactos están completos y conformes en estructura, trazabilidad upstream/downstream, las tres tablas de la matriz, cobertura por capa y DoD canónica no redefinida en 07. El único defecto material es P1 de consistencia interna (H-GS-01): el plan-pruebas §5 cruza TC entre tramos y etiqueta mal la batería de transparencia, y la misma confusión aparece en una frase del catálogo §1; no rompe la trazabilidad (la matriz conserva la cobertura bidireccional). Más dos P3 cosméticos (rótulo de pirámide en README, README sin campo Documento). Sin hallazgo bloqueante: puede avanzar; se recomienda corregir H-GS-01 antes del cierre del repositorio para evitar que el desfase de plan se arrastre a 09/10/11.

### Consolidado

**APROBADO CON OBSERVACIONES.** Conteo total: **P0 = 0, P1 = 1, P2 = 2, P3 = 3.** Ningún hallazgo bloqueante. La Fase E de nivel 0 puede promover a la fase siguiente. Acciones recomendadas: corregir la desalineación TC↔tramo y el etiquetado de la batería de transparencia en geovial-storage (H-GS-01, P1); los demás hallazgos son reconciliaciones o cosméticos.

---

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Informe inicial del audit independiente de Fase E (08 calidad y pruebas, variante library) para los proyectos de nivel 0 aplicada-sync y geovial-storage. Matriz D1-D8, matriz de estructura §6, coherencia cross-doc, trazabilidad TC↔CU/RN/NFR, 6 hallazgos (0 P0 / 1 P1 / 2 P2 / 3 P3) y veredicto APROBADO (aplicada-sync) / APROBADO CON OBSERVACIONES (geovial-storage) / APROBADO CON OBSERVACIONES (consolidado). |
