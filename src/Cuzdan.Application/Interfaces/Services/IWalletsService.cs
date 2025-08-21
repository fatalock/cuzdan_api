using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Interfaces;

public interface IWalletService
{
    Task<ApiResponse> CreateWalletAsync(CreateWalletDto createWalletDto, Guid Id);
    Task<List<WalletDto>> GetWalletsAsyc(Guid Id);

    Task<PagedResult<TransactionDto>> GetWalletTransactionsAsync(
        Guid userId,
        Guid walletId,
        string type,
        int page,
        int pageSize
    );

}
