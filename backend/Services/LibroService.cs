using BibliotecaApi.DTOs;
using BibliotecaApi.Models;
using BibliotecaApi.Repositories;

namespace BibliotecaApi.Services;

public class LibroService : ILibroService
{
    private readonly ILibroRepository _repo;
    public LibroService(ILibroRepository repo) => _repo = repo;    // DI por constructor

    public async Task<PagedResultDto<LibroResponseDto>> ListarAsync(int pagina, int tamanio, string? buscar)
    {
        // Regla de negocio: valores razonables para la paginación
        if (pagina < 1) pagina = 1;
        if (tamanio < 1 || tamanio > 50) tamanio = 5;

        var (libros, total) = await _repo.GetPaginadoAsync(buscar, pagina, tamanio);

        return new PagedResultDto<LibroResponseDto>
        {
            Items = libros.Select(ToDto).ToList(),                 // Entidad -> DTO
            Total = total,
            Pagina = pagina,
            TotalPaginas = Math.Max(1, (int)Math.Ceiling(total / (double)tamanio))
        };
    }

    public async Task<LibroResponseDto?> ObtenerAsync(int id)
    {
        var libro = await _repo.GetByIdAsync(id);
        return libro == null ? null : ToDto(libro);                // null = no existe (el controller responde 404)
    }

    public async Task<LibroResponseDto> CrearAsync(LibroDto dto)
    {
        var libro = new Libro
        {
            Titulo = dto.Titulo.Trim(),
            Autor = dto.Autor.Trim(),
            Genero = dto.Genero.Trim()
        };
        await _repo.AddAsync(libro);
        return ToDto(libro);
    }

    public async Task<LibroResponseDto?> ActualizarAsync(int id, LibroDto dto)
    {
        var libro = await _repo.GetByIdAsync(id);
        if (libro == null) return null;

        libro.Titulo = dto.Titulo.Trim();
        libro.Autor = dto.Autor.Trim();
        libro.Genero = dto.Genero.Trim();
        await _repo.UpdateAsync(libro);
        return ToDto(libro);
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var libro = await _repo.GetByIdAsync(id);
        if (libro == null) return false;

        // Regla de negocio: no se puede borrar un libro que ya tuvo préstamos
        if (await _repo.TienePrestamosAsync(id))
            throw new InvalidOperationException("No se puede eliminar: el libro tiene préstamos registrados.");

        await _repo.DeleteAsync(libro);
        return true;
    }

    private static LibroResponseDto ToDto(Libro l) => new()
    {
        Id = l.Id,
        Titulo = l.Titulo,
        Autor = l.Autor,
        Genero = l.Genero,
        Disponible = l.Disponible
    };
}
