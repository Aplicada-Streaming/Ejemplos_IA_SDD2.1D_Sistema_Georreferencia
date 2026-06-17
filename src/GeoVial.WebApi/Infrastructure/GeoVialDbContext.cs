using GeoVial.WebApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace GeoVial.WebApi.Infrastructure;

/// <summary>
/// Contexto de persistencia del backend. En este incremento modela los usuarios y su
/// jerarquía. El proveedor (almacén relacional) se elige por configuración; el modelo
/// es agnóstico del motor concreto.
/// </summary>
public sealed class GeoVialDbContext(DbContextOptions<GeoVialDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Relevamiento> Relevamientos => Set<Relevamiento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var usuario = modelBuilder.Entity<Usuario>();
        usuario.ToTable("usuarios");
        usuario.HasKey(u => u.Id);
        usuario.Property(u => u.NombreUsuario).IsRequired().HasMaxLength(100);
        usuario.HasIndex(u => u.NombreUsuario).IsUnique();
        usuario.Property(u => u.HashContrasena).IsRequired();
        usuario.Property(u => u.Rol).HasConversion<int>().IsRequired();
        usuario.Property(u => u.Activo).IsRequired();
        usuario.Property(u => u.FechaAlta).IsRequired();
        usuario.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(u => u.IdAdministrador)
            .OnDelete(DeleteBehavior.Restrict);

        var relevamiento = modelBuilder.Entity<Relevamiento>();
        relevamiento.ToTable("relevamientos");
        relevamiento.HasKey(r => r.Id);
        relevamiento.Property(r => r.Nombre).IsRequired().HasMaxLength(200);
        relevamiento.Property(r => r.TramoVial).HasMaxLength(300);
        relevamiento.Property(r => r.Estado).HasConversion<int>().IsRequired();
        relevamiento.Property(r => r.IdJefeArea).IsRequired();
        relevamiento.Property(r => r.FechaCreacion).IsRequired();
        relevamiento.Property(r => r.CerradoEn);
        relevamiento.HasOne<Usuario>().WithMany().HasForeignKey(r => r.IdJefeArea).OnDelete(DeleteBehavior.Restrict);
        relevamiento.HasMany(r => r.Marcadores).WithOne().HasForeignKey(m => m.RelevamientoId).OnDelete(DeleteBehavior.Cascade);
        relevamiento.HasMany(r => r.Asignaciones).WithOne().HasForeignKey(a => a.RelevamientoId).OnDelete(DeleteBehavior.Cascade);
        relevamiento.Navigation(r => r.Marcadores).UsePropertyAccessMode(PropertyAccessMode.Field);
        relevamiento.Navigation(r => r.Asignaciones).UsePropertyAccessMode(PropertyAccessMode.Field);

        var marcador = modelBuilder.Entity<MarcadorGeografico>();
        marcador.ToTable("marcadores");
        marcador.HasKey(m => m.Id);
        marcador.Property(m => m.Latitud).IsRequired();
        marcador.Property(m => m.Longitud).IsRequired();
        marcador.Property(m => m.Descripcion).HasMaxLength(500);
        marcador.Property(m => m.IdOrigen).HasMaxLength(128);
        marcador.Property(m => m.FechaCreacion).IsRequired();
        marcador.Property(m => m.ActualizadoEn).IsRequired();
        marcador.HasIndex(m => new { m.RelevamientoId, m.ActualizadoEn });
        // Lookup del id de origen para reconocer reenvíos en la subida (RN-07); la unicidad
        // efectiva la garantiza la capa de aplicación (el id_origen puede ser nulo en línea).
        marcador.HasIndex(m => new { m.RelevamientoId, m.IdOrigen });

        var asignacion = modelBuilder.Entity<AsignacionAgente>();
        asignacion.ToTable("asignaciones_agente");
        asignacion.HasKey(a => new { a.RelevamientoId, a.IdAgente });
        asignacion.Property(a => a.FechaAsignacion).IsRequired();
        asignacion.HasOne<Usuario>().WithMany().HasForeignKey(a => a.IdAgente).OnDelete(DeleteBehavior.Restrict);

        var observacion = modelBuilder.Entity<Observacion>();
        observacion.ToTable("observaciones");
        observacion.HasKey(o => o.Id);
        observacion.Property(o => o.Nota).HasMaxLength(2000);
        observacion.Property(o => o.IdOrigen).HasMaxLength(128);
        observacion.Property(o => o.FechaCreacion).IsRequired();
        observacion.HasIndex(o => o.MarcadorId);
        // RC-02: la observación exige un marcador existente; sin cascada para no perder
        // trabajo (la baja del marcador con observaciones se impide en la capa de aplicación).
        observacion.HasOne<MarcadorGeografico>().WithMany().HasForeignKey(o => o.MarcadorId).OnDelete(DeleteBehavior.Restrict);
        // RN-02: la autoría se conserva; la baja del autor no borra sus observaciones.
        observacion.HasOne<Usuario>().WithMany().HasForeignKey(o => o.AutorId).OnDelete(DeleteBehavior.Restrict);

        var etiqueta = modelBuilder.Entity<Etiqueta>();
        etiqueta.ToTable("etiquetas");
        etiqueta.HasKey(e => e.Id);
        etiqueta.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
        etiqueta.HasIndex(e => new { e.RelevamientoId, e.Nombre }).IsUnique();
        etiqueta.HasOne<Relevamiento>().WithMany().HasForeignKey(e => e.RelevamientoId).OnDelete(DeleteBehavior.Cascade);

        var etiquetaMarcador = modelBuilder.Entity<EtiquetaMarcador>();
        etiquetaMarcador.ToTable("etiquetas_marcador");
        etiquetaMarcador.HasKey(em => new { em.EtiquetaId, em.MarcadorId });
        etiquetaMarcador.HasOne<Etiqueta>().WithMany().HasForeignKey(em => em.EtiquetaId).OnDelete(DeleteBehavior.Cascade);
        etiquetaMarcador.HasOne<MarcadorGeografico>().WithMany().HasForeignKey(em => em.MarcadorId).OnDelete(DeleteBehavior.Restrict);

        var foto = modelBuilder.Entity<Foto>();
        foto.ToTable("fotos");
        foto.HasKey(f => f.Id);
        foto.Property(f => f.ReferenciaAlmacen).IsRequired().HasMaxLength(512);
        foto.Property(f => f.ContentType).IsRequired().HasMaxLength(100);
        foto.Property(f => f.PendienteUbicacion).IsRequired();
        foto.Property(f => f.FechaCreacion).IsRequired();
        foto.HasIndex(f => f.ObservacionId);
        foto.HasOne<Observacion>().WithMany().HasForeignKey(f => f.ObservacionId).OnDelete(DeleteBehavior.Cascade);

        var comentario = modelBuilder.Entity<Comentario>();
        comentario.ToTable("comentarios");
        comentario.HasKey(co => co.Id);
        comentario.Property(co => co.Texto).IsRequired().HasMaxLength(1000);
        comentario.Property(co => co.FechaCreacion).IsRequired();
        // A lo sumo un comentario por foto (cardinalidad 1—0..1).
        comentario.HasIndex(co => co.FotoId).IsUnique();
        comentario.HasOne<Foto>().WithMany().HasForeignKey(co => co.FotoId).OnDelete(DeleteBehavior.Cascade);

        var conflicto = modelBuilder.Entity<ConflictoMarcadores>();
        conflicto.ToTable("conflictos_marcadores");
        conflicto.HasKey(co => co.Id);
        conflicto.Property(co => co.Estado).HasConversion<int>().IsRequired();
        conflicto.Property(co => co.Resolucion).HasConversion<int?>();
        conflicto.Property(co => co.DetectadoEn).IsRequired();
        conflicto.HasIndex(co => new { co.RelevamientoId, co.Estado });
        conflicto.HasOne<Relevamiento>().WithMany().HasForeignKey(co => co.RelevamientoId).OnDelete(DeleteBehavior.Cascade);

        var conflictoMiembro = modelBuilder.Entity<ConflictoMarcadorMiembro>();
        conflictoMiembro.ToTable("conflictos_marcador_miembro");
        conflictoMiembro.HasKey(cm => new { cm.ConflictoId, cm.MarcadorId });
        conflictoMiembro.HasIndex(cm => cm.MarcadorId);
        conflictoMiembro.HasOne<ConflictoMarcadores>().WithMany().HasForeignKey(cm => cm.ConflictoId).OnDelete(DeleteBehavior.Cascade);
        conflictoMiembro.HasOne<MarcadorGeografico>().WithMany().HasForeignKey(cm => cm.MarcadorId).OnDelete(DeleteBehavior.Restrict);

        var claveIdempotencia = modelBuilder.Entity<ClaveIdempotencia>();
        claveIdempotencia.ToTable("claves_idempotencia");
        claveIdempotencia.HasKey(k => k.Id);
        claveIdempotencia.Property(k => k.Clave).IsRequired().HasMaxLength(200);
        claveIdempotencia.HasIndex(k => k.Clave).IsUnique();
        claveIdempotencia.Property(k => k.HuellaSolicitud).IsRequired().HasMaxLength(128);
        claveIdempotencia.Property(k => k.Estado).HasConversion<int>().IsRequired();
        claveIdempotencia.Property(k => k.CreadoEn).IsRequired();

        var marca = modelBuilder.Entity<MarcaSincronizacion>();
        marca.ToTable("marcas_sincronizacion");
        marca.HasKey(s => s.Id);
        marca.Property(s => s.Valor).IsRequired();
        marca.Property(s => s.SubidaConcluida).IsRequired();
        marca.Property(s => s.ActualizadoEn).IsRequired();
        // Una marca por par relevamiento-cliente (RC-06).
        marca.HasIndex(s => new { s.RelevamientoId, s.ClienteId }).IsUnique();
        marca.HasOne<Relevamiento>().WithMany().HasForeignKey(s => s.RelevamientoId).OnDelete(DeleteBehavior.Cascade);
        marca.HasOne<Usuario>().WithMany().HasForeignKey(s => s.ClienteId).OnDelete(DeleteBehavior.Restrict);
    }
}
