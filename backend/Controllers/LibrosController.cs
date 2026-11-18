using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BibliotecaApi.DTOs;
using BibliotecaApi.Services;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/libros")]
[Authorize]                       
public class LibrosController : ControllerBase
{
    private readonly ILibroService _libros;
    public LibrosController(ILibroService libros) => _libros = libros;

    
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int pagina = 1, [FromQuery] int tamanio = 5, [FromQuery] string? buscar = null) =>
        Ok(await _libros.ListarAsync(pagina, tamanio, buscar));

    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var libro = await _libros.ObtenerAsync(id);
        if (libro == null) return NotFound(new { mensaje = "El libro no existe." });   
        return Ok(libro);
    }

    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Post(LibroDto dto)
    {
        var creado = await _libros.CrearAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);        
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Put(int id, LibroDto dto)
    {
        var actualizado = await _libros.ActualizarAsync(id, dto);
        if (actualizado == null) return NotFound(new { mensaje = "El libro no existe." });
        return Ok(actualizado);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var borrado = await _libros.EliminarAsync(id);
            if (!borrado) return NotFound(new { mensaje = "El libro no existe." });
            return NoContent();                                    
        }
        catch (InvalidOperationException ex)                      
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
