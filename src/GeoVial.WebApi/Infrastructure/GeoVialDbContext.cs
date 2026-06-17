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
        marcador.Property(m => m.FechaCreacion).IsRequired();

        var asignacion = modelBuilder.Entity<AsignacionAgente>();
        asignacion.ToTable("asignaciones_agente");
        asignacion.HasKey(a => new { a.RelevamientoId, a.IdAgente });
        asignacion.Property(a => a.FechaAsignacion).IsRequired();
        asignacion.HasOne<Usuario>().WithMany().HasForeignKey(a => a.IdAgente).OnDelete(DeleteBehavior.Restrict);
    }
}
