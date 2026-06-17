# Entornos y distribución — geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** entornos-deploy_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps + Mobile Release Engineer

## 1. Modelo de distribución: canales, no ambientes

`geovial-mobile` es una app móvil de plataforma única (Android); su artefacto es un paquete de aplicación que se instala en el dispositivo del agente de campo, no un servicio desplegado en un ambiente. Por eso el modelo es de canales de distribución móvil `internal`, `alpha`, `beta` y `production` (regla 09 §2.2 para `mobile-app-maui`), no de ambientes de servicio DEV/QA/STAGING/PROD. Confundir los canales con ambientes de servicio es el anti-patrón 09 §4.8 "confundir publicación con despliegue"; este documento mantiene la distinción.

Los cuatro canales se sirven por un canal de distribución interno (no se publica en tienda pública en v1, intake §17 P.7). Cada canal se alimenta de su propio tag (ver `estrategia-versionado_v1.0.md` §5.2) y se promueve según `pipeline-ci-cd_v1.0.md` §6.

| Canal | Destino | Aprobador | Audiencia | Ventana de soak |
| --- | --- | --- | --- | --- |
| `internal` | Canal de distribución interno, grupo de validación interna | Automático | Equipo (el dev en `equipo_n=1`) | — |
| `alpha` | Canal de distribución interno, grupo de campo acotado | Mobile Release Engineer | Probadores de campo seleccionados | ≥ 1 ciclo de campo sin blocker abierto |
| `beta` | Canal de distribución interno, grupo de piloto | Mobile Release Engineer | Piloto de campo ampliado (mitiga el riesgo de adopción, intake §11) | ≥ 1 piloto sin blocker abierto |
| `production` | Canal de distribución interno, grupo de operación | Mobile Release Engineer (aprobación registrada de forma auditable) | Todos los agentes de campo en operación | — |

No hay SLA de disponibilidad de servicio que declarar: el proyecto fija `tiene_observabilidad_critica = false` (intake §17 P.10), sin SLO de disponibilidad ≥ 99,9 % ni objetivo de latencia p99. El equivalente al "SLA" de cada canal es la ventana de soak y la verificación post-distribución (ver `guia-publicacion-store-mobile_v1.0.md` §3), no un objetivo de uptime de servicio. Los NFR que se verifican antes de promover a un canal de prueba o de producción son los de campo: captura 100 % offline, cola ≥ 1000, ciclo de 100 cambios ≤ 30 s, reanudación sin pérdida y arranque ≤ 3 s (05 §8; intake §17 P.10), medidos en el dispositivo de referencia.

## 2. Provisión

La app no provisiona infraestructura de servicio (no hay clúster, base ni balanceador propios): la unidad de despliegue es el paquete de aplicación que corre en el dispositivo del agente (05 §5). La "infraestructura" que sí se gestiona de forma declarativa y versionada es la configuración del pipeline y de los canales de distribución:

- La definición del pipeline, los triggers por evento y la matriz de plataforma se versionan con el repositorio (configuración como código), de modo que un cambio de canal o de gate pasa por PR.
- La configuración de los grupos de distribución de cada canal (quién recibe `internal`, `alpha`, `beta`, `production`) se mantiene en el panel de distribución y se documenta acá; su cambio queda registrado.
- El plan de la configuración del pipeline se revisa en el PR antes de aplicarse a `main`, lo que cumple la práctica de revisar el cambio antes de su efecto.

## 3. Configuración por canal (12-factor)

La configuración vive en variables de entorno o en archivos referenciados por canal, nunca en código (12-factor; anti-patrón 09 §4.8 "secretos en commit" evitado). Mapa de configuración por canal:

| Parámetro | `internal` | `alpha` | `beta` | `production` |
| --- | --- | --- | --- | --- |
| Identificador del host remoto del contrato REST | Host de prueba | Host de prueba | Host de preproducción | Host de producción |
| Nivel de registro de eventos local | Detallado | Detallado | Normal | Normal |
| Grupo de distribución | Validación interna | Campo acotado | Piloto | Operación |
| Indicador de build (visible en la app) | Sí | Sí | Sí | No |

