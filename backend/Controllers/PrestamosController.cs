using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BibliotecaApi.DTOs;
using BibliotecaApi.Services;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/prestamos")]
[Authorize]
public class PrestamosController : ControllerBase
{
    private readonly IPrestamoService _prestamos;
    public PrestamosController(IPrestamoService prestamos) => _prestamos = prestamos;

    // Datos del usuario logueado, sacados de los claims del token JWT
    private string UsuarioId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool EsAdmin => User.IsInRole("Admin");

    // GET api/prestamos -> el socio ve los suyos, el admin ve todos
    [HttpGet]
    public async Task<IActionResult> Get() =>
        Ok(await _prestamos.ListarAsync(UsuarioId, EsAdmin));

    // POST api/prestamos  { "libroId": 3 }
    [HttpPost]
    public async Task<IActionResult> Post(PrestamoCreateDto dto)
    {
        try
        {
            var prestamo = await _prestamos.PrestarAsync(UsuarioId, dto.LibroId);
            return StatusCode(201, prestamo);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });         // 404
        }
        catch (InvalidOperationException ex)                       // libro no disponible, límite de préstamos
        {
            return BadRequest(new { mensaje = ex.Message });       // 400
        }
    }

    // PUT api/prestamos/5/devolver
    [HttpPut("{id}/devolver")]
    public async Task<IActionResult> Devolver(int id)
    {
        try
        {
            return Ok(await _prestamos.DevolverAsync(id, UsuarioId, EsAdmin));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { mensaje = ex.Message });  // 403: no tenés permiso
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
