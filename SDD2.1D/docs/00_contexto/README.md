# Contexto del producto — GeoVial (solución)

Esta carpeta reúne los documentos de contexto de nivel solución de GeoVial: el porqué del producto, su alcance, su roadmap de fases y la compatibilidad de plataformas. Son el inicio de la cadena de trazabilidad y alimentan las categorías 01 a 11.

**Proyecto:** GeoVial (solución)
**Fecha:** 2026-06-15
**Autor:** Product Manager + API Product Owner

## Documentos de la sección

| Orden de lectura | Documento | Propósito | Estado |
|---|---|---|---|
| 1 | `vision-producto_v1.0.md` | Por qué existe GeoVial, audiencia y stakeholders, propuesta de valor, visión a 3 años, objetivos SMART, métricas de éxito, restricciones, riesgos y glosario del dominio | Propuesto |
| 2 | `alcance-proyecto_v1.0.md` | Qué entra y qué no entra en la primera versión, capacidades incluidas, exclusiones justificadas, supuestos, restricciones y criterios de aceptación | Propuesto |
| 3 | `roadmap-producto_v1.0.md` | Fases de construcción por cortes verticales, dependencias entre fases y criterios verificables de transición | Propuesto |
| 4 | `compatibilidad-plataformas_v1.0.md` | Plataformas target soportadas, versiones mínimas propuestas y plataformas fuera de v1 | Propuesto |

Orden de lectura sugerido: visión, alcance, roadmap y compatibilidad. La visión fija el porqué y el vocabulario; el alcance delimita qué se construye; el roadmap ordena la construcción; la compatibilidad acota las plataformas.

## Stakeholders

| Rol | Nombre o cargo | Categoría |
|---|---|---|
| Dueño del problema / aprobador del intake | Vialidad provincial | Propietario |
| Equipo de desarrollo | Departamento de desarrollo de software (1 desarrollador) | Implementador |
| Usuario raíz | Rol del sistema | Beneficiario |
| Jefe general | Rol del sistema | Beneficiario |
| Jefe de área | Rol del sistema | Beneficiario |
| Agente de campo | Rol del sistema | Beneficiario |

## Nota de omisión

`acuerdo-equipo_v1.0.md` se omite. La solución la construye un único desarrollador (equipo_n = 1), por lo que no hay equipo cuya cadencia, ceremonias, branching y SLA de respuesta haya que acordar. La omisión se ampara en 00_rules_contexto §2.1 (el documento se omite para proyectos de un solo desarrollador) y §1.3 (en proyectos de un desarrollador sin cliente externo, el rol de Product Manager actúa también como Analista de Negocio y el acuerdo de equipo queda omitido). Si el equipo crece a más de dos personas, este documento deberá generarse.
