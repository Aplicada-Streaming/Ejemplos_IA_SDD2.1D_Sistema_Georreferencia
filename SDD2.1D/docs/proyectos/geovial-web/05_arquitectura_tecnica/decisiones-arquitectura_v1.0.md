# Decisiones de arquitectura — geovial-web

**Proyecto:** geovial-web
**Documento:** decisiones-arquitectura_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto Senior

## 1. Objetivo

Índice navegable de los Architecture Decision Records (ADRs) de `geovial-web`. Cada ADR vive en un archivo individual bajo `adrs/` (regla 05 §3.3); este documento no contiene el cuerpo de las decisiones, solo su identificador, título, categoría, estado y fecha. Una decisión aceptada es inmutable: si evoluciona, se crea una ADR nueva y la anterior pasa a `Superado por ADR-YY`.

## 2. Índice de ADRs

| ADR | Título | Categoría | Estado | Fecha |
| --- | --- | --- | --- | --- |
| [ADR-01](adrs/ADR-01-estilo-render-server-side-circuito-interactivo_v1.0.md) | Estilo: render server-side con circuito interactivo persistente y separación de capas en el cliente | Estilo | Aceptado | 2026-06-15 |
| [ADR-02](adrs/ADR-02-sin-persistencia-dominio-estado-efimero_v1.0.md) | Sin persistencia de dominio en el front; estado de UI/sesión efímero | Persistencia | Aceptado | 2026-06-15 |
| [ADR-03](adrs/ADR-03-autenticacion-token-bearer-lado-servidor_v1.0.md) | Autenticación por credenciales con token bearer custodiado del lado servidor del circuito | Seguridad | Aceptado | 2026-06-15 |
| [ADR-04](adrs/ADR-04-separacion-capas-presentacion-aplicacion-cliente-api_v1.0.md) | Separación de capas en el cliente: Presentación / Aplicación de UI / Cliente de API | Estilo | Propuesto | 2026-06-15 |
| [ADR-05](adrs/ADR-05-manejo-errores-mapeo-problem-json-a-feedback_v1.0.md) | Manejo de errores: mapeo de problem+json de la API a feedback de UI | Comunicación | Propuesto | 2026-06-15 |
| [ADR-06](adrs/ADR-06-omision-developer-guide_v1.0.md) | Omisión de la categoría 10 (developer guide): la documentación de consumo colapsa en el README | Despliegue | Aceptado | 2026-06-15 |

## 3. Cobertura del mínimo del tipo

El tipo `web-monolith` exige un mínimo de cinco ADRs: estilo, persistencia, autenticación, separación de capas y manejo de errores (regla 05 §2.2). El mínimo se cumple exactamente: ADR-01 (estilo), ADR-02 (persistencia), ADR-03 (autenticación), ADR-04 (separación de capas) y ADR-05 (manejo de errores). ADR-01, ADR-02 y ADR-03 están `Aceptado` por ser decisiones pre-tomadas en el intake (§17 geovial-web P.2, P.4, P.5, P.11); ADR-04 y ADR-05 están `Propuesto` por ser defaults del arquitecto sobre ejes ratificables del intake (§17.P.2, §17.P.3), a confirmar en Sprint 0. ADR-06 se suma por encima del mínimo del tipo: registra la omisión de la categoría 10 (developer guide), opcional para este tipo de proyecto, y no forma parte de las cinco categorías exigidas.

## 4. Notas de inmutabilidad y versionado

- Los ADRs no se versionan en el mismo archivo: una decisión que evoluciona se reemplaza por una ADR nueva con identificador siguiente y la anterior pasa a `Superado por ADR-YY`, ambas coexistiendo en `adrs/` (regla 05 §3.6).
- A la fecha no hay ADRs superados ni rechazados.

## 5. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice inicial de los cinco ADRs de geovial-web (estilo, persistencia, autenticación, separación de capas, manejo de errores), cubriendo el mínimo del tipo web-monolith. |
| 1.0 | 2026-06-15 | Se incorpora ADR-06 de omisión de la categoría 10. |
