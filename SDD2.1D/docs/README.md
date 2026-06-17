# GeoVial

| Campo | Valor |
| --- | --- |
| Solución | GeoVial |
| Versión del documento | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-06-15 |
| Stack principal | .NET 8 (LTS) / C#; backend ASP.NET Core sobre SQL Server; front Blazor Interactive Server + MudBlazor; móvil .NET MAUI Blazor Hybrid + MudBlazor + SQLite (Android); mapas OpenStreetMap + Leaflet |
| Composición | 5 proyectos (ver tabla de proyectos) |
| Proyecto principal | geovial-api |
| Documento | README raíz de la solución |

---

## 1. Identidad de la solución

GeoVial es una solución para el relevamiento fotográfico georreferenciado de tramos viales (puentes y caminos). Un relevamiento corresponde a un tramo vial que puede abarcar varios puentes y caminos, y consiste en registrar una serie de observaciones del estado de ese tramo: notas, comentarios y fotografías ancladas a un punto geográfico. El jefe de área crea el relevamiento, lo asigna a agentes de campo, la cuadrilla recolecta las observaciones en terreno y luego el jefe revisa y evalúa la información para confeccionar sus informes rutinarios y cerrar el relevamiento.

El trabajo de campo ocurre en lugares donde no se puede asumir conectividad. Por eso la captura de observaciones es offline-first: se hace sin internet y se sincroniza después, subiendo primero los cambios locales y luego bajando las actualizaciones del relevamiento asignado. La solución estructura la recolección alrededor de marcadores geográficos que agrupan fotos, comentarios y etiquetas, tolera conflictos de marcadores durante la recolección sin bloquear el acceso a la información, y ofrece una revisión visual sobre mapa que difiere la resolución de conflictos al cierre.

La propuesta de valor es capturar evidencia georreferenciada en el lugar del tramo vial, aun sin conexión, y revisarla después sobre un mapa para producir informes trazables y reproducibles. La audiencia objetivo está formada por la organización propietaria (Vialidad provincial) y cuatro roles del sistema en jerarquía de mayor a menor alcance: usuario raíz, jefe general, jefe de área y agente de campo.

Los diferenciadores derivados del alcance pretendido son: captura móvil offline-first con sincronización automática al detectar conexión; georreferenciación en el momento de la captura y, en carga manual, a partir de los metadatos de ubicación de la foto, con un radio configurable para agrupar fotos en un mismo marcador; tolerancia a conflictos de marcadores, que conviven con la operación y se resuelven al cierre; revisión visual sobre mapa con carrusel de fotos por marcador encadenando marcadores contiguos; y portabilidad del relevamiento completo (comentarios, etiquetas y fotos) en un único archivo. GeoVial documenta el estado del tramo; la evaluación y el diagnóstico los hace el jefe de área, no el sistema.

## 2. Proyectos de la solución

La tabla refleja el `SOLUTION-MANIFEST-geovial_v1.0.md` sin divergencias. El proyecto principal es `geovial-api`.

| Proyecto | Tipo D8 | Rol | Dependencias | Redistribuible |
| --- | --- | --- | --- | --- |
| geovial-api (principal) | rest-api | Backend monolítico que expone la API REST consumida por el front web y la app móvil; concentra lógica, persistencia y seguridad | geovial-storage | false |
| geovial-web | web-monolith | Front web de creación, recolección y revisión de relevamientos sobre mapa | geovial-api | false |
| geovial-mobile | mobile-app-maui | App de captura de observaciones en terreno, offline-first, con sincronización | geovial-api, aplicada-sync | false |
| geovial-storage | library | Alojamiento de archivos transparente al sistema, con backend configurable (local / S3 / otro) por el usuario raíz; se integra al backend, no se publica como NuGet | — | false |
| aplicada-sync | library | Sincronización para apps móviles, integrable a .NET MAUI vía NuGet (repo en GitHub), reutilizable fuera de la solución | — | true |

