# Estrategia de calidad — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** estrategia-calidad_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (AG-08), variante QA + SDET Library

## 1. Definición de calidad para el proyecto

`aplicada-sync` es una librería redistribuible y agnóstica del dominio: no tiene interfaz de usuario, ni ambiente desplegable propio, ni servicio en ejecución. Su calidad se juzga exclusivamente por su contrato de entrada-salida y por las garantías de su motor de sincronización. El sistema tiene calidad cuando, para cualquier integrador, la superficie pública (capa Abstractions) cumple sus invariantes sin excepción: el ciclo sube siempre antes de bajar (RN-01), un mismo cambio se aplica una sola vez sin importar cuántos reintentos o reanudaciones ocurran (RN-02), un estado en conflicto reportado por el backend nunca aborta ni bloquea el ciclo (RN-03), y un corte de conectividad durante la subida no produce pérdida ni duplicación de datos al reanudar.

El perfil de riesgo está concentrado en la integridad de datos y en la estabilidad del contrato publicable. Por ser redistribuible (`redistribuible: true`), un cambio incompatible silencioso rompe a consumidores que el proyecto no controla; por ser un motor que opera en trabajo de campo sin conexión, un defecto de idempotencia o de orden produce pérdida o duplicación invisible hasta que el dato corrupto llega al backend. La estrategia prioriza, por lo tanto, la fiabilidad funcional del motor y la compatibilidad de la superficie pública por encima de cualquier otro atributo.

## 2. Atributos de calidad priorizados (ISO/IEC 25010)

Los atributos se priorizan según el perfil de riesgo de una librería de sincronización sin UI. Las métricas numéricas provienen del intake §17 P.10 y de la tabla de quality attributes de `arquitectura-solucion_v1.0.md` §8.

| Atributo ISO 25010 | Prioridad | Justificación para esta librería | Métrica numérica y NFR de origen |
| --- | --- | --- | --- |
| Funcionalidad (corrección, completitud) | Crítica | El valor del motor es que sus invariantes se cumplan siempre; un fallo de orden o de idempotencia corrompe datos de campo | Orden subir-antes-de-bajar: 0 bajadas mientras quedan pendientes confirmables (NFR Orden, ADR-05). Idempotencia: 100 % efecto neto único (NFR Idempotencia, ADR-07) |
| Fiabilidad (tolerancia a fallos, recuperabilidad, madurez) | Crítica | El trabajo sin conexión asume cortes como condición normal; la reanudación sin pérdida es la garantía central de NB-04 | Reanudación: 0 perdidos y 0 duplicados tras corte en subida (NFR Reanudación, ADR-06/07). Continuidad ante conflicto: 0 ciclos abortados (NFR Continuidad, ADR-08) |
| Mantenibilidad (modularidad, capacidad de prueba, reusabilidad) | Alta | Clean Architecture con capa Abstractions exige que el núcleo se pruebe sin infraestructura; la reusabilidad fuera de la solución depende de contratos estables | Cobertura dominio >= 85 % líneas / >= 80 % branches; mutation score >= 60 % en dominio (estrategia-testing §2) |
| Compatibilidad (de versión / interoperabilidad de la superficie pública) | Alta | Paquete redistribuible con consumidores externos: un cambio de contrato no señalado los rompe en silencio | 100 % de cambios incompatibles con incremento de versión mayor; 0 remociones sin deprecación (ADR-03 §8) |
| Eficiencia de desempeño (comportamiento temporal, uso de recursos) | Media | El motor opera en red móvil típica y sobre una cola local acotada; hay objetivos numéricos verificables pero no es el atributo dominante | Lote de 100 cambios en <= 30 s; cola local >= 1000 pendientes sin degradación (NFR Tiempo de lote / Capacidad, ADR-05/04) |
| Seguridad | Media | El motor no emite ni almacena credenciales: las recibe de un proveedor inyectado del host y las usa solo durante la fase que las requiere; no registra carga útil de dominio | No expone secretos en diagnóstico; la credencial no se persiste fuera de su fase (cross-cutting §7 de 05) |
| Portabilidad | Media | El núcleo no depende de un almacén ni transporte concretos; toda dependencia se inyecta como abstracción del host | Núcleo del motor sin dependencias de infraestructura; estrategias sustituibles sin tocar el núcleo (guia-testing-extensibilidad) |
| Usabilidad | No aplica (UI); aplica como DX | La librería no tiene UI; la "usabilidad" se traslada a la experiencia del developer integrador, cubierta por la categoría 03 (DX) | Mensajes de error accionables y códigos estables (catálogo de errores de 03); no se mide como atributo de UI |

## 3. Quality gates

Cada gate es un criterio mecánico que el pipeline aplica antes de declarar un build, una rama o un release como aceptable. Estos gates se materializan como stages del pipeline en la categoría 09; aquí se declaran su condición, su herramienta abstracta y su consecuencia. Reconcilian el gate global del intake §17 P.6 (>= 80 % líneas / >= 70 % branches) con las coberturas por capa de esta estrategia (ver §4 de `estrategia-testing_v1.0.md` y la nota de reconciliación al final de esta sección).

