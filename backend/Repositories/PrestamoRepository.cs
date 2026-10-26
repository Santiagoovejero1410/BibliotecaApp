using Microsoft.EntityFrameworkCore;
using BibliotecaApi.Data;
using BibliotecaApi.Models;

namespace BibliotecaApi.Repositories;

public class PrestamoRepository : IPrestamoRepository
{
    private readonly AppDbContext _db;
    public PrestamoRepository(AppDbContext db) => _db = db;

    // Include = hace el JOIN para traer también el Libro y el Usuario de cada préstamo.
    // Si usuarioId viene, filtra solo los de ese usuario.
    public async Task<List<Prestamo>> GetAllAsync(string? usuarioId)
    {
        var query = _db.Prestamos
            .Include(p => p.Libro)
            .Include(p => p.Usuario)
            .AsQueryable();

        if (usuarioId != null)
            query = query.Where(p => p.UsuarioId == usuarioId);

        return await query.OrderByDescending(p => p.FechaPrestamo).ToListAsync();
    }

    public async Task<Prestamo?> GetByIdAsync(int id) =>
        await _db.Prestamos
            .Include(p => p.Libro)
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.Id == id);

    // Cuántos libros tiene prestados (sin devolver) un usuario
    public async Task<int> ContarActivosAsync(string usuarioId) =>
        await _db.Prestamos.CountAsync(p => p.UsuarioId == usuarioId && p.FechaDevolucion == null);

    // OJO: todos los repositorios comparten el MISMO DbContext (es "scoped": uno por request).
    // Por eso un solo SaveChanges guarda juntos el préstamo nuevo Y el cambio en el libro.
    // Esa es la idea de "Unit of Work": varios cambios, una sola transacción.
    public async Task<Prestamo> AddAsync(Prestamo prestamo)
    {
        _db.Prestamos.Add(prestamo);
        await _db.SaveChangesAsync();
        return prestamo;
    }

    public async Task UpdateAsync(Prestamo prestamo)
    {
        await _db.SaveChangesAsync();                     // el préstamo ya está "trackeado" por EF
    }
}
