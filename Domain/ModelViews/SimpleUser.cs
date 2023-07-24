using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Identity.Domain.Entities;

namespace Identity.Domain.ModelViews;
public class SimpleUser
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public string Email { get; set; } = default!;
    public string RoleName { get; set; }

    public static SimpleUser Build(User user)
    {
        return new SimpleUser
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            RoleName = user.Role.ToString()
        };
    }
}
