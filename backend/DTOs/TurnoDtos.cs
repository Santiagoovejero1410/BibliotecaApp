using System.ComponentModel.DataAnnotations;

namespace TurnosApi.DTOs;

// DTO de ENTRADA: reservar un turno. El UsuarioId NO viene del front: se saca del token (más seguro).
public class TurnoCreateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Debe elegir un servicio")]
    public int ServicioId { get; set; }

    [Required(ErrorMessage = "La fecha y hora son obligatorias")]
    public DateTime FechaHora { get; set; }

    [StringLength(300, ErrorMessage = "Las notas no pueden superar los 300 caracteres")]
    public string? Notas { get; set; }
}

// DTO de ENTRADA: cambiar el estado de un turno
public class TurnoEstadoDto
{
    [Required(ErrorMessage = "El estado es obligatorio")]
    public string Estado { get; set; } = string.Empty;
}

// DTO de SALIDA: lo que ve el front (ya viene con los nombres, no hay que hacer más pedidos)
public class TurnoResponseDto
{
    public int Id { get; set; }
    public DateTime FechaHora { get; set; }
    public DateTime FechaFin { get; set; }          // FechaHora + duración del servicio
    public string Estado { get; set; } = string.Empty;
    public string? Notas { get; set; }
    public int ServicioId { get; set; }
    public string ServicioNombre { get; set; } = string.Empty;
    public int DuracionMin { get; set; }
    public int UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
}
