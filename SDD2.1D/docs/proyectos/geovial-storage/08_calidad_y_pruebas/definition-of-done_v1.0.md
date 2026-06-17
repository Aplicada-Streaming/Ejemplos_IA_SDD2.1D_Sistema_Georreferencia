# Definition of Done — geovial-storage

**Proyecto:** geovial-storage
**Documento:** definition-of-done_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (variante QA + SDET Library)

Este documento es la fuente canónica de la Definition of Done de `geovial-storage`. El mini-plan de 07 lo referencia y no lo reproduce ni redefine (regla 08 §4.8); su nota de pendencia ("enlazar este plan a la DoD canónica cuando la categoría 08 publique su artefacto") queda satisfecha por este documento. Cada criterio se valida con una operación mecánica concreta (un comando, un check del pipeline o una métrica de reporte). Las herramientas se nombran por rol abstracto.

## 1. DoD por capa

### 1.1 DoD de historia de usuario (US)

- [ ] Los criterios de aceptación Given-When-Then de la US tienen al menos un TC que los cubre y está en verde. Validación: reporte de la suite cruzado con `matriz-cobertura-pruebas_v1.0.md`.
- [ ] La US referencia su CU de origen y la matriz CU↔Tests no la deja huérfana. Validación: tabla §2 de la matriz.
- [ ] El TC asociado es determinista, reproducible y no depende del orden de ejecución. Validación: corrida repetida y aleatorización del orden en CI.
- [ ] Cada test de la US tiene al menos un assert explícito (sin tests sin verificación). Validación: revisión de PR y conteo de asserts.
- [ ] Las invariantes RN aplicables a la US (RN-01, RN-02, RN-03 según corresponda) están cubiertas por su TC o por la batería de contrato. Validación: tabla §4 de la matriz.
- [ ] La superficie pública tocada por la US no nombra ningún proveedor concreto. Validación: análisis estático y revisión de PR (RN-01, ADR-02).

### 1.2 DoD de tarea técnica (BT)

- [ ] La BT compila sin warnings tratados como error. Validación: gate G-01 en CI.
- [ ] La cobertura de la capa que la BT toca cumple su umbral (dominio 85/80; infraestructura 70/60). Validación: gate G-04, medidor de cobertura segmentado.
- [ ] Si la BT toca dominio, el mutation score de los componentes afectados es ≥ 60 %. Validación: gate G-05, framework de mutation testing.
- [ ] La BT no introduce issues críticos en el análisis estático. Validación: gate G-09.
- [ ] Si la BT implementa o modifica un adaptador de proveedor, pasa la batería de contrato única con resultados equivalentes. Validación: gate G-06 (TC-21, TC-27).
- [ ] Si la BT toca el manejo de credenciales, no se filtra ninguna credencial por resultado, error ni registro. Validación: gate G-07 (TC-24).

### 1.3 DoD de sprint (tramo del mini-plan de 07)

- [ ] Todas las US y BT comprometidas en el tramo cumplen su DoD de capa. Validación: tablero del tramo + reportes de CI.
- [ ] La suite completa del tramo está en verde. Validación: gate G-02 en CI.
- [ ] La cobertura global del tramo cumple el gate de intake §17 P.6 (líneas ≥ 80 %, branches ≥ 70 %). Validación: gate G-03.
- [ ] La matriz de cobertura se actualizó con el estado real de los tests del tramo (sin "Pendiente" donde ya hay tests). Validación: revisión de la matriz al cierre.
- [ ] Cada bug encontrado en el tramo y cerrado generó al menos un TC de regresión. Validación: vínculo bug↔TC.
- [ ] Ningún snapshot del contrato se regeneró sin PR con justificación y revisión. Validación: historial de PR y TC-22.

### 1.4 DoD de release

- [ ] Todos los criterios de `criterios-validacion_v1.0.md` (funcionales, no funcionales, regresión, calidad de código) están cumplidos o tienen excepción documentada con ADR y plan de remediación. Validación: checklist de criterios de validación.
- [ ] La batería de contrato única pasa contra cada proveedor soportado (local, remoto cuando aplica, doble en memoria) con resultados equivalentes. Validación: gate G-06.
- [ ] Las invariantes RN-01, RN-02 y RN-03 están en verde contra el proveedor local y el doble en memoria. Validación: TC-21, TC-23, TC-24.
- [ ] NFR-01 (latencia p95 ≤ 1 s para ≤ 5 MB local) medido y cumplido en ambiente equivalente al productivo, o excepción documentada (GAP-03). Validación: gate G-08 (TC-25).
- [ ] NFR-02 (tamaño máximo 25 MB configurable) verificado. Validación: TC-26.
- [ ] Cobertura por capa y global cumplidas; mutation score de dominio ≥ 60 %. Validación: gates G-03, G-04, G-05.
- [ ] La versión del contrato sigue SemVer 2.0.0 (ADR-03); un cambio incompatible incrementa la versión mayor y se coordinó con `geovial-api`. Validación: revisión de versión y de Conventional Commits.
- [ ] La suite de regresión completa en verde; ningún test verde de la versión anterior pasó a rojo sin justificación. Validación: comparación de reportes (CV-R1, CV-R2).

## 2. Excepciones admitidas

- Deuda técnica documentada: una BT puede declararse Done con una limitación conocida solo si la limitación queda registrada como BT explícita en el backlog de 06 con plan de remediación.
- Spikes: una BT de tipo spike puede declararse Done sin criterios Given-When-Then si tiene caja temporal y una pregunta de investigación clara (alineado con la DoR de 06).
- NFR diferida: NFR-01 puede medirse en CI como aproximación si el ambiente equivalente al productivo aún no está disponible (GAP-03), con ratificación obligatoria antes del release.
- Proveedor remoto: el contract test contra el remoto puede diferirse del MVP porque BT-09 es Should (GAP-02), siempre que la transparencia esté verde contra el proveedor local y el doble en memoria.
- Ninguna excepción puede afectar a RN-01, RN-02 ni RN-03 sobre el proveedor local y el doble en memoria.

## 3. Vigencia

Este documento es la fuente canónica de la DoD del proyecto. Los sprint plans y el mini-plan de 07 lo referencian, no lo redefinen (regla 08 §4.8). Cualquier cambio de un criterio versionable se registra en el control de cambios de este documento (§5) y se comunica al equipo en el sprint review siguiente (regla 08 §3.4).

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU | CU-01 a CU-06 (02) |
| RN | RN-01, RN-02, RN-03 (02) |
| NFR | NFR-01 a NFR-06 (05) |
| Gates | G-01 a G-09 de `estrategia-calidad_v1.0.md` §3 |
| Plan | mini-plan de 07 (referencia esta DoD); criterios de `criterios-validacion_v1.0.md` |
| Gate global | intake §17 P.6 |

## 5. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | DoD canónica inicial de geovial-storage: cuatro capas (US, BT, sprint/tramo, release) con criterios verificables mecánicamente y herramienta de validación nombrada por cada uno, excepciones admitidas y nota de vigencia que satisface la referencia pendiente del mini-plan de 07. |
