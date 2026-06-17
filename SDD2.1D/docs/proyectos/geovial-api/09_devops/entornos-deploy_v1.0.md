# Entornos y despliegue — geovial-api

**Proyecto:** geovial-api
**Documento:** entornos-deploy_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps + Platform Engineer

## 1. Modelo de ambientes

`geovial-api` es de tipo `rest-api` y se despliega como una imagen de contenedor; su modelo de ambientes son ambientes de servicio desplegable DEV / QA / STAGING / PROD con despliegue canary, no canales de distribución de paquete (regla §2.2 para `rest-api`; anti-patrón §4.8 "confundir publicación con despliegue"). Cada ambiente aloja una instancia desplegada del contenedor de backend que expone la API REST, con su propio almacén relacional y su destino de almacenamiento de fotos (`arquitectura-solucion_v1.0.md` §5). El contrato OpenAPI versionado se publica al hub de contratos como artefacto consumible por los clientes (ver `guia-publicacion-openapi_v1.0.md`); no es un ambiente.

| Ambiente | Propósito | Destino | Aprobador | SLA / ventana |
| --- | --- | --- | --- | --- |
| DEV | Integración continua del último merge a `main`; validación funcional del desarrollador | Instancia de desarrollo del contenedor de backend | Automático | Sin SLA; despliegue continuo desde `main` |
| QA | Verificación de la suite de integración y de regresión sobre un candidato a release | Instancia de QA del contenedor de backend | QA lead | Horas a verde de la suite de QA |
| STAGING | Ambiente equivalente al productivo; medición de los NFR numéricos antes de PROD | Instancia equivalente a producción del contenedor de backend | Release manager | Ventana de soak previa a PROD |
| PROD | Servicio en producción consumido por `geovial-web` y `geovial-mobile` | Instancia productiva del contenedor de backend, despliegue canary | Release manager + aprobación de negocio | Disponibilidad ≥ 99,5 % mensual (NFR 05 §8) |

Los cuatro ambientes son piso (regla §2.2); el equipo no puede quitar ninguno sin un ADR. STAGING es el ambiente de medición del gate G8 porque debe ser equivalente al productivo (criterios-validacion §3; 05 §8).

### 1.1 Despliegue canary y NFR por ambiente

| Ambiente | NFR observado o medido | Cómo |
| --- | --- | --- |
| QA | Integridad de jerarquía y ciclo (0 violaciones bajo concurrencia); idempotencia 100 % | Suite de integración contra base efímera (TC-29, TC-30, TC-33) |
| STAGING | Latencia p95 lecturas ≤ 300 ms; p95 escrituras ≤ 500 ms; lote de sincronización ≥ 1000 sin pérdida ni duplicación | Pruebas de carga (gate G8; TC-21, TC-22, TC-31) en ambiente equivalente al productivo |
| PROD | Disponibilidad mensual ≥ 99,5 % | Sondas de salud del contenedor de backend; métrica observada, sin SLO ≥ 99,9 % (`tiene_observabilidad_critica=false`, intake §17 P.10) |

El despliegue a PROD es canary: la versión nueva recibe tráfico incremental (5 % → 25 % → 100 %) verificando en cada paso las sondas de salud y los puntos de medición de latencia y de tasa de error por código (`arquitectura-solucion_v1.0.md` §7). La detención del avance y el rollback rápido por desvío de tráfico viven en `pipeline-ci-cd_v1.0.md` §6/§7.

## 2. Provisión (IaC)

La infraestructura de cada ambiente se declara como código versionado en el repositorio (herramienta declarativa de infraestructura, intake §16 carpeta `/deploy`), de modo que DEV, QA, STAGING y PROD se aprovisionen de forma reproducible y STAGING sea equivalente al productivo.

| Recurso del ambiente | Declarado como | Notas |
| --- | --- | --- |
| Contenedor de backend | Manifiesto de despliegue del orquestador de contenedores | Imagen por digest inmutable (`pipeline-ci-cd_v1.0.md` §5); réplicas y sondas de salud/preparación |
| Almacén relacional | Recurso de base declarado por ambiente | Migraciones aplicadas en arranque controlado antes de habilitar tráfico (ADR-02, 05 §5) |
| Destino de almacenamiento de fotos | Volumen persistente (proveedor local) o servicio de objetos remoto según configuración | El destino local debe ser persistente para no perder evidencia al reciclar el contenedor (05 §5, riesgo arquitectónico; ADR-09) |
| Enrutamiento y desvío de tráfico | Regla de tráfico del orquestador | Habilita el canary y el rollback por traffic shift |

- Layout de módulos: un módulo base reutilizable y una sobrescritura por ambiente (DEV/QA/STAGING/PROD) que parametriza réplicas, tamaño y endpoints, sin duplicar la definición.
- Política de state: el estado de la infraestructura se gestiona de forma remota y bloqueada; ningún `apply` corre sin un `plan` revisado y aprobado.
- Aprobación de `plan` antes de `apply`: el cambio de infraestructura de STAGING y PROD requiere aprobación humana del Release manager antes de aplicarse (registro auditable).

## 3. Configuración por ambiente (12-factor)

La configuración vive en variables de entorno o en la configuración del despliegue referenciada, nunca en el código ni en la imagen (12-factor; intake §17 P.5; 05 §7). La imagen de contenedor es idéntica entre ambientes; lo que cambia es la configuración inyectada.

