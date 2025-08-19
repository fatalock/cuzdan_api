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
public class WalletsController(IWalletService walletService) : ControllerBase
{
    private readonly IWalletService _walletService = walletService;

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateWalletDto createWalletDto)
    {
        Guid userId = User.GetUserId();
        var response = await _walletService.CreateWalletAsync(createWalletDto, userId);

        return Ok(response.SuccessMessage);
    }

    [HttpGet()]
    public async Task<IActionResult> GetWallets()
    {
        Guid userId = User.GetUserId();

        var result = await _walletService.GetWallets(userId);

        return Ok(new ApiResponse<List<WalletDto>>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "Wallets fetched succesfully"
        });
    }
}