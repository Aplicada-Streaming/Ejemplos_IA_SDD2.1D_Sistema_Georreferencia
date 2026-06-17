# Estrategia de versionado — geovial-storage

**Proyecto:** geovial-storage
**Documento:** estrategia-versionado_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps Senior (variante DevOps + Release Engineer, library)

## 0. Posición del documento

Este es el documento bisagra entre el código (Conventional Commits, branching) y el contrato versionado de la librería (SemVer, deprecación). `geovial-storage` es una `library` que no se publica como paquete redistribuible (intake §13, §17.P.7): se integra al backend `geovial-api` y su versión se alinea al ciclo de ese backend (ADR-03). Por tanto no hay canales externos (preview/stable sobre un feed): la "publicación" es la del artefacto del backend. La estrategia consolida lo fijado en el intake §17.P.7 y en ADR-03 (`05_arquitectura_tecnica/adrs/`).

## 1. SemVer 2.0.0

Se adopta versionado semántico 2.0.0 con formato `MAJOR.MINOR.PATCH[-PRERELEASE][+BUILDMETADATA]`. La versión describe el contrato público de la librería (la superficie pública de ADR-02). La clasificación de compatibilidad está fijada en la especificación funcional 02 §6 y en ADR-03:

| Cambio en el contrato público | Bump SemVer |
| --- | --- |
| Agregar un proveedor nuevo, una operación nueva o un parámetro opcional con valor por defecto | MINOR |
| Corregir un defecto sin cambiar la semántica observable del contrato | PATCH |
| Cambiar la semántica de una operación, quitar una operación, quitar o renombrar un código de error, o volver obligatorio un parámetro antes opcional | MAJOR (coordinar con `geovial-api`) |

El identificador lógico emitido por CU-01 conserva su significado a través de versiones menores y patches (ADR-03; CU-01). Un cambio MAJOR del contrato obliga a coordinación explícita con el consumidor `geovial-api` antes de mergear (DoD-release de 08).

## 2. Conventional Commits 1.0.0

Cada cambio se rotula con Conventional Commits 1.0.0 para que el cálculo de versión sea determinista y la clasificación mayor/menor sea revisable (ADR-03, métrica "100 % de los cambios del contrato clasificados").

| Prefijo | Efecto en la versión | Ejemplo |
| --- | --- | --- |
| `feat` | MINOR | `feat(adapters): agregar adaptador de proveedor de objetos remoto` |
| `fix` | PATCH | `fix(routing): normalizar error transitorio del proveedor` |
| `feat!` o `BREAKING CHANGE` en footer | MAJOR | `feat(contract)!: renombrar codigo de error de recurso ausente` |
| `refactor`, `perf`, `test`, `chore`, `docs`, `style`, `build`, `ci` | Ninguno | `test(contract): cubrir paginacion por testigo` |

El marcador `!` o el footer `BREAKING CHANGE` es obligatorio para todo cambio que la clasificación de §1 considere mayor. Un cambio mayor sin marcador es un defecto de proceso y bloquea el merge en revisión de PR.

## 3. Herramienta de versionado

La versión se deriva del tag del repositorio mediante una herramienta de cálculo de versión a partir de tags Git, alineada al ciclo del backend (intake §17.P.7: "versión derivada del tag con GitVersion, alineada al ciclo del backend"; ADR-03). Configuración base:

- Prefijo de tag: `v` (por ejemplo `v1.4.0`).
- La línea de versión de la librería sigue la del backend que la integra: el tag de release del backend marca también la versión efectiva de la librería embebida (ADR-03 §2).
- Los commits sin tag entre releases producen una versión de desarrollo derivada del último tag más la distancia de commits, sin publicación (no hay feed).
- No se calcula una versión independiente para un feed externo: la librería no tiene artefacto publicable propio.

## 4. Branching

Branching alineado al acuerdo de equipo (categoría 00) y a la cadencia de tramos del mini-plan de 07. Con `equipo_n=1` (intake §2) el flujo se mantiene simple:

- Trunk-based development con `main` protegida.
- Ramas `feature/<slug>` cortas; PR obligatorio con la suite verde antes de mergear.
- Merge por squash con mensaje Conventional Commits que define el bump.
- Reglas de protección de `main`: gates bloqueantes de merge de 08 §3 en verde (G-01, G-02, G-03, G-04, G-06, G-07, G-09); ningún push directo a `main`.

Nota: el acuerdo de equipo de la categoría 00 es la fuente del branching; si declara un flujo distinto, prevalece y este documento se ajusta en la siguiente versión.

## 5. Canales

La librería no tiene canales de distribución externos (preview/stable sobre un feed), porque no se publica como paquete redistribuible (anti-patrón "confundir publicación con despliegue" evitado, regla 09 §4.8). Su distribución es la del artefacto del backend que la integra:

| Canal lógico | Significado | Destino real |
| --- | --- | --- |
| Integrado al backend | Único canal: la librería viaja embebida en la imagen del backend | Imagen del backend `geovial-api` (ver `entornos-deploy_v1.0.md`) |

Los sufijos de prerelease (`-alpha.N`, `-beta.N`, `-rc.N`) los gobierna el ciclo de release del backend, no la librería. La librería los hereda cuando el tag del backend los lleva.

## 6. Deprecation policy

La deprecación de elementos del contrato público sigue ADR-03 §7 y 02 §6:

- Una operación o un código de error que se va a remover se marca como obsoleto en una versión MINOR antes de quitarse, sin removerlo todavía (compatibilidad hacia atrás dentro de la línea mayor).
- La remoción efectiva ocurre en la siguiente versión MAJOR, coordinada con `geovial-api`.
- Todo elemento obsoleto se anota en código (marca de obsolescencia del runtime objetivo) y se documenta en el CHANGELOG generado desde Conventional Commits.
- El CHANGELOG (Keep a Changelog) se publica con cada release del backend e identifica los cambios de la librería que afectan al contrato.

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Clasificación de compatibilidad | 02 §6; ADR-03 |
| SemVer y Conventional Commits | intake §17.P.7; ADR-03 |
| Herramienta de versión | intake §17.P.7 (derivada del tag, alineada al backend) |
| Branching | acuerdo de equipo de 00; mini-plan de 07 |
| Coordinación de cambio mayor | DoD-release de 08; `geovial-api` |
| Gates de merge | `estrategia-calidad_v1.0.md` §3 (G-01..G-09) |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Estrategia de versionado inicial de geovial-storage: SemVer 2.0.0 con clasificación mayor/menor alineada a 02 §6 y ADR-03, Conventional Commits 1.0.0, versión derivada del tag alineada al ciclo del backend, trunk-based con main protegida, un único canal lógico (integrado al backend) sin feed externo y deprecation policy de obsolescencia en MINOR antes de remover en MAJOR. |
