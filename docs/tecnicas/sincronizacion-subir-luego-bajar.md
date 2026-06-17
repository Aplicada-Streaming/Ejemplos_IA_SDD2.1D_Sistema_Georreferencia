# Sincronización subir-luego-bajar

> Nota técnica del proyecto **GeoVial**. Casos reales: el backend de sincronización
> ([ServicioSincronizacion](../../src/GeoVial.WebApi/Application/ServicioSincronizacion.cs)) y el motor
> cliente ([MotorSincronizacion](../../src/Aplicada.Sync/MotorSincronizacion.cs)).

## 1. Resumen

El agente de campo trabaja **sin conexión**: crea marcadores y observaciones que quedan en una **cola
local**. Cuando recupera red, ejecuta un **ciclo de sincronización** con una regla dura: **primero
sube** sus cambios locales al backend y **solo después baja** las novedades del relevamiento. Este
orden (RN-06) garantiza que el trabajo del agente llegue al servidor antes de mezclarse con las
actualizaciones de los demás, y —combinado con idempotencia y una marca monótona— hace que el ciclo
sea **seguro ante cortes**: reintentar no pierde ni duplica nada.

## 2. Fundamentos

### 2.1. Por qué subir antes de bajar (RN-06)

- **No perder trabajo local:** si bajáramos primero y aplicáramos cambios remotos sobre el estado
  local, podríamos pisar capturas todavía no enviadas.
- **Coherencia del relevamiento:** el backend integra lo del agente y *recién* calcula las novedades
  que le devuelve, evitando ciclos de ida y vuelta.

Es una **garantía**, no una opción configurable.

### 2.2. Idempotencia (RN-07)

Cada cambio porta un **identificador de origen** estable. Si la subida se corta y el cliente reenvía
el lote, el backend **reconoce los reenvíos** y no los reaplica. (Ver
[idempotencia-con-clave.md](idempotencia-con-clave.md).)

### 2.3. Marca de sincronización opaca y monótona (RC-06)

La **bajada** entrega solo las novedades **posteriores** a una **marca** que el cliente trae de su
último ciclo. La marca es **opaca** para el cliente (no la interpreta) y **monótona** (solo avanza).
Así la bajada es **incremental** y **repetible**.

### 2.4. Convivencia con conflictos (RN-03)

Los marcadores dentro de un mismo radio conviven como **conflicto** durante recolección/revisión; no
bloquean la sincronización. Se entregan marcados como tales y se resuelven al cierre (CU-13).

## 3. El protocolo

### 3.1. Subida (CU-10)

1. El cliente envía el lote de cambios locales **en orden de creación**.
2. El backend valida que el agente esté **asignado** al relevamiento y que esté **abierto**.
3. Aplica cada cambio; por cada id de origen **ya aplicado**, lo reconoce como reenvío (no duplica).
4. Marca la **subida como concluida** (compuerta `SubidaConcluida`).
5. Responde con `{ aplicados, reenviados, conflictos }`.

### 3.2. Bajada (CU-11)

1. El cliente pide las novedades aportando su **marca**.
2. El backend exige que la **subida del ciclo haya concluido** (RN-06); si no, `409 SUBIDA_NO_CONCLUIDA`.
3. Calcula los cambios posteriores a la marca y los entrega junto con una **marca nueva**.
4. Reinicia la compuerta para exigir una nueva subida en el próximo ciclo.

### 3.3. Estados de la sesión (cliente)

`NoAutenticada → Lista → Sincronizando → (Lista | Reanudable)`. Si la subida se interrumpe, la sesión
queda **reanudable**: el siguiente ciclo reenvía lo pendiente sin reaplicar lo confirmado.

## 4. Cómo lo aplica GeoVial

### 4.1. Backend

[ServicioSincronizacion](../../src/GeoVial.WebApi/Application/ServicioSincronizacion.cs):

- `SubirAsync`: valida asignación/estado, deduplica por id de origen, resuelve cada observación
  contra su marcador (del mismo lote o ya existente), detecta conflictos por radio y **concluye la
  subida**.
