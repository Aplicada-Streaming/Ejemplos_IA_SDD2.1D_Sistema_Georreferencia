using GeoVial.Web.Modelos;

namespace GeoVial.Web.Servicios;

/// <summary>
/// Sesión del usuario asociada al circuito interactivo del servidor. El token bearer se
/// mantiene del lado del servidor (no se expone al navegador), según el ADR de
/// autenticación de geovial-web. Es un servicio con alcance de circuito (scoped).
/// </summary>
public sealed class EstadoSesion
{
    public string? Token { get; private set; }
    public UsuarioDto? Usuario { get; private set; }

    public bool EstaAutenticado => Token is not null && Usuario is not null;

    public event Action? Cambio;

    public void Iniciar(RespuestaLogin login)
    {
        Token = login.Token;
        Usuario = login.Usuario;
        Cambio?.Invoke();
    }

    public void Cerrar()
    {
        Token = null;
        Usuario = null;
        Cambio?.Invoke();
    }
}
