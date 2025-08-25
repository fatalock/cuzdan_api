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

    private readonly ITransactionService _TransactionService = TransactionService;

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransactionDto transactionDto)
    {
        Guid userId = User.GetUserId();
        var response = await _TransactionService.TransferTransactionAsync(transactionDto, userId);

        return Ok(response);
    }
    [HttpPost("{walletId}")]
    public async Task<IActionResult> GetTransactionsByWalletAsync(Guid walletId, [FromQuery] TransactionFilterDto filter)
    {
        Guid userId = User.GetUserId();

        var result = await _TransactionService.GetTransactionsByWalletAsync(userId, walletId, filter);

        return Ok(new ApiResponse<PagedResult<TransactionDto>>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "Transactions fetched succesfully"
        });
    }
}