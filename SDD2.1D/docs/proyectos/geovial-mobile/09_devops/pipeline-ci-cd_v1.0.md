# Pipeline CI/CD — geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** pipeline-ci-cd_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps + Mobile Release Engineer

## 1. Objetivo y alcance

Este documento define el pipeline de integración y entrega continua de la app móvil de campo `geovial-mobile`. El artefacto que el pipeline construye, firma y distribuye es un paquete de aplicación para la plataforma Android, distribuido por un canal de distribución interno (no se publica en tienda pública en v1, intake §17 P.7). No hay servicio desplegable ni ambientes DEV/QA/STAGING/PROD: el modelo de promoción es por canales de distribución móvil `internal`, `alpha`, `beta` y `production` (regla 09 §2.2 para `mobile-app-maui`; detalle en `entornos-deploy_v1.0.md`).

Los gates de prueba del pipeline ejecutan la Definition of Done de `08_calidad_y_pruebas/definition-of-done_v1.0.md` y la cobertura por capa de `08_calidad_y_pruebas/estrategia-testing_v1.0.md` §2; este documento no las redefine. Los quality gates referenciados son los de `08_calidad_y_pruebas/estrategia-calidad_v1.0.md` §3 y se materializan acá como stages. Cada stage referencia el criterio DoD de 08 o el NFR de 05 que verifica.

## 2. Triggers

Triggers explícitos por evento (anti-patrón 09 §4.8 "trigger único y opaco" evitado): el PR no distribuye nada; solo los tags distribuyen, y el sufijo del tag decide el canal (ver §6).

| Trigger | Evento | Stages que ejecuta | Distribuye |
| --- | --- | --- | --- |
| PR a `main` | Apertura o actualización de un PR | Lint, Build, Test unit, Test offline-sync, Test UI móvil, Test snapshot, Análisis estático | No |
| Push a `main` | Merge de un PR (squash con Conventional Commits) | Todos los de PR + Cobertura + SCA + SBOM | No |
| Tag `vX.Y.Z-internal.N` | Tag de la herramienta de versión para el canal interno de prueba | Todos + Firma + Distribuir internal | Canal `internal` |
| Tag `vX.Y.Z-alpha.N` / `-beta.N` | Tag de prerelease | Todos + NFR de campo + Firma + Distribuir alpha/beta | Canal `alpha` / `beta` |
| Tag `vX.Y.Z` sin sufijo | Tag estable | Todos + NFR de campo + Firma + Distribuir production + Verificación post-distribución | Canal `production` |
| Schedule semanal | Cron de mantenimiento | SCA, dependency scanning, regeneración del aviso de CVE | No |

El stage NFR de campo (cola ≥ 1000, ciclo de 100 cambios ≤ 30 s, arranque ≤ 3 s) exige el dispositivo de referencia y se ejecuta en los tags que alimentan canales de prueba y de producción, no en cada PR, por el costo del ciclo de distribución del paquete y la dependencia del dispositivo (08 `matriz-cobertura-pruebas_v1.0.md` §6).

## 3. Stages obligatorios

Cada stage declara su comando abstracto, su tooling por rol y su criterio de éxito ligado a un quality gate de 08, a un criterio DoD o a un NFR de 05.

