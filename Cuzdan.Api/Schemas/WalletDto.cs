using System.ComponentModel.DataAnnotations;

namespace Cuzdan.Api.Schemas;

public class WalletDto
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public required string WalletName { get; set; }

    [Required]
    public decimal Balance { get; set; }

}