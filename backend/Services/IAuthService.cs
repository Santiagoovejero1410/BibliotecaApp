using BibliotecaApi.DTOs;

namespace BibliotecaApi.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);      // devuelve null si las credenciales son incorrectas
}
