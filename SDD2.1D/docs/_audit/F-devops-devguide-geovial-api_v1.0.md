# Auditoría Fase F — DevOps (09) + Developer guide (10) — geovial-api

**Fase:** F (DevOps + Developer guide)
**Proyecto auditado:** geovial-api (`rest-api`, proyecto principal, equipo_n=1)
**Categorías:** 09_devops, 10_developer_guide
**Auditor:** Arquitecto de Soluciones + QA Senior (independiente, sin participación en la generación)
**Fecha:** 2026-06-16
**Reglas aplicadas:** `09_rules_devops.md` v1.3 (§6 criterios de aceptación, §2.2 variante rest-api), `10_rules_developer_guide.md` v1.2 (§6 criterios, §2.2 variante rest-api), `SOLUTION-INTAKE-geovial_v1.0.md` v1.4 (§17 P.5/P.6/P.7/P.8/P.9/P.10 de geovial-api).
**Insumos upstream consultados:** 02 (22 CU, 7 RN, 6 RC), 05 (`arquitectura-solucion_v1.0.md` §5/§7/§8, `contratos-rest_v1.0.md` con 35 endpoints, ADR-01..10), 08 (`estrategia-calidad_v1.0.md` §3 gates G1-G8, `definition-of-done_v1.0.md`, `estrategia-testing_v1.0.md`, `criterios-validacion_v1.0.md`).

---

## 1. Resumen ejecutivo

geovial-api entrega el conjunto documental obligatorio de la Fase F **completo y conforme** en ambas categorías.

**09_devops (7 archivos):** los seis artefactos obligatorios de 09_rules §6 (`pipeline-ci-cd`, `estrategia-versionado`, `entornos-deploy`, las **DOS** guías de publicación `guia-publicacion-image-docker` y `guia-publicacion-openapi`, `supply-chain-seguridad`) más `README.md`. La doble guía de publicación exigida a `rest-api` por la tabla §2.2 (image-docker + OpenAPI versionado) está **presente y completa**, cada una con su artefacto distinto y sin solaparse. `pipeline-solucion` de nivel solución está correctamente **fuera de alcance** de esta carpeta (README §"Nivel solución" lo difiere a `_solucion/` en Fase H).

**10_developer_guide (7 archivos):** los seis obligatorios de rest-api de 10_rules §2.2 (`conceptos-fundamentales`, `guia-onboarding-developer`, `referencia-api`, `troubleshooting`, `glosario-tecnico` y la guía de integración recomendada `guia-integracion-cliente-http`) más `README.md`. La guía de integración usa el slug **genérico** `cliente-http`, no un nombre comercial.

La trazabilidad es sólida en las dos direcciones que el master-prompt exige:

- **Paridad 35 endpoints (10 ↔ 05):** verificada por conteo. `contratos-rest_v1.0.md` §3 lista 35 operaciones (3+6+6+3+8+2+2+2+3 por área). `referencia-api_v1.0.md` §2 reproduce **literalmente** las mismas 35 operaciones por las mismas nueve áreas, con idéntico método, ruta (sin el prefijo `/v1` que la referencia factoriza en `$BASE`), seguridad e idempotencia. Paridad uno-a-uno confirmada, incluida la taxonomía de errores §5/§7 y los esquemas §4/§3.
- **Pipeline ejecuta los gates de 08 (09 ↔ 08):** `pipeline-ci-cd_v1.0.md` §3 materializa G1..G8 como stages con el mapa explícito (G1=Build, G2=Test unit+integración, G3=Cobertura, G4=Test contract, G5=Validación de contrato, G6=Análisis estático, G7=Regresión, G8=NFR), citando para cada uno el criterio DoD (DoD US §1.1, BT §1.2, release §1.4) o el NFR de 05 §8 que verifica, y declarando explícitamente que **no redefine** la DoD ni la cobertura de 08. Coincide con la definición canónica de `estrategia-calidad_v1.0.md` §3 y `definition-of-done_v1.0.md`.
- **NFR numéricos con stage verificador:** los seis NFR de 05 §8 (latencia p95 lecturas ≤ 300 ms / escrituras ≤ 500 ms, lote ≥ 1000, idempotencia 100 %, integridad 0 violaciones, disponibilidad ≥ 99,5 % sin SLO 99,9 % por `tiene_observabilidad_critica=false`) están en la tabla §3.1 con su TC (TC-21, TC-22, TC-31, TC-29/30, TC-33) y ambiente de medición (STAGING equivalente al productivo para G8). Coherente con la matriz de 08.
- **Versionado §17 P.7:** SemVer 2.0.0 + Conventional Commits 1.0.0 + GitVersion + trunk-based con `main` protegida, exactamente lo pre-tomado en el intake §17 P.7. Artefacto desplegable = imagen de contenedor; modelo de promoción = ambientes DEV/QA/STAGING/PROD con canary (no canales de paquete).

