using Microsoft.AspNetCore.Mvc;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;

namespace Cuzdan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
    {

        var result = await authService.RegisterAsync(registerDto);

        return Ok(new ApiResponse<UserDto>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "User registered succesfully"
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
    {
        var result = await authService.LoginAsync(loginDto);

        return Ok(new ApiResponse<AuthResult>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "User registered succesfully"
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var result = await authService.RefresAccesshAsync(refreshToken);

        return Ok(new ApiResponse<AuthResult>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "User registered succesfully"
        });
    }
}