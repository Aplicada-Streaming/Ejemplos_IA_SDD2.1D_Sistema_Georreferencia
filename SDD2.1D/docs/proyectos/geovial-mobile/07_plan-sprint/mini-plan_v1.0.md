# Mini-plan de sprint — geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** mini-plan_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-16
**Autor:** Scrum Master + Mobile Release Manager
**Tipo (D8):** mobile-app-maui
**Modo:** mini-plan (equipo_n = 1; sustituye a plan-iteracion-sprint-XX, templates de review/retro y velocidad-equipo, §2.1/§2.2 de 07_rules)
**Técnica de estimación:** story points con escala Fibonacci (1, 2, 3, 5, 8, 13), heredada de 06
**Fase de roadmap:** F2 — Captura en campo y sincronización (roadmap-producto_v1.0.md §2)

Plan único condensado para un desarrollador. Combina objetivo, ítems comprometidos por tramos secuenciados por dependencias, trazabilidad a CU y NB, riesgos con mitigación y bitácora de avance. No se generan artefactos de sprint completos. La duración de cada tramo no se fija por calendario: la cadencia la marca el avance del rol único y los criterios de transición de la fase F2 (roadmap §5), coherente con el tipo `mobile-app-maui` cuyos cierres se atan al ciclo de distribución del paquete de la app (07_rules §3.2).

## 1. Objetivo

Disponer de la app de campo offline-first que permite al agente iniciar sesión y reanudar el trabajo con seguridad en el dispositivo compartido, capturar observaciones georreferenciadas sin conexión y sincronizarlas subiendo antes de bajar sin pérdida ni duplicación.

## 2. Unidad y capacidad

- Unidad de estimación: story points Fibonacci, declarada y mantenida desde 06.
- Equipo: un desarrollador (equipo_n = 1); el mismo rol sostiene refinement liviano, construcción y verificación.
- Total comprometido: 123 SP (62 SP de BT + 61 SP de US), distribuidos en tres tramos secuenciales.
- No se declara velocity ni capacidad efectiva por sprint: sin línea de base de cadencia del equipo de un solo dev, el compromiso se gestiona por tramos y por criterios de transición de fase, no por tope de puntos por iteración.

## 3. Ítems comprometidos por tramos

Los tramos respetan las dependencias técnicas del backlog (06): el almacén local y la sesión segura son base de todo; la captura georreferenciada se apoya en la cola y los permisos; la sincronización se apoya en la cola, la sesión y el adaptador del motor. Cada ítem referencia su identificador exacto de 06. Prioridad tomada de MoSCoW (US) y de la prioridad técnica (BT).

### 3.1 Tramo 1 — Esqueleto de sesión y almacén local

Objetivo del tramo: levantar el esqueleto de sesión (inicio en línea, relogueo por seguridad del dispositivo y deslogueo completo con custodia segura del token) sobre el almacén local persistente con migraciones versionadas, base de toda captura y sincronización posterior.

| ID | Tipo | Descripción | Prioridad | Estimación | Estado |
| --- | --- | --- | --- | --- | --- |
| BT-01 | Backlog técnico | Esquema del almacén local con índices y restricciones de réplica | Alta | 5 | Pendiente |
| BT-02 | Backlog técnico | Migraciones versionadas en el arranque con migración inicial auditable | Alta | 3 | Pendiente |
| BT-09 | Backlog técnico | Token en almacenamiento seguro del dispositivo y consumo del contrato de sesión | Alta | 5 | Pendiente |
| BT-10 | Backlog técnico | Tres modos de sesión y relogueo por seguridad del dispositivo en el ciclo de vida | Alta | 5 | Pendiente |
| US-01 | Historia | Iniciar sesión en línea con credenciales | Must | 5 | Pendiente |
| US-02 | Historia | Reloguear por seguridad del dispositivo y deslogueo completo | Must | 5 | Pendiente |

Subtotal tramo 1: 18 SP (BT) + 10 SP (US) = 28 SP.

Dependencias internas: BT-02 depende de BT-01; BT-10 depende de BT-09; BT-09 depende de BT-01 (esquema disponible). US-01 antes de US-02 (relogueo reanuda una sesión que el inicio en línea creó).

### 3.2 Tramo 2 — Captura georreferenciada

Objetivo del tramo: habilitar la selección del relevamiento asignado desde la copia local y la captura georreferenciada offline (marcadores, foto con resolución de coordenada en el momento, comentarios y etiquetas, carga manual con radio de agrupación), con permisos centralizados y degradaciones que no inventan datos, acumulando cada cambio en la cola local.

