using Microsoft.EntityFrameworkCore;
using TurnosApi.Data;
using TurnosApi.Models;

namespace TurnosApi.Repositories;

public class TurnoRepository : ITurnoRepository
{
    private readonly AppDbContext _db;
    public TurnoRepository(AppDbContext db) => _db = db;

    // Lista turnos con filtros opcionales. Si usuarioId viene, solo trae los de ese usuario.
    // Include = hace el JOIN para traer también el Servicio y el Usuario relacionados.
    public async Task<List<Turno>> GetAllAsync(int? usuarioId, DateTime? desde, DateTime? hasta)
    {
        var query = _db.Turnos
            .Include(t => t.Servicio)
            .Include(t => t.Usuario)
            .AsQueryable();

        if (usuarioId.HasValue) query = query.Where(t => t.UsuarioId == usuarioId.Value);
        if (desde.HasValue) query = query.Where(t => t.FechaHora >= desde.Value);
        if (hasta.HasValue) query = query.Where(t => t.FechaHora < hasta.Value);

        return await query.OrderBy(t => t.FechaHora).ToListAsync();
    }

    public async Task<Turno?> GetByIdAsync(int id) =>
        await _db.Turnos
            .Include(t => t.Servicio)
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.Id == id);

    // Turnos NO cancelados dentro de un rango: se usa para detectar si un horario ya está ocupado
    public async Task<List<Turno>> GetActivosEntreAsync(DateTime desde, DateTime hasta) =>
        await _db.Turnos
            .Include(t => t.Servicio)
            .Where(t => t.Estado != EstadosTurno.Cancelado && t.FechaHora >= desde && t.FechaHora < hasta)
            .ToListAsync();

    public async Task<Turno> AddAsync(Turno turno)
    {
        _db.Turnos.Add(turno);
        await _db.SaveChangesAsync();
        return turno;
    }

    // El turno ya viene "trackeado" por EF (lo trajimos con GetByIdAsync), así que alcanza con guardar:
    // EF detecta solo qué propiedades cambiaron y genera el UPDATE.
    public async Task UpdateAsync(Turno turno) =>
        await _db.SaveChangesAsync();

    public async Task DeleteAsync(Turno turno)
    {
        _db.Turnos.Remove(turno);
        await _db.SaveChangesAsync();
    }
}
