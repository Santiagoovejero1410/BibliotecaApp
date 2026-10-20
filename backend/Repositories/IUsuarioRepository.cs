using TurnosApi.Models;

namespace TurnosApi.Repositories;

// INTERFAZ: define QUÉ operaciones existen. El Service depende de la interfaz, no de la clase concreta
// (esto es lo que permite la inyección de dependencias y facilita los tests).
public interface IUsuarioRepository
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<bool> ExisteEmailAsync(string email);
    Task<Usuario> AddAsync(Usuario usuario);
}
