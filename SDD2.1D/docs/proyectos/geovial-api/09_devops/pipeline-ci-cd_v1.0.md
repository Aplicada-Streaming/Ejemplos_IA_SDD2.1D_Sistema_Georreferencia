# Pipeline CI/CD — geovial-api

**Proyecto:** geovial-api
**Documento:** pipeline-ci-cd_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps + Platform Engineer

## 1. Objetivo y alcance

Este documento define el pipeline de integración y entrega continua de `geovial-api`, el backend monolítico (`rest-api`) y proyecto principal de la solución GeoVial. El pipeline construye, valida, empaqueta, firma y publica dos artefactos: la imagen de contenedor del backend y el contrato OpenAPI versionado que consumen `geovial-web` y `geovial-mobile` (regla §2.2 para `rest-api`). El modelo de promoción es por ambientes desplegables DEV → QA → STAGING → PROD con despliegue canary y rollback por desvío de tráfico (no por canales de paquete), conforme a la naturaleza de servicio desplegable del proyecto.

Los gates de prueba del pipeline ejecutan la Definition of Done de `08_calidad_y_pruebas/definition-of-done_v1.0.md` y la cobertura por capa de `08_calidad_y_pruebas/estrategia-testing_v1.0.md`; este documento no las redefine. Los gates G1 a G8 referenciados son los de `08_calidad_y_pruebas/estrategia-calidad_v1.0.md` §3 y se materializan acá como stages. Cada gate referencia el criterio DoD de 08 o el NFR de 05 que verifica (`arquitectura-solucion_v1.0.md` §8).

## 2. Triggers

Triggers explícitos por evento (anti-patrón §4.8 "trigger único y opaco" evitado): el PR valida pero no despliega; el merge despliega a DEV; los tags y las aprobaciones promueven por ambientes.

| Trigger | Evento | Stages que ejecuta | Promueve a |
| --- | --- | --- | --- |
| PR a `main` | Apertura o actualización de un PR | Lint, Build, Test unit, Test integración, Test contract, Validación de contrato, SCA, Análisis estático | — |
| Push a `main` | Merge de un PR (squash con Conventional Commits) | Todos los de PR + SBOM + Firma + Build de imagen + Publish OpenAPI (interno) + Deploy DEV | DEV (auto) |
| Tag `vX.Y.Z-rc.N` | Tag de candidato a release | Todos + Deploy QA + suite de regresión + Deploy STAGING (tras aprobación) + NFR (G8) | QA → STAGING |
| Tag `vX.Y.Z` sin sufijo | Tag estable | Todos + NFR (G8) + Publish OpenAPI (público) + Deploy PROD canary (tras aprobación) | PROD (canary) |
| Schedule semanal | Cron de mantenimiento | SCA, dependency scanning, DAST programado, regeneración de aviso de CVE | — |

El sufijo del tag y la rama del evento determinan el ambiente destino; ninguna publicación a PROD ocurre sin un tag estable y una aprobación humana (§6).

## 3. Stages obligatorios

Cada stage declara su tooling por rol abstracto, su quality gate, el criterio DoD o NFR que verifica y si es bloqueante. La condición y la consecuencia de cada gate viven en 08; acá solo se declara en qué stage se ejecuta.

