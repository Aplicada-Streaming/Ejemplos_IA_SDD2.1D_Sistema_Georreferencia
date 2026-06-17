# Supply chain y seguridad — geovial-web

**Proyecto:** geovial-web
**Documento:** supply-chain-seguridad_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps + Deploy Engineer

## 0. Alcance

`geovial-web` es un `web-monolith` cuyo artefacto es una imagen de contenedor del front desplegada como servicio (intake §13, §17.P.7). La cadena de suministro cubre dos planos: el inventario y la verificación de las dependencias de la imagen, y la firma de la imagen publicada. A diferencia de las librerías de la solución, el front tiene una superficie dinámica desplegada, por lo que el análisis dinámico (DAST) sí aplica y se ejecuta sobre el front desplegado en un ambiente no productivo (§5).

Dos elementos sensibles exigen atención específica: el token bearer del usuario, que se retiene del lado servidor del circuito y nunca se expone al navegador (ADR-03; intake §17.P.5), y los secretos del front hacia el backend, que viven en vault y nunca en commit (`entornos-deploy_v1.0.md` §4). Se tratan en §7.

## 1. SBOM

| Aspecto | Decisión |
| --- | --- |
| Formato | Inventario de software en formato estándar interoperable (CycloneDX o SPDX, salida JSON), el mismo que adopte la solución |
| Generador | Generador de SBOM del runtime objetivo, ejecutado en STAGE-12 del pipeline (`pipeline-ci-cd_v1.0.md` §1) |
| Contenido | Todas las dependencias directas y transitivas del front y las capas base de la imagen de contenedor, con su versión y licencia |
| Publicación | El SBOM se adjunta al release de la imagen del front (`guia-publicacion-image-docker_v1.0.md` §3) |
| Firma del SBOM | El SBOM se firma junto con la imagen (ver §2) |

El SBOM de la imagen permite responder ante una CVE de una dependencia transitiva o de una capa base con el inventario completo del artefacto desplegado.

## 2. Firma

| Aspecto | Decisión |
| --- | --- |
| Qué se firma | La imagen de contenedor del front publicada al registro |
| Herramienta | Firmador de artefactos con registro de transparencia (sigstore/cosign u homólogo), ejecutado en STAGE-14 del pipeline |
| Registro de transparencia | La firma se registra en un transparency log verificable |
| Verificación | El despliegue de cada ambiente verifica la firma de la imagen antes de promover (gate de la promoción, `entornos-deploy_v1.0.md` §5) |
| Política | Ninguna imagen del front sin firma válida y registrada se promueve a un ambiente (regla 09 §4.8, "falta de firma del artefacto") |

## 3. SLSA

| Aspecto | Decisión |
| --- | --- |
| Nivel objetivo | SLSA L2 para la imagen del front |
| Criterios cumplidos en L2 | Build en servicio de CI hospedado (no en máquina del desarrollador), procedencia (provenance) generada y firmada de la imagen, fuente versionada con historial |
| Plan de elevación a L3 | Build con aislamiento reforzado y procedencia no falsificable; se evalúa alineado a la solución cuando el resto de los artefactos lo adopte |

La procedencia de la imagen liga el artefacto desplegado a su commit de origen y a su pipeline de construcción, de modo que un operador pueda verificar que la imagen en PROD proviene de la fuente esperada.

## 4. Dependency scanning (SCA)

| Aspecto | Decisión |
| --- | --- |
| Tooling | Analizador de composición de software del runtime objetivo, ejecutado en STAGE-11 del pipeline |
| Cobertura | Dependencias del front y capas base de la imagen de contenedor |
| Frecuencia | En cada PR y en cada build de `main`; además, escaneo programado periódico sobre la imagen publicada para detectar CVE nuevas en dependencias y capas base sin cambios |
| Automatización de actualizaciones | Bot de actualización de dependencias (Dependabot, Renovate u homólogo) que abre PR ante versiones con parche de seguridad |
| Política por severidad | Ver §6 (política de CVE) |

La política ante vulnerabilidad de dependencia: crítica y alta bloquean el merge (gate de SCA, STAGE-11) salvo excepción documentada con ADR y plan de remediación (alineado con DoD-release y `criterios-validacion_v1.0.md` §6); media y baja generan ticket sin bloquear.

## 5. SAST y DAST

