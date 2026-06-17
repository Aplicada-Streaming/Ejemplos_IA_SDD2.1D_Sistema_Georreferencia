using GeoVial.WebApi.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoVial.WebApi.Controllers;

/// <summary>
/// Sincronización de campo del agente (F2, NB-04). El ciclo es subir-antes-de-bajar (RN-06):
/// primero el cliente sube el lote de cambios locales (CU-10) y luego baja las novedades del
/// relevamiento (CU-11). La idempotencia y el orden se gobiernan en la capa de aplicación.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/relevamientos/{id:guid}/sincronizacion")]
public sealed class SincronizacionController(IServicioSincronizacion servicio) : ControladorAutenticado
{
    [HttpPost("subida")]
    [ProducesResponseType(typeof(ResultadoSubida), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ResultadoSubida>> Subir(Guid id, [FromBody] LoteSincronizacion lote, CancellationToken ct)
        => Ok(await servicio.SubirAsync(IdUsuarioActual(), id, lote, ct));

    [HttpPost("bajada")]
    [ProducesResponseType(typeof(ResultadoBajada), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ResultadoBajada>> Bajar(Guid id, [FromBody] SolicitudBajada solicitud, CancellationToken ct)
        => Ok(await servicio.BajarAsync(IdUsuarioActual(), id, solicitud, ct));
}
