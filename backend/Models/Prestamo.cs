
namespace BibliotecaApi.Models;

public class Prestamo
{
    public int Id { get; set; }

    public DateTime FechaPrestamo { get; set; }

    public DateTime? FechaDevolucion { get; set; }

    public int LibroId { get; set; }
    public Libro Libro { get; set; } = null!;

    public string UsuarioId { get; set; } = string.Empty;
    public Usuario Usuario { get; set; } = null!;
}
