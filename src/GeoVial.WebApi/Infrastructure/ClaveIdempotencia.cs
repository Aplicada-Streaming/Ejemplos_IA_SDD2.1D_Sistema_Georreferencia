namespace GeoVial.WebApi.Infrastructure;

/// <summary>Estado de procesamiento de una clave de idempotencia (ADR-08, CU-21).</summary>
public enum EstadoClaveIdempotencia
{
    EnCurso = 0,
    Completada = 1,
}

/// <summary>
/// Registro técnico de una clave de idempotencia y del resultado de la operación no segura que la
/// usó (ADR-08, RN-07, CU-21). Permite que un reintento con la misma clave devuelva el resultado
/// registrado sin reejecutar; una clave reutilizada con una huella de solicitud distinta se rechaza.
/// </summary>
public sealed class ClaveIdempotencia
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Clave { get; private set; } = string.Empty;

    /// <summary>Huella del contenido de la solicitud; detecta la reutilización con contenido distinto.</summary>
    public string HuellaSolicitud { get; private set; } = string.Empty;

    /// <summary>Respuesta registrada (estado, content-type y cuerpo) para reproducirla ante un reintento.</summary>
    public string? Resultado { get; private set; }

    public EstadoClaveIdempotencia Estado { get; private set; } = EstadoClaveIdempotencia.EnCurso;
    public DateTimeOffset CreadoEn { get; private set; } = DateTimeOffset.UtcNow;

    private ClaveIdempotencia() { }

    public ClaveIdempotencia(string clave, string huellaSolicitud)
    {
        Clave = clave;
        HuellaSolicitud = huellaSolicitud;
    }

    public void Completar(string resultado)
    {
        Resultado = resultado;
        Estado = EstadoClaveIdempotencia.Completada;
    }
}
