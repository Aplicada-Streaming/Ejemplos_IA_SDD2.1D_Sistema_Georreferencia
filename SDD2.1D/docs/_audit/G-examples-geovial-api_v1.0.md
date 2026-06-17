# Auditoría Fase G — Examples (11) — geovial-api

**Documento:** G-examples-geovial-api_v1.0.md
**Fase auditada:** G (categoría `11_examples`)
**Alcance:** proyecto principal `geovial-api` (tipo D8 `rest-api`, nivel 1)
**Auditor:** Arquitecto de Soluciones + QA Senior (independiente, no participó de la generación)
**Fecha:** 2026-06-16
**Estado:** Vigente
**Reglas aplicadas:** `SDD2.1D/devs/rules/11_rules_examples.md` (§6, variante `rest-api`); master-prompt §10
**Nota de alcance:** en esta fase de documentación NO se materializa código en `/samples/`; se auditan solo los markdown explicativos y el README de la categoría.
**Veredicto consolidado:** APROBADO CON OBSERVACIONES (sin P0)

---

## 1. Resumen ejecutivo

Se auditaron 4 entregables de `geovial-api`: `README.md` más tres samples (`ejemplo-01-cliente-http-basico_v1.0.md`, `ejemplo-02-postman-collection_v1.0.md`, `ejemplo-03-sdk-tipado-generado_v1.0.md`). El proyecto es `rest-api`, tipo para el que la categoría 11 es obligatoria con piso de 3 samples (cliente HTTP básico + colección de pruebas + SDK tipado). Se cumple el piso exacto y la progresión de niveles (Básico → Intermedio → Avanzado).

Ningún entregable presenta hallazgos P0. No falta documento obligatorio; los tres slugs (`cliente-http-basico`, `postman-collection`, `sdk-tipado-generado`) son valores admitidos por §3.1 y nombran por capacidad, nunca por dominio; no hay stack ni protocolo de aplicación en prosa (sin .NET/ASP.NET/JWT/ROPC/OAuth; la auth se describe como "enviar credenciales / token bearer"); no hay vocabulario del dominio fuente del bootstrap (sin multa/factura/recibo/MultaApp); las cabeceras traen Nivel y Ubicación del código; las nueve secciones obligatorias están completas y en orden; hay 3 samples; y la trazabilidad upstream (02/05) resuelve contra artefactos vigentes. La estructura `/samples/geovial-api/0X-.../` coincide con la matriz §2.3 para `rest-api` y con la materialización declarada en intake §16.1 (línea 272: "cliente HTTP de referencia, colección de pruebas y SDK tipado").

La coherencia cross-doc es sólida: los 22 CU referenciados existen en `02/casos-de-uso/`; los 6 ADR citados (03/04/05/07/08/10) existen en `05/adrs/`; los endpoints y los códigos problem+json de los samples coinciden con `contratos-rest_v1.0.md`; el conteo de "35 operaciones" es exacto (suma verificada §3.1–§3.9 del contrato); y el NFR de sample 03 (lote >= 1000 cambios) coincide literal con intake §17.P.10.

Los hallazgos abiertos son de severidad media y baja: la narrativa de cobertura del README (§2, línea 19) atribuye a los samples CU que no aparecen en ninguna tabla `§8` de trazabilidad ni en la tabla maestra (CU-01, CU-06, CU-09, CU-14), sobredeclarando cobertura por área (P2); la cabecera declara Estado "Propuesto" mientras el upstream contrato 05 también está "Propuesto" pero el README de la categoría no lleva bloque de cabecera versionado (P3); y un sample concentra varios CU por documento sin que la regla lo prohíba pero conviene justificar la combinación (P3, observación).

**Conteo por nivel: P0: 0 — P1: 0 — P2: 1 — P3: 2.**

---

## 2. Matriz D1-D8 (idioma, codificación, nomenclatura, vocabulario)

| Criterio | README | ejemplo-01 | ejemplo-02 | ejemplo-03 |
| --- | --- | --- | --- | --- |
| D1 Idioma rioplatense técnico, sin emojis ni negrita decorativa | Cumple | Cumple | Cumple | Cumple |
| D2 UTF-8 sin BOM / LF | Cumple (verificado) | Cumple (verificado) | Cumple (verificado) | Cumple (verificado) |
| D3 kebab-case en filenames | Cumple | Cumple | Cumple | Cumple |
| D4 Sufijo `_vX.Y` (nunca `.v`) | N/A (índice sin sufijo, §3.1) | Cumple | Cumple | Cumple |
| D5 Sin vocabulario del dominio fuente del bootstrap (multa/factura/recibo/MultaApp) | Cumple (búsqueda negativa) | Cumple | Cumple | Cumple |
| D6 Slug por capacidad, no por dominio del proyecto | Cumple | Cumple | Cumple | Cumple |
| D7 Cuerpo stack-abstract; sin .NET/ASP.NET/JWT/ROPC/OAuth en prosa; auth como "token bearer / enviar credenciales" | Cumple | Cumple | Cumple | Cumple |
| D8 Sin slug comercial hardcodeado en el nombre del markdown | Cumple | Cumple | Cumple | Cumple |

