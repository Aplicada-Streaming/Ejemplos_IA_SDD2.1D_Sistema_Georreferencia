# Estrategia de versionado — geovial-api

**Proyecto:** geovial-api
**Documento:** estrategia-versionado_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps + Platform Engineer

## 1. Objetivo

Este documento es la bisagra entre el código (Conventional Commits, branching) y el artefacto publicado (SemVer, ambientes, deprecación). Lo consumen el autor del backend y, de forma indirecta, los consumidores del contrato (`geovial-web` y `geovial-mobile`), porque la versión del contrato REST gobierna su compatibilidad. `geovial-api` no es redistribuible (`redistribuible: false`): su artefacto desplegable es una imagen de contenedor y su artefacto publicable hacia los clientes es el contrato OpenAPI versionado por URI. La política de versionado del contrato la fija ADR-10 y `contratos-rest_v1.0.md` §6; este documento la operativiza para el pipeline y la coordina con SemVer del proyecto.

## 2. SemVer 2.0.0

Formato adoptado: `MAJOR.MINOR.PATCH[-PRERELEASE][+BUILDMETADATA]`.

La unidad versionada es el backend desplegable y, de forma acoplada, su contrato público REST. La relación entre la versión del proyecto y la versión mayor del contrato por URI se declara en §6.

Reglas de incremento, derivadas de `contratos-rest_v1.0.md` §6 y de la tabla SemVer de 09_rules §4.7:

| Tipo de cambio | Bump | Ejemplos en el contrato y en el backend |
| --- | --- | --- |
| Incompatible | MAJOR | Quitar un campo, volver obligatorio uno opcional, cambiar la semántica de una operación, quitar o renombrar un código de error; implica nueva versión mayor del contrato por URI (`/v2`, ADR-10) |
| Compatible | MINOR | Agregar un endpoint, un campo opcional, un valor de enum adicional o un código de error nuevo, preservando el comportamiento existente; se incorpora dentro de la misma versión mayor del contrato |
| Corrección | PATCH | Arreglos de comportamiento que no alteran el contrato observable |
| Sin efecto | Ninguno | `refactor`, `perf`, `test`, `chore`, `docs`, `style`, `build`, `ci` |

Versión inicial publicable: la primera versión `stable` la fija la DoD de release de 08 §1.4; el contrato está estabilizado bajo el prefijo `/v1` desde el primer release (intake §17 P.3). El `BUILDMETADATA` opcional registra el identificador de la corrida de CI y no afecta la precedencia de versión.

## 3. Conventional Commits 1.0.0

Los mensajes de commit usan prefijos semánticos. El merge por squash (§4) produce un único commit Conventional por PR que la herramienta de versión interpreta para calcular el bump.

| Prefijo de Conventional Commit | Bump SemVer | Ejemplo |
| --- | --- | --- |
| `feat` | MINOR | `feat(relevamientos): agregar filtro por etiqueta en el listado` |
| `fix` | PATCH | `fix(sincronizacion): reconocer reenvío por identificador de origen` |
| `feat!` o footer `BREAKING CHANGE` | MAJOR | `feat(contrato)!: migrar el recurso de conflictos a /v2` |
| `refactor`, `perf`, `test`, `chore`, `docs`, `style`, `build`, `ci` | Ninguno | `refactor(aplicacion): extraer el servicio de paginación` |

El marcador `!` tras el tipo/scope o un footer `BREAKING CHANGE:` declara el cambio mayor. La clasificación del commit debe coincidir con la clasificación del cambio de contrato del gate G4/G5 (08 estrategia-calidad §3): un `feat!` sin nueva versión mayor del contrato, o un cambio incompatible enviado como `fix`, es un hallazgo de la auditoría del changelog contra los contract tests de compatibilidad (DoD release §1.4; CU-22; ADR-10).

## 4. Branching

Trunk-based development con `main` protegida (intake §17 P.7), alineado a `equipo_n=1` (08 estrategia-calidad §4; mini-plan de 07):

- `main` es la única rama de larga vida; las ramas `feature/<slug>` son cortas y se integran por PR.
- Protección de `main`: ningún cambio entra sin pasar los gates bloqueantes de merge G1-G6 del pipeline (`pipeline-ci-cd_v1.0.md` §3). Con `equipo_n=1`, el control del pipeline y la revisión del propio PR sustituyen la revisión por pares ausente (08 §4).
- Merge por squash: un PR produce un único commit con mensaje Conventional Commits que define el bump.
- Los releases se disparan por tag sobre `main`, no por merge a una rama de release; no hay Git Flow ni ramas `release/*` permanentes. El tag con sufijo `-rc.N` dispara la promoción a QA/STAGING; el tag sin sufijo dispara la promoción a PROD (`pipeline-ci-cd_v1.0.md` §2/§6).

Esta estrategia encaja con el branching del acuerdo de equipo de la categoría 00 (trunk-based, PR obligatorio); si el acuerdo de equipo declarara otro flujo, prevalece el acuerdo y se versiona este documento.

