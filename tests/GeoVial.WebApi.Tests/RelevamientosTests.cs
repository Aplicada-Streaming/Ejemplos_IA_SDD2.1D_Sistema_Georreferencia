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

    [Fact]
    public async Task Marcador_con_observaciones_y_etiquetas_y_movimiento_conserva_identidad()
    {
        var c = _fabrica.CreateClient();
        var (tokenJa, tokenAg, idAgente) = await CrearJerarquiaAsync(c);

        // Jefe crea relevamiento, asigna al agente y crea un marcador.
        Con(c, tokenJa);
        var rel = (await (await c.PostAsJsonAsync("/api/v1/relevamientos", new CrearRelevamientoRequest("Tramo Ruta 11", "Ruta 11, puente sur"))).Content.ReadFromJsonAsync<RelevamientoDto>())!;
        (await c.PostAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/agentes", new AsignarAgenteRequest(idAgente))).StatusCode.Should().Be(HttpStatusCode.NoContent);
        var marcador = (await (await c.PostAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/marcadores", new CrearMarcadorRequest(-34.6, -58.4, "Junta de dilatación"))).Content.ReadFromJsonAsync<MarcadorDto>())!;
        marcador.CantidadObservaciones.Should().Be(0);
        marcador.Etiquetas.Should().BeEmpty();

        // Etiqueta del relevamiento y aplicación al marcador.
        var etiqueta = (await (await c.PostAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/etiquetas", new CrearEtiquetaRequest("fisura"))).Content.ReadFromJsonAsync<EtiquetaDto>())!;
        (await c.PostAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/marcadores/{marcador.Id}/etiquetas", new EtiquetarMarcadorRequest(etiqueta.Id)))
            .StatusCode.Should().Be(HttpStatusCode.NoContent);

        // El agente asignado registra una observación anclada al marcador (RC-02).
        Con(c, tokenAg);
        var obs = await c.PostAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/marcadores/{marcador.Id}/observaciones", new CrearObservacionRequest("Fisura longitudinal de 30 cm"));
        obs.StatusCode.Should().Be(HttpStatusCode.Created);

        // El marcador listado refleja el conteo de observaciones y la etiqueta.
        var lista = await c.GetFromJsonAsync<List<MarcadorDto>>($"/api/v1/relevamientos/{rel.Id}/marcadores");
        var enLista = lista!.Single(m => m.Id == marcador.Id);
        enLista.CantidadObservaciones.Should().Be(1);
        enLista.Etiquetas.Should().Contain("fisura");

        // RC-01: mover el marcador conserva su identidad (mismo Id, nuevas coordenadas).
        Con(c, tokenJa);
        var movido = (await (await c.PutAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/marcadores/{marcador.Id}", new MoverMarcadorRequest(-34.61, -58.41))).Content.ReadFromJsonAsync<MarcadorDto>())!;
        movido.Id.Should().Be(marcador.Id);
        movido.Latitud.Should().Be(-34.61);
        movido.CantidadObservaciones.Should().Be(1);
        movido.Etiquetas.Should().Contain("fisura");

        // US-15 / RC-02: no se da de baja un marcador con observaciones.
        var bajaConObs = await c.DeleteAsync($"/api/v1/relevamientos/{rel.Id}/marcadores/{marcador.Id}");
        bajaConObs.StatusCode.Should().Be(HttpStatusCode.Conflict);

        // Un marcador sin observaciones sí se puede dar de baja.
        var vacio = (await (await c.PostAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/marcadores", new CrearMarcadorRequest(-34.62, -58.42, null))).Content.ReadFromJsonAsync<MarcadorDto>())!;
        (await c.DeleteAsync($"/api/v1/relevamientos/{rel.Id}/marcadores/{vacio.Id}")).StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Etiqueta_duplicada_en_relevamiento_409()
    {
        var c = _fabrica.CreateClient();
        var (tokenJa, _, _) = await CrearJerarquiaAsync(c);

        Con(c, tokenJa);
        var rel = (await (await c.PostAsJsonAsync("/api/v1/relevamientos", new CrearRelevamientoRequest("Tramo Z", "Camino W"))).Content.ReadFromJsonAsync<RelevamientoDto>())!;

        (await c.PostAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/etiquetas", new CrearEtiquetaRequest("bache"))).StatusCode.Should().Be(HttpStatusCode.Created);
        (await c.PostAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/etiquetas", new CrearEtiquetaRequest("bache"))).StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private static MultipartFormDataContent FormularioFoto(byte[] bytes, string contentType, IDictionary<string, string> campos)
    {
        var form = new MultipartFormDataContent();
        var archivo = new ByteArrayContent(bytes);
        archivo.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        form.Add(archivo, "Archivo", "foto.bin");
        foreach (var (clave, valor) in campos)
        {
            form.Add(new StringContent(valor), clave);
        }

        return form;
    }

    [Fact]
    public async Task Captura_de_foto_prioriza_ubicacion_incrustada_y_se_descarga()
    {
        var c = _fabrica.CreateClient();
        var (tokenJa, tokenAg, idAgente) = await CrearJerarquiaAsync(c);

        // Jefe crea relevamiento, marcador y asigna al agente.
        Con(c, tokenJa);
        var rel = (await (await c.PostAsJsonAsync("/api/v1/relevamientos", new CrearRelevamientoRequest("Tramo Ruta 8", "Ruta 8, alcantarilla"))).Content.ReadFromJsonAsync<RelevamientoDto>())!;
        (await c.PostAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/agentes", new AsignarAgenteRequest(idAgente))).StatusCode.Should().Be(HttpStatusCode.NoContent);
        var marcador = (await (await c.PostAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/marcadores", new CrearMarcadorRequest(-34.5, -58.5, "Alcantarilla"))).Content.ReadFromJsonAsync<MarcadorDto>())!;

        // El agente registra una observación y le sube una foto con coordenada incrustada Y manual.
        Con(c, tokenAg);
        var obs = (await (await c.PostAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/marcadores/{marcador.Id}/observaciones", new CrearObservacionRequest("Erosión"))).Content.ReadFromJsonAsync<ObservacionDto>())!;

        var bytes = new byte[] { 10, 20, 30, 40, 50, 60 };
        using var form = FormularioFoto(bytes, "image/jpeg", new Dictionary<string, string>
        {
            ["LatitudIncrustada"] = "-34.61",
            ["LongitudIncrustada"] = "-58.61",
            ["LatitudManual"] = "-10.0",
            ["LongitudManual"] = "-10.0",
            ["Comentario"] = "Erosión en la base",
        });
        var subida = await c.PostAsync($"/api/v1/relevamientos/{rel.Id}/observaciones/{obs.Id}/fotos", form);
        var cuerpo = await subida.Content.ReadAsStringAsync();
        subida.StatusCode.Should().Be(HttpStatusCode.Created, cuerpo);
        var foto = (await subida.Content.ReadFromJsonAsync<FotoDto>())!;

        // RN-04: la coordenada incrustada gana a la manual.
        foto.PendienteUbicacion.Should().BeFalse();
        foto.Latitud.Should().Be(-34.61);
        foto.Longitud.Should().Be(-58.61);
        foto.Comentario.Should().Be("Erosión en la base");

        // Listar por observación y por marcador (US-26).
        var porObs = await c.GetFromJsonAsync<List<FotoDto>>($"/api/v1/relevamientos/{rel.Id}/observaciones/{obs.Id}/fotos");
        porObs!.Should().ContainSingle(f => f.Id == foto.Id);

        var porMarcador = await c.GetFromJsonAsync<List<FotoDto>>($"/api/v1/relevamientos/{rel.Id}/marcadores/{marcador.Id}/fotos");
        porMarcador!.Should().ContainSingle(f => f.Id == foto.Id);

        // Descargar el binario: coincide con lo subido.
        var contenido = await c.GetAsync($"/api/v1/relevamientos/{rel.Id}/fotos/{foto.Id}/contenido");
        contenido.StatusCode.Should().Be(HttpStatusCode.OK);
        (await contenido.Content.ReadAsByteArrayAsync()).Should().Equal(bytes);
    }

    [Fact]
    public async Task Foto_sin_coordenadas_queda_pendiente_de_ubicacion()
    {
        var c = _fabrica.CreateClient();
        var (tokenJa, _, _) = await CrearJerarquiaAsync(c);

        Con(c, tokenJa);
        var rel = (await (await c.PostAsJsonAsync("/api/v1/relevamientos", new CrearRelevamientoRequest("Tramo Ruta 7", "Ruta 7"))).Content.ReadFromJsonAsync<RelevamientoDto>())!;
        var marcador = (await (await c.PostAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/marcadores", new CrearMarcadorRequest(-34.5, -58.5, null))).Content.ReadFromJsonAsync<MarcadorDto>())!;
        var obs = (await (await c.PostAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/marcadores/{marcador.Id}/observaciones", new CrearObservacionRequest(null))).Content.ReadFromJsonAsync<ObservacionDto>())!;

        using var form = FormularioFoto(new byte[] { 1, 2, 3 }, "image/png", new Dictionary<string, string>());
        var subida = await c.PostAsync($"/api/v1/relevamientos/{rel.Id}/observaciones/{obs.Id}/fotos", form);
        subida.StatusCode.Should().Be(HttpStatusCode.Created);
        var foto = (await subida.Content.ReadFromJsonAsync<FotoDto>())!;
        foto.PendienteUbicacion.Should().BeTrue();
        foto.Latitud.Should().BeNull();
    }

    [Fact]
    public async Task Archivo_no_imagen_415()
    {
        var c = _fabrica.CreateClient();
        var (tokenJa, _, _) = await CrearJerarquiaAsync(c);

        Con(c, tokenJa);
        var rel = (await (await c.PostAsJsonAsync("/api/v1/relevamientos", new CrearRelevamientoRequest("Tramo Ruta 6", "Ruta 6"))).Content.ReadFromJsonAsync<RelevamientoDto>())!;
        var marcador = (await (await c.PostAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/marcadores", new CrearMarcadorRequest(-34.5, -58.5, null))).Content.ReadFromJsonAsync<MarcadorDto>())!;
        var obs = (await (await c.PostAsJsonAsync($"/api/v1/relevamientos/{rel.Id}/marcadores/{marcador.Id}/observaciones", new CrearObservacionRequest(null))).Content.ReadFromJsonAsync<ObservacionDto>())!;

        using var form = FormularioFoto(new byte[] { 1, 2, 3 }, "application/pdf", new Dictionary<string, string>());
        var subida = await c.PostAsync($"/api/v1/relevamientos/{rel.Id}/observaciones/{obs.Id}/fotos", form);
        subida.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }
}
