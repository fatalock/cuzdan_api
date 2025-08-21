using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Interfaces;

public interface ITransactionService
{
    Task<ApiResponse> TransferTransactionAsync(TransactionDto transactionDto, Guid UserId);


}