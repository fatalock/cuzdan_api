using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Interfaces;

public interface ITransactionService
{
    Task<ApiResponse> TransferTransactionAsync(TransactionDto transactionDto, Guid UserId);
    Task<PagedResult<TransactionDto>> GetTransactionsByWalletAsync(
        Guid userId,
        Guid walletId,
        TransactionFilterDto filter

    );

    Task RequestDepositAsync(Guid userId, Guid walletId, decimal amount);
    Task RequestWithdrawalAsync(Guid userId, Guid walletId, decimal amount);

    Task FinalizePaymentAsync(Guid transactionId, bool isSuccessful);
}