using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cuzdan.Api.Extensions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;

namespace Cuzdan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletsController(IWalletService walletService) : ControllerBase
{

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateWalletDto createWalletDto)
    {
        Guid userId = User.GetUserId();
        var response = await walletService.CreateWalletAsync(createWalletDto, userId);

        return Ok(response.SuccessMessage);
    }

    [HttpGet()]
    public async Task<IActionResult> GetWalletsAsyc()
    {
        Guid userId = User.GetUserId();

        var result = await walletService.GetWalletsAsyc(userId);

        return Ok(new ApiResponse<List<WalletDto>>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "Wallets fetched succesfully"
        });
    }
    [HttpGet("total-balance-per-currency")]
    public async Task<IActionResult> GetTotalBalancePerCurrencyAsyc()
    {
        Guid userId = User.GetUserId();

        var result = await walletService.GetTotalBalancePerCurrencyAsync(userId);

        return Ok(new ApiResponse<List<UserBalanceByCurrencyResponseDto>>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "Wallets fetched succesfully"
        });
    }
    

}