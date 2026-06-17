# Casos de prueba referenciales — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** casos-prueba-referenciales_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (AG-08), variante QA + SDET Library

## 1. Propósito

Catálogo de casos de prueba referenciales (TC-XX) de la librería `aplicada-sync`. Cada TC referencia al menos un CU, RN o NFR upstream (08_rules §4.10, anti-patrón de tests sin trazabilidad) y declara tipo, setup, pasos en Given/When/Then, expected output, actual output y status. Hay al menos un TC por CU crítico y por RN, y un TC por cada NFR con objetivo numérico. Los TC operan sobre dobles de las estrategias de extensión (almacén local, transporte, credencial, conectividad); ningún TC depende de servicios externos reales ni del orden de ejecución.

Estado inicial del catálogo: todos los TC están en estado Pendiente (la implementación arranca con el tramo R1 del mini-plan de 07; ver línea de base en 07 §9). El campo Actual se completa al ejecutar. Convención de status: Verde, Rojo, Pendiente, Deshabilitado con motivo.

## 2. Catálogo de casos de prueba

### TC-01 — inicializar-sesion-completa

- Tipo: unit
- Cubre: CU-01 (CA-01); US-01
- Setup: doble de almacén local accesible vacío; doble de transporte; proveedor de credencial vigente; configuración completa con identificador de host "host-01".
- Pasos: Given una configuración completa con almacén local accesible, backend remoto y credencial vigente, When el host solicita inicializar la sesión, Then el motor devuelve un identificador de sesión no vacío y estado "listo".
- Expected: identificador de sesión no vacío; estado "listo"; estructuras de metadatos creadas en el almacén local.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-02 — inicializar-rechaza-config-incompleta

- Tipo: unit
- Cubre: CU-01 (CA-02); US-01
- Setup: configuración a la que le falta la referencia al backend remoto.
- Pasos: Given una configuración sin backend remoto, When el host solicita inicializar la sesión, Then el motor rechaza con CONFIGURACION_INCOMPLETA y no crea sesión ni estructuras parciales.
- Expected: código de error CONFIGURACION_INCOMPLETA con el campo faltante; ninguna sesión creada; almacén local sin estructuras parciales.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-03 — inicializar-sin-credencial-no-autenticada

- Tipo: unit
- Cubre: CU-01 (CA-04); US-01
- Setup: configuración completa pero sin proveedor de credencial.
- Pasos: Given una configuración sin proveedor de credencial, When el host inicializa la sesión, Then el motor devuelve estado "no autenticada" y admite encolar pero no ejecutar la sincronización.
- Expected: estado "no autenticada"; encolar permitido; ejecutar rechazado por falta de credencial.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-04 — encolar-cambio-nuevo-incrementa-cola

- Tipo: unit
- Cubre: CU-02 (CA-01); RN-02; US-03
- Setup: sesión inicializada con la cola vacía.
- Pasos: Given una sesión inicializada con cola vacía, When el host encola un cambio con identificador "chg-100", Then el motor confirma el encolado y reporta tamaño de cola igual a 1.
- Expected: confirmación de encolado; tamaño de cola = 1; orden de creación conservado.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-05 — encolar-reencolado-no-duplica

- Tipo: unit
- Cubre: CU-02 (CA-02); RN-02; US-04
- Setup: cola que ya contiene el cambio "chg-100" sin subir.
- Pasos: Given una cola con "chg-100" pendiente, When el host vuelve a encolar "chg-100", Then el motor conserva una sola entrada y reporta tamaño de cola igual a 1.
- Expected: una sola entrada para "chg-100"; tamaño de cola = 1; carga útil actualizada sin incrementar la cola.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-06 — encolar-rechaza-sin-identificador

- Tipo: unit
- Cubre: CU-02 (CA-03); US-03
- Setup: sesión inicializada; cambio sin identificador estable.
- Pasos: Given una sesión inicializada, When el host intenta encolar un cambio sin identificador, Then el motor rechaza con IDENTIFICADOR_CAMBIO_AUSENTE y deja la cola inalterada.
- Expected: código IDENTIFICADOR_CAMBIO_AUSENTE; cola inalterada.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-07 — ejecutar-orden-subir-antes-de-bajar