| Stage | Tooling (rol abstracto) | Quality gate | Criterio DoD / NFR verificado | Bloqueante |
| --- | --- | --- | --- | --- |
| Lint | Formateador y linter del runtime objetivo | 0 issues de formato nuevos | DoD BT §1.2 (compila sin advertencias tratadas como error, parcial) | Sí en PR |
| Build | Compilador del runtime objetivo | G1: compila sin advertencias tratadas como error | DoD US §1.1 y BT §1.2 (la suite compila) | Sí |
| Test unit | Framework de tests unitarios | G2: suite unitaria verde, ningún test sin assert | DoD US §1.1 (criterios Given/When/Then con test); RN-01 a RN-07 (invariantes de dominio) | Sí |
| Test integración | Framework de integración contra base de datos efímera | G2: suite de integración verde | DoD US §1.1; integridad de jerarquía y ciclo bajo concurrencia (RC-03/04/05) | Sí |
| Test contract | Framework de validación de contrato sobre OpenAPI | G4: 100 % de los 35 endpoints públicos con al menos un contract test por versión vigente | DoD US §1.1 (endpoint público con contract test); DoD release §1.4 | Sí |
| Validación de contrato | Framework de validación de contrato; fuzz de endpoints | G5: la especificación OpenAPI valida contra la implementación, sin deriva | DoD release §1.4 (OpenAPI valida contra implementación); CU-22 | Sí |
| Cobertura | Herramienta de cobertura del runtime con reporte por capa | G3: dominio 85/80, aplicación 80/70, infraestructura 70/60; global líneas ≥ 80 % / branches ≥ 70 % (intake §17 P.6) | DoD US §1.1 y release §1.4 (cobertura por capa y global) | Sí |
| SCA | Herramienta de software composition analysis del runtime | 0 CVE críticas, 0 altas sin excepción registrada | Política de CVE (ver `supply-chain-seguridad_v1.0.md` §6) | Sí |
| Análisis estático | Analizador estático del runtime (incluye reglas de seguridad, SAST) | G6: sin issues críticos nuevos | DoD US §1.1 y BT §1.2 (sin issues críticos del análisis estático) | Sí |
| SBOM | Generador CycloneDX o SPDX | SBOM JSON generado, firmado y adjunto al release | DoD release §1.4 (artefacto trazable); supply chain §1 | Sí en release |
| Build de imagen | Constructor de imagen de contenedor reproducible | Imagen construida con etiqueta de versión y digest inmutable | DoD release §1.4 (artefacto publicable se construye); ver `guia-publicacion-image-docker_v1.0.md` §2 | Sí en release |
| Firma | Firma de artefacto (sigstore/cosign u homólogo) y firma del SBOM | Firma válida y registrada en transparency log | Supply chain §2; DoD release §1.4 (artefacto se firma) | Sí en release |
| Publish OpenAPI | Comando de publicación del contrato OpenAPI versionado al hub de contratos | Contrato OpenAPI de la versión mayor disponible y validado | Ver `guia-publicacion-openapi_v1.0.md` §2; CU-22 | Sí en release |
| Publish imagen | Comando de publicación de la imagen al registro de imágenes | Imagen disponible en el registro por digest y etiqueta | Ver `guia-publicacion-image-docker_v1.0.md` §2 | Sí en release |
| Regresión | Comparación de suite entre revisiones | G7: ningún test verde anterior pasa a rojo sin justificación | DoD release §1.4 y criterios-validacion §4 (sin regresión injustificada) | Bloquea release |
| NFR | Cliente de carga / pruebas de rendimiento en ambiente equivalente al productivo | G8: cada NFR numérico de 05 §8 medido y cumplido (ver §3.1) | DoD release §1.4 (NFR numérico medido y cumple) | Bloquea release |
| Deploy + verificación | Orquestador de contenedores; sondas de salud y de preparación | Despliegue saludable; sondas verdes antes de habilitar tráfico | Promotion rules §6; vista de despliegue 05 §5 | Sí por ambiente |

Mapa gate → stage (sin duplicar la definición de 08): G1=Build, G2=Test unit + Test integración, G3=Cobertura, G4=Test contract, G5=Validación de contrato, G6=Análisis estático, G7=Regresión, G8=NFR.

### 3.1 NFR numéricos con su stage de verificación (gate G8)

Cada NFR numérico de `arquitectura-solucion_v1.0.md` §8 (origen intake §17 P.10) tiene un stage que lo verifica antes de promover a PROD. G8 bloquea el release, no el merge (08 estrategia-calidad §3).

| NFR (05 §8) | Objetivo numérico | Stage / TC que lo verifica | Ambiente de medición |
| --- | --- | --- | --- |
| Latencia p95 de lecturas | ≤ 300 ms | NFR (TC-21, cliente de carga sobre endpoints de consulta y listado) | STAGING (equivalente al productivo) |
| Latencia p95 de escrituras | ≤ 500 ms | NFR (TC-22, cliente de carga sobre endpoints de alta y mutación) | STAGING |
| Capacidad del lote de sincronización | ≥ 1000 cambios sin pérdida ni duplicación | NFR (TC-31, carga del endpoint de subida con lote ≥ 1000) | STAGING |
| Idempotencia de operaciones no seguras | 100 % sin efecto duplicado | Test integración + NFR (TC-29, TC-30, reintento por clave y por identificador de origen) | QA y STAGING |
| Integridad de jerarquía y ciclo | 0 violaciones bajo concurrencia | Test integración (TC-33, pruebas de concurrencia sobre restricciones del almacén) | QA |
| Disponibilidad mensual | ≥ 99,5 % | Deploy + verificación (sondas de salud del contenedor de backend; métrica observada, sin SLO ≥ 99,9 %, `tiene_observabilidad_critica=false`) | PROD (observación continua) |
| Cobertura de pruebas | líneas ≥ 80 % / branches ≥ 70 %; por capa; 100 % endpoints con contract test | Cobertura (G3) + Test contract (G4) | CI (cada build) |

