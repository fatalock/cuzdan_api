using Microsoft.AspNetCore.Mvc;
using Cuzdan.Api.Schemas;
using Cuzdan.Api.Interfaces;

namespace Cuzdan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
    {

        await _authService.RegisterAsync(registerDto);

        return Ok(new ApiResponse
        {
            IsSuccessful = true,
            SuccessMessage = "Wallets fetched succesfully"
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
    {
        var response = await _authService.LoginAsync(loginDto);

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var response = await _authService.RefresAccesshAsync(refreshToken);

        return Ok(response);
    }
}