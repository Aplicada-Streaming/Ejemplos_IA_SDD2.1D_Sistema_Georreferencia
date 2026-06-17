using System.Globalization;
using GeoVial.WebApi.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoVial.WebApi.Controllers;

/// <summary>
/// Captura y recuperación de fotos de observaciones (F2). La subida es multipart; el binario
/// se delega a la librería de almacenamiento y la API expone solo metadatos y un endpoint de
/// descarga del contenido. La autorización (jefe dueño o agente asignado) vive en la capa de
/// aplicación.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/relevamientos")]
public sealed class FotosController(IServicioFotos servicio) : ControladorAutenticado
{
    /// <summary>
    /// Formulario de carga de una foto (multipart/form-data). Las coordenadas se reciben como
    /// texto y se parsean con cultura invariante para no depender de la cultura del servidor.
    /// </summary>
    public sealed class CargaFotoForm
    {
        public IFormFile? Archivo { get; set; }
        public string? LatitudIncrustada { get; set; }
        public string? LongitudIncrustada { get; set; }
        public string? LatitudManual { get; set; }
        public string? LongitudManual { get; set; }
        public string? Comentario { get; set; }
    }

    private static double? ParsearCoordenada(string? valor)
        => double.TryParse(valor, NumberStyles.Float | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var d)
            ? d
            : null;

    [HttpPost("{id:guid}/observaciones/{idObservacion:guid}/fotos")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(FotoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FotoDto>> Agregar(Guid id, Guid idObservacion, [FromForm] CargaFotoForm form, CancellationToken ct)
    {
        if (form.Archivo is null || form.Archivo.Length == 0)
        {
            throw new ArchivoVacioException();
        }

        await using var contenido = form.Archivo.OpenReadStream();
        var ubicacion = new UbicacionFoto(
            ParsearCoordenada(form.LatitudIncrustada),
            ParsearCoordenada(form.LongitudIncrustada),
            ParsearCoordenada(form.LatitudManual),
            ParsearCoordenada(form.LongitudManual));
        var dto = await servicio.AgregarFotoAsync(
            IdUsuarioActual(), id, idObservacion, contenido, form.Archivo.ContentType, ubicacion, form.Comentario, ct);

        return CreatedAtAction(nameof(ListarPorObservacion), new { id, idObservacion }, dto);
    }

    [HttpGet("{id:guid}/observaciones/{idObservacion:guid}/fotos")]
    [ProducesResponseType(typeof(IReadOnlyList<FotoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<FotoDto>>> ListarPorObservacion(Guid id, Guid idObservacion, CancellationToken ct)
        => Ok(await servicio.ListarPorObservacionAsync(IdUsuarioActual(), id, idObservacion, ct));

    [HttpGet("{id:guid}/marcadores/{idMarcador:guid}/fotos")]
    [ProducesResponseType(typeof(IReadOnlyList<FotoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<FotoDto>>> ListarPorMarcador(Guid id, Guid idMarcador, CancellationToken ct)
        => Ok(await servicio.ListarPorMarcadorAsync(IdUsuarioActual(), id, idMarcador, ct));

    [HttpGet("{id:guid}/fotos/{idFoto:guid}/contenido")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Contenido(Guid id, Guid idFoto, CancellationToken ct)
    {
        var (contenido, contentType) = await servicio.ObtenerContenidoAsync(IdUsuarioActual(), id, idFoto, ct);
        return File(contenido, contentType);
    }
}
