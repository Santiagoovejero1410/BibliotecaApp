using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BibliotecaApi.Models;

namespace BibliotecaApi.Data;

// Se ejecuta al arrancar: aplica las migraciones y crea los datos mínimos
// (roles, un admin, un socio de prueba y algunos libros) si todavía no existen.
public static class DbInicial
{
    public static async Task CrearAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();                 // aplica las migraciones pendientes

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // 1) Roles
        foreach (var rol in new[] { "Admin", "Socio" })
        {
            if (!await roleManager.RoleExistsAsync(rol))
                await roleManager.CreateAsync(new IdentityRole(rol));
        }

        // 2) Usuarios de prueba (Identity se encarga de hashear la contraseña)
        await CrearUsuario(userManager, "admin@biblioteca.com", "Administrador", "Admin123!", "Admin");
        await CrearUsuario(userManager, "socio@biblioteca.com", "Socio Demo", "Socio123!", "Socio");

        // 3) Libros de ejemplo
        if (!db.Libros.Any())
        {
            db.Libros.AddRange(
                new Libro { Titulo = "Don Quijote de la Mancha", Autor = "Miguel de Cervantes", Genero = "Novela" },
                new Libro { Titulo = "Cien años de soledad", Autor = "Gabriel García Márquez", Genero = "Novela" },
                new Libro { Titulo = "El Aleph", Autor = "Jorge Luis Borges", Genero = "Cuentos" },
                new Libro { Titulo = "Rayuela", Autor = "Julio Cortázar", Genero = "Novela" },
                new Libro { Titulo = "Martín Fierro", Autor = "José Hernández", Genero = "Poesía" },
                new Libro { Titulo = "Ficciones", Autor = "Jorge Luis Borges", Genero = "Cuentos" },
                new Libro { Titulo = "El túnel", Autor = "Ernesto Sabato", Genero = "Novela" });
            await db.SaveChangesAsync();
        }
    }

    private static async Task CrearUsuario(UserManager<ApplicationUser> um, string email, string nombre, string clave, string rol)
    {
        if (await um.FindByEmailAsync(email) != null) return;
        var user = new ApplicationUser { UserName = email, Email = email, Nombre = nombre };
        await um.CreateAsync(user, clave);
        await um.AddToRoleAsync(user, rol);
    }
}
