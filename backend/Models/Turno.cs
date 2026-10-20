namespace TurnosApi.Models;

// ENTIDAD: tabla "Turnos". Tiene dos claves foráneas (Servicio y Usuario).
public class Turno
{
    public int Id { get; set; }
    public DateTime FechaHora { get; set; }                      // Cuándo empieza el turno
    public string Estado { get; set; } = EstadosTurno.Pendiente;
    public string? Notas { get; set; }                           // El "?" significa que puede ser null (opcional)

    // Clave foránea + propiedad de navegación hacia Servicio
    public int ServicioId { get; set; }
    public Servicio Servicio { get; set; } = null!;

    // Clave foránea + propiedad de navegación hacia Usuario
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
}
