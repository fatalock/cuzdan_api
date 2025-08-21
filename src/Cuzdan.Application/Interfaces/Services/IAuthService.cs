using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Interfaces;

public interface IAuthService
{



    Task RegisterAsync(RegisterUserDto registerDto);
    Task<ApiResponse<AuthResult>> LoginAsync(LoginUserDto loginDto);
    Task<ApiResponse<string>> RefresAccesshAsync(string refreshToken);
    

}