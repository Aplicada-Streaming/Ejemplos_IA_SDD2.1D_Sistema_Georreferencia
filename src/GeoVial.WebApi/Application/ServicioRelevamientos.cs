using GeoVial.WebApi.Domain;
using GeoVial.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GeoVial.WebApi.Application;

public sealed class ServicioRelevamientos(GeoVialDbContext db) : IServicioRelevamientos
{
    public async Task<RelevamientoDto> CrearAsync(Guid idJefe, CrearRelevamientoRequest req, CancellationToken ct = default)
    {
        var jefe = await UsuarioActivoAsync(idJefe, ct);
        if (jefe.Rol != Rol.JefeDeArea)
        {
            throw new OperacionNoAutorizadaException("Solo un jefe de área puede crear relevamientos.");
        }

        var rel = new Relevamiento(req.Nombre, req.TramoVial, idJefe);
        db.Relevamientos.Add(rel);
        await db.SaveChangesAsync(ct);
        return new RelevamientoDto(rel.Id, rel.Nombre, rel.TramoVial, rel.Estado, rel.FechaCreacion, 0, 0);
    }

    public async Task<IReadOnlyList<RelevamientoDto>> ListarAsync(Guid idUsuario, CancellationToken ct = default)
    {
        var usuario = await UsuarioActivoAsync(idUsuario, ct);

        var consulta = usuario.Rol switch
        {
            Rol.JefeDeArea => db.Relevamientos.Where(r => r.IdJefeArea == idUsuario),
            Rol.AgenteDeCampo => db.Relevamientos.Where(r => r.Asignaciones.Any(a => a.IdAgente == idUsuario)),
            _ => db.Relevamientos.Where(_ => false),
        };

        // Proyección con conteos sin materializar las colecciones (evita tracking).
        var filas = await consulta
            .OrderByDescending(r => r.FechaCreacion)
            .Select(r => new RelevamientoDto(
                r.Id, r.Nombre, r.TramoVial, r.Estado, r.FechaCreacion, r.Marcadores.Count, r.Asignaciones.Count))
            .ToListAsync(ct);

        return filas;
    }

    public async Task<RelevamientoDto> CambiarEstadoAsync(Guid idJefe, Guid idRelevamiento, EstadoRelevamiento nuevo, CancellationToken ct = default)
    {
        var rel = await CargarDelJefeAsync(idJefe, idRelevamiento, ct);
        try
        {
            rel.Avanzar(nuevo);
        }
        catch (InvalidOperationException ex)
        {
            throw new TransicionEstadoInvalidaException(ex.Message);
        }

        await db.SaveChangesAsync(ct);
        var marcadores = await db.Set<MarcadorGeografico>().CountAsync(m => m.RelevamientoId == rel.Id, ct);
        var agentes = await db.Set<AsignacionAgente>().CountAsync(a => a.RelevamientoId == rel.Id, ct);
        return new RelevamientoDto(rel.Id, rel.Nombre, rel.TramoVial, rel.Estado, rel.FechaCreacion, marcadores, agentes);
    }

