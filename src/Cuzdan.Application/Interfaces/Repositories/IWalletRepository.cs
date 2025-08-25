using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Entities;

namespace Cuzdan.Application.Interfaces;

public interface IWalletRepository : IRepository<Wallet> 
{
    Task<List<WalletDto>> GetWalletsAsyc(Guid Id);

    Task<bool> DoesWalletBelongToUserAsync(Guid walletId, Guid userId);
    Task<PagedResult<TransactionDto>> GetTransactionsByWalletIdAsync(
        Guid walletId, string type, int page, int pageSize);
}