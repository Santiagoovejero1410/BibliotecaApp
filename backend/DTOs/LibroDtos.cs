using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.DTOs;

// DTO de ENTRADA (crear y editar un libro)
public class LibroDto
{
    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "El título debe tener entre 2 y 150 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El autor es obligatorio")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El autor debe tener entre 2 y 100 caracteres")]
    public string Autor { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "El género no puede superar los 50 caracteres")]
    public string Genero { get; set; } = string.Empty;
}

// DTO de SALIDA
public class LibroResponseDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public bool Disponible { get; set; }
}

// Respuesta paginada: la lista de la página actual + datos para armar los botones de paginación
public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int TotalPaginas { get; set; }
}
