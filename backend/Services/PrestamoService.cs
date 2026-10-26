using BibliotecaApi.DTOs;
using BibliotecaApi.Models;
using BibliotecaApi.Repositories;

namespace BibliotecaApi.Services;

public class PrestamoService : IPrestamoService
{
    private const int MaximoPrestamosActivos = 3;

    private readonly IPrestamoRepository _prestamos;
    private readonly ILibroRepository _libros;

    // Este service usa DOS repositorios, los dos inyectados por constructor
    public PrestamoService(IPrestamoRepository prestamos, ILibroRepository libros)
    {
        _prestamos = prestamos;
        _libros = libros;
    }

    // El admin ve todos los préstamos; el socio solo los suyos
    public async Task<List<PrestamoResponseDto>> ListarAsync(string usuarioId, bool esAdmin)
    {
        var lista = await _prestamos.GetAllAsync(esAdmin ? null : usuarioId);
        return lista.Select(ToDto).ToList();
    }

    public async Task<PrestamoResponseDto> PrestarAsync(string usuarioId, int libroId)
    {
        var libro = await _libros.GetByIdAsync(libroId)
            ?? throw new KeyNotFoundException("El libro no existe.");

        // Reglas de negocio
        if (!libro.Disponible)
            throw new InvalidOperationException("El libro no está disponible, ya está prestado.");

        if (await _prestamos.ContarActivosAsync(usuarioId) >= MaximoPrestamosActivos)
            throw new InvalidOperationException($"No podés tener más de {MaximoPrestamosActivos} libros prestados a la vez.");

        var prestamo = new Prestamo
        {
            LibroId = libro.Id,
            UsuarioId = usuarioId,                // sale del TOKEN, no de lo que mande el front
            FechaPrestamo = DateTime.Now
        };
        libro.Disponible = false;                 // el libro pasa a estar prestado

        // Un solo SaveChanges guarda el préstamo nuevo y el cambio del libro juntos
        await _prestamos.AddAsync(prestamo);

        var creado = await _prestamos.GetByIdAsync(prestamo.Id);   // lo pedimos de nuevo para traer Libro y Usuario
        return ToDto(creado!);
    }

    public async Task<PrestamoResponseDto> DevolverAsync(int prestamoId, string usuarioId, bool esAdmin)
    {
        var prestamo = await _prestamos.GetByIdAsync(prestamoId)
            ?? throw new KeyNotFoundException("El préstamo no existe.");

        // Un socio solo puede devolver sus propios préstamos
        if (!esAdmin && prestamo.UsuarioId != usuarioId)
            throw new UnauthorizedAccessException("No podés devolver el préstamo de otra persona.");

        if (prestamo.FechaDevolucion != null)
            throw new InvalidOperationException("Este libro ya fue devuelto.");

        prestamo.FechaDevolucion = DateTime.Now;
        prestamo.Libro.Disponible = true;         // el libro vuelve a estar disponible

        await _prestamos.UpdateAsync(prestamo);
        return ToDto(prestamo);
    }

    private static PrestamoResponseDto ToDto(Prestamo p) => new()
    {
        Id = p.Id,
        LibroId = p.LibroId,
        LibroTitulo = p.Libro.Titulo,
        UsuarioNombre = p.Usuario.Nombre,
        FechaPrestamo = p.FechaPrestamo,
        FechaDevolucion = p.FechaDevolucion,
        Devuelto = p.FechaDevolucion != null
    };
}