El **scan léxico de stack/protocolo prohibido (D7)** sobre el cuerpo de los 14 archivos arroja **cero fugas de stack o protocolo de la capa de aplicación**: ni `.NET`, ASP.NET, SQL Server, EF Core, Blazor, MAUI, SQLite, S3, ni —lo más sensible dado el P0 de Fase B— **ningún** `JWT`, `ROPC` ni `OAuth`. La autenticación se nombra siempre como "token bearer" (fiel a ADR-03, que es igualmente abstracto y declara "no hay IdP externo"). Se detectó **una** ocurrencia literal de un término de la lista prohibida —`OIDC`— en `supply-chain-seguridad_v1.0.md` §2, pero referido a la **identidad keyless de firma del CI** (workload identity de sigstore/keyless), no al protocolo de autenticación de la API; se reporta como hallazgo de higiene léxica P2 (no P0), por las razones de §5. El resto de nombres propios encontrados (CycloneDX, SPDX, SLSA, sigstore/cosign, Dependabot/Renovate, GitVersion, Keep a Changelog) son **vocabulario y tooling explícitamente admitido** por el master-prompt §1 y por 09_rules §2.1/§4.6, y se usan con "u homólogo / o equivalente". El token `image-docker`/`openapi` aparece solo en nombres de archivo, títulos, metadatos y la sección "Alcance" anclada a la tabla §2.2 (uso permitido).

**Encoding (D2):** los 14 archivos están en UTF-8 **sin BOM** (primeros bytes = contenido, no `EF BB BF`) y con finales de línea **LF** (verificado con `grep -cU $'\r$'` = 0 CR en cada archivo, consistente con el resto del working tree). Conforme, sin hallazgo.

**Punto de reconciliación RFC 7807/9457 (instruido por el master-prompt §4):** 10 adopta RFC 9457 mientras 05/ADR-05 y `contratos-rest_v1.0.md` §2/§5 citan RFC 7807. Se evaluó si la nota de paridad de 10 es suficiente. **Lo es**, y además **está mandada por la propia regla 10**: `10_rules_developer_guide.md` §1.2 (variante rest-api: "errores RFC 9457") y §4.5 ("Los códigos de error siguen RFC 9457 (`application/problem+json`)") obligan a la developer guide de rest-api a usar RFC 9457. `referencia-api_v1.0.md` §1 (nota de versión de RFC) y §7, más `glosario-tecnico_v1.0.md`, documentan explícitamente que 9457 obsoleta 7807 **sin cambiar el cuerpo ni el tipo de contenido** y que la estructura es válida bajo ambas. La nota es correcta técnicamente y suficiente como reconciliación; se deja como observación de coherencia menor (P3) para que 05/ADR-05 incorporen la equivalencia en su próximo ciclo y la solución hable de una sola nomenclatura.

No se detectaron defectos materiales que rompan trazabilidad ni omisión de documento obligatorio. Los hallazgos son una higiene léxica (P2), dos reconciliaciones/coherencias (P2/P3) y dos matices de estilo (P3).

**Conteo de hallazgos: P0 = 0 | P1 = 0 | P2 = 2 | P3 = 3.**

| Proyecto | P0 | P1 | P2 | P3 | Veredicto |
| --- | --- | --- | --- | --- | --- |
| geovial-api | 0 | 0 | 2 | 3 | APROBADO CON OBSERVACIONES |

Sin P0: el proyecto puede avanzar a la fase siguiente (11 examples; cierre de bucle y nivel solución en Fase H).

