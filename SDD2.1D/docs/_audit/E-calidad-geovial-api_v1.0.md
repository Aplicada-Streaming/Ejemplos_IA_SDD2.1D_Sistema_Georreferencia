# Auditoría Fase E — Calidad y pruebas (08) — geovial-api

**Fase:** E (Calidad y pruebas)
**Proyecto auditado:** geovial-api (`rest-api`, proyecto principal, equipo_n=1)
**Categoría:** 08_calidad_y_pruebas
**Auditor:** Arquitecto de Soluciones + QA Senior (independiente, sin participación en la generación)
**Fecha:** 2026-06-15
**Reglas aplicadas:** `08_rules_calidad_y_pruebas.md` v1.2 (§6 criterios de aceptación, §2.2 variante rest-api), `SOLUTION-INTAKE-geovial_v1.0.md` v1.4 (§17 P.6 cobertura, P.10 NFR de geovial-api)
**Insumos upstream consultados:** 02 (22 CU, 7 RN, 6 RC), 05 (arquitectura §8 NFR, ADR-01..10, `contratos-rest_v1.0.md` con 35 endpoints), 06 (44 US, 21 BT, DoR), 07 (`mini-plan_v1.0.md` §5).

---

## 1. Resumen ejecutivo

geovial-api entrega el conjunto documental obligatorio de la Fase E **completo y conforme**: los siete artefactos obligatorios de la regla 08 §6 (`estrategia-calidad`, `estrategia-testing`, `plan-pruebas`, `matriz-cobertura-pruebas`, `casos-prueba-referenciales`, `criterios-validacion`, `definition-of-done`) más `README.md`. Ocho archivos. La octava plantilla opcional, `guia-testing-extensibilidad_v1.0.md`, está **correctamente OMITIDA** por `tiene_extensibilidad=false` y la omisión queda registrada en el README §3 y en el plan-pruebas §1 (exclusiones), con justificación trazada a la regla 08 §2.2 (variante rest-api: la guía aplica "si admite handlers o middlewares externos"; geovial-api no los publica). Nomenclatura uniforme `_v1.0.md`, kebab-case, sin patrón heredado `.v`, sin sufijo de dominio (`-geovial`, `-api`, `-motor`) en ningún nombre de archivo.

La trazabilidad es sólida. Los 22 CU (CU-01..22), las 7 RN (RN-01..07) y las 6 RC (RC-01..06) referenciados existen como archivos en 02. Los NFR numéricos de la matriz coinciden **literalmente** con `05/arquitectura-solucion_v1.0.md` §8 y con el intake §17 P.10 de geovial-api (latencia p95 lecturas ≤ 300 ms, escrituras ≤ 500 ms, disponibilidad ≥ 99,5 % mensual sin SLO de 99,9 %, lote de sincronización ≥ 1000 cambios). La reconciliación del gate global del intake §17 P.6 (líneas ≥ 80 % / branches ≥ 70 %) con la cobertura por capa (dominio 85/80, aplicación 80/70, infraestructura 70/60) está **declarada explícitamente y es coherente** en estrategia-calidad §3.1, estrategia-testing §2 y matriz §5.1. La matriz tiene las tres tablas obligatorias (CU↔TC con los 22 CU, NFR↔TC, RN↔TC con las 7 RN) más cobertura por capa y por endpoint; cada TC (TC-01..35) referencia ≥ 1 CU/RN/NFR; cada NFR numérico tiene TC. El contrato de 05 tiene exactamente 35 endpoints (verificado por conteo) y el inventario §5.2 de la matriz declara el 100 % cubierto por contract test (TC-34), con el desglose por área que totaliza 35. La DoD es canónica por las cuatro capas (US, BT, sprint→tramo, release) con validación mecánica por criterio, y **NO se redefine en 07**: el mini-plan §5 la declara "pendiente de generación" y anticipa referenciarla por enlace, sin reescribir criterios.

