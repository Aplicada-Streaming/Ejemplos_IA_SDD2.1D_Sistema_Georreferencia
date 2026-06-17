# Pipeline CI/CD — geovial-web

**Proyecto:** geovial-web
**Documento:** pipeline-ci-cd_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps + Deploy Engineer

## 0. Alcance y modelo

`geovial-web` es un proyecto de tipo `web-monolith`: un front de render server-side que mantiene, por sesión de usuario, un circuito interactivo persistente y consume por contrato la API REST de `geovial-api` (intake §13, §17 geovial-web). No tiene persistencia de dominio propia (`tiene_persistencia=false`, ADR-02) ni expone una API externa. El artefacto publicable es una imagen de contenedor del front (intake §17.P.7, §17.P.8), que se promueve por los ambientes DEV → QA → STAGING → PROD (`entornos-deploy_v1.0.md`). No es redistribuible.

Por ser una solución multi-proyecto, el orden de construcción inter-proyecto (el backend `geovial-api` de nivel 1 antes que el front de nivel 2) se gobierna en `_solucion/pipeline-solucion_v1.0.md`; este documento asume ese orden y describe el pipeline propio del front.

Cada quality gate de este pipeline ejecuta un criterio de la Definition of Done canónica de 08 (`definition-of-done_v1.0.md`) o verifica una NFR de 05 (`arquitectura-solucion_v1.0.md` §8). La DoD no se redefine aquí: se ejecuta como conjunto de gates (regla 08 §4.8). Los gates nombrados provienen de `estrategia-calidad_v1.0.md` §3 de 08.

## 1. Stages obligatorios

Cada stage declara su tooling por rol abstracto (la herramienta concreta del runtime objetivo se fija en la guía de reproducción local de 10), su quality gate y el criterio de 08/05 que verifica. El runtime objetivo es el del contenedor de front (intake §17.P.9).

| Stage | Tooling (rol abstracto) | Quality gate | Criterio 08 (DoD/gate) / NFR 05 | Bloqueante |
| --- | --- | --- | --- | --- |
| STAGE-01 Lint | Linter y formateador del runtime objetivo | 0 warnings nuevos de formato | Gate "Análisis estático"; DoD-BT análisis estático | Sí en PR |
| STAGE-02 Build | Compilador del runtime objetivo | 0 errores; 0 warnings tratados como error | Gate "Compilación limpia"; DoD-BT "compila sin warnings tratados como error" | Sí |
| STAGE-03 Test unit | Framework de pruebas unitarias | Suite unitaria de presentación y orquestación en verde | Gate "Pruebas unitarias y de integración en verde"; DoD-US/sprint suite en verde | Sí |
| STAGE-04 Test integración | Framework de integración contra base efímera (a través de la API) | Suite de integración por el contrato REST en verde | Gate "Pruebas unitarias y de integración en verde"; DoD-US integración por el contrato | Sí |
| STAGE-05 Test componente UI | Motor headless de interfaz | Pruebas de componente de las vistas clave en verde | Gate "Pruebas de componente de UI y snapshot en verde"; DoD-US componente de UI | Sí |
| STAGE-06 Test snapshot | Framework de snapshot de vistas | Snapshots de vistas clave sin diferencias no aprobadas (TC-22) | Gate "Pruebas de componente de UI y snapshot en verde"; DoD-US snapshot | Sí |
| STAGE-07 Cobertura | Medidor de cobertura segmentado por capa | Global ≥ 80 % líneas / ≥ 70 % branches; Aplicación de UI 80/70; infraestructura 70/60; presentación 60/50 | Gates "Cobertura global" y "Cobertura por capa"; DoD-sprint cobertura; NFR cobertura (05 §8) | Sí |
| STAGE-08 Custodia del token | Prueba de componente de no exposición (motor headless) | 0 exposiciones del token bearer al navegador (TC-19) | DoD-release custodia del token; NFR custodia del token (05 §8) | Bloquea release |
| STAGE-09 NFR interacción | Banco de medición de desempeño de interacción | Latencia de interacción p95 ≤ 200 ms sobre vistas clave (CU-03, CU-06, CU-08) en el ambiente de referencia (TC-20) | DoD-release NFR; NFR latencia de interacción p95 (05 §8, intake §17.P.10) | Bloquea release; excepción solo con ADR y plan de remediación |
| STAGE-10 NFR concurrencia | Cliente de pruebas de carga de circuitos | ≥ 50 circuitos interactivos concurrentes sosteniendo p95 y sin pérdida de estado de sesión (TC-21) | DoD-release NFR; NFR circuitos concurrentes (05 §8, intake §17.P.10) | Bloquea release; excepción solo con ADR y plan de remediación |
| STAGE-11 SCA | Analizador de composición de software | 0 CVE críticas; 0 altas sin excepción documentada | DoD-release calidad de código; ver `supply-chain-seguridad_v1.0.md` §4 | Sí |
| STAGE-12 SBOM | Generador de SBOM (formato estándar de inventario) | SBOM de la imagen del front generado y adjunto al release | Supply chain §1; DoD-release | Sí |
| STAGE-13 Build de imagen | Constructor de la imagen de contenedor del front | Imagen reproducible construida con etiqueta de versión derivada del tag | DoD-release "la imagen de contenedor del front se construye" | Sí |
| STAGE-14 Firma de imagen | Firmador de artefactos con registro de transparencia | Firma válida y registrada sobre la imagen del front | Supply chain §2; DoD-release "la imagen se firma" | Sí |
| STAGE-15 DAST | Escáner dinámico sobre el front desplegado en DEV/QA | 0 hallazgos críticos sin excepción documentada | Supply chain §5; DoD-release calidad de código | Bloquea promoción a STAGING |
| STAGE-16 Publish imagen | Publicador al registro de imágenes del ambiente | Imagen disponible en el registro y consumible por el orquestador de contenedores | DoD-release "la imagen se publica"; `guia-publicacion-image-docker_v1.0.md` | Sí |

