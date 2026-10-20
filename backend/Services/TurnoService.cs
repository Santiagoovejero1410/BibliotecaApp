using TurnosApi.DTOs;
using TurnosApi.Exceptions;
using TurnosApi.Models;
using TurnosApi.Repositories;

namespace TurnosApi.Services;

public class TurnoService : ITurnoService
{
    // Horario de atención (regla de negocio)
    private const int HoraApertura = 8;
    private const int HoraCierre = 20;

    private readonly ITurnoRepository _turnos;
    private readonly IServicioRepository _servicios;

    // Este service usa DOS repositorios, ambos inyectados por constructor
    public TurnoService(ITurnoRepository turnos, IServicioRepository servicios)
    {
        _turnos = turnos;
        _servicios = servicios;
    }

    // Admin ve TODOS los turnos; Cliente solo los suyos (pasamos null como filtro de usuario si es admin)
    public async Task<List<TurnoResponseDto>> ListarAsync(int usuarioId, bool esAdmin, DateTime? desde, DateTime? hasta)
    {
        var lista = await _turnos.GetAllAsync(esAdmin ? null : usuarioId, desde, hasta);
        return lista.Select(ToDto).ToList();
    }

    public async Task<TurnoResponseDto> CrearAsync(int usuarioId, TurnoCreateDto dto)
    {
        // 1) El servicio debe existir y estar activo
        var servicio = await _servicios.GetByIdAsync(dto.ServicioId)
            ?? throw new NotFoundException("El servicio elegido no existe.");
        if (!servicio.Activo)
            throw new BadRequestException("El servicio elegido no está disponible.");

        var inicio = dto.FechaHora;
        var fin = inicio.AddMinutes(servicio.DuracionMin);

        // 2) No se puede reservar en el pasado
        if (inicio <= DateTime.Now)
            throw new BadRequestException("La fecha y hora del turno deben ser futuras.");

        // 3) Debe estar dentro del horario de atención
        if (inicio.Hour < HoraApertura || fin > inicio.Date.AddHours(HoraCierre))
            throw new BadRequestException($"El horario de atención es de {HoraApertura}:00 a {HoraCierre}:00.");

        // 4) El horario no puede estar ocupado: buscamos turnos del día y vemos si se SUPERPONEN
        //    (dos turnos se superponen si uno empieza antes de que el otro termine, y viceversa)
        var delDia = await _turnos.GetActivosEntreAsync(inicio.Date, inicio.Date.AddDays(1));
        var ocupado = delDia.Any(t => t.FechaHora < fin && t.FechaHora.AddMinutes(t.Servicio.DuracionMin) > inicio);
        if (ocupado)
            throw new ConflictException("Ese horario ya está ocupado. Elegí otro.");

        var turno = new Turno
        {
            FechaHora = inicio,
            Notas = dto.Notas?.Trim(),
            ServicioId = servicio.Id,
            UsuarioId = usuarioId,                  // Viene del TOKEN, no del body
            Estado = EstadosTurno.Pendiente
        };
        await _turnos.AddAsync(turno);

        // Lo volvemos a pedir para que traiga Servicio y Usuario cargados
        var creado = await _turnos.GetByIdAsync(turno.Id);
        return ToDto(creado!);
    }

    public async Task<TurnoResponseDto> CambiarEstadoAsync(int id, int usuarioId, bool esAdmin, string estado)
    {
        if (!EstadosTurno.Todos.Contains(estado))
            throw new BadRequestException("Estado inválido. Valores permitidos: Pendiente, Confirmado, Cancelado.");

        var turno = await BuscarOFallar(id);

        // Regla: el cliente solo puede tocar sus propios turnos, y solo para CANCELARLOS
        if (!esAdmin)
        {
            if (turno.UsuarioId != usuarioId)
                throw new ForbiddenException("No podés modificar turnos de otra persona.");
            if (estado != EstadosTurno.Cancelado)
                throw new ForbiddenException("Solo podés cancelar tus turnos.");
        }

        if (turno.Estado == EstadosTurno.Cancelado)
            throw new BadRequestException("El turno ya está cancelado.");

        turno.Estado = estado;
        await _turnos.UpdateAsync(turno);
        return ToDto(turno);
    }

    public async Task EliminarAsync(int id, int usuarioId, bool esAdmin)
    {
        var turno = await BuscarOFallar(id);

        if (!esAdmin && turno.UsuarioId != usuarioId)
            throw new ForbiddenException("No podés eliminar turnos de otra persona.");

        await _turnos.DeleteAsync(turno);
    }

    private async Task<Turno> BuscarOFallar(int id) =>
        await _turnos.GetByIdAsync(id) ?? throw new NotFoundException($"No existe el turno con id {id}.");

    // Convierte la entidad en DTO de salida
    private static TurnoResponseDto ToDto(Turno t) => new()
    {
        Id = t.Id,
        FechaHora = t.FechaHora,
        FechaFin = t.FechaHora.AddMinutes(t.Servicio.DuracionMin),
        Estado = t.Estado,
        Notas = t.Notas,
        ServicioId = t.ServicioId,
        ServicioNombre = t.Servicio.Nombre,
        DuracionMin = t.Servicio.DuracionMin,
        UsuarioId = t.UsuarioId,
        UsuarioNombre = t.Usuario.Nombre
    };
}
