# Mini-plan de release — aplicada-sync

**Proyecto:** aplicada-sync
**Solución:** GeoVial
**Documento:** mini-plan_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha inicio:** 2026-06-15
**Fecha fin:** por confirmar al cierre del tramo R3 (proyecto de investigación sin fecha externa, intake §10)
**Autor:** Scrum Master + Maintainer Lead (AG-07)
**Modo:** mini-plan (equipo_n=1, un solo desarrollador)
**Unidad de estimación:** story points con escala Fibonacci (1, 2, 3, 5, 8, 13), coherente con el backlog de 06

## 1. Información general

aplicada-sync es una librería redistribuible y agnóstica del dominio (tipo library, variante release-driven). El trabajo se organiza por versiones publicables más que por iteraciones timeboxed: el equipo es de un solo desarrollador (equipo_n=1), por lo que se documenta este mini-plan único en lugar de los planes de sprint, plantillas de review/retro y tracking de velocity (§2.2 de 07_rules_plan_sprint.md).

- Capacidad declarada: un desarrollador a dedicación parcial; cadencia fijada por el avance del equipo (sin hito externo, intake §10).
- Alcance total comprometido: 13 historias (US-01 a US-13) y 14 tareas técnicas (BT-01 a BT-14) del backlog de 06, sumando 54 story points de US.
- Estrategia de entrega: tramos de release incrementales. R1 entrega el MVP por Must Have (las garantías que NB-04 exige); R2 y R3 completan disparo automático, observabilidad, endurecimiento y verificación de compatibilidad de la superficie pública.
- Política de compatibilidad: la superficie pública (capa Abstractions) sigue versionado semántico; los tramos posteriores no rompen el contrato fijado en R1 sin un salto de versión mayor (gobernado por ADR-03).

## 2. Objetivo del release

Publicar una librería de sincronización consumible cuya superficie pública garantice el ciclo subir-luego-bajar en orden estricto, idempotente, reanudable sin pérdida ni duplicación y conviviente con estados en conflicto, de modo que una aplicación de campo sin conectividad pueda propagar su trabajo de forma confiable (NB-04).

## 3. Ítems comprometidos por tramo de release

Los ítems referencian el identificador exacto del backlog de 06 (product-backlog_v1.0.md para US, backlog-tecnico_v1.0.md para BT). La prioridad y la estimación se transcriben del backlog; no se reestiman aquí. El orden interno de cada tramo respeta las dependencias técnicas declaradas en la matriz de 06 (ver §4).

### Tramo R1 — MVP: garantía central del motor

Objetivo del tramo: entregar las garantías que NB-04 exige (integridad 0 perdidos / 0 duplicados, orden subir-antes-de-bajar y convivencia con conflicto sin bloqueo). Corresponde al MVP por Must Have de 06 (32 SP de US).

| ID | Tipo | Descripción corta | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-01 | Backlog técnico | Contratos de extensión de la capa Abstractions | Alta | 5 SP | Dev único | Pendiente |
| BT-03 | Backlog técnico | Spike de registro y resolución explícita de estrategias (caja temporal 2 días) | Media | 3 SP | Dev único | Pendiente |
| BT-02 | Backlog técnico | Operación de inicialización y armado de configuración | Alta | 5 SP | Dev único | Pendiente |
| US-01 | Historia | Inicializar una sesión con estrategias obligatorias | Alta (Must) | 5 SP | Dev único | Pendiente |
| BT-04 | Backlog técnico | Cola persistente única por identificador estable | Alta | 5 SP | Dev único | Pendiente |
| US-03 | Historia | Encolar un cambio local con identificador estable | Alta (Must) | 3 SP | Dev único | Pendiente |
| US-04 | Historia | Reencolar sin duplicar por identificador | Alta (Must) | 3 SP | Dev único | Pendiente |
| BT-06 | Backlog técnico | Orquestación del ciclo de dos fases en orden estricto | Alta | 8 SP | Dev único | Pendiente |
| BT-07 | Backlog técnico | Ejecutores de fase de subida y de bajada | Alta | 5 SP | Dev único | Pendiente |
| US-05 | Historia | Ejecutar el ciclo subir-luego-bajar en orden | Alta (Must) | 8 SP | Dev único | Pendiente |
| BT-12 | Backlog técnico | Registro de estado, progreso y elementos en conflicto | Media | 5 SP | Dev único | Pendiente |
| US-06 | Historia | Convivir con estados en conflicto sin abortar | Alta (Must) | 5 SP | Dev único | Pendiente |
| BT-09 | Backlog técnico | Marca de progreso y reanudación desde el corte | Alta | 8 SP | Dev único | Pendiente |
| US-12 | Historia | Reanudar una subida interrumpida sin duplicar | Alta (Must) | 8 SP | Dev único | Pendiente |

