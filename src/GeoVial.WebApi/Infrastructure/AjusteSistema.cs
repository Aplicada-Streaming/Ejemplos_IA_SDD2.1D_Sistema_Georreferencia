namespace GeoVial.WebApi.Infrastructure;

/// <summary>
/// Ajuste de configuración del sistema persistido como clave-valor. Sostiene preferencias que el
/// nivel raíz fija en runtime y que deben sobrevivir a un reinicio (por ejemplo, el destino de
/// almacenamiento activo, CU-17).
/// </summary>
public sealed class AjusteSistema
{
    /// <summary>Clave del ajuste del destino de almacenamiento activo.</summary>
    public const string ProveedorAlmacenamientoActivo = "almacenamiento.proveedor.activo";

    public string Clave { get; private set; } = string.Empty;
    public string Valor { get; private set; } = string.Empty;
    public DateTimeOffset ActualizadoEn { get; private set; } = DateTimeOffset.UtcNow;

    private AjusteSistema() { }

    public AjusteSistema(string clave, string valor)
    {
        Clave = clave;
        Valor = valor;
    }

    public void Establecer(string valor)
    {
        Valor = valor;
        ActualizadoEn = DateTimeOffset.UtcNow;
    }
}
