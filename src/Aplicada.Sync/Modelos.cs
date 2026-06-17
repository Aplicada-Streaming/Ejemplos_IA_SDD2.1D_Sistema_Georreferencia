namespace Aplicada.Sync;

/// <summary>Estado de una sesión de sincronización del motor.</summary>
public enum EstadoSesionSync
{
    NoAutenticada = 0,
    Lista = 1,
    Sincronizando = 2,
    Reanudable = 3,
}

/// <summary>
/// Cambio local capturado sin conexión. La <see cref="Carga"/> es opaca para el motor: la
/// librería no interpreta el contenido de dominio del host (CU-02). El <see cref="IdCambio"/>
/// estable es la base de la idempotencia (RN-02) y <see cref="OrdenCreacion"/> preserva el orden.
/// </summary>
public sealed record CambioLocal(string IdCambio, string Tipo, string Carga, long OrdenCreacion);

/// <summary>Actualización entregada por el backend en la bajada; puede venir en conflicto (RN-03).</summary>
public sealed record ActualizacionRemota(string Id, string Tipo, string Carga, bool EnConflicto);

/// <summary>Resultado de la fase de bajada: actualizaciones a aplicar y la nueva marca opaca.</summary>
public sealed record ResultadoBajada(IReadOnlyList<ActualizacionRemota> Actualizaciones, string NuevaMarca);

/// <summary>Resumen del ciclo de sincronización (parte de la superficie pública, CU-03 §17).</summary>
public sealed record ResumenCiclo(int Subidos, int Bajados, int EnConflicto, EstadoSesionSync EstadoFinal);

/// <summary>Estado de la cola de pendientes y de la sesión (CU-05).</summary>
public sealed record EstadoCola(int Pendientes, string? UltimaMarca, EstadoSesionSync Estado);

// ---- Errores del motor ----

public sealed class IdentificadorCambioAusenteException()
    : Exception("El cambio local no porta un identificador de cambio estable.");

public sealed class SesionNoInicializadaException()
    : Exception("No hay una sesión de sincronización inicializada.");

/// <summary>El backend remoto no respondió; el ciclo se detiene y la sesión queda reanudable (CU-03/CU-06).</summary>
public sealed class BackendInalcanzableException(Exception? inner = null)
    : Exception("El backend remoto no es alcanzable.", inner);

/// <summary>El backend rechazó la credencial; el host debe renovarla (CU-03).</summary>
public sealed class CredencialInvalidaException()
    : Exception("La credencial fue rechazada por el backend.");
