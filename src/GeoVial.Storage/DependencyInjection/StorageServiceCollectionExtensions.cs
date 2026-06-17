using GeoVial.Storage.Abstractions;
using GeoVial.Storage.Providers.Local;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeoVial.Storage.DependencyInjection;

/// <summary>
/// Integración de la abstracción de almacenamiento al contenedor de servicios del
/// backend. La selección del proveedor activo (CU-06) la fija el usuario raíz por
/// configuración; en este incremento se ofrece el proveedor local.
/// </summary>
public static class StorageServiceCollectionExtensions
{
    /// <summary>
    /// Registra el proveedor de almacenamiento activo como <see cref="IObjectStore"/>.
    /// Lee la sección "Storage:Provider" (por defecto "local") y enlaza las opciones
    /// del proveedor elegido.
    /// </summary>
    public static IServiceCollection AddGeoVialStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["Storage:Provider"] ?? "local";

        switch (provider.ToLowerInvariant())
        {
            case "local":
                services.AddOptions<LocalStorageOptions>()
                    .Bind(configuration.GetSection(LocalStorageOptions.SectionName));
                services.AddSingleton<IObjectStore, LocalObjectStore>();
                break;

            default:
                throw new ProviderNotConfiguredException(
                    $"El proveedor de almacenamiento '{provider}' no está soportado en este incremento.");
        }

        return services;
    }
}
