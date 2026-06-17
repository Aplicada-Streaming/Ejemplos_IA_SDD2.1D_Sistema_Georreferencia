# Guía de publicación del paquete de aplicación — geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** guia-publicacion-store-mobile_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps + Mobile Release Engineer

## 1. Objetivo y alcance

Esta guía describe cómo se empaqueta, firma y distribuye el paquete de aplicación Android de `geovial-mobile` por un canal de distribución interno. En v1 no se publica en una tienda pública (intake §17 P.7): la distribución es interna por los canales `internal`, `alpha`, `beta` y `production` (ver `entornos-deploy_v1.0.md` §1). La ruta de tienda pública queda documentada como destino futuro al final de esta guía (§6), sin habilitarse en v1.

El tipo de artefacto publicable es `store-mobile` (regla 09 §3.1, valor admitido), correspondiente al paquete de aplicación Android (`aab-android`). El proyecto no publica ningún otro tipo de artefacto, por lo que esta es la única guía de publicación de la sección (regla 09 §6).

## 2. Pre-requisitos y comando/stage de empaquetado y firma

### 2.1 Pre-requisitos

- Credencial de firma resguardada (almacén de claves de firma) disponible en el almacén seguro del pipeline, nunca en el repositorio (`supply-chain-seguridad_v1.0.md` §2; `entornos-deploy_v1.0.md` §4). Es el pre-requisito central: sin la credencial resguardada no se firma ni se distribuye.
- Acceso al canal de distribución interno con la clave de acceso correspondiente en el gestor de secretos del pipeline.
- Versión calculada por la herramienta de versión a partir del tag y de los Conventional Commits (`estrategia-versionado_v1.0.md` §4), con número de build Android monótono.
- Para la publicación manual de respaldo: el SDK fijado (.NET 8 LTS, target `net8.0-android`) y las herramientas de plataforma Android API 26+ instaladas localmente, con la credencial de firma inyectada desde el almacén seguro y nunca tecleada en texto plano.

### 2.2 Comando y stage de empaquetado y firma

El empaquetado y la firma son los stages Build y Firma del paquete del pipeline (`pipeline-ci-cd_v1.0.md` §3):

| Paso | Acción (comando abstracto) | Variables de entorno requeridas | Stage del pipeline |
| --- | --- | --- | --- |
| Empaquetar | Compilar y empaquetar el paquete de aplicación Android para el target `net8.0-android` en configuración de release | Versión calculada; identificador del host remoto del canal | Build |
| Firmar | Firmar el paquete con la credencial de firma resguardada (almacén de claves de firma) obtenida del almacén seguro | Referencia a la credencial de firma en el gestor de secretos | Firma del paquete |
| Distribuir | Subir el paquete firmado al canal interno correspondiente al sufijo del tag | Clave de acceso al canal de distribución interno; canal destino | Distribuir internal/alpha/beta/production |

El stage de distribución se ejecuta solo en tags (`pipeline-ci-cd_v1.0.md` §2): el sufijo del tag decide el canal (`-internal.N`, `-alpha.N`, `-beta.N`, o sin sufijo para `production`). El gate Firma del paquete de 08 §3 bloquea la distribución si la firma no es válida o no procede de la credencial resguardada.

## 3. Verificación post-distribución

Tras distribuir, se confirma que el paquete quedó disponible y es instalable y consumible:

1. Descargar el paquete distribuido desde el canal interno en el dispositivo de referencia Android API 26+ conectado por USB en modo desarrollador (intake §17 P.8/P.9).
2. Verificar la firma: la firma del paquete instalado corresponde a la credencial de firma resguardada; un paquete con firma distinta o ausente no se acepta.
3. Instalar y arrancar: el paquete instala y arranca hasta la pantalla de sesión/verificación dentro del objetivo de arranque en frío ≤ 3 s (NFR de 05 §8; TC-26).
4. Humo de campo: ejecutar el journey crítico mínimo (ingreso, selección de un relevamiento, creación de un marcador y captura de una foto offline) para confirmar que el paquete distribuido es consumible (08 `criterios-validacion_v1.0.md` §2).

La verificación post-distribución es el stage homónimo del pipeline (`pipeline-ci-cd_v1.0.md` §3), bloqueante para `production`.

