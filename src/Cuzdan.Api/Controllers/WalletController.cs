using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cuzdan.Api.Extensions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;

namespace Cuzdan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletsController(IWalletService walletService) : BaseController
{

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateWalletDto createWalletDto)
    {
        Guid userId = User.GetUserId();
        var result = await walletService.CreateWalletAsync(createWalletDto, userId);

        return HandleResult(result);
    }

    [HttpGet()]
    public async Task<IActionResult> GetWalletsAsync()
    {
        Guid userId = User.GetUserId();

        var result = await walletService.GetWalletsAsync(userId);

        return HandleResult(result);
    }
    [HttpGet("total-balance-per-currency")]
    public async Task<IActionResult> GetTotalBalancePerCurrencyAsync()
    {
        Guid userId = User.GetUserId();

        var result = await walletService.GetTotalBalancePerCurrencyAsync(userId);
        
        return HandleResult(result);
    }
    

}