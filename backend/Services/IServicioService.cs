using TurnosApi.DTOs;

namespace TurnosApi.Services;

public interface IServicioService
{
    Task<List<ServicioResponseDto>> ListarAsync(bool soloActivos);
    Task<ServicioResponseDto> ObtenerAsync(int id);
    Task<ServicioResponseDto> CrearAsync(ServicioDto dto);
    Task<ServicioResponseDto> ActualizarAsync(int id, ServicioDto dto);
    Task EliminarAsync(int id);
}
