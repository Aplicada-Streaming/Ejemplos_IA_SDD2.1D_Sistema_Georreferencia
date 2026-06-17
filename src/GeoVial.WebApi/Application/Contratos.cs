using GeoVial.WebApi.Domain;

namespace GeoVial.WebApi.Application;

// ---- DTOs (contrato REST, versionado por URI v1) ----

public sealed record SolicitudLogin(string NombreUsuario, string Contrasena);

public sealed record RespuestaLogin(string Token, string TipoToken, int ExpiraEnSegundos, UsuarioDto Usuario);

public sealed record SolicitudCrearUsuario(string NombreUsuario, string Contrasena, Rol Rol);

public sealed record UsuarioDto(Guid Id, string NombreUsuario, Rol Rol, bool Activo, DateTimeOffset FechaAlta);

// ---- Puertos de aplicación (los consumen los controllers) ----

public interface IServicioAutenticacion
{
    /// <summary>Valida credenciales y emite un token bearer. (CU-01)</summary>
    Task<RespuestaLogin> LoginAsync(SolicitudLogin solicitud, CancellationToken ct = default);
}

public interface IServicioUsuarios
{
    /// <summary>Alta de un usuario respetando la jerarquía del administrador (RN-01). (CU-02)</summary>
    Task<UsuarioDto> CrearAsync(Guid idAdministrador, SolicitudCrearUsuario solicitud, CancellationToken ct = default);

    /// <summary>Lista los usuarios que administra el solicitante. (CU-03)</summary>
    Task<IReadOnlyList<UsuarioDto>> ListarAdministradosAsync(Guid idAdministrador, CancellationToken ct = default);

    /// <summary>Baja lógica de un usuario que administra el solicitante. (CU-02)</summary>
    Task DarDeBajaAsync(Guid idAdministrador, Guid idUsuario, CancellationToken ct = default);
}

// ---- Puertos de infraestructura (los implementa la capa Infrastructure) ----

public interface IHasheadorContrasena
{
    string Hashear(string contrasena);
    bool Verificar(string contrasena, string hash);
}

public interface IEmisorTokens
{
    /// <summary>Emite un token bearer firmado para el usuario. Devuelve (token, segundos de validez).</summary>
    (string Token, int ExpiraEnSegundos) Emitir(Usuario usuario);
}

// ---- Errores de dominio/aplicación con código estable (problem+json RFC 7807) ----

public abstract class ErrorAplicacion(string codigo, string mensaje) : Exception(mensaje)
{
    public string Codigo { get; } = codigo;
}

public sealed class CredencialesInvalidasException()
    : ErrorAplicacion("CREDENCIALES_INVALIDAS", "El nombre de usuario o la contraseña son incorrectos.");

public sealed class OperacionNoAutorizadaException(string mensaje)
    : ErrorAplicacion("OPERACION_NO_AUTORIZADA", mensaje);

public sealed class UsuarioYaExisteException(string nombreUsuario)
    : ErrorAplicacion("USUARIO_YA_EXISTE", $"Ya existe un usuario con el nombre '{nombreUsuario}'.");

public sealed class UsuarioNoEncontradoException(Guid id)
    : ErrorAplicacion("USUARIO_NO_ENCONTRADO", $"No se encontró el usuario '{id}'.");
