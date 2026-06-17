namespace Aplicada.Sync;

/// <summary>
/// Almacén local del host (cola de cambios pendientes, marca de sincronización y aplicación de
/// actualizaciones bajadas). El host provee la implementación concreta (por ejemplo SQLite en la
/// app móvil); la librería ofrece una en memoria para escenarios simples y pruebas.
/// </summary>
public interface IAlmacenLocal
{
    /// <summary>Devuelve los cambios pendientes en su orden de creación.</summary>
    Task<IReadOnlyList<CambioLocal>> ObtenerPendientesAsync(CancellationToken ct = default);

    /// <summary>Encola un cambio; si ya existe uno con el mismo <see cref="CambioLocal.IdCambio"/>, actualiza su carga sin duplicar (CU-02 5.A).</summary>
    Task EncolarAsync(CambioLocal cambio, CancellationToken ct = default);

    /// <summary>Quita de la cola el cambio confirmado por el backend.</summary>
    Task QuitarAsync(string idCambio, CancellationToken ct = default);

    Task<int> ContarPendientesAsync(CancellationToken ct = default);

    Task<string?> ObtenerMarcaAsync(CancellationToken ct = default);

    Task GuardarMarcaAsync(string marca, CancellationToken ct = default);

    /// <summary>Aplica al almacén local una actualización bajada del backend.</summary>
    Task AplicarActualizacionAsync(ActualizacionRemota actualizacion, CancellationToken ct = default);
}

/// <summary>
/// Puerto hacia el backend remoto de sincronización (geovial-api). El host lo implementa sobre
/// HTTP, mapeando la carga opaca al contrato real. Señala los fallos con excepciones tipadas para
/// que el motor preserve la cola y deje la sesión reanudable.
/// </summary>
public interface IBackendSincronizacion
{
    /// <summary>Sube un cambio local. La idempotencia por identificador la garantiza el backend (RN-02).</summary>
    /// <exception cref="BackendInalcanzableException">El backend no responde.</exception>
    /// <exception cref="CredencialInvalidaException">La credencial fue rechazada.</exception>
    Task SubirCambioAsync(CambioLocal cambio, CancellationToken ct = default);

    /// <summary>Baja las actualizaciones posteriores a la marca y devuelve la nueva marca.</summary>
    Task<ResultadoBajada> BajarAsync(string? marca, CancellationToken ct = default);
}
