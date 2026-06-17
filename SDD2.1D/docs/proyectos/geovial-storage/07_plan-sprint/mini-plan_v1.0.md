# Mini-plan — geovial-storage

**Proyecto:** geovial-storage
**Documento:** mini-plan_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha inicio:** 2026-06-15
**Fecha fin:** 2026-08-21
**Autor:** Scrum Master + Maintainer Lead
**Tipo (D8):** library (release-driven)
**Modo:** mini-plan para un solo desarrollador (equipo_n=1), regla 07 §2.1/§2.2

## 1. Información general

Plan único condensado que sustituye a los cuatro artefactos completos de sprint (regla 07 §2.2, escenario equipo de 1 dev). El trabajo de `geovial-storage` es release-driven: se organiza alrededor de una versión publicable del contrato de Abstractions y de su compatibilidad hacia atrás (ADR-02, ADR-03), no de features de pantalla.

- Composición del equipo: un desarrollador (equipo_n=1), con revisiones acotadas de AG-06 (backlog), AG-05 (arquitectura) y AG-08 (verificabilidad para 08).
- Unidad de estimación: story points en escala Fibonacci (1, 2, 3, 5, 8, 13), coherente con el backlog de 06.
- Volumen comprometido: 37 SP en historias (US-01 a US-09) y 71 SP en tareas técnicas (BT-01 a BT-13), tomados con su identificador exacto del backlog de 06, sin re-estimación.
- Cadencia de seguimiento: bitácora semanal (§7), no ceremonias formales por tratarse de un solo dev.
- Ventana de ejecución estimada: cinco tramos release-driven entre 2026-06-15 y 2026-08-21, consistentes con los sprints estimados por épica en 06 (EP-01 ×2, EP-02 ×1, EP-03 ×2, EP-04 ×2).

## 2. Objetivo del plan

Publicar una versión inicial estable de la abstracción de almacenamiento de `geovial-storage` que permita al backend consumidor guardar, recuperar, eliminar, verificar y listar archivos con integridad byte a byte, y al usuario raíz activar el proveedor de destino, con transparencia verificada por una batería de contrato única sobre al menos dos proveedores soportados.

## 3. Ítems comprometidos por tramos

Cada ítem referencia el identificador exacto del backlog de 06 (US-XX, BT-XX). La prioridad y la estimación se transcriben del product-backlog y del backlog-tecnico sin alteración. El orden de los tramos respeta las dependencias declaradas en los criterios de cada BT (ver §4). Estado inicial: Pendiente.

### Tramo 1 — Fundaciones del contrato (EP-04, base)

| ID | Tipo | Descripción corta | Prioridad | Estimación | Estado |
| --- | --- | --- | --- | --- | --- |
| BT-11 | Backlog técnico | Declarar la capa de Abstracciones y el puerto de proveedor | Must | 5 SP | Pendiente |
| BT-05 | Backlog técnico | Definir el catálogo de errores uniforme | Must | 3 SP | Pendiente |
| BT-13 | Backlog técnico | Doble de proveedor en memoria para pruebas del núcleo | Must | 3 SP | Pendiente |
| BT-04 | Backlog técnico | Núcleo de enrutado, validación de entrada y normalización de errores | Must | 8 SP | Pendiente |

Subtotal Tramo 1: 19 SP (BT). Habilita la verificación de transparencia y desbloquea todas las operaciones de datos.

### Tramo 2 — Operación de guardado y su rama de colisión (EP-01)

| ID | Tipo | Descripción corta | Prioridad | Estimación | Estado |
| --- | --- | --- | --- | --- | --- |
| BT-01 | Backlog técnico | Operación de guardado con marca de sobrescritura | Must | 5 SP | Pendiente |
| BT-02 | Backlog técnico | Generación y colisión del identificador lógico | Should | 3 SP | Pendiente |
| US-01 | Historia | Guardar un archivo y obtener su identificador lógico | Must | 5 SP | Pendiente |
| US-02 | Historia | Controlar la colisión de identificadores al guardar (duplicado y sobrescritura) | Should | 3 SP | Pendiente |

Subtotal Tramo 2: 16 SP (8 BT + 8 US).

