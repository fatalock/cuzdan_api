using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cuzdan.Api.Services;
using Cuzdan.Api.Schemas;
using Cuzdan.Api.Interfaces;
using Cuzdan.Api.Extensions;

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
        var response = await _TransactionService.TransferTransactionAsync(transactionDto,userId);

        return Ok(response);
    }
}