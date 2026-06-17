# Idempotencia con clave (Idempotency-Key)

> Nota técnica del proyecto **GeoVial**. Casos reales: el middleware de idempotencia
> ([IdempotenciaMiddleware](../../src/GeoVial.WebApi/Api/IdempotenciaMiddleware.cs)) y la
> idempotencia por identificador de origen de la sincronización
> ([ServicioSincronizacion](../../src/GeoVial.WebApi/Application/ServicioSincronizacion.cs)).

## 1. Resumen

Una operación es **idempotente** si ejecutarla varias veces produce el **mismo efecto** que
ejecutarla una sola vez. Las redes fallan: un cliente que no recibe la respuesta **reintenta**, y sin
idempotencia ese reintento **duplica** el efecto (dos altas, dos cobros, dos marcadores). La técnica
de la **clave de idempotencia** permite que el cliente marque cada operación con un identificador
estable; el servidor reconoce el reintento y **devuelve el resultado ya registrado** en lugar de
volver a ejecutar.

En GeoVial esto sostiene la captura sin conexión (que reenvía lotes tras cortes) y cualquier
escritura reintentable (RN-07, CU-21).

## 2. Fundamentos

### 2.1. Métodos seguros e idempotentes (HTTP)

- **Seguro:** no cambia estado (`GET`, `HEAD`). No necesita idempotencia.
- **Idempotente por definición:** `PUT`, `DELETE` (repetirlos deja el mismo estado final).
- **No idempotente por naturaleza:** `POST` (cada uno tiende a **crear** algo nuevo). Aquí es donde
  la **clave de idempotencia** agrega la garantía que el método no da por sí mismo.

### 2.2. La idea de la clave

El cliente genera una **clave estable** por operación (un identificador único que **no cambia entre
reintentos** del *mismo* pedido) y la envía, por convención de industria, en el encabezado
`Idempotency-Key`. El servidor:

1. Si la clave es **nueva**: ejecuta, **registra** el resultado asociado a la clave y responde.
2. Si la clave **ya se procesó** (mismo contenido): **no** ejecuta de nuevo y **devuelve el resultado
   registrado**.
3. Si la clave se reutiliza con **contenido distinto**: **rechaza** (la clave identifica *una*
   operación; reusarla para otra es un error del cliente).
4. Si la operación original **está en curso**: responde "en curso", sin iniciar una segunda.

### 2.3. La huella de la solicitud

Para distinguir el caso (2) del (3), el servidor guarda una **huella** del contenido (un hash del
método + ruta + cuerpo). Misma clave + misma huella → reintento legítimo (reproducir). Misma clave +
huella distinta → reutilización indebida.

## 3. Cómo lo aplica GeoVial

GeoVial tiene **dos sabores** de idempotencia, complementarios:

### 3.1. Transversal por `Idempotency-Key` (CU-21)

Un **middleware** ([IdempotenciaMiddleware](../../src/GeoVial.WebApi/Api/IdempotenciaMiddleware.cs))
intercepta `POST/PUT/PATCH/DELETE` **cuando el cliente envía el encabezado** (es *opt-in*):

```csharp
if (!EsNoSegura(ctx.Request.Method) ||
    !ctx.Request.Headers.TryGetValue("Idempotency-Key", out var clave) || string.IsNullOrWhiteSpace(clave))
{
    await next(ctx);   // sin clave: pasa de largo
    return;
}
```

El registro vive en la tabla técnica
[ClaveIdempotencia](../../src/GeoVial.WebApi/Infrastructure/ClaveIdempotencia.cs) (clave única,
huella, resultado serializado, estado `EnCurso`/`Completada`). El flujo:

- **Nueva** → inserta `EnCurso`, ejecuta la operación, **captura la respuesta** (estado + cuerpo),
  marca `Completada` y la devuelve.
- **Completada + misma huella** → **reproduce** la respuesta guardada (con cabecera
  `Idempotent-Replayed`), sin reejecutar.
