using System.ComponentModel.DataAnnotations;

namespace Cuzdan.Application.DTOs;

public class CreateWalletDto
{
    [Required]
    public required string WalletName { get; set; }



}