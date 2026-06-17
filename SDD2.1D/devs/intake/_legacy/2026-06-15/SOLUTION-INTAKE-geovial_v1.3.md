# SOLUTION-INTAKE-geovial

| Campo | Valor |
|---|---|
| Nombre de la solución | GeoVial |
| Cliente / Stakeholder principal | PENDIENTE |
| Repositorio | `Ejemplos_IA_SDD2.1D_Sistema_Georreferencia` (URL PENDIENTE) |
| Lead técnico | PENDIENTE |
| Documento | `SOLUTION-INTAKE-geovial_v1.0.md` |
| Versión | 1.3 |
| Fecha | 2026-06-15 |
| Stack principal | .NET / C# — backend `rest-api` sobre SQL Server; front `web-monolith` Blazor Interactive Server + MudBlazor; móvil .NET MAUI (Blazor Hybrid) + MudBlazor + SQLite; mapas OpenStreetMap + Leaflet |
| Estado | Borrador |

> Este documento captura qué quiere el cliente, cómo se compone la solución y cómo se construye cada proyecto. El orquestador deriva de §13 el `SOLUTION-MANIFEST` canónico; no se completa el manifiesto a mano.

---

# Parte A — Negocio de la solución

## §1 Idea y problema

GeoVial es una solución para el relevamiento fotográfico georreferenciado de tramos viales (puentes y caminos). Un relevamiento corresponde a un tramo vial —que puede abarcar varios puentes y caminos— y consiste en registrar una serie de observaciones del estado de ese tramo: notas, comentarios y fotografías ancladas a un punto geográfico. El jefe de área crea el relevamiento, lo asigna a agentes de campo, la cuadrilla recolecta las observaciones en terreno y luego el jefe revisa y evalúa la información recabada para la confección de sus informes rutinarios y el cierre.

El trabajo de campo ocurre en lugares donde no se puede asumir conectividad: la captura de observaciones técnicas debe poder hacerse sin internet y sincronizarse después. La solución estructura esa recolección alrededor de marcadores geográficos que agrupan fotos, comentarios y etiquetas, y ofrece una revisión visual sobre mapa que permite confeccionar los informes.

Consecuencia de no construirlo y disparador externo (por qué ahora): hoy el relevamiento de tramos viales se hace con métodos manuales (planillas y fotos sueltas sin georreferencia confiable), con pérdida de trazabilidad entre cada foto y su ubicación, retrabajo en oficina y demoras en los informes; en tramos sin conectividad no se puede registrar en el momento. El disparador es la necesidad de estandarizar y digitalizar el relevamiento para que los informes rutinarios del jefe de área sean trazables y reproducibles.

## §2 Audiencia y stakeholders

La jerarquía de usuarios quedó definida en cuatro niveles, de mayor a menor alcance: usuario raíz → jefe general → jefe de área → agente de campo. Los stakeholders de propiedad e implementación quedaron definidos en la consolidación de Parte A (ver tabla).

| Rol | Nombre o cargo | Categoría | Responsabilidad principal |
|---|---|---|---|
| Dueño del problema / aprobador del intake | Vialidad provincial | Propietario | Aprueba el intake |
| Equipo de desarrollo | Departamento de desarrollo de software (1 dev) | Implementador | Construye y mantiene la solución |
| Usuario raíz | Rol del sistema | Beneficiario | Administra todo el sistema con acceso pleno; configura el sistema y da de alta al jefe general |
| Jefe general | Rol del sistema | Beneficiario | Administra a los jefes de área |
| Jefe de área | Rol del sistema | Beneficiario | Administra a los agentes de campo (altas/bajas), administra los relevamientos y los asigna a los agentes, y revisa los relevamientos para su cierre |
| Agente de campo | Rol del sistema | Beneficiario | Toma los relevamientos asignados e ingresa y administra sus observaciones |

## §3 Propuesta de valor y diferenciación

La promesa central es capturar observaciones georreferenciadas en el lugar del tramo vial, aun sin conexión, agrupándolas por marcador geográfico con fotos, comentarios y etiquetas, y revisarlas después sobre un mapa para confeccionar informes.

Diferenciadores derivados del alcance pretendido:
- Captura móvil offline-first con sincronización automática al detectar conexión.
- Georreferenciación en el momento de la captura y, en carga manual, a partir de los metadatos EXIF de la foto, con un parámetro de radio para agrupar fotos en un mismo marcador.
- Tolerancia a conflictos: el relevamiento se crea y la información queda accesible aunque haya marcadores en conflicto; la resolución se difiere al cierre.
- Revisión visual sobre mapa con carrusel de fotos por marcador que encadena marcadores contiguos.
- Exportación/importación de un relevamiento completo (comentarios, etiquetas y fotos) en un único archivo ZIP.

Qué hace hoy el cliente y por qué no le alcanza: PENDIENTE. Diferenciador defendible frente a la competencia: PENDIENTE.

## §4 Alcance funcional pretendido (MoSCoW)

Las capacidades surgen del material de descubrimiento. Las etiquetas MoSCoW son provisorias y requieren confirmación del cliente (ver §PENDIENTES).