---

## 2. Matriz D1-D8 por documento

Leyenda: OK = conforme; Obs = observación menor (ver hallazgos). D1 idioma rioplatense técnico; D2 encoding UTF-8/LF sin BOM; D3 kebab-case filename; D4 versionado `_vX.Y` (no `.v`); D5 sin stacks/productos de la capa de aplicación (tooling por rol abstracto; vocabulario genérico permitido); D6 sin vocabulario del dominio fuente del bootstrap (Motor DSL); D7 **scan léxico de stack/protocolo prohibido en el cuerpo**; D8 conjunto cerrado de documentos / cabecera y secciones.

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 09/pipeline-ci-cd_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 09/estrategia-versionado_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 09/entornos-deploy_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 09/guia-publicacion-image-docker_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 09/guia-publicacion-openapi_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 09/supply-chain-seguridad_v1.0.md | OK | OK | OK | OK | OK | OK | **Obs (H-01)** | OK |
| 09/README.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 10/conceptos-fundamentales_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 10/guia-onboarding-developer_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 10/guia-integracion-cliente-http_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 10/referencia-api_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 10/troubleshooting_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 10/glosario-tecnico_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 10/README.md | OK | OK | OK | OK | OK | OK | OK | Obs (H-05) |

### 2.1 Fila explícita de scan léxico de stack/protocolo (D7)

Resultado del scan de cuerpo (case-insensitive) por cada término prohibido. "Ausente" = ninguna ocurrencia en cuerpo; "Permitido" = presente solo en uso admitido por el master-prompt §1 / 09_rules.

| Término prohibido | Resultado en los 14 cuerpos | Detalle |
| --- | --- | --- |
| .NET / ASP.NET | Ausente | Runtime nombrado como "runtime del backend en versión LTS", "núcleo abierto". |
| SQL Server / EF Core / Entity Framework | Ausente | Persistencia como "almacén relacional", "migraciones en arranque controlado" (ADR-02). |
| JWT | **Ausente** | Auth = "token bearer" en los 14 archivos (ADR-03). |
| ROPC | **Ausente** | El P0 de Fase B (ROPC/JWT) **no reaparece**. |
| OAuth | **Ausente** | — |
| OIDC | **1 ocurrencia** | `supply-chain-seguridad_v1.0.md` §2: "keyless con OIDC del CI" = identidad de firma keyless del CI, no auth de la API. Ver H-01 (P2). |
| Docker | Ausente como tecnología | Solo en token de tipo-artefacto `image-docker` (filename, título, metadatos, §0 Alcance anclado a tabla §2.2). Cuerpo usa "imagen de contenedor", "orquestador de contenedores". |
| Kubernetes / k8s | Ausente | "orquestador de contenedores", "manifiesto de despliegue del orquestador". |
| Blazor / MAUI | Ausente | Clientes nombrados como `geovial-web`/`geovial-mobile` (referencia de contexto admisible). |
| SQLite | Ausente | — |
| S3 / Amazon S3 | Ausente | Almacenamiento como "destino de almacenamiento de fotos", "servicio de objetos remoto", "librería de almacenamiento" (ADR-09). |

Vocabulario y tooling **admitido** hallado (no es fuga): imagen de contenedor, orquestador de contenedores, token bearer, OpenAPI, SBOM, CycloneDX/SPDX, SLSA, sigstore/cosign (con "u homólogo"), Dependabot/Renovate (con "o equivalente"), GitVersion, Keep a Changelog, canary, problem+json, RFC 7807/9457, idempotencia, paginación, transparency log. Todos avalados por el master-prompt §1 (lista de vocabulario genérico permitido), por 09_rules §2.1/§4.6 (sigstore/cosign, CycloneDX/SPDX, Dependabot/Renovate como ejemplos de tooling) y por el intake §17 P.7 (GitVersion ratificable).

Notas de la matriz:

