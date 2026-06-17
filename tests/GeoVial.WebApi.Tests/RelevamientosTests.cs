using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using GeoVial.WebApi.Application;
using GeoVial.WebApi.Domain;

namespace GeoVial.WebApi.Tests;

public sealed class RelevamientosTests(FabricaWebApi fabrica) : IClassFixture<FabricaWebApi>
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

    private async Task<Guid> CrearUsuarioAsync(HttpClient c, string token, string nombre, Rol rol)
    {
        Con(c, token);
        var resp = await c.PostAsJsonAsync("/api/v1/usuarios", new SolicitudCrearUsuario(nombre, Clave, rol));
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await resp.Content.ReadFromJsonAsync<UsuarioDto>())!.Id;
    }

    /// <summary>Crea raíz→jefe general→jefe de área→agente con nombres únicos y devuelve sus tokens.</summary>
    private async Task<(string TokenJefeArea, string TokenAgente, Guid IdAgente)> CrearJerarquiaAsync(HttpClient c)
    {
        var sufijo = Guid.NewGuid().ToString("N")[..8];
        var tokenRaiz = await LoginAsync(c, FabricaWebApi.UsuarioRaiz, FabricaWebApi.ContrasenaRaiz);
        await CrearUsuarioAsync(c, tokenRaiz, $"jg-{sufijo}", Rol.JefeGeneral);

        var tokenJg = await LoginAsync(c, $"jg-{sufijo}", Clave);
        await CrearUsuarioAsync(c, tokenJg, $"ja-{sufijo}", Rol.JefeDeArea);

        var tokenJa = await LoginAsync(c, $"ja-{sufijo}", Clave);
        var idAgente = await CrearUsuarioAsync(c, tokenJa, $"ag-{sufijo}", Rol.AgenteDeCampo);

        var tokenAg = await LoginAsync(c, $"ag-{sufijo}", Clave);
        return (tokenJa, tokenAg, idAgente);
    }

    [Fact]
    public async Task Flujo_completo_de_relevamiento_F1()
    {
        var c = _fabrica.CreateClient();
        var (tokenJa, tokenAg, idAgente) = await CrearJerarquiaAsync(c);

        // Crear relevamiento (jefe de área)
        Con(c, tokenJa);
        var crear = await c.PostAsJsonAsync("/api/v1/relevamientos", new CrearRelevamientoRequest("Tramo Ruta 9 km 100-120", "Ruta 9, puentes 3 y 4"));
        crear.StatusCode.Should().Be(HttpStatusCode.Created);
        var rel = (await crear.Content.ReadFromJsonAsync<RelevamientoDto>())!;
        rel.Estado.Should().Be(EstadoRelevamiento.Recoleccion);

        // Asignar agente
        var asignar = await c.PostAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/agentes", new AsignarAgenteRequest(idAgente));
        asignar.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Crear marcador
        var marcar = await c.PostAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/marcadores", new CrearMarcadorRequest(-34.6037, -58.3816, "Pila norte del puente 3"));
        var cuerpoMarcar = await marcar.Content.ReadAsStringAsync();
        marcar.StatusCode.Should().Be(HttpStatusCode.Created, cuerpoMarcar);

        // Listar (jefe) -> con conteos
        var lista = await c.GetFromJsonAsync<List<RelevamientoDto>>("/api/v1/relevamientos");
        var enLista = lista!.Single(r => r.Id == rel.Id);
        enLista.CantidadMarcadores.Should().Be(1);
        enLista.CantidadAgentes.Should().Be(1);

        // Transición recolección -> revisión
        var transicion = await c.PostAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/transicion", new CambiarEstadoRequest(EstadoRelevamiento.Revision));
        transicion.StatusCode.Should().Be(HttpStatusCode.OK);
        (await transicion.Content.ReadFromJsonAsync<RelevamientoDto>())!.Estado.Should().Be(EstadoRelevamiento.Revision);

        // El agente asignado ve el relevamiento
        Con(c, tokenAg);
        var listaAgente = await c.GetFromJsonAsync<List<RelevamientoDto>>("/api/v1/relevamientos");
        listaAgente!.Should().Contain(r => r.Id == rel.Id);
    }

    [Fact]
    public async Task Agente_no_puede_crear_relevamiento_403()
    {
        var c = _fabrica.CreateClient();
        var (_, tokenAg, _) = await CrearJerarquiaAsync(c);

        Con(c, tokenAg);
        var crear = await c.PostAsJsonAsync("/api/v1/relevamientos", new CrearRelevamientoRequest("X", "Y"));
        crear.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Transicion_salteada_recoleccion_a_cerrado_409()
    {
        var c = _fabrica.CreateClient();
        var (tokenJa, _, _) = await CrearJerarquiaAsync(c);

        Con(c, tokenJa);
        var rel = (await (await c.PostAsJsonAsync("/api/v1/relevamientos", new CrearRelevamientoRequest("Tramo X", "Camino Y"))).Content.ReadFromJsonAsync<RelevamientoDto>())!;

        var transicion = await c.PostAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/transicion", new CambiarEstadoRequest(EstadoRelevamiento.Cerrado));
        transicion.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
