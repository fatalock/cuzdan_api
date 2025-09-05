using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Interfaces;

public interface IAdminService
{
    Task<Result<PagedResult<UserDto>>> GetAllUsersProfileAsync(UserFilterDto filter);
    Task<Result<UserDto>> GetUserProfileAsync(Guid userId);

    Task<Result<PagedResult<WalletDto>>> GetAllWalletsAsync(WalletFilterDto filter);

    Task<Result<List<WalletDto>>> GetUserWalletsAsync(Guid userId);


    Task<Result<PagedResult<TransactionDto>>> GetAllTransactionsAsync(TransactionFilterDto filter);

    Task<Result<PagedResult<TransactionDto>>> GetTransactionsByWalletAsync(Guid walletId, TransactionFilterDto filter);
    Task<Result<TransactionDto>> GetTransactionAsync(Guid transactionId);



}