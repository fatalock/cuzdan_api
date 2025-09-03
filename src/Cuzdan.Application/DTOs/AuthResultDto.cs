namespace Cuzdan.Application.DTOs;

public class AuthResult
{
    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }
    
    public Guid UserId { get; set; }
}