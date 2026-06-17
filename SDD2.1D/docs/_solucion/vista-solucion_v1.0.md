# Vista de solución — GeoVial

**Proyecto:** GeoVial (solución)
**Documento:** vista-solucion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Soluciones Senior

## 1. Objetivo y alcance

Esta vista de solución describe GeoVial por encima de la arquitectura de cada uno de sus cinco proyectos: el mapa de proyectos, el grafo de dependencias, los contratos que cruzan fronteras entre proyectos, las decisiones de nivel solución, los cross-cutting que la solución impone a todos sus proyectos, los riesgos de integración inter-proyecto y la trazabilidad de cada contrato a su arista y a los casos de uso que cruzan la frontera. Está dirigida a quien necesita entender cómo se componen los proyectos entre sí: el equipo de desarrollo, los revisores de integración (08), el responsable de despliegue (09) y quien planifica el orden de construcción (07).

La vista referencia, no duplica. El detalle interno de cada proyecto —estilo, vistas C4, modelo de datos, NFR internos y decisiones propias— vive en su `arquitectura-solucion_v1.0.md` bajo `proyectos/<kebab>/05_arquitectura_tecnica/`, y el detalle de cada contrato vive en el `contratos-<area>` del proyecto productor. Aquí se referencian esos documentos y se documenta exclusivamente lo que ocurre en las fronteras entre proyectos. La vista de solución es obligatoria porque GeoVial tiene más de un proyecto (regla 05 §2.1, §4.8).

Documentos hermanos de nivel solución: `contratos-inter-proyecto_v1.0.md` (detalle de los cuatro contratos) y los ADRs bajo `adrs/` (estilo de composición, versionado inter-proyecto, comunicación entre proyectos).

## 2. Mapa de proyectos

Refleja el manifiesto (`SOLUTION-MANIFEST-geovial_v1.0.md` §2) sin divergencias: mismos proyectos, mismos tipos D8 y mismos nombres de código.

| `nombre-proyecto-kebab` | `nombre-proyecto-codigo` | `project_type` (D8) | Rol en la solución | `redistribuible` |
| --- | --- | --- | --- | --- |
| `geovial-api` (principal) | `GeoVial.WebApi` | `rest-api` | Backend monolítico que expone la API REST consumida por el front web y la app móvil; concentra la lógica, la persistencia y la seguridad | false |
| `geovial-web` | `GeoVial.Web` | `web-monolith` | Front web de creación, recolección y revisión de relevamientos sobre mapa | false |
| `geovial-mobile` | `GeoVial.Mobile` | `mobile-app-maui` | App de captura de observaciones en terreno, offline-first, con sincronización | false |
| `geovial-storage` | `GeoVial.Storage` | `library` | Librería de almacenamiento de archivos transparente, con backend de archivos configurable por el usuario raíz; se integra al backend, no se publica como paquete | false |
| `aplicada-sync` | `Aplicada.Sync` | `library` | Librería de sincronización para apps móviles, redistribuible y reutilizable fuera de la solución | true |

Convención de nombres aplicada (manifiesto §1.1): cada proyecto se nombra `GeoVial.<Sufijo>` salvo el redistribuible, que arranca con el prefijo de organización `Aplicada`. `aplicada-sync` es el único `redistribuible: true` y por eso resuelve a `Aplicada.Sync`. Hay exactamente un proyecto principal (`geovial-api`) y no hay colisión de `nombre-proyecto-kebab` ni de `nombre-proyecto-codigo`.

## 3. Grafo de dependencias

DAG del manifiesto (`SOLUTION-MANIFEST-geovial_v1.0.md` §3). Cada arista representa una dependencia de construcción y de contrato del proyecto consumidor hacia el productor.

```text
aplicada-sync   ─┐
geovial-storage ─┼─> geovial-api ─┬─> geovial-web
                 │                └─> geovial-mobile
aplicada-sync ───────────────────────> geovial-mobile
```

Aristas declaradas (cuatro, todas con contrato formal):

| Arista (consumidor → productor) | Productor expone | Contrato |
| --- | --- | --- |
| `geovial-api → geovial-storage` | Abstracción de almacenamiento | C-01 |
| `geovial-web → geovial-api` | Contrato REST | C-02 |
| `geovial-mobile → geovial-api` | Contrato REST | C-03 |
| `geovial-mobile → aplicada-sync` | Contrato de sincronización | C-04 |

