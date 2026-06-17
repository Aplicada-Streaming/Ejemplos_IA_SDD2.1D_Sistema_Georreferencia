using GeoVial.WebApi.Domain;
using GeoVial.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GeoVial.WebApi.Application;

/// <summary>
/// Gestión de conflictos de marcadores y su resolución al cierre (CU-13). El jefe de área
/// lista los conflictos pendientes y los resuelve unificando los marcadores en uno solo —con
/// reasignación de observaciones y unión de etiquetas— o manteniéndolos separados. La
/// resolución solo procede con el relevamiento en revisión (RN-05).
/// </summary>
public sealed class ServicioConflictos(GeoVialDbContext db) : IServicioConflictos
{
    public async Task<IReadOnlyList<ConflictoDto>> ListarPendientesAsync(Guid idJefe, Guid idRelevamiento, CancellationToken ct = default)
    {
        await CargarDelJefeAsync(idJefe, idRelevamiento, ct);
        var conflictos = await db.Set<ConflictoMarcadores>()
            .Where(c => c.RelevamientoId == idRelevamiento && c.Estado == EstadoConflicto.Pendiente)
            .OrderBy(c => c.DetectadoEn)
            .ToListAsync(ct);

        return await ProyectarAsync(conflictos, ct);
    }

    public async Task<ConflictoDto> ResolverAsync(Guid idJefe, Guid idRelevamiento, Guid idConflicto, ResolucionConflicto resolucion, CancellationToken ct = default)
    {
        var rel = await CargarDelJefeAsync(idJefe, idRelevamiento, ct);
        if (!rel.EstaEnRevision)
        {
            throw new RelevamientoNoEnRevisionException();
        }

        var conflicto = await db.Set<ConflictoMarcadores>()
            .FirstOrDefaultAsync(c => c.Id == idConflicto && c.RelevamientoId == idRelevamiento && c.Estado == EstadoConflicto.Pendiente, ct)
            ?? throw new ConflictoNoEncontradoException(idConflicto);

        if (resolucion == ResolucionConflicto.Unificar)
        {
            await UnificarAsync(idRelevamiento, conflicto, ct);
        }

        conflicto.Resolver(resolucion);
        await db.SaveChangesAsync(ct);

        return (await ProyectarAsync([conflicto], ct))[0];
    }

    private async Task UnificarAsync(Guid idRelevamiento, ConflictoMarcadores conflicto, CancellationToken ct)
    {
        var miembros = await db.Set<ConflictoMarcadorMiembro>()
            .Where(cm => cm.ConflictoId == conflicto.Id)
            .Select(cm => cm.MarcadorId)
            .ToListAsync(ct);

        var marcadores = await db.Set<MarcadorGeografico>()
            .Where(m => miembros.Contains(m.Id))
            .ToListAsync(ct);
        if (marcadores.Count < 2)
        {
            return;
        }

        // Marcador resultante: el más antiguo (identidad estable, RC-01).
        var resultante = marcadores.OrderBy(m => m.FechaCreacion).ThenBy(m => m.Id).First();
        var absorbidos = marcadores.Where(m => m.Id != resultante.Id).Select(m => m.Id).ToList();

        // Reasignar las observaciones de los marcadores absorbidos al resultante (con sus fotos).
        var observaciones = await db.Set<Observacion>().Where(o => absorbidos.Contains(o.MarcadorId)).ToListAsync(ct);
        foreach (var obs in observaciones)
        {
            obs.Reanclar(resultante.Id);
        }

        // Unión de etiquetas: migrar las de los absorbidos al resultante sin duplicar.
        var etiquetasResultante = (await db.Set<EtiquetaMarcador>()
                .Where(em => em.MarcadorId == resultante.Id)
                .Select(em => em.EtiquetaId)
                .ToListAsync(ct))
            .ToHashSet();
        var vinculosAbsorbidos = await db.Set<EtiquetaMarcador>().Where(em => absorbidos.Contains(em.MarcadorId)).ToListAsync(ct);
        foreach (var vinculo in vinculosAbsorbidos)
        {
            if (etiquetasResultante.Add(vinculo.EtiquetaId))
            {
                db.Set<EtiquetaMarcador>().Add(new EtiquetaMarcador(vinculo.EtiquetaId, resultante.Id));
            }
        }

        db.Set<EtiquetaMarcador>().RemoveRange(vinculosAbsorbidos);

        // Quitar a los absorbidos de todo conflicto (FK restrict) antes de eliminarlos.
        var membresias = await db.Set<ConflictoMarcadorMiembro>().Where(cm => absorbidos.Contains(cm.MarcadorId)).ToListAsync(ct);
        db.Set<ConflictoMarcadorMiembro>().RemoveRange(membresias);

        var entidadesAbsorbidas = marcadores.Where(m => absorbidos.Contains(m.Id)).ToList();
        db.Set<MarcadorGeografico>().RemoveRange(entidadesAbsorbidas);

        await db.SaveChangesAsync(ct);

        // Otros conflictos que quedaron con menos de dos miembros dejan de serlo.
        await ResolverConflictosDegeneradosAsync(idRelevamiento, conflicto.Id, ct);
    }

