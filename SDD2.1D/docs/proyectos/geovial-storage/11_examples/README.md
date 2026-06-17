# Ejemplos — geovial-storage

**Proyecto:** geovial-storage
**Documento:** README.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-15
**Autor:** Developer Advocate / Sample Engineer Senior (variante Sample Engineer, library)

## 1. Propósito de la carpeta

Esta carpeta documenta los samples ejecutables de `geovial-storage`. Cada sample es una aplicación consumidora del proveedor de almacenamiento que ejercita un nivel distinto de la superficie pública: del camino feliz mínimo a la extensión del puerto de proveedor. El lector encuentra acá el qué y el porqué de cada sample; el código vive en `/samples/geovial-storage/` del repositorio de la solución, con correspondencia uno a uno entre cada markdown `ejemplo-XX-...` y una carpeta ejecutable.

La progresión es por nivel de complejidad: el sample 01 introduce el guardado y la recuperación con el proveedor local; el sample 02 agrega la configuración y selección del proveedor activo más las operaciones de gestión, mostrando transparencia entre proveedores; el sample 03 demuestra el punto de extensión, implementando y registrando un proveedor nuevo y validándolo con la suite de conformidad.

## 2. Tabla maestra de samples

| Sample | Nivel | Tiempo de setup | CU ilustrados | Ubicación |
| --- | --- | --- | --- | --- |
| `ejemplo-01-basico_v1.0.md` | Básico | < 5 min | CU-01, CU-02 | `/samples/geovial-storage/01-basico-consola/` |
| `ejemplo-02-intermedio_v1.0.md` | Intermedio | 10-15 min | CU-03, CU-04, CU-05, CU-06 | `/samples/geovial-storage/02-intermedio-con-extensiones/` |
| `ejemplo-03-avanzado-con-extensiones_v1.0.md` | Avanzado | 20-30 min | CU-06 (registro) y extensión del puerto de proveedor | `/samples/geovial-storage/03-avanzado-integracion-real/` |

Cobertura de casos de uso entre los tres samples: CU-01 y CU-02 en el sample 01; CU-03, CU-04, CU-05 y CU-06 en el sample 02; el punto de extensión (puerto de proveedor) y RN-01 (transparencia) en el sample 03, que reusa CU-06 para registrar y seleccionar el proveedor nuevo. Los seis casos de uso CU-01 a CU-06 quedan ilustrados por el conjunto.

## 3. Convenciones de los samples

- Autocontenidos: cada sample se clona, se ejecuta en un entorno limpio y se modifica como punto de partida, sin depender de servicios productivos.
- Ejecutables en entorno limpio: cada sample arranca en cinco pasos o menos hasta el primer resultado exitoso, con su output esperado documentado.
- Nivel declarado: cada sample declara explícitamente su nivel (básico, intermedio o avanzado) y qué capacidad agrega respecto del anterior.
- Trazabilidad obligatoria: cada sample referencia en su sección de trazabilidad al menos un CU, ADR o NFR de las categorías 02 y 05.
- Proveedores abstractos: los samples se describen como local / remoto / otro; el proveedor "otro" del sample 03 puede ser un doble de conformidad o un destino real, siempre con credenciales sintéticas no productivas (RN-03).
- Transparencia verificable: ningún sample escribe ramas de código por proveedor; el mismo flujo de consumo opera contra cualquier proveedor activo (RN-01).

## 4. Cómo agregar un sample nuevo

1. Definir el nivel o la capacidad que agrega respecto de los existentes, manteniendo la progresión y sin nombrar por dominio.
2. Crear la carpeta ejecutable en `/samples/geovial-storage/XX-<kebab-progresion>/` con su README propio y sus tests de verificación.
3. Redactar el markdown explicativo `ejemplo-XX-<kebab-progresion>_v1.0.md` con las nueve secciones obligatorias, según §4 de las reglas constructivas de la categoría 11 (`SDD2.1D/devs/rules/11_rules_examples.md`).
4. Declarar la trazabilidad a CU, ADR o NFR en la sección 8 y actualizar la tabla maestra de este README.

## 5. Vínculo con la developer guide y la arquitectura

- Developer guide (10): el sample 01 materializa el Hello world de `guia-onboarding-developer_v1.0.md`; el sample 02 materializa el primer caso real y el cambio de proveedor de esa misma guía y de `guia-integracion-servicio-backend_v1.0.md`; el sample 03 materializa la `guia-testing-extensibilidad_v1.0.md` (08) y la `extensibilidad_v1.0.md` (05).
- Arquitectura (05): los samples respetan el contrato de `contratos-abstractions_v1.0.md` y las decisiones de ADR-01 (proveedores intercambiables), ADR-02 (superficie pública estable), ADR-04 (transparencia e integridad) y ADR-05 (manejo seguro de credenciales). El punto de extensión es el puerto de proveedor de almacenamiento descrito en `extensibilidad_v1.0.md`.
- Especificación funcional (02): cada sample ejercita uno o más de los casos de uso CU-01 a CU-06 y verifica las reglas de negocio RN-01 (transparencia), RN-02 (integridad) y RN-03 (manejo seguro de credenciales).

## 6. Tabla tipo de proyecto vs estructura de `/samples`

| Tipo D8 | Estructura de `/samples/geovial-storage/` |
| --- | --- |
| library | `01-basico-consola/`, `02-intermedio-con-extensiones/`, `03-avanzado-integracion-real/` |
| (resto) | Ver §2.3 de `SDD2.1D/devs/rules/11_rules_examples.md`. |

`geovial-storage` es de tipo `library`: aplica el piso de tres samples (básico, intermedio, avanzado) de §2.2 de las reglas constructivas. La estructura de carpetas refleja la matriz §2.3 para `library`, namespaciada bajo `geovial-storage/` por ser una solución multiproyecto (intake §16.1).

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice inicial de samples de geovial-storage: tabla maestra de tres samples progresivos (básico, intermedio, avanzado), convenciones, guía para agregar samples y vínculo con las developer guides de 10 y la arquitectura de 05. |
