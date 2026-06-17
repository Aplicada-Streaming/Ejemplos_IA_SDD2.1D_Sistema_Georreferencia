using GeoVial.WebApi.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoVial.WebApi.Controllers;

/// <summary>
/// Portabilidad de relevamientos (CU-15/CU-16, NB-06): exporta un relevamiento como una unidad
/// transferible única (ZIP) e importa una unidad reconstruyendo su estructura. El alcance y el rol
/// se validan en la capa de aplicación.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/relevamientos")]
public sealed class PortabilidadController(IServicioPortabilidad servicio) : ControladorAutenticado
{
    [HttpGet("{id:guid}/exportacion")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Exportar(Guid id, CancellationToken ct)
    {
        var unidad = await servicio.ExportarAsync(IdUsuarioActual(), id, ct);
        return File(unidad, "application/zip", $"relevamiento-{id:N}.zip");
    }

    [HttpPost("importacion")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ResultadoImportacion), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResultadoImportacion>> Importar([FromForm] IFormFile unidad, CancellationToken ct)
    {
        if (unidad is null || unidad.Length == 0)
        {
            throw new UnidadInvalidaException("No se recibió ninguna unidad para importar.");
        }

        await using var contenido = unidad.OpenReadStream();
        var resultado = await servicio.ImportarAsync(IdUsuarioActual(), contenido, ct);
        return Created($"/api/v1/relevamientos/{resultado.IdRelevamiento}", resultado);
    }
}
