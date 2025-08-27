namespace Cuzdan.Application.DTOs
{
    public class PaymentUpdateDto
    {
        public Guid TransactionId { get; set; }
        public required string Status { get; set; }
    }
}