# Entornos y despliegue — geovial-web

**Proyecto:** geovial-web
**Documento:** entornos-deploy_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps + Deploy Engineer

## 0. Alcance y modelo

`geovial-web` es un `web-monolith` desplegable como imagen de contenedor del front (intake §13, §17.P.7, §17.P.9). Su modelo de despliegue es de ambientes de servicio DEV → QA → STAGING → PROD (regla 09 §2.2), no de canales de paquete: el front es un servicio en ejecución, no un artefacto que un consumidor descarga. El front consume por red el contrato REST de `geovial-api` y no comparte proceso ni accede a la persistencia del dominio (05 §5). No tiene almacenamiento de dominio propio (`tiene_persistencia=false`, ADR-02): su estado es efímero y reconstruible desde la API.

La particularidad de despliegue de este proyecto es el circuito interactivo persistente: el front mantiene, por sesión de usuario, una conexión de larga vida que sincroniza el estado de la interfaz con el navegador. Eso exige afinidad de sesión cuando hay más de una réplica del contenedor de front (riesgo identificado en 05 §5 y §9, delegado a esta categoría); se trata en §6.

## 1. Ambientes

Cuatro ambientes de servicio, piso del tipo `web-monolith` (regla 09 §2.2). Cada uno declara su propósito, su aprobador de promoción y su SLA o ventana de soak. La disponibilidad objetivo del front es ≥ 99,5 % mensual en PROD (intake §17.P.10; 05 §8).

| Ambiente | Destino | Propósito | Aprobador de promoción | SLA / ventana |
| --- | --- | --- | --- | --- |
| DEV | Contenedor de front en el entorno de integración continua | Verificación continua de cada merge a `main`; apunta a un `geovial-api` de DEV | Auto (build verde) | — |
| QA | Contenedor de front en el entorno de prueba | Ejecución de la suite de integración y de los escaneos dinámicos; apunta a un `geovial-api` de QA | Auto a verde de gates | Horas a verde de la suite |
| STAGING | Contenedor de front equivalente al productivo | Ambiente de referencia donde se ratifican los NFR numéricos (interacción p95, ≥ 50 circuitos, custodia del token) y se ejecuta la ventana de soak | Release manager | Ventana de soak antes de PROD |
| PROD | Contenedor de front productivo | Operación real para los roles administradores y la carga manual del agente | Release manager + aprobación de negocio | Disponibilidad ≥ 99,5 % mensual |

Notas:

- STAGING es el ambiente de referencia para la validación de NFR (`criterios-validacion_v1.0.md` §3): la latencia de interacción p95 ≤ 200 ms, el sostén de ≥ 50 circuitos concurrentes y la custodia del token (0 exposiciones) se ratifican aquí antes de promover a PROD.
- La disponibilidad del front depende además de la disponibilidad de `geovial-api`; el SLO ≥ 99,5 % se mide sobre el contenedor de front y se reporta junto al del backend (05 §8). La observabilidad no es crítica en esta versión (`tiene_observabilidad_critica=false`, intake §17.P.10): no se fija un SLO ≥ 99,9 % ni un objetivo de latencia p99 numérico.
- Cada ambiente apunta a la instancia de `geovial-api` de su mismo nivel; la dirección del contrato REST es configuración por ambiente (§3), no se hornea en la imagen.

## 2. Provisión (IaC)

La infraestructura de cada ambiente se describe de forma declarativa (regla 09 §4.4): el contenedor de front, su política de réplicas, su afinidad de sesión, su comprobación de salud y su red hacia `geovial-api` y hacia el proveedor del componente de mapa son código versionado, no configuración manual.

| Aspecto | Decisión |
| --- | --- |
| Herramienta | Herramienta de IaC declarativa del entorno objetivo (Terraform, Pulumi, Bicep u homólogo); la elección concreta se alinea a la de la solución |
| Layout | Un módulo del contenedor de front parametrizado por ambiente (réplicas, recursos, afinidad de sesión, variables); composición por ambiente que instancia el módulo con los valores de DEV/QA/STAGING/PROD |
| Política de state | Estado remoto versionado y bloqueado; un solo aplicador a la vez |
| Aprobación del plan | El `plan` se revisa y se aprueba antes del `apply` en STAGING y PROD; en DEV/QA el `apply` puede ser automático |
| Definición de la imagen | El contenedor del front se construye desde la definición de imagen versionada del repositorio (scripts de imagen de `/deploy`, intake §16); 12-factor exige que la misma imagen sirva a todos los ambientes, parametrizada por configuración |

