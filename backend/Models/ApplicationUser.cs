using Microsoft.AspNetCore.Identity;

namespace BibliotecaApi.Models;

// Usuario de la app. Hereda de IdentityUser (ASP.NET Identity), que ya trae:
// Id, Email, PasswordHash, etc. Nosotros solo agregamos el Nombre.
public class ApplicationUser : IdentityUser
{
    public string Nombre { get; set; } = string.Empty;

    // Relación UNO-A-MUCHOS: un usuario tiene muchos préstamos
    public List<Prestamo> Prestamos { get; set; } = new();
}