| ID | Capacidad | MoSCoW |
|---|---|---|
| F-01 | Jerarquía y administración de usuarios en cuatro niveles (raíz → jefe general → jefe de área → agente), con altas/bajas según jerarquía | Must Have |
| F-02 | Alta y baja de agentes de campo directamente por el jefe de área | Must Have |
| F-03 | Alta/baja/visualización de relevamientos por el jefe de área; un relevamiento abarca un tramo vial (uno o varios puentes y caminos) | Must Have |
| F-04 | Asignación de agentes de campo a un relevamiento | Must Have |
| F-05 | Captura móvil de observaciones con foto + resolución de coordenadas geográficas en el momento | Must Have |
| F-06 | Modelo de observación: marcador geográfico con notas, fotos, comentarios por foto y etiquetas; marcador compartible por varias observaciones | Must Have |
| F-07 | Captura offline con sincronización (sube cambios locales y luego baja actualizaciones del relevamiento asignado) | Must Have |
| F-08 | Login online inicial con credenciales; deslogueo completo para cambio de usuario; relogueo en sesión activa por la seguridad del dispositivo (patrón, huella) | Must Have |
| F-09 | Carga manual con priorización de EXIF y radio de agrupación de fotos en un marcador | Must Have |
| F-10 | Visualización en mapa (OpenStreetMap/Leaflet) con pines: mover el pin y centrar por GPS en móvil | Must Have |
| F-11 | Transición de estado recolección → revisión y cierre del relevamiento por el jefe | Must Have |
| F-12 | Carrusel de fotos por marcador con encadenado al marcador siguiente/anterior; ampliar, comentar, etiquetar y filtrar | Should Have |
| F-13 | Resolución de conflictos de marcadores al cierre del relevamiento (el sistema convive con los conflictos durante la recolección y la información queda accesible) | Should Have |
| F-14 | Reasignación de agentes a un relevamiento desde la app móvil por el jefe | Should Have |
| F-15 | Carga manual completa del relevamiento vía web por el agente | Should Have |
| F-16 | Exportar/importar relevamiento completo (comentarios, etiquetas y fotos) en un único ZIP | Could Have |
| F-17 | Configuración del backend de almacenamiento de archivos (local / Amazon S3 / otro) por el usuario raíz | Could Have |
| F-18 | Flujo de auto-registro o solicitud/aceptación self-service de agentes: las altas las hace directamente el jefe de área | Won't Have v1 |

## §5 Historias de usuario / experiencias deseadas

- Como jefe de área, quiero crear un relevamiento de un tramo vial y asignarle agentes de campo, para organizar la recolección en terreno.
- Como agente de campo, quiero capturar fotos georreferenciadas con comentarios y etiquetas sin conexión a internet, para relevar el estado del tramo en el lugar.
- Como agente de campo, quiero que la app sincronice mis observaciones automáticamente cuando recupero conexión, para que el jefe disponga de los datos recolectados.
- Como jefe de área, quiero revisar las observaciones sobre un mapa y resolver los conflictos de marcadores al cierre, para confeccionar mis informes rutinarios y cerrar el relevamiento.

## §6 Flujos típicos

Flujo 1 — Alta y asignación (jefe de área):
1. El jefe de área crea un relevamiento del tramo vial a relevar.
2. Crea uno o más marcadores geográficos iniciales para previsualizar la experiencia.
3. Asigna agentes de campo al relevamiento.
4. Deja el relevamiento en estado de recolección.

Flujo 2 — Recolección en campo offline (agente de campo):
1. El agente abre la app, ya logueado, y selecciona uno de sus relevamientos asignados.
2. Sobre el mapa centra por GPS, toca el pin general y crea un marcador en la posición tomada del GPS (o la mueve).
3. Captura fotos; la app resuelve las coordenadas y asocia las fotos al entorno del marcador, con comentarios y etiquetas.
4. Trabaja sin internet; al detectar conexión, la app sincroniza: primero sube cambios locales y luego baja las últimas actualizaciones.

Flujo 3 — Revisión y cierre (jefe de área):
1. El jefe abre el relevamiento sobre el mapa y recorre los marcadores con sus fotos en carrusel.
2. Al cerrar, resuelve los conflictos pendientes (por ejemplo, dos marcadores dentro de un mismo radio: unificar o no), que hasta ese momento convivieron sin bloquear el acceso a la información.
3. Pasa el relevamiento de recolección a revisión.
4. Cuando lo considera terminado, lo cierra para sus informes.

## §7 Casos límite y "qué pasa si"

- ¿Qué pasa si hay dos o más marcadores dentro de un mismo radio? → Respuesta del cliente: el sistema convive con el conflicto durante la recolección; los marcadores coexisten y la información queda accesible. El conflicto solo afecta la estructura de catalogación de las observaciones y se resuelve al cerrar el relevamiento.
- ¿Qué pasa si dos agentes sincronizan cambios conflictivos sobre el mismo relevamiento o marcador? → Respuesta del cliente: PENDIENTE (se asume la misma política de convivencia y resolución al cierre; confirmar).
- ¿Qué pasa si se pierde la conexión en medio de una sincronización (subida parcial de cambios)? → Respuesta del cliente: PENDIENTE.
- ¿Qué pasa si una foto cargada manualmente no tiene metadatos EXIF de geolocalización? → Respuesta del cliente: PENDIENTE.
- ¿Qué pasa si el dispositivo no obtiene señal de GPS al momento de capturar? → Respuesta del cliente: PENDIENTE.
- ¿Qué pasa si el jefe cierra un relevamiento mientras un agente todavía tiene cambios locales sin sincronizar? → Respuesta del cliente: PENDIENTE.

## §8 Métricas de éxito desde el negocio

Métricas de negocio definidas en la consolidación de Parte A. Los targets son objetivos iniciales del proyecto de investigación, revisables al confirmar la línea de base operativa.

| Criterio | Métrica | Target | Plazo |
|---|---|---|---|
| Calidad de georreferenciación | % de observaciones con coordenada geográfica válida asociada | ≥ 95 % | 3 meses post-despliegue |
| Disponibilidad de datos para revisión | Tiempo entre fin de recolección en campo y datos sincronizados tras recuperar conexión | ≤ 24 h | por relevamiento |
| Eficiencia del cierre | Reducción del tiempo de confección del informe de cierre respecto del método manual actual | ≥ 30 % | 6 meses post-despliegue |

## §9 Lo que NO es esta solución (exclusiones)

- Auto-registro o flujo de solicitud/aceptación self-service de agentes de campo: queda fuera de v1 porque las altas y bajas de agentes las realiza directamente el jefe de área.
- Análisis automático de imágenes y detección de fallas por visión/IA: queda fuera porque la evaluación del estado del tramo la hace el jefe de área manualmente; GeoVial documenta, no diagnostica.
- Ruteo y navegación asistida: queda fuera porque el mapa sirve para ubicar y revisar marcadores, no para guiar el desplazamiento del agente en terreno.

