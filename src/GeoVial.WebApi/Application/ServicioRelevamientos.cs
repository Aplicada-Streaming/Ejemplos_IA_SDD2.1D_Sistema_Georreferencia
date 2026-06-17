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
        var rel = await db.Relevamientos.AsNoTracking().FirstOrDefaultAsync(r => r.Id == idRelevamiento, ct)
                  ?? throw new RelevamientoNoEncontradoException(idRelevamiento);

        var esJefe = rel.IdJefeArea == idUsuario;
        var esAgenteAsignado = await db.Set<AsignacionAgente>()
            .AnyAsync(a => a.RelevamientoId == idRelevamiento && a.IdAgente == idUsuario, ct);
        if (!esJefe && !esAgenteAsignado)
        {
            throw new OperacionNoAutorizadaException("No tenés acceso a este relevamiento.");
        }

        return await db.Set<MarcadorGeografico>()
            .Where(m => m.RelevamientoId == idRelevamiento)
            .OrderBy(m => m.FechaCreacion)
            .Select(m => new MarcadorDto(m.Id, m.Latitud, m.Longitud, m.Descripcion, m.FechaCreacion))
            .ToListAsync(ct);
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
        return new MarcadorDto(marcador.Id, marcador.Latitud, marcador.Longitud, marcador.Descripcion, marcador.FechaCreacion);
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
