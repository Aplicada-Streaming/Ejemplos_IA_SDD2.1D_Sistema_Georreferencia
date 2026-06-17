using GeoVial.WebApi.Domain;

namespace GeoVial.WebApi.Application;

// ---- Subida (CU-10) ----

/// <summary>Cambio de marcador capturado sin conexión; el <see cref="IdOrigen"/> habilita la idempotencia (RN-07).</summary>
public sealed record CambioMarcador(string IdOrigen, double Latitud, double Longitud, string? Descripcion);

/// <summary>
/// Observación capturada sin conexión, anclada a un marcador por su id de origen
/// (<see cref="MarcadorIdOrigen"/>), que puede venir en el mismo lote o ya existir.
/// </summary>
public sealed record CambioObservacion(string IdOrigen, string MarcadorIdOrigen, string? Nota);

public sealed record LoteSincronizacion(
    IReadOnlyList<CambioMarcador> Marcadores,
    IReadOnlyList<CambioObservacion> Observaciones);

public sealed record ResultadoSubida(int Aplicados, int Reenviados, int Conflictos);

// ---- Bajada (CU-11) ----

public sealed record SolicitudBajada(string? Marca);

public sealed record MarcadorBajadaDto(
    Guid Id, double Latitud, double Longitud, string? Descripcion, bool EnConflicto, DateTimeOffset ActualizadoEn);

public sealed record ObservacionBajadaDto(Guid Id, Guid MarcadorId, string? Nota, DateTimeOffset FechaCreacion);

public sealed record ResultadoBajada(
    EstadoRelevamiento Estado,
    IReadOnlyList<MarcadorBajadaDto> Marcadores,
    IReadOnlyList<ObservacionBajadaDto> Observaciones,
    string MarcaNueva);

// ---- Puerto de aplicación ----

public interface IServicioSincronizacion
{
    Task<ResultadoSubida> SubirAsync(Guid idAgente, Guid idRelevamiento, LoteSincronizacion lote, CancellationToken ct = default);
    Task<ResultadoBajada> BajarAsync(Guid idAgente, Guid idRelevamiento, SolicitudBajada solicitud, CancellationToken ct = default);
}

// ---- Errores ----

public sealed class RelevamientoNoAsignadoException()
    : ErrorAplicacion("RELEVAMIENTO_NO_ASIGNADO", "El relevamiento no está asignado al agente.");

public sealed class LoteMalformadoException(string mensaje)
    : ErrorAplicacion("LOTE_MALFORMADO", mensaje);

public sealed class SubidaNoConcluidaException()
    : ErrorAplicacion("SUBIDA_NO_CONCLUIDA", "Debe completar la subida del ciclo antes de bajar (RN-06).");

public sealed class MarcaInvalidaException()
    : ErrorAplicacion("MARCA_INVALIDA", "La marca de sincronización aportada no es reconocible.");
