namespace GeoVial.WebApi.Domain;

/// <summary>
/// Usuario del sistema. Cada usuario tiene un rol de la jerarquía y, salvo el raíz,
/// un administrador que lo dio de alta (RN-01). La baja es lógica (campo Activo) para
/// conservar la trazabilidad de autoría de las observaciones.
/// </summary>
public sealed class Usuario
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string NombreUsuario { get; private set; } = string.Empty;
    public string HashContrasena { get; private set; } = string.Empty;
    public Rol Rol { get; private set; }
    public Guid? IdAdministrador { get; private set; }
    public bool Activo { get; private set; } = true;
    public DateTimeOffset FechaAlta { get; private set; } = DateTimeOffset.UtcNow;

    private Usuario() { }

    public Usuario(string nombreUsuario, string hashContrasena, Rol rol, Guid? idAdministrador)
    {
        if (string.IsNullOrWhiteSpace(nombreUsuario))
        {
            throw new ArgumentException("El nombre de usuario es obligatorio.", nameof(nombreUsuario));
        }

        NombreUsuario = nombreUsuario.Trim().ToLowerInvariant();
        HashContrasena = hashContrasena;
        Rol = rol;
        IdAdministrador = idAdministrador;
    }

    public void DarDeBaja() => Activo = false;
}
