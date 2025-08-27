using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Entities;

namespace Cuzdan.Application.Interfaces;

public interface IPaymentGatewayService
{
    Task InitiatePayment(Guid transactionId);

}