using Microsoft.AspNetCore.Mvc;
using BibliotecaApi.DTOs;
using BibliotecaApi.Services;

namespace BibliotecaApi.Controllers;

// CONTROLLER: recibe el pedido HTTP y se lo pasa al Service. No tiene reglas de negocio.
// [ApiController] valida los DTO automáticamente (si no cumplen, responde 400 solo).
// Este controller NO tiene [Authorize]: registrarse y loguearse tiene que ser público.
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;      // DI por constructor (interfaz)

    // POST api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try
        {
            return Ok(await _auth.RegisterAsync(dto));
        }
        catch (InvalidOperationException ex)                       // ej: el email ya existe
        {
            return BadRequest(new { mensaje = ex.Message });       // 400
        }
    }

    // POST api/auth/login -> devuelve el JWT
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var respuesta = await _auth.LoginAsync(dto);
        if (respuesta == null)
            return Unauthorized(new { mensaje = "Email o contraseña incorrectos." });   // 401

        return Ok(respuesta);                                      // 200
    }
}
