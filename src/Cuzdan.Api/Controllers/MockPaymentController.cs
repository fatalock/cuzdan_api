using Cuzdan.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace Cuzdan.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MockPaymentController(IHttpClientFactory httpClientFactory, IConfiguration configuration) : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IConfiguration _configuration = configuration;

        [HttpPost("approve/{transactionId}")]
        public async Task<IActionResult> Approve(Guid transactionId)
        {
            return await SendNotification(transactionId, "Success");
        }

        [HttpPost("reject/{transactionId}")]
        public async Task<IActionResult> Reject(Guid transactionId)
        {
            return await SendNotification(transactionId, "Failed");
        }

        private async Task<IActionResult> SendNotification(Guid transactionId, string status)
        {
            var webhookUrl = _configuration["PaymentSettings:WebhookUrl"];
            if (string.IsNullOrEmpty(webhookUrl))
            {
                return StatusCode(500, "Hata: Webhook URL'si appsettings.json'da tanımlanmamış.");
            }

            var payload = new PaymentUpdateDto{ TransactionId = transactionId, Status = status };

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                
                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(webhookUrl, content);

                response.EnsureSuccessStatusCode(); 

                return Ok($"'{status}' bildirimi {transactionId} için başarıyla gönderildi.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Webhook'a bildirim gönderilemedi. Hata: {ex.Message}");
            }
        }
    }
}