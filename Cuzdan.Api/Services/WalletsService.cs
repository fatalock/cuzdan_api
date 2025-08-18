using Cuzdan.Api.Models;
using Microsoft.EntityFrameworkCore;
using Cuzdan.Api.Schemas;
using Cuzdan.Api.Data;
using Azure;
using Cuzdan.Api.Interfaces;

namespace Cuzdan.Api.Services;


public class WalletService(CuzdanContext context) : IWalletService
{
    private readonly CuzdanContext _context = context;

    public async Task<ApiResponse> CreateWalletAsync(CreateWalletDto createDto, Guid Id)
    {

        var newWallet = new Wallet
        {
            WalletName = createDto.WalletName,
            UserId = Id,
            Balance = 0,
            CreatedAt = DateTime.UtcNow,
        };

        _context.Wallets.Add(newWallet);
        await _context.SaveChangesAsync();

        var response = new ApiResponse
        {
            IsSuccessful = true,
            SuccessMessage = "Wallet Created",
        };

        return response;
    }

}