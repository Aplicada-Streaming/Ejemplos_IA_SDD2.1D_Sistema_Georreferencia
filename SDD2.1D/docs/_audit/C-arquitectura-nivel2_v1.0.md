# Auditoría independiente — Fase C (Arquitectura técnica) — Nivel 2 de GeoVial

| Campo | Valor |
| --- | --- |
| Fase | C — Arquitectura del proyecto |
| Alcance | Proyectos de nivel topológico 2 de GeoVial: `geovial-web` (`web-monolith`) y `geovial-mobile` (`mobile-app-maui`). Categoría `05_arquitectura_tecnica` de cada proyecto. La vista de solución de `_solucion/` queda fuera de alcance (se genera en Fase H) |
| Documento | C-arquitectura-nivel2_v1.0.md |
| Versión | 1.0 |
| Auditor | Arquitecto de Soluciones + QA Senior (independiente; no participó de la generación) |
| Fecha | 2026-06-16 |
| Insumos de reglas | `05_rules_arquitectura_tecnica.md` (v1.2; §2.2, §3.3 ADR individuales, §4 estructura, §6 criterios de aceptación), `master-prompt.md` §10 |
| Fuentes de verdad | `SOLUTION-INTAKE-geovial_v1.0.md` (contenido v1.5, §17 P.2/P.4/P.5/P.10/P.11 de cada proyecto), upstream `00_contexto`, `01_necesidades_negocio`, `02_especificacion_funcional` de cada proyecto; `geovial-api/05_arquitectura_tecnica` (`contratos-rest`, `modelo-datos-logico` autoritativos), `aplicada-sync/05_arquitectura_tecnica/contratos-abstractions_v1.0.md` |

---

## 1. Resumen ejecutivo

Se auditaron los 16 entregables de la Fase C de los dos proyectos de nivel 2.

- `geovial-web` (`web-monolith`): 8 documentos — documento maestro, índice de ADRs, README y 5 ADRs individuales (estilo, persistencia, autenticación, separación de capas, manejo de errores). Omite correctamente `modelo-datos-logico` (`tiene_persistencia=false`), `contratos-<area>` (no expone API), `flujo-ejecucion` (sin orquestación compleja) y `extensibilidad` (`tiene_extensibilidad=false`).
- `geovial-mobile` (`mobile-app-maui`): 9 documentos — documento maestro, índice de ADRs, README, `modelo-datos-logico`, `flujo-ejecucion` y 5 ADRs individuales (estilo, persistencia local, sincronización, gestión de permisos, autenticación). Omite correctamente `contratos-<area>` (solo consume) y `extensibilidad` (`tiene_extensibilidad=false`).

Ambos proyectos cumplen el §6 de `05_rules` para su tipo D8. El documento maestro de cada uno tiene las cuatro vistas mínimas (lógica, procesos, despliegue, datos) y las diez secciones del §4.2; el índice de ADRs refleja el estado real; se cumple el mínimo de ADRs (5/5 en web, exigido 5; 5 en mobile, exigido 4) con archivos individuales bajo `adrs/`, cada ADR con las diez secciones del §4.3 y estado declarado. El estilo está justificado contra ≥2 alternativas en el documento maestro y en el ADR de estilo; cada NFR tiene objetivo numérico y mecanismo de medición; ningún ADR está consolidado dentro de otro documento (convención crítica §3.3 respetada sin excepción).

