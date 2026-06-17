using GeoVial.Storage.Abstractions;
using GeoVial.WebApi.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeoVial.WebApi.Tests;

/// <summary>
/// Fábrica de la API para pruebas de integración. Reemplaza el almacén relacional por
/// una base en memoria aislada por instancia y fija credenciales de seed y clave de token
/// deterministas, de modo que cada prueba ejercite el camino end-to-end sin infraestructura.
/// </summary>
public sealed class FabricaWebApi : WebApplicationFactory<Program>
{
    public const string UsuarioRaiz = "raiz";
    public const string ContrasenaRaiz = "Clave.Test.2026";

    private readonly string _dbName = Guid.NewGuid().ToString("N");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, cfg) =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Seed:UsuarioRaiz"] = UsuarioRaiz,
                ["Seed:ContrasenaRaiz"] = ContrasenaRaiz,
                ["Token:ClaveFirma"] = "clave-de-pruebas-suficientemente-larga-para-hmacsha256-0123456789",
            });
        });

        builder.ConfigureServices(servicios =>
        {
            foreach (var tipo in new[] { typeof(DbContextOptions<GeoVialDbContext>), typeof(GeoVialDbContext) })
            {
                var descriptor = servicios.SingleOrDefault(d => d.ServiceType == tipo);
                if (descriptor is not null)
                {
                    servicios.Remove(descriptor);
                }
            }

            servicios.AddDbContext<GeoVialDbContext>(o => o.UseInMemoryDatabase(_dbName));

            // Almacén de archivos en memoria: las pruebas no tocan el disco.
            var almacen = servicios.SingleOrDefault(d => d.ServiceType == typeof(IObjectStore));
            if (almacen is not null)
            {
                servicios.Remove(almacen);
            }

            servicios.AddSingleton<IObjectStore, AlmacenEnMemoria>();
        });
    }
}
