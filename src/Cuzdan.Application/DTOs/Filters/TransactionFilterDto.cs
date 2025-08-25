using System.ComponentModel.DataAnnotations;
using Cuzdan.Domain.Entities;

namespace Cuzdan.Application.DTOs;

public class TransactionFilterDto : BaseFilter<Transaction>
{
    public Guid? WalletId { get; set; }


    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string? Type { get; set; }
}
