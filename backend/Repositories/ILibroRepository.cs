using BibliotecaApi.Models;

namespace BibliotecaApi.Repositories;

public interface ILibroRepository
{
    Task<(List<Libro> Items, int Total)> GetPaginadoAsync(string? buscar, int pagina, int tamanio);
    Task<Libro?> GetByIdAsync(int id);
    Task<Libro> AddAsync(Libro libro);
    Task UpdateAsync(Libro libro);
    Task DeleteAsync(Libro libro);
    Task<bool> TienePrestamosAsync(int libroId);
}
