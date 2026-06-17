# Pipeline CI/CD — geovial-storage

**Proyecto:** geovial-storage
**Documento:** pipeline-ci-cd_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps Senior (variante DevOps + Release Engineer, library)

## 0. Alcance y modelo

`geovial-storage` es una librería de tipo `library` que no se publica como paquete redistribuible (intake §13, §17.P.7; ADR-03). Se integra al backend consumidor `geovial-api` y se construye junto a él. En consecuencia, este pipeline no tiene un stage de publicación a un feed propio: el artefacto que viaja al canal es la imagen del backend que integra la librería; la firma y el SBOM se producen sobre ese artefacto del backend, con la librería ya embebida. El detalle de stages internos de la imagen del backend pertenece al `pipeline-ci-cd` de `geovial-api`; aquí se documentan los stages que validan la librería y el punto en el que su build se incorpora al del backend.

Por ser una solución multi-proyecto, el orden de construcción inter-proyecto (la librería de nivel 0 se construye antes que el backend de nivel 1 que la consume) se gobierna en `_solucion/pipeline-solucion_v1.0.md`; este documento asume ese orden y describe el pipeline del proyecto.

Cada quality gate ejecuta un criterio de la Definition of Done canónica de 08 (`definition-of-done_v1.0.md`) o verifica una NFR de 05 (`arquitectura-solucion_v1.0.md` §8). La DoD no se redefine aquí: se ejecuta como conjunto de gates (regla 08 §4.8). Los gates G-01 a G-09 provienen de `estrategia-calidad_v1.0.md` §3.

## 1. Stages obligatorios

Cada stage declara su tooling por rol abstracto (la herramienta concreta del runtime objetivo se fija en la guía de reproducción local de 10), su quality gate y el criterio de 08/05 que verifica.

| Stage | Tooling (rol abstracto) | Quality gate | Criterio 08 / NFR 05 | Bloqueante |
| --- | --- | --- | --- | --- |
| STAGE-01 Lint | Linter y formateador del runtime objetivo | 0 warnings nuevos de formato | DoD-BT (análisis estático), G-09 | Sí en PR |
| STAGE-02 Build | Compilador del runtime objetivo | 0 errores; 0 warnings tratados como error | G-01; DoD-BT "compila sin warnings tratados como error" | Sí |
| STAGE-03 Test unit | Framework de tests unitarios | Suite unitaria en verde | G-02; DoD-sprint "suite del tramo en verde" | Sí |
| STAGE-04 Test contract | Corredor de contract tests parametrizado por proveedor | Batería de contrato única equivalente contra cada proveedor soportado | G-06; DoD-release "batería de contrato pasa contra cada proveedor"; NFR-03 transparencia | Sí |
| STAGE-05 Cobertura | Medidor de cobertura segmentado por capa | Global ≥ 80 % líneas / ≥ 70 % branches; dominio 85/80; infraestructura 70/60 | G-03, G-04; DoD-sprint cobertura; NFR-06 | Sí |
| STAGE-06 Mutation | Framework de mutation testing | Mutation score de dominio ≥ 60 % | G-05; DoD-BT mutation | Bloquea release; informativo en feature branch |
| STAGE-07 No filtración de credenciales | Property-based test + analizador estático | 0 ocurrencias de credenciales o parámetros de conexión en resultados, errores y registros | G-07; DoD-BT credenciales; NFR-05; RN-03; ADR-05 | Sí |
| STAGE-08 NFR latencia | Banco de medición de desempeño | p95 ≤ 1 s para archivos ≤ 5 MB con proveedor local | G-08; DoD-release NFR-01; NFR-01 | Bloquea release |
| STAGE-09 SCA | Analizador de composición de software | 0 CVE críticas; 0 altas sin excepción documentada | DoD-release calidad de código; ver `supply-chain-seguridad_v1.0.md` §4 | Sí |
| STAGE-10 SBOM | Generador de SBOM (formato estándar de inventario) | SBOM de la librería generado y propagado al SBOM del artefacto del backend | Supply chain §1; DoD-release | Sí |
| STAGE-11 Integración al backend | Builder de la imagen del backend (`geovial-api`) | La librería se referencia y compila dentro del build del backend; gate de integración de solución en verde | DoD-release; `pipeline-solucion_v1.0.md` §6 | Sí |
| STAGE-12 Firma del artefacto del backend | Firmador de artefactos con registro de transparencia | Firma válida y registrada sobre la imagen del backend que integra la librería | Supply chain §2; DoD-release | Sí (en release del backend) |

