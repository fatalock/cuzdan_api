using System.ComponentModel.DataAnnotations;

namespace Cuzdan.Api.Schemas;

public class TransactionDto
{
    [Required]
    public Guid FromId { get; set; }
    [Required]
    public Guid ToId { get; set; }

    [Required]
    [Range(1,1000000, ErrorMessage = "Amount should be greater than 0.")]
    public decimal Amount { get; set; }
}