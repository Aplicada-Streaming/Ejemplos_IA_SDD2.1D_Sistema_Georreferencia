using GeoVial.Storage.Abstractions;
using GeoVial.WebApi.Domain;
using GeoVial.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GeoVial.WebApi.Application;

/// <summary>
/// Captura de fotos de observaciones (F2). El binario se delega a la librería de
/// almacenamiento (<see cref="IObjectStore"/>, ADR-09) y la base solo guarda la referencia
/// lógica. La ubicación se resuelve con prioridad a la coordenada incrustada (RN-04).
/// </summary>
public sealed class ServicioFotos(GeoVialDbContext db, IObjectStore almacen) : IServicioFotos
{
    public async Task<FotoDto> AgregarFotoAsync(
        Guid idUsuario, Guid idRelevamiento, Guid idObservacion,
        Stream contenido, string contentType, UbicacionFoto ubicacion, string? comentario,
        CancellationToken ct = default)
    {
        var rel = await GarantizarAccesoAsync(idUsuario, idRelevamiento, ct);
        if (rel.EstaCerrado)
        {
            throw new RelevamientoCerradoException();
        }

        if (string.IsNullOrWhiteSpace(contentType) || !contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            throw new TipoArchivoInvalidoException("Solo se admiten imágenes.");
        }

        // La observación debe existir y pertenecer a un marcador de este relevamiento.
        await CargarObservacionAsync(idRelevamiento, idObservacion, ct);

        var (lat, lng) = ResolverUbicacion(ubicacion);
        var clave = $"relevamientos/{idRelevamiento}/observaciones/{idObservacion}/fotos/{Guid.NewGuid():N}";

        await almacen.SaveAsync(clave, contenido, contentType, ct);

        var foto = new Foto(idObservacion, clave, contentType, lat, lng);
        db.Set<Foto>().Add(foto);

        string? textoComentario = null;
        if (!string.IsNullOrWhiteSpace(comentario))
        {
            textoComentario = comentario.Trim();
            db.Set<Comentario>().Add(new Comentario(foto.Id, textoComentario));
        }

        await db.SaveChangesAsync(ct);
        return new FotoDto(foto.Id, foto.ObservacionId, foto.Latitud, foto.Longitud, foto.PendienteUbicacion, textoComentario, foto.ContentType, foto.FechaCreacion);
    }

    public async Task<IReadOnlyList<FotoDto>> ListarPorObservacionAsync(Guid idUsuario, Guid idRelevamiento, Guid idObservacion, CancellationToken ct = default)
    {
        await GarantizarAccesoAsync(idUsuario, idRelevamiento, ct);
        await CargarObservacionAsync(idRelevamiento, idObservacion, ct);

        var fotos = await db.Set<Foto>()
            .Where(f => f.ObservacionId == idObservacion)
            .OrderBy(f => f.FechaCreacion)
            .ToListAsync(ct);

        return await ProyectarAsync(fotos, ct);
    }

    public async Task<IReadOnlyList<FotoDto>> ListarPorMarcadorAsync(Guid idUsuario, Guid idRelevamiento, Guid idMarcador, CancellationToken ct = default)
    {
        await GarantizarAccesoAsync(idUsuario, idRelevamiento, ct);
        await CargarMarcadorAsync(idRelevamiento, idMarcador, ct);

        var observacionIds = await db.Set<Observacion>()
            .Where(o => o.MarcadorId == idMarcador)
            .Select(o => o.Id)
            .ToListAsync(ct);

        var fotos = await db.Set<Foto>()
            .Where(f => observacionIds.Contains(f.ObservacionId))
            .OrderBy(f => f.FechaCreacion)
            .ToListAsync(ct);

        return await ProyectarAsync(fotos, ct);
    }

    public async Task<(Stream Contenido, string ContentType)> ObtenerContenidoAsync(Guid idUsuario, Guid idRelevamiento, Guid idFoto, CancellationToken ct = default)
    {
        await GarantizarAccesoAsync(idUsuario, idRelevamiento, ct);

        var foto = await db.Set<Foto>().FirstOrDefaultAsync(f => f.Id == idFoto, ct)
                   ?? throw new FotoNoEncontradaException(idFoto);

        // La foto debe pertenecer a una observación de un marcador de este relevamiento.
        var observacion = await db.Set<Observacion>().FirstOrDefaultAsync(o => o.Id == foto.ObservacionId, ct)
                          ?? throw new FotoNoEncontradaException(idFoto);
        await CargarMarcadorAsync(idRelevamiento, observacion.MarcadorId, ct);

        var contenido = await almacen.GetAsync(foto.ReferenciaAlmacen, ct);
        return (contenido, foto.ContentType);
    }

    private async Task<IReadOnlyList<FotoDto>> ProyectarAsync(List<Foto> fotos, CancellationToken ct)
    {
        var ids = fotos.Select(f => f.Id).ToList();
        var comentarios = (await db.Set<Comentario>()
                .Where(co => ids.Contains(co.FotoId))
                .ToListAsync(ct))
            .ToDictionary(co => co.FotoId, co => co.Texto);

        return fotos
            .Select(f => new FotoDto(
                f.Id, f.ObservacionId, f.Latitud, f.Longitud, f.PendienteUbicacion,
                comentarios.GetValueOrDefault(f.Id), f.ContentType, f.FechaCreacion))
            .ToList();
    }

    // RN-04: la coordenada incrustada en la imagen tiene prioridad sobre la asignada manualmente.
    private static (double? Lat, double? Lng) ResolverUbicacion(UbicacionFoto u)
    {
        if (u.LatitudIncrustada is { } li && u.LongitudIncrustada is { } loi)
        {
            return (li, loi);
        }

        if (u.LatitudManual is { } lm && u.LongitudManual is { } lom)
        {
            return (lm, lom);
        }

        return (null, null);
    }

    private async Task<Observacion> CargarObservacionAsync(Guid idRelevamiento, Guid idObservacion, CancellationToken ct)
    {
        var obs = await db.Set<Observacion>().FirstOrDefaultAsync(o => o.Id == idObservacion, ct)
                  ?? throw new ObservacionNoEncontradaException(idObservacion);

        // Garantiza que el marcador anclado pertenezca al relevamiento.
        await CargarMarcadorAsync(idRelevamiento, obs.MarcadorId, ct);
        return obs;
    }

    private async Task CargarMarcadorAsync(Guid idRelevamiento, Guid idMarcador, CancellationToken ct)
    {
        var existe = await db.Set<MarcadorGeografico>()
            .AnyAsync(m => m.Id == idMarcador && m.RelevamientoId == idRelevamiento, ct);
        if (!existe)
        {
            throw new MarcadorNoEncontradoException(idMarcador);
        }
    }

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
}
