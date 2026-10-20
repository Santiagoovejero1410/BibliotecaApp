using Microsoft.EntityFrameworkCore;
using TurnosApi.Data;
using TurnosApi.Models;

namespace TurnosApi.Repositories;

// REPOSITORY: es la ÚNICA capa que habla con la base de datos (usa EF Core). No tiene reglas de negocio.
public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _db;

    // Inyección de dependencias por constructor: ASP.NET nos entrega el DbContext ya armado
    public UsuarioRepository(AppDbContext db) => _db = db;

    public async Task<Usuario?> GetByEmailAsync(string email) =>
        await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<bool> ExisteEmailAsync(string email) =>
        await _db.Usuarios.AnyAsync(u => u.Email == email);

    public async Task<Usuario> AddAsync(Usuario usuario)
    {
        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();                // Acá se ejecuta el INSERT en SQL Server
        return usuario;
    }
}
