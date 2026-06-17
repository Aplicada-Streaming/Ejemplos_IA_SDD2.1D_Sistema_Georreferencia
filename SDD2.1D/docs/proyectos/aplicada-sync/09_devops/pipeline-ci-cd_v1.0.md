# Pipeline CI/CD — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** pipeline-ci-cd_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps Senior (AG-09), variante DevOps + Release Engineer (library)

## 1. Objetivo y alcance

Este documento define el pipeline de integración y entrega continua del paquete redistribuible `Aplicada.Sync`. El artefacto que produce y publica el pipeline es un paquete del gestor del runtime, publicado al feed declarado en el intake §17 P.7 (GitHub Packages como feed inicial, ratificable). No hay servicio ni ambiente desplegable: el modelo de promoción es por canales `preview` y `stable` sobre el feed, no por ambientes DEV/QA/STAGING/PROD (regla §2.2 para `library`; ver `entornos-deploy_v1.0.md`).

Los gates de prueba del pipeline ejecutan la Definition of Done de `08_calidad_y_pruebas/definition-of-done_v1.0.md` y la cobertura por capa de `08_calidad_y_pruebas/estrategia-testing_v1.0.md` §2; este documento no las redefine. Los gates G1 a G9 referenciados son los de `08_calidad_y_pruebas/estrategia-calidad_v1.0.md` §3 y se materializan acá como stages. Cada gate referencia el criterio DoD o el NFR que verifica.

## 2. Triggers

| Trigger | Evento | Stages que ejecuta | Publica |
| --- | --- | --- | --- |
| PR a `main` | Apertura o actualización de un PR | Lint, Build, Test unit, Test contract, Test snapshot, SCA, Análisis estático | No |
| Push a `main` | Merge de un PR (squash con Conventional Commits) | Todos los de PR + SBOM | No |
| Tag `vX.Y.Z-alpha.N` / `-beta.N` / `-rc.N` | Tag de prerelease generado por la herramienta de versión | Todos + NFR (G7) + Mutation (G5) + Firma + Publish preview | Canal `preview` |
| Tag `vX.Y.Z` sin sufijo | Tag estable | Todos + NFR (G7) + Mutation (G5) + Firma + Publish stable + Verificación post-publish | Canal `stable` |
| Schedule semanal | Cron de mantenimiento | SCA, dependency scanning, regeneración de aviso de CVE | No |

Triggers explícitos por evento (anti-patrón 4.8 "trigger único y opaco" evitado): el PR no publica nada; solo los tags publican, y el sufijo del tag decide el canal (ver §4).

## 3. Stages obligatorios

Cada stage declara su comando abstracto, su tooling por rol y su criterio de éxito ligado a un gate de 08, a un criterio DoD o a un NFR de 05.

