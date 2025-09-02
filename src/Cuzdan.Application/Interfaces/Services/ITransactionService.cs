using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Interfaces;

public interface ITransactionService
{
    Task<ApiResponse> TransferTransactionAsync(CreateTransactionDto transactionDto, Guid UserId);
    Task<PagedResult<TransactionDto>> GetTransactionsByWalletAsync(
        Guid userId,
        Guid walletId,
        TransactionFilterDto filter

    );

    Task RequestDepositAsync(Guid userId, Guid walletId, DepositWithdrawalRequestDto depositRequestDto);
    Task RequestWithdrawalAsync(Guid userId, Guid walletId, DepositWithdrawalRequestDto withdrawalRequestDto);

    Task FinalizePaymentAsync(Guid transactionId, bool isSuccessful);
}