    public async Task AsignarAgenteAsync(Guid idJefe, Guid idRelevamiento, Guid idAgente, CancellationToken ct = default)
    {
        var rel = await CargarDelJefeAsync(idJefe, idRelevamiento, ct);
        if (rel.EstaCerrado)
        {
            throw new RelevamientoCerradoException();
        }

        var agente = await db.Usuarios.FirstOrDefaultAsync(u => u.Id == idAgente && u.Activo, ct);
        if (agente is null || agente.Rol != Rol.AgenteDeCampo)
        {
            throw new AgenteInvalidoException("Solo se pueden asignar agentes de campo activos.");
        }

        var yaAsignado = await db.Set<AsignacionAgente>()
            .AnyAsync(a => a.RelevamientoId == idRelevamiento && a.IdAgente == idAgente, ct);
        if (yaAsignado)
        {
            return;
        }

        db.Set<AsignacionAgente>().Add(new AsignacionAgente(idRelevamiento, idAgente));
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<MarcadorDto>> ListarMarcadoresAsync(Guid idUsuario, Guid idRelevamiento, CancellationToken ct = default)
    {
        await GarantizarAccesoAsync(idUsuario, idRelevamiento, ct);

        var marcadores = await db.Set<MarcadorGeografico>()
            .Where(m => m.RelevamientoId == idRelevamiento)
            .OrderBy(m => m.FechaCreacion)
            .ToListAsync(ct);

        var ids = marcadores.Select(m => m.Id).ToList();

        var conteos = (await db.Set<Observacion>()
                .Where(o => ids.Contains(o.MarcadorId))
                .GroupBy(o => o.MarcadorId)
                .Select(g => new { MarcadorId = g.Key, Total = g.Count() })
                .ToListAsync(ct))
            .ToDictionary(x => x.MarcadorId, x => x.Total);

        var etiquetas = (await db.Set<EtiquetaMarcador>()
                .Where(em => ids.Contains(em.MarcadorId))
                .Join(db.Set<Etiqueta>(), em => em.EtiquetaId, e => e.Id, (em, e) => new { em.MarcadorId, e.Nombre })
                .ToListAsync(ct))
            .GroupBy(x => x.MarcadorId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Nombre).OrderBy(n => n).ToList());

        return marcadores
            .Select(m => new MarcadorDto(
                m.Id, m.Latitud, m.Longitud, m.Descripcion, m.FechaCreacion,
                conteos.GetValueOrDefault(m.Id, 0),
                etiquetas.TryGetValue(m.Id, out var ns) ? ns : []))
            .ToList();
    }

    public async Task<MarcadorDto> CrearMarcadorAsync(Guid idJefe, Guid idRelevamiento, CrearMarcadorRequest req, CancellationToken ct = default)
    {
        var rel = await CargarDelJefeAsync(idJefe, idRelevamiento, ct);
        if (rel.EstaCerrado)
        {
            throw new RelevamientoCerradoException();
        }

        MarcadorGeografico marcador;
        try
        {
            marcador = new MarcadorGeografico(idRelevamiento, req.Latitud, req.Longitud, req.Descripcion);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new CoordenadaInvalidaException(ex.Message);
        }

        db.Set<MarcadorGeografico>().Add(marcador);
        await db.SaveChangesAsync(ct);
        return new MarcadorDto(marcador.Id, marcador.Latitud, marcador.Longitud, marcador.Descripcion, marcador.FechaCreacion, 0, []);
    }

    public async Task<MarcadorDto> MoverMarcadorAsync(Guid idJefe, Guid idRelevamiento, Guid idMarcador, MoverMarcadorRequest req, CancellationToken ct = default)
    {
        var rel = await CargarDelJefeAsync(idJefe, idRelevamiento, ct);
        if (rel.EstaCerrado)
        {
            throw new RelevamientoCerradoException();
        }

        var marcador = await CargarMarcadorAsync(idRelevamiento, idMarcador, ct);
        try
        {
            // RC-01: el marcador conserva su Id al moverse; observaciones y etiquetas siguen ancladas.
            marcador.Mover(req.Latitud, req.Longitud);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new CoordenadaInvalidaException(ex.Message);
        }

        await db.SaveChangesAsync(ct);

        var observaciones = await db.Set<Observacion>().CountAsync(o => o.MarcadorId == idMarcador, ct);
        var etiquetas = await EtiquetasDeMarcadorAsync(idMarcador, ct);
        return new MarcadorDto(marcador.Id, marcador.Latitud, marcador.Longitud, marcador.Descripcion, marcador.FechaCreacion, observaciones, etiquetas);
    }

    public async Task BajaMarcadorAsync(Guid idJefe, Guid idRelevamiento, Guid idMarcador, CancellationToken ct = default)
    {
        var rel = await CargarDelJefeAsync(idJefe, idRelevamiento, ct);
        if (rel.EstaCerrado)
        {
            throw new RelevamientoCerradoException();
        }

        var marcador = await CargarMarcadorAsync(idRelevamiento, idMarcador, ct);

        // US-15 / RC-02: solo se da de baja un marcador sin observaciones.
        var tieneObservaciones = await db.Set<Observacion>().AnyAsync(o => o.MarcadorId == idMarcador, ct);
        if (tieneObservaciones)
        {
            throw new MarcadorConObservacionesException();
        }

        var vinculos = await db.Set<EtiquetaMarcador>().Where(em => em.MarcadorId == idMarcador).ToListAsync(ct);
        db.Set<EtiquetaMarcador>().RemoveRange(vinculos);
        db.Set<MarcadorGeografico>().Remove(marcador);
        await db.SaveChangesAsync(ct);
    }

