
using Microsoft.EntityFrameworkCore;
using BibliotecaApi.Models;

namespace BibliotecaApi.Data;

public static class DbInicial
{
    public static async Task CrearAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();

        // 1) Aplicar las migraciones pendientes.
        await db.Database.MigrateAsync();

        // 2) Crear los roles manuales si todavía no existen.
        foreach (var nombreRol in new[] { "Admin", "Socio" })
        {
            var rolExiste = await db.RolesManuales
                .AnyAsync(r => r.Nombre == nombreRol);

            if (!rolExiste)
            {
                db.RolesManuales.Add(new Rol
                {
                    Id = Guid.NewGuid().ToString(),
                    Nombre = nombreRol
                });
            }
        }

        await db.SaveChangesAsync();

        // 3) Crear los usuarios de prueba solamente si no existen.
        // Los usuarios existentes no se modifican.
        await CrearUsuario(
            db,
            "admin@biblioteca.com",
            "Administrador",
            "Admin123!",
            "Admin"
        );

        await CrearUsuario(
            db,
            "socio@biblioteca.com",
            "Socio Demo",
            "Socio123!",
            "Socio"
        );

        // 4) Crear libros de ejemplo si la tabla está vacía.
        if (!await db.Libros.AnyAsync())
        {
            db.Libros.AddRange(
                new Libro { Titulo = "Don Quijote de la Mancha", Autor = "Miguel de Cervantes", Genero = "Novela" },
                new Libro { Titulo = "Cien años de soledad", Autor = "Gabriel García Márquez", Genero = "Novela" },
                new Libro { Titulo = "El Aleph", Autor = "Jorge Luis Borges", Genero = "Cuentos" },
                new Libro { Titulo = "Rayuela", Autor = "Julio Cortázar", Genero = "Novela" },
                new Libro { Titulo = "Martín Fierro", Autor = "José Hernández", Genero = "Poesía" },
                new Libro { Titulo = "Ficciones", Autor = "Jorge Luis Borges", Genero = "Cuentos" },
                new Libro { Titulo = "El túnel", Autor = "Ernesto Sabato", Genero = "Novela" }
            );

            await db.SaveChangesAsync();
        }
    }

    private static async Task CrearUsuario(
        AppDbContext db,
        string email,
        string nombre,
        string clave,
        string nombreRol)
    {
        var usuario = await db.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email);

        // Si ya existe, no cambiamos sus datos ni su contraseña.
        if (usuario == null)
        {
            usuario = new Usuario
            {
                Id = Guid.NewGuid().ToString(),
                Nombre = nombre,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(clave)
            };

            db.Usuarios.Add(usuario);
            await db.SaveChangesAsync();
        }

        var rol = await db.RolesManuales
            .FirstOrDefaultAsync(r => r.Nombre == nombreRol);

        if (rol == null)
            throw new InvalidOperationException($"No existe el rol {nombreRol}.");

        var tieneRol = await db.UsuariosRoles.AnyAsync(ur =>
            ur.UsuarioId == usuario.Id && ur.RolId == rol.Id);

        if (!tieneRol)
        {
            db.UsuariosRoles.Add(new UsuarioRol
            {
                UsuarioId = usuario.Id,
                RolId = rol.Id
            });

            await db.SaveChangesAsync();
        }
    }
}
