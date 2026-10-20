using Microsoft.EntityFrameworkCore;
using TurnosApi.Data;
using TurnosApi.Models;

namespace TurnosApi.Repositories;

public class ServicioRepository : IServicioRepository
{
    private readonly AppDbContext _db;
    public ServicioRepository(AppDbContext db) => _db = db;

    public async Task<List<Servicio>> GetAllAsync(bool soloActivos)
    {
        var query = _db.Servicios.AsQueryable();
        if (soloActivos) query = query.Where(s => s.Activo);     // Filtro opcional
        return await query.OrderBy(s => s.Nombre).ToListAsync();
    }

    public async Task<Servicio?> GetByIdAsync(int id) =>
        await _db.Servicios.FindAsync(id);

    public async Task<Servicio> AddAsync(Servicio servicio)
    {
        _db.Servicios.Add(servicio);
        await _db.SaveChangesAsync();
        return servicio;
    }

    public async Task UpdateAsync(Servicio servicio)
    {
        _db.Servicios.Update(servicio);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Servicio servicio)
    {
        _db.Servicios.Remove(servicio);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> TieneTurnosAsync(int servicioId) =>
        await _db.Turnos.AnyAsync(t => t.ServicioId == servicioId);
}
