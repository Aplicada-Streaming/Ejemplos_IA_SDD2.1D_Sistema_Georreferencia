# Guía de publicación del artefacto image-docker — geovial-api

**Proyecto:** geovial-api
**Documento:** guia-publicacion-image-docker_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps + Platform Engineer

## 0. Alcance

Esta guía cubre la publicación de la imagen de contenedor de `geovial-api` al registro de imágenes, desde el que el orquestador de contenedores la despliega en DEV/QA/STAGING/PROD. El tipo de artefacto `image-docker` es el nombre normalizado de la tabla §2.2 de 09_rules para el artefacto desplegable del proyecto; el cuerpo de esta guía describe la imagen de contenedor del backend en términos genéricos. La publicación del contrato OpenAPI versionado se documenta aparte en `guia-publicacion-openapi_v1.0.md`.

## 1. Pre-requisitos

| Pre-requisito | Detalle |
| --- | --- |
| Acceso al registro de imágenes | Cuenta o identidad de servicio con permiso de escritura sobre el repositorio de imágenes del proyecto |
| Credencial de publicación | Token o identidad con scope de escritura al registro, almacenado en el vault del CI (`entornos-deploy_v1.0.md` §4); nunca en commit |
| Identidad de firma del artefacto | Clave o identidad keyless con transparency log para firmar la imagen y el SBOM (`supply-chain-seguridad_v1.0.md` §2) |
| SDK y runtime fijados | Runtime del backend en versión LTS y SDK fijado por el lock de dependencias, para un build reproducible (intake §17 P.9) |
| Scripts locales | Scripts de construcción y de creación de la imagen (`build-*`, `publish-*`, intake §16 `/build` y `/deploy`) reproducibles localmente |

Para una publicación manual de emergencia, el desarrollador autentica contra el registro con la credencial del vault y ejecuta los mismos scripts que el pipeline; el camino normal es automatizado (§2).

## 2. Comando o stage de publicación

La publicación la ejecuta el pipeline en los stages Build de imagen, Firma y Publish imagen (`pipeline-ci-cd_v1.0.md` §3), disparados por push a `main` (build) y por tag (publicación a registro para promoción).

Secuencia abstracta reproducible (los comandos exactos los cita la developer guide de 10):

1. Construir la imagen contra el runtime LTS del backend a partir del manifiesto de construcción versionado, etiquetándola con la versión derivada por la herramienta de versión (`estrategia-versionado_v1.0.md` §5) y registrando el digest inmutable.
2. Generar el SBOM de la imagen (CycloneDX/SPDX JSON) en el stage SBOM.
3. Firmar la imagen y el SBOM en el stage Firma; registrar la firma en el transparency log.
4. Publicar la imagen al registro por digest y etiqueta de versión en el stage Publish imagen, solo si la firma es válida (gate bloqueante).

Variables de entorno requeridas (inyectadas desde el vault del CI):

| Variable (rol abstracto) | Propósito |
| --- | --- |
| URL del registro de imágenes | Destino de publicación |
| Credencial de escritura al registro | Autenticación de la publicación |
| Identidad de firma | Firma de la imagen y del SBOM |
| Versión derivada (etiqueta) | Etiqueta de la imagen, coincidente con el tag Git y el contrato OpenAPI |

## 3. Verificación post-publish

| Verificación | Cómo confirmar |
| --- | --- |
| Imagen disponible | La imagen se resuelve en el registro por su digest y etiqueta de versión |
| Firma válida | La firma de la imagen y del SBOM verifica contra la identidad de firma y el transparency log antes de cualquier despliegue |
| SBOM adjunto | El SBOM JSON está adjunto al release y es verificable (`supply-chain-seguridad_v1.0.md` §1) |
| Despliegue saludable | Al desplegar la imagen en el ambiente destino, las sondas de salud y de preparación quedan en verde antes de habilitar tráfico (`entornos-deploy_v1.0.md` §1) |
| Contrato coherente | El contrato OpenAPI publicado para esa versión valida contra la implementación de la imagen (gate G5; `guia-publicacion-openapi_v1.0.md` §3) |

Ningún despliegue habilita tráfico sin verificar la firma y las sondas de salud; en PROD el canary verifica además las métricas observadas en cada escalón (`entornos-deploy_v1.0.md` §1.1).

## 4. Rollback

El rollback de una imagen desplegada es por desvío de tráfico al digest previo (rollback rápido de la variante `rest-api`), no por borrado de la imagen. El procedimiento completo vive en `pipeline-ci-cd_v1.0.md` §7; en resumen:

| Paso | Acción |
| --- | --- |
| 1 | Desviar el tráfico de la versión nueva a la versión previa estable por el orquestador de contenedores (corte inmediato) |
| 2 | Escalar a 0 la versión defectuosa y marcar su digest como no promovible; la imagen no se borra del registro |
| 3 | Si no había versión previa activa, redesplegar el digest inmutable de la última imagen estable conocida (intake §17 P.8) |
| 4 | Publicar el fix como PATCH (retrocompatible) o versión mayor (incompatible) y comunicar en CHANGELOG y release notes |

Ventana de gracia: la imagen previa permanece desplegable hasta que el fix supere STAGING; no se retira ninguna imagen que un ambiente aún necesite para análisis o rollback.

## 5. Métricas

| Métrica | Qué mide | Fuente |
| --- | --- | --- |
| Tiempo medio de despliegue | Desde la publicación de la imagen hasta el tráfico al 100 % en canary | Pipeline / orquestador |
| Tiempo medio hasta detección de regresión | Desde el despliegue hasta la alerta que dispara el rollback | Sondas de salud y puntos de medición (05 §7) |
| Tiempo de rollback | Desde la decisión de revertir hasta el tráfico restaurado al digest previo | Orquestador (objetivo: minutos, variante rest-api) |
| Vulnerabilidades detectadas post-publish | CVE descubiertas sobre la imagen ya publicada | Schedule de SCA (`supply-chain-seguridad_v1.0.md` §4) |
| Disponibilidad observada | Disponibilidad mensual del servicio en PROD | Sondas de salud (NFR ≥ 99,5 %, 05 §8) |

## 6. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Stages de build, firma y publicación | `pipeline-ci-cd_v1.0.md` §3 |
| Rollback por desvío de tráfico | `pipeline-ci-cd_v1.0.md` §7; intake §17 P.8 |
| SBOM y firma | `supply-chain-seguridad_v1.0.md` §1/§2 |
| Secretos de publicación | `entornos-deploy_v1.0.md` §4 |
| Despliegue y ambientes | `entornos-deploy_v1.0.md` §1; 05 §5 |
| Coherencia con el contrato | `guia-publicacion-openapi_v1.0.md` §3; gate G5 (08) |

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Guía inicial de publicación del artefacto image-docker de geovial-api: pre-requisitos (acceso al registro, credenciales en vault, identidad de firma, runtime LTS fijado, scripts reproducibles), secuencia de build/SBOM/firma/publish ejecutada por el pipeline, verificación post-publish (imagen resuelta, firma válida, SBOM adjunto, despliegue saludable, contrato coherente), rollback por desvío de tráfico al digest previo con ventana de gracia, y métricas observables. Derivado de `pipeline-ci-cd_v1.0.md` §3/§7, de `supply-chain-seguridad_v1.0.md` §1/§2, de 05 §5 y del intake §17 P.8/P.9. |
