using MyBlog.Entity.Entities.Identity;

namespace MyBlog.Entity.DTOs.Users;

public class UserAddDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public Guid ImageId { get; set; } = Guid.Parse("F71F4B9A-AA60-461D-B398-DE31001BF214");
    public Guid RoleId { get; set; }
    public List<AppRole> Roles { get; set; }
}