**Énfasis D7 (neutralidad de stack): sin fugas.** El scan léxico con límites de palabra sobre los cuerpos de los 16 documentos arroja cero coincidencias de stack/protocolo prohibido (`.NET`, `ASP.NET`, `Blazor`, `MudBlazor`, `SignalR`, `MAUI`, `SQLite`, `SQL Server`, `JWT`, `ROPC`, `OAuth`, `Leaflet`, `OpenStreetMap`, `Android`, `iOS`, `EXIF`, `PostgreSQL`, `Entity Framework`, `Keychain`, `Keystore`, `Xamarin`, `.razor`, `WebSocket`). Las decisiones se expresan por patrón/mecanismo abstracto permitido (render server-side, circuito interactivo persistente, MVVM, app híbrida con vistas embebidas, almacén local, token bearer, componente de mapa, almacenamiento seguro del dispositivo). Esto es notable porque el intake §17 de ambos proyectos sí carga el stack prohibido completo (Blazor Interactive Server + MudBlazor + SignalR para web; .NET MAUI + Blazor Hybrid + SQLite + Android para móvil; OSM + Leaflet, JWT bearer, flujo ROPC): los generadores tradujeron correctamente el stack a mecanismos abstractos. El único token `mobile-app-maui` aparece como valor D8 en cabeceras/índice de mobile (uso permitido); `web-monolith` solo como valor D8 en cabeceras de web. `HTTP`, `REST` y `problem+json` son protocolos del contrato autoritativo de `geovial-api`, no del dominio fuente: se evalúan como mecanismo permitido, no como fuga.

La trazabilidad resuelve en ambas direcciones. Upstream: web ancla en NB-01/02/05/06/07 y cubre CU-01..CU-11 y RN-01..RN-05 (todos existentes en su 02); mobile ancla en NB-03/04 y cubre CU-01..CU-07 y RN-01..RN-05. Cross-doc autoritativo: las 15 referencias a CU de `geovial-api` que hace web (CU-01..CU-22, incluidas CU-12/13/14/15/16/17/19/20/22) resuelven contra el catálogo real de 22 CU de la API; mobile referencia el contrato REST (endpoints subida/bajada, revalidación, códigos `SUBIDA_NO_CONCLUIDA`, `MARCA_INVALIDA`, `RELEVAMIENTO_CERRADO`, todos presentes en `contratos-rest §5`) y las 6 operaciones del contrato de `aplicada-sync` (todas presentes). El modelo lógico local de mobile mapea exactamente las 8 entidades del conceptual local de 02 (más dos tablas de asociación que materializan las relaciones N:N, no entidades nuevas), con migración inicial `0001_inicial_almacen_local` referenciada y trazabilidad entidad por entidad al conceptual local y al dominio autoritativo de la API. Los NFR de quality attributes coinciden literalmente con §17 P.10 de cada proyecto (web: interacción p95 ≤200 ms, ≥50 circuitos, 99,5 %; mobile: captura 100 % offline, cola ≥1000, 100 cambios ≤30 s, arranque ≤3 s).

La omisión del `modelo-datos-logico` en web está **correctamente justificada y no es un defecto**: el front no tiene persistencia de dominio (`tiene_persistencia=false`, §17 P.4), el dato autoritativo y su modelo lógico viven en `geovial-api`, y la omisión queda registrada como decisión en ADR-02 (Persistencia, §2 y §10) y en el README §"Notas de alcance". Cumple el §6 (la cláusula del modelo lógico aplica "si el tipo D8 exige persistencia").

Los hallazgos son menores y cosméticos. Conteo: **P0 = 0; P1 = 0; P2 = 1; P3 = 3.** Veredicto consolidado: **APROBADO CON OBSERVACIONES.**

---

## 2. Matriz D1-D8 por documento

Convención: OK = conforme. D1 idioma rioplatense técnico; D2 UTF-8/LF; D3 kebab/filename; D4 versión `_vX.Y` (nunca `.v`); D5 estado y control de cambios; D6 trazabilidad; D7 sin stack/protocolo del dominio fuente; D8 conjunto cerrado D8. La fila de scan de stack es la columna D7.

