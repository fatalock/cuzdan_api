using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Interfaces;

public interface IAdminService
{
    Task<PagedResult<UserDto>> GetAllUsersProfileAsync(UserFilterDto filter);
    Task<UserDto> GetUserProfileAsync(Guid userId);

    Task<PagedResult<WalletDto>> GetAllWalletsAsync(WalletFilterDto filter);

    Task<List<WalletDto>> GetUserWalletsAsync(Guid userId);


    Task<PagedResult<TransactionDto>> GetAllTransactionsAsync(TransactionFilterDto filter);

    Task<PagedResult<TransactionDto>> GetTransactionsByWalletAsync(Guid walletId, TransactionFilterDto filter);
    Task<TransactionDto> GetTransactionAsync(Guid transactionId);



}