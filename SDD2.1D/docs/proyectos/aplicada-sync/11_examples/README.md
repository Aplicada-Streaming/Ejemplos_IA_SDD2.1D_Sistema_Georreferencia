# Examples — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** README.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Sample Engineer (AG-11)
**Audiencia:** Developer integrador que evalúa o adopta el motor de sincronización desde su propia aplicación

Índice navegable de los samples ejecutables del motor de sincronización `aplicada-sync`, un paquete distribuible y agnóstico del dominio que un host integra para propagar cambios locales a un backend remoto bajo la política subir-luego-bajar, sin perder ni duplicar datos ante cortes. En esta carpeta (`docs/.../11_examples/`) están los markdown explicativos de cada sample; el código ejecutable correspondiente vive en `/samples/aplicada-sync/` del repositorio, con correspondencia uno a uno entre cada markdown y su carpeta de código.

Este README es una tabla de contenidos viva; no duplica el contenido de cada markdown explicativo.

## 1. Propósito de la carpeta

- En `docs/.../11_examples/` el lector encuentra, por cada sample, su objetivo, su nivel, sus prerequisites, cómo correrlo, la estructura del código, el output esperado, variaciones sugeridas y la trazabilidad a CU/ADR/NFR.
- En `/samples/aplicada-sync/` el lector encuentra el proyecto ejecutable de cada sample, autocontenido, que se clona, se corre en un entorno limpio y se modifica como punto de partida.
- La progresión es por nivel de complejidad (básico, intermedio, avanzado). Entre los tres samples se ilustra el grueso de los casos de uso CU-01 a CU-06 y el punto de extensión principal del motor.

## 2. Tabla maestra de samples vigentes

| Sample | Nivel | Tiempo de setup | CU ilustrados | Ubicación |
| --- | --- | --- | --- | --- |
| `ejemplo-01-basico_v1.0.md` | Básico | < 5 min | CU-01, CU-03 | `/samples/aplicada-sync/01-basico/` |
| `ejemplo-02-intermedio_v1.0.md` | Intermedio | 10-15 min | CU-02, CU-05, CU-06 | `/samples/aplicada-sync/02-intermedio/` |
| `ejemplo-03-avanzado-integracion-real_v1.0.md` | Avanzado | 20-30 min | CU-04 (más CU-01, CU-03 en integración) | `/samples/aplicada-sync/03-avanzado-demo-maui/` |

Cobertura de casos de uso entre los tres samples: CU-01, CU-02, CU-03, CU-04, CU-05 y CU-06 quedan ilustrados; el punto de extensión principal (las estrategias del motor descritas en la extensibilidad de 05) lo ejercita el sample avanzado.

## 3. Convenciones de los samples

- Autocontenidos: cada sample incluye sus estrategias de prueba y su escenario; no depende de otros samples para correr.
- Ejecutables en entorno limpio: cada sample llega a su primera ejecución exitosa en cinco pasos o menos.
- Niveles declarados: cada markdown declara su nivel explícito en su sección 2 y justifica qué agrega respecto del anterior.
- Nomenclatura por progresión: los nombres responden al nivel de complejidad (básico, intermedio, avanzado/integración), nunca al dominio de un producto particular. Todos los markdown llevan el sufijo de versión `_v1.0.md`.
- Trazabilidad obligatoria: cada sample declara en su sección 8 al menos un CU, ADR o NFR que ilustra, con enlace al artefacto fuente de 02 y 05.

## 4. Cómo agregar un sample nuevo

1. Elegir el siguiente número correlativo y un slug de progresión por nivel o capacidad (nunca por dominio), con sufijo `_v1.0.md`.
2. Redactar el markdown explicativo con las nueve secciones obligatorias siguiendo `SDD2.1D/devs/rules/11_rules_examples.md` §4.2 y el ejemplo de cabecera de §4.1.
3. Crear la carpeta de código correspondiente en `/samples/aplicada-sync/XX-<slug>/` (materialización gobernada por el SOLUTION-INTAKE §16.1).
4. Declarar la trazabilidad a CU/ADR/NFR en la sección 8 y registrar el sample en la tabla maestra de §2 de este README.

## 5. Vínculo con la developer guide (10) y la arquitectura (05)

- Conceptos y onboarding: `10_developer_guide/conceptos-fundamentales_v1.0.md` y `10_developer_guide/guia-onboarding-developer_v1.0.md` son el acompañamiento conceptual del sample básico y del intermedio.
- Integración en una app host: `10_developer_guide/guia-integracion-aplicacion-movil_v1.0.md` es el acompañamiento conceptual del sample avanzado de integración.
- Contrato y extensibilidad: `05_arquitectura_tecnica/extensibilidad_v1.0.md` y `05_arquitectura_tecnica/contratos-abstractions_v1.0.md` definen los puntos de extensión y los contratos que los tres samples respetan; los ADR-01 a ADR-08 gobiernan las decisiones que los samples materializan.

## 6. Estructura de `/samples` para este tipo de proyecto

`aplicada-sync` es de tipo `library`. La estructura de `/samples/aplicada-sync/` deriva del SOLUTION-INTAKE §16.1.

| Tipo D8 | Estructura de `/samples/aplicada-sync/` |
| --- | --- |
| library | `01-basico/`, `02-intermedio/`, `03-avanzado-demo-maui/` |

Las tres carpetas son el piso de samples del tipo `library`. La carpeta del sample avanzado conserva el nombre canónico `03-avanzado-demo-maui` definido en el intake §16.1; el markdown explicativo se nombra por progresión (`ejemplo-03-avanzado-integracion-real_v1.0.md`).

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice inicial de la categoría 11 para aplicada-sync con la tabla maestra de los tres samples (básico, intermedio, avanzado-integración) y la cobertura de CU-01 a CU-06 más el punto de extensión principal. |