Notas D7. La búsqueda negativa sobre los 4 archivos no devolvió `.NET`, `ASP.NET`, `JWT`, `ROPC`, `OAuth`, `C#`, `NuGet`, `MAUI`, `Newtonsoft`, `Swagger`, `NSwag`, `Refit`, `Newman`, `curl`, `dotnet`, ni `.cs/.csproj/.bat/.ps1`. La autenticación se describe siempre como "enviar credenciales y recibir un token bearer", forma admitida por el alcance. El término `bearer` se usa como vocabulario REST genérico (token bearer / cabecera de autorización), no como protocolo de aplicación. El slug `postman-collection` aparece solo en nombre de archivo/carpeta y como valor admitido por §3.1; la prosa usa lenguaje genérico ("colección de pruebas", "ejecutor de colecciones", "runner de línea de comandos"), sin nombrar la herramienta comercial. Los nombres de script `.sh` y el patrón `export VAR=` aparecen en árboles de "Estructura del código" y en pasos de "Cómo correrlo"; replican el precedente de la propia regla §7.2 (que usa `./confirmar-pago.sh` y `export API_KEY=`) y refieren código que vive en `/samples`, no stack de aplicación; no constituyen leak.

Veredicto del bloque D1-D8: sin P0.

---

## 3. Matriz de estructura — cabecera + 9 secciones obligatorias (§4.2)

| Elemento | ejemplo-01 | ejemplo-02 | ejemplo-03 |
| --- | --- | --- | --- |
| Cabecera con Nivel | Sí (Básico) | Sí (Intermedio) | Sí (Avanzado) |
| Cabecera con Ubicación del código | Sí `/samples/geovial-api/01-cliente-http-basico/` | Sí `/samples/geovial-api/02-postman-collection/` | Sí `/samples/geovial-api/03-sdk-tipado-generado/` |
| §1 Objetivo del sample | Cumple | Cumple | Cumple |
| §2 Nivel (declarado + justificación vs anterior) | Cumple | Cumple (asume 01) | Cumple (asume 01 y 02) |
| §3 Prerequisites (con versión mínima cuando aplica) | Cumple | Cumple | Cumple |
| §4 Cómo correrlo (≤5 pasos) | Cumple (5 pasos) | Cumple (5 pasos) | Cumple (5 pasos) |
| §5 Estructura del código (árbol) | Cumple | Cumple | Cumple |
| §6 Qué esperar (output exacto + payloads + problem+json) | Cumple | Cumple | Cumple |
| §7 Variaciones sugeridas (2-4 filas) | Cumple (4) | Cumple (4) | Cumple (4) |
| §8 Trazabilidad (≥1 CU/ADR/NFR) | Cumple (5 CU + 3 ADR) | Cumple (9 CU + 3 ADR) | Cumple (8 CU + 3 ADR + 1 NFR) |
| §9 Control de cambios | Cumple | Cumple | Cumple |

Observaciones de §6 (output esperado exacto, incluido problem+json). Los tres samples documentan payloads concretos y al menos un cuerpo problem+json con código estable, estado y recurso:
- ejemplo-01: `200` login, `200` listado paginado, `201` alta agente, y problem+json `CREDENCIALES_INVALIDAS`/`401`.
- ejemplo-02: `200` login, `201` relevamiento, `201` asignación, `200` listado paginado, problem+json `TRAMO_INCOMPLETO`/`400`, y resumen de corrida (7 requests / 14 aserciones).
- ejemplo-03: salida de generación (35 operaciones), `200` subida, `200` bajada, resumen de recorrido, y problem+json `SUBIDA_NO_CONCLUIDA`/`409`.

README de la categoría (§4.3 / §4.4): cumple. Propósito de la carpeta (§1), tabla maestra con las cinco columnas exigidas — Sample, Nivel, Tiempo de setup, CU ilustrados, Ubicación — (§2), convenciones (§3), cómo agregar un sample con referencia a §6 de la regla (§4), vínculo con 10 y 05 (§5) y réplica resumida de la matriz §2.3 para rest-api (§6). Las tres filas de la tabla maestra declaran tiempo de setup estimado (< 5 min, 10-15 min, 20-30 min).

Veredicto del bloque estructura: sin P0.

---

## 4. Coherencia cross-doc (02 / 05 / 10 / intake)

