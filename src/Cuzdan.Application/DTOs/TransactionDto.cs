using System.ComponentModel.DataAnnotations;
using Cuzdan.Domain.Enums;

namespace Cuzdan.Application.DTOs;



public class TransactionDto
{

    public Guid Id { get; set; }
    [Required]
    public Guid FromId { get; set; }
    [Required]
    public Guid ToId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; }

    public TransactionStatus Status { get; set; }
}