## §10 Restricciones del cliente

- Fecha objetivo y qué la motiva: sin fecha. Es un proyecto de investigación educativo, no atado a un hito externo; la cadencia la fija el avance del equipo.
- Presupuesto orientativo o rango: sin presupuesto formal asignado. Proyecto de investigación educativo con fines comerciales futuros.
- Restricciones legales o regulatorias: sin exigencias regulatorias declaradas.
- Integraciones obligatorias con sistemas existentes: ninguna.

(Las restricciones técnicas y de infraestructura presentes en el material —monolito, stack .NET, contenedores, almacenamiento configurable— se registran como decisiones técnicas en §14 y §17, no como restricciones del cliente.)

## §11 Riesgos detectados desde el negocio

Riesgos de negocio definidos en la consolidación de Parte A.

| Riesgo | Probabilidad | Impacto | Mitigación |
|---|---|---|---|
| Baja adopción en campo por curva de aprendizaje o desconfianza de los agentes | Media | Alto | UX simple, capacitación y piloto acotado antes del despliegue masivo |
| Georreferenciación imprecisa (GPS pobre o fotos sin EXIF) que genera marcadores mal ubicados | Media | Medio | Radio de agrupación configurable, edición manual del pin y validación del jefe al cierre |
| Pérdida o duplicación de datos en la sincronización offline (cortes, conflictos) | Media | Alto | Cola local persistente, sincronización idempotente subir-luego-bajar, convivencia con conflictos y resolución al cierre |

## §12 Glosario del dominio del cliente

- Relevamiento: tarea que registra una serie de observaciones del estado de un tramo vial; tiene un ciclo de recolección, revisión y cierre.
- Tramo vial: extensión a relevar que puede abarcar uno o varios puentes y caminos; es el alcance de un relevamiento.
- Observación: registro anclado a un marcador geográfico, compuesto por notas, comentarios y fotos (con comentario y etiqueta por foto).
- Marcador geográfico: punto en el mapa que agrupa observaciones, fotos, comentarios y textos; es etiquetable y puede ser compartido por varias observaciones.
- Conflicto de marcadores: situación en la que dos o más marcadores caen dentro de un mismo radio; convive con la operación y solo afecta la estructura de catalogación, resolviéndose al cierre.
- Agente de campo (relevador): persona que toma relevamientos asignados e ingresa y administra sus observaciones, fotos, comentarios y etiquetas en terreno.
- Jefe de área: usuario que administra agentes de campo, administra y asigna relevamientos, y los revisa para su cierre.
- Jefe general: usuario que administra a los jefes de área.
- Usuario raíz: usuario con acceso pleno que administra y configura todo el sistema y da de alta al jefe general.
- Etiqueta: marca aplicable a fotos y a marcadores para filtrado posterior.
- Sincronización: proceso que sube primero los cambios locales del agente y luego baja las últimas actualizaciones de sus relevamientos asignados.
- Radio de agrupación: parámetro que, en carga manual, agrupa fotos dentro de un mismo marcador según su georreferenciación.

---

# Parte B — Composición de la solución

## §13 Proyectos de la solución

Valores cerrados D8, exactamente 8:

```text
library, web-monolith, web-microservices, desktop-app, mobile-app-maui, rest-api, cli-tool, worker-service
```

Tabla de proyectos (fuente del manifiesto derivado):

| `nombre-proyecto-kebab` | `project_type` (D8) | Rol en la solución | Dependencias | `redistribuible` |
|---|---|---|---|---|
| `geovial-api` (principal) | `rest-api` | Backend monolítico que expone la API REST consumida por el front web y la app móvil; concentra la lógica, la persistencia y la seguridad | `geovial-storage` | false |
| `geovial-web` | `web-monolith` | Front web de creación, recolección y revisión de relevamientos sobre mapa | `geovial-api` | false |
| `geovial-mobile` | `mobile-app-maui` | App de captura de observaciones en terreno, offline-first, con sincronización | `geovial-api`, `aplicada-sync` | false |
| `geovial-storage` | `library` | Soporte de alojamiento de archivos (fotos) transparente al sistema, con backend configurable (local / S3 / otro) por el usuario raíz; se integra al backend, no se publica como NuGet | — | false |
| `aplicada-sync` | `library` | Soporte de sincronización enfocado a aplicaciones móviles, integrable a .NET MAUI vía NuGet (repositorio en GitHub), reutilizable en otros proyectos | — | true |

Proyecto principal: `geovial-api` (ver §PENDIENTES: confirmar si la cabeza de la solución es el backend o una de las apps cliente).

Perfil de convención de nombres de código:

| Parámetro | Valor por defecto | Notas |
|---|---|---|
| Forma del nombre de solución en código | PascalCase | `NombreSolucionCodigo` = `GeoVial` |
| Separador de segmentos | `.` | Separa la raíz de la solución del sufijo de rol |
| Prefijo de paquetes redistribuibles | `Aplicada` | Reemplaza la raíz cuando `redistribuible: true` |

Nombres de código derivados (cada proyecto `GeoVial.<Sufijo>`; redistribuibles con prefijo `Aplicada.<X>`):

| `nombre-proyecto-kebab` | `nombre-proyecto-codigo` |
|---|---|
| `geovial-api` | `GeoVial.WebApi` |
| `geovial-web` | `GeoVial.Web` |
| `geovial-mobile` | `GeoVial.Mobile` |
| `geovial-storage` | `GeoVial.Storage` |
| `aplicada-sync` | `Aplicada.Sync` |

Verificación del grafo: las aristas de dependencia son `geovial-api → geovial-storage`, `geovial-web → geovial-api`, `geovial-mobile → geovial-api`, `geovial-mobile → aplicada-sync`. No hay ciclos (DAG válido), hay exactamente un proyecto principal y no hay colisión de `nombre-proyecto-kebab` ni de `nombre-proyecto-codigo`.

Orden topológico:

```text
nivel 0: aplicada-sync, geovial-storage
nivel 1: geovial-api
nivel 2: geovial-web, geovial-mobile   (paralelizables)
```