| Stage | Tooling (rol abstracto) | Quality gate | Criterio DoD / NFR verificado | Bloqueante |
| --- | --- | --- | --- | --- |
| Lint | Formateador y linter del runtime objetivo | 0 issues de formato nuevos | DoD BT §1.2 (compila sin warnings tratados como error, parcial) | Sí en PR |
| Build | Compilador del runtime objetivo; empaquetado del paquete de aplicación Android | Gate Compilación de 08 §3: compila sin warnings tratados como error | DoD BT §1.2; DoD US §1.1 (la suite compila) | Sí |
| Test unit | Framework de pruebas unitarias del runtime objetivo, con dobles de los adaptadores de plataforma | Gate Pruebas en verde de 08 §3 (parte unitaria): suite unitaria de captura, cola y orquestación de sincronización verde | DoD US §1.1, DoD BT §1.2 (suite unitaria verde sin dispositivo ni red) | Sí |
| Test offline-sync | Doble del adaptador de conectividad y doble del backend (contrato subir-luego-bajar) | Gate Pruebas en verde de 08 §3 (parte sincronización): cola, reanudación y convivencia con conflicto verdes (TC-18, TC-19, TC-20, TC-21) | DoD US §1.1, DoD BT §1.2; RN-02, RN-03, RN-05 | Sí |
| Test UI móvil | Framework de pruebas de interfaz móvil sobre el journey crítico | Gate Pruebas en verde de 08 §3 (parte interfaz): journey crítico verde (TC-05, TC-06, TC-28) | DoD US §1.1 (camino degradado y happy path) | Sí |
| Test snapshot | Framework de snapshot de vistas de las pantallas críticas | Gate Snapshot de pantallas críticas de 08 §3: render coincide con su baseline aprobado (TC-27) | DoD US §1.1, DoD tramo §1.3, DoD release §1.4 (snapshot sin diferencias no aprobadas) | Sí |
| Cobertura | Reporte de cobertura por capa del runtime | Gate Cobertura global de 08 §3 (líneas ≥ 80 % / branches ≥ 70 %) y Gate Cobertura por capa de 08 §3 (lógica ≥ 75 %, infraestructura ≥ 70 %, presentación ≥ 60 %); el global no compensa una capa bajo su piso | DoD US §1.1, DoD tramo §1.3, DoD release §1.4; intake §17 P.6 | Sí |
| NFR de campo | Doble de cola determinista; medidor de tiempo de ciclo y de arranque sobre el dispositivo de referencia | Gate NFR de release de 08 §3: captura 100 % offline (TC-08, TC-12); cola ≥ 1000 (TC-24); ciclo de 100 cambios ≤ 30 s (TC-25); reanudación sin pérdida (TC-19); arranque ≤ 3 s (TC-26) | NFR de 05 §8 (captura offline, capacidad de cola, tiempo de ciclo, reanudación, arranque); DoD release §1.4 | Bloquea release |
| SCA | Herramienta de software composition analysis del runtime | 0 CVE críticas, 0 altas sin excepción registrada | Política de CVE (ver `supply-chain-seguridad_v1.0.md` §6) | Sí |
| SBOM | Generador CycloneDX o SPDX | SBOM JSON generado, firmado y adjunto al release | DoD release §1.4 (artefacto trazable); supply chain §1 | Sí en release |
| Análisis estático | Analizador estático del runtime | Gate Análisis estático de 08 §3: sin issues críticos nuevos | DoD BT §1.2, DoD US §1.1, DoD release §1.4 | Sí |
| Firma del paquete | Firma del paquete de aplicación con la credencial de firma resguardada en almacén seguro (almacén de claves de firma) | Gate Firma del paquete de 08 §3: firma válida con la credencial resguardada | DoD release §1.4; supply chain §2 | Sí en distribución |
| Distribuir internal/alpha/beta | Comando de distribución al canal interno por el panel de distribución | Paquete disponible en el canal de prueba correspondiente | Ver `guia-publicacion-store-mobile_v1.0.md` §2 | Solo en tag `-internal`/`-alpha`/`-beta` |
| Distribuir production | Comando de distribución al canal `production` | Paquete disponible en el canal `production` | Ver `guia-publicacion-store-mobile_v1.0.md` §2 | Solo en tag `vX.Y.Z` |
| Verificación post-distribución | Instalación de prueba del paquete distribuido en el dispositivo de referencia y comprobación de firma | El paquete instala, arranca y su firma corresponde a la credencial resguardada | DoD release §1.4; ver `guia-publicacion-store-mobile_v1.0.md` §3 | Sí en production |

El listado de stages cubre el conjunto obligatorio de la regla 09 §4.2 (lint, build, test, SCA, SBOM, firma, publish), con la pirámide de testing de 08 representada por un stage por nivel: Test unit (unitario), Test offline-sync y Test UI móvil (integración y extremo a extremo) y Test snapshot (snapshot de pantallas críticas), conforme a `estrategia-testing_v1.0.md` §1.