- **D1:** rioplatense técnico, tildes correctas, sin emojis ni negritas decorativas. Conforme en los 14.
- **D2:** UTF-8 sin BOM, finales LF (0 bytes CR verificados sobre el working tree). Conforme.
- **D3/D4:** los 14 nombres kebab-case con sufijo uniforme `_v1.0.md`; ninguno usa el patrón prohibido `.v1.0`; ninguno lleva sufijo de dominio (`-geovial`, `-api`) salvo el slug de tipo-artefacto (`image-docker`, `openapi`) y el slug de sistema-objetivo (`cliente-http`), todos genéricos y admitidos. Conforme.
- **D7:** ver §2.1. Única observación H-01 (P2) por la ocurrencia léxica de `OIDC`.
- **D8 / cabecera y secciones:** los 14 abren con H1 + bloque de metadatos uniforme (Proyecto/Documento/Versión/Estado/Fecha/Autor). Los seis documentos de contenido de 10 agregan además `Tipo Diátaxis`/`Audiencia`/`Nivel`/`Tiempo estimado de lectura` como exige 10_rules §4.1 (verificado uno a uno). Conjunto cerrado, sin documento de más. Única observación H-05 (P3) sobre la cabecera del README de 10.

---

## 3. Matriz de estructura obligatoria por categoría

### 3.1 09_devops (09_rules §6, variante rest-api §2.2)

| Requisito §6 / §2.2 (rest-api) | geovial-api | Veredicto |
| --- | --- | --- |
| pipeline-ci-cd_v1.0.md (stages lint/build/test/SCA/SBOM/firma/publish + promotion + rollback + notificaciones) | Presente; §3 con 17 stages incl. Test unit/integración/contract, Validación de contrato, Cobertura, SAST, Build de imagen, Firma, Publish OpenAPI, Publish imagen, Regresión, NFR, Deploy | OK |
| estrategia-versionado_v1.0.md (SemVer + Conventional Commits + GitVersion + trunk-based + deprecation) | Presente; §2 SemVer 2.0.0, §3 Conventional Commits, §5 GitVersion, §4 trunk-based, §6 deprecation por URI | OK |
| entornos-deploy_v1.0.md (DEV/QA/STAGING/PROD como **modelo de servicio**, IaC, 12-factor, secretos en vault) | Presente; §1 cuatro ambientes desplegables con canary, §2 IaC, §3 12-factor, §4 vault | OK |
| guia-publicacion-image-docker_v1.0.md (artefacto desplegable) | Presente; pre-requisitos, stage, verificación, rollback por traffic shift, métricas | OK |
| guia-publicacion-openapi_v1.0.md (contrato consumido por clientes) | Presente; pre-requisitos, stage, verificación, versionado por URI, compatibilidad | OK |
| supply-chain-seguridad_v1.0.md (SBOM, firma, SLSA, SCA, SAST/DAST, CVE) | Presente; §1 SBOM CycloneDX, §2 firma + transparency log, §3 SLSA L3 objetivo, §4 SCA, §5 SAST/DAST, §6 CVE | OK |
| README.md (índice + orden de lectura) | Presente; índice, orden de lectura, mapeo gates→stages, NFR→stage, modelo de ambientes | OK |
| Gates ejecutan DoD de 08 sin redefinirla | Cumplido; §3 cita DoD US/BT/release de 08 por gate y declara "no las redefine" | OK |
| Cada NFR numérico con stage que lo verifica | Cumplido; §3.1 los seis NFR con TC y ambiente | OK |
| Ambientes ≠ canales de paquete (no confundir publicación con despliegue) | Cumplido; declarado explícitamente en README, pipeline §1/§6, entornos §1, versionado §6 (anti-patrón §4.8 citado) | OK |
| pipeline-solucion (nivel solución) | Correctamente FUERA DE ALCANCE (Fase H, `_solucion/`); README lo difiere | OK |

### 3.2 10_developer_guide (10_rules §6, variante rest-api §2.2 — obligatoria)