| Verificación | Resultado |
| --- | --- |
| Los CU citados en las tablas §8 existen en `02/casos-de-uso/` | Cumple. Los 22 CU existen (CU-01..22). Todos los CU citados (02,03,04,05,07,08,10,11,12,13,18,19,20,21,22) resuelven a archivo vigente. |
| Los ADR citados existen en `05/adrs/` | Cumple. ADR-03, ADR-04, ADR-05, ADR-07, ADR-08, ADR-10 existen y son vigentes. |
| Endpoints de los samples coinciden con `contratos-rest_v1.0.md` | Cumple. `POST /v1/sesiones`, `GET /v1/relevamientos`, `POST /v1/agentes`, `POST /v1/relevamientos`, `POST .../asignaciones`, `POST .../marcadores`, `POST /v1/marcadores/{id}/observaciones`, `POST /v1/observaciones/{id}/fotos`, `POST .../sincronizacion/subida` y `.../bajada`: todos presentes en el contrato §3. |
| Códigos problem+json coinciden con la taxonomía del contrato §5 | Cumple. `CREDENCIALES_INVALIDAS` (401), `TRAMO_INCOMPLETO` (400), `FUERA_DE_ALCANCE` (403), `VERSION_NO_SOPORTADA` (400), `FILTRO_NO_SOPORTADO` (400), `ROL_NO_AUTORIZADO` (403), `SUBIDA_NO_CONCLUIDA` (409), `MARCA_INVALIDA` (400): todos en el catálogo §5. |
| Conteo "35 operaciones" del SDK y del README | Cumple. Suma verificada del contrato §3.1–§3.9 = 3+6+6+3+8+2+2+2+3 = 35. |
| Idempotencia de las escrituras ilustradas | Cumple. Alta agente, alta relevamiento, asignación y subida figuran como idempotentes (clave / id de origen) en el contrato §3.2, §3.3, §3.4, §3.6. |
| NFR de sample 03 (§8) vs intake | Cumple. "lote de al menos 1000 cambios por relevamiento" coincide literal con intake §17.P.10 (línea 332). |
| Guías de 10 citadas en README §5 existen | Cumple. `guia-onboarding-developer`, `guia-integracion-cliente-http`, `referencia-api`, `conceptos-fundamentales`, `troubleshooting` existen en `10_developer_guide/`. |
| Estructura `/samples/geovial-api/0X-...` vs §2.3 y vs intake §16.1 | Cumple. Carpetas `01-cliente-http-basico`, `02-postman-collection`, `03-sdk-tipado-generado` coinciden con la matriz rest-api de §2.3 y con la materialización declarada en intake §16.1 (línea 272). Prefijo de proyecto `/samples/geovial-api/` correcto para solución multi-proyecto. |

Inconsistencia detectada (no rompe trazabilidad). La narrativa del README §2 (línea 19) enumera por área "usuarios y agentes (CU-01, CU-02), relevamientos y ciclo (CU-04, CU-06, CU-12, CU-14) ... marcadores y observaciones (CU-07, CU-08, CU-09)". Los CU-01, CU-06, CU-09 y CU-14 no aparecen en la columna "CU ilustrados" de la tabla maestra ni en ninguna tabla §8 de los tres samples. La trazabilidad autoritativa (tabla maestra + §8) no se rompe — todo CU citado en §8 existe — pero la prosa sobredeclara cobertura por área. Ver hallazgo H-1 (P2).

---

## 5. Cobertura de CU (MVP)

CU efectivamente ilustrados según tablas §8 (unión de los tres samples):
CU-02, CU-03, CU-04, CU-05, CU-07, CU-08, CU-10, CU-11, CU-12, CU-13, CU-18, CU-19, CU-20, CU-21, CU-22 (15 de 22).

CU no ilustrados por ningún sample en §8:
- CU-01 (administrar jerarquía de usuarios), CU-06 (transicionar estado), CU-09 (cargar fotos manualmente), CU-14 (cerrar relevamiento): no aparecen en §8 ni en la tabla maestra, pese a la mención por área del README §2.
- CU-15, CU-16 (portabilidad), CU-17 (configuración de almacenamiento): el README §2 los declara explícitamente como diferidos a "variaciones sugeridas / samples adicionales".

Evaluación de criticidad. El criterio de cobertura exige que ningún CU crítico del MVP quede sin ilustrar al menos tangencialmente y declarado. CU-15/16/17 quedan declarados como diferidos (cumple). CU-01, CU-06, CU-09 y CU-14 son CU del núcleo (ciclo de relevamiento, jerarquía, carga manual, cierre): no están en §8 pero el README los menciona como cubiertos por área, lo que es a la vez una declaración (atenúa el gap) y una inconsistencia (la mención no se respalda en §8). Como el ciclo de sincronización avanzado (sample 03) ejercita el flujo en torno al relevamiento y el cierre se apoya en la resolución de conflictos ilustrada (CU-13), el grueso del MVP queda cubierto. No se eleva a P0/P1 porque la regla §5.1 admite cobertura "al menos tangencial y declarada" y los samples son piso ampliable; se registra como P2 por la incoherencia entre prosa y tablas. Recomendación: o se agrega una fila §8 o variación que respalde CU-01/06/09/14, o se ajusta la prosa del README §2 para no atribuirlos como ilustrados.

