namespace GeoVial.Web.Modelos;

/// <summary>
/// Modelos del contrato REST de geovial-api que el front consume. Se replican del lado
/// cliente (el front no comparte el ensamblado del backend; consume el contrato por HTTP).
/// </summary>
public enum Rol
{
    Raiz = 0,
    JefeGeneral = 1,
    JefeDeArea = 2,
    AgenteDeCampo = 3,
}

public sealed record SolicitudLogin(string NombreUsuario, string Contrasena);

public sealed record UsuarioDto(Guid Id, string NombreUsuario, Rol Rol, bool Activo, DateTimeOffset FechaAlta);

public sealed record RespuestaLogin(string Token, string TipoToken, int ExpiraEnSegundos, UsuarioDto Usuario);

public sealed record SolicitudCrearUsuario(string NombreUsuario, string Contrasena, Rol Rol);

public static class RolDescripcion
{
    public static string Texto(this Rol rol) => rol switch
    {
        Rol.Raiz => "Usuario raíz",
        Rol.JefeGeneral => "Jefe general",
        Rol.JefeDeArea => "Jefe de área",
        Rol.AgenteDeCampo => "Agente de campo",
        _ => rol.ToString(),
    };
}

public enum EstadoRelevamiento
{
    Recoleccion = 0,
    Revision = 1,
    Cerrado = 2,
}

public sealed record RelevamientoDto(
    Guid Id,
    string Nombre,
    string TramoVial,
    EstadoRelevamiento Estado,
    DateTimeOffset FechaCreacion,
    int CantidadMarcadores,
    int CantidadAgentes);

public sealed record CrearRelevamientoRequest(string Nombre, string TramoVial);

public sealed record CambiarEstadoRequest(EstadoRelevamiento NuevoEstado);

public static class EstadoRelevamientoDescripcion
{
    public static string Texto(this EstadoRelevamiento estado) => estado switch
    {
        EstadoRelevamiento.Recoleccion => "Recolección",
        EstadoRelevamiento.Revision => "Revisión",
        EstadoRelevamiento.Cerrado => "Cerrado",
        _ => estado.ToString(),
    };

    /// <summary>Estado al que puede avanzar (o null si ya está cerrado).</summary>
    public static EstadoRelevamiento? Siguiente(this EstadoRelevamiento estado) => estado switch
    {
        EstadoRelevamiento.Recoleccion => EstadoRelevamiento.Revision,
        EstadoRelevamiento.Revision => EstadoRelevamiento.Cerrado,
        _ => null,
    };
}
