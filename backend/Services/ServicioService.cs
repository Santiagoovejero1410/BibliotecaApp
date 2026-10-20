using TurnosApi.DTOs;
using TurnosApi.Exceptions;
using TurnosApi.Models;
using TurnosApi.Repositories;

namespace TurnosApi.Services;

public class ServicioService : IServicioService
{
    private readonly IServicioRepository _repo;
    public ServicioService(IServicioRepository repo) => _repo = repo;

    public async Task<List<ServicioResponseDto>> ListarAsync(bool soloActivos)
    {
        var lista = await _repo.GetAllAsync(soloActivos);
        return lista.Select(ToDto).ToList();                      // Entidad -> DTO
    }

    public async Task<ServicioResponseDto> ObtenerAsync(int id) =>
        ToDto(await BuscarOFallar(id));

    public async Task<ServicioResponseDto> CrearAsync(ServicioDto dto)
    {
        var servicio = new Servicio
        {
            Nombre = dto.Nombre.Trim(),
            DuracionMin = dto.DuracionMin,
            Precio = dto.Precio,
            Activo = dto.Activo
        };
        await _repo.AddAsync(servicio);
        return ToDto(servicio);
    }

    public async Task<ServicioResponseDto> ActualizarAsync(int id, ServicioDto dto)
    {
        var servicio = await BuscarOFallar(id);
        servicio.Nombre = dto.Nombre.Trim();
        servicio.DuracionMin = dto.DuracionMin;
        servicio.Precio = dto.Precio;
        servicio.Activo = dto.Activo;
        await _repo.UpdateAsync(servicio);
        return ToDto(servicio);
    }

    public async Task EliminarAsync(int id)
    {
        var servicio = await BuscarOFallar(id);

        // Regla de negocio: no se puede borrar un servicio que ya tiene turnos (habría que desactivarlo)
        if (await _repo.TieneTurnosAsync(id))
            throw new ConflictException("No se puede eliminar: el servicio tiene turnos asociados. Desactivalo en su lugar.");

        await _repo.DeleteAsync(servicio);
    }

    // Método auxiliar: busca o lanza 404 (lo captura el middleware)
    private async Task<Servicio> BuscarOFallar(int id) =>
        await _repo.GetByIdAsync(id) ?? throw new NotFoundException($"No existe el servicio con id {id}.");

    private static ServicioResponseDto ToDto(Servicio s) => new()
    {
        Id = s.Id,
        Nombre = s.Nombre,
        DuracionMin = s.DuracionMin,
        Precio = s.Precio,
        Activo = s.Activo
    };
}