    public async Task<ObservacionDto> CrearObservacionAsync(Guid idUsuario, Guid idRelevamiento, Guid idMarcador, CrearObservacionRequest req, CancellationToken ct = default)
    {
        var rel = await GarantizarAccesoAsync(idUsuario, idRelevamiento, ct);
        if (rel.EstaCerrado)
        {
            throw new RelevamientoCerradoException();
        }

        // RC-02: la observación se ancla a un marcador existente del relevamiento.
        await CargarMarcadorAsync(idRelevamiento, idMarcador, ct);

        var obs = new Observacion(idMarcador, idUsuario, req.Nota);
        db.Set<Observacion>().Add(obs);
        await db.SaveChangesAsync(ct);
        return new ObservacionDto(obs.Id, obs.MarcadorId, obs.AutorId, obs.Nota, obs.FechaCreacion);
    }

    public async Task<IReadOnlyList<ObservacionDto>> ListarObservacionesAsync(Guid idUsuario, Guid idRelevamiento, Guid idMarcador, CancellationToken ct = default)
    {
        await GarantizarAccesoAsync(idUsuario, idRelevamiento, ct);
        await CargarMarcadorAsync(idRelevamiento, idMarcador, ct);

        return await db.Set<Observacion>()
            .Where(o => o.MarcadorId == idMarcador)
            .OrderBy(o => o.FechaCreacion)
            .Select(o => new ObservacionDto(o.Id, o.MarcadorId, o.AutorId, o.Nota, o.FechaCreacion))
            .ToListAsync(ct);
    }

    public async Task<EtiquetaDto> CrearEtiquetaAsync(Guid idJefe, Guid idRelevamiento, CrearEtiquetaRequest req, CancellationToken ct = default)
    {
        var rel = await CargarDelJefeAsync(idJefe, idRelevamiento, ct);
        if (rel.EstaCerrado)
        {
            throw new RelevamientoCerradoException();
        }

        if (string.IsNullOrWhiteSpace(req.Nombre))
        {
            throw new EtiquetaInvalidaException("El nombre de la etiqueta es obligatorio.");
        }

        var etiqueta = new Etiqueta(idRelevamiento, req.Nombre);

        var existe = await db.Set<Etiqueta>()
            .AnyAsync(e => e.RelevamientoId == idRelevamiento && e.Nombre == etiqueta.Nombre, ct);
        if (existe)
        {
            throw new EtiquetaYaExisteException(etiqueta.Nombre);
        }

        db.Set<Etiqueta>().Add(etiqueta);
        await db.SaveChangesAsync(ct);
        return new EtiquetaDto(etiqueta.Id, etiqueta.Nombre);
    }

    public async Task<IReadOnlyList<EtiquetaDto>> ListarEtiquetasAsync(Guid idUsuario, Guid idRelevamiento, CancellationToken ct = default)
    {
        await GarantizarAccesoAsync(idUsuario, idRelevamiento, ct);
        return await db.Set<Etiqueta>()
            .Where(e => e.RelevamientoId == idRelevamiento)
            .OrderBy(e => e.Nombre)
            .Select(e => new EtiquetaDto(e.Id, e.Nombre))
            .ToListAsync(ct);
    }

    public async Task EtiquetarMarcadorAsync(Guid idJefe, Guid idRelevamiento, Guid idMarcador, Guid idEtiqueta, CancellationToken ct = default)
    {
        var rel = await CargarDelJefeAsync(idJefe, idRelevamiento, ct);
        if (rel.EstaCerrado)
        {
            throw new RelevamientoCerradoException();
        }

        await CargarMarcadorAsync(idRelevamiento, idMarcador, ct);
        var etiquetaValida = await db.Set<Etiqueta>()
            .AnyAsync(e => e.Id == idEtiqueta && e.RelevamientoId == idRelevamiento, ct);
        if (!etiquetaValida)
        {
            throw new EtiquetaNoEncontradaException(idEtiqueta);
        }

        var yaEtiquetado = await db.Set<EtiquetaMarcador>()
            .AnyAsync(em => em.EtiquetaId == idEtiqueta && em.MarcadorId == idMarcador, ct);
        if (yaEtiquetado)
        {
            return;
        }

        db.Set<EtiquetaMarcador>().Add(new EtiquetaMarcador(idEtiqueta, idMarcador));
        await db.SaveChangesAsync(ct);
    }