Ningún NFR con objetivo numérico se declara cumplido por inspección cualitativa (criterios-validacion §3); la disponibilidad se observa sobre sondas sin fijar SLO de 99,9 %.

## 4. Matriz de SO y runtime

El backend se ejecuta en un contenedor de backend con base de sistema operativo de núcleo abierto y runtime del backend en versión de soporte prolongado (intake §17 P.9). El target de ejecución es único; la matriz no cruza sistemas operativos porque el artefacto desplegable es una única imagen de contenedor.

| Trigger | Sistema operativo del runner | Runtime objetivo | Justificación |
| --- | --- | --- | --- |
| PR a `main` | Núcleo abierto | Runtime del backend en versión LTS; compilación y suite | Cubre build, suite unitaria y de integración con base efímera al menor costo de minutos |
| Push a `main` | Núcleo abierto | Igual que PR + SBOM + build de imagen | Mismo runner; agrega inventario y empaqueta la imagen |
| Tag rc / estable | Núcleo abierto | Runtime del backend en versión LTS; imagen base del runtime del backend | La imagen se construye contra el mismo runtime de ejecución del contenedor de backend (intake §17 P.9) |

Justificación de la matriz: el contenedor de backend corre sobre un único runtime objetivo; una matriz cruzada de sistemas operativos o de múltiples runtimes no aporta cobertura de consumidores reales frente al costo de minutos de CI, porque el único consumidor de ejecución es el orquestador de contenedores que aloja la imagen. La base de datos efímera para la suite de integración se levanta como contenedor de soporte de la corrida, no como matriz.

## 5. Caché y artefactos

Política de caché del gestor de dependencias del runtime:

| Caché | Llave | Expiración | Notas |
| --- | --- | --- | --- |
| Paquetes restaurados del gestor de dependencias | Hash del archivo de lock de dependencias | 7 días o invalidación por cambio del lock | Acelera la restauración; se reconstruye al cambiar dependencias |
| Capas de imagen de contenedor | Hash del manifiesto de construcción y de las dependencias | Por cambio de capa base o de dependencias | Reuso de capas inferiores entre builds; la capa de aplicación se reconstruye por corrida |
| Artefactos intermedios de build | Hash del árbol de fuentes | Por corrida | Reuso entre stages de la misma corrida |

Artefactos producidos por stage y su retención:

| Artefacto | Stage productor | Retención |
| --- | --- | --- |
| Reporte de cobertura por capa | Cobertura | 90 días |
| Reporte de NFR (latencias p95, lote de sincronización, idempotencia) | NFR | 90 días |
| Reporte de SCA y de dependency scanning | SCA / Schedule | 180 días |
| Reporte de DAST | Schedule | 180 días |
| SBOM (CycloneDX/SPDX JSON, firmado) | SBOM | Permanente, adjunto al release |
| Imagen de contenedor firmada (por digest) | Build de imagen + Firma | Permanente en el registro; copia de manifiesto 180 días |
| Contrato OpenAPI versionado (`.yaml`/`.json`) | Publish OpenAPI | Permanente, versionado con la versión mayor del contrato |
| Log de firma y referencia al transparency log | Firma | Permanente, adjunto al release |
| Provenance (procedencia del build) | Firma | Permanente, adjunto al release |

## 6. Promotion rules (DEV → QA → STAGING → PROD con canary)

El modelo de promoción es por ambientes de servicio desplegable, no por canales de paquete (regla §2.2 para `rest-api`; anti-patrón §4.8 "confundir publicación con despliegue"). Cada transición declara su trigger, su aprobador y sus prerrequisitos. Detalle operativo de ambientes en `entornos-deploy_v1.0.md`.

