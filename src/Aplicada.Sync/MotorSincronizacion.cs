namespace Aplicada.Sync;

/// <summary>
/// Motor de sincronización del host de campo (aplicada-sync). Garantiza el orden
/// subir-luego-bajar (RN-01), la idempotencia por identificador de cambio (RN-02) y la
/// convivencia con estados en conflicto (RN-03). Si la subida se interrumpe, conserva la cola y
/// deja la sesión reanudable sin pérdida ni duplicación (CU-03/CU-06).
/// </summary>
public sealed class MotorSincronizacion(IAlmacenLocal almacen, IBackendSincronizacion backend)
{
    private EstadoSesionSync _estado = EstadoSesionSync.Lista;
    private bool _sincronizando;

    public EstadoSesionSync Estado => _estado;

    /// <summary>Registra y encola un cambio local (CU-02). Devuelve el tamaño actualizado de la cola.</summary>
    public async Task<int> EncolarAsync(CambioLocal cambio, CancellationToken ct = default)
    {
        if (cambio is null || string.IsNullOrWhiteSpace(cambio.IdCambio))
        {
            throw new IdentificadorCambioAusenteException();
        }

        // El almacén conserva una sola entrada por identificador (CU-02 5.A).
        await almacen.EncolarAsync(cambio, ct);
        return await almacen.ContarPendientesAsync(ct);
    }

    /// <summary>Consulta el estado de la cola y de la sesión (CU-05).</summary>
    public async Task<EstadoCola> EstadoColaAsync(CancellationToken ct = default)
        => new(await almacen.ContarPendientesAsync(ct), await almacen.ObtenerMarcaAsync(ct), _estado);

    /// <summary>Ejecuta un ciclo subir-luego-bajar (CU-03).</summary>
    public async Task<ResumenCiclo> SincronizarAsync(CancellationToken ct = default)
    {
        // 5.C: ya hay un ciclo en curso; no se inicia un segundo.
        if (_sincronizando)
        {
            return new ResumenCiclo(0, 0, 0, _estado);
        }

        _sincronizando = true;
        _estado = EstadoSesionSync.Sincronizando;
        try
        {
            var subidos = await SubirPendientesAsync(ct);
            var (bajados, enConflicto) = await BajarYAplicarAsync(ct);

            _estado = EstadoSesionSync.Lista;
            return new ResumenCiclo(subidos, bajados, enConflicto, _estado);
        }
        finally
        {
            _sincronizando = false;
        }
    }

    private async Task<int> SubirPendientesAsync(CancellationToken ct)
    {
        var pendientes = await almacen.ObtenerPendientesAsync(ct); // en orden de creación
        var subidos = 0;

        foreach (var cambio in pendientes)
        {
            try
            {
                await backend.SubirCambioAsync(cambio, ct);
            }
            catch (BackendInalcanzableException)
            {
                // Conserva los pendientes no confirmados; no inicia la bajada; sesión reanudable (CU-03 5.B/CA-03).
                _estado = EstadoSesionSync.Reanudable;
                throw;
            }
            catch (CredencialInvalidaException)
            {
                // Detiene el ciclo sin alterar la cola; el host debe renovar la credencial.
                _estado = EstadoSesionSync.NoAutenticada;
                throw;
            }

            await almacen.QuitarAsync(cambio.IdCambio, ct);
            subidos++;
        }

        return subidos;
    }

    private async Task<(int Bajados, int EnConflicto)> BajarYAplicarAsync(CancellationToken ct)
    {
        var marca = await almacen.ObtenerMarcaAsync(ct);
        var bajada = await backend.BajarAsync(marca, ct);

        var enConflicto = 0;
        foreach (var actualizacion in bajada.Actualizaciones)
        {
            // RN-03: las entidades en conflicto se aplican como estado válido, sin abortar.
            await almacen.AplicarActualizacionAsync(actualizacion, ct);
            if (actualizacion.EnConflicto)
            {
                enConflicto++;
            }
        }

        await almacen.GuardarMarcaAsync(bajada.NuevaMarca, ct);
        return (bajada.Actualizaciones.Count, enConflicto);
    }
}
