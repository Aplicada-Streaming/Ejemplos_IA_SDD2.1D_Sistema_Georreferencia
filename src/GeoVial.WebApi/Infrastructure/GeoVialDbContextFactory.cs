using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GeoVial.WebApi.Infrastructure;

/// <summary>
/// Fábrica de diseño usada por las herramientas de EF Core (migraciones). Construye el
/// contexto sin pasar por <c>Program</c>, evitando efectos secundarios (seed, arranque)
/// en tiempo de diseño. Las migraciones se generan para el proveedor del entorno de
/// desarrollo (almacén relacional embebido en archivo).
/// </summary>
public sealed class GeoVialDbContextFactory : IDesignTimeDbContextFactory<GeoVialDbContext>
{
    public GeoVialDbContext CreateDbContext(string[] args)
    {
        var opciones = new DbContextOptionsBuilder<GeoVialDbContext>()
            .UseSqlite("Data Source=geovial-design.db")
            .Options;

        return new GeoVialDbContext(opciones);
    }
}