## §14 Estilo arquitectónico de la solución

La solución se organiza alrededor de un backend monolítico (`geovial-api`) que expone una API REST y concentra la lógica de negocio, la persistencia en SQL Server y la seguridad. Es la espina sobre la que se apoyan los dos clientes: el front web (`geovial-web`, Blazor Interactive Server) y la app móvil (`geovial-mobile`, .NET MAUI), que consumen su contrato REST autenticado con JWT bearer (flujo ROPC).

El modelo de dominio admite conflictos de marcadores como estado válido durante la recolección: el sistema convive con ellos, mantiene la información accesible y difiere la resolución al cierre del relevamiento, donde el jefe de área decide la unificación o separación.

Dos librerías completan la composición. `geovial-storage` se integra al backend y le expone una abstracción de alojamiento de archivos transparente, con proveedores configurables (local, Amazon S3 u otro) que el usuario raíz selecciona; no se publica como paquete. `aplicada-sync` es redistribuible: expone un contrato de sincronización enfocado a clientes MAUI (subir cambios locales y luego bajar actualizaciones) y se publica como NuGet desde GitHub para poder reutilizarse fuera de la solución; la consume `geovial-mobile`.

Contratos inter-proyecto, coherentes con las aristas de §13:
- `geovial-api` expone su contrato REST a `geovial-web` y `geovial-mobile`.
- `geovial-storage` expone su abstracción de almacenamiento a `geovial-api`.
- `aplicada-sync` expone su contrato de sincronización a `geovial-mobile`.

Por qué esta descomposición y no otra (un monolito único o más microservicios): el material fija explícitamente un backend monolítico con API, clientes web y móvil separados, una librería de sincronización reutilizable y una librería de almacenamiento transparente; esa decisión descarta tanto fusionar el front en el backend como partir el backend en microservicios. Justificación detallada contra alternativas: ver §17 P.2 de cada proyecto.

## §15 Esquema de descomposición y delivery

Estrategia: vertical slicing con walking skeleton, alineada al flujo Scrum de vertical slicing pedido, de modo que cada fase finalizada sea funcional respecto de la anterior y se pueda probar automáticamente y luego revisar.

El primer incremento entrega valor demostrable end-to-end a través de la jerarquía: autenticación y manejo de usuarios por jerarquía (raíz → jefe general → jefe de área → agente) atravesando `geovial-storage` (cuando aplica) → `geovial-api` → `geovial-web`, con base SQL Server, de modo que se pueda ejercitar el alta/baja según jerarquía y los logueos de punta a punta. Los incrementos siguientes agregan profundidad manteniendo el camino end-to-end: alta/asignación de relevamientos y marcadores; luego captura y visualización en `geovial-mobile` con `aplicada-sync`; luego revisión, resolución de conflictos al cierre, import/export y configuración de almacenamiento. Dado que los conflictos conviven con la operación y se resuelven al cierre, la recolección puede entregarse antes que la resolución de conflictos sin romper el camino end-to-end.

El orden de construcción respeta el orden topológico de §13: primero `aplicada-sync` y `geovial-storage` (nivel 0), luego `geovial-api` (nivel 1) y por último `geovial-web` y `geovial-mobile` (nivel 2, paralelizables). Las fases tentativas del material (scaffolding; backend/front/DB con jerarquía y pruebas unitarias; jerarquía en móvil; alta/visualización de relevamientos y marcadores con asignación; recolección por el agente; resto de los requerimientos) se mapean a este esquema.

## §16 Estructura de repositorio de la solución

Árbol derivado de la jerarquía de §13 y de la convención de nombres. La infraestructura pedida (un contenedor para el front, uno para el backend y uno para la base, con el almacenamiento local de archivos sobre el contenedor del backend) y los scripts `.bat` (`build-*.bat`, `publish-*.bat`) y de creación de imágenes quedan reflejados en `/build` y `/deploy`.

```text
geovial/
├── src/
│   ├── GeoVial.WebApi/          # rest-api (principal)
│   ├── GeoVial.Web/             # web-monolith (Blazor Interactive Server + MudBlazor)
│   ├── GeoVial.Mobile/          # mobile-app-maui (Blazor Hybrid + MudBlazor + SQLite)
│   ├── GeoVial.Storage/         # library de almacenamiento (no NuGet)
│   └── Aplicada.Sync/           # library redistribuible (NuGet, repo en GitHub)
├── tests/
│   ├── GeoVial.WebApi.Tests/
│   ├── GeoVial.Web.Tests/
│   ├── GeoVial.Mobile.Tests/
│   ├── GeoVial.Storage.Tests/
│   └── Aplicada.Sync.Tests/
├── samples/                     # ver §16.1
├── docs/                        # categorías 00-11 SDD (por proyecto bajo proyectos/<kebab>/)
├── build/                       # build-*.bat, publish-*.bat
├── deploy/                      # scripts de imágenes de contenedor (front, backend, base)
└── devs/intake/                 # SOLUTION-INTAKE
```

### §16.1 Materialización de `/samples`

Los samples se materializan según el tipo D8 de cada proyecto que los produce. Cada sample es autocontenido, ejecutable y declara su nivel.

- `aplicada-sync` (`library`, redistribuible): tres samples (`01-basico`, `02-intermedio`, `03-avanzado`). Aquí vive, como nivel avanzado/integración, el ejemplo de demostración MAUI ajeno al sistema pedido para evaluar la librería con vistas a reutilizarla en otros proyectos.
- `geovial-mobile` (`mobile-app-maui`): samples de app básica y sync offline. El sample multiplataforma se omite porque el único target es Android (ver §17 P.9).
- `geovial-api` (`rest-api`): cliente HTTP de referencia, colección de pruebas y SDK tipado. Detalle/cantidad: PENDIENTE.
- `geovial-web` (`web-monolith`): datos seed y, si hay punto de extensión visual, tema custom. Detalle: PENDIENTE.
- `geovial-storage` (`library`): consumidores progresivos del proveedor de almacenamiento. Detalle: PENDIENTE.