- Tipo: integration
- Cubre: CU-03 (CA-01); RN-01; US-05; NFR Orden
- Setup: sesión autenticada con 2 cambios pendientes; doble de transporte que registra la secuencia de llamadas (subidas y bajadas) con marca temporal de orden.
- Pasos: Given una sesión autenticada con 2 pendientes y backend alcanzable, When el host ejecuta la sincronización, Then el motor sube primero los 2 cambios y solo después baja, devolviendo un resumen con 2 subidos antes de cualquier bajada.
- Expected: en la secuencia registrada por el doble de transporte, ninguna bajada precede a la confirmación de la última subida; resumen con 2 subidos y N bajados; estado final "listo".
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-08 — ejecutar-cola-vacia-omite-subida

- Tipo: integration
- Cubre: CU-03 (CA-02); US-05
- Setup: sesión autenticada con cola vacía; backend alcanzable con actualizaciones disponibles.
- Pasos: Given una sesión autenticada con cola vacía, When el host ejecuta la sincronización, Then el motor omite la subida, reporta 0 subidos y baja las actualizaciones disponibles.
- Expected: 0 subidos; bajada ejecutada; ninguna llamada de subida en el doble de transporte.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-09 — ejecutar-corte-en-subida-no-baja-y-deja-reanudable

- Tipo: integration
- Cubre: CU-03 (CA-03); RN-01; US-05; NFR Reanudación
- Setup: sesión con 3 pendientes; doble de transporte que confirma el primero y luego simula backend inalcanzable.
- Pasos: Given una sesión con 3 pendientes donde el backend deja de responder tras confirmar el primero, When el host ejecuta la sincronización, Then el motor detiene el ciclo con BACKEND_INALCANZABLE, conserva 2 pendientes, no baja actualizaciones y deja la sesión reanudable.
- Expected: código BACKEND_INALCANZABLE; 2 pendientes conservados; cero bajadas registradas; estado "reanudable".
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-10 — reanudar-reenvia-solo-faltantes

- Tipo: integration
- Cubre: CU-06 (CA-01); RN-01, RN-02; US-12; NFR Reanudación
- Setup: almacén persistente efímero con una sesión reanudable de 5 cambios, 2 ya confirmados antes del corte; backend nuevamente alcanzable que reconoce por identificador.
- Pasos: Given una sesión reanudable con 5 cambios, 2 ya confirmados, When el host reanuda, Then el motor reenvía solo los 3 restantes, recién entonces baja y reporta 3 nuevos confirmados.
- Expected: exactamente 3 subidas efectivas; bajada solo tras concluir la subida; resumen con 3 nuevos confirmados; 0 duplicados.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-11 — reanudar-reconoce-por-identificador-sin-duplicar

- Tipo: integration
- Cubre: CU-06 (CA-02); RN-02; US-12; NFR Idempotencia
- Setup: sesión reanudable donde el backend ya recibió los 5 cambios pero el motor no registró la confirmación; doble de transporte que reconoce los 5 por identificador sin reaplicar.
- Pasos: Given una sesión reanudable con 5 cambios recibidos pero no registrados, When el host reanuda, Then el backend reconoce los 5 por identificador, no aplica ninguno de nuevo y el motor procede a la bajada sin duplicar.
- Expected: 0 aplicaciones efectivas nuevas en el backend; 5 reconocidos como ya recibidos; bajada ejecutada; conjunto aplicado igual al esperado.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-12 — propiedad-no-duplicacion-por-identificador

- Tipo: property-based
- Cubre: RN-02; CU-02, CU-03, CU-06; US-04; NFR Idempotencia
- Setup: generador property-based de secuencias arbitrarias de encolados con identificadores repetidos y reenvíos tras cortes en posiciones arbitrarias; doble de transporte idempotente por identificador.
- Pasos: Given cualquier secuencia generada de encolados, reintentos y reanudaciones, When se ejecuta el ciclo hasta completar, Then cada identificador estable produce exactamente una aplicación efectiva en el backend y una en el local.
- Expected: para todo caso generado, conteo de aplicaciones efectivas por identificador = 1; ningún contraejemplo; toda semilla que falle se fija como TC de regresión.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-13 — propiedad-orden-subir-antes-de-bajar

