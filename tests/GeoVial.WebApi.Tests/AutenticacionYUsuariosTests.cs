using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using GeoVial.WebApi.Application;
using GeoVial.WebApi.Domain;

namespace GeoVial.WebApi.Tests;

public sealed class AutenticacionYUsuariosTests(FabricaWebApi fabrica) : IClassFixture<FabricaWebApi>
{
    private readonly FabricaWebApi _fabrica = fabrica;

    private async Task<string> LoginRaizAsync(HttpClient cliente)
    {
        var resp = await cliente.PostAsJsonAsync("/api/v1/sesion",
            new SolicitudLogin(FabricaWebApi.UsuarioRaiz, FabricaWebApi.ContrasenaRaiz));
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var login = await resp.Content.ReadFromJsonAsync<RespuestaLogin>();
        login!.Token.Should().NotBeNullOrWhiteSpace();
        login.TipoToken.Should().Be("Bearer");
        return login.Token;
    }

    [Fact]
    public async Task Login_con_raiz_devuelve_token_y_rol_raiz()
    {
        var cliente = _fabrica.CreateClient();
        var resp = await cliente.PostAsJsonAsync("/api/v1/sesion",
            new SolicitudLogin(FabricaWebApi.UsuarioRaiz, FabricaWebApi.ContrasenaRaiz));

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var login = await resp.Content.ReadFromJsonAsync<RespuestaLogin>();
        login!.Usuario.Rol.Should().Be(Rol.Raiz);
    }

    [Fact]
    public async Task Login_con_credenciales_invalidas_devuelve_401()
    {
        var cliente = _fabrica.CreateClient();
        var resp = await cliente.PostAsJsonAsync("/api/v1/sesion",
            new SolicitudLogin(FabricaWebApi.UsuarioRaiz, "incorrecta"));

        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Usuarios_sin_token_devuelve_401()
    {
        var cliente = _fabrica.CreateClient();
        var resp = await cliente.GetAsync("/api/v1/usuarios");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Raiz_crea_jefe_general_y_lo_lista()
    {
        var cliente = _fabrica.CreateClient();
        var token = await LoginRaizAsync(cliente);
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var crear = await cliente.PostAsJsonAsync("/api/v1/usuarios",
            new SolicitudCrearUsuario("jefe.general.1", "Clave.Jg.2026", Rol.JefeGeneral));
        crear.StatusCode.Should().Be(HttpStatusCode.Created);
        var creado = await crear.Content.ReadFromJsonAsync<UsuarioDto>();
        creado!.Rol.Should().Be(Rol.JefeGeneral);
        creado.NombreUsuario.Should().Be("jefe.general.1");

        var lista = await cliente.GetFromJsonAsync<List<UsuarioDto>>("/api/v1/usuarios");
        lista.Should().ContainSingle(u => u.NombreUsuario == "jefe.general.1" && u.Rol == Rol.JefeGeneral);
    }

    [Fact]
    public async Task Raiz_no_puede_crear_agente_de_campo_devuelve_403()
    {
        var cliente = _fabrica.CreateClient();
        var token = await LoginRaizAsync(cliente);
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var crear = await cliente.PostAsJsonAsync("/api/v1/usuarios",
            new SolicitudCrearUsuario("agente.1", "Clave.Ag.2026", Rol.AgenteDeCampo));

        crear.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
