using System.IO.Compression;
using System.Text.Json;
using GeoVial.Storage.Abstractions;
using GeoVial.WebApi.Domain;
using GeoVial.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GeoVial.WebApi.Application;

/// <summary>
/// Portabilidad de relevamientos (CU-15/CU-16, NB-06). Empaqueta un relevamiento completo
/// —marcadores, observaciones, fotos, comentarios y etiquetas— en una unidad transferible única
/// (ZIP con manifiesto + binarios) y lo reconstruye preservando la correspondencia entre piezas.
/// Los binarios de las fotos se recuperan y se alojan de forma transparente al proveedor (ADR-09).
/// </summary>
public sealed class ServicioPortabilidad(GeoVialDbContext db, IObjectStore almacen) : IServicioPortabilidad
{
    private const int VersionManifiesto = 1;
    private const string NombreManifiesto = "manifiesto.json";
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public async Task<byte[]> ExportarAsync(Guid idJefe, Guid idRelevamiento, CancellationToken ct = default)
    {
        var rel = await db.Relevamientos.FirstOrDefaultAsync(r => r.Id == idRelevamiento, ct)
                  ?? throw new RelevamientoNoEncontradoException(idRelevamiento);
        if (rel.IdJefeArea != idJefe)
        {
            throw new RelevamientoFueraDeAmbitoException();
        }

        var etiquetasRel = await db.Set<Etiqueta>()
            .Where(e => e.RelevamientoId == idRelevamiento)
            .OrderBy(e => e.Nombre)
            .Select(e => e.Nombre)
            .ToListAsync(ct);

        var marcadores = await db.Set<MarcadorGeografico>()
            .Where(m => m.RelevamientoId == idRelevamiento)
            .OrderBy(m => m.FechaCreacion)
            .ToListAsync(ct);

        var marcadoresExport = new List<MarcadorExport>();
        var binarios = new List<(string Archivo, string Clave)>();

        foreach (var marcador in marcadores)
        {
            var etiquetasMarcador = await db.Set<EtiquetaMarcador>()
                .Where(em => em.MarcadorId == marcador.Id)
                .Join(db.Set<Etiqueta>(), em => em.EtiquetaId, e => e.Id, (em, e) => e.Nombre)
                .OrderBy(n => n)
                .ToListAsync(ct);

            var observaciones = await db.Set<Observacion>()
                .Where(o => o.MarcadorId == marcador.Id)
                .OrderBy(o => o.FechaCreacion)
                .ToListAsync(ct);

            var observacionesExport = new List<ObservacionExport>();
            foreach (var obs in observaciones)
            {
                var fotos = await db.Set<Foto>()
                    .Where(f => f.ObservacionId == obs.Id)
                    .OrderBy(f => f.FechaCreacion)
                    .ToListAsync(ct);

                var fotosExport = new List<FotoExport>();
                foreach (var foto in fotos)
                {
                    var comentario = await db.Set<Comentario>()
                        .Where(c => c.FotoId == foto.Id)
                        .Select(c => c.Texto)
                        .FirstOrDefaultAsync(ct);

                    var archivo = $"fotos/{foto.Id:N}";
                    binarios.Add((archivo, foto.ReferenciaAlmacen));
                    fotosExport.Add(new FotoExport(archivo, foto.Latitud, foto.Longitud, foto.PendienteUbicacion, foto.ContentType, comentario));
                }

                observacionesExport.Add(new ObservacionExport(obs.Id.ToString("N"), obs.Nota, fotosExport));
            }

            marcadoresExport.Add(new MarcadorExport(
                marcador.Id.ToString("N"), marcador.Latitud, marcador.Longitud, marcador.Descripcion,
                etiquetasMarcador, observacionesExport));
        }

        var manifiesto = new ManifiestoExport(
            VersionManifiesto,
            new RelevamientoExport(rel.Nombre, rel.TramoVial, rel.Estado, rel.FechaCreacion),
            etiquetasRel,
            marcadoresExport);

        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            var entradaManifiesto = zip.CreateEntry(NombreManifiesto);
            await using (var s = entradaManifiesto.Open())
            {
                await JsonSerializer.SerializeAsync(s, manifiesto, Json, ct);
            }

            foreach (var (archivo, clave) in binarios)
            {
                Stream contenido;
                try
                {
                    contenido = await almacen.GetAsync(clave, ct);
                }
                catch (ObjectNotFoundException)
                {
                    throw new FotoNoRecuperableException(clave);
                }

                var entrada = zip.CreateEntry(archivo);
                await using var destino = entrada.Open();
                await using (contenido)
                {
                    await contenido.CopyToAsync(destino, ct);
                }
            }
        }