- Tipo: property-based
- Cubre: RN-01; CU-03, CU-04, CU-06; US-05; NFR Orden
- Setup: generador property-based de colas no vacías de tamaño arbitrario; doble de transporte que registra el orden global de llamadas.
- Pasos: Given cualquier cola no vacía generada, When se ejecuta un ciclo (manual, disparado por conectividad o reanudado), Then ninguna bajada se observa antes de confirmar la última subida.
- Expected: para todo caso generado, 0 bajadas antes de la última confirmación de subida; ningún contraejemplo.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-14 — capacidad-cola-1000-pendientes

- Tipo: integration
- Cubre: NFR Capacidad de cola (>= 1000); CU-02, CU-05; US-03, US-10; ADR-04
- Setup: generador determinista (semilla fija) de 1000 cambios con identificadores únicos; doble de almacén local persistente efímero.
- Pasos: Given una cola cargada con 1000 cambios únicos, When el host consulta el estado y luego ejecuta la sincronización, Then el encolado, la consulta y la ejecución funcionan sin degradación funcional y el tamaño reportado coincide con las entradas únicas.
- Expected: tamaño reportado = 1000; consulta no altera la cola; ejecución correcta; sin error de capacidad.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-15 — disparo-automatico-ante-red-recuperada

- Tipo: integration
- Cubre: CU-04 (CA-01); RN-01; US-08
- Setup: sesión autenticada con disparo automático habilitado y 1 cambio pendiente; doble de fuente de conectividad que emite "red disponible".
- Pasos: Given una sesión autenticada con disparo habilitado y 1 pendiente, When el detector emite un evento de red disponible, Then el motor dispara un ciclo y notifica al host el resultado con 1 subido.
- Expected: un ciclo disparado; notificación al host con 1 subido; orden subir-antes-de-bajar respetado.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-16 — convivencia-con-conflicto-sin-abortar

- Tipo: integration
- Cubre: CU-03 (CA-04); RN-03; US-06; NFR Continuidad ante conflicto
- Setup: sesión autenticada; doble de transporte cuya bajada incluye una entidad marcada en conflicto.
- Pasos: Given una bajada que incluye una entidad marcada en conflicto, When el host ejecuta la sincronización, Then el motor aplica la actualización en conflicto sin abortar y la reporta como elemento en conflicto en el resumen.
- Expected: ciclo concluido (no abortado); entidad en conflicto aplicada como estado válido; resumen lista 1 elemento en conflicto; el motor no resuelve el conflicto.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-17 — exclusion-mutua-no-segundo-ciclo

- Tipo: integration
- Cubre: CU-03 (flujo 5.C); US-07
- Setup: sesión con un ciclo de sincronización ya en curso (doble de transporte con latencia controlada que mantiene el ciclo activo).
- Pasos: Given un ciclo en curso para una sesión, When el host solicita ejecutar otra vez, Then el motor no inicia un segundo ciclo y devuelve el estado de la ejecución vigente.
- Expected: un único ciclo activo; la segunda solicitud devuelve el estado vigente sin iniciar otro ciclo.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-18 — descarte-rebote-conectividad

- Tipo: integration
- Cubre: CU-04 (CA-03); US-09
- Setup: sesión con disparo habilitado y un ciclo en curso; doble de fuente de conectividad que emite dos eventos de red disponible en una ventana breve.
- Pasos: Given una sesión con disparo habilitado y un ciclo en curso, When el detector emite dos eventos de red disponible en 1 segundo, Then el motor no inicia ciclos paralelos y mantiene un único ciclo activo.
- Expected: un único ciclo activo; eventos redundantes ignorados; sin reentrada.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-19 — consultar-estado-progreso-y-conflictos