| Stage | Tooling (rol abstracto) | Quality gate | Criterio DoD / NFR verificado | Bloqueante |
| --- | --- | --- | --- | --- |
| Lint | Formateador y linter del runtime objetivo | 0 issues de formato nuevos | DoD BT §1.2 "compila sin advertencias tratadas como error" (parcial) | Sí en PR |
| Build | Compilador del runtime objetivo | G1: compila sin advertencias tratadas como error | DoD BT §1.2; DoD US §1.1 (suite compila) | Sí |
| Test unit | Framework de tests unitarios; framework de property-based para invariantes | G2 (suite unitaria verde, ningún test sin assert) y G6 (property-based de orden, idempotencia, no duplicación) | DoD US §1.1 (suite unitaria verde); DoD US §1.1 (property-based de invariantes) | Sí |
| Test contract | Framework de tests unitarios sobre dobles de contrato | G3: contract tests por interfaz de extensión (almacén local, transporte, credencial, conectividad) verdes | DoD BT §1.2 (contrato de infraestructura compartida no rompe consumidores) | Sí |
| Test snapshot | Framework de snapshot testing | Forma estable del resumen de ciclo, resumen de reanudación, estado y catálogo de errores | DoD release §1.4 (snapshot del contrato sin diferencias no justificadas) | Sí |
| Cobertura | Herramienta de cobertura del runtime con reporte por capa | G4: dominio >= 85 % líneas / >= 80 % branches; infraestructura >= 70 % / >= 60 %; global >= 80 % / >= 70 % (intake §17 P.6) | DoD US §1.1 y DoD release §1.4 (cobertura por capa y global) | Sí |
| Mutation | Framework de mutation testing | G5: mutation score del dominio >= 60 % | DoD tramo §1.3 (mutation score del dominio >= 60 %) | Advierte en merge; bloquea release |
| NFR | Cliente de benchmark/carga con backend de prueba que simula latencia móvil | G7: lote de 100 <= 30 s; cola >= 1000 sin degradación; 0 perdidos / 0 duplicados en reanudación | NFR Tiempo de lote, Capacidad de cola, Reanudación (05 §8); DoD release §1.4 | Bloquea release |
| SCA | Herramienta de software composition analysis del runtime | 0 CVE críticas, 0 altas sin excepción registrada | Política de CVE (ver `supply-chain-seguridad_v1.0.md` §6) | Sí |
| SBOM | Generador CycloneDX o SPDX | SBOM JSON generado, firmado y adjunto al release | DoD release §1.4 (artefacto trazable); supply chain §1 | Sí en release |
| Análisis estático | Analizador estático del runtime | G9: sin issues críticos | DoD US §1.1 (sin catch silencioso, análisis estático); DoD release §1.4 | Sí |
| Compatibilidad | Comparación contra la matriz de compatibilidad de la superficie pública | G8: ningún cambio incompatible sin incremento de versión mayor | DoD US §1.1 (cambio de superficie pública clasificado, ADR-03); DoD release §1.4 | Sí en publicación |
| Firma | Firma de artefacto (sigstore/cosign u homólogo del runtime) y firma del SBOM | Firma válida y registrada en transparency log | Supply chain §2; DoD release §1.4 | Sí en release |
| Publish preview | Comando de publicación del gestor de paquetes al feed, canal `preview` | Paquete disponible en canal `preview` | Ver `guia-publicacion-paquete-nuget_v1.0.md` §2 | Solo en tag `-alpha`/`-beta`/`-rc` |
| Publish stable | Comando de publicación al feed, canal `stable` | Paquete disponible en canal `stable` | Ver `guia-publicacion-paquete-nuget_v1.0.md` §2 | Solo en tag `vX.Y.Z` |
| Verificación post-publish | Restauración del paquete en un proyecto limpio y reproducción del quick-start | G8: el quick-start reproduce el contrato en proyecto limpio | DoD release §1.4 (verificación post-publicación, BT-14, intake §17 P.8); contratos §6 | Sí en stable |

Mapa gate -> stage para no duplicar la definición de 08: G1=Build, G2=Test unit, G3=Test contract, G4=Cobertura, G5=Mutation, G6=Test unit (property-based), G7=NFR, G8=Compatibilidad + Verificación post-publish, G9=Análisis estático. La condición y la consecuencia de cada gate viven en 08; acá solo se declara en qué stage se ejecuta.

## 4. Matriz de SO y runtime

El target único de la librería es Android, alineado con `geovial-mobile` y con el intake §17 P.9 (net8.0-android sobre .NET 8 LTS; sin iOS ni Windows en v1).

| Trigger | Sistemas operativos del runner | Runtime / target framework | Justificación |
| --- | --- | --- | --- |
| PR a `main` | linux | .NET 8 LTS; compilación de `net8.0` para la suite y `net8.0-android` para el empaquetado | Linux cubre build y suite a menor costo de minutos; el núcleo del motor no depende del SO de build |
| Push a `main` | linux | igual que PR + SBOM | Mismo runner; agrega inventario de dependencias |
| Tag preview / stable | linux | .NET 8 LTS; empaquetado `net8.0-android` con workload MAUI/Android | El paquete redistribuible se compila contra el target real del consumidor (MAUI Android) |

Justificación de la matriz: la cobertura de consumidores reales es un único target (`net8.0-android`); una matriz cruzada de SO o de múltiples runtimes no aporta valor frente al costo de minutos de CI mientras el alcance sea Android únicamente. Si una versión futura agrega iOS o Windows, la matriz se amplía con un TFM por plataforma y se versiona este documento.

