using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Cuzdan.Application.Interfaces;

namespace Cuzdan.Infrastructure.Gateways;

public class PaymentGatewayService : IPaymentGatewayService
{

    public async Task InitiatePayment(Guid transactionId)
    {

        await Task.CompletedTask;
    }
}
