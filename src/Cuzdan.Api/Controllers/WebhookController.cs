using Cuzdan.Application.DTOs;
using Cuzdan.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cuzdan.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController(ITransactionService transactionService) : ControllerBase
    {

        [HttpPost("payment-updates")]
        public async Task<IActionResult> HandlePaymentUpdate([FromBody] PaymentUpdateDto updateDto)
        {
            bool isSuccessful = "Success".Equals(updateDto.Status, StringComparison.OrdinalIgnoreCase);

            await transactionService.FinalizePaymentAsync(updateDto.TransactionId, isSuccessful);
            
            return Ok();
        }
    }
}