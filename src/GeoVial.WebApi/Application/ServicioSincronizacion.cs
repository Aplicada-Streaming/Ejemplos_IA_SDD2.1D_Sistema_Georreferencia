using System.Globalization;
using GeoVial.WebApi.Domain;
using GeoVial.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GeoVial.WebApi.Application;

/// <summary>
/// Sincronización de campo (F2). Implementa el ciclo subir-antes-de-bajar (RN-06) con
/// idempotencia por identificador de origen (RN-07) y una marca opaca monótona por cliente
/// (RC-06). Los marcadores dentro de un radio conviven como conflicto sin bloquear (RN-03).
/// </summary>
public sealed class ServicioSincronizacion(GeoVialDbContext db) : IServicioSincronizacion
{
    public async Task<ResultadoSubida> SubirAsync(Guid idAgente, Guid idRelevamiento, LoteSincronizacion lote, CancellationToken ct = default)
    {
        var rel = await CargarAsignadoAsync(idAgente, idRelevamiento, ct);
        if (rel.EstaCerrado)
        {
            throw new RelevamientoCerradoException();
        }

        ValidarLote(lote);

        // Estado conocido del relevamiento (sin tracking de colecciones).
        var existentes = await db.Set<MarcadorGeografico>()
            .Where(m => m.RelevamientoId == idRelevamiento)
            .Select(m => new { m.Id, m.IdOrigen, m.Latitud, m.Longitud })
            .ToListAsync(ct);

        var marcadorPorOrigen = existentes
            .Where(m => m.IdOrigen is not null)
            .ToDictionary(m => m.IdOrigen!, m => m.Id);

        var aplicados = 0;
        var reenviados = 0;
        var nuevos = new List<(Guid Id, double Lat, double Lng)>();

        foreach (var cambio in lote.Marcadores)
        {
            if (marcadorPorOrigen.ContainsKey(cambio.IdOrigen))
            {
                reenviados++;
                continue;
            }

            var marcador = new MarcadorGeografico(idRelevamiento, cambio.Latitud, cambio.Longitud, cambio.Descripcion, cambio.IdOrigen);
            db.Set<MarcadorGeografico>().Add(marcador);
            marcadorPorOrigen[cambio.IdOrigen] = marcador.Id;
            nuevos.Add((marcador.Id, cambio.Latitud, cambio.Longitud));
            aplicados++;
        }

        // Observaciones ya aplicadas (por id de origen) en los marcadores del relevamiento.
        var marcadorIds = marcadorPorOrigen.Values.ToList();
        var obsVistas = (await db.Set<Observacion>()
                .Where(o => marcadorIds.Contains(o.MarcadorId) && o.IdOrigen != null)
                .Select(o => o.IdOrigen!)
                .ToListAsync(ct))
            .ToHashSet();

        foreach (var cambio in lote.Observaciones)
        {
            if (!marcadorPorOrigen.TryGetValue(cambio.MarcadorIdOrigen, out var idMarcador))
            {
                throw new LoteMalformadoException($"La observación '{cambio.IdOrigen}' referencia un marcador desconocido '{cambio.MarcadorIdOrigen}'.");
            }

            if (!obsVistas.Add(cambio.IdOrigen))
            {
                reenviados++;
                continue;
            }

            db.Set<Observacion>().Add(new Observacion(idMarcador, idAgente, cambio.Nota, cambio.IdOrigen));
            aplicados++;
        }

        var marca = await ObtenerOCrearMarcaAsync(idRelevamiento, idAgente, ct);
        marca.ConcluirSubida();

        await db.SaveChangesAsync(ct);

        // RN-03: los marcadores nuevos dentro de un radio quedan registrados como conflicto,
        // sin bloquear la subida; se cuentan los conflictos detectados en este lote.
        var conflictos = 0;
        foreach (var nuevo in nuevos)
        {
            if (await DeteccionConflictos.RegistrarAsync(db, idRelevamiento, nuevo.Id, nuevo.Lat, nuevo.Lng, ct))
            {
                conflictos++;
            }
        }

        return new ResultadoSubida(aplicados, reenviados, conflictos);
    }

