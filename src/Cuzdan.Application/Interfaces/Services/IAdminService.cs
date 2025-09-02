using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Interfaces;

public interface IAdminService
{
    Task<PagedResult<AdminUserDto>> GetAllUsersProfileAsync(UserFilterDto filter);
    Task<AdminUserDto> GetUserProfileAsync(Guid userId);

    Task<PagedResult<AdminWalletDto>> GetAllWalletsAsyc(WalletFilterDto filter);

    Task<List<AdminWalletDto>> GetUserWalletsAsyc(Guid userId);


    Task<PagedResult<AdminTransactionDto>> GetAllTransactionsAsync(
        
        TransactionFilterDto filter
    );

    Task<PagedResult<AdminTransactionDto>> GetTransactionsByWalletAsync(
        
        Guid walletId,
        TransactionFilterDto filter

    );
    Task<AdminTransactionDto> GetTransactionAsync(
        
        Guid transactionId


    );



}