Grafo de dependencias acíclico (DAG), orden topológico de construcción: nivel 0 `aplicada-sync` y `geovial-storage`; nivel 1 `geovial-api`; nivel 2 `geovial-web` y `geovial-mobile` (paralelizables). La cadena de trazabilidad SDD recorre Visión → NB → Especificación → Arquitectura → Backlog → Sprint → Pruebas → Pipeline; cada eslabón vive en las categorías enlazadas en la sección 4.

Contratos entre proyectos, coherentes con las aristas del grafo: `geovial-api` expone su contrato REST a `geovial-web` y `geovial-mobile`; `geovial-storage` expone una abstracción de almacenamiento a `geovial-api`; `aplicada-sync` expone su contrato de sincronización a `geovial-mobile`. La descripción integral de esos contratos y la justificación de la descomposición (un monolito con clientes separados más dos librerías, frente a un monolito único o microservicios) viven en la vista de solución y en los ADR de `_solucion/`.

## 3. Stack y composición

El stack se declara como `tecnología @ versión` por proyecto. La plataforma de ejecución del backend, el front y las librerías de servidor es un contenedor Linux con runtime .NET 8 (LTS).

| Proyecto | Tipo D8 | Stack principal | Plataforma target |
| --- | --- | --- | --- |
| geovial-api | rest-api | .NET @ 8 (LTS), ASP.NET Core @ 8, SQL Server, JWT bearer (flujo ROPC) | Contenedor Linux, runtime .NET 8 |
| geovial-web | web-monolith | .NET @ 8 (LTS), Blazor Interactive Server @ 8, MudBlazor, OpenStreetMap + Leaflet | Contenedor Linux; navegadores evergreen de escritorio y móvil |
| geovial-mobile | mobile-app-maui | .NET MAUI @ 8 (Blazor Hybrid), MudBlazor, SQLite, OpenStreetMap + Leaflet, consume Aplicada.Sync | Android (net8.0-android), API mínima 26 (Android 8.0) |
| geovial-storage | library | .NET @ 8 (LTS), proveedores intercambiables (local / Amazon S3 / otro) | Runtime .NET 8 del backend (contenedor Linux) |
| aplicada-sync | library | .NET @ 8 (LTS), integrable a .NET MAUI, paquete NuGet (GitHub Packages) | Android (net8.0-android), alineado con geovial-mobile |

### 3.A Quick-start de consumo de la API (geovial-api)

El proyecto principal es un `rest-api`. El camino feliz de consumo es: obtener un token, listar los relevamientos asignados y leer sus marcadores. Los endpoints concretos, su versionado y el contrato OpenAPI viven en la documentación de `geovial-api` (categorías 02 y 05); este bloque solo ilustra la secuencia de validación de extremo a extremo.

```bash
# 1) Autenticación por flujo ROPC: credenciales -> token JWT bearer
curl -X POST https://<host>/api/v1/auth/token \
  -H "Content-Type: application/json" \
  -d '{"usuario":"<usuario>","clave":"<clave>"}'

# 2) Listar los relevamientos asignados (token del paso 1)
curl https://<host>/api/v1/relevamientos \
  -H "Authorization: Bearer <token>"

# 3) Leer los marcadores de un relevamiento
curl https://<host>/api/v1/relevamientos/<id>/marcadores \
  -H "Authorization: Bearer <token>"
```

La autenticación usa JWT bearer con flujo ROPC; el versionado de endpoints es por URI (prefijo de versión mayor en la ruta). La referencia completa de endpoints y esquemas es responsabilidad de la documentación del proyecto, no de este README.

### 3.B Cómo consumir aplicada-sync como dependencia

`aplicada-sync` es la única librería redistribuible (`Aplicada.Sync`). Se publica como paquete NuGet desde GitHub Packages y está pensada para integrarse a clientes .NET MAUI fuera de la solución. La instalación mínima y un ejemplo de uso del motor de sincronización viven en la guía de desarrollo y los ejemplos del proyecto; el contrato es subir primero los cambios locales y luego bajar las actualizaciones.

