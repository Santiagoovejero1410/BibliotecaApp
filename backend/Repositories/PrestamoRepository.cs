using Microsoft.EntityFrameworkCore;
using BibliotecaApi.Data;
using BibliotecaApi.Models;

namespace BibliotecaApi.Repositories;

public class PrestamoRepository : IPrestamoRepository
{
    private readonly AppDbContext _db;
    public PrestamoRepository(AppDbContext db) => _db = db;

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

    public async Task<int> ContarActivosAsync(string usuarioId) =>
        await _db.Prestamos.CountAsync(p => p.UsuarioId == usuarioId && p.FechaDevolucion == null);

    public async Task<Prestamo> AddAsync(Prestamo prestamo)
    {
        _db.Prestamos.Add(prestamo);
        await _db.SaveChangesAsync();
        return prestamo;
    }

    public async Task UpdateAsync(Prestamo prestamo)
    {
        await _db.SaveChangesAsync();                     
    }
}
