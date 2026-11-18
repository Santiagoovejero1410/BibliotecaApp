using Microsoft.AspNetCore.Mvc;
using BibliotecaApi.DTOs;
using BibliotecaApi.Services;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;      

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try
        {
            return Ok(await _auth.RegisterAsync(dto));
        }
        catch (InvalidOperationException ex)                      
        {
            return BadRequest(new { mensaje = ex.Message });     
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var respuesta = await _auth.LoginAsync(dto);
        if (respuesta == null)
            return Unauthorized(new { mensaje = "Email o contraseña incorrectos." });   

        return Ok(respuesta);                                      
    }
}