- **Huella distinta** → `409 CLAVE_REUTILIZADA_INCONSISTENTE`.
- **En curso** → `409` "operación en curso" (CU-21 5.A).
- Si la operación **falla**, se **libera** la clave para permitir un reintento corregido.

> Detalle de diseño: el middleware usa su **propio `DbContext`** (un *scope* aparte) para registrar
> la clave, de modo que su bookkeeping no se mezcle con la transacción de la operación.

### 3.2. Por identificador de origen en la sincronización (RN-07)

En la **subida** de sincronización, cada cambio capturado offline porta un **id de origen** estable.
El backend deduplica por ese id: si un marcador/observación con ese id ya fue aplicado, lo reconoce
como **reenvío** y no lo duplica (ver
[ServicioSincronizacion](../../src/GeoVial.WebApi/Application/ServicioSincronizacion.cs) y la nota
[sincronizacion-subir-luego-bajar.md](sincronizacion-subir-luego-bajar.md)). Es la **aplicación más
crítica** de RN-07, porque la captura sin conexión reenvía lotes enteros tras un corte.

La diferencia: la clave de `Idempotency-Key` es **por solicitud HTTP**; el id de origen es **por
entidad** dentro del lote.

## 4. Ejemplo representativo (prueba del repo)

De [IdempotenciaTests](../../tests/GeoVial.WebApi.Tests/IdempotenciaTests.cs): el alta repetida con
la misma clave **no duplica** y **reproduce** el mismo recurso.

```csharp
var cuerpo = new SolicitudCrearUsuario($"jg-{sufijo}", Clave, Rol.JefeGeneral);
var clave  = $"alta-{sufijo}";

var primera  = await PostConClaveAsync(c, "/api/v1/usuarios", cuerpo, clave);   // 201, crea
var reintento = await PostConClaveAsync(c, "/api/v1/usuarios", cuerpo, clave);  // 201, REPRODUCE

reintento.Headers.Contains("Idempotent-Replayed").Should().BeTrue();
(await reintento.Content.ReadFromJsonAsync<UsuarioDto>())!.Id
    .Should().Be(creado.Id);                  // el mismo usuario, no uno nuevo
lista.Count(u => u.NombreUsuario == $"jg-{sufijo}").Should().Be(1);   // sin duplicado
```

Y el rechazo por reutilización inconsistente (CA-03): misma clave, **contenido distinto** → `409`.

## 5. Buenas prácticas

- **La clave la genera el cliente** y debe ser **estable por intención** (la misma para todos los
  reintentos del mismo pedido; distinta para pedidos distintos). Un GUID por acción del usuario es lo
  habitual.
- **Comparar la huella** del contenido para no confundir dos operaciones bajo una misma clave.
- **Capturar el resultado** (estado + cuerpo) para devolver exactamente lo mismo en el reintento.
- **No bloquear la clave ante fallos**: si la operación falló, liberar para reintentar.
- **Opt-in cuando aplica:** no toda operación necesita exigir clave; ofrecerla en las reintentables.
- **Idempotencia natural primero:** diseñar `PUT`/`DELETE` idempotentes; reservar la clave para los
  `POST` que crean.
- **Retención:** las claves se conservan un tiempo acotado (política de retención); pasada la
  ventana, se reciclan.

### 5.1. Olores (anti-patrones)

- Reintentos sin clave que **duplican** datos (el riesgo que esto evita).
- Reusar una clave para operaciones distintas (rompe la semántica; por eso se rechaza).
- Registrar la clave en la **misma** transacción que la operación y dejarla "completada" aun si la
  operación se revirtió.

## 6. Referencias

- IETF — *The Idempotency-Key HTTP Header Field* (draft) y RFC 7231 (métodos seguros/idempotentes).
- Patrón de industria de claves de idempotencia (p. ej. APIs de pagos).
- RN-07 y CU-21 de geovial-api (en [SDD2.1D/docs](../../SDD2.1D/docs/)).

## 7. Control de cambios

| Versión | Fecha | Cambios |
|---|---|---|
| 1.0 | 2026-06-17 | Nota inicial sobre idempotencia con clave y por id de origen, con el middleware y la sincronización de GeoVial. |
