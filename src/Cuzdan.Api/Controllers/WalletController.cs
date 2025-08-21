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
    private readonly IWalletService _walletService = walletService;

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateWalletDto createWalletDto)
    {
        Guid userId = User.GetUserId();
        var response = await _walletService.CreateWalletAsync(createWalletDto, userId);

        return Ok(response.SuccessMessage);
    }

    [HttpGet()]
    public async Task<IActionResult> GetWalletsAsyc()
    {
        Guid userId = User.GetUserId();

        var result = await _walletService.GetWalletsAsyc(userId);

        return Ok(new ApiResponse<List<WalletDto>>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "Wallets fetched succesfully"
        });
    }
    [HttpGet("{walletId}")]
    public async Task<IActionResult> GetWalletTransactionsAsync(Guid walletId, [FromQuery] string type = "all", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        Guid userId = User.GetUserId();

        var result = await _walletService.GetWalletTransactionsAsync(userId, walletId, type, page, pageSize);

        return Ok(new ApiResponse<PagedResult<TransactionDto>>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "Transactions fetched succesfully"
        });
    }
}