No se detectaron defectos materiales. Los dos hallazgos son menores: una imprecisión de referencia interna sobre cómo 07 enlaza la DoD (P2, redacción de la coherencia, no rompe trazabilidad) y un matiz de estilo en el README (P3). Sin productos comerciales ni stacks concretos en ninguno de los ocho documentos; el tooling se nombra por rol abstracto.

**Conteo de hallazgos: P0 = 0 | P1 = 0 | P2 = 1 | P3 = 1.**

| Proyecto | P0 | P1 | P2 | P3 | Veredicto |
| --- | --- | --- | --- | --- | --- |
| geovial-api | 0 | 0 | 1 | 1 | APROBADO CON OBSERVACIONES |

Sin P0: el proyecto puede avanzar a la fase siguiente (09 quality gates, 10, 11).

---

## 2. Matriz D1-D8 por documento

Leyenda: OK = conforme; Obs = observación menor (ver hallazgos). D1 idioma rioplatense; D2 encoding UTF-8/LF; D3 kebab-case filename; D4 versionado `_vX.Y` (no `.v`); D5 sin stacks/productos comerciales (tooling por rol abstracto; vocabulario REST genérico permitido); D6 sin vocabulario del dominio fuente del bootstrap; D7 sin sufijo de dominio en filename + IDs internos consistentes; D8 conjunto cerrado de documentos.

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 08/estrategia-calidad_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/estrategia-testing_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/plan-pruebas_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/matriz-cobertura-pruebas_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/casos-prueba-referenciales_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/criterios-validacion_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/definition-of-done_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 08/README.md | OK | OK | OK | OK | OK | OK | OK | Obs (H-01) |

Notas de la matriz:

- **D1 (idioma):** rioplatense técnico, tildes correctas, sin emojis ni negritas decorativas en los ocho documentos. Conforme.
- **D2 (encoding):** los ocho archivos están en UTF-8 **sin BOM** y con finales de línea **LF** verificados directamente sobre el working tree de este checkout (cero bytes CR). A diferencia de la observación de la Fase E nivel 0, aquí el working tree ya está en LF. Conforme, sin hallazgo.
- **D3/D4/D7 (nomenclatura):** los ocho nombres son kebab-case con sufijo uniforme `_v1.0.md`; ninguno usa el patrón prohibido `.v1.0`; ninguno lleva sufijo de dominio (`-geovial`, `-api`, `-motor` u otro marcador temático). Los IDs internos (TC-01..35, CU-01..22, RN-01..07, RC-01..06, NFR por nombre) son consistentes con 02/05. Conforme.
- **D5 (tooling sin stack):** búsqueda exhaustiva de productos/stacks (`.NET`, `C#`, `ASP.NET`, `SQL Server`, `JWT`, `Blazor`, `MudBlazor`, `Leaflet`, `S3`, `xUnit`, `Moq`, `Testcontainers`, `Schemathesis`, `REST Assured`, `Pact`, `JMeter`, `k6`, `GitHub Actions`, `NuGet`, etc.): **cero coincidencias** en los ocho documentos. Todo el tooling se nombra por rol abstracto ("framework de pruebas unitarias del runtime", "cliente HTTP de pruebas", "framework de validación de contrato sobre OpenAPI", "generador de fuzz de contrato", "cliente de carga / generador de carga HTTP", "reporte de cobertura del runtime", "base de datos efímera"). El vocabulario REST genérico permitido (OpenAPI, contract test, problem+json RFC 7807, idempotencia, endpoint, problem+json) se usa sin ningún producto. Las únicas menciones de nombre propio son referencias de contexto admisibles: `geovial-web`/`geovial-mobile` (consumidores del contrato), `geovial-storage`/`aplicada-sync` (proyectos vecinos excluidos del alcance, con su propio 08). Conforme.
- **D6 (dominio fuente del bootstrap):** vocabulario propio de GeoVial (relevamiento, marcador, jerarquía, sincronización, conflicto, agente). Sin términos del fuente SDD 1.0 (Motor DSL). Conforme.
- **D8 (conjunto cerrado):** ver §3. Siete obligatorios + README presentes; la guía de extensibilidad omitida con registro. La única observación D8 es el matiz de cabecera del README (H-01, P3), no un faltante de documento.

