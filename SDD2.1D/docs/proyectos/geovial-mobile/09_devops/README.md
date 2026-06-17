# 09 DevOps — geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** README.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps + Mobile Release Engineer

Índice navegable de los artefactos DevOps de la app móvil de campo `geovial-mobile` (tipo `mobile-app-maui`, no redistribuible). El artefacto que el pipeline construye, firma y distribuye es un paquete de aplicación Android, distribuido por un canal de distribución interno (no se publica en tienda pública en v1, intake §17 P.7). El modelo de distribución son canales de distribución móvil `internal`, `alpha`, `beta` y `production`, no ambientes de servicio DEV/QA/STAGING/PROD: la app corre en el dispositivo del agente de campo, no es un servicio desplegado.

## Artefactos vigentes

| Documento | Propósito | Estado |
| --- | --- | --- |
| [estrategia-versionado_v1.0.md](estrategia-versionado_v1.0.md) | SemVer 2.0.0, Conventional Commits, GitVersion, branching trunk-based, canales internal/alpha/beta/production y deprecation policy. Documento bisagra entre código y artefacto | Propuesto |
| [pipeline-ci-cd_v1.0.md](pipeline-ci-cd_v1.0.md) | Stages (lint, build, test unit/offline-sync/UI móvil/snapshot, cobertura, NFR de campo, SCA, SBOM, análisis estático, firma del paquete, distribución y verificación post-distribución), matriz de plataforma Android, caché, promotion internal→alpha→beta→production, rollback por redistribución y notificaciones | Propuesto |
| [entornos-deploy_v1.0.md](entornos-deploy_v1.0.md) | Canales de distribución móvil (no ambientes), configuración 12-factor por canal, secretos (incl. la credencial de firma) en almacén seguro y promoción | Propuesto |
| [guia-publicacion-store-mobile_v1.0.md](guia-publicacion-store-mobile_v1.0.md) | Pre-requisitos (credencial de firma resguardada), empaquetado y firma, distribución por canal interno, verificación post-distribución, rollback por redistribución y métricas; ruta de tienda pública como destino futuro | Propuesto |
| [supply-chain-seguridad_v1.0.md](supply-chain-seguridad_v1.0.md) | SBOM CycloneDX firmado, firma del paquete con credencial resguardada, SLSA L2 objetivo, dependency scanning, SAST (DAST no aplica) y política de CVE | Propuesto |

## Orden de lectura sugerido

1. Acuerdo de equipo (categoría 00, branching trunk-based).
2. [estrategia-versionado_v1.0.md](estrategia-versionado_v1.0.md) — define versión, commits y canales.
3. [pipeline-ci-cd_v1.0.md](pipeline-ci-cd_v1.0.md) — los gates de 08 como stages.
4. [entornos-deploy_v1.0.md](entornos-deploy_v1.0.md) — canales de distribución y secretos.
5. [guia-publicacion-store-mobile_v1.0.md](guia-publicacion-store-mobile_v1.0.md) — cómo empaquetar, firmar, distribuir y revertir.
6. [supply-chain-seguridad_v1.0.md](supply-chain-seguridad_v1.0.md) — SBOM, firma, SLSA, CVE.

## Quality gates ejecutados como stages

Los quality gates los define `08_calidad_y_pruebas/estrategia-calidad_v1.0.md` §3; el pipeline los ejecuta como stages sin redefinirlos (`pipeline-ci-cd_v1.0.md` §3). La Definition of Done canónica vive en `08_calidad_y_pruebas/definition-of-done_v1.0.md` y se ejecuta como gates; este pipeline no la redefine.

| Gate de 08 §3 | Stage del pipeline | Criterio DoD / NFR que verifica |
| --- | --- | --- |
| Compilación | Build | DoD BT §1.2 (compila sin warnings tratados como error) |
| Pruebas en verde | Test unit + Test offline-sync + Test UI móvil | DoD US §1.1, BT §1.2 (suite unitaria, sincronización e interfaz verdes) |
| Cobertura global | Cobertura | DoD US §1.1, release §1.4; intake §17 P.6 (líneas ≥ 80 / branches ≥ 70) |
| Cobertura por capa | Cobertura | intake §17 P.6 (lógica ≥ 75, presentación ≥ 60; infraestructura ≥ 70 de 08) |
| Análisis estático | Análisis estático | DoD BT §1.2, US §1.1, release §1.4 (sin issues críticos) |
| Snapshot de pantallas críticas | Test snapshot | DoD release §1.4 (TC-27 sin diferencias no aprobadas) |
| Firma del paquete | Firma del paquete | DoD release §1.4 (firma con credencial resguardada) |
| NFR de release | NFR de campo | NFR de 05 §8; intake §17 P.10 (captura offline, cola ≥ 1000, ciclo ≤ 30 s, reanudación, arranque ≤ 3 s) |

## Modelo de distribución

Canales de distribución móvil sobre el canal interno (no ambientes desplegables, regla 09 §2.2 para `mobile-app-maui`):

| Canal | Tag que lo alimenta | Aprobador |
| --- | --- | --- |
| `internal` | `vX.Y.Z-internal.N` | Automático |
| `alpha` | `vX.Y.Z-alpha.N` | Mobile Release Engineer |
| `beta` | `vX.Y.Z-beta.N` | Mobile Release Engineer |
| `production` | `vX.Y.Z` sin sufijo | Mobile Release Engineer (aprobación auditable) |

## Nivel solución

Este README es de nivel proyecto. La orquestación de build y publicación multi-proyecto de la solución GeoVial (orden topológico del manifiesto, matriz de artefactos, coordinación inter-proyecto) corresponde a `_solucion/pipeline-solucion_v1.0.md` (09_rules §4.9), que se genera una vez al cierre del bucle de proyectos; no es parte de esta carpeta (Fase H).

## Trazabilidad upstream/downstream

- Upstream: 05 (`arquitectura-solucion_v1.0.md` §5/§7/§8 NFR; ADR-01 a ADR-05; flujo de sincronización), 08 (`estrategia-calidad_v1.0.md` §3 gates; `estrategia-testing_v1.0.md` §2 cobertura; `definition-of-done_v1.0.md`; `matriz-cobertura-pruebas_v1.0.md`; `criterios-validacion_v1.0.md`), intake §17 P.6/P.7/P.8/P.9/P.10.
- Downstream: 10 (developer guide cita los comandos exactos del pipeline para reproducción local), 11 (los samples de la app consumen los canales declarados en `entornos-deploy_v1.0.md`).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 09 de geovial-mobile con el índice de los cinco artefactos obligatorios (estrategia de versionado, pipeline CI/CD, entornos/distribución por canales, guía de publicación del paquete de aplicación Android y supply chain), el orden de lectura, el mapeo de los quality gates de 08 §3 a stages y a su criterio DoD/NFR de origen, el modelo de canales internal/alpha/beta/production y la trazabilidad upstream/downstream. |
