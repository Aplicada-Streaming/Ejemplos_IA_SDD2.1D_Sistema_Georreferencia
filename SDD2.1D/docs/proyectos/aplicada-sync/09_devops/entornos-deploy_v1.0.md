# Entornos y distribución — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** entornos-deploy_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps Senior (AG-09), variante DevOps + Release Engineer (library)

## 1. Modelo de distribución

`Aplicada.Sync` es de tipo `library` y redistribuible. Su modelo de ambientes son canales de distribución sobre un feed único (`preview` y `stable`), no ambientes desplegables DEV/QA/STAGING/PROD (regla §2.2 y anti-patrón 4.8 "confundir publicación con despliegue"). La librería no es una unidad de despliegue autónoma: corre embebida en el proceso de la aplicación host (`arquitectura-solucion_v1.0.md` §5). No hay servicio, contenedor ni URL propia; lo que se "promueve" es la disponibilidad de una versión del paquete en un canal del feed.

El feed declarado es GitHub Packages (intake §17 P.7, ratificable), repositorio en GitHub (intake §17 P.7/P.11). El detalle de publicación al feed vive en `guia-publicacion-paquete-nuget_v1.0.md`.

## 2. Canales

| Canal | Destino | Tags que lo alimentan | Aprobador | SLA / ventana |
| --- | --- | --- | --- | --- |
| `preview` | Feed de paquetes, versiones con sufijo de prerelease | `vX.Y.Z-alpha.N`, `-beta.N`, `-rc.N` | Automático | Sin SLA; uso de validación e integración temprana |
| `stable` | Feed de paquetes, versiones release | `vX.Y.Z` sin sufijo | Release manager (AG-07, Maintainer Lead del RACI de 08 §4) | Sin SLA de servicio (no es un servicio); ventana de verificación post-publish previa a anunciar la versión |

Criterios de uso:

- `preview`: lo consumen integradores que quieren probar una versión candidata antes del release, y el sample de demostración MAUI ajeno a la solución (intake §18) durante su validación en la categoría 11. No ofrece garantía de compatibilidad entre prereleases sucesivos.
- `stable`: la versión recomendada para consumo productivo; respeta la política de compatibilidad de la superficie pública (ADR-03; `contratos-abstractions_v1.0.md` §6) y pasó la verificación post-publish.

No aplican NFR de disponibilidad ni de latencia de ambiente: la librería no expone un endpoint ni un SLA operativo. Los NFR numéricos del proyecto (05 §8: tiempo de lote, capacidad de cola, reanudación, idempotencia, orden, continuidad ante conflicto) son atributos del motor que el pipeline verifica como gate G7 antes de promover a `stable` (`pipeline-ci-cd_v1.0.md` §3), no SLA de un ambiente desplegado.

## 3. Provisión (IaC)

La librería no provisiona infraestructura propia (no hay servidores, clústeres ni bases de datos que aprovisionar; `arquitectura-solucion_v1.0.md` §5). La única infraestructura es la configuración del feed y de la automatización de CI/CD, que se gestiona como configuración versionada en el repositorio (workflow del pipeline y configuración del feed como código). No se usa Terraform, Pulumi ni equivalente porque no hay recursos de nube que declarar para este proyecto; si una versión futura agregara un feed espejo o infraestructura de firma dedicada, se incorporaría su IaC y se versionaría este documento.

## 4. Configuración 12-factor

La configuración de la sesión de sincronización la arma la aplicación host, no la librería (`arquitectura-solucion_v1.0.md` §7, cross-cutting). Para el pipeline y la publicación, la configuración sigue 12-factor: vive en variables de entorno o en la configuración del workflow referenciada, nunca en el código del paquete.

| Variable (rol abstracto) | Propósito | Ámbito |
| --- | --- | --- |
| URL del feed de paquetes | Destino de publicación y de restauración | CI; configuración local opcional |
| Token de publicación al feed | Credencial de escritura al feed | CI (secreto); ver §5 |
| Versión del SDK del runtime | Fijar .NET 8 LTS para build reproducible | CI; configuración local |
| Canal objetivo (`preview` / `stable`) | Derivado del sufijo del tag, no es una variable manual | CI |

El motor no almacena credenciales de dominio: recibe una credencial vigente de un proveedor inyectado por el host y la usa solo durante la fase que la requiere (`arquitectura-solucion_v1.0.md` §7). Esa credencial es responsabilidad del host, no de este pipeline.

## 5. Secretos

| Secreto | Almacén | Scope mínimo | Rotación |
| --- | --- | --- | --- |
| Token de publicación al feed | Secret manager del CI (GitHub Secrets u homólogo) | Escritura de paquetes al feed del repositorio, sin permisos administrativos | Periódica y ante sospecha de exposición |
| Clave o identidad de firma del artefacto y del SBOM | Secret manager del CI o keyless con transparency log (ver `supply-chain-seguridad_v1.0.md` §2) | Firma del paquete y del SBOM | Según política de la identidad de firma |

Prohibición explícita: ningún secreto se commitea al repositorio (anti-patrón 4.8 "secretos en commit"). El scan de secretos en commits es parte del dependency/secret scanning (`supply-chain-seguridad_v1.0.md` §4). Los tokens viven en el secret manager del CI con el menor scope posible y rotan con frecuencia declarada.

## 6. Promoción

La promoción entre canales está integrada al pipeline (`pipeline-ci-cd_v1.0.md` §6):

- A `preview`: automática desde un tag de prerelease, con G1-G6 y G9 verdes, SBOM presente y firma válida. Sin aprobador humano.
- A `stable`: requiere, además, G5 (mutation), G7 (NFR) y G8 (compatibilidad) verdes y verificación post-publish exitosa. Aprobador: Release manager, con registro auditable del aprobador y del tag (anti-patrón 4.8 "promotion sin aprobador humano").
- La promoción no reusa el binario de `preview`: cada canal se alimenta de su propio tag y vuelve a pasar la suite (ver `estrategia-versionado_v1.0.md` §6). El registro de auditoría es la corrida de CI ligada al tag, al aprobador y al artefacto firmado.

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Modelo de canales (no ambientes) | Regla §2.2 (library); anti-patrón 4.8 "confundir publicación con despliegue" |
| Feed y herramienta | intake §17 P.7; `guia-publicacion-paquete-nuget_v1.0.md` |
| Gate previo a `stable` | G5, G7, G8 (08 estrategia-calidad §3); `pipeline-ci-cd_v1.0.md` §6 |
| NFR del motor (no SLA de ambiente) | 05 §8; verificados por G7 |
| Secretos y configuración | `supply-chain-seguridad_v1.0.md` §2/§4; 12-factor de este documento §4 |
| Downstream | examples de 11 (consumen los canales declarados); developer guide de 10 |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Modelo de distribución inicial de aplicada-sync por canales preview/stable sobre feed único (GitHub Packages, intake §17 P.7), no por ambientes desplegables, conforme a la regla §2.2 para library y al anti-patrón de no confundir publicación con despliegue. Declara aprobador por canal (automático en preview, Release manager en stable), aclara que no aplican SLA de disponibilidad/latencia de ambiente sino NFR del motor verificados por G7, configuración 12-factor de CI, secretos en secret manager con rotación y prohibición de commit, y promoción integrada al pipeline con registro auditable. Derivado del intake §17 P.7 y de 05 §5/§8. |
