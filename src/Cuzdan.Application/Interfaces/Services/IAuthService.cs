using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Interfaces;

public interface IAuthService
{



    Task<UserDto> RegisterAsync(RegisterUserDto registerDto);
    Task<AuthResult> LoginAsync(LoginUserDto loginDto);
    Task<AuthResult> RefresAccesshAsync(string refreshToken);
    

}