    public async Task QuitarEtiquetaMarcadorAsync(Guid idJefe, Guid idRelevamiento, Guid idMarcador, Guid idEtiqueta, CancellationToken ct = default)
    {
        var rel = await CargarDelJefeAsync(idJefe, idRelevamiento, ct);
        if (rel.EstaCerrado)
        {
            throw new RelevamientoCerradoException();
        }

        await CargarMarcadorAsync(idRelevamiento, idMarcador, ct);
        var vinculo = await db.Set<EtiquetaMarcador>()
            .FirstOrDefaultAsync(em => em.EtiquetaId == idEtiqueta && em.MarcadorId == idMarcador, ct);
        if (vinculo is null)
        {
            return;
        }

        db.Set<EtiquetaMarcador>().Remove(vinculo);
        await db.SaveChangesAsync(ct);
    }

    private async Task<List<string>> EtiquetasDeMarcadorAsync(Guid idMarcador, CancellationToken ct)
        => await db.Set<EtiquetaMarcador>()
            .Where(em => em.MarcadorId == idMarcador)
            .Join(db.Set<Etiqueta>(), em => em.EtiquetaId, e => e.Id, (em, e) => e.Nombre)
            .OrderBy(n => n)
            .ToListAsync(ct);

    private async Task<MarcadorGeografico> CargarMarcadorAsync(Guid idRelevamiento, Guid idMarcador, CancellationToken ct)
        => await db.Set<MarcadorGeografico>()
               .FirstOrDefaultAsync(m => m.Id == idMarcador && m.RelevamientoId == idRelevamiento, ct)
           ?? throw new MarcadorNoEncontradoException(idMarcador);

    /// <summary>Garantiza que el usuario sea el jefe dueño o un agente asignado, y devuelve el relevamiento.</summary>
    private async Task<Relevamiento> GarantizarAccesoAsync(Guid idUsuario, Guid idRelevamiento, CancellationToken ct)
    {
        var rel = await db.Relevamientos.FirstOrDefaultAsync(r => r.Id == idRelevamiento, ct)
                  ?? throw new RelevamientoNoEncontradoException(idRelevamiento);

        if (rel.IdJefeArea == idUsuario)
        {
            return rel;
        }

        var esAgenteAsignado = await db.Set<AsignacionAgente>()
            .AnyAsync(a => a.RelevamientoId == idRelevamiento && a.IdAgente == idUsuario, ct);
        if (!esAgenteAsignado)
        {
            throw new OperacionNoAutorizadaException("No tenés acceso a este relevamiento.");
        }

        return rel;
    }

    private async Task<Usuario> UsuarioActivoAsync(Guid id, CancellationToken ct)
        => await db.Usuarios.FirstOrDefaultAsync(u => u.Id == id && u.Activo, ct)
           ?? throw new OperacionNoAutorizadaException("El usuario no existe o está inactivo.");

    private async Task<Relevamiento> CargarDelJefeAsync(Guid idJefe, Guid idRelevamiento, CancellationToken ct)
    {
        var jefe = await UsuarioActivoAsync(idJefe, ct);
        if (jefe.Rol != Rol.JefeDeArea)
        {
            throw new OperacionNoAutorizadaException("Solo un jefe de área puede administrar relevamientos.");
        }

        var rel = await db.Relevamientos.FirstOrDefaultAsync(r => r.Id == idRelevamiento, ct)
                  ?? throw new RelevamientoNoEncontradoException(idRelevamiento);

        if (rel.IdJefeArea != idJefe)
        {
            throw new OperacionNoAutorizadaException("El relevamiento pertenece a otro jefe de área.");
        }

        return rel;
    }
}