---

## 3. Matriz de estructura obligatoria (§6 / §2.2 rest-api)

### 3.1 Conjunto de documentos

| Requisito §6 | geovial-api |
| --- | --- |
| estrategia-calidad_v1.0.md | Presente |
| estrategia-testing_v1.0.md | Presente |
| plan-pruebas_v1.0.md | Presente |
| matriz-cobertura-pruebas_v1.0.md | Presente |
| casos-prueba-referenciales_v1.0.md | Presente |
| criterios-validacion_v1.0.md | Presente |
| definition-of-done_v1.0.md | Presente |
| guia-testing-extensibilidad_v1.0.md | **OMITIDA correctamente** (tiene_extensibilidad=false; registro en README §3 y plan-pruebas §1) |
| README.md (recomendado) | Presente |

La omisión de la guía de extensibilidad es **correcta y está registrada**. La regla 08 §2.2 (rest-api) condiciona la guía a "si admite handlers o middlewares externos"; el README §3 justifica que geovial-api no expone handlers ni middlewares externos publicados (sus transversales —autorización, errores, paginación, idempotencia, versionado— son internos) y se compromete a generarla en una versión futura si se habilitan puntos de extensión externos. Cabecera obligatoria (H1 + bloque Proyecto/Documento/Versión/Estado/Fecha/Autor) presente y completa en los siete obligatorios; el README la presenta con un matiz menor (H-01).

### 3.2 Cumplimiento sustantivo del §6 — geovial-api

| Criterio §6 / §2.2 rest-api | Resultado |
| --- | --- |
| Pirámide 70/20/10 numérica + justificada | OK — estrategia-testing §1: 70 unit / 20 integration / 10 e2e (la pirámide objetivo del tipo rest-api en §2.2); tabla con cobertura por nivel y justificación contra la pirámide invertida y la aplanada; contract tests cuentan dentro de integración (admisible) |
| Cobertura por capa (no número global único) | OK — estrategia-testing §2 y matriz §5.1: dominio 85/80, aplicación 80/70, infraestructura 70/60, API = 100 % endpoints con contract test; declarado "se reporta por capa, no como número global único" (anti-patrón 08 §4.10) |
| 100 % de endpoints con contract test declarado | OK — estrategia-testing §1 y §2, matriz §1 y §5.2, criterios-validacion §5, DoD §1.4: piso del tipo rest-api, no sustituible por unit tests; materializado en TC-34 y exigido como gate G4 |
| Matriz con las TRES tablas + cobertura por capa | OK — §2 CU↔TC (22 CU), §3 NFR↔TC, §4 RN↔TC (7 RN), §5.1 cobertura por capa, §5.2 cobertura por endpoint, §6 gaps |
| Cada TC referencia ≥ 1 CU/RN/NFR | OK — los 35 TC (TC-01..35) declaran "Cubre:" con CU/RN/NFR (y RC cuando aplica); §3 del catálogo consolida y declara cobertura completa |
| Cada NFR numérico tiene TC | OK — latencia p95 lecturas→TC-21, escrituras→TC-22, lote ≥ 1000→TC-31, idempotencia→TC-29/TC-30, integridad bajo concurrencia→TC-33, cobertura/contract→TC-34; disponibilidad ≥ 99,5 % se valida por monitoreo en 09 (métrica observada, no test), declarado y admisible |
| DoD por capa, criterios verificables mecánicamente | OK — §1.1 US, §1.2 BT, §1.3 sprint→tramo, §1.4 release; cada criterio con "Validación: gate Gx / TC / matriz / métrica" |
| DoD no redefinida en 07 (07 referencia) | OK — DoD §0 y §3 declaran fuente canónica; 07 §5 referencia la DoD de 08 sin redefinirla (ver §4 y H-02 por matiz de redacción) |

