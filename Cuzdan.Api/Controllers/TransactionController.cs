using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cuzdan.Api.Services;
using Cuzdan.Api.Schemas;
using Cuzdan.Api.Interfaces;

namespace Cuzdan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController(ITransactionService TransactionService) : ControllerBase
{

    private readonly ITransactionService _TransactionService = TransactionService;

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransactionDto TransactionDto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString))
        {
            return Unauthorized("No id found.");
        }

        Guid userId = Guid.Parse(userIdString);
        var response = await _TransactionService.TransferTransactionAsync(TransactionDto,userId);

        if (!response.IsSuccessful)
        {
            return BadRequest(response.ErrorMessage);
        }

        return Ok(response.SuccessMessage);




    }
}