### 2.1 Proyecto `geovial-web` (`web-monolith`)

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 (scan stack) | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| arquitectura-solucion_v1.0.md | OK | OK | OK | OK | OK | OK | OK (sin fuga) | OK |
| decisiones-arquitectura_v1.0.md | OK | OK | OK | OK | OK | OK | OK (sin fuga) | OK |
| README.md | OK | OK | OK | n/a (sin versión, correcto) | OK | OK | OK (`web-monolith` solo como D8) | OK |
| ADR-01 estilo-render-server-side-circuito-interactivo | OK | OK | OK | OK | OK | OK | OK (sin fuga) | OK |
| ADR-02 sin-persistencia-dominio-estado-efimero | OK | OK | OK | OK | OK | OK | OK (sin fuga) | OK |
| ADR-03 autenticacion-token-bearer-lado-servidor | OK | OK | OK | OK | OK | OK | OK (sin fuga) | OK |
| ADR-04 separacion-capas-presentacion-aplicacion-cliente-api | OK | OK | OK | OK | OK | OK | OK (sin fuga) | OK |
| ADR-05 manejo-errores-mapeo-problem-json-a-feedback | OK | OK | OK | OK | OK | OK | OK (sin fuga) | OK |

### 2.2 Proyecto `geovial-mobile` (`mobile-app-maui`)

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 (scan stack) | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| arquitectura-solucion_v1.0.md | OK | OK | OK | OK | OK | OK | OK (sin fuga) | OK |
| decisiones-arquitectura_v1.0.md | OK | OK | OK | OK | OK | OK | OK (sin fuga) | OK |
| modelo-datos-logico_v1.0.md | OK | OK | OK | OK | OK | OK | OK (tipos físicos abstractos, sin motor) | OK |
| flujo-ejecucion_v1.0.md | OK | OK | OK | OK | OK | OK | OK (sin fuga) | OK |
| README.md | OK | OK | OK | n/a (sin versión, correcto) | OK | OK | OK (`mobile-app-maui` solo como D8) | OK |
| ADR-01 estilo-app-hibrida-mvvm-offline-first | OK | OK | OK | OK | OK | OK | OK (sin fuga) | OK |
| ADR-02 persistencia-almacen-local-migraciones | OK | OK | OK | OK | OK | OK | OK (sin fuga) | OK |
| ADR-03 sincronizacion-motor-subir-luego-bajar | OK | OK | OK | OK | OK | OK | OK (sin fuga) | OK |
| ADR-04 gestion-permisos-degradacion | OK | OK | OK | OK | OK | OK | OK (sin fuga) | OK |
| ADR-05 autenticacion-token-seguro-relogueo-dispositivo | OK | OK | OK | OK | OK | OK | OK (ver H-04, P3) | OK |

### 2.3 Verificaciones de respaldo (ambos proyectos)