        return ms.ToArray();
    }

    public async Task<ResultadoImportacion> ImportarAsync(Guid idUsuario, Stream unidad, CancellationToken ct = default)
    {
        var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.Id == idUsuario && u.Activo, ct)
                      ?? throw new RolNoAutorizadoException();
        if (usuario.Rol is not (Rol.JefeDeArea or Rol.Raiz))
        {
            throw new RolNoAutorizadoException();
        }

        // El ZIP necesita un stream con posicionamiento: se copia a memoria.
        using var enMemoria = new MemoryStream();
        await unidad.CopyToAsync(enMemoria, ct);
        enMemoria.Position = 0;

        ZipArchive zip;
        try
        {
            zip = new ZipArchive(enMemoria, ZipArchiveMode.Read, leaveOpen: true);
        }
        catch (InvalidDataException)
        {
            throw new UnidadInvalidaException("La unidad no es un archivo comprimido válido.");
        }

        using (zip)
        {
            var entradaManifiesto = zip.GetEntry(NombreManifiesto)
                                    ?? throw new UnidadInvalidaException("La unidad no contiene un manifiesto.");

            ManifiestoExport? manifiesto;
            try
            {
                await using var s = entradaManifiesto.Open();
                manifiesto = await JsonSerializer.DeserializeAsync<ManifiestoExport>(s, Json, ct);
            }
            catch (JsonException)
            {
                throw new UnidadInvalidaException("El manifiesto de la unidad está corrupto.");
            }

            if (manifiesto?.Relevamiento is null || manifiesto.Marcadores is null)
            {
                throw new UnidadIncompletaException("Al manifiesto le faltan piezas necesarias del relevamiento.");
            }

            return await ReconstruirAsync(idUsuario, manifiesto, zip, ct);
        }
    }

    private async Task<ResultadoImportacion> ReconstruirAsync(Guid idUsuario, ManifiestoExport manifiesto, ZipArchive zip, CancellationToken ct)
    {
        var rel = new Relevamiento(manifiesto.Relevamiento.Nombre, manifiesto.Relevamiento.TramoVial, idUsuario);
        db.Relevamientos.Add(rel);

        // Etiquetas del relevamiento (nombre → entidad).
        var etiquetaPorNombre = new Dictionary<string, Etiqueta>(StringComparer.Ordinal);
        foreach (var nombre in (manifiesto.Etiquetas ?? []).Distinct())
        {
            var etiqueta = new Etiqueta(rel.Id, nombre);
            etiquetaPorNombre[nombre] = etiqueta;
            db.Set<Etiqueta>().Add(etiqueta);
        }

        var fotosNoAlojadas = new List<string>();

        foreach (var marcadorExport in manifiesto.Marcadores)
        {
            var marcador = new MarcadorGeografico(rel.Id, marcadorExport.Latitud, marcadorExport.Longitud, marcadorExport.Descripcion);
            db.Set<MarcadorGeografico>().Add(marcador);

            foreach (var nombre in (marcadorExport.Etiquetas ?? []).Distinct())
            {
                if (!etiquetaPorNombre.TryGetValue(nombre, out var etiqueta))
                {
                    etiqueta = new Etiqueta(rel.Id, nombre);
                    etiquetaPorNombre[nombre] = etiqueta;
                    db.Set<Etiqueta>().Add(etiqueta);
                }

                db.Set<EtiquetaMarcador>().Add(new EtiquetaMarcador(etiqueta.Id, marcador.Id));
            }

            foreach (var obsExport in marcadorExport.Observaciones ?? [])
            {
                // El autor en el entorno destino es el solicitante de la importación (RN-02).
                var obs = new Observacion(marcador.Id, idUsuario, obsExport.Nota);
                db.Set<Observacion>().Add(obs);

                foreach (var fotoExport in obsExport.Fotos ?? [])
                {
                    var entrada = zip.GetEntry(fotoExport.Archivo);
                    if (entrada is null)
                    {
                        throw new UnidadIncompletaException($"Falta el binario de la foto '{fotoExport.Archivo}'.");
                    }

                    var clave = $"relevamientos/{rel.Id}/observaciones/{obs.Id}/fotos/{Guid.NewGuid():N}";
                    try
                    {
                        using var contenido = new MemoryStream();
                        await using (var s = entrada.Open())
                        {
                            await s.CopyToAsync(contenido, ct);
                        }

                        contenido.Position = 0;
                        await almacen.SaveAsync(clave, contenido, fotoExport.ContentType, ct);
                    }
                    catch (StorageException)
                    {
                        // CU-16 5.B: la foto no se pudo alojar; se reconstruye el resto y se reporta.
                        fotosNoAlojadas.Add(fotoExport.Archivo);
                        continue;
                    }

                    var foto = new Foto(obs.Id, clave, fotoExport.ContentType, fotoExport.Latitud, fotoExport.Longitud);
                    db.Set<Foto>().Add(foto);

                    if (!string.IsNullOrWhiteSpace(fotoExport.Comentario))
                    {
                        db.Set<Comentario>().Add(new Comentario(foto.Id, fotoExport.Comentario));
                    }
                }
            }
        }

        // Restablece el estado del relevamiento por las transiciones válidas (RN-05).
        if (manifiesto.Relevamiento.Estado is EstadoRelevamiento.Revision or EstadoRelevamiento.Cerrado)
        {
            rel.CambiarEstado(EstadoRelevamiento.Revision);
        }

        if (manifiesto.Relevamiento.Estado is EstadoRelevamiento.Cerrado)
        {
            rel.CambiarEstado(EstadoRelevamiento.Cerrado);
        }

        await db.SaveChangesAsync(ct);
        return new ResultadoImportacion(rel.Id, fotosNoAlojadas);
    }
}
