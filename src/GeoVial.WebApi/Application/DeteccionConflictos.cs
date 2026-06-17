using GeoVial.WebApi.Domain;
using GeoVial.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GeoVial.WebApi.Application;

/// <summary>
/// Detección y registro de conflictos de marcadores por cercanía (RN-03/RN-04). Se invoca tras
/// persistir un marcador (creación en línea o sincronización). Un marcador dentro del radio de
/// otros queda agrupado en un conflicto pendiente que convive con la operación y se resuelve al
/// cierre (CU-13). La unión por cluster evita duplicar conflictos para el mismo grupo.
/// </summary>
public static class DeteccionConflictos
{
    /// <summary>Radio de agrupación/conflicto de marcadores, en metros.</summary>
    public const double RadioMetros = 30.0;

    /// <summary>
    /// Registra/extiende el conflicto que corresponda al marcador recién persistido.
    /// Devuelve true si se creó un conflicto nuevo (para conteos de la sincronización).
    /// </summary>
    public static async Task<bool> RegistrarAsync(GeoVialDbContext db, Guid idRelevamiento, Guid idMarcador, double lat, double lng, CancellationToken ct)
    {
        var otros = await db.Set<MarcadorGeografico>()
            .Where(m => m.RelevamientoId == idRelevamiento && m.Id != idMarcador)
            .Select(m => new { m.Id, m.Latitud, m.Longitud })
            .ToListAsync(ct);

        var vecinos = otros
            .Where(m => DistanciaMetros(lat, lng, m.Latitud, m.Longitud) <= RadioMetros)
            .Select(m => m.Id)
            .ToList();

        if (vecinos.Count == 0)
        {
            return false;
        }

        var candidatos = new List<Guid>(vecinos) { idMarcador };

        // ¿Algún candidato ya está en un conflicto pendiente? Entonces se extiende ese cluster.
        var conflictoExistente = await db.Set<ConflictoMarcadorMiembro>()
            .Where(cm => candidatos.Contains(cm.MarcadorId))
            .Join(db.Set<ConflictoMarcadores>().Where(c => c.Estado == EstadoConflicto.Pendiente),
                  cm => cm.ConflictoId, c => c.Id, (cm, c) => c.Id)
            .FirstOrDefaultAsync(ct);

        if (conflictoExistente != default)
        {
            var miembros = (await db.Set<ConflictoMarcadorMiembro>()
                    .Where(cm => cm.ConflictoId == conflictoExistente)
                    .Select(cm => cm.MarcadorId)
                    .ToListAsync(ct))
                .ToHashSet();

            foreach (var id in candidatos.Where(id => !miembros.Contains(id)))
            {
                db.Set<ConflictoMarcadorMiembro>().Add(new ConflictoMarcadorMiembro(conflictoExistente, id));
            }

            await db.SaveChangesAsync(ct);
            return false;
        }

        var conflicto = new ConflictoMarcadores(idRelevamiento);
        db.Set<ConflictoMarcadores>().Add(conflicto);
        foreach (var id in candidatos.Distinct())
        {
            db.Set<ConflictoMarcadorMiembro>().Add(new ConflictoMarcadorMiembro(conflicto.Id, id));
        }

        await db.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>Distancia aproximada entre dos coordenadas en metros (fórmula de Haversine).</summary>
    public static double DistanciaMetros(double lat1, double lon1, double lat2, double lon2)
    {
        const double radioTierra = 6_371_000.0;
        var dLat = GradosARadianes(lat2 - lat1);
        var dLon = GradosARadianes(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + Math.Cos(GradosARadianes(lat1)) * Math.Cos(GradosARadianes(lat2))
                  * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return radioTierra * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double GradosARadianes(double grados) => grados * Math.PI / 180.0;
}
