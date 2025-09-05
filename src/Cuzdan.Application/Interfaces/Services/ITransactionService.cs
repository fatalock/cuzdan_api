using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Interfaces;

public interface ITransactionService
{
    Task<Result> TransferTransactionAsync(CreateTransactionDto transactionDto, Guid userId);
    Task<Result<PagedResult<TransactionDto>>> GetTransactionsByWalletAsync(
        Guid userId,
        Guid walletId,
        TransactionFilterDto filter
    );

    Task<Result> RequestDepositAsync(Guid userId, Guid walletId, DepositWithdrawalRequestDto depositRequestDto);
    Task<Result>  RequestWithdrawalAsync(Guid userId, Guid walletId, DepositWithdrawalRequestDto withdrawalRequestDto);

    Task<Result> FinalizePaymentAsync(Guid transactionId, bool isSuccessful);
}