using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GeoVial.Mobile.Servicios;

/// <summary>Error de la API traducido para la UI, con el código estable del problem+json.</summary>
public sealed class ApiException(string codigo, string mensaje) : Exception(mensaje)
{
    public string Codigo { get; } = codigo;
}

/// <summary>
/// Cliente HTTP tipado del contrato REST de geovial-api para la app de campo. Adjunta el token
/// bearer de la sesión y traduce las respuestas problem+json a <see cref="ApiException"/>.
/// </summary>
public sealed class ClienteApi(HttpClient http, EstadoSesion sesion)
{
    public async Task<RespuestaLogin> LoginAsync(SolicitudLogin solicitud, CancellationToken ct = default)
    {
        using var resp = await http.PostAsJsonAsync("api/v1/sesion", solicitud, ct);
        await GarantizarExitoAsync(resp, ct);
        return (await resp.Content.ReadFromJsonAsync<RespuestaLogin>(ct))!;
    }

    public async Task<IReadOnlyList<RelevamientoDto>> ListarRelevamientosAsync(CancellationToken ct = default)
    {
        using var req = Autorizado(HttpMethod.Get, "api/v1/relevamientos");
        using var resp = await http.SendAsync(req, ct);
        await GarantizarExitoAsync(resp, ct);
        return (await resp.Content.ReadFromJsonAsync<List<RelevamientoDto>>(ct)) ?? [];
    }

    private HttpRequestMessage Autorizado(HttpMethod metodo, string ruta)
    {
        var req = new HttpRequestMessage(metodo, ruta);
        if (sesion.Token is { } token)
        {
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return req;
    }

    private static async Task GarantizarExitoAsync(HttpResponseMessage resp, CancellationToken ct)
    {
        if (resp.IsSuccessStatusCode)
        {
            return;
        }

        string codigo = "ERROR";
        string mensaje = $"La solicitud falló con código {(int)resp.StatusCode}.";
        try
        {
            var problema = await resp.Content.ReadFromJsonAsync<ProblemaApi>(ct);
            if (problema is not null)
            {
                codigo = problema.Codigo ?? codigo;
                mensaje = problema.Detail ?? problema.Title ?? mensaje;
            }
        }
        catch
        {
            // Si el cuerpo no es problem+json, se conserva el mensaje por defecto.
        }

        throw new ApiException(codigo, mensaje);
    }

    private sealed record ProblemaApi(string? Title, string? Detail, string? Codigo);
}
