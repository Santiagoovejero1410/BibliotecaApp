
namespace BibliotecaApi.Models;

public class Rol
{
    public string Id { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public List<UsuarioRol> UsuariosRoles { get; set; } = new();
}