- **D3/D4 filenames:** los 10 ADRs matchean `^ADR-[0-9]{2}-[a-z0-9-]+_v[0-9]+\.[0-9]+\.md$`; los documentos versionados usan `_v1.0.md`; los README no llevan versión (correcto). Scan negativo del anti-patrón `.v<X.Y>.md` con punto en ambas carpetas. Subcarpeta `adrs/` presente en los dos proyectos. Estructura bajo `proyectos/<kebab>/05_arquitectura_tecnica/` correcta.
- **D7 — fila de scan léxico de stack/protocolo prohibido (énfasis del alcance):** scan con límites de palabra sobre los cuerpos de los 16 documentos. Cero coincidencias de: `.net`/`dotnet`/`c#`, `asp.net`, `blazor`, `mudblazor`, `signalr`, `maui` (salvo el valor D8 `mobile-app-maui` en cabeceras/índice de mobile, permitido), `sqlite`, `sql server`, `postgres`, `entity framework`, `ef core`, `jwt`, `ropc`, `oauth`, `leaflet`, `openstreetmap`, `android`, `ios`, `exif`, `keychain`, `keystore`, `xamarin`, `.razor`, `websocket`. El token `web-monolith` solo aparece como valor D8 en cabeceras de web. No hay vocabulario del dominio fuente del bootstrap (impresoras, ESC-POS, DSL, Bluetooth). **D7 conforme en ambos proyectos; sin P0 de fuga.**
- **D7 — términos permitidos verificados:** render server-side, circuito interactivo persistente, separación de capas, MVVM, app híbrida con vistas embebidas en contenedor nativo, almacén local, token bearer, componente de mapa, almacenamiento seguro del dispositivo, problem+json/RFC 7807 y token bearer. Todos son mecanismos/patrones abstractos (permitidos por el alcance). Los tipos físicos del `modelo-datos-logico` de mobile se expresan de forma abstracta (identificador, cadena, entero, decimal, fecha-hora, geográfico, binario/blob, booleano) sin nombrar el motor del almacén local (cumple la excepción explícita del alcance).
- **D6 — referencias cross-doc autoritativas (subagente de verificación):** las CU de `geovial-api` citadas por web (CU-01..CU-22) resuelven contra el catálogo real de 22 CU; los endpoints de subida/bajada/revalidación y los códigos `SUBIDA_NO_CONCLUIDA`/`MARCA_INVALIDA`/`RELEVAMIENTO_CERRADO` existen en `contratos-rest §5` de la API; las 6 operaciones del contrato de `aplicada-sync` (inicializar sesión, encolar cambio, ejecutar sincronización, habilitar disparo automático, consultar estado, reanudar) existen en `contratos-abstractions §3`. Las 12 entidades de dominio que web/mobile dicen vivir en `geovial-api` están en su `modelo-datos-logico`.
- Sin emojis; negritas limitadas a los bloques de cabecera del §4.1.

---

## 3. Matriz de estructura (§6 de 05_rules)

### 3.1 `geovial-web` (`web-monolith`) — exigido por §2.2/§6

| Criterio | Exigido | Hallado | Estado |
| --- | --- | --- | --- |
| `arquitectura-solucion` con 4 vistas + §1-§10 | Sí | 4 vistas (lógica §3, procesos §4, despliegue §5, datos §6) y §1-§11 (control de cambios extra) | OK |
| `decisiones-arquitectura` como índice (no cuerpo) | Sí | Índice con ID, título, categoría, estado, fecha; sin cuerpo de decisiones | OK |
| ADRs individuales bajo `adrs/`, mínimo 5 | 5 (estilo, persistencia, autenticación, separación de capas, manejo de errores) | 5 ADRs, una categoría cada uno, exactos al mínimo | OK |
| 10 secciones por ADR + estado | Sí | Las 10 secciones del §4.3 y estado declarado en los 5 | OK |
| `modelo-datos-logico` OMITIDO con justificación | Omitir (`tiene_persistencia=false`) | Omitido; justificado en ADR-02 §2/§10 y README "Notas de alcance" | OK (omisión correcta, no defecto) |
| `contratos-<area>` omitido (no expone API) | Omitir | Omitido; declarado en README | OK |
| `extensibilidad` omitido (`tiene_extensibilidad=false`) | Omitir | Omitido; declarado en README | OK |
| `flujo-ejecucion` omitido (sin orquestación compleja) | Omitir | Omitido; justificado en arquitectura §4 y README | OK |
| README de sección (recomendado) | Recomendado | Presente, completo | OK |
| Estilo justificado vs ≥2 alternativas | Sí | Tabla de 3 columnas + 3 alternativas descartadas (arquitectura §2; ADR-01 §4) | OK |
| Cada NFR con objetivo numérico y mecanismo | Sí | 5 NFR, todos con valor y mecanismo (§8) | OK |
| Trazabilidad NFR↔arquitectura↔ADR en tabla | Sí | §8 columna "ADR relacionada" + §10 | OK |

### 3.2 `geovial-mobile` (`mobile-app-maui`) — exigido por §2.2/§6