| Gate | Condición | Herramienta (rol abstracto) | Consecuencia si falla |
| --- | --- | --- | --- |
| G1 Compilación limpia | Compila sin advertencias tratadas como error | Compilador del runtime objetivo | Bloquea el merge y la publicación |
| G2 Suite unitaria verde | 100 % de los tests unitarios del dominio pasan; ningún test sin assert | Framework de tests unitarios | Bloquea el merge |
| G3 Contratos verdes | 100 % de los contract tests por interfaz de extensión (almacén local, transporte, credencial, conectividad) pasan | Framework de tests unitarios sobre dobles de contrato | Bloquea el merge |
| G4 Cobertura por capa | Dominio >= 85 % líneas / >= 80 % branches; infraestructura >= 70 % líneas / >= 60 % branches; global >= 80 % líneas / >= 70 % branches (intake §17 P.6) | Herramienta de cobertura del runtime | Bloquea el merge; excepción solo con ADR |
| G5 Mutation score en dominio | Mutation score del dominio >= 60 % | Framework de mutation testing | Advierte en el merge; bloquea el release |
| G6 Propiedades invariantes | Las suites property-based de orden, idempotencia y no duplicación pasan sobre el espacio de entradas generado | Framework de property-based testing | Bloquea el merge |
| G7 NFR numéricos | Lote de 100 en <= 30 s; cola de >= 1000 sin degradación; 0 perdidos / 0 duplicados en reanudación | Pruebas de rendimiento y de carga del ciclo | Bloquea el release |
| G8 Compatibilidad de superficie pública | Ningún cambio incompatible sin incremento de versión mayor; verificación post-publicación reproduce el contrato | Comparación contra la matriz de compatibilidad; restauración del paquete en proyecto limpio | Bloquea la publicación (BT-14) |
| G9 Análisis estático | Sin issues críticos del análisis estático | Analizador estático del runtime | Bloquea el merge |

Reconciliación del gate global con las coberturas por capa: el intake §17 P.6 fija un piso global de >= 80 % líneas y >= 70 % branches; esta estrategia declara coberturas diferenciadas por capa (dominio 85 % / 80 %, infraestructura 70 % / 60 %). Son compatibles: el dominio concentra la lógica de invariantes y se exige por encima del piso global, mientras que la infraestructura adaptadora se exige al piso de su capa; el agregado ponderado de ambas capas no puede caer por debajo del piso global, de modo que cumplir las coberturas por capa implica cumplir el gate global, nunca al revés. El gate G4 verifica las tres condiciones simultáneamente y la global nunca relaja una capa.

## 4. Roles QA dentro del equipo

El proyecto es de un solo desarrollador (`equipo_n=1`, intake §13 y mini-plan de 07). El RACI se simplifica en consecuencia, manteniendo separadas las responsabilidades aunque las ejerza la misma persona en momentos distintos.

| Actividad | Responsable | Aprobador | Consultado | Informado |
| --- | --- | --- | --- | --- |
| Diseño de los casos de prueba (TC) | AG-08 (rol QA del dev) | AG-08 | AG-02 (trazabilidad a CU), AG-05 (NFR) | AG-06, AG-07 |
| Implementación de tests y fixtures | AG-08 (rol SDET del dev) | AG-08 | AG-05 (contratos de extensión) | — |
| Ejecución de la suite en CI | Pipeline (09) | AG-08 | — | AG-06, AG-07 |
| Aprobación del release | AG-08 | AG-07 (Maintainer Lead) | AG-05 (compatibilidad) | AG-06 |
| Regeneración de baselines de snapshot | AG-08 | AG-08 vía revisión de PR | — | — |

Con `equipo_n=1`, la separación de aprobador y responsable se materializa mediante el control del pipeline y la revisión del propio PR: ningún cambio entra a la rama principal sin pasar los gates G1-G9, lo que sustituye la revisión por pares ausente.

## 5. Cadencia de revisión

- La estrategia de calidad y sus umbrales se revisan al cierre de cada tramo de release (R1, R2, R3 del mini-plan de 07), no por sprint timeboxed, dado el modo release-driven del proyecto.
- Los umbrales de cobertura y mutation score son piso, no techo (08_rules §2.2); solo se bajan con un ADR que lo justifique y que quede registrado en el control de cambios de este documento.
- Cualquier defecto de integridad de datos detectado en una versión publicada dispara una revisión inmediata de la estrategia y un TC de regresión que lo prevenga (08_rules §5.4).
- La matriz de cobertura (`matriz-cobertura-pruebas_v1.0.md`) se actualiza al cierre de cada tramo; un estado "Pendiente" persistente tras el cierre del tramo correspondiente es un hallazgo de la revisión.

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Estrategia de calidad inicial de aplicada-sync: definición de calidad para librería redistribuible sin UI, ocho atributos ISO 25010 priorizados con métrica numérica y NFR de origen, nueve quality gates mecánicos con reconciliación del gate global del intake §17 P.6 contra las coberturas por capa, RACI para equipo_n=1 y cadencia de revisión release-driven. Derivada de la arquitectura de 05 (§8 quality attributes, ADR-01 a ADR-08), del intake §17 P.6/P.10 y de las reglas 08 §2.2/§4.2. |
