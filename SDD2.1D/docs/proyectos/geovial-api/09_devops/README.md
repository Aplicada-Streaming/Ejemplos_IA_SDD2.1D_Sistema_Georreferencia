# 09 DevOps — geovial-api

**Proyecto:** geovial-api
**Documento:** README.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps + Platform Engineer

Índice navegable de los artefactos DevOps de `geovial-api`, el backend monolítico (`rest-api`) y proyecto principal de la solución GeoVial. El pipeline produce y publica dos artefactos: la imagen de contenedor del backend (artefacto desplegable) y el contrato OpenAPI versionado (artefacto que consumen `geovial-web` y `geovial-mobile`). El modelo de promoción son ambientes de servicio desplegable DEV/QA/STAGING/PROD con despliegue canary y rollback rápido por desvío de tráfico, no canales de paquete (regla §2.2 para `rest-api`).

## Artefactos vigentes

| Documento | Propósito | Estado |
| --- | --- | --- |
| [estrategia-versionado_v1.0.md](estrategia-versionado_v1.0.md) | SemVer 2.0.0, Conventional Commits, GitVersion, branching trunk-based, ambientes y deprecation policy coordinada con el versionado del contrato por URI. Documento bisagra entre código y artefacto | Propuesto |
| [pipeline-ci-cd_v1.0.md](pipeline-ci-cd_v1.0.md) | Stages (lint, build, test unit/integración/contract, validación de contrato, cobertura, SCA, análisis estático/SAST, SBOM, build de imagen, firma, publish OpenAPI e imagen, regresión, NFR, deploy), matriz de runtime, caché, promotion DEV→QA→STAGING→PROD con canary, rollback por desvío de tráfico y notificaciones | Propuesto |
| [entornos-deploy_v1.0.md](entornos-deploy_v1.0.md) | Ambientes DEV/QA/STAGING/PROD con canary, IaC, configuración 12-factor, secretos en vault y promoción con aprobador. Modelo de servicio desplegable, no de canales de paquete | Propuesto |
| [guia-publicacion-image-docker_v1.0.md](guia-publicacion-image-docker_v1.0.md) | Publicación de la imagen de contenedor: pre-requisitos, stage, verificación post-publish, rollback por desvío de tráfico y métricas | Propuesto |
| [guia-publicacion-openapi_v1.0.md](guia-publicacion-openapi_v1.0.md) | Publicación del contrato OpenAPI versionado al hub de contratos: pre-requisitos, stage, verificación, versionado por URI y compatibilidad | Propuesto |
| [supply-chain-seguridad_v1.0.md](supply-chain-seguridad_v1.0.md) | SBOM CycloneDX, firma con transparency log, SLSA L3 objetivo, dependency scanning, SAST y DAST, política de CVE y secretos del backend en vault | Propuesto |

## Orden de lectura sugerido

1. Acuerdo de equipo (categoría 00, branching trunk-based).
2. [estrategia-versionado_v1.0.md](estrategia-versionado_v1.0.md) — define versión, commits y promoción por ambientes.
3. [pipeline-ci-cd_v1.0.md](pipeline-ci-cd_v1.0.md) — los gates G1-G8 como stages.
4. [entornos-deploy_v1.0.md](entornos-deploy_v1.0.md) — ambientes, canary y secretos.
5. [guia-publicacion-image-docker_v1.0.md](guia-publicacion-image-docker_v1.0.md) — cómo publicar y revertir la imagen.
6. [guia-publicacion-openapi_v1.0.md](guia-publicacion-openapi_v1.0.md) — cómo publicar y versionar el contrato.
7. [supply-chain-seguridad_v1.0.md](supply-chain-seguridad_v1.0.md) — SBOM, firma, SLSA, SAST/DAST, CVE.

## Quality gates ejecutados como stages

Los gates G1 a G8 los define `08_calidad_y_pruebas/estrategia-calidad_v1.0.md` §3; el pipeline los ejecuta como stages sin redefinirlos (`pipeline-ci-cd_v1.0.md` §3). La Definition of Done canónica vive en `08_calidad_y_pruebas/definition-of-done_v1.0.md` y se ejecuta como gates; este pipeline no la redefine.

