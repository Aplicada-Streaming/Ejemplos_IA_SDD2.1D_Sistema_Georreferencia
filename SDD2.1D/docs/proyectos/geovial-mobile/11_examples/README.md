# Examples — geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** README.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Sample Engineer (mobile)
**Audiencia:** Developer que arranca, prueba o extiende la app de campo offline-first

Índice navegable de los samples ejecutables de la app de campo `geovial-mobile`, una app móvil offline-first que captura observaciones georreferenciadas en terreno sin conexión y las sincroniza después con su backend. En esta carpeta (`docs/.../11_examples/`) están los markdown explicativos de cada sample; el código ejecutable correspondiente vive en `/samples/geovial-mobile/` del repositorio, con correspondencia uno a uno entre cada markdown y su carpeta de código.

Este README es una tabla de contenidos viva; no duplica el contenido de cada markdown explicativo.

## 1. Propósito de la carpeta

- En `docs/.../11_examples/` el lector encuentra, por cada sample, su objetivo, su nivel, sus prerequisites, cómo correrlo, la estructura del código, el output esperado, variaciones sugeridas y la trazabilidad a CU/ADR/NFR.
- En `/samples/geovial-mobile/` el lector encuentra el proyecto ejecutable de cada sample, autocontenido, que se clona, se corre en un entorno limpio con datos mock y se modifica como punto de partida.
- La progresión es por capacidad demostrada, con nivel implícito declarado en cada sample. El sample 01 (app básica) es de nivel básico; el sample 02 (sync offline) es de nivel intermedio/avanzado. Entre los dos samples se ilustra el grueso de los casos de uso CU-01 a CU-07.

## 2. Tabla maestra de samples vigentes

| Sample | Nivel | Tiempo de setup | CU ilustrados | Ubicación |
| --- | --- | --- | --- | --- |
| `ejemplo-01-app-basica_v1.0.md` | Básico | < 5 min | CU-01, CU-02 | `/samples/geovial-mobile/01-app-basica/` |
| `ejemplo-02-sync-offline_v1.0.md` | Intermedio/Avanzado | 15-25 min | CU-03, CU-04, CU-05, CU-06, CU-07 | `/samples/geovial-mobile/02-sync-offline/` |

Cobertura de casos de uso entre los dos samples: el sample 01 ilustra CU-01 (sesión, deslogueo y relogueo por seguridad del dispositivo) y CU-02 (selección de relevamiento asignado); el sample 02 ilustra CU-03 (crear/mover marcador), CU-04 (captura de foto con resolución de coordenadas), CU-05 (comentarios y etiquetas), CU-06 (trabajo sin conexión y sincronización subir-luego-bajar) y CU-07 (carga manual con radio de agrupación). El conjunto cubre los siete casos de uso CU-01 a CU-07.

## 2.1 Sample omitido

| Sample del tipo | Estado | Motivo |
| --- | --- | --- |
| `03-multiplatform-demo` | Omitido | El único objetivo de plataforma del proyecto es Android (referencia a intake §17 P.9, decisión confirmada por el cliente: sin iOS ni Windows en v1). Sin segunda plataforma, un sample multiplataforma no demostraría capacidad nueva. La omisión está registrada en el intake §16.1 con el mismo criterio que `aplicada-sync`. |

El tipo `mobile-app-maui` declara un piso de tres samples (app básica + sync offline + multiplataforma) en §2.2 de las reglas constructivas de la categoría 11. El sample multiplataforma se omite por target Android único; los dos samples restantes son obligatorios y están presentes.

## 3. Convenciones de los samples

- Autocontenidos: cada sample incluye sus datos mock y su escenario; no depende de otros samples ni de un backend productivo para correr.
- Ejecutables en entorno limpio: cada sample llega a su primera ejecución exitosa en cinco pasos o menos, con su output esperado documentado.
- Niveles declarados: cada markdown declara su nivel explícito en su sección 2 y justifica qué agrega respecto del anterior.
- Nomenclatura por capacidad: los nombres responden a la capacidad demostrada (app básica, sync offline), nunca al dominio de un producto particular. Todos los markdown llevan el sufijo de versión `_v1.0.md`.
- Trazabilidad obligatoria: cada sample declara en su sección 8 al menos un CU, ADR o NFR que ilustra, con enlace al artefacto fuente de 02 y 05.

## 4. Cómo agregar un sample nuevo

1. Elegir el siguiente número correlativo y un slug de progresión por nivel o capacidad (nunca por dominio), con sufijo `_v1.0.md`.
2. Redactar el markdown explicativo con las nueve secciones obligatorias siguiendo `SDD2.1D/devs/rules/11_rules_examples.md` §4.2 y el ejemplo de cabecera de §4.1.
3. Crear la carpeta de código correspondiente en `/samples/geovial-mobile/XX-<slug>/` con datos mock, README propio y tests de verificación (materialización gobernada por el SOLUTION-INTAKE §16.1).
4. Declarar la trazabilidad a CU/ADR/NFR en la sección 8 y registrar el sample en la tabla maestra de §2 de este README.

## 5. Vínculo con la developer guide (10) y la arquitectura (05)

- Developer guide (10): este proyecto omite la developer guide de la categoría 10 (ver `05_arquitectura_tecnica/adrs/ADR-06-omision-developer-guide_v1.0.md`); el acompañamiento conceptual de los samples lo aportan los casos de uso de 02 y los ADR de 05.
- Arquitectura (05): los samples respetan las decisiones de ADR-01 (estilo híbrido con patrón de presentación y diseño offline-first), ADR-02 (almacén local con migraciones versionadas), ADR-03 (sincronización por consumo del motor subir-luego-bajar), ADR-04 (gestión de permisos con degradación) y ADR-05 (autenticación con token seguro y relogueo por seguridad del dispositivo). El pipeline paso a paso vive en `05_arquitectura_tecnica/flujo-ejecucion_v1.0.md`.
- Especificación funcional (02): cada sample ejercita uno o más de los casos de uso CU-01 a CU-07 y verifica las reglas de negocio RN-01 a RN-05.

## 6. Tabla tipo de proyecto vs estructura de `/samples`

| Tipo D8 | Estructura de `/samples/geovial-mobile/` |
| --- | --- |
| mobile-app-maui | `01-app-basica/`, `02-sync-offline/` (el sample `03-multiplatform-demo` se omite por target Android único) |
| (resto) | Ver §2.3 de `SDD2.1D/devs/rules/11_rules_examples.md`. |

`geovial-mobile` es de tipo `mobile-app-maui`: el piso de tres samples de §2.2 de las reglas constructivas se reduce a dos por la omisión del sample multiplataforma (intake §17 P.9, §16.1). La estructura de carpetas refleja la matriz §2.3 para `mobile-app-maui`, namespaciada bajo `geovial-mobile/` por ser una solución multiproyecto.

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice inicial de la categoría 11 para geovial-mobile con la tabla maestra de los dos samples (app básica, sync offline), el registro de la omisión del sample multiplataforma por target Android único y la cobertura de CU-01 a CU-07. |
