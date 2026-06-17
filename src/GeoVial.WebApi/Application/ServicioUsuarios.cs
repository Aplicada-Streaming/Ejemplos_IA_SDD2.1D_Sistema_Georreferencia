using GeoVial.WebApi.Domain;
using GeoVial.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GeoVial.WebApi.Application;

public sealed class ServicioUsuarios(GeoVialDbContext db, IHasheadorContrasena hasheador) : IServicioUsuarios
{
    public async Task<UsuarioDto> CrearAsync(Guid idAdministrador, SolicitudCrearUsuario solicitud, CancellationToken ct = default)
    {
        var administrador = await db.Usuarios.FirstOrDefaultAsync(u => u.Id == idAdministrador && u.Activo, ct)
            ?? throw new OperacionNoAutorizadaException("El administrador no existe o está inactivo.");

        if (!administrador.Rol.PuedeAdministrar(solicitud.Rol))
        {
            throw new OperacionNoAutorizadaException(
                $"Un {administrador.Rol} no puede dar de alta usuarios con rol {solicitud.Rol}.");
        }

        var nombre = solicitud.NombreUsuario.Trim().ToLowerInvariant();
        if (await db.Usuarios.AnyAsync(u => u.NombreUsuario == nombre, ct))
        {
            throw new UsuarioYaExisteException(nombre);
        }

        var usuario = new Usuario(nombre, hasheador.Hashear(solicitud.Contrasena), solicitud.Rol, idAdministrador);
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync(ct);

        return Mapear(usuario);
    }

    public async Task<IReadOnlyList<UsuarioDto>> ListarAdministradosAsync(Guid idAdministrador, CancellationToken ct = default)
    {
        var administrador = await db.Usuarios.FirstOrDefaultAsync(u => u.Id == idAdministrador && u.Activo, ct)
            ?? throw new OperacionNoAutorizadaException("El administrador no existe o está inactivo.");

        var rolAdministrado = administrador.Rol.RolQueAdministra();
        if (rolAdministrado is null)
        {
            return [];
        }

        var usuarios = await db.Usuarios
            .Where(u => u.Rol == rolAdministrado)
            .OrderBy(u => u.NombreUsuario)
            .ToListAsync(ct);

        return usuarios.Select(Mapear).ToList();
    }

    public async Task DarDeBajaAsync(Guid idAdministrador, Guid idUsuario, CancellationToken ct = default)
    {
        var administrador = await db.Usuarios.FirstOrDefaultAsync(u => u.Id == idAdministrador && u.Activo, ct)
            ?? throw new OperacionNoAutorizadaException("El administrador no existe o está inactivo.");

        var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.Id == idUsuario, ct)
            ?? throw new UsuarioNoEncontradoException(idUsuario);

        if (!administrador.Rol.PuedeAdministrar(usuario.Rol))
        {
            throw new OperacionNoAutorizadaException(
                $"Un {administrador.Rol} no puede dar de baja usuarios con rol {usuario.Rol}.");
        }

        usuario.DarDeBaja();
        await db.SaveChangesAsync(ct);
    }

    private static UsuarioDto Mapear(Usuario u) => new(u.Id, u.NombreUsuario, u.Rol, u.Activo, u.FechaAlta);
}
