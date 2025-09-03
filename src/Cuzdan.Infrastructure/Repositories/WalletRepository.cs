using Cuzdan.Application.DTOs;
using Cuzdan.Application.Interfaces;
using Cuzdan.Domain.Entities;
using Cuzdan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cuzdan.Infrastructure.Repositories;

public class WalletRepository(CuzdanContext context) : Repository<Wallet>(context), IWalletRepository
{

    public async Task<List<Wallet>> GetWalletsAsync(Guid Id)
    {
        var wallets = await Context.Wallets
            .Where(w => w.UserId == Id)
            .ToListAsync();

        return wallets;
    }

    public async Task<bool> DoesWalletBelongToUserAsync(Guid walletId, Guid userId)
    {
        var walletExists = await Context.Wallets
            .AnyAsync(w => w.Id == walletId && w.UserId == userId);
        return walletExists;
    }
    public async Task<IEnumerable<UserBalanceByCurrencyDto>> GetTotalBalancePerCurrencyAsync(Guid userId)
    {
        return await Context.Set<UserBalanceByCurrencyDto>()
            .FromSqlInterpolated($"SELECT * FROM get_user_balance_by_currency({userId})")
            .ToListAsync();
    }
}