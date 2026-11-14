
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BibliotecaApi.Data;
using BibliotecaApi.DTOs;
using BibliotecaApi.Models;

namespace BibliotecaApi.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var existe = await _db.Usuarios
            .AnyAsync(u => u.Email.ToLower() == email);

        if (existe)
            throw new InvalidOperationException("El email ya está registrado.");

        var rolSocio = await _db.RolesManuales
            .FirstOrDefaultAsync(r => r.Nombre == "Socio");

        if (rolSocio == null)
            throw new InvalidOperationException("No existe el rol Socio.");

        var usuario = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            Nombre = dto.Nombre.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        usuario.UsuariosRoles.Add(new UsuarioRol
        {
            UsuarioId = usuario.Id,
            RolId = rolSocio.Id
        });

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        return ArmarRespuesta(usuario, "Socio");
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var usuario = await _db.Usuarios
            .Include(u => u.UsuariosRoles)
                .ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email);

        if (usuario == null)
            return null;

        bool passwordCorrecta = BCrypt.Net.BCrypt.Verify(
            dto.Password,
            usuario.PasswordHash
        );

        if (!passwordCorrecta)
            return null;

        var rol = usuario.UsuariosRoles
            .Select(ur => ur.Rol.Nombre)
            .FirstOrDefault() ?? "Socio";

        return ArmarRespuesta(usuario, rol);
    }

    private AuthResponseDto ArmarRespuesta(Usuario usuario, string rol)
    {
        return new AuthResponseDto
        {
            Token = GenerarToken(usuario, rol),
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Rol = rol
        };
    }

    private string GenerarToken(Usuario usuario, string rol)
    {
        var clave = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        var credenciales = new SigningCredentials(
            clave,
            SecurityAlgorithms.HmacSha256
        );

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, rol)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credenciales
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