| Variable (rol abstracto) | Propósito | DEV | QA | STAGING | PROD |
| --- | --- | --- | --- | --- | --- |
| Cadena de conexión al almacén relacional | Endpoint y credencial del almacén del ambiente | Por ambiente (secreto) | Por ambiente (secreto) | Por ambiente (secreto) | Por ambiente (secreto) |
| Proveedor de almacenamiento activo y sus parámetros | Destino de las fotos (local o de objetos remoto) | Local | Local | Equivalente a PROD | Configurado por el usuario raíz (CU-17) |
| Clave de firma del token bearer | Emisión y validación del token por el propio backend (ADR-03) | Por ambiente (secreto) | Por ambiente (secreto) | Por ambiente (secreto) | Por ambiente (secreto) |
| Parámetros del token (vigencia) | Política de expiración del token | Por ambiente | Por ambiente | Equivalente a PROD | Por ambiente |
| Versión del runtime / etiqueta de imagen | Fijar el digest desplegado | Última de `main` | Tag `-rc.N` | Tag `-rc.N` aprobado | Tag estable `vX.Y.Z` |
| Versiones mayores del contrato vigentes | Convivencia de `/v1` (y `/v2` durante migración) | Según versión | Según versión | Según versión | Según versión (ADR-10) |

La configuración del proveedor de almacenamiento la ejerce solo el usuario raíz vía la API (CU-17, ADR-09); el pipeline no la fija salvo el destino por defecto del ambiente.

## 4. Secretos

Los secretos viven en un gestor de secretos del entorno (vault), nunca en el control de versiones ni en la imagen (intake §17 P.5; 05 §7/§9; anti-patrón §4.8 "secretos en commit").

| Secreto | Almacén | Scope mínimo | Rotación |
| --- | --- | --- | --- |
| Clave de firma del token bearer | Vault del entorno por ambiente | Emisión y validación del token del backend del ambiente | Periódica y ante sospecha de exposición; invalida sesiones |
| Cadena de conexión al almacén relacional | Vault del entorno por ambiente | Acceso del backend a su propio almacén | Periódica; coordinada con la rotación de credenciales del almacén |
| Credenciales del proveedor de almacenamiento de fotos | Vault del entorno por ambiente | Acceso del backend al destino de objetos remoto cuando aplica (ADR-09) | Periódica; las credenciales no salen por ninguna respuesta del backend (RN-03 de storage; ADR-09) |
| Credencial de publicación de imagen y de contrato | Vault del CI | Escritura al registro de imágenes y al hub de contratos | Periódica y ante sospecha |
| Clave o identidad de firma del artefacto y del SBOM | Vault del CI o keyless con transparency log | Firma de la imagen y del SBOM | Según política de la identidad de firma |

La decisión arquitectónica pertinente a las credenciales del proveedor de almacenamiento es ADR-09 (integración de la abstracción de almacenamiento): el backend las recibe del vault del entorno y nunca las expone en una respuesta ni en un error (RN-03 de storage). El scan de secretos en commits es parte del dependency/secret scanning (`supply-chain-seguridad_v1.0.md` §4).

## 5. Promoción

La promoción entre ambientes está integrada al pipeline (`pipeline-ci-cd_v1.0.md` §6) y requiere aprobador con registro de auditoría:

- A DEV: automática desde un merge a `main`, con G1-G6 verdes, SBOM presente e imagen firmada. Sin aprobador humano.
- A QA: tag `vX.Y.Z-rc.N`; prerrequisito la suite de integración verde y el despliegue DEV saludable. Aprobador: QA lead.
- A STAGING: aprobación tras QA en verde y G7 (regresión) verde; ventana de soak en QA cumplida. Aprobador: Release manager.
- A PROD: tag `vX.Y.Z` sin sufijo, con G8 (NFR numéricos) medidos y cumplidos en STAGING y ventana de soak de STAGING cumplida; despliegue canary. Aprobador: Release manager + aprobación de negocio, con registro auditable del aprobador y del tag (anti-patrón §4.8 "promotion sin aprobador humano para PROD").

Con `equipo_n=1` (intake §13; 08 §4), QA lead, Release manager y aprobación de negocio los ejerce la misma persona en momentos distintos; el registro de la corrida del pipeline ligada al tag y al aprobador es la evidencia de auditoría que sustituye la separación de roles.

## 6. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Modelo de ambientes (no canales) | Regla §2.2 (rest-api); anti-patrón §4.8 "confundir publicación con despliegue" |
| Vista de despliegue (contenedor, almacén, almacenamiento) | `arquitectura-solucion_v1.0.md` §5; ADR-02, ADR-09 |
| NFR por ambiente | 05 §8; intake §17 P.10; gate G8 (08 estrategia-calidad §3) |
| Secretos en vault | intake §17 P.5; 05 §7/§9; ADR-09; `supply-chain-seguridad_v1.0.md` §4 |
| Versionado del contrato vigente por ambiente | ADR-10; CU-22; `estrategia-versionado_v1.0.md` §6 |
| Promoción y canary | `pipeline-ci-cd_v1.0.md` §6/§7 |
| Downstream | developer guide de 10 (levantar ambiente local); examples de 11 |

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Modelo de ambientes inicial de geovial-api por ambientes de servicio desplegable DEV/QA/STAGING/PROD con despliegue canary (no canales de paquete), conforme a la regla §2.2 para rest-api. Declara propósito, aprobador y SLA/ventana por ambiente; NFR medido u observado por ambiente con STAGING equivalente al productivo para el gate G8; provisión por IaC versionada con aprobación de plan; configuración 12-factor por ambiente con imagen idéntica entre ambientes; secretos en vault por ambiente con rotación y prohibición de commit (incluidas las credenciales del proveedor de almacenamiento, ADR-09); promoción integrada al pipeline con aprobador y registro auditable. Derivado de 05 §5/§7/§8/§9, de ADR-02/ADR-09/ADR-10 y del intake §17 P.5/P.7/P.10. |
