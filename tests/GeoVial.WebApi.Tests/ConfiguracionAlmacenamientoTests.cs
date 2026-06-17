using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using GeoVial.WebApi.Application;
using GeoVial.WebApi.Domain;

namespace GeoVial.WebApi.Tests;

/// <summary>
/// CU-17 (NB-07): configuración del destino de almacenamiento por el usuario raíz. Usa su propia
/// fábrica (fixture) para aislar el singleton del router de almacenamiento de las demás pruebas.
/// </summary>
public sealed class ConfiguracionAlmacenamientoTests(FabricaWebApi fabrica) : IClassFixture<FabricaWebApi>
{
    private readonly FabricaWebApi _fabrica = fabrica;
    private const string Clave = "Clave.Prueba.2026";

    private static HttpClient Con(HttpClient c, string token)
    {
        c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return c;
    }

    private async Task<string> LoginAsync(HttpClient c, string usuario, string clave)
    {
        var resp = await c.PostAsJsonAsync("/api/v1/sesion", new SolicitudLogin(usuario, clave));
        resp.StatusCode.Should().Be(HttpStatusCode.OK, $"login de {usuario} debería funcionar");
        return (await resp.Content.ReadFromJsonAsync<RespuestaLogin>())!.Token;
    }

    [Fact]
    public async Task Raiz_consulta_valida_sin_activar_y_cambia_el_destino()
    {
        var c = _fabrica.CreateClient();
        var tokenRaiz = await LoginAsync(c, FabricaWebApi.UsuarioRaiz, FabricaWebApi.ContrasenaRaiz);
        Con(c, tokenRaiz);

        // Línea base determinista: activar "local".
        var baseLocal = await c.PutAsJsonAsync("/api/v1/almacenamiento/activo", new ActivarAlmacenamientoRequest("local"));
        baseLocal.StatusCode.Should().Be(HttpStatusCode.OK);

        var config = await c.GetFromJsonAsync<ConfiguracionAlmacenamientoDto>("/api/v1/almacenamiento");
        config!.Activo.Should().Be("local");
        config.Disponibles.Should().Contain(new[] { "local", "memoria" });

        // CU-17 5.B: validar "memoria" no cambia el destino activo.
        var validacion = (await (await c.PostAsJsonAsync("/api/v1/almacenamiento/validacion", new ValidarAlmacenamientoRequest("memoria"))).Content.ReadFromJsonAsync<ResultadoValidacionDto>())!;
        validacion.Valido.Should().BeTrue();
        (await c.GetFromJsonAsync<ConfiguracionAlmacenamientoDto>("/api/v1/almacenamiento"))!.Activo.Should().Be("local");

        // CU-17 CA-01: cambiar el destino activo a "memoria".
        var activar = await c.PutAsJsonAsync("/api/v1/almacenamiento/activo", new ActivarAlmacenamientoRequest("memoria"));
        activar.StatusCode.Should().Be(HttpStatusCode.OK);
        (await activar.Content.ReadFromJsonAsync<ConfiguracionAlmacenamientoDto>())!.Activo.Should().Be("memoria");
    }

    [Fact]
    public async Task Proveedor_inexistente_400()
    {
        var c = _fabrica.CreateClient();
        Con(c, await LoginAsync(c, FabricaWebApi.UsuarioRaiz, FabricaWebApi.ContrasenaRaiz));

        var resp = await c.PutAsJsonAsync("/api/v1/almacenamiento/activo", new ActivarAlmacenamientoRequest("nube-inexistente"));
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task No_raiz_no_puede_configurar_403()
    {
        var c = _fabrica.CreateClient();
        var tokenRaiz = await LoginAsync(c, FabricaWebApi.UsuarioRaiz, FabricaWebApi.ContrasenaRaiz);

        // El raíz crea un jefe general; ese rol no puede configurar el almacenamiento (CU-17 CA-02).
        var sufijo = Guid.NewGuid().ToString("N")[..8];
        Con(c, tokenRaiz);
        (await c.PostAsJsonAsync("/api/v1/usuarios", new SolicitudCrearUsuario($"jg-{sufijo}", Clave, Rol.JefeGeneral))).StatusCode.Should().Be(HttpStatusCode.Created);

        var tokenJg = await LoginAsync(c, $"jg-{sufijo}", Clave);
        Con(c, tokenJg);
        (await c.GetAsync("/api/v1/almacenamiento")).StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
