using System.ComponentModel.DataAnnotations;

namespace Cuzdan.Api.Schemas;

public class CreateWalletDto
{
    [Required]
    public required string WalletName { get; set; }



}