Puntos de US comprometidos en R1: 32 SP (US-01, US-03, US-04, US-05, US-06, US-12).

### Tramo R2 — Disparo automático, observabilidad y endurecimiento

Objetivo del tramo: completar el valor operativo (sincronización por sí sola al recuperar conectividad, presentación de estado al usuario) y endurecer el motor ante rebote de red, concurrencia y cortes encadenados. Cubre las US Should y Could y sus BT de soporte.

| ID | Tipo | Descripción corta | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-08 | Backlog técnico | Exclusión mutua de un único ciclo por sesión | Media | 3 SP | Dev único | Pendiente |
| US-07 | Historia | Evitar ciclos concurrentes en una misma sesión | Media (Should) | 3 SP | Dev único | Pendiente |
| BT-10 | Backlog técnico | Resolución de progreso inconsistente con la cola como fuente de verdad | Media | 3 SP | Dev único | Pendiente |
| US-13 | Historia | Tolerar un nuevo corte durante la reanudación | Media (Should) | 3 SP | Dev único | Pendiente |
| BT-11 | Backlog técnico | Observador de conectividad con descarte de rebote | Media | 5 SP | Dev único | Pendiente |
| US-08 | Historia | Disparar el ciclo al recuperar conectividad | Media (Should) | 5 SP | Dev único | Pendiente |
| US-09 | Historia | No reentrar por rebote de conectividad | Baja (Could) | 3 SP | Dev único | Pendiente |
| US-02 | Historia | Recuperar una sesión persistida con su cola | Media (Should) | 3 SP | Dev único | Pendiente |
| US-10 | Historia | Consultar estado y tamaño de la cola | Media (Should) | 3 SP | Dev único | Pendiente |
| US-11 | Historia | Listar elementos en conflicto conocidos | Baja (Could) | 2 SP | Dev único | Pendiente |

Puntos de US comprometidos en R2: 22 SP (US-02, US-07, US-08, US-09, US-10, US-11, US-13).

### Tramo R3 — Endurecimiento no funcional, errores y compatibilidad publicable

Objetivo del tramo: consolidar la infraestructura transversal (catálogo estable de errores y diagnóstico), validar el volumen objetivo de la cola y verificar la compatibilidad de la superficie pública con un quick-start antes de publicar. Es trabajo técnico de release sin US nuevas; las US ya entregadas dependen de estas BT para sus criterios de error y de capacidad.

| ID | Tipo | Descripción corta | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-13 | Backlog técnico | Catálogo estable de errores y diagnóstico estructurado | Media | 3 SP | Dev único | Pendiente |
| BT-05 | Backlog técnico | Verificación del volumen objetivo de la cola (>= 1000 pendientes) | Media | 3 SP | Dev único | Pendiente |
| BT-14 | Backlog técnico | Verificación de compatibilidad de la superficie pública con un quick-start | Media | 3 SP | Dev único | Pendiente |

Nota sobre BT-13: por proveer códigos de error estables consumidos por los criterios de aceptación de US-01, US-03, US-05, US-08 y US-12, su materialización se adelanta de hecho durante R1 y R2 (cada operación que reporta un código usa el catálogo); en R3 se consolida, se cierra y se verifica como contrato estable. Se planifica en R3 para no bloquear la entrega del MVP, dejando los códigos provisionales saldados antes de publicar.

Resumen de compromiso total: 54 SP de US (32 en R1, 22 en R2) más las 14 BT distribuidas en los tres tramos. Todas las US y BT tienen origen verificable en el backlog de 06; no se inventan identificadores.

## 4. Secuencia y orden de construcción (dependencias)