| Criterio | Exigido | Hallado | Estado |
| --- | --- | --- | --- |
| `arquitectura-solucion` con 4 vistas + §1-§10 | Sí | 4 vistas (lógica §3, procesos §4, despliegue §5, datos §6) y §1-§11 | OK |
| `decisiones-arquitectura` como índice | Sí | Índice con ID, título, categoría, estado, fecha (ver H-02, P3) | OK |
| ADRs individuales bajo `adrs/`, mínimo 4 | 4 (estilo, persistencia local, sincronización, gestión de permisos) | 5 ADRs (los 4 obligatorios + autenticación); mínimo superado | OK |
| 10 secciones por ADR + estado | Sí | Las 10 secciones del §4.3 y estado declarado en los 5 | OK |
| `modelo-datos-logico` PRESENTE (almacén local) | Sí (`tiene_persistencia=true`) | Presente; 8 entidades + 2 asociaciones; tipos físicos abstractos, índices, restricciones | OK |
| Migración inicial referenciada | Sí | `0001_inicial_almacen_local` (§4) | OK |
| Trazabilidad al conceptual local de 02 | Sí | §6 mapea cada tabla a su entidad conceptual local y a la entidad autoritativa de la API | OK |
| `flujo-ejecucion` presente (captura + sync) | Recomendado (sync engine) | Presente; pipeline arranque/captura/sincronización/reanudación | OK |
| `contratos-<area>` omitido (solo consume) | Omitir | Omitido; declarado en README | OK |
| `extensibilidad` omitido (`tiene_extensibilidad=false`) | Omitir | Omitido; declarado en README | OK |
| README de sección (recomendado) | Recomendado | Presente, completo, con enlaces | OK |
| Estilo justificado vs ≥2 alternativas | Sí | Tabla de 3 columnas + 4 alternativas descartadas (arquitectura §2; ADR-01 §4) | OK |
| Cada NFR con objetivo numérico y mecanismo | Sí | 5 NFR, todos con valor y mecanismo (§8) | OK |
| Trazabilidad NFR↔arquitectura↔ADR | Sí | §8 columna "ADR relacionada" + §10 | OK |
| Convención §3.3 (ADR individuales, jamás consolidados) | Sí | Cumplida en ambos proyectos | OK |

---

## 4. Coherencia cross-doc

### 4.1 ADRs ↔ CU/RN/NFR reales

- **web:** los 5 ADRs referencian CU del front (CU-01..CU-11) y RN de presentación (RN-01..RN-05) existentes en el 02 de web (11 CU, 5 RN confirmados). Ningún ADR huérfano de motivación. ADR-01 (NB-01/02/05, CU-01..11), ADR-02 (CU-01..11, RN-01..05, §17 P.4), ADR-03 (NB-01, CU-01/02/09, RN-01/02/03, §17 P.5), ADR-04 (CU-01..11, §17 P.2/P.6), ADR-05 (CU-01/07/08/10/11, RN-01/03/04/05, §17 P.3). Las referencias a `geovial-api` (ADR-03 de la API, contratos-rest §2/§5) resuelven.
- **mobile:** los 5 ADRs referencian CU (CU-01..CU-07) y RN (RN-01..RN-05) existentes en el 02 de mobile. ADR-01 (NB-03/04, CU-01..07, RN-05), ADR-02 (CU-02..07, RN-05/02, §17 P.4), ADR-03 (NB-04, CU-06/02, RN-02/03/05, §17 P.3, contrato de aplicada-sync + endpoints REST), ADR-04 (CU-03/04/07/01, RN-01/05), ADR-05 (NB-01, CU-01/06, RN-04, F-08, §17 P.5). Ningún ADR huérfano.
- **Estados de ADR:** web declara ADR-01/02/03 `Aceptado` (pre-tomados en intake) y ADR-04/05 `Propuesto` (defaults del arquitecto sobre ejes ratificables §17 P.2/P.3); coherente y declarado en el índice §3 y en cada ADR §3. mobile declara los 5 `Aceptado`; ADR-04 derivado de CU/RN, el resto pre-tomados; coherente. No hay ADR sin estado.

