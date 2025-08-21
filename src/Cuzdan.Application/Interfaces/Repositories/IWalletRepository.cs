using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Entities;

namespace Cuzdan.Application.Interfaces;

public interface IWalletRepository
{
    Task AddWalletAsync(Wallet wallet);
    Task<List<WalletDto>> GetWalletsAsyc(Guid Id);
    Task<Wallet?> GetWalletByIdAsyc(Guid Id);


    Task<bool> DoesWalletBelongToUserAsync(Guid walletId, Guid userId);
    Task<PagedResult<TransactionDto>> GetTransactionsByWalletIdAsync(
        Guid walletId, string type, int page, int pageSize);
}