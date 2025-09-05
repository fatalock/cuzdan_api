using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Interfaces;

public interface IAuthService
{



    Task<Result<UserDto>> RegisterAsync(RegisterUserDto registerDto);
    Task<Result<AuthResult>> LoginAsync(LoginUserDto loginDto);
    Task<Result<AuthResult>> RefresAccesshAsync(string refreshToken);
    

}