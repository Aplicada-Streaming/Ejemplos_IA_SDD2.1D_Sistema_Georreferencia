# Guía de publicación — paquete-nuget — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** guia-publicacion-paquete-nuget_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps Senior (AG-09), variante DevOps + Release Engineer (library)

## 0. Sobre el tipo de artefacto

El artefacto de `aplicada-sync` es un paquete redistribuible publicado al feed declarado. `paquete-nuget` es un valor admitido de `<tipo-artefacto>` (09_rules §3.1) y se usa acá porque el proyecto efectivamente publica un paquete de ese gestor: es la librería integrable a .NET MAUI vía el repositorio en GitHub (intake §17 P.7/P.11; §13 `redistribuible: true`). El nombre del documento sigue el patrón parametrizado `guia-publicacion-<tipo-artefacto>_v<X.Y>.md`; el gestor concreto aparece solo en esta guía y en la tabla tipo-artefacto, nunca como vocabulario del dominio.

## 1. Pre-requisitos

| Requisito | Detalle | Scope mínimo |
| --- | --- | --- |
| Cuenta en el feed | Acceso al feed de paquetes declarado (GitHub Packages, intake §17 P.7) sobre el repositorio del proyecto en GitHub | Lectura/escritura de paquetes del repositorio |
| Token de publicación | Token de escritura al feed, almacenado en el secret manager del CI (`entornos-deploy_v1.0.md` §5) | Escritura de paquetes; sin permisos administrativos |
| Identidad de firma | Identidad para firmar el paquete y el SBOM (`supply-chain-seguridad_v1.0.md` §2) | Firma de artefacto |
| SDK del runtime | .NET 8 LTS con el workload de Android/MAUI (intake §17 P.9: `net8.0-android`) | Build reproducible |
| Tag de versión | Tag Git `vX.Y.Z[-prerelease]` generado por la herramienta de versión (`estrategia-versionado_v1.0.md` §5) | Disparo del trigger de publicación |

Configuración local opcional para publicación manual de emergencia: el mismo token con menor scope en un secret store local, nunca en el repositorio. La publicación normal es automatizada por el pipeline; la manual es excepcional y queda registrada.

## 2. Comando o stage de publicación

La publicación la ejecuta el stage Publish del pipeline (`pipeline-ci-cd_v1.0.md` §3), disparado por el tag de versión. El canal lo decide el sufijo del tag (`pipeline-ci-cd_v1.0.md` §6): prerelease -> `preview`; sin sufijo -> `stable`.

Secuencia del stage (comandos abstractos reproducibles):

1. Empaquetar el proyecto contra el target real (`net8.0-android`) con la versión inyectada por la herramienta de versión, produciendo el `.nupkg`:
   `dotnet pack --configuration Release -p:PackageVersion=<version-de-gitversion>`
2. Firmar el paquete y el SBOM adjunto (`supply-chain-seguridad_v1.0.md` §2):
   firma del `.nupkg` y registro en el transparency log.
3. Publicar al feed:
   `dotnet nuget push <paquete>.nupkg --source <URL-del-feed> --api-key <TOKEN_DE_PUBLICACION>`

Variables de entorno requeridas (definidas en `entornos-deploy_v1.0.md` §4/§5):

| Variable | Propósito |
| --- | --- |
| `URL del feed` | Destino del push |
| `TOKEN_DE_PUBLICACION` | Credencial de escritura, desde el secret manager del CI |
| `version-de-gitversion` | Versión calculada por la herramienta de versión, no manual |

Prerrequisitos del stage: gates verdes según el canal (`pipeline-ci-cd_v1.0.md` §6). A `stable` se exige además G5, G7 y G8 y la aprobación del Release manager.

## 3. Verificación post-publish

Confirma que el artefacto quedó publicado y es consumible. Es el gate G8 en su tramo de verificación post-publicación (08 estrategia-calidad §3; DoD release §1.4; intake §17 P.8; `contratos-abstractions_v1.0.md` §6/§8) y es bloqueante para `stable`.

1. Restaurar el paquete publicado en un proyecto limpio (un proyecto de consumo recién creado que referencia la versión recién publicada desde el feed):
   `dotnet restore` contra el feed declarado, resolviendo la versión `X.Y.Z`.
