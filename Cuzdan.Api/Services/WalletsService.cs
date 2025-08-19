using Cuzdan.Api.Models;
using Microsoft.EntityFrameworkCore;
using Cuzdan.Api.Schemas;
using Cuzdan.Api.Data;
using Azure;
using Cuzdan.Api.Interfaces;
using Cuzdan.Api.Exceptions;

namespace Cuzdan.Api.Services;


public class WalletService(CuzdanContext context) : IWalletService
{
    private readonly CuzdanContext _context = context;

    public async Task<ApiResponse> CreateWalletAsync(CreateWalletDto createWalletDto, Guid Id)
    {

        var newWallet = new Wallet
        {
            WalletName = createWalletDto.WalletName,
            UserId = Id,
            Balance = 0,
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

    public async Task<List<WalletDto>> GetWallets(Guid Id)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == Id);
        if (!userExists)
            throw new NotFoundException("User not found");

        var wallets = await _context.Wallets
            .Where(w => w.UserId == Id)
            .Select(w => new WalletDto
            {
                Id = w.Id,
                WalletName = w.WalletName,
                Balance = w.Balance
            })
            .ToListAsync();

        return wallets;
    }

}