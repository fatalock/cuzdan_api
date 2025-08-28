using Cuzdan.Domain.Enums;

namespace Cuzdan.Domain.Entities;

public class Wallet
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string WalletName { get; set; }

    public Guid UserId { get; set; }

    public User? User { get; set; }

    public decimal Balance { get; set; }
    public decimal AvailableBalance { get; set; }

    public CurrencyType Currency { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  
    public ICollection<Transaction> SentTransactions { get; set; } = new List<Transaction>();


    public ICollection<Transaction> ReceivedTransactions { get; set; } = new List<Transaction>();


}