| Requisito §6 / §2.2 (rest-api) | geovial-api | Veredicto |
| --- | --- | --- |
| conceptos-fundamentales_v1.0.md (Explanation) | Presente; concepto central, modelo mental, decisiones ADR-03..10, vocabulario, "qué NO hace" | OK |
| guia-onboarding-developer_v1.0.md (Tutorial; TTFS 5/30/60) | Presente; Hello world <5 min (§2), primer caso real <30 min (§3), integración encadenada <1 h (§4), siguientes pasos | OK |
| guia-integracion-<sistema-objetivo> (slug genérico) | Presente como `guia-integracion-cliente-http_v1.0.md`; slug genérico, sin nombre comercial; §1 lo declara aplicable a cualquier cliente HTTP | OK |
| referencia-api_v1.0.md (curada desde OpenAPI; paridad 35 endpoints) | Presente; §2 las 35 operaciones en paridad 1:1 con 05 §3; esquemas, errores, versionado | OK |
| troubleshooting_v1.0.md (≥5 ISSUE-XX con códigos coincidentes CU/05) | Presente; **6 entradas** ISSUE-01..06; códigos estables coinciden con `contratos-rest_v1.0.md` §5 y CU-18/19/20/21/22 | OK |
| glosario-tecnico_v1.0.md (kebab-case + cross-doc) | Presente; 28 términos kebab-case con definición operativa y referencia cruzada | OK |
| README.md (índice + orden + prereq + quick-start) | Presente; índice con Tipo Diátaxis/Nivel, orden de lectura, prerequisitos, quick-start de 5 pasos | OK (Obs H-05) |
| Cada doc con Tipo Diátaxis/Audiencia/Nivel/Tiempo | Cumplido en los 6 documentos de contenido | OK |
| Cada doc con Referencias cruzadas y ≥1 enlace a 05 | Cumplido; los 6 + README citan `contratos-rest_v1.0.md` y/o ADR de 05 en su sección de referencias | OK |

---

## 4. Coherencia cross-doc

| Verificación | Resultado | Evidencia |
| --- | --- | --- |
| referencia-api (10) == 35 endpoints de contratos-rest (05) | **Coincide 1:1** | Conteo por área idéntico (3/6/6/3/8/2/2/2/3 = 35) en ambos; mismos método+ruta, seguridad e idempotencia |
| Pipeline (09) ejecuta gates G1-G8 de 08 sin redefinirlos | **Coherente** | `pipeline-ci-cd` §3 + mapa gate→stage; cita DoD de `definition-of-done_v1.0.md` y gates de `estrategia-calidad_v1.0.md` §3 |
| NFR numéricos de 05 §8 con stage verificador | **Coherente** | `pipeline-ci-cd` §3.1 y README §"NFR numéricos"; valores idénticos a 05 §8 y 08 |
| Versionado usa §17 P.7 (SemVer/Conv. Commits/GitVersion/trunk-based) | **Coherente** | `estrategia-versionado` §2/§3/§4/§5 == intake §17 P.7 |
| Ambientes = servicio desplegable, no canales de paquete | **Coherente** | 09_rules §2.2 (rest-api → DEV/QA/STAGING/PROD + canary); declarado en 4 documentos |
| Deprecation/versionado del contrato coordinado con ADR-10 | **Coherente** | `estrategia-versionado` §6, `guia-publicacion-openapi` §4, `referencia-api` §6.3 alineados a ADR-10/CU-22 |
| problem+json RFC 7807 (05) vs RFC 9457 (10) | **Inconsistencia menor reconciliada** | 10 adopta 9457 por mandato de 10_rules §1.2/§4.5; nota de equivalencia explícita en referencia-api §1/§7 y glosario. Ver H-02 (P3) |
| 10 cita comandos del pipeline para reproducción local (downstream 09→10) | **Coherente** | `pipeline-ci-cd` §9 declara ser la fuente; onboarding/integración de 10 remiten a 11 para el código ejecutable |

---

## 5. Hallazgos

### H-01 — `OIDC` literal en el cuerpo de supply-chain-seguridad (D7) — **P2**

- **Archivo / sección:** `09_devops/supply-chain-seguridad_v1.0.md` §2 (tabla Firma, fila "Identidad").
- **Evidencia:** "Identidad de firma de la organización o keyless con **OIDC** del CI; la clave o identidad vive en el vault del CI".
- **Análisis:** `OIDC` figura en la lista de protocolos prohibidos del master-prompt. Sin embargo, aquí no designa el protocolo de autenticación de la API (que se mantiene abstracto como "token bearer" en los 14 archivos, fiel a ADR-03 y sin reincidir en el JWT/ROPC del P0 de Fase B), sino la **identidad keyless de firma del artefacto en el CI** (workload identity de sigstore/keyless), un concepto de cadena de suministro. Por eso se clasifica **P2 (higiene léxica)** y no P0: no es una fuga del stack/protocolo de la solución ni contradice ADR-03; es un término técnico evitable que rompe la disciplina de nombrar todo por rol abstracto.
- **Recomendación:** reemplazar "keyless con OIDC del CI" por una formulación abstracta equivalente, p. ej. "identidad keyless federada del CI" o "identidad de carga de trabajo del CI con transparency log", manteniendo el resto de la fila. No bloquea el avance.

