
using Microsoft.EntityFrameworkCore;
using BibliotecaApi.Models;

namespace BibliotecaApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Libro> Libros => Set<Libro>();
    public DbSet<Prestamo> Prestamos => Set<Prestamo>();

    // Modelos para la autenticación manual.
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> RolesManuales => Set<Rol>();
    public DbSet<UsuarioRol> UsuariosRoles => Set<UsuarioRol>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Nombres de las tablas.
        modelBuilder.Entity<Usuario>().ToTable("Usuarios");
        modelBuilder.Entity<Rol>().ToTable("Roles");
        modelBuilder.Entity<UsuarioRol>().ToTable("UsuariosRoles");

        // Configuración de Usuario.
        modelBuilder.Entity<Usuario>()
            .Property(u => u.Nombre)
            .HasMaxLength(150)
            .IsRequired();

        modelBuilder.Entity<Usuario>()
            .Property(u => u.Email)
            .HasMaxLength(256)
            .IsRequired();

        modelBuilder.Entity<Usuario>()
            .Property(u => u.PasswordHash)
            .IsRequired();

        // Configuración de Rol.
        modelBuilder.Entity<Rol>()
            .Property(r => r.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        // Configuración de Libro.
        modelBuilder.Entity<Libro>()
            .Property(l => l.Titulo)
            .HasMaxLength(150)
            .IsRequired();

        modelBuilder.Entity<Libro>()
            .Property(l => l.Autor)
            .HasMaxLength(100)
            .IsRequired();

        // Relación Libro -> Prestamos.
        modelBuilder.Entity<Prestamo>()
            .HasOne(p => p.Libro)
            .WithMany(l => l.Prestamos)
            .HasForeignKey(p => p.LibroId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación Usuario -> Prestamos.
        modelBuilder.Entity<Prestamo>()
            .HasOne(p => p.Usuario)
            .WithMany(u => u.Prestamos)
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Clave primaria compuesta de UsuarioRol.
        modelBuilder.Entity<UsuarioRol>()
            .HasKey(ur => new { ur.UsuarioId, ur.RolId });

        // Relación Usuario -> UsuarioRol.
        modelBuilder.Entity<UsuarioRol>()
            .HasOne(ur => ur.Usuario)
            .WithMany(u => u.UsuariosRoles)
            .HasForeignKey(ur => ur.UsuarioId);

        // Relación Rol -> UsuarioRol.
        modelBuilder.Entity<UsuarioRol>()
            .HasOne(ur => ur.Rol)
            .WithMany(r => r.UsuariosRoles)
            .HasForeignKey(ur => ur.RolId);
    }
}
