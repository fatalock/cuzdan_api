using System.ComponentModel.DataAnnotations;

namespace Cuzdan.Application.DTOs;

public class TransactionDto
{
    [Required]
    public Guid FromId { get; set; }
    [Required]
    public Guid ToId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; }
}