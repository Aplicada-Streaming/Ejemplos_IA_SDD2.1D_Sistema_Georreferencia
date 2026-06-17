using GeoVial.Storage.Abstractions;
using GeoVial.WebApi.Application;
using GeoVial.WebApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace GeoVial.WebApi.Infrastructure;

/// <summary>
/// Inicialización del almacén: aplica las migraciones versionadas (proveedor relacional)
/// y da de alta el usuario raíz a partir de la configuración (Seed:UsuarioRaiz /
/// Seed:ContrasenaRaiz). El raíz es el único usuario sin administrador y la cabeza de la
/// jerarquía (SOLUTION-INTAKE §2). Para el proveedor en memoria de las pruebas, que no
/// soporta migraciones, se crea el esquema con EnsureCreated.
/// </summary>
public static class SeedInicial
{
    public static async Task InicializarAsync(IServiceProvider servicios, CancellationToken ct = default)
    {
        using var scope = servicios.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GeoVialDbContext>();
        var hasheador = scope.ServiceProvider.GetRequiredService<IHasheadorContrasena>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        if (db.Database.IsRelational())
        {
            await db.Database.MigrateAsync(ct);
        }
        else
        {
            await db.Database.EnsureCreatedAsync(ct);
        }

        await RestaurarDestinoAlmacenamientoAsync(scope, db, ct);

        var nombreRaiz = (config["Seed:UsuarioRaiz"] ?? "raiz").Trim().ToLowerInvariant();
        if (await db.Usuarios.AnyAsync(u => u.Rol == Rol.Raiz, ct))
        {
            return;
        }

        var contrasenaRaiz = config["Seed:ContrasenaRaiz"] ?? "CambiarEnPrimerInicio!";
        var raiz = new Usuario(nombreRaiz, hasheador.Hashear(contrasenaRaiz), Rol.Raiz, idAdministrador: null);
        db.Usuarios.Add(raiz);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Restaura el destino de almacenamiento activo que el raíz haya fijado en un arranque previo
    /// (CU-17), de modo que la elección sobreviva a los reinicios. Si no hay ninguno persistido o el
    /// proveedor ya no está disponible, se conserva el destino por defecto (Storage:Provider).
    /// </summary>
    private static async Task RestaurarDestinoAlmacenamientoAsync(IServiceScope scope, GeoVialDbContext db, CancellationToken ct)
    {
        var registro = scope.ServiceProvider.GetRequiredService<IRegistroAlmacenamiento>();
        var ajuste = await db.Set<AjusteSistema>()
            .FirstOrDefaultAsync(a => a.Clave == AjusteSistema.ProveedorAlmacenamientoActivo, ct);

        if (ajuste is not null &&
            registro.Disponibles.Any(p => string.Equals(p, ajuste.Valor, StringComparison.OrdinalIgnoreCase)))
        {
            registro.Activar(ajuste.Valor);
        }
    }
}
