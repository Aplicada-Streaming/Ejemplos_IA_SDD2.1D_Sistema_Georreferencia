namespace GeoVial.Mobile.Servicios;

/// <summary>
/// Sesión del agente en la app de campo. Mantiene el token bearer y el usuario en memoria del
/// proceso (la app es de un único usuario por dispositivo). Notifica cambios para refrescar la UI.
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