El orden respeta las dependencias técnicas de la matriz de 06 y el orden topológico de las épicas técnicas (ET-01 fundaciones → ET-02 cola → ET-03 pipeline → ET-04 reanudación → ET-05 conectividad/estado → ET-06 versionado/distribución). Esta sección no redefine arquitectura; referencia la de 05.

Cadena de dependencias comprometida:

- BT-01 (contratos) es raíz: no tiene dependencias y habilita BT-02, BT-04 y BT-13.
- BT-03 (spike) depende de BT-01 y eleva hallazgos a BT-02; se ejecuta antes de cerrar BT-02 dentro de R1.
- BT-02 (inicialización) depende de BT-01 y soporta US-01 y US-02.
- BT-04 (cola única) depende de BT-01 y BT-02; soporta US-03, US-04 y US-10. BT-05 depende de BT-04.
- BT-06 (orquestación) depende de BT-02 y BT-04; soporta US-05 y habilita BT-07, BT-08, BT-09, BT-11.
- BT-07 (ejecutores) depende de BT-06; soporta US-05 y US-06.
- BT-12 (registro de estado y conflictos) depende de BT-04 y BT-07; soporta US-06, US-10 y US-11.
- BT-09 (marca de progreso y reanudación) depende de BT-06 y BT-07; soporta US-12 y US-13. BT-10 depende de BT-09.
- BT-08 (exclusión mutua) depende de BT-06; soporta US-07.
- BT-11 (observador de conectividad) depende de BT-06; soporta US-08 y US-09.
- BT-13 (catálogo de errores) depende de BT-01.
- BT-14 (verificación de compatibilidad) depende de BT-02, BT-06 y BT-09; es la última puerta antes de publicar.

Orden lineal de construcción para un solo desarrollador:

1. R1: BT-01 → BT-03 → BT-02 → US-01 → BT-04 → US-03 → US-04 → BT-06 → BT-07 → US-05 → BT-12 → US-06 → BT-09 → US-12.
2. R2: BT-08 → US-07 → BT-10 → US-13 → BT-11 → US-08 → US-09 → US-02 → US-10 → US-11.
3. R3: BT-13 (consolidación) → BT-05 → BT-14.

Cada US se cierra solo cuando su BT de soporte está terminada; ninguna US se adelanta a la BT que la habilita.

## 5. Definition of Done aplicada

La Definition of Done canónica del proyecto vive en la categoría 08 y aún no está generada. Este mini-plan la referencia como pendiente de 08: al generarse `08`, cada US y cada BT de este plan se cierra contra la DoD canónica de ese documento (sin redefinir criterios aquí). Hasta entonces, se usan como criterios de cierre transitorios los criterios de aceptación Given/When/Then de cada US (product-backlog_v1.0.md §3) y los criterios técnicos de cada BT (backlog-tecnico_v1.0.md §2), más las puertas de calidad declaradas para la librería en el intake (cobertura >= 80 % líneas / >= 70 % branches, pruebas de contrato del orden subir-antes-de-bajar, idempotencia y reanudación, y verificación post-publicación).

Criterio de cierre específico del release, adicional a la DoD canónica: ningún tramo se publica si rompe la compatibilidad de la superficie pública fijada por el tramo anterior sin un salto de versión mayor (gobernado por ADR-03 y verificado por BT-14).

## 6. Riesgos y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El spike BT-03 no arroja una recomendación clara sobre registro de estrategias y bloquea el cierre de BT-02 y de US-01 | Media | Alto | Caja temporal de 2 días declarada en 06; si al cierre no hay recomendación, documentar el bloqueo y adoptar el registro explícito por defecto de ADR-02, dejando la optimización para un tramo posterior sin frenar R1 |
| Las garantías 0 perdidos / 0 duplicados de US-12 y la reanudación (BT-09/BT-10) son las más caras (8 SP cada US) y concentran el riesgo del MVP en un único desarrollador | Alta | Alto | Construir BT-06 y BT-07 con pruebas de contrato de idempotencia antes de abordar BT-09; verificar la reanudación con cortes simulados y cola como fuente de verdad; si R1 se desborda, diferir US-13 y BT-10 a R2 (ya planificados allí) sin tocar la garantía central |
| Con equipo_n=1 no hay velocity histórica ni promedio móvil; el compromiso de 32 SP en R1 puede sobreestimar la capacidad real | Media | Medio | No comprometer fechas duras; usar la bitácora de avance semanal (§9) como instrumento de recalibración; ajustar el alcance de R2/R3 según el ritmo observado en R1 |
| BT-13 (códigos de error) se consolida en R3 pero sus códigos los consumen US de R1/R2; códigos provisionales podrían filtrarse a la superficie publicada | Media | Medio | Mantener un único origen de códigos desde R1 aunque el catálogo se cierre en R3; bloquear la publicación de cualquier tramo hasta que BT-14 verifique que los códigos expuestos son los estables del catálogo |

