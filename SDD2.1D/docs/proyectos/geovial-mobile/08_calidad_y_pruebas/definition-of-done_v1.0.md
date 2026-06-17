# Definition of Done — geovial-mobile

Proyecto: geovial-mobile
Documento: definition-of-done_v1.0.md
Versión: 1.0
Estado: Propuesto
Fecha: 2026-06-15
Autor: Ingeniero QA / SDET (mobile)

Esta es la Definition of Done canónica de `geovial-mobile`. Es la fuente única de la condición de terminación: el `mini-plan_v1.0.md` de 07 la referencia y no la redefine, y la `definition-of-ready_v1.0.md` de 06 (cuándo empezar) no se solapa con ella (cuándo terminar). Cada criterio responde a la pregunta "¿cómo se valida?" con una operación mecánica: un comando, un check del pipeline o una métrica del reporte. Las cuatro capas son acumulativas: el release exige la DoD de tramo, que exige la de BT y la de US.

## 1. DoD por capa

### 1.1 Historia de usuario (US)

- [ ] Los criterios de aceptación Given/When/Then de la US están cubiertos por al menos un TC del catálogo (`casos-prueba-referenciales_v1.0.md`), happy path y camino degradado (permiso, sin señal o corte de conexión cuando aplica). Validación: la matriz de cobertura (§2) referencia el TC de cada CU asociado a la US y el TC está en verde en el reporte de la suite.
- [ ] La US referencia al menos un CU y la RN aplicable; la trazabilidad no tiene huérfanas. Validación: revisión de la fila de la US contra la matriz CU↔Tests y RN↔Tests.
- [ ] La lógica de captura, de cola o de orquestación de sincronización de la US pasa sus pruebas unitarias y, si toca el almacén local o el contrato, sus pruebas de integración o de sincronización. Validación: la suite de unidad, integración y sincronización de la US está en verde en CI, ejecutada con dobles de los adaptadores de plataforma (sin dispositivo ni red).
- [ ] La US no inventa datos en la degradación: si depende de ubicación, cámara o espacio, la falta de permiso, señal o espacio produce el error declarado y conserva los datos sin inventar coordenada (RN-01, ADR-04). Validación: TC de degradación de la US en verde.
- [ ] Si la US toca una pantalla crítica, su render es estable contra el snapshot baseline. Validación: TC-27 (snapshot) en verde sin diferencias no aprobadas.
- [ ] La cobertura del código nuevo de la US no baja los pisos por capa. Validación: el gate de cobertura por capa del pipeline pasa (lógica 75 / infraestructura 70 / presentación 60).

### 1.2 Tarea técnica (BT)

- [ ] La BT compila sin warnings tratados como error. Validación: stage de build del pipeline en verde.
- [ ] La BT tiene sus pruebas verificables: el código nuevo está cubierto por pruebas unitarias y, si integra el almacén local, el contrato de sincronización o un componente de interfaz, por pruebas de integración, de sincronización o de interfaz móvil. Validación: la suite de la BT en verde y el delta de cobertura no baja los pisos por capa.
- [ ] La BT respeta la dependencia de capas hacia adentro (Presentación → Aplicación → Dominio local; la Infraestructura implementa los puertos que la Aplicación define) y no deja lógica de captura ni de sincronización en las vistas (ADR-01). Validación: revisión de arquitectura sobre el cambio y análisis estático sin issues críticos nuevos.
- [ ] La BT que persiste datos lo hace en una transacción local atómica (entidad + cambio encolado) y respeta el orden de creación y el identificador de origen de la cola (ADR-02, ADR-03). Validación: TC de integración del almacén local o de la cola en verde.
- [ ] La BT no introduce deuda no registrada. Validación: cualquier atajo queda como BT explícita en el backlog (excepción §2).

### 1.3 Tramo (sprint del mini-plan)

- [ ] Todos los ítems comprometidos del tramo cumplen su DoD de US o de BT. Validación: checklist por ítem del tramo en la bitácora de avance del mini-plan de 07.
- [ ] Los CU que avanzan en el tramo (trazabilidad por tramo de 07) tienen sus TC en verde. Validación: la matriz de cobertura del tramo está actualizada y verde.
- [ ] La suite de regresión completa acumulada hasta el tramo está en verde; ningún TC verde anterior pasó a rojo sin justificación. Validación: corrida de regresión en CI.
- [ ] El gate de cobertura global (líneas ≥ 80 %, branches ≥ 70 %) y los pisos por capa (lógica 75/70, infraestructura 70/60, presentación 60/50) se cumplen sobre el alcance del tramo. Validación: reporte de cobertura por capa del pipeline.
- [ ] La capacidad construida en el tramo es demostrable de punta a punta sobre el dispositivo de referencia o el ambiente de prueba (criterio de cierre de tramo de 07). Validación: demostración del journey del tramo.
- [ ] Los defectos blocker y críticos del tramo están cerrados y cada uno generó su TC de regresión. Validación: tablero de defectos sin blockers abiertos del tramo.
- [ ] Si el tramo construye un NFR de campo, su TC de NFR se midió o se dejó con plan de remediación declarado: cola ≥ 1000 (TC-24) en Tramo 2; ciclo de 100 cambios ≤ 30 s (TC-25) en Tramo 3; arranque ≤ 3 s (TC-26) en Tramo 1. Validación: corrida del TC de NFR del tramo en el dispositivo de referencia o nota de remediación.

