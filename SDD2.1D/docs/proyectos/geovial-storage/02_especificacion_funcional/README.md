# 02 Especificación funcional — geovial-storage

**Proyecto:** geovial-storage
**Tipo (D8):** library
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

Punto de entrada navegable de la especificación funcional de `geovial-storage`, la librería que provee al backend de GeoVial una abstracción de alojamiento de archivos con proveedores intercambiables (local / remoto / otro) seleccionables por el usuario raíz. Cada caso de uso describe un contrato de uso de la superficie pública de la librería. Por ser `library`, no se produce modelo conceptual de datos (regla 02 §2.2).

## Índice maestro

- [especificacion-funcional_v1.0.md](especificacion-funcional_v1.0.md) — índice maestro con la matriz NB → CU → RN → US y la nota de compatibilidad de versión pública.

## Casos de uso

| CU | Documento | Propósito en una línea | Estado |
| --- | --- | --- | --- |
| CU-01 | [CU-01-guardar-archivo_v1.0.md](casos-de-uso/CU-01-guardar-archivo_v1.0.md) | Persistir un archivo en el proveedor activo y devolver su identificador lógico | Propuesto |
| CU-02 | [CU-02-recuperar-archivo_v1.0.md](casos-de-uso/CU-02-recuperar-archivo_v1.0.md) | Devolver el contenido de un archivo por su identificador lógico, idéntico al guardado | Propuesto |
| CU-03 | [CU-03-eliminar-archivo_v1.0.md](casos-de-uso/CU-03-eliminar-archivo_v1.0.md) | Quitar del proveedor activo un archivo por su identificador lógico | Propuesto |
| CU-04 | [CU-04-verificar-existencia-archivo_v1.0.md](casos-de-uso/CU-04-verificar-existencia-archivo_v1.0.md) | Informar si un identificador lógico corresponde a un archivo presente | Propuesto |
| CU-05 | [CU-05-listar-archivos_v1.0.md](casos-de-uso/CU-05-listar-archivos_v1.0.md) | Enumerar los identificadores presentes bajo un prefijo dado | Propuesto |
| CU-06 | [CU-06-configurar-proveedor-activo_v1.0.md](casos-de-uso/CU-06-configurar-proveedor-activo_v1.0.md) | Seleccionar y validar el proveedor activo y sus credenciales (usuario raíz) | Propuesto |

## Reglas de negocio

| RN | Documento | Invariante en una línea | Estado |
| --- | --- | --- | --- |
| RN-01 | [RN-01-transparencia-proveedor_v1.0.md](reglas-de-negocio/RN-01-transparencia-proveedor_v1.0.md) | El contrato público es idéntico cualquiera sea el proveedor activo | Propuesto |
| RN-02 | [RN-02-integridad-archivo-almacenado_v1.0.md](reglas-de-negocio/RN-02-integridad-archivo-almacenado_v1.0.md) | Lo recuperado es idénticamente igual a lo guardado bajo el mismo identificador | Propuesto |
| RN-03 | [RN-03-manejo-seguro-credenciales_v1.0.md](reglas-de-negocio/RN-03-manejo-seguro-credenciales_v1.0.md) | Las credenciales del proveedor nunca se exponen por la superficie pública | Propuesto |

## Modelo conceptual

No aplica. Por regla 02 §2.2, un proyecto `library` no genera modelo conceptual de datos ni reglas conceptuales de modelo.

## Trazabilidad upstream

- NB-07 (principal) — almacenamiento de archivos configurable.
- NB-03 (soporte) — captura georreferenciada: origina los archivos que la librería aloja.
- NB-06 (soporte) — portabilidad del relevamiento: la exportación e importación se apoyan en las operaciones de almacenamiento.

La matriz completa de cobertura bidireccional NB → CU → RN → US está en el índice maestro.