### 4.2 NFR de quality attributes ↔ §17 P.10 v1.5

| NFR web (arquitectura §8 y README) | §17 P.10 geovial-web | Coincide |
| --- | --- | --- |
| Interacción p95 ≤ 200 ms sobre el circuito | latencia de interacción p95 ≤ 200 ms sobre el circuito | Sí |
| ≥ 50 circuitos interactivos concurrentes | ≥ 50 circuitos interactivos concurrentes | Sí |
| Disponibilidad mensual ≥ 99,5 % | disponibilidad 99,5 % mensual | Sí |
| `tiene_observabilidad_critica=false` (sin SLO 99,9 % ni p99) | idem | Sí |

| NFR mobile (arquitectura §8 y README) | §17 P.10 geovial-mobile | Coincide |
| --- | --- | --- |
| Captura 100 % offline (observación con foto) | captura de observación con foto 100 % sin conexión | Sí |
| Cola local ≥ 1000 cambios | cola tolera ≥ 1000 cambios pendientes | Sí |
| 100 cambios ≤ 30 s en red móvil típica | ciclo de 100 cambios ≤ 30 s en red móvil típica | Sí |
| Reanudación sin pérdida tras corte | reanuda sin pérdida tras un corte | Sí |
| Arranque en frío ≤ 3 s | arranque en frío ≤ 3 s en el dispositivo de referencia | Sí |

### 4.3 Modelo lógico local de mobile ↔ 8 entidades del conceptual local

Las 8 tablas (`relevamiento_local`, `marcador_local`, `observacion_local`, `foto_local`, `comentario_local`, `etiqueta_local`, `cambio_encolado`, `marca_sincronizacion_local`) corresponden 1:1 a las 8 entidades del conceptual local de 02 (RelevamientoLocal, MarcadorLocal, ObservacionLocal, FotoLocal, ComentarioLocal, EtiquetaLocal, CambioEncolado, MarcaSincronizacionLocal). Las dos tablas de asociación (`etiqueta_foto_local`, `etiqueta_marcador_local`) materializan las relaciones N:N declaradas en el conceptual §3/§4 — son tablas técnicas de asociación, no entidades nuevas; no rompen el mapeo. La trazabilidad §6 liga cada tabla a su entidad conceptual de origen y a la entidad autoritativa de la API, con las invariantes RC-01/RC-02/RC-06 declaradas como replicadas (gobernadas por el backend). Migración `0001_inicial_almacen_local` referenciada (§4). Conforme.

### 4.4 Referencias inter-proyecto (mobile → aplicada-sync y geovial-api; web → geovial-api)

- mobile referencia el contrato de `aplicada-sync` (`contratos-abstractions_v1.0.md`) y el REST de `geovial-api` (`contratos-rest_v1.0.md`) con paths concretos en arquitectura §5, flujo §2, ADR-03 §6/§9 y ADR-05 §9. Las 6 operaciones del motor citadas existen en el contrato de aplicada-sync; los endpoints de subida/bajada/revalidación y los códigos de error citados existen en contratos-rest de la API. Aristas del manifiesto (`geovial-mobile → geovial-api`, `geovial-mobile → aplicada-sync`) declaradas y correctas.
- web referencia que el dominio es de `geovial-api` (arquitectura §1/§6, ADR-02, README); el modelo lógico autoritativo y el contrato REST de la API se citan, no se reescriben. Arista `geovial-web → geovial-api` declarada. Las 15 referencias a CU de la API resuelven.

---

## 5. Hallazgos

