using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Interfaces;

public interface ITransactionService
{
    Task TransferTransactionAsync(CreateTransactionDto transactionDto, Guid userId);
    Task<PagedResult<TransactionDto>> GetTransactionsByWalletAsync(
        Guid userId,
        Guid walletId,
        TransactionFilterDto filter
    );

    Task RequestDepositAsync(Guid userId, Guid walletId, DepositWithdrawalRequestDto depositRequestDto);
    Task RequestWithdrawalAsync(Guid userId, Guid walletId, DepositWithdrawalRequestDto withdrawalRequestDto);

    Task FinalizePaymentAsync(Guid transactionId, bool isSuccessful);
}