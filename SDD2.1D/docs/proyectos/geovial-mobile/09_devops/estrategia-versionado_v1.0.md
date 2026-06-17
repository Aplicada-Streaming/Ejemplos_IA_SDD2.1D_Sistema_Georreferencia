# Estrategia de versionado — geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** estrategia-versionado_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps + Mobile Release Engineer

## 1. Objetivo y alcance

Este documento es la bisagra entre el código (Conventional Commits, branching) y el artefacto distribuido (SemVer, canales de distribución móvil, deprecation). Lo consumen tanto quien construye la app móvil como quien la recibe por un canal de distribución interno. La estrategia parte del intake §17 P.7 de `geovial-mobile`: SemVer 2.0.0, Conventional Commits, versión derivada del tag con la herramienta de versión (GitVersion), branching trunk-based (`equipo_n=1`) y canales `internal`, `alpha`, `beta`, `production` con distribución del paquete de aplicación Android por un canal de distribución interno (no se publica en tienda pública en v1).

## 2. SemVer 2.0.0

Formato adoptado: `MAJOR.MINOR.PATCH[-PRERELEASE][+BUILDMETADATA]`.

- MAJOR: cambio incompatible para el agente de campo o en el contrato que la app consume cuando obliga a una reinstalación con migración del esquema local no retrocompatible (por ejemplo, un cambio del modelo de cola que invalida los cambios encolados pendientes).
- MINOR: capacidad nueva retrocompatible (una pantalla, una etiqueta nueva, un flujo de captura adicional) que no rompe los datos locales ni la sesión activa.
- PATCH: corrección de defecto sobre el comportamiento existente sin cambio de capacidad ni del esquema local.

El sufijo de prerelease distingue el canal de distribución que alimenta el tag (ver §5). El metadato de build puede transportar el número de build de la plataforma sin afectar la precedencia SemVer.

Reglas de incremento ligadas a Conventional Commits:

| Tipo de cambio en Conventional Commits | Bump SemVer | Ejemplo |
| --- | --- | --- |
| `feat` | MINOR | 1.2.3 → 1.3.0 |
| `fix` | PATCH | 1.2.3 → 1.2.4 |
| `feat!` o `BREAKING CHANGE` en footer | MAJOR | 1.2.3 → 2.0.0 |
| `refactor`, `perf`, `test`, `chore`, `docs`, `style`, `build`, `ci` | Ninguno | 1.2.3 → 1.2.3 |

## 3. Conventional Commits 1.0.0

Convención de mensajes con prefijos semánticos: `feat`, `fix`, `chore`, `docs`, `refactor`, `perf`, `test`, `build`, `ci`, `style`. El marcador `!` tras el tipo o un footer `BREAKING CHANGE:` señala un cambio mayor. El merge a `main` se hace por squash con un mensaje Conventional Commits que define el bump.

Ejemplos del dominio de la app:

| Mensaje | Bump | Efecto |
| --- | --- | --- |
| `feat(captura): agrupar fotos por radio en carga manual` | MINOR | Capacidad nueva retrocompatible |
| `fix(sync): no duplicar cambios al reanudar tras un corte` | PATCH | Corrección sobre la reanudación (RN-02) |
| `feat!(local): nuevo esquema de cola incompatible con cambios encolados previos` | MAJOR | Migración local no retrocompatible; obliga a sincronizar antes de actualizar |
| `chore(ci): cachear las herramientas de plataforma Android` | Ninguno | Sin cambio de versión publicada |

## 4. Herramienta de versionado

La versión se deriva del tag de git con la herramienta de versión declarada en el intake §17 P.7 (GitVersion). Configuración base:

- Prefijo de tag: `v` (por ejemplo `v1.0.0`).
- Modo trunk-based: la versión se calcula desde el último tag en `main` más los commits posteriores; los prereleases se etiquetan con sufijo de canal (`-internal.N`, `-alpha.N`, `-beta.N`).
- Versión inicial: `1.0.0` para la primera distribución a `production`.
- El número de build de la plataforma Android (versionCode interno del paquete) se deriva de forma monótona del cálculo de versión para que cada paquete distribuido tenga un código de build estrictamente creciente, requisito del empaquetado Android; este código no altera la precedencia SemVer.

