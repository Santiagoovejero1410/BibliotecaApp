using TurnosApi.DTOs;

namespace TurnosApi.Services;

public interface ITurnoService
{
    Task<List<TurnoResponseDto>> ListarAsync(int usuarioId, bool esAdmin, DateTime? desde, DateTime? hasta);
    Task<TurnoResponseDto> CrearAsync(int usuarioId, TurnoCreateDto dto);
    Task<TurnoResponseDto> CambiarEstadoAsync(int id, int usuarioId, bool esAdmin, string estado);
    Task EliminarAsync(int id, int usuarioId, bool esAdmin);
}