### H-02 — Divergencia RFC 7807 (05) vs RFC 9457 (10) — **P2**

- **Archivos / secciones:** `10/referencia-api_v1.0.md` §1/§7 y `10/glosario-tecnico_v1.0.md` (term `problem-json`) adoptan **RFC 9457**; `05/contratos-rest_v1.0.md` §2/§5 y ADR-05 citan **RFC 7807**.
- **Evidencia:** referencia-api §1: "problem+json (RFC 9457, que actualiza y reemplaza a RFC 7807; el tipo de contenido `application/problem+json` es idéntico)"; nota de versión de RFC en §1.
- **Análisis:** la adopción de 9457 en 10 **está mandada por 10_rules** §1.2 (variante rest-api) y §4.5, de modo que el documento de 10 hace lo correcto según su propia regla. La nota de paridad de 10 es técnicamente correcta (9457 obsoleta 7807 sin cambiar cuerpo ni content-type) y **suficiente** como reconciliación documental para no inducir error en el consumidor. Queda, no obstante, una nomenclatura distinta entre capas de la misma solución (05 dice 7807, 10 dice 9457). Se clasifica **P2** por ser una reconciliación cross-doc pendiente de cierre formal, no un defecto que rompa trazabilidad.
- **Recomendación:** en el próximo ciclo de 05, actualizar ADR-05 y `contratos-rest_v1.0.md` para citar "RFC 9457 (obsoleta RFC 7807)", unificando la nomenclatura de la solución. Mientras tanto, la nota de 10 es aceptable. (Si el evaluador prefiere tratarla como puramente cosmética, degrada a P3; el efecto sobre el veredicto es nulo.)

### H-03 — `image-docker`/`openapi` en sección de Alcance, no solo en filename — **P3**

- **Archivos / secciones:** `09/guia-publicacion-image-docker_v1.0.md` §0 y `09/guia-publicacion-openapi_v1.0.md` §0.
- **Evidencia:** "El tipo de artefacto `image-docker` es el nombre normalizado de la tabla §2.2 de 09_rules para el artefacto desplegable del proyecto".
- **Análisis:** el master-prompt admite `image-docker`/`openapi` "solo en nombres de archivo de guía / tabla tipo-artefacto". El uso en §0 está **explícitamente anclado a la tabla §2.2** y aclara que el cuerpo describe el artefacto "en términos genéricos" (imagen de contenedor). Es un uso conforme al espíritu de la regla (referenciar el token de la tabla tipo-artefacto), pero rozando el límite literal. **P3 (estilo)**; no requiere cambio. Se documenta para trazabilidad del scan.
- **Recomendación:** ninguna obligatoria; opcionalmente, encerrar el token entre comillas de código (ya lo está) basta para señalar que es el identificador de la tabla, no la tecnología.

### H-04 — Referencia interna a §3.3 del propio doc en conceptos-fundamentales — **P3**

- **Archivo / sección:** `10/conceptos-fundamentales_v1.0.md` §2.1 ("el cierre siempre exige que no queden conflictos pendientes (§3.3)") y §2.1 ("la sincronización siempre sube antes de bajar (§3.2)").
- **Evidencia:** el documento remite a "§3.2" y "§3.3" propios, pero su §3 es una tabla única de decisiones de diseño sin subsecciones 3.2/3.3 numeradas; los temas referidos viven en §2.4 (sync) y §2.5 (conflictos).
- **Análisis:** referencia cruzada interna imprecisa; no afecta contenido ni trazabilidad upstream. **P3 (estilo)**.
- **Recomendación:** corregir los punteros internos a §2.4 (sincronización) y §2.5 (tolerancia a conflictos) en una corrección de redacción sobre la misma versión.

