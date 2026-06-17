using System.Runtime.CompilerServices;
using GeoVial.Storage.Abstractions;
using Microsoft.Extensions.Options;

namespace GeoVial.Storage.Providers.Local;

/// <summary>
/// Proveedor de almacenamiento sobre el sistema de archivos local. Es uno de los
/// proveedores intercambiables detrás de <see cref="IObjectStore"/>; el consumidor
/// no distingue el proveedor activo (transparencia, RN-01). Materializa el flujo de
/// enrutado de 05 (flujo-ejecucion) para el caso local.
/// </summary>
public sealed class LocalObjectStore : IObjectStore
{
    private readonly LocalStorageOptions _options;
    private readonly string _root;

    public LocalObjectStore(IOptions<LocalStorageOptions> options)
    {
        _options = options.Value;
        _root = Path.GetFullPath(_options.RootPath);
        Directory.CreateDirectory(_root);
    }

    public async Task<StoredObjectInfo> SaveAsync(string key, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        var path = ResolvePath(key);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        long written;
        await using (var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await content.CopyToAsync(file, cancellationToken);
            written = file.Length;
        }

        if (written > _options.MaxSizeBytes)
        {
            File.Delete(path);
            throw new ObjectTooLargeException(written, _options.MaxSizeBytes);
        }

        var info = new FileInfo(path);
        return new StoredObjectInfo(NormalizeKey(key), info.Length, contentType, info.LastWriteTimeUtc);
    }

    public Task<Stream> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        var path = ResolvePath(key);
        if (!File.Exists(path))
        {
            throw new ObjectNotFoundException(key);
        }

        Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult(stream);
    }

    public Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        var path = ResolvePath(key);
        if (!File.Exists(path))
        {
            return Task.FromResult(false);
        }

        File.Delete(path);
        return Task.FromResult(true);
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        => Task.FromResult(File.Exists(ResolvePath(key)));

    public async IAsyncEnumerable<StoredObjectInfo> ListAsync(string prefix, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(_root))
        {
            yield break;
        }

        var normalizedPrefix = NormalizeKey(prefix ?? string.Empty);
        foreach (var path in Directory.EnumerateFiles(_root, "*", SearchOption.AllDirectories))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var key = Path.GetRelativePath(_root, path).Replace(Path.DirectorySeparatorChar, '/');
            if (normalizedPrefix.Length > 0 && !key.StartsWith(normalizedPrefix, StringComparison.Ordinal))
            {
                continue;
            }

            var info = new FileInfo(path);
            yield return new StoredObjectInfo(key, info.Length, "application/octet-stream", info.LastWriteTimeUtc);
        }

        await Task.CompletedTask;
    }

    private string ResolvePath(string key)
    {
        var normalized = NormalizeKey(key);
        if (normalized.Length == 0)
        {
            throw new InvalidObjectKeyException(key);
        }

        var full = Path.GetFullPath(Path.Combine(_root, normalized));
        // Evita el traversal fuera de la raíz del proveedor.
        if (!full.StartsWith(_root + Path.DirectorySeparatorChar, StringComparison.Ordinal) && full != _root)
        {
            throw new InvalidObjectKeyException(key);
        }

        return full;
    }

    private static string NormalizeKey(string key)
    {
        var original = key ?? string.Empty;
        var trimmed = original.Replace('\\', '/').Trim('/');
        var segments = trimmed.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Any(s => s is "." or ".."))
        {
            throw new InvalidObjectKeyException(original);
        }

        return string.Join('/', segments);
    }
}