La versión se calcula una sola vez por corrida y se propaga a todos los stages (anti-patrón 09 §4.8 "versionado manual" evitado): el tag y los Conventional Commits son la única fuente de la versión.

## 5. Branching y canales

### 5.1 Branching

Trunk-based development, alineado con el intake §17 P.7 (`equipo_n=1`):

- `main` protegida; ramas `feature/<slug>` cortas (menos de 5 días).
- PR obligatorio; con `equipo_n=1` la revisión por pares la sustituyen los gates del pipeline y el registro auditable del aprobador del release (08 `estrategia-calidad_v1.0.md` §4).
- Merge por squash con mensaje Conventional Commits que define el bump.

### 5.2 Canales de distribución móvil

El modelo no es de ambientes de servicio, sino de canales de distribución móvil sobre el canal de distribución interno (regla 09 §2.2 para `mobile-app-maui`). Cada canal se alimenta de su propio tag; el detalle de audiencia y soak está en `entornos-deploy_v1.0.md` §1.

| Canal | Tag que lo alimenta | Semántica | Audiencia |
| --- | --- | --- | --- |
| `internal` | `vX.Y.Z-internal.N` | Build de validación interna; distribución automática | Equipo |
| `alpha` | `vX.Y.Z-alpha.N` | Prerelease temprano para campo acotado | Probadores de campo seleccionados |
| `beta` | `vX.Y.Z-beta.N` | Prerelease estabilizado para piloto ampliado | Piloto de campo |
| `production` | `vX.Y.Z` sin sufijo | Versión estable distribuida a todos los agentes | Agentes de campo en operación |

La precedencia de prereleases sigue SemVer: `1.2.0-internal.1` < `1.2.0-alpha.1` < `1.2.0-beta.1` < `1.2.0`. La ruta a una tienda pública queda como destino futuro documentado en `guia-publicacion-store-mobile_v1.0.md` §1, sin alterar este modelo de canales en v1.

## 6. Deprecation policy

- Una capacidad o pantalla marcada para retiro vive al menos dos MINOR antes de removerse, para dar margen a los agentes de campo a actualizar.
- Un cambio que afecte el esquema local o la compatibilidad de los cambios encolados pendientes se anuncia como `BREAKING CHANGE` y se acompaña de una migración que sincroniza o preserva la cola antes de aplicar el nuevo esquema (ADR-02, ADR-03); nunca se descarta la cola sin sincronizar.
- Los breaking changes se comunican en el CHANGELOG (Keep a Changelog 1.1.0), generado a partir de Conventional Commits, y en las notas de versión del canal `production`.
- La app fija la versión mayor del contrato que consume y declara el período de convivencia con la versión previa (05 §9, riesgo de incompatibilidad de contrato); un cambio mayor del contrato consumido obliga a un MAJOR de la app con nota de migración.

## 7. Trazabilidad

- Branching trunk-based y Conventional Commits alineados al intake §17 P.7 y al acuerdo de equipo (categoría 00); el pipeline `pipeline-ci-cd_v1.0.md` §2 mapea cada tag a su canal.
- Los canales de §5.2 son los que `entornos-deploy_v1.0.md` §1 detalla con audiencia, soak y aprobador, y los que `guia-publicacion-store-mobile_v1.0.md` consume para distribuir.
- El bump derivado de Conventional Commits alimenta el CHANGELOG que consume la developer guide de 10 y las notas de versión.

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Estrategia de versionado inicial de geovial-mobile: SemVer 2.0.0 con reglas de incremento ligadas a Conventional Commits 1.0.0, herramienta de versión derivada del tag (GitVersion) con número de build Android monótono, branching trunk-based para equipo_n=1, canales de distribución móvil internal/alpha/beta/production sobre el canal de distribución interno (sin tienda pública en v1; ruta futura documentada en la guía de publicación) y deprecation policy que protege la cola y el esquema local ante breaking changes. Derivado del intake §17 P.7 y alineado al acuerdo de equipo de 00. |