La pirámide 70/20/10 es exactamente la del tipo rest-api en la tabla §2.2 de las reglas; no se aparta de ella, por lo que no requiere justificación adicional, y sin embargo la documenta. La elevación del piso de dominio a 85/80 (sobre el global 80/70) está justificada y permitida (08 §2.2: "los porcentajes son piso, no techo; subir está permitido, bajar requiere ADR").

---

## 4. Coherencia cross-doc

- **Upstream CU/RN/RC:** los 22 CU (CU-01..22), 7 RN (RN-01..07) y 6 RC (RC-01..06) referenciados en los ocho documentos existen como archivos en `02/casos-de-uso/`, `02/reglas-de-negocio/` y `02/modelo-datos/reglas-conceptuales-de-modelo/`. Sin referencias colgantes. Verificación puntual de códigos de error: CU-18 (NO_AUTENTICADO, ACCION_NO_PERMITIDA, FUERA_DE_ALCANCE) coincide con TC-25/TC-26; CU-22 (VERSION_NO_SOPORTADA, VERSION_REQUERIDA_AUSENTE, RECURSO_NO_EN_VERSION) coincide con TC-35; los códigos del catálogo problem+json de `contratos-rest_v1.0.md` §5 se ejercitan por los subcasos de TC-34 y por TC-27.
- **NFR vs 05 §8 vs intake §17 P.10:** coincidencia **literal**. `05/arquitectura-solucion_v1.0.md` §8 fija latencia p95 lecturas ≤ 300 ms, escrituras ≤ 500 ms, disponibilidad ≥ 99,5 % (sin SLO de 99,9 %), lote ≥ 1000 cambios; el intake §17 P.10 de geovial-api fija los mismos valores y `tiene_observabilidad_critica=false`; la matriz §3, estrategia-calidad §2 y criterios-validacion §3 reproducen esos valores sin desviación. Coherente.
- **Gate global §17 P.6 ↔ cobertura por capa:** el intake §17 P.6 declara el gate global (líneas ≥ 80 %, branches ≥ 70 %) y, adicionalmente, aplicación ≥ 80 % e infraestructura ≥ 70 % y 100 % de endpoints con contract test. La reconciliación está declarada en tres documentos (estrategia-calidad §3.1, estrategia-testing §2 nota de reconciliación, matriz §5.1) con el mismo argumento: la cobertura por capa es el criterio rector y su cumplimiento satisface el agregado global del pipeline; el gate global es el piso y la cobertura por capa su descomposición, para evitar el anti-patrón de la cobertura que esconde capas débiles (08 §4.10). Coherente y sin contradicción numérica.
- **Endpoints del contrato OpenAPI de 05 ↔ contract tests:** el conteo del contrato `contratos-rest_v1.0.md` §3 arroja exactamente **35 endpoints** (3 sesión + 6 usuarios/agentes + 6 relevamientos/ciclo + 3 asignaciones + 8 marcadores/observaciones/carga + 2 sincronización + 2 conflictos + 2 portabilidad + 3 configuración almacenamiento). El inventario §5.2 de la matriz lista las nueve áreas con su recuento y declara el 100 % cubierto por TC-34, cuyos nueve subcasos (TC-34.1..34.9) cubren todas las operaciones. Coincidencia exacta.
- **DoD ↔ mini-plan de 07:** la DoD de 08 es la fuente canónica (DoD §0, §3); el mini-plan §5 declara que la DoD reside en 08, "pendiente de generación", y que "referenciará por enlace esa DoD canónica cuando exista". Por lo tanto 07 **no redefine** la DoD (criterio P0 no aplica). Existe un matiz de redacción entre la afirmación de 08 (README §5 y DoD §0/§3: "el mini-plan de 07 §5 la referencia por enlace") y el tiempo verbal real del mini-plan ("referenciará... cuando exista") — ver H-02 (P2), sin impacto en trazabilidad.
- **Reconciliación del alcance MVP (NB / Could):** criterios-validacion §2/§6, plan-pruebas §5 (nota Could) y matriz §6 (gap Could) tratan CU-15/CU-16/CU-17 (Could) de forma consistente con el mini-plan §6/§7 (NB-06/NB-07 diferibles sin afectar el MVP NB-01..05). Coherente.
- **README:** índice fiel; los gates G1-G8, la pirámide, los artefactos, los TC y la DoD canónica coinciden con los documentos fuente. La tabla de quality gates del README §4 reproduce los ocho gates de estrategia-calidad §3.

