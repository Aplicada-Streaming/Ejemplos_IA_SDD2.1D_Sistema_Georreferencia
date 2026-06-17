namespace GeoVial.Storage.Providers.S3;

/// <summary>
/// Opciones del proveedor de almacenamiento compatible con S3 (AWS S3 o un servicio de objetos
/// auto-hospedado como MinIO/Cloudflare R2). Se enlaza desde la sección "Storage:S3". El proveedor
/// solo se habilita si hay un <see cref="Bucket"/> configurado.
/// </summary>
public sealed class S3StorageOptions
{
    public const string SectionName = "Storage:S3";

    /// <summary>Bucket destino. Si está vacío, el proveedor s3 no se habilita.</summary>
    public string? Bucket { get; set; }

    /// <summary>Endpoint del servicio para proveedores compatibles (p. ej. http://localhost:9000 de MinIO). Nulo = AWS.</summary>
    public string? ServiceUrl { get; set; }

    /// <summary>Región de AWS (p. ej. us-east-1). Se ignora si se fija <see cref="ServiceUrl"/>.</summary>
    public string? Region { get; set; }

    public string? AccessKey { get; set; }

    public string? SecretKey { get; set; }

    /// <summary>Estilo de ruta (true para MinIO y la mayoría de los compatibles).</summary>
    public bool ForcePathStyle { get; set; }
}