El grafo es acíclico (DAG verificado en el manifiesto §4). Orden topológico de construcción, en tres niveles:

```text
nivel 0: aplicada-sync, geovial-storage   (sin dependencias; construibles en paralelo)
nivel 1: geovial-api                       (depende de geovial-storage)
nivel 2: geovial-web, geovial-mobile       (dependen de geovial-api; geovial-mobile también de aplicada-sync; paralelizables)
```

El orden es navegable de izquierda a derecha: ningún proyecto se construye antes que aquel del que depende. Cualquier ciclo sería un defecto del manifiesto y detendría la generación; no lo hay.

## 4. Contratos inter-proyecto

Por cada arista de dependencia con contrato formal, el proyecto productor expone una superficie y el consumidor consume un subconjunto. El detalle completo de cada contrato vive en `contratos-inter-proyecto_v1.0.md`; esta sección los indexa y referencia el `contratos-<area>` del productor.

| # | Arista | Productor | Consumidor | Qué expone el productor | Naturaleza | Contrato de origen (productor) |
| --- | --- | --- | --- | --- | --- | --- |
| C-01 | `geovial-api → geovial-storage` | `geovial-storage` | `geovial-api` | Dos interfaces de la capa Abstractions: almacenamiento (guardar, recuperar, eliminar, verificar, listar) y configuración del proveedor activo; transparencia e igualdad binaria; errores uniformes sin credenciales | En proceso | `proyectos/geovial-storage/.../contratos-abstractions_v1.0.md` |
| C-02 | `geovial-web → geovial-api` | `geovial-api` | `geovial-web` | Contrato REST por recurso bajo versión mayor en la ruta; token bearer, paginación, idempotencia, errores problem+json | Red (REST) | `proyectos/geovial-api/.../contratos-rest_v1.0.md` |
| C-03 | `geovial-mobile → geovial-api` | `geovial-api` | `geovial-mobile` | El mismo contrato REST, con los endpoints de sincronización (subida y bajada) que sirven al cliente offline-first | Red (REST) | `proyectos/geovial-api/.../contratos-rest_v1.0.md` |
| C-04 | `geovial-mobile → aplicada-sync` | `aplicada-sync` | `geovial-mobile` | Superficie pública versionada: operaciones del ciclo subir-luego-bajar y contratos de extensión que el host inyecta; orden, idempotencia y convivencia con conflicto como garantías duras | En proceso (redistribuible) | `proyectos/aplicada-sync/.../contratos-abstractions_v1.0.md` |

Cada contrato corresponde uno a uno con una arista del grafo (§3). La naturaleza de cada comunicación la gobierna ADR-03; la compatibilidad y el orden de publicación, ADR-02. Caso compuesto: el contrato de sincronización (C-04, en proceso) orquesta el ciclo y consume los endpoints de sincronización del contrato REST del backend (C-03, por red) a través de un puerto de transporte que la app móvil implementa.

## 5. Decisiones de nivel solución

Las decisiones que afectan a más de un proyecto viven como ADRs individuales bajo `_solucion/adrs/`, con las mismas reglas de individualidad e inmutabilidad que los ADRs de proyecto (regla 05 §2.1, §3.3).

| ADR | Título | Categoría | Estado | Fecha |
| --- | --- | --- | --- | --- |
| ADR-01 | Estilo de composición: backend monolítico, clientes y librerías | Estilo | Aceptado | 2026-06-15 |
| ADR-02 | Política de versionado inter-proyecto | Despliegue | Aceptado | 2026-06-15 |
| ADR-03 | Estrategia de comunicación entre proyectos | Comunicación | Aceptado | 2026-06-15 |

- ADR-01 fija la composición: un backend autoritativo dueño del dominio, dos clientes sin dominio propio y dos librerías de soporte (una integrada al backend, una redistribuible). Gobierna las cuatro aristas del grafo.
- ADR-02 fija que cada productor publica antes que sus consumidores y mantiene compatibilidad hacia atrás por versión mayor; en particular, el redistribuible `aplicada-sync` se publica y verifica antes de que `geovial-mobile` lo consuma.
- ADR-03 fija dos mecanismos según la frontera: REST autenticado por token bearer y versionado por URI para las fronteras cliente-backend; abstracciones invocadas en proceso por inversión de dependencia para las librerías.

