namespace Aplicada.Sync;

/// <summary>
/// Implementación en memoria de <see cref="IAlmacenLocal"/>: útil para escenarios simples del
/// host y para las pruebas. La app móvil reemplaza este almacén por uno persistente (SQLite).
/// Conserva el orden de creación y una sola entrada por identificador de cambio (CU-02).
/// </summary>
public sealed class AlmacenLocalEnMemoria : IAlmacenLocal
{
    private readonly object _candado = new();
    private readonly List<CambioLocal> _pendientes = [];
    private readonly List<ActualizacionRemota> _aplicadas = [];
    private string? _marca;

    /// <summary>Actualizaciones bajadas y aplicadas (para inspección del host/pruebas).</summary>
    public IReadOnlyList<ActualizacionRemota> Aplicadas
    {
        get { lock (_candado) { return _aplicadas.ToList(); } }
    }

    public Task<IReadOnlyList<CambioLocal>> ObtenerPendientesAsync(CancellationToken ct = default)
    {
        lock (_candado)
        {
            return Task.FromResult<IReadOnlyList<CambioLocal>>(
                _pendientes.OrderBy(c => c.OrdenCreacion).ToList());
        }
    }

    public Task EncolarAsync(CambioLocal cambio, CancellationToken ct = default)
    {
        lock (_candado)
        {
            var indice = _pendientes.FindIndex(c => c.IdCambio == cambio.IdCambio);
            if (indice >= 0)
            {
                // Reencolado idempotente: actualiza la carga conservando el orden original (CU-02 5.A).
                _pendientes[indice] = cambio with { OrdenCreacion = _pendientes[indice].OrdenCreacion };
            }
            else
            {
                _pendientes.Add(cambio);
            }
        }

        return Task.CompletedTask;
    }

    public Task QuitarAsync(string idCambio, CancellationToken ct = default)
    {
        lock (_candado)
        {
            _pendientes.RemoveAll(c => c.IdCambio == idCambio);
        }

        return Task.CompletedTask;
    }

    public Task<int> ContarPendientesAsync(CancellationToken ct = default)
    {
        lock (_candado)
        {
            return Task.FromResult(_pendientes.Count);
        }
    }

    public Task<string?> ObtenerMarcaAsync(CancellationToken ct = default)
    {
        lock (_candado)
        {
            return Task.FromResult(_marca);
        }
    }

    public Task GuardarMarcaAsync(string marca, CancellationToken ct = default)
    {
        lock (_candado)
        {
            _marca = marca;
        }

        return Task.CompletedTask;
    }

    public Task AplicarActualizacionAsync(ActualizacionRemota actualizacion, CancellationToken ct = default)
    {
        lock (_candado)
        {
            _aplicadas.Add(actualizacion);
        }

        return Task.CompletedTask;
    }
}
