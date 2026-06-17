using GeoVial.WebApi.Domain;

namespace GeoVial.WebApi.Application;

// ---- DTOs ----

public sealed record CrearRelevamientoRequest(string Nombre, string TramoVial);

public sealed record RelevamientoDto(
    Guid Id,
    string Nombre,
    string TramoVial,
    EstadoRelevamiento Estado,
    DateTimeOffset FechaCreacion,
    int CantidadMarcadores,
    int CantidadAgentes);

public sealed record CambiarEstadoRequest(EstadoRelevamiento NuevoEstado);

public sealed record AsignarAgenteRequest(Guid IdAgente);

public sealed record CrearMarcadorRequest(double Latitud, double Longitud, string? Descripcion);

public sealed record MarcadorDto(Guid Id, double Latitud, double Longitud, string? Descripcion, DateTimeOffset FechaCreacion);

// ---- Puerto de aplicación ----

public interface IServicioRelevamientos
{
    Task<RelevamientoDto> CrearAsync(Guid idJefe, CrearRelevamientoRequest req, CancellationToken ct = default);
    Task<IReadOnlyList<RelevamientoDto>> ListarAsync(Guid idUsuario, CancellationToken ct = default);
    Task<RelevamientoDto> CambiarEstadoAsync(Guid idJefe, Guid idRelevamiento, EstadoRelevamiento nuevo, CancellationToken ct = default);
    Task AsignarAgenteAsync(Guid idJefe, Guid idRelevamiento, Guid idAgente, CancellationToken ct = default);
    Task<IReadOnlyList<MarcadorDto>> ListarMarcadoresAsync(Guid idUsuario, Guid idRelevamiento, CancellationToken ct = default);
    Task<MarcadorDto> CrearMarcadorAsync(Guid idJefe, Guid idRelevamiento, CrearMarcadorRequest req, CancellationToken ct = default);
}

// ---- Errores ----

public sealed class RelevamientoNoEncontradoException(Guid id)
    : ErrorAplicacion("RELEVAMIENTO_NO_ENCONTRADO", $"No se encontró el relevamiento '{id}'.");

public sealed class TransicionEstadoInvalidaException(string mensaje)
    : ErrorAplicacion("TRANSICION_INVALIDA", mensaje);

public sealed class RelevamientoCerradoException()
    : ErrorAplicacion("RELEVAMIENTO_CERRADO", "El relevamiento está cerrado y no admite cambios.");

public sealed class AgenteInvalidoException(string mensaje)
    : ErrorAplicacion("AGENTE_INVALIDO", mensaje);

public sealed class CoordenadaInvalidaException(string mensaje)
    : ErrorAplicacion("COORDENADA_INVALIDA", mensaje);
