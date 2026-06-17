namespace GeoVial.Storage.Providers.Local;

/// <summary>
/// Configuración del proveedor de almacenamiento local (sobre el sistema de archivos
/// del contenedor del backend). Selección y parámetros los fija el usuario raíz (CU-06).
/// </summary>
public sealed class LocalStorageOptions
{
    public const string SectionName = "Storage:Local";

    /// <summary>Carpeta raíz donde se alojan los objetos.</summary>
    public string RootPath { get; set; } = "storage-data";

    /// <summary>Tamaño máximo de objeto en bytes. Por defecto 25 MB (NFR P.10).</summary>
    public long MaxSizeBytes { get; set; } = 25L * 1024 * 1024;
}
