using System.ComponentModel.DataAnnotations;

namespace Cuzdan.Api.Schemas;

public class TransactionDto
{
    [Required]
    public Guid FromId { get; set; }
    [Required]
    public Guid ToId { get; set; }

    [Required]
    public decimal Amount { get; set; }
}