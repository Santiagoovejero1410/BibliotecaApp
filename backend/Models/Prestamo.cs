namespace BibliotecaApi.Models;

// ENTIDAD -> tabla "Prestamos". Une un Libro con un Usuario.
public class Prestamo
{
    public int Id { get; set; }
    public DateTime FechaPrestamo { get; set; }
    public DateTime? FechaDevolucion { get; set; }    // null = todavía no lo devolvió

    // Clave foránea hacia Libro + propiedad de navegación
    public int LibroId { get; set; }
    public Libro Libro { get; set; } = null!;

    // Clave foránea hacia el usuario (el Id de Identity es string)
    public string UsuarioId { get; set; } = string.Empty;
    public ApplicationUser Usuario { get; set; } = null!;
}
