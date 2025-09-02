using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Entities;

namespace Cuzdan.Application.Interfaces;

public interface IWalletRepository : IRepository<Wallet> 
{
    Task<List<Wallet>> GetWalletsAsyc(Guid Id);

    Task<bool> DoesWalletBelongToUserAsync(Guid walletId, Guid userId);
    Task<IEnumerable<UserBalanceByCurrencyDto>> GetTotalBalancePerCurrencyAsync(Guid userId);
}