La definición de la afinidad de sesión y de la política de réplicas (§6) vive en el módulo de IaC del contenedor de front, de modo que ningún ambiente quede sin afinidad por descuido.

## 3. Configuración por ambiente (12-factor)

La configuración vive en el entorno, nunca en el código ni en la imagen (12-factor; intake §17.P.5; 05 §7). La misma imagen de contenedor del front se promueve sin recompilar entre ambientes; solo cambia su configuración inyectada.

| Variable (rol) | DEV | QA | STAGING | PROD | Naturaleza |
| --- | --- | --- | --- | --- | --- |
| Dirección del contrato REST de `geovial-api` | API de DEV | API de QA | API de STAGING | API de PROD | Configuración no secreta |
| Parámetros del proveedor del componente de mapa | Endpoint de mapas | Endpoint de mapas | Endpoint de mapas | Endpoint de mapas | Configuración; clave del proveedor es secreto (§4) |
| Credenciales del front hacia el backend | Secreto de DEV | Secreto de QA | Secreto de STAGING | Secreto de PROD | Secreto (§4) |
| Política de afinidad de sesión y réplicas | 1 réplica | 1 réplica | ≥ 2 réplicas con afinidad | ≥ 2 réplicas con afinidad | Configuración de despliegue (§6) |
| Identificador de ambiente para correlación de trazas | `dev` | `qa` | `staging` | `prod` | Configuración no secreta |
| Nivel de registro estructurado | Verboso | Normal | Normal | Normal | Configuración no secreta |

El token bearer del usuario no es configuración de despliegue: vive solo en memoria del circuito durante la sesión, del lado servidor, y nunca se expone al navegador (ADR-03; 05 §5).

## 4. Secretos

Los secretos viven en un gestor de secretos del entorno (vault) y nunca en el control de versiones (intake §17.P.5; 05 §7; regla 09 §4.4 y §4.8 anti-patrón "secretos en commit").

| Aspecto | Decisión |
| --- | --- |
| Gestor | Vault o gestor de secretos del entorno objetivo; un scope de secretos por ambiente |
| Secretos del front | Credenciales del front hacia `geovial-api`; clave o token del proveedor del componente de mapa |
| Inyección | Los secretos se inyectan como variables de entorno o referencias en el arranque del contenedor; no se hornean en la imagen ni quedan en logs |
| Rotación | Rotación periódica declarada (al menos por release mayor o ante sospecha de exposición); la rotación no requiere reconstruir la imagen, solo redesplegar con el nuevo secreto |
| Prohibición de commit | Escaneo de secretos sobre el historial y los PR (`supply-chain-seguridad_v1.0.md` §5); un secreto detectado bloquea el merge y dispara rotación |
| Token de sesión del lado servidor | El token bearer del usuario se retiene del lado servidor del circuito, en memoria, no es un secreto de despliegue ni se persiste; se descarta al cerrar la sesión o reciclar el circuito (ADR-03) |

## 5. Promoción

La promoción entre ambientes está integrada al pipeline (`pipeline-ci-cd_v1.0.md` §4) y mueve la misma imagen firmada, sin reconstruirla. Cada transición tiene su aprobador y su registro auditable (regla 09 §4.4, §4.8 "promotion sin aprobador humano para PROD").

| Transición | Trigger | Aprobador | Registro |
| --- | --- | --- | --- |
| `main` → DEV | Merge a `main` con imagen firmada | Auto | Bitácora de despliegue de DEV |
| DEV → QA | SCA en verde y DAST sin críticos | Auto | Bitácora de despliegue de QA |
| QA → STAGING | Tag `-rc.N`; NFR ratificados sobre el ambiente de referencia | Release manager | Registro de promoción firmado |
| STAGING → PROD | Tag `v<X.Y.Z>` tras la ventana de soak | Release manager + aprobación de negocio | Registro de promoción firmado y auditable |