```bash
# Instalación desde el feed de paquetes (GitHub Packages)
dotnet add package Aplicada.Sync
```

El detalle de configuración, el contrato de sincronización y los tres samples progresivos (`01-basico`, `02-intermedio`, `03-avanzado-demo-maui`) se documentan en [proyectos/aplicada-sync](proyectos/aplicada-sync/).

### 3.C Compatibilidad de plataformas (geovial-mobile)

`geovial-mobile` es `mobile-app-maui` con un único target en v1. El detalle de versiones mínimas y plataformas fuera de v1 vive en `00_contexto/compatibilidad-plataformas_v1.0.md`.

| Plataforma | Versión mínima | Observaciones |
| --- | --- | --- |
| Android | API 26 (Android 8.0), net8.0-android | Único target de v1; distribución del paquete (APK/AAB) por canal interno, sin tienda pública |
| iOS | No soportado en v1 | Fuera de alcance por decisión del cliente |
| Windows | No soportado en v1 | Fuera de alcance por decisión del cliente |

## 4. Mapa de la documentación

Tabla A. Categorías de nivel solución, vista y pipeline de solución, y la carpeta de cada proyecto. Los enlaces son relativos a `/SDD2.1D/docs/`.

| Sección | Propósito | Responsable | Enlace |
| --- | --- | --- | --- |
| 00_contexto (solución) | Visión, alcance, roadmap y compatibilidad de plataformas | AG-00 | [00_contexto](00_contexto/) |
| 01_necesidades_negocio (solución) | Catálogo de necesidades de negocio (NB-01 a NB-07) | AG-01 | [01_necesidades_negocio](01_necesidades_negocio/) |
| _solucion — vista de solución | Vista integral de la composición y los contratos entre proyectos | AG-05 | [_solucion/vista-solucion_v1.0.md](_solucion/vista-solucion_v1.0.md) |
| _solucion — pipeline de solución | Pipeline de construcción y entrega de la solución | AG-09 | [_solucion/pipeline-solucion_v1.0.md](_solucion/pipeline-solucion_v1.0.md) |
| proyectos/geovial-api | Documentación por proyecto (categorías 02 a 11 según su tipo D8; 04 omitida en toda la solución por no usar LLM) del backend REST (principal) | AG-02 a AG-11 | [proyectos/geovial-api](proyectos/geovial-api/) |
| proyectos/geovial-web | Documentación por proyecto (categorías 02 a 11 según su tipo D8; 04 omitida en toda la solución por no usar LLM; 10 omitida en los clientes web y móvil por audiencia interna) del front web | AG-02 a AG-11 | [proyectos/geovial-web](proyectos/geovial-web/) |
| proyectos/geovial-mobile | Documentación por proyecto (categorías 02 a 11 según su tipo D8; 04 omitida en toda la solución por no usar LLM; 10 omitida en los clientes web y móvil por audiencia interna) de la app móvil | AG-02 a AG-11 | [proyectos/geovial-mobile](proyectos/geovial-mobile/) |
| proyectos/geovial-storage | Documentación por proyecto (categorías 02 a 11 según su tipo D8; 04 omitida en toda la solución por no usar LLM) de la librería de almacenamiento | AG-02 a AG-11 | [proyectos/geovial-storage](proyectos/geovial-storage/) |
| proyectos/aplicada-sync | Documentación por proyecto (categorías 02 a 11 según su tipo D8; 04 omitida en toda la solución por no usar LLM) de la librería de sincronización | AG-02 a AG-11 | [proyectos/aplicada-sync](proyectos/aplicada-sync/) |

Para entrar por la solución antes que por un proyecto, el recorrido recomendado es: leer `00_contexto` (visión, alcance, roadmap, compatibilidad), luego `01_necesidades_negocio` (las siete NB que ordenan el valor de negocio), después la vista y el pipeline de `_solucion/` (composición, contratos y entrega), y recién entonces abrir la carpeta del proyecto de interés. La tabla de proyectos de la sección 2 es el índice cruzado entre cada proyecto y su carpeta de documentación.