Notas:

- STAGE-08, STAGE-09 y STAGE-10 verifican los NFR numéricos de P.10 antes de promover: ningún release se valida sin la custodia del token (0 exposiciones), la latencia de interacción p95 ≤ 200 ms y el sostén de ≥ 50 circuitos concurrentes (`criterios-validacion_v1.0.md` §3). Esto satisface la regla 09 §5.1 (cada NFR numérico tiene un stage que lo verifica antes de promover).
- STAGE-09 y STAGE-10 pueden medirse en CI como aproximación mientras el ambiente equivalente al productivo no esté disponible (`criterios-validacion_v1.0.md` §6), con ratificación obligatoria sobre el ambiente de referencia antes del release. La latencia de interacción excluye la atribuible al backend, medida aparte contra el NFR de `geovial-api` (05 §8).
- STAGE-15 (DAST) aplica porque el front tiene superficie dinámica desplegada (a diferencia de las librerías de la solución): se ejecuta sobre el front ya desplegado en un ambiente no productivo (DEV o QA). Detalle en `supply-chain-seguridad_v1.0.md` §5.
- No hay mutation testing: la regla 08 §2.2 lo reserva a `library`; `web-monolith` no lo incluye (`criterios-validacion_v1.0.md` §5).

## 2. Matriz de sistema operativo y runtime

El destino productivo es un contenedor con runtime de front sobre base de contenedor del runtime objetivo (intake §17.P.9). La matriz cubre ese destino y, en PR, una verificación headless de las vistas clave sobre navegadores evergreen para detectar regresiones de presentación antes del merge.

| Trigger | Sistemas operativos / entorno | Runtime / target | Justificación |
| --- | --- | --- | --- |
| PR a `main` | contenedor base del runtime objetivo + matriz de navegadores evergreen (motor headless: dos últimas versiones mayores de los navegadores de uso corriente) | Runtime del front del contenedor objetivo | Las pruebas de componente y snapshot deben validar las vistas clave sobre los navegadores evergreen soportados (intake §17.P.9) antes del merge |
| Merge a `main` / tag de release | contenedor base del runtime objetivo (entorno equivalente al productivo) | Runtime del front del contenedor objetivo | El destino productivo es el contenedor del front; la imagen se construye y valida en el mismo runtime en el que se desplegará |

La matriz se justifica por cobertura del consumidor real (los navegadores evergreen de escritorio y móvil) contra el costo de minutos de CI: la matriz cruzada de navegadores corre en PR con motor headless; el build de imagen y los NFR de interacción corren en el entorno equivalente al productivo.

## 3. Caché y artefactos

