using System.ComponentModel.DataAnnotations;

namespace TurnosApi.DTOs;

// DTO de ENTRADA (crear y editar un servicio)
public class ServicioDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Range(5, 480, ErrorMessage = "La duración debe estar entre 5 y 480 minutos")]
    public int DuracionMin { get; set; }

    [Range(0, 1000000, ErrorMessage = "El precio debe ser mayor o igual a 0")]
    public decimal Precio { get; set; }

    public bool Activo { get; set; } = true;
}

// DTO de SALIDA
public class ServicioResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int DuracionMin { get; set; }
    public decimal Precio { get; set; }
    public bool Activo { get; set; }
}
