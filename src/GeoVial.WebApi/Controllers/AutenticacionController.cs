using GeoVial.WebApi.Application;
using Microsoft.AspNetCore.Mvc;

namespace GeoVial.WebApi.Controllers;

/// <summary>
/// Inicio de sesión con credenciales (CU-01). Devuelve un token bearer que los clientes
/// presentan en las llamadas autenticadas a la API.
/// </summary>
[ApiController]
[Route("api/v1/sesion")]
public sealed class AutenticacionController(IServicioAutenticacion servicio) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(RespuestaLogin), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<RespuestaLogin>> Login([FromBody] SolicitudLogin solicitud, CancellationToken ct)
    {
        var respuesta = await servicio.LoginAsync(solicitud, ct);
        return Ok(respuesta);
    }
}
