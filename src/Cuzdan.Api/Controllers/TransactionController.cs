using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cuzdan.Api.Extensions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;

namespace Cuzdan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController(ITransactionService TransactionService) : BaseController
{

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] CreateTransactionDto transactionDto)
    {
        Guid userId = User.GetUserId();
        var result = await TransactionService.TransferTransactionAsync(transactionDto, userId);

        return HandleResult(result);
    }
    [HttpPost("wallet_history/{walletId}")]
    public async Task<IActionResult> GetTransactionsByWalletAsync(Guid walletId, [FromBody] TransactionFilterDto filter)
    {
        Guid userId = User.GetUserId();

        var result = await TransactionService.GetTransactionsByWalletAsync(userId, walletId, filter);

        return HandleResult(result);
    }
    [HttpPost("deposit/{walletId}")]
    public async Task<IActionResult> RequestDepositAsync(Guid walletId, [FromBody] DepositWithdrawalRequestDto depositRequestDto)
    {
        Guid userId = User.GetUserId();

        var result = await TransactionService.RequestDepositAsync(userId, walletId, depositRequestDto);

        return HandleResult(result);
    }
    [HttpPost("withdrawal/{walletId}")]
    public async Task<IActionResult> RequestWithdrawalAsync(Guid walletId, [FromBody] DepositWithdrawalRequestDto withdrawalRequestDto)
    {
        Guid userId = User.GetUserId();

        var result = await TransactionService.RequestWithdrawalAsync(userId, walletId, withdrawalRequestDto);

        return HandleResult(result);
    }
}