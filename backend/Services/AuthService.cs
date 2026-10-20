using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TurnosApi.DTOs;
using TurnosApi.Exceptions;
using TurnosApi.Models;
using TurnosApi.Repositories;

namespace TurnosApi.Services;

// SERVICE: acá viven las REGLAS DE NEGOCIO (validar, decidir, calcular). No toca la BD directamente: usa el Repository.
public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IConfiguration _config;

    // DI por constructor: recibe el repositorio y la configuración (appsettings.json)
    public AuthService(IUsuarioRepository usuarios, IConfiguration config)
    {
        _usuarios = usuarios;
        _config = config;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        if (await _usuarios.ExisteEmailAsync(email))
            throw new ConflictException("Ya existe un usuario registrado con ese email.");

        var usuario = new Usuario
        {
            Nombre = dto.Nombre.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),  // Hash con sal automática
            Rol = Roles.Cliente        // Todo el que se registra es Cliente. Admin solo se crea por seed/BD.
        };

        await _usuarios.AddAsync(usuario);
        return Armar(usuario);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var usuario = await _usuarios.GetByEmailAsync(dto.Email.Trim().ToLower());

        // Mismo mensaje si falla el email o la clave (no damos pistas a un atacante)
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            throw new UnauthorizedAppException("Email o contraseña incorrectos.");

        return Armar(usuario);
    }

    private AuthResponseDto Armar(Usuario u) => new()
    {
        Token = GenerarToken(u),
        Id = u.Id,
        Nombre = u.Nombre,
        Email = u.Email,
        Rol = u.Rol
    };

    // Genera el JWT: un string firmado que contiene "claims" (datos del usuario: id, nombre, rol).
    // El servidor lo firma con una clave secreta; si alguien lo modifica, la firma deja de ser válida.
    private string GenerarToken(Usuario u)
    {
        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, u.Id.ToString()),  // Quién es
            new Claim(ClaimTypes.Name, u.Nombre),
            new Claim(ClaimTypes.Email, u.Email),
            new Claim(ClaimTypes.Role, u.Rol)                       // Esto es lo que usa [Authorize(Roles = "Admin")]
        };

        var horas = int.Parse(_config["Jwt:ExpireHours"] ?? "8");

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(horas),               // El token vence
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
