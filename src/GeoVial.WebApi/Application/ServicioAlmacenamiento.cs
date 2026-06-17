using GeoVial.Storage.Abstractions;
using GeoVial.WebApi.Domain;
using GeoVial.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GeoVial.WebApi.Application;

/// <summary>
/// Configuración del destino de almacenamiento (CU-17, NB-07). Solo el usuario raíz (RN-01)
/// consulta el destino activo, valida un proveedor sin activarlo (5.B) o cambia el destino activo,
/// de forma transparente para los demás roles. Delega la materialización del proveedor en la
/// librería de almacenamiento (<see cref="IRegistroAlmacenamiento"/>); nunca expone credenciales.
/// </summary>
public sealed class ServicioAlmacenamiento(GeoVialDbContext db, IRegistroAlmacenamiento registro) : IServicioAlmacenamiento
{
    public async Task<ConfiguracionAlmacenamientoDto> ObtenerAsync(Guid idUsuario, CancellationToken ct = default)
    {
        await GarantizarRaizAsync(idUsuario, ct);
        return Configuracion();
    }

    public async Task<ResultadoValidacionDto> ValidarAsync(Guid idUsuario, string proveedor, CancellationToken ct = default)
    {
        await GarantizarRaizAsync(idUsuario, ct);
        var nombre = Normalizar(proveedor);
        if (!Existe(nombre))
        {
            throw new ProveedorNoDisponibleException(proveedor);
        }

        // 5.B: valida alcance y credenciales sin cambiar el destino activo.
        var resultado = await registro.ValidarAsync(nombre, ct);
        return new ResultadoValidacionDto(resultado.Valido, resultado.Detalle);
    }

    public async Task<ConfiguracionAlmacenamientoDto> ActivarAsync(Guid idUsuario, string proveedor, CancellationToken ct = default)
    {
        await GarantizarRaizAsync(idUsuario, ct);
        var nombre = Normalizar(proveedor);
        if (!Existe(nombre))
        {
            throw new ProveedorNoDisponibleException(proveedor);
        }

        var validacion = await registro.ValidarAsync(nombre, ct);
        if (!validacion.Valido)
        {
            throw new CredencialesProveedorInvalidasException(validacion.Detalle);
        }

        registro.Activar(nombre);
        await PersistirActivoAsync(nombre, ct);
        return Configuracion();
    }

    private async Task PersistirActivoAsync(string proveedor, CancellationToken ct)
    {
        var ajuste = await db.Set<AjusteSistema>()
            .FirstOrDefaultAsync(a => a.Clave == AjusteSistema.ProveedorAlmacenamientoActivo, ct);
        if (ajuste is null)
        {
            db.Set<AjusteSistema>().Add(new AjusteSistema(AjusteSistema.ProveedorAlmacenamientoActivo, proveedor));
        }
        else
        {
            ajuste.Establecer(proveedor);
        }

        await db.SaveChangesAsync(ct);
    }

    private ConfiguracionAlmacenamientoDto Configuracion()
        => new(registro.Activo, registro.Disponibles.OrderBy(p => p).ToList());

    private bool Existe(string proveedor)
        => registro.Disponibles.Any(p => string.Equals(p, proveedor, StringComparison.OrdinalIgnoreCase));

    private static string Normalizar(string proveedor) => (proveedor ?? string.Empty).Trim().ToLowerInvariant();

    private async Task GarantizarRaizAsync(Guid idUsuario, CancellationToken ct)
    {
        var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.Id == idUsuario && u.Activo, ct);
        if (usuario is null || usuario.Rol != Rol.Raiz)
        {
            // RN-01: solo el nivel raíz configura el sistema.
            throw new RolNoAutorizadoException();
        }
    }
}