## 5. Herramienta de versión

Herramienta adoptada: GitVersion (intake §17 P.7, ratificable). Calcula la versión a partir de los tags Git y de los Conventional Commits desde el último tag, sin versionado manual (anti-patrón §4.8 "versionado manual" evitado).

- Prefijo de tag: `v` (por ejemplo `v1.0.0`, `v1.1.0-rc.1`).
- Fuente del bump: los Conventional Commits desde el último tag estable, según la tabla de §3.
- Sufijos de prerelease para la promoción por ambientes: `-rc.N` (candidato a release que recorre QA y STAGING). La precedencia SemVer es `rc < release`.
- Configuración base: la herramienta deriva la versión en CI y la inyecta como etiqueta de la imagen de contenedor y como versión del documento OpenAPI publicado, de modo que la imagen, el tag Git y el contrato coincidan. El `BUILDMETADATA` lleva el identificador de corrida.
- El CHANGELOG (Keep a Changelog 1.1.0) se genera automáticamente desde los Conventional Commits y se publica en el release (anti-patrón §4.8 "CHANGELOG ausente" evitado).

## 6. Ambientes, canales y deprecation policy

El modelo del proyecto son ambientes de servicio desplegable DEV / QA / STAGING / PROD con canary (regla §2.2 para `rest-api`), no canales de paquete; el detalle operativo de ambientes vive en `entornos-deploy_v1.0.md`. La promoción por ambientes la dispara el sufijo del tag:

| Ambiente | Tag o evento que promueve | Aprobador |
| --- | --- | --- |
| DEV | Merge a `main` (build automático) | Automático |
| QA | Tag `vX.Y.Z-rc.N` | QA lead |
| STAGING | Aprobación tras QA en verde (mismo `-rc.N`) | Release manager |
| PROD (canary) | Tag `vX.Y.Z` sin sufijo | Release manager + aprobación de negocio |

Deprecation policy (coordinada con el versionado del contrato por URI, ADR-10, CU-22 y `contratos-rest_v1.0.md` §6):

- Ningún elemento del contrato (recurso, campo, código de error, semántica de operación) se remueve sin un período de deprecación documentado y un incremento de versión mayor del contrato por URI en la remoción.
- Un cambio incompatible publica una versión mayor nueva (`/v2`) conservando la versión mayor previa (`/v1`) durante un período de convivencia de al menos un MINOR (intake §17 P.3); el backend atiende ambas versiones mayores durante la convivencia.
- Un recurso o código a retirar se marca obsoleto en una versión menor antes de removerse en la mayor siguiente; el backend comunica el plan de retiro y responde a una versión retirada con `VERSION_NO_SOPORTADA` y a un recurso ausente en la versión con `RECURSO_NO_EN_VERSION` (`contratos-rest_v1.0.md` §6).
- No se declara una línea LTS del backend en v1: `equipo_n=1` y un único contrato vigente por versión mayor no justifican mantener líneas paralelas; se incorpora con un ADR si una versión futura lo requiere.
- Métrica de cumplimiento: 100 % de los cambios incompatibles publicados con nueva versión mayor del contrato; 0 remociones sin período de convivencia previo (verificado por los contract tests de compatibilidad por versión, CU-22, intake §17 P.6).

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Política de compatibilidad del contrato | ADR-10; `contratos-rest_v1.0.md` §6; CU-22 |
| Gate que verifica la clasificación del cambio | G4 y G5 (08 estrategia-calidad §3); DoD US §1.1 y release §1.4 |
| Conventional Commits y bump | §3 de este documento; tabla SemVer de 09_rules §4.7 |
| Branching | Acuerdo de equipo de 00 (trunk-based); intake §17 P.7; 08 §4 (equipo_n=1) |
| Ambientes y promoción | `entornos-deploy_v1.0.md`; `pipeline-ci-cd_v1.0.md` §6; intake §17 P.7 |
| Herramienta de versión | GitVersion (intake §17 P.7) |
| Downstream | Pipeline (`pipeline-ci-cd_v1.0.md` §2 triggers por tag); guía de publicación del contrato (`guia-publicacion-openapi_v1.0.md`); developer guide de 10 (workflow de versionado) |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Estrategia de versionado inicial de geovial-api: SemVer 2.0.0 con el backend desplegable y su contrato público REST como unidad versionada acoplada; reglas de incremento coordinadas con el versionado del contrato por URI (ADR-10) y la tabla SemVer de 09_rules §4.7; Conventional Commits 1.0.0 con mapeo a bump y coincidencia obligatoria con los gates de contrato G4/G5; branching trunk-based con main protegida por los gates de merge y merge por squash; GitVersion como herramienta de versión con tags prefijo v y sufijo -rc para la promoción por ambientes; modelo de ambientes DEV/QA/STAGING/PROD con canary (no canales de paquete) y deprecation policy coordinada con la convivencia de versiones mayores del contrato. Derivado del intake §17 P.3/P.7, de ADR-10 y de contratos-rest §6. |
