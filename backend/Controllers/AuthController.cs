using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TurnosApi.DTOs;
using TurnosApi.Services;

namespace TurnosApi.Controllers;

// CONTROLLER: solo recibe el request HTTP y delega en el Service. NO tiene lógica de negocio.
// [ApiController] activa la validación automática de los DTO (devuelve 400 si no cumplen los atributos).
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;   // DI por constructor (interfaz, no clase)

    // POST api/auth/register  -> público (no hace falta token)
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto) =>
        Ok(await _auth.RegisterAsync(dto));

    // POST api/auth/login  -> público, devuelve el JWT
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto) =>
        Ok(await _auth.LoginAsync(dto));
}
