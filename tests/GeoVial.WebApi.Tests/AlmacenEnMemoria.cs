using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using GeoVial.Storage.Abstractions;

namespace GeoVial.WebApi.Tests;

/// <summary>
/// Implementación de <see cref="IObjectStore"/> en memoria para las pruebas de integración:
/// evita tocar el disco y aísla cada instancia de la fábrica.
/// </summary>
public sealed class AlmacenEnMemoria : IObjectStore
{
    private readonly ConcurrentDictionary<string, (byte[] Datos, string ContentType, DateTimeOffset Fecha)> _objetos = new();

    public async Task<StoredObjectInfo> SaveAsync(string key, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        using var ms = new MemoryStream();
        await content.CopyToAsync(ms, cancellationToken);
        var datos = ms.ToArray();
        var fecha = DateTimeOffset.UnixEpoch;
        _objetos[key] = (datos, contentType, fecha);
        return new StoredObjectInfo(key, datos.Length, contentType, fecha);
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
        foreach (var (key, obj) in _objetos.Where(kv => kv.Key.StartsWith(prefix, StringComparison.Ordinal)))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return new StoredObjectInfo(key, obj.Datos.Length, obj.ContentType, obj.Fecha);
            await Task.CompletedTask;
        }
    }
}
