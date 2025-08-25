namespace Cuzdan.Application.DTOs;

public class UpdateUserDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? Password { get; set; }
}