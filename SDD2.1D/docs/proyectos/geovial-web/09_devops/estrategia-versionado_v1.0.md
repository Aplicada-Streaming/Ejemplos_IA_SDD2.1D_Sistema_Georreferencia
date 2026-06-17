# Estrategia de versionado — geovial-web

**Proyecto:** geovial-web
**Documento:** estrategia-versionado_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps + Deploy Engineer

## 0. Alcance

`geovial-web` es un `web-monolith` cuyo artefacto publicable es una imagen de contenedor del front, no un paquete redistribuible (intake §13, §17.P.7). Por tanto, el versionado gobierna la imagen y su promoción por ambientes de servicio (DEV/QA/STAGING/PROD), no canales de paquete. Este documento es la bisagra entre el código (Conventional Commits, branching) y el artefacto desplegado (SemVer, etiquetas de imagen, deprecation). Lo consumen el desarrollador (que produce los commits y los tags) y la categoría 09 (que deriva la etiqueta de la imagen y la promueve).

## 1. SemVer 2.0.0

Se adopta SemVer 2.0.0 con formato `MAJOR.MINOR.PATCH[-PRERELEASE][+BUILDMETADATA]`. La versión etiqueta la imagen de contenedor del front y los tags Git de release.

| Componente | Cuándo se incrementa en el front |
| --- | --- |
| MAJOR | Cambio incompatible para el operador o el usuario: requiere coordinación con un cambio incompatible del contrato REST consumido de `geovial-api` (el front fija la versión mayor del contrato, 05 §9), o un cambio de configuración de despliegue no retrocompatible (nueva variable obligatoria, cambio de la política de afinidad de sesión) |
| MINOR | Nueva capacidad retrocompatible: una nueva vista o pantalla, un nuevo CU cubierto, una mejora de interacción que no rompe la configuración existente |
| PATCH | Corrección retrocompatible: arreglo de un defecto de presentación, de mapeo de error o de consumo del contrato, sin cambiar la configuración ni el contrato consumido |

Como el front no expone una API ni se distribuye como paquete, no hay consumidores de un contrato propio que rompan ante un MAJOR; el MAJOR refleja un cambio incompatible para quien opera el despliegue o para el contrato del backend que el front fija. La versión inicial es `1.0.0`.

El sufijo `+BUILDMETADATA` registra metadatos de construcción no significativos para la precedencia (por ejemplo, el identificador de commit corto), útil para trazar la imagen a su fuente.

## 2. Conventional Commits 1.0.0

Se adopta Conventional Commits 1.0.0. El mensaje de cada commit determina el bump de la versión. El merge a `main` es por squash con un mensaje Conventional Commits que define el incremento.

| Prefijo en Conventional Commits | Bump SemVer | Ejemplo |
| --- | --- | --- |
| `feat` | MINOR | `feat(mapa): encadenar el carrusel con el marcador contiguo` |
| `fix` | PATCH | `fix(sesion): no serializar el token bearer en el render de la vista de ingreso` |
| `feat!` o `BREAKING CHANGE` en el footer | MAJOR | `feat!(api-client): migrar al contrato v2 de geovial-api` |
| `refactor`, `perf`, `test`, `chore`, `docs`, `style`, `build`, `ci` | Ninguno | `perf(circuito): reducir el estado retenido por sesión` |

El marcador `!` después del tipo o un footer `BREAKING CHANGE:` fuerzan MAJOR. El cuerpo del commit describe el cambio para el CHANGELOG, que se genera automáticamente a partir de los commits (regla 09 §4.8, anti-patrón de CHANGELOG ausente).

## 3. Herramienta de versionado

La versión se deriva automáticamente del historial de tags Git con una herramienta de auto-versioning (GitVersion, intake §17.P.7), no se asigna a mano (regla 09 §4.8, anti-patrón de versionado manual).

| Aspecto | Decisión |
| --- | --- |
| Herramienta | Herramienta de auto-versioning derivada de tags Git (GitVersion, intake §17.P.7) |
| Prefijo de tag | `v` (por ejemplo, `v1.4.2`) |
| Fuente del bump | El tipo de Conventional Commit del o de los commits desde el último tag |
| Versión de la imagen | La etiqueta de la imagen de contenedor del front es la versión SemVer calculada; cada ambiente referencia una etiqueta concreta (`entornos-deploy_v1.0.md`) |
| Versión inicial | `1.0.0` desde el primer release |

La etiqueta de la imagen no es mutable: una imagen `v1.4.2` apunta siempre al mismo digest firmado. La promoción entre ambientes mueve la misma imagen, no la reconstruye (`guia-publicacion-image-docker_v1.0.md`).