| Gate | Stage del pipeline | Criterio DoD / NFR que verifica |
| --- | --- | --- |
| G1 | Build | DoD BT §1.2 (compila sin advertencias tratadas como error) |
| G2 | Test unit + Test integración | DoD US §1.1 (suite verde, criterios Given/When/Then); RN-01 a RN-07; RC-03/04/05 |
| G3 | Cobertura | DoD US §1.1 y release §1.4; intake §17 P.6 (cobertura por capa y global) |
| G4 | Test contract | DoD US §1.1 y release §1.4 (100 % de los 35 endpoints con contract test) |
| G5 | Validación de contrato | DoD release §1.4 (OpenAPI valida contra implementación); CU-22 |
| G6 | Análisis estático (SAST) | DoD US §1.1 y BT §1.2 (sin issues críticos) |
| G7 | Regresión | DoD release §1.4 y criterios-validacion §4 (sin regresión injustificada) |
| G8 | NFR | NFR de 05 §8 (latencias p95, lote de sincronización, idempotencia, integridad, disponibilidad) |

## NFR numéricos y su stage de verificación

Cada NFR numérico de `arquitectura-solucion_v1.0.md` §8 (intake §17 P.10) tiene un stage que lo verifica antes de promover a PROD (`pipeline-ci-cd_v1.0.md` §3.1):

| NFR | Objetivo | Stage / ambiente |
| --- | --- | --- |
| Latencia p95 lecturas | ≤ 300 ms | NFR (G8) en STAGING |
| Latencia p95 escrituras | ≤ 500 ms | NFR (G8) en STAGING |
| Capacidad del lote de sincronización | ≥ 1000 cambios sin pérdida ni duplicación | NFR (G8) en STAGING |
| Idempotencia de operaciones no seguras | 100 % sin efecto duplicado | Test integración + NFR (QA/STAGING) |
| Integridad de jerarquía y ciclo | 0 violaciones bajo concurrencia | Test integración (QA) |
| Disponibilidad mensual | ≥ 99,5 % (sin SLO 99,9 %, `tiene_observabilidad_critica=false`) | Sondas de salud en PROD |

## Modelo de ambientes

Ambientes de servicio desplegable (no canales de paquete, regla §2.2 para `rest-api`):

| Ambiente | Tag o evento que promueve | Aprobador |
| --- | --- | --- |
| DEV | Merge a `main` | Automático |
| QA | Tag `vX.Y.Z-rc.N` | QA lead |
| STAGING | Aprobación tras QA en verde | Release manager |
| PROD (canary) | Tag `vX.Y.Z` sin sufijo | Release manager + aprobación de negocio |

## Nivel solución

Este README es de nivel proyecto. La orquestación de build y publicación multi-proyecto de la solución GeoVial (orden topológico del manifiesto, matriz de artefactos, coordinación inter-proyecto) corresponde a `_solucion/pipeline-solucion_v1.0.md` (09_rules §4.9), que se genera una vez al cierre del bucle de proyectos (Fase H); no es parte de esta carpeta.

## Trazabilidad upstream/downstream

- Upstream: 05 (`arquitectura-solucion_v1.0.md` §5 despliegue, §7 cross-cutting/secretos, §8 NFR, §9 riesgos; `contratos-rest_v1.0.md` §3/§6; ADR-02, ADR-03, ADR-09, ADR-10), 08 (`estrategia-calidad_v1.0.md` §3 gates G1-G8; `estrategia-testing_v1.0.md` cobertura; `definition-of-done_v1.0.md`; `criterios-validacion_v1.0.md`), intake §17 P.6/P.7/P.8/P.9/P.10.
- Downstream: 10 (developer guide cita los comandos exactos del pipeline para reproducción local), 11 (los samples consumen el contrato OpenAPI publicado y los ambientes declarados).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 09 de geovial-api con el índice de los seis artefactos obligatorios (estrategia de versionado, pipeline CI/CD, entornos de despliegue, dos guías de publicación —imagen de contenedor y contrato OpenAPI— y supply chain), el orden de lectura, el mapeo de los gates G1-G8 a stages y a su criterio DoD/NFR de origen, los NFR numéricos con su stage de verificación, el modelo de ambientes DEV/QA/STAGING/PROD con canary y la trazabilidad upstream/downstream. |
