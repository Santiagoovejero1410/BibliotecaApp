namespace BibliotecaApi.Models;

// ENTIDAD -> tabla "Libros". Sobre esta entidad hacemos el CRUD completo.
public class Libro
{
    public int Id { get; set; }                       // Clave primaria
    public string Titulo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public bool Disponible { get; set; } = true;      // false = está prestado

    // Relación UNO-A-MUCHOS: un libro tiene muchos préstamos (a lo largo del tiempo)
    public List<Prestamo> Prestamos { get; set; } = new();
}
