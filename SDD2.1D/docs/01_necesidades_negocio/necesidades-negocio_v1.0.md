# Necesidades de negocio — Solución GeoVial

| Campo | Valor |
| --- | --- |
| Proyecto | geovial-api (proyecto principal de la solución GeoVial) |
| Documento | necesidades-negocio_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-06-15 |
| Autor | Analista de Negocio + API Product Analyst |
| Cantidad de NB | 7 |
| Versión del catálogo de NB | 1.0 |
| Trazabilidad upstream | SOLUTION-INTAKE §1, §3, §4, §8; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | 02_especificacion_funcional (CU-01 a CU-17 previstas); 06_backlog-tecnico; 07_plan-sprint; 08_calidad_y_pruebas |

## 1. Propósito

Este índice maestro consolida las necesidades de negocio (NB) de la solución GeoVial, derivadas de los dolores del SOLUTION-INTAKE (§1), de la propuesta de valor (§3) y de las capacidades Must Have y Should Have del alcance (§4, F-01 a F-15), más dos NB Could Have (F-16, F-17). Cada NB articula un problema concreto del negocio, su métrica de éxito y su prioridad relativa, y declara las CU previstas que la implementarán en la categoría 02. La categoría 01 se genera una vez a nivel solución a partir del intake único; la variante de especialidad aplicada es la del proyecto principal `geovial-api` (`rest-api`): Analista de Negocio + API Product Analyst.

## 2. Tabla resumen de NB

| ID | Necesidad | Prioridad MoSCoW | CU previstas | Estado | Enlace |
| --- | --- | --- | --- | --- | --- |
| NB-01 | Administración jerárquica de usuarios y control de acceso | Must Have | CU-01, CU-02, CU-03 | Propuesto | [NB-01](necesidades-de-negocio/NB-01-administracion-jerarquica-usuarios_v1.0.md) |
| NB-02 | Gestión y asignación de relevamientos por el jefe de área | Must Have | CU-04, CU-05, CU-06 | Propuesto | [NB-02](necesidades-de-negocio/NB-02-gestion-asignacion-relevamientos_v1.0.md) |
| NB-03 | Captura georreferenciada de observaciones en campo | Must Have | CU-07, CU-08, CU-09 | Propuesto | [NB-03](necesidades-de-negocio/NB-03-captura-georreferenciada-observaciones_v1.0.md) |
| NB-04 | Trabajo sin conexión con sincronización confiable | Must Have | CU-10, CU-11 | Propuesto | [NB-04](necesidades-de-negocio/NB-04-trabajo-sin-conexion-sincronizacion_v1.0.md) |
| NB-05 | Revisión sobre mapa y cierre con resolución de conflictos | Must Have | CU-12, CU-13, CU-14 | Propuesto | [NB-05](necesidades-de-negocio/NB-05-revision-mapa-cierre-conflictos_v1.0.md) |
| NB-06 | Portabilidad del relevamiento completo | Could Have | CU-15, CU-16 | Propuesto | [NB-06](necesidades-de-negocio/NB-06-portabilidad-relevamiento_v1.0.md) |
| NB-07 | Almacenamiento de archivos configurable | Could Have | CU-17 | Propuesto | [NB-07](necesidades-de-negocio/NB-07-almacenamiento-archivos-configurable_v1.0.md) |

## 3. Mapa de dependencias entre NB

Las dependencias son acíclicas y ninguna NB depende de más de tres otras. Forman una cadena que respeta el ciclo del relevamiento (administrar, gestionar, capturar, sincronizar, revisar) más dos capacidades de portabilidad y almacenamiento derivadas.

| NB | Depende de | Es prerequisito de |
| --- | --- | --- |
| NB-01 | (ninguna, fundacional) | NB-02 |
| NB-02 | NB-01 | NB-03 |
| NB-03 | NB-02 | NB-04, NB-05, NB-07 |
| NB-04 | NB-03 | NB-05 |
| NB-05 | NB-03, NB-04 | NB-06 |
| NB-06 | NB-05 | (ninguna) |
| NB-07 | NB-03 | (ninguna) |

Representación de la cadena principal:

```text
NB-01 → NB-02 → NB-03 → NB-04 → NB-05 → NB-06
                  └────────────→ NB-07
```

Verificación: el grafo es un DAG (sin ciclos); la dependencia máxima por NB es 2 (NB-05 depende de NB-03 y NB-04), por debajo del tope de 3.

## 4. Trazabilidad agregada

Upstream. Toda NB nace del SOLUTION-INTAKE de GeoVial y de los documentos consolidados de 00_contexto:

| NB | Capacidades del intake §4 | Otras referencias del intake | 00_contexto |
| --- | --- | --- | --- |
| NB-01 | F-01, F-02, F-08 | §1, §2 | vision-producto §2; alcance §4.1 |
| NB-02 | F-03, F-04, F-11, F-14 | §1, §6 | vision-producto §1; alcance §4.1 |
| NB-03 | F-05, F-06, F-09, F-10, F-15 | §1, §3, §8 | vision-producto §3, §6; alcance §4.1 |
| NB-04 | F-07 | §1, §3, §8, §11 | vision-producto §3, §8 (R-03); alcance §3 |
| NB-05 | F-11, F-12, F-13 | §1, §3, §8 | vision-producto §3, §6; alcance §3, §4.1 |
| NB-06 | F-16 | §3 | vision-producto §4; alcance §4.1 |
| NB-07 | F-17 | §3 | alcance §4.1 |

Downstream. La trazabilidad sigue la cadena obligatoria SOLUTION-INTAKE → 00_contexto → NB → CU → US → BT → Sprint → Test → Pipeline:

- 02_especificacion_funcional: las CU previstas CU-01 a CU-17 (estado `a generar`) desarrollan las NB. La capacidad F-18 (auto-registro de agentes) queda fuera por ser Won't Have v1 y por las exclusiones de alcance §5; no origina NB ni CU.
- 06_backlog-tecnico y 07_plan-sprint: la priorización MoSCoW ordena el backlog y los sprints. Las cinco NB Must Have (NB-01 a NB-05) componen el camino principal end-to-end; las dos NB Could Have (NB-06, NB-07) se incorporan si la cadencia lo permite.
- 08_calidad_y_pruebas: cada criterio de éxito de la §5 de cada NB alimenta los criterios de aceptación de la categoría 08.

## 5. Cobertura de capacidades del alcance

| Capacidad del intake | Prioridad | NB que la cubre |
| --- | --- | --- |
| F-01 jerarquía y administración de usuarios | Must Have | NB-01 |
| F-02 alta y baja de agentes por el jefe | Must Have | NB-01 |
| F-03 alta, baja y visualización de relevamientos | Must Have | NB-02 |
| F-04 asignación de agentes a un relevamiento | Must Have | NB-02 |
| F-05 captura con foto y resolución de coordenadas | Must Have | NB-03 |
| F-06 modelo de observación y marcador | Must Have | NB-03 |
| F-07 captura sin conexión con sincronización | Must Have | NB-04 |
| F-08 inicio de sesión, deslogueo y relogueo | Must Have | NB-01 |
| F-09 carga manual con ubicación y radio de agrupación | Must Have | NB-03 |
| F-10 visualización en mapa con puntos | Must Have | NB-03 |
| F-11 transición a revisión y cierre | Must Have | NB-02, NB-05 |
| F-12 carrusel de fotos por marcador | Should Have | NB-05 |
| F-13 resolución de conflictos al cierre | Should Have | NB-05 |
| F-14 reasignación de agentes | Should Have | NB-02 |
| F-15 carga manual completa por el agente desde web | Should Have | NB-03 |
| F-16 exportar e importar relevamiento completo | Could Have | NB-06 |
| F-17 destino de almacenamiento configurable | Could Have | NB-07 |
| F-18 auto-registro self-service de agentes | Won't Have v1 | sin NB (excluido) |

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Catálogo inicial de 7 NB de la solución GeoVial, derivado de SOLUTION-INTAKE §1, §3, §4, §8 y de los documentos consolidados de 00_contexto, con mapa de dependencias acíclico y trazabilidad agregada upstream/downstream. |