---

## 6. Hallazgos

### H-1 (P2) — README sobredeclara cobertura de CU en la narrativa de §2
- **Archivo:** `11_examples/README.md`
- **Sección:** §2 Tabla maestra de samples (prosa posterior a la tabla, línea 19)
- **Evidencia:** La prosa atribuye a los samples "usuarios y agentes (CU-01, CU-02)", "relevamientos y ciclo (CU-04, CU-06, CU-12, CU-14)" y "marcadores y observaciones (CU-07, CU-08, CU-09)", pero CU-01, CU-06, CU-09 y CU-14 no figuran en la columna "CU ilustrados" de la tabla maestra ni en ninguna tabla §8 de los tres samples.
- **Recomendación:** Alinear la prosa con la trazabilidad autoritativa: o bien agregar una fila §8 / variación sugerida que ejercite CU-01, CU-06, CU-09 y CU-14, o bien reformular la narrativa para listar solo los CU efectivamente ilustrados y marcar el resto como diferidos (como ya se hace con CU-15/16/17). No bloqueante: no rompe trazabilidad porque todo CU citado en §8 existe en 02.

### H-2 (P3) — Inconsistencia/ausencia de bloque de cabecera versionado en el README de la categoría
- **Archivo:** `11_examples/README.md`
- **Sección:** Encabezado del documento
- **Evidencia:** Los tres samples llevan bloque de metadatos (Proyecto, Documento, Versión, Estado "Propuesto", Fecha, Autor, Nivel, Ubicación). El README de la categoría no lleva bloque de metadatos. La regla §3.1 admite el README como índice sin sufijo, pero no exige cabecera; los samples sí la traen y declaran Estado "Propuesto", coherente con el contrato 05 también "Propuesto".
- **Recomendación:** Opcional. Para consistencia con el resto de la cadena, considerar un encabezado mínimo (Documento, Versión de la sección, Fecha) en el README. Estilo, no bloqueante.

### H-3 (P3, observación) — Concentración de múltiples CU por sample sin nota de justificación
- **Archivo:** `ejemplo-02-postman-collection_v1.0.md` (9 CU en §8), `ejemplo-03-sdk-tipado-generado_v1.0.md` (8 CU en §8)
- **Sección:** §8 Trazabilidad
- **Evidencia:** La regla §5.1 admite que un sample cubra más de un CU pero pide que la combinación esté justificada. La justificación está implícita (corrida reproducible que encadena flujos; recorrido e2e de integración) y razonable, pero no hay una nota explícita que diga por qué no se separan.
- **Recomendación:** Opcional. Una frase en §1 o §2 que explicite que la combinación responde a un recorrido encadenado intencional cerraría la pregunta guía §5.1. Estilo, no bloqueante.

---

## 7. Veredicto

**APROBADO CON OBSERVACIONES.**

Se cumplen todos los criterios bloqueantes: README con tabla maestra de cinco columnas; piso de 3 samples para `rest-api` (cliente HTTP básico + colección de pruebas + SDK tipado); cabecera con Nivel y Ubicación del código y nueve secciones obligatorias por markdown; nivel declarado en §2; ≤5 pasos en §4; trazabilidad §8 con ≥1 CU/ADR/NFR resolviendo a artefactos vigentes; output esperado exacto en §6 con payloads y problem+json; prerequisites en §3; nomenclatura por capacidad (nunca por dominio); D1-D8 sin violaciones, con D7 limpio (sin stack ni protocolo de aplicación en prosa); estructura `/samples/geovial-api/0X-...` coincidente con §2.3 e intake §16.1; y coherencia cross-doc verificada contra 02, 05, 10 e intake. No hay hallazgos P0 ni P1.

Las observaciones P2/P3 son de coherencia narrativa y estilo, no bloquean el avance. Se recomienda atender H-1 (alinear la cobertura declarada del README con las tablas §8) en la próxima emisión.

**Conteo por nivel: P0: 0 — P1: 0 — P2: 1 — P3: 2.**

---

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-16 | Auditoría independiente inicial de la Fase G (categoría 11_examples) del proyecto geovial-api (rest-api, nivel 1): matriz D1-D8, matriz de estructura de las nueve secciones por markdown, coherencia cross-doc contra 02/05/10/intake, cobertura de CU y hallazgos. Veredicto APROBADO CON OBSERVACIONES (0 P0, 0 P1, 1 P2, 2 P3). |
