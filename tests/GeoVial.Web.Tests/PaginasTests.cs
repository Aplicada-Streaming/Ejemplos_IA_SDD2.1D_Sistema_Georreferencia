using System.Net;
using Bunit;
using FluentAssertions;
using GeoVial.Web.Servicios;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace GeoVial.Web.Tests;

public sealed class PaginasTests : TestContext
{
    public PaginasTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMudServices();
        Services.AddScoped<EstadoSesion>();
        Services.AddScoped(_ => new HttpClient(new HandlerVacio()) { BaseAddress = new Uri("http://localhost/") });
        Services.AddScoped<ClienteApi>();
    }

    [Fact]
    public void Login_renderiza_el_formulario_de_ingreso()
    {
        var cut = RenderComponent<GeoVial.Web.Components.Pages.Login>();

        cut.Markup.Should().Contain("Iniciar sesión");
        cut.FindAll("input").Should().HaveCountGreaterThanOrEqualTo(2);
        cut.FindAll("button").Should().NotBeEmpty();
    }

    [Fact]
    public void Usuarios_sin_sesion_redirige_a_login()
    {
        RenderComponent<GeoVial.Web.Components.Pages.Usuarios>();

        var nav = Services.GetRequiredService<NavigationManager>();
        nav.Uri.Should().EndWith("/login");
    }

    [Fact]
    public void Relevamientos_sin_sesion_redirige_a_login()
    {
        RenderComponent<GeoVial.Web.Components.Pages.Relevamientos>();

        var nav = Services.GetRequiredService<NavigationManager>();
        nav.Uri.Should().EndWith("/login");
    }

    [Fact]
    public void DetalleRelevamiento_sin_sesion_redirige_a_login()
    {
        RenderComponent<GeoVial.Web.Components.Pages.RelevamientoDetalle>(
            p => p.Add(c => c.Id, Guid.NewGuid()));

        var nav = Services.GetRequiredService<NavigationManager>();
        nav.Uri.Should().EndWith("/login");
    }

    private sealed class HandlerVacio : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]"),
            });
    }
}
