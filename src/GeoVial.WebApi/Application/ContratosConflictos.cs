using GeoVial.WebApi.Domain;

namespace GeoVial.WebApi.Application;

// ---- DTOs ----

public sealed record ConflictoMarcadorDto(Guid Id, double Latitud, double Longitud, string? Descripcion);

public sealed record ConflictoDto(
    Guid Id,
    EstadoConflicto Estado,
    ResolucionConflicto? Resolucion,
    DateTimeOffset DetectadoEn,
    IReadOnlyList<ConflictoMarcadorDto> Marcadores);

public sealed record ResolverConflictoRequest(ResolucionConflicto Resolucion);

// ---- Puerto de aplicación ----

public interface IServicioConflictos
{
    Task<IReadOnlyList<ConflictoDto>> ListarPendientesAsync(Guid idJefe, Guid idRelevamiento, CancellationToken ct = default);
    Task<ConflictoDto> ResolverAsync(Guid idJefe, Guid idRelevamiento, Guid idConflicto, ResolucionConflicto resolucion, CancellationToken ct = default);
}

// ---- Errores ----

public sealed class ConflictoNoEncontradoException(Guid id)
    : ErrorAplicacion("CONFLICTO_INEXISTENTE", $"No se encontró un conflicto pendiente '{id}' en el relevamiento.");

public sealed class RelevamientoNoEnRevisionException()
    : ErrorAplicacion("RELEVAMIENTO_NO_EN_REVISION", "El relevamiento no está en revisión.");

public sealed class ConflictosPendientesException()
    : ErrorAplicacion("CONFLICTOS_PENDIENTES", "No se puede cerrar el relevamiento con conflictos de marcadores pendientes (RN-05).");
