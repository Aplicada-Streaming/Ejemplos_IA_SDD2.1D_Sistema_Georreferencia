# Guía de publicación del artefacto openapi — geovial-api

**Proyecto:** geovial-api
**Documento:** guia-publicacion-openapi_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps + Platform Engineer

## 0. Alcance

Esta guía cubre la publicación del contrato OpenAPI versionado de `geovial-api` al hub de contratos, el artefacto que consumen `geovial-web` y `geovial-mobile` para generar sus clientes y validar su integración (regla §2.2 para `rest-api`; `contratos-rest_v1.0.md`). El tipo de artefacto `openapi` es el nombre normalizado de la tabla §2.2 de 09_rules. La publicación de la imagen de contenedor desplegable se documenta aparte en `guia-publicacion-image-docker_v1.0.md`. El contrato es la fuente de verdad del 100 % de los 35 endpoints públicos del backend (`contratos-rest_v1.0.md` §3).

## 1. Pre-requisitos

| Pre-requisito | Detalle |
| --- | --- |
| Acceso al hub de contratos | Identidad de servicio con permiso de escritura sobre el espacio de contratos del proyecto |
| Credencial de publicación del contrato | Token con scope de escritura al hub, almacenado en el vault del CI (`entornos-deploy_v1.0.md` §4); nunca en commit |
| Especificación OpenAPI materializada | El documento OpenAPI (`.yaml`/`.json`) generado a partir de la implementación, con la versión mayor del contrato por URI (`/v1`, ADR-10) |
| Validador de contrato | Framework de validación de contrato disponible en el stage Validación de contrato (gate G5) |

La especificación OpenAPI se materializa desde la implementación del backend (no se mantiene a mano de forma divergente), de modo que el gate G5 detecte cualquier deriva entre contrato e implementación.

## 2. Comando o stage de publicación

La publicación la ejecuta el pipeline en los stages Validación de contrato y Publish OpenAPI (`pipeline-ci-cd_v1.0.md` §3), disparados por push a `main` (publicación interna para DEV/QA) y por tag estable (publicación pública del contrato de la versión correspondiente).

Secuencia abstracta reproducible (los comandos exactos los cita la developer guide de 10):

1. Materializar la especificación OpenAPI de la versión mayor del contrato desde la implementación, etiquetada con la versión derivada por la herramienta de versión (`estrategia-versionado_v1.0.md` §5).
2. Validar la especificación contra la implementación en el stage Validación de contrato (gate G5): sin deriva entre el contrato declarado y los 35 endpoints implementados.
3. Ejecutar los contract tests del 100 % de los endpoints por versión vigente en el stage Test contract (gate G4).
4. Publicar el documento OpenAPI al hub de contratos en el stage Publish OpenAPI, bajo la versión mayor del contrato (`/v1`), solo si G4 y G5 están en verde (gate bloqueante).

Variables de entorno requeridas (inyectadas desde el vault del CI):

| Variable (rol abstracto) | Propósito |
| --- | --- |
| URL del hub de contratos | Destino de publicación del contrato |
| Credencial de escritura al hub | Autenticación de la publicación |
| Versión mayor del contrato | Identificador de la versión publicada (`/v1`), coincidente con la etiqueta de la imagen y el tag Git |

## 3. Verificación post-publish

| Verificación | Cómo confirmar |
| --- | --- |
| Contrato disponible | El documento OpenAPI de la versión mayor se resuelve en el hub de contratos para los consumidores |
| Validez de la especificación | La especificación es un documento OpenAPI válido y completo para los 35 endpoints (`contratos-rest_v1.0.md` §3) |
| Sin deriva | La especificación publicada valida contra la implementación de la imagen desplegada para esa versión (gate G5) |
| Contract tests | El 100 % de los endpoints de la versión pasa su contract test (gate G4; TC-34) |
| Cliente de prueba | Un cliente generado desde el contrato publicado ejecuta una operación de prueba contra el ambiente correspondiente sin discrepancia |