| Elemento | Política | Llave de caché | Retención |
| --- | --- | --- | --- |
| Caché de dependencias del runtime | Restaurar antes de build; invalidar al cambiar el manifiesto de dependencias | Hash del manifiesto de dependencias del runtime objetivo | Hasta cambio de la llave |
| Caché de capas de la imagen de contenedor | Reutilizar capas base no modificadas entre builds | Hash de la etapa base y del manifiesto de dependencias | Hasta cambio de la llave |
| Reporte de cobertura segmentado por capa | Artefacto de CI por corrida | — | 30 días |
| Snapshots baseline de vistas clave | Artefacto versionado en el repositorio; regeneración con revisión registrada | — | Vida del repositorio |
| Evidencia de medición NFR (interacción, concurrencia) | Artefacto de CI de release | — | 1 año |
| SBOM de la imagen del front | Artefacto adjunto al release de la imagen | — | Vida del release |
| Reporte de SCA y de DAST | Artefacto de CI por corrida | — | 90 días |
| Imagen de contenedor del front | Artefacto firmado, etiquetado por versión, publicado al registro | Etiqueta de versión derivada del tag | Política de retención del registro (mínimo: la versión vigente y la inmediata anterior para rollback) |

El artefacto firmado y adjunto al release es la imagen de contenedor del front (§1 STAGE-12, STAGE-14; `supply-chain-seguridad_v1.0.md` §2). La retención mínima de la versión anterior en el registro habilita el rollback por redepliegue (§5).

## 4. Promotion rules

El modelo es de ambientes de servicio DEV → QA → STAGING → PROD (`entornos-deploy_v1.0.md`). Cada transición declara su trigger y los gates que deben estar en verde. La promoción a PROD exige aprobador humano (regla 09 §4.8).

| Transición | Trigger | Prerequisitos (gates) | Aprobador |
| --- | --- | --- | --- |
| Feature branch → `main` (merge) | PR aprobado con suite verde | STAGE-01..STAGE-07 en verde (compilación limpia, unit, integración, componente UI, snapshot, cobertura global y por capa, análisis estático): gates bloqueantes de merge de 08 §3 | Auto (gate) |
| `main` → DEV | Merge a `main` | Todos los anteriores + STAGE-12 SBOM generado + STAGE-13 build de imagen + STAGE-14 firma | Auto |
| DEV → QA | Imagen firmada disponible en DEV | STAGE-11 SCA en verde + STAGE-15 DAST sin hallazgos críticos | Auto a verde |
| QA → STAGING | Tag de release-candidate | STAGE-08 custodia del token + STAGE-09 NFR interacción + STAGE-10 NFR concurrencia en verde sobre el ambiente de referencia | Release manager |
| STAGING → PROD | Tag de release `v<X.Y.Z>` tras ventana de soak | Suite de release completa verde + criterios de validación de 08 firmados | Release manager + aprobación de negocio |

La promoción a STAGING exige los tres NFR numéricos en verde sobre el ambiente de referencia (no solo en CI), porque STAGING es el ambiente equivalente al productivo donde se ratifican (`criterios-validacion_v1.0.md` §3). La promoción a PROD nunca es automática: tiene aprobador humano y registro auditable (`entornos-deploy_v1.0.md` §5).

## 5. Rollback

El rollback es por redepliegue de la imagen previa estable (intake §17.P.8; regla 09 §4.2 modelo de rollback por reversión de deploy). No hay delist en feed (la imagen no se publica a un canal de paquete) ni migración de datos (el front no tiene persistencia de dominio, ADR-02): el estado de UI es efímero y reconstruible desde la API.

| Paso | Acción | Comando o procedimiento concreto |
| --- | --- | --- |
| 1 | Detectar la regresión en PROD | Alerta de las notificaciones (§6) o reporte de incidente; identificar la versión desplegada por su etiqueta |
| 2 | Identificar la imagen previa estable | Leer del registro la etiqueta de versión inmediatamente anterior verde (retenida por §3) |
| 3 | Redesplegar la imagen previa | Indicar al orquestador de contenedores que ejecute la imagen con la etiqueta previa; el desvío de tráfico hacia la versión anterior es inmediato y respeta la afinidad de sesión de los circuitos (`entornos-deploy_v1.0.md` §6) |
| 4 | Verificar el redepliegue | Comprobación de salud del contenedor de front, verificación de firma de la imagen redeployada y prueba de humo de inicio de sesión y de las vistas clave (`guia-publicacion-image-docker_v1.0.md` §3) |
| 5 | Revertir el cambio en `main` | `git revert <sha-del-commit>` del cambio que introdujo la regresión; abrir PR de reversión con la suite en verde |
| 6 | Publicar el fix | Corregir, incrementar la versión según SemVer (PATCH si es retrocompatible; MAJOR si toca el contrato consumido, coordinando con `geovial-api`), reconstruir y promover por el flujo normal |
| 7 | Comunicar | Registrar en el CHANGELOG y en las notas de release; notificar al equipo |

