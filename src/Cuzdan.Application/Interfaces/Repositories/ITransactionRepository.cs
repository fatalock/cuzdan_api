using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Entities;

namespace Cuzdan.Application.Interfaces;

public interface ITransactionRepository : IRepository<Transaction> 
{
    Task<PagedResult<TransactionDto>> GetTransactionsByWalletIdAsync(
        Guid walletId, string type, int page, int pageSize);
}