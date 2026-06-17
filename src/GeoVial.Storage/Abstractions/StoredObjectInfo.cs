namespace GeoVial.Storage.Abstractions;

/// <summary>
/// Metadatos de un objeto almacenado, independientes del proveedor.
/// </summary>
public sealed record StoredObjectInfo(
    string Key,
    long SizeBytes,
    string ContentType,
    DateTimeOffset LastModified);
