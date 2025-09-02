namespace Cuzdan.Application.DTOs;



public class TransactionDto
{

    public Guid Id { get; set; }
    public Guid FromId { get; set; }
    public Guid ToId { get; set; }
    public decimal Amount { get; set; }
    public string? OriginalCurrency { get; set; }
    public decimal ConvertedAmount { get; set; }
    public string? TargetCurrency { get; set; }
    public decimal ConversionRate { get; set; }
    public DateTime CreatedAt { get; set; }

    public string? Status { get; set; }
    public string? Type { get; set; }
}