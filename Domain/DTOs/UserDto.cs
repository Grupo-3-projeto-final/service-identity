using System.ComponentModel.DataAnnotations;

namespace Identity.Domain.DTOs;
public record UserDto
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Role { get; set; }
}
