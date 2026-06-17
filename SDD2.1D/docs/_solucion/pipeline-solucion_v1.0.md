# Pipeline de solución — GeoVial

**Proyecto:** GeoVial (solución)
**Documento:** pipeline-solucion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps Senior (Release/Platform Engineering)

## 1. Objetivo y alcance

Este documento orquesta el build y la publicación multi-proyecto de GeoVial por encima del pipeline de cada uno de sus cinco proyectos. Fija el orden de construcción derivado del grafo de dependencias del manifiesto, la matriz de artefactos publicables por proyecto, la coordinación inter-proyecto, el versionado de la solución, el gate de integración, el rollback coordinado y la trazabilidad. Es obligatorio porque GeoVial tiene más de un proyecto (regla 09 §2.1, §4.9).

El documento referencia, no duplica. El detalle de stages internos de cada proyecto —lint, build, test, SCA, SBOM, firma, publish, promotion y rollback propio— vive en el `pipeline-ci-cd_v1.0.md` de ese proyecto, bajo `proyectos/<kebab>/09_devops/`. Aquí se define exclusivamente lo que ocurre entre proyectos: el orden en que se construyen y publican, cómo el consumidor obtiene el artefacto del productor, cómo se coordinan los bumps de versión, cómo se verifica la solución integrada antes de publicar y cómo se revierte en cadena. Cada artefacto publicable de la matriz remite a la `guia-publicacion-<tipo-artefacto>_v1.0.md` del proyecto que lo produce.

Insumos normativos: `SOLUTION-MANIFEST-geovial_v1.0.md` (grafo §3, tipos de artefacto §2, nombres de código §1.1), `vista-solucion_v1.0.md` (grafo y contratos), `ADR-02-versionado-inter-proyecto_v1.0.md` (orden de publicación y compatibilidad) y los `09_devops/` de los cinco proyectos.

## 2. Orden de construcción

El orden de construcción es el orden topológico del grafo de dependencias del manifiesto (`SOLUTION-MANIFEST-geovial_v1.0.md` §3): ningún proyecto se construye antes que aquel del que depende, y los paquetes redistribuibles se construyen y publican antes que los proyectos que los consumen. El grafo es un DAG verificado en el manifiesto (§4); cualquier ciclo sería un defecto que detendría la construcción.

```text
aplicada-sync   ─┐
geovial-storage ─┼─> geovial-api ─┬─> geovial-web
                 │                └─> geovial-mobile
aplicada-sync ───────────────────────> geovial-mobile
```

Tabla de niveles topológicos. Los proyectos del mismo nivel no dependen entre sí y se construyen en paralelo:

| Nivel | Proyectos del nivel | Paralelizables en el nivel | Dependencias que ya deben estar resueltas |
| --- | --- | --- | --- |
| 0 | `aplicada-sync`, `geovial-storage` | Sí (no dependen entre sí) | Ninguna (sin dependencias) |
| 1 | `geovial-api` | N/A (proyecto único del nivel) | `geovial-storage` (integrada al build del backend) |
| 2 | `geovial-web`, `geovial-mobile` | Sí (no dependen entre sí) | `geovial-api` (ambos); `aplicada-sync` (solo `geovial-mobile`) |

Notas de orden derivadas del grafo y de ADR-02:

- El redistribuible `aplicada-sync` (nivel 0, `redistribuible: true`) se construye y publica al feed antes que su único consumidor en la solución, `geovial-mobile` (nivel 2). Aunque ambos productores del nivel 0 son paralelizables entre sí, la publicación de `aplicada-sync` precede al build del nivel 2.
- `geovial-storage` (nivel 0) no se publica por separado: se construye e integra al artefacto del backend `geovial-api` (nivel 1) por build conjunto. Su disponibilidad para el backend es una precondición de build, no un paso de publicación.
- El nivel 2 se construye una vez que el nivel 1 publicó su artefacto y su contrato consumible. `geovial-web` y `geovial-mobile` se construyen en paralelo entre sí.

## 3. Matriz de build y publicación multi-proyecto

Por proyecto: su `project_type` (D8), el tipo de artefacto publicable, el canal o feed de publicación, la guía de publicación del productor y si su artefacto lo consume otro proyecto de la solución. Refleja la tabla §2.2 de la regla 09 aplicada a cada proyecto del manifiesto.