## 4. Rollback (redistribución de la versión previa)

El paquete distribuido es inmutable por versión; el rollback redistribuye por el mismo canal interno la versión previa estable, no edita una versión ya distribuida (intake §17 P.8; `pipeline-ci-cd_v1.0.md` §7):

| Paso | Acción (comando abstracto) | Ventana de gracia |
| --- | --- | --- |
| 1. Pausar la versión rota | Pausar la promoción de la versión `X.Y.Z` afectada en el canal del panel de distribución para detener nuevas instalaciones | Inmediata al detectar la regresión |
| 2. Redistribuir la versión previa | Re-publicar por el mismo canal interno el paquete firmado de la versión previa estable `X.Y.(Z-1)`, ya almacenado como artefacto del pipeline (retención 180 días, `pipeline-ci-cd_v1.0.md` §5) | Dentro de la ventana de soporte del canal |
| 3. Comunicar | Notas de versión y aviso a los agentes de campo de actualizar a la versión previa; entrada en CHANGELOG (`estrategia-versionado_v1.0.md` §6) | Junto con la redistribución |
| 4. Publicar fix | PATCH `X.Y.(Z+1)` con el arreglo y nueva pasada completa por la suite y la firma antes de redistribuir | Según severidad |

La redistribución de la versión previa por el canal interno se ensaya al menos una vez antes de la primera distribución a `production`.

## 5. Métricas

Indicadores observables de la distribución, acotados a `tiene_observabilidad_critica = false` (no hay SLO de servicio que medir, intake §17 P.10):

| Métrica | Qué mide | Fuente |
| --- | --- | --- |
| Instalaciones por canal | Adopción de cada versión en `internal`/`alpha`/`beta`/`production` | Panel de distribución |
| Tasa de éxito de actualización | Porcentaje de dispositivos que actualizan sin error a la nueva versión | Panel de distribución |
| Tiempo hasta detección de regresión | Tiempo entre la distribución y el primer reporte de defecto o NFR fuera de objetivo | Reportes de campo + NFR de campo del pipeline |
| Vulnerabilidades detectadas post-distribución | CVE de dependencias detectadas tras distribuir | SCA del schedule semanal (`supply-chain-seguridad_v1.0.md` §4) |
| NFR de campo en dispositivo | Cola ≥ 1000, ciclo de 100 cambios ≤ 30 s, arranque ≤ 3 s en el dispositivo de referencia | Stage NFR de campo (TC-24, TC-25, TC-26) |

## 6. Destino futuro: ruta de tienda pública

En v1 no se habilita la publicación en una tienda pública (intake §17 P.7). Cuando se decida habilitarla, esta guía se versiona para incorporar: la cuenta de publicación de la tienda y sus credenciales en el almacén seguro, el flujo de revisión de la tienda como gate adicional previo a `production`, y la conciliación de los canales internos de prueba con los tracks de la tienda. La credencial de firma resguardada se reutiliza; el modelo de canales de §1 no cambia. Hasta entonces, la distribución es exclusivamente por el canal interno.

## 7. Trazabilidad

- El empaquetado, la firma y la distribución son los stages Build, Firma del paquete y Distribuir de `pipeline-ci-cd_v1.0.md` §3; el rollback es `pipeline-ci-cd_v1.0.md` §7.
- La credencial de firma resguardada y su política se detallan en `supply-chain-seguridad_v1.0.md` §2; los canales y su configuración, en `entornos-deploy_v1.0.md` §1/§3.
- La verificación post-distribución valida el NFR de arranque de 05 §8 (TC-26) y el journey crítico de 08 `criterios-validacion_v1.0.md` §2.

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Guía de publicación inicial del paquete de aplicación Android de geovial-mobile (tipo de artefacto store-mobile): pre-requisitos con la credencial de firma resguardada, comando/stage de empaquetado y firma con la credencial en almacén seguro, distribución por el canal interno (internal/alpha/beta/production) sin tienda pública en v1, verificación post-distribución con comprobación de firma y arranque ≤ 3 s, rollback por redistribución de la versión previa, métricas de distribución y ruta de tienda pública documentada como destino futuro. Derivado del intake §17 P.7/P.8/P.9/P.10, de 05 §5/§8 y de la regla 09 §2.2/§3.1 para mobile-app-maui. |