## 5. Caché y artefactos

Política de caché del gestor de dependencias del runtime:

| Caché | Llave | Expiración | Notas |
| --- | --- | --- | --- |
| Paquetes restaurados del gestor | Hash del archivo de lock de dependencias | 7 días o invalidación por cambio del lock | Acelera la restauración; se reconstruye al cambiar dependencias |
| Workload del runtime (MAUI/Android) | Versión del SDK + versión del workload | Por versión del SDK | Evita reinstalar el workload Android en cada corrida |
| Artefactos intermedios de build | Hash del árbol de fuentes del proyecto | Por corrida | Reuso entre stages de la misma corrida |

Artefactos producidos por stage y su retención:

| Artefacto | Stage productor | Retención |
| --- | --- | --- |
| Reporte de cobertura por capa | Cobertura | 90 días |
| Reporte de mutation score | Mutation | 90 días |
| Reporte de NFR (tiempos de lote, cola, reanudación) | NFR | 90 días |
| Reporte de SCA y de dependency scanning | SCA / Schedule | 180 días |
| SBOM (CycloneDX/SPDX JSON, firmado) | SBOM | Permanente, adjunto al release |
| Paquete del gestor (`.nupkg`) firmado | Build + Firma | Permanente en el feed; copia de artefacto 180 días |
| Snapshot del contrato público (baselines) | Test snapshot | Versionado con el código (no como artefacto efímero) |
| Log de firma y referencia al transparency log | Firma | Permanente, adjunto al release |

## 6. Promotion rules (canales preview -> stable)

El modelo de promoción es por canales sobre el feed, no por ambientes (regla §2.2 para `library`). Detalle de canales en `entornos-deploy_v1.0.md`.

- `preview` se publica automáticamente desde tags con sufijo `-alpha.N`, `-beta.N` o `-rc.N`, generados por la herramienta de versión a partir de Conventional Commits (ver `estrategia-versionado_v1.0.md` §3). Aprobador: automático. Prerrequisito: G1-G6 y G9 verdes, SBOM presente y firma válida.
- `stable` se publica desde tags `vX.Y.Z` sin sufijo. Aprobador: Release manager (rol del Maintainer Lead, AG-07, según RACI de 08 §4). Prerrequisito: además de los gates de preview, G5 (mutation), G7 (NFR), G8 (compatibilidad) verdes y verificación post-publish exitosa.
- La transición de `preview` a `stable` no es una repromoción del mismo binario: cada canal se alimenta de su propio tag. Un `-rc.N` validado en `preview` precede a un `vX.Y.Z` que reusa el mismo árbol de fuentes y vuelve a pasar la suite antes de publicar a `stable`.
- Gate humano en `stable`: la aprobación del Release manager queda registrada de forma auditable (run del pipeline ligado al tag y al aprobador), cumpliendo el anti-patrón 4.8 "promotion sin aprobador humano".

## 7. Rollback

El artefacto es un paquete inmutable en un feed; no se borra una versión publicada (los consumidores que ya la restauraron la conservan). El rollback se ejecuta por `unlist` de la versión afectada más publicación de un fix, según el intake §17 P.8.

| Paso | Comando o acción | Responsable |
| --- | --- | --- |
| 1. Detectar regresión | Alerta de NFR, CVE o reporte de consumidor; ver métricas en `guia-publicacion-paquete-nuget_v1.0.md` §5 | Release manager |
| 2. Unlist de la versión rota | Comando de `unlist` del gestor de paquetes contra el feed para la versión `X.Y.Z` afectada; la versión deja de resolverse para nuevas restauraciones pero sigue disponible para quien la fijó | Release manager |
| 3. Publicar fix | PATCH `X.Y.(Z+1)` con el arreglo si el defecto es retrocompatible; MAJOR + guía de migración si el fix es incompatible (ADR-03) | AG-09 |
| 4. Comunicar | Entrada en CHANGELOG (Keep a Changelog), release notes y aviso a consumidores; ver deprecation policy en `estrategia-versionado_v1.0.md` §6 | AG-09 |
| 5. Regresión | TC de regresión que reproduzca el defecto antes del fix (08 estrategia-calidad §5; 08_rules §5.4) | AG-08 |

