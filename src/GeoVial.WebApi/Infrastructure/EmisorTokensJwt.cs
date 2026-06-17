using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GeoVial.WebApi.Application;
using GeoVial.WebApi.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GeoVial.WebApi.Infrastructure;

/// <summary>Opciones del token bearer firmado (clave de firma, emisor, audiencia, validez).</summary>
public sealed class OpcionesToken
{
    public const string SectionName = "Token";

    public string ClaveFirma { get; set; } = string.Empty;
    public string Emisor { get; set; } = "geovial-api";
    public string Audiencia { get; set; } = "geovial-clientes";
    public int MinutosValidez { get; set; } = 60;
}

/// <summary>
/// Emite tokens bearer firmados con clave simétrica. Las afirmaciones incluyen el
/// identificador del usuario y su rol, que el resto de la API usa para autorizar (RN-01).
/// </summary>
public sealed class EmisorTokensJwt(IOptions<OpcionesToken> opciones) : IEmisorTokens
{
    private readonly OpcionesToken _opciones = opciones.Value;

    public (string Token, int ExpiraEnSegundos) Emitir(Usuario usuario)
    {
        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opciones.ClaveFirma));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        var afirmaciones = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, usuario.NombreUsuario),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
        };

        var expira = DateTime.UtcNow.AddMinutes(_opciones.MinutosValidez);
        var token = new JwtSecurityToken(
            issuer: _opciones.Emisor,
            audience: _opciones.Audiencia,
            claims: afirmaciones,
            notBefore: DateTime.UtcNow,
            expires: expira,
            signingCredentials: credenciales);

        var serializado = new JwtSecurityTokenHandler().WriteToken(token);
        return (serializado, _opciones.MinutosValidez * 60);
    }
}

/// <summary>
/// Configura la validación del token bearer a partir del mismo <see cref="OpcionesToken"/>
/// que usa el emisor, leído de la configuración final (incluye overrides de entorno o de
/// pruebas). Evita el desajuste de clave entre emisión y validación.
/// </summary>
public sealed class ConfiguradorJwtBearer(IOptions<OpcionesToken> opciones) : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly OpcionesToken _opciones = opciones.Value;

    public void Configure(string? name, JwtBearerOptions options) => Configure(options);

    public void Configure(JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _opciones.Emisor,
            ValidAudience = _opciones.Audiencia,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opciones.ClaveFirma)),
        };
    }
}
