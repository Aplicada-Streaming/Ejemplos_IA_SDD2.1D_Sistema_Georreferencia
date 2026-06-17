# 05 — Arquitectura técnica — geovial-web

**Proyecto:** geovial-web
**Tipo D8:** web-monolith (front web de la solución GeoVial)
**Estado:** Propuesto (ADR-01 a ADR-03 Aceptados; ADR-04 y ADR-05 Propuestos)
**Fecha:** 2026-06-15
**Autor:** Arquitecto Senior

Punto de entrada navegable de la arquitectura técnica de `geovial-web`, el front web de render server-side con circuito interactivo persistente que crea, recolecta y revisa relevamientos sobre un mapa, consume por contrato la API REST de `geovial-api` y no posee persistencia de dominio propia.

## Documento maestro

- [`arquitectura-solucion_v1.0.md`](arquitectura-solucion_v1.0.md) — Estilo (render server-side con circuito interactivo persistente y separación de capas en el cliente), cuatro vistas mínimas (lógica, procesos, despliegue, datos), cross-cutting, NFR con métricas numéricas, riesgos y trazabilidad. La vista de datos referencia que el estado de dominio es de `geovial-api` y que el front solo maneja estado de UI/sesión efímero.

## Decisiones de arquitectura (ADRs)

- [`decisiones-arquitectura_v1.0.md`](decisiones-arquitectura_v1.0.md) — Índice navegable de los ADRs.

| ADR | Título | Categoría | Estado |
| --- | --- | --- | --- |
| [ADR-01](adrs/ADR-01-estilo-render-server-side-circuito-interactivo_v1.0.md) | Estilo: render server-side con circuito interactivo persistente y separación de capas | Estilo | Aceptado |
| [ADR-02](adrs/ADR-02-sin-persistencia-dominio-estado-efimero_v1.0.md) | Sin persistencia de dominio en el front; estado de UI/sesión efímero | Persistencia | Aceptado |
| [ADR-03](adrs/ADR-03-autenticacion-token-bearer-lado-servidor_v1.0.md) | Autenticación por credenciales con token bearer custodiado del lado servidor del circuito | Seguridad | Aceptado |
| [ADR-04](adrs/ADR-04-separacion-capas-presentacion-aplicacion-cliente-api_v1.0.md) | Separación de capas: Presentación / Aplicación de UI / Cliente de API | Estilo | Propuesto |
| [ADR-05](adrs/ADR-05-manejo-errores-mapeo-problem-json-a-feedback_v1.0.md) | Manejo de errores: mapeo de problem+json de la API a feedback de UI | Comunicación | Propuesto |

El mínimo del tipo `web-monolith` (5 ADRs: estilo, persistencia, autenticación, separación de capas, manejo de errores) se cumple exactamente con ADR-01 a ADR-05.

## NFR vigentes (intake §17 geovial-web P.10)

| NFR | Objetivo |
| --- | --- |
| Latencia de interacción p95 | ≤ 200 ms sobre el circuito en red estable |
| Circuitos interactivos concurrentes | ≥ 50 en el ambiente de referencia |
| Disponibilidad mensual | ≥ 99,5 % |
| Custodia del token | 0 exposiciones del token bearer al navegador |
| Cobertura de pruebas (gate de CI) | Líneas ≥ 80 %, branches ≥ 70 %, presentación ≥ 60 % |

La observabilidad no es crítica en esta versión (`tiene_observabilidad_critica=false`): sin SLO de 99,9 % ni objetivo de latencia p99 numérico.

## Notas de alcance (artefactos omitidos)

- No se produce `modelo-datos-logico_v1.0.md`: el front no tiene persistencia de dominio propia (intake §17.P.4, `tiene_persistencia=false`). El estado de dominio y su modelo lógico autoritativo viven en `geovial-api` (`modelo-datos-logico_v1.0.md` de ese proyecto). La omisión queda registrada como decisión en [ADR-02](adrs/ADR-02-sin-persistencia-dominio-estado-efimero_v1.0.md).
- No se produce `contratos-<area>_v1.0.md`: el front no expone una API externa; consume el contrato REST de `geovial-api` (`contratos-rest_v1.0.md` de ese proyecto).
- No se produce `flujo-ejecucion_v1.0.md`: el front no tiene orquestación compleja (cada interacción es una o pocas llamadas al contrato REST; ver arquitectura §4).
- No se produce `extensibilidad_v1.0.md`: `tiene_extensibilidad=false`.
- La vista de solución y los contratos inter-proyecto viven en `_solucion/` y aquí solo se referencian. Contrato consumido: `geovial-api` (`contratos-rest_v1.0.md`), arista del manifiesto §13 (`geovial-web → geovial-api`).

## Trazabilidad

- Upstream: NB-01, NB-02, NB-05, NB-06, NB-07; once CU (CU-01 a CU-11) y cinco RN de presentación (RN-01 a RN-05) de 02; experiencia de uso y wireframes de 03.
- Dominio autoritativo: `geovial-api` (modelo lógico y contrato REST). La correspondencia de consumo CU del front → recurso de la API está en 02 §7 y en la §10 del documento maestro.
- Downstream: 06 (US US-01 a US-25), 08 (tests de componente, snapshot, accesibilidad e integración a través de la API) y 09 (despliegue del contenedor de front, afinidad de sesión y escalado).

## Convenciones

- Nomenclatura `<nombre>_v1.0.md` y `ADR-XX-<kebab-lowercase>_v1.0.md` con guion bajo antes de `v`. UTF-8 LF, fechas YYYY-MM-DD, sin emojis.
- Disciplina de neutralidad de stack (D7): las decisiones se expresan por patrón o mecanismo abstracto (render server-side, circuito interactivo persistente, componente de mapa, token bearer, biblioteca de componentes de UI), sin nombrar stacks ni productos concretos. Los NFR numéricos sí se declaran.
- Política de versionado (05 §3.6): una sola versión vigente por nombre lógico; los ADRs no se versionan en el mismo archivo (un ADR superado se reemplaza por uno nuevo y queda `Superado por ADR-YY`). Hoy no hay versiones superadas.

## Estructura de la sección

```text
05_arquitectura_tecnica/
├── README.md                                                          # este archivo
├── arquitectura-solucion_v1.0.md                                      # documento maestro (§1 a §10, 4 vistas)
├── decisiones-arquitectura_v1.0.md                                    # índice de ADRs
└── adrs/
    ├── ADR-01-estilo-render-server-side-circuito-interactivo_v1.0.md
    ├── ADR-02-sin-persistencia-dominio-estado-efimero_v1.0.md
    ├── ADR-03-autenticacion-token-bearer-lado-servidor_v1.0.md
    ├── ADR-04-separacion-capas-presentacion-aplicacion-cliente-api_v1.0.md
    └── ADR-05-manejo-errores-mapeo-problem-json-a-feedback_v1.0.md
```

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 05 de geovial-web: documento maestro, cinco ADRs (mínimo del tipo web-monolith), NFR vigentes, notas de alcance (omisión de modelo lógico, contratos, flujo y extensibilidad) y trazabilidad. |
