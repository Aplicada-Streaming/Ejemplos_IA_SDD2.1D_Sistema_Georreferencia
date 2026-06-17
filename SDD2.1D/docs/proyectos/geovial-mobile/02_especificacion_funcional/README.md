# 02 Especificación funcional — geovial-mobile

**Proyecto:** geovial-mobile (mobile-app-maui, app de captura en terreno de la solución GeoVial)
**Estado de la sección:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + Mobile UX Analyst

Punto de entrada navegable de la especificación funcional de la app móvil `geovial-mobile`. El índice maestro es [`especificacion-funcional_v1.0.md`](especificacion-funcional_v1.0.md), que contiene el catálogo completo y la matriz de trazabilidad NB→CU→RN→US.

## Resumen

- 7 casos de uso de flujos de campo del agente (CU-01 a CU-07).
- 5 reglas de negocio del lado móvil (RN-01 a RN-05).
- Modelo conceptual del almacén local de 8 entidades; sin reglas conceptuales de modelo (RC) por no superar las diez entidades.
- Cobertura del lado de campo de NB-01 (login), NB-03 (captura georreferenciada) y NB-04 (trabajo sin conexión y sincronización). El dominio autoritativo es el de geovial-api; el modelo local es una réplica para trabajo offline.

## Casos de uso

| CU | Nombre | NB | Estado |
| --- | --- | --- | --- |
| [CU-01](casos-de-uso/CU-01-iniciar-cerrar-sesion-relogueo-dispositivo_v1.0.md) | Iniciar sesión, deslogueo completo y relogueo por seguridad del dispositivo | NB-01 | Propuesto |
| [CU-02](casos-de-uso/CU-02-seleccionar-relevamiento-asignado_v1.0.md) | Seleccionar un relevamiento asignado | NB-04, NB-03 | Propuesto |
| [CU-03](casos-de-uso/CU-03-centrar-gps-crear-mover-marcador_v1.0.md) | Centrar por GPS y crear o mover un marcador en el mapa | NB-03 | Propuesto |
| [CU-04](casos-de-uso/CU-04-capturar-foto-resolver-coordenadas_v1.0.md) | Capturar una foto con resolución de coordenadas en el momento | NB-03 | Propuesto |
| [CU-05](casos-de-uso/CU-05-agregar-comentarios-etiquetas-observacion_v1.0.md) | Agregar comentarios y etiquetas a la observación | NB-03 | Propuesto |
| [CU-06](casos-de-uso/CU-06-trabajar-sin-conexion-sincronizar_v1.0.md) | Trabajar sin conexión y sincronizar subiendo antes de bajar | NB-04 | Propuesto |
| [CU-07](casos-de-uso/CU-07-cargar-fotos-manualmente-radio-agrupacion_v1.0.md) | Cargar fotos manualmente priorizando ubicación con radio de agrupación | NB-03 | Propuesto |

## Reglas de negocio

| RN | Nombre | Estado |
| --- | --- | --- |
| [RN-01](reglas-de-negocio/RN-01-prioridad-ubicacion-radio-agrupacion_v1.0.md) | Prioridad de la ubicación incrustada y radio de agrupación en la carga manual | Propuesto |
| [RN-02](reglas-de-negocio/RN-02-orden-sincronizacion-subir-antes-de-bajar_v1.0.md) | Orden de sincronización subir antes de bajar | Propuesto |
| [RN-03](reglas-de-negocio/RN-03-convivencia-con-conflictos-en-el-cliente_v1.0.md) | Convivencia con conflictos de marcadores en el cliente | Propuesto |
| [RN-04](reglas-de-negocio/RN-04-relogueo-por-seguridad-del-dispositivo_v1.0.md) | Relogueo por seguridad del dispositivo en sesión activa | Propuesto |
| [RN-05](reglas-de-negocio/RN-05-captura-sin-conexion_v1.0.md) | Captura sin conexión con cola local persistente | Propuesto |

## Modelo de datos

- [Modelo conceptual del almacén local](modelo-datos/modelo-conceptual_v1.0.md) — 8 entidades, relaciones, cardinalidades, diagrama y trazabilidad. Réplica del dominio autoritativo de geovial-api para trabajo offline.

No se incorporan reglas conceptuales de modelo (RC): el modelo local no supera las diez entidades (02 §2.2). Las invariantes de integridad fina las gobierna geovial-api.

## Estructura de la sección

```text
02_especificacion_funcional/
├── especificacion-funcional_v1.0.md      # índice maestro y matriz NB→CU→RN→US
├── README.md                             # este archivo
├── casos-de-uso/                         # CU-01 a CU-07
├── reglas-de-negocio/                    # RN-01 a RN-05
└── modelo-datos/
    └── modelo-conceptual_v1.0.md         # almacén local del dispositivo (8 entidades, sin RC)
```
