using Cuzdan.Api.Schemas;

namespace Cuzdan.Api.Interfaces;

public interface IWalletService
{
    Task<ApiResponse> CreateWalletAsync(CreateWalletDto CreateDto,Guid Id);


}
