namespace GeoVial.Mobile.Servicios;

/// <summary>
/// Modelos del contrato REST de geovial-api que la app de campo consume. Se replican del lado
/// cliente (la app no comparte el ensamblado del backend; consume el contrato por HTTP).
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

public static class Descripciones
{
    public static string Texto(this Rol rol) => rol switch
    {
        Rol.Raiz => "Usuario raíz",
        Rol.JefeGeneral => "Jefe general",
        Rol.JefeDeArea => "Jefe de área",
        Rol.AgenteDeCampo => "Agente de campo",
        _ => rol.ToString(),
    };

    public static string Texto(this EstadoRelevamiento estado) => estado switch
    {
        EstadoRelevamiento.Recoleccion => "Recolección",
        EstadoRelevamiento.Revision => "Revisión",
        EstadoRelevamiento.Cerrado => "Cerrado",
        _ => estado.ToString(),
    };
}
