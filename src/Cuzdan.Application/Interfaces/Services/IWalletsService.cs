using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Interfaces;

public interface IWalletService
{
    Task<Result<WalletDto>> CreateWalletAsync(CreateWalletDto createWalletDto, Guid userId);
    Task<Result<List<WalletDto>>> GetWalletsAsync(Guid userId);

    Task<Result<List<UserBalanceByCurrencyResponseDto>>> GetTotalBalancePerCurrencyAsync(Guid userId);

}
