using BibliotecaApi.DTOs;

namespace BibliotecaApi.Services;

public interface ILibroService
{
    Task<PagedResultDto<LibroResponseDto>> ListarAsync(int pagina, int tamanio, string? buscar);
    Task<LibroResponseDto?> ObtenerAsync(int id);
    Task<LibroResponseDto> CrearAsync(LibroDto dto);
    Task<LibroResponseDto?> ActualizarAsync(int id, LibroDto dto);
    Task<bool> EliminarAsync(int id);
}
