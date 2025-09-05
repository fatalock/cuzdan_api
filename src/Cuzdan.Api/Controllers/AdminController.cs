using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Enums;


namespace Cuzdan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class AdminController(IAdminService adminService) : BaseController
{
    [HttpPost("users")]
    public async Task<IActionResult> GetAllUsersProfileAsync([FromBody] UserFilterDto filter)
    {
        var result = await adminService.GetAllUsersProfileAsync(filter);

        return HandleResult(result);
    }
    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserProfileAsync(Guid userId)
    {
        var result = await adminService.GetUserProfileAsync(userId);

        return HandleResult(result);
    }

    [HttpPost("wallets")]
    public async Task<IActionResult> GetAllWalletsAsync([FromBody] WalletFilterDto filter)
    {
        

        var result = await adminService.GetAllWalletsAsync(filter);

        return HandleResult(result);
    }
    [HttpGet("wallets/{userId}")]
    public async Task<IActionResult> GetUserWalletsAsync(Guid userId)
    {
        

        var result = await adminService.GetUserWalletsAsync(userId);

        return HandleResult(result);
    }
    [HttpPost("transactions")]
    public async Task<IActionResult> GetAllTransactionsAsync(
        [FromBody] TransactionFilterDto filter)
    {
        

        var result = await adminService.GetAllTransactionsAsync(filter);

        return HandleResult(result);
    }
    [HttpPost("transactions/wallet/{walletId}")]
    public async Task<IActionResult> GetTransactionsByWalletAsync(
        Guid walletId,
        [FromBody] TransactionFilterDto filter)
    {
        

        var result = await adminService.GetTransactionsByWalletAsync(walletId, filter);

        return HandleResult(result);
    }
    [HttpGet("transactions/{transactionId}")]
    public async Task<IActionResult> GetTransactionAsync(Guid transactionId)
    {
        

        var result = await adminService.GetTransactionAsync(transactionId);

        return HandleResult(result);
    }
}