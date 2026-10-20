using TurnosApi.Models;

namespace TurnosApi.Repositories;

public interface IServicioRepository
{
    Task<List<Servicio>> GetAllAsync(bool soloActivos);
    Task<Servicio?> GetByIdAsync(int id);
    Task<Servicio> AddAsync(Servicio servicio);
    Task UpdateAsync(Servicio servicio);
    Task DeleteAsync(Servicio servicio);
    Task<bool> TieneTurnosAsync(int servicioId);
}
