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
        ValidarCoordenada(latitud, longitud);
        RelevamientoId = relevamientoId;
        Latitud = latitud;
        Longitud = longitud;
        Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim();
    }

    /// <summary>
    /// Reubica el marcador conservando su identidad (RC-01): el <see cref="Id"/> no cambia,
    /// de modo que las observaciones y etiquetas ancladas siguen vigentes tras el movimiento.
    /// </summary>
    public void Mover(double latitud, double longitud)
    {
        ValidarCoordenada(latitud, longitud);
        Latitud = latitud;
        Longitud = longitud;
    }

    private static void ValidarCoordenada(double latitud, double longitud)
    {
        if (latitud is < -90 or > 90)
        {
            throw new ArgumentOutOfRangeException(nameof(latitud), "La latitud debe estar entre -90 y 90.");
        }

        if (longitud is < -180 or > 180)
        {
            throw new ArgumentOutOfRangeException(nameof(longitud), "La longitud debe estar entre -180 y 180.");
        }
    }
}

/// <summary>
/// Observación anclada a un marcador geográfico (RC-02): registra el estado de un punto del
/// tramo con una nota y un autor identificado. Un marcador es compartible por varias
/// observaciones (relación 1—0..N del modelo conceptual).
/// </summary>
public sealed class Observacion
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid MarcadorId { get; private set; }
    public Guid AutorId { get; private set; }
    public string? Nota { get; private set; }
    public DateTimeOffset FechaCreacion { get; private set; } = DateTimeOffset.UtcNow;

    private Observacion() { }

    public Observacion(Guid marcadorId, Guid autorId, string? nota)
    {
        MarcadorId = marcadorId;
        AutorId = autorId;
        Nota = string.IsNullOrWhiteSpace(nota) ? null : nota.Trim();
    }
}

/// <summary>
/// Etiqueta reutilizable dentro de un relevamiento, aplicable a marcadores (y, en F2, a
/// fotos) para clasificarlos y filtrarlos en la revisión. Su nombre es único por relevamiento.
/// </summary>
public sealed class Etiqueta
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid RelevamientoId { get; private set; }
    public string Nombre { get; private set; } = string.Empty;

    private Etiqueta() { }

    public Etiqueta(Guid relevamientoId, string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre de la etiqueta es obligatorio.", nameof(nombre));
        }

        RelevamientoId = relevamientoId;
        Nombre = nombre.Trim();
    }
}

/// <summary>Vínculo N—N entre una etiqueta y un marcador geográfico.</summary>
public sealed class EtiquetaMarcador
{
    public Guid EtiquetaId { get; private set; }
    public Guid MarcadorId { get; private set; }

    private EtiquetaMarcador() { }

    public EtiquetaMarcador(Guid etiquetaId, Guid marcadorId)
    {
        EtiquetaId = etiquetaId;
        MarcadorId = marcadorId;
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