El procedimiento de unlist es el único rollback aplicable a un paquete redistribuible; está documentado con comando concreto en `guia-publicacion-paquete-nuget_v1.0.md` §4 y se ensaya al menos una vez antes del primer release `stable`.

## 8. Notificaciones

| Evento | Canal | Severidad | Destinatario |
| --- | --- | --- | --- |
| PR con gates en rojo | Comentario en el PR + canal del equipo | Media | Autor del PR (AG-09 en equipo_n=1) |
| Push a `main` con build roto | Canal del equipo | Alta | Release manager |
| Publicación a `preview` exitosa | Canal del equipo | Informativa | Equipo |
| Publicación a `stable` exitosa | Canal del equipo + release notes | Informativa | Equipo y consumidores |
| Falla de publicación o de verificación post-publish | Canal del equipo + escalamiento | Crítica | Release manager |
| CVE crítica o alta detectada (SCA o schedule) | Canal del equipo + aviso a consumidores | Crítica | Release manager; política en supply chain §6 |

Con `equipo_n=1` (intake §13; 08 estrategia-calidad §4), los roles de autor, Release manager y QA los ejerce la misma persona en momentos distintos; el pipeline y el registro auditable del aprobador sustituyen la revisión por pares ausente (08 §4).

## 9. Reproducibilidad local

Todos los stages son reproducibles en máquina local con las mismas versiones del runtime (anti-patrón 4.8 "pipeline irreproducible localmente" evitado). Los comandos exactos del pipeline para reproducción local los cita la developer guide de la categoría 10; este documento es su fuente. El build usa el SDK fijado (.NET 8 LTS) y el lock de dependencias versionado, de modo que la corrida local y la de CI restauran el mismo grafo.

## 10. Trazabilidad

| Stage / gate | Verifica | Fuente |
| --- | --- | --- |
| Build (G1) | Compila sin advertencias | DoD BT §1.2; 08 estrategia-calidad §3 |
| Test unit (G2, G6) | Suite unitaria y property-based verdes | DoD US §1.1; RN-01/02/03 |
| Test contract (G3) | Contratos de extensión verdes | DoD BT §1.2; contratos-abstractions de 05 |
| Cobertura (G4) | Cobertura por capa y global | DoD US §1.1, release §1.4; intake §17 P.6 |
| Mutation (G5) | Mutation score dominio >= 60 % | DoD tramo §1.3 |
| NFR (G7) | Lote, cola, reanudación | NFR de 05 §8; DoD release §1.4 |
| SCA / SBOM / Firma | Cadena de suministro | `supply-chain-seguridad_v1.0.md` |
| Análisis estático (G9) | Sin issues críticos | DoD US §1.1, release §1.4 |
| Compatibilidad + post-publish (G8) | Superficie pública y reproducción del contrato | DoD release §1.4; ADR-03; intake §17 P.8 |
| Publish preview/stable | Artefacto en el canal | `guia-publicacion-paquete-nuget_v1.0.md`; `entornos-deploy_v1.0.md` |

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Pipeline CI/CD inicial de aplicada-sync (library, redistribuible): triggers por evento (PR, push, tag preview/stable, schedule), stages obligatorios (lint, build, test unit/contract/snapshot, cobertura, mutation, NFR, SCA, SBOM, análisis estático, compatibilidad, firma, publish, verificación post-publish), matriz de SO/runtime acotada a net8.0-android sobre .NET 8 LTS, caché y artefactos con retención, promotion preview->stable con aprobador humano en stable, rollback por unlist y notificaciones. Los gates G1-G9 se materializan como stages sin redefinir la DoD ni la cobertura de 08; cada gate referencia su criterio DoD de 08 o su NFR de 05. Derivado de 08 (estrategia-calidad §3, estrategia-testing §2, definition-of-done), de 05 (§8 quality attributes) y del intake §17 P.6/P.7/P.8. |
