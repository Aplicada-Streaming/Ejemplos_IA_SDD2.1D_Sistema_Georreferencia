using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using GeoVial.WebApi.Application;
using GeoVial.WebApi.Domain;

namespace GeoVial.WebApi.Tests;

/// <summary>CU-21 (RN-07): idempotencia de operaciones no seguras vía encabezado Idempotency-Key.</summary>
public sealed class IdempotenciaTests(FabricaWebApi fabrica) : IClassFixture<FabricaWebApi>
{
    private readonly FabricaWebApi _fabrica = fabrica;
    private const string Clave = "Clave.Prueba.2026";

    private static HttpClient Con(HttpClient c, string token)
    {
        c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return c;
    }

    private async Task<string> LoginRaizAsync(HttpClient c)
    {
        var resp = await c.PostAsJsonAsync("/api/v1/sesion", new SolicitudLogin(FabricaWebApi.UsuarioRaiz, FabricaWebApi.ContrasenaRaiz));
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await resp.Content.ReadFromJsonAsync<RespuestaLogin>())!.Token;
    }

    private static Task<HttpResponseMessage> PostConClaveAsync(HttpClient c, string ruta, object cuerpo, string clave)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, ruta) { Content = JsonContent.Create(cuerpo) };
        req.Headers.Add("Idempotency-Key", clave);
        return c.SendAsync(req);
    }

    [Fact]
    public async Task CA01_alta_con_misma_clave_no_duplica_y_reproduce_el_resultado()
    {
        var c = _fabrica.CreateClient();
        Con(c, await LoginRaizAsync(c));

        var sufijo = Guid.NewGuid().ToString("N")[..8];
        var cuerpo = new SolicitudCrearUsuario($"jg-{sufijo}", Clave, Rol.JefeGeneral);
        var clave = $"alta-{sufijo}";

        var primera = await PostConClaveAsync(c, "/api/v1/usuarios", cuerpo, clave);
        primera.StatusCode.Should().Be(HttpStatusCode.Created);
        var creado = (await primera.Content.ReadFromJsonAsync<UsuarioDto>())!;

        // Reintento con la misma clave y el mismo contenido: reproduce el resultado sin duplicar.
        var reintento = await PostConClaveAsync(c, "/api/v1/usuarios", cuerpo, clave);
        reintento.StatusCode.Should().Be(HttpStatusCode.Created);
        reintento.Headers.Contains("Idempotent-Replayed").Should().BeTrue();
        (await reintento.Content.ReadFromJsonAsync<UsuarioDto>())!.Id.Should().Be(creado.Id);

        var lista = (await c.GetFromJsonAsync<List<UsuarioDto>>("/api/v1/usuarios"))!;
        lista.Count(u => u.NombreUsuario == $"jg-{sufijo}").Should().Be(1);
    }

    [Fact]
    public async Task CA03_clave_reutilizada_con_contenido_distinto_409()
    {
        var c = _fabrica.CreateClient();
        Con(c, await LoginRaizAsync(c));

        var sufijo = Guid.NewGuid().ToString("N")[..8];
        var clave = $"reuso-{sufijo}";

        var primera = await PostConClaveAsync(c, "/api/v1/usuarios", new SolicitudCrearUsuario($"a-{sufijo}", Clave, Rol.JefeGeneral), clave);
        primera.StatusCode.Should().Be(HttpStatusCode.Created);

        // Misma clave, contenido distinto → rechazo.
        var inconsistente = await PostConClaveAsync(c, "/api/v1/usuarios", new SolicitudCrearUsuario($"b-{sufijo}", Clave, Rol.JefeGeneral), clave);
        inconsistente.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Sin_clave_el_alta_repetida_falla_por_duplicado()
    {
        // Sin Idempotency-Key, repetir el alta exacta no se reproduce: el backend la trata como nueva
        // y la rechaza por usuario ya existente (control: el reintento idempotente de CA-01 sí difiere).
        var c = _fabrica.CreateClient();
        Con(c, await LoginRaizAsync(c));
        var sufijo = Guid.NewGuid().ToString("N")[..8];
        var cuerpo = new SolicitudCrearUsuario($"sc-{sufijo}", Clave, Rol.JefeGeneral);

        (await c.PostAsJsonAsync("/api/v1/usuarios", cuerpo)).StatusCode.Should().Be(HttpStatusCode.Created);
        (await c.PostAsJsonAsync("/api/v1/usuarios", cuerpo)).StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