## 5. Flujo de lectura recomendado por audiencia

Tabla B. Cada rol tiene un orden de lectura sugerido y su justificación. Los números refieren a las categorías SDD dentro de cada proyecto; 00 y 01 son de nivel solución. El comodín `*` indica que el orden aplica al proyecto que la audiencia esté trabajando.

| Rol | Orden recomendado | Por qué |
| --- | --- | --- |
| Product Manager | 00 → 01 → proyectos/*/06 → proyectos/*/07 | Necesita la visión y las necesidades de negocio antes que el backlog y los planes de sprint. |
| Desarrollador | 00 → proyectos/*/02 → proyectos/*/05 → proyectos/*/10 → proyectos/*/11 | Necesita contexto, especificación funcional, arquitectura, guía de desarrollo y ejemplos para construir. |
| QA | 00 → proyectos/*/02 → proyectos/*/08 | Necesita los requisitos funcionales y la estrategia de calidad y pruebas para diseñar la verificación. |
| DevOps | 00 → _solucion/pipeline-solucion_v1.0.md → proyectos/*/09 | Necesita la vista de pipeline de solución y el DevOps de cada proyecto para construir y desplegar. |
| Arquitecto / Lead técnico | 00 → _solucion/vista-solucion_v1.0.md → _solucion/adrs → proyectos/*/05 | Necesita la composición integral, las decisiones de arquitectura de solución y la arquitectura técnica de cada proyecto. |

Las cuatro audiencias comparten el arranque por `00_contexto` porque allí se fija el porqué y el vocabulario; a partir de ahí divergen según el artefacto que cada rol necesita producir o validar.

## 6. Cómo contribuir y cómo regenerar la documentación

Esta documentación se genera con el flujo de subagentes SDD 2.1. El orquestador deriva el `SOLUTION-MANIFEST` desde el `SOLUTION-INTAKE` y luego invoca, en orden de categoría, a los subagentes AG-00 a AG-11 por proyecto y a los subagentes de nivel solución (AG-ROOT para este README, AG-05 y AG-09 para `_solucion/`). Cada categoría tiene su propio archivo de reglas en `SDD2.1D/devs/rules/` y produce sus artefactos con cabecera versionada y control de cambios.

Para contribuir: no se edita este README a mano salvo para corregir enlaces o reflejar un cambio del manifiesto. El detalle de cada categoría se modifica en su carpeta, a través del subagente correspondiente, respetando la nomenclatura kebab-case con sufijo de versión (`_vX.Y`) y el enum cerrado de estados. La regeneración del README raíz consume el `SOLUTION-MANIFEST` y el `SOLUTION-INTAKE` vigentes y vuelve a validar que todos los enlaces internos resuelvan.

Cada categoría de la documentación tiene un subagente responsable. El detalle de su producción y reglas vive en `SDD2.1D/devs/rules/`.

| Subagente | Categoría que produce |
| --- | --- |
| AG-ROOT | README raíz de la solución (este documento) |
| AG-00 | 00_contexto (visión, alcance, roadmap, compatibilidad) |
| AG-01 | 01_necesidades_negocio (catálogo de NB) |
| AG-02 a AG-11 | Documentación 02 a 11 de cada proyecto bajo `proyectos/<kebab>/` |
| AG-05 | Vista de solución y arquitectura técnica |
| AG-09 | Pipeline de solución y DevOps |

Trazabilidad de regeneración: el README raíz es downstream del `SOLUTION-MANIFEST` y el `SOLUTION-INTAKE` (de los que toma el nombre de la solución, la composición, los tipos D8 y las dependencias) y upstream de la navegación hacia las categorías de solución y la carpeta de cada proyecto.

## 7. Estado actual y roadmap

Tabla C. Estado por categoría de nivel solución y por proyecto. El roadmap detallado por fases vive en `00_contexto` y no se replica aquí; ver [00_contexto/roadmap-producto_v1.0.md](00_contexto/roadmap-producto_v1.0.md).

