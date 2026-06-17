# Flujo de ejecución — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** flujo-ejecucion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer

## 1. Objetivo

Describir paso a paso el motor de procesamiento del ciclo de sincronización subir-luego-bajar, con sus transformaciones de datos, sus puntos de corte y sus transiciones de estado. Complementa el documento maestro (`arquitectura-solucion_v1.0.md`) detallando cómo se materializa la garantía de orden (RN-01), la idempotencia (RN-02) y la convivencia con conflictos (RN-03). Cubre el ciclo disparado manualmente (CU-03), el disparado por conectividad (CU-04) y la reanudación de una subida parcial (CU-06).

## 2. Estados de la sesión

El ciclo opera sobre una sesión que transita entre estos estados (conjunto cerrado, parte del contrato):

- listo: la sesión está inicializada y autenticada; puede ejecutar un ciclo.
- no autenticada: inicializada sin credencial vigente; admite encolar, no ejecutar.
- sincronizando: hay un ciclo activo; admite consulta de progreso, no un segundo ciclo.
- reanudable: el último ciclo quedó con una subida parcial tras un corte.

## 3. Pipeline subir-luego-bajar (ciclo manual, CU-03)

Entrada: solicitud de ejecución sobre una sesión en estado listo. Salida: resumen del ciclo y sesión en estado listo.

1. Verificación de precondiciones. El motor confirma que la sesión está inicializada y autenticada y que no hay otro ciclo activo. Si hay un ciclo activo, devuelve el estado de la ejecución vigente y termina sin efectos (CU-03 5.C).
2. Apertura del ciclo. El motor marca la sesión como sincronizando, genera el identificador de correlación del ciclo y toma la cola de pendientes en su orden de creación.
3. Decisión de fase de subida. Si la cola está vacía, registra cero subidos y salta al paso 7 (fase de bajada) (CU-03 5.A). Si no, continúa.
4. Fase de subida (en orden estricto). Por cada pendiente, en orden de creación:
   - El motor envía el cambio al backend remoto por su identificador de cambio estable, usando la credencial vigente.
   - Si el backend confirma, el motor retira la entrada de la cola y avanza la marca de progreso (transformación: cola menos la entrada confirmada; progreso al identificador confirmado).
   - Si el backend reconoce el identificador como ya recibido, lo confirma sin reaplicar (idempotencia, RN-02).
5. Punto de corte de la subida. Si el backend deja de responder, el motor detiene el ciclo: conserva los pendientes no confirmados en la cola, no inicia la bajada y deja la sesión recuperable o reanudable; devuelve BACKEND_INALCANZABLE o SUBIDA_INCOMPLETA (CU-03 errores). Fin del ciclo en estado reanudable.
6. Verificación de fin de subida. El motor confirma que no quedan pendientes confirmables. Solo si esta verificación pasa, habilita la fase de bajada (compuerta de orden, RN-01).
7. Fase de bajada. El motor solicita al backend las actualizaciones posteriores a la marca de última sincronización; aplica cada actualización al almacén local una sola vez por su identidad (RN-02). Si una actualización viene marcada en conflicto, la aplica como estado válido, la registra como conviviente y no aborta (RN-03; CU-03 5.B).
8. Cierre del ciclo. El motor avanza la marca de última sincronización, deja la sesión en estado listo y arma el resumen: cantidad de subidos, cantidad de bajados, elementos en conflicto, estado final.

Transformaciones de datos clave: la cola decrece a medida que la subida confirma; la marca de progreso registra el último confirmado; la marca de última sincronización avanza solo tras la bajada; el resumen agrega los contadores del ciclo.

## 4. Disparo automático por conectividad (CU-04)

1. La fuente de eventos de conectividad notifica una transición a red disponible.
2. El observador de conectividad verifica que el disparo automático esté habilitado y la sesión autenticada; si no, registra el evento como ignorado o notifica que se requiere credencial (CU-04 errores), y termina.
3. El observador comprueba que no haya un ciclo activo. Ante eventos redundantes por rebote, ignora los sobrantes y no inicia ciclos paralelos (CU-04 5.C).
4. El observador dispara internamente el pipeline del paso 3 de este documento (delegado en el flujo de CU-03), que respeta el mismo orden subir-antes-de-bajar.
5. Al concluir, el motor notifica al host el resultado del ciclo.

## 5. Reanudación de una subida parcial (CU-06)

Entrada: sesión en estado reanudable. Salida: resumen de reanudación y sesión en estado listo (o nuevamente reanudable ante un nuevo corte).

1. El host solicita reanudar (o vuelve a ejecutar; el motor reconoce la sesión reanudable como reanudación).
2. El motor identifica los pendientes no confirmados a partir de la cola y de la marca de progreso. Ante inconsistencia entre ambas, adopta la cola persistida como fuente de verdad y registra la inconsistencia (PROGRESO_INCONSISTENTE; CU-06).
3. Fase de subida reanudada (en orden estricto). El motor reenvía únicamente los no confirmados, en orden de creación; el backend reconoce por identificador los ya recibidos y los confirma sin reaplicar (RN-02). Si todos ya habían sido recibidos, ninguno se reaplica y procede a la bajada (CU-06 5.A).
4. Punto de corte de la reanudación. Si la conectividad cae otra vez, conserva el avance, deja la sesión nuevamente reanudable y no inicia la bajada (CU-06 5.B). Fin en estado reanudable.
5. Verificación de fin de subida. Misma compuerta de orden que el ciclo normal (RN-01).
6. Fase de bajada y cierre. Igual que los pasos 7 y 8 del ciclo manual. El resumen de reanudación distingue cambios efectivamente nuevos confirmados de cambios reconocidos como ya recibidos.

## 6. Invariantes que el pipeline preserva

| Invariante | Cómo la preserva el pipeline | Paso(s) |
| --- | --- | --- |
| RN-01 orden subir-antes-de-bajar | La fase de bajada solo se habilita tras la verificación de fin de subida; un corte no la dispara | 5, 6, 7 (y 5.4, 5.5 en reanudación) |
| RN-02 idempotencia | Confirmación por identificador estable en la subida; aplicación única por identidad en la bajada | 4, 7, 5.3 |
| RN-03 convivencia con conflicto | La actualización en conflicto se aplica como válida, se registra y no aborta el ciclo | 7 |
| Exclusión mutua del ciclo | Un solo ciclo activo por sesión; segundo disparo devuelve el estado vigente | 1, 4.3 |

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU cubiertos | CU-03, CU-04, CU-06 (con apoyo de CU-01, CU-02, CU-05 para precondiciones y observabilidad) |
| RN aplicables | RN-01, RN-02, RN-03 |
| ADRs que lo gobiernan | ADR-05 (orden), ADR-06 (reanudación), ADR-07 (idempotencia), ADR-08 (conflicto) |
| Componentes | Orquestador del ciclo; ejecutores de fase de subida y de bajada; observador de conectividad; registro de estado y progreso; cola |
| Tests previstos (08) | Orden subir-antes-de-bajar; cola vacía omite subida; corte no dispara bajada; convivencia con conflicto; reanudación reenvía solo faltantes; no reentrada por rebote |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Flujo de ejecución inicial del pipeline subir-luego-bajar: ciclo manual, disparo por conectividad, reanudación de subida parcial, transiciones de estado e invariantes preservadas. Derivado de CU-03/04/06, RN-01/02/03 y los ADR-05/06/07/08. |