| Transición | Trigger | Prerrequisitos (gates) | Aprobador |
| --- | --- | --- | --- |
| → DEV | Merge a `main` | G1, G2, G3, G4, G5, G6 verdes; SBOM presente; imagen firmada | Automático |
| DEV → QA | Tag `vX.Y.Z-rc.N` | Además, suite de integración verde y despliegue DEV saludable | QA lead |
| QA → STAGING | Aprobación tras QA en verde | Además, G7 (regresión) verde y soak en QA cumplido | Release manager |
| STAGING → PROD | Tag `vX.Y.Z` + aprobación | Además, G8 (NFR numéricos) medidos y cumplidos en STAGING; ventana de soak de STAGING cumplida | Release manager + aprobación de negocio |

Despliegue canary a PROD: la versión nueva recibe tráfico de forma incremental (5 % → 25 % → 100 %) mientras las sondas de salud y los puntos de medición de latencia y de tasa de error por código (`arquitectura-solucion_v1.0.md` §7) confirman el comportamiento esperado en cada paso. Si un escalón degrada las métricas observadas (latencia fuera del objetivo de §3.1 o aumento de errores), el avance se detiene y se ejecuta el rollback por desvío de tráfico (§7). El contrato OpenAPI de la versión mayor se publica al hub de contratos antes de habilitar el tráfico de la versión correspondiente, conservando la versión mayor previa durante la convivencia (ADR-10, CU-22). La aprobación a PROD queda registrada de forma auditable (corrida del pipeline ligada al tag y al aprobador), cumpliendo el anti-patrón §4.8 "promotion sin aprobador humano para PROD".

## 7. Rollback

El artefacto desplegable es una imagen de contenedor inmutable identificada por digest; el rollback no reconstruye, reusa el digest previo. Es el procedimiento por tipo de artefacto exigido por la regla §4.2.5, alineado con el intake §17 P.8 (redepliegue de la imagen previa con desvío de tráfico).

| Paso | Acción concreta | Responsable |
| --- | --- | --- |
| 1. Detectar regresión | Alerta de las sondas de salud, de los puntos de medición de latencia/errores o reporte de consumidor durante el canary o en PROD; ver métricas en `guia-publicacion-image-docker_v1.0.md` §5 | Release manager |
| 2. Desviar tráfico (rollback rápido) | Redirigir el tráfico de la versión nueva a la versión previa estable por el orquestador de contenedores (traffic shift); la versión previa sigue desplegada y saludable, el corte es inmediato | Release manager |
| 3. Retirar la versión rota | Escalar a 0 la versión defectuosa y marcar su digest como no promovible; la imagen no se borra del registro (otros entornos pueden necesitarla para análisis) | AG-09 |
| 4. Re-desplegar versión previa | Si no había versión previa activa (primer despliegue), redesplegar el digest inmutable de la última imagen estable conocida (intake §17 P.8) | AG-09 |
| 5. Contrato OpenAPI | Si el rollback cruza una versión mayor del contrato, mantener la versión mayor previa publicada en el hub; nunca se retira la versión mayor que un cliente aún consume (ADR-10, CU-22) | AG-09 |
| 6. Publicar fix | PATCH `X.Y.(Z+1)` con el arreglo si el defecto es retrocompatible; versión mayor nueva con plan de migración si el fix es incompatible (ADR-10) | AG-09 |
| 7. Comunicar | Entrada en CHANGELOG (Keep a Changelog), release notes y aviso a `geovial-web` y `geovial-mobile`; ver deprecation policy en `estrategia-versionado_v1.0.md` §6 | AG-09 |
| 8. Regresión | TC de regresión que reproduzca el defecto antes del fix (08 estrategia-calidad §5; criterios-validacion §4) | AG-08 |

El desvío de tráfico es el rollback de minutos exigido por la variante `rest-api` (§1.2 de 09_rules): no requiere reconstruir ni re-publicar, solo reapuntar el tráfico al digest previo. Se ensaya al menos una vez en STAGING antes del primer despliegue a PROD.

## 8. Notificaciones

| Evento | Canal | Severidad | Destinatario |
| --- | --- | --- | --- |
| PR con gates en rojo | Comentario en el PR + canal del equipo | Media | Autor del PR (AG-09 en equipo_n=1) |
| Push a `main` con build o deploy DEV roto | Canal del equipo | Alta | Release manager |
| Despliegue a QA / STAGING exitoso | Canal del equipo | Informativa | Equipo |
| Falla del gate G8 (NFR) antes de PROD | Canal del equipo + escalamiento | Alta | Release manager + Arquitecto |
| Avance o detención del canary en PROD | Canal del equipo + dashboard de despliegue | Alta | Release manager |
| Rollback ejecutado por desvío de tráfico | Canal del equipo + escalamiento | Crítica | Release manager + aprobación de negocio |
| CVE crítica o alta detectada (SCA o schedule) | Canal del equipo + aviso a consumidores | Crítica | Release manager; política en supply chain §6 |

