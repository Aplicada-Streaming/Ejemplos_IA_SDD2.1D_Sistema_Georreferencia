# 02 Especificación funcional — geovial-api

**Proyecto:** geovial-api (rest-api, principal de la solución GeoVial)
**Estado de la sección:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

Punto de entrada navegable de la especificación funcional del backend `geovial-api`. El índice maestro es [`especificacion-funcional_v1.0.md`](especificacion-funcional_v1.0.md), que contiene el catálogo completo y la matriz de trazabilidad NB→CU→RN→US.

## Resumen

- 22 casos de uso: 17 de recursos públicos (CU-01 a CU-17) y 5 transversales (CU-18 a CU-22).
- 7 reglas de negocio (RN-01 a RN-07).
- Modelo conceptual de 12 entidades con 6 reglas conceptuales de modelo (RC-01 a RC-06).
- Cobertura completa de las necesidades de negocio NB-01 a NB-07 del lado servidor.

## Casos de uso

| CU | Nombre | NB | Estado |
| --- | --- | --- | --- |
| [CU-01](casos-de-uso/CU-01-administrar-jerarquia-usuarios_v1.0.md) | Administrar la jerarquía de usuarios en cuatro niveles | NB-01 | Propuesto |
| [CU-02](casos-de-uso/CU-02-administrar-agentes-campo_v1.0.md) | Dar de alta y de baja agentes de campo por el jefe de área | NB-01 | Propuesto |
| [CU-03](casos-de-uso/CU-03-iniciar-cerrar-sesion_v1.0.md) | Iniciar sesión, cerrar sesión completa y revalidar credenciales | NB-01 | Propuesto |
| [CU-04](casos-de-uso/CU-04-gestionar-relevamientos_v1.0.md) | Crear, dar de baja y visualizar relevamientos de un tramo vial | NB-02 | Propuesto |
| [CU-05](casos-de-uso/CU-05-asignar-agentes-relevamiento_v1.0.md) | Asignar y reasignar agentes de campo a un relevamiento | NB-02 | Propuesto |
| [CU-06](casos-de-uso/CU-06-transicionar-estado-relevamiento_v1.0.md) | Transicionar el estado del relevamiento de recolección a revisión | NB-02 | Propuesto |
| [CU-07](casos-de-uso/CU-07-administrar-marcadores-geograficos_v1.0.md) | Administrar marcadores geográficos del relevamiento | NB-03 | Propuesto |
| [CU-08](casos-de-uso/CU-08-administrar-observaciones_v1.0.md) | Administrar observaciones con notas, fotos, comentarios y etiquetas | NB-03 | Propuesto |
| [CU-09](casos-de-uso/CU-09-cargar-fotos-manualmente_v1.0.md) | Cargar fotos manualmente con priorización de ubicación y radio de agrupación | NB-03 | Propuesto |
| [CU-10](casos-de-uso/CU-10-recibir-cambios-locales_v1.0.md) | Recibir el lote de cambios locales del agente (subida de sincronización) | NB-04 | Propuesto |
| [CU-11](casos-de-uso/CU-11-entregar-actualizaciones-relevamiento_v1.0.md) | Entregar las actualizaciones del relevamiento asignado (bajada de sincronización) | NB-04 | Propuesto |
| [CU-12](casos-de-uso/CU-12-consultar-relevamiento-revision_v1.0.md) | Consultar el relevamiento para la revisión sobre mapa | NB-05 | Propuesto |
| [CU-13](casos-de-uso/CU-13-resolver-conflictos-marcadores_v1.0.md) | Resolver los conflictos de marcadores al cierre | NB-05 | Propuesto |
| [CU-14](casos-de-uso/CU-14-cerrar-relevamiento_v1.0.md) | Cerrar el relevamiento como hito que habilita el informe | NB-05 | Propuesto |
| [CU-15](casos-de-uso/CU-15-exportar-relevamiento_v1.0.md) | Exportar un relevamiento completo en una unidad transferible única | NB-06 | Propuesto |
| [CU-16](casos-de-uso/CU-16-importar-relevamiento_v1.0.md) | Importar un relevamiento completo reconstruyendo su estructura | NB-06 | Propuesto |
| [CU-17](casos-de-uso/CU-17-configurar-destino-almacenamiento_v1.0.md) | Configurar el destino de almacenamiento de archivos | NB-07 | Propuesto |
| [CU-18](casos-de-uso/CU-18-autorizar-por-rol_v1.0.md) | Autorizar el acceso a cada recurso según el rol y el alcance | NB-01 | Propuesto |
| [CU-19](casos-de-uso/CU-19-manejar-errores-problem-json_v1.0.md) | Devolver errores con un formato de problema uniforme | NB-01 a NB-05 | Propuesto |
| [CU-20](casos-de-uso/CU-20-paginar-filtrar-listados_v1.0.md) | Paginar y filtrar los listados de recursos | NB-02, NB-03, NB-05 | Propuesto |
| [CU-21](casos-de-uso/CU-21-garantizar-idempotencia-operaciones_v1.0.md) | Garantizar la idempotencia de las operaciones no seguras | NB-04 | Propuesto |
| [CU-22](casos-de-uso/CU-22-versionar-contrato-publico_v1.0.md) | Versionar el contrato público de la API | NB-01 a NB-05 | Propuesto |

