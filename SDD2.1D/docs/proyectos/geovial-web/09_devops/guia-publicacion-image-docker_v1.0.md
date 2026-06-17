# Guía de publicación de la imagen de contenedor — geovial-web

**Proyecto:** geovial-web
**Documento:** guia-publicacion-image-docker_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps + Deploy Engineer

## 0. Alcance

`geovial-web` publica como artefacto una imagen de contenedor del front (`image-docker`, regla 09 §2.2; intake §17.P.7, §17.P.8). El destino de publicación es un registro de imágenes de contenedor del entorno, desde el cual el orquestador de contenedores de cada ambiente la consume (`entornos-deploy_v1.0.md`). No es un paquete redistribuible: el front es un servicio desplegable, no una dependencia que un tercero descargue. Esta guía cubre los pre-requisitos, el comando o stage de publicación, la verificación post-publish, el rollback y las métricas (regla 09 §4.5).

## 1. Pre-requisitos

| Requisito | Detalle |
| --- | --- |
| Acceso al registro de imágenes | Credencial con scope de escritura al repositorio de imágenes del front, por ambiente; vive en el vault (`entornos-deploy_v1.0.md` §4), nunca en commit |
| Credencial de firma | Clave o identidad de firma del firmador de artefactos con registro de transparencia (`supply-chain-seguridad_v1.0.md` §2) |
| Definición de imagen versionada | La definición de la imagen del front en el repositorio (scripts de imagen de `/deploy`, intake §16); reproducible localmente |
| Versión calculada | La etiqueta SemVer derivada del tag Git por la herramienta de auto-versioning (`estrategia-versionado_v1.0.md` §3) |
| Gates en verde | Suite de gates de merge y de release en verde (`pipeline-ci-cd_v1.0.md` §1); SBOM generado (STAGE-12) |
| Configuración local opcional | Para publicación manual: sesión iniciada en el registro y en el firmador con las credenciales del vault; runtime de construcción de imágenes instalado |

## 2. Comando o stage de publicación

La publicación es automatizada por el pipeline (STAGE-13 build de imagen, STAGE-14 firma, STAGE-16 publish de `pipeline-ci-cd_v1.0.md` §1). El equivalente local lo materializan los scripts `build-*.bat` y `publish-*.bat` y los scripts de imagen de `/deploy` (intake §16).

Secuencia (rol abstracto de cada paso; el comando concreto del runtime lo fija la developer guide de 10):

| Paso | Acción | Variables de entorno requeridas |
| --- | --- | --- |
| 1 | Construir la imagen del front desde su definición versionada, etiquetada con la versión SemVer calculada y el digest | Versión SemVer; manifiesto de dependencias |
| 2 | Generar y adjuntar el SBOM de la imagen | — |
| 3 | Firmar la imagen y registrar la firma en el transparency log | Credencial de firma (del vault) |
| 4 | Publicar la imagen firmada al registro del ambiente destino | Credencial de escritura al registro (del vault); coordenadas del registro |

La etiqueta de la imagen es la versión SemVer inmutable (`v<X.Y.Z>` o `v<X.Y.Z>-rc.N`): una vez publicada, apunta siempre al mismo digest firmado. La promoción entre ambientes (`entornos-deploy_v1.0.md` §5) reutiliza esa misma imagen, no la reconstruye.

## 3. Verificación post-publish

Tras publicar, se confirma que la imagen quedó publicada, firmada y es desplegable (regla 09 §4.5).

| Verificación | Cómo |
| --- | --- |
| Imagen presente en el registro | Consultar el registro por la etiqueta de versión y confirmar el digest publicado |
| Firma válida y registrada | Verificar la firma de la imagen contra el transparency log antes de cualquier despliegue (gate de la promoción, `supply-chain-seguridad_v1.0.md` §2) |
| SBOM adjunto | Confirmar que el SBOM de la imagen está disponible junto al release |
| Despliegue de prueba | Desplegar la imagen en DEV/QA y ejecutar la prueba de humo: inicio de sesión (CU-01), apertura de una vista clave (CU-03) y verificación de que el circuito interactivo se establece |
| Custodia del token | Confirmar que el token bearer no se serializa al navegador en la vista de ingreso (TC-19), sobre la imagen publicada |
| Salud del contenedor | La comprobación de salud del contenedor de front responde y el front alcanza al `geovial-api` del ambiente |

