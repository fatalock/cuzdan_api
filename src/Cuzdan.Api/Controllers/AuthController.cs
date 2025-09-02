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

        await authService.RegisterAsync(registerDto);

        return Ok(new ApiResponse
        {
            IsSuccessful = true,
            SuccessMessage = "User registered succesfully"
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
    {
        var response = await authService.LoginAsync(loginDto);

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var response = await authService.RefresAccesshAsync(refreshToken);

        return Ok(response);
    }
}