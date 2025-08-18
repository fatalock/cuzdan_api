using Cuzdan.Api.Schemas;

namespace Cuzdan.Api.Interfaces;

public interface IAuthService
{



    Task<ApiResponse> RegisterUserAsync(RegisterUserDto registerDto);
    Task<ApiResponse<AuthResult>> LoginUserAsync(LoginUserDto loginDto);
    Task<ApiResponse<string>> RefresAccesshAsync(string refreshToken);
    

}