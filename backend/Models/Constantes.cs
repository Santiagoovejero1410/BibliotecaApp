namespace TurnosApi.Models;

// Constantes para no escribir "Admin" o "Pendiente" a mano en todos lados (evita errores de tipeo).
public static class Roles
{
    public const string Admin = "Admin";
    public const string Cliente = "Cliente";
}

public static class EstadosTurno
{
    public const string Pendiente = "Pendiente";
    public const string Confirmado = "Confirmado";
    public const string Cancelado = "Cancelado";

    // Lista para validar que el estado que llega desde el front sea uno válido
    public static readonly string[] Todos = { Pendiente, Confirmado, Cancelado };
}
