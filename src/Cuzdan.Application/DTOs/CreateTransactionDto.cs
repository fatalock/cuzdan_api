namespace Cuzdan.Application.DTOs;

public class CreateTransactionDto
{
    public Guid FromId { get; set; }
    public Guid ToId { get; set; }
    public decimal Amount { get; set; }
}