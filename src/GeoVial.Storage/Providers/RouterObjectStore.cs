using GeoVial.Storage.Abstractions;

namespace GeoVial.Storage.Providers;

/// <summary>
/// Almacén que enruta las operaciones al proveedor activo y permite conmutarlo en caliente (CU-17).
/// Las escrituras van siempre al proveedor activo; las lecturas, borrados y existencia consultan el
/// activo y, como respaldo, los demás proveedores, de modo que los archivos alojados antes de un
/// cambio de destino siguen accesibles (CU-17 5.A) de forma transparente al consumidor.
/// </summary>
public sealed class RouterObjectStore : IObjectStore, IRegistroAlmacenamiento
{
    private readonly IReadOnlyDictionary<string, IObjectStore> _proveedores;
    private readonly object _candado = new();
    private string _activo;

    public RouterObjectStore(IReadOnlyDictionary<string, IObjectStore> proveedores, string activoInicial)
    {
        _proveedores = proveedores ?? throw new ArgumentNullException(nameof(proveedores));
        if (!_proveedores.ContainsKey(activoInicial))
        {
            throw new ProviderNotConfiguredException($"El proveedor de almacenamiento '{activoInicial}' no está disponible.");
        }

        _activo = activoInicial;
    }

    public string Activo
    {
        get { lock (_candado) { return _activo; } }
    }

    public IReadOnlyCollection<string> Disponibles => _proveedores.Keys.ToArray();

    public void Activar(string proveedor)
    {
        if (!_proveedores.ContainsKey(proveedor))
        {
            throw new ProviderNotConfiguredException($"El proveedor de almacenamiento '{proveedor}' no está disponible.");
        }

        lock (_candado)
        {
            _activo = proveedor;
        }
    }

    public async Task<ResultadoValidacionAlmacen> ValidarAsync(string proveedor, CancellationToken cancellationToken = default)
    {
        if (!_proveedores.TryGetValue(proveedor, out var store))
        {
            return new ResultadoValidacionAlmacen(false, "El proveedor no está disponible.");
        }

        // Sonda no intrusiva: aloja, recupera y elimina un objeto efímero (CU-17 §3/§5.B).
        var clave = $"_validacion/{Guid.NewGuid():N}";
        try
        {
            using (var contenido = new MemoryStream(new byte[] { 0x47, 0x56 }))
            {
                await store.SaveAsync(clave, contenido, "application/octet-stream", cancellationToken);
            }

            await using (await store.GetAsync(clave, cancellationToken))
            {
            }

            await store.DeleteAsync(clave, cancellationToken);
            return new ResultadoValidacionAlmacen(true, null);
        }
        catch (Exception ex)
        {
            return new ResultadoValidacionAlmacen(false, ex.Message);
        }
    }

    public Task<StoredObjectInfo> SaveAsync(string key, Stream content, string contentType, CancellationToken cancellationToken = default)
        => Activa().SaveAsync(key, content, contentType, cancellationToken);

    public async Task<Stream> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        var activo = Activa();
        try
        {
            return await activo.GetAsync(key, cancellationToken);
        }
        catch (ObjectNotFoundException)
        {
            foreach (var store in _proveedores.Values)
            {
                if (ReferenceEquals(store, activo))
                {
                    continue;
                }

                try
                {
                    return await store.GetAsync(key, cancellationToken);
                }
                catch (ObjectNotFoundException)
                {
                    // Sigue con el próximo proveedor de respaldo.
                }
            }

            throw new ObjectNotFoundException(key);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        foreach (var store in _proveedores.Values)
        {
            if (await store.ExistsAsync(key, cancellationToken))
            {
                return true;
            }
        }

        return false;
    }

    public async Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        var eliminado = false;
        foreach (var store in _proveedores.Values)
        {
            eliminado |= await store.DeleteAsync(key, cancellationToken);
        }

        return eliminado;
    }

    public IAsyncEnumerable<StoredObjectInfo> ListAsync(string prefix, CancellationToken cancellationToken = default)
        => Activa().ListAsync(prefix, cancellationToken);

    private IObjectStore Activa()
    {
        lock (_candado)
        {
            return _proveedores[_activo];
        }
    }
}
