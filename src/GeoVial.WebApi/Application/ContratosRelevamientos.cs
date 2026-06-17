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

public sealed record MoverMarcadorRequest(double Latitud, double Longitud);

public sealed record MarcadorDto(
    Guid Id,
    double Latitud,
    double Longitud,
    string? Descripcion,
    DateTimeOffset FechaCreacion,
    int CantidadObservaciones,
    IReadOnlyList<string> Etiquetas);

public sealed record CrearObservacionRequest(string? Nota);

public sealed record ObservacionDto(Guid Id, Guid MarcadorId, Guid AutorId, string? Nota, DateTimeOffset FechaCreacion);

public sealed record CrearEtiquetaRequest(string Nombre);

public sealed record EtiquetaDto(Guid Id, string Nombre);

public sealed record EtiquetarMarcadorRequest(Guid IdEtiqueta);

// ---- Puerto de aplicación ----

public interface IServicioRelevamientos
{
    Task<RelevamientoDto> CrearAsync(Guid idJefe, CrearRelevamientoRequest req, CancellationToken ct = default);
    Task<IReadOnlyList<RelevamientoDto>> ListarAsync(Guid idUsuario, CancellationToken ct = default);
    Task<RelevamientoDto> CambiarEstadoAsync(Guid idJefe, Guid idRelevamiento, EstadoRelevamiento nuevo, CancellationToken ct = default);
    Task AsignarAgenteAsync(Guid idJefe, Guid idRelevamiento, Guid idAgente, CancellationToken ct = default);
    Task<IReadOnlyList<MarcadorDto>> ListarMarcadoresAsync(Guid idUsuario, Guid idRelevamiento, CancellationToken ct = default);
    Task<MarcadorDto> CrearMarcadorAsync(Guid idJefe, Guid idRelevamiento, CrearMarcadorRequest req, CancellationToken ct = default);
    Task<MarcadorDto> MoverMarcadorAsync(Guid idJefe, Guid idRelevamiento, Guid idMarcador, MoverMarcadorRequest req, CancellationToken ct = default);
    Task BajaMarcadorAsync(Guid idJefe, Guid idRelevamiento, Guid idMarcador, CancellationToken ct = default);

    Task<ObservacionDto> CrearObservacionAsync(Guid idUsuario, Guid idRelevamiento, Guid idMarcador, CrearObservacionRequest req, CancellationToken ct = default);
    Task<IReadOnlyList<ObservacionDto>> ListarObservacionesAsync(Guid idUsuario, Guid idRelevamiento, Guid idMarcador, CancellationToken ct = default);

    Task<EtiquetaDto> CrearEtiquetaAsync(Guid idJefe, Guid idRelevamiento, CrearEtiquetaRequest req, CancellationToken ct = default);
    Task<IReadOnlyList<EtiquetaDto>> ListarEtiquetasAsync(Guid idUsuario, Guid idRelevamiento, CancellationToken ct = default);
    Task EtiquetarMarcadorAsync(Guid idJefe, Guid idRelevamiento, Guid idMarcador, Guid idEtiqueta, CancellationToken ct = default);
    Task QuitarEtiquetaMarcadorAsync(Guid idJefe, Guid idRelevamiento, Guid idMarcador, Guid idEtiqueta, CancellationToken ct = default);
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

public sealed class MarcadorNoEncontradoException(Guid id)
    : ErrorAplicacion("MARCADOR_NO_ENCONTRADO", $"No se encontró el marcador '{id}' en el relevamiento.");

public sealed class MarcadorConObservacionesException()
    : ErrorAplicacion("MARCADOR_CON_OBSERVACIONES", "No se puede dar de baja un marcador que tiene observaciones.");

public sealed class EtiquetaYaExisteException(string nombre)
    : ErrorAplicacion("ETIQUETA_YA_EXISTE", $"Ya existe una etiqueta '{nombre}' en el relevamiento.");

public sealed class EtiquetaInvalidaException(string mensaje)
    : ErrorAplicacion("ETIQUETA_INVALIDA", mensaje);

public sealed class EtiquetaNoEncontradaException(Guid id)
    : ErrorAplicacion("ETIQUETA_NO_ENCONTRADA", $"No se encontró la etiqueta '{id}' en el relevamiento.");