    private async Task ResolverConflictosDegeneradosAsync(Guid idRelevamiento, Guid conflictoActual, CancellationToken ct)
    {
        var pendientes = await db.Set<ConflictoMarcadores>()
            .Where(c => c.RelevamientoId == idRelevamiento && c.Estado == EstadoConflicto.Pendiente && c.Id != conflictoActual)
            .ToListAsync(ct);

        var cambios = false;
        foreach (var c in pendientes)
        {
            var miembros = await db.Set<ConflictoMarcadorMiembro>().CountAsync(cm => cm.ConflictoId == c.Id, ct);
            if (miembros < 2)
            {
                c.Resolver(ResolucionConflicto.Separar);
                cambios = true;
            }
        }

        if (cambios)
        {
            await db.SaveChangesAsync(ct);
        }
    }

    private async Task<IReadOnlyList<ConflictoDto>> ProyectarAsync(IReadOnlyList<ConflictoMarcadores> conflictos, CancellationToken ct)
    {
        var ids = conflictos.Select(c => c.Id).ToList();
        var miembros = await db.Set<ConflictoMarcadorMiembro>()
            .Where(cm => ids.Contains(cm.ConflictoId))
            .ToListAsync(ct);

        var marcadorIds = miembros.Select(m => m.MarcadorId).Distinct().ToList();
        var marcadores = (await db.Set<MarcadorGeografico>()
                .Where(m => marcadorIds.Contains(m.Id))
                .ToListAsync(ct))
            .ToDictionary(m => m.Id);

        return conflictos
            .Select(c => new ConflictoDto(
                c.Id, c.Estado, c.Resolucion, c.DetectadoEn,
                miembros.Where(m => m.ConflictoId == c.Id)
                    .Where(m => marcadores.ContainsKey(m.MarcadorId))
                    .Select(m => marcadores[m.MarcadorId])
                    .Select(m => new ConflictoMarcadorDto(m.Id, m.Latitud, m.Longitud, m.Descripcion))
                    .ToList()))
            .ToList();
    }

    private async Task<Relevamiento> CargarDelJefeAsync(Guid idJefe, Guid idRelevamiento, CancellationToken ct)
    {
        var rel = await db.Relevamientos.FirstOrDefaultAsync(r => r.Id == idRelevamiento, ct)
                  ?? throw new RelevamientoNoEncontradoException(idRelevamiento);

        if (rel.IdJefeArea != idJefe)
        {
            throw new OperacionNoAutorizadaException("Solo el jefe de área dueño administra los conflictos del relevamiento.");
        }

        return rel;
    }
}
