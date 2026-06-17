using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using GeoVial.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GeoVial.WebApi.Api;

/// <summary>
/// Idempotencia transversal de operaciones no seguras (CU-21, RN-07). Es opcional por solicitud:
/// se activa cuando el cliente envía el encabezado <c>Idempotency-Key</c> en un POST/PUT/PATCH/DELETE.
/// Una clave nueva ejecuta la operación y registra su resultado; un reintento con la misma clave y
/// el mismo contenido reproduce el resultado sin reejecutar; con contenido distinto se rechaza
/// (CLAVE_REUTILIZADA_INCONSISTENTE); mientras la original está en curso, responde conflicto.
/// El registro usa su propio contexto para no interferir con la transacción de la operación.
/// </summary>
public sealed class IdempotenciaMiddleware(RequestDelegate next)
{
    private const string Encabezado = "Idempotency-Key";
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext ctx)
    {
        if (!EsNoSegura(ctx.Request.Method) ||
            !ctx.Request.Headers.TryGetValue(Encabezado, out var valorClave) ||
            string.IsNullOrWhiteSpace(valorClave))
        {
            await next(ctx);
            return;
        }

        var clave = valorClave.ToString();
        ctx.Request.EnableBuffering();
        var huella = await ComputarHuellaAsync(ctx.Request);

        using var scope = ctx.RequestServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GeoVialDbContext>();

        var existente = await db.Set<ClaveIdempotencia>().AsNoTracking()
            .FirstOrDefaultAsync(k => k.Clave == clave, ctx.RequestAborted);
        if (existente is not null)
        {
            if (!string.Equals(existente.HuellaSolicitud, huella, StringComparison.Ordinal))
            {
                await EscribirProblemaAsync(ctx, StatusCodes.Status409Conflict, "CLAVE_REUTILIZADA_INCONSISTENTE",
                    "La clave de idempotencia ya se usó con un contenido distinto.");
                return;
            }

            if (existente.Estado == EstadoClaveIdempotencia.EnCurso)
            {
                await EscribirProblemaAsync(ctx, StatusCodes.Status409Conflict, "OPERACION_EN_CURSO",
                    "La operación con esa clave de idempotencia está en curso.");
                return;
            }

            await ReproducirAsync(ctx, existente.Resultado);
            return;
        }

        var registro = new ClaveIdempotencia(clave, huella);
        db.Set<ClaveIdempotencia>().Add(registro);
        try
        {
            await db.SaveChangesAsync(ctx.RequestAborted);
        }
        catch (DbUpdateException)
        {
            // Otra solicitud con la misma clave ganó la carrera (CU-21 5.A).
            await EscribirProblemaAsync(ctx, StatusCodes.Status409Conflict, "OPERACION_EN_CURSO",
                "La operación con esa clave de idempotencia está en curso.");
            return;
        }

        var original = ctx.Response.Body;
        using var buffer = new MemoryStream();
        ctx.Response.Body = buffer;
        try
        {
            await next(ctx);
        }
        catch
        {
            ctx.Response.Body = original;
            await DescartarAsync(db, registro);
            throw;
        }

        ctx.Response.Body = original;
        var cuerpo = buffer.ToArray();

        if (ctx.Response.StatusCode is >= 200 and < 300)
        {
            registro.Completar(Serializar(ctx.Response.StatusCode, ctx.Response.ContentType, cuerpo));
            await db.SaveChangesAsync(ctx.RequestAborted);
        }
        else
        {
            // La operación falló: no se bloquea la clave, para permitir un reintento.
            await DescartarAsync(db, registro);
        }

        if (cuerpo.Length > 0)
        {
            await original.WriteAsync(cuerpo, ctx.RequestAborted);
        }
    }

    private static bool EsNoSegura(string metodo)
        => HttpMethods.IsPost(metodo) || HttpMethods.IsPut(metodo) || HttpMethods.IsPatch(metodo) || HttpMethods.IsDelete(metodo);

    private static async Task<string> ComputarHuellaAsync(HttpRequest req)
    {
        using var ms = new MemoryStream();
        await req.Body.CopyToAsync(ms);
        req.Body.Position = 0;
        var cuerpo = ms.ToArray();

        var prefijo = Encoding.UTF8.GetBytes($"{req.Method}\n{req.Path}\n");
        var datos = new byte[prefijo.Length + cuerpo.Length];
        Buffer.BlockCopy(prefijo, 0, datos, 0, prefijo.Length);
        Buffer.BlockCopy(cuerpo, 0, datos, prefijo.Length, cuerpo.Length);
        return Convert.ToHexString(SHA256.HashData(datos)).ToLowerInvariant();
    }

    private static string Serializar(int status, string? contentType, byte[] cuerpo)
        => JsonSerializer.Serialize(new RespuestaGuardada(status, contentType, Convert.ToBase64String(cuerpo)), Json);

    private static async Task ReproducirAsync(HttpContext ctx, string? resultado)
    {
        if (string.IsNullOrEmpty(resultado))
        {
            ctx.Response.StatusCode = StatusCodes.Status204NoContent;
            return;
        }

        var guardada = JsonSerializer.Deserialize<RespuestaGuardada>(resultado, Json)!;
        ctx.Response.StatusCode = guardada.Status;
        if (!string.IsNullOrEmpty(guardada.ContentType))
        {
            ctx.Response.ContentType = guardada.ContentType;
        }

        ctx.Response.Headers["Idempotent-Replayed"] = "true";
        var cuerpo = Convert.FromBase64String(guardada.Cuerpo);
        if (cuerpo.Length > 0)
        {
            await ctx.Response.Body.WriteAsync(cuerpo);
        }
    }

    private static async Task DescartarAsync(GeoVialDbContext db, ClaveIdempotencia registro)
    {
        db.Set<ClaveIdempotencia>().Remove(registro);
        try
        {
            await db.SaveChangesAsync();
        }
        catch
        {
            // Mejor esfuerzo: si no se puede limpiar el registro en curso, la retención lo recicla.
        }
    }

    private static async Task EscribirProblemaAsync(HttpContext ctx, int status, string codigo, string detalle)
    {
        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = "application/problem+json";
        var problema = new { type = $"https://geovial/errores/{codigo}", title = "Conflicto", status, detail = detalle, codigo };
        await ctx.Response.WriteAsync(JsonSerializer.Serialize(problema, Json));
    }

    private sealed record RespuestaGuardada(int Status, string? ContentType, string Cuerpo);
}
