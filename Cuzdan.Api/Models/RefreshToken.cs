namespace Cuzdan.Api.Models;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public string Token { get; set; } = null!; // Refresh token string (JWT veya random string)

    public DateTime ExpiresAt { get; set; }   // Bitiş zamanı
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; } // İptal edilme zamanı, null ise aktif

    public string? ReplacedByToken { get; set; } // Rotation ile yenilenen token

    public bool IsActive = true;
    public bool IsRevoked = false;
}
