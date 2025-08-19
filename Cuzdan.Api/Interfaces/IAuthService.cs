using Cuzdan.Api.Schemas;

namespace Cuzdan.Api.Interfaces;

public interface IAuthService
{



    Task RegisterAsync(RegisterUserDto registerDto);
    Task<ApiResponse<AuthResult>> LoginAsync(LoginUserDto loginDto);
    Task<ApiResponse<string>> RefresAccesshAsync(string refreshToken);
    

}