- Tipo: unit
- Cubre: CU-05 (CA-01, CA-02, CA-03); RN-03; US-10, US-11
- Setup: sesión inicializada con 4 pendientes; variante con ciclo en curso que ya subió 1 de 3; variante con 2 elementos en conflicto reportados.
- Pasos: Given una sesión con 4 pendientes sin ciclo, When el host consulta el estado, Then devuelve "listo" y 4 pendientes; Given un ciclo que subió 1 de 3, Then devuelve "sincronizando" con 1 subido y 2 restantes; Given 2 conflictos reportados, When el host los consulta, Then devuelve 2 identificadores en conflicto como convivientes y no resueltos.
- Expected: estado y conteos exactos por variante; la consulta no altera la cola; conflictos marcados como convivientes y no resueltos por el motor.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-20 — tiempo-lote-100-bajo-30s

- Tipo: integration (rendimiento)
- Cubre: NFR Tiempo de sincronización de lote (<= 30 s); CU-03; ADR-05
- Setup: cola de 100 cambios; backend de prueba que simula latencia de red móvil típica; medición del tiempo total del ciclo subir-luego-bajar.
- Pasos: Given una cola de 100 cambios y un backend con latencia móvil simulada, When el host ejecuta la sincronización, Then el ciclo completo concluye en <= 30 s.
- Expected: tiempo total del ciclo <= 30 s en las condiciones de medición declaradas; medición reproducible por configuración fija.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-21 — compatibilidad-superficie-publica-quick-start

- Tipo: contract / snapshot
- Cubre: ADR-03; contrato §6; verificación post-publicación (intake §17 P.8); CU-01 a CU-06; BT-14
- Setup: paquete publicado restaurado en un proyecto limpio; baseline de snapshot del contrato (conjunto de operaciones, formas de resumen y estado, conjunto de códigos de error); matriz de compatibilidad.
- Pasos: Given el paquete publicado restaurado en un proyecto limpio, When se ejecuta el quick-start del contrato, Then reproduce el comportamiento del contrato y el snapshot coincide con la baseline; un cambio incompatible queda detectado contra la matriz de compatibilidad.
- Expected: quick-start reproduce el contrato; snapshot del contrato sin diferencias no justificadas; cualquier cambio incompatible exige incremento de versión mayor; un quick-start que no reproduzca el comportamiento bloquea la publicación.
- Actual: pendiente de ejecución.
- Status: Pendiente.

## 3. Resumen de cobertura del catálogo

| CU / RN / NFR | TC que lo cubren |
| --- | --- |
| CU-01 | TC-01, TC-02, TC-03, (TC-21) |
| CU-02 | TC-04, TC-05, TC-06, TC-12, TC-14 |
| CU-03 | TC-07, TC-08, TC-09, TC-13, TC-16, TC-17, TC-20 |
| CU-04 | TC-15, TC-18, TC-13 |
| CU-05 | TC-19, TC-14 |
| CU-06 | TC-10, TC-11, TC-12, TC-13 |
| RN-01 | TC-07, TC-09, TC-13 |
| RN-02 | TC-04, TC-05, TC-11, TC-12 |
| RN-03 | TC-16, TC-19 |
| NFR Tiempo de lote | TC-20 |
| NFR Capacidad de cola | TC-14 |
| NFR Reanudación sin pérdida | TC-09, TC-10 |
| NFR Idempotencia | TC-11, TC-12 |
| NFR Orden | TC-07, TC-13 |
| NFR Continuidad ante conflicto | TC-16 |

Sin TC huérfanos: cada TC referencia al menos un CU, RN o NFR. Sin requisitos huérfanos: cada CU, cada RN y cada NFR con objetivo numérico tiene al menos un TC. El detalle de la trazabilidad cruzada vive en `matriz-cobertura-pruebas_v1.0.md`.

## 4. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Catálogo inicial de 21 casos de prueba referenciales (TC-01 a TC-21) de aplicada-sync, con tipo, setup, pasos Given/When/Then, expected, actual y status, derivados de los criterios de aceptación de CU-01 a CU-06 (02), de RN-01 a RN-03 y de los NFR del intake §17 P.10 / 05 §8. Incluye property-based para invariantes (TC-12, TC-13), rendimiento/carga para NFR numéricos (TC-14, TC-20) y verificación de compatibilidad de superficie pública (TC-21). Todos en estado Pendiente como línea de base previa a R1. |