## 4. Versionado y compatibilidad

El contrato se versiona por URI con prefijo de versión mayor en la ruta (ADR-10, CU-22; `contratos-rest_v1.0.md` §6), coordinado con SemVer del proyecto (`estrategia-versionado_v1.0.md` §6). No hay "rollback" de borrado de un contrato publicado del modo de un servicio: una versión mayor publicada permanece mientras un cliente la consuma.

| Tipo de cambio | Acción sobre el contrato publicado |
| --- | --- |
| Compatible (campo opcional, endpoint nuevo, código de error nuevo) | Se incorpora dentro de la misma versión mayor (`/v1`); no rompe a los consumidores; bump MINOR del proyecto |
| Incompatible (quitar campo, cambiar semántica, renombrar código de error) | Se publica una versión mayor nueva (`/v2`) conservando la previa (`/v1`) durante la convivencia de al menos un MINOR (intake §17 P.3); bump MAJOR |
| Deprecación | El recurso o código se marca obsoleto en una versión menor antes de removerse en la mayor siguiente; el backend comunica el plan de retiro |
| Retiro | Una versión mayor retirada o inexistente responde `VERSION_NO_SOPORTADA`; un recurso ausente en la versión, `RECURSO_NO_EN_VERSION` (`contratos-rest_v1.0.md` §6) |

Reversión efectiva: si una versión nueva del contrato resultara defectuosa, el rollback es del despliegue del backend (desvío de tráfico al digest previo, `guia-publicacion-image-docker_v1.0.md` §4) manteniendo publicada la versión mayor del contrato que los clientes ya consumen; nunca se retira la versión mayor en uso (ADR-10).

## 5. Métricas

| Métrica | Qué mide | Fuente |
| --- | --- | --- |
| Cobertura de contract tests | % de endpoints con contract test por versión | Gate G4 (objetivo 100 %; TC-34) |
| Deriva contrato-implementación | Incidencias de deriva detectadas por el validador | Gate G5 |
| Versiones mayores en convivencia | Cantidad de versiones mayores del contrato vivas a la vez | Hub de contratos (objetivo: la mínima necesaria) |
| Tiempo de adopción de versión nueva | Tiempo hasta que los consumidores migran de versión mayor | Coordinación con `geovial-web` y `geovial-mobile` |
| Rupturas silenciosas evitadas | Cambios incompatibles publicados con nueva versión mayor (objetivo 100 %) | Auditoría del changelog contra los contract tests (`estrategia-versionado_v1.0.md` §6) |

## 6. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Contrato y operaciones | `contratos-rest_v1.0.md` §3 (35 endpoints); CU-01 a CU-22 |
| Versionado por URI | ADR-10; CU-22; `estrategia-versionado_v1.0.md` §6 |
| Gates de contrato | G4 (contract test total) y G5 (validación de contrato), 08 estrategia-calidad §3 |
| Stages de validación y publicación | `pipeline-ci-cd_v1.0.md` §3 |
| Secretos de publicación | `entornos-deploy_v1.0.md` §4 |
| Consumidores del contrato | `geovial-web`, `geovial-mobile` (intake §14; se indexan en la vista de solución de `_solucion/`, Fase H) |
| Downstream | examples de 11 (consumen el contrato publicado); developer guide de 10 |

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Guía inicial de publicación del artefacto openapi de geovial-api: pre-requisitos (acceso al hub de contratos, credencial en vault, especificación materializada desde la implementación, validador de contrato), secuencia de materialización/validación/contract test/publish ejecutada por el pipeline, verificación post-publish (contrato disponible y válido, sin deriva, contract tests al 100 %, cliente de prueba), versionado por URI y política de compatibilidad coordinada con SemVer (convivencia de versiones mayores, deprecación y retiro), y métricas de contrato. Derivado de `contratos-rest_v1.0.md` §3/§6, de ADR-10/CU-22, de los gates G4/G5 de 08 §3 y del intake §17 P.3. |