| Proyecto | `project_type` | Nivel | Tipo de artefacto publicable | Canal / feed de publicación | Guía de publicación del productor | ¿Lo consume otro proyecto de la solución? |
| --- | --- | --- | --- | --- | --- | --- |
| `aplicada-sync` | `library` | 0 | `paquete-nuget` (paquete redistribuible) | Feed de paquetes; canales `preview` / `stable` por sufijo de tag | `proyectos/aplicada-sync/09_devops/guia-publicacion-paquete-nuget_v1.0.md` | Sí: `geovial-mobile` lo consume por referencia de versión (C-04) |
| `geovial-storage` | `library` | 0 | Ninguno (librería integrada al backend; no se publica externamente) | Sin feed ni canal propio; viaja embebida en el artefacto de `geovial-api` | — (guía de publicación omitida: ver `proyectos/geovial-storage/09_devops/README.md`) | Sí: `geovial-api` la integra por build conjunto (C-01) |
| `geovial-api` | `rest-api` | 1 | `image-docker` (imagen de contenedor) + `openapi` (contrato versionado) | Registro de imágenes (DEV/QA/STAGING/PROD + canary); contrato al hub de contratos | `proyectos/geovial-api/09_devops/guia-publicacion-image-docker_v1.0.md`; `proyectos/geovial-api/09_devops/guia-publicacion-openapi_v1.0.md` | Sí: `geovial-web` y `geovial-mobile` consumen el contrato REST/OpenAPI (C-02, C-03) |
| `geovial-web` | `web-monolith` | 2 | `image-docker` (imagen de contenedor) | Registro de imágenes (DEV/QA/STAGING/PROD) | `proyectos/geovial-web/09_devops/guia-publicacion-image-docker_v1.0.md` | No (cliente hoja del grafo) |
| `geovial-mobile` | `mobile-app-maui` | 2 | `store-mobile` (paquete de aplicación) | Canal interno (`internal` / `alpha` / `beta` / `production`) | `proyectos/geovial-mobile/09_devops/guia-publicacion-store-mobile_v1.0.md` | No (cliente hoja del grafo) |

Lectura de la matriz: hay tres familias de artefacto publicable en la solución —un paquete redistribuible (nivel 0), un par imagen de contenedor más contrato versionado del backend (nivel 1), y dos artefactos de cliente, una imagen de contenedor y un paquete de aplicación por canal interno (nivel 2)—, más una librería integrada al backend que no se publica por separado. Los valores de tipo-artefacto (`paquete-nuget`, `image-docker`, `openapi`, `store-mobile`) aparecen solo en esta matriz, como nombres normalizados de la tabla §2.2.

## 4. Coordinación inter-proyecto

Por cada arista de dependencia del grafo, cómo el proyecto consumidor obtiene el artefacto del productor. La política la gobierna ADR-02 (el productor publica antes que el consumidor; compatibilidad hacia atrás por versión mayor).

| # | Arista (consumidor → productor) | Productor | Mecanismo de obtención del artefacto | Precondición de orden |
| --- | --- | --- | --- | --- |
| C-01 | `geovial-api → geovial-storage` | `geovial-storage` | Build conjunto en el repositorio: la librería de almacenamiento se integra al artefacto del backend; no se publica por separado ni se referencia por feed. El backend la incorpora a su imagen de contenedor durante su propio build. | `geovial-storage` (nivel 0) construida antes del build del backend (nivel 1) |
| C-02 | `geovial-web → geovial-api` | `geovial-api` | Referencia al contrato publicado: el front consume el contrato versionado del backend (publicado al hub de contratos) para generar su cliente y validar la integración. El artefacto desplegable del backend está disponible en su registro antes de levantar el front. | `geovial-api` (nivel 1) publica imagen y contrato antes del build del front (nivel 2) |
| C-03 | `geovial-mobile → geovial-api` | `geovial-api` | Referencia al contrato publicado: la app consume el mismo contrato versionado del backend, incluidos los endpoints de sincronización del cliente offline-first. | `geovial-api` (nivel 1) publica imagen y contrato antes del build de la app (nivel 2) |
| C-04 | `geovial-mobile → aplicada-sync` | `aplicada-sync` | Referencia al paquete publicado: la app referencia una versión del paquete redistribuible ya publicada al feed. El redistribuible se publica primero; si la app referenciara una versión aún no publicada, su build fallaría. | `aplicada-sync` (nivel 0) publicada y verificada post-publish antes del build de la app (nivel 2) |

Resumen de políticas de obtención:

- Build conjunto (C-01): el artefacto de almacenamiento se integra al backend por compilación en el mismo repositorio. No hay paso de publicación intermedio; la disponibilidad de la librería es una precondición de build del backend.
- Referencia al paquete publicado (C-04): el redistribuible se publica al feed primero y se verifica post-publish restaurándolo en un proyecto limpio (ADR-02 §7; verificación post-publish de `aplicada-sync`) antes de habilitar su consumo por la app. Esta es la coordinación más sensible de la solución: una referencia a versión inexistente rompe la construcción del consumidor.
- Referencia al contrato publicado (C-02, C-03): el backend publica imagen y contrato versionado; los dos clientes consumen el contrato fijando la versión mayor. El backend conserva la versión mayor previa del contrato durante al menos un MINOR para que los dos clientes migren de forma escalonada (ADR-02).

