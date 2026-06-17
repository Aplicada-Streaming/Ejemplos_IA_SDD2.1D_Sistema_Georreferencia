# ADR-06 — Omisión de la categoría 10 (developer guide): la documentación de consumo colapsa en el README

**Proyecto:** geovial-mobile
**Documento:** ADR-06-omision-developer-guide_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto Móvil
**Categoría:** Despliegue

## 1. Contexto

La cadena de generación incluye una categoría 10 (developer guide) cuyo destinatario es un desarrollador externo o integrador que consume el proyecto desde afuera. Para el tipo de este proyecto esa categoría es opcional, no obligatoria: la audiencia de consumo es interna al equipo y a la operación, no existe un desarrollador integrador externo que necesite un onboarding propio, la app no publica un kit de desarrollo ni un canal de extensión hacia terceros y no expone un portal de developers (la bandera de portal de developers está en false en el manifiesto de la solución). La app es un cliente de captura en terreno que consume el contrato del backend y el contrato de sincronización; no ofrece a terceros una superficie de integración propia. En ausencia de esa superficie pública, la documentación orientada al consumidor se reduce a lo que un miembro del equipo o de operación necesita para instalar y operar la app, alcance que se cubre con el README del repositorio. La regla de la categoría 10 establece que cuando la categoría es opcional y el equipo decide omitirla, la decisión se registra en una ADR con su justificación; la orquestación, para la fase de developer guide del proyecto, indica que si la categoría se omite se debe registrar un ADR de omisión en la categoría 05 del proyecto. Esta ADR materializa ese registro.

## 2. Decisión

Se decide no generar la categoría 10 (developer guide) para este proyecto. La documentación orientada al consumidor no se produce como carpeta de developer guide independiente: colapsa en el README del repositorio de la solución (producido por el rol de README raíz al cierre del bucle de proyectos) y en el README de cada sección de documentación del proyecto. El README raíz presenta la solución, su jerarquía y la tabla de proyectos con su rol y dependencias, y enlaza la documentación de cada proyecto; el README de la sección de arquitectura sirve de punto de entrada navegable al diseño interno. No se materializan conceptos, onboarding, guía de integración, referencia, troubleshooting ni glosario como artefactos separados, porque no hay audiencia integradora externa que los consuma.

## 3. Estado

Aceptado el 2026-06-15. La omisión es una decisión firme del arquitecto fundada en el tipo del proyecto y en las banderas del manifiesto (audiencia interna, sin superficie pública de integración, sin portal de developers). Si en una evolución futura la app expusiera una superficie de integración hacia terceros (un kit de desarrollo, un canal de extensión o un portal de developers), esta decisión dejaría de aplicar: se crearía una ADR nueva con identificador siguiente que habilite la categoría 10 y esta pasaría a `Superado por ADR-YY` sin reescribirse.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Omitir la categoría 10 y colapsar el consumo en el README (elegida) | Coherente con audiencia interna y sin superficie pública; evita documentación sin lector; punto de entrada único y mantenido en el README | Si en el futuro aparece un integrador externo habrá que habilitar la categoría 10 mediante una ADR nueva |
| Generar la categoría 10 completa (conceptos, onboarding, integración, referencia, troubleshooting, glosario) | Cobertura máxima de documentación de consumo | Rechazada: no hay audiencia integradora externa; infla la documentación sin valor y agrega artefactos a mantener que nadie consume; contradice el carácter opcional de la categoría para este tipo |
| Generar la categoría 10 parcial (solo algunos artefactos) | Documentación intermedia | Rechazada para esta versión: aun el subconjunto mínimo presupone un consumidor externo inexistente; duplicaría en otra carpeta lo que ya cubre el README sin agregar destinatario |

## 5. Consecuencias positivas

1. La documentación de consumo tiene un único punto de entrada vigente (el README del repositorio y el README de cada sección), sin una carpeta paralela que se desactualice.
2. Se evita producir y mantener artefactos de developer guide sin lector, alineando el esfuerzo de documentación con la audiencia real (equipo interno y operación).
3. La omisión queda registrada y trazable como decisión arquitectónica formal, no como un vacío silencioso de la cadena.
4. La puerta queda abierta y explícita: si aparece una audiencia integradora externa, la reactivación de la categoría es una ADR nueva, sin deshacer trabajo previo.

## 6. Consecuencias negativas y trade-offs

1. Si en el futuro surge un consumidor externo, no existirá una developer guide preparada y habrá que generarla desde cero mediante una ADR que habilite la categoría. Se acepta porque hoy no hay tal consumidor y anticiparlo produciría documentación sin lector.
2. El consumo interno depende de que el README del repositorio y los README de sección cubran instalación y operación con suficiencia. Se acepta y se mitiga con la métrica de validación de §8, que exige que el README de la solución cubra el consumo interno.
3. No hay un glosario ni una referencia de consumo formal independiente. Se acepta porque el vocabulario del consumidor coincide con el del equipo interno, ya presente en la documentación funcional y de arquitectura.

## 7. Implementación

- No se crea la carpeta de developer guide del proyecto ni sus artefactos (conceptos, onboarding, integración, referencia, troubleshooting, glosario).
- La documentación orientada al consumidor interno se concentra en el README del repositorio de la solución, que presenta la solución, la jerarquía, la tabla de proyectos y los enlaces a la documentación de cada proyecto, y en el README de cada sección de documentación del proyecto.
- El índice de ADRs del proyecto referencia esta decisión para que la omisión sea visible y auditable.
- Convención impuesta: ningún artefacto del proyecto asume la existencia de una developer guide; toda guía de consumo se enlaza desde el README. Si la categoría se reactivara, se haría mediante una ADR nueva que la habilite, no editando esta.

## 8. Métricas de validación

- La decisión se valida con que el README de la solución y el README de la sección cubran el consumo interno: instalación, ejecución y operación de la app por un miembro del equipo o de operación sin necesidad de una developer guide separada.
- Verificación de ausencia controlada: no existe carpeta de developer guide del proyecto y la omisión está registrada en el índice de ADRs, de modo que la auditoría de la fase confirma una omisión deliberada y no un faltante.
- Verificación de la condición de reapertura: a la fecha el manifiesto declara la bandera de portal de developers en false y no hay superficie pública de integración; mientras esa condición se mantenga, la omisión sigue siendo válida.

## 9. Referencias

- Orquestación de la solución, fase de developer guide del proyecto: la generación de la categoría 10 depende del tipo del proyecto y de la bandera de portal de developers; si se omite, se registra un ADR de omisión en la categoría 05 del proyecto.
- Reglas de la categoría 10, §1.2 y §2.2: la categoría es opcional para este tipo; la audiencia es el equipo interno y la operación, y se materializa solo si la app expone una superficie pública de integración.
- Manifiesto de la solución: bandera de portal de developers en false para este proyecto; ausencia de superficie pública de integración.
- `decisiones-arquitectura_v1.0.md` (índice de ADRs del proyecto); README raíz de la solución y README de la sección de arquitectura.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de omitir la categoría 10 (developer guide) por ser opcional para el tipo del proyecto, con audiencia interna, sin superficie pública de integración y con la bandera de portal de developers en false. La documentación de consumo colapsa en el README del repositorio y en el README de cada sección. Aceptada. |
