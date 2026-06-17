# Plan de pruebas — geovial-mobile

Proyecto: geovial-mobile
Documento: plan-pruebas_v1.0.md
Versión: 1.0
Estado: Propuesto
Fecha: 2026-06-15
Autor: Ingeniero QA / SDET (mobile)

## 1. Alcance del plan

Este plan cubre la fase de roadmap F2 (captura en campo y sincronización) de `geovial-mobile`, organizada en los tres tramos secuenciales del `mini-plan_v1.0.md` de 07. Comprende la verificación de los 7 CU (CU-01 a CU-07), las 5 RN (RN-01 a RN-05) y los NFR numéricos de P.10 (captura 100 % offline, cola ≥ 1000, ciclo de 100 cambios ≤ 30 s, reanudación sin pérdida, arranque ≤ 3 s).

Módulos incluidos: sesión (inicio online, relogueo por seguridad del dispositivo, deslogueo), almacén local y migraciones, cola de cambios, mapa y marcadores, captura de foto con resolución de coordenada, comentarios y etiquetas, carga manual por radio, y el ciclo de sincronización subir-luego-bajar con reanudación y convivencia con conflictos.

Módulos excluidos: la resolución de conflictos de marcadores (se realiza al cierre desde `geovial-web`, no en la app); el dominio y la persistencia autoritativos (residen en `geovial-api`); el motor interno de sincronización (lo provee `aplicada-sync` y se valida por contrato, no se reimplementa, ADR-03); plataformas distintas de Android (sin iOS ni Windows en v1, P.9).

## 2. Criterios de entrada

El plan, y cada tramo dentro de él, se ejecuta cuando se cumplen:

- Build verde del paquete de aplicación sin warnings tratados como error.
- Los ítems del tramo cumplen la Definition of Ready de 06: criterios de aceptación en Given/When/Then (DoR §3) y dobles de prueba disponibles (DoR §1.7: doble del adaptador de ubicación, de cámara, de almacén seguro, de conectividad y del motor de sincronización).
- Almacén local efímero disponible y migración inicial aplicable en el arranque (ADR-02).
- Doble del backend que implementa el contrato subir-luego-bajar, deduplica por identificador de origen y reporta conflictos sin abortar (ADR-03).
- Para los TC de NFR en dispositivo: dispositivo Android de referencia disponible por USB en modo desarrollador (P.8) y red móvil típica para el ciclo de sincronización.
- ADR aplicables al tramo ratificadas o en proceso de ratificación declarado (ADR-01 a ADR-05).

## 3. Criterios de salida

El plan se declara ejecutado con éxito, y cada tramo se cierra, cuando:

- Todos los TC del tramo (matriz de cobertura §2) están en verde.
- El gate de cobertura global (líneas ≥ 80 %, branches ≥ 70 %) y los pisos por capa (lógica ≥ 75 %, presentación ≥ 60 %) se cumplen sobre el alcance del tramo (intake §17 P.6).
- La suite de regresión acumulada hasta el tramo está en verde; ningún TC verde anterior pasó a rojo sin justificación.
- Los defectos blocker y críticos del tramo están cerrados y cada uno generó su TC de regresión (regla 08 §4.10).
- Para el cierre de F2 (release): los NFR numéricos de P.10 están validados en el dispositivo de referencia (TC-24, TC-25, TC-26) y la captura offline y la reanudación sin pérdida verificadas (TC-08, TC-12, TC-19); se cumplen todos los criterios de `criterios-validacion_v1.0.md`.

## 4. Riesgos de calidad

Cada riesgo lleva impacto, probabilidad y mitigación, alineado con los riesgos de negocio del intake §11 y los riesgos del mini-plan de 07 §6.

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Reanudación idempotente que no reconoce reenvíos por identificador de origen → duplicación de cambios | Media | Alto | TC-19 con corte simulado tras la primera confirmación; doble del backend que deduplica por identificador de origen; índice único de idempotencia verificado en TC-28 |
| Pérdida de cambios en la cola por un corte de conexión o por falta de espacio | Media | Alto | TC-19 (corte) y TC-24 (cola ≥ 1000); transacción local atómica entidad + cambio encolado (ADR-02); degradación por falta de espacio sin pérdida (ADR-04) |
| Demora del ciclo de distribución del paquete que retrasa la verificación en dispositivo real | Media | Medio | Verificar la mayor parte con dobles de adaptadores (ubicación, cámara, conectividad, motor de sync) sin dispositivo (DoR §1.7); reservar el dispositivo para los TC de NFR (TC-24, TC-25, TC-26) |
| Contrato de la librería de sincronización no publicado o incompatible | Media | Medio | Ready condicional y doble del motor que ejercita el contrato subir-luego-bajar (ADR-03); ejercitar el contrato real al integrar en el Tramo 3 |
| Georreferenciación imprecisa o foto sin ubicación incrustada que invente coordenada | Media | Medio | TC-10, TC-13, TC-17, TC-23 verifican degradación a pendiente de ubicación sin coordenada inventada (RN-01, ADR-04) |
| Acceso a sesión ajena en dispositivo compartido tras un deslogueo incompleto | Baja | Alto | TC-03 verifica que el deslogueo borra token y datos y que un segundo agente no accede a sesión ajena (RN-04, ADR-05) |
| Snapshot de pantallas críticas regenerado para que pase, perdiendo su valor | Baja | Medio | Política de regeneración con cambio justificado y revisión (estrategia-testing §6; regla 08 §4.10); TC-27 falla ante diferencia no aprobada |

