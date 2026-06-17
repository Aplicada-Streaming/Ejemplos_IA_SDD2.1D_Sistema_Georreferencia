namespace GeoVial.WebApi.Application;

// ---- DTOs (CU-17) ----

public sealed record ConfiguracionAlmacenamientoDto(string Activo, IReadOnlyList<string> Disponibles);

public sealed record ActivarAlmacenamientoRequest(string Proveedor);

public sealed record ValidarAlmacenamientoRequest(string Proveedor);

public sealed record ResultadoValidacionDto(bool Valido, string? Detalle);

// ---- Puerto de aplicación ----

public interface IServicioAlmacenamiento
{
    Task<ConfiguracionAlmacenamientoDto> ObtenerAsync(Guid idUsuario, CancellationToken ct = default);
    Task<ResultadoValidacionDto> ValidarAsync(Guid idUsuario, string proveedor, CancellationToken ct = default);
    Task<ConfiguracionAlmacenamientoDto> ActivarAsync(Guid idUsuario, string proveedor, CancellationToken ct = default);
}

// ---- Errores ----

public sealed class ProveedorNoDisponibleException(string proveedor)
    : ErrorAplicacion("PROVEEDOR_NO_DISPONIBLE", $"El proveedor de almacenamiento '{proveedor}' no está disponible.");

public sealed class CredencialesProveedorInvalidasException(string? detalle)
    : ErrorAplicacion("CREDENCIALES_PROVEEDOR_INVALIDAS", detalle ?? "El proveedor no permite alojar ni recuperar archivos.");
