# Examples — geovial-web

**Proyecto:** geovial-web
**Documento:** README.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Sample Engineer (web-monolith)
**Audiencia:** Developer que arranca la app de demostración del front web y evaluador técnico que recorre los flujos administrativos

Índice navegable de los samples del front web `geovial-web`. En esta carpeta (`docs/.../11_examples/`) viven los markdown explicativos de cada sample; el código ejecutable correspondiente vive en `/samples/geovial-web/` del repositorio, con correspondencia uno a uno entre cada markdown y su carpeta de código.

Este README es una tabla de contenidos viva; no duplica el contenido de cada markdown explicativo.

## 1. Propósito de la carpeta

- En `docs/.../11_examples/` el lector encuentra, por cada sample, su objetivo, su nivel, sus prerequisites, cómo correrlo, la estructura del código, el output esperado, las variaciones sugeridas y la trazabilidad a CU/ADR/NFR.
- En `/samples/geovial-web/` el lector encuentra el proyecto ejecutable de cada sample, autocontenido, que se clona, se corre en un entorno limpio y se modifica como punto de partida para conocer la app de demostración.
- La progresión es por capacidad demostrada. Para el tipo `web-monolith` el sample principal es el conjunto de datos seed que deja la app de demostración con un estado inicial recorrible, de modo que un evaluador ejercite los flujos administrativos sin tener que poblar el dominio a mano.

## 2. Tabla maestra de samples vigentes

| Sample | Nivel | Tiempo de setup | CU ilustrados | Ubicación |
| --- | --- | --- | --- | --- |
| `ejemplo-01-datos-seed_v1.0.md` | Básico | < 5 min | CU-01, CU-02, CU-03, CU-04, CU-05, CU-06, CU-07, CU-08 | `/samples/geovial-web/01-datos-seed/` |

Cobertura de casos de uso del sample de datos seed: deja sembrados los usuarios de la jerarquía (CU-01, CU-02), un relevamiento con su composición de tramo (CU-03), las asignaciones de agentes (CU-04), los marcadores iniciales con etiquetas (CU-05), la evidencia recorrible sobre mapa y carrusel (CU-06), un conflicto de marcadores pendiente para ejercitar la resolución (CU-07) y un relevamiento en cada estado del ciclo para ejercitar las transiciones y el cierre (CU-08). La carga manual del agente (CU-09), la portabilidad (CU-10) y la configuración de almacenamiento (CU-11) se enumeran como variaciones sugeridas sobre el mismo conjunto seed y quedan disponibles para samples adicionales.

## 3. Convenciones de los samples

- Autocontenidos: cada sample se clona y se ejecuta en un entorno limpio sin depender de otro sample.
- Ejecutables en cinco pasos o menos hasta la primera ejecución exitosa, contra el entorno de demostración del front web.
- Niveles declarados: cada markdown declara su nivel explícito en su sección 2 y justifica qué demuestra.
- Nomenclatura por capacidad demostrada, nunca por entidad del dominio. Todos los markdown llevan el sufijo de versión `_v1.0.md`.
- Trazabilidad obligatoria: cada sample declara en su sección 8 al menos un CU, ADR o NFR que ilustra, con enlace al artefacto fuente de 02 y 05.
- Vocabulario abstracto del front: se habla de datos seed, app de demostración, front web y componente de mapa; el detalle de stack y de componentes concretos vive en la categoría 05 y el código en `/samples`, no en estos markdown.

## 4. Sample omitido: tema custom

La matriz del tipo `web-monolith` (regla 11 §2.3) admite un segundo sample `02-tema-custom`, condicionado a que el proyecto exponga un punto de extensión visual. `geovial-web` no declara punto de extensión visual: en el intake §17 geovial-web P.11 las decisiones pre-tomadas fijan la biblioteca de componentes y el componente de mapa, sin un mecanismo de tematización extensible por el consumidor, y el flag de extensibilidad del front es false. En consecuencia, el sample `02-tema-custom` se OMITE y no existe la carpeta `/samples/geovial-web/02-tema-custom/`. Si en una versión futura se incorpora un punto de extensión visual, se agregará el sample siguiendo la regla 11 §2.2 y se registrará en la tabla maestra de §2.

## 5. Cómo agregar un sample nuevo

1. Elegir el siguiente número correlativo y un slug de capacidad admitido por la regla 11 §3.1 (por ejemplo `datos-seed`, `tema-custom`), nunca por dominio, con sufijo `_v1.0.md`.
2. Redactar el markdown explicativo con las nueve secciones obligatorias siguiendo `SDD2.1D/devs/rules/11_rules_examples.md` §4.2 y la cabecera de §4.1.
3. Crear la carpeta de código correspondiente en `/samples/geovial-web/XX-<slug>/` con su README propio y sus tests de verificación (materialización gobernada por el SOLUTION-INTAKE §16.1).
4. Declarar la trazabilidad a CU/ADR/NFR en la sección 8 y registrar el sample en la tabla maestra de §2 de este README.
5. Verificar contra los criterios de aceptación de la regla 11 §6.

## 6. Vínculo con la arquitectura (05) y la especificación (02)

- Especificación funcional (categoría 02): CU-01 a CU-11 y RN-01 a RN-05 son los flujos y reglas que el conjunto seed deja listos para recorrer; el modelo conceptual (vista de consumo) enumera las entidades que el seed instancia.
- Arquitectura (categoría 05): ADR-01 (render server-side con circuito interactivo), ADR-02 (sin persistencia de dominio en el front: el dato seed es autoritativo del backend y el front lo consume), ADR-03 (autenticación por token bearer custodiado del lado servidor), ADR-04 (separación de capas Presentación / Aplicación de UI / Cliente de API) y ADR-05 (mapeo de errores a feedback). El sample respeta estas decisiones: el seed puebla el backend de demostración y el front lo consume por contrato, sin persistir dominio propio.
- Developer guide (categoría 10): omitida para este proyecto (ADR-06 de 05). El acompañamiento conceptual de la integración con el contrato del backend se encuentra en la categoría 11 del proyecto autoritativo.

## 7. Estructura de `/samples` para este tipo de proyecto

`geovial-web` es de tipo `web-monolith`. La estructura de `/samples/geovial-web/` deriva de la matriz de la regla 11 §2.3 y del SOLUTION-INTAKE §16.1.

| Tipo D8 | Estructura de `/samples/geovial-web/` |
| --- | --- |
| web-monolith | `01-datos-seed/` (`02-tema-custom/` OMITIDO: sin punto de extensión visual) |

La carpeta `01-datos-seed/` es el piso de samples del tipo `web-monolith`. La carpeta `02-tema-custom/` no se materializa por la ausencia de punto de extensión visual (ver §4). Las carpetas base no se renombran por nombres atados al dominio; solo se agregan carpetas extra si se suman samples de capacidades adicionales.

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice inicial de la categoría 11 para geovial-web con la tabla maestra del sample de datos seed (CU-01 a CU-08 ilustrados) y el registro de la omisión del sample de tema custom por ausencia de punto de extensión visual. |
