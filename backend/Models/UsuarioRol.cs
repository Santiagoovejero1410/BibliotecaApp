
namespace BibliotecaApi.Models;

public class UsuarioRol
{
    public string UsuarioId { get; set; } = string.Empty;

    public string RolId { get; set; } = string.Empty;

    public Usuario Usuario { get; set; } = null!;

    public Rol Rol { get; set; } = null!;
}
