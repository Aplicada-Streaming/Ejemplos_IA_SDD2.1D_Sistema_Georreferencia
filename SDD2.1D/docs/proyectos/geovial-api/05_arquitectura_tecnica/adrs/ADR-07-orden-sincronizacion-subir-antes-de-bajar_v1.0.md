# ADR-07 — Orden de sincronización subir antes de bajar

**Proyecto:** geovial-api
**Documento:** ADR-07-orden-sincronizacion-subir-antes-de-bajar_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Comunicación

## 1. Contexto

El agente de campo captura sin conexión y sincroniza al recuperar la red. En todo ciclo de sincronización, el backend debe incorporar primero los cambios locales que el cliente sube y solo después entregar las actualizaciones que el cliente baja (RN-06). Subir antes de bajar evita que el cliente sobrescriba, con datos del servidor, cambios locales aún no enviados, y hace predecible la sincronización, reduciendo el riesgo de pérdida o duplicación que el negocio identificó como de alto impacto (R-03). La bajada se calcula incrementalmente a partir de una marca de sincronización opaca y monótona por relevamiento y cliente (RC-06). El endpoint de subida tolera lotes de al menos 1000 cambios (intake §17.P.10). Cubre CU-10 (subida) y CU-11 (bajada).

## 2. Decisión

Se adopta un ciclo de sincronización de dos fases estrictamente ordenadas: la fase de subida (CU-10) incorpora el lote de cambios locales y solo cuando concluye se atiende la fase de bajada (CU-11), que entrega las novedades posteriores a la marca aportada por el cliente junto con una marca nueva. Si el cliente solicita la bajada sin haber concluido la subida del ciclo, el backend rechaza con SUBIDA_NO_CONCLUIDA y no entrega actualizaciones. La marca de sincronización es opaca para el cliente y solo avanza (RC-06): se adopta cuando el cliente confirma haber aplicado las novedades, evitando retroceder ante un corte. Una marca no reconocible se rechaza con MARCA_INVALIDA y obliga a una sincronización completa.

## 3. Estado

Aceptado el 2026-06-15. Decisión pre-tomada en el intake (§17.P.3, §17.P.11) y derivada de RN-06.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Dos fases ordenadas subir-luego-bajar (elegida) | Evita sobrescribir cambios locales no enviados; sincronización predecible; reduce R-03 (RN-06) | El cliente debe completar la subida antes de la bajada; una compuerta verifica el orden |
| Bajar antes de subir | El cliente recibe novedades primero | El servidor podría sobrescribir cambios locales aún no enviados; materializa el riesgo de pérdida; contradice RN-06 |
| Sincronización bidireccional simultánea con merge automático | Un solo paso | Merge automático decide conflictos sin criterio del jefe; contradice la tolerancia a conflictos (ADR-06); aumenta el riesgo de duplicación |
| Sincronización completa siempre (sin marca incremental) | Implementación simple | No escala con el volumen del relevamiento; incumple el objetivo de lectura; retransmite todo en cada ciclo |

## 5. Consecuencias positivas

1. La subida se aplica antes de cualquier entrega de actualizaciones, de modo que el servidor nunca sobrescribe cambios locales no enviados (RN-06).
2. La bajada incremental por marca monótona (RC-06) escala con el volumen de cambios sin retransmitir todo el relevamiento.
3. La marca solo se adopta tras la confirmación del cliente, evitando perder cambios ante un corte durante la bajada (CU-11, idempotencia y reintento).
4. El orden hace la sincronización verificable de forma independiente en cada fase (02 §8, decisión de recorte).

## 6. Consecuencias negativas y trade-offs

1. El cliente debe completar la subida antes de poder bajar; se acepta a cambio de la predecibilidad y la seguridad de datos (compuerta SUBIDA_NO_CONCLUIDA).
2. El backend mantiene una marca por relevamiento y cliente (MarcaSincronizacion), un estado adicional respecto de una sincronización sin marca.
3. Una marca no reconocible obliga a una sincronización completa; se acepta como salvaguarda de consistencia (MARCA_INVALIDA).

## 7. Implementación

- El control de orden subir-antes-de-bajar se materializa como una compuerta en la fase de bajada (CU-11) que verifica que la subida del ciclo concluyó (RN-06).
- La marca de sincronización se persiste por par relevamiento-cliente y solo avanza (RC-06); su valor es opaco para el cliente.
- El cálculo de novedades de la bajada selecciona los cambios posteriores a la marca aportada; las entidades en conflicto viajan como estado válido (ADR-06).
- El pipeline paso a paso de las dos fases vive en `flujo-ejecucion_v1.0.md`.
- Convención impuesta: ninguna bajada se atiende antes de concluir la subida del mismo ciclo.

## 8. Métricas de validación

- Subida aplicada antes de cualquier entrega de actualizaciones (RN-06, verificado en 08 sobre CU-10).
- Bajada sin subida concluida rechazada con SUBIDA_NO_CONCLUIDA (CU-11).
- Marca no reconocible rechazada con MARCA_INVALIDA y sincronización completa forzada (RC-06).
- El endpoint de subida tolera un lote de ≥ 1000 cambios por relevamiento sin pérdida ni duplicación (intake §17.P.10).

## 9. Referencias

- NB-04; CU-10, CU-11; RN-06; RC-06.
- Intake §11 R-03 (riesgo), §17.P.3 (comunicación), §17.P.10 (NFR de lote), §17.P.11.
- ADRs relacionadas: ADR-06 (tolerancia a conflictos), ADR-08 (idempotencia).
- `flujo-ejecucion_v1.0.md`; `contratos-rest_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de comunicación: ciclo de sincronización de dos fases ordenadas subir-luego-bajar con marca de sincronización opaca y monótona. Aceptada (pre-tomada en intake §17.P.3, §17.P.11). |