### H-05 — Cabecera del README de 10 sin campos Diátaxis/Nivel/Tiempo — **P3**

- **Archivo / sección:** `10/README.md` (bloque de metadatos).
- **Evidencia:** el README lleva Proyecto/Documento/Versión/Estado/Fecha/Autor/Audiencia, pero no `Tipo Diátaxis`/`Nivel`/`Tiempo estimado de lectura`.
- **Análisis:** 10_rules §4.1 define ese bloque extendido para los artefactos de cuadrante Diátaxis; §3.5 trata el README como índice navegable (no un cuadrante) y §6 solo le exige índice, orden de lectura, prerequisitos y quick-start, todo lo cual el README cumple. La ausencia de los campos Diátaxis en un índice es coherente con su naturaleza; el master-prompt §3 ("cada doc con Tipo Diátaxis/...") apunta a los documentos de contenido. **P3 (estilo / interpretación de regla)**, sin impacto en aceptación.
- **Recomendación:** opcional; si se desea uniformidad estricta, declarar `Tipo Diátaxis: Índice (no aplica)` o añadir Nivel/Tiempo de la sección. No bloquea.

---

## 6. Verificaciones explícitas solicitadas por el master-prompt

- **Las DOS guías de publicación de rest-api existen:** Sí — `guia-publicacion-image-docker_v1.0.md` y `guia-publicacion-openapi_v1.0.md`, ambas completas. No hay omisión de documento obligatorio (no hay P0 por este eje).
- **No se confunden ambientes con canales:** Confirmado — DEV/QA/STAGING/PROD se declaran como ambientes de servicio desplegable con canary en README, pipeline §1/§6, entornos §1 y versionado §6, citando el anti-patrón §4.8 "confundir publicación con despliegue". No hay canales preview/stable de paquete.
- **Slug genérico (no comercial):** Confirmado — la guía de integración usa `cliente-http`, slug genérico.
- **Cabecera/secciones presentes:** Confirmado en los 14 archivos.
- **Reincidencia del P0 de Fase B (ROPC/JWT):** **No reaparece** — cero ocurrencias de ROPC y JWT en los 14 cuerpos.

---

## 7. Veredicto

**APROBADO CON OBSERVACIONES.**

geovial-api supera la Fase F en sus dos categorías obligatorias para rest-api. El conjunto documental está completo (incluidas las dos guías de publicación), la trazabilidad cross-doc es íntegra (paridad 1:1 de 35 endpoints entre 10 y 05; gates G1-G8 de 08 ejecutados como stages sin redefinir la DoD; NFR numéricos con stage verificador; versionado conforme al intake §17 P.7), y el scan léxico D7 **no detecta ninguna fuga de stack o protocolo de la capa de aplicación** —en particular, el P0 de ROPC/JWT de Fase B no reaparece. Encoding UTF-8/LF sin BOM, kebab-case y `_v1.0` correctos.

Los hallazgos son dos P2 (la ocurrencia léxica de `OIDC` para la identidad de firma del CI, evitable; y la reconciliación de nomenclatura RFC 7807↔9457, ya cubierta por una nota suficiente y mandada por 10_rules) y tres P3 de estilo. **Ningún P0 ni P1.** Conforme a la regla de veredicto (APROBADO CON OBSERVACIONES sin P0 permite avanzar), el proyecto habilita la continuación a Fase G (11 examples) y al cierre de bucle / nivel solución de Fase H. Las observaciones son atendibles en correcciones sobre la misma versión, sin re-emisión mayor.

---

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-16 | Auditoría independiente inicial de la Fase F de geovial-api (09_devops + 10_developer_guide). Matriz D1-D8 con fila explícita de scan léxico de stack/protocolo (D7), matriz de estructura por categoría (09 §6/§2.2 y 10 §6/§2.2 rest-api), coherencia cross-doc (paridad 35 endpoints 10↔05, gates 08↔09, versionado §17 P.7, reconciliación RFC 7807/9457), cinco hallazgos (0 P0, 0 P1, 2 P2, 3 P3) y veredicto APROBADO CON OBSERVACIONES. |
