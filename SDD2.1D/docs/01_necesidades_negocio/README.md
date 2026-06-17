# Necesidades de negocio — GeoVial (guía de la sección)

Esta sección reúne las necesidades de negocio (NB) de la solución GeoVial. El punto de entrada formal es el índice maestro [necesidades-negocio_v1.0.md](necesidades-negocio_v1.0.md); este README complementa con la guía de navegación, el orden de lectura y un RACI breve. Hay 7 NB, por lo que el README es obligatorio según las reglas de la categoría (§2.1 / §3.4).

## Tabla de NB

| NB | Título | Impacto principal | Prioridad MoSCoW | Estado | Enlace |
| --- | --- | --- | --- | --- | --- |
| NB-01 | Administración jerárquica de usuarios y control de acceso | Da accountability y alcance acotado a toda la operación | Must Have | Propuesto | [archivo](necesidades-de-negocio/NB-01-administracion-jerarquica-usuarios_v1.0.md) |
| NB-02 | Gestión y asignación de relevamientos por el jefe de área | Ordena y reparte el trabajo de campo por tramo | Must Have | Propuesto | [archivo](necesidades-de-negocio/NB-02-gestion-asignacion-relevamientos_v1.0.md) |
| NB-03 | Captura georreferenciada de observaciones en campo | Núcleo de valor: evidencia anclada a su ubicación | Must Have | Propuesto | [archivo](necesidades-de-negocio/NB-03-captura-georreferenciada-observaciones_v1.0.md) |
| NB-04 | Trabajo sin conexión con sincronización confiable | Habilita relevar sin red y mitiga pérdida de datos | Must Have | Propuesto | [archivo](necesidades-de-negocio/NB-04-trabajo-sin-conexion-sincronizacion_v1.0.md) |
| NB-05 | Revisión sobre mapa y cierre con resolución de conflictos | Cierra el ciclo y acelera el informe reproducible | Must Have | Propuesto | [archivo](necesidades-de-negocio/NB-05-revision-mapa-cierre-conflictos_v1.0.md) |
| NB-06 | Portabilidad del relevamiento completo | Respaldo e intercambio del relevamiento autocontenido | Could Have | Propuesto | [archivo](necesidades-de-negocio/NB-06-portabilidad-relevamiento_v1.0.md) |
| NB-07 | Almacenamiento de archivos configurable | Control del destino de la evidencia según costo y contexto | Could Have | Propuesto | [archivo](necesidades-de-negocio/NB-07-almacenamiento-archivos-configurable_v1.0.md) |

## Mapa de dependencias

| NB | Depende de | Es prerequisito de |
| --- | --- | --- |
| NB-01 | (fundacional) | NB-02 |
| NB-02 | NB-01 | NB-03 |
| NB-03 | NB-02 | NB-04, NB-05, NB-07 |
| NB-04 | NB-03 | NB-05 |
| NB-05 | NB-03, NB-04 | NB-06 |
| NB-06 | NB-05 | (ninguna) |
| NB-07 | NB-03 | (ninguna) |

Cadena acíclica (DAG), dependencia máxima por NB igual a 2:

```text
NB-01 → NB-02 → NB-03 → NB-04 → NB-05 → NB-06
                  └────────────→ NB-07
```

## Orden de lectura sugerido

1. NB-01 — base de roles y acceso, sin la cual nada tiene responsable.
2. NB-02 — la unidad de trabajo (relevamiento) y su reparto.
3. NB-03 — la captura georreferenciada, núcleo de la propuesta de valor.
4. NB-04 — el trabajo sin conexión que hace viable la captura en campo.
5. NB-05 — la revisión y el cierre que producen el informe.
6. NB-06 y NB-07 — capacidades Could Have, en cualquier orden, al final.

El orden coincide con el ciclo del relevamiento y con la entrega incremental end-to-end del proyecto: las cinco Must Have forman el camino principal y las dos Could Have se incorporan según la cadencia.

## RACI breve

Convenciones: R responsable de ejecutar, A rinde cuentas y aprueba, C consultado, I informado. El propietario es Vialidad provincial; el implementador es el Departamento de desarrollo de software (1 desarrollador); los beneficiarios son los roles del sistema.

| NB | Propietario (A) | Implementador (R) | Revisor / beneficiario clave (C) |
| --- | --- | --- | --- |
| NB-01 | Vialidad provincial | Departamento de desarrollo de software | Usuario raíz y jefe de área |
| NB-02 | Vialidad provincial | Departamento de desarrollo de software | Jefe de área |
| NB-03 | Vialidad provincial | Departamento de desarrollo de software | Agente de campo y jefe de área |
| NB-04 | Vialidad provincial | Departamento de desarrollo de software | Agente de campo |
| NB-05 | Vialidad provincial | Departamento de desarrollo de software | Jefe de área |
| NB-06 | Vialidad provincial | Departamento de desarrollo de software | Jefe de área |
| NB-07 | Vialidad provincial | Departamento de desarrollo de software | Usuario raíz |

Los cuatro roles del sistema (usuario raíz, jefe general, jefe de área, agente de campo) y el propietario quedan informados (I) del catálogo completo a través del índice maestro.

## Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección con tabla de NB, mapa de dependencias, orden de lectura y RACI breve, para un catálogo de 7 NB. |
