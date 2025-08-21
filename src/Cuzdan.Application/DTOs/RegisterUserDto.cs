using System.ComponentModel.DataAnnotations;

namespace Cuzdan.Application.DTOs;

public class RegisterUserDto
{
    [Required]
    public required string Name { get; set; }

    [Required] 
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    public required string Password { get; set; }
}