## 5. Plan por tramo

El mini-plan de 07 define tres tramos secuenciales para equipo de un dev. Cada tramo lista su alcance de testing, recursos y entregables.

| Tramo | Alcance de testing | Recursos | Entregables |
| --- | --- | --- | --- |
| Tramo 1 — Esqueleto de sesión y almacén local (28 SP; BT-01, BT-02, BT-09, BT-10; US-01, US-02; CU-01) | CU-01 (inicio online, relogueo por seguridad del dispositivo, deslogueo, token seguro) y la migración inicial en el arranque; TC-01, TC-02, TC-03, TC-04, TC-26, TC-28; snapshot de login/relogueo (parte de TC-27); RN-04 y RN-05 | Doble del backend de sesión; doble del almacén seguro; doble de verificación por seguridad del dispositivo; almacén local efímero; dispositivo de referencia para TC-26 (arranque ≤ 3 s) | Suite de sesión y de ciclo de vida en verde; migración inicial verificada; cobertura por capa del tramo; snapshot baseline de login/relogueo |
| Tramo 2 — Captura georreferenciada (63 SP; BT-03..BT-08; US-03..US-10, US-14, US-15; CU-02, CU-03, CU-04, CU-05, CU-07) | Selección de relevamiento, marcadores, captura de foto con coordenada, comentarios y etiquetas, carga manual por radio, permisos centralizados y degradaciones; TC-05..TC-17, TC-22, TC-23, TC-24; snapshot de lista, mapa, detalle de observación (parte de TC-27); RN-01, RN-03, RN-05 | Dobles de ubicación, cámara y almacenamiento/galería; almacén local efímero; factoría de cola para TC-24 (≥ 1000); datasets de fotos con y sin ubicación incrustada | Suite de captura offline y de carga manual en verde; cola ≥ 1000 verificada; degradaciones por permiso, sin señal y sin espacio verificadas; snapshot baseline de pantallas de captura |
| Tramo 3 — Sincronización (32 SP; BT-11, BT-12, BT-13; US-11, US-12, US-13; CU-06; cierra US-04) | Ciclo subir-luego-bajar, detección de conectividad, reanudación idempotente, convivencia no bloqueante con conflictos; TC-18, TC-19, TC-20, TC-21, TC-25; snapshot de estado de sincronización (parte de TC-27); RN-02, RN-03; cierra el refresco de la lista (US-04, CU-02) | Doble de conectividad con corte y recuperación; doble o integración real del motor `aplicada-sync`; dispositivo de referencia y red móvil típica para TC-25 (ciclo de 100 cambios ≤ 30 s) | Suite de sincronización en verde; reanudación sin duplicación verificada; ciclo de 100 cambios ≤ 30 s medido; snapshot baseline de estado de sincronización; matriz de cobertura completa y verde |

Nota sobre dependencias entre tramos: US-04 (refrescar la lista con conexión, CU-02) se compromete en el Tramo 2 pero su ciclo de bajada (BT-12) se completa en el Tramo 3; el TC de refresco (parte de la cobertura de CU-02) se cierra al cerrar el Tramo 3. Las US Could (US-10, US-15) entran con un solo escenario por la excepción de la DoR §3.

## 6. Recursos

- Personas: un desarrollador que concentra diseño, implementación y ejecución de los tests (QA/SDET mobile, `equipo_n=1`); un revisor de release.
- Ambientes: ambiente de CI con almacén local efímero y dobles; dispositivo Android de referencia conectado por USB en modo desarrollador para los TC de NFR y de interfaz móvil; red móvil típica para el TC de tiempo de ciclo.
- Datasets: biblioteca de datos de prueba sintéticos versionada con el código (relevamientos por estado, marcadores con y sin conflicto, fotos con y sin ubicación incrustada, colas de ≥ 1000 y de 100 cambios, representaciones de error por código estable).
- Herramientas: framework de pruebas unitarias, framework de pruebas de integración con almacén local, framework de pruebas de interfaz móvil, framework de snapshot, doble de conectividad y de backend para el modo offline/sincronización, medidor de tiempo de ciclo y de arranque, reporte de cobertura por capa y analizador estático (todos por rol abstracto; ver estrategia-testing §3).

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Plan de pruebas inicial de geovial-mobile: alcance sobre F2 con módulos incluidos y excluidos; criterios de entrada y salida; siete riesgos de calidad con impacto, probabilidad y mitigación alineados con el intake §11 y el mini-plan §6; plan por los tres tramos del mini-plan de 07 con alcance de testing, recursos y entregables por tramo, incluida la dependencia de US-04 entre tramos; recursos de personas, ambientes, datasets y herramientas. |
