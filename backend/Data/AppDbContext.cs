using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BibliotecaApi.Models;

namespace BibliotecaApi.Data;

// El DbContext conecta nuestras clases con SQL Server.
// Hereda de IdentityDbContext: así EF también crea las tablas de Identity
// (AspNetUsers, AspNetRoles, etc.) además de las nuestras.
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Libro> Libros => Set<Libro>();           // cada DbSet = una tabla
    public DbSet<Prestamo> Prestamos => Set<Prestamo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);               // IMPORTANTE: configura las tablas de Identity

        modelBuilder.Entity<Libro>().Property(l => l.Titulo).HasMaxLength(150).IsRequired();
        modelBuilder.Entity<Libro>().Property(l => l.Autor).HasMaxLength(100).IsRequired();

        // Relación 1-N: Libro -> Prestamos (Restrict = no se puede borrar un libro con préstamos)
        modelBuilder.Entity<Prestamo>()
            .HasOne(p => p.Libro)
            .WithMany(l => l.Prestamos)
            .HasForeignKey(p => p.LibroId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación 1-N: Usuario -> Prestamos
        modelBuilder.Entity<Prestamo>()
            .HasOne(p => p.Usuario)
            .WithMany(u => u.Prestamos)
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
