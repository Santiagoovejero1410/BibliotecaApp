namespace TurnosApi.Models;

// ENTIDAD: se convierte en la tabla "Usuarios" de SQL Server (Entity Framework Core).
public class Usuario
{
    public int Id { get; set; }                                  // Clave primaria (autoincremental)
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;            // Es único (índice en AppDbContext)
    public string PasswordHash { get; set; } = string.Empty;     // NUNCA se guarda la contraseña en texto plano
    public string Rol { get; set; } = Roles.Cliente;             // "Admin" o "Cliente"

    // Relación UNO-A-MUCHOS: un usuario tiene muchos turnos
    public List<Turno> Turnos { get; set; } = new();
}
