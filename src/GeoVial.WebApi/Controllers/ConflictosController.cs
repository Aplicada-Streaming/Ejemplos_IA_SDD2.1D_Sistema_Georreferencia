using GeoVial.WebApi.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoVial.WebApi.Controllers;

/// <summary>
/// Conflictos de marcadores y su resolución al cierre (CU-13, F3). El jefe de área lista los
/// conflictos pendientes del relevamiento y los resuelve unificando o separando. El cierre del
/// relevamiento (gate de conflictos, RN-05) se hace por el endpoint de transición.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/relevamientos/{id:guid}/conflictos")]
public sealed class ConflictosController(IServicioConflictos servicio) : ControladorAutenticado
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ConflictoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<ConflictoDto>>> Listar(Guid id, CancellationToken ct)
        => Ok(await servicio.ListarPendientesAsync(IdUsuarioActual(), id, ct));

    [HttpPost("{idConflicto:guid}/resolucion")]
    [ProducesResponseType(typeof(ConflictoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ConflictoDto>> Resolver(Guid id, Guid idConflicto, [FromBody] ResolverConflictoRequest req, CancellationToken ct)
        => Ok(await servicio.ResolverAsync(IdUsuarioActual(), id, idConflicto, req.Resolucion, ct));
}
