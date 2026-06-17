using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using GeoVial.Storage.Abstractions;

namespace GeoVial.Storage.Providers.Memory;

/// <summary>
/// Proveedor de almacenamiento en memoria del proceso. Es un destino alternativo válido
/// (efímero) que materializa <see cref="IObjectStore"/> sin tocar disco ni servicios externos;
/// sirve para despliegues de prueba/demo y como segundo proveedor conmutable (CU-17). Un proveedor
/// de objetos remoto (S3, Azure Blob, etc.) se integra del mismo modo, implementando este contrato.
/// </summary>
public sealed class MemoryObjectStore : IObjectStore
{
    private readonly ConcurrentDictionary<string, (byte[] Datos, string ContentType, DateTimeOffset Fecha)> _objetos = new(StringComparer.Ordinal);

    public async Task<StoredObjectInfo> SaveAsync(string key, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidObjectKeyException(key);
        }

        using var ms = new MemoryStream();
        await content.CopyToAsync(ms, cancellationToken);
        var datos = ms.ToArray();
        var info = (datos, contentType ?? "application/octet-stream", DateTimeOffset.UtcNow);
        _objetos[key] = info;
        return new StoredObjectInfo(key, datos.Length, info.Item2, info.Item3);
    }

    public Task<Stream> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        if (!_objetos.TryGetValue(key, out var obj))
        {
            throw new ObjectNotFoundException(key);
        }

        return Task.FromResult<Stream>(new MemoryStream(obj.Datos, writable: false));
    }

    public Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default)
        => Task.FromResult(_objetos.TryRemove(key, out _));

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        => Task.FromResult(_objetos.ContainsKey(key));

    public async IAsyncEnumerable<StoredObjectInfo> ListAsync(string prefix, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var (key, obj) in _objetos.Where(kv => kv.Key.StartsWith(prefix ?? string.Empty, StringComparison.Ordinal)))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return new StoredObjectInfo(key, obj.Datos.Length, obj.ContentType, obj.Fecha);
            await Task.CompletedTask;
        }
    }
}
