# Estrategia de versionado — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** estrategia-versionado_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps Senior (AG-09), variante DevOps + Release Engineer (library)

## 1. Objetivo

Este documento es la bisagra entre el código (Conventional Commits, branching) y el artefacto publicado (SemVer, canales, deprecación). Lo consumen tanto el autor del paquete como sus consumidores externos, dado que `Aplicada.Sync` es redistribuible (`redistribuible: true`) y reutilizable fuera de la solución. La política de compatibilidad de la superficie pública la fija ADR-03 y `contratos-abstractions_v1.0.md` §6; este documento la operativiza para el pipeline.

## 2. SemVer 2.0.0

Formato adoptado: `MAJOR.MINOR.PATCH[-PRERELEASE][+BUILDMETADATA]`.

La superficie pública versionada es la capa Abstractions y el contrato del ciclo de vida (operaciones CU-01 a CU-06, formas de datos, conjunto de estados de la sesión, garantía de orden subir-antes-de-bajar RN-01, semántica de no duplicación por identificador RN-02 y catálogo de códigos de error). La implementación interna no es superficie pública (ADR-03 §2).

Reglas de incremento, derivadas de `contratos-abstractions_v1.0.md` §6:

| Tipo de cambio | Bump | Ejemplos en el contrato |
| --- | --- | --- |
| Incompatible | MAJOR | Quitar o renombrar una operación, campo o estado; cambiar la obligatoriedad de un campo; invertir o relajar la garantía de orden RN-01; alterar la semántica de no duplicación RN-02; cambiar o quitar un código de error |
| Compatible | MINOR | Agregar una operación, un campo opcional, un estado nuevo no excluyente o un código de error nuevo, preservando el comportamiento existente |
| Corrección | PATCH | Arreglos que no alteran el contrato observable |

Versión inicial publicable: la primera versión `stable` la fija la DoD de release de 08 §1.4; antes de ella, los tags llevan sufijo de prerelease (ver §5). El `BUILDMETADATA` opcional registra el identificador de la corrida de CI y no afecta la precedencia de versión.

## 3. Conventional Commits 1.0.0

Los mensajes de commit usan prefijos semánticos. El branching de merge por squash (§4) produce un único commit Conventional por PR que la herramienta de versión interpreta para calcular el bump.

| Prefijo de Conventional Commit | Bump SemVer | Ejemplo |
| --- | --- | --- |
| `feat` | MINOR | `feat(engine): exponer consulta de progreso parcial del ciclo` |
| `fix` | PATCH | `fix(queue): evitar reencolado duplicado por identificador estable` |
| `feat!` o footer `BREAKING CHANGE` | MAJOR | `feat(abstractions)!: cambiar la firma del contrato de transporte` |
| `refactor`, `perf`, `test`, `chore`, `docs`, `style`, `build`, `ci` | Ninguno | `refactor(state): extraer el registro de progreso` |

El marcador `!` tras el tipo/scope o un footer `BREAKING CHANGE:` declara el cambio mayor. La clasificación del commit debe coincidir con la clasificación del cambio de superficie pública del gate G8 (08 estrategia-calidad §3): un `feat!` sin incremento MAJOR, o un cambio incompatible enviado como `fix`, es un hallazgo de la auditoría del changelog contra la matriz de compatibilidad (DoD release §1.4; ADR-03 §8).

## 4. Branching

Trunk-based development con `main` protegida, alineado al modo release-driven y a `equipo_n=1` del proyecto (08 estrategia-calidad §4; mini-plan de 07):

- `main` es la única rama de larga vida; las ramas `feature/<slug>` son cortas y se integran por PR.
- Protección de `main`: ningún cambio entra sin pasar los gates G1-G9 del pipeline (los stages bloqueantes de `pipeline-ci-cd_v1.0.md` §3). Con `equipo_n=1`, el control del pipeline y la revisión del propio PR sustituyen la revisión por pares ausente (08 §4).
- Merge por squash: un PR produce un único commit con mensaje Conventional Commits que define el bump.
- Los releases se disparan por tag sobre `main`, no por merge a una rama de release; no hay Git Flow ni ramas `release/*` permanentes.

