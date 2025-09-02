using Cuzdan.Domain.Enums;

namespace Cuzdan.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid FromId { get; set; }

    public Wallet? From { get; set; }

    public Guid ToId { get; set; }

    public Wallet? To { get; set; }
    public decimal OriginalAmount { get; set; }
    public CurrencyType OriginalCurrency { get; set; }
    public decimal ConvertedAmount { get; set; }
    public CurrencyType TargetCurrency { get; set; }
    public decimal ConversionRate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public TransactionStatus Status { get; set; }

    public TransactionType Type { get; set; }


}