## 4. Matriz de plataforma y runtime

El target único de la app es la plataforma Android (intake §17 P.9: `net8.0-android`; versión mínima de Android API 26; sin iOS ni Windows en v1).

| Trigger | Sistema operativo del runner | Plataforma / target de empaquetado | Justificación |
| --- | --- | --- | --- |
| PR a `main` | linux | Compilación y suite del núcleo lógico; empaquetado del paquete de aplicación Android | Linux cubre build y suite a menor costo de minutos; la lógica de captura y sincronización no depende del SO de build |
| Push a `main` | linux | Igual que PR + cobertura + SCA + SBOM | Mismo runner; agrega inventario de dependencias |
| Tag de canal de prueba o de producción | linux para build y firma; dispositivo de referencia Android API 26+ conectado para el stage NFR de campo | Empaquetado del paquete de aplicación Android firmado; medición de NFR en dispositivo | El paquete se compila contra el target real del consumidor; los NFR de arranque y de ciclo se miden en el dispositivo de referencia (08 `estrategia-testing_v1.0.md` §7) |

Justificación de la matriz: la cobertura de consumidores reales es un único target de plataforma (Android API 26 y superiores). Una matriz cruzada de sistemas operativos o de múltiples runtimes no aporta valor frente al costo de minutos de CI mientras el alcance sea Android únicamente. Si una versión futura agrega otra plataforma, la matriz se amplía con un target por plataforma y se versiona este documento.

## 5. Caché y artefactos

Política de caché del gestor de dependencias del runtime:

| Caché | Llave | Expiración | Notas |
| --- | --- | --- | --- |
| Paquetes restaurados del gestor | Hash del archivo de lock de dependencias | 7 días o invalidación por cambio del lock | Acelera la restauración; se reconstruye al cambiar dependencias |
| Componentes de la plataforma Android (SDK y herramientas de build) | Versión del SDK + versión de las herramientas de plataforma | Por versión del SDK | Evita reinstalar las herramientas de plataforma en cada corrida |
| Artefactos intermedios de build | Hash del árbol de fuentes del proyecto | Por corrida | Reuso entre stages de la misma corrida |

Artefactos producidos por stage y su retención:

| Artefacto | Stage productor | Retención |
| --- | --- | --- |
| Reporte de cobertura por capa | Cobertura | 90 días |
| Reporte de NFR de campo (cola, ciclo, arranque) | NFR de campo | 90 días |
| Reporte de SCA y de dependency scanning | SCA / Schedule | 180 días |
| SBOM (CycloneDX/SPDX JSON, firmado) | SBOM | Permanente, adjunto al release |
| Paquete de aplicación Android firmado | Build + Firma del paquete | Permanente en el canal interno; copia de artefacto 180 días |
| Baselines de snapshot de pantallas críticas | Test snapshot | Versionado con el código (no como artefacto efímero) |
| Registro de firma y referencia a la credencial usada | Firma del paquete | Permanente, adjunto al release |

La credencial de firma (almacén de claves de firma) no es un artefacto del pipeline: vive en el almacén seguro y nunca se publica como artefacto ni se versiona en el repositorio (ver `supply-chain-seguridad_v1.0.md` §2 y `entornos-deploy_v1.0.md` §4).

## 6. Promotion rules (canales internal → alpha → beta → production)

El modelo de promoción es por canales de distribución móvil, no por ambientes de servicio (regla 09 §2.2 para `mobile-app-maui`). Detalle de cada canal en `entornos-deploy_v1.0.md`.

