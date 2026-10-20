using Microsoft.EntityFrameworkCore;
using TurnosApi.Models;

namespace TurnosApi.Data;

// DbContext = "puente" entre las clases C# y la base SQL Server.
// Cada DbSet representa una tabla.
public class AppDbContext : DbContext
{
    // El constructor recibe las opciones (cadena de conexión) por inyección de dependencias
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Servicio> Servicios => Set<Servicio>();
    public DbSet<Turno> Turnos => Set<Turno>();

    // Acá se configuran las reglas de las tablas (Fluent API)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(e =>
        {
            e.Property(u => u.Nombre).HasMaxLength(100).IsRequired();
            e.Property(u => u.Email).HasMaxLength(150).IsRequired();
            e.Property(u => u.Rol).HasMaxLength(20).IsRequired();
            e.HasIndex(u => u.Email).IsUnique();                 // No puede haber dos usuarios con el mismo email
        });

        modelBuilder.Entity<Servicio>(e =>
        {
            e.Property(s => s.Nombre).HasMaxLength(100).IsRequired();
            e.Property(s => s.Precio).HasPrecision(10, 2);       // decimal(10,2) en SQL Server
        });

        modelBuilder.Entity<Turno>(e =>
        {
            e.Property(t => t.Estado).HasMaxLength(20).IsRequired();
            e.Property(t => t.Notas).HasMaxLength(300);
            e.HasIndex(t => t.FechaHora);                        // Acelera las búsquedas por fecha

            // RELACIÓN 1-N: Servicio -> Turnos.
            // Restrict = no deja borrar un servicio que tenga turnos (protege los datos).
            e.HasOne(t => t.Servicio)
             .WithMany(s => s.Turnos)
             .HasForeignKey(t => t.ServicioId)
             .OnDelete(DeleteBehavior.Restrict);

            // RELACIÓN 1-N: Usuario -> Turnos
            e.HasOne(t => t.Usuario)
             .WithMany(u => u.Turnos)
             .HasForeignKey(t => t.UsuarioId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
