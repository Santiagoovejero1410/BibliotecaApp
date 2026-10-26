using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.DTOs;

// DTO de ENTRADA: pedir un libro prestado. El usuario NO viene del front: se saca del token.
public class PrestamoCreateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Debe elegir un libro")]
    public int LibroId { get; set; }
}

// DTO de SALIDA
public class PrestamoResponseDto
{
    public int Id { get; set; }
    public int LibroId { get; set; }
    public string LibroTitulo { get; set; } = string.Empty;
    public string UsuarioNombre { get; set; } = string.Empty;
    public DateTime FechaPrestamo { get; set; }
    public DateTime? FechaDevolucion { get; set; }
    public bool Devuelto { get; set; }
}
