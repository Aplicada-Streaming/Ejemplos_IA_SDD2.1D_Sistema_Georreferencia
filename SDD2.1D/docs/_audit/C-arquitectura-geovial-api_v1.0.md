# Auditoría Fase C — Arquitectura técnica — geovial-api (nivel 1)

**Documento:** C-arquitectura-geovial-api_v1.0.md
**Fase auditada:** C — Arquitectura técnica (categoría `05_arquitectura_tecnica`)
**Proyecto:** geovial-api (`rest-api`, proyecto principal de la solución GeoVial)
**Auditor:** Independiente — Arquitecto de Soluciones + QA Senior (sin participación en la generación)
**Fecha:** 2026-06-15
**Veredicto:** APROBADO CON OBSERVACIONES (sin P0)

---

## 1. Alcance y método

Se auditó la categoría `05_arquitectura_tecnica` de nivel proyecto de `geovial-api`. La vista de solución de `_solucion/` queda fuera (es Fase H) y se confirmó que los entregables solo la referencian, sin reescribirla.

Entregables leídos íntegramente:

- `arquitectura-solucion_v1.0.md` (documento maestro)
- `decisiones-arquitectura_v1.0.md` (índice de ADRs)
- 10 ADRs individuales: `adrs/ADR-01` a `adrs/ADR-10`
- `modelo-datos-logico_v1.0.md`
- `contratos-rest_v1.0.md`
- `flujo-ejecucion_v1.0.md`
- `README.md`

Insumos de contraste leídos: 02 (22 CU, 7 RN, modelo conceptual de 12 entidades, 6 RC), 01 (NB-01 a NB-07), 03 (`dx-error-messages_v1.0.md` referenciado por ADR-05), `geovial-storage/05/contratos-abstractions_v1.0.md`, reglas `05_rules_arquitectura_tecnica.md` (§2.2, §3.3, §4, §6) e intake `SOLUTION-INTAKE-geovial_v1.0.md` (v1.4, §17 P.4/P.10).

Método: lectura completa de cada entregable, scan léxico explícito de neutralidad de stack (D7) sobre toda la carpeta, verificación de existencia de identificadores upstream (CU/RN/RC/NB/entidades) y reconciliación cruzada con el intake.

---

## 2. Matriz de evaluación D1–D8

| Disciplina | Evaluación | Resultado |
| --- | --- | --- |
| D1 — Trazabilidad upstream/downstream | Cada ADR referencia NB/CU/RN/RC/NFR; cada componente del maestro lista CU; el modelo lógico mapea entidad por entidad; el contrato cubre los 22 CU; downstream a 06/08/09 declarado. | Cumple |
| D2 — Cobertura de alcance del tipo D8 | `rest-api`: 5 ADRs mínimos presentes (estilo, persistencia, autenticación, paginación, errores) + 5 adicionales justificados; modelo lógico presente; OpenAPI descriptivo; flujo de ejecución por orquestación de sync; extensibilidad omitida con justificación. | Cumple |
| D3 — Profundidad y rigor | NFR con objetivo numérico y mecanismo de medición; ADRs con ≥2 alternativas con pros/contras; trade-offs explícitos; riesgos con impacto/probabilidad/mitigación. | Cumple |
| D4 — Consistencia interna | Componentes, ADRs, modelo, contrato y flujo coherentes entre sí; índice de ADRs y README alineados con los archivos reales. | Cumple (ver H-03, reconciliación menor) |
| D5 — Claridad y estructura | Cabeceras completas; secciones obligatorias presentes; idioma rioplatense técnico; sin emojis. | Cumple |
| D6 — Cadena de trazabilidad SDD | Ancla 02→05→06/08/09; el modelo lógico referencia el conceptual; los contratos referencian CU; consumo de `geovial-storage` referenciado. | Cumple |
| **D7 — Neutralidad de stack (scan léxico)** | **Scan explícito sin coincidencias en el cuerpo arquitectónico (ver §3). Vocabulario REST genérico y tipos físicos lógicos correctamente usados.** | **Cumple — sin fugas** |
| D8 — Inmutabilidad y versionado | ADRs individuales bajo `adrs/`, ninguno consolidado; estado `Aceptado` declarado en los 10; sufijo `_v1.0.md` uniforme; sin patrón `.v`. | Cumple |

### Fila explícita — Scan de stack (D7)