| ID | Tipo | Descripción | Prioridad | Estimación | Estado |
| --- | --- | --- | --- | --- | --- |
| BT-03 | Backlog técnico | Cola de cambios con orden de creación e identificador de origen y persistencia transaccional | Alta | 5 | Pendiente |
| BT-04 | Backlog técnico | Verificación del volumen objetivo de la cola local sin pérdida | Media | 3 | Pendiente |
| BT-05 | Backlog técnico | Componente de mapa local y creación y movimiento de marcadores | Alta | 5 | Pendiente |
| BT-06 | Backlog técnico | Captura de foto y resolución de coordenada anclada a una observación | Alta | 5 | Pendiente |
| BT-07 | Backlog técnico | Solicitud y chequeo de permisos centralizado en adaptadores de plataforma | Alta | 5 | Pendiente |
| BT-08 | Backlog técnico | Degradaciones por permiso, falta de señal y falta de espacio | Media | 5 | Pendiente |
| US-03 | Historia | Ver y seleccionar un relevamiento asignado | Must | 3 | Pendiente |
| US-04 | Historia | Refrescar la lista de relevamientos con conexión | Should | 3 | Pendiente |
| US-05 | Historia | Centrar por ubicación y crear un marcador | Must | 5 | Pendiente |
| US-06 | Historia | Mover un marcador conservando su identidad | Should | 3 | Pendiente |
| US-07 | Historia | Capturar una foto con resolución de coordenada | Must | 5 | Pendiente |
| US-08 | Historia | Conservar la foto sin señal de ubicación como pendiente | Should | 3 | Pendiente |
| US-09 | Historia | Agregar nota, comentario y etiquetas a la observación | Must | 3 | Pendiente |
| US-10 | Historia | Reutilizar y editar etiquetas del relevamiento | Could | 2 | Pendiente |
| US-14 | Historia | Cargar fotos manualmente con radio de agrupación | Must | 5 | Pendiente |
| US-15 | Historia | Resolver fotos sin ubicación incrustada como pendientes | Could | 3 | Pendiente |

Subtotal tramo 2: 28 SP (BT) + 35 SP (US) = 63 SP.

Dependencias internas: BT-03 depende de BT-01 (tramo 1); BT-04 depende de BT-03; BT-05 depende de BT-03; BT-06 depende de BT-03 y BT-05; BT-07 depende de BT-05; BT-08 depende de BT-06 y BT-07. La refrescada de la lista (US-04) consume el ciclo de bajada del adaptador del motor (BT-12); se compromete en este tramo como camino con conexión opcional y se completa cuando el tramo 3 deja el ciclo disponible. Las US Could (US-10, US-15) entran con un solo escenario por la excepción de la DoR (definition-of-ready §3).

### 3.3 Tramo 3 — Sincronización

Objetivo del tramo: cerrar el ciclo subir-luego-bajar consumiendo el motor de la librería de sincronización por contrato, con detección de conectividad, reanudación idempotente tras corte y convivencia no bloqueante con los marcadores en conflicto.

| ID | Tipo | Descripción | Prioridad | Estimación | Estado |
| --- | --- | --- | --- | --- | --- |
| BT-11 | Backlog técnico | Adaptador de la librería de sincronización implementando sus puertos | Alta | 5 | Pendiente |
| BT-12 | Backlog técnico | Ciclo subir-luego-bajar con detección de conectividad y reanudación idempotente | Alta | 8 | Pendiente |
| BT-13 | Backlog técnico | Convivencia con conflictos y estado de sincronización | Media | 3 | Pendiente |
| US-11 | Historia | Sincronizar subiendo antes de bajar | Must | 8 | Pendiente |
| US-12 | Historia | Reanudar una sincronización interrumpida sin duplicar | Must | 5 | Pendiente |
| US-13 | Historia | Convivir con conflictos durante la sincronización | Should | 3 | Pendiente |

Subtotal tramo 3: 16 SP (BT) + 16 SP (US) = 32 SP.

Dependencias internas: BT-11 depende de BT-03 (cola, tramo 2) y BT-09 (proveedor de credencial, tramo 1); BT-12 depende de BT-11; BT-13 depende de BT-12. Cierra la dependencia de US-04 (refresco con conexión) dejada abierta en el tramo 2.

### 3.4 Resumen de compromiso

