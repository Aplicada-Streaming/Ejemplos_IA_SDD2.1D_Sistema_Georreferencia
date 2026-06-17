# ADR-03 — Estrategia de versionado de la superficie pública

**Proyecto:** aplicada-sync
**Documento:** ADR-03-versionado-superficie-publica_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Despliegue

## 1. Contexto

`aplicada-sync` es redistribuible y se reutiliza fuera de la solución: tiene consumidores que no controla. Cualquier cambio que altere el contrato del ciclo de vida (firmas de la superficie pública, semántica del orden subir-luego-bajar, contrato de los identificadores de cambio, conjunto de estados de la sesión o códigos de error) puede romper a esos consumidores en silencio. Lo motivan la especificación funcional §8 (compatibilidad de versión pública), las secciones §17 de cada CU (CU-01 a CU-06) y el intake §17 P.7 del proyecto aplicada-sync, que adopta versionado semántico para el paquete.

## 2. Decisión

Se adopta versionado semántico de la superficie pública con compatibilidad hacia atrás explícita. Todo cambio incompatible del contrato descrito por los CU obliga a incrementar la versión mayor; las aclaraciones y agregados compatibles avanzan la versión menor; las correcciones que no alteran el contrato avanzan la versión de parche. La superficie pública versionada es la capa Abstractions y el contrato del ciclo de vida, no la implementación interna.

## 3. Estado

Aceptado el 2026-06-15. Pre-tomada por el intake §17 P.7 (versionado semántico y convención de commits) y por la especificación funcional §8.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Versionado semántico con compatibilidad hacia atrás (elegida) | Contrato predecible para consumidores externos; el incremento mayor señala el cambio incompatible | Disciplina de clasificar cada cambio; gestión de deprecaciones |
| Versión continua sin garantía de compatibilidad | Máxima libertad de cambio | Rompe consumidores en silencio; inviable para un paquete redistribuible |
| Compatibilidad perpetua sin versión mayor | Nunca rompe consumidores | Congela el diseño; impide corregir contratos defectuosos |

## 5. Consecuencias positivas

- Los consumidores externos pueden adoptar versiones nuevas con una expectativa clara de compatibilidad.
- El incremento de versión mayor documenta de forma inequívoca un cambio de contrato, alineado con el changelog del portal de developers (categoría 03).
- Habilita una matriz de compatibilidad de la superficie pública publicable.

## 6. Consecuencias negativas y trade-offs

- Se acepta la disciplina de clasificar cada cambio como mayor, menor o parche y de mantener un período de deprecación antes de remover.
- Se acepta que corregir un contrato defectuoso pueda requerir una versión mayor en lugar de un parche silencioso.
- Coexistir con versiones mayores anteriores puede exigir mantenimiento paralelo durante la deprecación.

## 7. Implementación

La política de compatibilidad y deprecación se detalla en `contratos-abstractions_v1.0.md` §6. Convención: el contrato del ciclo de vida (CU-01 a CU-06), el conjunto de estados de la sesión, la garantía de orden (RN-01) y los códigos de error son superficie pública; alterarlos es cambio mayor. La implementación interna (componentes, esquema físico de metadatos) no es superficie pública y puede cambiar sin versión mayor mientras preserve el contrato.

## 8. Métricas de validación

- 100 % de los cambios incompatibles publicados se corresponden con un incremento de versión mayor (auditoría del changelog contra la matriz de compatibilidad).
- Cero remociones de elementos del contrato sin período de deprecación previo documentado.
- Verificación post-publicación: el paquete publicado se restaura en un proyecto limpio y el quick-start reproduce el contrato (alineado con el intake §17 P.8).

## 9. Referencias

- Especificación funcional §8; secciones §17 de CU-01 a CU-06.
- ADR-01 (qué es superficie pública), ADR-02 (contratos de extensión versionados).
- SOLUTION-INTAKE §17 P.7 y P.8 (aplicada-sync).
- `contratos-abstractions_v1.0.md` §6.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión inicial de estrategia de versionado semántico de la superficie pública con compatibilidad hacia atrás. |
