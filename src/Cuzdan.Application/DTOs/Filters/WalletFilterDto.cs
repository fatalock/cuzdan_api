using Cuzdan.Domain.Entities;
using Cuzdan.Domain.Enums;

namespace Cuzdan.Application.DTOs;



public class WalletFilterDto : BaseFilter<Wallet>
{

    public WalletSortField? OrderBy { get; set; }

    public string? WalletName { get; set; }

    public decimal? MinBalance { get; set; }

    public decimal? MaxBalance { get; set; }

    public CurrencyType? Currency { get; set; }
    
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
