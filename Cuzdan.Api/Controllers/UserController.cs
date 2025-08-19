using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cuzdan.Api.Schemas;
using Cuzdan.Api.Interfaces;
using Cuzdan.Api.Extensions;

namespace Cuzdan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class UserController(IUserService userService) : ControllerBase
{

    private readonly IUserService _userService = userService;

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        Guid userId = User.GetUserId();

        var result = await _userService.GetProfileAsync(userId);

        return Ok(new ApiResponse<UserProfileDto>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "Profile fetched successfully."
        });
    }

}