### Tramo 3 — Operaciones de lectura, borrado, verificación y listado (EP-01)

| ID | Tipo | Descripción corta | Prioridad | Estimación | Estado |
| --- | --- | --- | --- | --- | --- |
| BT-03 | Backlog técnico | Recuperación (rango y solo-metadatos), eliminación, verificación y listado | Must | 8 SP | Pendiente |
| US-03 | Historia | Recuperar un archivo idéntico al guardado | Must | 5 SP | Pendiente |
| US-04 | Historia | Recuperar metadatos y segmentos por rango sin filtrar credenciales | Should | 3 SP | Pendiente |
| US-05 | Historia | Eliminar un archivo de forma idempotente y por prefijo | Must | 3 SP | Pendiente |
| US-06 | Historia | Verificar la existencia de un archivo sin transferir contenido | Must | 2 SP | Pendiente |
| US-07 | Historia | Listar archivos bajo un prefijo con paginación por testigo | Should | 5 SP | Pendiente |

Subtotal Tramo 3: 26 SP (8 BT + 18 US).

### Tramo 4 — Configuración del proveedor activo y resguardo de credenciales (EP-02)

| ID | Tipo | Descripción corta | Prioridad | Estimación | Estado |
| --- | --- | --- | --- | --- | --- |
| BT-07 | Backlog técnico | Resguardo de credenciales que entra pero no sale | Must | 5 SP | Pendiente |
| BT-06 | Backlog técnico | Registro de proveedores, activación y validación-en-seco con autorización | Must | 8 SP | Pendiente |
| US-08 | Historia | Activar un proveedor de almacenamiento como usuario raíz | Must | 8 SP | Pendiente |
| US-09 | Historia | Validar una configuración de proveedor sin activarla (prueba en seco) | Could | 3 SP | Pendiente |

Subtotal Tramo 4: 24 SP (13 BT + 11 US).

### Tramo 5 — Proveedores intercambiables, transferencia por tramos y gate de transparencia (EP-03)

| ID | Tipo | Descripción corta | Prioridad | Estimación | Estado |
| --- | --- | --- | --- | --- | --- |
| BT-08 | Backlog técnico | Adaptador de proveedor local | Must | 5 SP | Pendiente |
| BT-12 | Backlog técnico | Transferencia por tramos (rango y paginación) uniforme entre proveedores | Should | 5 SP | Pendiente |
| BT-09 | Backlog técnico | Adaptador de proveedor de objetos remoto | Should | 8 SP | Pendiente |
| BT-10 | Backlog técnico | Batería de contrato única por proveedor y gate de cobertura | Must | 5 SP | Pendiente |

Subtotal Tramo 5: 23 SP (BT).

### Total comprometido

| Dimensión | Cantidad | SP |
| --- | --- | --- |
| Historias (US-01 a US-09) | 9 | 37 |
| Tareas técnicas (BT-01 a BT-13) | 13 | 71 |
| Total | 22 ítems | 108 SP |

El alcance comprometido es el backlog completo de 06 hacia la versión inicial publicable. La unidad para un solo dev es el tramo release-driven, no un timebox de velocity de equipo: el orden de §4 prima sobre la fecha de cada tramo.

## 4. Orden de construcción y dependencias

El orden respeta estrictamente las dependencias declaradas en la columna Dependencias del backlog-tecnico de 06. La capa de Abstracciones y el catálogo de errores son la base de todo; los adaptadores y el gate de transparencia cierran al final porque dependen del núcleo y del doble en memoria.

| ID | Depende de | Habilita | Tramo |
| --- | --- | --- | --- |
| BT-11 | — | BT-05, BT-13, BT-07, BT-04 | 1 |
| BT-05 | BT-11 | BT-04 | 1 |
| BT-13 | BT-11 | BT-08, BT-10 | 1 |
| BT-04 | BT-11, BT-05 | BT-01, BT-03, BT-06 | 1 |
| BT-01 | BT-04 | BT-02, US-01 | 2 |
| BT-02 | BT-01 | US-02 | 2 |
| BT-03 | BT-04 | US-03 a US-07, BT-12 | 3 |
| BT-07 | BT-11 | BT-06, BT-09 | 4 |
| BT-06 | BT-04, BT-07 | US-08, US-09, BT-09 | 4 |
| BT-08 | BT-04, BT-13 | BT-09, BT-12, BT-10 | 5 |
| BT-12 | BT-03, BT-08 | US-04, US-07 (uniformidad) | 5 |
| BT-09 | BT-06, BT-08 | BT-10 (segundo destino real) | 5 |
| BT-10 | BT-08, BT-09, BT-13 | Cierre de transparencia y cobertura | 5 |