Esta estrategia encaja con el branching del acuerdo de equipo de la categoría 00 (trunk-based, PR obligatorio); si el acuerdo de equipo declarara otro flujo, prevalece el acuerdo y se versiona este documento.

## 5. Herramienta de versión

Herramienta adoptada: GitVersion (intake §17 P.7, ratificable). Calcula la versión a partir de los tags Git y de los Conventional Commits desde el último tag, sin versionado manual (anti-patrón 4.8 "versionado manual" evitado).

- Prefijo de tag: `v` (por ejemplo `v1.0.0`, `v1.1.0-rc.1`).
- Fuente del bump: los Conventional Commits desde el último tag estable, según la tabla de §3.
- Sufijos de prerelease para el canal `preview`: `-alpha.N`, `-beta.N`, `-rc.N`. La precedencia SemVer es `alpha < beta < rc < release`.
- Configuración base: la herramienta deriva la versión de CI y la inyecta en el empaquetado (`PackageVersion`), de modo que el `.nupkg` y el tag Git coincidan. El `BUILDMETADATA` lleva el identificador de corrida.
- El CHANGELOG (Keep a Changelog 1.1.0) se genera automáticamente desde los Conventional Commits y se publica en el release (anti-patrón 4.8 "CHANGELOG ausente" evitado).

## 6. Canales y deprecation policy

Canales sobre el feed único (detalle operativo en `entornos-deploy_v1.0.md`):

| Canal | Tags que lo alimentan | Sufijo SemVer | Aprobador |
| --- | --- | --- | --- |
| `preview` | `vX.Y.Z-alpha.N`, `-beta.N`, `-rc.N` | prerelease | Automático |
| `stable` | `vX.Y.Z` sin sufijo | release | Release manager (AG-07) |

No se declara un canal LTS en v1: el alcance Android único y `equipo_n=1` no justifican mantener líneas LTS paralelas; se incorpora con un ADR si una versión futura lo requiere.

Deprecation policy (alineada con ADR-03 §6/§7 y `contratos-abstractions_v1.0.md` §6):

- Ningún elemento del contrato (operación, campo, estado, código de error, garantía de orden) se remueve sin un período de deprecación documentado y un incremento de versión MAJOR en la remoción.
- Un elemento deprecado se marca como obsoleto en el código (atributo de obsolescencia del runtime) con un mensaje que indica el reemplazo, y se anuncia en el CHANGELOG y en el portal de developers de la categoría 03.
- Período mínimo de gracia: el elemento deprecado vive al menos una versión MINOR antes de su remoción en la MAJOR siguiente, de modo que los consumidores tengan una versión intermedia con el aviso de obsolescencia antes del cambio incompatible.
- Métrica de cumplimiento (ADR-03 §8): 100 % de los cambios incompatibles publicados con incremento MAJOR; 0 remociones sin período de deprecación previo documentado.

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Política de compatibilidad de la superficie pública | ADR-03; `contratos-abstractions_v1.0.md` §6 |
| Gate que verifica la clasificación del cambio | G8 (08 estrategia-calidad §3); DoD US §1.1 y release §1.4 |
| Conventional Commits y bump | §3 de este documento; tabla SemVer de 09_rules §4.7 |
| Branching | Acuerdo de equipo de 00 (trunk-based); 08 §4 (equipo_n=1) |
| Canales y feed | `entornos-deploy_v1.0.md`; intake §17 P.7 |
| Herramienta de versión | GitVersion (intake §17 P.7) |
| Downstream | Pipeline (`pipeline-ci-cd_v1.0.md` §2 triggers por tag); developer guide de 10 (workflow de versionado); examples de 11 (canales declarados) |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Estrategia de versionado inicial de aplicada-sync: SemVer 2.0.0 con la superficie pública (capa Abstractions y contrato del ciclo de vida) como unidad versionada y reglas de incremento derivadas de ADR-03 y contratos §6; Conventional Commits 1.0.0 con mapeo a bump y coincidencia obligatoria con el gate G8; branching trunk-based con main protegida por los gates G1-G9 y merge por squash; GitVersion como herramienta de versión con tags prefijo v y sufijos de prerelease; canales preview/stable con aprobador y deprecation policy alineada a contratos §6. Documento bisagra entre código y artefacto. Derivado del intake §17 P.7, de ADR-03 y de contratos-abstractions §6. |