## 6. Cross-cutting compartido

Convenciones transversales que la solución impone a sus proyectos en las fronteras. El detalle interno de cada concern vive en la sección de cross-cutting de cada `arquitectura-solucion`; aquí se fija lo que debe ser coherente entre proyectos.

- Correlación de logging y tracing entre proyectos. El backend emite registros estructurados con un identificador de correlación por solicitud, propagable a la librería de almacenamiento integrada y a las trazas que cruzan hacia los clientes (`geovial-api` cross-cutting §7). El front y la app deben propagar o reflejar ese identificador en sus llamadas al contrato REST para permitir seguir una operación de punta a punta. Ningún registro de ningún proyecto incluye credenciales, el token bearer ni binarios de fotos. La observabilidad no es crítica en esta versión: todos los proyectos declaran `tiene_observabilidad_critica = false` (intake §17.P.10), por lo que no se fija un SLO de disponibilidad ≥ 99,9 % ni una correlación distribuida obligatoria; el identificador de correlación es una convención de diagnóstico, no un requisito de trazado distribuido.
- Formato de errores común. La frontera REST usa problem+json RFC 7807 con código estable en mayúsculas sin tildes, opaco al idioma (definido por el productor en `geovial-api` `contratos-rest_v1.0.md` §5). Los consumidores mapean ese formato a su feedback: el front a feedback de interfaz (`geovial-web` ADR-05), la app a mensaje accionable distinguiendo defecto de integración de condición transitoria. Las librerías exponen sus propios catálogos de errores estables (uniforme por proveedor en `geovial-storage`; con distinción de transitorio/reanudable en `aplicada-sync`); el backend normaliza los códigos de la librería de almacenamiento al cruzar a su contrato REST.
- Autenticación común. La frontera cliente-backend usa token bearer presentado en la cabecera de autorización (ADR-03). El backend es el único emisor y validador del token; no hay proveedor de identidad externo (`geovial-api` §17.P.5). Cada consumidor custodia el token según su naturaleza: el front del lado servidor del circuito sin exponerlo al navegador (`geovial-web` ADR-03), la app en el almacén seguro del dispositivo (`geovial-mobile` ADR-05); la librería de sincronización reutiliza el token de la app host por el proveedor de credencial que esta le inyecta.
- Gestión de versiones de los paquetes compartidos y del redistribuible. Todos los proyectos adoptan SemVer 2.0.0 y Conventional Commits (intake §17.P.7). El versionado inter-proyecto lo gobierna ADR-02: el productor publica antes que el consumidor y conserva compatibilidad hacia atrás por versión mayor. `aplicada-sync`, único redistribuible, se publica al feed con verificación post-publish (restauración en un proyecto limpio) antes de su consumo; `geovial-storage` se versiona alineada al ciclo del backend porque se integra a él y no se distribuye. El contrato REST conserva la versión mayor previa durante al menos un MINOR para que los dos clientes migren escalonadamente.
- Secretos. Ningún proyecto incluye secretos en el control de versiones: la clave de firma de tokens y la cadena de conexión del backend, las credenciales de proveedor de la librería de almacenamiento y el token de los clientes se inyectan desde el gestor de secretos del entorno o el almacén seguro del dispositivo (intake §17.P.5 de cada proyecto).

## 7. Riesgos de integración inter-proyecto

Riesgos enfocados en las fronteras entre proyectos. Los riesgos internos de cada proyecto viven en la sección de riesgos de su `arquitectura-solucion`.

