namespace GeoVial.WebApi.Application;

// ---- DTOs ----

/// <summary>Coordenadas candidatas de una foto: la incrustada en la imagen tiene prioridad sobre la manual (RN-04).</summary>
public sealed record UbicacionFoto(
    double? LatitudIncrustada,
    double? LongitudIncrustada,
    double? LatitudManual,
    double? LongitudManual);

public sealed record FotoDto(
    Guid Id,
    Guid ObservacionId,
    double? Latitud,
    double? Longitud,
    bool PendienteUbicacion,
    string? Comentario,
    string ContentType,
    DateTimeOffset FechaCreacion);

// ---- Puerto de aplicación ----

public interface IServicioFotos
{
    Task<FotoDto> AgregarFotoAsync(
        Guid idUsuario, Guid idRelevamiento, Guid idObservacion,
        Stream contenido, string contentType, UbicacionFoto ubicacion, string? comentario,
        CancellationToken ct = default);

    Task<IReadOnlyList<FotoDto>> ListarPorObservacionAsync(Guid idUsuario, Guid idRelevamiento, Guid idObservacion, CancellationToken ct = default);

    Task<IReadOnlyList<FotoDto>> ListarPorMarcadorAsync(Guid idUsuario, Guid idRelevamiento, Guid idMarcador, CancellationToken ct = default);

    Task<(Stream Contenido, string ContentType)> ObtenerContenidoAsync(Guid idUsuario, Guid idRelevamiento, Guid idFoto, CancellationToken ct = default);
}

// ---- Errores ----

public sealed class ObservacionNoEncontradaException(Guid id)
    : ErrorAplicacion("OBSERVACION_NO_ENCONTRADA", $"No se encontró la observación '{id}' en el relevamiento.");

public sealed class FotoNoEncontradaException(Guid id)
    : ErrorAplicacion("FOTO_NO_ENCONTRADA", $"No se encontró la foto '{id}' en el relevamiento.");

public sealed class TipoArchivoInvalidoException(string mensaje)
    : ErrorAplicacion("TIPO_ARCHIVO_INVALIDO", mensaje);

public sealed class ArchivoVacioException()
    : ErrorAplicacion("ARCHIVO_VACIO", "El archivo de la foto está vacío.");
