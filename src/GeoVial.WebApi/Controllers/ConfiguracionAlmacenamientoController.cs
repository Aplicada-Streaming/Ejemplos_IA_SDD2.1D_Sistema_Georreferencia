using GeoVial.WebApi.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoVial.WebApi.Controllers;

/// <summary>
/// Configuración del destino de almacenamiento de archivos (CU-17, NB-07). Operaciones reservadas
/// al usuario raíz (validado en la capa de aplicación): consultar el destino activo, validar un
/// proveedor sin activarlo y cambiar el destino activo. No expone credenciales de los proveedores.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/almacenamiento")]
public sealed class ConfiguracionAlmacenamientoController(IServicioAlmacenamiento servicio) : ControladorAutenticado
{
    [HttpGet]
    [ProducesResponseType(typeof(ConfiguracionAlmacenamientoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ConfiguracionAlmacenamientoDto>> Obtener(CancellationToken ct)
        => Ok(await servicio.ObtenerAsync(IdUsuarioActual(), ct));

    [HttpPost("validacion")]
    [ProducesResponseType(typeof(ResultadoValidacionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResultadoValidacionDto>> Validar([FromBody] ValidarAlmacenamientoRequest req, CancellationToken ct)
        => Ok(await servicio.ValidarAsync(IdUsuarioActual(), req.Proveedor, ct));

    [HttpPut("activo")]
    [ProducesResponseType(typeof(ConfiguracionAlmacenamientoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ConfiguracionAlmacenamientoDto>> Activar([FromBody] ActivarAlmacenamientoRequest req, CancellationToken ct)
        => Ok(await servicio.ActivarAsync(IdUsuarioActual(), req.Proveedor, ct));
}