Ninguna imagen sin firma válida y registrada se promueve a un ambiente (regla 09 §4.8, "falta de firma del artefacto").

## 4. Rollback

El rollback es por redepliegue de la imagen previa estable (`pipeline-ci-cd_v1.0.md` §5; intake §17.P.8). No hay delist en un feed de paquetes: la imagen no se distribuye a consumidores externos.

| Paso | Acción |
| --- | --- |
| 1 | Identificar la imagen previa estable por su etiqueta en el registro (retenida por `pipeline-ci-cd_v1.0.md` §3) |
| 2 | Indicar al orquestador de contenedores que ejecute esa etiqueta previa, con desvío inmediato de tráfico y respeto de la afinidad de sesión (`entornos-deploy_v1.0.md` §6) |
| 3 | Aplicar la ventana de drenaje breve de los circuitos en curso antes de cortar la versión defectuosa |
| 4 | Verificar el redepliegue con la prueba de humo (§3) y la verificación de firma de la imagen redeployada |
| 5 | Comunicar el rollback y abrir el fix por el flujo normal (revert en `main`, nueva versión PATCH o MAJOR según SemVer) |

- Ventana de gracia: la imagen previa estable se retiene en el registro al menos mientras la versión vigente esté en PROD (mínimo: la vigente y la inmediata anterior, `pipeline-ci-cd_v1.0.md` §3), de modo que el rollback sea ejecutable en minutos.
- El redepliegue no pierde dato de dominio: el estado de UI es efímero y se reconstruye desde la API (ADR-02). Los circuitos activos se cortan y reconectan.

## 5. Métricas

Indicadores observables de la publicación y del artefacto desplegado (regla 09 §4.5):

| Métrica | Qué mide |
| --- | --- |
| Tiempo de publicación | Duración desde el build de la imagen hasta su disponibilidad firmada en el registro |
| Tasa de éxito de publicación | Proporción de publicaciones que pasan la verificación post-publish sin reintento |
| Tiempo medio de despliegue | Duración desde la imagen publicada hasta el front saludable en el ambiente |
| Tiempo medio hasta rollback | Duración desde la detección de una regresión hasta el redepliegue de la imagen previa saludable |
| Vulnerabilidades detectadas post-publish | CVE descubiertas en la imagen publicada por el escaneo programado (`supply-chain-seguridad_v1.0.md` §4) |
| Disponibilidad del front desplegado | SLO observado ≥ 99,5 % mensual en PROD (intake §17.P.10; `entornos-deploy_v1.0.md` §1) |
| Verificación de firma en despliegue | Proporción de despliegues que verifican la firma antes de promover (objetivo: 100 %) |

## 6. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Stages de build, firma y publish | STAGE-13, STAGE-14, STAGE-16 de `pipeline-ci-cd_v1.0.md` |
| Etiqueta de versión | `estrategia-versionado_v1.0.md` §3 |
| Registro, ambientes y afinidad de sesión | `entornos-deploy_v1.0.md` §1, §5, §6 |
| SBOM, firma y CVE | `supply-chain-seguridad_v1.0.md` §1, §2, §4 |
| Verificación de custodia del token | NFR custodia del token (05 §8); TC-19 (08) |

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Guía de publicación inicial de la imagen de contenedor del front de geovial-web: pre-requisitos (acceso al registro y al firmador desde vault, definición de imagen versionada, gates verdes), secuencia de build/SBOM/firma/publish con etiqueta SemVer inmutable, verificación post-publish (presencia, firma contra transparency log, SBOM, despliegue de prueba con humo de sesión y circuito, custodia del token), rollback por redepliegue de la imagen previa con drenaje y afinidad de sesión, y métricas de publicación, despliegue, rollback, CVE post-publish y disponibilidad. |
