using GeoVial.WebApi.Domain;

namespace GeoVial.WebApi.Application;

// ---- Manifiesto de la unidad transferible (CU-15/CU-16, NB-06) ----
// Es el formato del empaquetado (categoría 05): un ZIP con "manifiesto.json" + los binarios de
// las fotos bajo "fotos/". Las referencias internas se preservan con identificadores locales (Ref).

public sealed record ManifiestoExport(
    int Version,
    RelevamientoExport Relevamiento,
    IReadOnlyList<string> Etiquetas,
    IReadOnlyList<MarcadorExport> Marcadores);

public sealed record RelevamientoExport(
    string Nombre,
    string TramoVial,
    EstadoRelevamiento Estado,
    DateTimeOffset FechaCreacion);

public sealed record MarcadorExport(
    string Ref,
    double Latitud,
    double Longitud,
    string? Descripcion,
    IReadOnlyList<string> Etiquetas,
    IReadOnlyList<ObservacionExport> Observaciones);

public sealed record ObservacionExport(
    string Ref,
    string? Nota,
    IReadOnlyList<FotoExport> Fotos);

public sealed record FotoExport(
    string Archivo,
    double? Latitud,
    double? Longitud,
    bool PendienteUbicacion,
    string ContentType,
    string? Comentario);

/// <summary>Resultado de una importación: ubicación del relevamiento reconstruido y fotos no alojadas (CU-16 5.B).</summary>
public sealed record ResultadoImportacion(Guid IdRelevamiento, IReadOnlyList<string> FotosNoAlojadas);

// ---- Puerto de aplicación ----

public interface IServicioPortabilidad
{
    /// <summary>Exporta el relevamiento como una unidad transferible única (ZIP). CU-15.</summary>
    Task<byte[]> ExportarAsync(Guid idJefe, Guid idRelevamiento, CancellationToken ct = default);

    /// <summary>Reconstruye un relevamiento desde una unidad transferible en el ámbito del solicitante. CU-16.</summary>
    Task<ResultadoImportacion> ImportarAsync(Guid idUsuario, Stream unidad, CancellationToken ct = default);
}

// ---- Errores ----

public sealed class RelevamientoFueraDeAmbitoException()
    : ErrorAplicacion("RELEVAMIENTO_FUERA_DE_AMBITO", "El relevamiento no pertenece al jefe solicitante.");

public sealed class FotoNoRecuperableException(string referencia)
    : ErrorAplicacion("FOTO_NO_RECUPERABLE", $"No se pudo recuperar del almacén la foto '{referencia}'.");

public sealed class UnidadInvalidaException(string mensaje)
    : ErrorAplicacion("UNIDAD_INVALIDA", mensaje);

public sealed class UnidadIncompletaException(string mensaje)
    : ErrorAplicacion("UNIDAD_INCOMPLETA", mensaje);

public sealed class RolNoAutorizadoException()
    : ErrorAplicacion("ROL_NO_AUTORIZADO", "El rol del solicitante no está habilitado para esta operación.");
