using BibliotecaApi.Models;

namespace BibliotecaApi.Repositories;

public interface IPrestamoRepository
{
    Task<List<Prestamo>> GetAllAsync(string? usuarioId);
    Task<Prestamo?> GetByIdAsync(int id);
    Task<int> ContarActivosAsync(string usuarioId);
    Task<Prestamo> AddAsync(Prestamo prestamo);
    Task UpdateAsync(Prestamo prestamo);
}