    public async Task<ResultadoBajada> BajarAsync(Guid idAgente, Guid idRelevamiento, SolicitudBajada solicitud, CancellationToken ct = default)
    {
        var rel = await CargarAsignadoAsync(idAgente, idRelevamiento, ct);

        var marca = await db.Set<MarcaSincronizacion>()
            .FirstOrDefaultAsync(s => s.RelevamientoId == idRelevamiento && s.ClienteId == idAgente, ct);
        if (marca is null || !marca.SubidaConcluida)
        {
            throw new SubidaNoConcluidaException();
        }

        var desde = InterpretarMarca(solicitud.Marca);

        var marcadores = await db.Set<MarcadorGeografico>()
            .Where(m => m.RelevamientoId == idRelevamiento)
            .Select(m => new { m.Id, m.Latitud, m.Longitud, m.Descripcion, m.ActualizadoEn })
            .ToListAsync(ct);

        var novedadesMarcadores = marcadores
            .Where(m => m.ActualizadoEn > desde)
            .OrderBy(m => m.ActualizadoEn)
            .Select(m => new MarcadorBajadaDto(
                m.Id, m.Latitud, m.Longitud, m.Descripcion,
                EnConflicto(m.Id, m.Latitud, m.Longitud, marcadores.Select(o => (o.Id, o.Latitud, o.Longitud))),
                m.ActualizadoEn))
            .ToList();

        var marcadorIds = marcadores.Select(m => m.Id).ToList();
        var novedadesObservaciones = await db.Set<Observacion>()
            .Where(o => marcadorIds.Contains(o.MarcadorId) && o.FechaCreacion > desde)
            .OrderBy(o => o.FechaCreacion)
            .Select(o => new ObservacionBajadaDto(o.Id, o.MarcadorId, o.Nota, o.FechaCreacion))
            .ToListAsync(ct);

        var marcaNueva = DateTimeOffset.UtcNow;
        marca.AvanzarMarca(marcaNueva);
        await db.SaveChangesAsync(ct);

        return new ResultadoBajada(rel.Estado, novedadesMarcadores, novedadesObservaciones, Serializar(marcaNueva));
    }

    private static void ValidarLote(LoteSincronizacion lote)
    {
        foreach (var m in lote.Marcadores)
        {
            if (string.IsNullOrWhiteSpace(m.IdOrigen))
            {
                throw new LoteMalformadoException("Un marcador del lote no porta identificador de origen.");
            }

            if (m.Latitud is < -90 or > 90 || m.Longitud is < -180 or > 180)
            {
                throw new LoteMalformadoException($"El marcador '{m.IdOrigen}' tiene coordenadas fuera de rango.");
            }
        }

        foreach (var o in lote.Observaciones)
        {
            if (string.IsNullOrWhiteSpace(o.IdOrigen) || string.IsNullOrWhiteSpace(o.MarcadorIdOrigen))
            {
                throw new LoteMalformadoException("Una observación del lote no porta identificador de origen o de marcador.");
            }
        }
    }

    private async Task<Relevamiento> CargarAsignadoAsync(Guid idAgente, Guid idRelevamiento, CancellationToken ct)
    {
        var rel = await db.Relevamientos.FirstOrDefaultAsync(r => r.Id == idRelevamiento, ct)
                  ?? throw new RelevamientoNoEncontradoException(idRelevamiento);

        var asignado = await db.Set<AsignacionAgente>()
            .AnyAsync(a => a.RelevamientoId == idRelevamiento && a.IdAgente == idAgente, ct);
        if (!asignado)
        {
            throw new RelevamientoNoAsignadoException();
        }

        return rel;
    }

    private async Task<MarcaSincronizacion> ObtenerOCrearMarcaAsync(Guid idRelevamiento, Guid idAgente, CancellationToken ct)
    {
        var marca = await db.Set<MarcaSincronizacion>()
            .FirstOrDefaultAsync(s => s.RelevamientoId == idRelevamiento && s.ClienteId == idAgente, ct);
        if (marca is null)
        {
            marca = new MarcaSincronizacion(idRelevamiento, idAgente);
            db.Set<MarcaSincronizacion>().Add(marca);
        }

        return marca;
    }

    private static bool EnConflicto(Guid id, double lat, double lng, IEnumerable<(Guid Id, double Lat, double Lng)> todos)
        => todos.Any(o => o.Id != id && DeteccionConflictos.DistanciaMetros(lat, lng, o.Lat, o.Lng) <= DeteccionConflictos.RadioMetros);

    // Marca opaca: instante en formato ISO 8601 de ida y vuelta. Nula/vacía = sincronización completa.
    private static DateTimeOffset InterpretarMarca(string? marca)
    {
        if (string.IsNullOrWhiteSpace(marca))
        {
            return DateTimeOffset.MinValue;
        }

        if (!DateTimeOffset.TryParse(marca, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var valor))
        {
            throw new MarcaInvalidaException();
        }

        return valor;
    }

    private static string Serializar(DateTimeOffset marca) => marca.ToString("O", CultureInfo.InvariantCulture);
}
