using System.Net.Http.Headers;
using System.Net.Http.Json;
using GeoVial.Web.Modelos;

namespace GeoVial.Web.Servicios;

/// <summary>Error de la API traducido para la UI, con el código estable del problem+json.</summary>
public sealed class ApiException(string codigo, string mensaje) : Exception(mensaje)
{
    public string Codigo { get; } = codigo;
}

/// <summary>
/// Cliente HTTP tipado del contrato REST de geovial-api. Adjunta el token bearer de la
/// sesión del circuito y traduce las respuestas problem+json a <see cref="ApiException"/>.
/// </summary>
public sealed class ClienteApi(HttpClient http, EstadoSesion sesion)
{
    public async Task<RespuestaLogin> LoginAsync(SolicitudLogin solicitud, CancellationToken ct = default)
    {
        using var resp = await http.PostAsJsonAsync("api/v1/sesion", solicitud, ct);
        await GarantizarExitoAsync(resp, ct);
        return (await resp.Content.ReadFromJsonAsync<RespuestaLogin>(ct))!;
    }

    public async Task<IReadOnlyList<UsuarioDto>> ListarUsuariosAsync(CancellationToken ct = default)
    {
        using var req = Autorizado(HttpMethod.Get, "api/v1/usuarios");
        using var resp = await http.SendAsync(req, ct);
        await GarantizarExitoAsync(resp, ct);
        return (await resp.Content.ReadFromJsonAsync<List<UsuarioDto>>(ct)) ?? [];
    }

    public async Task<UsuarioDto> CrearUsuarioAsync(SolicitudCrearUsuario solicitud, CancellationToken ct = default)
    {
        using var req = Autorizado(HttpMethod.Post, "api/v1/usuarios");
        req.Content = JsonContent.Create(solicitud);
        using var resp = await http.SendAsync(req, ct);
        await GarantizarExitoAsync(resp, ct);
        return (await resp.Content.ReadFromJsonAsync<UsuarioDto>(ct))!;
    }

    public async Task DarDeBajaAsync(Guid id, CancellationToken ct = default)
    {
        using var req = Autorizado(HttpMethod.Delete, $"api/v1/usuarios/{id}");
        using var resp = await http.SendAsync(req, ct);
        await GarantizarExitoAsync(resp, ct);
    }

    public async Task<IReadOnlyList<RelevamientoDto>> ListarRelevamientosAsync(CancellationToken ct = default)
    {
        using var req = Autorizado(HttpMethod.Get, "api/v1/relevamientos");
        using var resp = await http.SendAsync(req, ct);
        await GarantizarExitoAsync(resp, ct);
        return (await resp.Content.ReadFromJsonAsync<List<RelevamientoDto>>(ct)) ?? [];
    }

    public async Task<RelevamientoDto> CrearRelevamientoAsync(CrearRelevamientoRequest solicitud, CancellationToken ct = default)
    {
        using var req = Autorizado(HttpMethod.Post, "api/v1/relevamientos");
        req.Content = JsonContent.Create(solicitud);
        using var resp = await http.SendAsync(req, ct);
        await GarantizarExitoAsync(resp, ct);
        return (await resp.Content.ReadFromJsonAsync<RelevamientoDto>(ct))!;
    }

    public async Task<RelevamientoDto> CambiarEstadoRelevamientoAsync(Guid id, EstadoRelevamiento nuevo, CancellationToken ct = default)
    {
        using var req = Autorizado(HttpMethod.Post, $"api/v1/relevamientos/{id}/transicion");
        req.Content = JsonContent.Create(new CambiarEstadoRequest(nuevo));
        using var resp = await http.SendAsync(req, ct);
        await GarantizarExitoAsync(resp, ct);
        return (await resp.Content.ReadFromJsonAsync<RelevamientoDto>(ct))!;
    }

    public async Task<IReadOnlyList<MarcadorDto>> ListarMarcadoresAsync(Guid idRelevamiento, CancellationToken ct = default)
    {
        using var req = Autorizado(HttpMethod.Get, $"api/v1/relevamientos/{idRelevamiento}/marcadores");
        using var resp = await http.SendAsync(req, ct);
        await GarantizarExitoAsync(resp, ct);
        return (await resp.Content.ReadFromJsonAsync<List<MarcadorDto>>(ct)) ?? [];
    }

    public async Task<MarcadorDto> CrearMarcadorAsync(Guid idRelevamiento, CrearMarcadorRequest solicitud, CancellationToken ct = default)
    {
        using var req = Autorizado(HttpMethod.Post, $"api/v1/relevamientos/{idRelevamiento}/marcadores");
        req.Content = JsonContent.Create(solicitud);
        using var resp = await http.SendAsync(req, ct);
        await GarantizarExitoAsync(resp, ct);
        return (await resp.Content.ReadFromJsonAsync<MarcadorDto>(ct))!;
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