- `BajarAsync`: exige `SubidaConcluida`, interpreta la marca (`MARCA_INVALIDA` si no parsea), calcula
  novedades, **avanza la marca** ([MarcaSincronizacion](../../src/GeoVial.WebApi/Domain/Relevamiento.cs),
  monótona) y reinicia la compuerta.

### 4.2. Cliente (librería de sincronización)

[MotorSincronizacion](../../src/Aplicada.Sync/MotorSincronizacion.cs) contiene **solo la política**,
con dos puertos que el host móvil implementa
([Puertos.cs](../../src/Aplicada.Sync/Puertos.cs)):

```csharp
public async Task<ResumenCiclo> SincronizarAsync(CancellationToken ct = default)
{
    if (_sincronizando) return new ResumenCiclo(0, 0, 0, _estado);   // 5.C: no reentra
    _sincronizando = true;
    _estado = EstadoSesionSync.Sincronizando;
    try
    {
        var subidos = await SubirPendientesAsync(ct);   // 1) subir (idempotente, reanudable)
        var (bajados, enConflicto) = await BajarYAplicarAsync(ct);   // 2) recién entonces, bajar
        _estado = EstadoSesionSync.Lista;
        return new ResumenCiclo(subidos, bajados, enConflicto, _estado);
    }
    finally { _sincronizando = false; }
}
```

Si `SubirPendientesAsync` se topa con un backend inalcanzable, deja la sesión **reanudable** y **no
inicia la bajada**.

## 5. Ejemplos representativos (pruebas del repo)

Del cliente ([MotorSincronizacionTests](../../tests/Aplicada.Sync.Tests/MotorSincronizacionTests.cs)):

- **Orden subir-antes-de-bajar:** el log del backend registra `subir:a`, `subir:b`, `bajar:(null)`.
- **Corte reanudable:** si el backend cae tras confirmar el primer cambio, quedan los pendientes, la
  sesión queda `Reanudable` y **no se baja**.
- **Reanudación sin reaplicar:** al recuperarse, el reintento sube solo lo que faltaba.

Del backend ([RelevamientosTests](../../tests/GeoVial.WebApi.Tests/RelevamientosTests.cs)):

- **Reenvío idempotente:** reenviar el mismo lote da `aplicados=0, reenviados=N`.
- **Bajada sin subida → `409`** (RN-06).
- **Bajada por marca:** con la marca previa no trae novedades anteriores a ella.

## 6. Buenas prácticas

- **El orden es una invariante**, no un parámetro: nunca bajar antes de concluir la subida.
- **Idempotencia de extremo a extremo:** id de origen estable en el cliente + dedup en el backend.
- **Marca opaca:** el cliente la guarda y la devuelve tal cual; su semántica vive en el backend.
- **Reanudable por diseño:** conservar la cola hasta la **confirmación**; nunca borrar lo no
  confirmado.
- **El motor no resuelve conflictos de dominio:** los transporta como estado válido y difiere su
  resolución (al cierre, CU-13).
- **Política, no transporte, en el núcleo:** el motor cliente no conoce HTTP ni SQLite (puertos).

### 6.1. Olores (anti-patrones)

- Bajar y subir "en paralelo" o en orden invertido (rompe RN-06).
- Borrar de la cola **antes** de la confirmación del backend.
- Recalcular novedades sin marca (entrega todo siempre) o con marca no monótona (entrega de menos).

## 7. Referencias

- CU-10 / CU-11 y reglas RN-03, RN-06, RN-07, RC-06 de geovial-api; CU-03 de aplicada-sync
  (en [SDD2.1D/docs](../../SDD2.1D/docs/)).
- Patrones de **sincronización offline-first** y **colas de salida** (*outbox*).
- [idempotencia-con-clave.md](idempotencia-con-clave.md) y
  [puertos-y-adaptadores.md](puertos-y-adaptadores.md).

## 8. Control de cambios

| Versión | Fecha | Cambios |
|---|---|---|
| 1.0 | 2026-06-17 | Nota inicial sobre el protocolo subir-luego-bajar (backend y motor cliente) de GeoVial. |