```text
samples/
├── aplicada-sync/
│   ├── 01-basico/
│   ├── 02-intermedio/
│   └── 03-avanzado-demo-maui/      # demo MAUI ajeno al sistema (evaluación de la librería)
├── geovial-mobile/
│   ├── 01-app-basica/
│   └── 02-sync-offline/
└── ...                             # resto PENDIENTE
```

---

# Parte C — Técnica por proyecto

## §17 Bloque técnico por proyecto

### Proyecto: geovial-api

| Campo | Valor |
|---|---|
| `nombre-proyecto-kebab` | `geovial-api` |
| `nombre-proyecto-codigo` | `GeoVial.WebApi` |
| `project_type` (D8) | `rest-api` |
| Rol | Backend monolítico que expone la API REST consumida por los clientes web y móvil |
| `redistribuible` | false |

#### §17.P.1 Stack tecnológico
.NET / C#, ASP.NET Core Web API. Dependencias core: acceso a datos sobre SQL Server e integración de la librería `GeoVial.Storage` para fotos. Versión mínima de .NET y runtime: PENDIENTE. Dependencias core exactas (ORM/driver, JWT): PENDIENTE.

#### §17.P.2 Estilo arquitectónico del proyecto
Monolito de backend (fijado por requisito). Alternativas descartadas: microservicios (descartado por el requisito de monolito) y servir la UI desde el mismo proceso (descartado: el front es un proyecto Blazor separado). Estilo interno (capas, hexagonal): PENDIENTE.

#### §17.P.3 Comunicación e integración
REST sobre HTTP, payloads JSON, autenticación JWT bearer con flujo ROPC. Expone a `geovial-web` y `geovial-mobile`; incluye los endpoints de sincronización (recibe primero los cambios locales y luego entrega actualizaciones). La sincronización admite estados en conflicto (marcadores dentro de un mismo radio) como válidos: no bloquea la integración y difiere la resolución al cierre del relevamiento. Versionado de contratos y política de breaking changes: PENDIENTE.

#### §17.P.4 Persistencia
SQL Server (fijado por requisito). El modelo persiste marcadores en conflicto sin forzar su unificación durante la recolección; la unificación/separación se aplica al cierre. Versionado del esquema (tooling de migraciones): PENDIENTE. Multi-tenant (por área u organización): PENDIENTE.

#### §17.P.5 Seguridad y autenticación
Autenticación y autorización por flujo ROPC con bearer token JWT (fijado por requisito). Roles: raíz, jefe general, jefe de área y agente de campo, con el alcance jerárquico de §2. Ubicación del Identity Provider (el propio backend u otro), manejo de secretos y compliance: PENDIENTE.

#### §17.P.6 Estrategia de testing
Se prevén pruebas unitarias del manejo de usuarios y la administración por jerarquía. Cobertura mínima numérica de líneas y branches (gate de CI): PENDIENTE. Frameworks por nivel y tests de contrato: PENDIENTE.

#### §17.P.7 Estrategia de versionado y release
Se adopta SemVer 2.0.0 y Conventional Commits. Herramienta de cálculo de versión, branching, canales y feed: PENDIENTE.

#### §17.P.8 Pipeline CI/CD
Existen scripts `.bat` (`build-*.bat`, `publish-*.bat`) y scripts de creación de imágenes de contenedor. Plataforma de CI, stages y quality gates bloqueantes para mergear, y procedimiento de rollback: PENDIENTE.

#### §17.P.9 Compatibilidad y plataformas target
Se ejecuta en un contenedor de backend dedicado. Versión de runtime/SO base del contenedor y versiones mínimas: PENDIENTE.

#### §17.P.10 Requerimientos no funcionales (NFR)
Métricas numéricas de performance, escalabilidad, disponibilidad y observabilidad: PENDIENTE.

#### §17.P.11 Decisiones técnicas pre-tomadas (pre-ADR)
Backend monolítico; API REST; persistencia SQL Server; autenticación ROPC + JWT; almacenamiento de archivos vía librería transparente; conflictos de marcadores tolerados y resueltos al cierre. Alternativas evaluadas en cada decisión y decisiones que quedan abiertas para Sprint 0: PENDIENTE.

#### §17.P.12 Restricciones técnicas y trade-offs aceptados
Se prioriza la simplicidad operativa de un monolito sobre el escalado independiente por servicio. Otros trade-offs y cargas no soportadas: PENDIENTE.

---

### Proyecto: geovial-web

| Campo | Valor |
|---|---|
| `nombre-proyecto-kebab` | `geovial-web` |
| `nombre-proyecto-codigo` | `GeoVial.Web` |
| `project_type` (D8) | `web-monolith` |
| Rol | Front web de creación, recolección y revisión de relevamientos sobre mapa |
| `redistribuible` | false |

#### §17.P.1 Stack tecnológico
.NET / C#, Blazor Interactive Server con la librería MudBlazor; mapas con OpenStreetMap y Leaflet. Versión mínima de .NET: PENDIENTE.

#### §17.P.2 Estilo arquitectónico del proyecto
Blazor Interactive Server (render server-side con circuito SignalR), fijado por requisito. Alternativas descartadas: Blazor WebAssembly y un front MVC/SPA tradicional (descartados por el requisito de Interactive Server + MudBlazor). Detalle interno: PENDIENTE.

#### §17.P.3 Comunicación e integración
Consume el contrato REST de `geovial-api` sobre HTTP con JWT bearer. Incluye la pantalla de resolución de conflictos de marcadores que se ejercita al cierre del relevamiento. Manejo de sesión sobre el circuito interactivo: PENDIENTE.

#### §17.P.4 Persistencia
No aplica: el estado de dominio se persiste vía la API; el front no tiene almacenamiento propio (a confirmar respecto de estado de UI/caché). PENDIENTE de confirmación.

#### §17.P.5 Seguridad y autenticación
Obtiene el JWT del backend por ROPC y lo usa para las llamadas a la API. Manejo de token en el circuito server-side y secretos: PENDIENTE.

