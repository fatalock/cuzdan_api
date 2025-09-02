using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cuzdan.Api.Extensions;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;

namespace Cuzdan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController(ITransactionService TransactionService) : ControllerBase
{

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] CreateTransactionDto transactionDto)
    {
        Guid userId = User.GetUserId();
        var response = await TransactionService.TransferTransactionAsync(transactionDto, userId);

        return Ok(response);
    }
    [HttpPost("wallet_history/{walletId}")]
    public async Task<IActionResult> GetTransactionsByWalletAsync(Guid walletId, [FromBody] TransactionFilterDto filter)
    {
        Guid userId = User.GetUserId();

        var result = await TransactionService.GetTransactionsByWalletAsync(userId, walletId, filter);

        return Ok(new ApiResponse<PagedResult<TransactionDto>>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "Transactions fetched succesfully"
        });
    }
    [HttpPost("deposit/{walletId}")]
    public async Task<IActionResult> RequestDepositAsync(Guid walletId, [FromBody] DepositWithdrawalRequestDto depositRequestDto)
    {
        Guid userId = User.GetUserId();

        await TransactionService.RequestDepositAsync(userId, walletId, depositRequestDto);

        return Ok(new ApiResponse
        {
            IsSuccessful = true,
            SuccessMessage = "Deposit requested succesfully"
        });
    }
    [HttpPost("withdrawal/{walletId}")]
    public async Task<IActionResult> RequestWithdrawalAsync(Guid walletId, [FromBody] DepositWithdrawalRequestDto withdrawalRequestDto)
    {
        Guid userId = User.GetUserId();

        await TransactionService.RequestWithdrawalAsync(userId, walletId, withdrawalRequestDto);

        return Ok(new ApiResponse
        {
            IsSuccessful = true,
            SuccessMessage = "Withdrawal requested succesfully"
        });
    }
}