- `internal` se distribuye automáticamente desde tags con sufijo `-internal.N`, generados por la herramienta de versión a partir de Conventional Commits (ver `estrategia-versionado_v1.0.md` §3). Aprobador: automático. Prerrequisito: gates Compilación, Pruebas en verde, Cobertura global y por capa, Análisis estático y Snapshot verdes, SBOM presente y firma válida. Audiencia: el equipo y validación interna.
- `alpha` se distribuye desde tags `-alpha.N`. Aprobador: Mobile Release Engineer (el rol DevOps en `equipo_n=1`). Prerrequisito: además de los gates de `internal`, el stage NFR de campo verde. Audiencia: probadores de campo acotados.
- `beta` se distribuye desde tags `-beta.N`. Aprobador: Mobile Release Engineer. Prerrequisito: igual que `alpha` más al menos un ciclo de `alpha` sin defecto blocker abierto. Audiencia: piloto de campo ampliado (mitigación del riesgo de adopción, intake §11).
- `production` se distribuye desde tags `vX.Y.Z` sin sufijo. Aprobador: Mobile Release Engineer con aprobación registrada de forma auditable (run del pipeline ligado al tag y al aprobador), cumpliendo el anti-patrón 09 §4.8 "promotion sin aprobador humano para PROD". Prerrequisito: todos los gates verdes, los criterios de `08_calidad_y_pruebas/criterios-validacion_v1.0.md` cumplidos, las ADR-01 a ADR-05 ratificadas (DoD release §1.4) y verificación post-distribución exitosa.

La promoción de un canal al siguiente no es una redistribución del mismo binario sin más: cada canal se alimenta de su propio tag. Un `-beta.N` validado en campo precede a un `vX.Y.Z` que reusa el mismo árbol de fuentes, vuelve a pasar la suite y se firma antes de distribuir a `production`. La ventana de soak de cada canal de prueba y la audiencia están en `entornos-deploy_v1.0.md` §1.

## 7. Rollback (redistribución de la versión previa)

El paquete distribuido es inmutable por versión; el rollback no edita una versión ya distribuida, sino que redistribuye por el mismo canal interno la versión previa estable, conforme al intake §17 P.8.

| Paso | Comando o acción | Responsable |
| --- | --- | --- |
| 1. Detectar regresión | Alerta de NFR de campo, CVE, falla de verificación post-distribución o reporte de un agente de campo; ver métricas en `guia-publicacion-store-mobile_v1.0.md` §5 | Mobile Release Engineer |
| 2. Detener la promoción de la versión rota | Pausar el canal afectado (`alpha`/`beta`/`production`) en el panel de distribución para detener nuevas instalaciones de la versión `X.Y.Z` afectada | Mobile Release Engineer |
| 3. Redistribuir la versión previa | Re-publicar por el mismo canal interno el paquete firmado de la versión previa estable `X.Y.(Z-1)` ya almacenado como artefacto; ver `guia-publicacion-store-mobile_v1.0.md` §4 | Mobile Release Engineer |
| 4. Publicar fix | PATCH `X.Y.(Z+1)` con el arreglo si el defecto es retrocompatible; MINOR/MAJOR con nota de migración si cambia el comportamiento offline (ADR-03) | Mobile Release Engineer |
| 5. Comunicar | Entrada en CHANGELOG (Keep a Changelog), notas de versión y aviso a los agentes de campo de actualizar; ver deprecation policy en `estrategia-versionado_v1.0.md` §6 | Mobile Release Engineer |
| 6. Regresión | TC de regresión que reproduzca el defecto antes del fix (08 `criterios-validacion_v1.0.md` §4) | QA / SDET (mobile) |

La redistribución de la versión previa por el canal interno es el rollback aplicable a un paquete de aplicación móvil; está documentada con su comando concreto en `guia-publicacion-store-mobile_v1.0.md` §4 y se ensaya al menos una vez antes de la primera distribución a `production`.

## 8. Notificaciones

| Evento | Canal | Severidad | Destinatario |
| --- | --- | --- | --- |
| PR con gates en rojo | Comentario en el PR + canal del equipo | Media | Autor del PR (el dev en `equipo_n=1`) |
| Push a `main` con build roto | Canal del equipo | Alta | Mobile Release Engineer |
| Distribución a `internal`/`alpha`/`beta` exitosa | Canal del equipo | Informativa | Equipo y probadores del canal |
| Distribución a `production` exitosa | Canal del equipo + notas de versión | Informativa | Equipo y agentes de campo |
| Falla de distribución o de verificación post-distribución | Canal del equipo + escalamiento | Crítica | Mobile Release Engineer |
| NFR de campo fuera de objetivo en dispositivo | Canal del equipo + escalamiento | Alta | Mobile Release Engineer |
| CVE crítica o alta detectada (SCA o schedule) | Canal del equipo | Crítica | Mobile Release Engineer; política en supply chain §6 |

