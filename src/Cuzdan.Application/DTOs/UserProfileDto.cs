namespace Cuzdan.Application.DTOs;

public class UserProfileDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}