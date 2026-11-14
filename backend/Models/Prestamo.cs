
namespace BibliotecaApi.Models;

// Entidad que representa un préstamo de un libro a un usuario.
public class Prestamo
{
    public int Id { get; set; }

    public DateTime FechaPrestamo { get; set; }

    public DateTime? FechaDevolucion { get; set; }

    // Relación con Libro.
    public int LibroId { get; set; }
    public Libro Libro { get; set; } = null!;

    // Relación con el nuevo modelo Usuario.
    public string UsuarioId { get; set; } = string.Empty;
    public Usuario Usuario { get; set; } = null!;
}
