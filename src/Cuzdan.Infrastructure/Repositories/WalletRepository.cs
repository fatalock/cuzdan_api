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
                Balance = w.Balance
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
    public async Task<PagedResult<TransactionDto>> GetTransactionsByWalletIdAsync(
        Guid walletId, string type, int page, int pageSize)
    {
        IQueryable<Transaction> transactionsQuery;
        switch (type.ToLowerInvariant())
        {
            case "sent":
                transactionsQuery = Context.Transactions.Where(t => t.FromId == walletId);
                break;
            case "received":
                transactionsQuery = Context.Transactions.Where(t => t.ToId == walletId);
                break;
            default:
                transactionsQuery = Context.Transactions.Where(t => t.FromId == walletId || t.ToId == walletId);
                break;
        }

        var totalCount = await transactionsQuery.CountAsync();

        var transactions = await transactionsQuery
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TransactionDto
            {
                Amount = t.Amount,
                CreatedAt = t.CreatedAt,
                FromId = t.FromId,
                ToId = t.ToId,
            })
            .ToListAsync();

        return new PagedResult<TransactionDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = transactions
        };
    }
}