El identificador del host remoto y los parámetros de sesión se inyectan a la app en la inicialización de la librería de sincronización (05 §5); el token y los secretos nunca viajan en texto plano y se custodian en el almacén seguro del dispositivo (05 §7, ADR-05).

## 4. Secretos

| Secreto | Dónde vive | Rotación | Scope |
| --- | --- | --- | --- |
| Credencial de firma (almacén de claves de firma) | Almacén seguro del pipeline (gestor de secretos), nunca en el repositorio | Rotación planificada y ante sospecha de compromiso; documentada en `supply-chain-seguridad_v1.0.md` §2 | Solo el stage Firma del paquete del pipeline |
| Clave de acceso al canal de distribución interno | Gestor de secretos del pipeline | Rotación periódica | Solo el stage de distribución |
| Credenciales de prueba (no productivas) | Entorno de pruebas del pipeline | Por corrida | Suite de pruebas; nunca productivas |
| Token bearer del agente (en runtime) | Almacén seguro del dispositivo (no es secreto del pipeline) | Deslogueo completo lo borra; relogueo por seguridad del dispositivo (RN-04) | Sesión del agente en el dispositivo |

Prohibición explícita: ningún secreto, y en particular la credencial de firma, se versiona en el repositorio ni se imprime en logs (05 §7; 08 `estrategia-testing_v1.0.md` §7). El escaneo de secretos en commits es parte del supply chain (`supply-chain-seguridad_v1.0.md` §4).

## 5. Promoción

La promoción entre canales está integrada con el pipeline (`pipeline-ci-cd_v1.0.md` §6) y se dispara por el tag correspondiente:

1. `internal`: automático desde `-internal.N`, sin aprobador humano; prerrequisito de gates Compilación, Pruebas en verde, Cobertura, Análisis estático y Snapshot verdes más SBOM y firma.
2. `internal → alpha`: tag `-alpha.N`; aprobador Mobile Release Engineer; prerrequisito adicional del stage NFR de campo verde.
3. `alpha → beta`: tag `-beta.N`; aprobador Mobile Release Engineer; prerrequisito de soak de `alpha` sin blocker.
4. `beta → production`: tag `vX.Y.Z` sin sufijo; aprobador Mobile Release Engineer con aprobación registrada de forma auditable; prerrequisito de todos los gates, los criterios de `08_calidad_y_pruebas/criterios-validacion_v1.0.md`, las ADR-01 a ADR-05 ratificadas y verificación post-distribución.

Registro de auditoría: cada promoción queda ligada al run del pipeline, al tag y al aprobador; con `equipo_n=1` el registro auditable sustituye la aprobación por un segundo par (08 §4). La promoción nunca salta canales sin su tag: no se distribuye a `production` un binario que no pasó por su propio tag estable.

## 6. Trazabilidad

- Los canales y sus tags provienen de `estrategia-versionado_v1.0.md` §5.2; los gates prerrequisito de cada promoción son los de 08 `estrategia-calidad_v1.0.md` §3 ejecutados como stages en `pipeline-ci-cd_v1.0.md` §3.
- Los NFR que condicionan la promoción a canales de prueba y de producción son los de 05 §8 e intake §17 P.10.
- La credencial de firma y su resguardo se detallan en `supply-chain-seguridad_v1.0.md` §2; el procedimiento de distribución y verificación, en `guia-publicacion-store-mobile_v1.0.md`.

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Modelo de distribución inicial de geovial-mobile: canales de distribución móvil internal/alpha/beta/production sobre un canal de distribución interno (no ambientes de servicio), cada uno con aprobador, audiencia y ventana de soak; configuración 12-factor por canal; secretos (incluida la credencial de firma resguardada) en almacén seguro y nunca en commit; promoción integrada con el pipeline y registro de auditoría para equipo_n=1. Sin SLA de uptime por tiene_observabilidad_critica = false; los NFR de campo condicionan la promoción. Derivado del intake §17 P.7/P.10, de 05 §5/§7/§8 y de la regla 09 §2.2 para mobile-app-maui. |
