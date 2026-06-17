# 02 Especificación funcional — geovial-web

**Proyecto:** geovial-web (web-monolith, front web de la solución GeoVial)
**Estado de la sección:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional

Punto de entrada navegable de la especificación funcional del front web `geovial-web`. El índice maestro es [`especificacion-funcional_v1.0.md`](especificacion-funcional_v1.0.md), que contiene el catálogo completo y la matriz de trazabilidad NB→CU→RN→US.

## Resumen

- 11 casos de uso de flujos de experiencia del usuario administrador (CU-01 a CU-11).
- 5 reglas de negocio de presentación y flujo (RN-01 a RN-05), derivadas de las RN del backend autoritativo.
- Modelo conceptual como vista de consumo del modelo AUTORITATIVO de `geovial-api`; sin persistencia ni reglas conceptuales de modelo (RC) propias.
- Cobertura del lado de experiencia de NB-01, NB-02, NB-05, NB-06 y NB-07.

## Casos de uso

| CU | Nombre | NB | Estado |
| --- | --- | --- | --- |
| [CU-01](casos-de-uso/CU-01-iniciar-cerrar-sesion-web_v1.0.md) | Iniciar y cerrar sesión en el front web | NB-01 | Propuesto |
| [CU-02](casos-de-uso/CU-02-administrar-usuarios-jerarquia_v1.0.md) | Administrar usuarios por jerarquía desde el front web | NB-01 | Propuesto |
| [CU-03](casos-de-uso/CU-03-crear-editar-listar-relevamiento_v1.0.md) | Crear, editar y listar relevamientos | NB-02 | Propuesto |
| [CU-04](casos-de-uso/CU-04-asignar-reasignar-agentes_v1.0.md) | Asignar y reasignar agentes a un relevamiento | NB-02 | Propuesto |
| [CU-05](casos-de-uso/CU-05-crear-marcadores-iniciales_v1.0.md) | Crear marcadores geográficos iniciales sobre el mapa | NB-02 | Propuesto |
| [CU-06](casos-de-uso/CU-06-revisar-relevamiento-mapa-carrusel_v1.0.md) | Revisar el relevamiento sobre el mapa con carrusel de fotos | NB-05 | Propuesto |
| [CU-07](casos-de-uso/CU-07-resolver-conflictos-cierre_v1.0.md) | Resolver conflictos de marcadores al cierre | NB-05 | Propuesto |
| [CU-08](casos-de-uso/CU-08-transicionar-estado-cerrar_v1.0.md) | Transicionar el estado del relevamiento y cerrarlo | NB-05 | Propuesto |
| [CU-09](casos-de-uso/CU-09-carga-manual-relevamiento-web_v1.0.md) | Cargar manualmente un relevamiento completo vía web | NB-02 | Propuesto |
| [CU-10](casos-de-uso/CU-10-exportar-importar-relevamiento_v1.0.md) | Exportar e importar un relevamiento completo | NB-06 | Propuesto |
| [CU-11](casos-de-uso/CU-11-configurar-destino-almacenamiento_v1.0.md) | Configurar el destino de almacenamiento de archivos | NB-07 | Propuesto |

## Reglas de negocio

| RN | Nombre | Estado |
| --- | --- | --- |
| [RN-01](reglas-de-negocio/RN-01-visibilidad-acciones-por-rol_v1.0.md) | Visibilidad y acciones por rol jerárquico en el front web | Propuesto |
| [RN-02](reglas-de-negocio/RN-02-conservacion-traza-autoria_v1.0.md) | Conservación de la traza de autoría al dar de baja | Propuesto |
| [RN-03](reglas-de-negocio/RN-03-acceso-web-roles-administradores_v1.0.md) | Acceso al front web restringido a roles administradores | Propuesto |
| [RN-04](reglas-de-negocio/RN-04-estados-visibles-habilitacion-acciones_v1.0.md) | Estados visibles del relevamiento y habilitación de acciones | Propuesto |
| [RN-05](reglas-de-negocio/RN-05-conflictos-precondicion-cierre_v1.0.md) | Resolución de conflictos como precondición visible del cierre | Propuesto |

## Modelo de datos

- [Modelo conceptual](modelo-datos/modelo-conceptual_v1.0.md) — vista de consumo del modelo autoritativo de `geovial-api`; 12 entidades presentadas (incluida la proyección DestinoAlmacenamiento), relaciones, cardinalidades, diagrama y trazabilidad. Sin reglas conceptuales de modelo (RC) propias: el front no posee invariantes de integridad.

## Relación con el proyecto autoritativo

El modelo de dominio y su integridad pertenecen a `geovial-api` (`docs/proyectos/geovial-api/02_especificacion_funcional/`). `geovial-web` consume ese contrato y no lo redefine; su numeración de CU y RN es propia. La correspondencia de consumo entre los CU del front y los recursos de la API está en la §7 del índice maestro.

## Estructura de la sección

```text
02_especificacion_funcional/
├── especificacion-funcional_v1.0.md      # índice maestro y matriz NB→CU→RN→US
├── README.md                             # este archivo
├── casos-de-uso/                         # CU-01 a CU-11
├── reglas-de-negocio/                    # RN-01 a RN-05
└── modelo-datos/
    └── modelo-conceptual_v1.0.md         # vista de consumo (sin RC propias)
```
