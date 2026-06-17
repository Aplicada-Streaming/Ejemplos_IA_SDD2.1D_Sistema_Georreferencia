# ADR-01 — Estilo: Clean Architecture en capas para el backend monolítico

**Proyecto:** geovial-api
**Documento:** ADR-01-estilo-clean-architecture-capas_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Estilo

## 1. Contexto

`geovial-api` es el backend monolítico y principal de la solución: concentra la lógica de negocio, la persistencia y la seguridad, y expone el contrato REST que consumen el front web y la app móvil (intake §14, §17.P.2). El dominio tiene invariantes de consistencia inmediata —jerarquía de usuarios (RN-01, RC-03), ciclo del relevamiento (RN-05, RC-04), unicidad de asignación (RC-05), referencia observación-marcador (RC-02), tolerancia a conflictos (RN-03)— que deben poder probarse sin levantar infraestructura. El equipo es de un único desarrollador (intake §2), lo que exige simplicidad operativa. El intake fija el monolito como requisito y descarta de antemano microservicios y servir la UI desde el mismo proceso (§17.P.2). Cubre la totalidad de los CU (CU-01 a CU-22).

## 2. Decisión

Se adopta Clean Architecture en cuatro capas concéntricas con dependencia unidireccional hacia el dominio: Dominio (entidades e invariantes, sin dependencias salientes), Aplicación (casos de uso, orquestación de comandos y consultas, puertos hacia el exterior), Infraestructura (adaptadores de persistencia relacional, almacenamiento e identidad que implementan los puertos) y API (la superficie REST y el middleware transversal). El backend es un único proceso desplegable. No se adopta CQRS pleno; se admite separar servicios de consulta de servicios de comando dentro de la capa de Aplicación cuando la asimetría lectura/escritura lo justifique.

## 3. Estado

Aceptado el 2026-06-15. Decisión pre-tomada en el intake (§17.P.2, §17.P.11): backend monolítico con Clean Architecture en capas.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Clean Architecture en capas (elegida) | Dominio testeable sin infraestructura; un único artefacto desplegable; invariantes centralizadas en Dominio; cumple el requisito de monolito | Disciplina para no filtrar infraestructura hacia adentro; algo de ceremonia en los puertos |
| Microservicios | Deploy independiente por servicio; escalado granular | Descartado por el requisito de monolito; introduce transacciones distribuidas y consistencia eventual donde el dominio pide consistencia inmediata; complejidad operativa inviable para un equipo de un dev |
| Servir la UI desde el mismo proceso del backend | Menos piezas de despliegue | Descartado: el front es un proyecto cliente separado; rompe la frontera cliente-servidor del manifiesto y el versionado independiente del contrato |
| Monolito sin separación de capas | Mínima ceremonia inicial | Impide probar el dominio sin infraestructura; dispersa las invariantes por la capa de transporte; deuda técnica creciente |

## 5. Consecuencias positivas

1. El Dominio se prueba con pruebas unitarias puras, sin base ni red, habilitando el gate de cobertura de aplicación ≥ 80 % (intake §17.P.6).
2. Las invariantes de negocio (RN-01 a RN-07) quedan en un único lugar (Dominio) y no se repiten en la capa de transporte.
3. Un único artefacto desplegable (imagen de contenedor) simplifica el CI/CD y el rollback para un equipo de un desarrollador.
4. Los puertos permiten sustituir el adaptador de almacenamiento (ADR-09) o de identidad sin tocar el Dominio ni la Aplicación.

## 6. Consecuencias negativas y trade-offs

1. Se prioriza la simplicidad operativa de un monolito sobre el escalado independiente por servicio (trade-off declarado en intake §17.P.12).
2. La separación en capas impone una indirección de puertos y adaptadores que agrega código respecto de un acceso directo; se acepta a cambio de la testeabilidad del dominio.
3. Mantener la dependencia hacia adentro exige disciplina de revisión: ningún tipo de Infraestructura debe filtrarse al Dominio.

## 7. Implementación

- La capa de Dominio declara las 12 entidades del modelo conceptual como agregados con sus invariantes; no referencia el almacén ni el transporte.
- La capa de Aplicación expone un caso de uso por CU funcional (CU-01 a CU-17) y declara los puertos (repositorios, puerto de almacenamiento, puerto de identidad, puerto de idempotencia).
- La capa de Infraestructura implementa los puertos: adaptador de persistencia relacional (ADR-02), adaptador de almacenamiento (ADR-09) y adaptador de identidad y token (ADR-03).
- La capa de API expone la superficie REST y el middleware transversal: autorización (ADR-03), errores (ADR-05), paginación (ADR-04), idempotencia (ADR-08) y versionado (ADR-10).
- Convención impuesta: la dependencia siempre apunta hacia el Dominio; la API nunca accede al almacén sin pasar por la Aplicación.

## 8. Métricas de validación

- Cobertura de pruebas: líneas ≥ 80 %, branches ≥ 70 %, capa de aplicación ≥ 80 % en el gate de CI (intake §17.P.6).
- Latencia p95 de lecturas ≤ 300 ms y de escrituras ≤ 500 ms en ambiente equivalente al productivo (intake §17.P.10).
- Cero dependencias del Dominio hacia Infraestructura, verificado por análisis estático de dependencias entre capas.

## 9. Referencias

- NB-01 a NB-07; CU-01 a CU-22; RN-01 a RN-07; RC-01 a RC-06.
- Intake §14 (estilo de la solución), §17.P.2 (estilo del proyecto), §17.P.11 (decisión pre-tomada), §17.P.12 (trade-off).
- ADRs relacionadas: ADR-02 (persistencia), ADR-03 (autenticación), ADR-09 (almacenamiento).
- `arquitectura-solucion_v1.0.md` §2, §3.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de estilo: Clean Architecture en capas para el backend monolítico. Aceptada (pre-tomada en intake §17.P.2, §17.P.11). |