Con `equipo_n=1` (intake §13; 08 `estrategia-calidad_v1.0.md` §4), los roles de autor, Mobile Release Engineer y QA los ejerce la misma persona en momentos distintos; el pipeline y el registro auditable del aprobador sustituyen la revisión por pares ausente (08 §4). Un dashboard visible al equipo muestra el estado del último build, la cobertura por capa y el estado de cada canal de distribución.

## 9. Reproducibilidad local

Todos los stages son reproducibles en máquina local con las mismas versiones del runtime y de las herramientas de la plataforma Android (anti-patrón 09 §4.8 "pipeline irreproducible localmente" evitado). La depuración y la prueba se hacen sobre un dispositivo Android conectado por USB en modo desarrollador (intake §17 P.8). Los comandos exactos del pipeline para reproducción local los cita la developer guide de la categoría 10; este documento es su fuente. El build usa el SDK fijado (.NET 8 LTS, target `net8.0-android`) y el lock de dependencias versionado, de modo que la corrida local y la de CI restauran el mismo grafo.

## 10. Trazabilidad

| Stage | Verifica | Fuente |
| --- | --- | --- |
| Build | Compila sin warnings tratados como error | DoD BT §1.2; 08 `estrategia-calidad_v1.0.md` §3 (gate Compilación) |
| Test unit | Suite unitaria de captura, cola y sincronización verde | DoD US §1.1; RN-01/02/03/05 |
| Test offline-sync | Cola, reanudación y convivencia con conflicto | DoD US §1.1; RN-02, RN-03, RN-05; NFR de 05 §8 |
| Test UI móvil | Journey crítico de campo | DoD US §1.1; CU-01 a CU-06 |
| Test snapshot | Pantallas críticas estables | DoD release §1.4 (TC-27) |
| Cobertura | Cobertura por capa y global | DoD US §1.1, release §1.4; intake §17 P.6 |
| NFR de campo | Captura offline, cola ≥ 1000, ciclo ≤ 30 s, reanudación, arranque ≤ 3 s | NFR de 05 §8; DoD release §1.4; intake §17 P.10 |
| SCA / SBOM / Firma del paquete | Cadena de suministro y firma con credencial resguardada | `supply-chain-seguridad_v1.0.md` |
| Análisis estático | Sin issues críticos | DoD BT §1.2, US §1.1, release §1.4 |
| Distribuir / Verificación post-distribución | Paquete en el canal e instalable | `guia-publicacion-store-mobile_v1.0.md`; `entornos-deploy_v1.0.md` |

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Pipeline CI/CD inicial de geovial-mobile (mobile-app-maui, no redistribuible): triggers por evento (PR, push, tags de canal interno/alpha/beta/production, schedule), stages obligatorios (lint, build, test unit/offline-sync/UI móvil/snapshot, cobertura, NFR de campo, SCA, SBOM, análisis estático, firma del paquete con credencial resguardada, distribución por canal interno y verificación post-distribución), matriz de plataforma acotada a Android (net8.0-android, API 26+) sobre .NET 8 LTS, caché y artefactos con retención, promotion internal→alpha→beta→production con aprobador humano en production, rollback por redistribución de la versión previa y notificaciones. Los gates de 08 §3 se materializan como stages sin redefinir la DoD ni la cobertura de 08; cada stage referencia su criterio DoD de 08 o su NFR de 05. Derivado de 08 (estrategia-calidad §3, estrategia-testing §2, definition-of-done, matriz-cobertura-pruebas, criterios-validacion) y de 05 (§8 quality attributes), e intake §17 P.6/P.7/P.8/P.9/P.10. |
