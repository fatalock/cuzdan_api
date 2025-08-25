using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cuzdan.Api.Extensions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;

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

        var result = await _userService.GetUserProfileAsync(userId);

        return Ok(new ApiResponse<UserProfileDto>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "Profile fetched successfully."
        });
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserDto updateUserDto)
    {
        Guid userId = User.GetUserId();

        await _userService.UpdateUserProfileAsync(userId, updateUserDto);

        return Ok(new ApiResponse
        {
            IsSuccessful = true,
            SuccessMessage = "Profile updated successfully."
        });
    }

}