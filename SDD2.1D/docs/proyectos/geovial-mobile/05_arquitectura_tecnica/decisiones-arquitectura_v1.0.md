# Decisiones de arquitectura — geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** decisiones-arquitectura_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto Móvil

## 1. Objetivo

Índice navegable de los ADR de `geovial-mobile`. El cuerpo de cada decisión vive en su archivo individual bajo `adrs/`. Cada ADR es inmutable una vez aceptada: si una decisión evoluciona, se crea un ADR nuevo con identificador siguiente y el anterior pasa a estado `Superado por ADR-YY` sin reescribirse.

## 2. Índice de ADRs

| ADR | Título | Categoría | Estado | Fecha |
| --- | --- | --- | --- | --- |
| ADR-01 | estilo-app-hibrida-mvvm-offline-first | Estilo | Aceptado | 2026-06-15 |
| ADR-02 | persistencia-almacen-local-migraciones | Persistencia | Aceptado | 2026-06-15 |
| ADR-03 | sincronizacion-motor-subir-luego-bajar | Comunicación | Aceptado | 2026-06-15 |
| ADR-04 | gestion-permisos-degradacion | Seguridad | Aceptado | 2026-06-15 |
| ADR-05 | autenticacion-token-seguro-relogueo-dispositivo | Seguridad | Aceptado | 2026-06-15 |
| ADR-06 | omision-developer-guide | Despliegue | Aceptado | 2026-06-15 |

## 3. Cobertura del mínimo del tipo

El tipo `mobile-app-maui` exige un mínimo de cuatro ADR (regla 05 §2.2): estilo, persistencia local, sincronización y gestión de permisos. Se cumplen con ADR-01, ADR-02, ADR-03 y ADR-04, y se agrega ADR-05 (autenticación con almacenamiento seguro del token y relogueo por seguridad del dispositivo), derivada de la decisión pre-tomada en intake §17.P.5, §17.P.11. El mínimo es piso, no techo. ADR-06 se suma por encima de ese mínimo: registra la omisión de la categoría 10 (developer guide), opcional para este tipo de proyecto, y no corresponde a ninguna de las cuatro categorías exigidas.

## 4. Trazabilidad a CU y RN

| ADR | CU que motiva | RN que la motiva | NFR relacionado |
| --- | --- | --- | --- |
| ADR-01 | CU-01 a CU-07 | RN-05 | Captura offline; arranque ≤ 3 s |
| ADR-02 | CU-02 a CU-07 | RN-05, RN-02 | Cola ≥ 1000 cambios |
| ADR-03 | CU-06, CU-02 | RN-02, RN-03, RN-05 | Ciclo 100 cambios ≤ 30 s; reanudación sin pérdida |
| ADR-04 | CU-03, CU-04, CU-07, CU-01 | RN-01, RN-05 | Captura offline; sin coordenada inventada |
| ADR-05 | CU-01, CU-06 | RN-04 | Arranque ≤ 3 s |
| ADR-06 | No aplica (decisión de proceso documental) | No aplica | No aplica |

## 5. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice inicial de los 5 ADR de geovial-mobile: estilo, persistencia local, sincronización, gestión de permisos y autenticación. Todos Aceptados (pre-tomados en intake §17.P.11) salvo gestión de permisos, derivada de los CU y RN. |
| 1.0 | 2026-06-15 | Se incorpora ADR-06 de omisión de la categoría 10. |