Patrón aplicado sobre toda la carpeta `05_arquitectura_tecnica/` (maestro, índice, 10 ADRs, modelo, contrato, flujo, README), insensible a mayúsculas:

`.NET, ASP.NET, SQL Server, EF Core / Entity Framework, JWT, ROPC, OAuth, Docker, Blazor, MAUI, Amazon S3 / S3, PostgreSQL/Postgres, MySQL, MongoDB, Redis, Kestrel, Dapper, Serilog, nginx, Kubernetes, RabbitMQ, Azure, AWS, GCP, JavaScript/TypeScript/Python/Java/Node.js, Keycloak/Auth0/IdentityServer, xUnit/NUnit, Swashbuckle, FluentValidation, MediatR, gRPC, Npgsql, "bearer token JWT".`

**Resultado: 0 (cero) coincidencias.** El cuerpo arquitectónico abstrae correctamente todo el stack que el intake sí nombra (.NET 8, ASP.NET Core, SQL Server, JWT, ROPC, Amazon S3, GitVersion, etc.):

- "almacén relacional" en vez de SQL Server.
- "token bearer" / "el backend es su propio emisor y validador; no hay IdP externo" en vez de JWT/ROPC.
- "abstracción de almacenamiento" / "proveedor local o de objetos remoto" en vez de S3.
- "herramienta de migraciones del runtime" en vez del ORM concreto.
- "contenedor de backend" / "runtime del backend" en vez de Docker/.NET.
- Tipos físicos lógicos en el modelo de datos (cadena, entero, decimal, fecha-hora, identificador, geográfico, lógico, enum) sin nombrar el motor; la elección del motor se difiere explícitamente a 09.

Vocabulario REST permitido por el tipo (problem+json, RFC 7807, idempotencia, paginación, OpenAPI, endpoint, token bearer, SemVer, URI) presente y correcto. **No hay P0 de fuga de protocolo del dominio fuente; la lección P0 del proyecto anterior está internalizada.**

---

## 3. Matriz de estructura (§6 / §3.3 / §5 del encargo)

| Requisito estructural | Esperado | Observado | Estado |
| --- | --- | --- | --- |
| Carpeta target | `proyectos/geovial-api/05_arquitectura_tecnica/` | Correcta | OK |
| Subcarpeta de ADRs | `adrs/` | Presente, con 10 ADRs | OK |
| Nombre de ADR | `ADR-XX-<kebab>_v1.0.md`, dos dígitos, slug minúsculas, `_v` | Los 10 cumplen el patrón | OK |
| Documento maestro | `arquitectura-solucion_v1.0.md`, 4 vistas + §1–§10 | Presente; vistas lógica/procesos/despliegue/datos + cross-cutting + NFR + riesgos + trazabilidad | OK |
| Índice de ADRs | `decisiones-arquitectura_v1.0.md` sin cuerpo de decisiones | Presente; tabla índice + cobertura del mínimo + trazabilidad upstream | OK |
| Mínimo de ADRs `rest-api` | 5 (estilo, persistencia, autenticación, paginación, errores) | Los 5 presentes (ADR-01..05) + 5 adicionales | OK |
| ADR — 10 secciones §4.3 | Contexto, Decisión, Estado, Alternativas, Consecuencias +/−, Implementación, Métricas, Referencias, Control de cambios | Los 10 ADRs tienen las 10 secciones | OK |
| ADR — estado declarado | Propuesto/Aceptado/Superado/Rechazado | Los 10 en `Aceptado` con fecha | OK |
| Modelo lógico (tiene_persistencia) | `modelo-datos-logico_v1.0.md` con migración referenciada y traza al conceptual | Presente; `M0001_inicial` referenciado; traza entidad por entidad | OK |
| Contrato externo | `contratos-rest_v1.0.md` (OpenAPI obligatorio) con esquema/errores/versionado | Presente; OpenAPI descriptivo inline; esquemas DTO; taxonomía problem+json; versionado por URI | OK |
| Extensibilidad | Omitir si `tiene_extensibilidad=false` | Correctamente omitido y declarado en README | OK |
| README | Recomendado | Presente y navegable | OK |
| Patrón `.v` prohibido | Ninguno | Ninguno | OK |
| ADR consolidado | Prohibido | Ninguno; una decisión por archivo | OK |

Estructura: **conforme en su totalidad.**

---