## 5. Versionado de la solución

GeoVial adopta versionado independiente por proyecto, no lockstep. Cada proyecto se versiona con SemVer 2.0.0 y Conventional Commits según su propio `estrategia-versionado_v1.0.md`; no existe una versión única que abarque a los cinco proyectos. Esta decisión la fija ADR-02: el versionado conjunto fue descartado porque obligaría a re-publicar y re-desplegar proyectos sin cambios y rompería la reutilización del redistribuible fuera de la solución.

Coordinación de bumps cuando un productor cambia:

- Cambio menor de un productor (nuevo proveedor, endpoint, operación u opción retrocompatible): el consumidor no requiere bump ni cambio de código; su construcción sigue verde contra la nueva versión del productor sin intervención. Es el caso esperado y mayoritario.
- Cambio mayor de un productor (incompatible): el productor publica la versión mayor nueva conservando la versión mayor previa durante un período de convivencia de al menos un MINOR. Cada consumidor migra de forma escalonada dentro de ese período y bumpea su propia versión cuando adopta la versión mayor nueva. No hay despliegue coordinado de toda la solución.
- Redistribuible `aplicada-sync`: su versionado no queda atado al calendario de release de GeoVial; conserva su valor de reutilización externa. Se publica al feed y se verifica antes de que la app referencie la nueva versión.
- `geovial-storage`: se versiona alineada al ciclo del backend porque se integra a él y no se distribuye; su bump viaja con el del backend en el build conjunto.

Orden de publicación de un release coordinado de la solución (ADR-02 §7): primero el redistribuible `aplicada-sync` (publicación al feed y verificación post-publish), luego `geovial-storage` integrada y la imagen y el contrato de `geovial-api`, y por último la imagen de `geovial-web` y el paquete de aplicación de `geovial-mobile`. El orden de publicación honra el mismo orden topológico que el de construcción.

## 6. Gate de integración de solución

Antes de publicar la solución como conjunto se ejecuta un gate de integración que verifica end-to-end que los proyectos integrados funcionan juntos con los artefactos reales producidos por sus pipelines, no con mocks. Es un gate de nivel solución, complementario y posterior a los quality gates internos de cada `pipeline-ci-cd`; no los redefine.

Procedimiento del gate:

1. Reunir los artefactos del release candidato en el orden topológico ya construido: el paquete redistribuible `aplicada-sync` publicado y verificado en el feed, el backend `geovial-api` con `geovial-storage` integrada (imagen de contenedor) y su contrato versionado publicado, y los artefactos de cliente (`geovial-web` y `geovial-mobile`).
2. Levantar la solución integrada en un ambiente de pruebas: el backend con su almacenamiento integrado operativo, el front apuntando al contrato del backend y la app consumiendo el contrato REST y el paquete de sincronización.
3. Smoke test de la solución levantada sobre las fronteras de los cuatro contratos: el front opera contra el contrato REST (C-02); la app captura y sincroniza ejercitando el ciclo subir-luego-bajar a través del paquete de sincronización (C-04) que compone los endpoints de sincronización del backend (C-03); el backend aloja y recupera por la abstracción de almacenamiento integrada (C-01).
4. Verificación de la cadena de publicación: el redistribuible se restaura en un proyecto limpio y la app construye contra la versión publicada; ningún consumidor referencia una versión ausente en su feed o hub.

El gate es bloqueante: si el smoke test de la solución integrada falla en cualquier frontera, o si un consumidor no puede obtener el artefacto de su productor en el canal declarado, la publicación de la solución se detiene. Solo con el gate en verde se promueve el conjunto a publicación.

## 7. Rollback coordinado

El rollback de la solución se ejecuta en orden inverso al de construcción: primero los consumidores (nivel 2), luego el backend (nivel 1), por último las librerías de soporte (nivel 0). Cada productor revierte a su versión previa sin obligar a revertir a sus consumidores mientras la versión mayor de su contrato no cambie (ADR-02 §7); el rollback interno de cada artefacto se ejecuta por el procedimiento de su propio `pipeline-ci-cd` (revertir tag o deploy, retirar del feed, re-desplegar la versión previa, etc.).

| Orden de rollback | Nivel | Proyecto | Acción de reversión (referencia al procedimiento del proyecto) |
| --- | --- | --- | --- |
| 1.º | 2 | `geovial-web`, `geovial-mobile` | Re-desplegar la imagen de contenedor previa del front; redistribuir el paquete de aplicación previo por el canal interno |
| 2.º | 1 | `geovial-api` | Re-desplegar la imagen de contenedor previa del backend (con su almacenamiento integrado) y republicar la versión previa del contrato versionado |
| 3.º | 0 | `aplicada-sync` | Retirar del feed la versión rota del paquete redistribuible y dejar vigente la versión previa |
| 3.º | 0 | `geovial-storage` | No tiene rollback propio: se revierte por la reversión de la imagen del backend que la integra |

