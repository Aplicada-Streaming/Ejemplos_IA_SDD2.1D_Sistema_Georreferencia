# 05 Arquitectura técnica — geovial-mobile

**Proyecto:** geovial-mobile
**Tipo (D8):** mobile-app-maui
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto Móvil

Índice navegable de la arquitectura técnica de `geovial-mobile`, la app de campo offline-first del agente de relevamiento. Es la primera categoría de diseño técnico (05) del proyecto; recibe upstream de 02 (7 CU, 5 RN, modelo conceptual del almacén local de 8 entidades) y de los NFR del intake (§17.P.10), y ancla 06, 07, 08 y 09.

## Documentos

| Documento | Descripción |
| --- | --- |
| [arquitectura-solucion_v1.0.md](arquitectura-solucion_v1.0.md) | Documento maestro: estilo, cuatro vistas mínimas (lógica, procesos, despliegue, datos), cross-cutting, NFR, riesgos y trazabilidad. |
| [decisiones-arquitectura_v1.0.md](decisiones-arquitectura_v1.0.md) | Índice navegable de los ADR con estado, fecha y categoría. |
| [modelo-datos-logico_v1.0.md](modelo-datos-logico_v1.0.md) | Modelo lógico del almacén local offline (8 entidades, cola y marca de sincronización), réplica del dominio autoritativo de la API. |
| [flujo-ejecucion_v1.0.md](flujo-ejecucion_v1.0.md) | Pipeline de captura offline y sincronización subir-luego-bajar con reanudación y convivencia de conflictos. |

## ADRs vigentes

| ADR | Título | Categoría | Estado |
| --- | --- | --- | --- |
| [ADR-01](adrs/ADR-01-estilo-app-hibrida-mvvm-offline-first_v1.0.md) | Estilo app híbrida con MVVM offline-first | Estilo | Aceptado |
| [ADR-02](adrs/ADR-02-persistencia-almacen-local-migraciones_v1.0.md) | Persistencia en almacén local con migraciones versionadas | Persistencia | Aceptado |
| [ADR-03](adrs/ADR-03-sincronizacion-motor-subir-luego-bajar_v1.0.md) | Sincronización por consumo del motor subir-luego-bajar | Comunicación | Aceptado |
| [ADR-04](adrs/ADR-04-gestion-permisos-degradacion_v1.0.md) | Gestión de permisos del sistema operativo con degradación | Seguridad | Aceptado |
| [ADR-05](adrs/ADR-05-autenticacion-token-seguro-relogueo-dispositivo_v1.0.md) | Autenticación con token seguro y relogueo por seguridad del dispositivo | Seguridad | Aceptado |

El mínimo del tipo `mobile-app-maui` (4 ADR: estilo, persistencia local, sincronización, gestión de permisos) se cumple con ADR-01 a ADR-04 y se supera con ADR-05.

## NFR (resumen, ver §8 del documento maestro)

| NFR | Objetivo |
| --- | --- |
| Captura offline | 100 % de la captura con foto sin conexión |
| Capacidad de la cola local | ≥ 1000 cambios pendientes |
| Tiempo de un ciclo de sincronización | 100 cambios ≤ 30 s en red móvil típica |
| Reanudación | sin pérdida ni duplicación tras un corte |
| Arranque en frío | ≤ 3 s en el dispositivo de referencia |

## Contratos consumidos (no definidos aquí)

- Contrato REST del dominio autoritativo: `proyectos/geovial-api/05_arquitectura_tecnica/contratos-rest_v1.0.md` (arista `geovial-mobile → geovial-api`).
- Contrato de la librería de sincronización: `proyectos/aplicada-sync/05_arquitectura_tecnica/contratos-abstractions_v1.0.md` (arista `geovial-mobile → aplicada-sync`).

`geovial-mobile` no define contrato propio (`contratos-<area>` se omite: solo consume) ni `extensibilidad` (`tiene_extensibilidad = false`).
