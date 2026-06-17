# Flujo de ejecución — Pipeline de sincronización subir-luego-bajar — geovial-api

**Proyecto:** geovial-api
**Documento:** flujo-ejecucion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer

## 1. Objetivo

Este documento describe paso a paso el pipeline de sincronización de `geovial-api`, la única orquestación de varios pasos del backend que justifica un flujo de ejecución dedicado (regla 05 §2.2, rest-api con orquestación). El pipeline coordina dos fases estrictamente ordenadas —subida de cambios locales (CU-10) y bajada de actualizaciones (CU-11)— con tolerancia a conflictos de marcadores (RN-03, ADR-06), orden subir-antes-de-bajar (RN-06, ADR-07) e idempotencia por identificador de origen (RN-07, ADR-08). Está dirigido al equipo que implementa el backend de sincronización y a las categorías 06 (backlog técnico) y 08 (testing de integración del ciclo). El detalle del cliente de campo y de la librería de sincronización vive en los proyectos `geovial-mobile` y `aplicada-sync`; aquí se describe el lado servidor.

## 2. Actores y precondiciones del pipeline

- Actor primario: el cliente de campo del agente, que presenta un token bearer vigente (CU-03, ADR-03).
- Precondición de alcance: el relevamiento destino existe y está asignado al agente (RC-05); el relevamiento no está cerrado para la subida.
- Precondición de idempotencia: cada cambio del lote porta un identificador de origen estable (RN-07); la operación de subida porta una clave de idempotencia (ADR-08).
- Precondición de orden: la bajada del ciclo no se atiende hasta concluir la subida del mismo ciclo (RN-06, marca con `subida_concluida`).

## 3. Vista general del pipeline

```text
Fase A — Subida (CU-10)                         Fase B — Bajada (CU-11)
[A1] Autenticar y autorizar                     [B1] Autenticar y autorizar
[A2] Validar lote y relevamiento abierto        [B2] Verificar subida concluida (RN-06)
[A3] Deduplicar por identificador de origen     [B3] Validar marca aportada (RC-06)
[A4] Aplicar cambios nuevos en transacción      [B4] Calcular novedades posteriores a la marca
[A5] Registrar conflictos sin bloquear (RN-03)  [B5] Entregar novedades + marca nueva
[A6] Marcar subida concluida del ciclo          [B6] Adoptar marca al confirmar el cliente
[A7] Responder resultado de subida
```

La Fase B no comienza hasta que la Fase A del mismo ciclo concluyó (compuerta del paso B2 sobre el estado `subida_concluida` de la marca; RN-06, ADR-07).

## 4. Fase A — Subida de cambios locales (CU-10)

Paso A1 — Autenticar y autorizar. El backend valida el token bearer y resuelve el rol y el alcance del agente sobre el relevamiento (ADR-03, CU-18). Si el agente no tiene el relevamiento asignado, rechaza con RELEVAMIENTO_NO_ASIGNADO (403) y no aplica nada. Transformación de datos: token → identidad y alcance validados.

Paso A2 — Validar lote y estado del relevamiento. El backend verifica que el relevamiento esté abierto (no cerrado) y que el lote esté bien formado y que cada cambio porte identificador de origen. Si el relevamiento fue cerrado, rechaza con RELEVAMIENTO_CERRADO (409); si un cambio no porta identificador o viola la estructura, rechaza el lote completo con LOTE_MALFORMADO (400) sin aplicar nada. Transformación: lote recibido → lote validado en el orden en que se generó.

Paso A3 — Deduplicar por identificador de origen. Por cada cambio, el backend consulta si su identificador de origen ya fue aplicado (tabla ClaveIdempotencia / id_origen de la entidad). Los ya aplicados se reconocen como reenvío y no se reaplican; los nuevos pasan al paso A4 (RN-07, ADR-08). Transformación: lote validado → (cambios nuevos, reenvíos reconocidos).

Paso A4 — Aplicar cambios nuevos en transacción. El backend aplica cada cambio nuevo dentro de una transacción local atómica (ADR-02): crea o actualiza marcadores, observaciones, fotos, comentarios y etiquetas; el binario de cada foto se delega a la abstracción de almacenamiento, que devuelve la referencia lógica persistida en la tabla Foto (ADR-09). La identidad del marcador permanece estable ante movimiento o etiquetado (RC-01). Transformación: cambios nuevos → entidades persistidas + referencias de almacén.

Paso A5 — Registrar conflictos sin bloquear. Si un marcador del lote cae dentro del radio de un marcador existente, el backend lo incorpora y registra el conflicto (entidad ConflictoMarcadores, estado PENDIENTE) sin unificar y sin bloquear la subida (RN-03, ADR-06). El conflicto convive con la operación y se diferirá su resolución al cierre (CU-13). Transformación: marcadores próximos → conflicto registrado, marcador incorporado.

Paso A6 — Marcar subida concluida del ciclo. Al terminar de procesar el lote, el backend marca la subida del ciclo como concluida para ese par relevamiento-cliente (`subida_concluida`=verdadero en MarcaSincronizacion), habilitando la Fase B (RN-06). Transformación: estado del ciclo → subida concluida.

Paso A7 — Responder resultado de subida. El backend responde con cambios aplicados, reenvíos reconocidos y conflictos registrados (CU-10, postcondición). La respuesta queda asociada a la clave de idempotencia de la subida para devolverla idéntica ante un reenvío (ADR-08).

### 4.1 Flujo alternativo A.FA-01 — Subida parcial por corte de conexión

