using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cuzdan.Api.Schemas;
using Cuzdan.Api.Interfaces;

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
        try
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("No id found.");
            }

            Guid userId = Guid.Parse(userIdString);

            var result = await _userService.GetProfileAsync(userId);

            return Ok(new ApiResponse<UserProfileDto>
            {
                IsSuccessful = true,
                Data = result,
                SuccessMessage = "Profile fetched successfully."
            });
        }
        catch (Exception ex)
        {
            return NotFound(new ApiResponse<UserProfileDto>
            {
                IsSuccessful = false,
                ErrorMessage = ex.Message
            });
        }

    }









}