Notas de secuencia:

- El segundo destino verificable del MVP de transparencia se logra con el adaptador local (BT-08) y el doble en memoria (BT-13); el adaptador remoto (BT-09, Should) eleva la prueba a un destino de capacidades dispares pero no bloquea la versión inicial (backlog-tecnico §2).
- Cada US de operación se construye sobre su BT habilitante ya terminada: US-01/US-02 sobre BT-01/BT-02; US-03 a US-07 sobre BT-03; US-08/US-09 sobre BT-06/BT-07.
- Esta sección no redefine arquitectura: la estructura hexagonal, el puerto único y la normalización viven en 05 (ADR-01, ADR-02, ADR-04, `contratos-abstractions_v1.0.md`, `extensibilidad_v1.0.md`).

## 5. Definition of Done aplicada

La Definition of Done canónica del proyecto vive en la categoría 08 (testing y QA) y todavía no está publicada; este plan la referencia y no la reproduce (regla 07 §4.2 punto 5; la DoR de 06 ya separa el cuándo empezar del cuándo terminar). Cada ítem se considera terminado cuando cumple la DoD canónica de 08 una vez disponible.

Criterios específicos de este plan, derivados de las ADRs de 05, que se suman a la DoD canónica sin reemplazarla:

- Ninguna superficie pública nombra un proveedor concreto (RN-01, ADR-01, ADR-02).
- La ida y vuelta guardar-recuperar conserva igualdad binaria (RN-02, ADR-04).
- Ningún resultado, mensaje de error ni registro emite credenciales o parámetros de conexión (RN-03, ADR-05).
- La batería de contrato única produce resultados y códigos equivalentes contra cada proveedor soportado, y el gate de CI exige líneas ≥ 80 % y branches ≥ 70 % (ADR-04, BT-10).

Pendiente de 08: enlazar este plan a la DoD canónica cuando la categoría 08 publique su artefacto.

## 6. Riesgos y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Un solo desarrollador concentra todo el camino crítico (BT-11 → BT-04 → BT-03/BT-06 → BT-10); una ausencia o un bloqueo desplaza toda la cadena | Alta | Alto | Priorizar las BT de fundación (Tramo 1) primero para acortar el camino crítico; mantener la bitácora semanal (§7) como alerta temprana; diferir las Should (BT-02, BT-09, BT-12, US-02, US-04, US-07) y la Could (US-09) si el avance se atrasa, sin tocar las Must del MVP |
| El mecanismo físico de almacenamiento seguro en reposo queda pendiente del intake (§17.P.5) y se delega a 09; el resguardo de credenciales (BT-07) podría construirse contra un mecanismo no definido | Media | Alto | Construir BT-07 contra el comportamiento fijado por ADR-05 (entra pero no sale), no contra un mecanismo concreto; cubrir la verificación de no filtración (RN-03) en la batería de contrato; registrar la dependencia de 09 como supuesto documentado (DoR §3, excepción de mecanismo pendiente) |
| El adaptador de proveedor remoto (BT-09) introduce un destino de capacidades dispares cuya conectividad y latencia no controla la librería; podría desviar la transparencia | Media | Medio | El MVP de transparencia no depende de BT-09: se verifica con el adaptador local (BT-08) y el doble en memoria (BT-13); BT-09 es Should y entra al final del Tramo 5; el objetivo de latencia p95 numérico se fija solo para el proveedor local (ADR-04 §6) |
| La superficie pública erosiona al evolucionar (se filtra un detalle de proveedor o se rompe la compatibilidad) | Media | Alto | Revisión de superficie pública como gate de cada cambio (ADR-02 §8); clasificación de cada cambio como mayor/menor según ADR-03 y 02 §6 antes de mergear |

## 7. Bitácora de avance (semanal)

