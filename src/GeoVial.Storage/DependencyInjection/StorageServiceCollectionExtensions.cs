using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using GeoVial.Storage.Abstractions;
using GeoVial.Storage.Providers;
using GeoVial.Storage.Providers.Local;
using GeoVial.Storage.Providers.Memory;
using GeoVial.Storage.Providers.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
    public const string ProveedorS3 = "s3";

    public static IServiceCollection AddGeoVialStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<LocalStorageOptions>()
            .Bind(configuration.GetSection(LocalStorageOptions.SectionName));

        services.AddSingleton<LocalObjectStore>();
        services.AddSingleton<MemoryObjectStore>();

        // Proveedor S3 (AWS o compatible) opcional: solo se habilita si hay un bucket configurado.
        services.AddOptions<S3StorageOptions>()
            .Bind(configuration.GetSection(S3StorageOptions.SectionName));

        var s3 = new S3StorageOptions();
        configuration.GetSection(S3StorageOptions.SectionName).Bind(s3);
        var s3Habilitado = !string.IsNullOrWhiteSpace(s3.Bucket);
        if (s3Habilitado)
        {
            services.AddSingleton<IAmazonS3>(_ => CrearClienteS3(s3));
            services.AddSingleton<S3ObjectStore>(sp =>
                new S3ObjectStore(sp.GetRequiredService<IAmazonS3>(), sp.GetRequiredService<IOptions<S3StorageOptions>>().Value));
        }

        var activoInicial = (configuration["Storage:Provider"] ?? ProveedorLocal).Trim().ToLowerInvariant();

        services.AddSingleton<RouterObjectStore>(sp =>
        {
            var proveedores = new Dictionary<string, IObjectStore>(StringComparer.OrdinalIgnoreCase)
            {
                [ProveedorLocal] = sp.GetRequiredService<LocalObjectStore>(),
                [ProveedorMemoria] = sp.GetRequiredService<MemoryObjectStore>(),
            };

            if (s3Habilitado)
            {
                proveedores[ProveedorS3] = sp.GetRequiredService<S3ObjectStore>();
            }

            return new RouterObjectStore(proveedores, activoInicial);
        });

        // El mismo singleton expone el almacén (consumidores) y el registro (configuración raíz).
        services.AddSingleton<IObjectStore>(sp => sp.GetRequiredService<RouterObjectStore>());
        services.AddSingleton<IRegistroAlmacenamiento>(sp => sp.GetRequiredService<RouterObjectStore>());

        return services;
    }

    private static IAmazonS3 CrearClienteS3(S3StorageOptions opciones)
    {
        var config = new AmazonS3Config();
        if (!string.IsNullOrWhiteSpace(opciones.ServiceUrl))
        {
            config.ServiceURL = opciones.ServiceUrl;
            config.ForcePathStyle = opciones.ForcePathStyle;
        }
        else if (!string.IsNullOrWhiteSpace(opciones.Region))
        {
            config.RegionEndpoint = RegionEndpoint.GetBySystemName(opciones.Region);
        }

        return !string.IsNullOrWhiteSpace(opciones.AccessKey) && !string.IsNullOrWhiteSpace(opciones.SecretKey)
            ? new AmazonS3Client(new BasicAWSCredentials(opciones.AccessKey, opciones.SecretKey), config)
            : new AmazonS3Client(config); // cadena de credenciales por defecto (rol/instancia/entorno)
    }
}
