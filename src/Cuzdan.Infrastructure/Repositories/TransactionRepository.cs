using Cuzdan.Application.DTOs;
using Cuzdan.Application.Interfaces;
using Cuzdan.Domain.Entities;
using Cuzdan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cuzdan.Infrastructure.Repositories;

public class TransactionRepository(CuzdanContext context) : Repository<Transaction>(context), ITransactionRepository
{
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
                Id = t.Id,
                Amount = t.OriginalAmount,
                CreatedAt = t.CreatedAt,
                FromId = t.FromId,
                ToId = t.ToId,
                Status = t.Status
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
