using TurnosApi.Models;

namespace TurnosApi.Data;

// Carga datos iniciales (un admin, un cliente, servicios y turnos de ejemplo)
// para que la app tenga algo que mostrar en la defensa. Solo corre si la base está vacía.
public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (!db.Usuarios.Any())
        {
            db.Usuarios.AddRange(
                new Usuario
                {
                    Nombre = "Administrador",
                    Email = "admin@turnos.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),   // Se guarda el HASH, no la clave
                    Rol = Roles.Admin
                },
                new Usuario
                {
                    Nombre = "Cliente Demo",
                    Email = "cliente@turnos.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Cliente123!"),
                    Rol = Roles.Cliente
                });
            db.SaveChanges();
        }

        if (!db.Servicios.Any())
        {
            db.Servicios.AddRange(
                new Servicio { Nombre = "Consulta general", DuracionMin = 30, Precio = 15000 },
                new Servicio { Nombre = "Control anual", DuracionMin = 60, Precio = 25000 },
                new Servicio { Nombre = "Limpieza", DuracionMin = 45, Precio = 20000 },
                new Servicio { Nombre = "Urgencia", DuracionMin = 30, Precio = 30000 });
            db.SaveChanges();
        }

        if (!db.Turnos.Any())
        {
            var cliente = db.Usuarios.First(u => u.Rol == Roles.Cliente);
            var servicios = db.Servicios.OrderBy(s => s.Id).ToList();

            // Buscamos el lunes de esta semana para que los turnos de ejemplo se vean en la agenda
            var lunes = DateTime.Today;
            while (lunes.DayOfWeek != DayOfWeek.Monday) lunes = lunes.AddDays(-1);

            db.Turnos.AddRange(
                new Turno { FechaHora = lunes.AddHours(9), ServicioId = servicios[0].Id, UsuarioId = cliente.Id, Estado = EstadosTurno.Confirmado },
                new Turno { FechaHora = lunes.AddDays(1).AddHours(10), ServicioId = servicios[1].Id, UsuarioId = cliente.Id },
                new Turno { FechaHora = lunes.AddDays(2).AddHours(11).AddMinutes(30), ServicioId = servicios[2].Id, UsuarioId = cliente.Id, Estado = EstadosTurno.Confirmado },
                new Turno { FechaHora = lunes.AddDays(3).AddHours(15), ServicioId = servicios[3].Id, UsuarioId = cliente.Id });
            db.SaveChanges();
        }
    }
}
