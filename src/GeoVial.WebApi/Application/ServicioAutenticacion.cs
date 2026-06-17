using GeoVial.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GeoVial.WebApi.Application;

public sealed class ServicioAutenticacion(
    GeoVialDbContext db,
    IHasheadorContrasena hasheador,
    IEmisorTokens emisorTokens) : IServicioAutenticacion
{
    public async Task<RespuestaLogin> LoginAsync(SolicitudLogin solicitud, CancellationToken ct = default)
    {
        var nombre = (solicitud.NombreUsuario ?? string.Empty).Trim().ToLowerInvariant();
        var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == nombre && u.Activo, ct);

        if (usuario is null || !hasheador.Verificar(solicitud.Contrasena ?? string.Empty, usuario.HashContrasena))
        {
            throw new CredencialesInvalidasException();
        }

        var (token, expiraEnSegundos) = emisorTokens.Emitir(usuario);
        var dto = new UsuarioDto(usuario.Id, usuario.NombreUsuario, usuario.Rol, usuario.Activo, usuario.FechaAlta);
        return new RespuestaLogin(token, "Bearer", expiraEnSegundos, dto);
    }
}
