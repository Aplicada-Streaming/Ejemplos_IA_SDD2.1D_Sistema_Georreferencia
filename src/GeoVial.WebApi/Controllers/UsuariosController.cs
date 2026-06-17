using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GeoVial.WebApi.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoVial.WebApi.Controllers;

/// <summary>
/// Administración de usuarios por jerarquía (CU-02, CU-03; RN-01). Cada administrador
/// solo opera sobre el nivel inmediatamente inferior, validado en la capa de aplicación.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/usuarios")]
public sealed class UsuariosController(IServicioUsuarios servicio) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UsuarioDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UsuarioDto>>> Listar(CancellationToken ct)
    {
        var lista = await servicio.ListarAdministradosAsync(IdUsuarioActual(), ct);
        return Ok(lista);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioDto>> Crear([FromBody] SolicitudCrearUsuario solicitud, CancellationToken ct)
    {
        var creado = await servicio.CrearAsync(IdUsuarioActual(), solicitud, ct);
        return CreatedAtAction(nameof(Listar), new { id = creado.Id }, creado);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DarDeBaja(Guid id, CancellationToken ct)
    {
        await servicio.DarDeBajaAsync(IdUsuarioActual(), id, ct);
        return NoContent();
    }

    private Guid IdUsuarioActual()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                  ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(sub, out var id)
            ? id
            : throw new OperacionNoAutorizadaException("El token no identifica a un usuario válido.");
    }
}