## Reglas de negocio

| RN | Nombre | Estado |
| --- | --- | --- |
| [RN-01](reglas-de-negocio/RN-01-jerarquia-altas-bajas_v1.0.md) | Jerarquía de altas, bajas y alcance | Propuesto |
| [RN-02](reglas-de-negocio/RN-02-conservacion-autoria-en-baja_v1.0.md) | Conservación de la autoría histórica ante la baja | Propuesto |
| [RN-03](reglas-de-negocio/RN-03-convivencia-conflictos-marcadores_v1.0.md) | Convivencia con conflictos de marcadores y resolución al cierre | Propuesto |
| [RN-04](reglas-de-negocio/RN-04-radio-agrupacion-fotos_v1.0.md) | Priorización de la ubicación incrustada y radio de agrupación de fotos | Propuesto |
| [RN-05](reglas-de-negocio/RN-05-transicion-estados-relevamiento_v1.0.md) | Transición de estados del relevamiento | Propuesto |
| [RN-06](reglas-de-negocio/RN-06-orden-subir-antes-de-bajar_v1.0.md) | Orden de sincronización subir antes de bajar | Propuesto |
| [RN-07](reglas-de-negocio/RN-07-idempotencia-sincronizacion_v1.0.md) | Idempotencia de la sincronización y de las escrituras reintentables | Propuesto |

## Modelo de datos

- [Modelo conceptual](modelo-datos/modelo-conceptual_v1.0.md) — 12 entidades, relaciones, cardinalidades, diagrama y trazabilidad.

Reglas conceptuales de modelo:

| RC | Nombre | Estado |
| --- | --- | --- |
| [RC-01](modelo-datos/reglas-conceptuales-de-modelo/RC-01-identidad-marcador_v1.0.md) | Identidad estable del marcador geográfico | Propuesto |
| [RC-02](modelo-datos/reglas-conceptuales-de-modelo/RC-02-referencia-observacion-marcador_v1.0.md) | Referencia obligatoria de observación a marcador | Propuesto |
| [RC-03](modelo-datos/reglas-conceptuales-de-modelo/RC-03-integridad-jerarquia-usuarios_v1.0.md) | Integridad de la jerarquía de usuarios | Propuesto |
| [RC-04](modelo-datos/reglas-conceptuales-de-modelo/RC-04-estado-relevamiento-valido_v1.0.md) | Estado del relevamiento dentro del ciclo válido | Propuesto |
| [RC-05](modelo-datos/reglas-conceptuales-de-modelo/RC-05-unicidad-asignacion_v1.0.md) | Unicidad de la asignación agente-relevamiento | Propuesto |
| [RC-06](modelo-datos/reglas-conceptuales-de-modelo/RC-06-monotonia-marca-sincronizacion_v1.0.md) | Monotonía de la marca de sincronización | Propuesto |

## Estructura de la sección

```text
02_especificacion_funcional/
├── especificacion-funcional_v1.0.md      # índice maestro y matriz NB→CU→RN→US
├── README.md                             # este archivo
├── casos-de-uso/                         # CU-01 a CU-22
├── reglas-de-negocio/                    # RN-01 a RN-07
└── modelo-datos/
    ├── modelo-conceptual_v1.0.md
    └── reglas-conceptuales-de-modelo/    # RC-01 a RC-06
```
