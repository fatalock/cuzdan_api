using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Interfaces;

public interface IWalletService
{
    Task<ApiResponse> CreateWalletAsync(CreateWalletDto createWalletDto, Guid Id);
    Task<List<WalletDto>> GetWalletsAsyc(Guid Id);



}