Nota sobre los circuitos en curso: el redepliegue corta los circuitos interactivos activos, que el front reconstruye al reconectar consultando a la API (fuente de verdad, ADR-02). Ninguna acción de dominio confirmada se pierde por el redepliegue, porque el dominio vive en `geovial-api`. Una ventana de drenaje breve de las conexiones de circuito antes del corte reduce la interrupción percibida (`entornos-deploy_v1.0.md` §6).

## 6. Notificaciones

| Evento | Canal | Severidad | Destinatario |
| --- | --- | --- | --- |
| Falla de gate bloqueante de merge (STAGE-01..STAGE-07) | Canal del equipo + estado del PR | Alta | Desarrollador autor del PR |
| Falla de gate de release (STAGE-08, STAGE-09, STAGE-10) | Canal del equipo + bloqueo del release | Alta | Desarrollador en rol QA |
| Falla de SCA o DAST (CVE crítica/alta, hallazgo dinámico crítico) | Canal del equipo + ticket de remediación | Crítica/Alta | Desarrollador; ver SLA en `supply-chain-seguridad_v1.0.md` §6 |
| Falla de firma o de build de imagen (STAGE-13, STAGE-14) | Canal del equipo + bloqueo del release | Alta | Desarrollador en rol QA |
| Promoción a STAGING o PROD pendiente de aprobación | Canal del equipo + solicitud al aprobador | Media | Release manager |
| Despliegue exitoso a un ambiente | Canal del equipo (informativo) + tablero | Informativa | Equipo |
| Rollback ejecutado en PROD | Canal del equipo + incidente | Alta | Equipo |

Con `equipo_n=1` (intake §2), el desarrollador asume los roles de autor, SDET, QA y release manager; las notificaciones se concentran en un único canal de equipo con el estado del PR, del release y de cada ambiente visible en el tablero. La aprobación de negocio para PROD es la única firma externa al desarrollador.

## 7. Reproducción local

Cada stage es reproducible en máquina local con las mismas versiones del runtime objetivo. Existen scripts `.bat` (`build-*.bat`, `publish-*.bat`) y de creación de la imagen del front (intake §16, §17.P.8) que materializan el build y la publicación local. Los comandos exactos por stage los publica la developer guide de 10; este documento fija qué se ejecuta y en qué orden, no el comando concreto del runtime. Anti-patrón evitado: pipeline irreproducible localmente (regla 09 §4.8).

## 8. Trazabilidad

| Stage / gate | Criterio 08 (DoD/gate) | NFR / RN 05 | ADR |
| --- | --- | --- | --- |
| STAGE-02 / Compilación limpia | DoD-BT compila sin warnings-as-errors | — | — |
| STAGE-03, STAGE-04 / Pruebas en verde | DoD-US/sprint suite en verde | — | ADR-04 |
| STAGE-05, STAGE-06 / Componente y snapshot | DoD-US componente de UI y snapshot | — | ADR-05 |
| STAGE-07 / Cobertura global y por capa | DoD-sprint cobertura | NFR cobertura (05 §8) | ADR-04, ADR-05 |
| STAGE-08 / Custodia del token | DoD-release custodia del token | NFR custodia del token (05 §8) | ADR-03 |
| STAGE-09 / NFR interacción | DoD-release NFR | NFR latencia de interacción p95 (05 §8) | ADR-01, ADR-05 |
| STAGE-10 / NFR concurrencia | DoD-release NFR | NFR circuitos concurrentes (05 §8) | ADR-01, ADR-04 |
| STAGE-01, STAGE-11 / Análisis estático y SCA | DoD-release calidad de código | — | — |
| STAGE-12, STAGE-14 / SBOM y firma | DoD-release supply chain | — | — |
| STAGE-15 / DAST | DoD-release calidad de código | — | ADR-03 |
| STAGE-13, STAGE-16 / build y publish de imagen | DoD-release imagen construida y publicada | NFR disponibilidad (05 §8, vía ambiente) | ADR-01 |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Pipeline CI/CD inicial de geovial-web: dieciséis stages (lint, build, unit, integración, componente UI, snapshot, cobertura, custodia del token, NFR interacción, NFR concurrencia, SCA, SBOM, build de imagen, firma, DAST y publish de imagen) que ejecutan los gates nombrados de 08 como verificación de la DoD; matriz runtime/navegadores evergreen; caché y artefactos con retención de la imagen previa para rollback; promotion DEV→QA→STAGING→PROD con aprobador humano para PROD; rollback por redepliegue de la imagen previa con afinidad de sesión; notificaciones para equipo_n=1. Verifica los tres NFR numéricos de P.10 (interacción p95 ≤ 200 ms, ≥ 50 circuitos, custodia del token) en stages antes de promover. |
