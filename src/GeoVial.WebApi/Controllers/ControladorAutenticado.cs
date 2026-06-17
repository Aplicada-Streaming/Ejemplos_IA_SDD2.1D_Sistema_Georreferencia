using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GeoVial.WebApi.Application;
using Microsoft.AspNetCore.Mvc;

namespace GeoVial.WebApi.Controllers;

/// <summary>
/// Base de los controladores autenticados: resuelve el identificador del usuario actual a
/// partir del claim <c>sub</c> del token (o <c>NameIdentifier</c> como alternativa).
/// </summary>
public abstract class ControladorAutenticado : ControllerBase
{
    protected Guid IdUsuarioActual()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(sub, out var id)
            ? id
            : throw new OperacionNoAutorizadaException("El token no identifica a un usuario válido.");
    }
}
