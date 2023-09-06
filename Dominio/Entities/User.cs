namespace Dominio.Entities;

public class User : BaseEntity
{
    public string UserName { get; set; }
    public string UserEmail { get; set;}
    public string Password { get; set;}
    public ICollection<Rol> Rols { get; set; } = new HashSet<Rol>();
    public ICollection<UserRol> UsersRols { get; set; }
}
