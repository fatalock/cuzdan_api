using Cuzdan.Application.DTOs;
using Cuzdan.Application.Interfaces;
using Cuzdan.Domain.Entities;
using Cuzdan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cuzdan.Infrastructure.Repositories;

public class WalletRepository(CuzdanContext context) : IWalletRepository
{
    private readonly CuzdanContext _context = context;

    public async Task<Wallet?> GetWalletByIdAsyc(Guid Id)
    {
        return await _context.Wallets.FirstOrDefaultAsync(u => u.Id == Id);
    }
    public async Task AddWalletAsync(Wallet wallet)
    {
        await _context.Wallets.AddAsync(wallet);
    }
    public async Task<List<WalletDto>> GetWalletsAsyc(Guid Id)
    {
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

    public async Task<bool> DoesWalletBelongToUserAsync(Guid walletId, Guid userId)
    {
        var walletExists = await _context.Wallets
            .AnyAsync(w => w.Id == walletId && w.UserId == userId);
        return walletExists;
    }
    public async Task<PagedResult<TransactionDto>> GetTransactionsByWalletIdAsync(
        Guid walletId, string type, int page, int pageSize)
    {
        // 3. Ana sorguyu oluşturma (Eski koddaki adım 3)
        IQueryable<Transaction> transactionsQuery;
        switch (type.ToLowerInvariant())
        {
            case "sent":
                transactionsQuery = _context.Transactions.Where(t => t.FromId == walletId);
                break;
            case "received":
                transactionsQuery = _context.Transactions.Where(t => t.ToId == walletId);
                break;
            default:
                transactionsQuery = _context.Transactions.Where(t => t.FromId == walletId || t.ToId == walletId);
                break;
        }

        // 4. Toplam kayıt sayısını hesapla
        var totalCount = await transactionsQuery.CountAsync();

        // 5. Sırala, sayfala ve DTO'ya dönüştür
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

        // 6. Sonucu döndür
        return new PagedResult<TransactionDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = transactions
        };
    }
}