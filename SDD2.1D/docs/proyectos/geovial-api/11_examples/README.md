# Examples — geovial-api

Índice navegable de los samples ejecutables de `geovial-api`. Esta carpeta documenta; el código vive en `/samples/geovial-api/`.

## 1. Propósito de la carpeta

En `/docs/proyectos/geovial-api/11_examples/` viven los markdown explicativos de cada sample: objetivo, prerequisitos, cómo correrlo, estructura del código, output esperado, variaciones, trazabilidad y control de cambios. En `/samples/geovial-api/` vive el código ejecutable correspondiente, con correspondencia 1:1 entre cada `ejemplo-XX-<capacidad>_v1.0.md` y su carpeta `0X-<capacidad>/`.

Los tres samples ilustran de forma progresiva cómo un consumidor (el front web o la app móvil) integra el contrato REST de geovial-api: primero el camino feliz mínimo con un cliente HTTP de línea de comandos, luego una colección de pruebas reproducible que recorre los flujos principales, y por último un cliente tipado generado a partir del contrato OpenAPI que cierra un ciclo de sincronización de punta a punta. Cada sample materializa una guía de la categoría 10 y respeta el contrato de la categoría 05.

## 2. Tabla maestra de samples

| Sample | Nivel | Tiempo de setup | CU ilustrados | Ubicación |
| --- | --- | --- | --- | --- |
| `ejemplo-01-cliente-http-basico_v1.0.md` | Básico | < 5 min | CU-03, CU-04, CU-02, CU-19, CU-22 | `/samples/geovial-api/01-cliente-http-basico/` |
| `ejemplo-02-postman-collection_v1.0.md` | Intermedio | 10-15 min | CU-03, CU-04, CU-05, CU-20, CU-19, CU-02, CU-07, CU-18, CU-21 | `/samples/geovial-api/02-postman-collection/` |
| `ejemplo-03-sdk-tipado-generado_v1.0.md` | Avanzado | 20-30 min | CU-10, CU-11, CU-22, CU-07, CU-08, CU-12, CU-13, CU-21 | `/samples/geovial-api/03-sdk-tipado-generado/` |

Entre los tres samples se ilustra el grueso de CU-01 a CU-22: autenticación y sesión (CU-03), usuarios y agentes (CU-01, CU-02), relevamientos y ciclo (CU-04, CU-06, CU-12, CU-14), asignaciones (CU-05), marcadores y observaciones (CU-07, CU-08, CU-09), sincronización (CU-10, CU-11), conflictos (CU-13), y los transversales (CU-18, CU-19, CU-20, CU-21, CU-22). La portabilidad (CU-15, CU-16) y la configuración de almacenamiento (CU-17) se mencionan como variaciones sugeridas y quedan disponibles para samples adicionales.

## 3. Convenciones de los samples

- Autocontenidos: cada sample se clona y se ejecuta en un entorno limpio sin depender de otro sample.
- Ejecutables en cinco pasos o menos hasta la primera ejecución exitosa, contra el entorno de prueba de geovial-api.
- Nivel declarado explícitamente en la sección 2 de cada markdown (básico, intermedio o avanzado).
- Trazabilidad obligatoria: cada markdown declara en su sección 8 al menos un CU, ADR o NFR que ilustra.
- Nomenclatura por capacidad demostrada, nunca por entidad del dominio. Sufijo de versión `_v1.0.md` en todos los markdown explicativos.
- Vocabulario REST genérico: la autenticación se describe como enviar credenciales y recibir un token bearer; el código de aplicación que materializa el cliente vive en `/samples` y no se nombra en estos markdown.

## 4. Cómo agregar un sample nuevo

1. Elegir un slug de capacidad admitido por la regla 11 §3.1 (por ejemplo `cliente-http-basico`, `postman-collection`, `sdk-tipado-generado`) con número correlativo y sufijo `_v1.0.md`.
2. Redactar el markdown explicativo con las nueve secciones obligatorias de la regla 11 §4.2.
3. Crear la carpeta ejecutable espejo en `/samples/geovial-api/0X-<capacidad>/` con su README propio y sus tests de verificación.
4. Declarar el sample en la tabla maestra de la sección 2 de este README, con nivel, tiempo de setup, CU ilustrados y ubicación.
5. Verificar contra los criterios de aceptación de la regla 11 §6.

El template y las reglas de redacción están en `SDD2.1D/devs/rules/11_rules_examples.md` (§4 y §6).

## 5. Vínculo con la developer guide y la arquitectura

- Developer guide (categoría 10): `guia-onboarding-developer_v1.0.md` (tutorial que el sample 01 y el 02 materializan), `guia-integracion-cliente-http_v1.0.md` (how-to que orquesta el ciclo que el sample 03 cierra), `referencia-api_v1.0.md` (firma de cada operación y catálogo de errores), `conceptos-fundamentales_v1.0.md` y `troubleshooting_v1.0.md`.
- Arquitectura (categoría 05): `contratos-rest_v1.0.md` (contrato OpenAPI lógico de 35 operaciones que los tres samples respetan), ADR-03 (autenticación con token bearer), ADR-04 (paginación), ADR-05 (errores problem+json), ADR-07 (orden de sincronización subir-antes-de-bajar), ADR-08 (idempotencia) y ADR-10 (versionado por URI).
- Especificación funcional (categoría 02): CU-01 a CU-22.

## 6. Estructura de `/samples` para un proyecto rest-api

| Tipo D8 | Estructura de `/samples/geovial-api/` |
| --- | --- |
| rest-api | `01-cliente-http-basico/`, `02-postman-collection/`, `03-sdk-tipado-generado/` |

Esta estructura deriva de la matriz de la regla 11 §2.3 para el tipo `rest-api` y es vinculante: las carpetas base no se renombran por nombres atados al dominio; solo se agregan carpetas extra si se suman samples de capacidades adicionales.
