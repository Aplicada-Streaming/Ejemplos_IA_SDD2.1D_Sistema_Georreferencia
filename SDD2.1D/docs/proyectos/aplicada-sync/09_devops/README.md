# 09 DevOps — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** README.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps Senior (AG-09), variante DevOps + Release Engineer (library)

Índice navegable de los artefactos DevOps del motor de sincronización `Aplicada.Sync` (tipo `library`, redistribuible). El artefacto que el pipeline produce y publica es un paquete redistribuible al feed declarado (GitHub Packages, intake §17 P.7). El modelo de distribución son canales `preview` y `stable` sobre el feed, no ambientes desplegables DEV/QA/STAGING/PROD: la librería corre embebida en el host, no es un servicio.

## Artefactos vigentes

| Documento | Propósito | Estado |
| --- | --- | --- |
| [estrategia-versionado_v1.0.md](estrategia-versionado_v1.0.md) | SemVer 2.0.0, Conventional Commits, GitVersion, branching trunk-based, canales y deprecation policy. Documento bisagra entre código y artefacto | Propuesto |
| [pipeline-ci-cd_v1.0.md](pipeline-ci-cd_v1.0.md) | Stages (lint, build, test, cobertura, mutation, NFR, SCA, SBOM, análisis estático, compatibilidad, firma, publish, post-publish), matriz SO/runtime, caché, promotion preview->stable, rollback y notificaciones | Propuesto |
| [entornos-deploy_v1.0.md](entornos-deploy_v1.0.md) | Canales preview/stable sobre el feed, configuración 12-factor, secretos y promoción. Modelo de canales, no de ambientes | Propuesto |
| [guia-publicacion-paquete-nuget_v1.0.md](guia-publicacion-paquete-nuget_v1.0.md) | Pre-requisitos, comando/stage de publicación, verificación post-publish, rollback por unlist y métricas del paquete redistribuible | Propuesto |
| [supply-chain-seguridad_v1.0.md](supply-chain-seguridad_v1.0.md) | SBOM CycloneDX, firma con transparency log, SLSA L3 objetivo, dependency scanning, SAST (DAST no aplica) y política de CVE | Propuesto |

## Orden de lectura sugerido

1. Acuerdo de equipo (categoría 00, branching trunk-based).
2. [estrategia-versionado_v1.0.md](estrategia-versionado_v1.0.md) — define versión, commits y canales.
3. [pipeline-ci-cd_v1.0.md](pipeline-ci-cd_v1.0.md) — los gates G1-G9 como stages.
4. [entornos-deploy_v1.0.md](entornos-deploy_v1.0.md) — canales y secretos.
5. [guia-publicacion-paquete-nuget_v1.0.md](guia-publicacion-paquete-nuget_v1.0.md) — cómo publicar y revertir.
6. [supply-chain-seguridad_v1.0.md](supply-chain-seguridad_v1.0.md) — SBOM, firma, SLSA, CVE.

## Quality gates ejecutados como stages

Los gates G1 a G9 los define `08_calidad_y_pruebas/estrategia-calidad_v1.0.md` §3; el pipeline los ejecuta como stages sin redefinirlos (`pipeline-ci-cd_v1.0.md` §3). La Definition of Done canónica vive en `08_calidad_y_pruebas/definition-of-done_v1.0.md` y se ejecuta como gates; este pipeline no la redefine.

| Gate | Stage del pipeline | Criterio DoD / NFR que verifica |
| --- | --- | --- |
| G1 | Build | DoD BT §1.2 (compila sin advertencias tratadas como error) |
| G2 | Test unit | DoD US §1.1 (suite unitaria verde) |
| G3 | Test contract | DoD BT §1.2 (contrato de extensión no rompe consumidores) |
| G4 | Cobertura | DoD US §1.1 y release §1.4; intake §17 P.6 (cobertura por capa y global) |
| G5 | Mutation | DoD tramo §1.3 (mutation score dominio >= 60 %) |
| G6 | Test unit (property-based) | DoD US §1.1 (invariantes orden/idempotencia/no duplicación) |
| G7 | NFR | NFR de 05 §8 (lote, cola, reanudación, idempotencia, orden, continuidad) |
| G8 | Compatibilidad + verificación post-publish | DoD release §1.4; ADR-03; intake §17 P.8 |
| G9 | Análisis estático | DoD US §1.1 y release §1.4 (sin issues críticos) |

## Modelo de distribución

Canales sobre el feed (no ambientes desplegables, regla §2.2 para `library`):

| Canal | Tags que lo alimentan | Aprobador |
| --- | --- | --- |
| `preview` | `vX.Y.Z-alpha.N`/`-beta.N`/`-rc.N` | Automático |
| `stable` | `vX.Y.Z` sin sufijo | Release manager (AG-07) |

## Nivel solución

Este README es de nivel proyecto. La orquestación de build y publicación multi-proyecto de la solución GeoVial (orden topológico del manifiesto, matriz de artefactos, coordinación inter-proyecto) corresponde a `_solucion/pipeline-solucion_v1.0.md` (09_rules §4.9), que se genera una vez al cierre del bucle de proyectos; no es parte de esta carpeta.

## Trazabilidad upstream/downstream

- Upstream: 05 (`arquitectura-solucion_v1.0.md` §5/§7/§8 NFR; ADR-03; `contratos-abstractions_v1.0.md` §6), 08 (`estrategia-calidad_v1.0.md` §3 gates G1-G9; `estrategia-testing_v1.0.md` §2 cobertura; `definition-of-done_v1.0.md`), intake §17 P.6/P.7/P.8.
- Downstream: 10 (developer guide cita los comandos exactos del pipeline para reproducción local), 11 (los samples, incluido el demo MAUI de `aplicada-sync`, consumen los canales declarados en `entornos-deploy_v1.0.md`).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 09 de aplicada-sync con el índice de los cinco artefactos obligatorios (estrategia de versionado, pipeline CI/CD, entornos/distribución por canales, guía de publicación del paquete redistribuible y supply chain), el orden de lectura, el mapeo de los gates G1-G9 a stages y a su criterio DoD/NFR de origen, el modelo de canales preview/stable y la trazabilidad upstream/downstream. |
