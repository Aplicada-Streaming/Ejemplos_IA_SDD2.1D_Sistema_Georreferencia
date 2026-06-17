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
/// y se asigna a agentes de campo. El ciclo de estados sigue RN-05: recolección ↔ revisión
/// (con retorno controlado), revisión → cierre (precondición: sin conflictos pendientes) y
/// reapertura cierre → revisión; ninguna otra transición es válida.
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

    /// <summary>Momento del cierre; nulo mientras no esté cerrado (CU-14).</summary>
    public DateTimeOffset? CerradoEn { get; private set; }

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
    public bool EstaEnRevision => Estado == EstadoRelevamiento.Revision;

    /// <summary>
    /// Aplica una transición de estado válida según RN-05. La precondición de ausencia de
    /// conflictos pendientes para el cierre se valida en la capa de aplicación (que conoce los
    /// conflictos). Transicionar al estado actual es un no-op idempotente.
    /// </summary>
    public void CambiarEstado(EstadoRelevamiento destino)
    {
        if (destino == Estado)
        {
            return;
        }

        var permitido = (Estado, destino) switch
        {
            (EstadoRelevamiento.Recoleccion, EstadoRelevamiento.Revision) => true,
            (EstadoRelevamiento.Revision, EstadoRelevamiento.Recoleccion) => true,
            (EstadoRelevamiento.Revision, EstadoRelevamiento.Cerrado) => true,
            (EstadoRelevamiento.Cerrado, EstadoRelevamiento.Revision) => true,
            _ => false,
        };

        if (!permitido)
        {
            throw new InvalidOperationException($"Transición no permitida de {Estado} a {destino} (RN-05).");
        }

        Estado = destino;
        CerradoEn = destino == EstadoRelevamiento.Cerrado ? DateTimeOffset.UtcNow : null;
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

    /// <summary>Identificador de origen del cliente para la idempotencia de la sincronización (RN-07); nulo si se creó en línea.</summary>
    public string? IdOrigen { get; private set; }

    public DateTimeOffset FechaCreacion { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>Marca temporal de la última modificación; sostiene el cálculo de novedades de la bajada (CU-11).</summary>
    public DateTimeOffset ActualizadoEn { get; private set; } = DateTimeOffset.UtcNow;

    private MarcadorGeografico() { }

    public MarcadorGeografico(Guid relevamientoId, double latitud, double longitud, string? descripcion, string? idOrigen = null)
    {
        ValidarCoordenada(latitud, longitud);
        RelevamientoId = relevamientoId;
        Latitud = latitud;
        Longitud = longitud;
        Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim();
        IdOrigen = string.IsNullOrWhiteSpace(idOrigen) ? null : idOrigen.Trim();
        ActualizadoEn = FechaCreacion;
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
        ActualizadoEn = DateTimeOffset.UtcNow;
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

    /// <summary>Identificador de origen del cliente para la idempotencia de la sincronización (RN-07); nulo si se creó en línea.</summary>
    public string? IdOrigen { get; private set; }

    public DateTimeOffset FechaCreacion { get; private set; } = DateTimeOffset.UtcNow;

    private Observacion() { }

    public Observacion(Guid marcadorId, Guid autorId, string? nota, string? idOrigen = null)
    {
        MarcadorId = marcadorId;
        AutorId = autorId;
        Nota = string.IsNullOrWhiteSpace(nota) ? null : nota.Trim();
        IdOrigen = string.IsNullOrWhiteSpace(idOrigen) ? null : idOrigen.Trim();
    }

    /// <summary>Reancla la observación a otro marcador (al unificar marcadores en conflicto, CU-13).</summary>
    public void Reanclar(Guid nuevoMarcadorId) => MarcadorId = nuevoMarcadorId;
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

/// <summary>
/// Foto de una observación (F2). El binario vive en el almacén de archivos vía la librería
/// de almacenamiento; aquí se guarda solo la referencia lógica (ADR-09). La ubicación se
/// resuelve priorizando la coordenada incrustada en la imagen sobre la asignada manualmente
/// (RN-04); si no hay ninguna, queda pendiente de ubicación.
/// </summary>
public sealed class Foto
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ObservacionId { get; private set; }
    public string ReferenciaAlmacen { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = "application/octet-stream";
    public double? Latitud { get; private set; }
    public double? Longitud { get; private set; }
    public bool PendienteUbicacion { get; private set; }
    public DateTimeOffset FechaCreacion { get; private set; } = DateTimeOffset.UtcNow;

    private Foto() { }

    public Foto(Guid observacionId, string referenciaAlmacen, string contentType, double? latitud, double? longitud)
    {
        if (latitud is { } la and (< -90 or > 90))
        {
            throw new ArgumentOutOfRangeException(nameof(latitud), "La latitud debe estar entre -90 y 90.");
        }

        if (longitud is { } lo and (< -180 or > 180))
        {
            throw new ArgumentOutOfRangeException(nameof(longitud), "La longitud debe estar entre -180 y 180.");
        }

        ObservacionId = observacionId;
        ReferenciaAlmacen = referenciaAlmacen;
        ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType;
        Latitud = latitud;
        Longitud = longitud;
        PendienteUbicacion = latitud is null || longitud is null;
    }
}

/// <summary>Estado de un conflicto de marcadores.</summary>
public enum EstadoConflicto
{
    Pendiente = 0,
    Resuelto = 1,
}

/// <summary>Decisión del jefe al resolver un conflicto de marcadores (CU-13).</summary>
public enum ResolucionConflicto
{
    Unificar = 0,
    Separar = 1,
}

/// <summary>
/// Conflicto de dos o más marcadores dentro de un mismo radio (RN-03). Es un estado válido que
/// convive con la recolección y la revisión, y se resuelve al cierre (CU-13): unificándolos en
/// uno solo o manteniéndolos separados. Su resolución es precondición del cierre (RN-05).
/// </summary>
public sealed class ConflictoMarcadores
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid RelevamientoId { get; private set; }
    public EstadoConflicto Estado { get; private set; } = EstadoConflicto.Pendiente;
    public ResolucionConflicto? Resolucion { get; private set; }
    public DateTimeOffset DetectadoEn { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ResueltoEn { get; private set; }

    private ConflictoMarcadores() { }

    public ConflictoMarcadores(Guid relevamientoId) => RelevamientoId = relevamientoId;

    public void Resolver(ResolucionConflicto resolucion)
    {
        Estado = EstadoConflicto.Resuelto;
        Resolucion = resolucion;
        ResueltoEn = DateTimeOffset.UtcNow;
    }
}

/// <summary>Marcador involucrado en un conflicto (relación 1—2..N del modelo conceptual).</summary>
public sealed class ConflictoMarcadorMiembro
{
    public Guid ConflictoId { get; private set; }
    public Guid MarcadorId { get; private set; }

    private ConflictoMarcadorMiembro() { }

    public ConflictoMarcadorMiembro(Guid conflictoId, Guid marcadorId)
    {
        ConflictoId = conflictoId;
        MarcadorId = marcadorId;
    }
}

/// <summary>
/// Punto de sincronización de un relevamiento para un cliente de campo (RC-06). Sostiene el
/// orden subir-antes-de-bajar (RN-06) con la compuerta <see cref="SubidaConcluida"/> y la
/// monotonía de la marca opaca <see cref="Valor"/>. Única por par (relevamiento, cliente).
/// </summary>
public sealed class MarcaSincronizacion
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid RelevamientoId { get; private set; }
    public Guid ClienteId { get; private set; }

    /// <summary>Marca opaca para el cliente; solo avanza (RC-06).</summary>
    public DateTimeOffset Valor { get; private set; }

    /// <summary>Compuerta del orden subir-antes-de-bajar del ciclo en curso (RN-06).</summary>
    public bool SubidaConcluida { get; private set; }

    public DateTimeOffset ActualizadoEn { get; private set; } = DateTimeOffset.UtcNow;

    private MarcaSincronizacion() { }

    public MarcaSincronizacion(Guid relevamientoId, Guid clienteId)
    {
        RelevamientoId = relevamientoId;
        ClienteId = clienteId;
        Valor = DateTimeOffset.UnixEpoch;
    }

    /// <summary>Marca la subida del ciclo como concluida (habilita la bajada, RN-06).</summary>
    public void ConcluirSubida()
    {
        SubidaConcluida = true;
        ActualizadoEn = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Adopta la nueva marca tras una bajada y reinicia la compuerta para exigir una nueva
    /// subida en el próximo ciclo. La marca solo avanza (RC-06).
    /// </summary>
    public void AvanzarMarca(DateTimeOffset nueva)
    {
        if (nueva > Valor)
        {
            Valor = nueva;
        }

        SubidaConcluida = false;
        ActualizadoEn = DateTimeOffset.UtcNow;
    }
}

/// <summary>Texto que describe una foto; a lo sumo uno por foto (cardinalidad 1—0..1).</summary>
public sealed class Comentario
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid FotoId { get; private set; }
    public string Texto { get; private set; } = string.Empty;
    public DateTimeOffset FechaCreacion { get; private set; } = DateTimeOffset.UtcNow;

    private Comentario() { }

    public Comentario(Guid fotoId, string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            throw new ArgumentException("El comentario no puede estar vacío.", nameof(texto));
        }

        FotoId = fotoId;
        Texto = texto.Trim();
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
