using Microsoft.EntityFrameworkCore;
using BibliotecaApi.Data;
using BibliotecaApi.Models;

namespace BibliotecaApi.Repositories;

// REPOSITORY: la ÚNICA capa que habla con la base de datos (con EF Core y LINQ). No tiene reglas de negocio.
public class LibroRepository : ILibroRepository
{
    private readonly AppDbContext _db;

    // Inyección de dependencias por constructor: ASP.NET nos entrega el DbContext
    public LibroRepository(AppDbContext db) => _db = db;

    public async Task<(List<Libro> Items, int Total)> GetPaginadoAsync(string? buscar, int pagina, int tamanio)
    {
        var query = _db.Libros.AsQueryable();

        // LINQ: filtro opcional por título o autor
        if (!string.IsNullOrWhiteSpace(buscar))
            query = query.Where(l => l.Titulo.Contains(buscar) || l.Autor.Contains(buscar));

        var total = await query.CountAsync();             // cuántos libros hay en total (para saber cuántas páginas)

        // PAGINACIÓN: Skip saltea los de las páginas anteriores, Take toma solo los de esta página
        var items = await query
            .OrderBy(l => l.Titulo)
            .Skip((pagina - 1) * tamanio)
            .Take(tamanio)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Libro?> GetByIdAsync(int id) =>
        await _db.Libros.FindAsync(id);

    public async Task<Libro> AddAsync(Libro libro)
    {
        _db.Libros.Add(libro);
        await _db.SaveChangesAsync();                     // acá se ejecuta el INSERT
        return libro;
    }

    public async Task UpdateAsync(Libro libro)
    {
        _db.Libros.Update(libro);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Libro libro)
    {
        _db.Libros.Remove(libro);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> TienePrestamosAsync(int libroId) =>
        await _db.Prestamos.AnyAsync(p => p.LibroId == libroId);
}
