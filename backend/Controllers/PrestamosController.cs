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

    
    private string UsuarioId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool EsAdmin => User.IsInRole("Admin");


    [HttpGet]
    public async Task<IActionResult> Get() =>
        Ok(await _prestamos.ListarAsync(UsuarioId, EsAdmin));

    
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
            return NotFound(new { mensaje = ex.Message });         
        }
        catch (InvalidOperationException ex)                       
        {
            return BadRequest(new { mensaje = ex.Message });       
        }
    }

    
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
            return StatusCode(403, new { mensaje = ex.Message });  
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