## 7. Criterios de hecho del release

- R1 se considera completo cuando US-01, US-03, US-04, US-05, US-06 y US-12 están en estado terminado contra la DoD canónica de 08, las pruebas de contrato del orden subir-antes-de-bajar, idempotencia y reanudación pasan, y el MVP es consumible por una aplicación host.
- R2 se considera completo cuando US-02, US-07, US-08, US-09, US-10, US-11 y US-13 están terminadas y el motor tolera rebote de red, concurrencia y cortes encadenados sin pérdida ni duplicación.
- R3 se considera completo cuando BT-13 cierra el catálogo de errores estable, BT-05 verifica el volumen objetivo (>= 1000) y BT-14 reproduce el contrato con un quick-start en un proyecto limpio; recién entonces el paquete es publicable.
- El release global se considera cerrado cuando los tres tramos están completos y la superficie pública verifica compatibilidad para su primera versión publicable.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB que avanzan | NB-04 (trabajo sin conexión con sincronización confiable); R1 entrega sus garantías centrales (integridad 0/0, orden, convivencia con conflicto), R2 su disparo automático y observabilidad, R3 su capacidad y compatibilidad publicable |
| CU que avanzan en R1 | CU-01 (US-01), CU-02 (US-03, US-04), CU-03 (US-05, US-06), CU-06 (US-12) |
| CU que avanzan en R2 | CU-01 (US-02), CU-03 (US-07), CU-04 (US-08, US-09), CU-05 (US-10, US-11), CU-06 (US-13) |
| CU que cierran cobertura en R3 | CU-01 a CU-06 (BT-13 y BT-14 consolidan códigos de error y compatibilidad del contrato que todos consumen) |
| RN implicadas | RN-01 (orden subir-antes-de-bajar; BT-06, US-05), RN-02 (idempotencia; US-04, BT-09, US-12), RN-03 (convivencia con conflicto; US-06, US-11, BT-12) |
| ADRs que gobiernan | ADR-01 y ADR-02 (BT-01), ADR-03 (BT-14, política de compatibilidad), ADR-04 y ADR-07 (BT-04, BT-05), ADR-05 (BT-06, BT-07), ADR-06 y ADR-07 (BT-09, BT-10), ADR-08 (BT-12) |
| Downstream a 08 | Cada US comprometida dispara la creación o actualización de su caso de aceptación en 08; la DoD canónica de 08 es la referencia de cierre (hoy pendiente) |

## 9. Bitácora de avance (a completar semanalmente)

Tabla de seguimiento del avance del release. Una fila por semana; se completa a medida que el desarrollador progresa. SP restantes parte de 54 (puntos de US del backlog total).

| Semana | Fecha | Tramo | Ítems en curso | Ítems cerrados en la semana | SP de US restantes | Observaciones / impedimentos |
| --- | --- | --- | --- | --- | --- | --- |
| 0 | 2026-06-15 | R1 | — | — | 54 | Línea de base; arranque del release, sin velocity histórica |
| 1 | | R1 | | | | |
| 2 | | R1 | | | | |
| 3 | | R1 | | | | |
| 4 | | R1 | | | | |
| ... | | | | | | |

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Mini-plan de release inicial de aplicada-sync (modo equipo_n=1, library release-driven). Objetivo de release, 13 US y 14 BT del backlog de 06 organizadas en tres tramos (R1 MVP por Must Have, R2 disparo/observabilidad/endurecimiento, R3 errores/capacidad/compatibilidad), secuencia de construcción por dependencias, DoD por referencia a 08 (pendiente), cuatro riesgos con mitigación, criterios de hecho, trazabilidad a NB-04, CU-01 a CU-06, RN-01 a RN-03 y ADR-01 a ADR-08, y bitácora de avance semanal a completar. |
