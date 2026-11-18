using Microsoft.EntityFrameworkCore;
using BibliotecaApi.Models;

namespace BibliotecaApi.Data;

public static class DbInicial
{
    public static async Task CrearAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();

        await db.Database.MigrateAsync();

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

        var libros = new[]
        {
            new Libro { Titulo = "Don Quijote de la Mancha", Autor = "Miguel de Cervantes", Genero = "Novela" },
            new Libro { Titulo = "Cien años de soledad", Autor = "Gabriel García Márquez", Genero = "Novela" },
            new Libro { Titulo = "El Aleph", Autor = "Jorge Luis Borges", Genero = "Cuentos" },
            new Libro { Titulo = "Rayuela", Autor = "Julio Cortázar", Genero = "Novela" },
            new Libro { Titulo = "Martín Fierro", Autor = "José Hernández", Genero = "Poesía" },
            new Libro { Titulo = "Ficciones", Autor = "Jorge Luis Borges", Genero = "Cuentos" },
            new Libro { Titulo = "El túnel", Autor = "Ernesto Sabato", Genero = "Novela" },
            new Libro { Titulo = "La sombra del viento", Autor = "Carlos Ruiz Zafón", Genero = "Novela" },
            new Libro { Titulo = "1984", Autor = "George Orwell", Genero = "Ciencia ficción" },
            new Libro { Titulo = "Rebelión en la granja", Autor = "George Orwell", Genero = "Novela" },
            new Libro { Titulo = "El principito", Autor = "Antoine de Saint-Exupéry", Genero = "Fantasía" },
            new Libro { Titulo = "Crónica de una muerte anunciada", Autor = "Gabriel García Márquez", Genero = "Novela" },
            new Libro { Titulo = "La casa de los espíritus", Autor = "Isabel Allende", Genero = "Novela" },
            new Libro { Titulo = "Pedro Páramo", Autor = "Juan Rulfo", Genero = "Novela" },
            new Libro { Titulo = "La ciudad y los perros", Autor = "Mario Vargas Llosa", Genero = "Novela" },
            new Libro { Titulo = "El nombre de la rosa", Autor = "Umberto Eco", Genero = "Misterio" },
            new Libro { Titulo = "Drácula", Autor = "Bram Stoker", Genero = "Terror" },
            new Libro { Titulo = "Frankenstein", Autor = "Mary Shelley", Genero = "Terror" },
            new Libro { Titulo = "Orgullo y prejuicio", Autor = "Jane Austen", Genero = "Romance" },
            new Libro { Titulo = "El retrato de Dorian Gray", Autor = "Oscar Wilde", Genero = "Novela" },
            new Libro { Titulo = "La metamorfosis", Autor = "Franz Kafka", Genero = "Novela" },
            new Libro { Titulo = "El extranjero", Autor = "Albert Camus", Genero = "Novela" },
            new Libro { Titulo = "El viejo y el mar", Autor = "Ernest Hemingway", Genero = "Novela" },
            new Libro { Titulo = "Alicia en el país de las maravillas", Autor = "Lewis Carroll", Genero = "Fantasía" },
            new Libro { Titulo = "Harry Potter y la piedra filosofal", Autor = "J. K. Rowling", Genero = "Fantasía" },
            new Libro { Titulo = "El Hobbit", Autor = "J. R. R. Tolkien", Genero = "Fantasía" },
            new Libro { Titulo = "El señor de los anillos", Autor = "J. R. R. Tolkien", Genero = "Fantasía" },
            new Libro { Titulo = "Los juegos del hambre", Autor = "Suzanne Collins", Genero = "Ciencia ficción" },
            new Libro { Titulo = "Dune", Autor = "Frank Herbert", Genero = "Ciencia ficción" },
            new Libro { Titulo = "El código Da Vinci", Autor = "Dan Brown", Genero = "Misterio" }
        };

        foreach (var libro in libros)
        {
            var existe = await db.Libros
                .AnyAsync(l => l.Titulo == libro.Titulo);

            if (!existe)
            {
                db.Libros.Add(libro);
            }
        }

        await db.SaveChangesAsync();
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