using Cuzdan.Domain.Enums;

namespace Cuzdan.Application.DTOs;



public class TransactionDto
{

    public Guid Id { get; set; }
    public Guid FromId { get; set; }
    public Guid ToId { get; set; }
    public decimal Amount { get; set; }
    public  CurrencyType OriginalCurrency { get; set; }
    public decimal ConvertedAmount { get; set; }
    public CurrencyType TargetCurrency { get; set; }
    public decimal ConversionRate { get; set; }
    public DateTime CreatedAt { get; set; }
    public TransactionStatus Status { get; set; }
    public TransactionType Type { get; set; }
}