Tabla de seguimiento semanal para un solo dev. SP comprometidos totales: 108. La columna Tramo indica el tramo de §3 en foco esa semana. Se completa al cierre de cada semana.

| Semana | Fecha de corte | Tramo en foco | SP completados (acum.) | Ítems cerrados en la semana | Bloqueos / notas |
| --- | --- | --- | --- | --- | --- |
| S1 | 2026-06-19 | Tramo 1 | — | — | — |
| S2 | 2026-06-26 | Tramo 1 | — | — | — |
| S3 | 2026-07-03 | Tramo 2 | — | — | — |
| S4 | 2026-07-10 | Tramo 3 | — | — | — |
| S5 | 2026-07-17 | Tramo 3 | — | — | — |
| S6 | 2026-07-24 | Tramo 4 | — | — | — |
| S7 | 2026-07-31 | Tramo 4 | — | — | — |
| S8 | 2026-08-07 | Tramo 5 | — | — | — |
| S9 | 2026-08-14 | Tramo 5 | — | — | — |
| S10 | 2026-08-21 | Tramo 5 (cierre) | — | — | — |

Regla de avance: si al cierre de una semana el camino crítico (Tramo 1 → BT-04 → BT-03/BT-06 → BT-10) acumula atraso, se difieren primero los ítems Should y Could antes de comprometer las Must del MVP (ver §6, riesgo 1).

## 8. Trazabilidad

Casos de uso (CU) que avanzan al cierre del plan:

| CU | Operación | US que lo cubren | BT que lo realizan |
| --- | --- | --- | --- |
| CU-01 | Guardar | US-01, US-02 | BT-01, BT-02, BT-04 |
| CU-02 | Recuperar (rango y metadatos) | US-03, US-04 | BT-03, BT-12 |
| CU-03 | Eliminar | US-05 | BT-03 |
| CU-04 | Verificar | US-06 | BT-03 |
| CU-05 | Listar | US-07 | BT-03, BT-12 |
| CU-06 | Activar proveedor activo y validación-en-seco | US-08, US-09 | BT-06, BT-07 |

Necesidades de negocio (NB) que avanzan:

| NB | Rol | Cómo avanza en este plan |
| --- | --- | --- |
| NB-07 | Principal | Se cierra el almacenamiento configurable con transparencia verificada sobre al menos dos proveedores soportados (adaptador local + doble en memoria; remoto como Should); criterio de éxito de cero diferencias observables verificado por BT-10 |
| NB-03 | Soporte | Avanza con el guardado y la verificación de presencia de evidencia (US-01, US-06) |
| NB-06 | Soporte | Avanza con la recuperación por rango/metadatos y el listado paginado (US-04, US-07) |

ADRs que gobiernan las decisiones técnicas de este plan:

| ADR | Gobierna |
| --- | --- |
| ADR-01 | Estilo hexagonal, puerto único y proveedores intercambiables (Tramo 1, Tramo 5) |
| ADR-02 | Superficie pública estable, frontera público/interno (Tramo 1, todos los tramos) |
| ADR-03 | Versionado del contrato y estabilidad del identificador lógico (BT-02, control de cambios) |
| ADR-04 | Transparencia, integridad byte a byte, tamaño máximo común, batería de contrato (Tramo 1, Tramo 5) |
| ADR-05 | Resguardo de credenciales que entra pero no sale y autorización a usuario raíz (Tramo 4) |

Downstream a 08: cada US comprometida dispara la creación o actualización de su caso de aceptación en 08 a partir de sus criterios Given/When/Then de 06 y de la batería de contrato (BT-10). Downstream a 09: el mecanismo físico de almacenamiento seguro en reposo queda delegado a 09 (intake §17.P.5, ADR-05).

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Mini-plan inicial de geovial-storage en modo equipo_n=1 (release-driven): objetivo único, 22 ítems comprometidos (US-01 a US-09 y BT-01 a BT-13, 108 SP) en cinco tramos por dependencias, orden de construcción, DoD por referencia a la canónica de 08 (pendiente), cuatro riesgos con mitigación, bitácora semanal y trazabilidad a CU-01 a CU-06, NB-07/NB-03/NB-06 y ADR-01 a ADR-05. |
