using Cuzdan.Application.DTOs;
using Cuzdan.Application.Interfaces;
using Cuzdan.Domain.Entities;
using Cuzdan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cuzdan.Infrastructure.Repositories;

public class WalletRepository(CuzdanContext context) : Repository<Wallet>(context), IWalletRepository
{

    public async Task<List<WalletDto>> GetWalletsAsyc(Guid Id)
    {
        var wallets = await Context.Wallets
            .Where(w => w.UserId == Id)
            .Select(w => new WalletDto
            {
                Id = w.Id,
                WalletName = w.WalletName,
                Balance = w.Balance,
                AvailableBalance = w.AvailableBalance
            })
            .ToListAsync();

        return wallets;
    }

    public async Task<bool> DoesWalletBelongToUserAsync(Guid walletId, Guid userId)
    {
        var walletExists = await Context.Wallets
            .AnyAsync(w => w.Id == walletId && w.UserId == userId);
        return walletExists;
    }

}