Notas:

- No hay stage `Publish` propio: la librería no tiene canal externo. El "publish" efectivo es la publicación de la imagen del backend, descrita en el `pipeline-ci-cd` de `geovial-api` y referenciada en `entornos-deploy_v1.0.md`.
- STAGE-08 (NFR-01) puede medirse en CI como aproximación mientras el ambiente equivalente al productivo no esté disponible (GAP-03 de `criterios-validacion_v1.0.md` §6), con ratificación obligatoria antes del release del backend.
- STAGE-04 contra el proveedor remoto puede diferirse al Tramo 5 porque el adaptador remoto es Should (GAP-02); hasta entonces la transparencia se valida contra el proveedor local y el doble en memoria.

## 2. Matriz de sistema operativo y runtime

La librería se ejecuta dentro del proceso del backend, sobre el runtime del backend (.NET 8 LTS sobre contenedor Linux, intake §17.P.9). La matriz cubre el destino real (Linux del contenedor del backend) y, en PR, una verificación cruzada para detectar diferencias de separador de rutas y fin de línea que afectan al proveedor de almacenamiento local.

| Trigger | Sistemas operativos | Runtime | Justificación |
| --- | --- | --- | --- |
| PR a `main` | linux, windows | .NET 8 LTS | La librería manipula rutas y archivos del proveedor local; la matriz cruzada detecta diferencias de path separator y line endings antes del merge |
| Merge a `main` / tag de integración | linux | .NET 8 LTS | El destino productivo es el contenedor Linux del backend; se valida en el mismo runtime en el que se desplegará |

La matriz se justifica por cobertura del consumidor real (el contenedor del backend es Linux) contra el costo de minutos de CI: windows solo se ejecuta en PR para la verificación del proveedor local.

## 3. Caché y artefactos

| Elemento | Política | Llave de caché | Retención |
| --- | --- | --- | --- |
| Caché de dependencias del runtime | Restaurar antes de build; invalidar al cambiar el manifiesto de dependencias | Hash del manifiesto de dependencias del runtime objetivo | Hasta cambio de la llave |
| Reporte de cobertura segmentado | Artefacto de CI por corrida | — | 30 días |
| Reporte de mutation testing | Artefacto de CI por corrida de release | — | 90 días |
| SBOM de la librería | Artefacto propagado al SBOM del backend | — | Vida del release del backend |
| Reporte de SCA | Artefacto de CI por corrida | — | 90 días |
| Evidencia de medición NFR-01 | Artefacto de CI de release | — | 1 año |

La librería no produce un paquete redistribuible como artefacto retenible: su salida es el ensamblado embebido en la imagen del backend. El único artefacto firmado y adjunto al release es la imagen del backend (ver §1 STAGE-12 y `supply-chain-seguridad_v1.0.md` §2).

## 4. Promotion rules

El modelo de la librería no tiene canales externos (preview/stable) ni ambientes propios (DEV/QA/STAGING/PROD): se integra al backend y hereda su ciclo (ADR-03, `entornos-deploy_v1.0.md`). La promoción que aplica es la del backend; aquí se declaran los gates que deben estar en verde para que el build de la librería se incorpore al del backend.

| Transición | Trigger | Prerequisitos (gates) |
| --- | --- | --- |
| Feature branch → `main` (merge) | PR aprobado con suite verde | G-01, G-02, G-03, G-04, G-06, G-07, G-09 en verde (gates bloqueantes de merge de 08 §3) |
| `main` → integración al backend | Merge a `main` o tag de integración alineado al backend | Todos los anteriores + STAGE-10 SBOM generado |
| Integración → release del backend | Tag de release del backend (alineado al ciclo del backend, ADR-03) | G-05, G-08 en verde (gates bloqueantes de release) + STAGE-11 integración verde + STAGE-12 firma del backend |

No hay aprobación humana entre la librería y la integración: el gate es la suite verde, la cobertura cumplida y el SBOM presente. La aprobación humana para promover a producción vive en la promoción del backend (`entornos-deploy_v1.0.md` §5), no en la librería.

## 5. Rollback

La librería no se publica a un feed, por lo que no hay delist ni unlist propio. El rollback de un defecto introducido por la librería se ejecuta por reversión de la imagen del backend que la integra (intake §17.P.8; ADR-05 delega el mecanismo físico a 09).

