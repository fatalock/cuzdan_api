using Cuzdan.Domain.Enums;

namespace Cuzdan.Application.DTOs;

public class CreateWalletDto
{
    public required string WalletName { get; set; }

    public required CurrencyType Currency { get; set; }

}