## 4. Coherencia cross-doc

| Verificación | Resultado |
| --- | --- |
| 12 entidades conceptuales de 02 mapeadas | Las 12 (Usuario, Rol, Relevamiento, TramoVial, Asignacion, MarcadorGeografico, ConflictoMarcadores, Observacion, Foto, Comentario, Etiqueta, MarcaSincronizacion) tienen tabla de origen. 4 tablas de soporte (TramoComponente, ConflictoMarcadorMiembro, EtiquetaFoto, EtiquetaMarcador) materializan composición/N–N/2..N; ClaveIdempotencia declarada explícitamente como única sin origen conceptual (deriva de ADR-08). Correcto. |
| ADRs referencian CU/RN/NFR reales | NB-01..07 existen (verificado en 01); 22 CU existen (verificado en 02); 7 RN y 6 RC existen. Sin referencias huérfanas. |
| NFR ↔ intake §17.P.10 | Coinciden exactamente: latencia p95 lecturas ≤300 ms, escrituras ≤500 ms, disponibilidad ≥99,5 %, lote ≥1000 cambios, `tiene_observabilidad_critica=false` (sin SLO 99,9 % ni p99). |
| multi_tenant=false reflejado | §6 del maestro y §5 del modelo lógico declaran single-tenant; sin columna discriminadora ni partición por tenant; jerarquía como control de acceso. Coherente con intake §17.P.4. |
| Contrato REST cubre los 22 CU | CU-01..17 vía superficie REST + caso de uso; CU-18..22 como transversales. Tabla de alcance §1 del contrato mapea cada CU a su área. Sin CU huérfano. |
| Integración con la abstracción de almacenamiento | ADR-09 y el contrato referencian `geovial-storage/contratos-abstractions_v1.0.md` como arista del manifiesto §13; el puerto de almacenamiento se alinea con las dos interfaces del contrato de storage. Correcto. |
| Trazabilidad upstream 02/01/00 | Maestro, ADRs, modelo y contrato referencian CU/RN/RC/NB. |
| Trazabilidad downstream 06/08/09 | README y maestro anclan 06 (backlog), 08 (testing/integración) y 09 (despliegue/migraciones). |

Coherencia cross-doc: **sólida.** Dos reconciliaciones menores listadas en §5 (no rompen trazabilidad).

---

## 5. Hallazgos

### P0 (bloqueantes)

Ninguno.

### P1 (incumplen §6 sin romper trazabilidad)

Ninguno.

### P2 (opcionales / reconciliaciones)

**H-01 (P2) — Código de error `OPERACION_NO_IDEMPOTENTE` no consolidado en el contrato.**
- Archivo / sección: `adrs/ADR-08-idempotencia-operaciones-no-seguras_v1.0.md` §7 vs. `contratos-rest_v1.0.md` §5.
- Evidencia: ADR-08 §7 menciona que las operaciones que no admiten clave "ignoran o rechazan la clave según el recurso (`OPERACION_NO_IDEMPOTENTE`)", pero ese código no aparece en la taxonomía de errores del contrato (§5), que sí incluye `CLAVE_REQUERIDA_AUSENTE` y `CLAVE_REUTILIZADA_INCONSISTENTE`. La regla 05 §4.3 pide que ADR-05 centralice el catálogo en `contratos-rest`.
- Recomendación: agregar `OPERACION_NO_IDEMPOTENTE` (estado 400 o 409 según se defina) a la tabla §5 del contrato, o suprimir la mención en ADR-08 si la política es ignorar la clave silenciosamente. Reconciliación de catálogo, no afecta trazabilidad.

**H-02 (P2) — `ux_clave_idempotencia` por `clave` simple no contempla el "ámbito de operación".**
- Archivo / sección: `modelo-datos-logico_v1.0.md` §1.17 (notas de `clave`) vs. §2 (índice `ux_clave_idempotencia`).
- Evidencia: la columna `clave` se documenta como "único por ámbito de operación", pero el índice único declarado es sobre `clave` sola, sin la columna de ámbito. Si dos ámbitos pudieran reutilizar la misma cadena de clave, la unicidad global colisionaría; si la clave ya es globalmente única, la frase "por ámbito de operación" sobra.
- Recomendación: alinear la nota con el índice (clave globalmente única) o incorporar la columna de ámbito al índice único. Precisión del modelo, sin impacto en trazabilidad.

