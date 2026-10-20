using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TurnosApi.DTOs;
using TurnosApi.Models;
using TurnosApi.Services;

namespace TurnosApi.Controllers;

[ApiController]
[Route("api/turnos")]
[Authorize]
public class TurnosController : ControllerBase
{
    private readonly ITurnoService _turnos;
    public TurnosController(ITurnoService turnos) => _turnos = turnos;

    // Datos del usuario logueado, sacados de los claims del JWT (no de lo que mande el front)
    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private bool EsAdmin => User.IsInRole(Roles.Admin);

    // GET api/turnos?desde=...&hasta=...  (los filtros son opcionales)
    [HttpGet]
    public async Task<ActionResult<List<TurnoResponseDto>>> Get([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta) =>
        Ok(await _turnos.ListarAsync(UsuarioId, EsAdmin, desde, hasta));

    // POST api/turnos -> reservar un turno (el usuario sale del token)
    [HttpPost]
    public async Task<ActionResult<TurnoResponseDto>> Post(TurnoCreateDto dto)
    {
        var creado = await _turnos.CrearAsync(UsuarioId, dto);
        return StatusCode(StatusCodes.Status201Created, creado);
    }

    // PUT api/turnos/5/estado -> confirmar o cancelar
    [HttpPut("{id}/estado")]
    public async Task<ActionResult<TurnoResponseDto>> PutEstado(int id, TurnoEstadoDto dto) =>
        Ok(await _turnos.CambiarEstadoAsync(id, UsuarioId, EsAdmin, dto.Estado));

    // DELETE api/turnos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _turnos.EliminarAsync(id, UsuarioId, EsAdmin);
        return NoContent();
    }
}
