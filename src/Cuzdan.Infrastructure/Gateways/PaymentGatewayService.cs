using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Cuzdan.Application.Interfaces;

namespace Cuzdan.Infrastructure.Gateways;

public class PaymentGatewayService(
    IConfiguration configuration) : IPaymentGatewayService
{
    private readonly IConfiguration _configuration = configuration;

    public async Task InitiatePayment(Guid transactionId)
    {
        // appsettings.json'dan Mock API'nin adresini oku
        var mockApiUrl = _configuration["PaymentGatewaySettings:MockApiUrl"];
        
        // Bu metodun asenkron doğasını korumak için tamamlanmış bir Task döndürüyoruz.
        await Task.CompletedTask;
    }
}