## 4. Branching

Trunk-based development (intake §17.P.7), alineado al acuerdo de equipo de 00 y a la cadencia de tramos del mini-plan de 07.

| Aspecto | Política |
| --- | --- |
| Rama troncal | `main` protegida; es la única rama de integración |
| Ramas de trabajo | `feature/<slug>` cortas (menos de cinco días); se integran a `main` por PR |
| Protección de `main` | PR obligatorio; al menos una aprobación; suite de gates de merge en verde (STAGE-01..STAGE-07 de `pipeline-ci-cd_v1.0.md`); merge por squash |
| Equipo | `equipo_n=1` (intake §2): el desarrollador es autor y aprobador; la aprobación se sustituye por la suite de gates verde y la autorrevisión registrada, sin saltar la protección de rama |
| Release | Tag `v<X.Y.Z>` sobre `main`; dispara la construcción y publicación de la imagen (`pipeline-ci-cd_v1.0.md` §4) |

Trunk-based encaja con un equipo de un desarrollador y con el vertical slicing por tramos de 07: cada tramo cierra con su capacidad demostrable e integrada en `main`.

## 5. Ambientes (no canales de paquete)

Por ser un servicio desplegable y no una librería, el modelo es de ambientes de servicio, no de canales preview/stable (regla 09 §2.2; anti-patrón "confundir publicación con despliegue", §4.8). La progresión de una versión es:

| Etapa de la versión | Ambiente destino | Semántica |
| --- | --- | --- |
| Build de `main` | DEV | Cada merge a `main` produce una imagen desplegada a DEV para verificación continua |
| Release-candidate (`-rc.N`) | QA → STAGING | Un tag con sufijo `-rc.N` marca una versión candidata que se valida en QA y se somete a la ventana de soak en STAGING |
| Release estable (`v<X.Y.Z>`) | PROD | Un tag sin sufijo es una versión estable promovible a PROD con aprobación |

Los sufijos de prerelease `-alpha.N`, `-beta.N`, `-rc.N` siguen la precedencia de SemVer 2.0.0 y se usan para marcar candidatos antes del release estable. El detalle de cada ambiente, su aprobador y su SLA vive en `entornos-deploy_v1.0.md`.

## 6. Deprecation policy

El front no expone un contrato propio a consumidores externos, por lo que la deprecación aplica a dos frentes: la configuración de despliegue y la versión mayor del contrato REST que consume.

- Configuración de despliegue: una variable de entorno o un parámetro de configuración que se vuelve obsoleto se mantiene aceptado durante al menos un MINOR antes de removerse en un MAJOR; el cambio se anuncia en el CHANGELOG y en las notas de release, y se documenta en `entornos-deploy_v1.0.md` §3.
- Contrato consumido de `geovial-api`: el front fija la versión mayor del contrato (05 §9) y migra a una versión mayor nueva solo en un MAJOR propio, coordinado con la deprecation policy del backend (que conserva la versión previa al menos un MINOR antes de removerla, intake §17 geovial-api P.3). El Cliente de API centraliza el consumo del contrato, de modo que la migración se contiene en un único componente (05 §3, ADR-04).
- Comunicación: todo breaking change se anuncia en el CHANGELOG (generado desde Conventional Commits) y en las notas de release, con guía de migración para el operador cuando cambia la configuración de despliegue.

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Conventional Commits y branching | Acuerdo de equipo de 00; intake §17.P.7 (trunk-based, GitVersion) |
| Bump SemVer → etiqueta de imagen | `pipeline-ci-cd_v1.0.md` §4 (promotion); `guia-publicacion-image-docker_v1.0.md` |
| Ambientes (no canales) | `entornos-deploy_v1.0.md` |
| Migración del contrato consumido | 05 §9 (riesgo de acoplamiento); ADR-04; intake §17 geovial-api P.3 |
| CHANGELOG y deprecation | Generado desde Conventional Commits; notas de release |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Estrategia de versionado inicial de geovial-web: SemVer 2.0.0 sobre la imagen de contenedor del front (MAJOR por cambio incompatible de operación o del contrato consumido, MINOR por capacidad retrocompatible, PATCH por corrección), Conventional Commits 1.0.0 con bump derivado del commit, herramienta de auto-versioning desde tags Git (GitVersion) con prefijo `v` e imagen etiquetada por versión inmutable, branching trunk-based con `main` protegida para equipo_n=1, modelo de ambientes de servicio (no canales de paquete) con prerelease `-rc.N` y deprecation policy para configuración de despliegue y versión mayor del contrato consumido. |
