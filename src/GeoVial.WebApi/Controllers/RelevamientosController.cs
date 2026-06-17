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
public sealed class RelevamientosController(IServicioRelevamientos servicio) : ControladorAutenticado
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

    [HttpPut("{id:guid}/marcadores/{idMarcador:guid}")]
    [ProducesResponseType(typeof(MarcadorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MarcadorDto>> MoverMarcador(Guid id, Guid idMarcador, [FromBody] MoverMarcadorRequest req, CancellationToken ct)
        => Ok(await servicio.MoverMarcadorAsync(IdUsuarioActual(), id, idMarcador, req, ct));

    [HttpDelete("{id:guid}/marcadores/{idMarcador:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> BajaMarcador(Guid id, Guid idMarcador, CancellationToken ct)
    {
        await servicio.BajaMarcadorAsync(IdUsuarioActual(), id, idMarcador, ct);
        return NoContent();
    }

    [HttpGet("{id:guid}/marcadores/{idMarcador:guid}/observaciones")]
    [ProducesResponseType(typeof(IReadOnlyList<ObservacionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ObservacionDto>>> ListarObservaciones(Guid id, Guid idMarcador, CancellationToken ct)
        => Ok(await servicio.ListarObservacionesAsync(IdUsuarioActual(), id, idMarcador, ct));

    [HttpPost("{id:guid}/marcadores/{idMarcador:guid}/observaciones")]
    [ProducesResponseType(typeof(ObservacionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ObservacionDto>> CrearObservacion(Guid id, Guid idMarcador, [FromBody] CrearObservacionRequest req, CancellationToken ct)
    {
        var creada = await servicio.CrearObservacionAsync(IdUsuarioActual(), id, idMarcador, req, ct);
        return CreatedAtAction(nameof(ListarObservaciones), new { id, idMarcador }, creada);
    }

    [HttpGet("{id:guid}/etiquetas")]
    [ProducesResponseType(typeof(IReadOnlyList<EtiquetaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<EtiquetaDto>>> ListarEtiquetas(Guid id, CancellationToken ct)
        => Ok(await servicio.ListarEtiquetasAsync(IdUsuarioActual(), id, ct));

    [HttpPost("{id:guid}/etiquetas")]
    [ProducesResponseType(typeof(EtiquetaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EtiquetaDto>> CrearEtiqueta(Guid id, [FromBody] CrearEtiquetaRequest req, CancellationToken ct)
    {
        var creada = await servicio.CrearEtiquetaAsync(IdUsuarioActual(), id, req, ct);
        return CreatedAtAction(nameof(ListarEtiquetas), new { id }, creada);
    }

    [HttpPost("{id:guid}/marcadores/{idMarcador:guid}/etiquetas")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EtiquetarMarcador(Guid id, Guid idMarcador, [FromBody] EtiquetarMarcadorRequest req, CancellationToken ct)
    {
        await servicio.EtiquetarMarcadorAsync(IdUsuarioActual(), id, idMarcador, req.IdEtiqueta, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}/marcadores/{idMarcador:guid}/etiquetas/{idEtiqueta:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> QuitarEtiquetaMarcador(Guid id, Guid idMarcador, Guid idEtiqueta, CancellationToken ct)
    {
        await servicio.QuitarEtiquetaMarcadorAsync(IdUsuarioActual(), id, idMarcador, idEtiqueta, ct);
        return NoContent();
    }
}