| ID | Nivel | Proyecto | Archivo / Sección | Evidencia | Recomendación |
| --- | --- | --- | --- | --- | --- |
| H-01 | P2 | geovial-mobile | `modelo-datos-logico_v1.0.md` §0 y cabecera; comparar con §4.4 (omisión de `Categoría`) | El documento maestro `arquitectura-solucion`, `modelo-datos-logico` y `flujo-ejecucion` deben usar la cabecera del §4.1 "omitiendo `Categoría`" (§4.1, último párrafo). Los tres documentos no-ADR de mobile y los de web cumplen (no llevan `Categoría`); sin embargo `modelo-datos-logico` antepone una sección `## 0. Propósito y alcance` adicional a las 7 secciones obligatorias del §4.4, que arrancan en `## 1. Tablas o colecciones`. El §4.4 lista exactamente 7 secciones (1 a 7); la `§0` extra es contenido válido pero no canónico. Es una desviación de forma sobre un documento obligatorio (de ahí P2, no P3), sin romper trazabilidad ni omitir contenido: las 7 secciones obligatorias están todas presentes y completas. | Renumerar fusionando `§0` dentro de `§1` o como preámbulo sin número, o documentar la `§0` como extensión aceptada del template. No bloquea: las 7 secciones del §4.4 están íntegras. |
| H-02 | P3 | geovial-mobile | `decisiones-arquitectura_v1.0.md` §2 (índice de ADRs) | El índice de mobile usa el slug como "Título" (p. ej. `estilo-app-hibrida-mvvm-offline-first`) y no enlaza los ADRs, mientras que el README sí los enlaza y usa títulos legibles. El índice de web sí enlaza y usa títulos legibles. La tabla tipo del §4.6 muestra `<kebab>` en la columna título, de modo que es admisible, pero es inconsistente con el README del propio proyecto y con la práctica de web. | Unificar: usar título legible y enlace markdown al archivo en el índice de mobile, como en web y en el README de mobile. Cosmético. |
| H-03 | P3 | geovial-mobile | `decisiones-arquitectura_v1.0.md` §2; ADR-04 y ADR-05 cabecera `Categoría` | ADR-04 (gestión de permisos) y ADR-05 (autenticación) declaran ambos `Categoría: Seguridad`. Es defendible (permisos de SO y custodia de token son seguridad), pero deja dos ADRs en la misma categoría mientras el conjunto cerrado del §4.1 ofrece `Comunicación`/`Despliegue` no usados; ADR-04 podría encuadrarse mejor (p. ej. categoría propia de plataforma/SO). No hay categoría inválida: ambas pertenecen al conjunto cerrado. | Opcional: reconsiderar la categoría de ADR-04. Sin impacto en trazabilidad. |
| H-04 | P3 | geovial-mobile | `ADR-05-...` §6.1 y §8; `flujo-ejecucion` §8 | El identificador `DISPOSITIVO_SIN_SEGURIDAD` se usa como nombre de condición/criterio de prueba del cliente. La verificación contra el autoritativo `geovial-api/contratos-rest` confirma que NO es un código de error del contrato REST de la API (la API delega explícitamente la seguridad del dispositivo al cliente móvil, `contratos-rest §3.1` revalidación). El ADR no lo presenta como código de la API (lo usa como salvaguarda/condición interna), por lo que la trazabilidad no se rompe; pero el formato en mayúsculas estilo-código-de-error puede confundirse con un código del contrato (como sí lo son `MARCA_INVALIDA`/`RELEVAMIENTO_CERRADO`, que sí existen en la API). | Aclarar que `DISPOSITIVO_SIN_SEGURIDAD` es una condición interna del cliente (no un código `problem+json` de `geovial-api`), o renombrarlo a una forma que no se confunda con los códigos del contrato REST. Cosmético; no afecta el veredicto. |

Notas de no-hallazgo relevantes para el alcance:

