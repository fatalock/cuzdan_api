namespace Cuzdan.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid FromId { get; set; }

    public Wallet? From { get; set; }

    public Guid ToId { get; set; }

    public Wallet? To { get; set; }
    public decimal Amount { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


}