#### §17.P.6 Estrategia de testing
Cobertura mínima numérica y frameworks: PENDIENTE.

#### §17.P.7 Estrategia de versionado y release
Se adopta SemVer 2.0.0 y Conventional Commits. Herramienta, branching, canales y feed: PENDIENTE.

#### §17.P.8 Pipeline CI/CD
Scripts `.bat` y de imagen de contenedor del front. Plataforma de CI, stages, quality gates y rollback: PENDIENTE.

#### §17.P.9 Compatibilidad y plataformas target
Se ejecuta en un contenedor de front dedicado. Navegadores soportados y versiones mínimas, y versión de runtime base: PENDIENTE.

#### §17.P.10 Requerimientos no funcionales (NFR)
Métricas numéricas (latencia de interacción, concurrencia de circuitos, disponibilidad): PENDIENTE.

#### §17.P.11 Decisiones técnicas pre-tomadas (pre-ADR)
Blazor Interactive Server; MudBlazor; OpenStreetMap + Leaflet para mapas. Alternativas evaluadas y decisiones abiertas: PENDIENTE.

#### §17.P.12 Restricciones técnicas y trade-offs aceptados
Server-side Blazor exige conexión persistente con el servidor (circuito) a cambio de un modelo de desarrollo unificado en C#. Otros trade-offs: PENDIENTE.

---

### Proyecto: geovial-mobile

| Campo | Valor |
|---|---|
| `nombre-proyecto-kebab` | `geovial-mobile` |
| `nombre-proyecto-codigo` | `GeoVial.Mobile` |
| `project_type` (D8) | `mobile-app-maui` |
| Rol | App de captura de observaciones en terreno, offline-first, con sincronización |
| `redistribuible` | false |

#### §17.P.1 Stack tecnológico
.NET MAUI con páginas Blazor integradas (Blazor Hybrid) y MudBlazor; SQLite para la persistencia interna; mapas con OpenStreetMap y Leaflet; consume `Aplicada.Sync`. Versiones mínimas de .NET/MAUI: PENDIENTE.

#### §17.P.2 Estilo arquitectónico del proyecto
MAUI Blazor Hybrid offline-first. Alternativas descartadas: app nativa no-MAUI y front WebAssembly servido (descartados por el requisito de MAUI con Blazor + MudBlazor). Detalle interno: PENDIENTE.

#### §17.P.3 Comunicación e integración
Consume el contrato REST de `geovial-api` (JWT) y sincroniza a través de `Aplicada.Sync`: detecta conexión automáticamente, sube primero los cambios locales y luego baja las actualizaciones de los relevamientos asignados. Los marcadores en conflicto se suben y conviven; su resolución se difiere al cierre desde la web. Versionado del contrato de sincronización: PENDIENTE.

#### §17.P.4 Persistencia
SQLite local (fijado por requisito) para trabajar sin conexión. Versionado del esquema local y metadatos de sincronización: PENDIENTE.

#### §17.P.5 Seguridad y autenticación
La primera vez, el usuario inicia sesión online ingresando sus credenciales (ROPC + JWT). Existe un deslogueo completo que libera el dispositivo para que otro usuario lo use con su propia cuenta. Durante una sesión activa, si el dispositivo se bloqueó o la aplicación se reinició, la app pide reloguearse mediante la seguridad del propio dispositivo (por ejemplo, patrón o huella digital), sin volver a pedir credenciales. Almacenamiento seguro del token en el dispositivo: PENDIENTE.

#### §17.P.6 Estrategia de testing
Cobertura mínima numérica y frameworks por nivel: PENDIENTE.

#### §17.P.7 Estrategia de versionado y release
Se adopta SemVer 2.0.0 y Conventional Commits. Herramienta, canales (incluida distribución del APK) y feed: PENDIENTE.

#### §17.P.8 Pipeline CI/CD
La depuración y prueba se hace sobre un dispositivo Android conectado por USB en modo desarrollador. Plataforma de CI, build de la app, quality gates y firma/distribución: PENDIENTE.

#### §17.P.9 Compatibilidad y plataformas target
Android únicamente; no se soportan iOS ni Windows en v1 (decisión confirmada por el cliente). Depuración sobre dispositivo Android conectado por USB en modo desarrollador. Versión mínima de Android: PENDIENTE (se confirma al resolver la Parte C de geovial-mobile).

#### §17.P.10 Requerimientos no funcionales (NFR)
Métricas numéricas de captura/sincronización offline (tamaño de cola tolerado, tiempos de sync, consumo): PENDIENTE.

#### §17.P.11 Decisiones técnicas pre-tomadas (pre-ADR)
MAUI Blazor Hybrid; SQLite local; modelo offline-first; login online inicial con credenciales, deslogueo completo para cambio de usuario y relogueo en sesión activa por seguridad del dispositivo. Alternativas evaluadas y decisiones abiertas: PENDIENTE.

#### §17.P.12 Restricciones técnicas y trade-offs aceptados
Se acepta la complejidad del modelo offline-first (sincronización y convivencia con conflictos) para habilitar el trabajo de campo sin conectividad. Otros trade-offs y plataformas no soportadas: PENDIENTE.

---

### Proyecto: geovial-storage

| Campo | Valor |
|---|---|
| `nombre-proyecto-kebab` | `geovial-storage` |
| `nombre-proyecto-codigo` | `GeoVial.Storage` |
| `project_type` (D8) | `library` |
| Rol | Soporte de alojamiento de archivos transparente al sistema, con backend configurable por el usuario raíz |
| `redistribuible` | false |

#### §17.P.1 Stack tecnológico
.NET / C# como librería de clases. Dependencias core según proveedor (por ejemplo, SDK de Amazon S3 para el proveedor S3). Versión mínima de .NET y conjunto exacto de proveedores más allá de local y S3: PENDIENTE.

#### §17.P.2 Estilo arquitectónico del proyecto
Abstracción con proveedores intercambiables (estrategia/provider) para que el backend almacene archivos de forma transparente. Alternativas descartadas: acoplar el acceso a disco directamente en el backend; integrar un único proveedor fijo. Justificación detallada: PENDIENTE.