2. Reproducir el quick-start del contrato: ejecutar el escenario mínimo de integración (inicializar la sesión, encolar un cambio, ejecutar el ciclo subir-luego-bajar contra un doble) y comprobar que el comportamiento observable coincide con el contrato (`contratos-abstractions_v1.0.md` §6). Un quick-start que no reproduzca el contrato bloquea la publicación.
3. Comprobar la firma y el checksum: verificar la firma del `.nupkg` contra la identidad de firma y el transparency log (`supply-chain-seguridad_v1.0.md` §2), y el checksum del artefacto descargado.
4. Confirmar la presencia del SBOM adjunto al release y su firma.

Si cualquier paso falla, la versión no se considera publicada y se ejecuta el rollback (§4).

## 4. Rollback

El paquete publicado es inmutable; no se borra (los consumidores que ya lo restauraron lo conservan). El rollback es por `unlist` de la versión afectada más publicación de un fix (intake §17 P.8; `pipeline-ci-cd_v1.0.md` §7).

| Paso | Acción | Comando abstracto |
| --- | --- | --- |
| 1 | Unlist de la versión rota en el feed: deja de resolverse para nuevas restauraciones, sigue disponible para quien la fijó | `dotnet nuget delete <paquete> <X.Y.Z> --source <URL-del-feed> --api-key <TOKEN> --non-interactive` en su modo de unlist del gestor/feed |
| 2 | Publicar el fix: PATCH `X.Y.(Z+1)` si el arreglo es retrocompatible; MAJOR + guía de migración si es incompatible (ADR-03) | Stage Publish con el nuevo tag |
| 3 | Comunicar: CHANGELOG (Keep a Changelog), release notes y aviso a consumidores | `estrategia-versionado_v1.0.md` §6 (deprecation/comunicación) |

Ventana de gracia: la versión queda unlisted de inmediato al detectar la regresión; el fix se publica en cuanto pasa los gates. El procedimiento se ensaya al menos una vez antes del primer release `stable` (09_rules §5.4). La comunicación al consumidor sigue la política de la deprecation policy y la política de CVE cuando el motivo es una vulnerabilidad (`supply-chain-seguridad_v1.0.md` §6).

## 5. Métricas

Indicadores observables del paquete publicado, base de las notificaciones del pipeline (`pipeline-ci-cd_v1.0.md` §8):

| Métrica | Qué mide | Fuente |
| --- | --- | --- |
| Descargas por versión | Adopción de cada versión en el feed | Estadísticas del feed |
| Tasa de adopción de la última `stable` | Proporción de consumidores en la versión recomendada | Estadísticas del feed; reportes de consumidores |
| Vulnerabilidades detectadas post-publish | CVE descubiertas en el paquete o sus dependencias tras publicar | SCA y dependency scanning (`supply-chain-seguridad_v1.0.md` §4) |
| Tiempo medio hasta detección de regresión | Latencia entre publicar una versión y detectar un defecto en ella | Reportes de consumidores; alertas de NFR del pipeline |
| Cumplimiento de compatibilidad | 100 % de cambios incompatibles con incremento MAJOR; 0 remociones sin deprecación | Auditoría del changelog contra la matriz de compatibilidad (ADR-03 §8) |

## 6. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Tipo de artefacto y feed | intake §17 P.7/P.11; §13 (`redistribuible: true`) |
| Stage de publicación | `pipeline-ci-cd_v1.0.md` §3/§6 |
| Canales y aprobador | `entornos-deploy_v1.0.md` §2/§6 |
| Verificación post-publish (gate) | G8 (08 estrategia-calidad §3); DoD release §1.4; contratos §6; intake §17 P.8; BT-14 |
| Versionado y comunicación | `estrategia-versionado_v1.0.md`; ADR-03 |
| Firma y SBOM | `supply-chain-seguridad_v1.0.md` §1/§2 |

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Guía de publicación inicial del paquete redistribuible Aplicada.Sync al feed declarado (GitHub Packages), nombrada con el patrón parametrizado guia-publicacion-paquete-nuget. Cubre pre-requisitos (cuenta, token con scope mínimo, identidad de firma, SDK .NET 8 LTS con workload Android, tag de versión), comando/stage de publicación con canal decidido por el sufijo del tag, verificación post-publish por restauración en proyecto limpio y reproducción del quick-start (gate G8; intake §17 P.8), rollback por unlist más fix con su comando, y métricas observables. Derivada del intake §17 P.7/P.8/P.11, de ADR-03 y de contratos-abstractions §6. |
