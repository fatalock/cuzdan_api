using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cuzdan.Api.Schemas;
using Cuzdan.Api.Interfaces;

namespace Cuzdan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletsController(IWalletService walletService) : ControllerBase
{

    private readonly IWalletService _walletService = walletService;


    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateWalletDto createDto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString))
        {
            return Unauthorized("No id found.");
        }

        Guid userId = Guid.Parse(userIdString);
        var response = await _walletService.CreateWalletAsync(createDto,userId);

        if (!response.IsSuccessful)
        {
            return BadRequest(response.ErrorMessage);
        }

        return Ok(response.SuccessMessage);




    }
}