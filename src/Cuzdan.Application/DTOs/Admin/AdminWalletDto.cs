
using Cuzdan.Domain.Enums;

namespace Cuzdan.Application.DTOs;
public class AdminWalletDto
{
    public Guid Id { get; set; }
    public required string WalletName { get; set; }
    public decimal Balance { get; set; }
    public decimal AvailableBalance { get; set; }
    public CurrencyType Currency { get; set; }
    public DateTime CreatedAt { get; set; }
}