### 1.4 Release

- [ ] Todos los criterios de validación de `criterios-validacion_v1.0.md` se cumplen. Validación: checklist de criterios de validación firmado.
- [ ] Cada CU (CU-01 a CU-07) está cubierto por al menos un TC verde y cada RN (RN-01 a RN-05) verificada. Validación: matriz CU↔Tests y RN↔Tests sin pendientes.
- [ ] Los NFR numéricos de P.10 están validados en el dispositivo de referencia: captura 100 % offline (TC-08, TC-12), cola ≥ 1000 (TC-24), ciclo de 100 cambios ≤ 30 s (TC-25), reanudación sin pérdida (TC-19) y arranque ≤ 3 s (TC-26). Validación: corrida de las pruebas de sincronización, de capacidad de cola y de tiempo de arranque en verde.
- [ ] El gate de cobertura global y por capa se cumple sobre el proyecto completo. Validación: reporte de cobertura del pipeline.
- [ ] El análisis estático no tiene issues críticos; el paquete de aplicación Android se construye, firma con el keystore resguardado y publica al canal de distribución interno. Validación: stages de análisis, firma y publicación del pipeline en verde (09).
- [ ] El snapshot de las pantallas críticas coincide con su baseline aprobado. Validación: TC-27 en verde sin diferencias no aprobadas.
- [ ] Las ADR que gobiernan el proyecto (ADR-01 a ADR-05) están ratificadas. Validación: índice de decisiones de 05 con las cinco en estado Aceptado.

## 2. Excepciones admitidas

- Deuda técnica documentada: un ítem puede declararse Done con un atajo conocido solo si la deuda queda registrada como BT explícita en el backlog con su plan de remediación; el atajo no puede violar una RN ni un NFR crítico.
- Capacidad Could diferida: una US Could Have (US-10, US-15) que no entre en el release no bloquea la DoD de release del camino principal; se documenta como exclusión de alcance, no como excepción.
- TC de NFR en dispositivo no medible por demora del ciclo de distribución del paquete: se difiere con plan de remediación declarado y el release no se declara Done hasta completar la medición; la cola ≥ 1000 (TC-24) admite verificación previa sin interfaz, pero el ciclo (TC-25) y el arranque (TC-26) exigen el dispositivo de referencia.
- Contrato de la librería de sincronización no publicado: la BT que lo consume se cierra contra un doble del motor que ejercita el contrato subir-luego-bajar (Ready condicional, mini-plan 07 §6), con el TC del contrato real planificado al integrar `aplicada-sync`.
- Criterio de NFR no cumplido: solo se acepta el release con un NFR fuera de objetivo mediante ADR explícita y plan de remediación (regla 08 §4.8); no se admite excepción silenciosa.
- Toda excepción se registra en la nota del ítem y la aprueba el rol QA / SDET (estrategia-calidad §4).

## 3. Vigencia

Este documento es la fuente canónica de la Definition of Done de `geovial-mobile`. Los planes de ejecución (el `mini-plan_v1.0.md` de 07) la referencian y no la redefinen. Cualquier cambio en los criterios versionables de esta DoD se registra en la §4 (control de cambios) y se comunica al equipo en la revisión del tramo siguiente (regla 08 §3.4). El mini-plan de 07 vincula su sección de Definition of Done a este documento.

## 4. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Definition of Done canónica inicial de geovial-mobile: DoD por las cuatro capas (US, BT, tramo, release) con criterios verificables mecánicamente (comando, check de pipeline o métrica de reporte), reconciliación del gate global de cobertura con los pisos por capa (lógica 75 / presentación 60), validación de los NFR numéricos de P.10 en el dispositivo de referencia en el release y excepciones explícitas (deuda registrada, capacidad Could diferida, TC de NFR no medible, contrato de sync no publicado, NFR fuera de objetivo solo con ADR). Es la DoD que referencia el mini-plan de 07. |
