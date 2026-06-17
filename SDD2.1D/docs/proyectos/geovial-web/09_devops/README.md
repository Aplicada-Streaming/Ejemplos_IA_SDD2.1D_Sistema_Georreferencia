# 09 DevOps — geovial-web

**Proyecto:** geovial-web
**Tipo (D8):** web-monolith
**Variante:** Ingeniero DevOps Senior (DevOps + Deploy Engineer, web-monolith)
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps + Deploy Engineer

Punto de entrada navegable de la sección 09 de `geovial-web`, el front web de render server-side de la solución GeoVial. El front mantiene, por sesión de usuario, un circuito interactivo persistente y consume por contrato la API REST de `geovial-api`; no tiene persistencia de dominio propia (ADR-02). El artefacto publicable es una imagen de contenedor del front, que se promueve por los ambientes de servicio DEV → QA → STAGING → PROD (intake §13, §17.P.7, §17.P.8). Por ser un servicio desplegable, esta sección modela ambientes (no canales de paquete) y produce la guía de publicación de la imagen.

## Documentos de la sección

| Documento | Estado | Descripción |
| --- | --- | --- |
| [estrategia-versionado_v1.0.md](estrategia-versionado_v1.0.md) | Propuesto | SemVer 2.0.0, Conventional Commits 1.0.0, versión derivada del tag (GitVersion) con imagen etiquetada por versión inmutable, trunk-based con `main` protegida (equipo_n=1), modelo de ambientes de servicio (no canales) y deprecation policy de configuración y del contrato consumido. |
| [pipeline-ci-cd_v1.0.md](pipeline-ci-cd_v1.0.md) | Propuesto | Dieciséis stages (lint, build, unit, integración, componente UI, snapshot, cobertura, custodia del token, NFR interacción, NFR concurrencia, SCA, SBOM, build de imagen, firma, DAST y publish) que ejecutan los gates de 08; matriz runtime/navegadores evergreen; caché y artefactos; promotion DEV→QA→STAGING→PROD; rollback por redepliegue; notificaciones. |
| [entornos-deploy_v1.0.md](entornos-deploy_v1.0.md) | Propuesto | Ambientes DEV/QA/STAGING/PROD con aprobador y SLA; IaC declarativa; configuración 12-factor; secretos en vault y token del lado servidor; promoción con aprobador para PROD; y el tratamiento de la afinidad de sesión y la política de réplicas del circuito persistente (riesgo de 05 §5 delegado a 09). |
| [guia-publicacion-image-docker_v1.0.md](guia-publicacion-image-docker_v1.0.md) | Propuesto | Publicación de la imagen de contenedor del front: pre-requisitos, secuencia build/SBOM/firma/publish con etiqueta SemVer inmutable, verificación post-publish, rollback por redepliegue y métricas. |
| [supply-chain-seguridad_v1.0.md](supply-chain-seguridad_v1.0.md) | Propuesto | SBOM de la imagen, firma con registro de transparencia, SLSA L2 objetivo, dependency scanning, SAST/escaneo de secretos y DAST sobre el front desplegado; política de CVE por severidad; token del lado servidor y secretos en vault. |

## Orden de lectura sugerido

Acuerdo de equipo (00) → estrategia de versionado → pipeline CI/CD → entornos y despliegue → guía de publicación de la imagen → supply chain y seguridad (regla 09 §3.5).

## Gates ejecutados (provenientes de 08)

Los quality gates se definen en `08_calidad_y_pruebas/estrategia-calidad_v1.0.md` §3 y se ejecutan como stages del pipeline (`pipeline-ci-cd_v1.0.md` §1). La DoD canónica (`08/definition-of-done_v1.0.md`) no se redefine aquí: se ejecuta como conjunto de gates (regla 08 §4.8).

| Gate (08 §3) | Stage del pipeline | Consecuencia |
| --- | --- | --- |
| Compilación limpia | STAGE-02 Build | Bloquea merge |
| Pruebas unitarias y de integración en verde | STAGE-03, STAGE-04 | Bloquea merge |
| Pruebas de componente de UI y snapshot en verde | STAGE-05, STAGE-06 | Bloquea merge |
| Gate de cobertura global | STAGE-07 | Bloquea merge |
| Gate de cobertura por capa | STAGE-07 | Bloquea merge |
| Análisis estático | STAGE-01, STAGE-11 | Bloquea merge |
| Custodia del token | STAGE-08 | Bloquea release |
| NFR de interacción | STAGE-09 | Bloquea release |
| NFR de concurrencia | STAGE-10 | Bloquea release |

Los tres NFR numéricos de P.10 (interacción p95 ≤ 200 ms, ≥ 50 circuitos concurrentes, custodia del token con 0 exposiciones) tienen un stage que los verifica antes de promover a STAGING/PROD (regla 09 §5.1).

## Omisiones registradas

| Artefacto | Estado | Motivo |
| --- | --- | --- |
| `_solucion/pipeline-solucion_v1.0.md` | Fuera del alcance de este proyecto | Es un artefacto de nivel solución (regla 09 §2.1, §4.9), no de proyecto; se produce una sola vez bajo `_solucion/` al cierre del bucle de proyectos. No corresponde generarlo en la sección 09 de un proyecto individual. |

No se omite la guía de publicación: el front sí publica un artefacto externo (la imagen de contenedor desplegable), a diferencia de las librerías no redistribuibles de la solución (regla 09 §2.1, columna "Obligatorio para todos los tipos D8 con artefacto publicable").

## Trazabilidad

- Upstream: NFR de 05 §8 (interacción p95, ≥ 50 circuitos, custodia del token, disponibilidad ≥ 99,5 %, cobertura) y ADR-01..ADR-05 (05); quality gates, DoD canónica y criterios de validación (08); intake §17 geovial-web P.6 (cobertura), P.7 (SemVer/Conventional Commits, GitVersion, trunk-based, imagen de contenedor, DEV/QA/STAGING/PROD), P.8 (CI con gates, rollback por redepliegue), P.9 (runtime LTS en contenedor Linux, navegadores evergreen), P.10 (NFR numéricos).
- Riesgo de despliegue del circuito persistente: 05 §5 y §9 delegan a esta categoría la afinidad de sesión y la política de réplicas; se resuelve en `entornos-deploy_v1.0.md` §6.
- Downstream: 10 (developer guide cita los comandos exactos del pipeline para reproducción local); 11 (el front no se consume desde un feed; los samples referencian el ambiente, no un canal de paquete).
- Nivel solución: el orden de build inter-proyecto (el backend `geovial-api` de nivel 1 antes que el front de nivel 2) lo gobierna `_solucion/pipeline-solucion_v1.0.md`.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 09 de geovial-web: índice de los cinco artefactos producidos (estrategia de versionado, pipeline CI/CD, entornos y despliegue, guía de publicación de la imagen de contenedor, supply chain y seguridad), orden de lectura, mapa de los quality gates de 08 a stages del pipeline, registro de las omisiones de nivel solución, y trazabilidad upstream (05 NFR/ADR, 08 DoD/gates, intake §17 P.6-P.10) y downstream, incluido el riesgo del circuito persistente delegado a entornos-deploy. |
