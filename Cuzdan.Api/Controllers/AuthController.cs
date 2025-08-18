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
        var response = await _authService.RegisterUserAsync(registerDto);

        if (!response.IsSuccessful)
        {
            return BadRequest(response.ErrorMessage);
        }

        return Ok(response.SuccessMessage);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
    {
        var response = await _authService.LoginUserAsync(loginDto);

        if (!response.IsSuccessful)
        {
            return Unauthorized(response.ErrorMessage);
        }

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var response = await _authService.RefresAccesshAsync(refreshToken);

        if (!response.IsSuccessful)
        {
            return Unauthorized(response.ErrorMessage);
        }

        return Ok(response);
    }
}