#### §17.P.3 Comunicación e integración
Expone una abstracción de almacenamiento (interfaz) a `geovial-api`. No tiene contrato de red propio más allá de los SDK de cada proveedor. Política de breaking changes: PENDIENTE.

#### §17.P.4 Persistencia
Archivos binarios (fotos): en el contenedor del backend para el proveedor local, o en Amazon S3 u otro según configuración. Detalle de organización/versionado de blobs: PENDIENTE.

#### §17.P.5 Seguridad y autenticación
Manejo de credenciales de los proveedores (por ejemplo, claves S3) y su almacenamiento seguro: PENDIENTE.

#### §17.P.6 Estrategia de testing
Cobertura mínima numérica (gate de CI), propuesto por el orquestador (ratificable): líneas >= 80 %, branches >= 70 %. Frameworks: pruebas unitarias del enrutado por proveedor y pruebas de integración por proveedor con dobles o contenedores efímeros.

#### §17.P.7 Estrategia de versionado y release
Se adopta SemVer 2.0.0 y Conventional Commits. No se publica como NuGet (se integra al backend). Herramienta de versionado (propuesto, ratificable): versión derivada del tag con GitVersion, alineada al ciclo del backend.

#### §17.P.8 Pipeline CI/CD
Se construye junto al backend mediante los scripts `.bat`. Quality gates bloqueantes (propuesto, ratificable): compilación sin warnings tratados como error, pruebas unitarias y de integración en verde, gate de cobertura (>= 80 % líneas / >= 70 % branches) y análisis estático sin issues críticos. Rollback: por reversión de la imagen del backend que la integra.

#### §17.P.9 Compatibilidad y plataformas target
Runtime .NET del backend. Versión (propuesto, ratificable): .NET 8 (LTS) sobre contenedor Linux, alineado al runtime de geovial-api.

#### §17.P.10 Requerimientos no funcionales (NFR)
Métricas (propuesto, ratificable): latencia de subida/descarga p95 <= 1 s para archivos de hasta 5 MB con el proveedor local; tamaño máximo de archivo configurable, por defecto 25 MB; sin degradación apreciable al cambiar de proveedor (transparencia).

#### §17.P.11 Decisiones técnicas pre-tomadas (pre-ADR)
Proveedores configurables (local / S3 / otro) seleccionables por el usuario raíz; transparencia hacia el resto del sistema; no se distribuye como NuGet. Alternativas evaluadas: PENDIENTE.

#### §17.P.12 Restricciones técnicas y trade-offs aceptados
Se acepta el costo de una capa de abstracción para independizar al sistema del proveedor de almacenamiento. Otros trade-offs: PENDIENTE.

---

### Proyecto: aplicada-sync

| Campo | Valor |
|---|---|
| `nombre-proyecto-kebab` | `aplicada-sync` |
| `nombre-proyecto-codigo` | `Aplicada.Sync` |
| `project_type` (D8) | `library` |
| Rol | Soporte de sincronización para apps móviles, integrable a .NET MAUI vía NuGet y reutilizable fuera de la solución |
| `redistribuible` | true |

#### §17.P.1 Stack tecnológico
.NET / C# como librería integrable a .NET MAUI. Trabaja con el almacén local del cliente (SQLite). Target frameworks compatibles con MAUI y dependencias core: PENDIENTE.

#### §17.P.2 Estilo arquitectónico del proyecto
Motor de sincronización con la política "subir cambios locales primero, luego bajar actualizaciones". Alternativas descartadas: sincronización ad-hoc embebida en la app; biblioteca acoplada al dominio de GeoVial (descartada por el requisito de reutilización). Justificación detallada: PENDIENTE.

#### §17.P.3 Comunicación e integración
Define un contrato de sincronización que la app móvil consume contra los endpoints de sincronización del backend. Transporte, formato y versionado del contrato: PENDIENTE.

#### §17.P.4 Persistencia
Gestiona metadatos de sincronización sobre el almacén local (SQLite) del host. Esquema de metadatos: PENDIENTE.

#### §17.P.5 Seguridad y autenticación
Reutiliza los tokens de autenticación de la app host (JWT). Detalle: PENDIENTE.

#### §17.P.6 Estrategia de testing
Cobertura mínima numérica (gate de CI), propuesto por el orquestador (ratificable): líneas >= 80 %, branches >= 70 %. Frameworks: pruebas unitarias del motor y pruebas de contrato de sincronización (orden subir-antes-de-bajar, idempotencia y reanudación).

#### §17.P.7 Estrategia de versionado y release
Se adopta SemVer 2.0.0 y Conventional Commits. Se publica como paquete NuGet redistribuible con repositorio en GitHub. Feed (propuesto, ratificable): GitHub Packages como feed inicial; herramienta de versionado: versión derivada del tag con GitVersion.

#### §17.P.8 Pipeline CI/CD
CI que construye y publica el paquete NuGet desde GitHub. Stages y quality gates bloqueantes (propuesto, ratificable): compilación, pruebas unitarias y de contrato en verde, gate de cobertura (>= 80 % líneas / >= 70 % branches), análisis estático sin issues críticos, empaquetado y publicación al feed, y verificación post-publish (restauración del paquete publicado en un proyecto limpio). Rollback: unlist de la versión afectada.

#### §17.P.9 Compatibilidad y plataformas target
Android únicamente, en línea con geovial-mobile (sin iOS ni Windows en v1). Target framework (propuesto, ratificable): net8.0-android sobre .NET 8 (LTS); la API mínima de Android se alinea con la del proyecto geovial-mobile.

#### §17.P.10 Requerimientos no funcionales (NFR)
Métricas (propuesto, ratificable): sincronización de un lote de 100 cambios <= 30 s en red móvil típica; tolera una cola local de >= 1000 cambios pendientes; reanuda sin pérdida tras un corte de conexión; idempotente ante reintentos.

#### §17.P.11 Decisiones técnicas pre-tomadas (pre-ADR)
Paquete NuGet redistribuible; repositorio en GitHub; integrable a .NET MAUI; demo MAUI ajeno al sistema para evaluación. Alternativas evaluadas: PENDIENTE.

