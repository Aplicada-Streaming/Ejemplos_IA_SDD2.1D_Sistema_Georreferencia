# 09 DevOps — geovial-storage

**Proyecto:** geovial-storage
**Tipo (D8):** library
**Variante:** Ingeniero DevOps Senior (DevOps + Release Engineer, library)
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps Senior

Punto de entrada navegable de la sección 09 de `geovial-storage`, la librería que expone al backend `geovial-api` una abstracción de alojamiento de archivos transparente con proveedores intercambiables (local / objetos remoto / otro) seleccionables por el usuario raíz. La librería no se publica como paquete redistribuible: se integra al artefacto del backend y se construye junto a él (intake §13, §17.P.7; ADR-03). Por eso esta sección no define un feed ni ambientes propios y omite la guía de publicación (ver §"Omisiones registradas").

## Documentos de la sección

| Documento | Estado | Descripción |
| --- | --- | --- |
| [estrategia-versionado_v1.0.md](estrategia-versionado_v1.0.md) | Propuesto | SemVer 2.0.0, Conventional Commits 1.0.0, versión derivada del tag alineada al ciclo del backend, trunk-based con main protegida, canal único integrado al backend (sin feed externo) y deprecation policy. |
| [pipeline-ci-cd_v1.0.md](pipeline-ci-cd_v1.0.md) | Propuesto | Doce stages (lint, build, unit, contract, cobertura, mutation, no filtración, NFR latencia, SCA, SBOM, integración al backend y firma del artefacto del backend) que ejecutan los gates G-01..G-09 de 08; matriz SO/runtime; caché; promotion alineada al backend; rollback por reversión de la imagen del backend; notificaciones. |
| [entornos-deploy_v1.0.md](entornos-deploy_v1.0.md) | Propuesto | Declara que no hay feed ni ambientes propios; la librería viaja embebida en la imagen del backend y hereda los ambientes de `geovial-api` (referenciados); volumen persistente para el proveedor local; configuración 12-factor por CU-06; secretos en vault, nunca en commit. |
| [supply-chain-seguridad_v1.0.md](supply-chain-seguridad_v1.0.md) | Propuesto | SBOM propagado al backend, firma de la imagen del backend, SLSA L2 objetivo, dependency scanning, SAST y escaneo de secretos, DAST no aplicable a la librería, política de CVE con SLA por severidad y tratamiento de credenciales según ADR-05. |

## Orden de lectura sugerido

Acuerdo de equipo (00) → estrategia de versionado → pipeline CI/CD → entornos y despliegue → supply chain y seguridad (regla 09 §3.5). La guía de publicación se omite (ver abajo).

## Omisiones registradas

| Artefacto | Estado | Motivo |
| --- | --- | --- |
| `guia-publicacion-<tipo-artefacto>_v1.0.md` | Omitido | `geovial-storage` es una `library` cuyo artefacto no se publica externamente: se integra al backend `geovial-api` y se distribuye dentro de su imagen (intake §13, §17.P.7; ADR-03). La regla 09 §2.1 (tabla maestra, columna "Omitir para") indica omitir la guía de publicación para "tipos cuyo artefacto no se publica externamente". La publicación efectiva es la del artefacto del backend, documentada en la categoría 09 de `geovial-api`. |
| `_solucion/pipeline-solucion_v1.0.md` | Fuera del alcance de este proyecto | Es un artefacto de nivel solución (regla 09 §2.1, §4.9), no de proyecto; se produce una sola vez bajo `_solucion/` al cierre del bucle de proyectos. No corresponde generarlo en la sección 09 de un proyecto individual. |

## Gates ejecutados (provenientes de 08)

Los gates G-01..G-09 se definen en `08_calidad_y_pruebas/estrategia-calidad_v1.0.md` §3 y se ejecutan como stages del pipeline (`pipeline-ci-cd_v1.0.md` §1). La DoD canónica (`08/definition-of-done_v1.0.md`) no se redefine aquí: se ejecuta como conjunto de gates (regla 08 §4.8).

| Gate | Stage del pipeline | Consecuencia |
| --- | --- | --- |
| G-01 Compilación limpia | STAGE-02 Build | Bloquea merge |
| G-02 Unit y contract verdes | STAGE-03, STAGE-04 | Bloquea merge |
| G-03 Cobertura global | STAGE-05 | Bloquea merge |
| G-04 Cobertura por capa | STAGE-05 | Bloquea merge |
| G-05 Mutation dominio | STAGE-06 | Bloquea release |
| G-06 Transparencia por proveedor | STAGE-04 | Bloquea merge |
| G-07 No filtración de credenciales | STAGE-07 | Bloquea merge |
| G-08 NFR latencia | STAGE-08 | Bloquea release |
| G-09 Análisis estático | STAGE-01, STAGE-09 | Bloquea merge |

## Trazabilidad

- Upstream: NFR-01..NFR-06 y ADR-01..ADR-05 (05); gates G-01..G-09, DoD canónica y criterios de validación (08); intake §17 P.6 (cobertura), P.7 (no NuGet, GitVersion alineado al backend), P.8 (gates, rollback por reversión de la imagen del backend), P.9 (.NET 8 LTS contenedor Linux).
- Downstream: 10 (developer guide cita los comandos exactos del pipeline para reproducción local); 11 (no aplica canal externo, la librería no se consume desde un feed).
- Nivel solución: el orden de build inter-proyecto (la librería de nivel 0 antes que el backend de nivel 1) lo gobierna `_solucion/pipeline-solucion_v1.0.md`; el rollback de la librería se ejecuta por reversión de la imagen del backend.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 09 de geovial-storage: índice de los cuatro artefactos producidos (estrategia de versionado, pipeline CI/CD, entornos y despliegue, supply chain y seguridad), orden de lectura, registro de la omisión de la guía de publicación con su motivo citando 09_rules §2.1, mapa de gates G-01..G-09 a stages del pipeline y trazabilidad upstream/downstream. |