| Riesgo | Impacto | Probabilidad | Mitigación |
| --- | --- | --- | --- |
| Cambio incompatible del contrato REST rompe a los dos clientes a la vez | Alto: `geovial-web` y `geovial-mobile` dejan de operar | Baja | Versionado por URI con convivencia de la versión mayor previa de al menos un MINOR (ADR-02; `geovial-api` ADR-10); contract tests del 100 % de endpoints en CI del backend; los clientes fijan la versión mayor que consumen |
| El redistribuible `aplicada-sync` se consume antes de publicarse, rompiendo la construcción de `geovial-mobile` | Alto: el cliente no compila | Media | El productor publica antes que el consumidor y se verifica post-publish en un proyecto limpio (ADR-02); el pipeline codifica el orden de publicación (librerías → backend → clientes) |
| Cambio incompatible del contrato de la librería de almacenamiento rompe al backend | Medio: el backend no aloja ni recupera fotos | Baja | Compatibilidad por versión mayor del contrato de Abstractions; los cambios menores (nuevo proveedor, operación u opción) no rompen al consumidor; coordinación explícita en cambios mayores (`geovial-storage` contrato §6) |
| Cambio incompatible del contrato de sincronización rompe el ciclo de la app | Medio: la app no sincroniza | Baja | Versionado semántico de la superficie pública con período de deprecación obligatorio; orden, idempotencia y convivencia son garantías duras estables; la app fija la versión mayor que integra (`aplicada-sync` contrato §6) |
| Orden de despliegue incorrecto entre librerías, backend y clientes | Medio: una versión del consumidor opera contra una versión del productor que no existe o ya se retiró | Media | Orden topológico del manifiesto codificado en el pipeline (ADR-02); construcción de nivel 0 antes de nivel 1 antes de nivel 2; rollback por productor sin obligar a revertir consumidores mientras la versión mayor no cambie |
| Doble contrato en la sincronización del móvil (REST + contrato de sincronización) acoplado al cambiar uno de los dos | Medio: el ciclo subir-luego-bajar falla en una frontera | Baja | Cada contrato versiona su compatibilidad por separado (ADR-02, ADR-03); el puerto de transporte que la app implementa aísla el motor del detalle del contrato REST; pruebas de sincronización de punta a punta en 08 |
| Pérdida de correlación de logs entre proyectos dificulta diagnosticar una operación de punta a punta | Bajo: mayor costo de diagnóstico, sin pérdida de datos | Media | Identificador de correlación propagado en la frontera REST (§6); aceptado como convención no crítica porque `tiene_observabilidad_critica = false` en todos los proyectos |

## 8. Trazabilidad

Cada contrato inter-proyecto se liga a la arista del manifiesto que materializa y a los casos de uso que cruzan la frontera entre proyectos. La fuente normativa de cada contrato es el `contratos-<area>` del productor; el detalle por contrato vive en `contratos-inter-proyecto_v1.0.md` §7.

| Contrato | Arista del manifiesto | Productor / contrato de origen | Consumidor | CU que cruzan la frontera | ADR de solución que lo gobierna |
| --- | --- | --- | --- | --- | --- |
| C-01 | `geovial-api → geovial-storage` | `geovial-storage` / `contratos-abstractions_v1.0.md` | `geovial-api` | CU-08, CU-09, CU-15, CU-16, CU-17 (backend) ↔ CU-01..CU-06 (storage) | ADR-01, ADR-03 |
| C-02 | `geovial-web → geovial-api` | `geovial-api` / `contratos-rest_v1.0.md` | `geovial-web` | CU-01..CU-11 (web) ↔ subconjunto administrador de CU-01..CU-22 (backend) | ADR-01, ADR-02, ADR-03 |
| C-03 | `geovial-mobile → geovial-api` | `geovial-api` / `contratos-rest_v1.0.md` | `geovial-mobile` | CU-01..CU-07 (móvil) ↔ subconjunto de captura/sync incl. CU-10, CU-11 (backend) | ADR-01, ADR-02, ADR-03 |
| C-04 | `geovial-mobile → aplicada-sync` | `aplicada-sync` / `contratos-abstractions_v1.0.md` | `geovial-mobile` | CU-06 (móvil) ↔ CU-01..CU-06 (sync); compone C-03 (CU-10, CU-11) | ADR-01, ADR-02, ADR-03 |

Cada fila corresponde a una arista real del grafo del manifiesto (§3) y referencia el `contratos-<area>` del proyecto productor. No existe contrato inter-proyecto que no corresponda a una arista, ni dependencia con contrato formal que no esté documentada en esta tabla y en `contratos-inter-proyecto_v1.0.md`.

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Vista de solución inicial de GeoVial con las ocho secciones de la regla 05 §4.8: objetivo y alcance, mapa de cinco proyectos (reflejo del manifiesto), grafo de dependencias acíclico con orden topológico de tres niveles, índice de los cuatro contratos inter-proyecto, índice de los tres ADRs de nivel solución, cross-cutting compartido (correlación, errores problem+json, autenticación por token bearer, versionado de paquetes y redistribuible, secretos), riesgos de integración inter-proyecto y trazabilidad contrato ↔ arista ↔ CU. |