Manejo de un artefacto compartido roto:

- Si el redistribuible `aplicada-sync` publica una versión rota, se retira del feed dentro de la ventana de gracia y la app permanece o vuelve a la versión previa por su referencia fijada; al ser el único consumidor en la solución, el alcance del impacto está acotado a `geovial-mobile`.
- Si el contrato del backend (`geovial-api`) introduce una rotura que afecta a sus dos consumidores a la vez (`geovial-web` y `geovial-mobile`), la mitigación es la convivencia de la versión mayor previa: los clientes vuelven a consumir la versión mayor anterior, aún disponible durante el período de convivencia, mientras el backend revierte. No se requiere revertir a los dos clientes en bloque si la versión mayor previa sigue vigente.
- Si la librería de almacenamiento (`geovial-storage`) introduce una rotura, el rollback es la reversión de la imagen del backend que la integra; no hay artefacto separado que retirar.

## 8. Trazabilidad

El orden de build de §2 se liga a las dependencias del manifiesto, y cada artefacto publicable de §3 se liga a la `guia-publicacion-<tipo-artefacto>` del proyecto que lo produce. No existe paso de orden que contradiga el grafo, ni artefacto publicable sin su guía de publicación (regla 09 §3.3, §6 nivel solución).

| Artefacto publicable | Proyecto productor | Arista(s) del manifiesto servidas | Nivel topológico | Guía de publicación del productor |
| --- | --- | --- | --- | --- |
| `paquete-nuget` (redistribuible) | `aplicada-sync` | C-04 (`geovial-mobile → aplicada-sync`) | 0 | `proyectos/aplicada-sync/09_devops/guia-publicacion-paquete-nuget_v1.0.md` |
| Librería integrada al backend (sin publicación externa) | `geovial-storage` | C-01 (`geovial-api → geovial-storage`) | 0 | Omitida (ver `proyectos/geovial-storage/09_devops/README.md`); la publicación efectiva es la del backend |
| `image-docker` (imagen de contenedor) | `geovial-api` | C-02, C-03 (consumen el backend) | 1 | `proyectos/geovial-api/09_devops/guia-publicacion-image-docker_v1.0.md` |
| `openapi` (contrato versionado) | `geovial-api` | C-02, C-03 (consumen el contrato) | 1 | `proyectos/geovial-api/09_devops/guia-publicacion-openapi_v1.0.md` |
| `image-docker` (imagen de contenedor) | `geovial-web` | — (cliente hoja) | 2 | `proyectos/geovial-web/09_devops/guia-publicacion-image-docker_v1.0.md` |
| `store-mobile` (paquete de aplicación) | `geovial-mobile` | — (cliente hoja) | 2 | `proyectos/geovial-mobile/09_devops/guia-publicacion-store-mobile_v1.0.md` |

Correspondencias verificadas:

- Orden de build ↔ dependencias del manifiesto: cada nivel topológico de §2 corresponde uno a uno con el orden topológico del manifiesto §3 (nivel 0 antes que nivel 1 antes que nivel 2); ningún proyecto se construye o publica antes que aquel del que depende, y el redistribuible se publica antes que su consumidor.
- Artefacto publicable ↔ guía de publicación: cada fila con artefacto publicable referencia la `guia-publicacion-<tipo-artefacto>` del proyecto productor; la única omisión registrada es la de `geovial-storage`, cuya publicación efectiva es la del backend que la integra (regla 09 §2.1, columna "Omitir para").
- Coordinación inter-proyecto ↔ contratos: las cuatro aristas C-01..C-04 de §4 corresponden a las cuatro aristas del grafo del manifiesto y a los cuatro contratos de la vista de solución (`vista-solucion_v1.0.md` §4).

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Pipeline de solución inicial de GeoVial con las ocho secciones de la regla 09 §4.9: objetivo y alcance (referencia, no duplica, los pipeline-ci-cd de proyecto), orden de construcción en tres niveles topológicos derivado del grafo del manifiesto con paralelizables por nivel, matriz de build y publicación multi-proyecto por proyecto (project_type, tipo de artefacto, canal/feed, consumo intra-solución), coordinación inter-proyecto por arista (build conjunto del almacenamiento, referencia al paquete publicado del redistribuible, referencia al contrato del backend), versionado independiente por proyecto con coordinación de bumps por versión mayor (ADR-02), gate de integración end-to-end de la solución levantada antes de publicar, rollback coordinado en orden inverso con manejo del artefacto compartido roto, y trazabilidad orden de build ↔ manifiesto y artefacto publicable ↔ guia-publicacion del productor. |