| Tramo | Foco | BT | US | SP |
| --- | --- | --- | --- | --- |
| 1 | Sesión y almacén local | BT-01, BT-02, BT-09, BT-10 | US-01, US-02 | 28 |
| 2 | Captura georreferenciada | BT-03, BT-04, BT-05, BT-06, BT-07, BT-08 | US-03, US-04, US-05, US-06, US-07, US-08, US-09, US-10, US-14, US-15 | 63 |
| 3 | Sincronización | BT-11, BT-12, BT-13 | US-11, US-12, US-13 | 32 |
| Total | — | 13 BT (62 SP) | 15 US (61 SP) | 123 |

## 4. Definition of Done

Este mini-plan referencia la Definition of Done canónica del proyecto, que vive en la categoría 08 y aún no está publicada (pendiente de 08). Cada ítem se da por terminado cuando cumple su DoD canónica una vez disponible. Mientras 08 no publique la DoD, se aplica como criterio de cierre provisional el cumplimiento de los criterios de aceptación del propio ítem en 06 (Given/When/Then de la US y criterios técnicos de la BT), sin sustituir a la DoD canónica. El criterio de entrada de cada ítem al tramo es la Definition of Ready de 06 (definition-of-ready_v1.0.md).

## 5. Trazabilidad a CU y NB por tramo

| Tramo | CU que avanzan | NB que avanza | ADRs que gobiernan |
| --- | --- | --- | --- |
| 1 | CU-01 | NB-01 (identidad y sesión segura del agente en dispositivo compartido) | ADR-02, ADR-05 |
| 2 | CU-02, CU-03, CU-04, CU-05, CU-07 | NB-03 (captura georreferenciada de observaciones) | ADR-01, ADR-02, ADR-04 |
| 3 | CU-06 | NB-04 (trabajo sin conexión y sincronización) | ADR-03 |

Al cierre de los tres tramos, los 7 CU del lado de campo (CU-01 a CU-07) quedan avanzados y las tres NB de campo (NB-01, NB-03, NB-04) registran avance, completando la capacidad de la fase F2 del roadmap.

## 6. Riesgos y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El ciclo de distribución del paquete de la app por el canal interno demora la verificación en dispositivo real y el cierre de cada tramo (07_rules §3.2: cierres atados a la revisión y publicación del paquete) | Alta | Alto | Verificar en dispositivo con paquete de prueba interno tan pronto el tramo lo permita, sin esperar publicación final; mantener dobles del adaptador de ubicación, cámara y motor (DoR §1.7) para verificar lógica sin dispositivo ni red; planificar la generación del paquete del tramo con holgura previa al cierre |
| La integración del motor de sincronización consumido (BT-11) depende de una versión compatible del contrato de la librería; un contrato no publicado o incompatible bloquea el tramo 3 | Media | Alto | Declarar BT-11 y BT-12 como Ready condicional sobre la versión menor compatible del contrato consumido (excepción de la DoR §3); fijar la versión mayor de los contratos; sustituir el motor por un doble que respete los puertos para no bloquear US-11/US-12 mientras se confirma la versión |
| La reanudación idempotente tras corte de subida (US-12, BT-12) no reconoce los reenvíos por identificador de origen y produce duplicación, riesgo de máximo impacto del negocio | Media | Alto | Verificar la reanudación con corte simulado tras la primera confirmación antes de dar BT-12 por terminada; cubrir el reenvío reconocido por identificador de origen con prueba dedicada; conservar la cola intacta ante token rechazado |
| La captura sin señal de ubicación o sin espacio de almacenamiento inventa coordenada o pierde evidencia encolada (RN-01, RN-05) | Media | Medio | Verificar las degradaciones de BT-08 (sin señal queda pendiente sin coordenada; sin espacio no persiste el binario, avisa y conserva lo encolado) con escenarios negativos antes del cierre del tramo 2 |

## 7. Bitácora de avance

Registro condensado del avance del tramo, en lugar de burndown y velocity de un equipo multi-dev. Se actualiza al cerrar cada tramo o ante un cambio de scope.

| Fecha | Tramo | Avance | Ítems cerrados | Notas |
| --- | --- | --- | --- | --- |
| 2026-06-16 | — | Plan propuesto | 0 de 28 (US+BT) | Compromiso inicial por tramos; sin ítems iniciados |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-16 | Mini-plan inicial de geovial-mobile (modo equipo_n=1): objetivo único orientado a valor, 13 BT y 15 US de 06 distribuidas en tres tramos secuenciados por dependencias (sesión y almacén local; captura georreferenciada; sincronización), DoD por referencia a la canónica de 08 (pendiente de 08), trazabilidad a CU-01..07 y NB-01/03/04 por tramo, cuatro riesgos con mitigación incluido el ciclo de distribución del paquete, y bitácora de avance. Sustituye a los cuatro artefactos de sprint completos. |
