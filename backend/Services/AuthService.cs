using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using BibliotecaApi.DTOs;
using BibliotecaApi.Models;

namespace BibliotecaApi.Services;

// SERVICE: reglas de negocio. Acá usamos UserManager (de ASP.NET Identity), que se encarga de
// guardar usuarios, hashear contraseñas y manejar roles. Nosotros solo armamos el JWT.
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;

    // Inyección de dependencias por constructor
    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email, Nombre = dto.Nombre };

        // Identity valida la contraseña, controla que el email no esté repetido y guarda el hash
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(" ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "Socio");     // todo el que se registra es Socio
        return await ArmarRespuesta(user);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        // Si no existe o la contraseña no coincide con el hash guardado -> null
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return null;

        return await ArmarRespuesta(user);
    }

    private async Task<AuthResponseDto> ArmarRespuesta(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var rol = roles.FirstOrDefault() ?? "Socio";

        return new AuthResponseDto
        {
            Token = GenerarToken(user, rol),
            Id = user.Id,
            Nombre = user.Nombre,
            Email = user.Email!,
            Rol = rol
        };
    }

    // JWT = un texto firmado con una clave secreta. Adentro lleva "claims" (datos del usuario: id, nombre, rol).
    // Si alguien modifica el token, la firma deja de ser válida y el servidor lo rechaza.
    private string GenerarToken(ApplicationUser user, string rol)
    {
        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Nombre),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Role, rol)                   // lo usa [Authorize(Roles = "Admin")]
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),             // el token vence a las 8 horas
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
