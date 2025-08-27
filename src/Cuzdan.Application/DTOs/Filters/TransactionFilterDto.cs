using System.ComponentModel.DataAnnotations;
using Cuzdan.Domain.Entities;
using Cuzdan.Domain.Enums;

namespace Cuzdan.Application.DTOs;



public class TransactionFilterDto : BaseFilter<Transaction>
{
    public TransactionSortField? OrderBy { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string? Type { get; set; }
}