- **Omisión del modelo lógico en web — verificada como correcta, NO es defecto.** `tiene_persistencia=false` (§17 P.4); la omisión está registrada en ADR-02 (§1 contexto, §2 decisión, §10 control de cambios) y en el README "Notas de alcance". El §6 condiciona el modelo lógico a "si el tipo D8 exige persistencia". No se eleva hallazgo.
- **`tiene_extensibilidad=false`** no figura como flag explícito en el §17 del intake, pero es consistente con la naturaleza de ambos proyectos (ni `library` ni `cli-tool`; sin puntos de extensión) y con el §2.2 (extensibilidad obligatoria solo para tipos con plugins). La omisión de `extensibilidad` es correcta. No es hallazgo.
- **Referencia al "almacenamiento de la configuración" en web** (ADR-02 §1, arquitectura §6): web la nombra como parte del dominio que "vive en geovial-api". En la API es un DTO/endpoint (CU-17, `GET/PUT /v1/configuracion/almacenamiento`) cuyo binario/credenciales se delegan a `geovial-storage`, no una tabla del modelo lógico. La afirmación de web ("vive en geovial-api") es correcta a nivel de contrato/responsabilidad. No es hallazgo.

---

## 6. Veredicto

### 6.1 `geovial-web` (`web-monolith`)

**APROBADO.** Cumple el §6 de `05_rules` para `web-monolith` sin observaciones materiales: 5 ADRs individuales (mínimo exacto) con las 10 secciones y estado; documento maestro con 4 vistas y §1-§10; índice de ADRs correcto; omisión del modelo lógico, contratos, flujo y extensibilidad justificada y registrada; NFR numéricos trazados a §17 P.10 v1.5; estilo justificado contra 3 alternativas. D7 sin fuga de stack en el cuerpo. P0 = 0, P1 = 0, P2 = 0, P3 = 0 (los hallazgos son todos de mobile). Habilitado para avanzar.

### 6.2 `geovial-mobile` (`mobile-app-maui`)

**APROBADO CON OBSERVACIONES.** Cumple el §6 para `mobile-app-maui`: 5 ADRs (supera el mínimo de 4) con 10 secciones y estado; documento maestro con 4 vistas y §1-§10; `modelo-datos-logico` presente con las 8 entidades del conceptual local, migración inicial y trazabilidad; `flujo-ejecucion` de captura + sync presente; NFR numéricos trazados a §17 P.10 v1.5; estilo justificado contra 4 alternativas. D7 sin fuga de stack en el cuerpo. Observaciones sin P0: H-01 (P2, sección `§0` extra en el modelo lógico, forma sobre documento obligatorio sin omisión de contenido), H-02/H-03/H-04 (P3 cosméticos). Habilitado para avanzar; se recomienda atender H-01 en una próxima iteración del modelo lógico.

### 6.3 Consolidado

**APROBADO CON OBSERVACIONES.** Ningún P0 en ninguno de los dos proyectos: sin fuga de stack/protocolo en cuerpos (énfasis D7), sin documento obligatorio omitido, sin ADR consolidado, sin ADR sin estado, sin falta de cabecera/secciones, sin omisión injustificada, trazabilidad upstream y cross-doc autoritativa intacta. Los hallazgos son 1×P2 y 3×P3, todos en `geovial-mobile` y todos de forma/estilo. La Fase C de nivel 2 queda habilitada para avanzar a las categorías downstream (06, 07, 08, 09).

**Conteo consolidado: P0 = 0 · P1 = 0 · P2 = 1 · P3 = 3.**

---

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-16 | Auditoría independiente inicial de la Fase C de nivel 2 (geovial-web web-monolith y geovial-mobile mobile-app-maui): matriz D1-D8 con fila de scan de stack, matriz de estructura §6, coherencia cross-doc (ADR↔CU/RN/NFR, NFR↔§17 P.10 v1.5, modelo lógico local↔8 entidades del conceptual, referencias inter-proyecto a geovial-api y aplicada-sync), 4 hallazgos (1 P2, 3 P3) y veredicto por proyecto y consolidado. |
