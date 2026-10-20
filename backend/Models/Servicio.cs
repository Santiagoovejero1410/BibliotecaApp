namespace TurnosApi.Models;

// ENTIDAD: tabla "Servicios". Es la que tiene el CRUD completo (solo Admin).
public class Servicio
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int DuracionMin { get; set; }                         // Duración en minutos (sirve para calcular cuándo termina un turno)
    public decimal Precio { get; set; }
    public bool Activo { get; set; } = true;                     // "Baja lógica": un servicio inactivo no se puede reservar

    // Relación UNO-A-MUCHOS: un servicio tiene muchos turnos
    public List<Turno> Turnos { get; set; } = new();
}
