using FluentAssertions;
using GeoVial.Web.Modelos;
using GeoVial.Web.Servicios;

namespace GeoVial.Web.Tests;

public sealed class EstadoSesionTests
{
    private static RespuestaLogin LoginDe(Rol rol) => new(
        Token: "token-de-prueba",
        TipoToken: "Bearer",
        ExpiraEnSegundos: 3600,
        Usuario: new UsuarioDto(Guid.NewGuid(), "raiz", rol, true, DateTimeOffset.UtcNow));

    [Fact]
    public void Sesion_nueva_no_esta_autenticada()
    {
        var sesion = new EstadoSesion();
        sesion.EstaAutenticado.Should().BeFalse();
        sesion.Token.Should().BeNull();
    }

    [Fact]
    public void Iniciar_autentica_y_notifica_cambio()
    {
        var sesion = new EstadoSesion();
        var notificado = 0;
        sesion.Cambio += () => notificado++;

        sesion.Iniciar(LoginDe(Rol.Raiz));

        sesion.EstaAutenticado.Should().BeTrue();
        sesion.Token.Should().Be("token-de-prueba");
        sesion.Usuario!.Rol.Should().Be(Rol.Raiz);
        notificado.Should().Be(1);
    }

    [Fact]
    public void Cerrar_limpia_y_notifica_cambio()
    {
        var sesion = new EstadoSesion();
        sesion.Iniciar(LoginDe(Rol.JefeDeArea));
        var notificado = 0;
        sesion.Cambio += () => notificado++;

        sesion.Cerrar();

        sesion.EstaAutenticado.Should().BeFalse();
        sesion.Token.Should().BeNull();
        sesion.Usuario.Should().BeNull();
        notificado.Should().Be(1);
    }
}
