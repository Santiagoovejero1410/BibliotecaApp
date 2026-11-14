
namespace BibliotecaApi.Models;

public class Usuario
{
    public string Id { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public List<UsuarioRol> UsuariosRoles { get; set; } = new();

     public List<Prestamo> Prestamos { get; set; } = new();
}