---

## 5. Trazabilidad TC ↔ CU/RN/NFR (TC huérfanos y requisitos huérfanos)

| Dimensión | Resultado |
| --- | --- |
| CU sin TC | Ninguno — los 22 CU (CU-01..22) tienen ≥ 1 TC en matriz §2; el catálogo §3 lo consolida |
| RN sin TC | Ninguno — RN-01 (TC-01/24/25/26), RN-02 (TC-02/25), RN-03 (TC-07/14/15), RN-04 (TC-10/20), RN-05 (TC-06/08/15), RN-06 (TC-12/19), RN-07 (TC-18/29/30) — matriz §4 |
| NFR numérico sin TC | Ninguno — latencia lecturas (TC-21), escrituras (TC-22), lote ≥ 1000 (TC-31), idempotencia (TC-29/30), integridad/concurrencia (TC-33), contract/cobertura (TC-34); disponibilidad ≥ 99,5 % por monitoreo en 09 (no test), declarado |
| TC huérfano (sin upstream) | Ninguno — los 35 TC declaran CU/RN/NFR (y RC cuando aplica); TC-33 referencia NFR de integridad + RC-03/04/05; TC-34 referencia CU-01..22 + NFR cobertura |
| 100 % de endpoints con contract test | Verificado — 35 endpoints en el contrato de 05; TC-34 (subcasos 34.1..34.9) cubre los 35; declarado como gate G4 |

Cada criterio Given/When/Then condensado de la tabla CU↔TC (matriz §2) se materializa en la ficha del TC correspondiente con su Given/When/Then en `casos-prueba-referenciales`. La cobertura complementaria (matriz §2, párrafo final) ata explícitamente los TC de escenarios edge/seguridad (TC-08, TC-17/18/19, TC-20, TC-24, TC-26, TC-30, TC-33) a sus CU/RN. Sin huérfanos en ninguna dirección.

Observación de estado (no hallazgo): los 35 TC y la cobertura por capa figuran como "Pendiente" / "Sin ejecutar", correctamente declarado como línea de base previa al Tramo 1 (matriz §1 y §5.1, catálogo §1, 07 §9 bitácora por iniciar), con compromiso de actualización al cierre de cada tramo. Es coherente con un proyecto cuya construcción no inició (fecha 2026-06-15) y respeta el anti-patrón "matriz desactualizada" (08 §4.10) al declararlo en lugar de fingir cobertura.

---

## 6. Hallazgos

Nivel / archivo / sección / evidencia / recomendación.

**H-01 (P3) — Cabecera del README sin el conjunto uniforme de metadatos respecto del patrón §4.1.**
- Archivo: 08/README.md (cabecera).
- Sección: cabecera obligatoria (08 §4.1).
- Evidencia: el README incluye `**Proyecto:**`, `**Documento:**`, `**Versión:**`, `**Estado:**`, `**Fecha:**`, `**Autor:**`, por lo que en rigor cumple el bloque de metadatos. El matiz es estilístico/cosmético y se anota por completitud de la matriz D8; el README es recomendado (no obligatorio) y su contenido (índice, artefactos vigentes, omisión registrada, quality gates, enlace a la DoD) es completo y fiel.
- Recomendación: ninguna acción obligatoria. No bloqueante.