#### §17.P.12 Restricciones técnicas y trade-offs aceptados
Se prioriza la generalidad y reutilización por sobre el acoplamiento al dominio de GeoVial. Otros trade-offs y cargas no soportadas: PENDIENTE.

---

## §18 Estrategia de demo / samples

El sample distintivo y explícitamente pedido es un ejemplo de demostración MAUI ajeno al sistema que consume `Aplicada.Sync`, para evaluar la librería con vistas a reutilizarla en otros proyectos; se ubica en `/samples/aplicada-sync/` como nivel avanzado/integración y demuestra el punto de extensión principal (el motor de sincronización reutilizable). Cada sample se vincula a `/src` consumiendo el proyecto correspondiente y debe ser reproducible en cinco pasos o menos.

Cantidad total de samples y enumeración completa por proyecto (más allá del demo MAUI de `aplicada-sync` y los samples de captura/sync de `geovial-mobile`): PENDIENTE. Vínculo exacto de cada sample con el código productivo: PENDIENTE.

---

## §19 Checklist de completitud del intake

Negocio (Parte A):
- [x] La cabecera tiene nombre de solución, cliente, fecha y estado. (lead técnico opcional sin completar; el resto presente)
- [x] §1 describe un problema concreto y qué pasa si no se construye.
- [x] §2 tiene al menos un stakeholder por categoría con rol explícito.
- [x] §4 tiene al menos un ítem en cada categoría MoSCoW y el Must Have es el mínimo razonable.
- [x] §5 tiene al menos 3 historias en formato `Como/quiero/para`, cubriendo 2 roles si hay más de uno.
- [x] §7 lista al menos 5 casos límite con espacio para respuesta del cliente.
- [x] §8 tiene al menos 3 métricas SMART de negocio con target y plazo numéricos.
- [x] §9 lista al menos 3 exclusiones con justificación.
- [x] §10 declara presupuesto orientativo y fecha objetivo (o "sin fecha" justificado).
- [x] §11 lista al menos 3 riesgos con probabilidad, impacto y mitigación.
- [x] §12 define al menos 5 términos del dominio.

Composición (Parte B):
- [x] §13 enumera todos los proyectos, cada uno con uno de los 8 valores D8, señala el principal, y el grafo de dependencias es acíclico.
- [x] §13 declara el perfil de convención de nombres; no hay colisión de nombres de proyecto.
- [x] §14 describe la composición y los contratos entre proyectos.
- [x] §15 garantiza valor demostrable end-to-end en el primer sprint a través de la jerarquía.
- [x] §16 publica el árbol `tree` derivado de la jerarquía y de la convención de nombres, con §16.1.

Técnica por proyecto (Parte C):
- [x] §17 está completo para cada proyecto de §13 (identidad + P.1 a P.12). (estructura completa; varios campos PENDIENTE)
- [ ] Cada proyecto: P.6 cobertura mínima numérica; P.7 SemVer y Conventional Commits; P.8 quality gates bloqueantes; P.9 plataformas y versiones mínimas; P.10 NFR numéricos. (P.7 OK; P.6, P.8, P.9, P.10 PENDIENTE)

General:
- [x] No hay vocabulario del dominio fuente del bootstrap ni stacks hardcodeados en el texto normativo (D7).
- [x] El control de cambios refleja la versión y fecha del documento.

---

## Control de cambios

| Versión | Fecha | Cambios | Autor |
|---|---|---|---|
| 1.0 | 2026-06-15 | Intake unificado inicial de la solución GeoVial, derivado de la conversación de descubrimiento, con la primera ronda de respuestas incorporada (jerarquía de usuarios, alcance del relevamiento por tramo vial, alta directa de agentes, convivencia y resolución de conflictos al cierre, relogueo móvil). | Analista de intake SDD 2.1 |
| 1.1 | 2026-06-15 | Consolidación de Parte A en la Fase de validación de intake (master-prompt §3, flujo §13), tras confirmación del usuario: §1 consecuencia y disparador; §2 stakeholders Propietario (Vialidad provincial) e Implementador (Departamento de desarrollo de software, 1 dev → equipo_n=1); §8 tres métricas SMART; §9 dos exclusiones adicionales; §10 presupuesto/fecha/regulatorio/integraciones; §11 tres riesgos de negocio. Snapshot previo en `_legacy/2026-06-15/`. Parte C (§17 P.6/P.8/P.9/P.10) se difiere y se resuelve por fase de cada proyecto. El nombre de archivo vigente se mantiene en `_v1.0.md` por ser la ruta de insumo que referencia el orquestador. | Orquestador SDD 2.1 |
| 1.2 | 2026-06-15 | Adelanto del eje plataformas de §17 P.9 (resto de Parte C sigue diferido): el único target móvil es Android, sin iOS ni Windows en v1, para geovial-mobile y aplicada-sync. §16.1 elimina el sample multiplataforma de geovial-mobile. Habilita generar `compatibilidad-plataformas_v1.0.md` en Fase A acotado a Android, contenedor de backend/front y navegadores. Snapshot v1.1 en `_legacy/2026-06-15/`. | Orquestador SDD 2.1 |
| 1.3 | 2026-06-15 | Resolución de la Parte C diferida de los proyectos de nivel 0 (`geovial-storage`, `aplicada-sync`) con valores por defecto propuestos por el orquestador y marcados como ratificables, ante la indicación del usuario de avanzar sin detenerse: §17 P.6 (cobertura >= 80 % líneas / >= 70 % branches), P.7 (feed/herramienta de versión), P.8 (quality gates bloqueantes y rollback), P.9 (.NET 8 LTS; net8.0-android para aplicada-sync) y P.10 (NFR numéricos) de ambas librerías. La Parte C de geovial-api, geovial-web y geovial-mobile sigue diferida y se resolverá al entrar a cada proyecto. Snapshot v1.2 en `_legacy/2026-06-15/`. | Orquestador SDD 2.1 |
