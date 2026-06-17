namespace GeoVial.WebApi.Domain;

/// <summary>Ciclo de vida del relevamiento (RN de transición): recolección → revisión → cierre.</summary>
public enum EstadoRelevamiento
{
    Recoleccion = 0,
    Revision = 1,
    Cerrado = 2,
}

/// <summary>
/// Relevamiento de un tramo vial: lo crea y administra el jefe de área, agrupa marcadores
/// y se asigna a agentes de campo. El estado avanza solo hacia adelante (recolección →
/// revisión → cierre); no se puede operar un relevamiento cerrado.
/// </summary>
public sealed class Relevamiento
{
    private readonly List<MarcadorGeografico> _marcadores = [];
    private readonly List<AsignacionAgente> _asignaciones = [];

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nombre { get; private set; } = string.Empty;
    public string TramoVial { get; private set; } = string.Empty;
    public EstadoRelevamiento Estado { get; private set; } = EstadoRelevamiento.Recoleccion;
    public Guid IdJefeArea { get; private set; }
    public DateTimeOffset FechaCreacion { get; private set; } = DateTimeOffset.UtcNow;

    public IReadOnlyCollection<MarcadorGeografico> Marcadores => _marcadores;
    public IReadOnlyCollection<AsignacionAgente> Asignaciones => _asignaciones;

    private Relevamiento() { }

    public Relevamiento(string nombre, string tramoVial, Guid idJefeArea)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del relevamiento es obligatorio.", nameof(nombre));
        }

        Nombre = nombre.Trim();
        TramoVial = (tramoVial ?? string.Empty).Trim();
        IdJefeArea = idJefeArea;
    }

    public bool EstaCerrado => Estado == EstadoRelevamiento.Cerrado;

    public void Avanzar(EstadoRelevamiento nuevo)
    {
        if ((int)nuevo != (int)Estado + 1)
        {
            throw new InvalidOperationException(
                $"Transición inválida de {Estado} a {nuevo}: el estado solo avanza un paso (recolección → revisión → cierre).");
        }

        Estado = nuevo;
    }

    public MarcadorGeografico AgregarMarcador(double latitud, double longitud, string? descripcion)
    {
        if (EstaCerrado)
        {
            throw new InvalidOperationException("No se pueden agregar marcadores a un relevamiento cerrado.");
        }

        var marcador = new MarcadorGeografico(Id, latitud, longitud, descripcion);
        _marcadores.Add(marcador);
        return marcador;
    }

    public void AsignarAgente(Guid idAgente)
    {
        if (EstaCerrado)
        {
            throw new InvalidOperationException("No se pueden asignar agentes a un relevamiento cerrado.");
        }

        if (_asignaciones.Any(a => a.IdAgente == idAgente))
        {
            return;
        }

        _asignaciones.Add(new AsignacionAgente(Id, idAgente));
    }
}

/// <summary>Marcador geográfico que agrupa observaciones dentro de un relevamiento.</summary>
public sealed class MarcadorGeografico
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid RelevamientoId { get; private set; }
    public double Latitud { get; private set; }
    public double Longitud { get; private set; }
    public string? Descripcion { get; private set; }
    public DateTimeOffset FechaCreacion { get; private set; } = DateTimeOffset.UtcNow;

    private MarcadorGeografico() { }

    public MarcadorGeografico(Guid relevamientoId, double latitud, double longitud, string? descripcion)
    {
        if (latitud is < -90 or > 90)
        {
            throw new ArgumentOutOfRangeException(nameof(latitud), "La latitud debe estar entre -90 y 90.");
        }

        if (longitud is < -180 or > 180)
        {
            throw new ArgumentOutOfRangeException(nameof(longitud), "La longitud debe estar entre -180 y 180.");
        }

        RelevamientoId = relevamientoId;
        Latitud = latitud;
        Longitud = longitud;
        Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim();
    }
}

/// <summary>Asignación de un agente de campo a un relevamiento.</summary>
public sealed class AsignacionAgente
{
    public Guid RelevamientoId { get; private set; }
    public Guid IdAgente { get; private set; }
    public DateTimeOffset FechaAsignacion { get; private set; } = DateTimeOffset.UtcNow;

    private AsignacionAgente() { }

    public AsignacionAgente(Guid relevamientoId, Guid idAgente)
    {
        RelevamientoId = relevamientoId;
        IdAgente = idAgente;
    }
}
