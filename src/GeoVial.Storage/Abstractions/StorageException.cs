namespace GeoVial.Storage.Abstractions;

/// <summary>
/// Error de la abstracción de almacenamiento. Cada subtipo declara un código estable,
/// alineado con el catálogo de errores de 03 (dx-error-messages) y 10 (referencia-api),
/// para que el consumidor pueda diagnosticar sin acoplarse al proveedor.
/// </summary>
public abstract class StorageException : Exception
{
    protected StorageException(string code, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        Code = code;
    }

    /// <summary>Código estable del error (por ejemplo ARCHIVO_INEXISTENTE).</summary>
    public string Code { get; }
}

/// <summary>El objeto solicitado no existe. Código: ARCHIVO_INEXISTENTE.</summary>
public sealed class ObjectNotFoundException(string key)
    : StorageException("ARCHIVO_INEXISTENTE", $"No existe un objeto con la clave '{key}'.")
{
    public string Key { get; } = key;
}

/// <summary>El proveedor activo no está configurado. Código: PROVEEDOR_NO_CONFIGURADO.</summary>
public sealed class ProviderNotConfiguredException(string message)
    : StorageException("PROVEEDOR_NO_CONFIGURADO", message);

/// <summary>El objeto supera el tamaño máximo configurado. Código: TAMANIO_EXCEDIDO.</summary>
public sealed class ObjectTooLargeException(long sizeBytes, long maxSizeBytes)
    : StorageException("TAMANIO_EXCEDIDO",
        $"El objeto de {sizeBytes} bytes supera el máximo permitido de {maxSizeBytes} bytes.")
{
    public long SizeBytes { get; } = sizeBytes;
    public long MaxSizeBytes { get; } = maxSizeBytes;
}

/// <summary>La clave del objeto es inválida. Código: CLAVE_INVALIDA.</summary>
public sealed class InvalidObjectKeyException(string key)
    : StorageException("CLAVE_INVALIDA", $"La clave '{key}' no es válida.")
{
    public string Key { get; } = key;
}