| Análisis | Aplicabilidad | Stage / criterio de bloqueo |
| --- | --- | --- |
| SAST (análisis estático) | Aplica. El front tiene código propio de presentación, orquestación de UI y consumo del contrato | STAGE-01 (lint) y el analizador estático del gate "Análisis estático" (sin issues críticos) bloquean el merge |
| Escaneo de secretos en commits | Aplica. Intake §17.P.5 prohíbe secretos en commit | Escaneo de secretos sobre el historial y los PR; un secreto detectado bloquea el merge y dispara rotación (anti-patrón "secretos en commit") |
| DAST (análisis dinámico) | Aplica. El front es un servicio desplegado con superficie dinámica (render server-side, conexión de circuito, formularios de ingreso y de carga) | STAGE-15 del pipeline ejecuta el DAST sobre el front desplegado en DEV o QA; los hallazgos críticos bloquean la promoción a STAGING (`pipeline-ci-cd_v1.0.md` §1, §4) |

A diferencia de las librerías de la solución (cuya superficie dinámica es nula), el front sí tiene superficie dinámica: el DAST escanea el front desplegado contra clases de vulnerabilidad de aplicación web (inyección, exposición de datos sensibles, configuración insegura) sobre las vistas y la conexión del circuito. El alcance del DAST excluye al backend, que tiene su propio análisis dinámico en el pipeline de `geovial-api`.

## 6. Política de CVE

SLA de remediación por severidad, desde la detección hasta la imagen del front con el fix publicada:

| Severidad | SLA de remediación | Consecuencia en el pipeline | Comunicación |
| --- | --- | --- | --- |
| Crítica | 48 horas | Bloquea merge y release (STAGE-11, STAGE-15); detiene la promoción | Notificación inmediata al equipo |
| Alta | 7 días | Bloquea merge salvo excepción documentada con ADR y plan de remediación | Notificación al equipo; registro en el seguimiento |
| Media | 30 días | No bloquea; ticket con BT en el backlog de 06 | Registro en el seguimiento |
| Baja | 90 días o próxima ventana de mantenimiento | No bloquea; ticket | Registro en el seguimiento |

- El fix de una CVE se entrega reconstruyendo y re-desplegando la imagen del front, con la versión incrementada según SemVer (PATCH si es retrocompatible; `estrategia-versionado_v1.0.md` §1).
- No hay consumidores externos del artefacto (el front es un servicio, no un paquete): la comunicación de CVE es interna al equipo y al operador del despliegue.
- Toda excepción a un bloqueo requiere ADR explícita y plan de remediación con BT en 06 (regla 08 §4.7; `criterios-validacion_v1.0.md` §6).

## 7. Token de sesión y secretos

Tratamiento específico exigido por el insumo (intake §17.P.5; ADR-03):

- Token bearer del lado servidor. El front obtiene el token del backend presentando credenciales (CU-01) y lo retiene del lado servidor del circuito, en memoria, asociado a la sesión; nunca lo serializa al navegador (ADR-03; 05 §5, §7). El token no es un secreto de despliegue: vive solo durante la sesión y se descarta al cerrar sesión o reciclar el circuito. La no exposición se verifica de forma mecánica en el pipeline (STAGE-08, NFR custodia del token, TC-19): 0 exposiciones al navegador.
- Secretos del front. Las credenciales del front hacia `geovial-api` y la clave del proveedor del componente de mapa viven en el vault del ambiente, nunca en commit ni en la imagen (`entornos-deploy_v1.0.md` §4). Se inyectan en el arranque del contenedor y no quedan en logs; rotan periódicamente sin reconstruir la imagen.
- Frente del repositorio. El escaneo de secretos en commits (§5) cierra el frente del repositorio: un secreto detectado bloquea el merge y dispara rotación.
- Registros sin secretos. Ningún registro estructurado del front contiene credenciales ni el token (05 §7); el identificador de correlación de la traza no porta datos sensibles.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| SBOM y firma de la imagen | STAGE-12, STAGE-14 de `pipeline-ci-cd_v1.0.md` |
| SCA y SAST | STAGE-11, STAGE-01; gate "Análisis estático" de 08 |
| DAST sobre el front desplegado | STAGE-15 de `pipeline-ci-cd_v1.0.md`; ambientes DEV/QA |
| Token del lado servidor | ADR-03; NFR custodia del token (05 §8); STAGE-08; TC-19 (08) |
| Secretos en vault | Intake §17.P.5; `entornos-deploy_v1.0.md` §4 |
| Excepciones y SLA | DoD-release de 08; `criterios-validacion_v1.0.md` §6 |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Supply chain y seguridad inicial de geovial-web: SBOM de la imagen del front (dependencias y capas base) adjunto y firmado; firma de la imagen con registro de transparencia y verificación antes de promover; SLSA L2 objetivo con procedencia firmada; dependency scanning con SLA por severidad y escaneo programado de la imagen publicada; SAST, escaneo de secretos y DAST aplicable sobre el front desplegado en DEV/QA (a diferencia de las librerías); política de CVE crítica 48 h / alta 7 d / media 30 d / baja 90 d; y tratamiento específico del token bearer del lado servidor (no exposición verificada por STAGE-08/TC-19) y de los secretos en vault. |
