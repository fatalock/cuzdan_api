using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Entities;

namespace Cuzdan.Application.Interfaces;

public interface IWalletRepository : IRepository<Wallet> 
{
    Task<List<Wallet>> GetWalletsAsync(Guid Id);

    Task<bool> DoesWalletBelongToUserAsync(Guid walletId, Guid userId);
    Task<IEnumerable<UserBalanceByCurrencyDto>> GetTotalBalancePerCurrencyAsync(Guid userId);
}