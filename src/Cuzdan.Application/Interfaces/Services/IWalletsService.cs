using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Interfaces;

public interface IWalletService
{
    Task<WalletDto> CreateWalletAsync(CreateWalletDto createWalletDto, Guid userId);
    Task<List<WalletDto>> GetWalletsAsync(Guid userId);

    Task<List<UserBalanceByCurrencyResponseDto>> GetTotalBalancePerCurrencyAsync(Guid userId);

}