La entrega es por cortes verticales con esqueleto de punta a punta: el primer incremento recorre la solución completa con autenticación y jerarquía de usuarios, y los incrementos siguientes agregan profundidad (relevamientos y marcadores; captura y sincronización en campo; revisión, resolución de conflictos al cierre, portabilidad y configuración de almacenamiento). El roadmap enlazado define las fases, sus dependencias y los criterios verificables de transición; este README no fija fechas porque el proyecto no tiene fecha objetivo y la cadencia la marca el avance del equipo.

| Categoría / Proyecto | Estado | Versión vigente |
| --- | --- | --- |
| 00_contexto | Propuesto | 1.0 |
| 01_necesidades_negocio | Propuesto | 1.0 |
| _solucion (vista y pipeline) | Propuesto | 1.0 |
| geovial-api | Propuesto | 1.0 |
| geovial-web | Propuesto | 1.0 |
| geovial-mobile | Propuesto | 1.0 |
| geovial-storage | Propuesto | 1.0 |
| aplicada-sync | Propuesto | 1.0 |

## 8. Glosario rápido

Glosario breve del dominio. El glosario completo vive en la documentación de cada categoría.

| Término | Definición |
| --- | --- |
| Relevamiento | Tarea que registra una serie de observaciones del estado de un tramo vial; tiene ciclo de recolección, revisión y cierre. |
| Tramo vial | Extensión a relevar que puede abarcar uno o varios puentes y caminos; es el alcance de un relevamiento. |
| Observación | Registro anclado a un marcador geográfico, compuesto por notas, comentarios y fotos con comentario y etiqueta por foto. |
| Marcador geográfico | Punto en el mapa que agrupa observaciones, fotos, comentarios y textos; es etiquetable y puede ser compartido por varias observaciones. |
| Conflicto de marcadores | Situación en que dos o más marcadores caen dentro de un mismo radio; convive con la operación y se resuelve al cierre. |
| Agente de campo | Persona que toma relevamientos asignados e ingresa y administra sus observaciones, fotos, comentarios y etiquetas en terreno. |
| Jefe de área | Usuario que administra agentes de campo, administra y asigna relevamientos, y los revisa para su cierre. |
| Jefe general | Usuario que administra a los jefes de área. |
| Usuario raíz | Usuario con acceso pleno que administra y configura todo el sistema y da de alta al jefe general. |
| Sincronización | Proceso que sube primero los cambios locales del agente y luego baja las últimas actualizaciones de sus relevamientos asignados. |
| Radio de agrupación | Parámetro que, en carga manual, agrupa fotos dentro de un mismo marcador según su georreferenciación. |
| Etiqueta | Marca aplicable a fotos y a marcadores para filtrado posterior. |

## 9. Contacto y responsables

| Rol | Responsable | Canal de comunicación |
| --- | --- | --- |
| Propietario / aprobador | Vialidad provincial | Aprobación del intake y validación de necesidades de negocio |
| Implementador | Departamento de desarrollo de software (1 desarrollador) | Construcción y mantenimiento de la solución |
| Usuario raíz | Rol del sistema | Configuración del sistema y alta del jefe general |
| Jefe general | Rol del sistema | Administración de jefes de área |
| Jefe de área | Rol del sistema | Administración de agentes, relevamientos y cierre |
| Agente de campo | Rol del sistema | Captura y administración de observaciones en terreno |

La solución es single-tenant para una única organización (Vialidad provincial): los cuatro roles del sistema son control de acceso jerárquico, no aislamiento por inquilino. El propietario aprueba y valida; el implementador construye y mantiene; los roles del sistema son los beneficiarios de la operación.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README raíz inicial de la solución GeoVial: identidad, tabla de 5 proyectos reflejando el manifiesto, stack y composición, mapa de documentación, flujo de lectura por audiencia, roadmap enlazado, glosario, responsables y control de cambios. |
