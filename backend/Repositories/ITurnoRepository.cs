using TurnosApi.Models;

namespace TurnosApi.Repositories;

public interface ITurnoRepository
{
    Task<List<Turno>> GetAllAsync(int? usuarioId, DateTime? desde, DateTime? hasta);
    Task<Turno?> GetByIdAsync(int id);
    Task<List<Turno>> GetActivosEntreAsync(DateTime desde, DateTime hasta);
    Task<Turno> AddAsync(Turno turno);
    Task UpdateAsync(Turno turno);
    Task DeleteAsync(Turno turno);
}