### P3 (estilo)

**H-03 (P3) — Conteo "16 tablas de dominio" en README/control de cambios puede confundir.**
- Archivo / sección: `README.md` ("16 tablas de dominio más una tabla técnica") y `modelo-datos-logico_v1.0.md` §7.
- Evidencia: el modelo tiene 16 subsecciones de tabla de dominio (12 de entidad conceptual + 4 de soporte de composición/unión) más ClaveIdempotencia. La frase es correcta pero un lector apresurado podría leer "16 entidades". El propio modelo §7 lo aclara bien; el README lo abrevia.
- Recomendación: matizar en el README ("16 tablas de dominio: 12 de entidad conceptual y 4 de composición/unión"). Cosmético.

**H-04 (P3) — Estado de cabecera del maestro/índice/modelo/contrato/flujo es `Propuesto` mientras los ADRs están `Aceptado`.**
- Archivo / sección: cabeceras de `arquitectura-solucion`, `decisiones-arquitectura`, `modelo-datos-logico`, `contratos-rest`, `flujo-ejecucion`.
- Evidencia: el README lo explicita ("Propuesto (ADRs Aceptados)"), lo cual es consistente con la regla (los no-ADR admiten `Propuesto`). No es un defecto, se registra solo como nota de coherencia de estados para la fase de aprobación.
- Recomendación: ninguna acción obligatoria; al aprobar la fase, considerar elevar el maestro a `Aceptado`.

---

## 6. Verificación de criterios de aceptación §6 de 05_rules

- [x] `arquitectura-solucion_v1.0.md` con 4 vistas mínimas y §1–§10.
- [x] `decisiones-arquitectura_v1.0.md` indexa los ADRs con estado y fecha.
- [x] ≥3 ADRs (hay 10) como archivos individuales con las 10 secciones.
- [x] Cada ADR con estado declarado (los 10 `Aceptado`).
- [x] `modelo-datos-logico_v1.0.md` con migración inicial (`M0001_inicial`) y traza al conceptual.
- [x] `contratos-rest_v1.0.md` con esquema, errores y versionado.
- [x] Estilo arquitectónico justificado contra ≥2 alternativas (microservicios, UI en proceso, monolito sin capas).
- [x] Cada NFR con objetivo numérico y mecanismo de medición.
- [x] Trazabilidad NFR↔arquitectura↔ADR en tabla (§8 y §10 del maestro).
- [x] Ningún archivo con patrón `.v<X.Y>.md`.
- [x] Ningún ADR consolidado.
- [x] Sin menciones a stacks/productos/protocolos del dominio fuente.
- [x] README presente.

Criterios de nivel solución: no aplican a esta fase (corresponden a `_solucion/`, Fase H).

---

## 7. Veredicto

**APROBADO CON OBSERVACIONES.**

La arquitectura técnica de `geovial-api` es completa, estructuralmente conforme a la regla 05 (§2.2, §3.3, §4, §6) y trazable upstream (02/01) y downstream (06/08/09). El scan de neutralidad de stack (D7) no arrojó ninguna fuga: el cuerpo abstrae correctamente todo el stack que el intake nombra, internalizando la lección P0 del proyecto anterior. Las 12 entidades conceptuales están mapeadas, los 22 CU cubiertos, los NFR coinciden con el intake §17.P.10 y `multi_tenant=false` está reflejado en el modelo. Los 10 ADRs son individuales, con estado declarado y las 10 secciones obligatorias.

No se detectaron P0 ni P1. Las observaciones son dos reconciliaciones de catálogo/modelo (P2) y dos notas de estilo (P3) que no rompen trazabilidad ni omiten documentos obligatorios. **La fase puede avanzar.** Se recomienda atender H-01 y H-02 en la próxima revisión de los entregables o como ajuste menor antes de pasar a 06.

### Conteo por nivel

| Nivel | Cantidad |
| --- | --- |
| P0 | 0 |
| P1 | 0 |
| P2 | 2 |
| P3 | 2 |

---

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Auditoría independiente inicial de la Fase C (arquitectura técnica) de geovial-api: matriz D1–D8 con fila explícita de scan de stack (0 fugas), matriz de estructura, coherencia cross-doc, hallazgos (0 P0, 0 P1, 2 P2, 2 P3) y veredicto APROBADO CON OBSERVACIONES. |