Con `equipo_n=1` (intake §13; 08 estrategia-calidad §4), los roles de autor, Release manager, QA lead y aprobador de negocio los ejerce la misma persona en momentos distintos; el pipeline y el registro auditable del aprobador y del tag sustituyen la revisión por pares ausente (08 §4). Los dashboards de latencia por operación, tasa de error por código y volumen de cambios por ciclo de sincronización (`arquitectura-solucion_v1.0.md` §7) son visibles al equipo durante el canary.

## 9. Reproducibilidad local

Todos los stages son reproducibles en máquina local con las mismas versiones del runtime (anti-patrón §4.8 "pipeline irreproducible localmente" evitado). Existen scripts de construcción y publicación (`build-*`, `publish-*`) y scripts de creación de la imagen de contenedor (intake §16, §17 P.8) que el pipeline invoca y que el desarrollador ejecuta localmente con el mismo SDK fijado y el lock de dependencias versionado, de modo que la corrida local y la de CI restauran el mismo grafo. La base de datos efímera y la imagen de contenedor se levantan localmente con los mismos manifiestos que usa CI. Los comandos exactos del pipeline para reproducción local los cita la developer guide de la categoría 10; este documento es su fuente.

## 10. Trazabilidad

| Stage / gate | Verifica | Fuente |
| --- | --- | --- |
| Build (G1) | Compila sin advertencias | DoD BT §1.2; 08 estrategia-calidad §3 |
| Test unit + integración (G2) | Suite unitaria y de integración verdes; invariantes | DoD US §1.1; RN-01 a RN-07; RC-03/04/05 |
| Cobertura (G3) | Cobertura por capa y global | DoD US §1.1 y release §1.4; intake §17 P.6 |
| Test contract (G4) | 100 % de los 35 endpoints con contract test | DoD US §1.1 y release §1.4; `contratos-rest_v1.0.md` §3 |
| Validación de contrato (G5) | OpenAPI valida contra implementación | DoD release §1.4; CU-22; `guia-publicacion-openapi_v1.0.md` |
| Análisis estático (G6) | Sin issues críticos | DoD US §1.1 y BT §1.2 |
| Regresión (G7) | Sin regresión injustificada | DoD release §1.4; criterios-validacion §4 |
| NFR (G8) | Latencias p95, lote de sincronización, idempotencia, integridad | NFR de 05 §8; DoD release §1.4; intake §17 P.10 |
| SCA / SBOM / Firma | Cadena de suministro | `supply-chain-seguridad_v1.0.md` |
| Publish imagen / OpenAPI | Artefactos en registro y hub de contratos | `guia-publicacion-image-docker_v1.0.md`; `guia-publicacion-openapi_v1.0.md` |
| Deploy + canary + rollback | Promoción por ambientes con desvío de tráfico | `entornos-deploy_v1.0.md`; 05 §5 |

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Pipeline CI/CD inicial de geovial-api (rest-api, principal): triggers por evento (PR, push a DEV, tag rc a QA/STAGING, tag estable a PROD canary, schedule); stages obligatorios (lint, build, test unit/integración/contract, validación de contrato, cobertura, SCA, análisis estático/SAST, SBOM, build de imagen, firma, publish OpenAPI e imagen, regresión, NFR, deploy y verificación); matriz de runtime con artefacto único de imagen de contenedor sobre runtime LTS; caché y artefactos con retención; promotion DEV→QA→STAGING→PROD con canary 5/25/100 y aprobador humano para PROD; rollback rápido por desvío de tráfico al digest previo; notificaciones. Los gates G1-G8 se materializan como stages sin redefinir la DoD ni la cobertura de 08; cada NFR numérico de §17 P.10 tiene un stage que lo verifica antes de promover. Derivado de 08 (estrategia-calidad §3, estrategia-testing, definition-of-done, criterios-validacion), de 05 (§5 despliegue, §7 cross-cutting, §8 NFR; ADR-10) y del intake §17 P.6/P.7/P.8/P.9/P.10. |