| Paso | Acción | Comando o procedimiento concreto |
| --- | --- | --- |
| 1 | Identificar la versión del backend que integró la librería defectuosa | Leer el tag del release del backend y la versión de la librería embebida (alineadas por ADR-03) |
| 2 | Revertir la imagen del backend a la versión previa estable | Re-desplegar la imagen del backend con el tag inmediatamente anterior verde (procedimiento operativo en el `pipeline-ci-cd` de `geovial-api`) |
| 3 | Revertir el cambio de código en `main` | `git revert <sha-del-commit>` del cambio de la librería; abrir PR de reversión con la suite en verde |
| 4 | Publicar el fix | Corregir, incrementar la versión según SemVer (PATCH si es fix retrocompatible; MAJOR + coordinación con `geovial-api` si es breaking, ADR-03), reconstruir la imagen del backend y promover |
| 5 | Comunicar | Registrar en el CHANGELOG de la librería y en las notas de release del backend; comunicar al consumidor `geovial-api` |

El rollback no requiere migrar los archivos ya almacenados: el cambio de versión de la librería no altera los identificadores lógicos a través de versiones menores (ADR-03; CU-01). Un rollback que afecte el contrato (versión mayor) requiere coordinación con `geovial-api` y guía de migración.

## 6. Notificaciones

| Evento | Canal | Severidad | Destinatario |
| --- | --- | --- | --- |
| Falla de gate bloqueante de merge (G-01..G-04, G-06, G-07, G-09) | Canal del equipo + estado del PR | Alta | Desarrollador autor del PR |
| Falla de gate de release (G-05, G-08) | Canal del equipo + bloqueo del release del backend | Alta | Desarrollador en rol QA |
| CVE crítica/alta detectada por SCA | Canal del equipo + ticket de remediación | Crítica/Alta | Desarrollador; ver SLA en `supply-chain-seguridad_v1.0.md` §6 |
| Falla de firma o SBOM del artefacto del backend | Canal del equipo + bloqueo del release del backend | Alta | Desarrollador en rol QA |
| Build verde integrado al backend | Canal del equipo (informativo) | Informativa | Equipo |

Con `equipo_n=1` (intake §2), el desarrollador asume los roles de autor, SDET y QA; las notificaciones se concentran en un único canal de equipo con el estado del PR y del release del backend visible en el tablero.

## 7. Reproducción local

Cada stage es reproducible en máquina local con las mismas versiones del runtime objetivo (.NET 8 LTS). Los comandos exactos por stage los publica la developer guide de 10; este documento fija qué se ejecuta y en qué orden, no el comando concreto del runtime. Anti-patrón evitado: pipeline irreproducible localmente (regla 09 §4.8).

## 8. Trazabilidad

| Stage / gate | Criterio 08 (DoD/gate) | NFR / RN 05 | ADR |
| --- | --- | --- | --- |
| STAGE-02 / G-01 | DoD-BT compila sin warnings-as-errors | — | — |
| STAGE-03 / G-02 | DoD-sprint suite en verde | — | — |
| STAGE-04 / G-06 | DoD-release batería de contrato por proveedor | NFR-03, RN-01 | ADR-01, ADR-04 |
| STAGE-05 / G-03, G-04 | DoD-sprint cobertura global; DoD-BT cobertura por capa | NFR-06 | ADR-01 |
| STAGE-06 / G-05 | DoD-BT mutation dominio | — | — |
| STAGE-07 / G-07 | DoD-BT no filtración de credenciales | NFR-05, RN-03 | ADR-05 |
| STAGE-08 / G-08 | DoD-release NFR-01 | NFR-01 | ADR-01, ADR-04 |
| STAGE-09 SCA | DoD-release calidad de código | — | — |
| STAGE-10 SBOM, STAGE-12 firma | DoD-release supply chain | — | — |
| STAGE-11 integración | Gate de integración de solución | — | ADR-03 |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Pipeline CI/CD inicial de geovial-storage: doce stages (lint, build, unit, contract, cobertura, mutation, no filtración, NFR latencia, SCA, SBOM, integración al backend y firma del artefacto del backend) que ejecutan los gates G-01 a G-09 de 08 como verificación de la DoD, matriz SO/runtime alineada a .NET 8 Linux, caché y artefactos sin paquete redistribuible, promotion alineada al ciclo del backend, rollback por reversión de la imagen del backend y notificaciones para equipo_n=1. Sin stage de publicación externa por ser library no redistribuible (intake §17.P.7). |