La promoción a PROD nunca es automática: exige aprobación humana de release y de negocio, con registro auditable. Con `equipo_n=1` (intake §2), el desarrollador asume el rol de release manager; la aprobación de negocio es la única firma externa.

## 6. Afinidad de sesión y política de réplicas del circuito persistente

Tratamiento del riesgo identificado en 05 §5 y §9, delegado a esta categoría (05 §5: "la estrategia concreta de afinidad y de escalado de réplicas pertenece a la categoría 09"). El front sostiene, por sesión de usuario, un circuito interactivo persistente sobre una conexión de larga vida; cuando hay más de una réplica del contenedor de front, las solicitudes de un mismo circuito deben volver siempre a la réplica que lo aloja, porque el estado de UI y el token bearer del usuario viven en memoria de esa réplica (05 §4).

| Aspecto | Decisión |
| --- | --- |
| Afinidad de sesión | Obligatoria con más de una réplica: el balanceador del orquestador de contenedores enruta todas las solicitudes de un circuito a la misma réplica (afinidad por sesión, "sticky session") durante toda la vida del circuito |
| Política de réplicas | DEV y QA: 1 réplica (sin necesidad de afinidad). STAGING y PROD: ≥ 2 réplicas con afinidad de sesión activada, para tolerancia a fallos y para validar el sostén de ≥ 50 circuitos concurrentes (NFR) bajo afinidad |
| Escalado | Escalado horizontal por número de réplicas; al agregar réplicas, los circuitos existentes permanecen en su réplica y los nuevos se distribuyen. El estado por circuito es de tamaño acotado y reconstruible (05 §4), lo que acota el costo de memoria por réplica |
| Pérdida de réplica | Si una réplica cae, sus circuitos se cortan; el front reconecta el navegador a otra réplica y reconstruye el estado de UI consultando a la API (fuente de verdad, ADR-02). No se pierde dato de dominio confirmado |
| Drenaje en redepliegue | Antes de reciclar una réplica (redepliegue o rollback), se aplica una ventana de drenaje breve que deja terminar o migrar los circuitos en curso, reduciendo la interrupción percibida (`pipeline-ci-cd_v1.0.md` §5) |
| Verificación | El NFR de ≥ 50 circuitos concurrentes (STAGE-10 del pipeline) se mide con la afinidad de sesión activa sobre ≥ 2 réplicas en el ambiente de referencia, para validar que la afinidad no degrada la concurrencia objetivo (05 §8) |

El estado efímero por circuito (ADR-02) hace que la afinidad de sesión sea la única exigencia de despliegue del circuito persistente: no se requiere un almacén de sesión compartido entre réplicas, porque la pérdida del circuito no pierde dominio. Esta decisión mitiga el riesgo de "saturación de circuitos concurrentes" y de "pérdida del circuito" de 05 §9.

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Modelo de ambientes (no canales) | Regla 09 §2.2 (web-monolith DEV/QA/STAGING/PROD); intake §17.P.7 |
| Disponibilidad y NFR por ambiente | 05 §8; intake §17.P.10; `criterios-validacion_v1.0.md` §3 |
| Configuración 12-factor y secretos | Intake §17.P.5; 05 §7; `supply-chain-seguridad_v1.0.md` §5 |
| Token del lado servidor | ADR-03; 05 §5, §7 |
| Afinidad de sesión y réplicas | 05 §5, §9 (riesgo delegado a 09); 05 §4 (estado por circuito) |
| Promoción y aprobador | `pipeline-ci-cd_v1.0.md` §4; regla 09 §4.8 |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Entornos y despliegue inicial de geovial-web: modelo de ambientes de servicio DEV/QA/STAGING/PROD (no canales de paquete) con propósito, aprobador y SLA por ambiente; provisión por IaC declarativa con el módulo del contenedor de front parametrizado; configuración 12-factor con la misma imagen promovida entre ambientes; secretos en vault con rotación y prohibición de commit, y el token bearer retenido del lado servidor del circuito; promoción integrada al pipeline con aprobador humano y registro auditable para PROD; y tratamiento del riesgo del circuito persistente (afinidad de sesión obligatoria con ≥ 2 réplicas en STAGING/PROD, escalado horizontal, drenaje en redepliegue y verificación del NFR de concurrencia bajo afinidad), delegado por 05 §5 a esta categoría. |
