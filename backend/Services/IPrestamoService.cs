using BibliotecaApi.DTOs;

namespace BibliotecaApi.Services;

public interface IPrestamoService
{
    Task<List<PrestamoResponseDto>> ListarAsync(string usuarioId, bool esAdmin);
    Task<PrestamoResponseDto> PrestarAsync(string usuarioId, int libroId);
    Task<PrestamoResponseDto> DevolverAsync(int prestamoId, string usuarioId, bool esAdmin);
}
