using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GeoVial.WebApi.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoVial.WebApi.Controllers;

/// <summary>
/// Gestión de relevamientos, su ciclo de estado, la asignación de agentes y los
/// marcadores geográficos (CU de NB-02; F1 del roadmap). La autorización por rol y la
/// pertenencia se validan en la capa de aplicación.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/relevamientos")]
public sealed class RelevamientosController(IServicioRelevamientos servicio) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RelevamientoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RelevamientoDto>>> Listar(CancellationToken ct)
        => Ok(await servicio.ListarAsync(IdUsuarioActual(), ct));

    [HttpPost]
    [ProducesResponseType(typeof(RelevamientoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<RelevamientoDto>> Crear([FromBody] CrearRelevamientoRequest req, CancellationToken ct)
    {
        var creado = await servicio.CrearAsync(IdUsuarioActual(), req, ct);
        return CreatedAtAction(nameof(Listar), new { id = creado.Id }, creado);
    }

    [HttpPost("{id:guid}/transicion")]
    [ProducesResponseType(typeof(RelevamientoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RelevamientoDto>> CambiarEstado(Guid id, [FromBody] CambiarEstadoRequest req, CancellationToken ct)
        => Ok(await servicio.CambiarEstadoAsync(IdUsuarioActual(), id, req.NuevoEstado, ct));

    [HttpPost("{id:guid}/agentes")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AsignarAgente(Guid id, [FromBody] AsignarAgenteRequest req, CancellationToken ct)
    {
        await servicio.AsignarAgenteAsync(IdUsuarioActual(), id, req.IdAgente, ct);
        return NoContent();
    }

    [HttpGet("{id:guid}/marcadores")]
    [ProducesResponseType(typeof(IReadOnlyList<MarcadorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MarcadorDto>>> ListarMarcadores(Guid id, CancellationToken ct)
        => Ok(await servicio.ListarMarcadoresAsync(IdUsuarioActual(), id, ct));

    [HttpPost("{id:guid}/marcadores")]
    [ProducesResponseType(typeof(MarcadorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MarcadorDto>> CrearMarcador(Guid id, [FromBody] CrearMarcadorRequest req, CancellationToken ct)
    {
        var creado = await servicio.CrearMarcadorAsync(IdUsuarioActual(), id, req, ct);
        return CreatedAtAction(nameof(ListarMarcadores), new { id }, creado);
    }

    private Guid IdUsuarioActual()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(sub, out var id)
            ? id
            : throw new OperacionNoAutorizadaException("El token no identifica a un usuario válido.");
    }
}
