namespace Cuzdan.Application.Interfaces;

public interface IPaymentGatewayService
{
    Task InitiatePayment(Guid transactionId);

}