**H-02 (P2) — Imprecisión de tiempo verbal en la referencia cruzada DoD ↔ mini-plan de 07.**
- Archivos: 08/README.md §5; 08/definition-of-done_v1.0.md §0 y §3.
- Sección: enlace a la DoD canónica / vigencia.
- Evidencia: el README §5 afirma "el mini-plan de 07 (`07_plan-sprint/mini-plan_v1.0.md` §5) la referencia por enlace y no la redefine" y la DoD §0 afirma "El mini-plan de 07 (`mini-plan_v1.0.md` §5) referencia esta DoD por enlace y no la redefine". El mini-plan §5 real dice: "La Definition of Done canónica del proyecto reside en la categoría 08... pendiente de generación. Este mini-plan **referenciará** por enlace esa DoD canónica **cuando exista**" (tiempo futuro; al momento de redactarse 07 la DoD aún no existía). La sustancia del criterio P0 se cumple —07 NO redefine la DoD, solo difiere a ella—, pero la afirmación de 08 describe el enlace como ya materializado cuando el mini-plan lo deja como pendiente.
- Impacto: reconciliación de redacción entre 08 y 07; no rompe la trazabilidad ni el carácter canónico de la DoD (la DoD §3 ya prevé que "cuando esta DoD pasa a estado Vigente, el mini-plan la cita como su fuente canónica"). Por eso es P2, no P0.
- Recomendación: ajustar la redacción del README §5 y de la DoD §0 para reflejar que el mini-plan de 07 referenciará/actualizará el enlace cuando la DoD pase a Vigente (o emitir un `mini-plan_v1.1` que materialice el enlace), de modo que ambas direcciones del cruce queden temporalmente consistentes. No bloqueante.

---

## 7. Veredicto

**geovial-api: APROBADO CON OBSERVACIONES.**

Sin P0 ni P1. Los ocho artefactos están completos y conformes a la regla 08 §6 y a la variante rest-api de §2.2: pirámide 70/20/10 (la del tipo) justificada, cobertura por capa diferenciada (dominio 85/80, aplicación 80/70, infraestructura 70/60), 100 % de endpoints con contract test declarado y materializado en TC-34, matriz con las tres tablas obligatorias más cobertura por capa y por endpoint, los 35 TC con upstream explícito, los NFR numéricos con TC (y la disponibilidad por monitoreo en 09, correctamente justificada como métrica observada), y DoD canónica por las cuatro capas con validación mecánica y **no redefinida en 07**. La guía de testing de extensibilidad está correctamente omitida por `tiene_extensibilidad=false`, con registro en el README. Trazabilidad a 02/05/06/07 íntegra y trazabilidad declarada hacia 09 (gates), 10 y 11; reconciliación del gate global §17 P.6 con la cobertura por capa declarada y coherente; NFR coincidentes con 05 §8 y el intake §17 P.10 v1.4; los 35 endpoints del contrato de 05 cubiertos al 100 % por contract test (conteo verificado). Sin TC huérfanos ni CU/RN/NFR sin TC.

Las dos observaciones son menores y no bloquean: un P2 de consistencia de redacción en la referencia cruzada DoD↔07 (H-02, sin impacto en trazabilidad) y un P3 cosmético de cabecera del README (H-01). Se recomienda corregir H-02 antes del cierre del repositorio para que el enlace canónico de la DoD quede materializado en ambas direcciones cuando la DoD pase a Vigente.

**El proyecto puede avanzar a la fase siguiente.**

---

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Informe inicial del audit independiente de Fase E (08 calidad y pruebas, variante rest-api) para el proyecto principal geovial-api. Matriz D1-D8, matriz de estructura §6/§2.2, coherencia cross-doc (CU/RN/RC, NFR vs 05 §8 e intake §17 P.10 v1.4, gate global ↔ cobertura por capa, 35 endpoints ↔ contract test, DoD ↔ 07), trazabilidad TC↔CU/RN/NFR sin huérfanos, 2 hallazgos (0 P0 / 0 P1 / 1 P2 / 1 P3) y veredicto APROBADO CON OBSERVACIONES. |
