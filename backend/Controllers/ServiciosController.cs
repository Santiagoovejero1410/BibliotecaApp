using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TurnosApi.DTOs;
using TurnosApi.Models;
using TurnosApi.Services;

namespace TurnosApi.Controllers;

[ApiController]
[Route("api/servicios")]
[Authorize]                       // TODO el controller exige estar logueado (token válido)
public class ServiciosController : ControllerBase
{
    private readonly IServicioService _servicios;
    public ServiciosController(IServicioService servicios) => _servicios = servicios;

    // GET api/servicios -> cualquier usuario logueado. El cliente solo ve los activos; el admin ve todos.
    [HttpGet]
    public async Task<ActionResult<List<ServicioResponseDto>>> Get() =>
        Ok(await _servicios.ListarAsync(soloActivos: !User.IsInRole(Roles.Admin)));

    [HttpGet("{id}")]
    public async Task<ActionResult<ServicioResponseDto>> GetById(int id) =>
        Ok(await _servicios.ObtenerAsync(id));

    // POST/PUT/DELETE -> SOLO Admin. Si un Cliente intenta, devuelve 403 Forbidden.
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<ServicioResponseDto>> Post(ServicioDto dto)
    {
        var creado = await _servicios.CrearAsync(dto);
        // 201 Created + header Location con la URL del nuevo recurso
        return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<ServicioResponseDto>> Put(int id, ServicioDto dto) =>
        Ok(await _servicios.ActualizarAsync(id, dto));

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        await _servicios.EliminarAsync(id);
        return NoContent();            // 204: se borró y no hay nada para devolver
    }
}