Disparador: la conexión se corta tras aplicar parte del lote. Los cambios ya confirmados en transacción quedan aplicados; el cliente reenvía el resto en una subida posterior. El paso A3 reconoce por identificador de origen lo ya confirmado y no lo reaplica (RN-07). El pipeline retoma en A3 con el remanente, sin pérdida ni duplicación (CU-10 FA-01, §15).

### 4.2 Flujo alternativo A.FA-02 — Lote con marcadores en conflicto

Disparador: el lote trae marcadores dentro del radio de marcadores existentes. El paso A5 los registra como conflicto y continúa; la subida no se bloquea (RN-03). El resultado del paso A7 informa los conflictos registrados.

## 5. Fase B — Bajada de actualizaciones (CU-11)

Paso B1 — Autenticar y autorizar. Igual que A1: token validado, alcance del agente sobre el relevamiento (ADR-03, CU-18). Si dejó de estar asignado, rechaza con RELEVAMIENTO_NO_ASIGNADO (403).

Paso B2 — Verificar subida concluida (compuerta de orden). El backend verifica que la subida del ciclo concluyó para ese par relevamiento-cliente (`subida_concluida`). Si el cliente solicita la bajada sin haber concluido la subida, rechaza con SUBIDA_NO_CONCLUIDA (409) y no entrega actualizaciones (RN-06, ADR-07). Transformación: estado del ciclo → habilitación de la bajada.

Paso B3 — Validar la marca aportada. El backend valida que la marca de última sincronización aportada por el cliente sea reconocible y no anterior a la registrada (RC-06). Si no es reconocible, rechaza con MARCA_INVALIDA (400) y solicita una sincronización completa. Transformación: marca del cliente → marca validada.

Paso B4 — Calcular novedades posteriores a la marca. El backend calcula el conjunto de cambios del relevamiento ocurridos después de la marca: marcadores, observaciones, fotos, comentarios, etiquetas, asignaciones y estado, usando el índice por `actualizado_en` (modelo lógico §2). Las entidades en conflicto se incluyen como estado válido (RN-03). Transformación: marca validada → conjunto de novedades incremental.

Paso B5 — Entregar novedades y marca nueva. El backend entrega el conjunto de novedades junto con una nueva marca de sincronización (posterior o igual a la anterior, RC-06) que el cliente guardará para el próximo ciclo (CU-11, postcondición). Si no hubo cambios, entrega un conjunto vacío y una marca equivalente (CU-11 FA-01).

Paso B6 — Adoptar la marca al confirmar el cliente. La marca nueva solo se adopta como punto de sincronización vigente cuando el cliente confirma haber aplicado las novedades, evitando retroceder ante un corte durante la bajada (CU-11 §15, RC-06). El estado `subida_concluida` se reinicia para el próximo ciclo. Transformación: confirmación del cliente → marca adoptada, ciclo cerrado.

### 5.1 Flujo alternativo B.FA-01 — Sin novedades

Disparador: no hubo cambios desde la marca. El paso B4 produce un conjunto vacío; B5 entrega vacío y una marca equivalente, sin obligar a aplicar nada (CU-11 FA-01).

### 5.2 Flujo alternativo B.FA-02 — Reasignación detectada en la bajada

Disparador: el relevamiento dejó de estar asignado al agente entre la subida y la bajada. El backend informa la pérdida de la asignación para que el cliente deje de mostrar el relevamiento, sin entregar más actualizaciones de su contenido (CU-11 FA-02).

## 6. Garantías del pipeline

- Orden: la bajada nunca se atiende antes de concluir la subida del mismo ciclo (RN-06, compuerta B2). Verificable de forma independiente en cada fase (02 §8).
- Idempotencia: reenviar un lote tras un corte no reaplica cambios confirmados (RN-07, A3); reintentar la subida con la misma clave devuelve el resultado registrado (ADR-08).
- Tolerancia a conflictos: ningún paso de subida ni de bajada se bloquea por un conflicto; el conflicto se registra y viaja como estado válido (RN-03, ADR-06).
- Atomicidad: cada cambio se aplica en una transacción local; un corte no deja efectos parciales no confirmados (ADR-02).
- Monotonía: la marca solo avanza y se adopta tras la confirmación del cliente (RC-06).
- Capacidad: el pipeline tolera un lote de al menos 1000 cambios por relevamiento sin pérdida ni duplicación (intake §17.P.10).

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU cubiertos | CU-10 (subida), CU-11 (bajada); soporte a CU-21 (idempotencia) |
| RN aplicables | RN-03 (conflictos), RN-06 (orden), RN-07 (idempotencia) |
| RC aplicables | RC-01 (identidad de marcador), RC-05 (asignación), RC-06 (marca monótona) |
| NB upstream | NB-04 (sincronización confiable) |
| ADRs que lo gobiernan | ADR-02 (transacciones), ADR-06 (conflictos), ADR-07 (orden de sync), ADR-08 (idempotencia), ADR-09 (almacenamiento de fotos) |
| NFR | Lote ≥ 1000 cambios; latencia de lectura de la bajada ≤ 300 ms en condiciones normales (intake §17.P.10) |
| Tests previstos (en 08) | Subida de lote nuevo; reenvío reconocido sin duplicar; conflicto incorporado sin bloquear; bajada sin subida rechazada con SUBIDA_NO_CONCLUIDA; bajada incremental por marca; marca inválida rechazada; reasignación detectada en la bajada |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Flujo de ejecución inicial del pipeline de sincronización subir-luego-bajar de geovial-api: dos fases ordenadas (subida CU-10, bajada CU-11) paso a paso con transformaciones de datos, flujos alternativos (subida parcial, conflictos, sin novedades, reasignación), garantías (orden, idempotencia, tolerancia a conflictos, atomicidad, monotonía, capacidad) y trazabilidad CU/RN/RC/NB/ADR/NFR. |
