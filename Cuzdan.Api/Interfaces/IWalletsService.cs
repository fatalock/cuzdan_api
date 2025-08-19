using Cuzdan.Api.Schemas;

namespace Cuzdan.Api.Interfaces;

public interface IWalletService
{
    Task<ApiResponse> CreateWalletAsync(CreateWalletDto createWalletDto, Guid Id);
    Task<List<WalletDto>> GetWallets(Guid Id);

}
