using GeoVial.Storage.Abstractions;
using GeoVial.Storage.Providers;
using GeoVial.Storage.Providers.Local;
using GeoVial.Storage.Providers.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeoVial.Storage.DependencyInjection;

/// <summary>
/// Integración de la abstracción de almacenamiento al contenedor de servicios del backend.
/// Registra los proveedores disponibles (local, memoria) y un router conmutable como
/// <see cref="IObjectStore"/>: el nivel raíz puede consultar, validar y cambiar el destino
/// activo en caliente a través de <see cref="IRegistroAlmacenamiento"/> (CU-17), de forma
/// transparente para los demás roles. El destino inicial se toma de "Storage:Provider".
/// </summary>
public static class StorageServiceCollectionExtensions
{
    /// <summary>Identificadores de los proveedores de almacenamiento soportados.</summary>
    public const string ProveedorLocal = "local";
    public const string ProveedorMemoria = "memoria";

    public static IServiceCollection AddGeoVialStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<LocalStorageOptions>()
            .Bind(configuration.GetSection(LocalStorageOptions.SectionName));

        services.AddSingleton<LocalObjectStore>();
        services.AddSingleton<MemoryObjectStore>();

        var activoInicial = (configuration["Storage:Provider"] ?? ProveedorLocal).Trim().ToLowerInvariant();

        services.AddSingleton<RouterObjectStore>(sp =>
        {
            var proveedores = new Dictionary<string, IObjectStore>(StringComparer.OrdinalIgnoreCase)
            {
                [ProveedorLocal] = sp.GetRequiredService<LocalObjectStore>(),
                [ProveedorMemoria] = sp.GetRequiredService<MemoryObjectStore>(),
            };

            return new RouterObjectStore(proveedores, activoInicial);
        });

        // El mismo singleton expone el almacén (consumidores) y el registro (configuración raíz).
        services.AddSingleton<IObjectStore>(sp => sp.GetRequiredService<RouterObjectStore>());
        services.AddSingleton<IRegistroAlmacenamiento>(sp => sp.GetRequiredService<RouterObjectStore>());

        return services;
    }
}
